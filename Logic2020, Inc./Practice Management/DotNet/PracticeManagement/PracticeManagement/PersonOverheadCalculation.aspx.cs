using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.OverheadService;

namespace PraticeManagement
{
    public partial class PersonOverheadCalculation : PracticeManagementPageBase
    {
		private const string EditRecordCommand = "EditRecord";

        protected override void Display()
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
			Display();
		}
	}
}

