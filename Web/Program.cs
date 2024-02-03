using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Web.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web.Services;
using Microsoft.Extensions.Logging;

// Initializing a new instance of the WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Set database context
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //Server=LAPTOP-N2DK8M6V\\SQLEXPRESS;Database=TEST_DOO;Integrated Security=True;TrustServerCertificate=True;
    //options.LogTo(Console.WriteLine, LogLevel.Information);
    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
    options.EnableSensitiveDataLogging();
});

// Add Services
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CustomerService>();

// Add swagger 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EmployeeManager API",
        Version = "v1"
    });

    // Include the security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    // Add the JWT token as a requirement for all operations
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        },
    });
});

// Add services for controllers
builder.Services.AddControllers();

// Discovering metadata such as the list of controllers and actions
builder.Services.AddEndpointsApiExplorer();

// Add authentication and configure JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "issuer",
        ValidAudience = "audiance",
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes("B+wNnFU7eT2Gu/D7jFB34exogIlMd8BwKasdfjalsdkfjalsdkfjalskdfjlsakdfjalskjfdasldkfjasdfasdfasdfasdfasdfasEEQKKjintc=")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

// Add authorization services
builder.Services.AddAuthorization();

//Configuring application to use HTTPS
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 443;
});

var app = builder.Build();

// User swagger if development enviroment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Strict-Transport-Security header
    app.UseHsts();
}

// Adds middleware for redirecting HTTP Requests to HTTPS
app.UseHttpsRedirection();

// Use authorization
app.UseAuthentication();
app.UseAuthorization();

// Adding endpoints for controller
app.MapControllers();

// Running the application
app.Run();
