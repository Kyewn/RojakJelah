<%@ Page Title="Register "Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="RojakJelah.Register" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
    
    <link rel="stylesheet" href="Content/css/register.css" />
    <script src="Scripts/Custom/register.js"></script>

</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <section id="sctRegister">
        <!-- Login Page Header -->
        <div id="divRegisterHeader">
            <a class="link-back-to-home" href="Default.aspx">
                <i class="fa-solid fa-arrow-left"></i>
                <span class="gray-animated-link-text">Back to Home</span>
            </a>
        </div>

        <!-- Login Form Container -->
        <div class="form-container">
            <!-- Input Form -->
            <div id="divRegisterForm">
                <h1 class="form-header">Register</h1>
                <div class="form-field">
                    <label>Username</label>
                    <input id="txtUsername" runat="server" type="text" maxlength="30" placeholder="Username" autocomplete="off" />
                </div>
                <div class="form-field">
                    <label>Password</label>
                    <input id="txtPassword" runat="server" type="password" placeholder="Password" />
                </div>
                <div class="form-field">
                    <label>Confirm Password</label>
                    <input id="txtConfirmPassword" runat="server" type="password" placeholder="Confirm Password" />
                </div>
                <asp:Button ID="btnRegister" class="form-button" runat="server" OnClick="BtnRegister_Click" Text="Register" />
                <div class="form-tip">
                    <p>Already have an account?</p>
                    <a href="Login.aspx" class="red-animated-link-text">Log in now</a>
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
