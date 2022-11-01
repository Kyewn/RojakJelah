using RojakJelah.Database;
using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Hide header and footer
            HtmlControl header = Master.FindControl("pageHeader") as HtmlControl;
            HtmlControl footer = Master.FindControl("pageFooter") as HtmlControl;
            header.Visible = false;
            footer.Visible = false;

            if (!IsPostBack)
            {
                if (User.Identity.IsAuthenticated)
                {
                    // Redirect user to Translator page
                    Response.Redirect("Translator.aspx", true);
                }
            }
        }

        protected void BtnRegister_Click(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext("server=localhost;user=root;database=rojakjelahv3;port=3306;password=2020twz05!8MSQL");

            // Hide notification
            notification.Style.Add("display", "none");

            // Get user input
            string username = txtUsername.Value.Trim();
            string password = txtPassword.Value.Trim();
            string confirmPassword = txtConfirmPassword.Value.Trim();

            // Check for existing user
            User existingUser = dataContext.Users.SingleOrDefault(x => x.Username == username) ?? null;

            // Error message
            bool hasError = false;
            string errorTitle = "";
            string errorMessage = "";

            // Validate inputs
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(confirmPassword))
            {
                // Empty input fields
                hasError = true;
                errorTitle = "Invalid input";
                errorMessage = "Input fields must not be empty.";
            }
            else if ((existingUser != null) && (existingUser.Username == username))
            {
                // Duplicate username
                hasError = true;
                errorTitle = "Invalid input";
                errorMessage = "Username must be unique, please try a different username.";
            }
            else if (!password.Equals(confirmPassword))
            {
                // Password and confirm password do not match
                hasError = true;
                errorTitle = "Invalid input";
                errorMessage = "Password must match Confirm Password, please try again.";
            }
            else
            {
                /* Password strength checking
                 * - (?=.*[a-z]): lowercase letters (lookahead)
                 * - (?=.*[A-Z]): uppercase letters (lookahead)
                 * - (?=.*\d): digits (lookahead)
                 * - (?=.*[@$!%*?&]): symbols (lookahead)
                 * - [A-Za-z\d@$!%*?&]: all required charaters (character set)
                 * - {8,}: length of 8 or more characters
                 */
                Regex regexPassword = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

                // Password is too weak
                if (!regexPassword.IsMatch(password))
                {
                    hasError = true;
                    errorTitle = "Password too weak";
                    errorMessage = "Your password must:"
                        + "<ul class='notification-message'>"
                        + "<li>be at least 8 characters long.</li>"
                        + "<li>include at least 1 number and 1 symbol.</li>"
                        + "<li>include both lowercase and uppercase letters.</li>"
                        + "</ul>";
                }
            }

            // Check for errors
            if (hasError)
            {
                // Display error notification
                ShowErrorNotification(errorTitle, errorMessage);
            }
            else
            {
                try
                {
                    // Generate password hash
                    string passwordHash = PasswordHasher.Hash(password, 100000);

                    // Add user account into database
                    User newUser = new User()
                    {
                        Username = username,
                        Password = passwordHash,
                        Role = dataContext.Roles.SingleOrDefault(x => x.Name == "User"),
                        CreationDate = DateTime.Now,
                        ModificationDate = DateTime.Now
                    };

                    dataContext.Users.Add(newUser);
                    dataContext.SaveChanges();

                    // Redirect user to Login page
                    Response.Redirect("Login.aspx", true);
                }
                catch (DbEntityValidationException dbEx)
                {
                    // Display error notification
                    errorTitle = "Unknown error";
                    errorMessage = "An unexpected error has occured, please contact support.";

                    ShowErrorNotification(errorTitle, errorMessage);

                    PrintDbEntityValidationErrors(dbEx);
                }
                catch (Exception ex)
                {
                    // Display error notification
                    errorTitle = "Unknown error";
                    errorMessage = "An unexpected error has occured, please contact support.";

                    ShowErrorNotification(errorTitle, errorMessage);

                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Prints detailed exception message for DbEntityValidationExceptions.
        /// </summary>
        /// <param name="validationException">DbEntityValidationException to retrieve exception message from.</param>
        protected void PrintDbEntityValidationErrors(DbEntityValidationException validationException)
        {
            // Retrieve and display the error messages
            foreach (DbEntityValidationResult validationResult in validationException.EntityValidationErrors)
            {
                string entityName = validationResult.Entry.Entity.GetType().Name;
                foreach (DbValidationError error in validationResult.ValidationErrors)
                {
                    Debug.WriteLine("[DbEntityValidationException] " + entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Displays error notification popup.
        /// </summary>
        /// <param name="title">Title of error.</param>
        /// <param name="message">Error message.</param>
        protected void ShowErrorNotification(string title, string message)
        {
            notificationTitle.InnerText = title;
            notificationMessage.InnerHtml = message;
            notification.Style.Add("display", "block");
        }
    }
}