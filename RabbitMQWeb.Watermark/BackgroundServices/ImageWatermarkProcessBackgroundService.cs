using System.Diagnostics.Tracing;
using System.Drawing;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Logging;
using RabbitMQWeb.Watermark.Services;

namespace RabbitMQWeb.Watermark.BackgroundServices;

public class ImageWatermarkProcessBackgroundService : BackgroundService
{
    private readonly RabbitMQClientService _rabbitMQClientService;
    private readonly ILogger<ImageWatermarkProcessBackgroundService> _logger;
    private IModel _channel;

    public ImageWatermarkProcessBackgroundService(RabbitMQClientService rabbitMQClientService,
        ILogger<ImageWatermarkProcessBackgroundService> logger)
    {
        _rabbitMQClientService = rabbitMQClientService;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = _rabbitMQClientService.Connect();

        _channel.BasicQos(0, 1, false);


        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);

        consumer.Received += Consumer_Received;


        return Task.CompletedTask;
    }

    private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
    {
        Task.Delay(10000).Wait();


        try
        {
            var productImageCreatedEvent =
                JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));


            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images",
                productImageCreatedEvent.ImageName);

            var siteName = "wwww.mysite.com";

            using var img = Image.FromFile(path);

            using var graphic = Graphics.FromImage(img);

            var font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold, GraphicsUnit.Pixel);

            var textSize = graphic.MeasureString(siteName, font);

            var color = Color.FromArgb(128, 255, 255, 255);
            var brush = new SolidBrush(color);

            var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height + 30));


            graphic.DrawString(siteName, font, brush, position);

            img.Save("wwwroot/images/watermarks/" + productImageCreatedEvent.ImageName);


            img.Dispose();
            graphic.Dispose();

            _channel.BasicAck(@event.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }


        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}