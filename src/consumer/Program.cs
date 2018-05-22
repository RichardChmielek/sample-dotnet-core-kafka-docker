using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Configuration;

namespace Consumer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var typeName = typeof(Program).Assembly.GetName();
            Console.WriteLine($"Starting {typeName.Name} v{typeName.Version}...");

            // Create the consumer configuration
            var config = new Dictionary<string, object>
            {
                { "group.id", Common.GetConfigValue("group.id") },
                { "bootstrap.servers", Common.GetConfigValue("bootstrap.servers") },
                { "auto.commit.interval.ms", Common.GetConfigValue("auto.commit.interval.ms") },
                { "auto.offset.reset", Common.GetConfigValue("auto.offset.reset") }
            };

            Console.WriteLine($"Connecting consumer to '{Common.GetConfigValue("bootstrap.servers")}' kafka endpoint...");
            // Create the consumer
            using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            {
                // Subscribe to the OnMessage event
                consumer.OnMessage += (obj, msg) => 
                {
                    Console.WriteLine($"Received: {msg.Value}. Offset: {msg.Offset}. Partition: {msg.Partition}");
                };

                // Subscribe to the Kafka topic
                consumer.Subscribe(new List<string>() { Common.GetConfigValue("topic") });

                // Handle Cancel Keypress 
                var cancelled = false;
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cancelled = true;
                };

                Console.WriteLine("Ctrl-C to exit.");

                // Poll for messages
                while (!cancelled)
                {
                    consumer.Poll(TimeSpan.FromMilliseconds(100));
                }
            }
        }
    }
}