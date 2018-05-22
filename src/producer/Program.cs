using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var typeName = typeof(Program).Assembly.GetName();
            Console.WriteLine($"Starting {typeName.Name} v{typeName.Version}...");
            var endpoint = Common.GetConfigValue("bootstrap.servers");
            var config = new Dictionary<string, object> 
            { 
                { "bootstrap.servers", endpoint } 
            };

            Console.WriteLine($"Connecting producer to '{Common.GetConfigValue("bootstrap.servers")}' kafka endpoint...");

            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                // Send 10 messages to the topic
                for (int i = 0; i < 10; i++)
                {
                    var message = $"Event {i}";
                    var result = producer.ProduceAsync(Common.GetConfigValue("topic"), null, message).GetAwaiter().GetResult();
                    Console.WriteLine($"Event {i} sent on Partition: {result.Partition} with Offset: {result.Offset}");
                }
                producer.Flush(250);
            }
        }
    }
}
