using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using DataTransferObjects;
using PraticeManagement.DefaultRecruiterCommissionService;
using Resources;
using System.Web.UI.WebControls;
using PraticeManagement.Utils;
namespace PraticeManagement.Controls
{
    public partial class RecruiterInfo : System.Web.UI.UserControl
    {
        #region Fields

        private Person personValue;

        #endregion

        #region Properties

        /// <summary>
        /// Internally gets or sets a <see cref="Person"/> to operate on.
        /// </summary>
        [Browsable(false)]
        public Person Person
        {
            private get
            {
                return personValue;
            }
            set
            {
                bool doRestoreRecruiter = personValue != null;
                personValue = value;
                // Preserve the selected value.
                int? selectedRecruiter = RecruiterId;
                DataHelper.FillRecruiterList(ddlRecruiter,
                    NeedFirstItemForRecruiterDropDown ? "--Select Recruiter--" : " ",
                    personValue != null ? personValue.Id : null,
                    personValue != null && personValue.HireDate != DateTime.MinValue ?
                    (DateTime?)personValue.HireDate : null);

                RecruiterCommission = personValue != null ? personValue.RecruiterCommission : null;
                if (doRestoreRecruiter)
                {
                    RecruiterId = selectedRecruiter;
                }
            }
        }

        /// <summary>
        /// Gets or internally sets a list of the <see cref="RecruiterCommission"/> objects.
        /// </summary>
        [Browsable(false)]
        public List<RecruiterCommission> RecruiterCommission
        {
            get
            {
                List<RecruiterCommission> result = new List<RecruiterCommission>();

                PopulateData(result);

                return result;
            }
            set
            {
                PopulateControls(value);
            }
        }

        private int? RecruiterId
        {
            get
            {
                int result;
                if (int.TryParse(ddlRecruiter.SelectedValue, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    if (value.HasValue)
                    {

                        ListItem selectedRecruiter = ddlRecruiter.Items.FindByValue(value.Value.ToString());
                        if (selectedRecruiter == null)
                        {
                            Person selectedPerson = DataHelper.GetPerson(value.Value);

                            selectedRecruiter = new ListItem(selectedPerson.PersonLastFirstName, selectedPerson.Id.Value.ToString());
                            ddlRecruiter.Items.Add(selectedRecruiter);
                            if (NeedFirstItemForRecruiterDropDown)
                            {
                                var firstItem = ddlRecruiter.Items[0];
                                ddlRecruiter.Items.RemoveAt(0);
                                ddlRecruiter.SortByText();
                                ddlRecruiter.Items.Insert(0, firstItem);
                            }
                        }

                        ddlRecruiter.SelectedValue = selectedRecruiter.Value;
                    }
                    else
                    {
                        ddlRecruiter.SelectedIndex = 0;
                    }
                }
                catch
                {
                    throw new Exception(Messages.RecruiterIsNotInTheList);
                }

                UpdateInfoState();
            }
        }

        /// <summary>
        /// Gets or sets wether the commission details will are visible.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowCommissionDetails
        {
            get
            {
                return trCommissionDetails.Visible;
            }
            set
            {
                trCommissionDetails.Visible =
                    compRecruiterCommission1.Enabled =
                    compAfret1.Enabled = custAfret1.Enabled = custRecruiterCommission1.Enabled =
                    custRecruiterCommission2.Enabled = compRecruiterCommission2.Enabled =
                    custAfter2.Enabled = compAfter2.Enabled = compAfter.Enabled =
                    value;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return !ddlRecruiter.Enabled;
            }
            set
            {
                ddlRecruiter.Enabled = !value;
                UpdateInfoState();
            }
        }

        public bool NeedFirstItemForRecruiterDropDown
        {
            set;
            get;
        }

        public event EventHandler InfoChanged;

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateInfoState();
            }
        }

