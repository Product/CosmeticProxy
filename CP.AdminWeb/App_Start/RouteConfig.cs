using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace CP.AdminWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GetPic",
                url: "{controller}/{action}/{fileid}/{size}/",
                defaults: new
                {
                    controller = "Product",
                    action = "GetPic",
                    fileid = RouteParameter.Optional,
                    size = RouteParameter.Optional
                }
            );
        }
    }
}