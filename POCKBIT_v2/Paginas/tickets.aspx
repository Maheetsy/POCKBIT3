<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="tickets.aspx.cs" Inherits="POCKBIT_v2.Paginas.tickets" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .table-responsive {
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
}

.rounded-3 {
    border-radius: 0.5rem !important;
}

.custom-table {
    width: 100%;
    margin-bottom: 1rem;
    color: #212529;
    border-collapse: collapse;
}

.custom-table th,
.custom-table td {
    padding: 0.75rem;
    vertical-align: top;
    border-top: 1px solid #dee2e6;
}

.custom-table thead th {
    vertical-align: bottom;
    border-bottom: 2px solid #dee2e6;
}

.custom-table tbody + tbody {
    border-top: 2px solid #dee2e6;
}

.custom-table .table {
    background-color: #fff;
}

/* Estilo para el paginado */
.custom-table > .pagination {
    margin-top: 0.5rem;
}

/* Estilo para el botón de impresión */
.btn-sm {
    padding: 0.25rem 0.5rem;
    font-size: 0.875rem;
    line-height: 1.5;
    border-radius: 0.2rem;
}
        .ticket {
            width: 80mm;
            font-family: 'Courier New', monospace;
            font-size: 12px;
            margin: 0 auto;
            padding: 5px;
            border: 1px dashed #ccc;
            background: white;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            z-index: 1000;
            display: none; /* Oculto por defecto */
        }
        .ticket-visible {
            display: block !important;
        }
        .ticket-header, .ticket-footer {
            text-align: center;
            margin: 5px 0;
        }
        .ticket hr {
            border-top: 1px dashed #000;
            margin: 5px 0;
        }
        .overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
            z-index: 999;
            display: none;
        }
        @media print {
            body * {
                visibility: hidden;
            }
            .ticket, .ticket * {
                visibility: visible;
            }
            .ticket {
                position: absolute;
                left: 0;
                top: 0;
                width: 80mm;
                border: none;
                margin: 0;
                padding: 0;
            }
            .overlay {
                display: none !important;
            }
        }
    </style>
    
    <h3>Tickets</h3>

    <!-- Tabla de tickets -->
    <div id="divTablaTickets" runat="server" class="table-responsive rounded-3">
    <asp:GridView ID="GVTickets" runat="server" CssClass="table custom-table" 
        AutoGenerateColumns="False" DataKeyNames="id_venta" 
        OnRowCommand="GVTickets_RowCommand" CellPadding="4" ForeColor="#333333" 
        GridLines="None" AllowPaging="True" PageSize="10">
        
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="id_venta" HeaderText="ID VENTA" SortExpression="id_venta">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            
            <asp:BoundField DataField="fecha_de_salida" HeaderText="FECHA" SortExpression="fecha_de_salida" 
                DataFormatString="{0:dd/MM/yyyy HH:mm}" HtmlEncode="false" />
                
            <asp:BoundField DataField="medicamento" HeaderText="MEDICAMENTO" SortExpression="medicamento" />
            
            <asp:BoundField DataField="cantidad" HeaderText="CANTIDAD" SortExpression="cantidad">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            
            <asp:BoundField DataField="precio_venta_total" HeaderText="TOTAL" SortExpression="precio_venta_total" 
                DataFormatString="${0:N2}" HtmlEncode="false">
                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>
            
            <asp:TemplateField HeaderText="ACCIÓN" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Button ID="btnImprimir" runat="server" CommandName="ImprimirTicket" 
                        CommandArgument='<%# Container.DataItemIndex %>' Text="🖨️ Imprimir"
                        CssClass="btn btn-primary btn-sm" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        
        <EditRowStyle BackColor="#2461BF" />
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#03c3ec" Font-Bold="True" ForeColor="White" />
        <PagerStyle HorizontalAlign="Center" BackColor="#2461BF" ForeColor="White" />
        <RowStyle BackColor="#EFF3FB" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#F5F7FB" />
        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
        <SortedDescendingCellStyle BackColor="#E9EBEF" />
        <SortedDescendingHeaderStyle BackColor="#4870BE" />
    </asp:GridView>
</div>
    <!-- Overlay para el fondo oscuro -->
    <div id="overlay" class="overlay"></div>
    
    <!-- Contenedor del ticket (oculto inicialmente) -->
    <div id="ticketContainer" class="ticket"></div>

    <script>
        function mostrarTicket(html) {
            try {
                var ticketContainer = document.getElementById('ticketContainer');
                var overlay = document.getElementById('overlay');

                ticketContainer.innerHTML = html;
                overlay.style.display = 'block';
                ticketContainer.style.display = 'block';

                // Configurar botones
                var btnImprimir = ticketContainer.querySelector('.btn-imprimir');
                if (btnImprimir) {
                    btnImprimir.onclick = function () {
                        var printContent = document.querySelector('.ticket-content').outerHTML;
                        var ventanaImpresion = window.open('', '_blank');
                        ventanaImpresion.document.open();
                        ventanaImpresion.document.write(`
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>Ticket de Venta</title>
                        <style>
                            body { font-family: 'Courier New', monospace; font-size: 12px; }
                            .ticket { width: 80mm; margin: 0; padding: 5px; }
                            hr { border-top: 1px dashed #000; margin: 5px 0; }
                        </style>
                    </head>
                    <body onload="window.print();window.close()">
                        ${printContent}
                    </body>
                    </html>
                `);
                        ventanaImpresion.document.close();
                    };
                    btnImprimir.focus();
                }
            } catch (e) {
                console.error('Error al mostrar ticket:', e);
                alert('Error al mostrar el ticket. Consulte la consola para detalles.');
            }
        }

        function cerrarTicket() {
            document.getElementById('overlay').style.display = 'none';
            document.getElementById('ticketContainer').style.display = 'none';
        }
    </script>
</asp:Content>