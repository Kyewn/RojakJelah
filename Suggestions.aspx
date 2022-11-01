<%@ Page Title="Manage Suggestions" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Suggestions.aspx.cs" Inherits="RojakJelah.Suggestions" %>
<asp:Content ID="PageStylesheet" ContentPlaceHolderID="PageStylesheet" runat="server">
    <link href="<%= Page.ResolveUrl("~/Content/css/suggestions.css")%>" type="text/css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div ID="modalWindow" ClientIDMode="Static" runat="server">
        <div class="modalBody">
            <div>
                <h5>ID</h5>
                <span ID="txtEditId" runat="server"></span>
            </div>
            <div>
                <h5>Slang</h5>
                <!-- <asp:TextBox ID="txtEditSlang" runat="server"></asp:TextBox> -->
            </div>
            <div>
                <h5>Translation</h5>
                <!-- <asp:TextBox ID="txtEditTranslation" runat="server"></asp:TextBox> -->
            </div>
            <div>
                <h5>Example</h5>
                <!-- <textarea ID="txtExample" runat="server"/> -->
            </div>
        </div>
    </div>
    <div class="contentWrapper">
        <h3>Dictionary suggestions</h3>
        <div class="contentContainer">
            <div class="leftContainer">
                <div class="searchContainer">
                    <div class="filterContainer">
                        <h6>Filter</h6>
                        <asp:DropDownList ID="cboFilter" class="cboFilter" runat="server"></asp:DropDownList>
                    </div>
                    <div class="searchBarContainer">
                        <h6>Search</h6>
                        <asp:TextBox ID="txtSearch" class="txtSearch" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div ID="listItemContainer" class="listItemContainer" runat="server">
                    <div class="listItem">
                        <div class="topRow">
                            <div class="itemDetail">
                                <span>ID</span>
                                <span>Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <span>Slang</span>
                                <span>Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <span>Translation</span>
                                <span>Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <span>Created by</span>
                                <span>Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <span>Created at</span>
                                <span>Testvalue</span>
                            </div>
                        </div>
                        <div class="bottomRow">
                            <div class="itemDetail">
                                <span>Example</span>
                                <span>Testvalue</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="rightContainer">
                <div class="actionMenuWrapper">
                    <h6>Actions</h6>
                    <div class="menuContainer">
                        <div class="buttonContainer">
                            <asp:Button ID="btnAccept" ClientIDMode="Static" runat="server" Text="Accept" />
                            <asp:Button ID="btnReject" ClientIDMode="Static" runat="server" Text="Reject" />
                        </div>
                        <div class="detailContainer">
                            <div class="itemDetail">
                                <h6>ID</h6>
                                <span id="lblId" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Slang</h6>
                                <span id="lblSlang" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Translation</h6>
                                <span id="lblTranslation" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Created by</h6>
                                <span id="lblAuthor" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Created at</h6>
                                <span id="lblDate" runat="server">Testvalue</span>
                            </div>
                            <div class="itemDetail">
                                <h6>Example</h6>
                                <span id="lblExample" runat="server">Testvalue</span>
                            </div>
                        </div>
                        <asp:Button ID="btnEdit" ClientIDMode="Static" runat="server" Text="Edit" OnClick="btnEdit_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
