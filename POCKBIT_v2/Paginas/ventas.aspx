<%@ Page Title="Ventas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ventas.aspx.cs" Inherits="POCKBIT_v2.Paginas.ventas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="text-center mb-4">
        <h2>Ventas De Medicamentos</h2>
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
                        <asp:SqlDataSource runat="server" ID="SqlDataSourceCodigosBarras" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>'
                            SelectCommand="SELECT DISTINCT m.id_medicamento, m.codigo_de_barras FROM medicamento m INNER JOIN lote l ON m.id_medicamento = l.id_medicamento WHERE m.activo = 1 AND l.activo = 1 AND l.cantidad > 0"></asp:SqlDataSource>
                    </div>

                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Seleccionar Lote:</label>
                        <asp:DropDownList ID="ddlLote" runat="server" CssClass="form-select" DataSourceID="SqlDataSourceLotes" DataTextField="numero_de_lote" DataValueField="id_lote"></asp:DropDownList>
                        <asp:SqlDataSource runat="server" ID="SqlDataSourceLotes" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_lote, numero_de_lote FROM lote WHERE (id_medicamento = @id_medicamento) AND (activo = 1) AND (cantidad > 0) ORDER BY id_lote DESC">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlCodigoB" PropertyName="SelectedValue" Name="id_medicamento"></asp:ControlParameter>
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Cantidad Vendida:</label>
                        <asp:TextBox ID="txtCantidadV" runat="server" Placeholder="Numeros enteros" CssClass="form-control"></asp:TextBox>
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
        <asp:GridView ID="GVVentas" runat="server" CssClass="table custom-table" DataSourceID="SqlDataSourceVentas" AutoGenerateColumns="False" DataKeyNames="id_venta" AllowSorting="True" CellPadding="4" ForeColor="#333333" GridLines="None" OnSelectedIndexChanged="GVVentas_SelectedIndexChanged" OnRowDataBound="GVVentas_RowDataBound" AllowPaging="True">
            <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
            <Columns>
                <asp:CommandField ShowSelectButton="True" ButtonType="Button"></asp:CommandField>
                <asp:BoundField DataField="id_venta" HeaderText="ID" ReadOnly="True" SortExpression="id_venta"></asp:BoundField>
                <asp:BoundField DataField="codigo_de_barras" HeaderText="Código De Barras" SortExpression="codigo_de_barras"></asp:BoundField>
                <asp:BoundField DataField="numero_de_lote" HeaderText="Número De Lote" SortExpression="numero_de_lote"></asp:BoundField>
                <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre"></asp:BoundField>
                <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="costo_venta" HeaderText="Costo De Venta" SortExpression="costo_venta" DataFormatString="{0:C2}" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="precio_venta_total" HeaderText="Total De Venta" SortExpression="precio_venta_total" DataFormatString="{0:C2}" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="ganancia_total" HeaderText="Ganancia Total" SortExpression="ganancia_total" DataFormatString="{0:C2}" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="fecha_de_salida" HeaderText="Fecha De Venta" SortExpression="fecha_de_salida" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundField>
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
    <asp:SqlDataSource runat="server" ID="SqlDataSourceVentas" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_venta, codigo_de_barras, numero_de_lote, nombre, cantidad, precio_venta_total, costo_venta, ganancia_total, fecha_de_salida, realizado_por FROM ViewVenta ORDER BY id_venta DESC"></asp:SqlDataSource>
</asp:Content>
