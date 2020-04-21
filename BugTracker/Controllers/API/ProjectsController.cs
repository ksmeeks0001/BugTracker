using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers.API
{
    [Authorize]
    public class ProjectsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Projects
        public IQueryable<Project> GetProjects()
        {
            var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
            return db.Projects.Where(model => model.OrganizationId == organizationId);
        }

        // GET: api/Projects/5
        [ResponseType(typeof(Project))]
        public IHttpActionResult GetProject(int id)
        {
            Project project = db.Projects.Include(model => model.Issues).SingleOrDefault(model => model.Id == id);
            if (project == null)
                return NotFound();
            if (!UserOnProject(project))
                return BadRequest();
                       
            return Ok(project);
        }



        /*
        // DELETE: api/Projects/5
        [ResponseType(typeof(Project))]
        public IHttpActionResult DeleteProject(int id)
        {
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }

            db.Projects.Remove(project);
            db.SaveChanges();

            return Ok(project);
        }
        */

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

            else if (User.IsInRole(RoleNames.Admin))
            {
                var organizationId = db.Users.Find(User.Identity.GetUserId()).OrganizationId;
                if (project.OrganizationId != organizationId)
                    return false;
            }
            //user on project
            return true;
        }
    }
}