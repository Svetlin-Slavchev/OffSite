using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OffSite.Abstraction.Entities;
using OffSite.Abstraction.Interfaces;
using OffSite.Data.ViewModels;
using OffSite.Web.Data;
using OffSite.Web.Utils.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffSite.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IDashboardService _dashboard;

        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IDashboardService dashboard)
            : base(userManager)
        {
            _userManager = userManager;
            _context = context;
            _dashboard = dashboard;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser currentUser = base.GetCurrentUser();

            DashboardViewModel model = new DashboardViewModel();
            model.RemainingPaidDaysOff = currentUser.PaidDaysOff;
            model.ApproversAndWatchers = await _dashboard.GetApprovers(currentUser.WatchrersId, _userManager);

            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }

        #region helpers
        public async Task<IEnumerable<CalendarData>> GetCalendarData()
        {
            return await _context.OffSiteRequests
                    .Include(x => x.Status)
                    .Include(x => x.User)
                    .Where(x => x.Approved)
                    .Select(x => new CalendarData()
                    {
                        title = string.Format("{0}{1} : {2}", x.Status.Name, x.IsHalfADayRequest ? "(half a day)" : "", x.User.UserName),
                        start = x.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"),
                        end = x.EndDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"),
                        color = SetCalendarColor(x.Status.Id)
                    })
                    .ToListAsync();
        }

        private string SetCalendarColor(int id)
        {
            string color;
            switch (id)
            {
                case 1: color = "#378006"; break;
                case 2: color = "#FF0000"; break;
                case 3: color = "#FF5733"; break;
                case 4: color = "#FFAF33"; break;
                case 5: color = "#33E6FF"; break;
                case 6: color = "#3396FF"; break;
                case 7: color = "#3342FF"; break;
                case 8: color = "#7A33FF"; break;
                case 9: color = "#C433FF"; break;
                case 10: color = "#FF33DA"; break;
                default:
                    color = "#33FFAC";
                    break;
            }

            return color;
        }

        #endregion
    }

    public class CalendarData
    {
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string description { get; set; }
        public string color { get; set; }
    }
}
