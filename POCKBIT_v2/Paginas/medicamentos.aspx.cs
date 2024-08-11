using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;

namespace POCKBIT_v2.Paginas
{
    public partial class medicamentos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TwoFactorVerified"] == null || !(bool)Session["TwoFactorVerified"])
            {
                Response.Redirect("~/Account/Login");
            }
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = GetAllMedicamentos();
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt, "Medicamentos");
                var headerRow = ws.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                headerRow.Style.Font.FontColor = XLColor.White;

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Medicamentos.xlsx");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private DataTable GetAllMedicamentos()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT id_medicamento, codigo_de_barras, nombre, descripcion, nombre_laboratorio, costo, precio_venta, precio_maximo_publico, cantidad_total, fecha_de_registro, activo FROM ViewMedicamento ORDER BY id_medicamento DESC", conexion))
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
            txtNombreC.Text = "";
            txtDescripcion.Text = "";
            txtCosto.Text = "";
            txtPrecioP.Text = "";
            txtPrecioV.Text = "";
            txtCodigoB.Text = "";
            lblId.Text = "";
            ddlEstado.SelectedIndex = 1;
        }

        public string Get_ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string alertType;
            switch (tipo)
            {
                case "success":
                    alertType = "alert-success";
                    break;
                case "info":
                    alertType = "alert-info";
                    break;
                case "warning":
                    alertType = "alert-warning";
                    break;
                case "danger":
                    alertType = "alert-danger";
                    break;
                default:
                    alertType = "alert-primary";
                    break;
            }

            ltlAlert.Text = $@"
                <div class='alert {alertType} alert-dismissible fade show' role='alert'>
                    {mensaje}
                    <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
                </div>";
        }

        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertarMedicamento", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nombre", txtNombreC.Text);
                        cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text);
                        cmd.Parameters.AddWithValue("@costo", txtCosto.Text);
                        cmd.Parameters.AddWithValue("@precio_maximo_publico", txtPrecioP.Text);
                        cmd.Parameters.AddWithValue("@precio_venta", txtPrecioV.Text);
                        cmd.Parameters.AddWithValue("@codigo_de_barras", txtCodigoB.Text);
                        cmd.Parameters.AddWithValue("@id_laboratorio", ddlLaboratorio.SelectedValue);
                        cmd.Parameters.AddWithValue("@fecha_de_registro", DateTime.Now);
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Medicamento insertado correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se insertó el medicamento.", "warning");
                        }
                    }
                    BorrarTxt();
                    GVMedicamentos.DataBind();
                }
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                MostrarMensaje("Error: Ya existe un medicamento con el mismo código de barras.", "warning");
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
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarMedicamento", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_medicamento", lblId.Text);
                        cmd.Parameters.AddWithValue("@nombre", txtNombreC.Text);
                        cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text);
                        cmd.Parameters.AddWithValue("@costo", txtCosto.Text);
                        cmd.Parameters.AddWithValue("@precio_maximo_publico", txtPrecioP.Text);
                        cmd.Parameters.AddWithValue("@precio_venta", txtPrecioV.Text);
                        cmd.Parameters.AddWithValue("@codigo_de_barras", txtCodigoB.Text);
                        cmd.Parameters.AddWithValue("@id_laboratorio", ddlLaboratorio.SelectedValue);
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Medicamento modificado correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se modificó el medicamento.", "warning");
                        }
                    }
                    BorrarTxt();
                    GVMedicamentos.DataBind();
                }
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                MostrarMensaje("Error: Ya existe otro medicamento con el mismo código de barras.", "warning");
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
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarMedicamento", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_medicamento", lblId.Text);
                        cmd.ExecuteNonQuery();
                    }
                    MostrarMensaje("Medicamento marcado como inactivo correctamente.", "success");
                    BorrarTxt();
                    GVMedicamentos.DataBind();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "danger");
            }
        }

        protected void GVMedicamentos_SelectedIndexChanged1(object sender, EventArgs e)
        {
            GridViewRow row = GVMedicamentos.SelectedRow;

            lblId.Text = row.Cells[1].Text.Trim();
            txtCodigoB.Text = row.Cells[2].Text.Trim();
            txtNombreC.Text = row.Cells[3].Text.Trim();
            txtDescripcion.Text = row.Cells[4].Text.Trim();
            txtCosto.Text = row.Cells[6].Text.Replace("$", "").Trim();
            txtPrecioV.Text = row.Cells[7].Text.Replace("$", "").Trim();
            txtPrecioP.Text = row.Cells[8].Text.Replace("$", "").Trim();
            ddlEstado.SelectedValue = row.Cells[10].Text.Trim() == "True" ? "1" : "0";
        }

        protected void GVMedicamentos_RowDataBound(object sender, GridViewRowEventArgs e)
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
    }
}