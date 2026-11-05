using FinalCloudDev.Application;
using FinalCloudDev.Application.Configuration;
using FinalCloudDev.Application.Data;
using FinalCloudDev.Infrastructure;
using FinalCloudDev.Infrastructure.Data;
using FinalCloudDev.Web.Options;
using FinalCloudDev.Web.Security;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.Configure<SiteOptions>(builder.Configuration.GetSection(SiteOptions.SectionName));
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(name: "database");

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var startup = scope.ServiceProvider.GetRequiredService<IDatabaseStartup>();
    await startup.StartAsync(CancellationToken.None);
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
