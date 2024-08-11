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
    public partial class lotes : System.Web.UI.Page
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
            txtFechaCaducidad.Value = "";
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

        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Get_ConnectionString()))
                {
                    conexion.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertarLote", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@numero_de_lote", txtNumeroLote.Text);
                        cmd.Parameters.AddWithValue("@fecha_caducidad", txtFechaCaducidad.Value);
                        cmd.Parameters.AddWithValue("@id_medicamento", int.Parse(ddlCodigoB.SelectedValue));
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Lote insertado correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se insertó el lote.", "warning");
                        }
                    }
                    BorrarTxt();
                    GVLotes.DataBind();
                }
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                MostrarMensaje("Error: Ya existe un lote con el mismo número para este medicamento.", "warning");
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

                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarLote", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(lblId.Text));
                        cmd.Parameters.AddWithValue("@numero_de_lote", txtNumeroLote.Text);
                        cmd.Parameters.AddWithValue("@fecha_caducidad", txtFechaCaducidad.Value);
                        cmd.Parameters.AddWithValue("@id_medicamento", int.Parse(ddlCodigoB.SelectedValue));
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Lote modificado correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se modificó el lote.", "warning");
                        }
                    }
                    BorrarTxt();
                    GVLotes.DataBind();
                }
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                MostrarMensaje("Error: Ya existe otro lote con el mismo número para este medicamento.", "warning");
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
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarLote", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_lote", int.Parse(lblId.Text));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MostrarMensaje("Lote deshabilitado correctamente.", "success");
                        }
                        else
                        {
                            MostrarMensaje("No se deshabilitó el lote.", "warning");
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
            txtNumeroLote.Text = row.Cells[3].Text.Trim();
            txtFechaCaducidad.Value = row.Cells[7].Text.Trim();

            ddlEstado.SelectedValue = row.Cells[8].Text.Trim() == "True" ? "1" : "0";
        }
    }
}
