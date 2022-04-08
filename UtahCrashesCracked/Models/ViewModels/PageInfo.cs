using System;
namespace UtahCrashesCracked.Models.ViewModels
{
    public class PageInfo
    {
        public int TotalNumCrashes { get; set; }
        public int CrashesPerPage { get; set; }
        public int CurrentPage { get; set; }

        // Calculate number of pages needed
        public int TotalPages => (int)Math.Ceiling((double)TotalNumCrashes / CrashesPerPage);

    }
}
