using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;

namespace PraticeManagement.Config
{
    public partial class PracticeCapabilities : System.Web.UI.Page
    {
        #region Constants

        private const string HdPracticeId = "hdPracticeId";
        private const string HdCapabilityId = "hdCapabilityId";
        private const string lblDeleteId = "lblDelete";
        private const string ImgPlus = "imgPlus";
        private const string ImgEdit = "imgEdit";
        private const string ImgUpdate = "imgUpdate";
        private const string ImgCancel = "imgCancel";
        private const string ImgDelete = "imgDelete";
        private const string TbInsertCapabilityName = "tbInsertCapabilityName";
        private const string TbEditCapabilityName = "tbEditCapabilityName";
        private const string chkInsertActive = "chkInsertActive";
        private const string chkEditActive = "chkEditActive";
        private const string RvInsertCapability = "rvInsertCapability";
        private const string RvEditCapability = "rvEditCapability";
        private const string LbCapabilityName = "lbCapabilityName";
        private const string IsInsertAttribute = "IsInsert";
        private const string UpdateSucess = "Capability updated successfully.";
        private const string InsertSucess = "Capability added successfully.";
        private const string DeleteSucess = "Capability deleted successfully.";
        private const string EditCapabilityId_Key = "EditCapabilityId_Key";
        private const string InsertPracticeId_Key = "InsertPracticeId_Key";
        private const string RepPracticeCapabilitiesId = "repPracticeCapabilities";
        private const string InUseAttribute = "InUse";
        private const string PracticeNameWithAbbrevationFormat = " <i>{0} - {1}</i> ({2})";
        private const string PracticeNameWithoutAbbrevationFormat = " <i>{0}</i> ({1})";
        private const string PrevValueForCheckBox = "PrevValue";

        #endregion

        #region Properties

        public int EditCapabilityId
        {
            get
            {
                if (ViewState[EditCapabilityId_Key] == null)
                {
                    ViewState[EditCapabilityId_Key] = -1;
                }
                return (int)ViewState[EditCapabilityId_Key];
            }
            set
            {
                ViewState[EditCapabilityId_Key] = value;
            }
        }

        public int InsertPracticeId
        {
            get
            {
                if (ViewState[InsertPracticeId_Key] == null)
                {
                    ViewState[InsertPracticeId_Key] = -1;
                }
                return (int)ViewState[InsertPracticeId_Key];
            }
            set
            {
                ViewState[InsertPracticeId_Key] = value;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                PopulateData();
            }
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
            }
        }

        #endregion

        #region Control Events

        protected string GetPracticeLable(string practicename, bool isActive, string abbreviation)
        {
            if (string.IsNullOrEmpty(abbreviation))
            {
                return string.Format(PracticeNameWithoutAbbrevationFormat, practicename, isActive ? "Active" : "InActive");
            }
            else
            {
                return string.Format(PracticeNameWithAbbrevationFormat, practicename, abbreviation, isActive ? "Active" : "InActive");
            }
        }

        protected void imgPlus_OnClick(object sender, EventArgs e)
        {
            var imgPlus = sender as ImageButton;
            var row = imgPlus.NamingContainer as RepeaterItem;
            DisableEditRows(false);
            ShowPlusIcon(row, false);
            DisableEditRows(true);
        }

