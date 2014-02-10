using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.ConfigurationService;
using DataTransferObjects;

namespace PraticeManagement
{
    public partial class SourceRecruitingMetrics : System.Web.UI.Page
    {
        private List<DataTransferObjects.RecruitingMetrics> sourceMetrics;
        private const string UpdateSucess = "Source Recruiting Metrics updated successfully.";
        private const string InsertSucess = "Source Recruiting Metrics added successfully.";
        private const string DeleteSucess = "Source Recruiting Metrics deleted successfully.";

        private RecruitingMetricsType sourceRecruitingMetricsType
        {
            get { return RecruitingMetricsType.Source; }
        }

        private List<DataTransferObjects.RecruitingMetrics> SourceMetrics
        {
            get
            {
                if (sourceMetrics == null)
                {
                    using (ConfigurationServiceClient cs = new ConfigurationServiceClient())
                    {
                        sourceMetrics = cs.GetRecruitingMetrics((int)sourceRecruitingMetricsType).ToList();
                    }
                }
                return sourceMetrics;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBindSourceMetrics();
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

        protected void gvSourceRecruitingMetrics_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
        }

        public void DataBindSourceMetrics()
        {
            gvSourceRecruitingMetrics.DataSource = SourceMetrics;
            gvSourceRecruitingMetrics.DataBind();
        }

        protected bool IsDeleteButtonVisible(bool value)
        {
            return !value;
        }

        public void btnCancel_OnClick(object sender, EventArgs e)
        {
            MakePlusVisible(true);
        }

        public void imgEditSourceMetrics_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            gvSourceRecruitingMetrics.EditIndex = row.DataItemIndex;
            DataBindSourceMetrics();
            MakePlusVisible(true);
        }

        public void imgUpdateSourceMetrics_OnClick(object sender, EventArgs e)
        {
            try
            {
                Page.Validate(valSummaryEditSource.ValidationGroup);
                if (Page.IsValid)
                {

                    ImageButton imgUpdate = sender as ImageButton;
                    GridViewRow row = imgUpdate.NamingContainer as GridViewRow;

                    var tbTitleName = row.FindControl("tbEditSourceName") as TextBox;
                    var oldTitleName = tbTitleName.Attributes["OldName"].ToLower().Replace(" ", "");
                    var newTitleName = tbTitleName.Text.ToLower().Replace(" ", "");
                    var tbSortOrder = row.FindControl("tbEditSortOrder") as TextBox;
                    var oldSortOrder = Convert.ToInt32(tbSortOrder.Attributes["OldSortOrder"]);
                    var newSortOrder = Convert.ToInt32(tbSortOrder.Text);
                    CustomValidator cvUniqueSource = row.FindControl("cvUniqueSource") as CustomValidator;
                    CustomValidator cvUniqueSortOrder = row.FindControl("cvUniqueSortOrder") as CustomValidator;

                    if (oldTitleName != newTitleName && IsTitleAlreadyExisting(tbTitleName.Text))
                    {
                        cvUniqueSource.IsValid = false;
                    }
                    if (oldSortOrder != newSortOrder && IsSortOrderAlreadyExisting(newSortOrder))
                    {
                        cvUniqueSortOrder.IsValid = false;
                    }
                    if (cvUniqueSource.IsValid && cvUniqueSortOrder.IsValid)
                    {
                        int recruitingMetricsId = Convert.ToInt32(imgUpdate.Attributes["SourceId"]);
                        var sourceMetric = new DataTransferObjects.RecruitingMetrics()
                        {
                            RecruitingMetricsId = recruitingMetricsId,
                            Name = tbTitleName.Text,
                            SortOrder = newSortOrder
                        };

                        using (ConfigurationServiceClient csc = new ConfigurationServiceClient())
                        {
                            csc.SaveRecruitingMetrics(sourceMetric);
                        }
                        gvSourceRecruitingMetrics.EditIndex = -1;
                        sourceMetrics = null;
                        DataBindSourceMetrics();
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
            gvSourceRecruitingMetrics.EditIndex = -1;
            DataBindSourceMetrics();
        }

        private bool IsTitleAlreadyExisting(string title)
        {
            return SourceMetrics.Any(p => p.Name.ToLower().Replace(" ", "") == title.ToLower().Replace(" ", ""));
        }

        private bool IsSortOrderAlreadyExisting(int sortOrder)
        {
            return SourceMetrics.Any(s => s.SortOrder == sortOrder);
        }

        public void imgDelete_OnClick(object sender, EventArgs e)
        {
            try
            {
                ImageButton imgDelete = sender as ImageButton;
                int sourceId = Convert.ToInt32(imgDelete.Attributes["SourceId"]);
                using (ConfigurationServiceClient csc = new ConfigurationServiceClient())
                {
                    csc.RecruitingMetricsDelete(sourceId);
                }
                sourceMetrics = null;
                DataBindSourceMetrics();
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
            gvSourceRecruitingMetrics.EditIndex = -1;
            DataBindSourceMetrics();
        }

        private void MakePlusVisible(bool isPlusVisible)
        {
            if (isPlusVisible)
            {
                btnCancel.Visible = false;
                btnInsert.Visible = false;
                btnPlus.Visible = true;
                tbInsertSource.Text = string.Empty;
                tbInsertSource.Enabled = false;
                tbInsertSortOrder.Text = string.Empty;
                tbInsertSortOrder.Enabled = false;
            }
            else
            {
                btnPlus.Visible = false;
                btnInsert.Visible = true;
                btnCancel.Visible = true;
                tbInsertSource.Text = string.Empty;
                tbInsertSource.Enabled = true;
                tbInsertSortOrder.Text = string.Empty;
                tbInsertSortOrder.Enabled = true;
            }
        }

        public void CheckUniqueTitleAndSortOrder()
        {
            if (IsTitleAlreadyExisting(tbInsertSource.Text))
            {
                cvInsertUniqueSource.IsValid = false;
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
                Page.Validate(valSummaryAddSource.ValidationGroup);
                if (Page.IsValid)
                {
                    CheckUniqueTitleAndSortOrder();
                    if (cvInsertUniqueSource.IsValid && cvInsertUniqueSortOrder.IsValid)
                    {
                        var sourceRecrutingMetric = new DataTransferObjects.RecruitingMetrics()
                        {
                            Name = tbInsertSource.Text,
                           RecruitingMetricsType = RecruitingMetricsType.Source,
                           SortOrder = Convert.ToInt32(tbInsertSortOrder.Text)
                        };

                        using(ConfigurationServiceClient csc = new ConfigurationServiceClient())
                        {
                            csc.RecruitingMetricsInsert(sourceRecrutingMetric);
                        }
                        sourceMetrics = null;
                        DataBindSourceMetrics();
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
