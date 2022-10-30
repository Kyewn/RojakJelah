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
            if (!IsPostBack)
            {
                if (Page.User.Identity.IsAuthenticated)
                {
                    lnkLogin.Visible = false;
                    lnkLogout.Visible = true;
                    lnkLogout.Text = "Logout \"" + Page.User.Identity.Name + "\"";
                }
            }
        }

        protected void LnkLogout_Click(object sender, EventArgs e)
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("Login.aspx");
        }
    }
}