<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="POCKBIT_v2._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body, html {
            margin: 0;
            padding: 0;
            height: 100%;
        }

        /* Fondo de la página */
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

        /* Contenido central */
        .overlay-content {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            text-align: center;
            color: white;
            z-index: 10;
        }

        .content {
            margin-top: 100vh;
        }

        /* Animación de píldoras mejorada */
        .pill {
            position: absolute;
            background: linear-gradient(to right, #0288d1 50%, white 50%); /* Mitad azul oscuro, mitad blanco */
            border-radius: 5px;
            animation: fallAndRotate 8s infinite linear;
            opacity: 0;
        }

        

        @keyframes fallAndRotate {
            0% { 
                transform: translateY(-100vh) rotate(0deg);
                opacity: 0; /* Inicio invisible */
            }
            5% { 
                opacity: 0.5; /* Aparece gradualmente */
            }
            95% { 
                opacity: 0.8; /* Mantiene visible */
            }
            100% { 
                transform: translateY(100vh) rotate(360deg);
                opacity: 0; /* Desaparece al final */
            }
        }

        /* Variaciones de tamaño, posición y velocidad */
        .pill-1 { width: 30px; height: 10px; left: 5%; top: 10%; animation-duration: 7s; animation-delay: 0s; }
        .pill-2 { width: 25px; height: 8px; left: 20%; top: 20%; animation-duration: 9s; animation-delay: 1s; }
        .pill-3 { width: 35px; height: 12px; left: 35%; top: 5%; animation-duration: 6s; animation-delay: 2s; }
        .pill-4 { width: 20px; height: 7px; left: 50%; top: 30%; animation-duration: 8s; animation-delay: 3s; }
        .pill-5 { width: 30px; height: 10px; left: 65%; top: 15%; animation-duration: 7s; animation-delay: 4s; }
        .pill-6 { width: 25px; height: 8px; left: 80%; top: 25%; animation-duration: 9s; animation-delay: 5s; }
        .pill-7 { width: 35px; height: 12px; left: 10%; top: 40%; animation-duration: 6s; animation-delay: 6s; }
        .pill-8 { width: 20px; height: 7px; left: 30%; top: 60%; animation-duration: 8s; animation-delay: 7s; }
        .pill-9 { width: 30px; height: 10px; left: 70%; top: 50%; animation-duration: 7s; animation-delay: 8s; }
        .pill-10 { width: 25px; height: 8px; left: 90%; top: 70%; animation-duration: 9s; animation-delay: 9s; }
        .pill-11 { width: 30px; height: 10px; left: 15%; top: 80%; animation-duration: 6s; animation-delay: 10s; }
        .pill-12 { width: 25px; height: 8px; left: 40%; top: 90%; animation-duration: 8s; animation-delay: 11s; }
        .pill-13 { width: 35px; height: 12px; left: 60%; top: 35%; animation-duration: 7s; animation-delay: 12s; }
        .pill-14 { width: 20px; height: 7px; left: 85%; top: 45%; animation-duration: 9s; animation-delay: 13s; }
        .pill-15 { width: 30px; height: 10px; left: 25%; top: 65%; animation-duration: 6s; animation-delay: 14s; }

        /* Animaciones del contenido */
        .hero-section {
            position: relative;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            color: white;
            text-align: center;
            z-index: 5;
        }

        .hero-section h1 {
            font-size: 3rem;
            font-weight: 700;
            margin-bottom: 1rem;
            animation: fadeInTitle 2s ease-out;
            color: white; /* Aseguramos que el texto sea blanco */
        }

        /* Animación de aparición del título */
        @keyframes fadeInTitle {
            0% {
                opacity: 0;
                transform: translateY(20px);
            }
            100% {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .hero-section p {
            font-size: 1.2rem;
            margin-bottom: 1.5rem;
            animation: fadeInUp 2s ease-out;
        }

        /* Animación de aparición */
        @keyframes fadeInUp {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        /* Botón "Iniciar sesión" */
        .btn-info2 {
            background-color: #0288d1;
            border-color: #0288d1;
            color: white;
            font-size: 1.5rem;
            font-weight: 600;
            padding: 0.5rem 1.5rem;
            border-radius: 25px;
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }

        .btn-info2:hover {
            background-color: #03a9f4;
            border-color: #03a9f4;
            transform: scale(1.05);
            color: whitesmoke;
        }
    </style>

    <!-- Fondo de la página -->
    <div class="background-image">
        <div class="pill pill-1"></div>
        <div class="pill pill-2"></div>
        <div class="pill pill-3"></div>
        <div class="pill pill-4"></div>
        <div class="pill pill-5"></div>
        <div class="pill pill-6"></div>
        <div class="pill pill-7"></div>
        <div class="pill pill-8"></div>
        <div class="pill pill-9"></div>
        <div class="pill pill-10"></div>
        <div class="pill pill-11"></div>
        <div class="pill pill-12"></div>
        <div class="pill pill-13"></div>
        <div class="pill pill-14"></div>
        <div class="pill pill-15"></div>
    </div>

    <!-- Contenido principal -->
    <div class="overlay-content">
        <!-- Píldoras en la animación -->
        

        <!-- Texto y botón -->
        <h1 style="color: white; font-weight: bold; font-size: 60px;">POCKBIT PHARMA</h1>
        <p style="color: white; font-weight: bold; text-shadow: 4px 4px 8px navy;">Tu solución integral en gestión farmacéutica</p>

        <asp:LoginView runat="server" ViewStateMode="Disabled">
            <AnonymousTemplate>
                <a runat="server" class="btn-info2" href="~/Account/Login">Iniciar Sesión &raquo;</a>
            </AnonymousTemplate>
        </asp:LoginView>
    </div>

    <!-- Espacio para el contenido adicional -->
    <main class="content"></main>

</asp:Content>