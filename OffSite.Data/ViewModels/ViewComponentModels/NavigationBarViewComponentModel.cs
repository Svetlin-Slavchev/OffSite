using OffSite.Data.Entities;
using System.Collections.Generic;

namespace OffSite.Data.ViewModels.ViewComponentModels
{
    public class NavigationBarViewComponentModel : NavigationViewComponentModel
    {
        public IEnumerable<NotificationMessage> ApproverNotifications { get; set; }
        public IEnumerable<OffSiteRequest> PendingRequests { get; set; }
        public int AllNotifications { get; set; }
    }
}
