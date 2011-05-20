using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using PraticeManagement.TimeEntryService;
using PraticeManagement.Utils;
using PraticeManagement.Controls;

namespace PraticeManagement.Config
{
    public partial class TimeTypes : PracticeManagementPageBase
    {

        protected override void Display()
        {
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
            rbIsDefault.Checked = false;
            showPlusIcon(false);
        }

        private void showPlusIcon(bool showPlus)
        {
            ibtnInsertTimeType.Visible = showPlus;
            ibtnInsert.Visible = ibtnCancel.Visible = tbNewTimeType.Visible = rbIsDefault.Visible = !showPlus;

            if (!showPlus)
            {
                tbNewTimeType.Text = string.Empty;
            }
        }

        protected void ibtnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    TimeTypeRecord newTimeType = new TimeTypeRecord();
                    newTimeType.Name = tbNewTimeType.Text;
                    newTimeType.IsDefault = rbIsDefault.Checked;

                    using (var serviceClient = new TimeEntryServiceClient())
                    {
                        serviceClient.AddTimeType(newTimeType);
                    }
                    //TimeEntryHelper.AddTimeType(newTimeType);
                    gvTimeTypes.DataBind();

                    HttpContext.Current.Cache[CacheKey] = InsertAction;
                    mlInsertStatus.ShowInfoMessage(Resources.Controls.TimeTypeAddedSuccessfully);
                    tbNewTimeType.Text = "New time type";

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

        protected void odsTimeTypes_Updated(object sender, ObjectDataSourceStatusEventArgs e)
        {
            HttpContext.Current.Cache[CacheKey] = UpdateAction;
        }

        protected void odsTimeTypes_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
        {
            HttpContext.Current.Cache[CacheKey] = DeleteAction;
        }

        protected void gvTimeTypes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var tt = e.Row.DataItem as TimeTypeRecord;
                if (tt != null) e.Row.Cells[gvTimeTypes.Columns.Count - 1].Visible = !tt.InUse;
            }
        }

        protected void gvTimeTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Page.Validate("UpdateTimeType");
            if (!Page.IsValid)
            {
                e.Cancel = true;
            }
            else
            {
                string newTimeType = e.NewValues["Name"].ToString().Trim();
                string oldTimeType = e.OldValues["Name"].ToString().Trim();

                if (newTimeType != oldTimeType)
                {
                    if (IsTimeTypeAlreadyExisting(newTimeType))
                    {
                        CustomValidator cvUpdatedTimeTypeName = gvTimeTypes.Rows[e.RowIndex].FindControl("cvUpdatedTimeTypeName") as CustomValidator;
                        cvUpdatedTimeTypeName.IsValid = false;
                        e.Cancel = true;
                    }
                }
            }
        }

        private bool IsTimeTypeAlreadyExisting(string newTimeType)
        {
            using (TimeEntryServiceClient serviceClient = new TimeEntryServiceClient())
            {
                TimeTypeRecord[] timeTypesArray = serviceClient.GetAllTimeTypes();

                foreach (TimeTypeRecord timeType in timeTypesArray)
                {
                    if (timeType.Name.ToLower() == newTimeType.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

