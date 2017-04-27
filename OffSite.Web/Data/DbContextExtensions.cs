using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OffSite.Abstraction.Entities;
using OffSite.Data.Entities;
using OffSite.Web.Utils.Constants;
using System.Collections.Generic;
using System.Linq;

namespace OffSite.Web.Data
{
    public class DbContextExtensions
    {
        public static void Seed(IApplicationBuilder app, ApplicationDbContext dbContext)
        {
            // Initial admin user.
            //this.CreateAdminUser(app);

            // Create SiteOffStatuses.
            DbContextExtensions.CreateOffSiteStatuses(app, dbContext);

            // Create roles.
            DbContextExtensions.CreateRoles(app);
        }

        public static async void CreateRoles(IApplicationBuilder app)
        {
            using (var roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>())
            {
                IdentityRole newRole;
                if (!await roleManager.RoleExistsAsync(Constants.Roles.UserRole))
                {
                    // Assume if we donot have UserRole, we donot nave any roles.
                    // Create the roles.
                    newRole = new IdentityRole(Constants.Roles.UserRole);
                    await roleManager.CreateAsync(newRole);

                    newRole = new IdentityRole(Constants.Roles.AdminRole);
                    await roleManager.CreateAsync(newRole);

                    newRole = new IdentityRole(Constants.Roles.WatcherRole);
                    await roleManager.CreateAsync(newRole);

                    newRole = new IdentityRole(Constants.Roles.ApproverRole);
                    await roleManager.CreateAsync(newRole);
                }
            }
        }

        public static void CreateAdminUser(IApplicationBuilder app)
        {
            using (var userManager = app.ApplicationServices.GetRequiredService<UserManager<ApplicationUser>>())
            {
                // Create admin user.
                var user = new ApplicationUser { UserName = "admin2", Email = "admin2@admin.com" };

                // Check if exist
                // If not, create it and assign it to Admin role.
                var userExist = userManager.FindByNameAsync(user.UserName).Result;
                if (userExist == null)
                {
                    var result = userManager.CreateAsync(user, "A123zxc%").Result;
                    if (result.Succeeded)
                    {
                        if (!userManager.IsInRoleAsync(user, Constants.Roles.AdminRole).Result)
                        {
                            var roleResult = userManager.AddToRoleAsync(user, Constants.Roles.AdminRole).Result;
                        }
                    }
                }
            }
        }

        public static void CreateOffSiteStatuses(IApplicationBuilder app, ApplicationDbContext dbContext)
        {
            if (!dbContext.OffSiteStatuses.Any())
            {
                var offSiteRequests = new List<OffSiteStatus>()
                    {
                        new OffSiteStatus() { Name = Constants.Global.PaidVacationName },
                        new OffSiteStatus() { Name = Constants.Global.NonPaidVacationName },
                        new OffSiteStatus() { Name = "Running late" },
                        new OffSiteStatus() { Name = "Work from home" },
                        new OffSiteStatus() { Name = "Sick leave" },
                        new OffSiteStatus() { Name = "Other" }
                    };

                dbContext.OffSiteStatuses.AddRange(offSiteRequests);
                dbContext.SaveChanges();
            }
        }
    }
}
