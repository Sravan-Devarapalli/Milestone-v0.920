using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.DefaultRecruiterCommissionService;
using PraticeManagement.PersonService;

namespace PraticeManagement
{
    public partial class DefaultRecruitingCommissionDetail : PracticeManagementPageBase
    {
        #region Constants

        private const string DefaultRecruiterCommissionKey = "DefaultRecruiterCommission";
        private const string PersonIdArgument = "personId";
        private const string HiddenHoursToCollectId = "hidHoursToCollect";
        private const string TextBoxHoursToCollectId = "txtHoursToCollect";
        private const string TextBoxAmountId = "txtAmount";

        #endregion

        #region Fields

        private Person personValue;

        #endregion

        #region Properties

        private int? RecruiterCommissionId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    return SelectedId;
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.hdnRecruiterCommissionId.Value))
                    {
                        int recruiterCommissionId;
                        if (Int32.TryParse(this.hdnRecruiterCommissionId.Value, out recruiterCommissionId))
                        {
                            return recruiterCommissionId;
                        }
                    }
                    return null;
                }
            }

            set
            {
                this.hdnRecruiterCommissionId.Value = value.ToString();
            }
        }
        private int? SelectedPersonId
        {
            get
            {
                return base.GetArgumentInt32(PersonIdArgument);
            }
        }

        private Person Person
        {
            get
            {
                if (personValue == null && SelectedPersonId.HasValue)
                {
                    using (PersonServiceClient serviceClient = new PersonServiceClient())
                    {
                        try
                        {
                            personValue = serviceClient.GetPersonDetail(SelectedPersonId.Value);
                        }
                        catch (CommunicationException)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }

                return personValue;
            }
        }

        private DefaultRecruiterCommission DefaultRecruiterCommission
        {
            get
            {
                DefaultRecruiterCommission result =
                    ViewState[DefaultRecruiterCommissionKey] as DefaultRecruiterCommission;
                return result;
            }
            set
            {
                if (value.Items != null)
                {
                    value.Items.Sort();
                }
                ViewState[DefaultRecruiterCommissionKey] = value;
            }
        }

        private ExceptionDetail InternalException
        {
            get;
            set;
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateAndSave() && Page.IsValid)
            {
                ClearDirty();
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Recruiting Commission"));
            }
            if (this.Page.IsValid && this.RecruiterCommissionId.HasValue)
            {
                var commission = CommissionGetById(RecruiterCommissionId);
                if (commission != null)
                {
                    PopulateControls(commission);
                }
                else
                {
                    commission = new DefaultRecruiterCommission();
                    commission.CommissionHeaderId = RecruiterCommissionId;
                    commission.Items = new List<DefaultRecruiterCommissionItem>();
                }
                this.DefaultRecruiterCommission = commission;
            }
        }

        protected override void Display()
        {
            if (Person != null)
            {
                personInfo.Person = Person;
            }

            var commission = CommissionGetById(RecruiterCommissionId);

            if (commission != null)
            {
                PopulateControls(commission);
            }
            else
            {
                commission = new DefaultRecruiterCommission();
                commission.Items = new List<DefaultRecruiterCommissionItem>();
            }
            DefaultRecruiterCommission = commission;
            BindGrid(commission.Items);
        }


        private DefaultRecruiterCommission CommissionGetById(int? recruiterCommissionId)
        {
            DefaultRecruiterCommission commission = null;

            if (recruiterCommissionId.HasValue)
            {
                using (DefaultRecruiterCommissionServiceClient serviceClient =
                    new DefaultRecruiterCommissionServiceClient())
                {
                    try
                    {
                        commission = serviceClient.DefaultRecruiterCommissionGetById(RecruiterCommissionId.Value);

                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }

            return commission;
        }

        protected override bool ValidateAndSave()
        {
            bool result = false;
            Page.Validate(vsumDefaultRecruitingCommissionHeader.ValidationGroup);
            if (Page.IsValid)
            {
                DefaultRecruiterCommission commission = DefaultRecruiterCommission;
                commission.PersonId = SelectedPersonId.Value;
                commission.StartDate = dpStartDate.DateValue;
                commission.EndDate =
                    dpEndDate.DateValue != DateTime.MinValue ? (DateTime?)dpEndDate.DateValue.AddDays(1) : null;

                using (DefaultRecruiterCommissionServiceClient serviceClient =
                    new DefaultRecruiterCommissionServiceClient())
                {
                    try
                    {
                        int? commissionHeaderId = serviceClient.DefaultRecruiterCommissionSave(DefaultRecruiterCommission);
                        if (commissionHeaderId.HasValue)
                        {
                            this.RecruiterCommissionId = commissionHeaderId;
                        }
                        result = true;
                    }
                    catch (FaultException<ExceptionDetail> ex)
                    {
                        InternalException = ex.Detail;
                        serviceClient.Abort();
                        Page.Validate(vsumDefaultRecruitingCommissionHeader.ValidationGroup);
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }

            return result;
        }

        private void PopulateControls(DefaultRecruiterCommission commission)
        {
            dpStartDate.DateValue = commission.StartDate;
            dpEndDate.DateValue =
                commission.EndDate.HasValue ? commission.EndDate.Value.AddDays(-1) : DateTime.MinValue;
        }

        #region Validation

        protected void custHoursToCollectDuplicate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            int days;
            e.IsValid =
                DefaultRecruiterCommission == null || DefaultRecruiterCommission.Items == null ||
                !int.TryParse(e.Value, out days) ||
                // Validating for the commission item's period
                DefaultRecruiterCommission.Items.Find(item => item.HoursToCollect == days * 8) == null;
        }

        protected void custCommissionItems_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid =
                DefaultRecruiterCommission != null &&
                DefaultRecruiterCommission.Items != null &&
                (DefaultRecruiterCommission.Items.Count > 0 || bool.Parse(hdDeleted.Value));
        }

        protected void custdateRangeBegining_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid =
                InternalException == null ||
                InternalException.Message != ErrorCode.DefaultRecruiterCommissionStartDateIncorrect.ToString();
        }

        protected void custdateRangeEnding_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid =
                InternalException == null ||
                InternalException.Message != ErrorCode.DefaultRecruiterCommissionEndDateIncorrect.ToString();
        }

        protected void custdateRangePeriod_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid =
                InternalException == null ||
                InternalException.Message != ErrorCode.DefaultRecruiterCommissionPeriodIncorrect.ToString();
        }

        protected void custStartDate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid =
                InternalException == null ||
                (Array.IndexOf(Enum.GetNames(typeof(ErrorCode)), InternalException.Message) >= 0 &&
                InternalException.Message != ErrorCode.Unknown.ToString());
            if (!e.IsValid)
            {
                custStartDate.ErrorMessage = custStartDate.ToolTip =
                    PrepareOperationErrorMessage(InternalException);
            }
        }

        private string PrepareOperationErrorMessage(ExceptionDetail internalException)
        {
            string detailedError = internalException.ToString();

            if (detailedError.Contains("Error 70013"))
            {
                return "Commmission period overlaps on start date with the existent.";
            }

            if (detailedError.Contains("Error 70014"))
            {
                return "Commmission period overlaps on end date with the existent.";
            }

            if (detailedError.Contains("Error 70015"))
            {
                return "Commmission period overlaps within the whole period with the existent.";
            }

            return internalException.InnerException != null ? internalException.InnerException.Message : internalException.Message;
        }

        #endregion

        protected void gvRecruitingCommissions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Visible = ((DefaultRecruiterCommissionItem)e.Row.DataItem).CommissionHeaderId >= 0;
            }
        }

        protected void gvRecruitingCommissions_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (DefaultRecruiterCommission != null && DefaultRecruiterCommission.Items != null)
            {
                gvRecruitingCommissions.EditIndex = e.NewEditIndex;
                BindGrid(DefaultRecruiterCommission.Items);
            }
            e.Cancel = true;
        }

        protected void gvRecruitingCommissions_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Page.Validate(vsumDefaultRecruitingCommissionItem.ValidationGroup);
            if (Page.IsValid && DefaultRecruiterCommission != null && DefaultRecruiterCommission.Items != null)
            {
                // Getting a key
                GridViewRow row = gvRecruitingCommissions.Rows[e.RowIndex];
                HiddenField hidHours = row.FindControl(HiddenHoursToCollectId) as HiddenField;
                int hoursToCollect = int.Parse(hidHours.Value);

                // Find a commission item
                DefaultRecruiterCommission commission = DefaultRecruiterCommission;
                DefaultRecruiterCommissionItem commissionItem =
                    commission.Items.Find(item => item.HoursToCollect == hoursToCollect);
                if (commissionItem != null)
                {
                    // Update the data
                    TextBox txtAmount = row.FindControl(TextBoxAmountId) as TextBox;
                    commissionItem.Amount = decimal.Parse(txtAmount.Text);
                    DefaultRecruiterCommission = commission;
                    gvRecruitingCommissions.EditIndex = -1;
                    BindGrid(DefaultRecruiterCommission.Items);
                }
            }

            e.Cancel = true;
        }

        protected void gvRecruitingCommissions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (DefaultRecruiterCommission != null && DefaultRecruiterCommission.Items != null)
            {
                // Getting a key
                GridViewRow row = gvRecruitingCommissions.Rows[e.RowIndex];
                HiddenField hidHours = row.FindControl(HiddenHoursToCollectId) as HiddenField;
                int hoursToCollect = int.Parse(hidHours.Value);

                // Find a commission item
                DefaultRecruiterCommission commission = DefaultRecruiterCommission;
                DefaultRecruiterCommissionItem commissionItem =
                    commission.Items.Find(item => item.HoursToCollect == hoursToCollect);
                if (commissionItem != null)
                {
                    // Update the data
                    commission.Items.Remove(commissionItem);
                    DefaultRecruiterCommission = commission;
                    BindGrid(DefaultRecruiterCommission.Items);
                }
            }
            e.Cancel = true;
            hdDeleted.Value = bool.TrueString;
        }

        protected void gvRecruitingCommissions_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            if (DefaultRecruiterCommission != null && DefaultRecruiterCommission.Items != null)
            {
                gvRecruitingCommissions.EditIndex = -1;
                BindGrid(DefaultRecruiterCommission.Items);
            }
            e.Cancel = true;
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            Page.Validate(vsumDefaultRecruitingCommissionItem.ValidationGroup);
            if (Page.IsValid && DefaultRecruiterCommission != null && DefaultRecruiterCommission.Items != null)
            {
                // Getting the values
                GridViewRow row = gvRecruitingCommissions.FooterRow;
                TextBox txtHours = row.FindControl(TextBoxHoursToCollectId) as TextBox;
                TextBox txtAmount = row.FindControl(TextBoxAmountId) as TextBox;

                DefaultRecruiterCommissionItem item =
                    new DefaultRecruiterCommissionItem()
                    {
                        HoursToCollect = int.Parse(txtHours.Text) * 8,
                        Amount = decimal.Parse(txtAmount.Text)
                    };

                // Update the data
                DefaultRecruiterCommission commission = DefaultRecruiterCommission;
                commission.Items.Add(item);
                DefaultRecruiterCommission = commission;
                BindGrid(DefaultRecruiterCommission.Items);
            }
        }

        private void BindGrid(List<DefaultRecruiterCommissionItem> items)
        {
            List<DefaultRecruiterCommissionItem> itemsTmp = new List<DefaultRecruiterCommissionItem>(items);
            if (itemsTmp != null && itemsTmp.Count == 0)
            {
                itemsTmp.Add(new DefaultRecruiterCommissionItem()
                    {
                        CommissionHeaderId = -1
                    });
            }

            gvRecruitingCommissions.ShowFooter = gvRecruitingCommissions.EditIndex < 0;
            gvRecruitingCommissions.DataSource = itemsTmp;
            gvRecruitingCommissions.DataBind();
        }
    }
}

