<%@ Page Title="Compras" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="compras.aspx.cs" Inherits="POCKBIT_v2.Paginas.compras" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="text-center mb-4">
        <h2>Compras De Medicamentos</h2>
    </div>
    <asp:Literal ID="ltlAlert" runat="server"></asp:Literal>
    <div class="card-row">
        <div class="card">
            <div class="card-body p-3">
                <div class="row mb-3">
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">ID:</label>
                        <asp:Label ID="lblId" runat="server" CssClass="form-control"></asp:Label>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Código de Barras:</label>
                        <asp:DropDownList ID="ddlCodigoB" runat="server" CssClass="form-select" AutoPostBack="True" DataSourceID="SqlDataSourceCodigosBarras" DataTextField="codigo_de_barras" DataValueField="id_medicamento"></asp:DropDownList>
                        <asp:SqlDataSource runat="server" ID="SqlDataSourceCodigosBarras" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_medicamento, codigo_de_barras FROM medicamento WHERE (activo = 1)"></asp:SqlDataSource>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Seleccionar Lote:</label>
                        <asp:DropDownList ID="ddlLote" runat="server" CssClass="form-select" DataSourceID="SqlDataSourceLotes" DataTextField="numero_de_lote" DataValueField="id_lote"></asp:DropDownList>
                        <asp:SqlDataSource runat="server" ID="SqlDataSourceLotes" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_lote, numero_de_lote FROM lote WHERE (id_medicamento = @id_medicamento) AND (activo = 1) ORDER BY id_lote DESC">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlCodigoB" PropertyName="SelectedValue" Name="id_medicamento"></asp:ControlParameter>
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Cantidad comprada:</label>
                        <asp:TextBox ID="txtCantidadC" runat="server" Placeholder="Numeros enteros" CssClass="form-control"></asp:TextBox>
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
        <asp:GridView ID="GVCompras" runat="server" CssClass="table custom-table" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" DataKeyNames="id_compra" DataSourceID="SqlDataSourceCompras" AllowSorting="True" OnSelectedIndexChanged="GVCompras_SelectedIndexChanged" OnRowDataBound="GVCompras_RowDataBound" AllowPaging="True">
            <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
            <Columns>
                <asp:CommandField ShowSelectButton="True" ButtonType="Button"></asp:CommandField>
                <asp:BoundField DataField="id_compra" HeaderText="ID" ReadOnly="True" SortExpression="id_compra"></asp:BoundField>
                <asp:BoundField DataField="codigo_de_barras" HeaderText="Código De Barras" SortExpression="codigo_de_barras"></asp:BoundField>
                <asp:BoundField DataField="numero_de_lote" HeaderText="Número De Lote" SortExpression="numero_de_lote"></asp:BoundField>
                <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre"></asp:BoundField>
                <asp:BoundField DataField="laboratorio" HeaderText="laboratorio" SortExpression="laboratorio"></asp:BoundField>
                <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="costo" HeaderText="Costo" SortExpression="costo" DataFormatString="{0:C2}" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="costo_total" HeaderText="Costo Total" SortExpression="costo_total" DataFormatString="{0:C2}" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="fecha_caducidad" HeaderText="Fecha De Caducidad" SortExpression="fecha_caducidad" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundField>
                <asp:BoundField DataField="fecha_de_entrada" HeaderText="Fecha De Entrada" SortExpression="fecha_de_entrada" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundField>
                <asp:BoundField DataField="realizado_por" HeaderText="Realizado Por" SortExpression="realizado_por"></asp:BoundField>
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
    <asp:SqlDataSource runat="server" ID="SqlDataSourceCompras" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_compra, codigo_de_barras, numero_de_lote, nombre, laboratorio, cantidad, costo, costo_total, fecha_caducidad, fecha_de_entrada, realizado_por FROM ViewCompra ORDER BY id_compra DESC"></asp:SqlDataSource>
</asp:Content>
