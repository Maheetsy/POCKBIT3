using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POCKBIT_v2.Paginas
{
    public partial class tickets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SeguridadHelper.VerificarAutenticacion2FA(this);
            if (!IsPostBack)
            {
                CargarVentas();
            }
        }

        private void CargarVentas()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
                {
                    // CORRECCIÓN: Usar 'total' en lugar de 'precio_venta_total' como está en la vista
                    string query = @"SELECT id_venta, fecha_de_salida, total AS precio_venta_total, 
                               medicamento, cantidad, realizado_por
                               FROM ViewTicket
                               ORDER BY fecha_de_salida DESC";

                    new SqlDataAdapter(query, conexion).Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        // Aquí ya no es necesario asignar el DataSource manualmente
                        // GVTickets.DataSource = dt;  // ELIMINAR esta línea
                        // GVTickets.DataBind();  // ELIMINAR esta línea
                    }
                    else
                    {
                        MostrarMensaje("No se encontraron ventas registradas.");
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar ventas: {ex.Message}");
            }
        }

        protected void GVTickets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ImprimirTicket")
            {
                try
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    int idVenta = Convert.ToInt32(GVTickets.DataKeys[index].Value);

                    if (idVenta <= 0)
                    {
                        MostrarError("ID de venta inválido");
                        return;
                    }

                    string ticketHtml = GenerarHtmlTicket(idVenta);

                    if (ticketHtml.Contains("error-ticket"))
                    {
                        MostrarError("Error al generar el ticket. Verifique los datos.");
                        return;
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "MostrarTicket",
                        $"mostrarTicket({Newtonsoft.Json.JsonConvert.SerializeObject(ticketHtml)});", true);
                }
                catch (Exception ex)
                {
                    MostrarError($"Error inesperado: {ex.Message}");
                }
            }
        }
        protected void GVTickets_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortDirection = "ASC";

            if (ViewState["SortDirection"] != null && ViewState["SortDirection"].ToString() == "ASC")
            {
                sortDirection = "DESC";
            }

            // Actualizar ViewState con la nueva dirección
            ViewState["SortDirection"] = sortDirection;

            // Actualizar el orden de la columna
            SqlDataSourceTickets.SelectCommand = $"SELECT id_venta, fecha_de_salida, medicamento, cantidad, precio_venta_total, " +
                                                 $"costo_venta, ganancia_total, fecha_de_salida, realizado_por, nombre_cliente " +
                                                 $"FROM ViewTicket ORDER BY {e.SortExpression} {sortDirection}";

            GVTickets.DataBind(); // Refrescar el GridView
        }

        private string GenerarHtmlTicket(int idVenta)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
                {
                    conexion.Open();

                    string query = @"SELECT id_venta, fecha_de_salida, total, 
                                   cliente, medicamento, cantidad, realizado_por
                                   FROM ViewTicket
                                   WHERE id_venta = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", idVenta);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                return "<div class='error-ticket'>Venta no encontrada</div>";
                            }

                            if (reader.Read())
                            {
                                sb.Append($@"
                                    <div class='ticket-content'>
                                        <div class='ticket-header'>
                                            <h3>POCKBIT PHARMA</h3>
                                            <p>RFC: XXX-123456</p>
                                            <p>Av. Central #123, CDMX</p>
                                        </div>
                                        <hr/>
                                        <p><strong>Venta No:</strong> {reader["id_venta"]}</p>
                                        <p><strong>Fecha:</strong> {Convert.ToDateTime(reader["fecha_de_salida"]).ToString("dd/MM/yyyy HH:mm")}</p>
                                        <p><strong>Cliente:</strong> {reader["cliente"]}</p>
                                        <hr/>
                                        <p><strong>Medicamento:</strong> {reader["medicamento"]}</p>
                                        <p><strong>Cantidad:</strong> {reader["cantidad"]}</p>
                                        <hr/>
                                        <p><strong>TOTAL:</strong> ${Convert.ToDecimal(reader["total"]).ToString("N2")}</p>
                                        <p><strong>Atendido por:</strong> {reader["realizado_por"]}</p>
                                        <hr/>
                                        <div class='ticket-footer'>
                                            <p style='font-weight:bold;'>¡GRACIAS POR SU COMPRA!</p>
                                        </div>
                                    </div>
                                    <div class='ticket-actions'>
                                        <button onclick='imprimirTicket()' class='btn-imprimir'>🖨️ Imprimir</button>
                                        <button onclick='cerrarTicket()' class='btn-cerrar'>✖ Cerrar</button>
                                    </div>");
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogError($"Error SQL: {sqlEx.Message}");
                return "<div class='error-ticket'>Error de base de datos</div>";
            }
            catch (Exception ex)
            {
                LogError($"Error general: {ex.Message}");
                return "<div class='error-ticket'>Error al generar ticket</div>";
            }

            return sb.ToString();
        }

        private string Get_ConnectionString()
        {
            try
            {
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"];
                if (connectionString == null)
                {
                    throw new Exception("Cadena de conexión no configurada");
                }
                return connectionString.ConnectionString;
            }
            catch (Exception ex)
            {
                LogError($"Error obteniendo conexión: {ex.Message}");
                throw;
            }
        }

        private void MostrarError(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarError",
                $"alert('ERROR: {mensaje.Replace("'", "\\'")}');", true);
        }

        private void MostrarMensaje(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarMensaje",
                $"alert('{mensaje.Replace("'", "\\'")}');", true);
        }

        private void LogError(string mensaje)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] {DateTime.Now}: {mensaje}");
        }
    }
}