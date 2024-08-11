<%@ Page Title="Medicamentos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="medicamentos.aspx.cs" Inherits="POCKBIT_v2.Paginas.medicamentos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="text-center mb-4">
        <h2>Medicamentos</h2>
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
                        <label class="form-control-label">Nombre comercial:</label>
                        <asp:TextBox ID="txtNombreC" runat="server" Placeholder="Nombre Comercial" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Descripción:</label>
                        <asp:TextBox ID="txtDescripcion" runat="server" Placeholder="Descripción" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Laboratorio:</label>
                        <div class="input-group input-group-merge">
                            <asp:DropDownList ID="ddlLaboratorio" runat="server" CssClass="form-select" DataSourceID="SqlDataSourceDdlLaboratorios" DataTextField="nombre" DataValueField="id_laboratorio"></asp:DropDownList>
                        </div>
                        <asp:SqlDataSource runat="server" ID="SqlDataSourceDdlLaboratorios" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_laboratorio, nombre FROM laboratorio WHERE (activo = 1)"></asp:SqlDataSource>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Costo:</label>
                        <div class="input-group input-group-merge">
                            <span class="input-group-text">$</span>
                            <asp:TextBox ID="txtCosto" runat="server" Placeholder="Costo" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Precio Máximo al Público:</label>
                        <div class="input-group input-group-merge">
                            <span class="input-group-text">$</span>
                            <asp:TextBox ID="txtPrecioP" runat="server" Placeholder="Precio Máx Público" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Precio de venta:</label>
                        <div class="input-group input-group-merge">
                            <span class="input-group-text">$</span>
                            <asp:TextBox ID="txtPrecioV" runat="server" Placeholder="Precio de Venta" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Código de Barras:</label>
                        <asp:TextBox ID="txtCodigoB" runat="server" Placeholder="Código de Barras" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-control-label">Estado:</label>
                        <div class="input-group input-group-merge">
                            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Activo" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Inactivo" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
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
        <asp:GridView ID="GVMedicamentos" runat="server" CssClass="table custom-table" AutoGenerateColumns="False" DataKeyNames="id_medicamento" DataSourceID="SqlDataSourceViewMedicamentos" AllowSorting="True" CellPadding="4" ForeColor="#333333" GridLines="None" OnSelectedIndexChanged="GVMedicamentos_SelectedIndexChanged1" OnRowDataBound="GVMedicamentos_RowDataBound" AllowPaging="True">
            <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
            <Columns>
                <asp:CommandField ShowSelectButton="True" ButtonType="Button"></asp:CommandField>
                <asp:BoundField DataField="id_medicamento" HeaderText="ID" ReadOnly="True" SortExpression="id_medicamento"></asp:BoundField>
                <asp:BoundField DataField="codigo_de_barras" HeaderText="Código De Barras" SortExpression="codigo_de_barras"></asp:BoundField>
                <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre"></asp:BoundField>
                <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion"></asp:BoundField>
                <asp:BoundField DataField="nombre_laboratorio" HeaderText="Nombre Laboratorio" SortExpression="nombre_laboratorio"></asp:BoundField>
                <asp:BoundField DataField="costo" HeaderText="Costo" SortExpression="costo" DataFormatString="{0:C2}" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="precio_venta" HeaderText="Precio Venta" SortExpression="precio_venta" DataFormatString="{0:C2}" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="precio_maximo_publico" HeaderText="Precio Máximo Público" SortExpression="precio_maximo_publico" DataFormatString="{0:C2}" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="cantidad_total" HeaderText="Cantidad Total" SortExpression="cantidad_total" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="fecha_de_registro" HeaderText="Fecha De Registro" SortExpression="fecha_de_registro" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundField>
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

    <asp:SqlDataSource runat="server" ID="SqlDataSourceViewMedicamentos" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_medicamento, codigo_de_barras, nombre, descripcion, nombre_laboratorio, costo, precio_venta, precio_maximo_publico, cantidad_total, fecha_de_registro, activo FROM ViewMedicamento ORDER BY id_medicamento DESC"></asp:SqlDataSource>
</asp:Content>
