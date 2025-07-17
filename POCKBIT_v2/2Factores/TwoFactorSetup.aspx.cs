using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Base32;
using OtpSharp;
using QRCoder;

namespace POCKBIT_v2._2Factores
{
    public partial class TwoFactorSetup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ Solo permitir acceso si el usuario está autenticado
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login");
                return;
            }

            if (!IsPostBack)
            {
                var user = User.Identity.Name;

                // ✅ Generar clave y código QR para el usuario
                var key = KeyGeneration.GenerateRandomKey(20);
                var secret = Base32Encoder.Encode(key);
                ViewState["Secret"] = secret;

                var barcodeUrl = $"otpauth://totp/{user}?secret={secret}&issuer=PockbitPharma";

                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(barcodeUrl, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new QRCode(qrCodeData))
                    {
                        var qrCodeImage = qrCode.GetGraphic(20);
                        using (var ms = new System.IO.MemoryStream())
                        {
                            qrCodeImage.Save(ms, ImageFormat.Png);
                            var base64Image = Convert.ToBase64String(ms.ToArray());
                            qrCodeImageControl.ImageUrl = "data:image/png;base64," + base64Image;
                        }
                    }
                }
            }
            else if (ViewState["Secret"] != null)
            {
                qrCodeImageControl.ImageUrl = "data:image/png;base64," + ViewState["Secret"].ToString();
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            var secret = ViewState["Secret"].ToString();
            var totp = new Totp(Base32Encoder.Decode(secret));
            var result = totp.VerifyTotp(txtCode.Text.Trim(), out long _, new VerificationWindow(2, 2));

            if (result)
            {
                var user = User.Identity.Name;
                SaveSecretToDatabase(user, secret);
                Session["TwoFactorVerified"] = true; // ✅ Marcar como verificado
                Response.Redirect("~/Paginas/dashboard");
            }
            else
            {
                Response.Write("<script>alert('Código incorrecto');</script>");
            }
        }

        private void SaveSecretToDatabase(string userName, string secret)
        {
            var connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "UPDATE AspNetUsers SET GoogleAuthenticatorSecret = @Secret WHERE UserName = @UserName";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Secret", secret);
                    command.Parameters.AddWithValue("@UserName", userName);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
