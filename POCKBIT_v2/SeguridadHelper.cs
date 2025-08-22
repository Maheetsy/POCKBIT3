using System.Web.UI;

namespace POCKBIT_v2
{
    public static class SeguridadHelper
    {
        public static void VerificarAutenticacion2FA(Page page)
        {
            if (!page.User.Identity.IsAuthenticated)
            {
                page.Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                bool twoFactorVerified = page.Session["TwoFactorVerified"] as bool? ?? false;

                if (!twoFactorVerified)
                {
                    page.Response.Redirect("~/Account/TwoFactorVerification.aspx");
                }
            }
        }
    }
}
