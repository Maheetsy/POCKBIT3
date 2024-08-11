<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="POCKBIT_v2._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body, html {
            margin: 0;
            padding: 0;
            height: 100%;
        }
        .background-image {
            background-image: url('<%= ResolveUrl("~/assets/img/backgrounds/pexels-cottonbro-5722883.jpg") %>');
            background-size: cover;
            background-position: center;
            background-repeat: no-repeat;
            height: 100vh;
            width: 100%;
            position: absolute;
            top: 0;
            left: 0;
            z-index: -1;
        }
        .overlay-content {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            text-align: center;
            color: white; /* Ajusta el color del texto según sea necesario */
        }
        .content {
            margin-top: 100vh;
        }
    </style>
    <div class="background-image"></div>
    <div class="overlay-content">
        <h1 id="aspnetTitle" class="text-white">POCKBIT PHARMA</h1>
        <p><a href="~/Account/Login" class="btn btn-info btn-md">Iniciar Sesión &raquo;</a></p>
    </div>
    <main class="content">
    </main>
</asp:Content>
