using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Configuration;
using PraticeManagement.PracticeService;

namespace PraticeManagement.Config
{
    public partial class PracticeAreas : PracticeManagementPageBase
    {
        private string PracticeUpdatedSuccessfully = "Practice updated successfully.";
        private string PracticeDeletedSuccessfully = "Practice deleted successfully.";
        private const string PracticeArea_KEY = "PracticeAreaList";
        private const string PracticeAreaManagers_KEY = "PracticeAreaManagersList";
        private const string PracticeManagerRoleName = "Practice Area Manager";
        private const string PracticeMangagerNameFormat = "{0}({1})";

        public List<Practice> Practices
        {
            get
            {
                if (ViewState[PracticeArea_KEY] == null)
                {
                    ViewState[PracticeArea_KEY] = DataHelper.GetPractices(null).ToList();
                }

                return (List<Practice>)ViewState[PracticeArea_KEY];
            }
            set { ViewState[PracticeArea_KEY] = value; }
        }

        public List<Person> PracticeManagers
        {
            get
            {
                if (ViewState[PracticeAreaManagers_KEY] == null)
                {
                    string statusIds = ((int)DataTransferObjects.PersonStatusType.Active).ToString();
                    string paytypeIds = ((int)TimescaleType.Salary).ToString();
                    ViewState[PracticeAreaManagers_KEY] = ServiceCallers.Custom.Person(p => p.GetPersonsByPayTypesAndByStatusIds(statusIds, paytypeIds)).OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToList();
                }

                return (List<Person>)ViewState[PracticeAreaManagers_KEY];
            }
            set { ViewState[PracticeAreaManagers_KEY] = value; }
        }

        public ListItem[] PracticeManagersList
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mlInsertStatus.ClearMessage();
            var practiceMangersList = new List<ListItem>();
            foreach (var item in PracticeManagers)
            {
                practiceMangersList.Add(new ListItem() { Text = item.PersonLastFirstName, Value = item.Id.Value.ToString() });
            }
            PracticeManagersList = practiceMangersList.ToArray();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            bool IsPageValid = true;
            try
            {
                IsPageValid = Page.IsValid;
            }
            catch
            { }

            if (!IsPageValid || mlInsertStatus.IsMessageExists)
            {
                PopulateErrorPanel();
            }
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            trInsertPractice.Attributes["class"] = (gvPractices.Rows.Count % 2 == 0) ? "" : "alterrow";
        }

        protected override void Display()
        {
            gvPractices.DataSource = Practices;
            gvPractices.DataBind();
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            plusMakeVisible(false);
            gvPractices.EditIndex = -1;
            gvPractices.DataSource = Practices;
            gvPractices.DataBind();
            ddlPracticeManagers.Items.Clear();
            ddlPracticeManagers.Items.Add(new ListItem() { Text = "-- Select Practice Area Owner --", Value = "-1" });
            ddlPracticeManagers.Items.AddRange(PracticeManagersList);
            DataHelper.FillPersonDivisionList(cblDivision, true);
            DataHelper.FillProjectDivisionList(cblProjectDivision, false, true);
        }

