using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace OffSite.Abstraction.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string WatchrersId { get; set; }

        public double PaidDaysOff { get; set; }
    }
}
