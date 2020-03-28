using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BugTracker
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            routes.MapRoute(
                "ProjectIssues",
                "issues/project/{id}",
                new { Controller = "Issues", action = "ProjectView", id = UrlParameter.Optional, resolved = false }
                );

            routes.MapRoute(
                "ProjectIssuesResolved",
                "issues/project/resolved/{id}",
                new { Controller = "Issues", action = "ProjectView", id = UrlParameter.Optional, resolved = true }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
