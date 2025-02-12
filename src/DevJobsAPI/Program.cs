using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using DevJobsAPI.Data;
using DevJobsAPI.Services.Interfaces;
using DevJobsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Load User Secrets Before Anything Else
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Debugging: Print all loaded configuration values
Console.WriteLine("Loaded Configuration Values:");
foreach (var configItem in builder.Configuration.AsEnumerable())
{
    Console.WriteLine($"{configItem.Key} = {configItem.Value}");
}


//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string"
//        + "'DefaultConnection' not found.");
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? throw new InvalidOperationException("? Connection string 'DefaultConnection' not found.");
// Add required NuGet packages
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 25))));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new ArgumentNullException("Missing Jwt secret."));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"] ?? throw new ArgumentNullException("Missing Jwt Issuer."),
            ValidAudience = jwtSettings["Audience"] ?? throw new ArgumentNullException("Missing Jwt Audience."),
            ValidateLifetime = true
        };
    });

builder.Services.AddScoped<IJobService, JobService>();

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
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

// **Required for Integration Tests**
public partial class Program { }