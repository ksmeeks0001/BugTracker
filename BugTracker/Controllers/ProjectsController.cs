﻿using System;
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
            projects = db.Projects.Include(p => p.Issues).Include(p => p.Manager)
                .Where(p => p.Complete == false);    
            return View("AdminIndex",projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var project = db.Projects.Include(p=> p.Manager).SingleOrDefault(p => p.Id == id);
            
            if (project == null)
                return HttpNotFound();

         
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles =RoleNames.Admin + "," + RoleNames.ProjectManager)]
        public ActionResult Create()
        {

            //get Role = Project Manager
            var managers = Utilities.Utilities.GetAllManagers(db);

            ViewBag.ManagerId = new SelectList(managers, "Id", "UserName"); 

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Details,ManagerId")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.DateStarted = DateTime.Today;
                project.Complete = false;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
            }
            var managers = Utilities.Utilities.GetAllManagers(db);
            ViewBag.ManagerId = new SelectList(managers, "Id", "Email", project.ManagerId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            var managers = Utilities.Utilities.GetAllManagers(db);
            ViewBag.ManagerId = new SelectList(managers, "Id", "Email", project.ManagerId);
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var managers = Utilities.Utilities.GetAllManagers(db);
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
            var userId = User.Identity.GetUserId();

            if (User.IsInRole(RoleNames.ProjectManager) && project.ManagerId != userId )
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            project.Complete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager)]
        public ActionResult AddDevelopers(int? id)
        {
            if (id == null)
                return HttpNotFound();

            //exclude devs already on project
            var users = Utilities.Utilities.GetAllDevelopers(db).Where(model => !(from projectDev in db.ProjectDevelopers
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
            var userId = User.Identity.GetUserId();
            if (project == null || (User.IsInRole(RoleNames.ProjectManager) && project.ManagerId != userId))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            foreach(var devId in devIds)
            {
                if (db.Users.Find(devId) != null)
                    db.ProjectDevelopers.Add(new ProjectDeveloper()
                    {
                        ProjectId = project.Id,
                        DeveloperId = devId
                    });
                else
                {
                    //exclude devs already on project
                    var users = Utilities.Utilities.GetAllDevelopers(db).Where(model => !(from projectDev in db.ProjectDevelopers
                                                                                          where projectDev.ProjectId == id
                                                                                          select projectDev.DeveloperId).Contains(model.Id)
                                                                                         );
                    return View(users.ToList());
                }
                    
            }

            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
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

        private bool UserOnProject(Project project)
        {
            var userId = User.Identity.GetUserId();

            //check if developer is on project
            if (User.IsInRole(RoleNames.Developer))
            {
                var ProjDev = db.ProjectDevelopers.SingleOrDefault(model => model.ProjectId == project.Id
                && model.DeveloperId == userId);

                if (ProjDev == null)
                    return false;
            }

            //check if manager is over project
            else if (User.IsInRole(RoleNames.ProjectManager) && project.ManagerId != userId)
                return false;

            //user on project or admin
            return true;
        }
    }
}
