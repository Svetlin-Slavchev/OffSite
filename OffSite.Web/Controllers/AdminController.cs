using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using OffSite.Abstraction.Entities;
using OffSite.Data.ViewModels;
using OffSite.Web.Data;
using OffSite.Web.Utils.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffSite.Web.Controllers
{
    [Authorize(Roles = Constants.Roles.AdminRole)]
    public class AdminController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> manager,
            RoleManager<IdentityRole> roleManager) : base(manager)
        {
            _context = context;
            _userManager = manager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var allUsers = await _context.Users.ToListAsync();

            // For later.
            //var watcherRole = _roleManager.Roles
            //    .FirstOrDefault(x => x.Name == Constants.Roles.WatcherRole);

            //var approverRole = _roleManager.Roles
            //    .FirstOrDefault(x => x.Name == Constants.Roles.ApproverRole);

            // Todo - Must be used cache.
            IList<ApplicationUser> watchers = new List<ApplicationUser>();
            IList<ApplicationUser> approvers = new List<ApplicationUser>();
            foreach (var item in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(item);
                foreach (var role in userRoles)
                {
                    if (role == Constants.Roles.WatcherRole)
                    {
                        watchers.Add(item);
                    }
                    else if (role == Constants.Roles.ApproverRole)
                    {
                        approvers.Add(item);
                    }
                }
            }

            ViewData["AllUsers"] = allUsers;
            ViewData["Watchers"] = watchers;
            ViewData["Approvers"] = approvers;
            ViewData["AllRoles"] = await _context.Roles.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToRole(UserToRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Error!";
                return View("Index");
            }

            // Get form data.
            StringValues userId = Request.Form["UserId"];
            StringValues rolesId = Request.Form["RolesId"];

            // Todo - get it form cache.
            var allUsers = await _context.Users.ToListAsync();
            var allRoles = await _context.Roles.ToListAsync();

            // Get requested user.
            var requestedUser = allUsers
                .FirstOrDefault(x => x.Id.ToString() == userId);

            IdentityRole role;
            foreach (var item in rolesId)
            {
                role = allRoles
                    .FirstOrDefault(x => x.Id.ToString() == item);

                if (!await _userManager.IsInRoleAsync(requestedUser, role.Name))
                {
                    await _userManager.AddToRoleAsync(requestedUser, role.Name);
                }
            }

            TempData["Message"] = "All done!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WatchersApproversToUser(WatchersApproversToUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Error!";
                return View("Index");
            }

            // Get form data.
            StringValues watcherId = Request.Form["WatchersId"];
            StringValues approversId = Request.Form["ApproversId"]; 
            StringValues userId = Request.Form["UserId"];

            // Todo - get it form cache.
            var allUsers = await _context.Users.ToListAsync();

            // Get requested user.
            var requestedUser = allUsers
                .FirstOrDefault(x => x.Id.ToString() == userId);

            ApplicationUser watcher;
            foreach (var item in watcherId)
            {
                watcher = allUsers
                    .FirstOrDefault(x => x.Id.ToString() == item);

                // Update user.
                requestedUser.WatchrersId = string.Format("{0},{1}", requestedUser.WatchrersId, watcher.Id);
                await _userManager.UpdateAsync(requestedUser);
            }

            ApplicationUser approver;
            foreach (var item in approversId)
            {
                approver = allUsers
                    .FirstOrDefault(x => x.Id.ToString() == item);

                // Update user.
                requestedUser.WatchrersId = string.Format("{0},{1}", requestedUser.WatchrersId, approver.Id);
                await _userManager.UpdateAsync(requestedUser);
            }

            TempData["Message"] = "All done!";
            return RedirectToAction("Index");
        }
    }
}