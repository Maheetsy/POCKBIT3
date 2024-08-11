<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="POCKBIT_v2._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body, html {
            margin: 0;
            padding: 0;
            height: 100%;
        }
        .background-image {
            background-image: url('<%= ResolveUrl("~/assets/img/backgrounds/pexels-koprivakart-3634855.jpg") %>');
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
        <h1 id="aspnetTitle" class="text-info">POCKBIT PHARMA</h1>
        <p class="lead card-header"></p>
        <p><a href="~/Paginas/dashboard" class="btn btn-info btn-md">Iniciar Sesión &raquo;</a></p>
    </div>
    <main class="content">
        <section class="text-center py-3 text-light">
            <p class="mt-3 text-muted small">example</p>
            <div class="d-flex justify-content-center mt-4">
            </div>
        </section>
    </main>
</asp:Content>
