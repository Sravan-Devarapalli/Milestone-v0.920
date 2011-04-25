using System;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.MilestonePersonService;
using PraticeManagement.Security;

namespace PraticeManagement.Controls.Milestones
{
    public partial class MilestonePersonList : UserControl
    {
        private const int FirstMonthColumn = 5;
        private const string TotalHoursHeaderText = "Total Hours";
        private const int TotalRows = 10;

        private MilestonePerson[] _milestonePersons;
        private SeniorityAnalyzer _seniorityAnalyzer;

        private MilestonePerson[] MilestonePersons
        {
            get
            {
                if (SelectedMilestoneId.HasValue)
                {
                    using (var serviceClient = new MilestonePersonServiceClient())
                    {
                        try
                        {
                            return serviceClient.GetMilestonePersonListByMilestone(SelectedMilestoneId.Value);
                        }
                        catch (FaultException<ExceptionDetail>)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }

                return null;
            }
        }

        protected int? SelectedMilestoneId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Request.QueryString[Constants.QueryStringParameterNames.Id]);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                PopulatePeopleGrid();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (SelectedMilestoneId.HasValue)
            {
                _seniorityAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);
            }
        }

        protected string GetMpeRedirectUrl(object args)
        {
            var mpePageUrl =
                string.Format(
                    Constants.ApplicationPages.RedirectMilestonePersonIdFormat,
                    Constants.ApplicationPages.MilestonePersonDetail,
                    SelectedMilestoneId.Value,
                    args);

            return Utils.Generic.GetTargetUrlWithReturn(mpePageUrl, Request.Url.AbsoluteUri);
        }

        protected void gvPeople_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var milestonePerson = e.Row.DataItem as MilestonePerson;

            if (milestonePerson != null &&
                milestonePerson.Person != null && milestonePerson.Person.ProjectedFinancialsByMonth != null &&
                milestonePerson.Person.ProjectedFinancialsByMonth.Count > 0)
            {
                var isOtherGreater = _seniorityAnalyzer.IsOtherGreater(milestonePerson.Person);

                var dtTemp =
                    new DateTime(milestonePerson.Milestone.StartDate.Year, milestonePerson.Milestone.StartDate.Month, 1);
                // Filling monthly workload for the person.
                var currentColumnIndex = FirstMonthColumn;
                for (;
                    dtTemp <= milestonePerson.Milestone.ProjectedDeliveryDate;
                    currentColumnIndex++, dtTemp = dtTemp.AddMonths(1))
                {
                    // The person works on the milestone at the month - has some workload
                    foreach (var financials in
                        milestonePerson.Person.ProjectedFinancialsByMonth)
                    {
                        // Find a record for the month we need for the column
                        if (financials.Key.Month == dtTemp.Month && financials.Key.Year == dtTemp.Year)
                        {
                            e.Row.Cells[currentColumnIndex].Text =
                                string.Format(Resources.Controls.MilestoneInterestFormat,
                                              financials.Value.Revenue,
                                              isOtherGreater
                                                  ? Resources.Controls.HiddenCellText
                                                  : financials.Value.GrossMargin.ToString(),
                                              financials.Value.HoursBilled);
                        }
                    }
                }

                var marginColumnIndex = currentColumnIndex + 2;
                var grossMargin = currentColumnIndex + 4;

                if (isOtherGreater)
                {
                    e.Row.Cells[marginColumnIndex].Text = Resources.Controls.HiddenCellText;
                    e.Row.Cells[grossMargin].Text = Resources.Controls.HiddenCellText;
                }
            }
        }

        private void PopulatePeopleGrid()
        {
            _milestonePersons = MilestonePersons;

            if (_milestonePersons.Length > 0)
            {
                var dtTemp =
                    new DateTime(_milestonePersons[0].Milestone.StartDate.Year,
                                 _milestonePersons[0].Milestone.StartDate.Month, 1);

                // Removing columns
                while (gvPeople.Columns[FirstMonthColumn].HeaderText != TotalHoursHeaderText)
                {
                    gvPeople.Columns.RemoveAt(FirstMonthColumn);
                }

                // Create the columns for the milestone months
                for (var i = FirstMonthColumn;
                     dtTemp <= _milestonePersons[0].Milestone.ProjectedDeliveryDate;
                     i++, dtTemp = dtTemp.AddMonths(1))
                {
                    var column = new BoundField
                                     {
                                         HeaderText = Resources.Controls.TableHeaderOpenTag +
                                                      dtTemp.ToString(Constants.Formatting.MonthYearFormat) +
                                                      Resources.Controls.TableHeaderCloseTag,
                                         HtmlEncode = false
                                     };
                    gvPeople.Columns.Insert(i, column);
                }
            }

            gvPeople.DataSource = _milestonePersons;
            gvPeople.DataBind();

            if (gvPeople.FooterRow != null)
            {
                for (var i = 0; i < gvPeople.FooterRow.Cells.Count - 2; i++)
                {
                    gvPeople.FooterRow.Cells[i].RowSpan = TotalRows;
                }

                if (_milestonePersons != null && _milestonePersons.Length > 0)
                {
                    // Totals by months
                    var dtTemp =
                        new DateTime(_milestonePersons[0].Milestone.StartDate.Year,
                                     _milestonePersons[0].Milestone.StartDate.Month, 1);
                    var dtEnd = _milestonePersons[0].Milestone.ProjectedDeliveryDate;

                    var currentPerson = DataHelper.CurrentPerson;
                    for (var i = FirstMonthColumn; dtTemp <= dtEnd; i++, dtTemp = dtTemp.AddMonths(1))
                    {
                        var sa = new SeniorityAnalyzer(currentPerson);

                        var financials = new ComputedFinancials();

                        var oneGreaterExists = false;
                        foreach (var milestonePerson in _milestonePersons)
                        {
                            var isOtherGreater = sa.IsOtherGreater(milestonePerson.Person);
                            if (isOtherGreater)
                                oneGreaterExists = true;

                            foreach (var tmpFinancials in
                                milestonePerson.Person.ProjectedFinancialsByMonth)
                            {
                                // Serch for the computed financials for the month
                                if (tmpFinancials.Key.Month == dtTemp.Month &&
                                    tmpFinancials.Key.Year == dtTemp.Year)
                                {
                                    financials.Revenue += tmpFinancials.Value.Revenue;
                                    financials.GrossMargin += tmpFinancials.Value.GrossMargin;
                                    financials.HoursBilled += tmpFinancials.Value.HoursBilled;
                                    break;
                                }
                            }
                        }

                        if (financials.HoursBilled > 0)
                        {
                            gvPeople.FooterRow.Cells[i].Font.Bold = true;
                            gvPeople.FooterRow.Cells[i].Text =
                                string.Format(Resources.Controls.MilestoneSummaryInterestFormat,
                                              financials.Revenue,
                                              oneGreaterExists
                                                  ? Resources.Controls.HiddenCellText
                                                  : financials.GrossMargin.ToString(),
                                              financials.HoursBilled,
                                              oneGreaterExists
                                                  ? Resources.Controls.HiddenCellText
                                                  : financials.TargetMargin.ToString("##0.00"));
                        }
                    }
                }
            }
        }
    }
}
