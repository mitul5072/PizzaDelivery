using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace PizzaDeliverySB
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "pizzadeliveryqueue";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            Client.OnMessage((receivedMessage) =>
            {
                try
                {
                    Trace.WriteLine("Processing", receivedMessage.SequenceNumber.ToString());

                    var order = receivedMessage.GetBody<DeliveryModel>();

                    Trace.WriteLine(order.CustomerName + ": " + order.ProductName, "ProcessingMessage");

                    receivedMessage.Complete();
                }
                catch (Exception ex)
                {
                    // Handle any message processing specific exceptions here  
                }
            });

            CompletedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections   
            ServicePointManager.DefaultConnectionLimit = 4;

            string connectionString = CloudConfigurationManager.GetSetting
                                        ("Microsoft.ServiceBus.ConnectionString");

            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);

            return base.OnStart();
        }

        public override void OnStop()
        {
            Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}