using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Notepad
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Define default behavior. Route main page - display entities
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "List" }
            );

           // Route for AJAX entities request
           routes.MapRoute(
               name: "AjaxList",
               url: "Home/AjaxList"
           );

           // Remove entities
           routes.MapRoute(
             name: "AjaxRemove",
             url: "Home/AjaxRemove"
         );

        }
    }
}
