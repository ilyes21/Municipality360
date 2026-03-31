// ═══════════════════════════════════════════════════════════════════
//  3_Infrastructure_MLClassifier.cs
//  Infrastructure/Services/AI/MLComplaintClassifier.cs
//
//  التبعيات (NuGet):
//    Microsoft.ML                        ≥ 3.0
//    Microsoft.Extensions.ML             ≥ 3.0   ← PredictionEnginePool
//    Microsoft.Extensions.Logging.Abstractions
// ═══════════════════════════════════════════════════════════════════

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using Microsoft.ML.Data;
using Municipality360.Application.DTOs.Intelligence;
using Municipality360.Domain.Entities;

namespace Municipality360.Infrastructure.Services.AI;

/// <summary>
/// مُصنِّف الشكاوى باستخدام ML.NET مع PredictionEnginePool.
///
/// الخوارزمية: FastForestOva (متعدد الفئات) — يعمل بدون GPU.
/// مسار النموذج: wwwroot/models/complaint_classifier.zip
///
/// خريطة الفئات:
///   Environnement   → مخلفات، نظافة، تلوث
///   Voirie          → طرقات، حفر، إشارات
///   Eclairage       → إنارة عمومية
///   Eau             → مياه، صرف صحي
///   Urbanisme       → بناء مخالف، ترخيص
///   Autre           → لا تنطبق فئة محددة
/// </summary>
public sealed class MLComplaintClassifier
{
    private readonly PredictionEnginePool<ReclamationMLInput, ReclamationMLOutput> _pool;
    private readonly ILogger<MLComplaintClassifier> _logger;

    // خريطة: كود فئة ML → (تسمية عربية، درجة خطورة)
    private static readonly Dictionary<string, (string Label, float BaseSeverity)> CategoryMap = new()
    {
        ["Environnement"] = ("بيئة ونظافة", 5.0f),
        ["Voirie"] = ("طرقات وأرصفة", 6.0f),
        ["Eclairage"] = ("إنارة عمومية", 4.0f),
        ["Eau"] = ("مياه وصرف صحي", 7.0f),
        ["Urbanisme"] = ("تعمير وبناء", 5.5f),
        ["Autre"] = ("أخرى", 3.0f),
    };

    // خريطة الأولوية بحسب الخطورة
    private static string MapPriorite(float severity) => severity switch
    {
        >= 8f => "Critique",
        >= 6f => "Haute",
        >= 4f => "Moyenne",
        _ => "Basse"
    };

    // كلمات مفتاحية بسيطة لكل فئة (تُستخدم كـ fallback)
    private static readonly Dictionary<string, string[]> Keywords = new()
    {
        ["Environnement"] = ["نفايات", "مخلفات", "قمامة", "تلوث", "نظافة"],
        ["Voirie"] = ["طريق", "رصيف", "حفرة", "إشارة", "مرور", "أرضية"],
        ["Eclairage"] = ["إنارة", "مصباح", "ضوء", "كهرباء", "ظلام"],
        ["Eau"] = ["مياه", "ماء", "صرف", "أنبوب", "تسرب", "فيضان"],
        ["Urbanisme"] = ["بناء", "ترخيص", "تعمير", "مخالفة", "عقار"],
    };

    public MLComplaintClassifier(
        PredictionEnginePool<ReclamationMLInput, ReclamationMLOutput> pool,
        ILogger<MLComplaintClassifier> logger)
    {
        _pool = pool;
        _logger = logger;
    }

    public ClassificationResultDto Classify(ClassificationRequestDto request)
    {
        try
        {
            var input = new ReclamationMLInput { Text = request.FullText };
            var output = _pool.Predict(modelName: "ComplaintClassifier", example: input);

            var category = output.PredictedLabel;
            var confidence = output.MaxScore;

            // Fallback إذا كان التصنيف غير موثوق
            if (confidence < 0.40f)
            {
                category = FallbackKeywordClassify(request.FullText);
                confidence = 0.50f; // ثقة تقريبية للـ fallback
            }

            var (label, baseSeverity) = CategoryMap.GetValueOrDefault(category, ("أخرى", 3.0f));

            // تعديل الخطورة بناءً على الكلمات الحرجة
            var adjustedSeverity = AdjustSeverity(request.FullText, baseSeverity);

            return new ClassificationResultDto
            {
                SuggestedCategoryCode = category,
                SuggestedCategoryLabel = label,
                SuggestedPriorite = MapPriorite(adjustedSeverity),
                ConfidenceScore = confidence,
                SeverityScore = adjustedSeverity,
                ExtractedKeywords = ExtractKeywords(request.FullText),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ML classification failed for ReclamationId={Id}", request.ReclamationId);

            // إرجاع قيم آمنة عند الفشل
            return new ClassificationResultDto
            {
                SuggestedCategoryCode = "Autre",
                SuggestedCategoryLabel = "أخرى",
                SuggestedPriorite = "Moyenne",
                ConfidenceScore = 0f,
                SeverityScore = 3f,
            };
        }
    }

    // ── مساعدات داخلية ────────────────────────────────────────────

    private static string FallbackKeywordClassify(string text)
    {
        var lower = text.ToLowerInvariant();
        foreach (var (cat, words) in Keywords)
            if (words.Any(lower.Contains))
                return cat;
        return "Autre";
    }

    private static float AdjustSeverity(string text, float base_)
    {
        var lower = text.ToLowerInvariant();
        float bonus = 0f;
        if (lower.Contains("عاجل") || lower.Contains("خطر") || lower.Contains("طوارئ")) bonus += 2f;
        if (lower.Contains("طفل") || lower.Contains("مسن") || lower.Contains("مريض")) bonus += 1f;
        if (lower.Contains("أيام") || lower.Contains("أسابيع") || lower.Contains("أشهر")) bonus += 0.5f;
        return Math.Min(base_ + bonus, 10f);
    }

    private static List<string> ExtractKeywords(string text)
    {
        var all = Keywords.Values.SelectMany(w => w);
        return all.Where(kw => text.Contains(kw, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    // ── تدريب (يُستدعى من RetrainModelAsync) ─────────────────────

    /// <summary>
    /// يُعيد بناء النموذج من بيانات CSV.
    /// Format: Text (string) | Category (string key label)
    /// </summary>
    public static void TrainAndSave(string trainingDataPath, string modelOutputPath, ILogger logger)
    {
        var ctx = new MLContext(seed: 42);

        var data = ctx.Data.LoadFromTextFile<TrainingRow>(
            trainingDataPath,
            separatorChar: ',',
            hasHeader: true);

        var pipeline = ctx.Transforms.Text
            .FeaturizeText("Features", nameof(TrainingRow.Text))
            .Append(ctx.Transforms.Conversion.MapValueToKey("Label", nameof(TrainingRow.Category)))
            .Append(ctx.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                labelColumnName: "Label",
                featureColumnName: "Features"))
            .Append(ctx.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        logger.LogInformation("ML.NET: Starting training...");
        var model = pipeline.Fit(data);

        ctx.Model.Save(model, data.Schema, modelOutputPath);
        logger.LogInformation("ML.NET: Model saved to {Path}", modelOutputPath);
    }

    private sealed class TrainingRow
    {
        [LoadColumn(0)] public string Text { get; set; } = string.Empty;
        [LoadColumn(1)] public string Category { get; set; } = string.Empty;
    }
}