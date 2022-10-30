using RojakJelah.Database;
using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext("server=localhost;user=root;database=xx;port=3306;password=******");

            // Hide notification
            notification.Style.Add("display", "none");

            // Get user input
            string username = txtUsername.Value.Trim();
            string password = txtPassword.Value.Trim();

            // Get all users in database
            List<User> existingUsers = dataContext.Users.ToList();

            // Error message
            bool hasError = false;
            string errorTitle = "";
            string errorMessage = "";

            // Validate inputs
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
            {
                // Empty input fields
                hasError = true;
                errorTitle = "Invalid input";
                errorMessage = "Input fields must not be empty.";
            }
            else if ((existingUsers != null) && (!existingUsers.Any(x => x.Username == username)))
            {
                // Username does not exist
                hasError = true;
                errorTitle = "Invalid input";
                errorMessage = "This username does not exist.";
            }
            else
            {
                // Password verification
                string existingPasswordHash = existingUsers.SingleOrDefault(x => x.Username == username).Password;
                bool isCorrectPassword = PasswordHasher.Verify(password, existingPasswordHash, 100000);

                // Incorrect password
                if (!isCorrectPassword)
                {
                    hasError = true;
                    errorTitle = "Invalid input";
                    errorMessage = "Username or password is incorrect.";
                }
            }

            // Check for errors
            if (hasError)
            {
                // Display error notification
                notificationTitle.InnerText = errorTitle;
                notificationMessage.InnerHtml = errorMessage;
                notification.Style.Add("display", "block");
            }
            else
            {
                // Log the user into the system
                

                // Redirect user to Translator page
                Response.Redirect("Default.aspx", true);
            }
        }
    }
}