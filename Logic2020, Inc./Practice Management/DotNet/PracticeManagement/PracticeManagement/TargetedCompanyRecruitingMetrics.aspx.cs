using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ConfigurationService;

namespace PraticeManagement
{
    public partial class TargetedCompanyRecruitingMetrics : System.Web.UI.Page
    {
        private List<DataTransferObjects.RecruitingMetrics> targetedCompanyMetrics;
        private const string UpdateSucess = "Targeted Company Recruiting Metrics updated successfully.";
        private const string InsertSucess = "Targeted Company Recruiting Metrics added successfully.";
        private const string DeleteSucess = "Targeted Company Recruiting Metrics deleted successfully.";

        private RecruitingMetricsType TargetedCompanyRecruitingMetricsType
        {
            get { return RecruitingMetricsType.TargetedCompany; }
        }

        private List<DataTransferObjects.RecruitingMetrics> TargetedCompanyMetrics
        {
            get
            {
                if (targetedCompanyMetrics == null)
                {
                    using (ConfigurationServiceClient cs = new ConfigurationServiceClient())
                    {
                        targetedCompanyMetrics = cs.GetRecruitingMetrics((int)TargetedCompanyRecruitingMetricsType).ToList();
                    }
                }
                return targetedCompanyMetrics;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBindTargetedCompanyMetrics();
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

        protected void gvTargetedCompanyRecruitingMetrics_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
        }

        public void DataBindTargetedCompanyMetrics()
        {
            gvTargetedCompanyRecruitingMetrics.DataSource = TargetedCompanyMetrics;
            gvTargetedCompanyRecruitingMetrics.DataBind();
        }

        protected bool IsDeleteButtonVisible(bool value)
        {
            return !value;
        }

        public void btnCancel_OnClick(object sender, EventArgs e)
        {
            MakePlusVisible(true);
        }

        public void imgEditTargetedCompanyMetrics_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            gvTargetedCompanyRecruitingMetrics.EditIndex = row.DataItemIndex;
            DataBindTargetedCompanyMetrics();
            MakePlusVisible(true);
        }

