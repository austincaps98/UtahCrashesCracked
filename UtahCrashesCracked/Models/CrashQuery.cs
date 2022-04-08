using System;
using System.ComponentModel.DataAnnotations;

namespace UtahCrashesCracked.Models.ViewModels
{
    public class CrashQuery
    {
        public string SearchQuery { get; set; }

        public string County { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public int Severity { get; set; }
    }
}
