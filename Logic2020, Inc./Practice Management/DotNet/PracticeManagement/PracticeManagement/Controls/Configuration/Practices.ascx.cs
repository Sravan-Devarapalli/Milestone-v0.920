using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Linq;

namespace PraticeManagement.Controls.Configuration
{
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;

    public partial class Practices : System.Web.UI.UserControl
    {
        private const int DELETE_BUTTON_INDEX = 6;
        private const int PRACTICE_OWNER_INDEX = 5;
        //protected bool _userIsAdmin;

        protected void Page_Load(object sender, EventArgs e)
        {
            mlInsertStatus.ClearMessage();
            //_userIsAdmin = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            plusMakeVisible(false);
        }

        private void plusMakeVisible(bool isplusVisible)
        {
            if (isplusVisible)
            {
                btnPlus.Visible = true;
                btnInsert.Visible = false;
                btnCancel.Visible = false;
                tbPracticeName.Visible = false;
                chbPracticeActive.Visible = false;
                chbIsInternalPractice.Visible = false;
                ddlPracticeManagers.Visible = false;
            }
            else
            {
                btnPlus.Visible = false;
                btnInsert.Visible = true;
                btnCancel.Visible = true;
                tbPracticeName.Text = string.Empty;
                tbPracticeName.Visible = true;
                chbPracticeActive.Checked = true;
                chbPracticeActive.Visible = true;
                chbIsInternalPractice.Checked = false;
                chbIsInternalPractice.Visible = true;
                ddlPracticeManagers.Visible = true;
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
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
                                                   chbPracticeActive.Checked, chbIsInternalPractice.Checked);
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

        protected void gvPractices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
           
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var item = e.Row.DataItem as PracticeExtended;
            if (item != null && item.InUse)
            {
                var cell = e.Row.Cells[DELETE_BUTTON_INDEX];
                cell.Enabled = false;
                cell.Visible = false;                
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
                }
                catch
                {
                    e.Row.Cells[0].ToolTip = "";
                }

                DropDownList ddl = e.Row.Cells[PRACTICE_OWNER_INDEX].FindControl("ddlActivePersons") as DropDownList;
                if (ddl != null)
                {
                    string id = item.PracticeManagerId.ToString(CultureInfo.InvariantCulture);
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

        protected void gvPractices_OnRowUpdating(object sender,GridViewUpdateEventArgs e)
        {
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

            DropDownList ddl = gvPractices.Rows[e.RowIndex].Cells[PRACTICE_OWNER_INDEX].FindControl("ddlActivePersons") as DropDownList;
            if (ddl != null)
            {
                if (ddl.SelectedValue != null)
                {
                    e.NewValues["PracticeManagerId"] = ddl.SelectedValue;
                }
            }
        }

        private bool IsPracticeAlreadyExisting(string newPractice)
        {
            IEnumerable<PracticeExtended> practiceList = PracticesHelper.GetAllPractices();
            return practiceList.Any(practice => practice.Name == newPractice);
        }
    }
}

