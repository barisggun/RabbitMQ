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

//Baştaki 0 herhangi bir boyutta dosyanın gönderilebileceğini söylüyor, 6 kaç tane mesaj gönderileceğini söylüyor, false veya true ise true yaparsak 6 mesaj varsa bunu 3-3- bölerek yollar.


var randomQueueName = channel.QueueDeclare().QueueName;
channel.QueueBind(randomQueueName,"logs-fanout","",null);
//uygulama down olduğunda kuyruk silinecek.

//
channel.BasicQos(0,1,false);

var consumer = new EventingBasicConsumer(channel);

//mesajları sildirmedik false ile
channel.BasicConsume(randomQueueName,false,consumer);
Console.WriteLine("logları dinleniyor");
consumer.Received += (object AssemblyDependencyResolver, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    
    Thread.Sleep(1500);
    
    Console.WriteLine("Gelen mesaj: " + message);
    
    //buradaki false ise eğer true dersek o an memoryde işlenmiş ama rabbitmqya gitmemiş başka mesajlar da varsa onun bilgilerini rabbitmq'ye haberdar eder. tek mesajı işliyoruz o yüzden false verdik.
    channel.BasicAck(e.DeliveryTag,false);
};

Console.ReadLine();