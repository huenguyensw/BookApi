using BookApi.Services;
using BookApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
// builder.WebHost.UseUrls("http://0.0.0.0:10000");

// Bind to PORT env variable (for Render)
var port = Environment.GetEnvironmentVariable("MYAPP_PORT") ?? "10000";

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(Int32.Parse(port));
});

DotNetEnv.Env.Load();

builder.Configuration.AddEnvironmentVariables(prefix: "MYAPP_") ;


//Load custom configuration settings

string GetEnvOrConfig(string envVar, string configValue) =>
    Environment.GetEnvironmentVariable(envVar) ?? configValue;

// Bind and configure BookStoreDatabaseSettings
var bookStoreSettings = new BookStoreDatabaseSettings();
builder.Configuration.GetSection("BookStoreDatabaseSettings").Bind(bookStoreSettings);
bookStoreSettings.ConnectionString = GetEnvOrConfig("MYAPP_MONGODB_CONNECTION", bookStoreSettings.ConnectionString);

builder.Services.Configure<BookStoreDatabaseSettings>(options =>
{
    options.ConnectionString = bookStoreSettings.ConnectionString;
    options.DatabaseName = bookStoreSettings.DatabaseName;
    options.BooksCollectionName = bookStoreSettings.BooksCollectionName;
});


// Bind and configure UserManagementSettings
var userManagementSettings = new UserManagementSettings();
builder.Configuration.GetSection("UserManagementSettings").Bind(userManagementSettings);
userManagementSettings.ConnectionString = GetEnvOrConfig("MYAPP_MONGODB_CONNECTION", userManagementSettings.ConnectionString);

builder.Services.Configure<UserManagementSettings>(options =>
{
    options.ConnectionString = userManagementSettings.ConnectionString;
    options.DatabaseName = userManagementSettings.DatabaseName;
    options.UsersCollectionName = userManagementSettings.UsersCollectionName;
});


var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
jwtSettings.SecretKey = Environment.GetEnvironmentVariable("MYAPP_JWT_SECRET_KEY");

if (string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JWT secret key is not configured.");
}
// Optional: Log the key for debugging (⚠️ Never do this in production!)
Console.WriteLine($"[Startup] JWT Secret Key (first 5 chars): {jwtSettings.SecretKey.Substring(0, 5)}...");
// Correct key for expiry minutes (matches your appsettings.json)
jwtSettings.ExpirationMinutes = int.TryParse(builder.Configuration["JwtSettings:ExpiryMinutes"], out var minutes)
                                ? minutes
                                : 30;

builder.Services.Configure<JwtSettings>(options =>
{
    options.Issuer = jwtSettings.Issuer ?? "BookApiIssuer";
    options.Audience = jwtSettings.Audience ?? "BookApiAudience";
    options.SecretKey = jwtSettings.SecretKey;
    options.ExpirationMinutes = jwtSettings.ExpirationMinutes;
});



var key = jwtSettings.SecretKey 
    ?? throw new InvalidOperationException("JWT SecretKey is missing.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
     var secretKey = builder.Configuration["Jwt:Key"];
        Console.WriteLine($"[JWT Validation] Using signing key: {secretKey}");
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
    // Extract token from cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // 1. From cookie
        var token = context.Request.Cookies["token"];
        
        // 2. Fallback to Authorization header
        if (string.IsNullOrEmpty(token))
        {
            token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Replace("Bearer ", "");
        }

        if (!string.IsNullOrEmpty(token))
        {
            context.Token = token;
        }

        return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthorization();


// 🔵 Registrera BookService som Singleton
builder.Services.AddSingleton<BookService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<JwtService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<UserService>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "BookApi", Version = "v1" });
        // Lägg till JWT-auth i Swagger
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
           Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Skriv in JWT med 'Bearer ' prefix. Exempel: Bearer eyJhbGciOiJIUzI1NiIs..."
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
                new string[] {}
            }
        });
    });

builder.Services.AddControllers();


var allowedOrigins = (Environment.GetEnvironmentVariable("MYAPP_FRONTEND_URLS") ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("https://book-app-delta-hazel.vercel.app", "https://book-app-delta.vercel.app", "http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); 
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
     app.UseHttpsRedirection();
}

// app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.Run();
