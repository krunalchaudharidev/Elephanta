using System.Text;
using Elephanta.API.Data;
using Elephanta.Domain.Constants;
using Elephanta.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Configure CORS to allow the frontend to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("Elephanta.Web", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Elephanta API",
        Version = "v1",
        Description = "Elephanta E-commerce API"
    });

    // JWT Bearer Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token. Example: eyJhbGciOi..."
    });

    // Apply Bearer authentication globally in Swagger
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

    // Include XML comments from application and API assemblies so DTO and controller remarks (like role) appear in Swagger UI
    try
    {
        var xmlAppPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Elephanta.Application.xml");
        if (File.Exists(xmlAppPath)) options.IncludeXmlComments(xmlAppPath, includeControllerXmlComments: true);

        var xmlApiPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Elephanta.API.xml");
        if (File.Exists(xmlApiPath)) options.IncludeXmlComments(xmlApiPath, includeControllerXmlComments: true);
    }
    catch
    {
        // ignore if XML doc not present
    }
    // Ensure endpoints with custom GroupName (ApiExplorerSettings) are included in the main v1 document
    options.DocInclusionPredicate((docName, apiDesc) => true);
});

// Configure DbContext for PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
                       ?? "Host=localhost;Database=elephanta;Username=postgres;Password=postgres";

builder.Services.AddDbContext<Elephanta.Infrastructure.Persistence.ElephantaDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Register infrastructure services
builder.Services.AddInfrastructure();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT configuration 'Jwt:Key' is missing.");
}

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey =
                new SymmetricSecurityKey(keyBytes),

            ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
            ValidIssuer = jwtIssuer,

            ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.AdminOnly, policy => policy.RequireRole("Admin"));
    options.AddPolicy(AuthorizationPolicies.UserOrAdmin, policy => policy.RequireRole("User", "Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// I will move this(Swagger) to the development condition after it goes live in production.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elephanta API V1");
    // Serve the Swagger UI at application root
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// Enable CORS for the configured policy so preflight requests succeed
app.UseCors("Elephanta.Web");

app.UseAuthentication();
app.UseAuthorization();

// Seed default roles and admin user at startup using centralized seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();

    // Seed using centralized DataSeeder which reads admin password from configuration.
    await DataSeeder.SeedAsync(services, config);
}

app.MapControllers();

app.Run();
