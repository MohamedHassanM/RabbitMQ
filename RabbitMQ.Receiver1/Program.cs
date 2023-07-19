using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using RabbitMQ.Model;
using RabbitMQ.EFContext;

namespace RabbitMQ.Receiver1
{
    internal class Program
    {
        static RabbitMQContext ctx;
        static void Main(string[] args)
        {
            ctx = new RabbitMQContext();
            ConnectionFactory factory = new();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            factory.ClientProvidedName = "Rabbit Receiver1 App";
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string exchangeName = "Demo Exchange";
            string routingKey = "demo-routing-key";
            string queueName = "Demo Queue";

            //channel.QueueDeclare(queue: queueName,
            //                     durable: false,
            //                     exclusive: false,
            //                     autoDelete: false,
            //                     arguments: null);

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine(" [*] Waiting for messages.");
            Thread.Sleep(5000);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            { 
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<Message>(body, JsonSerializerOptions.Default);
                if (message != null)
                {
                    Console.WriteLine($"  [XXX] Received,   ID: {message?.ID} , Body: {message?.Body} , To: {message?.To}");
                    //Console.WriteLine($"CC: {message?.CC} , Title: {message?.Title} ");
                    SaveMessage(message); 
                }
                Thread.Sleep(3000); 
                // Task.Delay(TimeSpan.FromSeconds(6)).Wait();
                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            Console.WriteLine("consumerTag");
            string consumerTag = channel.BasicConsume(queue: queueName,
                                    autoAck: false,
                                    consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.WriteLine("RabbitMQ.Receiver1");

            Console.ReadLine();
            channel.BasicCancel(consumerTag);
            channel.Close();
            connection.Close();

        }

        private static async Task SaveMessage(Message message)
        {
            message.ID = 0;
            ctx.Messages.AddAsync(message);
            await ctx.SaveChangesAsync();
        }
    }
}