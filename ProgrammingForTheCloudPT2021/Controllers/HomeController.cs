using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProgrammingForTheCloudPT2021.Models;

namespace ProgrammingForTheCloudPT2021.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExceptionLogger _exceptionlogger;

        public HomeController(ILogger<HomeController> logger, [FromServices] IExceptionLogger exceptionLogger)
        {
            _logger = logger;
            _exceptionlogger = exceptionLogger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            try
            {
                throw new Exception("Caught Error on purpose");
            }
            catch(Exception ex)
            {
                _exceptionlogger.Log(ex);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
