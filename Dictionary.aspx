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
                <button type="button" id="btnReport" class="button-small" runat="server" onclick="showModal($('#mdlReport'))">
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
    <section id="sctDictionary">
        <div id="divDictionary" class="dictionary-container" runat="server" ClientIDMode="Static">
            <asp:HiddenField ID="hfScrollPosition" runat="server" ClientIDMode="Static" />

            <asp:ListView ID="lvDictionary" runat="server" ClientIDMode="Static" DataKeyNames="Id" OnItemCommand="LvDictionary_ItemCommand"
                GroupPlaceholderID="groupPlaceHolder1" ItemPlaceholderID="itemPlaceHolder1" OnPagePropertiesChanging="OnPagePropertiesChanging">
                <LayoutTemplate>
                    <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                </LayoutTemplate>
                <GroupTemplate>
                    <asp:PlaceHolder runat="server" ID="itemPlaceHolder1"></asp:PlaceHolder>
                </GroupTemplate>
                <ItemTemplate>
                    <div class="dictionary-item">
                        <div class="dictionary-item-content">
                            <h4 class="dictionary-item-title"><%# Eval("Slang.WordValue") %></h4>
                            <div class="dictionary-item-text">
                                <p class="content-title">Meaning</p>
                                <p class="content-text"><%# Eval("Translation.WordValue") %></p>
                            </div>
                            <div class="dictionary-item-text">
                                <p class="content-title">Example</p>
                                <p class="content-text"><%# (Eval("Example") == null || Eval("Example").ToString() == "") ? "-" : Eval("Example") %></p>
                            </div>
                            <div class="dictionary-item-text">
                                <p class="content-title">Language</p>
                                <p class="content-text"><%# Eval("Slang.Language.Name") %></p>
                            </div>
                        </div>
                        
                            <%  if (ViewState["UserRole"] != null)
                                {
                                    if (ViewState["UserRole"].ToString() == "System" || ViewState["UserRole"].ToString() == "Admin")
                                    {
                               %>
                                    <div class="dictionary-item-controls">
                                        <asp:Button ID="btnEdit" CssClass="button-secondary button-edit" runat="server" CommandName="Modify" Text="Edit" />
                                        <asp:Button ID="btnDelete" CssClass="button-secondary button-delete" runat="server" CommandName="Remove" OnClientClick="confirmDelete(event, this.id)" Text="Delete" />
                                    </div>
                            <%      }
                                } %>
                    </div>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class='dictionary-empty'>
                        <i class='fa-regular fa-circle-xmark fa-2xl'></i>
                        <h4>No entries found</h4>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <div class="dictionary-pager">
            <div class="dictionary-data-pager">
                <asp:DataPager ID="dictionaryDataPager" runat="server" PagedControlID="lvDictionary" PageSize="20">
                    <Fields>
                        <asp:NextPreviousPagerField ButtonType="Link" ButtonCssClass="pager-previous-next" ShowFirstPageButton="false" ShowPreviousPageButton="true" ShowNextPageButton="false" />
                        <asp:NumericPagerField ButtonType="Link" NumericButtonCssClass="pager-button" CurrentPageLabelCssClass="pager-button-current" />
                        <asp:NextPreviousPagerField ButtonType="Link" ButtonCssClass="pager-previous-next" ShowNextPageButton="true" ShowLastPageButton="false" ShowPreviousPageButton = "false" />
                    </Fields>
                </asp:DataPager>
            </div>
            <div class="dictionary-pager-info">
                <p id="dictionaryDataPagerInfo" runat="server" ClientIDMode="Static"></p>
            </div>
        </div>
    </section>

    <!-- Modals -->
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

    <!-- Suggestion Modal -->
    <div id="mdlSuggestion" class="modal-window" ClientIDMode="Static" runat="server">
        <div id="dlgSuggestion" ClientIDMode="Static" class="modal-dialog" runat="server">
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
                        <asp:DropDownList ID="ddlLanguage" ClientIDMode="static" class="modal-dropdown" runat="server"></asp:DropDownList>
                        </select>
                    </div>
                    <div id="divTxtExample" class="modal-inputfield">
                        <label class="modal-inputlabel">Example</label>
                        <textarea id="txtExample" runat="server" class="modal-textinput" rows="5" maxlength="100" placeholder="Describe an example usage of the slang"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSubmitSuggestion" runat="server" class="button-primary" Text="Submit" OnClick="BtnSubmitSuggestion_Click"/>
                    <asp:Button ID="btnCancelSuggestion" runat="server" class="button-secondary" Text="Cancel" OnClick="BtnCancelSuggestion_Click"/>
                </div>
            </div>
        </div>
    </div>

    <!-- Edit Dictionary Entry Modal -->
    <div id="mdlEditDictionaryEntry" class="modal-window" ClientIDMode="Static" runat="server">
        <div id="dlgEditDictionaryEntry" class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <i class="modal-icon fa-solid fa-pen-to-square"></i>
                    <h2 class="modal-title">Edit dictionary entry</h2>
                    <button type="button" class="modal-btn-close" onclick="closeModal($('#mdlEditDictionaryEntry'))">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
                <div id="divEditModalBody" class="modal-body">
                    <div id="divEditReadOnlyFields">
                        <div id="divDictionaryEntryId" class="modal-inputfield">
                            <label class="modal-inputlabel">ID</label>
                            <p id="txtDictionaryEntryId" class="modal-fieldvalue" runat="server" ClientIDMode="Static"></p>
                        </div>
                        <div id="divEditSlang" class="modal-inputfield">
                            <label class="modal-inputlabel">Slang</label>
                            <p id="txtEditSlang" class="modal-fieldvalue" runat="server" ClientIDMode="Static"></p>
                        </div>
                        <div id="divEditTranslation" class="modal-inputfield">
                            <label class="modal-inputlabel">Translation</label>
                            <p id="txtEditTranslation" class="modal-fieldvalue" runat="server" ClientIDMode="Static"></p>
                        </div>
                    </div>
                    <div id="divEditLanguage" class="modal-inputfield">
                        <label class="modal-inputlabel">Origin Language *</label>
                        <select id="ddlEditLanguage" class="modal-dropdown" runat="server" ClientIDMode="Static">
                        </select>
                    </div>
                    <div id="divEditExample" class="modal-inputfield">
                        <label class="modal-inputlabel">Example</label>
                        <textarea id="txtEditExample" class="modal-textinput" runat="server" rows="5" maxlength="100" placeholder="Describe an example usage of the slang"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmEdit" class="button-primary" runat="server" OnClick="BtnConfirmEdit_Click" Text="Confirm" />
                    <asp:Button ID="btnCancelEdit" class="button-secondary" runat="server" OnClick="BtnCancelEdit_Click" Text="Cancel" />
                </div>
            </div>
        </div>
    </div>

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
