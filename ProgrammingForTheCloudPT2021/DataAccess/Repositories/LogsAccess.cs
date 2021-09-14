using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Logging.V2;
using Google.Cloud.Logging.Type;
using Microsoft.Extensions.Configuration;
using Google.Api;
using ProgrammingForTheCloudPT2021.DataAccess.Interfaces;

namespace ProgrammingForTheCloudPT2021.DataAccess.Repositories
{
    public class LogsAccess: ILogAccess
    {
        private string projectId;
        public LogsAccess(IConfiguration config)
        {
            projectId = config.GetSection("ProjectId").Value;
        }
        public void Log(string message)
        {

            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName(projectId, "mainapplication");
            LogEntry logEntry = new LogEntry
            {
                LogName = logName.ToString(),
                Severity = LogSeverity.Info,
                TextPayload = $"{message}"
            };
            MonitoredResource resource = new MonitoredResource { Type = "global" };
            //IDictionary<string, string> entryLabels = new Dictionary<string, string>
            //    {
            //        { "size", "large" },
            //        { "color", "red" }
            //    };
            client.WriteLogEntries(logName, resource, null,
                new[] { logEntry });
           
        }
    }
}
