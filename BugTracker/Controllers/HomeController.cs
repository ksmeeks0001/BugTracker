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
    
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();        

        public ActionResult Index()
        {

            if (User.IsInRole(RoleNames.Admin))
                return RedirectToAction("Index", "Admin");

            else if (User.Identity.IsAuthenticated)
                return View();

            //if user is not logged in
            else return RedirectToAction("LandingPage");
        }

        public ActionResult LandingPage()
        {
            
            return View();
        }

        [Authorize]
        public ActionResult Help()
        {
            ViewBag.Message = "This is the help page.";

            return View();
        }


    }
}