using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Configuration;
namespace PraticeManagement.Config
{
    public partial class PracticeAreas : PracticeManagementPageBase
    {
        private const int DELETE_BUTTON_INDEX = 7;
        private const int PRACTICE_OWNER_INDEX = 6;
        private string PracticeUpdatedSuccessfully = "Practice updated successfully.";
        private string PracticeDeletedSuccessfully = "Practice deleted successfully.";

        protected void Page_Load(object sender, EventArgs e)
        {
            mlInsertStatus.ClearMessage();
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
                UpdateDeleteButton();
            }
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            trInsertPractice.Attributes["class"] = (gvPractices.Rows.Count % 2 == 0) ? "" : "alterrow";
        }

        protected override void Display()
        {
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            plusMakeVisible(false);
            gvPractices.EditIndex = -1;
            gvPractices.DataBind();
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

                if (Page.IsValid)
                {

                    PracticesHelper.InsertPractice(tbPracticeName.Text, ddlPracticeManagers.SelectedValue,
                                                   chbPracticeActive.Checked, chbIsInternalPractice.Checked, tbAbbreviation.Text);
                    mlInsertStatus.ShowInfoMessage(Resources.Controls.PracticeAddedSuccessfully);
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

        private void UpdateDeleteButton()
        {
            foreach (GridViewRow row in gvPractices.Rows)
            {

                if (row.RowType == DataControlRowType.DataRow)
                {
                    var item = row.DataItem as PracticeExtended;
                    if (item != null && item.InUse)
                    {
                        var cell = row.Cells[DELETE_BUTTON_INDEX];
                        cell.Enabled = false;
                        if (cell.Controls.Count > 0)
                        {
                            var deleteButton = cell.Controls[0] as ImageButton;
                            bool isVisible = true;
                            Boolean.TryParse(deleteButton.Attributes["InUse"], out isVisible);
                            deleteButton.Visible = isVisible;
                        }
                    }
                }
            }
        }

        protected void gvPractices_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var item = e.Row.DataItem as PracticeExtended;
            if (item != null && item.InUse)
            {
                var cell = e.Row.Cells[DELETE_BUTTON_INDEX];
                cell.Enabled = false;
                if (cell.Controls.Count > 0)
                {
                    var deleteButton = cell.Controls[0] as ImageButton;
                    deleteButton.Visible = false;
                    deleteButton.Attributes["InUse"] = false.ToString();
                }
            }
            if (item != null)
            {
                try
                {
                    ((ImageButton)e.Row.Cells[0].Controls[0]).ToolTip = "Edit Practice Area";
                }
                catch
                {
                    e.Row.Cells[0].ToolTip = "Edit Practice Area";
                }

                if (!item.InUse)
                {
                    try
                    {
                        ((ImageButton)e.Row.Cells[DELETE_BUTTON_INDEX].Controls[0]).ToolTip = "Delete Practice Area";
                    }
                    catch
                    {
                        e.Row.Cells[DELETE_BUTTON_INDEX].ToolTip = "Delete Practice Area";
                    }

                }
            }


            if (item == null)
            {
                return;
            }

            // Edit mode.
            if ((e.Row.RowState & DataControlRowState.Edit) != 0)
            {
                try
                {
                    ((ImageButton)e.Row.Cells[0].Controls[0]).ToolTip = "Confirm";
                    ((ImageButton)e.Row.Cells[0].Controls[2]).ToolTip = "Cancel";
                    e.Row.Cells[DELETE_BUTTON_INDEX].ToolTip = "";
                    var chbIsActiveEd = e.Row.FindControl("chbIsActiveEd") as CheckBox;
                    if (item.IsActiveCapabilitiesExists && item.IsActive)
                        ((ImageButton)e.Row.Cells[0].Controls[0]).OnClientClick = string.Format("return showcapabilityActivePopup(\'{0}\',this);", chbIsActiveEd.ClientID);
                }
                catch
                {
                    e.Row.Cells[0].ToolTip = "";
                }

                DropDownList ddl = e.Row.Cells[PRACTICE_OWNER_INDEX].FindControl("ddlActivePersons") as DropDownList;
                if (ddl != null)
                {
                    string id = item.PracticeManagerId.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    if (ddl.Items.FindByValue(id) != null)
                    {
                        ddl.SelectedValue = id;
                    }
                    else
                    {
                        // Inactive owner.
                        ddl.SelectedIndex = 0;
                    }
                }
            }

        }

        protected void gvPractices_OnRowUpdating(object sender, GridViewUpdateEventArgs e)     {
            if (hdCapabilitiesInactivePopUpOperation.Value == "cancel")
            {
                e.Cancel = true;
                gvPractices.EditIndex = -1;
                gvPractices.DataBind();
                hdCapabilitiesInactivePopUpOperation.Value = "none";
                return;
            }
            string newPractice = e.NewValues["Name"].ToString().Trim();
            string oldPractice = e.OldValues["Name"].ToString().Trim();
            if (newPractice != oldPractice)
            {
                if (IsPracticeAlreadyExisting(newPractice))
                {
                    CustomValidator custValEditPractice = gvPractices.Rows[e.RowIndex].FindControl("custValEditPractice") as CustomValidator;
                    custValEditPractice.IsValid = false;
                    e.Cancel = true;
                }
            }

            string newPracticeAbbrivation = e.NewValues["Abbreviation"] != null ? e.NewValues["Abbreviation"].ToString().Trim() : string.Empty;
            string oldPracticeAbbrivation = e.OldValues["Abbreviation"] != null ? e.OldValues["Abbreviation"].ToString().Trim() : string.Empty;

            if (!string.IsNullOrEmpty(newPracticeAbbrivation) && newPracticeAbbrivation != oldPracticeAbbrivation)
            {
                if (IsPracticeAbbrivationAlreadyExisting(newPracticeAbbrivation))
                {
                    CustomValidator custValEditPracticeAbbreviation = gvPractices.Rows[e.RowIndex].FindControl("custValEditPracticeAbbreviation") as CustomValidator;
                    custValEditPracticeAbbreviation.IsValid = false;
                    e.Cancel = true;
                }
            }

            DropDownList ddl = gvPractices.Rows[e.RowIndex].Cells[PRACTICE_OWNER_INDEX].FindControl("ddlActivePersons") as DropDownList;
            if (ddl != null)
            {
                if (ddl.SelectedValue != null)
                {
                    e.NewValues["PracticeManagerId"] = ddl.SelectedValue;
                }
            }
        }

        protected void gvPractices_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            mlInsertStatus.ShowInfoMessage(PracticeDeletedSuccessfully);
            hdCapabilitiesInactivePopUpOperation.Value = "none";
        }

        protected void gvPractices_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            hdCapabilitiesInactivePopUpOperation.Value = "none";
        }

        protected void gvPractices_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            mlInsertStatus.ShowInfoMessage(PracticeUpdatedSuccessfully);
            hdCapabilitiesInactivePopUpOperation.Value = "none";
        }

        protected void gvPractices_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            plusMakeVisible(true);
            hdCapabilitiesInactivePopUpOperation.Value = "none";
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

