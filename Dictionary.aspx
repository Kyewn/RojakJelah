<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dictionary.aspx.cs" Inherits="RojakJelah.Dictionary" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Header Search / Add Suggest  -->
    <!-- Header Search / Add Suggest  -->
    <!-- Header Search / Add Suggest  -->
    <section class="dictionary-header">
        <div class="container-fluid">
            <div class="row">
                <div class="col-lg-3 col-md-12 col-sm-12">
                    <div class="header-title">
                        <div>
                            <h1>Dictionary</h1>
                        </div>
                        <div class="report">
                            <button>
                                <i class="fa-solid fa-circle-exclamation"></i>
                            </button>
                        </div>
                    </div>
                </div>
                <div class="col-lg-9 col-md-12 col-sm-12">

                    <!-- Search or Add Suggest   -->
                    <div class="header-search">
                        <div class="search-box">
                            <input type="text" placeholder="Search...">
                        </div>
                        <div class="add-suggest">
                            <!-- Button trigger modal -->
                            <button type="button" class="suggest-btn" data-bs-toggle="modal"
                                data-bs-target="#addSuggestItem">
                                <i class="fa-solid fa-plus"></i> Suggest terms
                            </button>
                            <!-- Modal -->
                            <!-- Add Suggest Terms Form  -->
                            <!-- Add Suggest Terms Form  -->
                            <!-- Add Suggest Terms Form  -->
                            <div class="modal fade" id="addSuggestItem" tabindex="-1" data-bs-backdrop="static"
                                aria-labelledby="addSuggest" aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h1 class="suggest-title" id="addSuggest">Add suggestion</h1>
                                            <button type="button" class="btn-close" data-bs-dismiss="modal"
                                                aria-label="Close"></button>
                                        </div>
                                        <div class="modal-body">

                                            <!-- Add Suggest Form  -->
                                            <!-- Add Suggest Form  -->
                                            <!-- Add Suggest Form  -->
                                            <form id="submitForm" action="#">
                                                <!-- Suggest  -->
                                                <div class="form-item">
                                                    <div class="form-field">
                                                        <label for="">Slang</label><br>
                                                        <input type="text" required>
                                                    </div>
                                                    <div class="form-field">
                                                        <label for="">Translation</label><br>
                                                        <input type="text" required>
                                                    </div>
                                                </div>
                                                <!-- Language  -->
                                                <div class="select-language">

                                                    <div class="form-field">
                                                        <label for="">Language</label><br>
                                                        <select name="language" id="language" required>
                                                            <option value="">Language name</option>
                                                            <option value="">Language 1</option>
                                                            <option value="">Language 2</option>
                                                            <option value="">language 3</option>
                                                            <option value="">Language 4</option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <!-- Example  -->
                                                <div class="example">
                                                    <div class="form-field">
                                                        <label for="">Example</label><br>
                                                        <textarea name="" id="" cols="" rows="3"
                                                            placeholder="Example Here ...." required></textarea>
                                                    </div>
                                                </div>
                                                <div class="button">
                                                    <div class="px-2">
                                                        <button type="button" class="cancel" data-bs-dismiss="modal"
                                                            aria-label="Close">Cancel</button>
                                                    </div>
                                                    <div>
                                                        <button class="submit" onclick="addSuggest()">Submit</button>
                                                    </div>
                                                </div>
                                            </form>
                                        </div>
                                    </div>
                                </div>
                            </div>


                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>


    <!-- Add Suggest Terms  -->
    <!-- Add Suggest Terms  -->
    <!-- Add Suggest Terms  -->




    <!-- Dictionary Container  -->
    <!-- Dictionary Container  -->
    <!-- Dictionary Container  -->
    <section class="dictionary-container">

        <div class="dictionary-item">
            <div class="title">
                <h3>Slang</h3>
            </div>
            <div class="item-content">
                <div class="left-item">
                    <p>Meaning:</p>
                </div>
                <div class="right-item">
                    <p>(Translation)</p>
                </div>
            </div>
            <div class="item-content">
                <div class="left-item">
                    <p>Example:</p>
                </div>
                <div class="right-item">
                    <p>(Example)</p>
                </div>
            </div>
        </div>

        <div class="dictionary-item">
            <div class="title">
                <h3>Aduhai</h3>
            </div>
            <div class="item-content">
                <div class="left-item">
                    <p>Meaning:</p>
                </div>
                <div class="right-item">
                    <p>(Translation)</p>
                </div>
            </div>
            <div class="item-content">
                <div class="left-item">
                    <p>Example:</p>
                </div>
                <div class="right-item">
                    <p>(Example)</p>
                </div>
            </div>
        </div>

        <div class="dictionary-item">
            <div class="title">
                <h3>Slang</h3>
            </div>
            <div class="item-content">
                <div class="left-item">
                    <p>Meaning:</p>
                </div>
                <div class="right-item">
                    <p>(Translation)</p>
                </div>
            </div>
            <div class="item-content">
                <div class="left-item">
                    <p>Example:</p>
                </div>
                <div class="right-item">
                    <p>(Example)</p>
                </div>
            </div>
        </div>

        <!-- Successfully Message  -->
        <div class="addSuggest-message">
            <div id="successMessage">
                <p>Successfully added suggestion</p>
                <button onclick="clsBtn()" id="cls-btn">
                    <i class="fa-solid fa-x"></i>
                </button>
            </div>
        </div>
    </section>

</asp:Content>
