using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Anders.WebFormSite
{
    public partial class Secure : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var cp = User as ClaimsPrincipal;

            var claims = cp.Claims.Select(c => new { name = c.Type, value = c.Value });

            rptClaims.DataSource = claims;
            rptClaims.DataBind();
        }

        protected void signout_Click(object sender, EventArgs e)
        {
            HttpContext
                .Current
                .GetOwinContext()
                .Authentication
                .SignOut();
        }
    }
}