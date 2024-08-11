<%@ Page Title="Laboratorios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="laboratorios.aspx.cs" Inherits="POCKBIT_v2.Paginas.laboratorios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="text-center mb-4">
        <h2>Laboratorios</h2>
    </div>
    <asp:Literal ID="ltlAlert" runat="server"></asp:Literal>
    <div class="card-row">
        <div class="card">
            <div class="card-body p-3">
                <div class="row mb-3 row-center">
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">ID:</label>
                        <asp:Label ID="lblId" runat="server" CssClass="form-control"></asp:Label>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Laboratorio:</label>
                        <asp:TextBox ID="txtNombreL" runat="server" Placeholder="Nombre Del Laboratorio" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Estado:</label>
                        <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Activo" Value="1"></asp:ListItem>
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
        <asp:GridView ID="GRVLaboratorios" runat="server" CssClass="table custom-table" AutoGenerateColumns="False" DataKeyNames="id_laboratorio" DataSourceID="SqlDataSourceLaboratorios" AllowSorting="True" CellPadding="4" ForeColor="#333333" GridLines="None" OnSelectedIndexChanged="GRVLaboratorios_SelectedIndexChanged" OnRowDataBound="GRVLaboratorios_RowDataBound" AllowPaging="True">
            <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
            <Columns>
                <asp:CommandField ShowSelectButton="True" ButtonType="Button"></asp:CommandField>
                <asp:BoundField DataField="id_laboratorio" HeaderText="ID" ReadOnly="True" InsertVisible="False" SortExpression="id_laboratorio"></asp:BoundField>
                <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre"></asp:BoundField>
                <asp:CheckBoxField DataField="activo" HeaderText="Activo" SortExpression="activo"></asp:CheckBoxField>
            </Columns>
            <EditRowStyle BackColor="#2461BF"></EditRowStyle>
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"></FooterStyle>
            <HeaderStyle BackColor="#03c3ec" Font-Bold="True" ForeColor="White"></HeaderStyle>
            <PagerStyle HorizontalAlign="Center" BackColor="#2461BF" ForeColor="White"></PagerStyle>
            <RowStyle BackColor="#EFF3FB"></RowStyle>
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333"></SelectedRowStyle>
            <SortedAscendingCellStyle BackColor="#F5F7FB"></SortedAscendingCellStyle>
            <SortedAscendingHeaderStyle BackColor="#6D95E1"></SortedAscendingHeaderStyle>
            <SortedDescendingCellStyle BackColor="#E9EBEF"></SortedDescendingCellStyle>
            <SortedDescendingHeaderStyle BackColor="#4870BE"></SortedDescendingHeaderStyle>
        </asp:GridView>
    </div>
    <asp:SqlDataSource runat="server" ID="SqlDataSourceLaboratorios" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT [id_laboratorio], [nombre], [activo] FROM [laboratorio] ORDER BY id_laboratorio DESC"></asp:SqlDataSource>
</asp:Content>
