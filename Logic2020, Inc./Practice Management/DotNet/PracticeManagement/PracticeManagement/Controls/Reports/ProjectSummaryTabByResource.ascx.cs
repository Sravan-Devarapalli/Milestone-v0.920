﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Text;
using System.Web.UI.HtmlControls;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectSummaryTabByResource : System.Web.UI.UserControl
    {
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

        protected void repResource_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                ImgProjectRoleFilter = e.Item.FindControl("imgProjectRoleFilter") as HtmlImage;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

            }
        }

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
                ImgProjectRoleFilter.Attributes["onclick"] = string.Format("Filter_Click({0},\'{1}\',\'{2}\',{3});", cblProjectRoles.FilterPopupId,
                    cblProjectRoles.SelectedIndexes, cblProjectRoles.ClientID, cblProjectRoles.SearchTextBoxId);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repResource.Visible = false;
            }
        }

        private void PopulateProjectRoleFilter(List<PersonLevelGroupedHours> reportData)
        {
            var projectRoles = reportData.Select(r => new { Name = r.Person.ProjectRoleName }).Distinct().ToList().OrderBy(s => s.Name);
            DataHelper.FillListDefault(cblProjectRoles.CheckBoxListObject, "All ProjectRoles", projectRoles.ToArray(), false, "Name", "Name");
            cblProjectRoles.SelectAllItems(true);
            cblProjectRoles.OKButtonId = btnUpdate.ClientID;
        }

        public string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {

            var project = ServiceCallers.Custom.Project(p => p.GetProjectShortByProjectNumber(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.StartDate, HostingPage.EndDate));
            List<PersonLevelGroupedHours> data = ServiceCallers.Custom.Report(r => r.ProjectSummaryReportByResource(HostingPage.ProjectNumber, HostingPage.MilestoneId, HostingPage.PeriodSelected == "0" ? HostingPage.StartDate : null, HostingPage.PeriodSelected == "0" ? HostingPage.EndDate : null, cblProjectRoles.SelectedItemsXmlFormat)).ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append(project.Client.Name);
            sb.Append("\t");
            sb.Append(project.Group.Name);
            sb.Append("\t");
            sb.AppendLine();
            //P081003 - [ProjectName]
            sb.Append(string.Format("{0} - {1}", project.ProjectNumber, project.Name));
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(string.IsNullOrEmpty(project.BillableType) ? project.Status.Name : project.Status.Name + ", " + project.BillableType);
            sb.Append("\t");
            sb.AppendLine();
            sb.Append(HostingPage.ProjectRange);
            sb.Append("\t");
            sb.AppendLine();
            sb.AppendLine();

            if (data.Count > 0)
            {
                //Header
                sb.Append("Resource");
                sb.Append("\t");
                sb.Append("Project Role");
                sb.Append("\t");
                sb.Append("Billable");
                sb.Append("\t");
                sb.Append("Non-Billable");
                sb.Append("\t");
                sb.Append("Total");
                sb.Append("\t");
                sb.Append("Person Variance (in Hours)");
                sb.Append("\t");
                sb.AppendLine();

                var list = data.OrderBy(p => p.Person.PersonLastFirstName);

                //Data
                foreach (var byPerson in list)
                {
                    sb.Append(byPerson.Person.PersonLastFirstName);
                    sb.Append("\t");
                    sb.Append(byPerson.Person.ProjectRoleName);
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.BillableHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.NonBillableHours));
                    sb.Append("\t");
                    sb.Append(GetDoubleFormat(byPerson.TotalHours));
                    sb.Append("\t");
                    sb.Append(byPerson.Variance);
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
            GridViewExportUtil.Export(filename, sb);

        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            HostingControl.PopulateByResourceSummaryReport();
        }

    }
}

