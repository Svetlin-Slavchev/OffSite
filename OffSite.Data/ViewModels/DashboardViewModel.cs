using OffSite.Abstraction.ViewModels;
using System.Collections.Generic;

namespace OffSite.Data.ViewModels
{
    public class DashboardViewModel
    {
        public double RemainingPaidDaysOff { get; set; }

        public IList<IApplicationUserViewModel> ApproversAndWatchers { get; set; }
    }
}
