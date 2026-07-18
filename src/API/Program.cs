using Application.Interfaces;
using Application.Services;
using Domain.Entities.Identity;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//For adding Swagger services. This allows us to generate API documentation and test our API endpoints using the Swagger UI.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CookinSocial API",
        Version = "v1",
        Description = "REST API for the CookinSocial social cooking platform.",
        Contact = new OpenApiContact
        {
            Name = "Gohar Ali",
            Url = new Uri("https://github.com/gohar-ali123")
        }
    });

    //For adding JWT Bearer token authentication to Swagger. This allows us to test our API endpoints that require authentication.
    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT Bearer token"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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
});

//For connecting to database using Entity Framework Core and SQL Server, we need to add the ApplicationDBContext to the services collection. This allows us to inject the context into our controllers or services where we need to interact with the database.
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//For adding Identity services. This allows us to use the built-in Identity features such as user registration, login, and role management (via userManager etc.).
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password policy
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Lockout will be configured later

    // Sign-in policy
    options.SignIn.RequireConfirmedEmail = false; // Will flip to true once email confirmation endpoint exists
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders(); //Required for generating tokens for password reset, email confirmation, etc.

//For strongly typed jwt settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

//For adding authentication services. This allows us to use JWT Bearer tokens for authenticating users.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //options.SaveToken = true; //to save exact access token e.g to pass on to other services
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

//For adding authorization services. This allows us to use the built-in authorization features such as role-based or policy-based authorization.
builder.Services.AddAuthorization();


// Services
builder.Services.AddScoped<IAuthService, AuthService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
