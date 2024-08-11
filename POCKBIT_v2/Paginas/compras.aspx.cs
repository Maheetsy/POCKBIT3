using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;

namespace POCKBIT_v2.Paginas
{
    public partial class compras : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LlenarDropDownLists();
            }
            if (Session["TwoFactorVerified"] == null || !(bool)Session["TwoFactorVerified"])
            {
                Response.Redirect("~/Account/Login");
            }
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = GetAllCompras();
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt, "Compras");
                var headerRow = ws.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                headerRow.Style.Font.FontColor = XLColor.White;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Compras.xlsx");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private DataTable GetAllCompras()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
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
            ddlCodigoB.SelectedIndex = -1;
            ddlLote.SelectedIndex = -1;
            lblId.Text = "";
        }

        public string Get_ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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
            ddlCodigoB.DataBind();
            ddlLote.DataBind();
            GVCompras.DataBind();
        }

        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertarCompra", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(ddlLote.SelectedValue));
                        cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidadC.Text));
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);
                        cmd.Parameters.AddWithValue("@fecha_de_entrada", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Compra registrada correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se registró la compra.", "warning");
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
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
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

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Compra modificada correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se modificó la compra.", "warning");
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
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarCompra", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_compra", int.Parse(lblId.Text));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Compra eliminada correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se eliminó la compra.", "warning");
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

        protected void GVCompras_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblId.Text = GVCompras.SelectedRow.Cells[1].Text.Trim();
            txtCantidadC.Text = GVCompras.SelectedRow.Cells[7].Text.Trim();
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
    }
}

