using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OffSite.Abstraction.Entities;
using OffSite.Data.Entities;
using OffSite.Data.ViewModels;
using OffSite.Web.Data;
using OffSite.Web.Utils;
using OffSite.Web.Utils.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffSite.Web.Controllers
{
    [Authorize]
    public class NotificationMessagesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationMessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: NotificationMessages
        public async Task<IActionResult> Index()
        {
            // Get current user.
            var currentUser = base.GetCurrentUser();
            var isInAdminRole = base.GetCurrentUserIsInAdminRole();
            IList<NotificationMessageViewModel> notificationModels = new List<NotificationMessageViewModel>();

            IList<NotificationMessage> notificationMessages;
            if (isInAdminRole)
            {
                notificationMessages = await _context.NotificationMessages
                    .Include(x => x.SelectedUser)
                    .Include(x => x.OffSiteRequest)
                    .Include(x => x.OffSiteRequest.Status)
                    .ToListAsync();

                this.MapNotifications(notificationMessages, notificationModels);
            }

            // Get only messages releated to current user.
            notificationMessages = await _context.NotificationMessages
                .Include(x => x.SelectedUser)
                .Include(x => x.OffSiteRequest)
                .Include(x => x.OffSiteRequest.Status)
                .Where(x => x.ApproverUser.Id == currentUser.Id)
                .ToListAsync();

            this.MapNotifications(notificationMessages, notificationModels);
            ViewData[Constants.Global.IsCurrentUserIsInAdminRole] = isInAdminRole;

            return View(notificationModels);
        }

        private void MapNotifications(IList<NotificationMessage> notificationMessages, IList<NotificationMessageViewModel> notificationModels)
        {
            foreach (var item in notificationMessages)
            {
                NotificationMessageViewModel model = new NotificationMessageViewModel();
                model.Id = item.Id;
                model.SelectedUser = new ApplicationUserViewModel();
                model.SelectedUser.Name = item.SelectedUser.UserName;
                model.SelectedUser.Email = item.SelectedUser.Email;

                model.SiteOffRequest = new OffSiteRequestViewModel();
                model.SiteOffRequest.SelectedStatusName = item.OffSiteRequest.Status.Name;
                model.SiteOffRequest.StartDate = item.OffSiteRequest.StartDate;
                model.SiteOffRequest.EndDate = item.OffSiteRequest.EndDate;
                model.SiteOffRequest.Reason = item.OffSiteRequest.Reason;
                model.Approved = item.Approved;
                model.ApprovedDone = item.ApprovedDone;

                model.DaysOff = Helpers.GetWorkingDays(model.SiteOffRequest.StartDate, model.SiteOffRequest.EndDate);

                notificationModels.Add(model);
            }
        }

        [Authorize(Roles = "Admin, Approver")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificationMessage = await _context.NotificationMessages
                .SingleOrDefaultAsync(m => m.Id == id);

            if (notificationMessage == null)
            {
                return NotFound();
            }

            return View(notificationMessage);
        }

        [Authorize(Roles = "Admin, Approver")]
        public async Task<IActionResult> DetailsForApprovement(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificationMessage = await _context.NotificationMessages
                .Include(x => x.SelectedUser)
                .Include(x => x.OffSiteRequest)
                .Include(x => x.OffSiteRequest.Status)
                .SingleOrDefaultAsync(m => m.Id == id);

            // If curent user is not authorize to approve selects notification message, set 403 access denied.
            if (notificationMessage.ApproverUserFkId != base.GetCurrentUser().Id)
            {
                return StatusCode(403);
            }

            if (notificationMessage == null)
            {
                return NotFound();
            }

            NotificationMessageViewModel model = new NotificationMessageViewModel();
            model.Id = notificationMessage.Id;
            model.SelectedUser = new ApplicationUserViewModel();
            model.SelectedUser.Name = notificationMessage.SelectedUser.UserName;
            model.SelectedUser.Email = notificationMessage.SelectedUser.Email;

            model.SiteOffRequest = new OffSiteRequestViewModel();
            model.SiteOffRequest.SelectedStatusName = notificationMessage.OffSiteRequest.Status.Name;
            model.SiteOffRequest.StartDate = notificationMessage.OffSiteRequest.StartDate;
            model.SiteOffRequest.EndDate = notificationMessage.OffSiteRequest.EndDate;
            model.SiteOffRequest.Reason = notificationMessage.OffSiteRequest.Reason;

            model.DaysOff = Helpers.GetWorkingDays(model.SiteOffRequest.StartDate, model.SiteOffRequest.EndDate);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsForApprovement(NotificationMessageViewModel model)
        {
            NotificationMessage notificationMessage = await _context.NotificationMessages
                .Include(x => x.OffSiteRequest)
                .SingleOrDefaultAsync(m => m.Id == model.Id);

            if (notificationMessage == null)
            {
                TempData["ErrorMessage"] = "Error!";
                return NotFound();
            }

            // Save to db.
            notificationMessage.Approved = model.Approved;
            notificationMessage.ApprovedDone = true;
            await _context.SaveChangesAsync();

            // Check if all is approved for current site.off request.
            var allNotificationsForCurrentRequest = _context.NotificationMessages
                .Include(x => x.OffSiteRequest)
                .Where(x => x.OffSiteRequest.Id == notificationMessage.OffSiteRequest.Id);

            bool isAllApproved = true;
            foreach (var item in allNotificationsForCurrentRequest)
            {
                if (!item.Approved && item.ApprovedDone)
                {
                    isAllApproved = false;
                }
                else if (!item.ApprovedDone)
                {
                    isAllApproved = false;
                }
            }

            if (isAllApproved)
            {
                OffSiteRequest request = _context.OffSiteRequests
                    .Include(x => x.Status)
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.Id == notificationMessage.OffSiteRequest.Id);

                if (request == null)
                {
                    return NotFound();
                }

                request.Approved = true;

                if (request.Status.Name == Constants.Global.PaidVacationName || request.Status.Name == Constants.Global.NonPaidVacationName)
                {
                    // Get requested user.
                    var currentUser = await _userManager.FindByIdAsync(request.User.Id);

                    // Calculate paid working days.
                    double remainingPaidDaysOff = currentUser.PaidDaysOff;
                    double currentRequestedDaysOff = Helpers.GetWorkingDays(request.StartDate, request.EndDate);

                    // If request in half day.
                    if (request.IsHalfADayRequest)
                    {
                        currentRequestedDaysOff = 0.5;
                    }

                    // Update current user days off.
                    currentUser.PaidDaysOff -= currentRequestedDaysOff;
                    await _userManager.UpdateAsync(currentUser);
                }

                await _context.SaveChangesAsync();

                // Todo - send email.
            }

            TempData["Message"] = "All done!";
            return RedirectToAction("Index");
        }

        // GET: NotificationMessages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificationMessage = await _context.NotificationMessages.SingleOrDefaultAsync(m => m.Id == id);
            if (notificationMessage == null)
            {
                return NotFound();
            }
            return View(notificationMessage);
        }

        // POST: NotificationMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Approved")] NotificationMessage notificationMessage)
        {
            if (id != notificationMessage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Todo
                    _context.Update(notificationMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationMessageExists(notificationMessage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(notificationMessage);
        }

        // GET: NotificationMessages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificationMessage = await _context.NotificationMessages.SingleOrDefaultAsync(m => m.Id == id);
            if (notificationMessage == null)
            {
                return NotFound();
            }

            return View(notificationMessage);
        }

        // POST: NotificationMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notificationMessage = await _context.NotificationMessages.SingleOrDefaultAsync(m => m.Id == id);
            _context.NotificationMessages.Remove(notificationMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool NotificationMessageExists(int id)
        {
            return _context.NotificationMessages.Any(e => e.Id == id);
        }
    }
}
