using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UtahCrashesCracked.Models;
using UtahCrashesCracked.Models.ViewModels;

namespace UtahCrashesCracked.Controllers
{
    public class HomeController : Controller
    {
        private CrashDbContext _context { get; set; }
        public HomeController(CrashDbContext temp)
        {
            _context = temp;   
        }

        public IActionResult Index( int pageNum = 1)
        {
            var blah = _context.crashes
                .FromSqlRaw("select * from crashes where crash_severity_id = 5")
                .ToList();

            return View(blah);
        }

        public IActionResult Crashes(int pageNum = 1)
        {
            int pageSize = 25;

            var x = new CrashesViewModel
            {
                Crashes = _context.crashes
                .OrderBy(c => c.crash_datetime)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes = _context.crashes.Count(),
                    CrashesPerPage = pageSize,
                    CurrentPage = pageNum
                }
            };

            return View(x);
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

        public IActionResult Seatbelts()
        {
            return View();
        }
        public IActionResult DrunkDrowsyDist()
        {
            return View();
        }
        [HttpGet]
        public IActionResult NewCrash()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewCrash(Crash c)
        {
            if (ModelState.IsValid)
            {
                _context.Add(c);
                _context.SaveChanges();

                return View("Confirmation", c);
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        public IActionResult Edit(int crashid)
        {

            var crash = _context.crashes.Single(x => x.crash_id == crashid);

            return View("NewCrash", crash);
        }
        [HttpPost]
        public IActionResult Edit (Crash c)
        {
            _context.Update(c);
            _context.SaveChanges();

            return View("Confirmation");
        }
        [HttpGet]
        public IActionResult Delete(int crashid)
        {
            var crash = _context.crashes.Single(x => x.crash_id == crashid);
            return View(crash);
        }
        [HttpPost]
        public IActionResult Delete (Crash c)
        {
            _context.crashes.Remove(c);
            _context.SaveChanges();

            return View("Confirmation");

        }

        public IActionResult Summary (int crashid)
        {
            var crash = _context.crashes.Single(x => x.crash_id == crashid);
            return View(crash);
        }
    }
}
