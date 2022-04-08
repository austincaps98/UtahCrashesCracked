using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NinjaNye.SearchExtensions;
using UtahCrashesCracked.Models;
using UtahCrashesCracked.Models.ViewModels;

namespace UtahCrashesCracked.Controllers
{
    public class HomeController : Controller
    {
        private CrashDbContext _context { get; set; }

        private InferenceSession _session;

        public HomeController(CrashDbContext temp, InferenceSession session)
        {
            _context = temp;
            _session = session;
        }

        public IActionResult Index()
        {
            //var blah = _context.crashes
            //    .FromSqlRaw("select * from crashes where crash_severity_id = 5")
            //    .ToList();

            //return View(blah);
            return View();
        }


        [HttpGet]
        public IActionResult Crashes(string county, int pageNum = 1)
        {
            int pageSize = 25;

            ViewBag.Counties = _context.crashes
                .Where(x => x.county_name != "")
                .Select(x => x.county_name)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var x = new CrashesViewModel
            {
                Crashes = _context.crashes
                .Where(c => c.county_name == county || county == null)
                .OrderBy(c => c.crash_datetime)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes = (county == null
                    ? _context.crashes.Count()
                    : _context.crashes.Where(x => x.county_name == county).Count()),
                    CrashesPerPage = pageSize,
                    CurrentPage = pageNum,
                },

                FilterBool = new FilterBool
                {
                    filterEmpty = true
                }
            };

            return View(x);
        }

        [HttpPost]
        public IActionResult Crashes(CrashesViewModel model, int pageNum = 1)
        {
            int pageSize = 25;

            ViewBag.Counties = _context.crashes
                .Where(x => x.county_name != "")
                .Select(x => x.county_name)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            string query = model.CrashQuery.SearchQuery;

            string county = model.CrashQuery.County;

            DateTime date = model.CrashQuery.Date;

            int severity = model.CrashQuery.Severity;

            var x = new CrashesViewModel
            {
                Crashes = _context.crashes
                .Where((c => c.city.Contains(query) || c.main_road_name.Contains(query) || query == null))
                .Where(c => c.county_name == county || county == null)
                .Where(c => c.crash_datetime.Date == date.Date || date.ToString() == "01/01/0001 00:00:00")
                .Where(c => c.crash_severity_id == severity || severity == 0)
                .OrderBy(c => c.crash_datetime)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes = (model.CrashQuery == null
                    ? _context.crashes.Count()
                    : _context.crashes
                    .Where((c => c.city.Contains(query) || c.main_road_name.Contains(query) || query == null))
                    .Where(c => c.county_name == county || county == null)
                    .Where(c => c.crash_datetime.Date == date.Date || date.ToString() == "01/01/0001 00:00:00")
                    .Where(c => c.crash_severity_id == severity || severity == 0)
                    .Count()),
                    CrashesPerPage = pageSize,
                    CurrentPage = pageNum
                },

                FilterBool = new FilterBool
                {
                    filterEmpty = false
                }
            };

            return View(x);
        }


        public IActionResult Summaries()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult DrunkDrowsyDist()
        {
            var data = new InputData
            {pedestrian_involved = 0,bicyclist_involved = 0,motorcycle_involved = 0,improper_restraint = 0,unrestrained = 0,dui = 0,intersection_related = 0,
                overturn_rollover = 0,older_driver_involved = 0,single_vehicle = 0,distracted_driving = 0,drowsy_driving = 0,roadway_departure = 0,city_SALT_LAKE_CITY = 0};
            //Normal Driving 
            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            Tensor<float> score = result.First().AsTensor<float>();
            var prediction = new Prediction { PredictedValue = score.First() };
            ViewBag.normaldriver = Convert.ToString(Math.Round(prediction.PredictedValue));
            result.Dispose();

            //Distracted
            data.distracted_driving = 1;
            result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            score = result.First().AsTensor<float>();
            prediction = new Prediction { PredictedValue = score.First() };
            ViewBag.distracted_driving = (Convert.ToString(Math.Round(prediction.PredictedValue)));
            result.Dispose();

            //Drowsy
            data.distracted_driving = 0;
            data.drowsy_driving = 1;
            result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            score = result.First().AsTensor<float>();
            prediction = new Prediction { PredictedValue = score.First() };
            ViewBag.drowsy_driving = Convert.ToString(Math.Round(prediction.PredictedValue));
            result.Dispose();

            //DUI
            data.drowsy_driving = 0;
            data.dui = 1;
            result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            score = result.First().AsTensor<float>();
            prediction = new Prediction { PredictedValue = score.First() };
            ViewBag.dui = Convert.ToString(Math.Round(prediction.PredictedValue));
            result.Dispose();

            return View();
        }

        public ActionResult Seatbelts()
        {
            var data = new InputData
            {
                pedestrian_involved = 0,
                bicyclist_involved = 0,
                motorcycle_involved = 0,
                improper_restraint = 0,
                unrestrained = 0,
                dui = 0,
                intersection_related = 0,
                overturn_rollover = 0,
                older_driver_involved = 0,
                single_vehicle = 0,
                distracted_driving = 0,
                drowsy_driving = 0,
                roadway_departure = 0,
                city_SALT_LAKE_CITY = 0
            };
            //Seatbelt
            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            Tensor<float> score = result.First().AsTensor<float>();
            var prediction = new Prediction { PredictedValue = score.First() };
            ViewBag.seatbelt = Convert.ToString(Math.Round(prediction.PredictedValue));
            result.Dispose();

            //No Seatbelt
            data.unrestrained = 1;
            result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            score = result.First().AsTensor<float>();
            prediction = new Prediction { PredictedValue = score.First() };
            ViewBag.noseatbelt = (Convert.ToString(Math.Round(prediction.PredictedValue)));
            result.Dispose();

            //Improper seatbelt
            data.unrestrained = 0;
            data.improper_restraint = 1;
            result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            score = result.First().AsTensor<float>();
            prediction = new Prediction { PredictedValue = score.First() };
            ViewBag.improper_restraint = Convert.ToString(Math.Round(prediction.PredictedValue));
            result.Dispose();

            return View("Seatbelts");
        }
        [HttpGet]
        public IActionResult NewCrash()
        {
            ViewBag.Counties = _context.crashes
                .Where(x => x.county_name != "")
                .Select(x => x.county_name)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

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
            ViewBag.Counties = _context.crashes
                .Where(x => x.county_name != "")
                .Select(x => x.county_name)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

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

        public IActionResult Privacy ()
        {
            return View();
        }
    }
}
