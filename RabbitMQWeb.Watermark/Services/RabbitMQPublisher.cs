using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitMQWeb.Watermark.Services;

public class RabbitMQPublisher
{
    private readonly RabbitMQClientService _rabbitMqClientService;

    public RabbitMQPublisher(RabbitMQClientService rabbitMqClientService)
    {
        _rabbitMqClientService = rabbitMqClientService;
    }

    public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
    {
        var channel = _rabbitMqClientService.Connect();
        var bodyString = JsonSerializer.Serialize(productImageCreatedEvent);
        var bodyByte = Encoding.UTF8.GetBytes(bodyString);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish(exchange: RabbitMQClientService.ExchangeName,  routingKey: RabbitMQClientService.RoutingWatermark, basicProperties: properties, body: bodyByte);

    }
}