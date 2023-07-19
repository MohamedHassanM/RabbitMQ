using RabbitMQ;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using RabbitMQ.Model;

namespace RabbitMQ.Sender
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            factory.ClientProvidedName = "Rabbit Sender App";
            IConnection cnn = factory.CreateConnection();

            IModel channel = cnn.CreateModel();
            string exchangeName = "Demo Exchange";
            string routingKey = "demo-routing-key";
            string queueName = "Demo Queue";
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey);

            for (int i = 0; i < 50; i++)
            {
                int ID = i;
                Message message = new Message(ID, "My Title ", "My Body", "To@Email.com", "CC@Email.com");
                var jsonString = JsonSerializer.Serialize(message);
                byte[] messageBodyBytes = Encoding.UTF8.GetBytes(jsonString);
                //  byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello World!");
                channel.BasicPublish(exchangeName, routingKey, body: messageBodyBytes);
                Console.WriteLine($"Rabbit MQ {ID}");

                Thread.Sleep(1000);
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.WriteLine("Rabbit MQ");
            Console.ReadLine();

            channel.Close();
            cnn.Close();

        }
    }
}