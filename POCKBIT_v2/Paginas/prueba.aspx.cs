using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POCKBIT_v2.Paginas
{
    public partial class prueba : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TwoFactorVerified"] == null || !(bool)Session["TwoFactorVerified"])
            {
                Response.Redirect("~/Account/Login");
            }
        }

    }
}