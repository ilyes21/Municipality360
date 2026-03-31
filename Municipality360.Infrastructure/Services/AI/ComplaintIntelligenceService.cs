// ═══════════════════════════════════════════════════════════════════
//  5_Infrastructure_ComplaintIntelligenceService.cs
//  Infrastructure/Services/AI/ComplaintIntelligenceService.cs
//
//  المُنسِّق الرئيسي — يُجمع MLComplaintClassifier + AutoResponseGenerator
//  وينفّذ IComplaintIntelligenceService من طبقة Application.
// ═══════════════════════════════════════════════════════════════════

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;
using Municipality360.Application.DTOs.Intelligence;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Domain.Entities;

namespace Municipality360.Infrastructure.Services.AI;

public sealed class ComplaintIntelligenceService(
    MLComplaintClassifier classifier,
    AutoResponseGenerator generator,
    ILogger<ComplaintIntelligenceService> logger) : IComplaintIntelligenceService
{
    private readonly MLComplaintClassifier _classifier = classifier;
    private readonly AutoResponseGenerator _generator = generator;
    private readonly ILogger<ComplaintIntelligenceService> _logger = logger;

    // ── تصنيف واحد ────────────────────────────────────────────────

    public Task<ClassificationResultDto> ClassifyAsync(ClassificationRequestDto request)
    {
        // ML.NET synchronous — نُغلّف في Task لتوافق الواجهة
        var result = _classifier.Classify(request);
        return Task.FromResult(result);
    }

    // ── توليد رد واحد ─────────────────────────────────────────────

    public Task<AutoResponseResultDto> GenerateAutoResponseAsync(AutoResponseRequestDto request)
        => _generator.GenerateAsync(request);

    // ── معالجة ذرية: تصنيف + رد في نفس الوقت ─────────────────────

    public async Task<(ClassificationResultDto Classification, AutoResponseResultDto Response)>
        ProcessNewComplaintAsync(
            ClassificationRequestDto classRequest,
            AutoResponseRequestDto responseRequest)
    {
        _logger.LogInformation(
            "AI Processing start → Reclamation#{Id}", classRequest.ReclamationId);

        // التصنيف أولاً (متزامن — سريع جداً)
        var classification = _classifier.Classify(classRequest);

        // تحديث معطيات طلب الرد بنتيجة التصنيف
        responseRequest = responseRequest with
        {
            CategoryLabel = classification.SuggestedCategoryLabel,
            Priorite = classification.SuggestedPriorite
        };

        // توليد الرد (غير متزامن — HTTP call)
        var response = await _generator.GenerateAsync(responseRequest);

        _logger.LogInformation(
            "AI Processing done → Reclamation#{Id} | Category={Cat} | Confidence={C:P0} | ResponseOk={Ok}",
            classRequest.ReclamationId,
            classification.SuggestedCategoryCode,
            classification.ConfidenceScore,
            response.IsSuccess);

        return (classification, response);
    }

    // ── إعادة التدريب ─────────────────────────────────────────────

    public Task RetrainModelAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            const string trainingPath = "Data/training_complaints.csv";
            const string modelPath = "wwwroot/models/complaint_classifier.zip";

            _logger.LogInformation("ML.NET Retraining started...");
            MLComplaintClassifier.TrainAndSave(trainingPath, modelPath, _logger);
            _logger.LogInformation("ML.NET Retraining completed.");
        }, cancellationToken);
    }
}