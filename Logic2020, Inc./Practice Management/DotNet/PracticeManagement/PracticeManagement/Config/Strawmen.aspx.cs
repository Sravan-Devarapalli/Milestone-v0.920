using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using System.ServiceModel;
using PraticeManagement.PersonService;
using PraticeManagement.ProjectGroupService;
using PraticeManagement.TimescaleService;
using PraticeManagement.Utils;

namespace PraticeManagement.Config
{
    public partial class Strawmen : System.Web.UI.Page
    {
        #region Variables

        private bool IsValidationPanelDisplay;

        private const string DuplicatePersonName = "There is another Person with the same First Name and Last Name.";

        private string ExMessage { get; set; }

        private List<Person> StrawmenList { get; set; }

        #endregion

        #region Methods

        private void GetStrawmenList()
        {
            try
            {
                StrawmenList = ServiceCallers.Custom.Person(p => p.GetStrawmenListAll()).ToList();
            }
            catch (FaultException<ExceptionDetail>)
            {
                StrawmenList = null;
                throw;
            }
        }

        private void DataBind_gvStrawmen()
        {
            if (StrawmenList == null)
            {
                GetStrawmenList();
            }
            gvStrawmen.DataSource = StrawmenList;
            gvStrawmen.DataBind();
        }

        private void PopulateData(Person strawman)
        {
            var hdnLastName = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("hdnLastName") as HiddenField;
            var hdnFirstName = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("hdnFirstName") as HiddenField;
            var chbIsActiveEd = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("chbIsActiveEd") as CheckBox;
            var txtAmount = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("txtAmount") as TextBox;
            var txtVacationDays = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("txtVacationDays") as TextBox;
            var ddlBasic = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("ddlBasic") as DropDownList;

            strawman.LastName = hdnLastName.Value;
            strawman.FirstName = hdnFirstName.Value;
            strawman.Status = new PersonStatus();
            strawman.Status.Id = chbIsActiveEd.Checked ? (int)PersonStatusType.Active : (int)PersonStatusType.Inactive;

            if (strawman.CurrentPay == null)
            {
                strawman.CurrentPay = new Pay();
            }
            else
            {
                strawman.CurrentPay.StartDate = SettingsHelper.GetCurrentPMTime().Date;
            }

            if (ddlBasic.SelectedIndex == 0)
            {
                strawman.CurrentPay.Timescale = TimescaleType.Salary;
            }
            else if (ddlBasic.SelectedIndex == 1)
            {
                strawman.CurrentPay.Timescale = TimescaleType.Hourly;
            }
            else if (ddlBasic.SelectedIndex == 2)
            {
                strawman.CurrentPay.Timescale = TimescaleType._1099Ctc;
            }
            else
            {
                strawman.CurrentPay.Timescale = TimescaleType.PercRevenue;
            }
            PracticeManagementCurrency amt = new PracticeManagementCurrency();
            amt.Value = Convert.ToDecimal(txtAmount.Text);
            strawman.CurrentPay.Amount = amt;
            int vacationHours = 0;
            int.TryParse(txtVacationDays.Text, out vacationHours);
            strawman.CurrentPay.VacationDays = (strawman.CurrentPay.Timescale != TimescaleType._1099Ctc && strawman.CurrentPay.Timescale != TimescaleType.PercRevenue) ? (int?)(vacationHours / 8) : null;
            strawman.CurrentPay.BonusAmount = (strawman.CurrentPay.Timescale != TimescaleType._1099Ctc && strawman.CurrentPay.Timescale != TimescaleType.PercRevenue) ? (decimal)strawman.CurrentPay.BonusAmount : 0;
            strawman.CurrentPay.BonusHoursToCollect = (strawman.CurrentPay.Timescale != TimescaleType._1099Ctc && strawman.CurrentPay.Timescale != TimescaleType.PercRevenue && !strawman.CurrentPay.IsYearBonus) ? (int?)strawman.CurrentPay.BonusHoursToCollect : null;
        }

        private void PopulateValidationPanel()
        {
            mpeValidationPanel.Show();
        }

