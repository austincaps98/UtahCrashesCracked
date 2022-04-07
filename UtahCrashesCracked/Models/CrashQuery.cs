using System;
namespace UtahCrashesCracked.Models.ViewModels
{
    public class CrashQuery
    {
        public string SearchQuery { get; set; }

        public string County { get; set; }

        public DateTime Date { get; set; }

        public int Severity { get; set; }
    }
}
