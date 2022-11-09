using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using RojakJelah.Database;
using RojakJelah.Database.Entity;

namespace RojakJelah
{
    [Serializable]
    public class ReportsPageState
    {
        public List<Report> _currentList = new List<Report>();
    }

    public partial class Reports: System.Web.UI.Page
    {
        DataContext dataContext = new DataContext(ConnectionStrings.RojakJelahConnection);
        private ReportsPageState pageState;
        private string[] filterEntries = new string[] {
            "Duplicate entries", "Incorrect entries", "Inappropriate entries", "Author", "Date (asc.)", "Date (dsc.)", "Resolved", "Closed"
        };

        //  FontAwesome icons
        private String IconExclamation = "fa-solid fa-circle-exclamation";
        private String IconCheck = "fa-solid fa-check";
        protected void Page_Load(object sender, EventArgs e)
        {
            // Hide notification message that may be open and isnt closed by backend yet
            // (front end js doesnt affect backend state, so even if it is clicked backend still think its open)
            notification.Style.Add("display", "none");
            
            pageState = ViewState["pageState"] as ReportsPageState ?? new ReportsPageState();

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
                List<Report> reportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 1).ToList();
                pageState._currentList.AddRange(reportList);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Populate list container based on saved list state
            HandlePopulateItems();
            // Update previous list state to latest state based on conditions (cobFilter, txtSearch, CRUD ops) for next postback
            ViewState["pageState"] = pageState;
        }

