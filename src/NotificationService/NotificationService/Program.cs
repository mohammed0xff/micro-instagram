using NotificationService.Common;
using NotificationService.Data;
using NotificationService.DBContext;
using NotificationService.EventProcessing;
using NotificationService.EventSubscriber;
using Microsoft.EntityFrameworkCore;
using Shared.Authentication;
using NotificationService.EventHandling;
using Shared.Events;
using NotificationService.Configurations;
using Shared.Events.FollowEvents.Follow;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ")
    );

builder.Services.AddSwagger();
builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserSession, Session>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddScoped<INotificationRepositroy, NotificationRepositroy>();
builder.Services.AddTransient<IEventHandler<FollowCreatedEvent>, FollowEventHandler>();
builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    if (builder.Environment.IsDevelopment())
        options.EnableDetailedErrors().EnableSensitiveDataLogging();
});

var app = builder.Build();

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
