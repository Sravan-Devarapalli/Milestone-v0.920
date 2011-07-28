using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.PracticeService;
using DataTransferObjects;
using PraticeManagement.ConfigurationService;
using PraticeManagement.Utils;

namespace PraticeManagement.Config
{
    public partial class Notes : PracticeManagementPageBase, IPostBackEventHandler
    {
        private const string PracticeList_KEY = "PRACTICE_LIST_KEY";

        public List<Practice> PracticeList
        {
            get
            {
                if (ViewState[PracticeList_KEY] != null)
                {
                    return ViewState[PracticeList_KEY] as List<Practice>;
                }

                using (var serviceClient = new PracticeServiceClient())
                {
                    try
                    {
                        var practices = serviceClient.GetPracticeList().AsQueryable().ToList();
                        ViewState[PracticeList_KEY] = practices;
                        return practices;
                    }
                    catch
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
            btnSave.Attributes["onclick"] = "GetpracticeIdsList();";
        }

        public void cblExemptNotes_DataBound(object sender, EventArgs e)
        {
        }

        public void cblMustEnterNotes_DataBound(object sender, EventArgs e)
        {

        }

        protected override bool ValidateAndSave()
        {
            var retValue = false;

            Page.Validate(vsumNotes.ValidationGroup);
            if (Page.IsValid)
            {

                SettingsHelper.SaveResourceKeyValuePairItem(SettingsType.Application, Constants.ResourceKeys.NotesRequiredForTimeEntryKey, chbNotesRequired.Checked.ToString());

                using (var serviceClient = new ConfigurationServiceClient())
                {
                    try
                    {
                        serviceClient.SavePracticesIsNotesRequiredDetails(hdnMustEnterNotes.Value, hdnExemptNotes.Value);

                        retValue = true;
                        ClearDirty();

                        ViewState.Remove(PracticeList_KEY);
                        btnSave.Enabled = false;

                    }
                    catch
                    {
                        serviceClient.Abort();
                        retValue = false;
                        throw;
                    }
                }
            }

            return retValue;
        }

        private void AddAttributesTocblExemptNotes()
        {
            foreach (ListItem item in cblExemptNotes.Items)
            {
                item.Attributes["practiceid"] = item.Value;
                item.Attributes["practicename"] = item.Text;
                if (!chbNotesRequired.Checked)
                {
                    item.Enabled = false;
                }
            }
        }

        private void AddAttributesTocblMustEnterNotes()
        {
            foreach (ListItem item in cblMustEnterNotes.Items)
            {
                item.Attributes["practiceid"] = item.Value;
                item.Attributes["practicename"] = item.Text;
            }
        }

        public void FillMustEnterNotesPracticesList()
        {
            var mustEnterNotesList = PracticeList.Where(practice => practice.IsNotesRequiredForTimeEnry == true);
            cblMustEnterNotes.DataSource = mustEnterNotesList;
            cblMustEnterNotes.DataBind();
        }

        public void FillExemptNotesPracticesList()
        {
            var exemptNotesList = PracticeList.Where(practice => practice.IsNotesRequiredForTimeEnry == false);
            cblExemptNotes.DataSource = exemptNotesList;
            cblExemptNotes.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool IsSavedWithoutErrors = ValidateAndSave();
            if (IsSavedWithoutErrors)
            {
                ViewState.Remove(PracticeList_KEY);
                PopulateControls();
                mlConfirmation.ShowInfoMessage("Changes saved successfully");
                ClearDirty();
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            AddAttributesTocblExemptNotes();
            AddAttributesTocblMustEnterNotes();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "GetpracticeIdsList", string.Format("GetpracticeIdsList();EnableOrDisableButtons('{0}');changeAlternateitemscolrsForCBL();", chbNotesRequired.Checked.ToString().ToLowerInvariant()), true);
        }

        private void PopulateControls()
        {
            bool result = true;
            var val = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.NotesRequiredForTimeEntryKey);
            bool.TryParse(val, out result);
            chbNotesRequired.Checked = result;
            FillMustEnterNotesPracticesList();
            FillExemptNotesPracticesList();
        }

        protected override void Display()
        {
            ViewState.Remove(PracticeList_KEY);
            PopulateControls();

        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            if (ValidateAndSave())
            {
                Redirect(eventArgument);
            }
        }

        #endregion

    }
}
