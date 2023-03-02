using Kernal.Events;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using StackExchange.Redis;
using web;
using web.Infrastructure.Filters;
using web.Infrastructure.Repository;
using web.IntegrationEvents.EventHandler;
using web.IntegrationEvents.Events;
using web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
builder.Host.UseSerilog((context, provider, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddOpenTelemetry().WithTracing(b =>
{
    b
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
        })
        .AddSource("Basket-API")
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: "Basket-API", serviceVersion: "1.0"))
        .AddRedisInstrumentation(ConnectionMultiplexer.Connect("0.0.0.0:6379"))
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation();
});
// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(GlobalExceptionFilter));
    options.Filters.Add(typeof(ValidateModelStateFilter));

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{            
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RentIt - Basket HTTP API",
        Version = "v1",
        Description = "The Basket Service HTTP API"
    });
});
//Configure other services up here
builder.Services.Configure<BasketSettings>(builder.Configuration);
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<BasketSettings>>().Value;
    return ConnectionMultiplexer.Connect(settings.ConnectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy
            .SetIsOriginAllowed((host) => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IBasketRepository, BasketRepository>();
builder.Services.AddTransient<IIdentityServices, IdentityService>();
builder.Services.AddTransient<IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>,ProductPriceChangedIntegrationEventHandler>();
builder.Services.AddTransient<IIntegrationEventHandler<OrderStartedIntegrationEvent>,OrderStartedIntegrationEventHandler>();
//builder.Services.AddSingleton<IHostedService, KafkaConsumerHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
        .UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();


