using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OffSite.Abstraction.Entities;
using OffSite.Web.Utils;
using OffSite.Web.Utils.Constants;
using System.Security.Claims;
using static OffSite.Web.Utils.Constants.Constants;

namespace OffSite.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var currentUser = this.GetCurrentUser();
            var isAdmin = this.GetCurrentUserIsInAdminRole();

            if (currentUser == null)
            {
                SetCurrentUser(context.HttpContext.User, context.HttpContext.Session);
            }
        }

        protected void SetCurrentUser(ClaimsPrincipal user, ISession sesion)
        {
            bool isAdmin = false;

            ApplicationUser currentUser = _userManager.GetUserAsync(user).Result;
            if (currentUser != null)
            {
                isAdmin = _userManager.IsInRoleAsync(currentUser, Roles.AdminRole).Result;
            }

            // Add to Session.
            Utils.SessionExtensions.Set<ApplicationUser>(sesion, Global.CurrentUser, currentUser);
            sesion.Set<bool>(Global.IsCurrentUserIsInAdminRole, isAdmin);
        }

        protected ApplicationUser GetCurrentUser()
        {
            return Utils.SessionExtensions.Get<ApplicationUser>(HttpContext.Session, Constants.Global.CurrentUser);
        }

        protected bool GetCurrentUserIsInAdminRole()
        {
            return HttpContext.Session.Get<bool>(Constants.Global.IsCurrentUserIsInAdminRole);
        }
    }
}