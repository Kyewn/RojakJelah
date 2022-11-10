using RojakJelah.Database;
using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class Translator : System.Web.UI.Page
    {
        /// Hash function divisor for modulo operator
        private const int HashDivisor = 97;

        /// Session variables
        private const string StrRojakHashtable = "RojakHashtable";
        private const string StrTranslationHistory = "TranslationHistory";
        private const string StrMostRecentTranslation = "MostRecentTranslation";

        /// Control attributes
        private const string AttrTranslationId = "data-translationid";
        private const string AttrUseSession = "data-use-session";

        /// FontAwesome icon class strings
        private const string IconExclamation = "fa-solid fa-circle-exclamation";
        private const string IconAccessDenied = "fa-solid fa-ban";
        private const string IconFloppyDisk = "fa-solid fa-floppy-disk";
        private const string IconCheck = "fa-regular fa-circle-check";
        private const string IconSolidBookmark = "fa-solid fa-bookmark";
        private const string IconRegularBookmark = "fa-regular fa-bookmark";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Hide nav header
            HtmlControl navHeader = Master.FindControl("navHeader") as HtmlControl;
            navHeader.Style.Add("visibility", "hidden");

            // Reset state of any controls that may persist in postback (this is so stupid)
            HtmlGenericControl body = Master.FindControl("body") as HtmlGenericControl;
            body.Style.Add("overflow-y", "auto");
            notification.Style.Add("display", "none");
            mdlSavedTranslations.Style.Add("display", "none");
            mdlTranslationHistory.Style.Add("display", "none");
            mdlReport.Style.Add("display", "none");

            if (!IsPostBack)
            {
                var rojakHashtable = InitializeRojakHashtable();

                PrepopulateControls();

                ManageSession(rojakHashtable);
            }

            PopulateSavedTranslations();
            PopulateTranslationHistory();     
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
            Session[StrRojakHashtable] = rojakHashtable;

            // Retrieve translation history from session
            var translationHistory = Session[StrTranslationHistory] as List<SavedTranslation>;
            if (translationHistory == null)
            {
                Session[StrTranslationHistory] = new List<SavedTranslation>();
            }
        }

        protected void BtnTranslate_Click(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

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
                var rojakHashtable = Session[StrRojakHashtable] as Dictionary<string, List<string>>[];
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

                // Save translation into session (most recent translation & translation history)
                List<SavedTranslation> translationHistory = Session[StrTranslationHistory] as List<SavedTranslation>;
                int translationId = translationHistory.Count + 1;

                SavedTranslation mostRecentTranslation = new SavedTranslation()
                {
                    Id = translationId,
                    Input = inputText,
                    Output = txtOutput.InnerText,
                    CreatedBy = dataContext.Users.SingleOrDefault(x => x.Username == User.Identity.Name) ?? null,
                    CreationDate = DateTime.Now
                };

                Session[StrMostRecentTranslation] = mostRecentTranslation;
                translationHistory.Add(mostRecentTranslation);
            }
        }

        protected void LnkSaveTranslation_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                ShowNotification(IconAccessDenied, "Access denied", "You need to be logged in to use this feature.", true);
            }
            else
            {
                DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

                string inputText = txtInput.InnerText.Trim();
                string outputText = txtOutput.InnerText.Trim();

                // Check if there is data to save
                if (String.IsNullOrWhiteSpace(inputText) || String.IsNullOrWhiteSpace(outputText))
                {
                    // Show error notification
                    ShowNotification(IconExclamation, "Unable to save", "Source message and translation output cannot be empty.", true);
                }
                else
                {
                    // Compare current input text with most recently translated input text
                    string notificationMessage;
                    var mostRecentTranslation = Session[StrMostRecentTranslation] as SavedTranslation;

                    if (!String.Equals(mostRecentTranslation.Input, inputText))
                    {
                        notificationMessage = "The source message has been changed since last translation, so the most recently translated source message is saved instead.";
                    }
                    else
                    {
                        notificationMessage = "This translation has been saved successfully.";
                    }

                    // Save translation into database
                    SavedTranslation newSavedTranslation = new SavedTranslation()
                    {
                        Input = mostRecentTranslation.Input,
                        Output = mostRecentTranslation.Output,
                        CreatedBy = dataContext.Users.SingleOrDefault(x => x.Username == User.Identity.Name),
                        CreationDate = DateTime.Now
                    };

                    dataContext.SavedTranslations.Add(newSavedTranslation);
                    dataContext.SaveChanges();

                    // Update save icon
                    iconSave.Attributes.Add("class", IconSolidBookmark);

                    // Show success notification
                    ShowNotification(IconFloppyDisk, "Save success", notificationMessage, false);
                }
            }
        }

        /// <summary>
        /// Populates and shows the saved translations modal with translations saved in the database.
        /// </summary>
        protected void LnkViewSavedTranslations_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                ShowNotification(IconAccessDenied, "Access denied", "You need to be logged in to use this feature.", true);
            }
            else
            {
                PopulateSavedTranslations();
                ShowModal(mdlSavedTranslations);
            }
        }

        /// <summary>
        /// Populates and shows the translation history modal with translations saved in session.
        /// </summary>
        protected void LnkViewTranslationHistory_Click(Object sender, EventArgs e)
        {
            PopulateTranslationHistory();
            ShowModal(mdlTranslationHistory);
        }

        /// <summary>
        /// Opens report modal.
        /// </summary>
        protected void LnkReport_Click(object sender, EventArgs e)
        {
            ShowModal(mdlReport);
        }

        protected void BtnDeleteTranslation_Click(object sender, EventArgs e)
        {
            try
            {
                // Get saved translation ID
                Button sourceButton = sender as Button;
                int translationId = Int32.Parse(sourceButton.Attributes[AttrTranslationId]);

                // Determine if it is saved translation or translation history
                if (sourceButton.Attributes[AttrUseSession] == null)
                {
                    // Saved translation
                    if (User.Identity.IsAuthenticated)
                    {
                        DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);
                        dataContext.SavedTranslations.Remove(dataContext.SavedTranslations.SingleOrDefault(x => x.Id == translationId));
                        dataContext.SaveChanges();

                        LnkViewSavedTranslations_Click(sender, e);

                        ShowNotification(IconCheck, "Delete succcess", "The translation has been deleted successfully.", false);
                    }
                    else
                    {
                        ShowNotification(IconAccessDenied, "Access denied", "You do not have permission to delete translations.", true);
                    }
                }
                else
                {
                    // Translation history
                    var translationHistory = Session[StrTranslationHistory] as List<SavedTranslation>;
                    translationHistory.Remove(translationHistory.SingleOrDefault(x => x.Id == translationId));

                    LnkViewTranslationHistory_Click(sender, e);

                    ShowNotification(IconCheck, "Delete succcess", "The translation has been deleted successfully.", false);
                }
            }
            catch (Exception ex)
            {
                ShowNotification();
            }
        }

        protected void DdlReportCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);
            var reportCategoryList = dataContext.ReportCategories.OrderBy(x => x.Id).ToList();

            if (ddlReportCategory.SelectedIndex + 1 == reportCategoryList.Last().Id)
            {
                // Other category
                divEntryInput.Style.Add("display", "none");
            } else
            {
                // "Entry issue" category
                divEntryInput.Style.Add("display", "flex");
            }

            //  Keep report modal open without animation
            ShowModal(mdlReport);
            dlgReport.Style.Remove("animation");
        }

        protected void TxtReportSlang_TextChanged(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            var txtProblemSlang = txtReportSlang.Text.ToLower().Trim();
            var matchingSlangEntries = dataContext.DictionaryEntries.
                Where((x) => x.Slang.WordValue.ToLower() == txtProblemSlang).ToList();

            // Clear everything before determining existence
            ddlReportTranslation.Items.Clear();
            if (matchingSlangEntries.Count != 0)
            {
                foreach (var entry in matchingSlangEntries)
                {
                    ddlReportTranslation.Items.Add(entry.Translation.WordValue);
                }
                // Enable dropdown
                ddlReportTranslation.Enabled = true;
            }
            else
            {
                // Disable dropdown
                ddlReportTranslation.Enabled = false;
            }

            //  Keep report modal open without animation
            ShowModal(mdlReport);
            dlgReport.Style.Remove("animation");
        }

        protected void BtnSubmitReport_Click(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);
            String notificationTitle, notificationMessage;

            var reportCategoryList = dataContext.ReportCategories.OrderBy(x => x.Id).ToList();
            var reportDescription = String.IsNullOrEmpty(txtReportDescription.InnerText) || String.IsNullOrWhiteSpace(txtReportDescription.InnerText) ?
                        null : txtReportDescription.InnerText;
            var reportStatus = dataContext.ReportStatuses.Where((x) => x.Id == 1).First();
            var reportAuthor = dataContext.Users.Where((x) => x.Username.ToLower() == Page.User.Identity.Name).First();
            var reportCategory = reportCategoryList.Where((x) => x.Id == ddlReportCategory.SelectedIndex + 1).First();
            var otherCategory = reportCategoryList.Last();

            if (ddlReportCategory.SelectedIndex + 1 == otherCategory.Id)
            {
                //  Error catching
                //  Description empty
                if (String.IsNullOrEmpty(txtReportDescription.InnerText) ||
                    String.IsNullOrWhiteSpace(txtReportDescription.InnerText))
                {
                    notificationTitle = "Empty description";
                    notificationMessage = "Other issues require a description, please try again.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);
                    //  Keep modal open without animation
                    ShowModal(mdlReport);
                    dlgReport.Style.Remove("animation");
                    return;
                }

                try
                {
                    // Insert new report
                    Report newReport = new Report()
                    {
                        ReportCategory = reportCategory,
                        Description = reportDescription,
                        ReportStatus = reportStatus,
                        CreatedBy = reportAuthor,
                        CreationDate = DateTime.Now,
                        ModifiedBy = reportAuthor,
                        ModificationDate = DateTime.Now,
                    };
                    dataContext.Reports.Add(newReport);
                    dataContext.SaveChanges();

                    ResetReportModal(sender, e);

                    // Show notification
                    notificationTitle = "Report submitted";
                    notificationMessage = "The report has been submitted for reviewing, thank you for your contribution!";

                    ShowNotification(IconCheck, notificationTitle, notificationMessage, false);
                }
                catch (Exception ex)
                {
                    // Display error notification
                    notificationTitle = "Unknown error";
                    notificationMessage = "An unexpected error has occured, please contact support.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);

                    Debug.WriteLine(ex.ToString());
                }
            } else
            {
                //  Error catching
                //  Empty slang/translation 
                if (String.IsNullOrEmpty(txtReportSlang.Text) ||
                    String.IsNullOrWhiteSpace(txtReportSlang.Text) ||
                    String.IsNullOrEmpty(ddlReportTranslation.SelectedValue) ||
                    String.IsNullOrWhiteSpace(ddlReportTranslation.SelectedValue))
                {
                    notificationTitle = "Empty required input fields";
                    notificationMessage = "Slang and translation inputs must have a value. As this usually happens when users input an incorrect entry, please check if the entry exists and try again.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);
                    //  Keep modal open without animation
                    ShowModal(mdlReport);
                    dlgReport.Style.Remove("animation");
                    return;
                }

                var reportEntry = dataContext.DictionaryEntries
                    .Where((x) => x.Slang.WordValue.ToLower() == txtReportSlang.Text.ToLower())
                    .Where((x) => x.Translation.WordValue.ToLower() == ddlReportTranslation.SelectedValue.ToLower())
                    .First();

                try
                {
                    // Insert new report
                    Report newReport = new Report()
                    {
                        ReportCategory = reportCategory,
                        DictionaryEntry = reportEntry,
                        Description = reportDescription,
                        ReportStatus = reportStatus,
                        CreatedBy = reportAuthor,
                        CreationDate = DateTime.Now,
                        ModifiedBy = reportAuthor,
                        ModificationDate = DateTime.Now,
                    };
                    dataContext.Reports.Add(newReport);
                    dataContext.SaveChanges();

                    ResetReportModal(sender, e);

                    // Show notification
                    notificationTitle = "Report submitted";
                    notificationMessage = "The report has been submitted for reviewing, thank you for your contribution!";

                    ShowNotification(IconCheck, notificationTitle, notificationMessage, false);
                }
                catch (Exception ex)
                {
                    // Display error notification
                    notificationTitle = "Unknown error";
                    notificationMessage = "An unexpected error has occured, please contact support.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);

                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        protected void BtnCancelReport_Click(object sender, EventArgs e)
        {
            // Reset control values
            ResetReportModal(sender, e);
        }

        /// <summary>
        /// Prepopulates page controls with options.
        /// </summary>
        protected void PrepopulateControls()
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            // Issue Category dropddown
            var reportCategoryList = dataContext.ReportCategories.OrderBy(x => x.Id).ToList();

            foreach (var reportCategory in reportCategoryList)
            {
                ddlReportCategory.Items.Add(new ListItem(reportCategory.Name, reportCategory.Id.ToString()));
            }
        }

        protected void PopulateSavedTranslations()
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            var savedTranslationList = dataContext.SavedTranslations.Where(x => x.CreatedBy.Username == User.Identity.Name).ToList();

            divSavedTranslationsModalBody.Controls.Clear();

            if (savedTranslationList.Count == 0)
            {
                FillEmptyModal(divSavedTranslationsModalBody);
                savedTranslationFooterText.InnerText = "";
            }
            else
            {
                int count = 0;

                foreach (var savedTranslation in savedTranslationList)
                {
                    count++;
                    AddModalItem(divSavedTranslationsModalBody, savedTranslation, false);
                }

                savedTranslationFooterText.InnerText = savedTranslationList.Count + " translation" + (savedTranslationList.Count > 1 ? "s " : " ") + "found";
            }
        }

        protected void PopulateTranslationHistory()
        {
            var translationHistory = Session[StrTranslationHistory] as List<SavedTranslation>;

            divTranslationHistoryModalBody.Controls.Clear();

            if (translationHistory.Count == 0)
            {
                FillEmptyModal(divTranslationHistoryModalBody);
                translationHistoryFooterText.InnerText = "";
            }
            else
            {
                int count = 0;

                foreach (var savedTranslation in translationHistory)
                {
                    count++;
                    AddModalItem(divTranslationHistoryModalBody, savedTranslation, true);
                }

                translationHistoryFooterText.InnerText = translationHistory.Count + " translation" + (translationHistory.Count > 1 ? "s " : " ") + "found";
            }
        }

        /// <summary>
        /// Shows the specified modal.
        /// </summary>
        /// <param name="modal">Modal to show.</param>
        protected void ShowModal(HtmlGenericControl modal)
        {
            HtmlGenericControl body = Master.FindControl("body") as HtmlGenericControl;
            body.Style.Add("overflow-y", "hidden");
            modal.Style.Add("display", "flex");
            dlgSavedTranslation.Style.Add("animation", "slideIn .3s ease-out forwards");
            dlgHistory.Style.Add("animation", "slideIn .3s ease-out forwards");
            dlgReport.Style.Add("animation", "slideIn .3s ease-out forwards");
        }

        protected void FillEmptyModal(HtmlGenericControl modal)
        {
            LiteralControl modalEmpty = new LiteralControl($@"
                <div class='modal-empty'>
                    <i class='fa-solid fa-file-excel'></i>
                    <h1>No translations found</h1>
                </div>
            ");

            modal.Controls.Add(modalEmpty);
        }

        protected void AddModalItem(HtmlGenericControl modal, SavedTranslation savedTranslation, bool useSession)
        {
            // Modal item container
            Panel modalItem = new Panel();
            modalItem.CssClass = "modal-item";

            // Modal item content
            LiteralControl modalItemContent = new LiteralControl($@"
                <div class='modal-item-content'>
                    <h2 class='modal-item-title'>{savedTranslation.Input}</h2>
                    <div class='modal-item-text'>
                        <h3 class='text-title'>Translation</h3>
                        <h3 class='text-content'>{savedTranslation.Output}</h3>
                    </div>
                    <small>Date: {savedTranslation.CreationDate}</small>
                </div>
            ");

            // Modal item controls
            Panel modalItemControls = new Panel();
            modalItemControls.CssClass = "modal-item-controls";

            Button btnDelete = new Button();
            btnDelete.ID = (!useSession ? "btnDeleteSaved" : "btnDeleteHistory") + savedTranslation.Id;
            btnDelete.ClientIDMode = ClientIDMode.Static;
            btnDelete.CssClass = "modal-item-btn";
            btnDelete.Text = "Delete";
            btnDelete.Attributes.Add(AttrTranslationId, savedTranslation.Id.ToString());
            btnDelete.Click += new EventHandler(BtnDeleteTranslation_Click);
            btnDelete.OnClientClick = $"confirmDelete(event, '{btnDelete.ID}');";

            if (useSession)
            {
                btnDelete.Attributes.Add(AttrUseSession, "true");
            }

            modalItemControls.Controls.Add(btnDelete);

            // Add item content and item controls to item container
            modalItem.Controls.Add(modalItemContent);
            modalItem.Controls.Add(modalItemControls);

            // Add item to modal
            modal.Controls.Add(modalItem);
        }

        protected void ResetReportModal(object sender, EventArgs e)
        {
            // Reset control values
            ddlReportCategory.SelectedIndex = 0;
            txtReportSlang.Text = String.Empty;
            ddlReportTranslation.Items.Clear();
            ddlReportTranslation.Enabled = false;
            txtReportDescription.InnerText = String.Empty;
            DdlReportCategory_SelectedIndexChanged(sender, e);

            //  Prevent keeping modal visible
            HtmlGenericControl body = Master.FindControl("body") as HtmlGenericControl;
            body.Style.Add("overflow-y", "auto");
            mdlReport.Style.Add("display", "hidden");
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