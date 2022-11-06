using RojakJelah.Database;
using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
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
            [Description("Slang (desc.")]
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

        /// FontAwesome icon class strings
        private const string IconExclamation = "fa-solid fa-circle-exclamation";
        private const string IconCheck = "fa-regular fa-circle-check";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Reset state of controls that may persist in postback
            notification.Style.Add("display", "none");

            if (!IsPostBack)
            {
                PrepopulateControls();
                PopulateDictionary();
            }
        }

        protected void DdlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateDictionary("", ddlSort.SelectedValue, "");
        }

        /// <summary>
        /// Search dictionary entries.
        /// </summary>
        protected void LnkSearch_Click(object sender, EventArgs e)
        {
            string filterOption = ddlSearchFilter.Items[ddlSearchFilter.SelectedIndex].Value;
            string sortOption = ddlSort.Items[ddlSort.SelectedIndex].Value;
            string searchKey = txtSearch.Value.Trim().ToLower();

            PopulateDictionary(filterOption, sortOption, searchKey);
        }

        /// <summary>
        /// Deletes the specified dictionary entry.
        /// </summary>
        protected void BtnDelete_Click(Object sender, EventArgs e)
        {
            try
            {
                DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

                Button sourceButton = sender as Button;
                int dictionaryEntryId = Int32.Parse(sourceButton.Attributes["data-entryid"]);

                dataContext.DictionaryEntries.Remove(dataContext.DictionaryEntries.SingleOrDefault(x => x.Id == dictionaryEntryId));
                dataContext.SaveChanges();

                PopulateDictionary();
                ShowNotification(IconCheck, "Delete succcess", "The dictionary entry has been deleted successfully.", false);
            }
            catch (Exception ex)
            {
                ShowNotification();
            }
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
                                <p class='content-text'>{dictionaryEntry.Example}</p>
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
                        btnDelete.Click += (sender, e) => { BtnDelete_Click(sender, e); };
                        btnDelete.OnClientClick = $"confirmDelete(event, '{btnDelete.ID}');";
                        //btnDelete.OnClientClick = $@"return promptConfirmation(event, 'This is an irreversible action. Are you sure?', '{btnDelete.ID}');";
                        
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
    }
}