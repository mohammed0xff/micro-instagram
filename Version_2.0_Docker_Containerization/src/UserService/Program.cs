using Microsoft.EntityFrameworkCore;
using Shared.Authentication;
using Swashbuckle.AspNetCore.SwaggerUI;
using UserService.Configurations;
using UserService.DBContext;
using UserService.Services;
using UserService.MessageBus;
using UserService.Common;
using UserService.DBContext.Seed;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserSession, Session>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IAppDbContext, AppDbContext>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ")
    );

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    if (builder.Environment.IsDevelopment())
        options.EnableDetailedErrors().EnableSensitiveDataLogging();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger()
        .UseSwaggerUI(options =>
        {
            options.DisplayRequestDuration();
            options.DocExpansion(DocExpansion.List);
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "swagger";
        });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.SeedDataAsync();

app.Run();
