using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.GrafanaLoki;
using KafkaConsumer;
using OpenTelemetry;

var  host = Host.CreateDefaultBuilder(args).
    ConfigureAppConfiguration(
        (hostContext, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appsettings.json", false, true);
            config.AddEnvironmentVariables();
        }
    )
    .ConfigureLogging(
        loggingBuilder =>
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            loggingBuilder.AddSerilog(logger, dispose: true);
        }
    )
    .ConfigureServices( services =>
    {
        services.AddOpenTelemetry().WithTracing(b =>
        {
            b
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                })
                .AddSource("KafkaConsumer")
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: "KafkaConsumer", serviceVersion: "1.0"))
                .AddAspNetCoreInstrumentation();
            
        });
        services.AddHttpClient();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();