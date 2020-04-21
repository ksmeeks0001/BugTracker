using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PendingUsers()
        {
            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            var pending = db.PendingRegistrations.Where(model => model.OrganizationId == organizationId).ToList();
            return View(pending);
        }
        
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
               db.Dispose();
            }

            base.Dispose(disposing);
        }


    }
}