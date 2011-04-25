using System;
using System.Globalization;
using System.Web.UI;
using DataTransferObjects;
using PraticeManagement.ProjectService;
using PraticeManagement.Security;

namespace PraticeManagement.Controls.Projects
{
    public partial class ProjectFinancials : UserControl
    {
        #region Constants

        private const string BenchCssClass = "Bench";
        private const string MarginCssClass = "Margin";

        #endregion

        #region Fields

        private SeniorityAnalyzer _milestonesSeniorityAnalyzer;
        private SeniorityAnalyzer _peopleSeniorityAnalyzer;
        private ComputedFinancials _financials;

        #endregion

        public Project Project { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitSeniorityAnalyzers();
                DisplayInterestValues(Project);
            }
        }

        private void InitSeniorityAnalyzers()
        {
            var currentPerson = DataHelper.CurrentPerson;
            _peopleSeniorityAnalyzer = new SeniorityAnalyzer(currentPerson);
            _milestonesSeniorityAnalyzer = new SeniorityAnalyzer(currentPerson);
            if (Project != null)
            {
                foreach (var milestone in Project.Milestones)
                {
                    if (_milestonesSeniorityAnalyzer.OneWithGreaterSeniorityExists(
                                DataHelper.GetPersonsInMilestone(milestone)))
                    {
                        break;
                    }
                }
            }
        }

        private void DisplayInterestValues(Project project)
        {
            // Interest values            
            if (Financials != null)
            {
                var hide =
                    _milestonesSeniorityAnalyzer.GreaterSeniorityExists ||
                    _peopleSeniorityAnalyzer.GreaterSeniorityExists;
                var ht = Resources.Controls.HiddenCellText;

                var nfi = new NumberFormatInfo {CurrencyDecimalDigits = 0, CurrencySymbol = "$"};

                lblEstimatedRevenue.Text = String.Format(nfi, "{0:c}", Financials.Revenue.Value);
                lblDiscount.Text = String.Format(nfi, "{0}", project.Discount);
                lblDiscountAmount.Text =
                    String.Format(nfi, "{0:c}", (Financials.Revenue.Value - Financials.RevenueNet.Value));
                lblRevenueNet.Text = String.Format(nfi, "{0:c}", Financials.RevenueNet.Value);
                lblEstimatedCogs.Text = hide ? ht : String.Format(nfi, "{0:c}", Financials.Cogs.Value);
                lblExpenses.Text = hide ? ht : String.Format(nfi, "{0:c}", Financials.Expenses);
                lblReimbursedExpenses.Text = hide ? ht : String.Format(nfi, "{0:c}", Financials.ReimbursedExpenses);
                lblGrossMargin.Text = hide ? ht : String.Format(nfi, "{0:c}", Financials.GrossMargin.Value);
                lblGrossMargin.CssClass =
                    Financials.GrossMargin.Value < 0 ? BenchCssClass : MarginCssClass;
                lblTargetMargin.Text = hide
                                           ? ht
                                           : string.Format(Constants.Formatting.PercentageFormat,
                                                           Financials.TargetMargin);
                lblSalesCommission.Text =
                    hide ? ht : String.Format(nfi, "{0:c}", Financials.SalesCommission.Value);
                lblEstimatedMargin.Text =
                    hide ? ht : String.Format(nfi, "{0:c}", Financials.MarginNet.Value);
                lblEstimatedMargin.CssClass =
                    Financials.MarginNet.Value < 0 ? BenchCssClass : MarginCssClass;
                lblPracticeManagerCommission.Text = hide
                                                        ? ht
                                                        : String.Format(nfi, "{0:c}",
                                                                        Financials.PracticeManagementCommission.Value);
            }
        }

        protected ComputedFinancials Financials
        {
            get
            {
                return _financials ?? (_financials = Project != null && Project.Id.HasValue
                                                         ? ServiceCallers.Invoke
                                                               <ProjectServiceClient, ComputedFinancials>(
                                                                   client =>
                                                                   client.GetProjectsComputedFinancials(Project.Id.Value))
                                                         : null);
            }
        }
    }
}

