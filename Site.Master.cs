using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Page.User.Identity.IsAuthenticated)
            {
                // Hide Login button and show Logout button
                lnkLogin.Visible = false;
                lnkLogout.Visible = true;
            }
        }

        protected void LnkLogout_Click(object sender, EventArgs e)
        {
            // Log the user out
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("Login.aspx");
        }
    }
}