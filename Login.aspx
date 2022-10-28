<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RojakJelah.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

       <section class="login">
        <div class="login-container">
            <div class="login-form">
                <div class="login-header">
                    <div class="back-home">
                        <a href="Translator.aspx"><i class="fa-solid fa-arrow-left"></i> Back To Home</a>
                    </div>
                    <div class="report-message">
                        <p><i class="fa-solid fa-circle-exclamation"></i> Status message title here!</p>
                        <p class="p2">Lorem ipsum dolor sit amet consectetur adipisicing elit. Quibusdam, ducimus.</p>
                    </div>
                </div>
                <div class="login-title">
                    <h1>Login</h1>
                </div>

                <!-- Login Form  -->
                <div class="form-container">
                    <div class="login-field">
                        <form action="#">
                            <div class="form-field">
                                <label for="">Username</label><br>
                                <input type="text" placeholder="Label Name" required>
                            </div>
                            <div class="form-field">
                                <label for="">Password</label><br>
                                <input type="password" placeholder="Label Password" required>
                            </div>
                            <div class="form-field">
                                <button class="login-button">Login</button>
                            </div>
                        </form>
                        <div class="register">
                            <p>Don't have and account?</p>
                            <a href="Register.aspx">Register now</a>
                        </div>
                    </div>

                    <!-- Login Right  -->
                    <div class="login-right">
                        <img src="./Content/image/icon_colour 1.png" alt="">
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
