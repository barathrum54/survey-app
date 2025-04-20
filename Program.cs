using System.Reflection;
using SqlBatis.DataMapper.DependencyInjection;
using SurveyApp.API.DAOs;
using SurveyApp.API.DAOs.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSqlMapper(options =>
    builder.Configuration.GetSection("DB").Bind(options));

builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.UseHttpsRedirection();

// List embedded resources for debugging
var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
Console.WriteLine("🔍 Embedded Resources:");
foreach (var name in resourceNames)
{
    Console.WriteLine($"  - {name}");
}
app.Run();
