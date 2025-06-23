using BookApi.Services;
using BookApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:10000");

DotNetEnv.Env.Load();

builder.Configuration.AddEnvironmentVariables() ;

builder.Services.Configure<BookStoreDatabaseSettings>(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION") 
                                ?? builder.Configuration["BookStoreDatabaseSettings:ConnectionString"];
    options.DatabaseName = builder.Configuration["BookStoreDatabaseSettings:DatabaseName"] ?? "BookStoreDb";
    options.BooksCollectionName = builder.Configuration["BookStoreDatabaseSettings:BooksCollectionName"] ?? "Books";
});

builder.Services.Configure<UserManagementSettings>(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION") 
                                ?? builder.Configuration["UserManagementSettings:ConnectionString"];
    options.DatabaseName = builder.Configuration["UserManagementSettings:DatabaseName"] ?? "UserManagementDb";
    options.UsersCollectionName = builder.Configuration["UserManagementSettings:UsersCollectionName"] ?? "Users";
});

builder.Services.Configure<JwtSettings>(options =>
{
    var configuration = builder.Configuration;

    options.Issuer = configuration["JwtSettings:Issuer"] ?? "BookApiIssuer";
    options.Audience = configuration["JwtSettings:Audience"] ?? "BookApiAudience";
    options.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                          ?? configuration["JwtSettings:SecretKey"]
                          ?? throw new InvalidOperationException("JWT secret key is not configured.");
    options.ExpirationMinutes = int.TryParse(configuration["JwtSettings:ExpirationMinutes"], out var minutes)
                                ? minutes
                                : 30;
});


// Lägg till autentisering
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings section is missing in configuration.");

var key = jwtSettings.SecretKey 
    ?? throw new InvalidOperationException("JWT SecretKey is missing.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
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


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();


app.Run();
