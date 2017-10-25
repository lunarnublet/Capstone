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
                   name: "GetPhoto",
                   url: "getphoto",
                   defaults: new { controller = "Home", action = "GetPhoto" }
               );

               routes.MapRoute(
                    name: "GetPhotoLength",
                    url: "getphotolength",
                    defaults: new { controller = "Home", action = "GetPhotoLength" }
               );

               routes.MapRoute(
                   name: "UploadPhoto",
                   url: "uploadphoto",
                   defaults: new { controller = "Home", action = "UploadPhoto" }
               );

               routes.MapRoute(
                   name: "AppendPhoto",
                   url: "appendphoto",
                   defaults: new { controller = "Home", action = "AppendPhoto" }
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
