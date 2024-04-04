using System.Runtime.Loader;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://fcioefaa:4JSksmDkw2uNLN-vB1Ut-qnzGzw9z-6L@cow.rmq2.cloudamqp.com/fcioefaa");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

//burasi istege bagli,publisherın kuyruğu oluşturduğuna emin isek kaldırabiliriz kuyruk yoksa oluşturur
//channel.QueueDeclare("hello-queue",true,false,false);

var consumer = new EventingBasicConsumer(channel);

channel.BasicConsume("hello-queue",true,consumer);

consumer.Received += (object AssemblyDependencyResolver, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine("Gelen mesaj: " + message);
};

Console.ReadLine();