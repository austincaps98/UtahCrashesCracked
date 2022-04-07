using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UtahCrashesCracked.Models;

namespace UtahCrashesCracked.Components
{
    public class FiltersViewComponent : ViewComponent
    {
        private CrashDbContext _context { get; set; }

        public FiltersViewComponent (CrashDbContext temp)
        {
            _context = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.selectedCounty = RouteData?.Values["county"];

            var filter = _context.crashes
                .Select(x => x.county_name)
                .Distinct()
                .OrderBy(x => x);

            return View(filter);
        }


    }
}
