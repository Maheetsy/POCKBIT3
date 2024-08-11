<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TwoFactorVerification.aspx.cs" Inherits="POCKBIT_v2.Account.TwoFactorVerification" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .mb-4-spacing {
            margin-bottom: 1.5rem;
        }

        .btn-center {
            display: flex;
            justify-content: center;
            margin-top: 1rem;
        }

        .card-row {
            display: flex;
            justify-content: center;
            margin-bottom: 2rem;
        }

        .card-row .card {
            width: 100%;
            max-width: 500px; /* Ajuste del tamaño máximo de la card */
            margin: 0 auto; /* Centramos la tarjeta horizontalmente */
        }

        .form-control {
            margin: 0 auto; /* Centra el textbox */
        }

        .form-control-label {
            display: block;
            text-align: center;
            font-weight: 600; /* Asegura que el texto sea semibold */
            margin-bottom: 0.5rem;
        }

        .text-center {
            text-align: center;
        }

        .card-body {
            display: flex;
            flex-direction: column;
            align-items: center; /* Centra verticalmente los elementos */
        }
    </style>

    <div class="text-center mb-4">
        <h2>Verificación de Dos Factores</h2>
    </div>
    <div class="card-row">
        <div class="card">
            <div class="card-body">
                <div class="mb-4-spacing">
                    <label class="form-control-label">Código de Verificación:</label>
                    <asp:TextBox ID="txtCode" runat="server" CssClass="form-control" Placeholder="Código"></asp:TextBox>
                </div>
                <div class="btn-center">
                    <asp:Button ID="btnVerify" runat="server" Text="Verificar" CssClass="btn btn-info" OnClick="btnVerify_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
