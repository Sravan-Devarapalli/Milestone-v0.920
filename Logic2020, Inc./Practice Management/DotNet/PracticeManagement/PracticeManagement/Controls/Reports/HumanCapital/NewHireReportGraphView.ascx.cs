using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports.HumanCapital
{
    public partial class NewHireGraphView : System.Web.UI.UserControl
    {
        private PraticeManagement.Reporting.NewHireReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.NewHireReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void PopulateGraph()
        {
            List<Person> data = ServiceCallers.Custom.Report(r => r.NewHireReport(HostingPage.StartDate.Value, HostingPage.EndDate.Value, HostingPage.PersonStatus, HostingPage.PersonStatus, HostingPage.Practices, HostingPage.ExcludeInternalProjects, null, null, null, null)).ToList();


        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            HostingPage.ExportToExcel();
        }
    }
}
