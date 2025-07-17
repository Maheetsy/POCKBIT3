using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using POCKBIT_v2.Helpers;

namespace POCKBIT_v2.Paginas
{
    public partial class compras : System.Web.UI.Page
    {
        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExcelHelper.ExportarDataTable(Response, GetAllCompras(), "Compras.xlsx");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LlenarDropDownLists();
            }
            SeguridadHelper.VerificarAutenticacion2FA(this);
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

                            MostrarMensaje("✅ Producto encontrado. Selecciona el lote.", "success");

                            // Rebind del DropDownList con parámetros manuales
                            SqlDataSourceLotes.SelectParameters["id_medicamento"].DefaultValue = idMedicamento;
                            ddlLote.DataBind();
                        }
                        else
                        {
                            hiddenIdMedicamento.Value = string.Empty;
                            ddlLote.Items.Clear();
                            MostrarMensaje("⚠️ Producto no dado de alta aún.", "danger");
                        }
                    }
                }
            }
        }
        private DataTable GetAllCompras()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
            {
                string query = "SELECT id_compra, codigo_de_barras, numero_de_lote, nombre, laboratorio, cantidad, costo, costo_total, fecha_caducidad, fecha_de_entrada, realizado_por FROM ViewCompra ORDER BY id_compra DESC";
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
            txtCantidadC.Text = ""; 
            txtCodigoBarras.Text = "";
            ddlLote.SelectedIndex = -1;
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
            //ddlLote.DataBind();
            GVCompras.DataBind();
        }
        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertarCompra", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(ddlLote.SelectedValue));
                        cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidadC.Text));
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);
                        cmd.Parameters.AddWithValue("@fecha_de_entrada", DateTime.Now);

                        // Cambiar para leer el mensaje de retorno
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                MostrarMensaje(reader["mensaje"].ToString(), "success");
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
                // Validación previa
                if (string.IsNullOrEmpty(lblId.Text))
                {
                    MostrarMensaje("⚠️ Por favor selecciona una compra de la tabla para modificar.", "warning");
                    return;
                }

                if (string.IsNullOrEmpty(ddlLote.SelectedValue))
                {
                    MostrarMensaje("⚠️ Por favor selecciona un lote válido antes de modificar.", "warning");
                    return;
                }

                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarCompra", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_compra", int.Parse(lblId.Text));
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(ddlLote.SelectedValue));
                        cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidadC.Text));
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);
                        cmd.Parameters.AddWithValue("@fecha_de_entrada", DateTime.Now);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                MostrarMensaje(reader["mensaje"].ToString(), "success");
                            }
                        }
                    }

                    // Limpiar campos y refrescar tabla
                    BorrarTxt();
                    LlenarDropDownLists();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("❌ Error al modificar: " + ex.Message, "danger");
            }
        }
        protected void GVCompras_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GVCompras.SelectedRow;

            lblId.Text = row.Cells[1].Text.Trim();
            txtCantidadC.Text = row.Cells[6].Text.Trim();

            string codigoBarras = row.Cells[2].Text.Trim();
            string numeroLote = row.Cells[3].Text.Trim();
            txtCodigoBarras.Text = codigoBarras;

            // Simular el comportamiento de TextChanged (obtener id_medicamento y llenar ddlLote)
            using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT id_medicamento FROM medicamento WHERE codigo_de_barras = @codigo AND activo = 1", conexion))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigoBarras);
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        string idMedicamento = resultado.ToString();
                        hiddenIdMedicamento.Value = idMedicamento;

                        SqlDataSourceLotes.SelectParameters["id_medicamento"].DefaultValue = idMedicamento;
                        ddlLote.DataBind();

                        // Seleccionar el lote correcto
                        foreach (ListItem item in ddlLote.Items)
                        {
                            if (item.Text == numeroLote)
                            {
                                ddlLote.ClearSelection();
                                item.Selected = true;
                                break;
                            }
                        }

                        MostrarMensaje("✅ Medicamento y lote recuperados correctamente.", "success");
                    }
                    else
                    {
                        MostrarMensaje("⚠️ El medicamento ya no existe o está inactivo.", "danger");
                        ddlLote.Items.Clear();
                        hiddenIdMedicamento.Value = "";
                    }
                }
            }
        }
        protected void GVCompras_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarCompra", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_compra", int.Parse(lblId.Text));

                        // Cambiar para leer el mensaje de retorno
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                MostrarMensaje(reader["mensaje"].ToString(), "success");
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
    }
}

