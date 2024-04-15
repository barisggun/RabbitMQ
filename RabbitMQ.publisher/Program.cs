using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://fcioefaa:4JSksmDkw2uNLN-vB1Ut-qnzGzw9z-6L@cow.rmq2.cloudamqp.com/fcioefaa");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

// channel.QueueDeclare("hello-queue",true,false,false);
channel.ExchangeDeclare("logs-fanout",durable:true,type:ExchangeType.Fanout);

Enumerable.Range(1,50).ToList().ForEach(x =>
{
string message = $"log {x}";

var messageBody = Encoding.UTF8.GetBytes(message);

channel.BasicPublish("logs-fanout","",null,messageBody);

Console.WriteLine($"Mesaj gönderilmiştir. : {message}");    
});

Console.ReadLine();