using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using DataTransferObjects;
using PraticeManagement.Controls.Opportunities;
using PraticeManagement.Utils;

namespace PraticeManagement.Config
{
    public partial class OpurtunityPriorities : PracticeManagementPageBase
    {
        #region constants

        private const int DDL_OPPORTUNITY_PRIORITY_INDEX = 1;
        private const string OPPORTUNITY_PRIORITY_LIST_KEY = "OPPORTUNITY_PRIORITY_LIST_KEY";
        private const string popupMessage = "This priority is linked to existing opportunities already.  Changing this value will also change the status for all existing opportunities that are using the old value.";

        #endregion constants

        #region properties

        private OpportunityPriority[] OpportunityPriorityList
        {
            get
            {
                if (ViewState[OPPORTUNITY_PRIORITY_LIST_KEY] != null)
                {
                    return ViewState[OPPORTUNITY_PRIORITY_LIST_KEY] as OpportunityPriority[];
                }
                else
                {
                    var result = OpportunityPriorityHelper.GetOpportunityPriorities(true);
                    ViewState[OPPORTUNITY_PRIORITY_LIST_KEY] = result;
                    return result;
                }
            }
        }

        #endregion properties

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState.Remove(OPPORTUNITY_PRIORITY_LIST_KEY);
                DatabindOpportunityPriorities();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (gvOpportunityPriorities.Rows.Count == 1)
            {
                var imgDelete = gvOpportunityPriorities.Rows[0].FindControl("imgDeletePriority") as ImageButton;
                if (imgDelete != null)
                    imgDelete.Visible = false;
            }

            var count = GetCountOfInsertedPriorities();

            if (count == 0)
            {
                pnlInsertPriority.Visible = false;
            }
            else
            {
                pnlInsertPriority.Visible = true;
            }

        }

        private void DatabindOpportunityPriorities()
        {
            gvOpportunityPriorities.DataSource = OpportunityPriorityList;
            gvOpportunityPriorities.DataBind();
        }

        protected override void Display()
        {

        }

        protected void gvOpportunityPriorities_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            var opportunityPriority = e.Row.DataItem as OpportunityPriority;

