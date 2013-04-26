using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.Utils;

namespace PraticeManagement.Config
{
    public partial class Titles : System.Web.UI.Page
    {
        #region constants

        private const int TITLETYPE_CELL_INDEX = 2;
        private const string UpdateSucess = "Salary Brand and PTO Accrual updated successfully.";
        private const string InsertSucess = "Salary Brand and PTO Accrual added successfully.";
        private const string DeleteSucess = "Salary Brand and PTO Accrual deleted successfully.";

        #endregion constants

        private Title[] salaryBands;
        private TitleType[] salaryBandsTitleTypes;

        #region properties

        private Title[] SalaryBands
        {
            get
            {
                if (salaryBands == null)
                {
                    salaryBands = ServiceCallers.Custom.Title(t => t.GetAllTitles());
                }
                return salaryBands;
            }
        }

        private TitleType[] SalaryBandsTitleTypes
        {
            get
            {
                if (salaryBandsTitleTypes == null)
                {
                    salaryBandsTitleTypes = SettingsHelper.GetTitleTypes();
                }
                return salaryBandsTitleTypes;
            }
        }

        #endregion properties

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBindTitles();
            }
            mlInsertStatus.ClearMessage();

        }

        private void DataBindTitles()
        {
            gvTitles.DataSource = SalaryBands;
            gvTitles.DataBind();
        }


        public void imgEditTitle_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            gvTitles.EditIndex = row.DataItemIndex;
            DataBindTitles();
            MakePlusVisible(true);
        }

        public void imgUpdateTitle_OnClick(object sender, EventArgs e)
        {
            try
            {
                Page.Validate(valSummaryEditTitle.ValidationGroup);
                if (Page.IsValid)
                {

                    ImageButton imgUpdate = sender as ImageButton;
                    GridViewRow row = imgUpdate.NamingContainer as GridViewRow;

                    var tbTitleName = row.FindControl("tbEditTitleName") as TextBox;
                    var oldTitleName = tbTitleName.Attributes["OldName"].ToLower().Replace(" ", "");
                    var newTitleName = tbTitleName.Text.ToLower().Replace(" ", "");
                    var tbSortOrder = row.FindControl("tbEditSortOrder") as TextBox;
                    var oldSortOrder = Convert.ToInt32(tbSortOrder.Attributes["OldSortOrder"]);
                    var newSortOrder = Convert.ToInt32(tbSortOrder.Text);
                    CustomValidator cvUniqueTitle = row.FindControl("cvUniqueTitle") as CustomValidator;
                    CustomValidator cvUniqueSortOrder = row.FindControl("cvUniqueSortOrder") as CustomValidator;

                    if (oldTitleName != newTitleName && IsTitleAlreadyExisting(tbTitleName.Text))
                    {
                        cvUniqueTitle.IsValid = false;
                    }
                    if (oldSortOrder != newSortOrder && IsSortOrderAlreadyExisting(newSortOrder))
                    {
                        cvUniqueSortOrder.IsValid = false;
                    }
                    if (cvUniqueTitle.IsValid && cvUniqueSortOrder.IsValid)
                    {
                        int titleId = Convert.ToInt32(imgUpdate.Attributes["TitleId"]);
                        var tbPTOAccrual = row.FindControl("tbEditPTOAccrual") as TextBox;
                        var tbMinimumSalary = row.FindControl("tbEditMinimumSalary") as TextBox;
                        var tbMaximumSalary = row.FindControl("tbEditMaximumSalary") as TextBox;
                        var ddl = row.FindControl("ddlTitleType") as DropDownList;
                        var pTOAccrual = (tbPTOAccrual.Text != string.Empty) ? Convert.ToInt32(tbPTOAccrual.Text) : 0;
                        var minimumSalary = (tbMinimumSalary.Text != string.Empty) ? (int?)Convert.ToInt32(tbMinimumSalary.Text) : null;
                        var maximumSalary = (tbMaximumSalary.Text != string.Empty) ? (int?)Convert.ToInt32(tbMaximumSalary.Text) : null;

                        ServiceCallers.Custom.Title(t => t.TitleUpdate(titleId, tbTitleName.Text, Convert.ToInt32(ddl.SelectedValue), newSortOrder, pTOAccrual, minimumSalary, maximumSalary, User.Identity.Name));
                        gvTitles.EditIndex = -1;
                        salaryBands = null;
                        DataBindTitles();
                        mlInsertStatus.ShowInfoMessage(UpdateSucess);
                    }
                }
            }
            catch (Exception ex)
            {
                mlInsertStatus.ShowErrorMessage(ex.Message);
            }
        }

        public void imgCancel_OnClick(object sender, EventArgs e)
        {
            gvTitles.EditIndex = -1;
            DataBindTitles();
        }



        public void imgDelete_OnClick(object sender, EventArgs e)
        {
            try
            {
                ImageButton imgDelete = sender as ImageButton;
                int titleId = Convert.ToInt32(imgDelete.Attributes["TitleId"]);
                ServiceCallers.Custom.Title(t => t.TitleDelete(titleId, User.Identity.Name));
                salaryBands = null;
                DataBindTitles();
                mlInsertStatus.ShowInfoMessage(DeleteSucess);
            }
            catch (Exception ex)
            {
                mlInsertStatus.ShowErrorMessage(ex.Message);
            }
        }

        public void btnPlus_Click(object sender, EventArgs e)
        {
            MakePlusVisible(false);
            DataHelper.FillListDefault(ddlInsertTitleType, "", SalaryBandsTitleTypes, true, "TitleTypeId", "TitleTypeName");
            gvTitles.EditIndex = -1;
            DataBindTitles();
        }

        public void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate(valSummaryAddTitle.ValidationGroup);
                if (Page.IsValid)
                {
                    CheckUniqueTitleAndSortOrder();
                    if (cvInsertUniqueTitle.IsValid && cvInsertUniqueSortOrder.IsValid)
                    {
                        var title = tbInsertTitleName.Text;
                        var titleTypeId = Convert.ToInt32(ddlInsertTitleType.SelectedValue);
                        var sortOrder = Convert.ToInt32(tbInsertSortOrder.Text);

                        var pTOAccrual = (tbInsertPTOAccrual.Text != string.Empty) ? Convert.ToInt32(tbInsertPTOAccrual.Text) : 0;
                        var minimumSalary = (tbInsertMinimumSalary.Text != string.Empty) ? (int?)Convert.ToInt32(tbInsertMinimumSalary.Text) : null;
                        var maximumSalary = (tbInsertMaximumSalary.Text != string.Empty) ? (int?)Convert.ToInt32(tbInsertMaximumSalary.Text) : null;

                        ServiceCallers.Custom.Title(t => t.TitleInset(title, titleTypeId, sortOrder, pTOAccrual, minimumSalary, maximumSalary, User.Identity.Name));
                        salaryBands = null;
                        DataBindTitles();
                        MakePlusVisible(true);
                        mlInsertStatus.ShowInfoMessage(InsertSucess);
                    }
                }
            }
            catch (Exception ex)
            {
                mlInsertStatus.ShowErrorMessage(ex.Message); 
            }
        }

        public void CheckUniqueTitleAndSortOrder()
        {
            if (IsTitleAlreadyExisting(tbInsertTitleName.Text))
            {
                cvInsertUniqueTitle.IsValid = false;
            }
            if (IsSortOrderAlreadyExisting(Convert.ToInt32(tbInsertSortOrder.Text)))
            {
                cvInsertUniqueSortOrder.IsValid = false;
            }
        }

        public void btnCancel_OnClick(object sender, EventArgs e)
        {
            MakePlusVisible(true);
        }

        private void MakePlusVisible(bool isPlusVisible)
        {
            if (isPlusVisible)
            {
                btnCancel.Visible = false;
                btnInsert.Visible = false;
                btnPlus.Visible = true;
                tbInsertTitleName.Text = string.Empty;
                tbInsertTitleName.Enabled = false;
                tbInsertPTOAccrual.Text = string.Empty;
                tbInsertPTOAccrual.Enabled = false;
                tbInsertMinimumSalary.Text = string.Empty;
                tbInsertMinimumSalary.Enabled = false;
                tbInsertMaximumSalary.Text = string.Empty;
                tbInsertMaximumSalary.Enabled = false;
                tbInsertSortOrder.Text = string.Empty;
                tbInsertSortOrder.Enabled = false;
                ddlInsertTitleType.Items.Clear();
                ddlInsertTitleType.Enabled = false;
            }
            else
            {
                btnPlus.Visible = false;
                btnInsert.Visible = true;
                btnCancel.Visible = true;
                tbInsertTitleName.Text = string.Empty;
                tbInsertTitleName.Enabled = true;
                tbInsertPTOAccrual.Text = string.Empty;
                tbInsertPTOAccrual.Enabled = true;
                tbInsertMinimumSalary.Text = string.Empty;
                tbInsertMinimumSalary.Enabled = true;
                tbInsertMaximumSalary.Text = string.Empty;
                tbInsertMaximumSalary.Enabled = true;
                tbInsertSortOrder.Text = string.Empty;
                tbInsertSortOrder.Enabled = true;
                ddlInsertTitleType.Enabled = true;

            }
        }


        protected void gvTitles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            var title = e.Row.DataItem as Title;

            // Edit mode.
            if ((e.Row.RowState & DataControlRowState.Edit) != 0)
            {
                DropDownList ddl = e.Row.Cells[TITLETYPE_CELL_INDEX].FindControl("ddlTitleType") as DropDownList;
                if (ddl != null)
                {
                    DataHelper.FillListDefault(ddl, "", SalaryBandsTitleTypes, true, "TitleTypeId", "TitleTypeName");
                }
                ddl.SelectedValue = title.TitleType.TitleTypeId.ToString();

                ddl.Attributes["SelectedValue"] = ddl.SelectedValue;
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == gvTitles.EditIndex)
                {
                    TextBox txtTitle = (TextBox)e.Row.FindControl("tbEditTitleName");
                    if (txtTitle.Text == "Senior Manager")
                    {
                        txtTitle.Enabled = false;
                    }
                }
            }

        }

        protected string GetCurrencyFormat(int? value)
        {
            return value.HasValue ? value.Value.ToString(Constants.Formatting.CurrencyFormat) : "";
        }

        protected bool IsDeleteButtonVisible(bool value)
        {
            return !value;
        }

        private bool IsTitleAlreadyExisting(string title)
        {
            return SalaryBands.Any(p => p.TitleName.ToLower().Replace(" ", "") == title.ToLower().Replace(" ", ""));
        }

        private bool IsSortOrderAlreadyExisting(int sortOrder)
        {
            return SalaryBands.Any(s => s.SortOrder == sortOrder);
        }

        private void PopulateErrorPanel()
        {
            mpeErrorPanel.Show();
        }
    }
}