        protected void ddlRecruiter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCommissionForRecruiter();
        }

        /// <summary>
        /// Sets a recruiter and updates the commission from default values.
        /// </summary>
        /// <param name="recruiterId">An ID of the recruiter.</param>
        public void SetRecruiter(int recruiterId)
        {
            this.RecruiterId = recruiterId;
            UpdateCommissionForRecruiter();
        }

        private void UpdateCommissionForRecruiter()
        {
            if (RecruiterId.HasValue)
            {
                using (DefaultRecruiterCommissionServiceClient serviceClient =
                    new DefaultRecruiterCommissionServiceClient())
                {
                    try
                    {
                        DefaultRecruiterCommission commissions =
                            serviceClient.DefaultRecruiterCommissionGetByPersonDate(
                            RecruiterId.Value,
                            Person != null && Person.HireDate != DateTime.MinValue ?
                            Person.HireDate : DateTime.Today);

                        PopulateControls(commissions);
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }
            UpdateInfoState();
            OnInfoChanged();
        }

        private void UpdateInfoState()
        {
            txtRecruiterCommission1.Enabled = compRecruiterCommission1.Enabled =
                txtAfter1.Enabled = compAfret1.Enabled = custAfret1.Enabled = custRecruiterCommission1.Enabled =
                txtRecruiterCommission2.Enabled = custRecruiterCommission2.Enabled = compRecruiterCommission2.Enabled =
                txtAfter2.Enabled = custAfter2.Enabled = compAfter2.Enabled =
                ddlRecruiter.Enabled;
        }

        private void PopulateData(List<RecruiterCommission> result)
        {
            result.Clear();

            RecruiterCommission commission1 = new RecruiterCommission();
            RecruiterCommission commission2 = new RecruiterCommission();
            if (RecruiterId.HasValue)
            {
                decimal tmpAmount;
                int tmpHours;

                commission1.RecruiterId = RecruiterId.Value;
                decimal.TryParse(txtRecruiterCommission1.Text, out tmpAmount);
                if (string.IsNullOrEmpty(txtRecruiterCommission1.Text))
                {
                    commission1.Amount = null;
                }
                else
                {
                    commission1.Amount = tmpAmount;
                }
                int.TryParse(txtAfter1.Text, out tmpHours);
                commission1.HoursToCollect = tmpHours * 8;

                commission2.RecruiterId = RecruiterId.Value;
                decimal.TryParse(txtRecruiterCommission2.Text, out tmpAmount);
                if (string.IsNullOrEmpty(txtRecruiterCommission2.Text))
                {
                    commission2.Amount = null;
                }
                else
                {
                    commission2.Amount = tmpAmount;
                }
                int.TryParse(txtAfter2.Text, out tmpHours);
                commission2.HoursToCollect = tmpHours * 8;
            }
            //else if (!string.IsNullOrEmpty(hidRecruiter.Value))
            //{
            //    commission1.RecruiterId = commission2.RecruiterId = int.Parse(hidRecruiter.Value);
            //}

            if (!string.IsNullOrEmpty(hidOldAfret1.Value))
            {
                commission1.Old_HoursToCollect = int.Parse(hidOldAfret1.Value);
            }

            if (!string.IsNullOrEmpty(hidOldAfret2.Value))
            {
                commission2.Old_HoursToCollect = int.Parse(hidOldAfret2.Value);
            }
            if (RecruiterId.HasValue && RecruiterId.Value != 0
                //|| !string.IsNullOrEmpty(hidRecruiter.Value)
                )
            {
                if (!commission2.Amount.HasValue || commission1.Amount.HasValue)
                {
                    result.Add(commission1);
                }
                if (commission2.Amount.HasValue)
                {
                    result.Add(commission2);
                }
            }
            result.Sort();
        }
        private void PopulateControls(DefaultRecruiterCommission commission)
        {
            txtRecruiterCommission1.Text = txtAfter1.Text =
            txtRecruiterCommission2.Text = txtAfter2.Text = string.Empty;
            if (commission != null && commission.Items != null && commission.Items.Count > 0)
            {
                txtRecruiterCommission1.Text = commission.Items[0].Amount.Value.ToString();
                txtAfter1.Text = (commission.Items[0].HoursToCollect / 8).ToString();
                hidOldAfret1.Value = commission.Items[0].HoursToCollect.ToString();

                if (commission.Items.Count > 1)
                {
                    txtRecruiterCommission2.Text = commission.Items[1].Amount.Value.ToString();
                    txtAfter2.Text = (commission.Items[1].HoursToCollect / 8).ToString();
                    hidOldAfret2.Value = commission.Items[1].HoursToCollect.ToString();
                }
            }

            UpdateInfoState();
        }

        protected void custRecruiterCommission1_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAfter1.Text) && string.IsNullOrEmpty(txtRecruiterCommission1.Text))
            {
                e.IsValid = false;
            }
            else
            {
                e.IsValid = true;
            }

        }

        protected void custAfret1_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAfter1.Text) && !string.IsNullOrEmpty(txtRecruiterCommission1.Text))
            {
                e.IsValid = false;
            }
            else
            {
                e.IsValid = true;
            }
        }


        protected void custRecruiterCommission2_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAfter2.Text) && string.IsNullOrEmpty(txtRecruiterCommission2.Text))
            {
                e.IsValid = false;
            }
            else
            {
                e.IsValid = true;
            }
        }

        protected void custAfter2_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAfter2.Text) && !string.IsNullOrEmpty(txtRecruiterCommission2.Text))
            {
                e.IsValid = false;
            }
            else
            {
                e.IsValid = true;
            }
        }
        private void PopulateControls(List<RecruiterCommission> value)
        {
            txtRecruiterCommission1.Text = txtAfter1.Text =
                txtAfter2.Text = txtRecruiterCommission2.Text = string.Empty;
            if (value != null && value.Count > 0)
            {
                RecruiterId = value[0].RecruiterId;
                hidRecruiter.Value = value[0].RecruiterId.ToString();
                if (value[0].Amount.HasValue)
                {
                    txtRecruiterCommission1.Text = value[0].Amount.Value.Value.ToString();
                    txtAfter1.Text = (value[0].HoursToCollect / 8).ToString();
                }
                hidOldAfret1.Value = value[0].HoursToCollect.ToString();
                if (value.Count > 1)
                {
                    if (value[1].Amount.HasValue)
                    {
                        txtRecruiterCommission2.Text = value[1].Amount.Value.Value.ToString();
                        txtAfter2.Text = (value[1].HoursToCollect / 8).ToString();
                    }
                    hidOldAfret2.Value = value[1].HoursToCollect.ToString();
                }
            }
            else
            {
                RecruiterId = null;
            }

            UpdateInfoState();
        }

        private void OnInfoChanged()
        {
            if (InfoChanged != null)
            {
                InfoChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}

