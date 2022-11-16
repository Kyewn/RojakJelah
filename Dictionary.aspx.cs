using RojakJelah.Database;
using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class Dictionary : System.Web.UI.Page
    {
        private enum Sorts
        {
            [Description("Slang (asc.)")]
            SlangASC,
            [Description("Slang (desc.)")]
            SlangDESC,
            [Description("Meaning (asc.)")]
            TranslationASC,
            [Description("Meaning (desc.)")]
            TranslationDESC
        }

        private enum Filters
        {
            [Description("Any")]
            Any,
            [Description("Slang")]
            Slang,
            [Description("Meaning")]
            Translation
        }

        private class SearchSettings
        {
            public string FilterOption;
            public string SortOption;
            public string SearchKey;
        }

        // Session variables
        private const string StrSearchSettings = "SearchSettings";

        /// FontAwesome icon class strings
        private const string IconExclamation = "fa-solid fa-circle-exclamation";
        private const string IconCheck = "fa-regular fa-circle-check";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Reset state of controls that may persist in postback
            notification.Style.Add("display", "none");

            if (!IsPostBack)
            {
                Session[StrSearchSettings] = new SearchSettings
                {
                    FilterOption = "",
                    SortOption = "",
                    SearchKey = ""
                };

                PrepopulateControls();
            }

            PopulateDictionary();
        }

        protected void DdlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchSettings sessionSearchSettings = Session[StrSearchSettings] as SearchSettings;
            sessionSearchSettings.SortOption = ddlSort.Items[ddlSort.SelectedIndex].Value;

            RenderDictionaryResult(true, true, true, true);
        }

        /// <summary>
        /// Search dictionary entries.
        /// </summary>
        protected void LnkSearch_Click(object sender, EventArgs e)
        {
            Session[StrSearchSettings] = new SearchSettings
            {
                FilterOption = ddlSearchFilter.Items[ddlSearchFilter.SelectedIndex].Value,
                SortOption = ddlSort.Items[ddlSort.SelectedIndex].Value,
                SearchKey = txtSearch.Value.Trim().ToLower()
            };

            RenderDictionaryResult(true, true, true, false);
        }

        /// <summary>
        /// Deletes the specified dictionary entry.
        /// </summary>
        protected void BtnDelete_Click(Object sender, EventArgs e)
        {
           try
            {
                DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

                // Get dictionary entry ID
                Button sourceButton = sender as Button;
                int dictionaryEntryId = Int32.Parse(sourceButton.Attributes["data-entryid"]);

                // Remove dictionary entry from database
                dataContext.DictionaryEntries.Remove(dataContext.DictionaryEntries.SingleOrDefault(x => x.Id == dictionaryEntryId));
                dataContext.SaveChanges();

                RenderDictionaryResult(false, false, false, true);

                ShowNotification(IconCheck, "Delete succcess", "The dictionary entry has been deleted successfully.", false);
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
            }
            else
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
            User reportAuthor;
            var reportCategory = reportCategoryList.Where((x) => x.Id == ddlReportCategory.SelectedIndex + 1).First();
            var otherCategory = reportCategoryList.Last();

            // Assign report author
            try
            {
                reportAuthor = dataContext.Users.Where((x) => x.Username.ToLower() == Page.User.Identity.Name).First();
            } catch
            {
                reportAuthor = null;
            }

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
            }
            else
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
            ResetReportModal();
            mdlReport.Style.Add("display", "none");
        }

        protected void BtnSubmitSuggestion_Click(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);
            User suggestionAuthor;
            string notificationTitle, notificationMessage;

            // Assign suggestion author
            try
            {
                suggestionAuthor = dataContext.Users.Where((x) => x.Username.ToLower() == Page.User.Identity.Name).First();
            }
            catch
            {
                suggestionAuthor = null;
            }

            //  Check if dictionary pair already exist
            DictionaryEntry existingSlangTranslationPair;

            try
            {
                //  Catch error thrown when system doesnt recognize new words that doesn't exist in DB
                //  Force the system to accept new words
                existingSlangTranslationPair = dataContext.DictionaryEntries
                    .Where(x => x.Slang.WordValue.ToLower() == txtSlang.Value.ToLower())
                    .Where(x => x.Translation.WordValue.ToLower() == txtTranslation.Value.ToLower()).First();
            }
            catch
            {
                existingSlangTranslationPair = null;
            }

            try
            {
                //  Error catching
                //  Required inputs empty
                if (String.IsNullOrWhiteSpace(txtSlang.Value) || String.IsNullOrEmpty(txtSlang.Value) ||
                    String.IsNullOrWhiteSpace(txtTranslation.Value) || String.IsNullOrEmpty(txtTranslation.Value))
                {
                    // Display error notification
                    notificationTitle = "Required fields are empty";
                    notificationMessage = "Slang and Translation fields must be filled.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);
                    ShowModal(mdlSuggestion);
                    return;
                }

                //  Same slang-translation input
                if (txtSlang.Value.ToLower() == txtTranslation.Value.ToLower())
                {
                    // Display error notification
                    notificationTitle = "Identical words";
                    notificationMessage = "Slangs and translations must be different words.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);
                    ShowModal(mdlSuggestion);
                    return;
                }

                //  Existing pair in dictionary
                if (existingSlangTranslationPair != null)
                {
                    // Display error notification
                    notificationTitle = "Word pair already exist";
                    notificationMessage = "The slang-translation pair already exist in the dictionary.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);
                    ShowModal(mdlSuggestion);
                    return;
                }

                //  Submit suggestion
                Suggestion newSuggestion = new Suggestion()
                {
                    Slang = txtSlang.Value.Trim(),
                    Translation = txtTranslation.Value.Trim(),
                    Language = dataContext.Languages.Find(int.Parse(ddlLanguage.SelectedValue)),
                    Example = String.IsNullOrEmpty(txtExample.Value.Trim()) || String.IsNullOrEmpty(txtExample.Value.Trim()) ?
                              null : txtExample.Value.Trim(),
                    CreatedBy = suggestionAuthor,
                    CreationDate = DateTime.Now,
                    ModifiedBy = suggestionAuthor,
                    ModificationDate = DateTime.Now,
                    SuggestionStatus = dataContext.SuggestionStatuses.Find(1),
                };
                dataContext.Suggestions.Add(newSuggestion);
                dataContext.SaveChanges();

                ResetSuggestionModal();
                mdlSuggestion.Style.Add("display", "none");

                //  Send success message
                notificationTitle = "Suggestion submitted";
                notificationMessage = "The suggestion has been submitted for reviewing, thank you for your contribution!";
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

        protected void BtnCancelSuggestion_Click(object sender, EventArgs e)
        {
            ResetSuggestionModal();
            mdlSuggestion.Style.Add("display", "none");
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
        }

        protected void ResetSuggestionModal()
        {
            // Reset control values
            ddlLanguage.SelectedIndex = 0;
            txtSlang.Value = String.Empty;
            txtTranslation.Value = String.Empty;
            txtExample.Value = String.Empty;
        }

        protected void ResetReportModal()
        {
            // Reset control values
            ddlReportCategory.SelectedIndex = 0;
            txtReportSlang.Text = String.Empty;
            ddlReportTranslation.Items.Clear();
            ddlReportTranslation.Enabled = false;
            txtReportDescription.InnerText = String.Empty;
        }

        /// <summary>
        /// Prepopulates page controls with options.
        /// </summary>
        protected void PrepopulateControls()
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            // Sort Options dropdown
            foreach (Enum sortOption in Enum.GetValues(typeof(Sorts)))
            {
                ddlSort.Items.Add(new ListItem(GetEnumDescription(sortOption), sortOption.ToString()));
            }

            // Search Option dropdown
            foreach (Enum filterOption in Enum.GetValues(typeof(Filters)))
            {
                ddlSearchFilter.Items.Add(new ListItem(GetEnumDescription(filterOption), filterOption.ToString()));
            }

            // Issue Category dropddown
            var reportCategoryList = dataContext.ReportCategories.OrderBy(x => x.Id).ToList();

            foreach (var reportCategory in reportCategoryList)
            {
                ddlReportCategory.Items.Add(new ListItem(reportCategory.Name, reportCategory.Id.ToString()));
            }

            // Origin Language dropdown
            var languageList = dataContext.Languages.OrderBy(x => x.Id).ToList();

            foreach (var language in languageList)
            {
                ddlLanguage.Items.Add(new ListItem(language.Name, language.Id.ToString()));
            }
        }

        /// <summary>
        /// Takes filterOption, sortOption, and searchKey to render dictionary with complete, applied settings.
        /// </summary>
        protected void RenderDictionaryResult(bool applyFilter, bool applySort, bool applySearchKey, bool useSession)
        {
            string filterOption, sortOption, searchKey;
            SearchSettings sessionSearchSettings = Session[StrSearchSettings] as SearchSettings ?? null;

            if (useSession && sessionSearchSettings != null)
            {
                filterOption = sessionSearchSettings.FilterOption;
                sortOption = sessionSearchSettings.SortOption;
                searchKey = sessionSearchSettings.SearchKey;
            }
            else
            {
                filterOption = applyFilter ? ddlSearchFilter.Items[ddlSearchFilter.SelectedIndex].Value : "";
                sortOption = applySort ? ddlSort.Items[ddlSort.SelectedIndex].Value : "";
                searchKey = applySearchKey ? txtSearch.Value.Trim().ToLower() : "";
            }

            PopulateDictionary(filterOption, sortOption, searchKey);
        }

        /// <summary>
        /// Populates dictionary with specified filter or search key.
        /// </summary>
        /// <param name="filter">Filter to be applied while retrieving dictionary entries.</param>
        /// <param name="sort">Sort option to be applied when ordering dictionary entries.</param>
        /// <param name="searchKey">Search key to be used for retreiving specific dictionary entries.</param>
        protected void PopulateDictionary(string filter = "", string sort = "", string searchKey = "")
        {
            DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

            // Check if user is authorized personnel
            bool userIsAuthorized = false;

            if (User.Identity.IsAuthenticated)
            {
                var currentUser = dataContext.Users.SingleOrDefault(x => x.Username == User.Identity.Name);
                userIsAuthorized = currentUser.Role.Name == "System" || currentUser.Role.Name == "Admin";
            }

            // Filter and sort dictionary entries if filter/sort/search key is applied
            List<DictionaryEntry> dictionaryEntryList = dataContext.DictionaryEntries.ToList();
            List<DictionaryEntry> filteredEntries = null;
            searchKey = searchKey.Trim().ToLower();

            try
            {
                // Filter
                if (!String.IsNullOrWhiteSpace(filter) && !String.IsNullOrWhiteSpace(searchKey))
                {
                    if (filter == Filters.Slang.ToString())
                    {
                        filteredEntries = dictionaryEntryList.FindAll(x => x.Slang.WordValue.Contains(searchKey));
                    }
                    else if (filter == Filters.Translation.ToString())
                    {
                        filteredEntries = dictionaryEntryList.FindAll(x => x.Translation.WordValue.Contains(searchKey));
                    }
                    else
                    {
                        filteredEntries = dictionaryEntryList.FindAll(x => x.Slang.WordValue.Contains(searchKey) || x.Translation.WordValue.Contains(searchKey));
                    }
                }
                else if (!String.IsNullOrWhiteSpace(searchKey))
                {
                    filteredEntries = dictionaryEntryList.FindAll(x => x.Slang.WordValue.Contains(searchKey) || x.Translation.WordValue.Contains(searchKey));
                }
                else
                {
                    filteredEntries = dictionaryEntryList;
                }

                // Sort
                if (!String.IsNullOrWhiteSpace(sort))
                {
                    if (sort == Sorts.SlangASC.ToString())
                    {
                        filteredEntries = filteredEntries.OrderBy(x => x.Slang.WordValue).ToList();
                    }
                    else if (sort == Sorts.SlangDESC.ToString())
                    {
                        filteredEntries = filteredEntries.OrderByDescending(x => x.Slang.WordValue).ToList();
                    }
                    else if (sort == Sorts.TranslationASC.ToString())
                    {
                        filteredEntries = filteredEntries.OrderBy(x => x.Translation.WordValue).ToList();
                    }
                    else if (sort == Sorts.TranslationDESC.ToString())
                    {
                        filteredEntries = filteredEntries.OrderByDescending(x => x.Translation.WordValue).ToList();
                    }
                }
                else
                {
                    filteredEntries = filteredEntries.OrderBy(x => x.Slang.WordValue).ToList();
                }
            }
            catch (Exception ex)
            {
                ShowNotification();
            }

            sctDictionary.Controls.Clear();

            // Populate dictionary container
            if ((filteredEntries == null) || (filteredEntries.Count == 0))
            {
                // Empty dictionary
                LiteralControl dictionaryEmpty = new LiteralControl($@"
                    <div class='dictionary-empty'>
                        <i class='fa-regular fa-circle-xmark fa-2xl'></i>
                        <h4>No entries found</h4>
                    </div>
                ");

                sctDictionary.Controls.Add(dictionaryEmpty);
            }
            else
            {
                int count = 0;

                foreach (var dictionaryEntry in filteredEntries)
                {
                    count++;

                    // Dictionary item container
                    Panel dictionaryItem = new Panel();
                    dictionaryItem.CssClass = "dictionary-item";

                    // Dictionary item text content
                    LiteralControl dictionaryItemContent = new LiteralControl($@"
                        <div class='dictionary-item-content'>
                            <h4 class='dictionary-item-title'>{dictionaryEntry.Slang.WordValue}</h4>
                            <div class='dictionary-item-text'>
                                <p class='content-title'>Meaning</p>
                                <p class='content-text'>{dictionaryEntry.Translation.WordValue}</p>
                            </div>
                            <div class='dictionary-item-text'>
                                <p class='content-title'>Example</p>
                                <p class='content-text'>{dictionaryEntry.Example ?? "None"}</p>
                            </div>
                            <div class='dictionary-item-text'>
                                <p class='content-title'>Language</p>
                                <p class='content-text'>{dictionaryEntry.Slang.Language.Name}</p>
                            </div>
                        </div>
                        ");

                    // Add item text content to item container
                    dictionaryItem.Controls.Add(dictionaryItemContent);

                    // Add dictionary item controls if user is authorized
                    if (userIsAuthorized)
                    {
                        Panel dictionaryItemControls = new Panel();
                        dictionaryItemControls.CssClass = "dictionary-item-controls";

                        Button btnDelete = new Button();
                        btnDelete.ID = "btnDelete" + dictionaryEntry.Id;
                        btnDelete.ClientIDMode = ClientIDMode.Static;
                        btnDelete.CssClass = "button-secondary button-delete";
                        btnDelete.Text = "Delete";
                        btnDelete.Attributes.Add("data-entryid", dictionaryEntry.Id.ToString());
                        btnDelete.Click += new EventHandler(BtnDelete_Click);
                        btnDelete.OnClientClick = $"confirmDelete(event, '{btnDelete.ID}');";
                        
                        dictionaryItemControls.Controls.Add(btnDelete);

                        dictionaryItem.Controls.Add(dictionaryItemControls);
                    }

                    // Add dictionary entry to container
                    sctDictionary.Controls.Add(dictionaryItem);
                }    
            }

            // Update entry count text
            entryCountText.InnerText = filteredEntries.Count + (filteredEntries.Count == 1 ? " entry " : " entries ") + "found";
        }

        protected static string GetEnumDescription(Enum selectedEnum)
        {
            FieldInfo fi = selectedEnum.GetType().GetField(selectedEnum.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return selectedEnum.ToString();
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
    }
}