using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using POCKBIT_v2.Models;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POCKBIT_v2.Paginas
{
    public partial class Roles : System.Web.UI.Page
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        protected void Page_Load(object sender, EventArgs e)
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (!IsPostBack)
            {
                CargarDatosIniciales();
            }
            SeguridadHelper.VerificarAutenticacion2FA(this);

        }

        private void CargarDatosIniciales()
        {
            CargarUsuarios();
            CargarRoles();
            CargarRolesEliminar();
            MostrarUsuariosConRoles();
            MostrarTodosRoles();
        }

        private void CargarUsuarios()
        {
            ddlUsuarios.DataSource = db.Users.OrderBy(u => u.UserName).ToList();
            ddlUsuarios.DataTextField = "UserName";
            ddlUsuarios.DataValueField = "Id";
            ddlUsuarios.DataBind();
            ddlUsuarios.Items.Insert(0, new ListItem("-- Seleccione un usuario --", ""));
        }

        private void CargarRoles()
        {
            ddlRoles.DataSource = db.Roles.OrderBy(r => r.Name).ToList();
            ddlRoles.DataTextField = "Name";
            ddlRoles.DataValueField = "Id";
            ddlRoles.DataBind();
            ddlRoles.Items.Insert(0, new ListItem("-- Seleccione un rol --", ""));
        }

        private void CargarRolesEliminar()
        {
            ddlRolesEliminar.DataSource = db.Roles.OrderBy(r => r.Name).ToList();
            ddlRolesEliminar.DataTextField = "Name";
            ddlRolesEliminar.DataValueField = "Id";
            ddlRolesEliminar.DataBind();
            ddlRolesEliminar.Items.Insert(0, new ListItem("-- Seleccione un rol --", ""));
        }

        private void MostrarUsuariosConRoles()
        {
            var usuariosConRoles = db.Users
                .OrderBy(u => u.UserName)
                .Select(user => new
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = user.Roles.Select(r => db.Roles.FirstOrDefault(role => role.Id == r.RoleId).Name)
                }).ToList();

            var datosFinales = usuariosConRoles.Select(u => new
            {
                u.UserId,
                u.UserName,
                u.Email,
                Roles = u.Roles.Any() ? string.Join(", ", u.Roles) : "Sin rol asignado"
            }).ToList();

            gvUsuariosRoles.DataSource = datosFinales;
            gvUsuariosRoles.DataBind();
        }

        private void MostrarTodosRoles()
        {
            gvRoles.DataSource = db.Roles.OrderBy(r => r.Name).ToList();
            gvRoles.DataBind();
        }

        protected void btnAsignarRol_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlUsuarios.SelectedValue) || string.IsNullOrEmpty(ddlRoles.SelectedValue))
            {
                MostrarMensaje("Por favor seleccione un usuario y un rol", false);
                return;
            }

            var userId = ddlUsuarios.SelectedValue;
            var roleId = ddlRoles.SelectedValue;
            var roleName = db.Roles.FirstOrDefault(r => r.Id == roleId)?.Name;

            if (userManager.IsInRole(userId, roleName))
            {
                MostrarMensaje("Este usuario ya tiene asignado este rol", false);
                return;
            }

            userManager.AddToRole(userId, roleName);
            MostrarMensaje($"Rol {roleName} asignado correctamente al usuario {ddlUsuarios.SelectedItem.Text}", true);
            MostrarUsuariosConRoles();
        }

        protected void btnAgregarRol_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNuevoRol.Text))
            {
                MostrarMensaje("Por favor ingrese un nombre para el nuevo rol", false);
                return;
            }

            var nombreRol = txtNuevoRol.Text.Trim();

            if (roleManager.RoleExists(nombreRol))
            {
                MostrarMensaje("Este rol ya existe en el sistema", false);
                return;
            }

            var resultado = roleManager.Create(new IdentityRole(nombreRol));

            if (resultado.Succeeded)
            {
                MostrarMensaje($"Rol {nombreRol} creado exitosamente", true);
                txtNuevoRol.Text = "";
                CargarRoles();
                CargarRolesEliminar();
                MostrarTodosRoles();
            }
            else
            {
                MostrarMensaje($"Error al crear rol: {string.Join(", ", resultado.Errors)}", false);
            }
        }

        protected void btnEliminarRol_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlRolesEliminar.SelectedValue))
            {
                MostrarMensaje("Por favor seleccione un rol para eliminar", false);
                return;
            }

            var roleId = ddlRolesEliminar.SelectedValue;
            var role = roleManager.FindById(roleId);

            if (role == null)
            {
                MostrarMensaje("El rol seleccionado no existe", false);
                return;
            }

            var usuariosEnRol = userManager.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId)).ToList();
            if (usuariosEnRol.Any())
            {
                MostrarMensaje("No se puede eliminar el rol porque hay usuarios asignados a él", false);
                return;
            }

            var resultado = roleManager.Delete(role);

            if (resultado.Succeeded)
            {
                MostrarMensaje($"Rol {role.Name} eliminado exitosamente", true);
                CargarRoles();
                CargarRolesEliminar();
                MostrarTodosRoles();
            }
            else
            {
                MostrarMensaje($"Error al eliminar rol: {string.Join(", ", resultado.Errors)}", false);
            }
        }

        private void MostrarMensaje(string mensaje, bool esExito)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esExito ? "alert alert-success" : "alert alert-danger";
            pnlMensaje.Visible = true;
        }
    }
}
