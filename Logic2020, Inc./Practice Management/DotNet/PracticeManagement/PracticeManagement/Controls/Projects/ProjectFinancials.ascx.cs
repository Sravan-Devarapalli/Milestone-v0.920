﻿using System;
using System.Globalization;
using System.Web.UI;
using DataTransferObjects;
using PraticeManagement.ProjectService;
using PraticeManagement.Security;
using System.Web.UI.MobileControls;
using System.Collections.Generic;
using PraticeManagement.Utils;

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
        private ComputedFinancials _financials;

        #endregion

        public Project Project { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            InitSeniorityAnalyzers();
            DisplayInterestValues(Project);

        }

        private void InitSeniorityAnalyzers()
        {
            var currentPerson = DataHelper.CurrentPerson;
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
                var hide =_milestonesSeniorityAnalyzer.GreaterSeniorityExists;
                var ht = Resources.Controls.HiddenCellText;

                var nfi = new NumberFormatInfo { CurrencyDecimalDigits = 0, CurrencySymbol = "$" };
                if (Financials.Revenue.Value >= 0)
                {
                    lblEstimatedRevenue.Text = String.Format(nfi, "{0:c}", Financials.Revenue.Value);
                }
                else
                {
                    lblEstimatedRevenue.Text = Financials.Revenue.ToString();
                }
                lblDiscount.Text = String.Format(nfi, "{0}", project.Discount);
                var discountAmt = (Financials.Revenue.Value - Financials.RevenueNet.Value);
                lblDiscountAmount.Text = discountAmt>=0?
                    String.Format(nfi, "{0:c}", discountAmt) : ((PracticeManagementCurrency)discountAmt).ToString();
                lblRevenueNet.Text = Financials.RevenueNet.Value>=0?
                    (String.Format(nfi, "{0:c}", Financials.RevenueNet.Value)):
                    Financials.RevenueNet.ToString();
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

                if (project.Client.Id.HasValue)
                {
                    SetBackgroundColorForMargin(project.Client.Id.Value, project.Client.IsMarginColorInfoEnabled);
                }

                lblSalesCommission.Text =
                    hide ? ht : 
                    (Financials.SalesCommission.Value>=0?
                    String.Format(nfi, "{0:c}", Financials.SalesCommission.Value) : Financials.SalesCommission.ToString());
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

        private void SetBackgroundColorForMargin(int clientId, bool? individualClientMarginColorInfoEnabled)
        {

            int margin = (int)Financials.TargetMargin;
            List<ClientMarginColorInfo> cmciList = new List<ClientMarginColorInfo>();

            if (individualClientMarginColorInfoEnabled.HasValue && individualClientMarginColorInfoEnabled.Value)
            {
                cmciList = DataHelper.GetClientMarginColorInfo(clientId);
            }
            else if (Convert.ToBoolean(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllClientsKey)))
            {
                cmciList = SettingsHelper.GetMarginColorInfoDefaults(DefaultGoalType.Client);
            }

            if (cmciList != null)
            {
                foreach (var item in cmciList)
                {
                    if (margin >= item.StartRange && margin <= item.EndRange)
                    {
                        tdTargetMargin.Style["background-color"] = item.ColorInfo.ColorValue;
                        break;
                    }
                }
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

