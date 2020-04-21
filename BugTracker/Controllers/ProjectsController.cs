using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using BugTracker.Utilities;

namespace BugTracker.Controllers
{
    
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            IQueryable<Project> projects;

            if (User.IsInRole(RoleNames.Developer))
            {
                projects = (from projectdev in db.ProjectDevelopers
                                where projectdev.DeveloperId == userId
                                select projectdev.Project).Include(p => p.Manager)
                                .Include(p => p.Issues);
                projects.Where(p => p.Complete == false);
                return View("DeveloperIndex", projects.ToList());  
            }
            if (User.IsInRole(RoleNames.ProjectManager))
            {
                projects = db.Projects.Where(p => p.ManagerId == userId && p.Complete == false)
                    .Include(p => p.Issues);
                return View("ManagerIndex", projects.ToList());
            }
            //else admin
            var user = db.Users.Find(User.Identity.GetUserId());
            projects = db.Projects.Include(p => p.Issues).Include(p => p.Manager)
                .Where(p => p.Complete == false && p.OrganizationId == user.OrganizationId);    
            return View("AdminIndex",projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Include(p=> p.Manager).SingleOrDefault(p => p.Id == id);
            
            if (!SharedFunctions.UserOnProject(this.HttpContext, db, project))
                return HttpNotFound();

         
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles =RoleNames.Admin)]
        public ActionResult Create()
        {

            //get Role = Project Manager
            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            var managers = SharedFunctions.GetAllManagers(db, organizationId);

            ViewBag.ManagerId = new SelectList(managers, "Id", "UserName"); 

            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ActionResult Create([Bind(Include = "Id,Name,Details,ManagerId")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.DateStarted = DateTime.Today;
                project.Complete = false;
                project.OrganizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
            }
            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            var managers = SharedFunctions.GetAllManagers(db, organizationId);
            ViewBag.ManagerId = new SelectList(managers, "Id", "Email", project.ManagerId);
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = RoleNames.Admin)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Project project = db.Projects.Find(id);
            
            if (project == null)
                return HttpNotFound();

            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            var managers = SharedFunctions.GetAllManagers(db, organizationId);
            ViewBag.ManagerId = new SelectList(managers, "Id", "Email", project.ManagerId);
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                var organizationTemp = project.OrganizationId;
                db.Entry(project).State = EntityState.Modified;
                project.OrganizationId = organizationTemp;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            var managers = SharedFunctions.GetAllManagers(db, organizationId);
            ViewBag.ManagerId = new SelectList(managers, "Id", "Email", project.ManagerId);
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager)]
        public ActionResult MarkComplete(int id)
        {
            var project = db.Projects.SingleOrDefault(p => p.Id == id);

            if (project == null)
                return HttpNotFound();

            if (!SharedFunctions.UserOnProject(this.HttpContext, db, project))
                return HttpNotFound();

            project.Complete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager)]
        public ActionResult AddDevelopers(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            //exclude devs already on project
            var users = SharedFunctions.GetAllDevelopers(db, organizationId).Where(model =>  (from projectDev in db.ProjectDevelopers
                                                                              where projectDev.ProjectId == id
                                                                              select projectDev.DeveloperId).Contains(model.Id)
                                                                                 );

            return View(users.ToList());
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager)]
        public ActionResult AddDevelopers(int? id, List<string> devIds)
        {
            if (id == null)
                return HttpNotFound();

            //project exists and if manager user on project
            var project = db.Projects.Find(id);

            if (!SharedFunctions.UserOnProject(this.HttpContext, db, project))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //verify all users exist and are developers
            var devRole = db.Roles.Single(model => model.Name == RoleNames.Developer);
            var addUsers = db.Users.Where(model => devIds.Contains(model.Id) && model.Roles.Any(r => r.RoleId == devRole.Id)).ToList();
            
            if (addUsers.Count == devIds.Count)
            {
                foreach (var addUser in addUsers)
                {
                    db.ProjectDevelopers.Add(new ProjectDeveloper()
                    {
                        ProjectId = project.Id,
                        DeveloperId = addUser.Id
                    });
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id = id });
            }     
            else
            {
                //exclude devs already on project
                var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
                var users = SharedFunctions.GetAllDevelopers(db, organizationId).Where(model => !(from projectDev in db.ProjectDevelopers
                                                                               where projectDev.ProjectId == id
                                                                               select projectDev.DeveloperId).Contains(model.Id));

                 return View(users.ToList());
             }

        }

        // GET: Projects/Delete/5
        [Authorize(Roles=RoleNames.Admin)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Project project = db.Projects.Find(id);
            
            if (project == null)
                return HttpNotFound();

            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            if (project.OrganizationId != organizationId)
                return HttpNotFound();

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            
            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            if (project.OrganizationId != organizationId)
                return HttpNotFound();
            
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
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
