using Microsoft.AspNetCore.Identity;
using OffSite.Abstraction.Entities;
using OffSite.Abstraction.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OffSite.Abstraction.Interfaces
{
    public interface IDashboardService
    {
        Task<IList<IApplicationUserViewModel>> GetApprovers(string watchrersId, UserManager<ApplicationUser> userManager);
    }
}
