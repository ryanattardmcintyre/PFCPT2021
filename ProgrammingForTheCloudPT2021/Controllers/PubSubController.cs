using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProgrammingForTheCloudPT2021.DataAccess.Interfaces;
using ProgrammingForTheCloudPT2021.Models;

namespace ProgrammingForTheCloudPT2021.Controllers
{
    public class PubSubController : Controller
    {
        private IPubSubAccess _pubSubAccess;
        public PubSubController(IPubSubAccess pubSubAccess)
        {
            _pubSubAccess = pubSubAccess;
        }

        public async Task<IActionResult> SendMail()
        {
            MyMailMessage mm = new MyMailMessage
            {
                Body = "This is a test body for pub sub " + DateTime.Now.ToShortTimeString(),
                To = "ryanattard@gmail.com"
            };

            await  _pubSubAccess.PublishEmail(mm);

            return Content("done");
        }


        public async Task<ActionResult> ReadEmail()
        {
            //process that is going to send out the email

            var result = await _pubSubAccess.ReadEmail();

            if (result != null)
            {
                string returnedResult = $"To: {result.MM.To},Body: {result.MM.Body}, AckId: {result.AckId}";
                //the above line can be replaced with sending out the actual email using some smtp server or mail gun api
                return Content(returnedResult);
            }
            else
            {
                return Content("no emails read");
            }
        }


        public  ActionResult  AcknowledgeMessage(string ackId)
        {
            _pubSubAccess.AcknowledgeMessage(ackId);

            return Content("done");
        }
    }
}
