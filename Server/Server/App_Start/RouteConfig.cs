using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Server
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "NewPhone",
                url: "newphone",
                defaults: new { controller = "Home", action = "NewPhone" }
            );

            routes.MapRoute(
                name: "UploadPicture",
                url: "uploadpicture",
                defaults: new { controller = "Home", action = "UploadPicture" }
            );

            routes.MapRoute(
                name: "PhoneAddConnection",
                url: "phoneaddconnection",
                defaults: new { controller = "Home", action = "PhoneAddConnection" }
            );

            routes.MapRoute(
                name: "NewDesktop",
                url: "newdesktop",
                defaults: new { controller = "Home", action = "NewDesktop" }
            );


            routes.MapRoute(
                name: "DesktopAlive",
                url: "desktopalive",
                defaults: new { controller = "Home", action = "DesktopAlive" }
            );

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
