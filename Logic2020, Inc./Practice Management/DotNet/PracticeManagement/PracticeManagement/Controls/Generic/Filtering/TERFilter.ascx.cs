using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using PraticeManagement.Controls.TimeEntry;
using System.Web.Security;

namespace PraticeManagement.Controls.Generic.Filtering
{
    public partial class TERFilter : System.Web.UI.UserControl
    {
        public IList<int?> PersonIds 
        { 
            get 
            {
                return cblPersons.SelectedValues != null
                           ? cblPersons.SelectedValues.Select(i => new int?(i)).ToList()
                           : new List<int?>();
            } 
        }

        public IList<int?> ProjectIds
        {
            get
            {
                return cblProjects.SelectedValues != null
                           ? cblProjects.SelectedValues.Select(i => new int?(i)).ToList()
                           : new List<int?>();
            }
        }
        
        public int? TimeTypeId { get { return soTimeTypes.SelectedId; } }
        public int? MilestoneId { get { return soMilestones.SelectedId; } }

        public DateTime MilestoneDateFrom { get { return diWeek.FromDate.HasValue ? diWeek.FromDate.Value : DateTime.Now; } }
        public DateTime MilestoneDateTo { get { return diWeek.ToDate.HasValue ? diWeek.ToDate.Value : DateTime.Now; } }

        public DateTime? EntryDateFrom { get { return diEntered.FromDate; } }
        public DateTime? EntryDateTo { get { return diEntered.ToDate; } }
        public DateTime? ModifiedDateFrom { get { return diLastModified.FromDate; } }
        public DateTime? ModifiedDateTo { get { return diLastModified.ToDate; } }

        public string Notes { get { return tbNotes.Text; } }
        public string IsReviewed { get { return ddlIsReviewed.SelectedValue; } }

        public bool? IsChargable { get { return ParseBool(ddlIsChargable); } }
        public bool? IsCorrect { get { return ParseBool(ddlIsCorrect); } }
        public bool? IsProjectChargeable { get { return ParseBool(ddlIsProjectChargeable); } }

        public double ForecastedHoursFrom { get { return riForecasted.MinValue; } }
        public double ForecastedHoursTo { get { return riForecasted.MaxValue; } }
        public double ActualHoursFrom { get { return riActual.MinValue; } }
        public double ActualHoursTo { get { return riActual.MaxValue; } }

        public event EventHandler Update;
        public event EventHandler ResetAllFilters;

        private static bool? ParseBool(ListControl ddl)
        {
            try
            {
                return bool.Parse(ddl.SelectedValue);
            }
            catch
            {
                return null;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillTimeEntryProjectsList(cblProjects, Resources.Controls.AllProjects, null);
                CheckAllCheckboxes(cblProjects);

                if (Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName))
                {
                    DataHelper.FillTimeEntryPersonList(cblPersons, Resources.Controls.AllPersons, null);
                }
                else
                {
                    DataHelper.FillTimeEntryPersonList(cblPersons, Resources.Controls.AllPersons, null, string.Empty, DataHelper.CurrentPerson.Id);
                }
                CheckAllCheckboxes(cblPersons);

                Utils.Generic.InitStartEndDate(diWeek);
            }
            
                
        }
       
        private void CheckAllCheckboxes(ScrollingDropDown chbList)
        {
            foreach (ListItem targetItem in chbList.Items)
                if (targetItem != null)
                    targetItem.Selected = true;
        }

        protected void wsChoose_WeekChanged(object sender, WeekChangedEventArgs args)
        {}

        public void ResetFilters()
        {
            Utils.Generic.InitStartEndDate(diWeek);
            diLastModified.Reset();
            diEntered.Reset();
            riForecasted.Reset();
            riActual.Reset();

            ddlIsReviewed.SelectedValue = string.Empty;
            ddlIsCorrect.SelectedIndex = 0;
            ddlIsChargable.SelectedIndex = 0;
            ddlIsProjectChargeable.SelectedIndex = 0;

            soTimeTypes.SelectedValue = string.Empty;
            cblProjects.SelectedItems = null;
            cblPersons.SelectedItems = null;
            soMilestones.SelectedValue = string.Empty;

            tbNotes.Text = string.Empty;
        }

        protected void btnApply_OnClick(object sender, EventArgs e)
        {
            Utils.Generic.InvokeEventHandler(Update, sender, e);
        }

        protected void btnReset_OnClick(object sender, EventArgs e)
        {
            Utils.Generic.InvokeEventHandler(ResetAllFilters, sender, e);
            
        }
    }
}

