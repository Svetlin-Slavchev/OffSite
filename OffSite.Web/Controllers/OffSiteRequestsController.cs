using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OffSite.Abstraction.Entities;
using OffSite.Abstraction.Interfaces;
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
    public class OffSiteRequestsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _messageSender;
        private readonly IHostingEnvironment _hostingEnvironment;

        public OffSiteRequestsController(ApplicationDbContext context, UserManager<ApplicationUser> manager,
            IEmailSender messageSender, IHostingEnvironment hostingEnvironment) : base(manager)
        {
            _context = context;
            _userManager = manager;
            _messageSender = messageSender;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: VacationRequests
        public async Task<IActionResult> Index()
        {
            // Get current user.
            var currentUser = base.GetCurrentUser();
            var isInAdminRole = base.GetCurrentUserIsInAdminRole();
            IList<OffSiteRequestViewModel> requestModels = new List<OffSiteRequestViewModel>();

            IList<OffSiteRequest> offsiteRequest;
            if (isInAdminRole)
            {
                offsiteRequest = await _context.OffSiteRequests
                    .Include(x => x.Status)
                    .Include(x => x.User)
                    .ToListAsync();
                this.MapRequests(offsiteRequest, requestModels);
            }

            // Get only messages releated to current user.
            offsiteRequest = await _context.OffSiteRequests
                .Include(x => x.Status)
                .Include(x => x.User)
                .Where(x => x.UserFkId == currentUser.Id)
                .ToListAsync();

            this.MapRequests(offsiteRequest, requestModels);
            ViewBag.IsAdmin = isInAdminRole;

            return View(requestModels);
        }

        private void MapRequests(IList<OffSiteRequest> offsiteRequest, IList<OffSiteRequestViewModel> requestModels)
        {
            foreach (var item in offsiteRequest)
            {
                OffSiteRequestViewModel model = new OffSiteRequestViewModel();
                model.Id = item.Id;
                model.CreatedDate = item.CreatedDate;
                model.StartDate = item.StartDate;
                model.EndDate = item.EndDate;
                model.Reason = item.Reason;
                model.UserName = item.User.UserName;
                model.Approved = item.Approved;
                model.SelectedStatusName = item.Status.Name;

                requestModels.Add(model);
            }
        }

        // GET: VacationRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacationRequest = await _context.OffSiteRequests
                .Include(x => x.Status)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (vacationRequest == null)
            {
                return NotFound();
            }

            ViewBag.IsAdmin = HttpContext.Session.Get<bool>(Constants.Global.IsCurrentUserIsInAdminRole);

            return View(vacationRequest);
        }

        // GET: VacationRequests/Create
        public async Task<IActionResult> Create()
        {
            // todo - get from cache.
            ViewData["RequestStatus"] = await _context.OffSiteStatuses.ToListAsync();
            return View();
        }

        // POST: VacationRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OffSiteRequestViewModel siteOffRequest)
        {
            // Validate model.
            if (siteOffRequest.TakeHalfADay)
            {
                siteOffRequest.EndDate = siteOffRequest.StartDate.AddHours(4);
                ModelState.Remove("EndDate");
            }

            if (ModelState.IsValid)
            {
                // Get current user.
                var currentUser = base.GetCurrentUser();

                // Todo - get from Cache.
                OffSiteStatus status = await _context.OffSiteStatuses
                    .FirstOrDefaultAsync(x => x.Id == siteOffRequest.SelectedVacationStatusId);

                // Check if current user have enoghth paid days off.
                // Todo - find better way to do this - it is hard coded!
                if (status.Name == Constants.Global.PaidVacationName || status.Name == Constants.Global.NonPaidVacationName)
                {
                    // Get current user remaining days.
                    double remainingPaidDaysOff = currentUser.PaidDaysOff;
                    double currentRequestedDaysOff;
                    if (siteOffRequest.TakeHalfADay)
                    {
                        currentRequestedDaysOff = 0.5d;
                    }
                    else
                    {
                        // Calculate real days off.
                        currentRequestedDaysOff = Helpers.GetWorkingDays(siteOffRequest.StartDate, siteOffRequest.EndDate);
                    }

                    if (currentRequestedDaysOff > remainingPaidDaysOff)
                    {
                        TempData["ErrorMessage"] = "Error! You do not have enought paid days off!";

                        // todo - get from cache.
                        ViewData["RequestStatus"] = await _context.OffSiteStatuses.ToListAsync();
                        return View(siteOffRequest);
                    }
                }

                OffSiteRequest entity = new OffSiteRequest(status, siteOffRequest.StartDate, 
                    siteOffRequest.EndDate, siteOffRequest.Reason, currentUser.Id, siteOffRequest.TakeHalfADay);

                _context.Add(entity);
                await _context.SaveChangesAsync();

                // Get all watchers ids.
                var watchersField = currentUser.WatchrersId;
                if (watchersField != null)
                {
                    // Get and distinct the ids.
                    var watchresIds = currentUser.WatchrersId.Split(new char[] { ',' }).Distinct();

                    // Get current vacation request id.
                    var currentRequest = _context.OffSiteRequests
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefault();
                    int currentRequestId = currentRequest.Id;

                    NotificationMessage message;
                    ApplicationUser watcher;
                    bool isApproved = false;
                    bool approvedDone = false;

                    // Todo - for review.
                    foreach (var item in watchresIds)
                    {
                        watcher = await _userManager.FindByIdAsync(item);
                        if (watcher != null)
                        {
                            if (await _userManager.IsInRoleAsync(watcher, Constants.Roles.WatcherRole))
                            {
                                isApproved = true;
                                approvedDone = true;
                            }

                            message = new NotificationMessage(currentRequest.Id, currentUser.Id, watcher.Id, isApproved, approvedDone);
                            _context.Add(message);

                            // Save to db.
                            await _context.SaveChangesAsync();

                            var currentnotificationMessage = _context.NotificationMessages
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault();

                            // Set default values.
                            isApproved = false;
                            approvedDone = false;

                            // Send email to all Watchers.
                            string emailTemplatePath = _hostingEnvironment.ContentRootPath + "\\Templates\\Off.SiteRequest.html";
                            string messageBody = _messageSender.GetBody(emailTemplatePath);
                            string requestLink = string.Format("{0}://{1}/{2}/{3}", HttpContext.Request.Scheme,
                                HttpContext.Request.Host.Value, "NotificationMessages/DetailsForApprovement", currentnotificationMessage.Id);
                            string buildTemplate = _messageSender.EmailTemplate(messageBody, watcher, requestLink);

                            _messageSender.SendEmailAsync(watcher.Email, "Off.Site request", buildTemplate);
                        }
                    }
                }

                TempData["Message"] = "All done!";

                return RedirectToAction("Index", "Home");
            }

            return View(siteOffRequest);
        }

        // GET: VacationRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacationRequest = await _context.OffSiteRequests.SingleOrDefaultAsync(m => m.Id == id);
            if (vacationRequest == null)
            {
                return NotFound();
            }
            return View(vacationRequest);
        }

        // POST: VacationRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Approved,CreatedDate,EndDate,Reason,StartDate")] OffSiteRequest vacationRequest)
        {
            if (id != vacationRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacationRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacationRequestExists(vacationRequest.Id))
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
            return View(vacationRequest);
        }

        // GET: VacationRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacationRequest = await _context.OffSiteRequests.SingleOrDefaultAsync(m => m.Id == id);
            if (vacationRequest == null)
            {
                return NotFound();
            }

            return View(vacationRequest);
        }

        // POST: VacationRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacationRequest = await _context.OffSiteRequests.SingleOrDefaultAsync(m => m.Id == id);
            _context.OffSiteRequests.Remove(vacationRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool VacationRequestExists(int id)
        {
            return _context.OffSiteRequests.Any(e => e.Id == id);
        }
    }
}
