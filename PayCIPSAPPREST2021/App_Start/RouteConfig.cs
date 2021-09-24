using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PayCIPSAPPREST2021
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{TerminalSerialNumber}",
                defaults: new { controller = "Home", action = "Index", TerminalSerialNumber = UrlParameter.Optional }
            );
        }
    }
}
