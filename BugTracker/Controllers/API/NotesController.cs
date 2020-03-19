using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace BugTracker.Controllers.API
{
    
    public class NotesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public IHttpActionResult CreateNote(Note note)
        {
            if (note == null)
                return BadRequest();
            Issue issue = db.Issues.SingleOrDefault(model => model.Id == note.IssueId);
            if (issue == null)
                return BadRequest();

            ModelState.Remove("Id"); // Key removal
            ModelState.Remove("DateCreated");
            if (!ModelState.IsValid)
                return BadRequest();
            
            note.DateCreated = DateTime.Today;
            issue.DateUpdated = DateTime.Today;
            db.Notes.Add(note);
            db.SaveChanges();

            return Ok(note);
        }
            
       [HttpDelete]
       public IHttpActionResult DeleteNote(int id)
        {
            Note note = db.Notes.SingleOrDefault(model => model.Id == id);
            if (note == null)
                return BadRequest();
            db.Notes.Remove(note);
            db.SaveChanges();
            return Ok();
        }
    }
}
