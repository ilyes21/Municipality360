// ═══════════════════════════════════════════════════════════════════
//  DesignTimeDbContextFactory.cs
//  Infrastructure/Data/DesignTimeDbContextFactory.cs
//
//  يُمكّن EF Core من إنشاء ApplicationDbContext وقت التصميم
//  (add-migration / update-database) دون الحاجة لتشغيل التطبيق.
//
//  ⚠️ هذا الملف يُستخدم فقط من أدوات EF Core — ليس في Runtime.
//  ⚠️ connection string هنا للـ migrations فقط، القيمة الحقيقية
//     تأتي من appsettings.json في وقت التشغيل.
// ═══════════════════════════════════════════════════════════════════

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Municipality360.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // البحث عن appsettings.json في مشروع API
        var basePath = Path.Combine(Directory.GetCurrentDirectory(),
            "..", "Municipality360.API");

        // إذا لم يُعثر عليه (مثلاً عند التشغيل من مجلد Infrastructure مباشرةً)
        if (!Directory.Exists(basePath))
            basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' introuvable dans appsettings.json.");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString,
            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
