using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.OverheadService;
using System.Collections.Generic;
using DataTransferObjects;
using System.Linq;
namespace PraticeManagement.Controls
{
    public partial class PersonOverheadCalculationControl : System.Web.UI.UserControl
    {
        private const string MLFOverHeadDesc = "Minimum Load Factor (MLF)";

        #region Properties

        private PraticeManagement.Financial HostingPage
        {
            get { return ((PraticeManagement.Financial)Page); }
        }

        #endregion Properties

        protected void Page_Load(object sender, EventArgs e)
        {
            Display();
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

        protected void Display()
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

        protected void btnAddOverhead_Click(object sender, EventArgs e)
        {
            HostingPage.Redirect(Constants.ApplicationPages.OverheadDetail);
        }

        protected void gvOverhead_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == EditRecordCommand)
            {
                HostingPage.Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                    Constants.ApplicationPages.OverheadDetail,
                    e.CommandArgument));
            }
        }

        protected void chbActive_CheckedChanged(object sender, EventArgs e)
        {
            Display();
        }

        protected NameValuePair[] GetRateMultipliers()
        {
            var nameValuePair = new NameValuePair[51];
            nameValuePair[0] = new NameValuePair { Id=0, Name="N/A" };
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
            using (var serviceClient = new OverheadServiceClient())
            {
                serviceClient.SaveMinimumLoadFactorOverheadMultipliers(MLFOverHeadDesc,
                    !chbMLFActive.Checked,
                    decimal.Parse(ddlW2Hourly.SelectedValue),
                    decimal.Parse(ddlW2Salary.SelectedValue),
                    decimal.Parse(ddl1099.SelectedValue));
            }
        }
    }
}
