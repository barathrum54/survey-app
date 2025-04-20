using SurveyApp.API.Services.IBatis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSingleton<IIbatisService, IbatisService>();

var app = builder.Build();

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Removed Openapi
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
