using ProgrammingForTheCloudPT2021.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Google.Protobuf;
using System.Threading;
using Grpc.Core;
using ProgrammingForTheCloudPT2021.Models;

namespace ProgrammingForTheCloudPT2021.DataAccess.Repositories
{

    

    public class PubSubAccess : IPubSubAccess
    {
        private string projectId;
        public PubSubAccess(IConfiguration config)
        {
            projectId = config.GetSection("ProjectId").Value;
        }

        public async Task<string> PublishEmail(MyMailMessage mail)
        {

            //queue = topic
            TopicName topic = new TopicName(projectId, "myQueue2");

            PublisherClient client = await  PublisherClient.CreateAsync(topic);

            
            string mail_serialized = JsonConvert.SerializeObject(mail);

            PubsubMessage message = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(mail_serialized),
                //OrderingKey = "1",
                Attributes =
                {
                    { "category" , "admin"}
                }
            };

             return  await client.PublishAsync(message);

        }

        public async Task<MailMessageWithAckId> ReadEmail()
        {

            SubscriptionName subName = new SubscriptionName(projectId, "myQueue2-sub");
            SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
            int messageCount = 0;
            MailMessageWithAckId mmWithAckId = null;
            try
            {
                // Pull messages from server,
                // allowing an immediate response if there are no messages.
                PullResponse response = await subscriberClient.PullAsync(subName, returnImmediately: true, maxMessages: 1);
                // Print out each received message.

                if (response.ReceivedMessages.Count > 0)
                {
                    string text = response.ReceivedMessages[0].Message.Data.ToStringUtf8();

                    var mm = JsonConvert.DeserializeObject<MyMailMessage>(text);

                    mmWithAckId = new MailMessageWithAckId
                    {
                        MM = mm,
                        AckId = response.ReceivedMessages[0].AckId
                    };
                }
                else return null;
                
                // If acknowledgement required, send to server.
            /*    if (acknowledge && messageCount > 0)
                {
                    subscriberClient.Acknowledge(subscriptionName, response.ReceivedMessages.Select(msg => msg.AckId));
                }*/
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
            {
                // UNAVAILABLE due to too many concurrent pull requests pending for the given subscription.
            }

            return mmWithAckId;

        }


        public void AcknowledgeMessage(string ackId)
        {
            SubscriptionName subName = new SubscriptionName(projectId, "myQueue2-sub");
            SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
            int messageCount = 0;
       
            try
            {
               
                // If acknowledgement required, send to server.
                     
                  subscriberClient.Acknowledge(subName, new List<string>() { ackId });
                    
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
            {
                // UNAVAILABLE due to too many concurrent pull requests pending for the given subscription.
            }
            

        }
    }
}
