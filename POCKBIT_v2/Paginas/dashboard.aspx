<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="POCKBIT_v2.Paginas.dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .mb-4-spacing {
            margin-bottom: 1.5rem;
        }

        .btn-center {
            display: flex;
            justify-content: center;
            flex-wrap: wrap;
            gap: 1rem;
        }

        .btn-center .btn {
            flex: 1 1 16%;
            min-width: 150px;
            max-width: 200px;
        }

        .card-row {
            display: flex;
            flex-wrap: wrap;
            gap: 1rem;
            margin-bottom: 2rem;
        }

        .card-row .card {
            flex: 1 1 calc(25% - 1rem);
            min-width: 200px;
        }

        @media (max-width: 768px) {
            .card-row .card {
                flex: 1 1 calc(50% - 1rem);
            }

            .btn-center .btn {
                flex: 1 1 calc(33.33% - 1rem);
            }
        }

        @media (max-width: 576px) {
            .card-row .card {
                flex: 1 1 100%;
            }

            .btn-center .btn {
                flex: 1 1 calc(50% - 1rem);
            }
        }
    </style>

    <div class="text-center mb-4">
        <h2>Dashboard</h2>
    </div>
    <div class="card-row">
        <div class="card">
            <div class="card-body">
                <div class="d-flex justify-content-between flex-sm-row flex-column gap-3">
                    <div class="d-flex flex-sm-column flex-row align-items-start justify-content-between">
                        <div class="card-title">
                            <h5 class="text-nowrap mb-2">Balance anual</h5>
                            <span class="badge bg-label-warning rounded-pill">Year <asp:Literal ID="lblYear" runat="server" /></span>
                        </div>
                        <div class="mt-sm-auto">
                            <asp:Label ID="lblBalance" runat="server" Text="$0.00" CssClass="text-success mb-0"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <div class="card-title d-flex align-items-start justify-content-between">
                    <div class="avatar flex-shrink-0">
                        <img src="../assets/img/icons/unicons/UilPricetagAlt.png" alt="Credit Card" class="rounded" />
                    </div>
                </div>
                <span class="fw-semibold d-block mb-1">Ventas de este mes</span>
                <asp:Label ID="lblVentasMes" runat="server" Text="$0.00" CssClass="card-title text-nowrap mb-2"></asp:Label>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <div class="card-title d-flex align-items-start justify-content-between">
                    <div class="avatar flex-shrink-0">
                        <img src="../assets/img/icons/unicons/UilShoppingCart.png" alt="Credit Card" class="rounded" />
                    </div>
                </div>
                <span class="fw-semibold d-block mb-1">Compras este mes</span>
                <asp:Label ID="lblComprasMes" runat="server" Text="$0.00" CssClass="card-title mb-2"></asp:Label>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <div class="card-title d-flex align-items-start justify-content-between">
                    <div class="avatar flex-shrink-0">
                        <img src="../assets/img/icons/unicons/UilBox.png" alt="Credit Card" class="rounded" />
                    </div>
                </div>
                <span class="fw-semibold d-block mb-1">Inventario</span>
                <asp:Label ID="lblInventario" runat="server" Text="0"></asp:Label>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-12">
            <div class="card mb-3">
                <h5 class="card-header">Descargar Reportes</h5>
                <div class="card-body">
                    <small class="text-light fw-semibold">Ventas</small>
                    <div class="row mb-3 btn-center">
                        <div class="col-md-3 mb-3">
                            <asp:Button ID="btnVentasSemana" runat="server" Text="Semana" CssClass="btn btn-info" OnClick="btnVentasSemana_Click" />
                        </div>
                        <div class="col-md-3 mb-3">
                            <asp:Button ID="btnVentasMes" runat="server" Text="Mes" CssClass="btn btn-info" OnClick="btnVentasMes_Click" />
                        </div>
                        <div class="col-md-3 mb-3">
                            <asp:Button ID="btnVentasAño" runat="server" Text="Año" CssClass="btn btn-info" OnClick="btnVentasAño_Click" />
                        </div>
                        <div class="col-md-3 mb-3">
                            <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalMedicamento">
                                Medicamento
                            </button>
                        </div>
                        <div class="modal fade" id="modalMedicamento" tabindex="-1" aria-hidden="true">
                            <div class="modal-dialog modal-dialog-centered" role="document">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title" id="modalMedicamentoLabel">Reporte de ventas por medicamento</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div class="modal-body">
                                        <div class="row">
                                            <div class="col mb-3">
                                                <label class="form-control-label">Nombre del medicamento:</label>
                                                <asp:DropDownList ID="DropDownListMedicamento" runat="server" DataSourceID="SqlDataSourceMedicamento" DataTextField="nombre" DataValueField="id_medicamento" CssClass="form-select"></asp:DropDownList>
                                                <asp:SqlDataSource runat="server" ID="SqlDataSourceMedicamento" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_medicamento, nombre FROM medicamento WHERE activo = 1"></asp:SqlDataSource>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-md-12">
                                                <label class="form-control-label">Rango de fechas:</label>
                                                <div class="d-flex">
                                                    <input type="date" id="txtFechaInicioMedicamento" runat="server" class="form-control me-2" />
                                                    <span class="align-self-center mx-2">a</span>
                                                    <input type="date" id="txtFechaFinMedicamento" runat="server" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Close</button>
                                        <asp:Button ID="btnReporteVentasMedicamento" runat="server" Text="Descargar" CssClass="btn btn-info" OnClick="btnReporteVentasMedicamento_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3 mb-3">
                            <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalLaboratorio">
                                Laboratorio
                            </button>
                        </div>
                        <div class="modal fade" id="modalLaboratorio" tabindex="-1" aria-hidden="true">
                            <div class="modal-dialog modal-dialog-centered" role="document">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title" id="modalLaboratorioLabel">Reporte de ventas por laboratorio</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div class="modal-body">
                                        <div class="row">
                                            <div class="col mb-3">
                                                <label class="form-control-label">Nombre del laboratorio:</label>
                                                <asp:DropDownList ID="DropDownListLaboratorio" runat="server" DataSourceID="SqlDataSourceLaboratorio" DataTextField="nombre" DataValueField="id_laboratorio" CssClass="form-select"></asp:DropDownList>
                                                <asp:SqlDataSource runat="server" ID="SqlDataSourceLaboratorio" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_laboratorio, nombre FROM laboratorio WHERE activo = 1"></asp:SqlDataSource>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-md-12">
                                                <label class="form-control-label">Rango de fechas:</label>
                                                <div class="d-flex">
                                                    <input type="date" id="txtFechaInicioLaboratorio" runat="server" class="form-control me-2" />
                                                    <span class="align-self-center mx-2">a</span>
                                                    <input type="date" id="txtFechaFinLaboratorio" runat="server" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Close</button>
                                        <asp:Button ID="btnReporteVentasLaboratorio" runat="server" Text="Descargar" CssClass="btn btn-info" OnClick="btnReporteVentasLaboratorio_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3 mb-3">
                            <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalFechas">
                                Entre Fechas
                            </button>
                        </div>
                        <div class="modal fade" id="modalFechas" tabindex="-1" aria-hidden="true">
                            <div class="modal-dialog modal-dialog-centered" role="document">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title" id="modalFechasLabel">Reporte de ventas entre fechas</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div class="modal-body">
                                        <div class="row mb-3">
                                            <div class="col-md-12">
                                                <label class="form-control-label">Rango de fechas:</label>
                                                <div class="d-flex">
                                                    <input type="date" id="txtFechaInicioFechas" runat="server" class="form-control me-2" />
                                                    <span class="align-self-center mx-2">a</span>
                                                    <input type="date" id="txtFechaFinFechas" runat="server" class="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Close</button>
                                        <asp:Button ID="btnReporteVentasFechas" runat="server" Text="Descargar" CssClass="btn btn-info" OnClick="btnReporteVentasFechas_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <hr class="m-0" />
                    <div class="card-body">
                        <small class="text-light fw-semibold">Compras</small>
                        <div class="row mb-3 btn-center">
                            <div class="col-md-3 mb-3">
                                <asp:Button ID="btnComprasSemana" runat="server" Text="Semana" CssClass="btn btn-info" OnClick="btnComprasSemana_Click" />
                            </div>
                            <div class="col-md-3 mb-3">
                                <asp:Button ID="btnComprasMes" runat="server" Text="Mes" CssClass="btn btn-info" OnClick="btnComprasMes_Click" />
                            </div>
                            <div class="col-md-3 mb-3">
                                <asp:Button ID="btnComprasAño" runat="server" Text="Año" CssClass="btn btn-info" OnClick="btnComprasAño_Click" />
                            </div>
                            <div class="col-md-3 mb-3">
                                <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalMedicamentoCompra">
                                    Medicamento
                                </button>
                            </div>
                            <div class="modal fade" id="modalMedicamentoCompra" tabindex="-1" aria-hidden="true">
                                <div class="modal-dialog modal-dialog-centered" role="document">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title" id="modalMedicamentoCompraLabel">Reporte de compras por medicamento</h5>
                                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                        </div>
                                        <div class="modal-body">
                                            <div class="row">
                                                <div class="col mb-3">
                                                    <label class="form-control-label">Nombre del medicamento:</label>
                                                    <asp:DropDownList ID="DropDownListMedicamentoCompra" runat="server" DataSourceID="SqlDataSource1" DataTextField="nombre" DataValueField="id_medicamento" CssClass="form-select"></asp:DropDownList>
                                                    <asp:SqlDataSource runat="server" ID="SqlDataSource1" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_medicamento, nombre FROM medicamento WHERE activo = 1"></asp:SqlDataSource>
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col-md-12">
                                                    <label class="form-control-label">Rango de fechas:</label>
                                                    <div class="d-flex">
                                                        <input type="date" id="txtFechaInicioMedicamentoCompra" runat="server" class="form-control me-2" />
                                                        <span class="align-self-center mx-2">a</span>
                                                        <input type="date" id="txtFechaFinMedicamentoCompra" runat="server" class="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Close</button>
                                            <asp:Button ID="btnReporteComprasMedicamento" runat="server" Text="Descargar" CssClass="btn btn-info" OnClick="btnReporteComprasMedicamento_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3 mb-3">
                                <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalLaboratorioCompra">
                                    Laboratorio
                                </button>
                            </div>
                            <div class="modal fade" id="modalLaboratorioCompra" tabindex="-1" aria-hidden="true">
                                <div class="modal-dialog modal-dialog-centered" role="document">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title" id="modalLaboratorioCompraLabel">Reporte de compras por laboratorio</h5>
                                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                        </div>
                                        <div class="modal-body">
                                            <div class="row">
                                                <div class="col mb-3">
                                                    <label class="form-control-label">Nombre del laboratorio:</label>
                                                    <asp:DropDownList ID="DropDownListLaboratorioCompra" runat="server" DataSourceID="SqlDataSource2" DataTextField="nombre" DataValueField="id_laboratorio" CssClass="form-select"></asp:DropDownList>
                                                    <asp:SqlDataSource runat="server" ID="SqlDataSource2" ConnectionString='<%$ ConnectionStrings:DefaultConnection %>' SelectCommand="SELECT id_laboratorio, nombre FROM laboratorio WHERE activo = 1"></asp:SqlDataSource>
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <div class="col-md-12">
                                                    <label class="form-control-label">Rango de fechas:</label>
                                                    <div class="d-flex">
                                                        <input type="date" id="txtFechaInicioLaboratorioCompra" runat="server" class="form-control me-2" />
                                                        <span class="align-self-center mx-2">a</span>
                                                        <input type="date" id="txtFechaFinLaboratorioCompra" runat="server" class="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Close</button>
                                            <asp:Button ID="btnReporteComprasLaboratorio" runat="server" Text="Descargar" CssClass="btn btn-info" OnClick="btnReporteComprasLaboratorio_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3 mb-3">
                                <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalFechasCompra">
                                    Entre Fechas
                                </button>
                            </div>
                            <div class="modal fade" id="modalFechasCompra" tabindex="-1" aria-hidden="true">
                                <div class="modal-dialog modal-dialog-centered" role="document">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title" id="modalFechasCompraLabel">Reporte de compras entre fechas</h5>
                                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                        </div>
                                        <div class="modal-body">
                                            <div class="row mb-3">
                                                <div class="col-md-12">
                                                    <label class="form-control-label">Rango de fechas:</label>
                                                    <div class="d-flex">
                                                        <input type="date" id="txtFechaInicioFechasCompra" runat="server" class="form-control me-2" />
                                                        <span class="align-self-center mx-2">a</span>
                                                        <input type="date" id="txtFechaFinFechasCompra" runat="server" class="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Close</button>
                                            <asp:Button ID="btnReporteComprasFechas" runat="server" Text="Descargar" CssClass="btn btn-info" OnClick="btnReporteComprasFechas_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
