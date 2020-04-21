using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Utilities
{
    public static class SharedFunctions
    {
        
        
        public static IQueryable<ApplicationUser> GetAllManagers(ApplicationDbContext db, int organizationId)
        {
            string manRoleId = (db.Roles.SingleOrDefault(r => r.Name == RoleNames.ProjectManager)).Id;
            var managers = from user in db.Users
                           where user.OrganizationId == organizationId
                           && user.Roles.Any(r => r.RoleId == manRoleId)
                           select user;
            return managers;
        }

        public static IQueryable<ApplicationUser> GetAllDevelopers(ApplicationDbContext db, int organizationId)
        {
            string devRoleId = (db.Roles.SingleOrDefault(r => r.Name == RoleNames.Developer)).Id;
            var developers = from user in db.Users
                             where user.OrganizationId == organizationId
                             && user.Roles.Any(r => r.RoleId == devRoleId)
                             select user;
            return developers;
        }

        public static bool UserOnProject(HttpContextBase HttpContext, ApplicationDbContext db, Project project)
        {
            var userId = HttpContext.User.Identity.GetUserId();

            //check if developer is on project
            if (HttpContext.User.IsInRole(RoleNames.Developer))
            {
                var ProjDev = db.ProjectDevelopers.SingleOrDefault(model => model.ProjectId == project.Id
                && model.DeveloperId == userId);

                if (ProjDev == null)
                    return false;
            }

            //check if manager is over project
            else if (HttpContext.User.IsInRole(RoleNames.ProjectManager) && project.ManagerId != userId)
                return false;

            //admin not in project organization
            else if (HttpContext.User.IsInRole(RoleNames.Admin) && project.OrganizationId != db.Users.Find(userId).OrganizationId)
                return false;

            //user on project
            return true;
        }

        public static bool UserOnProject(HttpContextBase HttpContext, ApplicationDbContext db, Issue issue)
        {
            //returns false if project does not exist or user not on project
            if (HttpContext.User.IsInRole(RoleNames.Developer))
            {
                //if project exists where user is manager or a developer
                var userId = HttpContext.User.Identity.GetUserId();
                var projectdev = db.ProjectDevelopers.Where(
                    model => model.ProjectId == issue.ProjectId &&
                             model.DeveloperId == userId );

                if (projectdev == null)
                    return false;
            }
            else if (HttpContext.User.IsInRole(RoleNames.ProjectManager))
            {
                var userId = HttpContext.User.Identity.GetUserId();
                if (issue.Project.ManagerId != userId)
                    return false;
            }

            //else the user is Admin
            else if (HttpContext.User.IsInRole(RoleNames.Admin))
            {
                var organizationId = db.Users.Find(HttpContext.User.Identity.GetUserId()).OrganizationId;
                if (issue.Project == null)
                {
                    var project = db.Projects.Find(issue.ProjectId);
                    if (project.OrganizationId != organizationId)
                        return false;
                }
                else if (issue.Project.OrganizationId != organizationId)
                    return false;
                    
            }

            return true;
        }
    }
}