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
builder.Services.AddSwaggerGen();

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
builder.Services.AddScoped<IUserDao, UserDao>();

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