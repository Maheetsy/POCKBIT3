﻿using POCKBIT_v2.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace POCKBIT_v2.Paginas
{
    public partial class medicamentos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SeguridadHelper.VerificarAutenticacion2FA(this);
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExcelHelper.ExportarDataTable(Response, GetAllMedicamentos(), "Medicamentos.xlsx");
        }

        private DataTable GetAllMedicamentos()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT id_medicamento, codigo_de_barras, nombre, descripcion, nombre_laboratorio, costo, precio_venta, precio_maximo_publico, cantidad_total, fecha_de_registro, activo,realizado_por FROM ViewMedicamento ORDER BY id_medicamento DESC", conexion))
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
                // Validar que todos los campos estén llenos
                if (string.IsNullOrWhiteSpace(txtNombreC.Text) ||
                    string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                    string.IsNullOrWhiteSpace(txtCosto.Text) ||
                    string.IsNullOrWhiteSpace(txtPrecioP.Text) ||
                    string.IsNullOrWhiteSpace(txtPrecioV.Text) ||
                    string.IsNullOrWhiteSpace(txtCodigoB.Text) ||
                    ddlLaboratorio.SelectedIndex == -1 ||
                    ddlEstado.SelectedIndex == -1)
                {
                    MostrarMensaje("Por favor, completa todos los campos antes de insertar.", "warning");
                    return;
                }

                // Conversión de tipos
                float costo = float.Parse(txtCosto.Text);
                float precioMaximo = float.Parse(txtPrecioP.Text);
                float precioVenta = float.Parse(txtPrecioV.Text);
                int idLaboratorio = int.Parse(ddlLaboratorio.SelectedValue);
                bool activo = ddlEstado.SelectedValue == "1";
                // Validaciones lógicas entre precios
                if (costo >= precioVenta)
                {
                    MostrarMensaje("El costo debe ser menor que el precio de venta.", "warning");
                    return;
                }

                if (precioVenta > precioMaximo)
                {
                    MostrarMensaje("El precio de venta no puede ser mayor que el precio máximo al público.", "warning");
                    return;
                }

                if (precioMaximo < precioVenta || precioMaximo < costo)
                {
                    MostrarMensaje("El precio máximo al público debe ser mayor o igual al precio de venta y al costo.", "warning");
                    return;
                }

                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertarMedicamento", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@nombre", txtNombreC.Text.Trim());
                        cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());
                        cmd.Parameters.AddWithValue("@costo", costo);
                        cmd.Parameters.AddWithValue("@precio_maximo_publico", precioMaximo);
                        cmd.Parameters.AddWithValue("@precio_venta", precioVenta);
                        cmd.Parameters.AddWithValue("@codigo_de_barras", txtCodigoB.Text.Trim());
                        cmd.Parameters.AddWithValue("@id_laboratorio", idLaboratorio);
                        cmd.Parameters.AddWithValue("@fecha_de_registro", DateTime.Now);
                        cmd.Parameters.AddWithValue("@activo", activo);
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string mensaje = reader["mensaje"].ToString();
                                if (mensaje.Contains("insertado correctamente"))
                                {
                                    MostrarMensaje(mensaje, "success");
                                    BorrarTxt();
                                    GVMedicamentos.DataBind();
                                }
                                else if (mensaje.StartsWith("Error"))
                                {
                                    MostrarMensaje(mensaje, "warning");
                                }
                                else
                                {
                                    MostrarMensaje("Ocurrió algo inesperado. Intenta de nuevo.", "danger");
                                }
                            }
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MostrarMensaje("Error: Asegúrate de que costo y precios sean valores numéricos.", "danger");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error inesperado: " + ex.Message, "danger");
            }
        }

        protected void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(txtNombreC.Text) ||
                    string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                    string.IsNullOrWhiteSpace(txtCosto.Text) ||
                    string.IsNullOrWhiteSpace(txtPrecioP.Text) ||
                    string.IsNullOrWhiteSpace(txtPrecioV.Text) ||
                    string.IsNullOrWhiteSpace(txtCodigoB.Text) ||
                    ddlLaboratorio.SelectedIndex == -1 ||
                    ddlEstado.SelectedIndex == -1)
                {
                    MostrarMensaje("Por favor, completa todos los campos antes de modificar.", "warning");
                    return;
                }

                // Conversión de tipos
                float costo = float.Parse(txtCosto.Text);
                float precioMaximo = float.Parse(txtPrecioP.Text);
                float precioVenta = float.Parse(txtPrecioV.Text);

                // Validaciones entre precios
                if (costo >= precioVenta)
                {
                    MostrarMensaje("El costo debe ser menor que el precio de venta.", "warning");
                    return;
                }
                if (precioVenta > precioMaximo)
                {
                    MostrarMensaje("El precio de venta no puede ser mayor que el precio máximo al público.", "warning");
                    return;
                }
                if (precioMaximo < precioVenta || precioMaximo < costo)
                {
                    MostrarMensaje("El precio máximo al público debe ser mayor o igual al precio de venta y al costo.", "warning");
                    return;
                }

                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarMedicamento", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_medicamento", lblId.Text);
                        cmd.Parameters.AddWithValue("@nombre", txtNombreC.Text.Trim());
                        cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());
                        cmd.Parameters.AddWithValue("@costo", costo);
                        cmd.Parameters.AddWithValue("@precio_maximo_publico", precioMaximo);
                        cmd.Parameters.AddWithValue("@precio_venta", precioVenta);
                        cmd.Parameters.AddWithValue("@codigo_de_barras", txtCodigoB.Text.Trim());
                        cmd.Parameters.AddWithValue("@id_laboratorio", ddlLaboratorio.SelectedValue);
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string mensaje = reader["mensaje"].ToString();
                                if (mensaje.StartsWith("Medicamento modificado correctamente"))
                                    MostrarMensaje(mensaje, "success");
                                else if (mensaje.StartsWith("Error"))
                                    MostrarMensaje(mensaje, "warning");
                                else
                                    MostrarMensaje("No se modificó el medicamento.", "warning");
                            }
                        }

                        BorrarTxt();
                        GVMedicamentos.DataBind();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                MostrarMensaje("Error: Ya existe otro medicamento con el mismo código de barras.", "warning");
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                MostrarMensaje("Error: Ya existe un medicamento con el mismo nombre y laboratorio.", "warning");
            }
            catch (FormatException)
            {
                MostrarMensaje("Error: Asegúrate de que costo y precios sean valores numéricos.", "danger");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error inesperado: " + ex.Message, "danger");
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(DBHelper.GetConnectionString()))
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

        protected void txtCodigoB_TextChanged(object sender, EventArgs e)
        {
            string codigo = txtCodigoB.Text.Trim();

        }
    }
}