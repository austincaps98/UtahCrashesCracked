﻿using System;
using System.Linq;

namespace UtahCrashesCracked.Models.ViewModels
{
    public class CrashesViewModel
    {
        public IQueryable<Crash> Crashes { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
