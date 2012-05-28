using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByBusinessDevelopment : System.Web.UI.UserControl
    {
        #region Properties

        private const string Text_GroupByBusinessUnit = "Group By Business Unit";
        private const string Text_GroupByPerson = "Group By Person";
        
        private PraticeManagement.Reporting.AccountSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.AccountSummaryReport)Page); }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnGroupBy_Click(object sender, EventArgs e)
        {
            PopulateByBusinessDevelopment();
        }

        private void PopulateGroupByBusinessUnit()
        {
            tpByBusinessUnit.PopulateData(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value);
        }

        private void PopulateGroupByPerson()
        {
            tpByPerson.PopulateData(HostingPage.AccountId, HostingPage.BusinessUnitIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value);
        }

        public void PopulateByBusinessDevelopment()
        {
            if (btnGroupBy.Text == Text_GroupByPerson)
            {
                btnGroupBy.Text = Text_GroupByBusinessUnit;
                btnGroupBy.ToolTip = Text_GroupByBusinessUnit;
                mvBusinessDevelopmentReport.ActiveViewIndex = 1;
                PopulateGroupByBusinessUnit();
            }
            else
            {
                mvBusinessDevelopmentReport.ActiveViewIndex = 0;
                btnGroupBy.Text = Text_GroupByPerson;
                btnGroupBy.ToolTip = Text_GroupByPerson;
                PopulateGroupByPerson();
            }
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }
    }
}
