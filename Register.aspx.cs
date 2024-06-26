﻿using RojakJelah.Database;
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
        /// FontAwesome icon class strings
        private const string IconExclamation = "fa-solid fa-circle-exclamation";
        private const string IconCheck = "fa-regular fa-circle-check";

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
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

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
                 * - (?=.*[!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~]): symbols (lookahead)
                 * - [A-Za-z\d!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~]: all required charaters (character set)
                 * - {8,}: length of 8 or more characters
                 */
                Regex regexPassword = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~])[A-Za-z\d!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~]{8,}$");

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
                ShowNotification(IconExclamation, errorTitle, errorMessage, true);
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

                    ShowNotification(IconCheck, "Registration success", "Your account has been registered successfully, you will be redirected to the Login page in 5 seconds.", false);

                    // Redirect user to Login page
                    Response.AddHeader("REFRESH", "5;URL=Login.aspx");
                }
                catch (DbEntityValidationException dbEx)
                {
                    // Display error notification
                    ShowNotification();

                    PrintDbEntityValidationErrors(dbEx);
                }
                catch (Exception ex)
                {
                    // Display error notification
                    ShowNotification();

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
        /// Displays status notification popup.
        /// </summary>
        /// <param name="icon">Icon of notification.</param>
        /// <param name="title">Title of status.</param>
        /// <param name="message">Status message.</param>
        /// <param name="isError">Is error notification or not.</param>
        protected void ShowNotification(string icon = IconExclamation, string title = "", string message = "", bool isError = true)
        {
            if (isError)
            {
                notification.Style.Add("background-color", "var(--notification-error)");
            }
            else
            {
                notification.Style.Add("background-color", "var(--notification-success)");
            }

            if (String.IsNullOrWhiteSpace(title) && String.IsNullOrWhiteSpace(message))
            {
                title = "Unknown error";
                message = "An unexpected error has occurred, please contact support.";
            }

            notificationIcon.Attributes.Add("class", icon);
            notificationTitle.InnerText = title;
            notificationMessage.InnerHtml = message;
            notification.Style.Add("display", "block");
        }
    }
}