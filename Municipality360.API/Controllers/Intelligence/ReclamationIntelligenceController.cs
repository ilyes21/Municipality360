// ═══════════════════════════════════════════════════════════════════
//  8_API_AIController.cs
//  API/Controllers/Intelligence/ReclamationIntelligenceController.cs
//
//  نقاط النهاية الإدارية لميزة الذكاء الاصطناعي:
//  - إعادة تدريب النموذج
//  - معاينة تصنيف شكوى بدون حفظ
//  - إحصائيات دقة النموذج
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Municipality360.Application.DTOs.Intelligence;
using Municipality360.Application.Interfaces.Services;

namespace Municipality360.API.Controllers.Intelligence;

[ApiController]
[Route("api/intelligence/reclamations")]
[Authorize(Roles = "SuperAdmin,Admin")]
public sealed class ReclamationIntelligenceController : ControllerBase
{
    private readonly IComplaintIntelligenceService _service;
    private readonly ILogger<ReclamationIntelligenceController> _logger;

    public ReclamationIntelligenceController(
        IComplaintIntelligenceService service,
        ILogger<ReclamationIntelligenceController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ── POST api/intelligence/reclamations/classify-preview ──────────

    /// <summary>
    /// معاينة تصنيف نص شكوى بدون حفظ — للاختبار من لوحة التحكم.
    /// </summary>
    [HttpPost("classify-preview")]
    public async Task<IActionResult> ClassifyPreview([FromBody] ClassifyPreviewRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Text))
            return BadRequest(new { message = "نص الشكوى مطلوب." });

        var result = await _service.ClassifyAsync(new ClassificationRequestDto
        {
            ReclamationId = 0,
            Objet = req.Text,
            Description = string.Empty,
        });

        return Ok(result);
    }

    // ── POST api/intelligence/reclamations/auto-response-preview ────

    /// <summary>
    /// معاينة رد آلي لشكوى افتراضية — للاختبار من لوحة التحكم.
    /// </summary>
    [HttpPost("auto-response-preview")]
    public async Task<IActionResult> AutoResponsePreview([FromBody] AutoResponseRequestDto req)
    {
        var result = await _service.GenerateAutoResponseAsync(req);
        return result.IsSuccess
            ? Ok(result)
            : StatusCode(502, new { message = "فشل توليد الرد من خدمة الذكاء الاصطناعي.", detail = result.ErrorMessage });
    }

    // ── POST api/intelligence/reclamations/retrain ───────────────────

    /// <summary>
    /// إعادة تدريب نموذج ML.NET من بيانات التدريب المحدَّثة.
    /// العملية تجري في الخلفية — الاستجابة فورية (202 Accepted).
    /// </summary>
    [HttpPost("retrain")]
    [Authorize(Roles = "SuperAdmin")]
    public IActionResult Retrain()
    {
        _logger.LogInformation("Manual ML retraining triggered by {User}", User.Identity?.Name);

        // نُشغّل في الخلفية لا نحجب الاستجابة
        _ = Task.Run(() => _service.RetrainModelAsync());

        return Accepted(new { message = "جارٍ إعادة تدريب النموذج في الخلفية. تحقق من السجلات عند الانتهاء." });
    }
}

// ── Request Records ───────────────────────────────────────────────

public sealed record ClassifyPreviewRequest(string Text);