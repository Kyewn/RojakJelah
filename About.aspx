<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="RojakJelah.About" %>
<asp:Content ID="PageStylesheet" ContentPlaceHolderID="PageStylesheet" runat="server">
    <link href="<%= Page.ResolveUrl("~/Content/css/about.css")%>" type="text/css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
       <!-- About Us  -->
    <section class="about-us">
        <div class="about-title">
            <h1>About Us</h1>
        </div>
        <div class="about-info">
            <p>RojakJelah is a Bahasa Rojak translation platform where users can exchange ideas and information about Bahasa Rojak. Bahasa Rojak itself is a weird language, so why not make a translator for it? 
                Powered by hardcore Malaysian users, we think that users would enjoy using the translator whether if its for entertainment or if they are genuinely interested in knowing how the language works.</p>
        </div>


        <div class="about-container">

            <div class="about">

                <div class="about-context">
                    <div class="title">
                        <h3>Purpose</h3>
                        <div class="about-info">
                            <p>We have our purpose for creating this translator, which is to </p>
                            <ul>
                                <li>Educate people about what rojak people mean
                                </li>
                                <li>Make Bahasa Rojak great again
                                </li>
                                <li>Just letting people have fun!
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>

                <div class="about-context">
                    <div class="title">
                        <h3>Credit</h3>
                        <div class="about-info">
                            <p>This translator system is made possible by these fantastic people: </p>
                            <ul>
                                <li>0205096 Thor Wen Zheng</li>
                                <li>0204677 Lim Zhe Yuan</li>
                                <li>0205213 Deshigan A/L Ganesan</li>
                                <li>0205430 Tan Peng Heng</li>
                                <li>0205603 Khor Han Yang</li>
                            </ul>
                        </div>
                    </div>
                </div>

                <div class="about-context">
                    <div class="title">
                        <h3>That’s all, have fun with Bahasa Rojak!</h3>
                    </div>
                </div>

            </div>

            <div class="about-photo">
                <img src="Content/image/about-photo.svg" alt="">
            </div>

        </div>
    </section>
</asp:Content>
