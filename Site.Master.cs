using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Diagnostics;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using RojakJelah.Database;
using RojakJelah.Database.Entity;


namespace RojakJelah
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            //Toggle admin menu and normal menu based on current page
            ToggleNavMenu();

            // Check if user is logged in
            if (Page.User.Identity.IsAuthenticated)
            {
                // Hide Login button and show Logout button
                lnkLogin.Visible = false;
                lnkLogout.Visible = true;

                var currentUser = dataContext.Users.ToList().Find((x) => x.Username == Page.User.Identity.Name);
                var roleList = dataContext.Roles.ToList();
                var systemRole = roleList.Find((x) => x.Id == 1).Name;
                var adminRole = roleList.Find((x) => x.Id == 2).Name;

                // Toggle admin panel if user is admin
                if (currentUser.Role.Name == adminRole || currentUser.Role.Name == systemRole)
                {
                    // Hide Login button and show Logout button
                    lnkAdminPanel.Visible = true;
                }
            }
        }

        protected void LnkLogout_Click(object sender, EventArgs e)
        {
            // Log the user out
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("Login.aspx");
        }

        protected void ToggleNavMenu()
        {
            var links = this.FindControl("navLinks").Controls.OfType<HtmlGenericControl>().ToList();

            string path = HttpContext.Current.Request.Url.AbsolutePath;

            if (path.Contains("/Suggestions") || path.Contains("/Reports"))
            {
                links[0].Style.Add("display", "block");
                links[4].Style.Add("display", "block");
                links[5].Style.Add("display", "block");
                links[1].Style.Add("display", "none");
                links[2].Style.Add("display", "none");
                links[3].Style.Add("display", "none");
                links[6].Style.Add("display", "none");
            } else
            {
                links[0].Style.Add("display", "none");
                links[4].Style.Add("display", "none");
                links[5].Style.Add("display", "none");
                links[1].Style.Add("display", "block");
                links[2].Style.Add("display", "block");
                links[3].Style.Add("display", "block");
                links[6].Style.Add("display", "block");
            }
        }
    }
}