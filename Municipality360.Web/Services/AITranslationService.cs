using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;

namespace Municipality360.Web.Services;

/// <summary>
/// نظام ترجمة تلقائي بـ Claude API.
/// • أول استدعاء لكل لغة → يترجم دفعة واحدة ويخزّن
/// • الاستدعاءات التالية → من الذاكرة فوراً
/// • لا ملفات، لا JSON، لا إعداد يدوي
/// </summary>
public class AITranslationService
{
    private readonly HttpClient           _http;
    private readonly ILocalStorageService _storage;

    // ذاكرة التخزين: lang → { key → translated }
    private readonly Dictionary<string, Dictionary<string, string>> _cache = new();

    // النصوص الأصلية المسجّلة من الصفحات
    private readonly Dictionary<string, string> _sourceTexts = new();

    private const string LangKey    = "app_lang";
    private const string ApiUrl     = "https://api.anthropic.com/v1/messages";

    // !! ضع مفتاح API في appsettings.json تحت "Anthropic:ApiKey" !!
    private readonly string _apiKey;

    public string CurrentLang { get; private set; } = "ar";
    public bool   IsRtl       => CurrentLang == "ar";
    public string Dir         => IsRtl ? "rtl" : "ltr";
    public string FontClass   => IsRtl ? "font-ar" : "font-fr";
    public bool   IsLoading   { get; private set; } = false;

    public event Action? OnChanged;

    public AITranslationService(HttpClient http, ILocalStorageService storage, IConfiguration config)
    {
        _http    = http;
        _storage = storage;
        _apiKey  = config["Anthropic:ApiKey"] ?? "";
    }

    // ══════════════════════════════════════════════════════════
    // التهيئة — يُستدعى مرة واحدة عند بدء التطبيق
    // ══════════════════════════════════════════════════════════
    public async Task InitAsync()
    {
        try
        {
            var saved = await _storage.GetItemAsync<string>(LangKey);
            if (saved is "ar" or "fr")
                CurrentLang = saved;
        }
        catch { }
    }

    // ══════════════════════════════════════════════════════════
    // تسجيل النصوص الأصلية (يُستدعى من كل صفحة)
    // ══════════════════════════════════════════════════════════
    /// <summary>
    /// سجّل النصوص التي تريد ترجمتها.
    /// مثال: Register("Login", new { Title="تسجيل الدخول", Btn="دخول" })
    /// </summary>
    public void Register(string ns, object texts)
    {
        foreach (var prop in texts.GetType().GetProperties())
        {
            var key = $"{ns}.{prop.Name}";
            var val = prop.GetValue(texts)?.ToString() ?? "";
            _sourceTexts.TryAdd(key, val);
        }
    }

    // ══════════════════════════════════════════════════════════
    // تغيير اللغة
    // ══════════════════════════════════════════════════════════
    public async Task SetLangAsync(string lang)
    {
        if (lang == CurrentLang || lang is not ("ar" or "fr")) return;

        CurrentLang = lang;

        try { await _storage.SetItemAsync(LangKey, lang); } catch { }

        // إذا لم تكن مترجمة بعد → اترجم الآن
        if (!_cache.ContainsKey(lang))
            await TranslateAllAsync(lang);

        OnChanged?.Invoke();
    }

    // ══════════════════════════════════════════════════════════
    // الترجمة: T("Login.Title") → النص المترجم
    // ══════════════════════════════════════════════════════════
    public string T(string key)
    {
        // العربية هي اللغة الأصلية → نرجع النص الأصلي مباشرة
        if (CurrentLang == "ar")
            return _sourceTexts.TryGetValue(key, out var src) ? src : key;

        // لغة أخرى → من الكاش
        if (_cache.TryGetValue(CurrentLang, out var dict) &&
            dict.TryGetValue(key, out var translated))
            return translated;

        // لم يُترجم بعد → النص الأصلي كـ fallback
        return _sourceTexts.TryGetValue(key, out var fallback) ? fallback : key;
    }

    // ══════════════════════════════════════════════════════════
    // الترجمة الفعلية عبر Claude API
    // ══════════════════════════════════════════════════════════
    private async Task TranslateAllAsync(string targetLang)
    {
        if (!_sourceTexts.Any() || string.IsNullOrEmpty(_apiKey)) return;

        IsLoading = true;
        OnChanged?.Invoke();

        try
        {
            var langName = targetLang == "fr" ? "French" : "Arabic";
            var payload  = JsonSerializer.Serialize(_sourceTexts, new JsonSerializerOptions
            {
                WriteIndented    = true,
                Encoder          = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var prompt = $"""
                You are a professional translator for government/municipal software.
                Translate the JSON values from Arabic to {langName}.

                Rules:
                - Keep the exact same JSON keys
                - Translate only the values
                - Use formal {langName} appropriate for government applications
                - Do NOT translate brand names like "Municipality360", "ELAIN", "EL AIN"
                - Return ONLY valid JSON, no explanations, no markdown

                JSON to translate:
                {payload}
                """;

            var request = new
            {
                model      = "claude-sonnet-4-6",
                max_tokens = 4096,
                messages   = new[] { new { role = "user", content = prompt } }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
            req.Headers.Add("x-api-key",         _apiKey);
            req.Headers.Add("anthropic-version", "2023-06-01");
            req.Content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8, "application/json");

            var res  = await _http.SendAsync(req);
            var json = await res.Content.ReadAsStringAsync();
            var doc  = JsonDocument.Parse(json);

            var text = doc.RootElement
                .GetProperty("content")[0]
                .GetProperty("text").GetString() ?? "{}";

            // تنظيف Markdown إذا وُجد
            text = System.Text.RegularExpressions.Regex
                .Replace(text.Trim(), @"^```json\s*|^```\s*|\s*```$", "");

            var translated = JsonSerializer.Deserialize<Dictionary<string, string>>(text)
                          ?? new Dictionary<string, string>();

            _cache[targetLang] = translated;
        }
        catch (Exception ex)
        {
            // عند الفشل → الكاش يبقى فارغاً والـ T() يرجع النص الأصلي
            Console.WriteLine($"[AITranslation] فشل: {ex.Message}");
            _cache[targetLang] = new Dictionary<string, string>();
        }
        finally
        {
            IsLoading = false;
            OnChanged?.Invoke();
        }
    }
}
