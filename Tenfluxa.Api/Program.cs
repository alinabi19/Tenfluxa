using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Tenfluxa.Api.Hubs;
using Tenfluxa.Api.Middleware;
using Tenfluxa.Api.Services;
using Tenfluxa.Application.Events.Handlers;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Application.Services;
using Tenfluxa.Domain.Events;
using Tenfluxa.Infrastructure.Events;
using Tenfluxa.Infrastructure.Persistence;
using Tenfluxa.Infrastructure.Persistence.Repositories;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer <your_token>"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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

builder.Services.AddDbContext<TenfluxaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();

// Infrastructure
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

builder.Services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

builder.Services.AddScoped<WorkerAssignedEventHandler>();

builder.Services.AddScoped<IDomainEventHandlerDispatcher, DomainEventHandlerDispatcher>();

builder.Services.AddScoped<IDomainEventHandler<WorkerAssignedEvent>, WorkerAssignedEventHandler>();

builder.Services.AddScoped<INotificationService, SignalRNotificationService>();

builder.Services.AddScoped<IAssignmentEngine, AssignmentEngine>();

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddSingleton(new JwtService(
    jwtSettings["Key"],
    jwtSettings["Issuer"],
    jwtSettings["Audience"],
    int.Parse(jwtSettings["ExpiryMinutes"])
));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

builder.Services.AddHangfireServer();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();

app.MapHub<JobHub>("/hubs/job");

app.Run();