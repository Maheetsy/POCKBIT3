<%@ Page Title="Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="POCKBIT_v2.Paginas.Clientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="text-center mb-4">
        <h2>Gestión de Clientes</h2>
    </div>

    <asp:Literal ID="ltlAlert" runat="server"></asp:Literal>

    <div class="card-row">
        <div class="card">
            <div class="card-body p-3">
                <div class="row mb-3">
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">ID:</label>
                        <asp:Label ID="lblIdCliente" runat="server" CssClass="form-control"></asp:Label>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Nombre:</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Dirección:</label>
                        <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Teléfono:</label>
                        <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Email:</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Estado:</label>
                        <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Activo" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Inactivo" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row mb-2 btn-center">
                    <div class="col-md-3 mb-2">
                        <asp:Button ID="btnInsertar" runat="server" Text="Insertar" CssClass="btn btn-success w-100" OnClick="btnInsertar_Click" />
                    </div>
                    <div class="col-md-3 mb-2">
                        <asp:Button ID="btnModificar" runat="server" Text="Modificar" CssClass="btn btn-info w-100" OnClick="btnModificar_Click" />
                    </div>
                    <div class="col-md-3 mb-2">
                        <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" CssClass="btn btn-danger w-100" OnClick="btnEliminar_Click" />
                    </div>
                    <div class="col-md-3 mb-2">
                        <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar a Excel" CssClass="btn btn-primary w-100" OnClick="btnExportarExcel_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <br />
    <div class="table-responsive rounded-3">
    <asp:GridView ID="GVClientes" runat="server" CssClass="table custom-table" CellPadding="4" ForeColor="#333333" GridLines="None"
        AutoGenerateColumns="False" DataKeyNames="id_cliente" DataSourceID="SqlDataSourceClientes"
        AllowPaging="True" PageSize="10" OnSelectedIndexChanged="GVClientes_SelectedIndexChanged" OnRowDataBound="GVClientes_RowDataBound">
        <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
        <Columns>
            <asp:CommandField ShowSelectButton="True" ButtonType="Button" ControlStyle-CssClass="btn btn-info"></asp:CommandField>
            <asp:BoundField DataField="id_cliente" HeaderText="ID" ReadOnly="True" SortExpression="id_cliente"></asp:BoundField>
            <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre"></asp:BoundField>
            <asp:BoundField DataField="direccion" HeaderText="Dirección" SortExpression="direccion"></asp:BoundField>
            <asp:BoundField DataField="telefono" HeaderText="Teléfono" SortExpression="telefono" ></asp:BoundField>
            <asp:BoundField DataField="email" HeaderText="Email" SortExpression="email"></asp:BoundField>
            <asp:BoundField DataField="fecha_registro" HeaderText="Fecha Registro" SortExpression="fecha_registro"
                DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="False"></asp:BoundField>
            <asp:CheckBoxField DataField="activo" HeaderText="Activo" SortExpression="activo"></asp:CheckBoxField>
        </Columns>
        <HeaderStyle BackColor="#03c3ec" Font-Bold="True" ForeColor="White" ></HeaderStyle>
        <HeaderStyle BackColor="#03c3ec" Font-Bold="True" ForeColor="White" Font-Overline="false" Font-Underline="false" Font-Strikeout="false"></HeaderStyle>
        <EditRowStyle BackColor="#2461BF"></EditRowStyle>
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"></FooterStyle>
        <PagerStyle HorizontalAlign="Center" BackColor="#2461BF" ForeColor="White"></PagerStyle>
        <RowStyle BackColor="#EFF3FB"></RowStyle>
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333"></SelectedRowStyle>
        <SortedAscendingCellStyle BackColor="#F5F7FB"></SortedAscendingCellStyle>
        <SortedAscendingHeaderStyle BackColor="#6D95E1"></SortedAscendingHeaderStyle>
        <SortedDescendingCellStyle BackColor="#E9EBEF"></SortedDescendingCellStyle>
        <SortedDescendingHeaderStyle BackColor="#4870BE"></SortedDescendingHeaderStyle>
    </asp:GridView>

        <asp:SqlDataSource ID="SqlDataSourceClientes" runat="server"
            ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
            SelectCommand="SELECT id_cliente, nombre, direccion, telefono, email, fecha_registro, activo FROM ViewCliente ORDER BY id_cliente DESC"></asp:SqlDataSource>
    </div>
</asp:Content>