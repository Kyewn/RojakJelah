<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RojakJelah.Login" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
    
    <link rel="stylesheet" href="Content/css/login.css" />
    <script src="Content/js/login.js"></script>

</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <section id="sctLogin">
        <!-- Login Page Header -->
        <div id="divLoginHeader">
            <a class="link-back-to-home" href="Default.aspx">
                <i class="fa-solid fa-arrow-left"></i>
                <span class="gray-animated-link-text">Back to Home</span>
            </a>
        </div>

        <!-- Login Form Container -->
        <div class="form-container">
            <!-- Input Form -->
            <div id="divLoginForm">
                <h1 class="form-header">Log in</h1>
                <div class="form-field">
                    <label>Username</label>
                    <input id="txtUsername" runat="server" type="text" maxlength="30" placeholder="Username" autocomplete="off"/>
                </div>
                <div class="form-field">
                    <label>Password</label>
                    <input id="txtPassword" runat="server" type="password" placeholder="Password" />
                </div>
                <asp:Button ID="btnLogin" class="form-button" runat="server" OnClick="BtnLogin_Click" Text="Login" />
                <div class="form-tip">
                    <p>Don't have an account?</p>
                    <a href="Register.aspx" class="red-animated-link-text">Register now</a>
                </div>
            </div>

            <!-- Site Logo -->
            <div class="form-logo">
                <img src="Images/rojakjelah_icon-logo.png" alt="RojakJelah Text Logo" />
            </div>
        </div>
    </section>

     <!-- Status Notification Popup -->
    <div id="notification" class="notification" runat="server" onclick="closeNotification($(this));">
        <div class="notification-title">
            <i class="fa-solid fa-circle-exclamation"></i>
            <h4 id="notificationTitle" runat="server"></h4>
        </div>
        <p id="notificationMessage" class="notification-message" runat="server"></p>
        <small class="notification-tip">CLICK TO CLOSE</small>
    </div>

</asp:Content>