        protected void imgEdit_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var row = imgEdit.NamingContainer as RepeaterItem;
            var hdCapabilityId = row.FindControl(HdCapabilityId) as HiddenField;
            DisableEditRows(true);
            ShowEditRow(row, true);
            DisableEditRows(false);
        }

        protected void imgUpdate_OnClick(object sender, EventArgs e)
        {
            try
            {
                var imgUpdate = sender as ImageButton;
                bool isInsert = imgUpdate.Attributes[IsInsertAttribute] == "True";

                Page.Validate(isInsert ? valSummaryInsert.ValidationGroup : valSummaryEdit.ValidationGroup);
                if (Page.IsValid)
                {
                    var row = imgUpdate.NamingContainer as RepeaterItem;
                    PracticeCapability capability = PopulateCapability(row, isInsert);
                    if (!isInsert)
                        ServiceCallers.Custom.Practice(p => p.CapabilityUpdate(capability, DataHelper.CurrentPerson.Alias));
                    else
                        ServiceCallers.Custom.Practice(p => p.CapabilityInsert(capability, DataHelper.CurrentPerson.Alias));
                    PopulateData();
                    mlInsertStatus.ShowInfoMessage(isInsert ? InsertSucess : UpdateSucess);
                }
            }
            catch (Exception ex)
            {
                mlInsertStatus.ShowErrorMessage(ex.Message);
            }

        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            var imgCancel = sender as ImageButton;
            bool isInsert = imgCancel.Attributes[IsInsertAttribute] == true.ToString();
            var row = imgCancel.NamingContainer as RepeaterItem;
            if (!isInsert)
            {
                ShowEditRow(row, false);
            }
            else
            {
                ShowPlusIcon(row, true);
            }
        }

        protected void imgDelete_OnClick(object sender, EventArgs e)
        {
            try
            {
                var imgDelete = sender as ImageButton;
                var row = imgDelete.NamingContainer as RepeaterItem;
                var hdCapabilityId = row.FindControl(HdCapabilityId) as HiddenField;
                int capabilityId = int.Parse(hdCapabilityId.Value);
                ServiceCallers.Custom.Practice(p => p.CapabilityDelete(capabilityId, DataHelper.CurrentPerson.Alias));
                PopulateData();
                mlInsertStatus.ShowInfoMessage(DeleteSucess);
                EditCapabilityId = InsertPracticeId = -1;
            }
            catch (Exception ex)
            {
                mlInsertStatus.ShowErrorMessage(ex.Message);
            }

        }

        #endregion

        #region Methods

        private void ShowEditRow(RepeaterItem row, bool isEditOperation)
        {
            var hdCapabilityId = row.FindControl(HdCapabilityId) as HiddenField;
            var imgEdit = row.FindControl(ImgEdit) as ImageButton;
            var imgUpdate = row.FindControl(ImgUpdate) as ImageButton;
            var imgCancel = row.FindControl(ImgCancel) as ImageButton;
            var imgDelete = row.FindControl(ImgDelete) as ImageButton;
            var lbCapabilityName = row.FindControl(LbCapabilityName) as Label;
            var tbCapabilityName = row.FindControl(TbEditCapabilityName) as TextBox;
            var rvEditCapability = row.FindControl(RvEditCapability) as RequiredFieldValidator;
            var lblDelete = row.FindControl(lblDeleteId) as Label;
            var chkActive = row.FindControl(chkEditActive) as CheckBox;
            ShowDeleteButton(imgDelete, !isEditOperation, lblDelete);
            chkActive.Enabled = imgUpdate.Visible = imgCancel.Visible = tbCapabilityName.Visible = rvEditCapability.Enabled = isEditOperation;
            bool prevValue = false;
            Boolean.TryParse(chkActive.Attributes[PrevValueForCheckBox], out prevValue);
            chkActive.Checked = prevValue;
            imgEdit.Visible = lbCapabilityName.Visible = !isEditOperation;
            tbCapabilityName.Text = lbCapabilityName.Text;
            EditCapabilityId = isEditOperation ? int.Parse(hdCapabilityId.Value) : -1;
        }

        private void ShowPlusIcon(RepeaterItem row, bool show)
        {
            var imgPlus = row.FindControl(ImgPlus) as ImageButton;
            var imgUpdate = row.FindControl(ImgUpdate) as ImageButton;
            var imgCancel = row.FindControl(ImgCancel) as ImageButton;
            var tbCapabilityName = row.FindControl(TbInsertCapabilityName) as TextBox;
            var chkActive = row.FindControl(chkInsertActive) as CheckBox;
            var hdPracticeId = row.FindControl(HdPracticeId) as HiddenField;
            var rvCapability = row.FindControl(RvInsertCapability) as RequiredFieldValidator;
            chkActive.Visible= imgUpdate.Visible = imgCancel.Visible = tbCapabilityName.Visible = rvCapability.Enabled = !show;
            chkActive.Checked = true;
            imgPlus.Visible = show;
            tbCapabilityName.Text = string.Empty;
            InsertPracticeId = !show ? int.Parse(hdPracticeId.Value) : -1;
        }

        private void DisableEditRows(bool isInsertOperation)
        {
            if (isInsertOperation)
            {
                if (EditCapabilityId > -1)
                {
                    foreach (RepeaterItem repCap in repPractices.Items)
                    {
                        if (repCap.ItemType == ListItemType.Item || repCap.ItemType == ListItemType.AlternatingItem)
                        {
                            var repPracticeCapabilities = repCap.FindControl(RepPracticeCapabilitiesId) as Repeater;
                            foreach (RepeaterItem repPra in repPracticeCapabilities.Items)
                            {
                                if (repPra.ItemType == ListItemType.Item || repPra.ItemType == ListItemType.AlternatingItem)
                                {
                                    var hdCapabilityId = repPra.FindControl(HdCapabilityId) as HiddenField;
                                    if (int.Parse(hdCapabilityId.Value) == EditCapabilityId)
                                    {
                                        ShowEditRow(repPra, false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (InsertPracticeId > -1)
                {
                    foreach (RepeaterItem repCap in repPractices.Items)
                    {
                        if (repCap.ItemType == ListItemType.Item || repCap.ItemType == ListItemType.AlternatingItem)
                        {
                            var hdPracticeId = repCap.FindControl(HdPracticeId) as HiddenField;
                            if (int.Parse(hdPracticeId.Value) == InsertPracticeId)
                            {
                                ShowPlusIcon(repCap, true);
                            }
                        }
                    }
                }
            }
        }

        private void PopulateErrorPanel()
        {
            mpeErrorPanel.Show();
        }

        private PracticeCapability PopulateCapability(RepeaterItem row, bool isInsert)
        {
            PracticeCapability capability = new PracticeCapability();
            var tbCapabilityName = row.FindControl(isInsert ? TbInsertCapabilityName : TbEditCapabilityName) as TextBox;
            var chkActive = row.FindControl(isInsert ? chkInsertActive : chkEditActive) as CheckBox;
            var hdCapabilityId = row.FindControl(HdCapabilityId) as HiddenField;
            var hdPracticeId = row.FindControl(HdPracticeId) as HiddenField;
            capability.Name = tbCapabilityName.Text.Trim();
            capability.IsActive = chkActive.Checked;
            capability.CapabilityId = isInsert ? 0 : int.Parse(hdCapabilityId.Value);
            capability.PracticeId = isInsert ? int.Parse(hdPracticeId.Value) : 0;
            return capability;
        }

        private void PopulateData()
        {
            var practices = ServiceCallers.Custom.Practice(p => p.PracticeListAllWithCapabilities());
            repPractices.DataSource = practices;
            repPractices.DataBind();
        }

        private void ShowDeleteButton(ImageButton imgBtn, bool show, Label lblDelete)
        {
            string inUseString = imgBtn.Attributes[InUseAttribute];
            bool inUse = true;
            bool.TryParse(inUseString, out inUse);
            imgBtn.Visible = !inUse && show;
            lblDelete.Visible = !imgBtn.Visible;
        }

        #endregion

    }
}

