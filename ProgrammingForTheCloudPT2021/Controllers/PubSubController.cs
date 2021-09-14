using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProgrammingForTheCloudPT2021.DataAccess.Interfaces;
using ProgrammingForTheCloudPT2021.Models;

namespace ProgrammingForTheCloudPT2021.Controllers
{
    public class PubSubController : Controller
    {
        private IPubSubAccess _pubSubAccess;
        private ILogAccess _log;
        public PubSubController(IPubSubAccess pubSubAccess, ILogAccess log)
        {
            _log = log;
            _pubSubAccess = pubSubAccess;
        }

        public async Task<IActionResult> SendMail(string category)
        {
            _log.Log("Pushing a mail into a queue");

            var d = DateTime.Now.ToShortTimeString();
            MyMailMessage mm = new MyMailMessage
            {
                Body = "This is a test body for pub sub " + d,
                To = "ryanattarddemo@gmail.com"
            };

            await  _pubSubAccess.PublishEmail(mm, category);
            _log.Log("Pushed mail message onto queue. Data: " + JsonConvert.SerializeObject(mm));

            return Content("done "+ d);
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
