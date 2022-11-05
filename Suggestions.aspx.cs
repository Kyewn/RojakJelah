using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Web.UI.HtmlControls;
using RojakJelah.Database;
using RojakJelah.Database.Entity;

namespace RojakJelah
{
    [Serializable]
    public class SuggestionsPageState
    {
        public List<Suggestion> _currentList = new List<Suggestion>();
    }

    public partial class Suggestions : System.Web.UI.Page
    {
        DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);
        private SuggestionsPageState pageState;
        private string[] filterEntries = new string[] { 
            "Slang", "Translation", "Author", "Date (asc.)", "Date (dsc.)", "Approved", "Rejected"
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            // Hide notification message that may be open and isnt closed by backend yet
            // (front end js doesnt affect backend state, so even if it is clicked backend still think its open)
            notification.Style.Add("display", "none");

            pageState = ViewState["pageState"] as SuggestionsPageState ?? new SuggestionsPageState();
            
            // If page first loads, otherwise ignores POST requests
            if (!Page.IsPostBack)
            {    
                //  cboFilter
                cboFilter.Items.Clear();
                foreach (string entry in filterEntries)
                {
                    cboFilter.Items.Add(entry);
                }

                //  cboEditLanguage
                cboEditLanguage.Items.Clear();
                foreach (Language entry in dataContext.Languages)
                {
                    cboEditLanguage.Items.Add(entry.Name);
                }

                //  listItemContainer
                List<Suggestion> suggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 1).ToList();
                pageState._currentList.AddRange(suggestionList);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Populate list container based on saved list state
            handlePopulateItems();
            // Update previous list state to latest state based on conditions (cobFilter, txtSearch, CRUD ops) for next postback
            ViewState["pageState"] = pageState;
        }

