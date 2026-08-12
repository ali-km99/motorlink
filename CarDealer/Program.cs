using CarDealer.API.Data;
using CarDealer.API.Extensions;
using CarDealer.API.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ─── Config loading ───────────────────────────────────────────────────────────
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ─── Validate required config ─────────────────────────────────────────────────
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connStr))
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
    throw new InvalidOperationException("Jwt:Secret must be configured and at least 32 characters.");

// ─── Services ─────────────────────────────────────────────────────────────────
builder.Services
    .AddDatabase(builder.Configuration)
    .AddRepositories()
    .AddAppServices()
    .AddValidation()
    .AddJwtAuth(builder.Configuration)
    .AddSwaggerDocs()
    .AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",  // Vue dev
                "https://shirmeet.ly"     // production
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ─── Rate Limiter Configuration ───────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("PublicSharePolicy", opt =>
    {
        opt.PermitLimit = 30;               // 30 طلب
        opt.Window = TimeSpan.FromMinutes(1); // كل دقيقة
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = 429;
});

// ─── Build ────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ─── Middleware Pipeline ──────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Dealer API v1"));

// ─── wwwroot auto create ──────────────────────────────────────────────────────
if (string.IsNullOrEmpty(app.Environment.WebRootPath))
{
    app.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    Directory.CreateDirectory(app.Environment.WebRootPath);
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("VueFrontend");
app.UseAuthentication();
app.UseAuthorization();

// ✅ تم إضافة الـ RateLimiter قبل MapControllers
app.UseRateLimiter();

app.MapControllers();

app.MapFallback(async context =>
{
    var path = context.Request.Path;

    // ✅ تجاهل API و images وخليهم يكملوا للـ Controllers
    if (path.StartsWithSegments("/api") ||
        path.StartsWithSegments("/images"))
    {
        return; // 🔥 مهم: لا ترجع 404
    }

    var indexPath = Path.Combine(app.Environment.WebRootPath ?? "", "index.html");

    if (File.Exists(indexPath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    }
    else
    {
        // ✅ لا تكسر السيرفر
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Frontend not found");
    }
});

// ─── Auto Migrate ─────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();