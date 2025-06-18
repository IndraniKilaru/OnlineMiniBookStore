using BookStore.NotificationService.Services;
using BookStore.NotificationService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Add RabbitMQ Consumers as Hosted Services
builder.Services.AddHostedService<UserRegisteredConsumer>();
builder.Services.AddHostedService<OrderCreatedConsumer>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add logging
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("NotificationService starting up...");
app.Logger.LogInformation("Consumers will start listening for RabbitMQ messages");

app.Run();
