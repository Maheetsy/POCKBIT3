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
        <p class="lead card-header">ASP.NET is a web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-info btn-md">Learn more &raquo;</a></p>
    </div>
    <main class="content">
        <section class="text-center py-3 text-light">
            <p class="mt-3 text-muted small">example</p>
            <div class="d-flex justify-content-center mt-4">
                <img class="img-fluid rounded-lg" src="https://images.unsplash.com/photo-1556761175-5973dc0f32e7?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8MHx8&auto=format&fit=crop&w=1632&q=80" alt="Image" />
            </div>
        </section>
    </main>
</asp:Content>
