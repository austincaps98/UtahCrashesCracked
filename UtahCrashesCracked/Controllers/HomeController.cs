using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UtahCrashesCracked.Models;

namespace UtahCrashesCracked.Controllers
{
    public class HomeController : Controller
    {
        private DataDbContext _context { get; set; }
        public HomeController(DataDbContext temp)
        {
            _context = temp;   
        }

        public IActionResult Index()
        {
            var blah = _context.data
                .FromSqlRaw("select * from crashes where crash_severity_id = 5")
                .ToList();

            return View(blah);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
