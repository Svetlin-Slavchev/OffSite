using Microsoft.AspNetCore.Identity;
using OffSite.Abstraction.Entities;
using OffSite.Abstraction.Interfaces;
using OffSite.Abstraction.ViewModels;
using OffSite.Data.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffSite.Data.Services
{
    public class DashboardService : IDashboardService
    {
        public async Task<IList<IApplicationUserViewModel>> GetApprovers(string watchrersId, UserManager<ApplicationUser> userManager)
        {
            if (string.IsNullOrEmpty(watchrersId))
            {
                return new List<IApplicationUserViewModel>();
            }

            IList<IApplicationUserViewModel> models = new List<IApplicationUserViewModel>();

            // Get and distinct the ids.
            var distinctedIds = watchrersId.Split(new char[] { ',' }).Distinct();

            ApplicationUser watcher;
            foreach (var item in distinctedIds)
            {
                watcher = await userManager.FindByIdAsync(item);
                if (watcher != null)
                {
                    // Get roles.
                    IList<string> roles = await userManager.GetRolesAsync(watcher);

                    IApplicationUserViewModel model = new ApplicationUserViewModel(watcher.Id, watcher.UserName, watcher.Email, roles);

                    // Add to list.
                    models.Add(model);
                }
            }

            return models;
        }
    }
}
