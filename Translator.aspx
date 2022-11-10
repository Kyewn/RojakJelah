<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Translator.aspx.cs" Inherits="RojakJelah.Translator" %>

<asp:Content ID="PageStylesheet" ContentPlaceHolderID="PageStylesheet" runat="server">
    <link rel="stylesheet" type="text/css" href="<%= Page.ResolveUrl("~/Content/css/translator.css")%>" />
</asp:Content>

<asp:Content ID="PageJavaScript" ContentPlaceHolderID="PageJavaScript" runat="server">
    <script src="Content/js/translator.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Translate Section -->
    <section class="translate-content">
        <!-- Banner -->
        <div class="banner">
            <div class="banner-container">
                <div class="banner-left">
                    <img src="Content/image/textual_col_whitebg.svg" alt="Logo">
                </div>
                <div class="banner-right">
                    <div class="title">
                        <h1>Be a <span>True</span> Malaysian</h1>
                    </div>
                    <div class="info">
                        <p>No need to pilih language anymore lah, become the CEO of Bahasa Rojak</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Source Message  -->
        <div class="translate">
            <div class="translate-input">
                <div class="title">
                    <h5>Source message</h5>
                </div>

                <div class="translate-container">
                    <!-- Translation Input Field -->
                    <textarea id="txtInput" class="context" runat="server" rows="3" maxlength="5000" placeholder="Enter message here..." spellcheck="true"></textarea>
                    <!-- Translate Buttons  -->
                    <div class="translate-button">
                        <asp:Button ID="btnTranslate" class="btnTranslate" runat="server" OnClick="BtnTranslate_Click" Text="Translate" />

                        <!-- Sub-feature Buttons -->
                        <div class="translate-user">
                            <!-- Saved Translations -->
                            <div class="button saved">
                                <asp:LinkButton ID="lnkViewSavedTranslations" CssClass="button-small" runat="server" ClientIDMode="Static" OnClick="LnkViewSavedTranslations_Click">
                                    <i class="fa-solid fa-star"></i>
                                    <span id="tooltipSavedTranslations" class="tool-tip">Saved translations</span>
                                </asp:LinkButton>
                            </div>

                            <!-- Translation History  -->
                            <div class="button history">
                                <asp:LinkButton ID="lnkViewTranslationHistory" CssClass="button-small" runat="server" ClientIDMode="Static" OnClick="LnkViewTranslationHistory_Click">
                                    <i class="fa-solid fa-clock-rotate-left"></i>
                                    <span id="tooltipTranslationHistory" class="tool-tip">Translation history</span>
                                </asp:LinkButton>
                            </div>

                            <!-- Make Report  -->
                            <div class="button report">
                                <asp:LinkButton ID="lnkMakeReport" CssClass="button-small" runat="server" ClientIDMode="Static" OnClick="LnkReport_Click">
                                    <i class="fa-solid fa-circle-exclamation"></i>
                                    <span id="tooltipReport" class="tool-tip">Report</span>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Translate Output  -->
            <div class="translate-output">
                <div class="translate-output-title">
                    <h5>Translation</h5>
                    <asp:LinkButton ID="lnkSaveTranslation" CssClass="translate-btn-save" runat="server" ClientIDMode="Static" OnClick="LnkSaveTranslation_Click">
                        <i id="iconSave" class="fa-regular fa-bookmark" runat="server"></i>
                        <span id="tooltipSave" class="tool-tip">Save translation</span>
                    </asp:LinkButton>
                </div>
                <div class="output">
                    <p id="txtOutput" runat="server"></p>
                    <img id="startQuote" src="Content/image/translatorOutput_startQuote.svg" />
                    <img id="endQuote" src="Content/image/translatorOutput_endQuote.svg" />
                </div>
            </div>
        </div>
    </section>

    <!-- User Generate -->
    <section class="user-generate">
        <!-- User Generate Info -->
        <div class="generate-container">
            <div class="generate-items">
                <div class="item">
                    <h4>More than</h4>
                    <h1>100</h1>
                    <p>Rojak terms documented</p>
                </div>
                <div class="item">
                    <h4>At least</h4>
                    <h1>10</h1>
                    <p>Different translation variations</p>
                </div>
                <div class="item">
                    <h4>Estimated</h4>
                    <h1>1M</h1>
                    <p>Users will laugh die themselves</p>
                </div>
            </div>
        </div>

        <!-- Learn More  -->
        <div class="learn-more">
            <h1>Click here and check us out!</h1>
            <div class="button">
                <a target="_blank" href="About.aspx">Learn more</a>
            </div>
        </div>
    </section>

    <!-- Modals -->
    <!-- Saved Translations Modal -->
    <div id="mdlSavedTranslations" class="modal-window" ClientIDMode="Static" runat="server">
        <div id="dlgSavedTranslation" class="modal-dialog" runat="server">
            <div class="modal-content">
                <div class="modal-header">
                    <i class="modal-icon fa-solid fa-star"></i>
                    <h2 class="modal-title">Saved translations</h2>
                    <button type="button" class="modal-btn-close" onclick="closeModal($('#mdlSavedTranslations'))">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
                <div id="divSavedTranslationsModalBody" class="modal-body" CientIDMode="Static" runat="server">
                </div>
                <div class="modal-footer">
                    <p id="savedTranslationFooterText" runat="server"></p>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Translation History Modal -->
    <div id="mdlTranslationHistory" class="modal-window" ClientIDMode="Static" runat="server">
        <div id="dlgHistory" class="modal-dialog" runat="server">
            <div class="modal-content">
                <div class="modal-header">
                    <i class="modal-icon fa-solid fa-clock-rotate-left"></i>
                    <h2 class="modal-title">Translation history</h2>
                    <button type="button" class="modal-btn-close" onclick="closeModal($('#mdlTranslationHistory'))">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
                <div id="divTranslationHistoryModalBody" class="modal-body" CientIDMode="Static" runat="server">
                </div>
                <div class="modal-footer">
                    <p id="translationHistoryFooterText" runat="server"></p>
                </div>
            </div>
        </div>
    </div>

    <!-- Report Modal -->
    <div id="mdlReport" class="modal-window" ClientIDMode="Static" runat="server">
        <div id="dlgReport" ClientIDMode="Static" class="modal-dialog" runat="server">
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
                        <asp:DropDownList ID="ddlReportCategory" ClientIDMode="static" runat="server" class="modal-dropdown" AutoPostback="true" OnSelectedIndexChanged="DdlReportCategory_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div id="divEntryInput" class="mdlReport-topInputRow" runat="server">
                        <div class="modal-inputfield">
                            <label class="modal-inputlabel">Problem slang *</label>
                            <asp:TextBox ID="txtReportSlang" runat="server" class="modal-textinput" placeholder="e.g. hello world" AutoPostback="true" OnTextChanged="TxtReportSlang_TextChanged"></asp:TextBox>
                        </div>
                        <div class="modal-inputfield">
                            <label class="modal-inputlabel">Problem translation *</label>
                            <asp:DropDownList ID="ddlReportTranslation" ClientIDMode="static" runat="server" class="modal-dropdown" Enabled="false"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="modal-inputfield">
                        <label class="modal-inputlabel">Description</label>
                        <textarea ID="txtReportDescription" runat="server" class="modal-textinput" rows="5" maxlength="500" placeholder="Describe your issue"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSubmitReport" runat="server" class="button-primary" Text="Submit" OnClick="BtnSubmitReport_Click" />
                    <asp:Button ID="btnCancelReport" runat="server" class="button-secondary" Text="Cancel" OnClick="BtnCancelReport_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Notification Popup -->
    <div id="notification" class="notification" runat="server" onclick="closeNotification($(this));">
        <div class="notification-title">
            <i id="notificationIcon" runat="server"></i>
            <h4 id="notificationTitle" runat="server"></h4>
        </div>
        <p id="notificationMessage" class="notification-message" runat="server"></p>
        <small class="notification-tip">CLICK TO CLOSE</small>
    </div>

</asp:Content>
