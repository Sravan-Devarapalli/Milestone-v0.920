using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Configuration
{
    public partial class RecruitingMetricsHeader : System.Web.UI.UserControl
    {
        
        private void ApplyCssHeader()
        {
            if (Page is PraticeManagement.SourceRecruitingMetrics)
            {
                thSourceRecruitingMetrics.Attributes["class"] = "bgcolorE2EBFF";
                thTargetCompanyRecruitingMetrics.Attributes["class"] = "bgcolorwhite";
            }
            else if (Page is PraticeManagement.TargetedCompanyRecruitingMetrics)
            {
                thSourceRecruitingMetrics.Attributes["class"] = "bgcolorwhite";
                thTargetCompanyRecruitingMetrics.Attributes["class"] = "bgcolorE2EBFF";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ApplyCssHeader();
            }
        }
    }
}
