using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using RojakJelah.Database;


namespace RojakJelah
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            //  Handle nav menu items and adding active class to them
            HandleNavMenu();

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
            } else
            {
                var path = HttpContext.Current.Request.Url.AbsolutePath;
                if (path.Contains("/Suggestions") || path.Contains("/Reports"))
                {
                    Response.Redirect("Translator.aspx");
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

        protected void HandleNavMenu()
        {
            // Toggle visibility
            string path = HttpContext.Current.Request.Url.AbsolutePath;

            if (path.Contains("/Suggestions") || path.Contains("/Reports"))
            {
                lnkBack.Style.Add("display", "flex");
                lnkSuggestions.Style.Add("display", "flex");
                lnkReports.Style.Add("display", "flex");
                lnkTranslator.Style.Add("display", "none");
                lnkDictionary.Style.Add("display", "none");
                lnkAbout.Style.Add("display", "none");
                lnkAdminPanel.Style.Add("display", "none");
            } else
            {
                lnkBack.Style.Add("display", "none");
                lnkSuggestions.Style.Add("display", "none");
                lnkReports.Style.Add("display", "none");
                lnkTranslator.Style.Add("display", "flex");
                lnkDictionary.Style.Add("display", "flex");
                lnkAbout.Style.Add("display", "flex");
                lnkAdminPanel.Style.Add("display", "flex");
            }

            // Add active class
            switch (path)
            {
                case "/Translator":
                    lnkTranslator.CssClass = "scroll-link active";
                    break;
                case "/Dictionary":
                    lnkDictionary.CssClass += "scroll-link active";
                    break;
                case "/About":
                    lnkAbout.CssClass += "scroll-link active";
                    break;
                case "/Suggestions":
                    lnkSuggestions.CssClass += "scroll-link active";
                    break;
                case "/Reports":
                    lnkReports.CssClass += "scroll-link active";
                    break;
            }
        }
    }
}