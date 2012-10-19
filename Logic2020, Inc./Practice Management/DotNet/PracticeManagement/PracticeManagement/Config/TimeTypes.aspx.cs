using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using PraticeManagement.TimeEntryService;
using PraticeManagement.Utils;
using PraticeManagement.Controls;
using System.Linq;
using PraticeManagement.TimeTypeService;
namespace PraticeManagement.Config
{
    public partial class TimeTypes : PracticeManagementPageBase
    {

        private System.Collections.Generic.List<TimeTypeRecord> AllTimeTypes
        {
            get
            {
                if (ViewState["AllTimeTypes"] == null)
                {
                    System.Collections.Generic.List<TimeTypeRecord> _AllTimeTypes = ServiceCallers.Custom.TimeType(t => t.GetAllTimeTypes()).ToList();
                    _AllTimeTypes = _AllTimeTypes.AsQueryable().Where(t => t.IsDefault || t.IsInternal || t.IsAdministrative).ToList();
                    ViewState["AllTimeTypes"] = _AllTimeTypes;
                }
                return (System.Collections.Generic.List<TimeTypeRecord>)ViewState["AllTimeTypes"];
            }
        }
        private void populatecontrols()
        {
            gvTimeTypes.DataSource = AllTimeTypes;
            gvTimeTypes.DataBind();
        }
        protected override void Display()
        {
            populatecontrols();
        }

        #region "Declarations"

        private const string CacheKey = "TimeTypeCachedData";
        private const string InsertAction = "Inserted";
        private const string DeleteAction = "Updated";
        private const string UpdateAction = "Deleted";

        #endregion "Declarations"
        protected void Page_Load(object sender, EventArgs e)
        {
            mlInsertStatus.ClearMessage();
        }

        protected void ibtnInsertTimeType_Click(object sender, EventArgs e)
        {
            rbIsDefault.Checked = true;
            showPlusIcon(false);
        }

        private void showPlusIcon(bool showPlus)
        {
            tbNewTimeType.Text = "New work type";
            ibtnInsertTimeType.Visible = showPlus;
            ibtnInsert.Visible = ibtnCancel.Visible = tbNewTimeType.Visible = rbIsDefault.Visible =
            rbIsInternal.Visible = rbIsAdministrative.Visible = rbIsActive.Visible = !showPlus;
            rbIsDefault.Checked = rbIsInternal.Checked = rbIsActive.Checked = !showPlus;

            if (!showPlus)
            {
                tbNewTimeType.Text = string.Empty;
            }
        }

        protected void cvIsDefaultOrInternal_Servervalidate(object sender, ServerValidateEventArgs e)
        {
            if (rbIsDefault.Checked || rbIsInternal.Checked || rbIsAdministrative.Checked)
            {
                e.IsValid = true;
            }
            else
            {
                e.IsValid = false;
            }

        }

        protected void cvIsDefaultOrInternalEdit_Servervalidate(object sender, ServerValidateEventArgs e)
        {
            var cvIsDefaultOrInternalEdit = sender as CustomValidator;
            var gvTimeTypesItem = cvIsDefaultOrInternalEdit.NamingContainer as GridViewRow;
            var rbIsDefault = gvTimeTypesItem.FindControl("rbIsDefault") as RadioButton;
            var rbIsInternal = gvTimeTypesItem.FindControl("rbIsInternal") as RadioButton;
            var rbIsAdministrative = gvTimeTypesItem.FindControl("rbIsAdministrative") as RadioButton;
            if (rbIsDefault.Checked || rbIsInternal.Checked || rbIsAdministrative.Checked)
            {
                e.IsValid = true;
            }
            else
            {
                e.IsValid = false;
            }

        }

        protected void ibtnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    TimeTypeRecord newTimeType = new TimeTypeRecord();
                    newTimeType.Name = tbNewTimeType.Text.Trim();
                    newTimeType.IsDefault = rbIsDefault.Checked;
                    newTimeType.IsInternal = rbIsInternal.Checked;
                    newTimeType.IsActive = rbIsActive.Checked;
                    newTimeType.IsAdministrative = rbIsAdministrative.Checked;
                    using (var serviceClient = new TimeTypeServiceClient())
                    {
                        serviceClient.AddTimeType(newTimeType);
                    }

                    ViewState.Remove("AllTimeTypes");
                    populatecontrols();

