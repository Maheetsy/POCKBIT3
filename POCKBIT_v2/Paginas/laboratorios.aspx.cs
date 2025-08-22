﻿using POCKBIT_v2.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace POCKBIT_v2.Paginas
{
    public partial class laboratorios : System.Web.UI.Page
    {
        private string ConnectionString => ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            SeguridadHelper.VerificarAutenticacion2FA(this);
        }
        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConnectionString))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertarLaboratorio", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nombre", txtNombreL.Text);
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);
                        cmd.ExecuteNonQuery();
                    }
                    MostrarMensaje("Laboratorio insertado correctamente.", "success");
                    BorrarTxt();
                    GRVLaboratorios.DataBind();
                    SqlDataSourceLaboratorios.DataBind();
                }
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                MostrarMensaje("El laboratorio ya existe y está activo.", "warning");
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
                using (SqlConnection conexion = new SqlConnection(ConnectionString))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarLaboratorio", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", lblId.Text);
                        cmd.Parameters.AddWithValue("@nombre", txtNombreL.Text);
                        cmd.Parameters.AddWithValue("@activo", ddlEstado.SelectedValue);
                        cmd.Parameters.AddWithValue("@realizado_por", HttpContext.Current.User.Identity.Name);
                        cmd.ExecuteNonQuery();
                    }
                    MostrarMensaje("Laboratorio modificado correctamente.", "success");
                    BorrarTxt();
                    GRVLaboratorios.DataBind();
                    SqlDataSourceLaboratorios.DataBind();
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
                using (SqlConnection conexion = new SqlConnection(ConnectionString))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarLaboratorio", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", lblId.Text);
                        cmd.ExecuteNonQuery();
                    }
                    MostrarMensaje("Laboratorio marcado como inactivo correctamente.", "success");
                    BorrarTxt();
                    GRVLaboratorios.DataBind();
                    SqlDataSourceLaboratorios.DataBind();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "danger");
            }
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExcelHelper.ExportarDataTable(Response, GetAllLaboratorios(), "Laboratorios.xlsx");

        }

        private DataTable GetAllLaboratorios()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conexion = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [id_laboratorio], [nombre], [activo] FROM [laboratorio]", conexion))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        protected void GRVLaboratorios_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblId.Text = GRVLaboratorios.SelectedRow.Cells[1].Text;
            txtNombreL.Text = GRVLaboratorios.SelectedRow.Cells[2].Text;
            ddlEstado.SelectedValue = GRVLaboratorios.SelectedRow.Cells[3].Text == "Activo" ? "1" : "0";
        }

        protected void GRVLaboratorios_RowDataBound(object sender, GridViewRowEventArgs e)
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

        private void BorrarTxt()
        {
            txtNombreL.Text = "";
            lblId.Text = "";
            ddlEstado.SelectedIndex = 1;
        }
    }
}
