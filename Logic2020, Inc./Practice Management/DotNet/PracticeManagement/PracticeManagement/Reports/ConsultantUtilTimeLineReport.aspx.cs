using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Reporting
{
    public partial class ConsultantUtilTimeLineReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int refreshValue = 3600;
            int result;

            if (Request.QueryString["Refresh"] != null)
            {                
                if (int.TryParse(Request.QueryString["Refresh"].ToString() ,out result))
                {
                    refreshValue = result * 60;
                }                
            }

            this.Response.AppendHeader("Refresh", refreshValue.ToString());

        }
    }
}
