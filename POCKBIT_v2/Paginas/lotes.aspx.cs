using ClosedXML.Excel;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POCKBIT_v2.Paginas
{
    public partial class lotes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GVLotes.DataBind();
            }
            SeguridadHelper.VerificarAutenticacion2FA(this);
        }
   

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = GetDataTableFromGridView(GVLotes);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Lotes");

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Lotes.xlsx");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private DataTable GetDataTableFromGridView(GridView gv)
        {
            DataTable dt = new DataTable();

            foreach (DataControlField column in gv.Columns)
            {
                dt.Columns.Add(column.HeaderText);
            }

            foreach (GridViewRow row in gv.Rows)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text.Trim();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        protected void GVLotes_RowDataBound(object sender, GridViewRowEventArgs e)
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

        public void BorrarTxt()
        {
            txtNumeroLote.Text = "";
            txtCodigoB.Text = "";
            txtFechaCaducidad.Value = "";
            lblId.Text = "";
            ddlEstado.SelectedIndex = 1;
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string alertType;

            if (tipo == "success")
            {
                alertType = "alert-success";
            }
            else if (tipo == "info")
            {
                alertType = "alert-info";
            }
            else if (tipo == "warning")
            {
                alertType = "alert-warning";
            }
            else if (tipo == "danger")
            {
                alertType = "alert-danger";
            }
            else
            {
                alertType = "alert-primary";
            }

            string alertHtml = $@"
        <div class='alert {alertType} alert-dismissible fade show' role='alert'>
            {mensaje}
            <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
        </div>";

            ltlAlert.Text = alertHtml;
        }
        protected void txtCodigoB_TextChanged(object sender, EventArgs e)
        {
            string codigo = txtCodigoB.Text.Trim();

        }
        private int? ObtenerIdMedicamentoDesdeCodigo(string codigoBarras, SqlConnection conexion)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT id_medicamento FROM medicamento WHERE codigo_de_barras = @codigo", conexion))
            {
                cmd.Parameters.AddWithValue("@codigo", codigoBarras);
                object resultado = cmd.ExecuteScalar();
                if (resultado != null && int.TryParse(resultado.ToString(), out int id))
                {
                    return id;
                }
                return null;
            }
        }
        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();

                    // Obtener el ID del medicamento a partir del código de barras
                    int? idMedicamento = ObtenerIdMedicamentoDesdeCodigo(txtCodigoB.Text.Trim(), conexion);
                    if (idMedicamento == null)
                    {
                        MostrarMensaje("Error: Código de barras no encontrado en la base de datos.", "warning");
                        return;
                    }

                    using (SqlCommand cmd = new SqlCommand("sp_InsertarLote", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@numero_de_lote", txtNumeroLote.Text);
                        cmd.Parameters.AddWithValue("@fecha_caducidad", txtFechaCaducidad.Value);
                        cmd.Parameters.AddWithValue("@id_medicamento", idMedicamento.Value);
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string mensaje = reader["mensaje"].ToString();
                                if (mensaje.StartsWith("Lote insertado correctamente"))
                                    MostrarMensaje(mensaje, "success");
                                else if (mensaje.StartsWith("Error"))
                                    MostrarMensaje(mensaje, "warning");
                                else
                                    MostrarMensaje("No se insertó el lote.", "warning");
                            }
                        }
                    }

                    BorrarTxt();
                    GVLotes.DataBind();
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

                    // Obtener el ID del medicamento a partir del código de barras
                    int? idMedicamento = ObtenerIdMedicamentoDesdeCodigo(txtCodigoB.Text.Trim(), conexion);
                    if (idMedicamento == null)
                    {
                        MostrarMensaje("Error: Código de barras no encontrado en la base de datos.", "warning");
                        return;
                    }

                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarLote", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(lblId.Text));
                        cmd.Parameters.AddWithValue("@numero_de_lote", txtNumeroLote.Text);
                        cmd.Parameters.AddWithValue("@fecha_caducidad", txtFechaCaducidad.Value);
                        cmd.Parameters.AddWithValue("@id_medicamento", idMedicamento.Value);
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string mensaje = reader["mensaje"].ToString();
                                if (mensaje.StartsWith("Lote modificado correctamente"))
                                    MostrarMensaje(mensaje, "success");
                                else if (mensaje.StartsWith("Error"))
                                    MostrarMensaje(mensaje, "warning");
                                else
                                    MostrarMensaje("No se modificó el lote.", "warning");
                            }
                        }
                    }

                    BorrarTxt();
                    GVLotes.DataBind();
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
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarLote", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(lblId.Text));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string mensaje = reader["mensaje"].ToString();
                                if (mensaje.StartsWith("Lote desactivado correctamente"))
                                    MostrarMensaje(mensaje, "success");
                                else if (mensaje.StartsWith("Error"))
                                    MostrarMensaje(mensaje, "warning");
                                else
                                    MostrarMensaje("No se desactivó el lote.", "warning");
                            }
                        }
                    }
                    BorrarTxt();
                    GVLotes.DataBind();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "danger");
            }
        }

        protected void GVLotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GVLotes.SelectedRow;

            lblId.Text = row.Cells[1].Text.Trim();
            txtCodigoB.Text = row.Cells[2].Text.Trim();
            txtNumeroLote.Text = row.Cells[3].Text.Trim();
            txtFechaCaducidad.Value = row.Cells[7].Text.Trim();

            ddlEstado.SelectedValue = row.Cells[8].Text.Trim() == "True" ? "1" : "0";

        }
    }
}
