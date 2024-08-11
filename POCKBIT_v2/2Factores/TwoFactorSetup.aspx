<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TwoFactorSetup.aspx.cs" Inherits="POCKBIT_v2._2Factores.TwoFactorSetup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .form-control, .form-control-label {
            height: 38px;
            width: 100%;
        }

        .form-control-label {
            display: flex;
            align-items: center;
            justify-content: center;
            text-align: center;
        }

        .mb-4-spacing {
            margin-bottom: 1.5rem;
        }

        .btn-center {
            display: flex;
            justify-content: center;
        }

        .btn-center .col-md-3 {
            margin-left: 1rem;
            margin-right: 1rem;
        }

        .row-center {
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .qr-code-image {
            width: 200px;
            height: 200px;
            margin: 0 auto;
        }
    </style>
    <div class="text-center mb-4">
        <h2>Configurar Autentificador de Google</h2>
    </div>

    <div class="row row-center mb-4">
        <div class="col-md-6 text-center">
            <asp:Image ID="qrCodeImageControl" runat="server" CssClass="qr-code-image" />
        </div>
    </div>

    <div class="row row-center mb-4">
        <div class="col-md-6 mb-4-spacing">
            <label class="form-control-label">Código de Verificación:</label>
            <div class="row row-center">
                <div class="col-md-6">
                    <asp:TextBox ID="txtCode" runat="server" CssClass="form-control" Placeholder="Ingrese el código de verificación"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>

    <div class="row btn-center">
        <div class="col-md-3 mb-3">
            <asp:Button ID="btnVerify" runat="server" Text="Verificar" CssClass="btn btn-info w-100" OnClick="btnVerify_Click" />
        </div>
    </div>
</asp:Content>
