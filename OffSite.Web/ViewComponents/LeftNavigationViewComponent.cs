using Microsoft.AspNetCore.Mvc;
using OffSite.Abstraction.Entities;
using OffSite.Data.Entities;
using OffSite.Data.ViewModels.ViewComponentModels;
using OffSite.Web.Utils;
using static OffSite.Web.Utils.Constants.Constants;

namespace OffSite.Web.ViewComponents
{
    public class LeftNavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var currentUser = SessionExtensions.Get<ApplicationUser>(HttpContext.Session, Global.CurrentUser);
            var isAdmin = HttpContext.Session.Get<bool>(Global.IsCurrentUserIsInAdminRole);

            LeftNavigationViewComponentModel model = new LeftNavigationViewComponentModel();
            model.CurrentUser = currentUser;
            model.IsAdmin = isAdmin;

            return View("_LeftNavigation", model);
        }
    }
}
