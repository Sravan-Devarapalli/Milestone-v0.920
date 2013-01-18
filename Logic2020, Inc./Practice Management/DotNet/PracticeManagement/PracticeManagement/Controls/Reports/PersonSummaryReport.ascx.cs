using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.Reports;
using System.Text;

namespace PraticeManagement.Controls.Reports
{
    public partial class PersonSummaryReport : System.Web.UI.UserControl
    {
        private string PersonSummaryReportExport = "Person Summary Report";
        private string ShowPanel = "ShowPanel('{0}', '{1}','{2}');";
        private string HidePanel = "HidePanel('{0}');";
        private string OnMouseOver = "onmouseover";
        private string OnMouseOut = "onmouseout";

        private Label LblProjectedHours { get; set; }

        private Label LblBillable { get; set; }

        private Label LblNonBillable { get; set; }

        private Label LblActualHours { get; set; }

        private Label LblBillableHoursVariance { get; set; }


        private PraticeManagement.Reporting.PersonDetailTimeReport HostingPage
        {
            get { return ((PraticeManagement.Reporting.PersonDetailTimeReport)Page); }
        }

        protected void repSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                LblProjectedHours = e.Item.FindControl("lblProjectedHours") as Label;
                LblBillable = e.Item.FindControl("lblBillable") as Label;
                LblNonBillable = e.Item.FindControl("lblNonBillable") as Label;
                LblActualHours = e.Item.FindControl("lblActualHours") as Label;
                LblBillableHoursVariance = e.Item.FindControl("lblBillableHoursVariance") as Label;
            }
        }

        public void DatabindRepepeaterSummary(List<TimeEntriesGroupByClientAndProject> timeEntriesGroupByClientAndProjectList)
        {
            if (timeEntriesGroupByClientAndProjectList.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repSummary.Visible = true;
                repSummary.DataSource = timeEntriesGroupByClientAndProjectList;
                repSummary.DataBind();
                PopulateHeaderHoverLabels(timeEntriesGroupByClientAndProjectList.ToList());
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repSummary.Visible = false;
            }
        }

        private void PopulateHeaderHoverLabels(List<TimeEntriesGroupByClientAndProject> reportData)
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

            lblTotalProjectedHours.Text = reportData.Sum(p => p.ProjectedHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHours.Text = lblTotalBillablePanlActual.Text = reportData.Sum(p => p.BillableHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalNonBillableHours.Text = lblTotalNonBillablePanlActual.Text = reportData.Sum(p => p.NonBillableHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalActualHours.Text = reportData.Sum(p => p.TotalHours).ToString(Constants.Formatting.DoubleValue);
            lblTotalBillableHoursVariance.Text = totalBillableHoursVariance.ToString(Constants.Formatting.DoubleValue);
            if (totalBillableHoursVariance < 0)
            {
                lblExclamationMarkPanl.Visible = true;
            }
        }

        protected string GetDoubleFormat(double value)
        {
            return value.ToString(Constants.Formatting.DoubleValue);
        }

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(PersonSummaryReportExport);

            // mso-number-format:"0\.00"
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var timeEntriesGroupByClientAndProjectList = ServiceCallers.Custom.Report(r => r.PersonTimeEntriesSummary(HostingPage.SelectedPersonId, HostingPage.StartDate.Value, HostingPage.EndDate.Value)).ToList();

                int personId = HostingPage.SelectedPersonId;
                var person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));
                string personType = person.IsOffshore ? "Offshore" : string.Empty;
                string payType = person.CurrentPay.TimescaleName;
                string personStatusAndType = string.IsNullOrEmpty(personType) && string.IsNullOrEmpty(payType) ? string.Empty :
                                                                                 string.IsNullOrEmpty(payType) ? personType :
                                                                                 string.IsNullOrEmpty(personType) ? payType :
                                                                                                                     payType + ", " + personType;
                StringBuilder sb = new StringBuilder();
                sb.Append(person.EmployeeNumber + " - " + person.FirstName + " " + person.LastName);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(personStatusAndType);
                sb.Append("\t");
                sb.AppendLine();
                sb.Append(HostingPage.Range);
                sb.Append("\t");
                sb.AppendLine();
                sb.AppendLine();
                if (timeEntriesGroupByClientAndProjectList.Count > 0)
                {
                    //Header
                    sb.Append("Account");
                    sb.Append("\t");
                    sb.Append("Account Name");
                    sb.Append("\t");
                    sb.Append("Business Unit");
                    sb.Append("\t");
                    sb.Append("Business Unit Name");
                    sb.Append("\t");
                    sb.Append("Project");
                    sb.Append("\t");
                    sb.Append("Project Name");
                    sb.Append("\t");
                    sb.Append("Status");
                    sb.Append("\t");
                    sb.Append("Billing");
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
                    sb.Append("Percent of Total Hours this Period");

                    sb.Append("\t");
                    sb.AppendLine();

                    //Data
                    foreach (var timeEntriesGroupByClientAndProject in timeEntriesGroupByClientAndProjectList)
                    {
                        sb.Append(timeEntriesGroupByClientAndProject.Client.Code);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Client.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.Group.Code);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.Group.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.ProjectNumber);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.HtmlEncodedName);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.Project.Status.Name);
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.BillableType);
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(timeEntriesGroupByClientAndProject.ProjectedHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(timeEntriesGroupByClientAndProject.BillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(timeEntriesGroupByClientAndProject.NonBillableHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(timeEntriesGroupByClientAndProject.TotalHours));
                        sb.Append("\t");
                        sb.Append(GetDoubleFormat(timeEntriesGroupByClientAndProject.BillableHoursVariance));
                        sb.Append("\t");
                        sb.Append(timeEntriesGroupByClientAndProject.ProjectTotalHoursPercent + "%");
                        sb.Append("\t");
                        sb.AppendLine();
                    }

                }
                else
                {
                    sb.Append("This person has not entered Time Entries for the selected period.");
                }
                //“[LastName]_[FirstName]-[“Summary” or “Detail”]-[StartOfRange]_[EndOfRange].xls”.  
                //example :Hong-Turney_Jason-Summary-03.01.2012_03.31.2012.xlsx
                var filename = string.Format("{0}_{1}_{2}_{3}_{4}.xls", person.LastName, person.FirstName, "Summary", HostingPage.StartDate.Value.ToString("MM.dd.yyyy"), HostingPage.EndDate.Value.ToString("MM.dd.yyyy"));
                GridViewExportUtil.Export(filename, sb);
            }
        }

        protected void btnExportToPDF_OnClick(object sender, EventArgs e)
        {

        }

    }
}

