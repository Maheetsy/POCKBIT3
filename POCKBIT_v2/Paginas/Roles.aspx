<%@ Page Title="Gestión de Roles" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="POCKBIT_v2.Paginas.Roles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Gestión de Roles de Usuario</h2>

    <!-- Asignar Rol -->
    <asp:Panel runat="server" CssClass="form-group">
        <h3>Asignar Rol a Usuario</h3>

        <label for="ddlUsuarios">Usuario:</label>
        <asp:DropDownList ID="ddlUsuarios" runat="server" CssClass="form-control" />

        <label for="ddlRoles">Rol:</label>
        <asp:DropDownList ID="ddlRoles" runat="server" CssClass="form-control" />

        <asp:Button ID="btnAsignarRol" runat="server" Text="Asignar Rol" CssClass="btn btn-primary mt-2" OnClick="btnAsignarRol_Click" />
    </asp:Panel>

    <!-- Crear Nuevo Rol -->
    <asp:Panel runat="server" CssClass="form-group mt-4">
        <h3>Agregar Nuevo Rol</h3>

        <label for="txtNuevoRol">Nombre del Rol:</label>
        <asp:TextBox ID="txtNuevoRol" runat="server" CssClass="form-control" />

        <asp:Button ID="btnAgregarRol" runat="server" Text="Agregar Rol" CssClass="btn btn-success mt-2" OnClick="btnAgregarRol_Click" />
    </asp:Panel>

    <!-- Eliminar Rol -->
    <asp:Panel runat="server" CssClass="form-group mt-4">
        <h3>Eliminar Rol</h3>

        <label for="ddlRolesEliminar">Seleccionar Rol:</label>
        <asp:DropDownList ID="ddlRolesEliminar" runat="server" CssClass="form-control" />

        <asp:Button ID="btnEliminarRol" runat="server" Text="Eliminar Rol" CssClass="btn btn-danger mt-2" OnClick="btnEliminarRol_Click" />
    </asp:Panel>

    <!-- Usuarios con Roles -->
    <asp:Panel runat="server" CssClass="mt-5">
        <h3>Usuarios con Roles</h3>
        <asp:GridView ID="gvUsuariosRoles" runat="server" CssClass="table table-striped" AutoGenerateColumns="true" />
    </asp:Panel>

    <!-- Todos los Roles -->
    <asp:Panel runat="server" CssClass="mt-5">
        <h3>Listado de Roles Existentes</h3>
        <asp:GridView ID="gvRoles" runat="server" CssClass="table table-bordered" AutoGenerateColumns="true" />
    </asp:Panel>

    <!-- Mensaje -->
    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert mt-3">
        <asp:Label ID="lblMensaje" runat="server" />
    </asp:Panel>
</asp:Content>
