using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Utilities
{
    public class Utilities
    {
        
        public static IQueryable<ApplicationUser> GetAllManagers(ApplicationDbContext db)
        {
            string manRoleId = (db.Roles.SingleOrDefault(r => r.Name == RoleNames.ProjectManager)).Id;
            var managers = from user in db.Users
                           where user.Roles.Any(r => r.RoleId == manRoleId)
                           select user;
            return managers;
        }

        public static IQueryable<ApplicationUser> GetAllDevelopers(ApplicationDbContext db)
        {
            string devRoleId = (db.Roles.SingleOrDefault(r => r.Name == RoleNames.Developer)).Id;
            var developers = from user in db.Users
                             where user.Roles.Any(r => r.RoleId == devRoleId)
                             select user;
            return developers;
        }
    }
}