                    mlInsertStatus.ShowInfoMessage(Resources.Controls.TimeTypeAddedSuccessfully);
                    showPlusIcon(true);
                }
            }
            catch (Exception ex)
            {
                mlInsertStatus.ShowErrorMessage(ex.Message);
            }
        }

        protected void ibtnCancel_OnClick(object sender, EventArgs e)
        {
            showPlusIcon(true);
        }

        protected void gvTimeTypes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var tt = e.Row.DataItem as TimeTypeRecord;

                if (tt != null)
                {
                    var lastCell = (TableCell)e.Row.Cells[gvTimeTypes.Columns.Count - 1];
                    var imgDelete = lastCell.FindControl("imgDelete") as ImageButton;
                    if(imgDelete != null)
                        imgDelete.Visible = !tt.InUse;
                }

                var rbIsDefault = e.Row.FindControl("rbIsDefault") as RadioButton;
                var rbIsInternal = e.Row.FindControl("rbIsInternal") as RadioButton;
                var rbIsAdministrativerow = e.Row.FindControl("rbIsAdministrative") as RadioButton;
                rbIsDefault.GroupName = tt.Name + "GroupName";
                rbIsInternal.GroupName = tt.Name + "GroupName";
                rbIsAdministrativerow.GroupName = tt.Name + "GroupName";

            }
        }

        protected void imgEdit_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var gvTimeTypesItem = imgEdit.NamingContainer as GridViewRow;
            gvTimeTypes.DataSource = AllTimeTypes;
            gvTimeTypes.EditIndex = gvTimeTypesItem.DataItemIndex;
            gvTimeTypes.DataBind();
        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            gvTimeTypes.EditIndex = -1;
            gvTimeTypes.DataSource = AllTimeTypes;
            gvTimeTypes.DataBind();

        }

        protected void imgDelete_OnClick(object sender, EventArgs e)
        {
            var imgDelete = sender as ImageButton;
            var timetypeId = Convert.ToInt32(imgDelete.Attributes["timetypeId"]);
            try
            {
                using (var serviceClient = new TimeTypeServiceClient())
                {
                    serviceClient.RemoveTimeType(timetypeId);
                }

                ViewState.Remove("AllTimeTypes");
                mlInsertStatus.ShowInfoMessage("WorkType Deleted Sucessfully");
                populatecontrols();
            }
            catch(Exception ex)
            {
                if (ex.Message == "You cannot delete this Work type.Because, there are some time entries related to it.")
                {
                    mlInsertStatus.ShowErrorMessage("You cannot delete this Work type.Because, there are some time entries related to it.");
                }
                else
                {
                    throw ex;
                }
          
            }
        }

        protected void imgUpdate_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var row = imgEdit.NamingContainer as GridViewRow;
            Page.Validate("UpdateTimeType");
            if (Page.IsValid)
            {
                var tbName = row.FindControl("tbName") as TextBox;
                var hdfTimeTypeId = row.FindControl("hdfTimeTypeId") as HiddenField;
                if (IsTimeTypeAlreadyExisting(hdfTimeTypeId.Value, tbName.Text))
                {
                    CustomValidator cvUpdatedTimeTypeName = row.FindControl("cvUpdatedTimeTypeName") as CustomValidator;
                    cvUpdatedTimeTypeName.IsValid = false;
                }
                else
                {
                    using (var serviceClient = new TimeTypeServiceClient())
                    {
                        var timeType = AllTimeTypes.First(t => t.Id.ToString() == hdfTimeTypeId.Value);
                        timeType.Name = tbName.Text.Trim();

                        var rbIsDefault = row.FindControl("rbIsDefault") as RadioButton;
                        var rbIsInternal = row.FindControl("rbIsInternal") as RadioButton;
                        var rbIsActive = row.FindControl("rbIsActive") as CheckBox;
                        var rbIsAdministrativerow = row.FindControl("rbIsAdministrative") as CheckBox;
                        timeType.IsDefault = rbIsDefault.Checked;
                        timeType.IsInternal = rbIsInternal.Checked;
                        timeType.IsActive = rbIsActive.Checked;
                        timeType.IsAdministrative = rbIsAdministrativerow.Checked;
                        serviceClient.UpdateTimeType(timeType);
                    }
                    ViewState.Remove("AllTimeTypes");
                    mlInsertStatus.ShowInfoMessage("WorkType Updated Sucessfully");
                    gvTimeTypes.EditIndex = -1;
                    gvTimeTypes.DataSource = AllTimeTypes;
                    gvTimeTypes.DataBind();
                }
            }
        }

        private bool IsTimeTypeAlreadyExisting(string Id, string newTimeType)
        {
            foreach (TimeTypeRecord timeType in AllTimeTypes)
            {
                if (Id != timeType.Id.ToString() && timeType.Name.ToLower().Trim() == newTimeType.ToLower().Trim())
                {
                    return true;
                }
            }

            return false;
        }
    }
}

