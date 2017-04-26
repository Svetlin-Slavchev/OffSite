using System;

namespace OffSite.Data.ViewModels
{
    public class NotificationMessageViewModel
    {
        public int Id { get; set; }
        public OffSiteRequestViewModel SiteOffRequest { get; set; }
        public bool Approved { get; set; }
        public bool ApprovedDone { get; set; }
        public ApplicationUserViewModel SelectedUser { get; set; }
        public double DaysOff { get; set; }
    }
}
