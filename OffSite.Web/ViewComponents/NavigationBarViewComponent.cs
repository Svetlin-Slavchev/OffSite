using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OffSite.Abstraction.Entities;
using OffSite.Data.Entities;
using OffSite.Data.ViewModels.ViewComponentModels;
using OffSite.Web.Data;
using OffSite.Web.Utils;
using System.Collections.Generic;
using System.Linq;
using static OffSite.Web.Utils.Constants.Constants;

namespace OffSite.Web.ViewComponents
{
    public class NavigationBarViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NavigationBarViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var currentUser = SessionExtensions.Get<ApplicationUser>(HttpContext.Session, Global.CurrentUser);
            var isAdmin = HttpContext.Session.Get<bool>(Global.IsCurrentUserIsInAdminRole);

            NavigationBarViewComponentModel model = new NavigationBarViewComponentModel();

            model.ApproverNotifications = new List<NotificationMessage>();
            model.PendingRequests = new List<OffSiteRequest>();

            if (currentUser != null)
            {
                // todo - get from cache.
                model.ApproverNotifications = _context.NotificationMessages
                    .Where(x => x.ApproverUser.Id == currentUser.Id && x.ApprovedDone == false);

                // Get pending request for curent user.
                model.PendingRequests = _context.OffSiteRequests
                    .Include(x => x.Status)
                    .Where(x => x.UserFkId == currentUser.Id);

                model.AllNotifications = model.ApproverNotifications.Count() + model.PendingRequests.Count();
                model.CurrentUser = currentUser;
                model.IsAdmin = isAdmin;
            }

            // test
            return View("_NavigationBar", model);
        }
    }
}
