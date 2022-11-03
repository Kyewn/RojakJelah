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
    public class PageState
    {
        public List<Suggestion> _currentList = new List<Suggestion>();
    }

    public partial class Suggestions : System.Web.UI.Page
    {
        /*DataContext dataContext = new DataContext("server=localhost;user=root;database=xx;port=3306;password=******");*/
        DataContext dataContext = new DataContext("server=localhost;user=root;database=rojakjelah;port=3306;password=brian89564");
        private PageState pageState;
        private string[] filterEntries = new string[] { 
            "Slang", "Translation", "Author", "Date (asc.)", "Date (dsc.)"
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            pageState = ViewState["pageState"] as PageState ?? new PageState();
            
            // If page first loads, otherwise ignores POST requests
            if (!Page.IsPostBack)
            {    
                //  cboFilter
                cboFilter.Items.Clear();
                foreach (string entry in filterEntries)
                {
                    cboFilter.Items.Add(entry);
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
            List<Suggestion> suggestionList = !String.IsNullOrEmpty(txtSearch.Text) ? 
                handleFilterList(selectedIndex, txtSearch.Text) : dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 1).ToList();

            txtSelectedListItem.Text = ""; // Reset selected list item
            pageState._currentList.Clear();
            if (selectedIndex == filterEntries.Length - 2 || selectedIndex == filterEntries.Length - 1)
            {
                List<Suggestion> orderedList = new List<Suggestion>();

                if (selectedIndex == filterEntries.Length - 2)
                {
                    // Date (asc.)
                    orderedList.AddRange(suggestionList.OrderBy((x) => x.CreationDate));
                }
                else
                {
                    // Date (dsc.)
                    orderedList.AddRange(suggestionList.OrderByDescending((x) => x.CreationDate));
                }

                pageState._currentList.AddRange(orderedList);
            }
            else
            {
                pageState._currentList.AddRange(suggestionList);
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            txtSelectedListItem.Text = ""; // Reset selected list item
            var searchKeys = txtSearch.Text.ToLower().Trim();
            var filter = cboFilter.SelectedIndex;
            List<Suggestion> suggestionList = dataContext.Suggestions.Where(x => x.SuggestionStatus.Id == 1).ToList();
            List<Suggestion> filteredList = handleFilterList(filter, searchKeys);

            pageState._currentList.Clear();
            if (String.IsNullOrEmpty(searchKeys) || searchKeys.Length == 0) {
                pageState._currentList.AddRange(suggestionList);
            }
            else {
                pageState._currentList.AddRange(filteredList);
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            txtEditId.InnerText = lblId.InnerText;
            txtEditSlang.Text = lblSlang.InnerText;
            txtEditTranslation.Text = lblTranslation.InnerText;
            txtEditExample.InnerText = lblExample.InnerText;

            editModalWindow.Style.Add("animation", "fadeIn .3s ease-out forwards");
        }



        protected void btnEditCancel_Click(object sender, EventArgs e)
        {
            hideModal();
        }

        protected void btnEditConfirm_Click(object sender, EventArgs e)
        {
            string notifcationTitle, notificationMessage;
            try
            {
                //  Update information
                Suggestion targetSuggestion = dataContext.Suggestions.SingleOrDefault(x => x.Id.ToString() == txtEditId.InnerText);
                targetSuggestion.Slang = txtEditSlang.Text.Trim();
                targetSuggestion.Translation = txtEditTranslation.Text.Trim();
                targetSuggestion.Example = txtEditExample.Value.Trim();
                dataContext.SaveChanges();

                //  Update UI - update record in pageState._currentList
                var targetRecord = pageState._currentList.SingleOrDefault(x => x.Id.ToString() == txtEditId.InnerText);
                targetRecord.Slang = txtEditSlang.Text.Trim();
                targetRecord.Translation = txtEditTranslation.Text.Trim();
                targetRecord.Example = txtEditExample.Value.Trim();

                //  Close Modal
                hideModal();

                //  Send success message
                notifcationTitle = "Update successful";
                notificationMessage = "The record has been updated.";
                ShowNotification(notifcationTitle, notificationMessage, false);
            }
            catch (Exception ex)
            {
                // Display error notification
                notifcationTitle = "Unknown error";
                notificationMessage = "An unexpected error has occured, please contact support.";

                ShowNotification(notifcationTitle, notificationMessage, true);

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
                    lblAuthor.InnerText = pageState._currentList[0].CreatedBy.Username;
                    lblDate.InnerText = pageState._currentList[0].CreationDate.ToShortDateString();
                    lblExample.InnerText = pageState._currentList[0].Example;
                }

                //  Show menu controls
                buttonContainer.Style.Add("visibility", "visible");
                detailContainer.Style.Add("visibility", "visible");
                btnEdit.Style.Add("visibility", "visible");
            }
        }

        private void hideModal()
        {
            editModalWindow.Style.Remove("animation");
            editModalWindow.Style.Add("opacity", "0");
            editModalWindow.Style.Add("visibility", "hidden");

        }

        private List<Suggestion> handleFilterList(int filter, string searchKeys)
        {
            List<Suggestion> suggestionList = dataContext.Suggestions.ToList();
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