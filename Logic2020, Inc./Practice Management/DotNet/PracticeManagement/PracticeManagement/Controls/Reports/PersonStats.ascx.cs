using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports
{
    public partial class PersonStatsReport : ProjectsReportsBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {          
                      
        }

        public void DisplayPersonStatsReport(IEnumerable<PersonStats> stats)
        {
            for (var i = tblPersonStats.Rows[0].Cells.Count - 1; i > 0; i--)
                for (var j = 0; j < tblPersonStats.Rows.Count; j++)
                    tblPersonStats.Rows[j].Cells.RemoveAt(i);

            foreach (var t in stats)
            {
                tblPersonStats.Rows[0].Cells.Add(
                    new TableHeaderCell
                        {
                            Text = 
                                Resources.Controls.TableHeaderOpenTag +
                                t.Date.ToString(Constants.Formatting.CompPerfMonthYearFormat) + 
                                Resources.Controls.TableHeaderCloseTag, 
                            CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Center
                        });
                tblPersonStats.Rows[1].Cells.Add(new TableCell { Text = t.Revenue.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
                tblPersonStats.Rows[2].Cells.Add(new TableCell { Text = t.EmployeesCount.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
                tblPersonStats.Rows[3].Cells.Add(new TableCell { Text = t.ConsultantsCount.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
                tblPersonStats.Rows[4].Cells.Add(
                    new TableCell { Text = t.VirtualConsultants.ToString(Constants.Formatting.TwoDecimalsFormat), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
                tblPersonStats.Rows[5].Cells.Add(
                    new TableCell { Text = t.VirtualConsultantsChange.ToString(Constants.Formatting.TwoDecimalsFormat), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
                tblPersonStats.Rows[6].Cells.Add(new TableCell { Text = t.RevenuePerEmployee.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
                tblPersonStats.Rows[7].Cells.Add(new TableCell { Text = t.RevenuePerConsultant.ToString(), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });

                var adminCostAsPerctageOfRev =
                    t.Revenue != 0 ?
                                       (decimal)t.AdminCosts / (decimal)t.Revenue : 0M;
                tblPersonStats.Rows[8].Cells.Add(
                    new TableCell { Text = adminCostAsPerctageOfRev.ToString("P"), CssClass = CompPerfDataCssClass, HorizontalAlign = HorizontalAlign.Right });
            }
            foreach (TableRow row in tblPersonStats.Rows)
                foreach (TableCell cell in row.Cells)
                    cell.Visible = true;

            if (!Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName))
            {
                tblPersonStats.Rows[2].Visible = false;
                tblPersonStats.Rows[6].Visible = false;
                tblPersonStats.Rows[8].Visible = false;
            }
        }
    }
}
