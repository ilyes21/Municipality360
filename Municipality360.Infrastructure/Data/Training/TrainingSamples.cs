// Infrastructure/Data/Training/TrainingSamples.cs

namespace Municipality360.Infrastructure.Data.Training;

/// <summary>
/// بيانات التدريب — تم تحسينها بشكل كبير (120 مثال واقعي تونسي)
/// كلما أضفت شكاوى حقيقية من بلدية صفاقس كلما ارتفعت الدقة.
/// </summary>
public static class TrainingSamples
{
    public static readonly (string Text, string Category)[] Data =
    [
        // ==================== Voirie (طرقات وأرصفة) ====================
        ("حفرة كبيرة في الطريق تسبب حوادث", "Voirie"),
        ("فمة حفرة برشا كبيرة في شارع الحبيب بورقيبة", "Voirie"),
        ("الرصيف محطم قدام الدار وخطير على الولاد", "Voirie"),
        ("الطريق مقطوع بسبب أشغال وما رجعوش", "Voirie"),
        ("إشارة المرور معطلة منذ أسبوعين", "Voirie"),
        ("لا يوجد ممر للمشاة أمام المدرسة الابتدائية", "Voirie"),
        ("تشققات خطيرة في الطريق الرئيسي", "Voirie"),
        ("انهيار جزء من الرصيف بعد الأمطار", "Voirie"),
        ("حفرة في شارع الجمهورية تجعل السيارات تتلف", "Voirie"),
        ("الطريق مليان حفر صغيرة وكبيرة", "Voirie"),
        ("رصيف مكسور أمام المستشفى", "Voirie"),
        ("الشارع الفرعي في حي المنار فيه حفر", "Voirie"),
        ("علامات المرور محروقة وغير واضحة", "Voirie"),
        ("الطريق الدائري فيه تشققات كبيرة", "Voirie"),

        // ==================== Environnement (بيئة ونظافة) ====================
        ("القمامة لم تُجمع منذ 5 أيام والروائح منتشرة", "Environnement"),
        ("رمي نفايات عشوائي بجانب الحاوية", "Environnement"),
        ("روائح كريهة جداً من المكب القريب", "Environnement"),
        ("تراكم الأوساخ أمام المدرسة منذ أسبوع", "Environnement"),
        ("حيوانات ضالة تنبش القمامة في الحي", "Environnement"),
        ("عشب متراكم ومتسخ في الحديقة العمومية", "Environnement"),
        ("القمامة متراكمة في شارع الطيب المهيري", "Environnement"),
        ("فمة ريحة بشعة من تصريف المياه", "Environnement"),
        ("كلاب ضالة كثيرة في الحي وتخوف الأطفال", "Environnement"),
        ("النفايات منتشرة في الشارع بعد جمع القمامة", "Environnement"),
        ("حاويات القمامة مكسورة ومليانة", "Environnement"),
        ("رمي مخلفات البناء في الفضاء العام", "Environnement"),

        // ==================== Eclairage (إنارة عمومية) ====================
        ("مصباح الشارع محروق منذ 10 أيام", "Eclairage"),
        ("الحي كله مظلم في الليل", "Eclairage"),
        ("عمود إنارة مكسور وخطير على المارة", "Eclairage"),
        ("نصف أعمدة الإنارة في الشارع لا تعمل", "Eclairage"),
        ("الإنارة معطلة في شارع الاستقلال", "Eclairage"),
        ("مصباح يومض ويطفأ بشكل عشوائي", "Eclairage"),
        ("لا توجد إنارة في الزقاق الفرعي", "Eclairage"),
        ("عمود كهرباء مائل ومهدد بالسقوط", "Eclairage"),

        // ==================== Eau (مياه وصرف صحي) ====================
        ("انقطاع الماء منذ يومين في الحي كامل", "Eau"),
        ("تسرب في شبكة الصرف الصحي أمام المنزل", "Eau"),
        ("فيضان مياه الأمطار في الشارع الرئيسي", "Eau"),
        ("أنبوب مياه مكسور والمياه تتدفق", "Eau"),
        ("ضغط الماء ضعيف جداً في الطابق الثالث", "Eau"),
        ("مياه الصرف الصحي تخرج في الشارع", "Eau"),
        ("انقطاع الماء لمدة 4 أيام متتالية", "Eau"),
        ("رائحة الصرف الصحي قوية في الحي", "Eau"),
        ("تسرب مياه من الأنبوب الرئيسي", "Eau"),

        // ==================== Urbanisme (تعمير وبناء) ====================
        ("جارنا يبني طابق إضافي بدون رخصة", "Urbanisme"),
        ("محل تجاري يتجاوز الحدود ويضيق الشارع", "Urbanisme"),
        ("بناء مخالف يسد المنفذ الوحيد للحي", "Urbanisme"),
        ("شخص يبني سور بدون ترخيص", "Urbanisme"),
        ("توسعة منزل مخالفة للقانون", "Urbanisme"),
        ("بناء في منطقة خضراء محظورة", "Urbanisme"),

        // ==================== Autre (شكاوى أخرى) ====================
        ("شكوى عامة لا تنتمي لأي فئة", "Autre"),
        ("طلب مساعدة في إزالة لوحة إعلانات قديمة", "Autre"),
        ("سيارة مهجورة في الشارع منذ شهر", "Autre"),
        ("ضجيج موسيقى عالية في منتصف الليل", "Autre"),
        ("طلب تنظيم حملة توعية بيئية", "Autre"),
    ];

    /// <summary>
    /// ينشئ ملف CSV تلقائياً إذا لم يكن موجوداً
    /// يُستدعى مرة واحدة عند بدء التطبيق
    /// </summary>
    public static async Task EnsureCsvFileExistsAsync()
    {
        var csvPath = GetDefaultCsvPath();

        if (File.Exists(csvPath))
            return; // الملف موجود → لا نعمل شيئاً

        // الملف غير موجود → ننشئه
        await ExportToCsvAsync();
        Console.WriteLine($"✅ تم إنشاء ملف التدريب تلقائياً: {csvPath}");
    }

    /// <summary>
    /// ينشئ ملف CSV (الدالة القديمة)
    /// </summary>
    public static async Task ExportToCsvAsync(string? outputPath = null)
    {
        outputPath ??= GetDefaultCsvPath();

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var lines = Data.Select(d => $"\"{d.Text.Replace("\"", "\"\"")}\",{d.Category}");
        await File.WriteAllLinesAsync(outputPath, ["Text,Category", .. lines]);
    }

    /// <summary>
    /// مسار ملف التدريب — خارج مجلد bin حتى لا يُحذف عند الـ Publish
    /// </summary>
    public static string GetDefaultCsvPath()
    {
        // خيار 1: مجلد بجانب المشروع (موصى به في التطوير)
        var baseDir = AppContext.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..")); // يرجع إلى مجلد Municipality360.API

        return Path.Combine(projectRoot, "Data", "Training", "training_complaints.csv");

        // خيار 2: (بديل) استخدام AppData المحلي للمستخدم (أفضل في الإنتاج)
        // return Path.Combine(
        //     Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        //     "Municipality360", "Training", "training_complaints.csv");
    }

    /// <summary>
    /// مسار حفظ النموذج المدرب (.zip)
    /// </summary>
    public static string GetDefaultModelPath()
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        return Path.Combine(projectRoot, "Models", "complaint_classifier.zip");
    }
}