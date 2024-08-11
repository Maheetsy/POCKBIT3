<%@ Page Title="Iniciar sesión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="POCKBIT_v2.Account.Login" Async="true" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="d-flex justify-content-center align-items-center vh-100">
        <div class="card w-50">
            <div class="card-body p-5">
                <main aria-labelledby="title" class="text-center">
                    <h2 id="title" class="mb-4"><%: Title %>.</h2>
                    <asp:Literal ID="ltlAlert" runat="server"></asp:Literal>
                    <section id="loginForm" class="text-left">
                        <h4 class="text-center">Utilice una cuenta local para iniciar sesión.</h4>
                        <hr />
                        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                            <p class="text-danger">
                                <asp:Literal runat="server" ID="FailureText" />
                            </p>
                        </asp:PlaceHolder>
                        <div class="form-group row mb-3 justify-content-center">
                            <label runat="server" AssociatedControlID="Email" CssClass="col-md-4 col-form-label text-md-right">Correo electrónico</label>
                            <div class="col-md-6">
                                <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" CssClass="text-danger" ErrorMessage="El campo de correo electrónico es obligatorio." />
                            </div>
                        </div>
                        <div class="form-group row mb-3 justify-content-center">
                            <label runat="server" AssociatedControlID="Password" CssClass="col-md-4 col-form-label text-md-right">Contraseña</label>
                            <div class="col-md-6">
                                <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="El campo de contraseña es obligatorio." />
                            </div>
                        </div>
                        <div class="form-group row mb-3 justify-content-center">
                            <div class="col-md-6 offset-md-1">
                                <div class="form-check d-flex align-items-center">
                                    <asp:CheckBox runat="server" ID="RememberMe" CssClass="form-check-input" />
                                    <label runat="server" AssociatedControlID="RememberMe" CssClass="form-check-label ml-2">¿Recordar cuenta?</label>
                                </div>
                            </div>
                        </div>
                        <div class="justify-content-center">
                            <div class="col-md-6 offset-md-3">
                                <asp:Button runat="server" OnClick="LogIn" Text="Iniciar sesión" CssClass="btn btn-info w-100" />
                            </div>
                        </div>
                    </section>
                    <section id="socialLoginForm" class="mt-4">
                        <uc:OpenAuthProviders runat="server" ID="OpenAuthLogin" />
                    </section>
                </main>
            </div>
        </div>
    </div>
</asp:Content>
