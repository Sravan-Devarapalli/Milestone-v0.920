using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.OverheadService;

namespace PraticeManagement
{
    public partial class OverheadDetail : PracticeManagementPageBase, IPostBackEventHandler
	{
        private int? OverheadId
        {
            get 
            {
                if (SelectedId.HasValue)
                {
                    return SelectedId;
                }
                else
                {
                    int id;
                    if (Int32.TryParse(hdnOverheadId.Value, out id))
                    {
                        return id;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                hdnOverheadId.Value = value.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                txtOverheadName.Focus();
            mlConfirmation.ClearMessage();
        }

		protected override void Display()
		{
			DataHelper.FillOverheadRateTypeList(ddlBasis, string.Empty);

			int? id = OverheadId;
			if (id.HasValue)
			{
				OverheadFixedRate overhead = null;

                overhead = GetOverhead(id);

				if (overhead != null)
				{
					PopulateControls(overhead);
				}
			}
		}

        private static OverheadFixedRate GetOverhead(int? id)
        {
            using (OverheadServiceClient serviceClient = new OverheadServiceClient())
            {
                try
                {
                    return serviceClient.GetOverheadFixedRateDetail(id.Value);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

		protected void ddlBasis_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateCostMultiplierSign();
		}

		protected void Period_SelectionChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		protected void rlstCogsExpense_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			compPeriodStartEnd.Enabled = dpEndDate.DateValue > DateTime.MinValue;
			Page.Validate();
			if (Page.IsValid)
			{
				SaveData();
                ClearDirty();
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Overhead"));
			}

            if (!SelectedId.HasValue && OverheadId.HasValue)
            {
                var Overhead = GetOverhead(OverheadId);
                if (Overhead != null)
                {
                    PopulateControls(Overhead);
                }
            }
		}

		protected override bool ValidateAndSave()
		{
			bool result = false;
			compPeriodStartEnd.Enabled = dpEndDate.DateValue > DateTime.MinValue;
			Page.Validate();
			if (Page.IsValid)
			{
				SaveData();
				result = true;
			}

			return result;
		}

		private void PopulateControls(OverheadFixedRate overhead)
		{
			chbActive.Checked = !overhead.Inactive;
			txtOverheadName.Text = overhead.Description;
			ddlBasis.SelectedIndex =
				ddlBasis.Items.IndexOf(ddlBasis.Items.FindByValue(
				overhead.RateType != null ? overhead.RateType.Id.ToString() : string.Empty));
			txtRate.Text = overhead.Rate.Value.ToString();
			dpStartDate.DateValue = overhead.StartDate;
			dpEndDate.DateValue = overhead.EndDate.HasValue ? overhead.EndDate.Value : DateTime.MinValue;
			rlstCogsExpense.SelectedIndex =
				rlstCogsExpense.Items.IndexOf(rlstCogsExpense.Items.FindByValue(overhead.IsCogs.ToString()));

			if (overhead.Timescales != null)
			{
				chbHourly.Checked = overhead.Timescales[TimescaleType.Hourly];
				chbSalary.Checked = overhead.Timescales[TimescaleType.Salary];
				chb1099.Checked = overhead.Timescales[TimescaleType._1099Ctc];
			}

			UpdateCostMultiplierSign();
		}

		private void SaveData()
		{
			OverheadFixedRate overhead = new OverheadFixedRate();
			PopulateData(overhead);

			using (OverheadServiceClient serviceClient = new OverheadServiceClient())
			{
				try
				{
					int? id = serviceClient.SaveOverheadFixedRateDetail(overhead);

                    if (id.HasValue)
                    {
                        OverheadId = id;
                    }
				}
				catch (FaultException<ExceptionDetail>)
				{
					serviceClient.Abort();
					throw;
				}
			}
		}

		private void PopulateData(OverheadFixedRate overhead)
		{
			overhead.Id = OverheadId;
			overhead.Inactive = !chbActive.Checked;
			overhead.Description = txtOverheadName.Text;
			overhead.Rate = decimal.Parse(txtRate.Text);
			overhead.StartDate = dpStartDate.DateValue;
			overhead.EndDate = dpEndDate.DateValue != DateTime.MinValue ? (DateTime?)dpEndDate.DateValue : null;
			overhead.IsCogs = bool.Parse(rlstCogsExpense.SelectedValue);

			// Overhead rate type
			overhead.RateType = new OverheadRateType();
			overhead.RateType.Id = int.Parse(ddlBasis.SelectedValue);

			// Overhead timescale applicability
			if (overhead.Timescales == null)
			{
				overhead.Timescales = new Dictionary<TimescaleType, bool>();
			}
			overhead.Timescales[TimescaleType.Hourly] = chbHourly.Checked;
			overhead.Timescales[TimescaleType.Salary] = chbSalary.Checked;
			overhead.Timescales[TimescaleType._1099Ctc] = chb1099.Checked;
		}

		/// <summary>
		/// Retrieves an info on the selected Bases and updates the view.
		/// </summary>
		private void UpdateCostMultiplierSign()
		{
			OverheadRateType overheadType = null;

			if (!string.IsNullOrEmpty(ddlBasis.SelectedValue))
			{
				using (OverheadServiceClient serviceClient = new OverheadServiceClient())
				{
					try
					{
						int overheadTypeId = int.Parse(ddlBasis.SelectedValue);
						overheadType = serviceClient.GetRateTypeDetail(overheadTypeId);
					}
					catch (FaultException<ExceptionDetail>)
					{
						serviceClient.Abort();
						throw;
					}
				}
			}

			if (overheadType == null)
			{
				lblCurrencySign.Visible = false;
				lblPersantageSign.Visible = false;
			}
			else
			{
				lblCurrencySign.Visible = !overheadType.IsPercentage;
				lblPersantageSign.Visible = overheadType.IsPercentage;
			}
		}

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            bool result = ValidateAndSave();
            if (result)
            {

                var query = Request.QueryString.ToString();
                var backUrl = string.Format(
                        Constants.ApplicationPages.DetailRedirectFormat,
                        Constants.ApplicationPages.OverheadDetail,
                        this.OverheadId.Value);
                RedirectWithBack(eventArgument, backUrl);
            }
        }

        #endregion
	}
}

