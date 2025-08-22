﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using POCKBIT_v2.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace POCKBIT_v2.Account
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["registro"] == "ok")
                {
                    SuccessMessage.Text = "✅ Registro exitoso. Ahora puedes iniciar sesión.";
                    SuccessMessage.Visible = true;
                }
            }
        }

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
            var user = new ApplicationUser() { UserName = Email.Text, Email = Email.Text };
            IdentityResult result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                // Inicia sesión automáticamente al usuario
                //signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);

                // Redirigir a la página de configuración de verificación de dos factores
                Response.Redirect("~/Account/Register.aspx?registro=ok");
            }
            else
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}
