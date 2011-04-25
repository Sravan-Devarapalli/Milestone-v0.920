using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.ContextObjects;

namespace PraticeManagement.Controls.Generic.Filtering
{
    public partial class OpportunityFilter : PracticeManagementFilterControl<OpportunityListContext>
    {
        protected void chbActiveOnly_CheckedChanged(object sender, EventArgs e)
        {
            FireFilterOptionsChanged();
        }

        protected void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FireFilterOptionsChanged();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ResetControlsForSearch();   //  Clean up filters
            FireFilterOptionsChanged(); //  Fire changes
        }

        private void ResetDropDowns()
        {
            ddlClients.SelectedIndex = 0;
            ddlSalespersons.SelectedIndex = 0;
        }

        public void ResetControlsForSearch()
        {
            ResetDropDowns();
            chbActiveOnly.Checked = false;

            UpdateFilter();
        }

        protected override void ResetControls()
        {
            ResetDropDowns();
            chbActiveOnly.Checked = true;
            txtSearch.Text = string.Empty;
        }

        protected override void InitControls()
        {
            chbActiveOnly.Checked = Filter.ActiveClientsOnly;
            txtSearch.Text = Filter.SearchText;
            ddlClients.SelectedValue = Filter.ClientId.HasValue ? Filter.ClientId.Value.ToString() : string.Empty;
            ddlSalespersons.SelectedValue = Filter.SalespersonId.HasValue ? Filter.SalespersonId.Value.ToString() : string.Empty;
        }

        protected override OpportunityListContext InitFilter()
        {
            return new OpportunityListContext
                        {
                            ActiveClientsOnly = chbActiveOnly.Checked,
                            SearchText = txtSearch.Text,
                            ClientId = GetDropDownValueWithDefault(ddlClients),
                            SalespersonId = GetDropDownValueWithDefault(ddlSalespersons)
                        };
        }
    }
}
