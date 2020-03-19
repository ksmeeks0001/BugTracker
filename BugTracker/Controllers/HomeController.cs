using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();        

        public ActionResult Index()
        {
            //example how to get user data
            var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = UserManager.GetRoles(User.Identity.GetUserId());
            foreach (var role in roles)
            {
                if (role == RoleNames.Admin)
                    return RedirectToAction("Index","Admin");
            }
            return View(); 
        }


        public ActionResult Help()
        {
            ViewBag.Message = "This is the help page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}