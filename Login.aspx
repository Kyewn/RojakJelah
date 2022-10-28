<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RojakJelah.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Login</title>
    
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Mitr:wght@200;300;400;500;600;700&family=Rubik:ital,wght@0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.1/jquery.min.js"></script>

    <style>
        * {
            padding: 0;
            margin: 0;
            box-sizing: border-box;
        }

        body {
            display: flex;
            align-items: center;
            justify-content: center;
            position: absolute;
            top: 0;
            left: 0;
            height: 100%;
            width: 100%;
            background-color: #D35555;
        }

        /* Notification */
        .notification {
            position: fixed;
            top: 2rem;
            right: 2rem;
            padding: 0.5rem 1.5rem;
            width: 300px;
            background-color: #e8ac13;
            border: 1px solid white;
            border-radius: 4px;
            box-shadow: 0 0 0 4px #e8ac13;
        }

        .notification:hover {
            cursor: pointer;
            opacity: 70%;
            transition: opacity linear 0.5s ease-out;
        }

        .notification-title {
            width: 100%;
            color: white;
            font-family: 'Mitr', Arial, sans-serif;
            font-weight: 500;
            word-wrap: break-word;
        }

        .notification-message {
            padding-top: 0.3rem;
            width: 100%;
            color: white;
            font-family: 'Rubik', Arial, sans-serif;
            font-size: 13px;
            font-weight: 400;
            word-wrap: break-word;
        }

        .notification-tip {
            display: block;
            padding-top: 0.6rem;
            color: white;
            font-family: 'Rubik', Arial, sans-serif;
            text-align: center;
            letter-spacing: 1px;
        }

        /* Main Container */
        #sctLogin {
            margin: auto;
            padding: 0 2rem;
            width: 98%;
            height: 96%;
            background-color: white;
            border: 4px #D35555;
            border-radius: 8px;
        }

        /* Login Page Header */
        #divLoginHeader {
            display: flex;
            align-items: center;
            width: 100%;
            height: 14%;
        }

        .link-back-to-home {
            margin-left: 8rem;
            color: #323232;
            font-family: 'Rubik', Arial, sans-serif;
            font-weight: 500;
            text-decoration: none;
        }

        .link-back-to-home i {
            margin-right: 0.5rem;
        }

        .gray-animated-link-text {
          display: inline-block;
        }

        .gray-animated-link-text::after {
          content: '';
          width: 0px;
          height: 1.5px;
          display: block;
          background: #323232;
          transition: 300ms;
        }

        .gray-animated-link-text:hover::after {
          width: 100%;
        }

        .red-animated-link-text {
          display: inline-block;
        }

        .red-animated-link-text::after {
          content: '';
          width: 0px;
          height: 1.5px;
          display: block;
          background: #d35555;
          transition: 300ms;
        }

        .red-animated-link-text:hover::after {
          width: 100%;
        }

        /* Login Form */
        #divLoginForm {
            display: grid;
            grid-template-columns: 1fr 1fr;
            width: 100%;
            height: 86%;
        }

        #frmLoginDiv {
            display: grid;
            grid-template-columns: 1fr;
            grid-template-rows: 1.2fr 1fr 1fr 1fr 0.5fr;
            grid-gap: 0.3rem 0;
            padding: 0 8rem;
            align-items: center;
        }

        .form-header {
            color: #D35555;
            font-family: 'Mitr', Arial, sans-serif;
            font-size: 72px;
            font-weight: 500;
        }

        .form-input label {
            display: block;
            padding-bottom: 4px;
            color: #D35555;
            font-family: 'Rubik', Arial, sans-serif;
            font-weight: 500;
        }

        .form-input input {
            width: 100%;
            padding: 1rem 0.5rem;
            border: 2px solid #d35555;
            border-radius: 2px;
            font-family: 'Rubik', Arial, sans-serif;
            font-size: 16px;
            font-weight: 400;
        }

        .form-input input:focus {
            box-shadow: 0 0 0 2px #d35555;
            outline: none;
        }

        .form-button {
            width: 100%;
            height: 60%;
            background-color: #d35555;
            border: none;
            border-radius: 3px;
            color: white;
            font-family: 'Rubik', Arial, sans-serif;
            font-size: 20px;
            font-weight: 500;
            letter-spacing: 1px;
        }

        .form-button:hover {
            background-color: #e37373;
            cursor: pointer;
        }

        .form-button:active {
            background-color: #ef8484;
        }

        #divNoAccount {
            font-family: 'Rubik', Arial, sans-serif;
            text-align: center;
        }

        #divNoAccount a {
            color: #d35555;
            text-decoration: none;
        }

        #divLoginLogo img {
            display: block;
            width: 60%;
            height: auto;
            margin: auto;
        }
    </style>
</head>

<body>
    <!-- Main Container -->
    <section id="sctLogin">
        <!-- Login Page Header -->
        <div id="divLoginHeader">
            <a class="link-back-to-home" href="Default.aspx">
                <i class="fa-solid fa-arrow-left"></i>
                <span class="gray-animated-link-text">Back to Home</span>
            </a>
        </div>

        <!-- Login Form Container -->
        <div id="divLoginForm">
            <!-- Input Form -->
            <form id="frmLogin" runat="server">
                <div id="frmLoginDiv">
                    <h1 class="form-header">Log in</h1>
                    <div class="form-input">
                        <label>Username</label>
                        <input type="text" placeholder="Username" />
                    </div>
                    <div class="form-input">
                        <label>Password</label>
                        <input type="password" placeholder="Password" />
                    </div>
                    <asp:Button ID="btnLogin" class="form-button" runat="server" OnClick="BtnLogin_Click" Text="Login" />
                    <div id="divNoAccount">
                        <p>Don't have an account?</p>
                        <a href="Register.aspx" class="red-animated-link-text">Register now</a>
                    </div>
                </div>
            </form>

            <!-- Site Logo -->
            <div id="divLoginLogo">
                <img src="Images/rojakjelah_logo-combined.png" alt="RojakJelah Text Logo" />
            </div>
        </div>
    </section>

    <!-- Status Notification Popup -->
    <section class="notification" onclick="$(this).css('display', 'none');">
        <h3 class="notification-title">
            <i class="fa-solid fa-circle-exclamation"></i>
            Status message title
        </h3>
        <p class="notification-message">
            Lorem ipsum dolor sit ametaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
        </p>
        <small class="notification-tip">
            CLICK TO CLOSE
        </small>
    </section>
</body>

</html>
