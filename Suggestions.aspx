<%@ Page Title="Manage Suggestions | RojakJelah" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Suggestions.aspx.cs" Inherits="RojakJelah.Suggestions" %>
<asp:Content ID="PageStylesheet" ContentPlaceHolderID="PageStylesheet" runat="server">
    <link href="<%= Page.ResolveUrl("~/Content/css/suggestions.css")%>" type="text/css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="PageJavaScript" ContentPlaceHolderID="PageJavaScript" runat="server">
    <script src="Content/js/suggestions.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="contentWrapper">
        <div class="header">
            <div class="divInline">
                <i class="fa-solid fa-spell-check"></i>
                <h3>Dictionary suggestions</h3>
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
                            <asp:TextBox ID="txtSearch" class="txtSearch" placeholder="e.g. Hello World" runat="server" AutoPostBack="True" OnTextChanged="CboSortAndFilter_SelectedIndexChanged"></asp:TextBox>
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
                            <asp:Button ID="btnAccept" ClientIDMode="Static" runat="server" Text="Accept" OnClick="BtnAccept_Click"></asp:Button>
                            <asp:Button ID="btnReject" ClientIDMode="Static" runat="server" Text="Reject" OnClick="BtnReject_Click"></asp:Button>
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
                                <h6>Language</h6>
                                <span id="lblLanguage" ClientIDMode="Static" runat="server"></span>
                            </div>
                            <div class="itemDetail">
                                <h6>Example</h6>
                                <span id="lblExample" ClientIDMode="Static" runat="server"></span>
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
                                    <span id="lblModifyAuthor" ClientIDMode="Static" runat="server"></span>
                                </div>
                                <div class="itemDetail">
                                    <h6>Modified at</h6>
                                    <span id="lblModifyDate" ClientIDMode="Static" runat="server"></span>
                                </div>
                            </div>
                        </div>
                        <asp:Button ID="btnEdit" ClientIDMode="Static" runat="server" Text="Edit" OnClick="BtnEdit_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div ID="editModalWindow" ClientIDMode="Static" runat="server" class="modal-window">
        <div id="modalDialog" class="modal-dialog" runat="server">
            <div class="modal-content">
                <div class="modal-header">
                    <i class="modal-icon fa-solid fa-pen-to-square"></i>
                    <h3 class="modal-title">Edit suggestion</h3>
                </div>
                <div class="modal-body">
                    <div class="editTopRow">
                        <div>
                            <h5>ID</h5>
                            <span ID="txtEditId" runat="server"></span>
                        </div>
                        <div>
                            <h5>Language</h5>
                            <asp:DropDownList ID="cboEditLanguage" class="cboEditLanguage" runat="server" AutoPostBack="True"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="editTopRow">
                        <div>
                            <h5>Slang*</h5>
                            <asp:TextBox ID="txtEditSlang" runat="server"></asp:TextBox>
                        </div>
                        <div>
                            <h5>Translation*</h5>
                            <asp:TextBox ID="txtEditTranslation" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="editBottomRow">
                        <h5>Example</h5>
                        <textarea ID="txtEditExample" runat="server"/>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnEditConfirm" ClientIDMode="Static" runat="server" Text="Confirm" OnClick="BtnEditConfirm_Click"/>
                    <asp:Button ID="btnEditCancel" ClientIDMode="Static" runat="server" Text="Cancel" OnClick="BtnEditCancel_Click"/>
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
