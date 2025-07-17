using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using POCKBIT_v2.Models;

namespace POCKBIT_v2.Account
{
    public partial class Login : Page
    {
        private const string DashboardUrl = "~/Paginas/dashboard";
        private const string TwoFactorSetupUrl = "~/2Factores/TwoFactorSetup.aspx";
        private const string TwoFactorVerifyUrl = "~/Account/TwoFactorVerification.aspx";
        private const string LockoutUrl = "~/Account/Lockout";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (OpenAuthLogin != null)
            {
                OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            }

            //var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            //if (!String.IsNullOrEmpty(returnUrl))
            //{
            //    // Registrar hiperlink si es necesario
            //}
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

                var result = signinManager.PasswordSignIn(Email.Text, Password.Text, RememberMe.Checked, shouldLockout: false);

                switch (result)
                {
                    case SignInStatus.Success:
                        HandleSuccessfulLogin(Email.Text);
                        break;

                    case SignInStatus.LockedOut:
                        Response.Redirect(LockoutUrl);
                        break;

                    case SignInStatus.RequiresVerification:
                        string returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
                        Response.Redirect($"{TwoFactorVerifyUrl}?ReturnUrl={returnUrl}&RememberMe={RememberMe.Checked}", true);
                        break;

                    case SignInStatus.Failure:
                    default:
                        FailureText.Text = "Intento de inicio de sesión no válido";
                        ErrorMessage.Visible = true;
                        break;
                }
            }
        }
        private void HandleSuccessfulLogin(string userEmail)
        {
            string secret = GetUser2FASecret(userEmail);

            if (string.IsNullOrEmpty(secret))
            {
                // Usuario aún no ha configurado 2FA
                Session["TwoFactorVerified"] = false;
                Response.Redirect(TwoFactorSetupUrl);
            }
            else
            {
                // Usuario ya tiene 2FA configurado, requiere verificación
                Session["TwoFactorVerified"] = false;
                Response.Redirect($"{TwoFactorVerifyUrl}?ReturnUrl={HttpUtility.UrlEncode(DashboardUrl)}");
            }
        }
        private string GetUser2FASecret(string userName)
        {
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("SELECT GoogleAuthenticatorSecret FROM AspNetUsers WHERE UserName = @UserName", connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    return result?.ToString();
                }
            }
            catch (Exception ex)
            {
                // Aquí podrías loguear el error si usas logging
                return null;
            }
        }
    }
}
