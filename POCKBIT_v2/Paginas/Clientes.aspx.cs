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
    public partial class Clientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GVClientes.DataBind();
            }
            SeguridadHelper.VerificarAutenticacion2FA(this);
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = GetAllClientes();
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt, "Clientes");
                var headerRow = ws.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                headerRow.Style.Font.FontColor = XLColor.White;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Clientes.xlsx");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private DataTable GetAllClientes()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
            {
                string query = "SELECT id_cliente, nombre, direccion, telefono, email, fecha_registro, activo FROM ViewCliente ORDER BY id_cliente DESC";
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
        protected void GVClientes_Sorting(object sender, GridViewSortEventArgs e)
        {
            // Obtener la dirección de la ordenación (ascendente o descendente)
            string sortDirection = "ASC";
            if (ViewState["SortDirection"] != null && ViewState["SortDirection"].ToString() == "ASC")
            {
                sortDirection = "DESC";
            }

            // Actualizar ViewState con la nueva dirección
            ViewState["SortDirection"] = sortDirection;

            // Actualizar el orden de la columna
            SqlDataSourceClientes.SelectCommand = $"SELECT id_cliente, nombre, direccion, telefono, email, fecha_registro, activo FROM cliente ORDER BY {e.SortExpression} {sortDirection}";
            GVClientes.DataBind(); // Refrescar el GridView
        }

        public void BorrarCampos()
        {
            lblIdCliente.Text = "";
            txtNombre.Text = "";
            txtDireccion.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            ddlEstado.SelectedIndex = 0;
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

        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertarCliente", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text.Trim());
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                MostrarMensaje(reader["mensaje"].ToString(), "success");
                            }
                        }
                    }
                    BorrarCampos();
                    GVClientes.DataBind();
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
                if (string.IsNullOrEmpty(lblIdCliente.Text))
                {
                    MostrarMensaje("Debe seleccionar un cliente primero", "warning");
                    return;
                }

                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarCliente", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_cliente", int.Parse(lblIdCliente.Text));
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text.Trim());
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim()); 
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                MostrarMensaje(reader["mensaje"].ToString(), "success");
                            }
                        }
                    }
                    BorrarCampos();
                    GVClientes.DataBind();
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
                if (string.IsNullOrEmpty(lblIdCliente.Text))
                {
                    MostrarMensaje("Debe seleccionar un cliente primero", "warning");
                    return;
                }

                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarCliente", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_cliente", int.Parse(lblIdCliente.Text));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                MostrarMensaje(reader["mensaje"].ToString(), "success");
                            }
                        }
                    }
                    BorrarCampos();
                    GVClientes.DataBind();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "danger");
            }
        }

        protected void GVClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GVClientes.SelectedRow;

            lblIdCliente.Text = row.Cells[1].Text.Trim(); // ID
            txtNombre.Text = row.Cells[2].Text.Trim(); // Nombre
            txtDireccion.Text = row.Cells[3].Text.Trim(); // Dirección
            txtTelefono.Text = row.Cells[4].Text.Trim(); // Teléfono
            txtEmail.Text = row.Cells[5].Text.Trim(); // Email
            ddlEstado.SelectedValue = row.Cells[6].Text.Trim() == "True" ? "1" : "0";
        }

        protected void GVClientes_RowDataBound(object sender, GridViewRowEventArgs e)
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