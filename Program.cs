using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using SchoolColab.Data;
using SchoolColab.Services;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Register database settings
builder.Services.Configure<databaseSetting>(
	builder.Configuration.GetSection("Mongodb"));

// Register TeacherService
builder.Services.AddSingleton<TeacherService>();
builder.Services.AddSingleton<StudentService>();
builder.Services.AddSingleton<CourseService>();

// Register Cloudinary settings and service
builder.Services.Configure<CloudinarySettings>(
	builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton<Cloudinary>(provider =>
{
	var settings = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
	var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
	return new Cloudinary(account);
});

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
	// Maps OpenAPI/Swagger only in development environment.
    // Useful to test your APIs in browser without deploying to production.
}

app.UseHttpsRedirection();
// Automatically redirects HTTP requests to HTTPS.
// Useful for security.

app.UseAuthorization();

app.MapControllers();
// Maps all controller endpoints (like TeacherController) to the app.
// Without this, all controller routes will return 404.

app.Run();
