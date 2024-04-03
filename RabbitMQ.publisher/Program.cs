using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://fcioefaa:4JSksmDkw2uNLN-vB1Ut-qnzGzw9z-6L@cow.rmq2.cloudamqp.com/fcioefaa");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.QueueDeclare("hello-queue",true,false,false);

string message = "hello world";

var messageBody = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(string.Empty,"hello-queue",null,messageBody);

Console.WriteLine("Mesaj gönderilmiştir.");

Console.ReadLine();