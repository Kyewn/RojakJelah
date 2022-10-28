<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="RojakJelah.WebForm2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <section class="register">
        <div class="register-container">
            <div class="register-form">
                <div class="register-header">
                    <div class="back-home">
                        <a href="/Translator.aspx"><i class="fa-solid fa-arrow-left"></i> Back To Home</a>
                    </div>
                    <div class="report-message">
                        <p><i class="fa-solid fa-circle-exclamation"></i> Status message title here!</p>
                        <p class="p2">Lorem ipsum dolor sit amet consectetur adipisicing elit. Quibusdam, ducimus.</p>
                    </div>
                </div>
                <div class="register-title">
                    <h1>Register</h1>
                </div>

                <!-- register Form  -->
                <div class="form-container">
                    <div class="register-field">
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
                                <label for="">Conform Password</label><br>
                                <input type="password" placeholder="Label Conform Password" required>
                            </div>
                            <div class="form-field">
                                <button class="register-button">Register</button>
                            </div>
                        </form>
                    </div>

                    <!-- register Right  -->
                    <div class="register-right">
                        <img src="/Content/image/icon_colour 1.png" alt="">
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
