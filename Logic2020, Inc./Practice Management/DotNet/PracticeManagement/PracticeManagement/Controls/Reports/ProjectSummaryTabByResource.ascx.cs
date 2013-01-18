using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Text;
using System.Web.UI.HtmlControls;
using iTextSharp.text.pdf;
using PraticeManagement.Objects;
using System.IO;
using iTextSharp.text;
using PraticeManagement.Configuration;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectSummaryTabByResource : System.Web.UI.UserControl
    {
        private string ProjectSummaryByResourceExport = "Project Summary Report By Resource";
        private string ByPersonByResourceUrl = "PersonDetailTimeReport.aspx?StartDate={0}&EndDate={1}&PeriodSelected={2}&PersonId={3}";
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";

        #region Variables
        private HtmlImage ImgProjectRoleFilter { get; set; }

        public FilteredCheckBoxList cblProjectRolesControl
        {
            get
            {
                return cblProjectRoles;
            }
        }

        private PraticeManagement.Reporting.ProjectSummaryReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.ProjectSummaryReport)Page.Page); }
        }

        private PraticeManagement.Controls.Reports.ProjectSummaryByResource HostingControl
        {
            get { return (PraticeManagement.Controls.Reports.ProjectSummaryByResource)HostingPage.ByResourceControl; }
        }

        private Label LblProjectedHours { get; set; }

        private Label LblBillable { get; set; }

        private Label LblNonBillable { get; set; }

        private Label LblActualHours { get; set; }

        private Label LblBillableHoursVariance { get; set; }

        #endregion

        #region Methods

        public void DataBindByResourceSummary(PersonLevelGroupedHours[] reportData, bool isFirstTime)
        {
            if (isFirstTime)
            {
                PopulateProjectRoleFilter(reportData.ToList());
            }
            if (reportData.Count() > 0 || cblProjectRoles.Items.Count > 1)
            {
                divEmptyMessage.Style["display"] = "none";
                repResource.Visible = true;
                repResource.DataSource = reportData;
                repResource.DataBind();
                cblProjectRoles.SaveSelectedIndexesInViewState();
                ImgProjectRoleFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectRoles.FilterPopupClientID,
                  cblProjectRoles.SelectedIndexes, cblProjectRoles.CheckBoxListObject.ClientID, cblProjectRoles.WaterMarkTextBoxBehaviorID);

                //Populate header hover               
                PopulateHeaderHoverLabels(reportData.ToList());
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }
            btnExportToPDF.Enabled =
            btnExportToExcel.Enabled = reportData.Count() > 0;
        }

        private void PopulateHeaderHoverLabels(List<PersonLevelGroupedHours> reportData)
        {
            LblProjectedHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblProjectedHours.ClientID, pnlTotalProjectedHours.ClientID, 0);
            LblProjectedHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalProjectedHours.ClientID);

            LblBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblBillable.ClientID, pnlTotalBillableHours.ClientID, 0);
            LblBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalBillableHours.ClientID);

            LblNonBillable.Attributes[OnMouseOver] = string.Format(ShowPanel, LblNonBillable.ClientID, pnlTotalNonBillableHours.ClientID, 0);
            LblNonBillable.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalNonBillableHours.ClientID);

            LblActualHours.Attributes[OnMouseOver] = string.Format(ShowPanel, LblActualHours.ClientID, pnlTotalActualHours.ClientID, 0);
            LblActualHours.Attributes[OnMouseOut] = string.Format(HidePanel, pnlTotalActualHours.ClientID);

            LblBillableHoursVariance.Attributes[OnMouseOver] = string.Format(ShowPanel, LblBillableHoursVariance.ClientID, pnlBillableHoursVariance.ClientID, 0);
            LblBillableHoursVariance.Attributes[OnMouseOut] = string.Format(HidePanel, pnlBillableHoursVariance.ClientID);

            double totalBillableHoursVariance = reportData.Sum(p => p.BillableHoursVariance);
            lblTotalProjectedHours.Text = reportData.Sum(p => p.ForecastedHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHours.Text = lblTotalBillablePanlActual.Text = reportData.Sum(p => p.BillableHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalNonBillableHours.Text = lblTotalNonBillablePanlActual.Text = reportData.Sum(p => p.NonBillableHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalActualHours.Text = reportData.Sum(p => p.TotalHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHoursVariance.Text = totalBillableHoursVariance.ToString(Constants.Formatting.DoubleValue);
            if (totalBillableHoursVariance < 0)
            {
                lblExclamationMarkPanl.Visible = true;
            }
        }

        private void PopulateProjectRoleFilter(List<PersonLevelGroupedHours> reportData)
        {
            var projectRoles = reportData.Select(r => new { Text = string.IsNullOrEmpty(r.Person.ProjectRoleName) ? "Unassigned" : r.Person.ProjectRoleName, Value = r.Person.ProjectRoleName }).Distinct().ToList().OrderBy(s => s.Value);
            DataHelper.FillListDefault(cblProjectRoles.CheckBoxListObject, "All Project Roles", projectRoles.ToArray(), false, "Value", "Text");
            cblProjectRoles.SelectAllItems(true);
            cblProjectRoles.OKButtonId = btnUpdate.ClientID;
        }

        #endregion

        #region Control Methods

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgProjectRoleFilter = e.Item.FindControl("imgProjectRoleFilter") as HtmlImage;

                LblProjectedHours = e.Item.FindControl("lblProjectedHours") as Label;
                LblBillable = e.Item.FindControl("lblBillable") as Label;
                LblNonBillable = e.Item.FindControl("lblNonBillable") as Label;
                LblActualHours = e.Item.FindControl("lblActualHours") as Label;
                LblBillableHoursVariance = e.Item.FindControl("lblBillableHoursVariance") as Label;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (PersonLevelGroupedHours)e.Item.DataItem;
                var LnkActualHours = e.Item.FindControl("lnkActualHours") as LinkButton;
                LnkActualHours.Attributes["NavigationUrl"] = string.Format(ByPersonByResourceUrl, (HostingPage.StartDate.HasValue) ? HostingPage.StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) : null,
                    (HostingPage.EndDate.HasValue) ? HostingPage.EndDate.Value.Date.ToString(Constants.Formatting.EntryDateFormat) : null, (HostingPage.PeriodSelected != "*") ? HostingPage.PeriodSelected : "0", dataItem.Person.Id);
            }
        }

        public string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ProjectSummaryByResourceExport);

            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            List<PersonLevelGroupedHours> data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(HostingPage.ProjectNumber,
                HostingPage.MilestoneId, HostingPage.PeriodSelected == "*" ? null : HostingPage.StartDate, HostingPage.PeriodSelected == "*" ? null : HostingPage.EndDate, cblProjectRoles.SelectedItemsXmlFormat)).ToList();

            string filterApplied = "Filters applied to columns: ";
            bool isFilterApplied = false;
            if (!cblProjectRoles.AllItemsSelected)
            {
                filterApplied = filterApplied + " ProjectRoles.";
                isFilterApplied = true;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(project.Client.HtmlEncodedName);
            sb.Append("\t");
            sb.Append(project.Group.HtmlEncodedName);
            sb.Append("\t");
            sb.AppendLine();
            //P081003 - [ProjectName]
            sb.Append(string.Format("{0} - {1}", project.ProjectNumber, project.HtmlEncodedName));
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType);
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(HostingPage.ProjectRange);
            sb.Append("\t");
            if (isFilterApplied)
            {
                sb.AppendLine();
                sb.Append(filterApplied);
                sb.Append("\t");
            }
            sb.AppendLine();
            sb.AppendLine();

            if (data.Count > 0)
            {
                //Header
                sb.Append("Employee Id");
                sb.Append("\t");
                sb.Append("Resource");
                sb.Append("\t");
                sb.Append("Project Role");
                sb.Append("\t");
                sb.Append("Projected Hours");
                sb.Append("\t");
                sb.Append("Billable");
                sb.Append("\t");
                sb.Append("Non-Billable");
                sb.Append("\t");
                sb.Append("Actual Hours");
                sb.Append("\t");
                sb.Append("Billable Hours Variance");
                sb.Append("\t");
                sb.AppendLine();

                var list = data.OrderBy(p => p.Person.PersonLastFirstName);

                //Data
                foreach (var byPerson in list)
                {
                    sb.Append(byPerson.Person.EmployeeNumber);
                    sb.Append("\t");
                    sb.Append(byPerson.Person.HtmlEncodedName);
                    sb.Append("\t");
                    sb.Append(byPerson.Person.ProjectRoleName);
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.ForecastedHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.BillableHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.NonBillableHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.TotalHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.BillableHoursVariance));
                    sb.Append("\t");
                    sb.AppendLine();
                }
            }
            else
            {
                sb.Append("There are no Time Entries towards this project.");
            }

            var filename = string.Format("{0}_{1}_{2}.xls", project.ProjectNumber, project.Name, "_ByResourceSummary");
            filename = filename.Replace(' ', '_');
            GridViewExportUtil.Export(Utils.Generic.EncodedFileName(filename), sb);

        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(ProjectSummaryByResourceExport);
            HostingPage.PDFExport();
        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            HostingControl.PopulateByResourceSummaryReport();
        }

        #endregion

    }
}

