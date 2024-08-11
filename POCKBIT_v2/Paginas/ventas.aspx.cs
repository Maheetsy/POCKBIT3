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
    public partial class ventas : System.Web.UI.Page
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
            DataTable dt = GetAllVentas();
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt, "Ventas");

                var headerRow = ws.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                headerRow.Style.Font.FontColor = XLColor.White;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Ventas.xlsx");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private DataTable GetAllVentas()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
            {
                string query = "SELECT id_venta, codigo_de_barras, numero_de_lote, nombre, cantidad, precio_venta_total, costo_venta, ganancia_total, fecha_de_salida, realizado_por FROM ViewVenta ORDER BY id_venta DESC";
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
            GVVentas.DataBind();
        }

        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertarVenta", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(ddlLote.SelectedValue));
                        cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidadV.Text));
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);
                        cmd.Parameters.AddWithValue("@fecha_de_salida", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Venta registrada correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se registró la venta.", "warning");
                        }
                    }
                    BorrarTxt();
                    LlenarDropDownLists();
                }
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                MostrarMensaje("No hay suficiente cantidad en el inventario.", "danger");
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
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarVenta", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_venta", int.Parse(lblId.Text));
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(ddlLote.SelectedValue));
                        cmd.Parameters.AddWithValue("@cantidad", int.Parse(txtCantidadV.Text));
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);
                        cmd.Parameters.AddWithValue("@fecha_de_salida", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Venta modificada correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se modificó la venta.", "warning");
                        }
                    }
                    BorrarTxt();
                    LlenarDropDownLists();
                }
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                MostrarMensaje("No hay suficiente cantidad en el inventario.", "danger");
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
            txtCantidadV.Text = GVVentas.SelectedRow.Cells[4].Text.Trim();
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
    }
}
