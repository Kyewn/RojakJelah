<%@ Page Title="Manage Suggestions" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Suggestions.aspx.cs" Inherits="RojakJelah.Suggestions" %>
<asp:Content ID="PageStylesheet" ContentPlaceHolderID="PageStylesheet" runat="server">
    <link href="<%= Page.ResolveUrl("~/Content/css/suggestions.css")%>" type="text/css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="PageJavaScript" ContentPlaceHolderID="PageJavaScript" runat="server">
    <script src="Content/js/suggestions.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div ID="editModalWindow" ClientIDMode="Static" runat="server">
        <div class="editModalBody">
            <div class="header">
                <i class="fa-solid fa-pen-to-square"></i>
                <h3>Edit suggestion</h3>
            </div>
            <div class="editIdRow">
                <h5>ID</h5>
                <span ID="txtEditId" runat="server"></span>
            </div>
            <div class="editTopRow">
                <div>
                    <h5>Slang</h5>
                    <asp:TextBox ID="txtEditSlang" runat="server"></asp:TextBox>
                </div>
                <div>
                    <h5>Translation</h5>
                    <asp:TextBox ID="txtEditTranslation" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="editBottomRow">
                <h5>Example</h5>
                <textarea ID="txtEditExample" runat="server"/>
            </div>
            <div class="editButtonContainer">
                <asp:Button ID="btnEditCancel" ClientIDMode="Static" runat="server" Text="Cancel" OnClick="btnEditCancel_Click"/>
                <asp:Button ID="btnEditConfirm" ClientIDMode="Static" runat="server" Text="Confirm" OnClick="btnEditConfirm_Click"/>
            </div>
        </div>
    </div>

    <div class="contentWrapper">
        <div class="header">
            <i class="fa-solid fa-spell-check"></i>
            <h3>Dictionary suggestions</h3>
        </div> 
        <div class="contentContainer">
            <div class="leftContainer">
                <div class="searchContainer">
                    <div class="filterContainer">
                        <h6>Filter</h6>
                        <asp:DropDownList ID="cboFilter" class="cboFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboFilter_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="searchBarContainer">
                        <h6>Search</h6>
                        <asp:TextBox ID="txtSearch" class="txtSearch" placeholder="e.g. Hello World" runat="server" AutoPostBack="True" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                    </div>
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
                            <asp:Button ID="btnAccept" ClientIDMode="Static" runat="server" Text="Accept" />
                            <asp:Button ID="btnReject" ClientIDMode="Static" runat="server" Text="Reject" />
                        </div>
                        <div ID="detailContainer" runat="server" class="detailContainer">
                            <div class="itemDetail">
                                <h6>ID</h6>
                                <span id="lblId" ClientIDMode="Static" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Slang</h6>
                                <span id="lblSlang" ClientIDMode="Static" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Translation</h6>
                                <span id="lblTranslation" ClientIDMode="Static" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Created by</h6>
                                <span id="lblAuthor" ClientIDMode="Static" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Created at</h6>
                                <span id="lblDate" ClientIDMode="Static" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Example</h6>
                                <span id="lblExample" ClientIDMode="Static" runat="server">Testvalue</span>
                            </div>
                        </div>
                        <asp:Button ID="btnEdit" ClientIDMode="Static" runat="server" Text="Edit" OnClick="btnEdit_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Status Notification Popup -->
    <div id="notification" class="notification" runat="server" onclick="closeNotification($(this));">
        <div class="notification-title">
            <i class="fa-solid fa-circle-exclamation"></i>
            <h4 id="notificationTitle" runat="server"></h4>
        </div>
        <p id="notificationMessage" class="notification-message" runat="server"></p>
        <small class="notification-tip">CLICK TO CLOSE</small>
    </div>

</asp:Content>