        private void PopulatePayHistoryPanel(int personId)
        {
            Person person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));
            lblStrawmanName.Text = person.PersonLastFirstName;
            gvCompensationHistory.DataSource = person.PaymentHistory;
            gvCompensationHistory.DataBind();
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind_gvStrawmen();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsValidationPanelDisplay)
            {
                PopulateValidationPanel();
            }
        }

        #endregion

        #region Control Events

        protected string GetVacationDays(Pay pay)
        {
            if (pay != null && pay.Timescale != TimescaleType._1099Ctc && pay.Timescale != TimescaleType.PercRevenue && pay.VacationDays.HasValue)
            {
                return (pay.VacationDays.Value * 8).ToString();
            }
            return string.Empty;
        }

        protected void cvDupliacteName_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (!string.IsNullOrEmpty(ExMessage) && ExMessage == DuplicatePersonName)
            {
                e.IsValid = false;
            }
        }

        protected void cvVacationDays_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = false;
            var txtVacationDays = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("txtVacationDays") as TextBox;
            var ddlBasic = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("ddlBasic") as DropDownList;
            if (ddlBasic.SelectedIndex == 2 || ddlBasic.SelectedIndex == 3)
            {
                e.IsValid = true;
                return;
            }
            int vacationDays;
            if (int.TryParse(txtVacationDays.Text, out vacationDays))
            {
                if (vacationDays % 8 == 0)
                {
                    e.IsValid = true;
                }
            }
        }

        protected void imgCopyStrawman_OnClick(object sender, EventArgs e)
        {
            ImageButton imgCopy = sender as ImageButton;
            int strawmanId = Convert.ToInt32(imgCopy.Attributes["strawmanId"]);
            GetStrawmenList();
            var copyStrawman = StrawmenList.First(s => s.Id == strawmanId);
            hdnCopyStrawman.Value = copyStrawman.Id.Value.ToString();
            StrawmenList.Add(new Person
            {
                FirstName = copyStrawman.FirstName,
                LastName = copyStrawman.LastName,
                CurrentPay = copyStrawman.CurrentPay,
                Status = copyStrawman.Status
            });
            gvStrawmen.DataSource = StrawmenList;
            gvStrawmen.EditIndex = StrawmenList.Count - 1;
            gvStrawmen.DataBind();
        }

        protected void imgCompersationStrawman_OnClick(object sender, EventArgs e)
        {
            ImageButton imgCompersation = sender as ImageButton;
            int strawmanId = Convert.ToInt32(imgCompersation.Attributes["strawmanId"]);
            PopulatePayHistoryPanel(strawmanId);
            mpeCompensation.Show();
        }

        protected void imgCompensationDelete_OnClick(object sender, EventArgs e)
        {
            hdnCopyStrawman.Value = "";
            ImageButton imgDelete = sender as ImageButton;
            GridViewRow row = imgDelete.NamingContainer as GridViewRow;
            var lblStartDate = row.FindControl("lblStartDate") as Label;
            var startDate = Convert.ToDateTime(lblStartDate.Text);
            int personId = Convert.ToInt32(imgDelete.Attributes["strawmanId"].ToString());
            ServiceCallers.Custom.Person(p => p.DeletePay(personId, startDate));
            DataBind_gvStrawmen();
            PopulatePayHistoryPanel(personId);
            mpeCompensation.Show();
        }

        protected void imgEditStrawman_OnClick(object sender, EventArgs e)
        {
            hdnCopyStrawman.Value = "";
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            gvStrawmen.EditIndex = row.DataItemIndex;
            DataBind_gvStrawmen();
            ExMessage = "";
        }

        protected void imgUpdateStrawman_OnClick(object sender, EventArgs e)
        {
            ImageButton imgUpdate = sender as ImageButton;
            GridViewRow row = imgUpdate.NamingContainer as GridViewRow;
            try
            {
                int strawmanId;
                if (int.TryParse(hdnCopyStrawman.Value, out strawmanId))
                {
                    Page.Validate(vsStrawmenSummary.ValidationGroup);
                    if (Page.IsValid)
                    {
                        Person strawman = new Person();
                        PopulateData(strawman);
                        ServiceCallers.Custom.Person(s => s.SaveStrawManFromExisting(strawmanId, strawman, User.Identity.Name));
                        hdnCopyStrawman.Value = "";
                    }
                }
                else
                {
                    strawmanId = Convert.ToInt32(imgUpdate.Attributes["strawmanId"]);
                    Page.Validate(vsStrawmenSummary.ValidationGroup);
                    if (Page.IsValid)
                    {
                        var strawman = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsByIdWithCurrentPay(strawmanId));
                        PopulateData(strawman);
                        int? strawmanIdAfterUpdate = null;
                        strawmanIdAfterUpdate = ServiceCallers.Custom.Person(s => s.SaveStrawman(strawman, strawman.CurrentPay, User.Identity.Name));
                    }
                }
                StrawmenList = null;
            }
            catch (Exception exMessage)
            {
                ExMessage = exMessage.Message;
                Page.Validate(vsStrawmenSummary.ValidationGroup);
            }
            if (Page.IsValid)// && strawmanIdAfterUpdate.HasValue && strawmanIdAfterUpdate.Value == strawmanId)
            {
                gvStrawmen.EditIndex = -1;
                DataBind_gvStrawmen();
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Strawman"));
            }
            else
            {
                mlConfirmation.ClearMessage();
            }
            IsValidationPanelDisplay = true;
            var ddlBasic = row.FindControl("ddlBasic") as DropDownList;
            var txtVacationDays = row.FindControl("txtVacationDays") as TextBox;
            txtVacationDays.Enabled = !(ddlBasic.SelectedIndex == 2 || ddlBasic.SelectedIndex == 3);
        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            gvStrawmen.EditIndex = -1;
            DataBind_gvStrawmen();
            hdnCopyStrawman.Value = "";
        }

        protected void imgDeleteStrawman_OnClick(object sender, EventArgs e)
        {
            hdnCopyStrawman.Value = "";
            var imgDelete = sender as ImageButton;
            var gvStrawmenEditRow = imgDelete.NamingContainer as GridViewRow;

            int strawmanId = Convert.ToInt32(imgDelete.Attributes["strawmanId"]);
            Person Strawman = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsByIdWithCurrentPay(strawmanId));
            if (!Strawman.InUse)
            {
                try
                {
                    ServiceCallers.Custom.Person(p => p.DeleteStrawman(strawmanId, User.Identity.Name));
                }
                catch (FaultException<ExceptionDetail>)
                {
                    throw;
                }
                DataBind_gvStrawmen();
            }
        }

        protected void lnkEditStrawman_OnClick(object sender, EventArgs e)
        {
            var hdnLastName = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("hdnLastName") as HiddenField;
            var hdnFirstName = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("hdnFirstName") as HiddenField;
            tbLastName.Text = hdnLastName.Value;
            tbFirstName.Text = hdnFirstName.Value;
            mpeEditStrawmanPopup.Show();
        }

        protected void btnOK_OnClick(object sender, EventArgs e)
        {
            Page.Validate(valSummary.ValidationGroup);
            if (Page.IsValid)
            {
                var lnkEditStrawman = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("lnkEditStrawman") as LinkButton;
                var hdnLastName = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("hdnLastName") as HiddenField;
                var hdnFirstName = gvStrawmen.Rows[gvStrawmen.EditIndex].FindControl("hdnFirstName") as HiddenField;
                hdnLastName.Value = tbLastName.Text;
                hdnFirstName.Value = tbFirstName.Text;
                lnkEditStrawman.Text = string.Format(Person.PersonNameFormat, hdnLastName.Value, hdnFirstName.Value);
                mpeEditStrawmanPopup.Hide();
            }
        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            tbLastName.Text = tbFirstName.Text = "";
            mpeEditStrawmanPopup.Hide();
        }

        protected void gvStrawmen_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && gvStrawmen.EditIndex != e.Row.DataItemIndex)
            {
                var imgDeleteStrawman = e.Row.FindControl("imgDeleteStrawman") as ImageButton;
                var Person = e.Row.DataItem as Person;
                if (Person.InUse)
                {
                    imgDeleteStrawman.Visible = false;
                }
            }
            if (gvStrawmen.EditIndex == e.Row.DataItemIndex && e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddlBasic = e.Row.FindControl("ddlBasic") as DropDownList;
                var txtVacationDays = e.Row.FindControl("txtVacationDays") as TextBox;
                var editPerson = StrawmenList[gvStrawmen.EditIndex] as Person;
                var pay = editPerson.CurrentPay;
                
                ddlBasic.Attributes["vacationdaysId"] = txtVacationDays.ClientID;
                if (pay != null)
                {
                    if (pay.Timescale == TimescaleType.Salary)
                    {
                        ddlBasic.SelectedIndex = 0;
                        txtVacationDays.Enabled = true;
                    }
                    else if (pay.Timescale == TimescaleType.Hourly)
                    {
                        ddlBasic.SelectedIndex = 1;
                        txtVacationDays.Enabled = true;
                    }
                    else if (pay.Timescale == TimescaleType._1099Ctc)
                    {
                        ddlBasic.SelectedIndex = 2;
                        txtVacationDays.Enabled = false;
                    }
                    else
                    {
                        ddlBasic.SelectedIndex = 3;
                        txtVacationDays.Enabled = false;
                    }
                }
            }
        }

        //protected string GetStrawmanDetailsUrlWithReturn(int? strawmanId)
        //{
        //    if (strawmanId.HasValue)
        //    {
        //        var StrawmanDetailPage = string.Format(Constants.ApplicationPages.DetailRedirectFormat,
        //                            Constants.ApplicationPages.StrawManDetail,
        //                            strawmanId);
        //        return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(StrawmanDetailPage,
        //           Request.Url.AbsoluteUri + (Request.Url.Query.Length > 0 ? string.Empty : Constants.FilterKeys.QueryStringOfApplyFilterFromCookie));
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}

        #endregion
    }
}