        private void plusMakeVisible(bool isplusVisible)
        {
            if (isplusVisible)
            {
                btnPlus.Visible = true;
                btnInsert.Visible =
                btnCancel.Visible =
                tbPracticeName.Visible =
                tbAbbreviation.Visible =
                chbPracticeActive.Visible =
                chbIsInternalPractice.Visible =
                ddlPracticeManagers.Visible = false;
                cblDivision.CssClass = "PracticeAreasCblDivision hidden";
                sdeCblDivision.Display = "none";
                cblProjectDivision.CssClass = "PracticeAreasCblDivision hidden";
                sdecblProjectDivision.Display = "none";
            }
            else
            {
                chbIsInternalPractice.Checked =
                btnPlus.Visible = false;
                tbAbbreviation.Text =
                tbPracticeName.Text = string.Empty;
                btnInsert.Visible =
                btnCancel.Visible =
                tbAbbreviation.Visible =
                tbPracticeName.Visible =
                chbPracticeActive.Checked =
                chbPracticeActive.Visible =
                chbIsInternalPractice.Visible =
                ddlPracticeManagers.Visible = true;
                cblDivision.Attributes.CssStyle.Add("style", "display:inline-block");
                cblDivision.CssClass = "PracticeAreasCblDivision";
                sdeCblDivision.Display = "inline-block;";
                cblProjectDivision.Attributes.CssStyle.Add("style", "display:inline-block");
                cblProjectDivision.CssClass = "PracticeAreasCblDivision";
                sdecblProjectDivision.Display = "inline-block;";
            }

        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            plusMakeVisible(true);
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate(valSummaryInsert.ValidationGroup);
                if (Page.IsValid)
                {
                    PracticesHelper.InsertPractice(tbPracticeName.Text, ddlPracticeManagers.SelectedValue,
                                                   chbPracticeActive.Checked, chbIsInternalPractice.Checked, tbAbbreviation.Text, cblDivision.SelectedItems, cblProjectDivision.SelectedItems);
                    mlInsertStatus.ShowInfoMessage(Resources.Controls.PracticeAddedSuccessfully);
                    Practices = null;
                    gvPractices.DataSource = Practices;
                    gvPractices.DataBind();
                    plusMakeVisible(true);
                }

            }
            catch (Exception exc)
            {
                mlInsertStatus.ShowErrorMessage(exc.Message);
            }
        }

        protected void cvPracticeName_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (IsPracticeAlreadyExisting(tbPracticeName.Text))
            {
                args.IsValid = false;
            }
        }

        protected void custPracticeManager_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlPracticeManagers.SelectedValue != "-1";
        }

        protected void gvPractices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var item = e.Row.DataItem as Practice;
            if (item != null)
            {
                var imgDelete = e.Row.FindControl("imgDelete") as ImageButton;
                var lblPracticeManager = e.Row.FindControl("lblPracticeManager") as Label;
                if (lblPracticeManager != null)
                {
                    lblPracticeManager.Text = item.PracticeOwner == null ? "Unassigned" : String.Format(PracticeMangagerNameFormat, item.PracticeOwner.PersonLastFirstName, item.PracticeOwner.Status.Name); //||  item.PracticeOwner.Status.Id != (int)PersonStatusType.Terminated ? String.Format(PracticeMangagerNameFormat, item.PracticeOwner.PersonLastFirstName, item.PracticeOwner.Status.Name) : "Unassigned";
                }
                var lblDivisions = e.Row.FindControl("lblDivisions") as Label;
                if (lblDivisions != null && !string.IsNullOrEmpty(item.DivisionIds))
                {
                    lblDivisions.Text = string.Empty;
                    var divisionIds = item.DivisionIds.Remove(item.DivisionIds.Length - 1).Split(',').Select(int.Parse).ToList();
                    foreach (int id in divisionIds)
                    {
                        lblDivisions.Text = lblDivisions.Text + DataHelper.GetDescription((PersonDivisionType)id) + ',';
                    }
                    lblDivisions.Text = lblDivisions.Text.Remove(lblDivisions.Text.Length - 1);
                }
                var lblProjectDivisions = e.Row.FindControl("lblProjectDivisions") as Label;
                if (lblProjectDivisions != null && !string.IsNullOrEmpty(item.ProjectDivisionIds))
                {
                    lblProjectDivisions.Text = string.Empty;
                    var divisionIds = item.ProjectDivisionIds.Remove(item.ProjectDivisionIds.Length - 1).Split(',').Select(int.Parse).ToList();
                    foreach (int id in divisionIds)
                    {
                        lblProjectDivisions.Text = lblProjectDivisions.Text + DataHelper.GetDescription((ProjectDivisionType)id) + ',';
                    }
                    lblProjectDivisions.Text = lblProjectDivisions.Text.Remove(lblProjectDivisions.Text.Length - 1);
                }
                if (imgDelete != null)
                {
                    imgDelete.Visible = !item.InUse;
                }
            }

            // Edit mode.
            if ((e.Row.RowState & DataControlRowState.Edit) != 0)
            {
                var chbIsActiveEd = e.Row.FindControl("chbIsActiveEd") as CheckBox;
                if (item.IsActiveCapabilitiesExists && item.IsActive)
                {
                    var imgUpdate = e.Row.FindControl("imgUpdate") as ImageButton;
                    imgUpdate.OnClientClick = string.Format("return showcapabilityActivePopup(\'{0}\',this);", chbIsActiveEd.ClientID);
                }
                DropDownList ddlActivePersons = e.Row.FindControl("ddlActivePersons") as DropDownList;
                if (ddlActivePersons != null)
                {
                    ddlActivePersons.Items.AddRange(PracticeManagersList);
                    string id = item.PracticeManagerId.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    if (ddlActivePersons.Items.FindByValue(id) != null)
                    {
                        ddlActivePersons.SelectedValue = id;
                    }
                    else
                    {
                        ddlActivePersons.SelectedIndex = 0;
                    }
                }
                ScrollingDropDown cblActiveDivisions = e.Row.FindControl("cblActiveDivisions") as ScrollingDropDown;
                ScrollingDropDown cblProjectDivisions = e.Row.FindControl("cblProjectDivisions") as ScrollingDropDown;
                if (cblActiveDivisions != null)
                {
                    DataHelper.FillPersonDivisionList(cblActiveDivisions, true);
                    string divisionIds = item.DivisionIds;
                    cblActiveDivisions.SelectedItems = divisionIds;
                }
                if (cblProjectDivisions != null)
                {
                    DataHelper.FillProjectDivisionList(cblProjectDivisions, false,true);
                    string divisionIds = item.ProjectDivisionIds;
                    cblProjectDivisions.SelectedItems = divisionIds;
                }
            }
        }

        protected void imgUpdate_OnClick(object sender, EventArgs e)
        {
            Page.Validate(valSummaryEdit.ValidationGroup);
            if (Page.IsValid)
            {
                var imgEdit = sender as ImageButton;
                var row = imgEdit.NamingContainer as GridViewRow;

                if (hdCapabilitiesInactivePopUpOperation.Value == "cancel")
                {
                    gvPractices.EditIndex = -1;
                    gvPractices.DataSource = Practices;
                    gvPractices.DataBind();
                    hdCapabilitiesInactivePopUpOperation.Value = "none";
                    return;
                }
                Practice practice = new Practice();
                int PracticeId = int.Parse(((HiddenField)row.FindControl("hdPracticeId")).Value);
                var tbEditPractice = row.FindControl("tbEditPractice") as TextBox;
                var tbEditAbbreviation = row.FindControl("tbEditAbbreviation") as TextBox;
                var chbIsActiveEd = row.FindControl("chbIsActiveEd") as CheckBox;
                var chbInternal = row.FindControl("chbInternal") as CheckBox;
                var ddlActivePersons = row.FindControl("ddlActivePersons") as DropDownList;
                var cblActiveDivisions = row.FindControl("cblActiveDivisions") as ScrollingDropDown;
                var cblProjectDivisions = row.FindControl("cblProjectDivisions") as ScrollingDropDown;
                practice.Id = PracticeId;
                practice.Name = tbEditPractice.Text;
                practice.Abbreviation = string.IsNullOrEmpty(tbEditAbbreviation.Text) ? null : tbEditAbbreviation.Text;
                practice.IsActive = chbIsActiveEd.Checked;
                practice.IsCompanyInternal = chbInternal.Checked;
                practice.PracticeOwner = new Person() { Id = int.Parse(ddlActivePersons.SelectedValue) };
                practice.DivisionIds = cblActiveDivisions.SelectedItems;
                practice.ProjectDivisionIds = cblProjectDivisions.SelectedItems;
                var oldPracticeObject = Practices.First(p => p.Id == practice.Id);

                bool isPageValid = true;

                string newPractice = practice.Name;
                string oldPractice = oldPracticeObject.Name.Trim();
                if (newPractice != oldPractice)
                {
                    if (IsPracticeAlreadyExisting(newPractice))
                    {
                        CustomValidator custValEditPractice = row.FindControl("custValEditPractice") as CustomValidator;
                        custValEditPractice.IsValid = false;
                        isPageValid = false;
                    }
                }

                string newPracticeAbbrivation = practice.Abbreviation != null ? practice.Abbreviation.Trim() : string.Empty;
                string oldPracticeAbbrivation = oldPracticeObject.Abbreviation != null ? oldPracticeObject.Abbreviation.Trim() : string.Empty;

                if (!string.IsNullOrEmpty(newPracticeAbbrivation) && newPracticeAbbrivation != oldPracticeAbbrivation)
                {
                    if (IsPracticeAbbrivationAlreadyExisting(newPracticeAbbrivation))
                    {
                        CustomValidator custValEditPracticeAbbreviation = row.FindControl("custValEditPracticeAbbreviation") as CustomValidator;
                        custValEditPracticeAbbreviation.IsValid = false;
                        isPageValid = false;
                    }
                }
                using (var serviceClient = new PracticeServiceClient())
                {
                    try
                    {
                        serviceClient.UpdatePractice(practice, DataHelper.CurrentPerson.Alias);
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
                Practices = null;
                gvPractices.EditIndex = -1;
                gvPractices.DataSource = Practices;
                gvPractices.DataBind();
                mlInsertStatus.ShowInfoMessage(PracticeUpdatedSuccessfully);
                hdCapabilitiesInactivePopUpOperation.Value = "none";
            }
        }

        protected void imgDelete_OnClick(object sender, EventArgs e)
        {
            var imgDelete = sender as ImageButton;
            var row = imgDelete.NamingContainer as GridViewRow;
            int PracticeId = int.Parse(((HiddenField)row.FindControl("hdPracticeId")).Value);
            using (var serviceClient = new PracticeServiceClient())
            {
                try
                {
                    serviceClient.RemovePractice(new Practice() { Id = PracticeId }, DataHelper.CurrentPerson.Alias);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
            Practices = null;
            gvPractices.DataSource = Practices;
            gvPractices.DataBind();
            mlInsertStatus.ShowInfoMessage(PracticeDeletedSuccessfully);
            hdCapabilitiesInactivePopUpOperation.Value = "none";
        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            hdCapabilitiesInactivePopUpOperation.Value = "none";
            gvPractices.EditIndex = -1;
            gvPractices.DataSource = Practices;
            gvPractices.DataBind();
        }

        protected void imgEdit_OnClick(object sender, EventArgs e)
        {
            //plusMakeVisible(true);
            hdCapabilitiesInactivePopUpOperation.Value = "none";
            var imgEdit = sender as ImageButton;
            var gvPracticeItem = imgEdit.NamingContainer as GridViewRow;
            gvPractices.EditIndex = gvPracticeItem.DataItemIndex;
            gvPractices.DataSource = Practices;
            gvPractices.DataBind();
            plusMakeVisible(true);


        }

        private bool IsPracticeAlreadyExisting(string newPractice)
        {
            newPractice = newPractice.Trim().Replace(" ", "<>").Replace("><", "").Replace("<>", " ");
            IEnumerable<PracticeExtended> practiceList = PracticesHelper.GetAllPractices();
            return practiceList.Any(practice => practice.Name.ToLowerInvariant() == newPractice.ToLowerInvariant());
        }

        private bool IsPracticeAbbrivationAlreadyExisting(string newPracticeAbbrivation)
        {
            newPracticeAbbrivation = newPracticeAbbrivation.Trim().Replace(" ", "<>").Replace("><", "").Replace("<>", " ");
            IEnumerable<PracticeExtended> practiceList = PracticesHelper.GetAllPractices();
            return practiceList.Any(practice => practice.Abbreviation.ToLowerInvariant() == newPracticeAbbrivation.ToLowerInvariant());
        }

        private void PopulateErrorPanel()
        {
            mpeErrorPanel.Show();
        }
    }
}

