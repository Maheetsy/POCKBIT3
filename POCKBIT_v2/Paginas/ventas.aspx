<%@ Page Title="Ventas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ventas.aspx.cs" Inherits="POCKBIT_v2.Paginas.ventas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="text-center mb-4">
        <h2>Ventas De Medicamentos</h2>
    </div>

    <asp:Literal ID="ltlAlert" runat="server"></asp:Literal>

   <div class="card-row">
    <div class="card">
        <div class="card-body p-3">
            <div class="row mb-4">
        <!-- COL 9: FORMULARIO -->
        <div class="col-md-9">
            <div class="row mb-3">
                <div class="col-md-4">
                    <label>ID:</label>
                    <asp:Label ID="lblId" runat="server" CssClass="form-control"></asp:Label>
                </div>
                <div class="col-md-4">
                    <label>Código de Barras:</label>
                    <asp:TextBox ID="txtCodigoBarras" runat="server" CssClass="form-control"
                        AutoPostBack="true" OnTextChanged="txtCodigoBarras_TextChanged"
                        placeholder="Escanea o escribe el código">
                    </asp:TextBox>
                    <asp:HiddenField ID="hiddenIdMedicamento" runat="server" />
                </div>
                <div class="col-md-4 d-flex align-items-end">
                    <button type="button" onclick="iniciarEscaneo()" class="btn btn-secondary w-80">
                        📷 Escanear
                    </button>
                </div>
            </div>
            <div class="row mb-3">
                <div class="col-md-4">
                    <label>Seleccionar Lote:</label>
                    <asp:DropDownList ID="ddlLote" runat="server" CssClass="form-select"
                        DataSourceID="SqlDataSourceLotes" DataTextField="numero_de_lote" DataValueField="id_lote">
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDataSourceLotes" runat="server" 
                        ConnectionString='<%$ ConnectionStrings:DefaultConnection %>'
                        SelectCommand="SELECT id_lote, numero_de_lote FROM lote WHERE (id_medicamento = @id_medicamento) AND (activo = 1) AND (cantidad > 0) ORDER BY id_lote DESC">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="hiddenIdMedicamento" PropertyName="Value" Name="id_medicamento" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                </div>
                <div class="col-md-4">
                    <label>Cantidad Vendida:</label>
                    <asp:TextBox ID="txtCantidadV" runat="server" CssClass="form-control" Placeholder="Números enteros"></asp:TextBox>
                </div>
              <div class="col-md-4">
                <label class="form-label">Cliente:</label>
                <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select"
                    DataSourceID="SqlDataSourceClientes" DataTextField="nombre" DataValueField="id_cliente" AppendDataBoundItems="true">
                    <asp:ListItem Value="" Text="Público en general" Selected="True"></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSourceClientes" runat="server" 
                    ConnectionString='<%$ ConnectionStrings:DefaultConnection %>'
                    SelectCommand="SELECT id_cliente, nombre FROM cliente WHERE (activo = 1) ORDER BY nombre">
                </asp:SqlDataSource>
            </div>
            </div>
        </div>

        <!-- COL 3: ESCÁNER OCUPANDO LAS DOS FILAS -->
        <div class="col-md-3 d-flex flex-column justify-content-between">
            <label>Vista Escáner:</label>
            <div id="reader" style="width:100%; height:100%; min-height:250px; border:1px solid #ccc; border-radius:6px;"></div>
        </div>
    </div>

    <!-- FILA DE BOTONES -->
    <div class="row mb-3 text-center">
        <div class="col-md-3">
            <asp:Button ID="btnInsertar" runat="server" Text="Insertar" CssClass="btn btn-success w-100" OnClick="btnInsertar_Click" />
        </div>
        <div class="col-md-3">
            <asp:Button ID="btnModificar" runat="server" Text="Modificar" CssClass="btn btn-info w-100" OnClick="btnModificar_Click" />
        </div>
        <div class="col-md-3">
            <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" CssClass="btn btn-danger w-100" OnClick="btnEliminar_Click" />
        </div>
        <div class="col-md-3">
            <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar a Excel" CssClass="btn btn-primary w-100" OnClick="btnExportarExcel_Click" />
        </div>
    </div>

        </div>
    </div>
