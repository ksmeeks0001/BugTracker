﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BugTracker.Views
{
    public class IssuesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            
            if (User.IsInRole(RoleNames.Developer))
            {
                //all issues assigned to developer
                
                var issues = db.Issues.Include(model => model.Project).
                    Where(model => model.DeveloperAssignedId == userId &&
                            model.Resolved == false).OrderBy(model => model.DateUpdated);

                ViewBag.Title = "Assigned Issues";
                return View(issues.ToList());
            }
            else if (User.IsInRole(RoleNames.ProjectManager))
            {
                //open issues for project where user is manager
                var projects = db.Projects.Where(model => model.ManagerId == userId).
                    Include(model => model.Issues);
                var issues = new List<Issue>();
                foreach (var project in projects)
                {
                    issues.AddRange(project.Issues.Where(model => model.Resolved == false).
                        OrderBy(model => model.DateCreated).ToList());
                }
                ViewBag.Title = "Open Issues";
                return View(issues);
            }
            else
            {
                //user is Admin all open issues
                var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;

                var issues = db.Issues.Include(model => model.Project).
                    Where(model => model.Resolved == false && model.Project.OrganizationId == organizationId).OrderBy(model => model.DateCreated);
                ViewBag.Title = "All Open Issues";
                return View(issues);
            }
                
            
        }

        public ActionResult Open()
        {
            
            var userId = User.Identity.GetUserId();
            
            if (User.IsInRole(RoleNames.Developer))
            {
                //all issues for all projects where developer is on team
                var projectDevelopers = db.ProjectDevelopers.Where(model => model.DeveloperId == userId)
                    .Include("Project.Issues");
                var issues = new List<Issue>();
                foreach (var projectDev in projectDevelopers)
                {
                    issues.AddRange(projectDev.Project.Issues.Where(model => model.Resolved == false).ToList());
                }
                ViewBag.Title = "Open Issues";
                return View("Index", issues.OrderBy(model => model.DateUpdated));
            }
            //if in a role other than developer than Index is equivalent
            return View("Index");
        }

        // GET: Issues/project/id
        public ActionResult ProjectView(int id, bool resolved)
        {
            //all issues for given project
            var project = db.Projects.Include(model => model.Issues).SingleOrDefault(p => p.Id == id);

            if (project == null)
                return HttpNotFound();

            if (!SharedFunctions.UserOnProject(this.HttpContext, db, project))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (resolved)
                ViewBag.Title = "Resolved ";
            else
                ViewBag.Title = "Open ";
            
            ViewBag.Title += "Issues for Project " + project.Name;
            ViewBag.ProjectView = true;
            ViewBag.ProjectId = project.Id;
            
            var issues = project.Issues.Where(model => model.Resolved == resolved && model.ProjectId == project.Id).
                OrderBy(model => model.DateCreated).ToList();
            
            return View("Index", issues);
        }
        
        // GET: Issues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Issue issue = db.Issues.Include(model => model.Project.Manager).Include(model => model.DeveloperAssigned).Single( i => i.Id == id);
            
            if (issue == null)
                return HttpNotFound();
            
            if (!SharedFunctions.UserOnProject(this.HttpContext, db, issue))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(issue);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkResolved(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var issue = db.Issues.Include(model => model.Project).Single(model => model.Id == id);
            
            if (issue == null)
                return HttpNotFound();
           
            if (!SharedFunctions.UserOnProject(this.HttpContext, db, issue))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            issue.Resolved = true;
            issue.DateUpdated = DateTime.Today;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Issues/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //init issue with for project
            
            var issue = new Issue()
            {
                ProjectId = (int)id
            };
            return View(issue);
        }

        // POST: Issues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Priority,ProjectId")] Issue issue)
        {
            if (ModelState.IsValid)
            {
                if (!SharedFunctions.UserOnProject(this.HttpContext, db, issue))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                issue.CreatedById = User.Identity.GetUserId();
                issue.DateCreated = DateTime.Today;
                issue.Resolved = false;
                db.Issues.Add(issue);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = issue.Id });
            }

            return View(issue);
        }

        // GET: Issues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Issue issue = db.Issues.Find(id);
            if (issue == null)
                return HttpNotFound();
            
            return View(issue);
        }

        // POST: Issues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Issue issue)
        {
            if (ModelState.IsValid)
            {
                if (!SharedFunctions.UserOnProject(this.HttpContext, db, issue))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                issue.DateUpdated = DateTime.Today;
                issue.DateCreated = (DateTime)issue.DateCreated;
                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = issue.Id});
            }
            return View(issue);
        }

        // GET: Issues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Issue issue = db.Issues.Find(id);
            
            if (issue == null)
                return HttpNotFound();
            
            return View(issue);
        }

        // POST: Issues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Issue issue = db.Issues.Find(id);
            if (!SharedFunctions.UserOnProject(this.HttpContext, db, issue))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            db.Issues.Remove(issue);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //all isues that have been resolved 
        [Authorize(Roles = RoleNames.Admin)]
        public ActionResult Resolved()
        {
            ViewBag.Resolved = true;
            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            return View("Index", db.Issues.Where(model => model.Resolved == true && model.Project.OrganizationId == organizationId)
                .OrderBy(model => model.DateCreated).ToList());
        }

        //POST add DeveloperId to assign developer to issue 
        //needs moved to API controller when probelme is resolved on stack overflow
        [HttpPost]
        public ActionResult AssignDeveloper(int IssueId, string DeveloperId)
        {
            if (DeveloperId == null)
                return HttpNotFound();

            var issue = db.Issues.Find(IssueId);

            if (issue == null)
                return HttpNotFound();

            var projDev = db.ProjectDevelopers.Where(model => model.ProjectId == issue.ProjectId && model.DeveloperId == DeveloperId);

            if (projDev == null)
                return HttpNotFound();

            issue.DeveloperAssignedId = DeveloperId;
            db.SaveChanges();
            return Json("");
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
