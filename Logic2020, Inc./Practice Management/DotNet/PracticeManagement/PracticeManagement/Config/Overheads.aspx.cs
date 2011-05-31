using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.OverheadService;
using System.ServiceModel;
namespace PraticeManagement.Config
{
    public partial class Overheads : PracticeManagementPageBase
    {
        private const string MLFOverHeadDesc = "Minimum Load Factor (MLF)";

        protected override void Display()
        {
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            DisplayContent();
            if (!IsPostBack)
            {
                var RateMultipliersItems = GetRateMultipliers();
                ddlW2Hourly.DataSource = RateMultipliersItems;
                ddlW2Hourly.DataBind();
                ddlW2Salary.DataSource = RateMultipliersItems;
                ddlW2Salary.DataBind();
                ddl1099.DataSource = RateMultipliersItems;
                ddl1099.DataBind();
                bool IsMLFInActive = false;
                Dictionary<int, decimal> MLFRateMultipliers;
                using (var serviceClient = new OverheadServiceClient())
                {
                    MLFRateMultipliers = serviceClient.GetMinimumLoadFactorOverheadMultipliers(MLFOverHeadDesc, ref IsMLFInActive);
                }
                if (MLFRateMultipliers != null)
                {
                    if (MLFRateMultipliers.Any(item => item.Key == (int)TimescaleType.Hourly))
                    {
                        ddlW2Hourly.SelectedValue = ((int)MLFRateMultipliers[(int)TimescaleType.Hourly]).ToString();
                    }
                    if (MLFRateMultipliers.Any(item => item.Key == (int)TimescaleType.Salary))
                    {
                        ddlW2Salary.SelectedValue = ((int)MLFRateMultipliers[(int)TimescaleType.Salary]).ToString();
                    }
                    if (MLFRateMultipliers.Any(item => item.Key == (int)TimescaleType._1099Ctc))
                    {
                        ddl1099.SelectedValue = ((int)MLFRateMultipliers[(int)TimescaleType._1099Ctc]).ToString();
                    }
                    chbMLFActive.Checked = !IsMLFInActive;
                }
            }
        }

        private const string EditRecordCommand = "EditRecord";

        protected void DisplayContent()
        {
            using (OverheadServiceClient serviceClient = new OverheadServiceClient())
            {
                try
                {
                    gvOverhead.DataSource = serviceClient.GetOverheadFixedRates(chbActive.Checked);
                    gvOverhead.DataBind();
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void lnkBtnViewMLFHistory_OnClick(object sender, EventArgs e)
        {
            using (OverheadServiceClient serviceClient = new OverheadServiceClient())
            {
                try
                {
                    gvMLFHistory.DataSource = serviceClient.GetOverheadHistory();
                    gvMLFHistory.DataBind();
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }

            mpeViewMLFHistory.Show();
        }

        protected void btnAddOverhead_Click(object sender, EventArgs e)
        {
            Redirect(Constants.ApplicationPages.OverheadDetail);
        }

        protected void gvOverhead_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == EditRecordCommand)
            {
                Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                    Constants.ApplicationPages.OverheadDetail,
                    e.CommandArgument));
            }
        }

        protected void chbActive_CheckedChanged(object sender, EventArgs e)
        {
            DisplayContent();
        }

        protected NameValuePair[] GetRateMultipliers()
        {
            var nameValuePair = new NameValuePair[51];
            nameValuePair[0] = new NameValuePair { Id = 0, Name = "N/A" };
            for (int index = 1; index <= 50; index++)
            {
                var nvPair = new NameValuePair();
                nvPair.Id = (index);
                nvPair.Name = (index).ToString() + " %";
                nameValuePair[index] = nvPair;
            }

            return nameValuePair;
        }

        protected void MLF_Changed(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;
            int timeScaleId;
            if(ddl.ID=="ddlW2Hourly")
            {
                timeScaleId= (int)TimescaleType.Hourly;
            }
            else if(ddl.ID=="ddlW2Salary")
            {
                timeScaleId = (int)TimescaleType.Salary;
            }
            else
            {
                timeScaleId = (int)TimescaleType._1099Ctc;
            }
            using (var serviceClient = new OverheadServiceClient())
            {
                serviceClient.UpdateMinimumLoadFactorHistory(timeScaleId,decimal.Parse(ddl.SelectedValue));
            }
        }

        protected void chbMLFActive_OnCheckedChanged(object sender, EventArgs e)
        {
            using (var serviceClient = new OverheadServiceClient())
            {
                serviceClient.UpdateMinimumLoadFactorStatus(!chbMLFActive.Checked);
            }
        }


       protected static string GetFormattedMLFRate(Decimal rate)
        {
            if (rate == 0M)
            {
                return "N/A";
            }
            else
            {
                return ((int)rate) + "%";
            }
        }
    }

}