</div>


    <br />
    <div class="table-responsive rounded-3">
       <asp:GridView ID="GVVentas" runat="server" CssClass="table custom-table" DataSourceID="SqlDataSourceVentas" AutoGenerateColumns="False" DataKeyNames="id_venta" AllowSorting="True"  CellPadding="4" ForeColor="#333333" GridLines="None" OnSelectedIndexChanged="GVVentas_SelectedIndexChanged" OnRowDataBound="GVVentas_RowDataBound" AllowPaging="True">
    <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
    <Columns>
        <asp:CommandField ShowSelectButton="True" ButtonType="Button"></asp:CommandField>
        <asp:BoundField DataField="id_venta" HeaderText="ID" SortExpression="id_venta"/>
        <asp:BoundField DataField="codigo_de_barras" HeaderText="Código" SortExpression="codigo_de_barras"/>
        <asp:BoundField DataField="numero_de_lote" HeaderText="Lote" SortExpression="numero_de_lote"/>
        <asp:BoundField DataField="nombre" HeaderText="Producto" SortExpression="nombre"/>
        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" SortExpression="cantidad"/>
        <asp:BoundField DataField="precio_venta_total" HeaderText="Precio Total" DataFormatString="{0:C}" SortExpression="precio_venta_total"/>
        <asp:BoundField DataField="costo_venta" HeaderText="Costo" DataFormatString="{0:C}" SortExpression="costo_venta"/>
        <asp:BoundField DataField="ganancia_total" HeaderText="Ganancia" DataFormatString="{0:C}" SortExpression="ganancia_total"/>
        <asp:BoundField DataField="fecha_de_salida" HeaderText="Fecha" DataFormatString="{0:g}" SortExpression="fecha_de_salida"/>
        <asp:BoundField DataField="realizado_por" HeaderText="Realizado por" SortExpression="realizado_por"/>
        <asp:BoundField DataField="nombre_cliente" HeaderText="Cliente" SortExpression="nombre_cliente"/>
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
    <asp:SqlDataSource ID="SqlDataSourceVentas" runat="server" 
    ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
    SelectCommand="SELECT v.id_venta, m.codigo_de_barras, l.numero_de_lote, m.nombre, 
                  v.cantidad, v.precio_venta_total, v.costo_venta, v.ganancia_total, 
                  v.fecha_de_salida, v.realizado_por, 
                  ISNULL(c.nombre, 'Público en general') AS nombre_cliente
                  FROM venta v
                  INNER JOIN lote l ON v.id_lote = l.id_lote
                  INNER JOIN medicamento m ON l.id_medicamento = m.id_medicamento
                  LEFT OUTER JOIN cliente c ON v.id_cliente = c.id_cliente
                  ORDER BY v.id_venta DESC">
</asp:SqlDataSource>
<script src="https://unpkg.com/html5-qrcode" type="text/javascript"></script>
<script>
    function iniciarEscaneo() {
        const html5QrCode = new Html5Qrcode("reader");
        const qrConfig = { fps: 10, qrbox: 250 };

        html5QrCode.start(
            { facingMode: "environment" },
            qrConfig,
            (decodedText) => {
                const input = document.getElementById('<%= txtCodigoBarras.ClientID %>');
                input.value = decodedText;

                html5QrCode.stop().then(() => {
                    document.getElementById("reader").innerHTML = "";
                });

                // Forzar blur y luego __doPostBack
                input.focus();
                setTimeout(() => {
                    input.blur(); // esto activa el TextChanged si es manual
                    __doPostBack('<%= txtCodigoBarras.UniqueID %>', '');
                }, 300);
            },
            (errorMessage) => {
                // Puedes ignorar o mostrar error del escáner
            }
        ).catch(err => {
            console.error("Error al acceder a la cámara", err);
        });
    }
</script>


</asp:Content>
