using System.Reflection.Emit;
using RabbitMQ.Client;
using RabbitMQ.Client.Logging;
using RabbitMQWeb.Watermark.Services;

namespace RabbitMQWeb.Watermark.BackgroundServices;

public class ImageWatermarkProcessBackgroundService : BackgroundService
{
    private readonly RabbitMQClientService _rabbitMqClientService;
    private readonly ILogger<ImageWatermarkProcessBackgroundService> _logger;
    private IModel _channel; //constructorda set etmeyecegimiz için readonly demedik. readonlyleri constructorda set edebiliriz.
    
    public ImageWatermarkProcessBackgroundService(RabbitMQClientService rabbitMqClientService, ILogger<ImageWatermarkProcessBackgroundService> logger)
    {
        _rabbitMqClientService = rabbitMqClientService;
        _logger = logger;
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = _rabbitMqClientService.Connect();
        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }


    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}