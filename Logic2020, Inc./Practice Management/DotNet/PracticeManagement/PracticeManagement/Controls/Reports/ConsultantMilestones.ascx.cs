using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Reports
{
    public partial class ConsultantMilestones : System.Web.UI.UserControl
    {
        #region Constants

        private const string UTILIZATION_COLUMN_NAME = "wutil";
        private const string PERSON_ID_PARAM = "personId";
        private const string START_DATE_PARAM = "startDate";
        private const string END_DATE_PARAM = "endDate";
        
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public string PersonId 
        { 
            set
            {
                odsConsultantMilestones.
                    SelectParameters[PERSON_ID_PARAM].DefaultValue = value;
            } 
        }

        public string StartDate
        {
            set
            {
                odsConsultantMilestones.
                    SelectParameters[START_DATE_PARAM].DefaultValue = value;
            }
        }

        public string EndDate
        {
            set
            {
                odsConsultantMilestones.
                    SelectParameters[END_DATE_PARAM].DefaultValue = value;
            }
        }

        public string PersonName
        {
            set
            {
                lblTitle.Text = value;
            }
        }

        protected void dvConsultantMilestones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                try
                {
                    int util = (int) DataBinder.Eval(e.Row.DataItem, UTILIZATION_COLUMN_NAME);
                    e.Row.BackColor = Utils.Coloring.GetColorByUtilization(util, util < 0);
                }
                catch
                {
                }
            }
        }
    }
}
