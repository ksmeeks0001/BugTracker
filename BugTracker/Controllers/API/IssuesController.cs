using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BugTracker.Controllers.API
{
    public class IssuesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Method not currently used
        public IHttpActionResult GetNotes(int id)
        {
            Issue issue = db.Issues.Include("Notes").SingleOrDefault(m => m.Id == id);
            if (issue == null)
                return BadRequest();

            return Ok(issue);
        }
    }
}
