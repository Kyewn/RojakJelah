<%@ Page Title="Manage Suggestions" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Suggestions.aspx.cs" Inherits="RojakJelah.Suggestions" %>
<asp:Content ID="PageStylesheet" ContentPlaceHolderID="PageStylesheet" runat="server">
    <link href="<%= Page.ResolveUrl("~/Content/css/suggestions.css")%>" type="text/css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <form ID="manageSuggestionsForm" runat="server">
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
                </div>
            </div>
        </div>
    </form>
</asp:Content>