        protected void CboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIndex = cboFilter.SelectedIndex;
            List<Report> reportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 1).ToList();
            List<Report> resolvedReportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 2).ToList();
            List<Report> closedReportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 3).ToList();
            List<Report> chosenList = new List<Report>();

            if (cboFilter.SelectedIndex == filterEntries.Length - 2)
            {
                chosenList.AddRange(resolvedReportList);
            }
            else if (cboFilter.SelectedIndex == filterEntries.Length - 1)
            {
                chosenList.AddRange(closedReportList);
            }
            else
            {
                chosenList.AddRange(reportList);
            }

            if (cboFilter.SelectedIndex == 0)
            {
                chosenList = chosenList.Where((x) => x.ReportCategory.Id == 1).ToList();
            }
            else if (cboFilter.SelectedIndex == 1)
            {
                chosenList = chosenList.Where((x) => x.ReportCategory.Id == 2).ToList();
            }
            else if (cboFilter.SelectedIndex == 2)
            {
                chosenList = chosenList.Where((x) => x.ReportCategory.Id == 3).ToList();
            }

            List<Report> finalList = !String.IsNullOrEmpty(txtSearch.Text) ?
                HandleFilterList(selectedIndex, txtSearch.Text, chosenList) : chosenList;

            txtSelectedListItem.Text = ""; // Reset selected list item
            pageState._currentList.Clear();
            if (selectedIndex == filterEntries.Length - 4 || selectedIndex == filterEntries.Length - 3)
            {
                List<Report> orderedList = new List<Report>();

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

        protected void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            txtSelectedListItem.Text = ""; // Reset selected list item
            var searchKeys = txtSearch.Text.ToLower().Trim();
            var filter = cboFilter.SelectedIndex;
            List<Report> reportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 1).ToList();
            List<Report> resolvedReportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 2).ToList();
            List<Report> closedReportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 3).ToList();
            List<Report> filteredList = new List<Report>();

            if (cboFilter.SelectedIndex == filterEntries.Length - 2)
            {
                filteredList = HandleFilterList(filter, searchKeys, resolvedReportList);
            }
            else if (cboFilter.SelectedIndex == filterEntries.Length - 1)
            {
                filteredList = HandleFilterList(filter, searchKeys, closedReportList);
            }
            else
            {
                filteredList = HandleFilterList(filter, searchKeys, reportList);
            }

            pageState._currentList.Clear();
            if (String.IsNullOrEmpty(searchKeys) || searchKeys.Length == 0)
            {
                if (cboFilter.SelectedIndex == filterEntries.Length - 2)
                {
                    pageState._currentList.AddRange(resolvedReportList);
                }
                else if (cboFilter.SelectedIndex == filterEntries.Length - 1)
                {
                    pageState._currentList.AddRange(closedReportList);
                }
                else
                {
                    pageState._currentList.AddRange(reportList);
                }
            }
            else
            {
                pageState._currentList.AddRange(filteredList);
            }
        }

        protected void BtnReset_Click(object sender, EventArgs e)
        {
            txtSelectedListItem.Text = String.Empty;
            txtSearch.Text = String.Empty;
            cboFilter.SelectedIndex = 0;
            CboFilter_SelectedIndexChanged(sender, e);
        }

        protected void BtnResolve_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;

            try
            {
                //  Update report status
                Report resolvedRecord = dataContext.Reports.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                resolvedRecord.ReportStatus = dataContext.ReportStatuses.SingleOrDefault(x => x.Id == 2);
                dataContext.SaveChanges();

                //  Update UI
                Report resolvedUIRecord = pageState._currentList.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                resolvedUIRecord.ReportStatus = dataContext.ReportStatuses.SingleOrDefault(x => x.Id == 2);
                pageState._currentList.Remove(resolvedUIRecord);
                txtSelectedListItem.Text = ""; // Reset selected list item index

                // Send notification message
                notificationTitle = "Issue resolved";
                notificationMessage = "The issue has been resolved successfully.";
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

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;

            try
            {
                //  Update report status
                Report closedRecord = dataContext.Reports.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                closedRecord.ReportStatus = dataContext.ReportStatuses.SingleOrDefault(x => x.Id == 3);
                dataContext.SaveChanges();

                //  Update UI
                Report closedUIRecord = pageState._currentList.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                closedUIRecord.ReportStatus = dataContext.ReportStatuses.SingleOrDefault(x => x.Id == 3);
                pageState._currentList.Remove(closedUIRecord);
                txtSelectedListItem.Text = ""; // Reset selected list item index

                // Send notification message
                notificationTitle = "Issue closed";
                notificationMessage = "The issue has been closed, but can still be restored later anytime.";
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

        protected void BtnRestore_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;

            try
            {
                //  Update report status
                Report targetRecord = dataContext.Reports.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                targetRecord.ReportStatus = dataContext.ReportStatuses.SingleOrDefault(x => x.Id == 1);
                dataContext.SaveChanges();

                //  Update UI
                Report targetUIRecord = pageState._currentList.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                targetUIRecord.ReportStatus = dataContext.ReportStatuses.SingleOrDefault(x => x.Id == 3);
                pageState._currentList.Remove(targetUIRecord);
                txtSelectedListItem.Text = ""; // Reset selected list item index

                // Send notification message
                notificationTitle = "Issue has been restored";
                notificationMessage = "The issue is restored for reviewing again.";
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

        private void AddListItem(Report item)
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
                                <span>{item.DictionaryEntry.Slang.WordValue}</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Translation</span>
                                <span>{item.DictionaryEntry.Translation.WordValue}</span>
                            </div>
                           <div class='itemDetail'>
                                <span>Issue Category</span>
                                <span>{item.ReportCategory.Name}</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Created by</span>
                                <span>{item.CreatedBy.Username}</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Created at</span>
                                <span>{item.CreationDate.ToShortDateString()}</span>
                            </div>
                        </div>";
            var bottomRowLiteralHTML = $@"
                        <div class='bottomRow'>
                            <div class='itemDetail'>
                                <span>Description</span>
                                <span>{item.Description ?? "None"}</span>
                            </div>
                        </div>";
            var topRowLiteralControl = new LiteralControl(topRowLiteralHTML);
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
                btnRestore.Style.Add("visibility", "hidden");
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
                    List<Report> reportList = dataContext.Reports.ToList();
                    Report selectedItem = reportList.Find((x) => x.Id.ToString() == txtSelectedListItem.Text);

                    lblId.InnerText = selectedItem.Id.ToString();
                    lblSlang.InnerText = selectedItem.DictionaryEntry.Slang.WordValue;
                    lblTranslation.InnerText = selectedItem.DictionaryEntry.Translation.WordValue;
                    lblCategory.InnerText = selectedItem.ReportCategory.Name;
                    lblAuthor.InnerText = selectedItem.CreatedBy.Username;
                    lblDate.InnerText = selectedItem.CreationDate.ToShortDateString();
                    lblDescription.InnerText = selectedItem.Description ?? "None";
                }
                else
                {
                    //  If user didn't interact with list items yet
                    //  Select first item details and display menu
                    lblId.InnerText = pageState._currentList[0].Id.ToString();
                    lblSlang.InnerText = pageState._currentList[0].DictionaryEntry.Slang.WordValue;
                    lblTranslation.InnerText = pageState._currentList[0].DictionaryEntry.Translation.WordValue;
                    lblCategory.InnerText = pageState._currentList[0].ReportCategory.Name;
                    lblAuthor.InnerText = pageState._currentList[0].CreatedBy.Username;
                    lblDate.InnerText = pageState._currentList[0].CreationDate.ToShortDateString();
                    lblDescription.InnerText = pageState._currentList[0].Description ?? "None";
                }

                // Apply selected css to selected list item
                var listItems = listItemContainer.Controls.OfType<Panel>().ToList();
                foreach (Panel item in listItems)
                {
                    if (item.ID == $"listItem{txtSelectedListItem.Text}")
                    {
                        item.CssClass += " listItem-selected";
                    }
                    else
                    {
                        item.CssClass = "listItem";
                    }
                }

                //  Show menu controls
                detailContainer.Style.Add("visibility", "visible");
                buttonContainer.Style.Add("display", "flex");
                buttonContainer.Style.Add("visibility", "visible");

                //  Toggle button interaction based on filter
                if (cboFilter.SelectedIndex == filterEntries.Length - 2 || cboFilter.SelectedIndex == filterEntries.Length - 1)
                {
                    btnResolve.Style.Add("display", "none");
                    btnClose.Style.Add("display", "none");

                    btnRestore.Style.Add("visibility", "visible");
                    btnRestore.Style.Add("display", "block");
                }
                else
                {
                    btnResolve.Style.Add("display", "block");
                    btnClose.Style.Add("display", "block");

                    btnRestore.Style.Add("visibility", "hidden");
                    btnRestore.Style.Add("display", "none");
                }
            }
        }

        private List<Report> HandleFilterList(int filter, string searchKeys, List<Report> reportList)
        {
            List<Report> filteredList = new List<Report>();
            List<Report> orderedList = new List<Report>();
            
            switch (filter)
            {
                case 0: case 1: case 2: case 6: case 7:
                    //  Duplicate entries, Incorrect entries, Inappropriate entries, Resolved, Closed
                    var searchForEntry = reportList
                        .Where((x) => x.DictionaryEntry.Slang.WordValue.ToLower().Contains(searchKeys.ToLower()) || 
                        x.DictionaryEntry.Translation.WordValue.ToLower().Contains(searchKeys.ToLower())).ToList();
                    var searchForDescription = reportList
                        .Where((x) => x.Description != null)
                        .Where((x) => x.Description.ToLower().Contains(searchKeys.ToLower())).ToList();

                    List<Report> chosenConditionList = new List<Report>();

                    if (searchForEntry.Count != 0)
                    {
                        chosenConditionList = searchForEntry;
                    } else if (searchForDescription.Count != 0)
                    {
                        chosenConditionList = searchForDescription;
                    }

                    filteredList.AddRange(chosenConditionList);
                    break;
                case 3:
                    //  Author
                    filteredList.AddRange(reportList
                        .Where((x) => x.CreatedBy.Username.ToLower().Contains(searchKeys.ToLower())));
                    break;
                case 4:
                    //  Date (asc.)
                    orderedList.AddRange(reportList.OrderBy((x) => x.CreationDate));
                    filteredList.AddRange(orderedList.Where((x) => x.CreationDate.ToShortDateString().Contains(searchKeys)));
                    break;
                case 5:
                    //  Date (dsc.)
                    orderedList.AddRange(reportList.OrderByDescending((x) => x.CreationDate));
                    filteredList.AddRange(orderedList.Where((x) => x.CreationDate.ToShortDateString().Contains(searchKeys)));
                    break;
            }

            return filteredList;
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