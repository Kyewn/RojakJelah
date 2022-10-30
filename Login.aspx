<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RojakJelah.WebForm1" %>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>RojakJeLah-Translator </title>

    <!-- Google Font  -->
    <link
        href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800;900&family=Mitr:wght@300;400;500;600;700&family=Rubik:ital,wght@0,400;0,500;0,600;0,700;0,800;0,900;1,400;1,500;1,600;1,700;1,800;1,900&display=swap"
        rel="stylesheet">


    <!-- font-awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">

    <!-- Bootstrap Css  -->
    <link href="Content/css/bootstrap.min.css" rel="stylesheet" />


    <!-- styles -->
    <link href="Content/css/login.css" rel="stylesheet" />
</head>

<body>

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
                            <a href="register.aspx">Register now</a>
                        </div>
                    </div>

                    <!-- Login Right  -->
                    <div class="login-right">
                        <img src="Content/image/icon_colour 1.png" alt="">
                    </div>
                </div>
            </div>
        </div>
    </section>

</body>

</html>
