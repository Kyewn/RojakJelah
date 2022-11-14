using RojakJelah.Database;
using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
            mdlEditDictionaryEntry.Style.Add("display", "none");

            if (!IsPostBack)
            {
                Session[StrSearchSettings] = new SearchSettings
                {
                    FilterOption = "",
                    SortOption = "",
                    SearchKey = ""
                };

                if (User.Identity.IsAuthenticated)
                {
                    string currentUserName = User.Identity.Name;

                    if (!String.IsNullOrWhiteSpace(currentUserName))
                    {
                        DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);
                        string currentUserRole = dataContext.Users.SingleOrDefault(x => x.Username == currentUserName).Role.Name;

                        ViewState["UserRole"] = currentUserRole ?? null;
                    }
                }

                PrepopulateControls();
                PopulateDictionary();
            }
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

            dictionaryDataPager.SetPageProperties(0, 20, true);

            RenderDictionaryResult(true, true, true, false);
        }


        /// <summary>
        /// Handles Dictionary ListView item commands
        /// </summary>
        protected void LvDictionary_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            // Edit dictionatry entry
            if (e.CommandName == "Modify")
            {
                try
                {
                    DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

                    ListViewDataItem dataItem = e.Item as ListViewDataItem;
                    int dictionaryEntryId = Int32.Parse(lvDictionary.DataKeys[dataItem.DisplayIndex].Value.ToString());

                    var selectedDictionaryEntry = dataContext.DictionaryEntries.SingleOrDefault(x => x.Id == dictionaryEntryId);

                    // Update edit modal field values
                    txtDictionaryEntryId.InnerText = selectedDictionaryEntry.Id.ToString();
                    txtEditSlang.InnerText = selectedDictionaryEntry.Slang.WordValue;
                    txtEditTranslation.InnerText = selectedDictionaryEntry.Translation.WordValue;
                    ddlEditLanguage.SelectedIndex = selectedDictionaryEntry.Slang.Language.Id - 1;
                    txtEditExample.Value = selectedDictionaryEntry.Example;

                    RenderDictionaryResult(false, false, false, true);

                    MaintainScrollPosition();
                    ShowModal(mdlEditDictionaryEntry);
                }
                catch (Exception ex)
                {
                    ShowNotification();
                }
            }
            // Delete dictionary entry
            else if (e.CommandName == "Remove")
            {
                try
                {
                    DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

                    // Get dictionary entry ID
                    ListViewDataItem dataItem = e.Item as ListViewDataItem;
                    int dictionaryEntryId = Int32.Parse(lvDictionary.DataKeys[dataItem.DisplayIndex].Value.ToString());

                    // Remove dictionary entry from database
                    dataContext.DictionaryEntries.Remove(dataContext.DictionaryEntries.SingleOrDefault(x => x.Id == dictionaryEntryId));
                    dataContext.SaveChanges();

                    RenderDictionaryResult(false, false, false, true);

                    MaintainScrollPosition();
                    ShowNotification(IconCheck, "Delete success", "The dictionary entry has been deleted successfully.", false);
                }
                catch (Exception ex)
                {
                    ShowNotification();
                }
            }
        }

        /// <summary>
        /// Updates the specified dictionary entry.
        /// </summary>
        protected void BtnConfirmEdit_Click(object sender, EventArgs e)
        {
            try
            {
                DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);

                // Get dictionary entry ID
                int dictionaryEntryId = Int32.Parse(txtDictionaryEntryId.InnerText);

                // Get DictionaryEntry entity
                var dictionaryEntry = dataContext.DictionaryEntries.SingleOrDefault(x => x.Id == dictionaryEntryId);

                // Update language and example
                int selectedLanguageId = Int32.Parse(ddlEditLanguage.Items[ddlEditLanguage.SelectedIndex].Value);
                dictionaryEntry.Slang.Language = dataContext.Languages.SingleOrDefault(x => x.Id == selectedLanguageId) ?? dictionaryEntry.Slang.Language;
                dictionaryEntry.Example = txtEditExample.Value.Trim();
                dictionaryEntry.ModifiedBy = dataContext.Users.SingleOrDefault(x => x.Username == User.Identity.Name);
                dictionaryEntry.ModificationDate = DateTime.Now;
                dataContext.SaveChanges();

                RenderDictionaryResult(false, false, false, true);

                MaintainScrollPosition();
                ResetEditModal();
                CloseModal(mdlEditDictionaryEntry);
                ShowNotification(IconCheck, "Edit success", "The dictionary entry has been updated.", false);
            }
            catch (Exception ex)
            {
                ShowNotification();
            }
        }

        /// <summary>
        /// Closes edit modal and resets input fields.
        /// </summary>
        protected void BtnCancelEdit_Click(object sender, EventArgs e)
        {
            MaintainScrollPosition();
            ResetEditModal();
            CloseModal(mdlEditDictionaryEntry);
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
                ddlEditLanguage.Items.Add(new ListItem(language.Name, language.Id.ToString()));
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

            // Populate dictionary container
            if ((filteredEntries == null) || (filteredEntries.Count == 0))
            {
                lvDictionary.DataSource = null;
                lvDictionary.DataBind();

                dictionaryDataPagerInfo.InnerText = "";
            }
            else
            {
                lvDictionary.DataSource = filteredEntries;
                lvDictionary.DataBind();

                dictionaryDataPagerInfo.InnerText = $"Showing {dictionaryDataPager.StartRowIndex + 1} - {dictionaryDataPager.StartRowIndex + lvDictionary.Items.Count}";
            }

            dictionaryDataPager.Visible = filteredEntries.Count == 0 ? false : true;

            // Update entry count text
            entryCountText.InnerText = filteredEntries.Count + (filteredEntries.Count == 1 ? " entry " : " entries ") + "found";
        }

        protected void OnPagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            dictionaryDataPager.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            RenderDictionaryResult(false, false, false, true);

            dictionaryDataPagerInfo.InnerText = $"Showing {e.StartRowIndex + 1} - {e.StartRowIndex + lvDictionary.Items.Count}";
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
        /// Resets input fields in Edit modal.
        /// </summary>
        protected void ResetEditModal()
        {
            txtDictionaryEntryId.InnerText = "";
            txtEditSlang.InnerText = "";
            txtEditTranslation.InnerText = "";
            ddlEditLanguage.SelectedIndex = 0;
            txtEditExample.Value = "";
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

        protected void CloseModal(HtmlGenericControl modal)
        {
            HtmlGenericControl body = Master.FindControl("body") as HtmlGenericControl;
            body.Style.Add("overflow-y", "auto");
            modal.Style.Add("display", "none");
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

        /// <summary>
        /// Maintains scroll position of the dictionary container.
        /// </summary>
        protected void MaintainScrollPosition()
        {
            string scriptScrollToView = @"window.onload = function scrollToView() {" +
                $"$('#divDictionary').scrollTop({hfScrollPosition.Value});" +
                "}";
            ClientScript.RegisterStartupScript(this.GetType(), "ScrollToView", scriptScrollToView, true);
        }
    }
}