        protected void cboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIndex = cboFilter.SelectedIndex;
            List<Suggestion> suggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 1).ToList();
            List<Suggestion> approvedSuggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 2).ToList();
            List<Suggestion> rejectedSuggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 3).ToList();
            List<Suggestion> chosenList = new List<Suggestion>();

            if (cboFilter.SelectedIndex == filterEntries.Length - 2)
            {
                chosenList.AddRange(approvedSuggestionList);
            }
            else if (cboFilter.SelectedIndex == filterEntries.Length - 1)
            {
                chosenList.AddRange(rejectedSuggestionList);
            }
            else
            {
                chosenList.AddRange(suggestionList);
            }

            List<Suggestion> finalList = !String.IsNullOrEmpty(txtSearch.Text) ? 
                handleFilterList(selectedIndex, txtSearch.Text, chosenList) : chosenList;

            txtSelectedListItem.Text = ""; // Reset selected list item
            pageState._currentList.Clear();
            if (selectedIndex == filterEntries.Length - 4 || selectedIndex == filterEntries.Length - 3)
            {
                List<Suggestion> orderedList = new List<Suggestion>();

                if (selectedIndex == filterEntries.Length - 4)
                {
                    //  Date (asc.)
                    orderedList.AddRange(finalList.OrderBy((x) => x.CreationDate));
                }
                else
                {
                    //  Date (dsc.)
                    orderedList.AddRange(finalList.OrderByDescending((x) => x.CreationDate));
                }

                pageState._currentList.AddRange(orderedList);
            } 
            else
            {
                pageState._currentList.AddRange(finalList);
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            txtSelectedListItem.Text = ""; // Reset selected list item
            var searchKeys = txtSearch.Text.ToLower().Trim();
            var filter = cboFilter.SelectedIndex;
            List<Suggestion> suggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 1).ToList();
            List<Suggestion> approvedSuggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 2).ToList();
            List<Suggestion> rejectedSuggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 3).ToList();
            List<Suggestion> filteredList = new List<Suggestion>();

            if (cboFilter.SelectedIndex == filterEntries.Length - 2) {
              filteredList = handleFilterList(filter, searchKeys, approvedSuggestionList);
            } else if (cboFilter.SelectedIndex == filterEntries.Length - 1) {
              filteredList = handleFilterList(filter, searchKeys, rejectedSuggestionList);
            } else {
              filteredList = handleFilterList(filter, searchKeys, suggestionList);
            }

            pageState._currentList.Clear();
            if (String.IsNullOrEmpty(searchKeys) || searchKeys.Length == 0) {
                if (cboFilter.SelectedIndex == filterEntries.Length - 2)
                {
                    pageState._currentList.AddRange(approvedSuggestionList);
                }
                else if (cboFilter.SelectedIndex == filterEntries.Length - 1)
                {
                    pageState._currentList.AddRange(rejectedSuggestionList);
                }
                else
                {
                    pageState._currentList.AddRange(suggestionList);
                }
            }
            else {
                pageState._currentList.AddRange(filteredList);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSelectedListItem.Text = String.Empty;
            txtSearch.Text = String.Empty;
            cboFilter.SelectedIndex = 0;
            cboFilter_SelectedIndexChanged(sender, e);
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            var selectedItemLanguageIndex = cboEditLanguage.Items.IndexOf(cboEditLanguage.Items.FindByValue(lblLanguage.InnerText));
            txtEditId.InnerText = lblId.InnerText;
            txtEditSlang.Text = lblSlang.InnerText;
            txtEditTranslation.Text = lblTranslation.InnerText;
            txtEditExample.InnerText = lblExample.InnerText;
            cboEditLanguage.SelectedIndex = selectedItemLanguageIndex;

            editModalWindow.Style.Add("animation", "fadeIn .3s ease-out forwards");
        }

        protected void btnEditCancel_Click(object sender, EventArgs e)
        {
            hideModal();
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;

            try
            {
                //  Insert record in dictionary
                var wordList = dataContext.Words.ToList();
                var languageList = dataContext.Languages.ToList();
                var userList = dataContext.Users.ToList();
            
                // Get slang and translation from Word table
                Word wordSlang = wordList.SingleOrDefault(x => x.WordValue.ToLower() == lblSlang.InnerText.ToLower());
                Word wordTranslation = wordList.SingleOrDefault(x => x.WordValue.ToLower() == lblTranslation.InnerText.ToLower());

                // Get author from User table
                User userAuthor = userList.SingleOrDefault(x => x.Username.ToLower() == lblAuthor.InnerText);

                DictionaryEntry newEntry = new DictionaryEntry()
                {
                    Slang = wordSlang,
                    Translation = wordTranslation,
                    Example = lblExample.InnerText,
                    CreatedBy = userAuthor,
                    CreationDate = DateTime.Now,
                    ModifiedBy = userAuthor,
                    ModificationDate = DateTime.Now,
                };

                dataContext.DictionaryEntries.Add(newEntry);
                dataContext.SaveChanges();

                //  Update suggestion status
                Suggestion approvedRecord = dataContext.Suggestions.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                approvedRecord.SuggestionStatus = dataContext.SuggestionStatuses.SingleOrDefault(x => x.Id == 2);
                dataContext.SaveChanges();

                //  Update UI
                Suggestion approvedUIRecord = pageState._currentList.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                approvedUIRecord.SuggestionStatus = dataContext.SuggestionStatuses.SingleOrDefault(x => x.Id == 2);
                pageState._currentList.Remove(approvedUIRecord);
                txtSelectedListItem.Text = ""; // Reset selected list item index

                // Send notification message
                notificationTitle = "Suggestion approved";
                notificationMessage = "The suggestion has been added to the dictionary.";
                ShowNotification(notificationTitle, notificationMessage, false);
            } catch (Exception ex)
            {
                // Display error notification
                notificationTitle = "Unknown error";
                notificationMessage = "An unexpected error has occured, please contact support.";

                ShowNotification(notificationTitle, notificationMessage, true);

                Debug.WriteLine(ex.ToString());
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;

            try
            {
                //  Update suggestion status
                Suggestion rejectedRecord = dataContext.Suggestions.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                rejectedRecord.SuggestionStatus = dataContext.SuggestionStatuses.SingleOrDefault(x => x.Id == 3);
                dataContext.SaveChanges();

                //  Update UI
                Suggestion rejectedUIRecord = pageState._currentList.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                rejectedUIRecord.SuggestionStatus = dataContext.SuggestionStatuses.SingleOrDefault(x => x.Id == 3);
                pageState._currentList.Remove(rejectedUIRecord);
                txtSelectedListItem.Text = ""; // Reset selected list item index

                // Send notification message
                notificationTitle = "Suggestion rejected";
                notificationMessage = "The suggestion has been discarded, but can still be viewed in the list using filters.";
                ShowNotification(notificationTitle, notificationMessage, false);
            }
            catch (Exception ex)
            {
                // Display error notification
                notificationTitle = "Unknown error";
                notificationMessage = "An unexpected error has occured, please contact support.";

                ShowNotification(notificationTitle, notificationMessage, true);

                Debug.WriteLine(ex.ToString());
            }
        }

        protected void btnEditConfirm_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;
            try
            {
                //  Update information
                Language updatedLanguage = dataContext.Languages.SingleOrDefault(x => x.Name == cboEditLanguage.SelectedValue);
                Suggestion targetSuggestion = dataContext.Suggestions.SingleOrDefault(x => x.Id.ToString() == txtEditId.InnerText);
                targetSuggestion.Slang = txtEditSlang.Text.Trim();
                targetSuggestion.Translation = txtEditTranslation.Text.Trim();
                targetSuggestion.Language = updatedLanguage;
                targetSuggestion.Example = txtEditExample.Value.Trim();
                dataContext.SaveChanges();

                //  Update UI - update record in pageState._currentList
                var targetRecord = pageState._currentList.SingleOrDefault(x => x.Id.ToString() == txtEditId.InnerText);
                targetRecord.Slang = txtEditSlang.Text.Trim();
                targetRecord.Translation = txtEditTranslation.Text.Trim();
                targetRecord.Language = updatedLanguage;
                targetRecord.Example = txtEditExample.Value.Trim();

                //  Close Modal
                hideModal();

                //  Send success message
                notificationTitle = "Update successful";
                notificationMessage = "The record has been updated.";
                ShowNotification(notificationTitle, notificationMessage, false);
            }
            catch (Exception ex)
            {
                // Display error notification
                notificationTitle = "Unknown error";
                notificationMessage = "An unexpected error has occured, please contact support.";

                ShowNotification(notificationTitle, notificationMessage, true);

                Debug.WriteLine(ex.ToString());
            }
        }

        private void addNoDataListItem()
        {
            var listItemHTML = @"                    
                    <div class='listItem noData'>
                        <i class='fa-solid fa-circle-exclamation'></i>
                        <span>No data found</span>
                    </div>";

            var listItem = new LiteralControl(listItemHTML);
            listItemContainer.Controls.Add(listItem);
        }
        
        private void addListItem(Suggestion item)
        {
            var listItemHTML = $@"
                    <div class='listItem'>
                        <div class='topRow'>
                            <div class='itemDetail'>
                                <span>ID</span>
                                <span>{item.Id}</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Slang</span>
                                <span>{item.Slang}</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Translation</span>
                                <span>{item.Translation}</span>
                            </div>
                           <div class='itemDetail'>
                                <span>Language</span>
                                <span>{item.Language.Name}</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Created by</span>
                                <span>{item.CreatedBy.Username}</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Created at</span>
                                <span>{item.CreationDate.ToShortDateString()}</span>
                            </div>
                        </div>
                        <div class='bottomRow'>
                            <div class='itemDetail'>
                                <span>Example</span>
                                <span>{item.Example}</span>
                            </div>
                        </div>
                    </div>";

            var listItem = new LiteralControl(listItemHTML);
            listItemContainer.Controls.Add(listItem);
        }

        private void handlePopulateItems()
        {
            if (pageState._currentList.Count == 0)
            {
                //  Show no data message
                addNoDataListItem();

                //  Hide menu controls
                buttonContainer.Style.Add("visibility", "hidden");
                detailContainer.Style.Add("visibility", "hidden");
                btnEdit.Style.Add("visibility", "hidden");
            }
            else
            {
                // Populate container
                foreach (var item in pageState._currentList)
                {
                    addListItem(item);
                }

                if (!String.IsNullOrEmpty(txtSelectedListItem.Text))
                {
                    //  If user interacted with list items
                    //  Show item details of selected list item
                    List<Suggestion> suggestionList = dataContext.Suggestions.ToList();
                    Suggestion selectedItem = suggestionList.Find((x) => x.Id.ToString() == txtSelectedListItem.Text);

                    lblId.InnerText = selectedItem.Id.ToString();
                    lblSlang.InnerText = selectedItem.Slang;
                    lblTranslation.InnerText = selectedItem.Translation;
                    lblLanguage.InnerText = selectedItem.Language.Name;
                    lblAuthor.InnerText = selectedItem.CreatedBy.Username;
                    lblDate.InnerText = selectedItem.CreationDate.ToShortDateString();
                    lblExample.InnerText = selectedItem.Example;
                }
                else
                {
                    //  If user didn't interact with list items yet
                    //  Select first item details and display menu
                    lblId.InnerText = pageState._currentList[0].Id.ToString();
                    lblSlang.InnerText = pageState._currentList[0].Slang;
                    lblTranslation.InnerText = pageState._currentList[0].Translation;
                    lblLanguage.InnerText = pageState._currentList[0].Language.Name;
                    lblAuthor.InnerText = pageState._currentList[0].CreatedBy.Username;
                    lblDate.InnerText = pageState._currentList[0].CreationDate.ToShortDateString();
                    lblExample.InnerText = pageState._currentList[0].Example;
                }

                //  Show menu controls
                detailContainer.Style.Add("visibility", "visible");

                //  Toggle button interaction based on filter
                if (cboFilter.SelectedIndex == filterEntries.Length - 2 || cboFilter.SelectedIndex == filterEntries.Length - 1)
                {
                    buttonContainer.Style.Add("visibility", "hidden");
                    btnEdit.Style.Add("visibility", "hidden");
                    buttonContainer.Style.Add("display", "none");
                    btnEdit.Style.Add("display", "none");
                }
                else
                {
                    buttonContainer.Style.Add("visibility", "visible");
                    btnEdit.Style.Add("visibility", "visible");
                    buttonContainer.Style.Add("display", "flex");
                    btnEdit.Style.Add("display", "block");
                }
            }
        }

        private void hideModal()
        {
            editModalWindow.Style.Remove("animation");
            editModalWindow.Style.Add("opacity", "0");
            editModalWindow.Style.Add("visibility", "hidden");

        }

        private List<Suggestion> handleFilterList(int filter, string searchKeys, List<Suggestion> suggestionList)
        {
            List<Suggestion> filteredList = new List<Suggestion>();
            List<Suggestion> orderedList = new List<Suggestion>();

            switch (filter)
            {
                case 0:
                    //  Slang
                    filteredList.AddRange(suggestionList.Where((x) => x.Slang.ToLower().Contains(searchKeys)));
                    break;
                case 1:
                    //  Translation
                    filteredList.AddRange(suggestionList.Where((x) => x.Translation.ToLower().Contains(searchKeys)));
                    break;
                case 2:
                    //  Created by
                    filteredList.AddRange(suggestionList.Where((x) => x.CreatedBy.Username.ToLower().Contains(searchKeys)));
                    break;
                case 3:
                    //  Date (asc.)
                    orderedList.AddRange(suggestionList.OrderBy((x) => x.CreationDate));
                    filteredList.AddRange(orderedList.Where((x) => x.CreationDate.ToShortDateString().Contains(searchKeys)));
                    break;
                case 4:
                    //  Date (dsc.)
                    orderedList.AddRange(suggestionList.OrderByDescending((x) => x.CreationDate));
                    filteredList.AddRange(orderedList.Where((x) => x.CreationDate.ToShortDateString().Contains(searchKeys)));
                    break;
                case 5:
                    //  Approved
                    filteredList.AddRange(suggestionList.Where((x) => x.Slang.ToLower().Contains(searchKeys)));
                    break;
                case 6:
                    //  Rejected
                    filteredList.AddRange(suggestionList.Where((x) => x.Slang.ToLower().Contains(searchKeys)));
                    break;
            }

            return filteredList;
        }

        /// <summary>
        /// Displays error notification popup.
        /// </summary>
        /// <param name="title">Title of error.</param>
        /// <param name="message">Error message.</param>
        protected void ShowNotification(string title, string message, bool isError)
        {
            Debug.WriteLine(title);
            notificationTitle.InnerText = title;
            notificationMessage.InnerHtml = message;
            notification.Style.Add("display", "block");
            if (isError)
            {
                notification.Style.Add("background-color", "var(--primary-color)");
            }
        }
    }
}