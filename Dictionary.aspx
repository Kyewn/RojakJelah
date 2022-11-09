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
        <div class="dictionary-header-text">
            <div class="dictionary-header-title">
                <i class="fa-solid fa-file-contract"></i>
                <h1>Dictionary</h1>
            </div>
            <h5 id="entryCountText" runat="server" ClientIDMode="static"></h5>
        </div>
        <div class="dictionary-controls">
            <div class="search-bar">
                <asp:DropDownList ID="ddlSort" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="DdlSort_SelectedIndexChanged">
                    <asp:ListItem value="" Text="Sort by:" Selected="True" disabled />
                </asp:DropDownList>
                <div id="divFilterDdl" class="dropdown">
                    <select id="ddlSearchFilter" runat="server">
                        <option value="" disabled selected>Search by:</option>
                    </select>
                </div>
                <input type="text" id="txtSearch" ClientIDMode="Static" runat="server" placeholder="Search..." autocomplete="off"/>
                <asp:LinkButton ID="lnkSearch" CssClass="button-small" runat="server" ClientIDMode="Static" OnClick="LnkSearch_Click">
                    <i class="fa-solid fa-magnifying-glass"></i>
                </asp:LinkButton>
            </div>
            <div class="dictionary-buttons">
                <button type="button" id="btnReport" class="button-small" onclick="showModal($('#mdlReport'))">
                    <i class="fa-solid fa-circle-exclamation fa-lg"></i>
                    <span id="tooltipReport" class="tool-tip">Report</span>
                </button>
                <button type="button" id="btnSuggest" class="button-primary" runat="server" onclick="showModal($('#mdlSuggestion'))">
                    <i class="fa-solid fa-plus"></i>
                    <h3>Suggest terms</h3>
                </button>
            </div>
        </div>
    </section>

    <!-- Dictionary Container  -->
    <section id="sctDictionary" class="dictionary-container" runat="server">
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
                    <div class="modal-inputfield">
                        <label class="modal-inputlabel">Issue Category *</label>
                        <select id="ddlReportCategory" class="modal-dropdown" runat="server" ClientIDMode="static">
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
        <div id="dlgSuggestion" class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <i class="modal-icon fa-solid fa-plus"></i>
                    <h2 class="modal-title">Send suggestion</h2>
                    <button type="button" class="modal-btn-close" onclick="closeModal($('#mdlSuggestion'))">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
                <div id="divSuggestionModalBody" class="modal-body">
                    <div class="modal-inputfield">
                        <label class="modal-inputlabel">Slang *</label>
                        <input type="text" id="txtSlang" class="modal-textinput" runat="server" placeholder="Rojak slang" autocomplete="off" />
                    </div>
                    <div class="modal-inputfield">
                        <label class="modal-inputlabel">Translation *</label>
                        <input type="text" id="txtTranslation" class="modal-textinput" runat="server" placeholder="English translation" autocomplete="off" />
                    </div>
                    <div id="divDdlLanguage" class="modal-inputfield">
                        <label class="modal-inputlabel">Origin Language *</label>
                        <select id="ddlLanguage" class="modal-dropdown" runat="server" ClientIDMode="static">
                        </select>
                    </div>
                    <div id="divTxtExample" class="modal-inputfield">
                        <label class="modal-inputlabel">Example</label>
                        <textarea class="modal-textinput" rows="5" maxlength="100" placeholder="Describe an example usage of the slang"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <button class="button-primary">Submit</button>
                    <button class="button-secondary">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <%--<!-- Confirmation Modal --> this shit just doesnt work
    <div id="mdlConfirmation" class="modal-window" ClientIDMode="Static" runat="server">
        <div id="dlgConfirmation" class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <i class="modal-icon fa-solid fa-triangle-exclamation"></i>
                    <h2 class="modal-title">Confirmation</h2>
                    <button type="button" class="modal-btn-close" onclick="closeModal($('#mdlConfirmation'))">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <h4 id="dialogMessage" class="modal-dialog-message"></h4>
                </div>
                <div class="modal-footer">
                    <button id="btnYes" class="button-primary">Yes</button>
                    <button id="btnNo" class="button-secondary">No</button>
                </div>
            </div>
        </div>
    </div>--%>

    <!-- Notification Popup -->
    <div id="notification" class="notification" runat="server" onclick="closeNotification($(this));">
        <div class="notification-title">
            <i id="notificationIcon" class="fa-solid fa-circle-exclamation" runat="server"></i>
            <h4 id="notificationTitle" runat="server"></h4>
        </div>
        <p id="notificationMessage" class="notification-message" runat="server"></p>
        <small class="notification-tip">CLICK TO CLOSE</small>
    </div>

</asp:Content>
