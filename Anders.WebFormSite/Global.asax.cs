using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Anders.WebFormSite
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.Ignore("{resource}.axd/{*pathInfo}");

            routes.MapPageRoute("home",
                "",
                "~/index.aspx",
                true,
                null);
        }
    }
}