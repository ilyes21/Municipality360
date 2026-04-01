// Infrastructure/Services/AI/ComplaintIntelligenceService.cs
// ✅ FIXED:
//  1. RetrainModelAsync يستخدم المسارات الصحيحة من TrainingSamples
//  2. تدريب + حفظ النموذج تلقائياً إذا لم يكن موجوداً

using Microsoft.Extensions.Logging;
using Municipality360.Application.DTOs.Intelligence;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Infrastructure.Data.Training;

namespace Municipality360.Infrastructure.Services.AI;

public sealed class ComplaintIntelligenceService : IComplaintIntelligenceService
{
    private readonly MLComplaintClassifier _classifier;
    private readonly AutoResponseGenerator _generator;
    private readonly ILogger<ComplaintIntelligenceService> _logger;

    public ComplaintIntelligenceService(
        MLComplaintClassifier classifier,
        AutoResponseGenerator generator,
        ILogger<ComplaintIntelligenceService> logger)
    {
        _classifier = classifier;
        _generator = generator;
        _logger = logger;
    }

    // ── تصنيف واحد ────────────────────────────────────────────────

    public Task<ClassificationResultDto> ClassifyAsync(ClassificationRequestDto request)
    {
        var result = _classifier.Classify(request);
        return Task.FromResult(result);
    }

    // ── توليد رد واحد ─────────────────────────────────────────────

    public Task<AutoResponseResultDto> GenerateAutoResponseAsync(AutoResponseRequestDto request)
        => _generator.GenerateAsync(request);

    // ── معالجة ذرية: تصنيف + رد ──────────────────────────────────

    public async Task<(ClassificationResultDto Classification, AutoResponseResultDto Response)>
        ProcessNewComplaintAsync(
            ClassificationRequestDto classRequest,
            AutoResponseRequestDto responseRequest)
    {
        _logger.LogInformation(
            "AI Processing start → Reclamation#{Id}", classRequest.ReclamationId);

        var classification = _classifier.Classify(classRequest);

        // ✅ استخدام with على record صحيح
        var updatedResponseRequest = responseRequest with
        {
            CategoryLabel = classification.SuggestedCategoryLabel,
            Priorite = classification.SuggestedPriorite
        };

        var response = await _generator.GenerateAsync(updatedResponseRequest);

        _logger.LogInformation(
            "AI Processing done → Reclamation#{Id} | Category={Cat} | Confidence={C:P0} | ResponseOk={Ok}",
            classRequest.ReclamationId,
            classification.SuggestedCategoryCode,
            classification.ConfidenceScore,
            response.IsSuccess);

        return (classification, response);
    }

    // ── إعادة التدريب ──────────────────────────────────────────────
    // ✅ FIXED: يستخدم TrainingSamples للمسارات الصحيحة

    public Task RetrainModelAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var trainingPath = TrainingSamples.GetDefaultCsvPath();
            var modelPath = TrainingSamples.GetDefaultModelPath();

            // تأكد من وجود مجلد النموذج
            var modelDir = Path.GetDirectoryName(modelPath);
            if (!string.IsNullOrEmpty(modelDir))
                Directory.CreateDirectory(modelDir);

            if (!File.Exists(trainingPath))
            {
                _logger.LogWarning(
                    "Training file not found at {Path}. Skipping retrain.", trainingPath);
                return;
            }

            _logger.LogInformation("ML.NET Retraining started from {Path}...", trainingPath);
            MLComplaintClassifier.TrainAndSave(trainingPath, modelPath, _logger);
            _logger.LogInformation("ML.NET Model saved to {Path}", modelPath);

        }, cancellationToken);
    }
}