        public void imgUpdateTargetedCompanyMetrics_OnClick(object sender, EventArgs e)
        {
            try
            {
                Page.Validate(valSummaryEditTargetedCompany.ValidationGroup);
                if (Page.IsValid)
                {

                    ImageButton imgUpdate = sender as ImageButton;
                    GridViewRow row = imgUpdate.NamingContainer as GridViewRow;

                    var tbTitleName = row.FindControl("tbEditTargetedCompanyName") as TextBox;
                    var oldTitleName = tbTitleName.Attributes["OldName"].ToLower().Replace(" ", "");
                    var newTitleName = tbTitleName.Text.ToLower().Replace(" ", "");
                    var tbSortOrder = row.FindControl("tbEditSortOrder") as TextBox;
                    var oldSortOrder = Convert.ToInt32(tbSortOrder.Attributes["OldSortOrder"]);
                    var newSortOrder = Convert.ToInt32(tbSortOrder.Text);
                    CustomValidator cvUniqueTargetedCompany = row.FindControl("cvUniqueTargetedCompany") as CustomValidator;
                    CustomValidator cvUniqueSortOrder = row.FindControl("cvUniqueSortOrder") as CustomValidator;

                    if (oldTitleName != newTitleName && IsTitleAlreadyExisting(tbTitleName.Text))
                    {
                        cvUniqueTargetedCompany.IsValid = false;
                    }
                    if (oldSortOrder != newSortOrder && IsSortOrderAlreadyExisting(newSortOrder))
                    {
                        cvUniqueSortOrder.IsValid = false;
                    }
                    if (cvUniqueTargetedCompany.IsValid && cvUniqueSortOrder.IsValid)
                    {
                        int recruitingMetricsId = Convert.ToInt32(imgUpdate.Attributes["TargetedCompanyId"]);
                        var TargetedCompanyMetric = new DataTransferObjects.RecruitingMetrics()
                        {
                            RecruitingMetricsId = recruitingMetricsId,
                            Name = tbTitleName.Text,
                            SortOrder = newSortOrder
                        };

                        using (ConfigurationServiceClient csc = new ConfigurationServiceClient())
                        {
                            csc.SaveRecruitingMetrics(TargetedCompanyMetric);
                        }
                        gvTargetedCompanyRecruitingMetrics.EditIndex = -1;
                        targetedCompanyMetrics = null;
                        DataBindTargetedCompanyMetrics();
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
            gvTargetedCompanyRecruitingMetrics.EditIndex = -1;
            DataBindTargetedCompanyMetrics();
        }

        private bool IsTitleAlreadyExisting(string title)
        {
            return TargetedCompanyMetrics.Any(p => p.Name.ToLower().Replace(" ", "") == title.ToLower().Replace(" ", ""));
        }

        private bool IsSortOrderAlreadyExisting(int sortOrder)
        {
            return TargetedCompanyMetrics.Any(s => s.SortOrder == sortOrder);
        }

        public void imgDelete_OnClick(object sender, EventArgs e)
        {
            try
            {
                ImageButton imgDelete = sender as ImageButton;
                int TargetedCompanyId = Convert.ToInt32(imgDelete.Attributes["TargetedCompanyId"]);
                using (ConfigurationServiceClient csc = new ConfigurationServiceClient())
                {
                    csc.RecruitingMetricsDelete(TargetedCompanyId);
                }
                targetedCompanyMetrics = null;
                DataBindTargetedCompanyMetrics();
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
            gvTargetedCompanyRecruitingMetrics.EditIndex = -1;
            DataBindTargetedCompanyMetrics();
        }

        private void MakePlusVisible(bool isPlusVisible)
        {
            if (isPlusVisible)
            {
                btnCancel.Visible = false;
                btnInsert.Visible = false;
                btnPlus.Visible = true;
                tbInsertTargetedCompany.Text = string.Empty;
                tbInsertTargetedCompany.Enabled = false;
                tbInsertSortOrder.Text = string.Empty;
                tbInsertSortOrder.Enabled = false;
            }
            else
            {
                btnPlus.Visible = false;
                btnInsert.Visible = true;
                btnCancel.Visible = true;
                tbInsertTargetedCompany.Text = string.Empty;
                tbInsertTargetedCompany.Enabled = true;
                tbInsertSortOrder.Text = string.Empty;
                tbInsertSortOrder.Enabled = true;
            }
        }

        public void CheckUniqueTitleAndSortOrder()
        {
            if (IsTitleAlreadyExisting(tbInsertTargetedCompany.Text))
            {
                cvInsertUniqueTargetedCompany.IsValid = false;
            }
            if (IsSortOrderAlreadyExisting(Convert.ToInt32(tbInsertSortOrder.Text)))
            {
                cvInsertUniqueSortOrder.IsValid = false;
            }
        }

        public void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate(valSummaryAddTargetedCompany.ValidationGroup);
                if (Page.IsValid)
                {
                    CheckUniqueTitleAndSortOrder();
                    if (cvInsertUniqueTargetedCompany.IsValid && cvInsertUniqueSortOrder.IsValid)
                    {
                        var TargetedCompanyRecrutingMetric = new DataTransferObjects.RecruitingMetrics()
                        {
                            Name = tbInsertTargetedCompany.Text,
                            RecruitingMetricsType = RecruitingMetricsType.TargetedCompany,
                            SortOrder = Convert.ToInt32(tbInsertSortOrder.Text)
                        };

                        using (ConfigurationServiceClient csc = new ConfigurationServiceClient())
                        {
                            csc.RecruitingMetricsInsert(TargetedCompanyRecrutingMetric);
                        }
                        targetedCompanyMetrics = null;
                        DataBindTargetedCompanyMetrics();
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

        private void PopulateErrorPanel()
        {
            mpeErrorPanel.Show();
        }
    }
}
