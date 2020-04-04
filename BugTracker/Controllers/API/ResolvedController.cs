using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BugTracker.Controllers.API
{
    public class ResolvedController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //get resolved issues for given year
        [HttpGet]
        public IQueryable<Issue> Get(int id)
        {
            var issues = db.Issues.Where(model => model.Resolved == true 
                && model.DateCreated.Year == id);
            return issues;
        }
    }
}
