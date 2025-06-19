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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (OpenAuthLogin != null)
            {
                OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            }

            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                // Registrar hiperlink si es necesario
            }
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
                        string userEmail = Email.Text;
                        string secret = GetUser2FASecret(userEmail);

                        if (string.IsNullOrEmpty(secret))
                        {
                            // Si no tiene el secreto, lo redirigimos al Setup
                            //Session["TwoFactorVerified"] = false;
                            //Response.Redirect("~/2Factores/TwoFactorSetup.aspx");
                            // Desactivando la verificación 2FA
                            Session["TwoFactorVerified"] = true;
                            Response.Redirect("~/Paginas/dashboard");

                        }
                        else
                        {
                            // Si ya tiene secreto, ir a verificación
                            //Session["TwoFactorVerified"] = false;
                            //Response.Redirect("~/Account/TwoFactorVerification.aspx");
                        }
                        break;

                    case SignInStatus.LockedOut:
                        Response.Redirect("/Account/Lockout");
                        break;

                    case SignInStatus.RequiresVerification:
                        Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}",
                                                        Request.QueryString["ReturnUrl"],
                                                        RememberMe.Checked), true);
                        break;

                    case SignInStatus.Failure:
                    default:
                        FailureText.Text = "Intento de inicio de sesión no válido";
                        ErrorMessage.Visible = true;
                        break;
                }
            }
        }

        private string GetUser2FASecret(string userName)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT GoogleAuthenticatorSecret FROM AspNetUsers WHERE UserName = @UserName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    connection.Open();

                    var result = command.ExecuteScalar();
                    return result != null ? result.ToString() : null;
                }
            }
        }
    }
}
