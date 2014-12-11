using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;
using System.Text;

namespace PraticeManagement.Reports
{
    public partial class CalendarReport : System.Web.UI.Page
    {
        public DateTime? StartDate
        {
            get
            {
                return diRange.FromDate.HasValue ? (DateTime?)diRange.FromDate.Value : null;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                return diRange.ToDate.HasValue ? (DateTime?)diRange.ToDate.Value : null;
            }
        }

        public string ProjectStatusIds
        {
            get
            {
                if (cblProjectStatus.Items.Count == 0)
                    return null;
                else
                {
                    var statusList = new StringBuilder();
                    foreach (ListItem item in cblProjectStatus.Items)
                        if (item.Selected)
                            statusList.Append(item.Value).Append(',');
                    return statusList.ToString();
                }
            }
        }

        public bool IncludeCompanyHolidays
        {
            get
            {
                return chbIncludeCompanyHolidays.Checked;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var excludeStatus = new List<int>();
                excludeStatus.Add((int)ProjectStatusType.Experimental);
                excludeStatus.Add((int)ProjectStatusType.Inactive);
                excludeStatus.Add((int)ProjectStatusType.Internal);
                DataHelper.FillProjectStatusList(cblProjectStatus, "All Project Statuses", excludeStatus);
                cblProjectStatus.SelectAll();
                SelectView();
            }
        }

        protected void btnUpdateView_Click(object sender, EventArgs e)
        {
            Page.Validate(valSum.ValidationGroup);
            if (Page.IsValid)
            {
                SelectView();
                lblRange.Text = StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
            }
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SwitchView((Control)sender, viewIndex);
        }

        private void SwitchView(Control control, int viewIndex)
        {
            SelectView(control, viewIndex);
            SelectView();
        }

        public void SelectView(Control sender, int viewIndex)
        {
            mvCalendarReport.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblProjectViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        private void SelectView()
        {
            if (StartDate.HasValue && EndDate.HasValue && (!string.IsNullOrEmpty(ProjectStatusIds)))
            {
                divWholePage.Style.Remove("display");
                LoadActiveView();
            }
            else
            {
                divWholePage.Style.Add("display", "none");
            }
        }

        private void LoadActiveView()
        {
            int activeView = mvCalendarReport.ActiveViewIndex;
            switch (activeView)
            {
                case 0: tpCalSummary.PopulateData();
                        break;
            }
        }
    }
}
