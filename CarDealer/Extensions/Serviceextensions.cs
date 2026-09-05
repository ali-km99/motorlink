using CarDealer.API.Authorization;
using CarDealer.API.Data;
using CarDealer.API.Features.Cars;
using CarDealer.API.Features.Cars.Repositories.Implementations;
using CarDealer.API.Features.Cars.Repositories.Interfaces;
using CarDealer.API.Features.Cars.Services.Implementations;
using CarDealer.API.Features.Cars.Services.Interfaces;
using CarDealer.API.Features.Customers.Repositories.Implementations;
using CarDealer.API.Features.Customers.Repositories.Interfaces;
using CarDealer.API.Features.Customers.Services.Implementations;
using CarDealer.API.Features.Customers.Services.Interfaces;
using CarDealer.API.Features.Dashboard.Services.Implementations;
using CarDealer.API.Features.Dashboard.Services.Interfaces;
using CarDealer.API.Features.Expenses.Services.Implementations;
using CarDealer.API.Features.Expenses.Services.Interfaces;
using CarDealer.API.Features.Maintenance.Repositories.Implementations;
using CarDealer.API.Features.Maintenance.Repositories.Interfaces;
using CarDealer.API.Features.Maintenance.Services.Implementations;
using CarDealer.API.Features.Maintenance.Services.Interfaces;
using CarDealer.API.Features.PublicSharing.Services;
using CarDealer.API.Features.Sales.Services;
using CarDealer.API.Features.Transactions.Services.Implementations;
using CarDealer.API.Features.Transactions.Services.Interfaces;
using CarDealer.API.Repositories;
using CarDealer.API.Repositories.Interfaces;
using CarDealer.API.Services;
using CarDealer.API.Services.Interfaces;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CarDealer.API.Extensions;

public static class ServiceExtensions
{
    // ─── Database ──────────────────────────────────────────────────────────────
    public static IServiceCollection AddDatabase(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(maxRetryCount: 3)
            ));

        return services;
    }

    // ─── Repositories ──────────────────────────────────────────────────────────
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<IMaintenanceCenterRepository, MaintenanceCenterRepository>();
        services.AddScoped<IMaintenancePaymentRepository, MaintenancePaymentRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPublicShareRepository, PublicShareRepository>();
        return services;
    }

    // ─── Services ──────────────────────────────────────────────────────────────
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<ICarImageService, CarImageService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IMaintenanceCenterService, MaintenanceCenterService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IFeatureService, FeatureService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ICarStatusService, CarStatusService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IClaimsTransformation, PermissionClaimsTransformation>();
        services.AddScoped<IPublicShareService, PublicShareService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
        services.AddScoped<IMarketplaceAuthService, MarketplaceAuthService>();
        // Multi-tenant services
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantFeatureService, TenantFeatureService>();

        return services;
    }

    // ─── Validators ────────────────────────────────────────────────────────────
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateCarValidator>();

        return services;
    }

    // ─── CORS ─────────────────────────────────────────────────────────────────
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("VueFrontend", policy =>
                policy
                    .WithOrigins("http://localhost:5173", "https://shirmeet.ly")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        return services;
    }


    // ─── JWT Authentication ────────────────────────────────────────────────────
    public static IServiceCollection AddJwtAuth(
        this IServiceCollection services, IConfiguration config)
    {
        var secret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured.");
        var issuer = config["Jwt:Issuer"] ?? "CarDealerAPI";
        var audience = config["Jwt:Audience"] ?? "CarDealerClient";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero   // لا نسمح بأي فارق وقت
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    ctx.Response.Headers["Token-Expired"] =
                        ctx.Exception is SecurityTokenExpiredException ? "true" : "false";
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
    // ─── Swagger with JWT support ─────────────────────────────────────────────
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Motor Link API",
                Version = "v1",
                Description = "API for Motor Link Management System"
            });

            // إضافة زر Authorize في Swagger UI
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter: Bearer {your_token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}