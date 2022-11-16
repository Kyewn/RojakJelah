<%@ Page Title="Manage Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="RojakJelah.Reports" %>
<asp:Content ID="PageStylesheet" ContentPlaceHolderID="PageStylesheet" runat="server">
    <link href="<%= Page.ResolveUrl("~/Content/css/reports.css")%>" type="text/css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="PageJavaScript" ContentPlaceHolderID="PageJavaScript" runat="server">
    <script src="Content/js/reports.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="contentWrapper">
        <div class="header">
            <div class="divInline">
                <i class="fa-sharp fa-solid fa-flag"></i>
                <h3>User reports</h3>
            </div>
            <div class="divInline">
                <h6>Limit rows</h6>
                <asp:DropDownList ID="ddlLimitRows" runat="server" AutoPostBack="true" CssClass="cboFilter" OnSelectedIndexChanged="CboSortAndFilter_SelectedIndexChanged"></asp:DropDownList>
            </div>
        </div> 
        <div class="contentContainer">
            <div class="leftContainer">
                <div class="searchContainer">
                    <div class="filterContainer">
                        <div>
                            <h6>Sorts</h6>
                            <asp:DropDownList ID="cboSorts" class="cboFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CboSortAndFilter_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div>
                            <h6>Filters</h6>
                            <asp:DropDownList ID="cboFilter" class="cboFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CboSortAndFilter_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="searchGroup">
                        <div class="searchBarContainer">
                            <h6>Search</h6>
                            <asp:TextBox ID="txtSearch" class="txtSearch" placeholder="e.g. Hello World" runat="server" AutoPostBack="True" OnTextChanged="TxtSearch_TextChanged"></asp:TextBox>
                        </div>
                        <asp:ImageButton ID="btnSearch" ClientIDMode="Static" runat="server" ImageUrl="~/Content/image/searchIcon.svg" />
                    </div>
                    <asp:Button ID="btnReset" ClientIDMode="Static" runat="server" Text="Reset" OnClick="BtnReset_Click"></asp:Button>
                </div>
                <div ID="listItemContainer" class="listItemContainer" runat="server">
                    <asp:TextBox ID="txtSelectedListItem" ClientIDMode="Static" runat="server" AutoPostBack="True"></asp:TextBox>
                </div>
            </div>
            <div class="rightContainer">
                <div class="actionMenuWrapper">
                    <h6>Actions</h6>
                    <div ID="menuContainer" class="menuContainer" runat="server">
                        <div ID="buttonContainer" runat="server" class="buttonContainer">
                            <asp:Button ID="btnResolve" ClientIDMode="Static" runat="server" Text="Resolve" OnClick="BtnResolve_Click"></asp:Button>
                            <asp:Button ID="btnClose" ClientIDMode="Static" runat="server" Text="Close" OnClick="BtnClose_Click"></asp:Button>
                            <asp:Button ID="btnRestore" ClientIDMode="Static" runat="server" Text="Restore" OnClick="BtnRestore_Click" />
                        </div>
                        <div ID="detailContainer" runat="server" class="detailContainer">
                            <div class="itemDetail">
                                <h6>ID</h6>
                                <span id="lblId" ClientIDMode="Static" runat="server"></span>
                            </div>
                            <div class="inlineDetail">
                                <div class="itemDetail">
                                    <h6>Slang</h6>
                                    <span id="lblSlang" ClientIDMode="Static" runat="server"></span>
                                </div>
                                <div class="itemDetail">
                                    <h6>Translation</h6>
                                    <span id="lblTranslation" ClientIDMode="Static" runat="server"></span>
                                </div>                         
                            </div>                         
                            <div class="itemDetail">
                                <h6>Issue Category</h6>
                                <span id="lblCategory" ClientIDMode="Static" runat="server"></span>
                            </div>
                            <div class="itemDetail">
                                <h6>Description</h6>
                                <span id="lblDescription" ClientIDMode="Static" runat="server"></span>
                            </div>
                            <div class="inlineDetail">
                                <div class="itemDetail">
                                    <h6>Created by</h6>
                                    <span id="lblAuthor" ClientIDMode="Static" runat="server"></span>
                                </div>
                                <div class="itemDetail">
                                    <h6>Created at</h6>
                                    <span id="lblDate" ClientIDMode="Static" runat="server"></span>
                                </div>
                            </div>             
                            <div class="inlineDetail">
                                <div class="itemDetail">
                                    <h6>Modified by</h6>
                                    <span id="lblModifyBy" ClientIDMode="Static" runat="server"></span>
                                </div>
                                <div class="itemDetail">
                                    <h6>Modified at</h6>
                                    <span id="lblModifyDate" ClientIDMode="Static" runat="server"></span>
                                </div>
                            </div> 
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Status Notification Popup -->
    <div id="notification" class="notification" runat="server" onclick="closeNotification($(this));">
        <div class="notification-title">
            <i id="notificationIcon" runat="server"></i>
            <h4 id="notificationTitle" runat="server"></h4>
        </div>
        <p id="notificationMessage" class="notification-message" runat="server"></p>
        <small class="notification-tip">CLICK TO CLOSE</small>
    </div>

</asp:Content>
