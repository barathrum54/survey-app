using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SurveyApp.API.Configuration;
using SurveyApp.API.DAOs;
using SurveyApp.API.DAOs.Interfaces;
using SqlBatis.DataMapper.DependencyInjection;
using SurveyApp.API.Services;
using SurveyApp.API.Services.Interfaces;
using SurveyApp.API.Middleware;
using SurveyApp.API.DTOs;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// 🔧 Configuration Setup
// ----------------------
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// ----------------------
// 🥉 Service Registration
// ----------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "SurveyApp API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token here. Example: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<IVoteService, VoteService>();

// 🔐 JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();


// 🗄️ iBATIS Mapper
builder.Services.AddSqlMapper(options =>
    builder.Configuration.GetSection("DB").Bind(options));

// 🧪 Scoped DAOs
Console.WriteLine("🔍 Registering DAOs...");
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<ISurveyDao, SurveyDao>();
builder.Services.AddScoped<IOptionDao, OptionDao>();
builder.Services.AddScoped<IVoteDao, VoteDao>();

Console.WriteLine("🔍 Registering DAOs DONE.");

// 🌍 Validators
builder.Services.AddTransient<IValidator<CreateSurveyRequest>, CreateSurveyRequestValidator>();
builder.Services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddTransient<IValidator<VoteRequest>, VoteRequestValidator>();

var app = builder.Build();



// ----------------------
// 🚀 Middleware Pipeline
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();

// ----------------------
// 🛠️ Debug: Embedded Resources
// ----------------------
var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
Console.WriteLine("🔍 Embedded Resources:");
foreach (var name in resourceNames)
{
    Console.WriteLine($"  - {name}");
}

app.Run();
public partial class Program { }