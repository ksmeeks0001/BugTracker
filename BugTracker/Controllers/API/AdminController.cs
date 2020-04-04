using BugTracker.Models;
using BugTracker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BugTracker.Controllers.API
{
    [Authorize(Roles = RoleNames.Admin)]
    public class AdminController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpDelete]
        public IHttpActionResult DeletePending(int? id)
        {
            if (id == null)
                return BadRequest();

            var pending = db.PendingRegistrations.SingleOrDefault(p => p.Id == id);
            if (pending == null)
                return NotFound();

            db.PendingRegistrations.Remove(pending);
            db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult ResendPendingEmail(int id)
        {
            var pending = db.PendingRegistrations.SingleOrDefault(model => model.Id == id);
            if (pending == null)
                return BadRequest();
            try
            {
                Email.SendPendingEmail(pending);
            }
            catch
            {
                return NotFound();
            }
            return Ok();

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
