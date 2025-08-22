using POCKBIT_v2.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace POCKBIT_v2.Paginas
{
    public partial class ventas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SeguridadHelper.VerificarAutenticacion2FA(this);
            if (!IsPostBack)
            {
                LlenarDropDownLists();
                CargarClientes(); // Nuevo método para cargar clientes
            }
        }

        // Nuevo método para cargar clientes
        private void CargarClientes()
        {
            ddlCliente.DataBind(); // Esto cargará los clientes desde el SqlDataSource
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExcelHelper.ExportarDataTable(Response, GetAllVentas(), "Ventas.xlsx");
        }
        private DataTable GetAllVentas()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
            {
                string query = @"SELECT v.id_venta, m.codigo_de_barras, l.numero_de_lote, m.nombre, 
                       v.cantidad, v.precio_venta_total, v.costo_venta, v.ganancia_total, 
                       v.fecha_de_salida, v.realizado_por, 
                       ISNULL(c.nombre, 'Público en general') AS nombre_cliente
                       FROM venta v
                       INNER JOIN lote l ON v.id_lote = l.id_lote
                       INNER JOIN medicamento m ON l.id_medicamento = m.id_medicamento
                       LEFT OUTER JOIN cliente c ON v.id_cliente = c.id_cliente
                       ORDER BY v.id_venta DESC";
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
        public void BorrarTxt()
        {
            txtCantidadV.Text = "";
            txtCodigoBarras.Text = "";
            ddlLote.SelectedIndex = -1;
            ddlCliente.SelectedIndex = 0; // Seleccionar "Público en general"
            lblId.Text = "";
        }
        private void MostrarMensaje(string mensaje, string tipo)
        {
            string alertType;
            if (tipo == "success")
                alertType = "alert-success";
            else if (tipo == "info")
                alertType = "alert-info";
            else if (tipo == "warning")
                alertType = "alert-warning";
            else if (tipo == "danger")
                alertType = "alert-danger";
            else
                alertType = "alert-primary";

            string alertHtml = $@"
                <div class='alert {alertType} alert-dismissible fade show' role='alert'>
                    {mensaje}
                    <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
                </div>";
            ltlAlert.Text = alertHtml;
        }

        protected void LlenarDropDownLists()
        {
            ddlLote.DataBind();
            GVVentas.DataBind();
        }

        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertarVenta", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(ddlLote.SelectedValue));
                        cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidadV.Text));
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);
                        cmd.Parameters.AddWithValue("@fecha_de_salida", DateTime.Now);

                        // Nuevo parámetro para el cliente
                        if (!string.IsNullOrEmpty(ddlCliente.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@id_cliente", int.Parse(ddlCliente.SelectedValue));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@id_cliente", DBNull.Value);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int resultado = reader.GetInt32(reader.GetOrdinal("Resultado"));
                                string mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));

                                MostrarMensaje(mensaje, resultado == 1 ? "success" : "danger");
                            }
                        }
                    }
                    BorrarTxt();
                    LlenarDropDownLists();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "danger");
            }
        }

        protected void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarVenta", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_venta", int.Parse(lblId.Text));
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(ddlLote.SelectedValue));
                        cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidadV.Text));
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);
                        cmd.Parameters.AddWithValue("@fecha_de_salida", DateTime.Now);

                        // Nuevo parámetro para el cliente
                        if (!string.IsNullOrEmpty(ddlCliente.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@id_cliente", int.Parse(ddlCliente.SelectedValue));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@id_cliente", DBNull.Value);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int resultado = reader.GetInt32(reader.GetOrdinal("Resultado"));
                                string mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));

                                MostrarMensaje(mensaje, resultado == 1 ? "success" : "danger");
                            }
                        }
                    }
                    BorrarTxt();
                    LlenarDropDownLists();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "danger");
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarVenta", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_venta", int.Parse(lblId.Text));
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(ddlLote.SelectedValue));
                        cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidadV.Text));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Venta eliminada correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se eliminó la venta.", "warning");
                        }
                    }
                    BorrarTxt();
                    LlenarDropDownLists();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "danger");
            }
        }

        protected void GVVentas_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblId.Text = GVVentas.SelectedRow.Cells[1].Text.Trim();
            string codigo = GVVentas.SelectedRow.Cells[2].Text.Trim();
            string loteTexto = GVVentas.SelectedRow.Cells[3].Text.Trim();

            txtCodigoBarras.Text = codigo;
            txtCantidadV.Text = GVVentas.SelectedRow.Cells[5].Text.Trim();

            // Obtener id_medicamento por el código de barras
            using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT id_medicamento FROM medicamento WHERE codigo_de_barras = @codigo AND activo = 1", conexion))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        hiddenIdMedicamento.Value = resultado.ToString();

                        // Recargar lotes
                        ddlLote.DataBind();

                        // Seleccionar el lote correspondiente
                        ddlLote.ClearSelection(); // Limpiar selección previa
                        ListItem item = ddlLote.Items.FindByText(loteTexto);
                        if (item != null)
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            MostrarMensaje("⚠️ Lote no disponible para este medicamento.", "warning");
                        }
                    }
                    else
                    {
                        hiddenIdMedicamento.Value = string.Empty;
                        ddlLote.Items.Clear();
                        MostrarMensaje("⚠️ Producto no encontrado.", "danger");
                    }
                }

                // Obtener y seleccionar el cliente asociado a la venta
                using (SqlCommand cmd = new SqlCommand("SELECT id_cliente FROM venta WHERE id_venta = @id_venta", conexion))
                {
                    cmd.Parameters.AddWithValue("@id_venta", lblId.Text);
                    object idCliente = cmd.ExecuteScalar();

                    ddlCliente.ClearSelection(); // Limpiar selección previa

                    if (idCliente != null && idCliente != DBNull.Value)
                    {
                        ListItem item = ddlCliente.Items.FindByValue(idCliente.ToString());
                        if (item != null)
                        {
                            item.Selected = true;
                        }
                    }
                    else
                    {
                        // Seleccionar "Público en general" (primer ítem)
                        if (ddlCliente.Items.Count > 0)
                        {
                            ddlCliente.Items[0].Selected = true;
                        }
                    }
                }
            }
        }
        protected void GVVentas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button selectButton = e.Row.Cells[0].Controls[0] as Button;
                if (selectButton != null)
                {
                    selectButton.CssClass = "btn btn-info";
                }
            }
        }
        protected void txtCodigoBarras_TextChanged(object sender, EventArgs e)
        {
            string codigo = txtCodigoBarras.Text.Trim();

            if (!string.IsNullOrEmpty(codigo))
            {
                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT id_medicamento FROM medicamento WHERE codigo_de_barras = @codigo AND activo = 1", conexion))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null)
                        {
                            string idMedicamento = resultado.ToString();

                            hiddenIdMedicamento.Value = idMedicamento;

                            ddlLote.DataBind(); // Recargar lotes del medicamento
                            MostrarMensaje("✅ Producto encontrado. Selecciona el lote.", "success");
                        }
                        else
                        {
                            hiddenIdMedicamento.Value = string.Empty;
                            ddlLote.Items.Clear(); // Limpiar lotes
                            MostrarMensaje("⚠️ Producto no dado de alta aún.", "danger");
                        }
                    }
                }
            }
        }
    }
}