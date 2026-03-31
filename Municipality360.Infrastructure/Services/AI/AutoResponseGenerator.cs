// ═══════════════════════════════════════════════════════════════════
//  4_Infrastructure_AutoResponseGenerator.cs
//  Infrastructure/Services/AI/AutoResponseGenerator.cs
//
//  التبعيات (NuGet):
//    Azure.AI.OpenAI   ≥ 2.1  (يدعم OpenAI + Azure OpenAI بنفس الكود)
//      أو
//    Betalgo.Ranul.OpenAI  ≥ 8.7  (مجاني — بديل مجتمعي)
//
//  appsettings.json:
//  "AI": {
//    "OpenAI": {
//      "ApiKey": "sk-...",
//      "Model": "gpt-4o-mini",
//      "MaxTokens": 500,
//      "TimeoutSeconds": 30
//    }
//  }
// ═══════════════════════════════════════════════════════════════════

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Municipality360.Application.DTOs.Intelligence;

namespace Municipality360.Infrastructure.Services.AI;

// ── Options ───────────────────────────────────────────────────────

public sealed class OpenAIOptions
{
    public const string Section = "AI:OpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o-mini";
    public int MaxTokens { get; set; } = 500;
    public int TimeoutSeconds { get; set; } = 30;
    /// <summary>Base URL — يمكن تغييره لـ Azure OpenAI أو Gemini proxy</summary>
    public string BaseUrl { get; set; } = "https://api.openai.com/v1/chat/completions";
}

// ── Generator ─────────────────────────────────────────────────────

/// <summary>
/// يُولّد ردوداً رسمية بالعربية الفصحى بأسلوب بلدية العين.
///
/// System Prompt مُحكم يضمن:
/// - التحية الرسمية
/// - تأكيد الاستلام مع رقم الشكوى والفئة
/// - التوجيه للقسم المختص
/// - الخاتمة الرسمية + الحث على المتابعة
/// - لا وعود زمنية — لا أسماء موظفين
/// </summary>
public sealed class AutoResponseGenerator
{
    private readonly HttpClient _http;
    private readonly OpenAIOptions _opts;
    private readonly ILogger<AutoResponseGenerator> _logger;

    // System Prompt الرسمي — ثابت، غير قابل للتغيير بواسطة المستخدم
    private const string SystemPrompt = """
        أنت مساعد ذكي رسمي لبلدية "العين".
        مهمتك الوحيدة: كتابة رد رسمي مختصر للمواطن بعد استلام شكواه.

        قواعد صارمة يجب الالتزام بها:
        1. اللغة: عربية فصحى مهنية مطمئنة — لا دارجة ولا تعابير عامية.
        2. الهيكل الإلزامي (بالترتيب):
           أ) تحية رسمية مختصرة موجّهة للمواطن باسمه.
           ب) تأكيد استلام الشكوى مع ذكر رقمها وفئتها.
           ج) إخبار المواطن بأنها أُحيلت للقسم المختص للدراسة والمعالجة.
           د) دعوة المواطن لمتابعة حالة طلبه عبر التطبيق برقم الشكوى.
           هـ) خاتمة رسمية باسم بلدية العين.
        3. المحظورات الصارمة:
           - لا تذكر أي وعد بتاريخ أو مدة زمنية محددة.
           - لا تذكر اسم أي موظف أو مسؤول.
           - لا تضمّن أي معلومات غير واردة في المعطيات.
           - لا تستخدم رموز تعبيرية (emoji) أو نقاط ترقيم مفرطة.
        4. الطول: بين 80 و 130 كلمة — مختصر ومكتمل.
        5. الشكل النهائي: فقرات نثرية فقط — لا قوائم ولا ترقيم.
        """;

    public AutoResponseGenerator(
        IHttpClientFactory httpFactory,
        IOptions<OpenAIOptions> opts,
        ILogger<AutoResponseGenerator> logger)
    {
        _http = httpFactory.CreateClient("OpenAI");
        _opts = opts.Value;
        _logger = logger;
    }

    public async Task<AutoResponseResultDto> GenerateAsync(
        AutoResponseRequestDto request,
        CancellationToken ct = default)
    {
        var userPrompt = BuildUserPrompt(request);

        var payload = new
        {
            model = _opts.Model,
            max_tokens = _opts.MaxTokens,
            temperature = 0.4,  // منخفض = ردود متسقة ورسمية
            messages = new[]
            {
                new { role = "system", content = SystemPrompt },
                new { role = "user",   content = userPrompt   }
            }
        };

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(_opts.TimeoutSeconds));

            var response = await _http.PostAsJsonAsync(_opts.BaseUrl, payload, cts.Token);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>(cts.Token);

            var text = result?.Choices?[0]?.Message?.Content?.Trim() ?? string.Empty;
            var tokens = result?.Usage?.TotalTokens ?? 0;

            _logger.LogInformation(
                "AutoResponse generated for Reclamation#{Id} — {Tokens} tokens used",
                request.ReclamationId, tokens);

            return new AutoResponseResultDto
            {
                ResponseText = text,
                IsSuccess = !string.IsNullOrWhiteSpace(text),
                TokensUsed = tokens
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "AutoResponse generation failed for Reclamation#{Id}", request.ReclamationId);

            return new AutoResponseResultDto
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
                // رد احتياطي مُعدّ مسبقاً عند فشل الـ API
                ResponseText = BuildFallbackResponse(request)
            };
        }
    }

    // ── User Prompt — يحتوي معطيات الشكوى فقط ────────────────────

    private static string BuildUserPrompt(AutoResponseRequestDto req) => $"""
        معطيات الشكوى:
        - اسم المواطن: {req.CitoyenPrenom}
        - رقم الشكوى: {req.NumeroReclamation}
        - موضوع الشكوى: {req.Objet}
        - فئة الشكوى: {req.CategoryLabel}
        - الأولوية المحددة: {req.Priorite}

        اكتب الرد الرسمي الآن:
        """;

    // ── رد احتياطي عند فشل API (offline fallback) ────────────────

    private static string BuildFallbackResponse(AutoResponseRequestDto req) => $"""
        السيد/ة {req.CitoyenPrenom}، حفظكم الله،

        تحية طيبة وبعد،

        تُفيدكم بلدية العين بأنها تلقّت شكواكم المتعلقة بـ"{req.Objet}"،
        المُسجَّلة تحت الرقم المرجعي ({req.NumeroReclamation}) ضمن فئة "{req.CategoryLabel}".

        وقد جرى توجيه هذه الشكوى إلى القسم المختص لدراستها واتخاذ الإجراءات اللازمة.
        يمكنكم متابعة حالة طلبكم في أي وقت عبر التطبيق باستخدام الرقم المرجعي أعلاه.

        وتقبّلوا خالص التحية والتقدير،
        بلدية العين
        """;

    // ── نماذج JSON الداخلية ────────────────────────────────────────

    private sealed record OpenAIResponse(
        [property: JsonPropertyName("choices")] List<Choice>? Choices,
        [property: JsonPropertyName("usage")] UsageInfo? Usage);

    private sealed record Choice(
        [property: JsonPropertyName("message")] MessageContent? Message);

    private sealed record MessageContent(
        [property: JsonPropertyName("content")] string? Content);

    private sealed record UsageInfo(
        [property: JsonPropertyName("total_tokens")] int TotalTokens);
}