using ST10538419_CLDV7112_Practicium.Application;
using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Data;
using ST10538419_CLDV7112_Practicium.Infrastructure;
using ST10538419_CLDV7112_Practicium.Infrastructure.Data;
using ST10538419_CLDV7112_Practicium.Infrastructure.Storage;
using ST10538419_CLDV7112_Practicium.Web;
using ST10538419_CLDV7112_Practicium.Web.Options;
using ST10538419_CLDV7112_Practicium.Web.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction() &&
    string.IsNullOrWhiteSpace(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
{
    throw new InvalidOperationException("APPLICATIONINSIGHTS_CONNECTION_STRING is empty. Production logs must go to Application Insights.");
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
if (!string.IsNullOrWhiteSpace(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
{
    builder.Services.AddApplicationInsightsTelemetry();
}
builder.Services.Configure<SiteOptions>(builder.Configuration.GetSection(SiteOptions.SectionName));
builder.Services.AddSingleton<HostingStatusReader>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Denied";
        options.Cookie.Name = "ArgiStream.Auth";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(name: "database");

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var startup = scope.ServiceProvider.GetRequiredService<IDatabaseStartup>();
    await startup.StartAsync(CancellationToken.None);
    var cloud = scope.ServiceProvider.GetRequiredService<ICloudBootstrap>();
    await cloud.StartAsync(CancellationToken.None);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => check.Name == "database"
});
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public partial class Program;
