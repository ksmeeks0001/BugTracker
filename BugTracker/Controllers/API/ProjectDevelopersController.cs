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

namespace BugTracker.Controllers.API
{
    public class ProjectDevelopersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

 
        //developers on given project
        // GET: api/ProjectDevelopers/5
        [ResponseType(typeof(ProjectDeveloper))]
        public IHttpActionResult GetProjectDeveloper(int id)
        {
            var projectDevelopers = db.ProjectDevelopers.Include(model => model.Developer).Where(model => model.ProjectId == id);
            

            return Ok(projectDevelopers.ToList());
        }




        //here down is scaffolded that needs modifications and implementation


        // PUT: api/ProjectDevelopers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProjectDeveloper(int id, ProjectDeveloper projectDeveloper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != projectDeveloper.ProjectId)
            {
                return BadRequest();
            }

            db.Entry(projectDeveloper).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectDeveloperExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ProjectDevelopers
        [ResponseType(typeof(ProjectDeveloper))]
        public IHttpActionResult PostProjectDeveloper(ProjectDeveloper projectDeveloper)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.ProjectDevelopers.Add(projectDeveloper);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ProjectDeveloperExists(projectDeveloper.ProjectId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = projectDeveloper.ProjectId }, projectDeveloper);
        }


        // DELETE: api/ProjectDevelopers/5
        [ResponseType(typeof(ProjectDeveloper))]
        public IHttpActionResult DeleteProjectDeveloper(int id)
        {
            ProjectDeveloper projectDeveloper = db.ProjectDevelopers.Find(id);
            if (projectDeveloper == null)
            {
                return NotFound();
            }

            db.ProjectDevelopers.Remove(projectDeveloper);
            db.SaveChanges();

            return Ok(projectDeveloper);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProjectDeveloperExists(int id)
        {
            return db.ProjectDevelopers.Count(e => e.ProjectId == id) > 0;
        }
    }
}