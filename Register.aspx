<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="RojakJelah.WebForm2" %>


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
    <link rel="stylesheet" href="Content/css/bootstrap.min.css">


    <!-- styles -->
    <link rel="stylesheet" href="Content/css/register.css">
</head>

<body>

    <section class="register">
        <div class="register-container">
            <div class="register-form">
                <div class="register-header">
                    <div class="back-home">
                        <a href="Translator.aspx"><i class="fa-solid fa-arrow-left"></i> Back To Home</a>
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
                        <img src="Content/image/icon_colour 1.png" alt="">
                    </div>
                </div>
            </div>
        </div>
    </section>

</body>

</html>
