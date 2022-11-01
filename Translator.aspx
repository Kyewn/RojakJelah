<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Translator.aspx.cs" Inherits="RojakJelah.Translator" %>
<asp:Content ID="PageStylesheet" ContentPlaceHolderID="PageStylesheet" runat="server">
    <link href="<%= Page.ResolveUrl("~/Content/css/translator.css")%>" type="text/css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

 <!-- Translate  -->
  <section class="translate-content">
    <!-- Banner Section  -->
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
    <!-- Source Message  -->
    <!-- Source Message  -->
    <div class="translate">
        <div class="translate-input">
             <div class="title">
                <h5>Source message</h5>
             </div>
             <div class="translate-container" runat="server">
              <!-- Translate Input  -->
              <div class="context">
                <textarea runat="server" name="inputValue" id="inputValue" rows="3" placeholder="Enter message here..." required></textarea>
              </div>
              <!-- Translate Button  -->
              <div class="translate-button">
                <asp:Button class="btnTranslate" ID="btnTranslate" runat="server" Text="Translate" />
                <div class="translate-user">

                  <!-- Saved Translate -->
                  <div class="button saved">
                    <!-- Button trigger modal -->
                    <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#saved">
                      <i class="fa-solid fa-star"></i>
                    </button>

                    <!-- Modal -->
                    <div class="modal fade" id="saved" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1"
                      aria-labelledby="translateSaved" aria-hidden="true">
                      <div class="modal-dialog">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h1 class="modal-title" id="translateSaved">Saved translations</h1>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"><i
                                class="fa-solid fa-x"></i></button>
                          </div>
                          <div class="modal-body">
                            <div class="body-top">
                              <h2>What is this what why how?</h2>
                              <h4>Translation</h4>
                              <div class="click-btn">
                                <div>
                                  <h3>Apa ni what why how</h3>
                                </div>
                                <div><button>Click Now</button></div>
                              </div>
                              <div class="ext-btn">
                                <button><i class="fa-solid fa-x"></i></button>
                              </div>
                            </div>
                            <div class="body-top">
                              <h2>What is this what why how?</h2>
                              <h4>Translation</h4>
                              <div class="ext-btn">
                                <button><i class="fa-solid fa-x"></i></button>
                              </div>
                            </div>
                            <div class="modal-footer">

                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>


                  <!-- Translation History  -->
                  <div class="button history">
                    <!-- Button trigger modal -->
                    <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#history">
                        <i class="fa-solid fa-clock-rotate-left"></i>
                    </button>

                    <!-- Modal -->
                    <div class="modal fade" id="history" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1"
                      aria-labelledby="translateHistory" aria-hidden="true">
                      <div class="modal-dialog">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h1 class="modal-title" id="translateHistory">Translation history</h1>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"><i
                                class="fa-solid fa-x"></i></button>
                          </div>
                          <div class="modal-body">
                            <div class="body-top">
                              <h2>What is this what why how?</h2>
                              <h4>Translation</h4>
                              <div class="click-btn">
                                <div>
                                  <h3>Apa ni what why how</h3>
                                </div>
                                <div><button>Click Now</button></div>
                              </div>

                            </div>
                            <div class="body-top">
                              <h2>What is this what why how?</h2>
                              <h4>Translation</h4>
                            </div>
                            <div class="modal-footer">

                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Translate Report  -->
                  <div class="button report">
                    <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#report">
                      <i class="fa-solid fa-circle-exclamation"></i>
                    </button>

                    <!-- Modal -->
                    <div class="modal fade" id="report" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1"
                      aria-labelledby="translateReport" aria-hidden="true">
                      <div class="modal-dialog">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h1 class="modal-title" id="translateReport">Report</h1>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"><i
                                class="fa-solid fa-x"></i></button>
                          </div>
                          <div class="modal-body">
                            <div class="body-top">
                              <h4>Issue Category<span>*</span></h4>
                              <form action="#">
                                <select id="label" name="label" required>
                                  <option value="">Label One</option>
                                  <option value="">Label Two</option>
                                  <option value="">Label Three</option>
                                  <option value="">Label Four</option>
                                </select>
                                <h4 class="pt-2">Description</h4>
                                <textarea name="" id="" cols="" rows="2" placeholder="Text here...." required></textarea>
                                <div class="button pt-3">
                                  <div class="px-2"><button type="button" class="cancel" data-bs-dismiss="modal"
                                      aria-label="Close">Cancel</button></div>
                                  <div><button class="submit">Submit</button></div>
                                </div>
                              </form>

                            </div>
                            <div class="modal-footer">

                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
        </div>
        <!-- Translate Output  -->
        <div class="translate-output">           
          <h5>Translation</h5>
          <div class="output">
              <p id="outputString">Test string. When you want to print output, get this component from backend and change the innerHTML of 
                  this p tag once you get the output. this p tag once you get the output.this p tag once you get the output.this p tag once you get the output.this p tag once you get the output.this p tag once you get the output.this p tag once you get the output.this p tag once you get the output.</p>
              <img id="startQuote" src="Content/image/translatorOutput_startQuote.svg"/>
              <img id="endQuote" src="Content/image/translatorOutput_endQuote.svg"/>
          </div>
        </div>
    </div>

  </section>
  <!-- User Generate -->
  <!-- User Generate -->
  <!-- User Generate -->
  <section class="user-generate">
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
        <a target="_blank" href="/About">Learn more</a>
      </div>
    </div>
  </section>

</asp:Content>
