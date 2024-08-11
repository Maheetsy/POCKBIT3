using Base32;
using OtpSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POCKBIT_v2.Account
{
    public partial class TwoFactorVerification : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            var user = User.Identity.Name;
            var secret = GetSecretFromDatabase(user);

            if (secret != null)
            {
                var totp = new Totp(Base32Encoder.Decode(secret));
                var result = totp.VerifyTotp(txtCode.Text.Trim(), out long timeStepMatched, new VerificationWindow(2, 2));

                if (result)
                {
                    Session["TwoFactorVerified"] = true;
                    Response.Redirect("~/Paginas/dashboard"); // Redirigir al usuario a la página principal
                }
                else
                {
                    Response.Write("<script>alert('Código incorrecto');</script>");
                }
            }
        }

        private string GetSecretFromDatabase(string userName)
        {
            var connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT GoogleAuthenticatorSecret FROM AspNetUsers WHERE UserName = @UserName";
                using (var command = new SqlCommand(query, connection))
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