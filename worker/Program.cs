using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Grpc.Net.Client;
using static gRPCTest.Weather;
using gRPCTest;

namespace worker
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] testStrings = new [] { "one", "two", "three", "four", "five" };

            System.Console.WriteLine("Sleeping to wait for Rabbit");

            // TODO: check eshopcontainers, I think there was a proper way of waiting for another service to startup
            Task.Delay(10000).Wait();

            System.Console.WriteLine("Posting message to webApi");
            foreach(var message in testStrings)
            {
                PostMessage(message).Wait();
            }

            Task.Delay(1000).Wait();
            System.Console.WriteLine("Consuming queue now.");

            // Connect to rabbitmq
            ConnectionFactory factory = new ConnectionFactory { HostName = "rabbitmq", Port = 5672 };
            factory.UserName = "guest";
            factory.Password = "guest";
            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            // Connect to "hello" channel on rabbitmq
            channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Add a function for when a message is received from the queue
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ModuleHandle, ea) => {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                System.Console.WriteLine($" [x] Received from Rabbit: {message}");
            };

            channel.BasicConsume(
                queue: "hello",
                autoAck: true,
                consumer: consumer
            );
        }


        public static async Task PostMessage(string postData)
        {
            var json = JsonConvert.SerializeObject(postData);
            var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            using(var httpHandler = new HttpClientHandler())
            {
                httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                using(var client = new HttpClient()){
                    var result = await client.PostAsync("http://publisher_api/WeatherForecast", content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    Console.WriteLine("Server returned: " + resultContent);
                }
            }
        }


        public static async Task gRPCTest(){

            System.Console.WriteLine("Retrieving gRPC weather forecast info...");
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // Allow http: This switch must be set before creating the GrpcChannel/HttpClient. https://docs.microsoft.com/en-us/aspnet/core/grpc/troubleshoot?view=aspnetcore-3.1#call-insecure-grpc-services-with-net-core-client
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new WeatherClient(channel);
            var reply = await client.ForecastAsync(new WeatherForecastRequest());

            Console.WriteLine($"Received a reply from gRPC weather forecase: {@reply}");
        }
    }
}
