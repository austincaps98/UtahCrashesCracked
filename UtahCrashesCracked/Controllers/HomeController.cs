using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
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
            var data = new InputData { pedestrian_involved = 0,
                bicyclist_involved= 0,
                motorcycle_involved= 0,
                improper_restraint= 0,
                unrestrained= 0,
                dui= 0,
                intersection_related= 0,
                overturn_rollover= 0,
                older_driver_involved= 0,
                single_vehicle= 0,
                distracted_driving= 0,
                drowsy_driving= 0,
                roadway_departure= 0,
                city_SALT_LAKE_CITY= 0 };
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
    }
}
