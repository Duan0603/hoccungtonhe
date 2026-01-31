using System.Text;
using DotNetEnv;
using EduVN.Application.Settings;
using EduVN.Infrastructure.Persistence;
using EduVN.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// Load environment variables from .env file
// Load environment variables from .env file (search up directory tree)
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Kestrel for larger uploads (500MB for video)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 500 * 1024 * 1024; // 500MB
});

// Database configuration from environment variables
var dbHost = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "eduvn";
var dbUser = Environment.GetEnvironmentVariable("DATABASE_USER") ?? "";
var dbPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "";
var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        b => b.MigrationsAssembly("EduVN.Infrastructure")
    )
);

// JWT Configuration from environment variables
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "https://eduvn.com";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "https://eduvn.com";
var jwtAccessTokenExpiry = int.TryParse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRY_MINUTES"), out var expiry) ? expiry : 15;
var jwtRefreshTokenExpiry = int.TryParse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRY_DAYS"), out var refreshExpiry) ? refreshExpiry : 7;

builder.Services.Configure<JwtSettings>(options =>
{
    options.SecretKey = jwtSecretKey;
    options.Issuer = jwtIssuer;
    options.Audience = jwtAudience;
    options.AccessTokenExpiryMinutes = jwtAccessTokenExpiry;
    options.RefreshTokenExpiryDays = jwtRefreshTokenExpiry;
});

var key = Encoding.UTF8.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<EduVN.Application.Interfaces.ICourseRepository, EduVN.Infrastructure.Repositories.CourseRepository>();
builder.Services.AddScoped<EduVN.Application.Interfaces.ILessonRepository, EduVN.Infrastructure.Repositories.LessonRepository>();

// Cloudinary configuration from environment variables
builder.Services.Configure<CloudinarySettings>(options =>
{
    options.CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME") ?? "";
    options.ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY") ?? "";
    options.ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET") ?? "";
});
builder.Services.AddScoped<EduVN.Application.Interfaces.IFileUploadService, EduVN.Infrastructure.Services.CloudinaryUploadService>();

// Google OAuth configuration
var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? "";
builder.Services.AddScoped<EduVN.Application.Interfaces.IGoogleAuthService>(
    _ => new EduVN.Infrastructure.Services.GoogleAuthService(googleClientId));

// CORS configuration for Next.js frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Ensure wwwroot exists for uploads
var webRootPath = app.Environment.WebRootPath;
if (string.IsNullOrEmpty(webRootPath))
{
    // Fallback if not set (e.g. absent wwwroot folder)
    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    app.Environment.WebRootPath = webRootPath;
}
if (!Directory.Exists(webRootPath))
{
    Directory.CreateDirectory(webRootPath);
}

// Seed database with demo users (Development only)
// if (app.Environment.IsDevelopment())
// {
    // Seeding now handled via EF Core Migrations (dotnet ef database update)
// }

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disabled for local development to avoid Network Error in some environments

// CORS must be before UseStaticFiles and UseAuthentication
app.UseCors("AllowFrontend");

app.UseStaticFiles(); // Enable serving files from wwwroot
app.UseAuthentication(); // MUST be before UseAuthorization
app.UseAuthorization();
app.MapControllers();

app.Run();
