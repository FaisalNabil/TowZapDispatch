using Dispatch.API;
using Dispatch.API.Hubs;
using Dispatch.Application.Common.Interface;
using Dispatch.Domain.Entities;
using Dispatch.Infrastructure.Persistence;
using Dispatch.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

// ✅ CORS Policy (Allow specific origin like Blazor client)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowTowZapClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Optional: if you're using cookies or auth headers
    });
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5047); // ✅ open HTTP port to all IPs
});


// Add app services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJobRequestService, JobRequestService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddControllers();
builder.Services.AddSignalR(); 

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");


var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Use Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExpiryMiddleware>();

app.UseCors("AllowTowZapClient");


#if DEBUG
#else

app.UseHttpsRedirection();

#endif

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<JobUpdateHub>("/hubs/jobUpdates");

// Seed roles and default users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbSeeder.SeedRolesAndUsersAsync(services);
}

app.Run();
app.MapGet("/", () => "TowZap Dispatch API is running!");

