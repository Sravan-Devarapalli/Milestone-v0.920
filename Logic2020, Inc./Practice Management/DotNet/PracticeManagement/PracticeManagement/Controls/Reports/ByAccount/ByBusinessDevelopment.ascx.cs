using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports.ByAccount;

namespace PraticeManagement.Controls.Reports.ByAccount
{
    public partial class ByBusinessDevelopment : System.Web.UI.UserControl
    {
        #region Properties

        private const string Text_GroupByBusinessUnit = "Group by Business Unit";
        private const string Text_GroupByPerson = "Group by Person";

        private PraticeManagement.Reporting.AccountSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.AccountSummaryReport)Page); }
        }

      

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            btnExpandOrCollapseAll.Attributes["onclick"] = "return CollapseOrExpandAll(" + btnExpandOrCollapseAll.ClientID +
                                                           ", " + hdnCollapsed.ClientID +
                                                           ", " + hdncpeExtendersIds.ClientID +
                                                           ");";

            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = (hdnCollapsed.Value.ToLower() == "true") ? "Expand All" : "Collapse All";
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


        public void ApplyAttributes(int count)
        {
            btnExpandOrCollapseAll.Visible = btnExportToPDF.Enabled =
                       btnExportToExcel.Enabled = count > 0;
        }

        public void SetExpandCollapseIdsTohiddenField(string output)
        {
            hdncpeExtendersIds.Value = output;
            btnExpandOrCollapseAll.Text = btnExpandOrCollapseAll.ToolTip = "Expand All";
            hdnCollapsed.Value = "true";
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
