using Microsoft.OpenApi.Models;
using Serilog;
using LegalVibes.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT Authentication
var jwtSecretKey = builder.Configuration["JWT:SecretKey"] ?? 
                  Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? 
                  "LegalVibes-Super-Secret-Key-For-Development-Only-Min-256-Bits-12345";

var jwtIssuer = builder.Configuration["JWT:ValidIssuer"] ?? "https://localhost:7032";
var jwtAudience = builder.Configuration["JWT:ValidAudience"] ?? "https://localhost:5173";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero // Remove default 5 minute tolerance
    };
    
    // Enhanced JWT events for better debugging and security
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information("JWT Token validated for user: {User}", 
                context.Principal?.Identity?.Name ?? "Unknown");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Log.Warning("JWT Authentication challenge: {Error}", context.Error);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Legal Vibes API", 
        Version = "v1",
        Description = "API for Legal Vibes application - AI-powered legal document generation for IP lawyers",
        Contact = new OpenApiContact
        {
            Name = "Legal Vibes Support",
            Email = "support@legalvibes.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            var scheme = httpReq.Scheme;
            var host = httpReq.Host.Value;
            swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{scheme}://{host}" } };
        });
    });
    
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("v1/swagger.json", "Legal Vibes API v1");
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

// Use CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Map controllers and log available routes
app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => "Healthy");

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
