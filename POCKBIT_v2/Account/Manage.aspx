<%@ Page Title="Administrar cuenta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="POCKBIT_v2.Account.Manage" %>
<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="text-center mb-4">
        <h2><%: Title %>.</h2>
    </div>
    <asp:Literal ID="ltlAlert" runat="server"></asp:Literal>
    <div class="card">
        <div class="card-body p-3">
            <div>
                <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
                    <p class="text-success"><%: SuccessMessage %></p>
                </asp:PlaceHolder>
            </div>
            <div class="row mb-3">
                <div class="col-md-12">
                    <h4>Cambiar la configuración de la cuenta</h4>
                    <hr />
                    <dl class="dl-horizontal">
                        <dt>Contraseña:</dt>
                        <dd>
                            <asp:HyperLink NavigateUrl="/Account/ManagePassword" Text="[Change]" Visible="false" ID="ChangePassword" runat="server" CssClass="btn btn-link p-0" />
                            <asp:HyperLink NavigateUrl="/Account/ManagePassword" Text="[Create]" Visible="false" ID="CreatePassword" runat="server" CssClass="btn btn-link p-0" />
                        </dd>
                    </dl>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

