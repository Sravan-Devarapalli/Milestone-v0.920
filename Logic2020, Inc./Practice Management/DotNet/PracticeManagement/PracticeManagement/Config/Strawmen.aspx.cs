using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
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

        private List<Pay> StrawmenCompersationList { get; set; }

        #endregion

        #region Methods

        private bool IsEditCompersationEnable = false;

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
            PopulateCompensationBasicData(strawman.CurrentPay, txtAmount, txtVacationDays, ddlBasic);
            strawman.CurrentPay.BonusAmount = (strawman.CurrentPay.Timescale != TimescaleType._1099Ctc && strawman.CurrentPay.Timescale != TimescaleType.PercRevenue) ? (decimal)strawman.CurrentPay.BonusAmount : 0;
            strawman.CurrentPay.BonusHoursToCollect = (strawman.CurrentPay.Timescale != TimescaleType._1099Ctc && strawman.CurrentPay.Timescale != TimescaleType.PercRevenue && !strawman.CurrentPay.IsYearBonus) ? (int?)strawman.CurrentPay.BonusHoursToCollect : null;
        }

        private void PopulateCompensationBasicData(Pay pay, TextBox txtAmount, TextBox txtVacationDays, DropDownList ddlBasic)
        {
            if (ddlBasic.SelectedIndex == 0)
            {
                pay.Timescale = TimescaleType.Salary;
            }
            else if (ddlBasic.SelectedIndex == 1)
            {
                pay.Timescale = TimescaleType.Hourly;
            }
            else if (ddlBasic.SelectedIndex == 2)
            {
                pay.Timescale = TimescaleType._1099Ctc;
            }
            else
            {
                pay.Timescale = TimescaleType.PercRevenue;
            }
            PracticeManagementCurrency amt = new PracticeManagementCurrency();
            amt.Value = Convert.ToDecimal(txtAmount.Text);
            pay.Amount = amt;
            int vacationHours = 0;
            int.TryParse(txtVacationDays.Text, out vacationHours);
            pay.VacationDays = pay.Timescale == TimescaleType.Salary ? (int?)(vacationHours / 8) : null;
        }

        private void PopulateCompersationData(Pay pay)
        {
            var txtAmount = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex].FindControl("txtAmount") as TextBox;
            var txtVacationDays = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex].FindControl("txtVacationDays") as TextBox;
            var ddlBasic = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex].FindControl("ddlBasic") as DropDownList;
            PopulateCompensationBasicData(pay, txtAmount, txtVacationDays, ddlBasic);
        }

        private void PopulateValidationPanel()
        {
            mpeValidationPanel.Show();
        }

        private void PopulatePayHistoryPanel(int personId)
        {
            hdCompersationStrawman.Value = personId.ToString();
            Person person = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(personId));
            lblStrawmanName.Text = person.HtmlEncodedName;
            StrawmenCompersationList = person.PaymentHistory;
            IsEditCompersationEnable = StrawmenCompersationList.Count() == 1;
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
            if (pay != null && pay.Timescale == TimescaleType.Salary && pay.VacationDays.HasValue)
            {
                return (pay.VacationDays.Value * 8).ToString();
            }
            return string.Empty;
        }

        #region Validation Control Events

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
            if (ddlBasic.SelectedIndex != 0)
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

        protected void cvVacationDaysCompersation_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = false;
            var txtVacationDays = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex].FindControl("txtVacationDays") as TextBox;
            var ddlBasic = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex].FindControl("ddlBasic") as DropDownList;
            if (ddlBasic.SelectedIndex != 0)
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

        #endregion

        #region gvStrawmen Control Events

        protected void gvStrawmen_PreRender(object sender, EventArgs e)
        {
            if (gvStrawmen.Rows.Count > 0)
            {
                //This replaces <td> with <th> and adds the scope attribute
                gvStrawmen.UseAccessibleHeader = true;

                //This will add the <thead> and <tbody> elements
                gvStrawmen.HeaderRow.TableSection = TableRowSection.TableHeader;

            }
        }

        protected void gvStrawmen_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && gvStrawmen.EditIndex != e.Row.DataItemIndex)
            {
                var lblAmount = e.Row.FindControl("lblAmount") as Label;
                var tdAmount = lblAmount.Parent as DataControlFieldCell;
                tdAmount.Attributes["sorttable_customkey"] = lblAmount.Text;

                var lblVacationDays = e.Row.FindControl("lblVacationDays") as Label;
                var tdVacationDays = lblVacationDays.Parent as DataControlFieldCell;
                tdVacationDays.Attributes["sorttable_customkey"] = string.IsNullOrEmpty(lblVacationDays.Text) ? "-1" : lblVacationDays.Text;

                var lblBasic = e.Row.FindControl("lblBasic") as Label;
                var tdBasic = lblBasic.Parent as DataControlFieldCell;
                tdBasic.Attributes["sorttable_customkey"] = lblBasic.Text;

                var chbIsActive = e.Row.FindControl("chbIsActive") as CheckBox;
                var tdIsActive = chbIsActive.Parent as DataControlFieldCell;
                tdIsActive.Attributes["sorttable_customkey"] = chbIsActive.Checked.ToString();


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
                var txtAmount = e.Row.FindControl("txtAmount") as TextBox;

                var editPerson = StrawmenList[gvStrawmen.EditIndex] as Person;
                var pay = editPerson.CurrentPay;
                PopulateTimeScale(ddlBasic, txtVacationDays, pay);

                var tdAmount = txtAmount.Parent as DataControlFieldCell;
                tdAmount.Attributes["sorttable_customkey"] = txtAmount.Text;
                var tdVacationDays = txtVacationDays.Parent as DataControlFieldCell;
                tdVacationDays.Attributes["sorttable_customkey"] = string.IsNullOrEmpty(txtVacationDays.Text) ? "-1" : txtVacationDays.Text;
                var tdBasic = ddlBasic.Parent as DataControlFieldCell;
                tdBasic.Attributes["sorttable_customkey"] = ddlBasic.SelectedValue;

                var chbIsActiveEd = e.Row.FindControl("chbIsActiveEd") as CheckBox;
                var tdIsActive = chbIsActiveEd.Parent as DataControlFieldCell;
                tdIsActive.Attributes["sorttable_customkey"] = chbIsActiveEd.Checked.ToString();
            }
        }

        private void PopulateTimeScale(DropDownList ddlBasic, TextBox txtVacationDays, Pay pay)
        {
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
                    txtVacationDays.Enabled = false;
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
            gvCompensationHistory.EditIndex = -1;
            mlConfirmationCompersation.ClearMessage();
            PopulatePayHistoryPanel(strawmanId);
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
            txtVacationDays.Enabled = ddlBasic.SelectedIndex == 0;
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

        #endregion

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
                lnkEditStrawman.Text = string.Format(Person.PersonNameFormat, HttpUtility.HtmlEncode(hdnLastName.Value), HttpUtility.HtmlEncode(hdnFirstName.Value));
                mpeEditStrawmanPopup.Hide();
            }
        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            tbLastName.Text = tbFirstName.Text = "";
            mpeEditStrawmanPopup.Hide();
        }

        #region gvCompensationHistory Control Events

        protected void gvCompensationHistory_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[0].Visible = IsEditCompersationEnable;
            e.Row.Cells[e.Row.Cells.Count - 1].Visible = !IsEditCompersationEnable;
            if (gvCompensationHistory.EditIndex == e.Row.DataItemIndex && e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddlBasic = e.Row.FindControl("ddlBasic") as DropDownList;
                var txtVacationDays = e.Row.FindControl("txtVacationDays") as TextBox;
                var pay = StrawmenCompersationList[gvCompensationHistory.EditIndex] as Pay;
                PopulateTimeScale(ddlBasic, txtVacationDays, pay);
            }
        }

        protected void imgEditStrawmanCompersation_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            int personId;
            if (int.TryParse(hdCompersationStrawman.Value, out personId))
            {
                gvCompensationHistory.EditIndex = row.DataItemIndex;
                mlConfirmationCompersation.ClearMessage();
                PopulatePayHistoryPanel(personId);
            }
            mpeCompensation.Show();
        }

        protected void imgUpdateStrawmanCompersation_OnClick(object sender, EventArgs e)
        {
            ImageButton imgUpdate = sender as ImageButton;
            GridViewRow row = imgUpdate.NamingContainer as GridViewRow;
            int strawmanId;
            if (int.TryParse(hdCompersationStrawman.Value, out strawmanId))
            {
                Page.Validate(vsStrawmanCompersationSummary.ValidationGroup);
                if (Page.IsValid)
                {
                    Person strawman = ServiceCallers.Custom.Person(p => p.GetStrawmanDetailsById(strawmanId));
                    strawman.Id = strawmanId;
                    var lblStartDate = row.FindControl("lblStartDate") as Label;
                    var startDate = Convert.ToDateTime(lblStartDate.Text);
                    Pay pay = strawman.PaymentHistory.First(p => p.StartDate == startDate);
                    PopulateCompersationData(pay);
                    ServiceCallers.Custom.Person(s => s.SaveStrawman(strawman, pay, User.Identity.Name));
                    mlConfirmationCompersation.ShowInfoMessage("Compensation details are saved successfully.");
                    gvCompensationHistory.EditIndex = -1;
                    PopulatePayHistoryPanel(strawmanId);
                    DataBind_gvStrawmen();
                }
            }
            mpeCompensation.Show();
        }

        protected void imgCancelCompensation_OnClick(object sender, EventArgs e)
        {
            gvCompensationHistory.EditIndex = -1;
            mlConfirmationCompersation.ClearMessage();
            int personId;
            if (int.TryParse(hdCompersationStrawman.Value, out personId))
            {
                PopulatePayHistoryPanel(personId);
            }
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
            mlConfirmationCompersation.ShowInfoMessage("Compensation successfully deleted.");
            mpeCompensation.Show();
        }

        protected void ddlBasic_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlBasic = sender as DropDownList;
            var row = ddlBasic.NamingContainer as GridViewRow;
            var txtVacationDays = row.FindControl("txtVacationDays") as TextBox;
            txtVacationDays.Enabled = ddlBasic.SelectedIndex == 0;
        }

        #endregion

        #endregion
    }
}

