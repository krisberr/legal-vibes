using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Legal Vibes API", 
        Version = "v1",
        Description = "API for Legal Vibes application"
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:5173") // Vite default port
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Legal Vibes API v1");
        c.RoutePrefix = "swagger";
    });
    Log.Information("Swagger UI enabled at /swagger");
}

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

// Configure HTTPS redirection
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    Log.Information("HTTPS redirection enabled for production");
}
else
{
    // In development, we'll handle both HTTP and HTTPS
    var httpsPort = builder.Configuration["https_port"] ?? "7032";
    app.UseHttpsRedirection();
    Log.Information("HTTPS redirection enabled for development (port: {HttpsPort})", httpsPort);
}

app.UseRouting();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

// Map controllers and log available routes
app.MapControllers();

var urls = string.Join(", ", builder.WebHost.GetSetting("urls")?.Split(';') ?? new[] { "No URLs configured" });
Log.Information("Application configured with URLs: {Urls}", urls);

try
{
    Log.Information("Starting Legal Vibes API");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