            //Edit mode.
            if ((e.Row.RowState & DataControlRowState.Edit) != 0)
            {
                opportunityPriority.InUse = OpportunityPriorityHelper.IsOpportunityPriorityInUse(opportunityPriority.Id);

                DropDownList ddl = e.Row.Cells[DDL_OPPORTUNITY_PRIORITY_INDEX].FindControl("ddlOpportunityPriority") as DropDownList;
                if (ddl != null)
                {
                    List<OpportunityPriority> priorityList = OpportunityPriorityHelper.GetOpportunityPriorities(false).ToList();
                    priorityList.Add(new OpportunityPriority() { Id = opportunityPriority.Id, Priority = opportunityPriority.Priority });

                    OpportunityPriority[] priorityies = priorityList.AsQueryable().ToArray();

                    DataHelper.FillListDefault(ddl, "", priorityies, true, "Id", "Priority");

                    if (ddl.Items.FindByValue(opportunityPriority.Id.ToString()) != null)
                    {
                        ddl.SelectedValue = opportunityPriority.Id.ToString();
                    }
                    ddl.SortByText();

                    ddl.Attributes["SelectedValue"] = ddl.SelectedValue;
                }

                ImageButton imgbtn = e.Row.Cells[0].FindControl("imgUpdatePriority") as ImageButton;

                if (imgbtn != null)
                {
                    if (opportunityPriority.InUse)
                    {
                        imgbtn.OnClientClick = string.Format("return canShowConfirm('{0}');", popupMessage);
                    }
                }
            }

        }

        private int GetCountOfInsertedPriorities()
        {
            return OpportunityPriorityHelper.GetOpportunityPriorities(false).Count();
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            plusMakeVisible(false);
            OpportunityPriority[] priorityList = OpportunityPriorityHelper.GetOpportunityPriorities(false);
            DataHelper.FillListDefault(ddlInsertOpportunityPriority, "", priorityList, true, "Id", "Priority");
        }

        private void plusMakeVisible(bool isplusVisible)
        {
            if (isplusVisible)
            {
                btnPlus.Visible = true;
                btnInsert.Visible = false;
                btnCancel.Visible = false;
                ddlInsertOpportunityPriority.Items.Clear();
                ddlInsertOpportunityPriority.Enabled = false;
                tbInsertPriorityDescription.Text = "";
                tbInsertPriorityDescription.Enabled = false;
            }
            else
            {
                btnPlus.Visible = false;
                btnInsert.Visible = true;
                btnCancel.Visible = true;
                tbInsertPriorityDescription.Text = string.Empty;
                tbInsertPriorityDescription.Enabled = true;
                ddlInsertOpportunityPriority.Enabled = true;
            }

        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            plusMakeVisible(true);
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            OpportunityPriority opportunityPriority = new OpportunityPriority();
            opportunityPriority.Id = Convert.ToInt32(ddlInsertOpportunityPriority.SelectedValue);
            opportunityPriority.Description = tbInsertPriorityDescription.Text;
            OpportunityPriorityHelper.InsertOpportunityPriority(opportunityPriority);
            ViewState.Remove(OPPORTUNITY_PRIORITY_LIST_KEY);
            DatabindOpportunityPriorities();
            plusMakeVisible(true);
        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            gvOpportunityPriorities.EditIndex = -1;
            DatabindOpportunityPriorities();
        }

        protected void imgEditPriority_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            gvOpportunityPriorities.EditIndex = row.DataItemIndex;
            DatabindOpportunityPriorities();
        }

        protected void imgUpdatePriority_OnClick(object sender, EventArgs e)
        {
            ImageButton imgUpdate = sender as ImageButton;
            GridViewRow row = imgUpdate.NamingContainer as GridViewRow;

            int oldPriorityId = Convert.ToInt32(imgUpdate.Attributes["PriorityId"]);
            var ddl = row.FindControl("ddlOpportunityPriority") as DropDownList;
            var tbox = row.FindControl("tbEditPriorityDescription") as TextBox;

            OpportunityPriority opportunityPriority = new OpportunityPriority();
            opportunityPriority.Id = Convert.ToInt32(ddl.SelectedValue);
            opportunityPriority.Description = tbox.Text;

            OpportunityPriorityHelper.UpdateOpportunityPriority(oldPriorityId, opportunityPriority);
            gvOpportunityPriorities.EditIndex = -1;
            ViewState.Remove(OPPORTUNITY_PRIORITY_LIST_KEY);
            DatabindOpportunityPriorities();
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            int? updatedPriorityId = Convert.ToInt32(ddlOpportunityPriorities.SelectedValue);

            int deletedPriorityId = Convert.ToInt32(btnOK.Attributes["PriorityId"]);

            OpportunityPriorityHelper.DeleteOpportunityPriority(updatedPriorityId, deletedPriorityId);

            ViewState.Remove(OPPORTUNITY_PRIORITY_LIST_KEY);
            DatabindOpportunityPriorities();
        }

        protected void imgDeletePriority_OnClick(object sender, EventArgs e)
        {
            ImageButton imgDelete = sender as ImageButton;
            int priorityId = Convert.ToInt32(imgDelete.Attributes["PriorityId"]);

            var inUse = OpportunityPriorityHelper.IsOpportunityPriorityInUse(priorityId);

            imgDelete.Attributes["InUse"] = inUse.ToString();

            btnOK.Attributes["PriorityId"] = priorityId.ToString();

            if (inUse)
            {
                var opportunityPrioritiesList = OpportunityPriorityHelper.GetOpportunityPriorities(true).ToList();

                OpportunityPriority[] prioritiesList = opportunityPrioritiesList.AsQueryable().Where(op => op.Id != priorityId).ToArray();
                DataHelper.FillListDefault(ddlOpportunityPriorities, "", prioritiesList, true, "Id", "Priority");
                mpeOpportunityPriorities.Show();
            }
            else
            {
                OpportunityPriorityHelper.DeleteOpportunityPriority(null, priorityId);
                ViewState.Remove(OPPORTUNITY_PRIORITY_LIST_KEY);
                DatabindOpportunityPriorities();
            }

        }

        #endregion Methods

    }
}

