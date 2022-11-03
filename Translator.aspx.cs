using Newtonsoft.Json;
using RojakJelah.Database;
using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class Translator : System.Web.UI.Page
    {
        /// Hash function divisor for modulo operator
        private const int HashDivisor = 97;

        /// FontAwesome icon class strings
        private const string IconSolidBookmark = "fa-solid fa-bookmark";
        private const string IconRegularBookmark = "fa-regular fa-bookmark";
        private const string IconExclamation = "fa-solid fa-circle-exclamation";
        private const string IconAccessDenied = "fa-solid fa-ban";
        private const string IconFloppyDisk = "fa-solid fa-floppy-disk";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Hide nav header
            HtmlControl navHeader = Master.FindControl("navHeader") as HtmlControl;
            navHeader.Style.Add("visibility", "hidden");

            if (!IsPostBack)
            {
                var rojakHashtable = InitializeRojakHashtable();
                ManageSession(rojakHashtable);
            }

            if (!User.Identity.IsAuthenticated)
            {
                // Disable saved translations modal
                btnViewSavedTranslations.Attributes.Remove("data-bs-toggle");
                btnViewSavedTranslations.Attributes.Remove("data-bs-target");
                // Display error notification if unauthorized user attempts to open modal
                btnViewSavedTranslations.Attributes.Add("onclick",
                    String.Format("showNotification('{0}', '{1}', '{2}')", IconAccessDenied, "Access denied", "You need to be logged in to use this feature."));
            }
            else
            {
                PrepopulateSavedTranslations();
            }

            PrepopulateTranslationHistory();
        }

        /// <summary>
        /// Initializes a hash table to store Rojak dictionary entries.
        /// </summary>
        /// <returns>Hashtable representing rojak dictionary.</returns>
        protected Dictionary<string, List<string>>[] InitializeRojakHashtable()
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            // Initialize array
            Dictionary<string, List<string>>[] rojakHashtable = new Dictionary<string, List<string>>[26];
            for (int i = 0; i < rojakHashtable.Length; i++)
            {
                rojakHashtable[i] = new Dictionary<string, List<string>>();
            }

            // Query database for all dictionary entries
            var dictionaryEntries = dataContext.DictionaryEntries.ToList();

            // Sort dictionary entries into rojakHashtable by hashing
            foreach (var dictionaryEntry in dictionaryEntries)
            {
                string currentTranslation = dictionaryEntry.Translation.WordValue;
                string currentSlang = dictionaryEntry.Slang.WordValue;

                // Get hash index (a = 0, b = 1, c = 2 ...)
                int index = currentTranslation[0] % HashDivisor;

                // If rojakHashtable already contains this translation, add the slang to existing slang list of the slang
                if (rojakHashtable[index].ContainsKey(currentTranslation))
                {
                    rojakHashtable[index][currentTranslation].Add(currentSlang);
                }
                // Else, create a new slang list before adding
                else
                {
                    List<string> slangList = new List<string>();
                    slangList.Add(currentSlang);
                    rojakHashtable[index].Add(currentTranslation, slangList);
                }
            }

            return rojakHashtable;
        }

        /// <summary>
        /// Manages session variables.
        /// </summary>
        /// <param name="rojakHashtable">Hash table storing Rojak dictionary entries.</param>
        protected void ManageSession(Dictionary<string, List<string>>[] rojakHashtable)
        {
            // Store rojakHashtable in session
            Session["RojakHashtable"] = rojakHashtable;

            // Retrieve translation history from session
            var translationHistory = Session["TranslationHistory"] as List<SavedTranslation>;
            if (translationHistory == null)
            {
                Session["TranslationHistory"] = new List<SavedTranslation>();
            }
        }

        protected void BtnTranslate_Click(Object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            // Hide notification
            notification.Style.Add("display", "none");

            // Update save icon
            iconSave.Attributes.Add("class", IconRegularBookmark);

            // Clear output
            txtOutput.InnerText = "";

            // Get trimmed input text
            string inputText = txtInput.Value.Trim();
            // Split input text into individual words
            var inputWords = !String.IsNullOrWhiteSpace(inputText) ? inputText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries) : null;

            // List to store output words
            List<string> outputWords = new List<string>();

            // Perform translation
            if (inputWords != null && inputWords.Count() > 0)
            {
                var rojakHashtable = Session["RojakHashtable"] as Dictionary<string, List<string>>[];
                Random randomizer = new Random();

                foreach (string word in inputWords)
                {
                    // Get hash index
                    int index = Char.ToLower(word[0]) % HashDivisor;

                    // If rojakHashtable contains the input word, add its corresponding slang to output words
                    if (index < rojakHashtable.Length && rojakHashtable[index].ContainsKey(word.ToLower()))
                    {
                        var matchedSlangList = rojakHashtable[index][word.ToLower()];

                        string matchedSlang = matchedSlangList.ElementAt(randomizer.Next(0, matchedSlangList.Count));

                        /* Rules for dealing with letter casing:
                         * 1. If first letter of input word is UPPERCASE, convert first letter of output word to UPPERCASE
                         * 2. If input word is full UPPERCASE, convert output word to full UPPERCASE
                         * 3. If input word is full LOWERCASE, simply add the output word in its original form (LOWERCASE)
                         */
                        outputWords.Add(word.All(c => Char.IsUpper(c)) ? matchedSlang.ToUpper() :
                                        Char.IsUpper(word[0]) ? Char.ToUpper(matchedSlang[0]) + matchedSlang.Substring(1) : matchedSlang);
                    }
                    // Else, add the original input word back into output words
                    else
                    {
                        outputWords.Add(word);
                    }
                }

                // Compose sentence
                for (int i = 0; i < outputWords.Count; i++)
                {
                    txtOutput.InnerText += outputWords.ElementAt(i);
                    txtOutput.InnerText += (i + 1) < outputWords.Count ? " " : "";
                }

                // Save translation into session-based translation history
                List<SavedTranslation> translationHistory = Session["TranslationHistory"] as List<SavedTranslation>;

                translationHistory.Add(new SavedTranslation()
                {
                    Input = inputText,
                    Output = txtOutput.InnerText,
                    CreatedBy = dataContext.Users.SingleOrDefault(x => x.Username == User.Identity.Name) ?? null,
                    CreationDate = DateTime.Now
                });
            }

            PrepopulateTranslationHistory();
        }

        protected void LnkSaveTranslation_Click(Object sender, EventArgs e)
        {
            // Hide notification
            notification.Style.Add("display", "none");

            if (!User.Identity.IsAuthenticated)
            {
                ShowStatusNotification(IconAccessDenied, "Access denied", "You need to be logged in to use this feature.");
            }
            else
            {
                DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

                string inputText = txtInput.InnerText;
                string outputText = txtOutput.InnerText;

                // Check if there is data to save
                if (String.IsNullOrWhiteSpace(inputText) || String.IsNullOrWhiteSpace(outputText))
                {
                    // Show error notification
                    ShowStatusNotification(IconExclamation, "Unable to save", "Source message and translation output cannot be empty.");
                }
                else
                {
                    // Save translation into database
                    SavedTranslation newSavedTranslation = new SavedTranslation()
                    {
                        Input = inputText,
                        Output = outputText,
                        CreatedBy = dataContext.Users.SingleOrDefault(x => x.Username == User.Identity.Name),
                        CreationDate = DateTime.Now
                    };

                    dataContext.SavedTranslations.Add(newSavedTranslation);
                    dataContext.SaveChanges();

                    // Show success notification
                    ShowStatusNotification(IconFloppyDisk, "Save success", "This translation has been saved successfully.");

                    // Update save icon
                    iconSave.Attributes.Add("class", IconSolidBookmark);

                    PrepopulateSavedTranslations();
                }
            }
        }

        /// <summary>
        /// Prepopulates the saved translations modal with translations saved in the database.
        /// </summary>
        protected void PrepopulateSavedTranslations()
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            var savedTranslationList = dataContext.SavedTranslations.Where(x => x.CreatedBy.Username == User.Identity.Name).ToList();

            divSavedTranslationsModalBody.Controls.Clear();

            if (savedTranslationList.Count == 0)
            {
                LiteralControl modalEmpty = new LiteralControl(
                        @"<div class=""modal-empty"">" +
                            @"<i class=""fa-solid fa-file-excel""></i>" +
                            "<h1>No translations found</h1>" +
                        "</div>");

                divSavedTranslationsModalBody.Controls.Add(modalEmpty);
            }
            else
            {
                int count = 0;
                foreach (var savedTranslation in savedTranslationList)
                {
                    count++;

                    LiteralControl modalItem = new LiteralControl(
                            @"<div class=""modal-item"">" +
                                "<h2>" + savedTranslation.Input + "</h2>" +
                                "<h4>Translation</h4>" +
                                "<h3>" + savedTranslation.Output + "</h3>" +
                                @"<div class=""modal-item-row"">" +
                                    "<small>Date: " + savedTranslation.CreationDate + "</small>" +
                                    @"<button type=""button"" class=""modal-item-btn"">Remove</button>" +
                                "</div>" +
                            "</div>");

                    divSavedTranslationsModalBody.Controls.Add(modalItem);
                }

                savedTranslationFooterText.InnerText = savedTranslationList.Count +
                    " translation" +
                    (savedTranslationList.Count > 1 ? "s " : " ") +
                    "found";
            }
        }

        /// <summary>
        /// Prepopulates the translation history modal with translations saved in session.
        /// </summary>
        protected void PrepopulateTranslationHistory()
        {
            var translationHistory = Session["TranslationHistory"] as List<SavedTranslation>;

            divTranslationHistoryModalBody.Controls.Clear();

            if (translationHistory.Count == 0)
            {
                LiteralControl modalEmpty = new LiteralControl(
                        @"<div class=""modal-empty"">" +
                            @"<i class=""fa-solid fa-file-excel""></i>" +
                            "<h1>No translations found</h1>" +
                        "</div>");

                divTranslationHistoryModalBody.Controls.Add(modalEmpty);
            }
            else
            {
                int count = 0;
                foreach (var savedTranslation in translationHistory)
                {
                    count++;

                    LiteralControl modalItem = new LiteralControl(
                            @"<div class=""modal-item"">" +
                                "<h2>" + savedTranslation.Input + "</h2>" +
                                "<h4>Translation</h4>" +
                                "<h3>" + savedTranslation.Output + "</h3>" +
                                @"<div class=""modal-item-row"">" +
                                    "<small>Date: " + savedTranslation.CreationDate + "</small>" +
                                    @"<button type=""button"" class=""modal-item-btn"">Remove</button>" +
                                "</div>" +
                            "</div>");

                    divTranslationHistoryModalBody.Controls.Add(modalItem);
                }

                translationHistoryFooterText.InnerText = translationHistory.Count +
                    " translation" +
                    (translationHistory.Count > 1 ? "s " : " ") +
                    "found";
            }
        }

        /// <summary>
        /// Displays status notification popup.
        /// </summary>
        /// <param name="title">Title of status.</param>
        /// <param name="message">Status message.</param>
        protected void ShowStatusNotification(string icon, string title, string message)
        {
            notificationIcon.Attributes.Add("class", icon);
            notificationTitle.InnerText = title;
            notificationMessage.InnerHtml = message;
            notification.Style.Add("display", "block");
        }
    }
}