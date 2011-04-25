using System;

namespace PraticeManagement.Controls
{
	public partial class BillingInfo : System.Web.UI.UserControl
	{
		public DataTransferObjects.BillingInfo Info
		{
			get
			{
				var result = new DataTransferObjects.BillingInfo();
				PopulateData(result);

				return result;
			}
			set
			{
				PopulateControls(value);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		private void PopulateControls(DataTransferObjects.BillingInfo data)
		{
			if (data != null)
			{
				txtBillingContact.Text = data.BillingContact;
				txtBillingPhone.Text = data.BillingPhone;
				txtBillingEmail.Text = data.BillingEmail;
				txtBillingType.Text = data.BillingType;
				txtBillingAddress1.Text = data.BillingAddress1;
				txtBillingAddress2.Text = data.BillingAddress2;
				txtBillingCity.Text = data.BillingCity;
				txtBillingState.Text = data.BillingState;
				txtBillingZip.Text = data.BillingZip;
				txtPurchaseOrder.Text = data.PurchaseOrder;
			}
			else
			{
				txtBillingContact.Text = txtBillingPhone.Text = txtBillingEmail.Text = txtBillingType.Text =
					txtBillingAddress1.Text = txtBillingAddress2.Text = txtBillingCity.Text = txtBillingState.Text =
					txtBillingZip.Text = txtPurchaseOrder.Text = string.Empty;
			}
		}

		private void PopulateData(DataTransferObjects.BillingInfo data)
		{
			data.BillingContact = txtBillingContact.Text;
			data.BillingPhone = txtBillingPhone.Text;
			data.BillingEmail = txtBillingEmail.Text;
			data.BillingType = txtBillingType.Text;
			data.BillingAddress1 = txtBillingAddress1.Text;
			data.BillingAddress2 = txtBillingAddress2.Text;
			data.BillingCity = txtBillingCity.Text;
			data.BillingState = txtBillingState.Text;
			data.BillingZip = txtBillingZip.Text;
			data.PurchaseOrder = txtPurchaseOrder.Text;
		}
	}
}

