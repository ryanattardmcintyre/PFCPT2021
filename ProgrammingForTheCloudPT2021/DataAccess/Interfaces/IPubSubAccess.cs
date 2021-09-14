using ProgrammingForTheCloudPT2021.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProgrammingForTheCloudPT2021.DataAccess.Interfaces
{
    public interface IPubSubAccess
    {
        Task<string> PublishEmail(MyMailMessage mail, string category);
        Task<MailMessageWithAckId> ReadEmail(); //FIFO 

        void AcknowledgeMessage(string ackId);
    }
}
