using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using ClosedXML.Excel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POCKBIT_v2.Paginas
{
    public partial class dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TwoFactorVerified"] == null || !(bool)Session["TwoFactorVerified"])
            {
                Response.Redirect("~/Account/Login");
            }
            if (!IsPostBack)
            {
                lblYear.Text = DateTime.Now.Year.ToString();
                CargarDatosDashboard();
            }
        }

        private void CargarDatosDashboard()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    decimal ventasMesActual = CargarVentasMes(con);
                    decimal comprasMesActual = CargarComprasMes(con);
                    decimal inventarioActual = CargarInventario(con);
                    CalcularBalance(con, ventasMesActual, comprasMesActual);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private decimal CargarVentasMes(SqlConnection con)
        {
            decimal ventasMesActual = 0;
            decimal ventasMesAnterior = 0;

            using (SqlCommand cmdActual = new SqlCommand("sp_ObtenerVentasMesActual", con))
            {
                cmdActual.CommandType = CommandType.StoredProcedure;
                ventasMesActual = Convert.ToDecimal(cmdActual.ExecuteScalar());
            }

            using (SqlCommand cmdAnterior = new SqlCommand("sp_ObtenerVentasMesAnterior", con))
            {
                cmdAnterior.CommandType = CommandType.StoredProcedure;
                ventasMesAnterior = Convert.ToDecimal(cmdAnterior.ExecuteScalar());
            }

            lblVentasMes.Text = $"${ventasMesActual:N2}";
            decimal cambioVentasMes = ventasMesAnterior == 0 ? 0 : ((ventasMesActual - ventasMesAnterior) / ventasMesAnterior) * 100;
            ActualizarEtiquetaPorcentaje(lblVentasMes, ventasMesActual, cambioVentasMes);

            return ventasMesActual;
        }

        private decimal CargarComprasMes(SqlConnection con)
        {
            decimal comprasMesActual = 0;
            decimal comprasMesAnterior = 0;

            using (SqlCommand cmdActual = new SqlCommand("sp_ObtenerComprasMesActual", con))
            {
                cmdActual.CommandType = CommandType.StoredProcedure;
                comprasMesActual = Convert.ToDecimal(cmdActual.ExecuteScalar());
            }

            using (SqlCommand cmdAnterior = new SqlCommand("sp_ObtenerComprasMesAnterior", con))
            {
                cmdAnterior.CommandType = CommandType.StoredProcedure;
                comprasMesAnterior = Convert.ToDecimal(cmdAnterior.ExecuteScalar());
            }

            lblComprasMes.Text = $"${comprasMesActual:N2}";
            decimal cambioComprasMes = comprasMesAnterior == 0 ? 0 : ((comprasMesActual - comprasMesAnterior) / comprasMesAnterior) * 100;
            ActualizarEtiquetaPorcentaje(lblComprasMes, comprasMesActual, cambioComprasMes);

            return comprasMesActual;
        }

        private decimal CargarInventario(SqlConnection con)
        {
            decimal inventarioActual = 0;

            using (SqlCommand cmd = new SqlCommand("sp_ObtenerInventarioActual", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                inventarioActual = Convert.ToDecimal(cmd.ExecuteScalar());
                lblInventario.Text = $"${inventarioActual:N2}";
            }

            return inventarioActual;
        }

        private void CalcularBalance(SqlConnection con, decimal ventasMesActual, decimal comprasMesActual)
        {
            decimal balanceActual = ventasMesActual - comprasMesActual;
            decimal balanceAnterior = 0;

            using (SqlCommand cmd = new SqlCommand("sp_ObtenerBalanceAnterior", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                balanceAnterior = Convert.ToDecimal(cmd.ExecuteScalar());
            }

            decimal cambioBalance = balanceAnterior == 0 ? 0 : ((balanceActual - balanceAnterior) / balanceAnterior) * 100;
            ActualizarEtiquetaPorcentaje(lblBalance, balanceActual, cambioBalance);
        }

        private void ActualizarEtiquetaPorcentaje(Label label, decimal valorActual, decimal porcentajeCambio)
        {
            bool esIncremento = porcentajeCambio >= 0;
            string claseColor = esIncremento ? "text-success" : "text-danger";
            string icono = esIncremento ? "bx-up-arrow-alt" : "bx-down-arrow-alt";
            label.CssClass = "card-title text-nowrap mb-2";
            label.Text = $"<span class='card-title text-nowrap mb-2'>${valorActual:N2}</span>";
       
            label.Text += $" <small class='{claseColor} fw-semibold'><i class='bx {icono}'></i>{Math.Abs(porcentajeCambio):N2}%</small>";
        }



        private void LogError(Exception ex)
        {
            string logFilePath = Server.MapPath("~/App_Data/ErrorLog.txt");
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine("Error Message : " + ex.Message);
                    writer.WriteLine("Stack Trace : " + ex.StackTrace);
                    writer.WriteLine();
                }
            }
            catch
            {
            }
        }
        // Eventos para los botones de ventas
        protected void btnVentasSemana_Click(object sender, EventArgs e)
        {
            DescargarReporteVentas("SEMANA");
        }

        protected void btnVentasMes_Click(object sender, EventArgs e)
        {
            DescargarReporteVentas("MES");
        }

        protected void btnVentasAño_Click(object sender, EventArgs e)
        {
            DescargarReporteVentas("AÑO");
        }

        protected void btnReporteVentasMedicamento_Click(object sender, EventArgs e)
        {
            int id_medicamento = int.Parse(DropDownListMedicamento.SelectedValue);
            DateTime fechaInicio = DateTime.Parse(txtFechaInicioMedicamento.Value);
            DateTime fechaFin = DateTime.Parse(txtFechaFinMedicamento.Value);

            DescargarReporteVentasMedicamento(id_medicamento, fechaInicio, fechaFin);
        }

        protected void btnReporteVentasLaboratorio_Click(object sender, EventArgs e)
        {
            int id_laboratorio = int.Parse(DropDownListLaboratorio.SelectedValue);
            DateTime fechaInicio = DateTime.Parse(txtFechaInicioLaboratorio.Value);
            DateTime fechaFin = DateTime.Parse(txtFechaFinLaboratorio.Value);

            DescargarReporteVentasLaboratorio(id_laboratorio, fechaInicio, fechaFin);
        }

        protected void btnReporteVentasFechas_Click(object sender, EventArgs e)
        {
            DateTime fechaInicio = DateTime.Parse(txtFechaInicioFechas.Value);
            DateTime fechaFin = DateTime.Parse(txtFechaFinFechas.Value);

            DescargarReporteVentasFechas(fechaInicio, fechaFin);
        }

        // Eventos para los botones de compras
        protected void btnComprasSemana_Click(object sender, EventArgs e)
        {
            DescargarReporteCompras("SEMANA");
        }

        protected void btnComprasMes_Click(object sender, EventArgs e)
        {
            DescargarReporteCompras("MES");
        }

        protected void btnComprasAño_Click(object sender, EventArgs e)
        {
            DescargarReporteCompras("AÑO");
        }

        protected void btnReporteComprasMedicamento_Click(object sender, EventArgs e)
        {
            int id_medicamento = int.Parse(DropDownListMedicamentoCompra.SelectedValue);
            DateTime fechaInicio = DateTime.Parse(txtFechaInicioMedicamentoCompra.Value);
            DateTime fechaFin = DateTime.Parse(txtFechaFinMedicamentoCompra.Value);

            DescargarReporteComprasMedicamento(id_medicamento, fechaInicio, fechaFin);
        }

        protected void btnReporteComprasLaboratorio_Click(object sender, EventArgs e)
        {
            int id_laboratorio = int.Parse(DropDownListLaboratorioCompra.SelectedValue);
            DateTime fechaInicio = DateTime.Parse(txtFechaInicioLaboratorioCompra.Value);
            DateTime fechaFin = DateTime.Parse(txtFechaFinLaboratorioCompra.Value);

            DescargarReporteComprasLaboratorio(id_laboratorio, fechaInicio, fechaFin);
        }

        protected void btnReporteComprasFechas_Click(object sender, EventArgs e)
        {
            DateTime fechaInicio = DateTime.Parse(txtFechaInicioFechasCompra.Value);
            DateTime fechaFin = DateTime.Parse(txtFechaFinFechasCompra.Value);

            DescargarReporteComprasFechas(fechaInicio, fechaFin);
        }

        // Métodos comunes para descargar reportes
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void DescargarReporteVentas(string periodo)
        {
            DataTable dt = ObtenerDatosVentas(periodo);
            string nombreArchivo = GenerarNombreArchivo("Ventas", periodo);
            DescargarExcel(dt, nombreArchivo);
        }

        private void DescargarReporteCompras(string periodo)
        {
            DataTable dt = ObtenerDatosCompras(periodo);
            string nombreArchivo = GenerarNombreArchivo("Compras", periodo);
            DescargarExcel(dt, nombreArchivo);
        }

        private void DescargarReporteVentasMedicamento(int id_medicamento, DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = ObtenerDatosVentasMedicamento(id_medicamento, fechaInicio, fechaFin);
            string nombreArchivo = $"Ventas_Medicamento_{id_medicamento}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";
            DescargarExcel(dt, nombreArchivo);
        }

        private void DescargarReporteVentasLaboratorio(int id_laboratorio, DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = ObtenerDatosVentasLaboratorio(id_laboratorio, fechaInicio, fechaFin);
            string nombreArchivo = $"Ventas_Laboratorio_{id_laboratorio}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";
            DescargarExcel(dt, nombreArchivo);
        }

        private void DescargarReporteVentasFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = ObtenerDatosVentasFechas(fechaInicio, fechaFin);
            string nombreArchivo = $"Ventas_EntreFechas_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";
            DescargarExcel(dt, nombreArchivo);
        }

        private void DescargarReporteComprasMedicamento(int id_medicamento, DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = ObtenerDatosComprasMedicamento(id_medicamento, fechaInicio, fechaFin);
            string nombreArchivo = $"Compras_Medicamento_{id_medicamento}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";
            DescargarExcel(dt, nombreArchivo);
        }

        private void DescargarReporteComprasLaboratorio(int id_laboratorio, DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = ObtenerDatosComprasLaboratorio(id_laboratorio, fechaInicio, fechaFin);
            string nombreArchivo = $"Compras_Laboratorio_{id_laboratorio}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";
            DescargarExcel(dt, nombreArchivo);
        }

        private void DescargarReporteComprasFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = ObtenerDatosComprasFechas(fechaInicio, fechaFin);
            string nombreArchivo = $"Compras_EntreFechas_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";
            DescargarExcel(dt, nombreArchivo);
        }

        // Métodos para obtener datos
        private DataTable ObtenerDatosVentas(string periodo)
        {
            DataTable dt = new DataTable();
            string connectionString = GetConnectionString();
            string query = GenerarConsulta("ViewVentaReporte", "fecha_de_salida", periodo);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private DataTable ObtenerDatosCompras(string periodo)
        {
            DataTable dt = new DataTable();
            string connectionString = GetConnectionString();
            string query = GenerarConsulta("ViewCompraReporte", "fecha_de_entrada", periodo);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private DataTable ObtenerDatosVentasMedicamento(int id_medicamento, DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            string connectionString = GetConnectionString();
            string query = $@"SELECT id_venta, codigo_de_barras, numero_de_lote, nombre, cantidad, precio_venta_total, costo_venta, ganancia_total, fecha_de_salida, realizado_por
                              FROM ViewVentaReporte 
                              WHERE id_medicamento = @IdMedicamento 
                              AND fecha_de_salida BETWEEN @FechaInicio AND @FechaFin 
                              ORDER BY fecha_de_salida DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdMedicamento", id_medicamento);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private DataTable ObtenerDatosVentasLaboratorio(int id_laboratorio, DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            string connectionString = GetConnectionString();
            string query = $@"SELECT id_venta, codigo_de_barras, numero_de_lote, nombre, cantidad, precio_venta_total, costo_venta, ganancia_total, fecha_de_salida, realizado_por
                              FROM ViewVentaReporte 
                              WHERE id_laboratorio = @IdLaboratorio 
                              AND fecha_de_salida BETWEEN @FechaInicio AND @FechaFin 
                              ORDER BY fecha_de_salida DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdLaboratorio", id_laboratorio);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private DataTable ObtenerDatosVentasFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            string connectionString = GetConnectionString();
            string query = $@"SELECT id_venta, codigo_de_barras, numero_de_lote, nombre, cantidad, precio_venta_total, costo_venta, ganancia_total, fecha_de_salida, realizado_por
                              FROM ViewVentaReporte 
                              WHERE fecha_de_salida BETWEEN @FechaInicio AND @FechaFin 
                              ORDER BY fecha_de_salida DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private DataTable ObtenerDatosComprasMedicamento(int id_medicamento, DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            string connectionString = GetConnectionString();
            string query = $@"SELECT id_compra, codigo_de_barras, numero_de_lote, nombre, laboratorio, cantidad, costo, costo_total, fecha_caducidad, fecha_de_entrada, realizado_por
                              FROM ViewCompraReporte 
                              WHERE id_medicamento = @IdMedicamento 
                              AND fecha_de_entrada BETWEEN @FechaInicio AND @FechaFin 
                              ORDER BY fecha_de_entrada DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdMedicamento", id_medicamento);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private DataTable ObtenerDatosComprasLaboratorio(int id_laboratorio, DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            string connectionString = GetConnectionString();
            string query = $@"SELECT id_compra, codigo_de_barras, numero_de_lote, nombre, laboratorio, cantidad, costo, costo_total, fecha_caducidad, fecha_de_entrada, realizado_por
                              FROM ViewCompraReporte 
                              WHERE id_laboratorio = @IdLaboratorio 
                              AND fecha_de_entrada BETWEEN @FechaInicio AND @FechaFin 
                              ORDER BY fecha_de_entrada DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdLaboratorio", id_laboratorio);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private DataTable ObtenerDatosComprasFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            string connectionString = GetConnectionString();
            string query = $@"SELECT id_compra, codigo_de_barras, numero_de_lote, nombre, laboratorio, cantidad, costo, costo_total, fecha_caducidad, fecha_de_entrada, realizado_por
                              FROM ViewCompraReporte 
                              WHERE fecha_de_entrada BETWEEN @FechaInicio AND @FechaFin 
                              ORDER BY fecha_de_entrada DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private string GenerarConsulta(string tabla, string campoFecha, string periodo)
        {
            string condicionFecha = "";

            if (periodo == "SEMANA")
            {
                condicionFecha = $"DATEPART(week, {campoFecha}) = DATEPART(week, GETDATE()) AND YEAR({campoFecha}) = YEAR(GETDATE())";
            }
            else if (periodo == "MES")
            {
                condicionFecha = $"MONTH({campoFecha}) = MONTH(GETDATE()) AND YEAR({campoFecha}) = YEAR(GETDATE())";
            }
            else if (periodo == "AÑO")
            {
                condicionFecha = $"YEAR({campoFecha}) = YEAR(GETDATE())";
            }

            return $"SELECT * FROM {tabla} WHERE {condicionFecha} ORDER BY {campoFecha} DESC";
        }

        private string GenerarNombreArchivo(string tipoReporte, string periodo)
        {
            string nombreArchivo = $"{tipoReporte}_";
            DateTime ahora = DateTime.Now;

            if (periodo == "SEMANA")
            {
                int semana = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(ahora, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                nombreArchivo += $"Semana_{semana}_{ahora.Year}.xlsx";
            }
            else if (periodo == "MES")
            {
                nombreArchivo += $"{ahora.ToString("MMMM")}_{ahora.Year}.xlsx";
            }
            else if (periodo == "AÑO")
            {
                nombreArchivo += $"{ahora.Year}.xlsx";
            }

            return nombreArchivo;
        }

        private void DescargarExcel(DataTable dt, string nombreArchivo)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Reporte");

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", $"attachment;filename={nombreArchivo}");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }
}
