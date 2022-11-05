<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dictionary.aspx.cs" Inherits="RojakJelah.Dictionary" %>

<asp:Content ID="PageStylesheet" ContentPlaceHolderID="PageStylesheet" runat="server">
    <link rel="stylesheet" type="text/css" href="<%= Page.ResolveUrl("~/Content/css/dictionary.css")%>" />
</asp:Content>

<asp:Content ID="PageJavaScript" ContentPlaceHolderID="PageJavaScript" runat="server">
    <script src="Content/js/dictionary.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Dictionary Header  -->
    <section class="dictionary-header">
        <div class="dictionary-header-title">
            <i class="fa-solid fa-file-contract"></i>
            <h1>Dictionary</h1>
        </div>
        <div class="dictionary-controls">
            <div class="search-bar">
                <div class="dropdown">
                    <select id="ddlSearchFilter" runat="server">
                        <option value="" disabled selected>Search Option:</option>
                        <option value="Any">Any</option>
                        <option value="Slang">Rojak Slang</option>
                        <option value="Translation">English Meaning</option>
                    </select>
                </div>
                <input type="text" id="txtSearch" ClientIDMode="Static" runat="server" placeholder="Search..."/>
                <asp:LinkButton ID="lnkSearch" CssClass="button-small" runat="server" ClientIDMode="Static" OnClick="LnkSearch_Click">
                    <i class="fa-solid fa-magnifying-glass"></i>
                </asp:LinkButton>
            </div>
            <div class="dictionary-buttons">
                <button type="button" id="lnkReport" class="button-small" onclick="showModal($('#mdlReport'))">
                    <i class="fa-solid fa-circle-exclamation fa-lg"></i>
                    <span id="tooltipReport" class="tool-tip">Report</span>
                </button>
                <asp:LinkButton ID="lnkSuggest" CssClass="button-primary" runat="server" OnClick="LnkSuggest_Click">
                    <i class="fa-solid fa-plus"></i>
                    <h3>Suggest terms</h3>
                </asp:LinkButton>
            </div>
        </div>
    </section>

    <!-- Dictionary Container  -->
    <section class="dictionary-container">
        <div id="divDictionaryEmpty" class="dictionary-empty" runat="server">
            <i class="fa-regular fa-circle-xmark fa-2xl"></i>
            <h4>No entries found</h4>
        </div>
        <div class="dictionary-item">
            <h4 class="dictionary-item-title">Slang</h4>
            <div class="dictionary-item-content">
                <p class="content-title">Meaning</p>
                <p class="content-text">(Translation)</p>
            </div>
            <div class="dictionary-item-content">
                <p class="content-title">Example</p>
                <p class="content-text">(Example)</p>
            </div>
        </div>
        <div class="dictionary-item">
            <h4 class="dictionary-item-title">Slang</h4>
            <div class="dictionary-item-content">
                <p class="content-title">Meaning</p>
                <p class="content-text">(Translation)</p>
            </div>
            <div class="dictionary-item-content">
                <p class="content-title">Example</p>
                <p class="content-text">(Example)</p>
            </div>
        </div>
        <div class="dictionary-item">
            <h4 class="dictionary-item-title">Slang</h4>
            <div class="dictionary-item-content">
                <p class="content-title">Meaning</p>
                <p class="content-text">(Translation)</p>
            </div>
            <div class="dictionary-item-content">
                <p class="content-title">Example</p>
                <p class="content-text">(Example)</p>
            </div>
        </div>
        <div class="dictionary-item">
            <h4 class="dictionary-item-title">Slang</h4>
            <div class="dictionary-item-content">
                <p class="content-title">Meaning</p>
                <p class="content-text">(Translation)</p>
            </div>
            <div class="dictionary-item-content">
                <p class="content-title">Example</p>
                <p class="content-text">(Example)</p>
            </div>
        </div>
        <div class="dictionary-item">
            <h4 class="dictionary-item-title">Slang</h4>
            <div class="dictionary-item-content">
                <p class="content-title">Meaning</p>
                <p class="content-text">(Translation)</p>
            </div>
            <div class="dictionary-item-content">
                <p class="content-title">Example</p>
                <p class="content-text">(Example)</p>
            </div>
        </div>
    </section>

    <!-- Modals -->
    <!-- Report Modal -->
    <div id="mdlReport" class="modal-window" ClientIDMode="Static" runat="server">
        <div id="dlgReport" class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <i class="modal-icon fa-solid fa-circle-exclamation"></i>
                    <h2 class="modal-title">Report</h2>
                    <button type="button" class="modal-btn-close" onclick="closeModal($('#mdlReport'))">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
                <div id="divReportModalBody" class="modal-body">
                    <div id="" class="modal-inputfield">
                        <label class="modal-inputlabel">Issue Category *</label>
                        <select class="modal-dropdown">
                            <option>Test</option>
                        </select>
                    </div>
                    <div class="modal-inputfield">
                        <label class="modal-inputlabel">Description *</label>
                        <textarea class="modal-textinput" rows="5" maxlength="500" placeholder="Describe your issue"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <button class="button-primary">Submit</button>
                    <button class="button-secondary">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Suggestion Modal -->
    <div id="mdlSuggestion" class="modal-window" ClientIDMode="Static" runat="server">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <i class="modal-icon fa-solid fa-plus"></i>
                    <h2 class="modal-title">Send suggestion</h2>
                    <button type="button" class="modal-btn-close" onclick="closeModal($('#mdlSuggestion'))">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
                <div class="modal-body" CientIDMode="Static" runat="server">
                </div>
                <div class="modal-footer">
                </div>
            </div>
        </div>
    </div>

    <!-- Notification Popup -->
    <div id="notification" class="notification" runat="server" onclick="closeNotification($(this));">
        <div class="notification-title">
            <i class="fa-solid fa-circle-exclamation"></i>
            <h4 id="notificationTitle" runat="server"></h4>
        </div>
        <p id="notificationMessage" class="notification-message" runat="server"></p>
        <small class="notification-tip">CLICK TO CLOSE</small>
    </div>

</asp:Content>
