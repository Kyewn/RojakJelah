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
        private string[] sortEntries = new string[] {
            "Date (dsc.)", "Date (asc.)", "Author"
        };
        private string[] filterEntries = new string[] {
            "All issues", "Duplicate entries", "Incorrect entries", "Inappropriate entries", "Other issues", "Resolved", "Closed"
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
            
            pageState = ViewState["pageState"] as ReportsPageState ?? new ReportsPageState();

            // If page first loads, otherwise ignores POST requests
            if (!Page.IsPostBack)
            {
                //  cboSorts
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

                ddlLimitRows.Items.Clear();
                foreach (int entry in limitRowEntries)
                {
                    ddlLimitRows.Items.Add(entry.ToString());
                }

                //  listItemContainer
                var limitRowCount = limitRowEntries[ddlLimitRows.SelectedIndex];
                List<Report> reportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 1)
                    .Take(limitRowCount)
                    .OrderByDescending((x) => x.CreationDate).ToList();
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

        protected void CboSortAndFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIndex = cboFilter.SelectedIndex;
            var sortIndex = cboSorts.SelectedIndex;
            var limitRowCount = limitRowEntries[ddlLimitRows.SelectedIndex];
            List<Report> reportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 1).Take(limitRowCount).ToList();
            List<Report> resolvedReportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 2).Take(limitRowCount).ToList();
            List<Report> closedReportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 3).Take(limitRowCount).ToList();
            List<Report> chosenList = new List<Report>();

            if (selectedIndex == filterEntries.Length - 2)
            {
                //  Resolved
                chosenList.AddRange(resolvedReportList);
            }
            else if (selectedIndex == filterEntries.Length - 1)
            {
                //  Closed
                chosenList.AddRange(closedReportList);
            }
            else
            {
                //  Open
                chosenList.AddRange(reportList);
            }

            if (selectedIndex == 1)
            {
                //  Duplicate entries
                chosenList = chosenList.Where((x) => x.ReportCategory.Id == 1).ToList();
            }
            else if (selectedIndex == 2)
            {
                //  Incorrect entries
                chosenList = chosenList.Where((x) => x.ReportCategory.Id == 2).ToList();
            }
            else if (selectedIndex == 3)
            {
                //  Inappropriate entries
                chosenList = chosenList.Where((x) => x.ReportCategory.Id == 3).ToList();
            } 
            else if (selectedIndex == 4)
            {
                //  Other entries
                chosenList = chosenList.Where((x) => x.ReportCategory.Id == 4).ToList();
            }

            List<Report> finalList = !String.IsNullOrEmpty(txtSearch.Text) ?
                HandleFilterList(txtSearch.Text, chosenList) : chosenList;

            txtSelectedListItem.Text = ""; // Reset selected list item
            pageState._currentList.Clear();
            
            List<Report> orderedList = new List<Report>();

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
            List<Report> reportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 1).Take(limitRowCount).ToList();
            List<Report> resolvedReportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 2).Take(limitRowCount).ToList();
            List<Report> closedReportList = dataContext.Reports.Where(x => x.ReportStatus.Id == 3).Take(limitRowCount).ToList();
            List<Report> filteredList = new List<Report>();

            if (cboFilter.SelectedIndex == filterEntries.Length - 2)
            {
                filteredList = HandleFilterList(searchKeys, resolvedReportList);
            }
            else if (cboFilter.SelectedIndex == filterEntries.Length - 1)
            {
                filteredList = HandleFilterList(searchKeys, closedReportList);
            }
            else
            {
                filteredList = HandleFilterList(searchKeys, reportList);
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
            cboSorts.SelectedIndex = 0;
            CboSortAndFilter_SelectedIndexChanged(sender, e);
        }

        protected void BtnResolve_Click(object sender, EventArgs e)
        {
            string notificationTitle, notificationMessage;

            try
            {
                //  Update report status
                Report resolvedRecord = dataContext.Reports.SingleOrDefault(x => x.Id.ToString() == lblId.InnerText);
                resolvedRecord.ReportStatus = dataContext.ReportStatuses.SingleOrDefault(x => x.Id == 2);
                resolvedRecord.ModifiedBy = dataContext.Users.Where((x) => x.Username.ToLower() == Page.User.Identity.Name).First();
                resolvedRecord.ModificationDate = DateTime.Now;
                dataContext.SaveChanges();

                //  Update UI
                CboSortAndFilter_SelectedIndexChanged(sender, e);
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
                closedRecord.ModifiedBy = dataContext.Users.Where((x) => x.Username.ToLower() == Page.User.Identity.Name).First();
                closedRecord.ModificationDate = DateTime.Now;
                dataContext.SaveChanges();

                //  Update UI
                CboSortAndFilter_SelectedIndexChanged(sender, e);
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
                CboSortAndFilter_SelectedIndexChanged(sender, e);
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
                                <span>{item.DictionaryEntry?.Slang?.WordValue ?? "-"}</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Translation</span>
                                <span>{item.DictionaryEntry?.Translation?.WordValue ?? "-"}</span>
                            </div>
                           <div class='itemDetail'>
                                <span>Issue Category</span>
                                <span>{item.ReportCategory.Name}</span>
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
                                <span>Description</span>
                                <span>{item.Description ?? "-"}</span>
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
                    lblSlang.InnerText = selectedItem.DictionaryEntry?.Slang?.WordValue ?? "-";
                    lblTranslation.InnerText = selectedItem.DictionaryEntry?.Translation?.WordValue ?? "-";
                    lblCategory.InnerText = selectedItem.ReportCategory.Name;
                    lblAuthor.InnerText = selectedItem.CreatedBy?.Username ?? "-";
                    lblDate.InnerText = selectedItem.CreationDate.ToShortDateString();
                    lblModifyBy.InnerText = selectedItem.ModifiedBy?.Username ?? "-";
                    lblModifyDate.InnerText = selectedItem.ModificationDate.ToShortDateString();
                    lblDescription.InnerText = selectedItem.Description ?? "-";
                }
                else
                {
                    //  If user didn't interact with list items yet
                    //  Select first item details and display menu
                    lblId.InnerText = pageState._currentList[0].Id.ToString();
                    lblSlang.InnerText = pageState._currentList[0].DictionaryEntry?.Slang?.WordValue ?? "-";
                    lblTranslation.InnerText = pageState._currentList[0].DictionaryEntry?.Translation?.WordValue ?? "-";
                    lblCategory.InnerText = pageState._currentList[0].ReportCategory.Name;
                    lblAuthor.InnerText = pageState._currentList[0].CreatedBy?.Username ?? "-";
                    lblDate.InnerText = pageState._currentList[0].CreationDate.ToShortDateString();
                    lblModifyBy.InnerText = pageState._currentList[0].ModifiedBy?.Username ?? "-";
                    lblModifyDate.InnerText = pageState._currentList[0].ModificationDate.ToShortDateString();
                    lblDescription.InnerText = pageState._currentList[0].Description ?? "-";
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

        private List<Report> HandleFilterList(string searchKeys, List<Report> reportList)
        {
            //  Filters - search by slang, translation, date, author, description
            //  Duplicate entries, Incorrect entries, Inappropriate entries, Other issues, Resolved, Closed
         return reportList
                .Where((x) => (x.DictionaryEntry?.Slang?.WordValue.ToLower().Contains(searchKeys.ToLower()) ?? false) ||
                (x.DictionaryEntry?.Translation?.WordValue.ToLower().Contains(searchKeys.ToLower()) ?? false) ||
                (x.CreatedBy?.Username.ToLower().Contains(searchKeys.ToLower()) ?? false) ||
                (x.CreationDate.ToShortDateString().ToLower().Contains(searchKeys.ToLower())) ||
                (x.Description != null && x.Description.ToLower().Contains(searchKeys.ToLower()))).ToList();
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