using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Municipality360.Application.DTOs.Intelligence;


public sealed class ClassificationResultDto
{
    /// <summary>كود الفئة المقترحة (يطابق CategorieReclamation.Code)</summary>
    public string SuggestedCategoryCode { get; set; } = string.Empty;

    /// <summary>تسمية الفئة بالعربية (للعرض)</summary>
    public string SuggestedCategoryLabel { get; set; } = string.Empty;

    /// <summary>الأولوية المقترحة</summary>
    public string SuggestedPriorite { get; set; } = "Moyenne";

    /// <summary>درجة الثقة [0.0 → 1.0]</summary>
    public float ConfidenceScore { get; set; }

    /// <summary>درجة الخطورة [0 → 10]</summary>
    public float SeverityScore { get; set; }

    /// <summary>الكلمات المفتاحية المستخرجة</summary>
    public List<string> ExtractedKeywords { get; set; } = [];

    /// <summary>هل الثقة كافية لتطبيق التصنيف تلقائياً؟ (عتبة: 0.70)</summary>
    public bool IsConfident => ConfidenceScore >= 0.70f;
}

/// نتيجة توليد الرد الآلي — تُعاد للـ ReclamationService
public sealed class AutoResponseResultDto
{
    /// <summary>نص الرد الرسمي الكامل بالعربية الفصحى</summary>
    public string ResponseText { get; set; } = string.Empty;

    /// <summary>هل تم التوليد بنجاح؟</summary>
    public bool IsSuccess { get; set; }

    /// <summary>رسالة الخطأ في حال الفشل (للـ logging فقط)</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>عدد الرموز المستخدمة (للمراقبة)</summary>
    public int TokensUsed { get; set; }
}

// طلب التصنيف — يُبنى من Reclamation Entity

public sealed class ClassificationRequestDto
{
    public int ReclamationId { get; set; }
    public string Objet { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Localisation { get; set; }

    /// <summary>نص مدمج للنموذج = Objet + " " + Description</summary>
    public string FullText => $"{Objet} {Description}".Trim();
}


// طلب الرد الآلي — يُبنى بعد التصنيف

public sealed record AutoResponseRequestDto
{
    public int ReclamationId { get; set; }
    public string NumeroReclamation { get; set; } = string.Empty;
    public string CitoyenPrenom { get; set; } = string.Empty;
    public string Objet { get; set; } = string.Empty;
    public string CategoryLabel { get; set; } = string.Empty;
    public string Priorite { get; set; } = string.Empty;
}