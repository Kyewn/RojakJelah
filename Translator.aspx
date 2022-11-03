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
                    <div class="context">
                        <textarea id="txtInput" runat="server" name="inputValue" rows="3" maxlength="5000" placeholder="Enter message here..."></textarea>
                    </div>

                    <!-- Translate Buttons  -->
                    <div class="translate-button">
                        <asp:Button ID="btnTranslate" class="btnTranslate" runat="server" OnClick="BtnTranslate_Click" Text="Translate" />

                        <!-- Sub-feature Buttons -->
                        <div class="translate-user">
                            <!-- Saved Translations -->
                            <div class="button saved">
                                <button id="btnViewSavedTranslations" type="button" class="btn btn-primary" runat="server" data-bs-toggle="modal" data-bs-target="#mdlSavedTranslations">
                                    <i class="fa-solid fa-star"></i>
                                </button>
                            </div>

                            <!-- Translation History  -->
                            <div class="button history">
                                <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#mdlTranslationHistory">
                                    <i class="fa-solid fa-clock-rotate-left"></i>
                                </button>
                            </div>

                            <!-- Make Report  -->
                            <div class="button report">
                                <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#mdlMakeReport">
                                    <i class="fa-solid fa-circle-exclamation"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Translate Output  -->
            <div class="translate-output">
                <div class="translate-output-title">
                    <h5>Translation</h5>
                    <asp:LinkButton ID="lnkSaveTranslation" CssClass="translate-btn-save" runat="server" OnClick="LnkSaveTranslation_Click">
                        <i id="iconSave" class="fa-regular fa-bookmark" runat="server"></i>
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
    <div id="mdlSavedTranslations" class="modal fade" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="translateSaved" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 id="translateSaved" class="modal-title">Saved translations</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div id="divSavedTranslationsModalBody" class="modal-body" runat="server">
                </div>
                <div class="modal-footer">
                    <p id="savedTranslationFooterText" runat="server"></p>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Translation History Modal -->
    <div id="mdlTranslationHistory" class="modal fade" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="translateHistory" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 id="translateHistory" class="modal-title">Translation history</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                    </button>
                </div>
                <div id="divTranslationHistoryModalBody" class="modal-body" runat="server">
                </div>
                <div class="modal-footer">
                    <p id="translationHistoryFooterText" runat="server"></p>
                </div>
            </div>
        </div>
    </div>

    <!-- Make Report Modal -->
    <div id="mdlMakeReport" class="modal fade" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="translateReport" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="translateReport">Report</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="modal-item">
                        <h4>Issue Category<span>*</span></h4>
                        <div>
                            <select id="label" name="label">
                                <option value="">Label One</option>
                                <option value="">Label Two</option>
                                <option value="">Label Three</option>
                                <option value="">Label Four</option>
                            </select>
                            <h4 class="pt-2">Description</h4>
                            <textarea name="" id="" cols="" rows="2" placeholder="Text here...."></textarea>
                            <div class="button pt-3">
                                <div class="px-2">
                                    <button type="button" class="cancel" data-bs-dismiss="modal" aria-label="Close">Cancel</button>
                                </div>
                                <div>
                                    <button class="submit">Submit</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
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
