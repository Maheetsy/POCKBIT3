using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using POCKBIT_v2.Models;

namespace POCKBIT_v2.Account
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TwoFactorVerified"] == null || !(bool)Session["TwoFactorVerified"])
            {
                Response.Redirect("~/Account/Login");
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
                signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);

                // Redirigir a la página de configuración de verificación de dos factores
                Response.Redirect("~/Account/TwoFactorSetup.aspx");
            }
            else
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}
