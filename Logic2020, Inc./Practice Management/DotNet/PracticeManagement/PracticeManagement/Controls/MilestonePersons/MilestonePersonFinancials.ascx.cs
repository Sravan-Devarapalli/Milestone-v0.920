using System;
using System.Web.UI;
using DataTransferObjects.Financials;
using PraticeManagement.Security;

namespace PraticeManagement.Controls.MilestonePersons
{
    public partial class MilestonePersonFinancials : UserControl
    {
        public MilestonePersonComputedFinancials Financials
        {
            set { DisplayPersonRate(value); }
        }

        public SeniorityAnalyzer SeniorityAnalyzer { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void DisplayPersonRate(MilestonePersonComputedFinancials financials)
        {
            if (financials != null)
            {
                lblClientDiscount.Text = financials.ProjectDiscount.ToString(Constants.Formatting.DoubleFormat);
                lblGrossHourlyBillRate.Text = financials.GrossHourlyBillRate.ToString();
                lblLoadedHourlyPay.Text =
                    SeniorityAnalyzer.GreaterSeniorityExists
                        ? Resources.Controls.HiddenCellText
                        : financials.LoadedHourlyPayRate.ToString();
                lblHoursInPeriod.Text = financials.HoursInPeriod.ToString();
                lblProjectedRevenueContribution.Text = financials.Revenue.ToString();
                lblProjectedCogs.Text =
                    SeniorityAnalyzer.GreaterSeniorityExists
                        ? Resources.Controls.HiddenCellText
                        : financials.Cogs.ToString();
                lblProjectedMarginContribution.Text =
                    SeniorityAnalyzer.GreaterSeniorityExists
                        ? Resources.Controls.HiddenCellText
                        : financials.Margin.ToString();
            }
            else
            {
                lblGrossHourlyBillRate.Text = lblClientDiscount.Text = 
                                              lblLoadedHourlyPay.Text =
                                              lblProjectedRevenueContribution.Text =
                                              lblProjectedMarginContribution.Text =
                                              lblProjectedCogs.Text = lblHoursInPeriod.Text =
                                                                      lblProjectedRevenueContribution.Text =
                                                                      lblProjectedMarginContribution.Text =
                                                                      Resources.Controls.UnavailableLabel;
            }
        }
    }
}
