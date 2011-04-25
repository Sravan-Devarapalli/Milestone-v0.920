using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Utils;
using DataTransferObjects.TimeEntry;

namespace PraticeManagement
{
    public partial class TimeEntrySettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnInsertTimeType_Click(object sender, EventArgs e)
        {
            TimeEntryHelper.AddTimeType(tbNewTimeType.Text);
            gvTimeTypes.DataBind();
        }

        protected void gvTimeTypes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TimeTypeRecord tt = e.Row.DataItem as TimeTypeRecord;
                e.Row.Cells[gvTimeTypes.Columns.Count - 1].Enabled = !tt.InUse;
            }            
        }
    }
}

