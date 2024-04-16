using RabbitMQ.Client;

namespace RabbitMQWeb.Watermark.Services;

public class RabbitMQClientService
{
    private readonly ConnectionFactory _connectionFactory;

    private IConnection _connection;

    private IModel _channel;

    public static string ExchangeName = "ImageDirectExchange";

    public static string RoutingWatermark = "watermark-route-image";
}