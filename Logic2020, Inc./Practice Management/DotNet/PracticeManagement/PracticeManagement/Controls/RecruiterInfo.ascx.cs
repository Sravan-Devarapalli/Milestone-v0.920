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
                    string.Empty,
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
                            ddlRecruiter.SortByText();
                        }

                        ddlRecruiter.SelectedValue = selectedRecruiter.Value;
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
                    reqRecruiterCommission1.Enabled = compRecruiterCommission1.Enabled =
                    reqAfret1.Enabled = compAfret1.Enabled =
                    reqRecruiterCommission2.Enabled = compRecruiterCommission2.Enabled =
                    reqAfter2.Enabled = compAfter2.Enabled = compAfter.Enabled =
                    value && !string.IsNullOrEmpty(ddlRecruiter.SelectedValue);
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
            txtRecruiterCommission1.Enabled = reqRecruiterCommission1.Enabled = compRecruiterCommission1.Enabled =
                txtAfter1.Enabled = reqAfret1.Enabled = compAfret1.Enabled =
                txtRecruiterCommission2.Enabled = reqRecruiterCommission2.Enabled = compRecruiterCommission2.Enabled =
                txtAfter2.Enabled = reqAfter2.Enabled = compAfter2.Enabled =
                !string.IsNullOrEmpty(ddlRecruiter.SelectedValue) && ddlRecruiter.Enabled;
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
                commission1.Amount = tmpAmount;
                int.TryParse(txtAfter1.Text, out tmpHours);
                commission1.HoursToCollect = tmpHours * 8;

                commission2.RecruiterId = RecruiterId.Value;
                decimal.TryParse(txtRecruiterCommission2.Text, out tmpAmount);
                commission2.Amount = tmpAmount;
                int.TryParse(txtAfter2.Text, out tmpHours);
                commission2.HoursToCollect = tmpHours * 8;
            }
            else if (!string.IsNullOrEmpty(hidRecruiter.Value))
            {
                commission1.RecruiterId = commission2.RecruiterId = int.Parse(hidRecruiter.Value);
            }

            if (!string.IsNullOrEmpty(hidOldAfret1.Value))
            {
                commission1.Old_HoursToCollect = int.Parse(hidOldAfret1.Value);
            }
            result.Add(commission1);
            if (!string.IsNullOrEmpty(hidOldAfret2.Value))
            {
                commission2.Old_HoursToCollect = int.Parse(hidOldAfret2.Value);
            }
            result.Add(commission2);
            result.Sort();
        }
        private void PopulateControls(DefaultRecruiterCommission commission)
        {
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

        private void PopulateControls(List<RecruiterCommission> value)
        {
            if (value != null && value.Count > 0)
            {
                RecruiterId = value[0].RecruiterId;
                hidRecruiter.Value = value[0].RecruiterId.ToString();

                txtRecruiterCommission1.Text = value[0].Amount.Value.ToString();
                txtAfter1.Text = (value[0].HoursToCollect / 8).ToString();
                hidOldAfret1.Value = value[0].HoursToCollect.ToString();

                if (value.Count > 1)
                {
                    txtRecruiterCommission2.Text = value[1].Amount.Value.ToString();
                    txtAfter2.Text = (value[1].HoursToCollect / 8).ToString();
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

