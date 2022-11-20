using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Web.UI.HtmlControls;
using System.Data.Entity;
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
            "All suggestions", "Approved", "Rejected"
        };
        private string[] sortEntries = new string[] { 
            "Date (dsc.)", "Date (asc.)", "Slang", "Translation", "Author"
        };
        private int[] limitRowEntries = new int[]
        {
            10,50,100
        };

        //  FontAwesome icons
        private String IconExclamation = "fa-solid fa-circle-exclamation";
        private String IconCheck = "fa-solid fa-check";
        protected void Page_Load(object sender, EventArgs e)
        {
            // Hide notification message that may be open and isnt closed by backend yet
            // (front end js doesnt affect backend state, so even if it is clicked backend still think its open)
            notification.Style.Add("display", "none");
            modalDialog.Style.Remove("animation"); //Remove modal animation to prevent unnecessary transition on postback

            pageState = ViewState["pageState"] as SuggestionsPageState ?? new SuggestionsPageState();
            
            // If page first loads, otherwise ignores POST requests
            if (!Page.IsPostBack)
            {    
                //  cboSort
                cboSorts.Items.Clear();
                foreach (string entry in sortEntries)
                {
                    cboSorts.Items.Add(entry);
                }
                
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

                ddlLimitRows.Items.Clear();
                foreach (int entry in limitRowEntries)
                {
                    ddlLimitRows.Items.Add(entry.ToString());
                }

                //  listItemContainer
                var limitRowCount = limitRowEntries[ddlLimitRows.SelectedIndex];
                List<Suggestion> suggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 1)
                    .Take(limitRowCount)
                    .OrderByDescending((x) => x.CreationDate).ToList();
                pageState._currentList.AddRange(suggestionList);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Populate list container based on saved list state
            HandlePopulateItems();
            // Update previous list state to latest state based on conditions (cobFilter, txtSearch, CRUD ops) for next postback
            ViewState["pageState"] = pageState;
        }

        protected void CboSortAndFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIndex = cboFilter.SelectedIndex;
            var sortIndex = cboSorts.SelectedIndex;
            var limitRowCount = limitRowEntries[ddlLimitRows.SelectedIndex];
            List<Suggestion> suggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 1).Take(limitRowCount).ToList();
            List<Suggestion> approvedSuggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 2).Take(limitRowCount).ToList();
            List<Suggestion> rejectedSuggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 3).Take(limitRowCount).ToList();
            List<Suggestion> chosenList = new List<Suggestion>();

            if (selectedIndex == filterEntries.Length - 2)
            {
                //  Approved
                chosenList.AddRange(approvedSuggestionList);
            }
            else if (selectedIndex == filterEntries.Length - 1)
            {
                //  Rejected
                chosenList.AddRange(rejectedSuggestionList);
            }
            else
            {
                //  Open
                chosenList.AddRange(suggestionList);
            }

            List<Suggestion> finalList = !String.IsNullOrEmpty(txtSearch.Text) ? 
                HandleFilterList(txtSearch.Text, chosenList) : chosenList;

            txtSelectedListItem.Text = ""; // Reset selected list item
            pageState._currentList.Clear();

            List<Suggestion> orderedList = new List<Suggestion>();

            if (sortIndex == 0)
            {
                //  Date (dsc.)
                orderedList.AddRange(finalList.OrderByDescending((x) => x.CreationDate));
            }
            else if (sortIndex == 1)
            {
                //  Date (asc.)
                orderedList.AddRange(finalList.OrderBy((x) => x.CreationDate));
            }
            else if (sortIndex == 2)
            {
                //  Slang
                orderedList.AddRange(finalList.OrderBy((x) => x.Slang));
            }
            else if (sortIndex == 3)
            {
                //  Translation
                orderedList.AddRange(finalList.OrderBy((x) => x.Translation));
            }
            else
            {
                //  Author
                orderedList.AddRange(finalList.OrderBy((x) => x.CreatedBy?.Username));
            }

            pageState._currentList.AddRange(orderedList);
        }

        protected void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            txtSelectedListItem.Text = ""; // Reset selected list item
            var searchKeys = txtSearch.Text.ToLower().Trim();
            var limitRowCount = limitRowEntries[ddlLimitRows.SelectedIndex];
            List<Suggestion> suggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 1).Take(limitRowCount).ToList();
            List<Suggestion> approvedSuggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 2).Take(limitRowCount).ToList();
            List<Suggestion> rejectedSuggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 3).Take(limitRowCount).ToList();
            List<Suggestion> filteredList = new List<Suggestion>();

            if (cboFilter.SelectedIndex == filterEntries.Length - 2) {
              filteredList = HandleFilterList(searchKeys, approvedSuggestionList);
            } else if (cboFilter.SelectedIndex == filterEntries.Length - 1) {
              filteredList = HandleFilterList(searchKeys, rejectedSuggestionList);
            } else {
              filteredList = HandleFilterList(searchKeys, suggestionList);
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

        protected void BtnReset_Click(object sender, EventArgs e)
        {
            txtSelectedListItem.Text = String.Empty;
            txtSearch.Text = String.Empty;
            cboFilter.SelectedIndex = 0;
            cboSorts.SelectedIndex = 0;
            CboSortAndFilter_SelectedIndexChanged(sender, e);
        }

        protected void BtnEdit_Click(object sender, EventArgs e)
        {
            var selectedItemLanguageIndex = cboEditLanguage.Items.IndexOf(cboEditLanguage.Items.FindByValue(lblLanguage.InnerText));
            txtEditId.InnerText = lblId.InnerText;
            txtEditSlang.Text = lblSlang.InnerText;
            txtEditTranslation.Text = lblTranslation.InnerText;
            txtEditExample.InnerText = lblExample.InnerText ?? "-";
            cboEditLanguage.SelectedIndex = selectedItemLanguageIndex;

            ShowModal(editModalWindow);
        }

        protected void BtnEditCancel_Click(object sender, EventArgs e)
        {
            HideModal();
        }

        protected void BtnAccept_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;

            //  Check if dictionary pair already exist
            DictionaryEntry existingSlangTranslationPair;

            try
            {
                //  Catch error thrown when system doesnt recognize new words that doesn't exist in DB
                //  Force the system to accept new words
                existingSlangTranslationPair = dataContext.DictionaryEntries
                    .Where(x => x.Slang.WordValue.ToLower() == lblSlang.InnerText.ToLower())
                    .Where(x => x.Translation.WordValue.ToLower() == lblTranslation.InnerText.ToLower()).First();
            }
            catch
            {
                existingSlangTranslationPair = null;
            }

            try
            {
                //  Insert record in dictionary
                var wordList = dataContext.Words.ToList();
                var languageList = dataContext.Languages.ToList();
                var userList = dataContext.Users.ToList();

                //  Get slang and translation from Word table
                Word wordSlang = wordList.SingleOrDefault(x => x.WordValue.ToLower() == lblSlang.InnerText.ToLower());
                Word wordTranslation = wordList.SingleOrDefault(x => x.WordValue.ToLower() == lblTranslation.InnerText.ToLower());
                //  Get author from User table
                User userAuthor = userList.SingleOrDefault(x => x.Username.ToLower() == lblAuthor.InnerText);
                Language slangLanguage = languageList.SingleOrDefault(x => x.Name.ToLower() == lblLanguage.InnerText.ToLower());
                Language translationLanguage = languageList.SingleOrDefault(x => x.Name.ToLower() == "english");

                //  Error catching
                //  Existing pair in dictionary
                if (existingSlangTranslationPair != null)
                {
                    // Display error notification
                    notificationTitle = "Word pair already exist";
                    notificationMessage = "The slang-translation pair already exist in the dictionary.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);

                    return;
                }
                
                //  Same slang-translation input
                if (lblTranslation.InnerText.ToLower() == lblSlang.InnerText.ToLower())
                {
                    // Display error notification
                    notificationTitle = "Identical words";
                    notificationMessage = "Slangs and translations must be different words.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);

                    return;
                }

                //  Input validated, process input
                //  Create word records if words do not exist in Words table
                using (DbContextTransaction transaction = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (wordSlang == null)
                        {
                            Word newWord = new Word()
                            {
                                WordValue = lblSlang.InnerText,
                                Language = slangLanguage
                            };
                            dataContext.Words.Add(newWord);
                            dataContext.SaveChanges();
                            wordSlang = newWord;

                        }

                        if (wordTranslation == null)
                        {
                            Word newWord = new Word()
                            {
                                WordValue = lblTranslation.InnerText,
                                Language = translationLanguage
                            };
                            dataContext.Words.Add(newWord);
                            dataContext.SaveChanges();
                            wordTranslation = newWord;
                        }

                        //  Create dictionary entry once words are confirmed to exist in Words table
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
                        transaction.Commit();
                    } catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
                
                //  Update UI
                CboSortAndFilter_SelectedIndexChanged(sender, e);
                txtSelectedListItem.Text = ""; // Reset selected list item index

                // Send notification message
                notificationTitle = "Suggestion approved";
                notificationMessage = "The suggestion has been added to the dictionary.";
                ShowNotification(IconCheck, notificationTitle, notificationMessage, false);
            } catch (Exception ex)
            {
                // Display error notification
                notificationTitle = "Unknown error";
                notificationMessage = "An unexpected error has occured, please contact support.";

                ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);

                Debug.WriteLine(ex.ToString());
            }
        }

        protected void BtnReject_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;

            try
            {
                //  Update suggestion status
                Suggestion rejectedRecord = dataContext.Suggestions.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                rejectedRecord.SuggestionStatus = dataContext.SuggestionStatuses.SingleOrDefault(x => x.Id == 3);
                dataContext.SaveChanges();

                //  Update UI
                CboSortAndFilter_SelectedIndexChanged(sender, e);
                txtSelectedListItem.Text = ""; // Reset selected list item index

                // Send notification message
                notificationTitle = "Suggestion rejected";
                notificationMessage = "The suggestion has been discarded, but can still be viewed in the list using filters.";
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

        protected void BtnEditConfirm_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;

            //  Check if dictionary pair already exist
            DictionaryEntry existingSlangTranslationPair;

            try
            {
                //  Catch error thrown when system doesnt recognize new words that doesn't exist in DB
                //  Force the system to accept new words
                existingSlangTranslationPair = dataContext.DictionaryEntries
                    .Where(x => x.Slang.WordValue.ToLower() == txtEditSlang.Text.ToLower())
                    .Where(x => x.Translation.WordValue.ToLower() == txtEditTranslation.Text.ToLower()).First();
            } catch 
            {
                existingSlangTranslationPair = null;
            }
            
            try
            {
                //  Error catching
                //  Required inputs empty
                if (String.IsNullOrWhiteSpace(txtEditSlang.Text) || String.IsNullOrEmpty(txtEditSlang.Text) || 
                    String.IsNullOrWhiteSpace(txtEditTranslation.Text) || String.IsNullOrEmpty(txtEditTranslation.Text))
                {
                    // Display error notification
                    notificationTitle = "Required fields are empty";
                    notificationMessage = "Slang and Translation fields must be filled.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);

                    return;
                }

                //  Same slang-translation input
                if (txtEditSlang.Text.ToLower() == txtEditTranslation.Text.ToLower())
                {
                    // Display error notification
                    notificationTitle = "Identical words";
                    notificationMessage = "Slangs and translations must be different words.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);

                    return;
                }

                //  Existing pair in dictionary
                if (existingSlangTranslationPair != null)
                {
                    // Display error notification
                    notificationTitle = "Word pair already exist";
                    notificationMessage = "The slang-translation pair already exist in the dictionary.";

                    ShowNotification(IconExclamation, notificationTitle, notificationMessage, true);

                    return;
                }

                //  Update information
                Language updatedLanguage = dataContext.Languages.SingleOrDefault(x => x.Name == cboEditLanguage.SelectedValue);
                Suggestion targetSuggestion = dataContext.Suggestions.SingleOrDefault(x => x.Id.ToString() == txtEditId.InnerText);
                targetSuggestion.Slang = txtEditSlang.Text.Trim();
                targetSuggestion.Translation = txtEditTranslation.Text.Trim();
                targetSuggestion.Language = updatedLanguage;
                targetSuggestion.Example = txtEditExample.Value.Trim();
                targetSuggestion.ModifiedBy = dataContext.Users.ToList().Find((x) => x.Username == Page.User.Identity.Name);
                targetSuggestion.ModificationDate = DateTime.Now;

                dataContext.SaveChanges();

                //  Update UI - update record in pageState._currentList
                var targetRecord = pageState._currentList.SingleOrDefault(x => x.Id.ToString() == txtEditId.InnerText);
                targetRecord.Slang = txtEditSlang.Text.Trim();
                targetRecord.Translation = txtEditTranslation.Text.Trim();
                targetRecord.Language = updatedLanguage;
                targetRecord.Example = txtEditExample.Value.Trim();
                targetRecord.ModifiedBy = dataContext.Users.ToList().Find((x) => x.Username == Page.User.Identity.Name);
                targetRecord.ModificationDate = DateTime.Now;

                //  Close Modal
                HideModal();

                //  Send success message
                notificationTitle = "Update successful";
                notificationMessage = "The record has been updated.";
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

        private void AddNoDataListItem()
        {
            var listItemHTML = @"                    
                    <div class='listItem noData'>
                        <i class='fa-solid fa-circle-exclamation'></i>
                        <span>No data found</span>
                    </div>";

            var listItem = new LiteralControl(listItemHTML);
            listItemContainer.Controls.Add(listItem);
        }
        
        private void AddListItem(Suggestion item)
        {
            Panel listItem = new Panel();
            var topRowLiteralHTML = $@"
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
                                <span>{item.CreatedBy?.Username ?? "-"}</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Created at</span>
                                <span>{item.CreationDate.ToShortDateString()}</span>
                            </div>
                        </div>";
            var bottomRowLiteralHTML = $@"
                        <div class='bottomRow'>
                            <div class='itemDetail'>
                                <span>Example</span>
                                <span>{item.Example ?? "-"}</span>
                            </div>
                        </div>";
            var topRowLiteralControl= new LiteralControl(topRowLiteralHTML);
            var bottomRowLiteralControl = new LiteralControl(bottomRowLiteralHTML);

            listItem.CssClass = "listItem";
            listItem.ID = $"listItem{item.Id}";
            listItem.ClientIDMode = ClientIDMode.Static;
            listItem.Controls.Add(topRowLiteralControl);
            listItem.Controls.Add(bottomRowLiteralControl);

            listItemContainer.Controls.Add(listItem);
        }

        private void HandlePopulateItems()
        {
            if (pageState._currentList.Count == 0)
            {
                //  Show no data message
                AddNoDataListItem();

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
                    AddListItem(item);
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
                    lblAuthor.InnerText = selectedItem.CreatedBy?.Username ?? "-";
                    lblDate.InnerText = selectedItem.CreationDate.ToShortDateString();
                    lblExample.InnerText = selectedItem.Example ?? "-";
                    lblModifyAuthor.InnerText = selectedItem.ModifiedBy?.Username ?? "-";
                    lblModifyDate.InnerText = selectedItem.ModificationDate.ToShortDateString();
                }
                else
                {
                    //  If user didn't interact with list items yet
                    //  Select first item details and display menu
                    lblId.InnerText = pageState._currentList[0].Id.ToString();
                    lblSlang.InnerText = pageState._currentList[0].Slang;
                    lblTranslation.InnerText = pageState._currentList[0].Translation;
                    lblLanguage.InnerText = pageState._currentList[0].Language.Name;
                    lblAuthor.InnerText = pageState._currentList[0].CreatedBy?.Username ?? "-";
                    lblDate.InnerText = pageState._currentList[0].CreationDate.ToShortDateString();
                    lblExample.InnerText = pageState._currentList[0].Example ?? "-";
                    lblModifyAuthor.InnerText = pageState._currentList[0].ModifiedBy?.Username ?? "-";
                    lblModifyDate.InnerText = pageState._currentList[0].ModificationDate.ToShortDateString();
                }

                // Apply selected css to selected list item
                var listItems = listItemContainer.Controls.OfType<Panel>().ToList();
                foreach(Panel item in listItems)
                {
                    if (item.ID == $"listItem{txtSelectedListItem.Text}")
                    {
                        item.CssClass += " listItem-selected";
                    } else
                    {
                        item.CssClass = "listItem";
                    }
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
                } else
                {
                    buttonContainer.Style.Add("visibility", "visible");
                    btnEdit.Style.Add("visibility", "visible");
                    buttonContainer.Style.Add("display", "flex");
                    btnEdit.Style.Add("display", "block");
                }
            }
        }

        private List<Suggestion> HandleFilterList(string searchKeys, List<Suggestion> suggestionList)
        {
            //  Filters - search by slang, translation, example, date, author, language
            //  Duplicate entries, Incorrect entries, Inappropriate entries, Other issues, Resolved, Closed
            return suggestionList
                   .Where((x) => (x.Slang.ToLower().Contains(searchKeys.ToLower())) ||
                   (x.Translation.ToLower().Contains(searchKeys.ToLower())) ||
                   (x.CreatedBy?.Username.ToLower().Contains(searchKeys.ToLower()) ?? false) ||
                   (x.CreationDate.ToShortDateString().ToLower().Contains(searchKeys.ToLower())) ||
                   (x.Language.Name.ToLower().Contains(searchKeys.ToLower())) ||
                   (x.Example != null && x.Example.ToLower().Contains(searchKeys.ToLower()))).ToList();
        }
        private void HideModal()
        {
            HtmlGenericControl body = Master.FindControl("body") as HtmlGenericControl;
            body.Style.Add("overflow-y", "auto");
            editModalWindow.Style.Add("display", "none");
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
            modalDialog.Style.Add("animation", "slideIn .3s ease-out forwards"); 
        }

        /// <summary>
        /// Displays status notification popup.
        /// </summary>
        /// <param name="icon">Icon of notification.</param>
        /// <param name="title">Title of status.</param>
        /// <param name="message">Status message.</param>
        /// <param name="isError">Is error notification or not.</param>
        protected void ShowNotification(string icon, string title, string message, bool isError = true)
        {
            if (isError)
            {
                notification.Style.Add("background-color", "var(--notification-error)");
            }
            else
            {
                notification.Style.Add("background-color", "var(--notification-success)");
            }

            notificationIcon.Attributes.Add("class", icon);
            notificationTitle.InnerText = title;
            notificationMessage.InnerHtml = message; 
            notification.Style.Add("display", "block");
        }
    }
}