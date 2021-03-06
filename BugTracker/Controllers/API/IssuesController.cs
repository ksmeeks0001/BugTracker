﻿using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BugTracker.Controllers.API
{
    public class IssuesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        
        [HttpGet]
        public List<Issue> GetIssues()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var issues = db.Issues.Where(model => model.Resolved == false && 
                model.Project.OrganizationId == user.OrganizationId);
            return issues.ToList();
        }


        [HttpGet]
        public IHttpActionResult GetNotes(int id)
        {
            Issue issue = db.Issues.Include("Notes").SingleOrDefault(m => m.Id == id);
            if (issue == null)
                return BadRequest();

            return Ok(issue);
        }
        

        //POST add DeveloperId to assign developer to issue not currently working
        [HttpPost]
        public IHttpActionResult AssignDeveloper([FromBody]int? IssueId,[FromBody] string DeveloperId)
        {
            if (IssueId == null || DeveloperId == null)
                return BadRequest();

            var issue = db.Issues.Find(IssueId);

            if (issue == null)
                return BadRequest();

            var projDev = db.ProjectDevelopers.Where(model => model.ProjectId == issue.ProjectId && model.DeveloperId == DeveloperId);

            if (projDev == null)
                return BadRequest();

            issue.DeveloperAssignedId = DeveloperId;
            issue.DateUpdated = DateTime.Today;
            db.SaveChanges();
            return Ok();
        }

    }
}
