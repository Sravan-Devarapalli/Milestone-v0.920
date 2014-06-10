using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Web.UI.HtmlControls;
using PraticeManagement.Utils.Excel;
using System.Data;
using PraticeManagement.Utils;
using System.Data.SqlClient;

namespace PraticeManagement.Controls.Reports
{
    public partial class ProjectFeedbackSummaryReport : System.Web.UI.UserControl
    {
        private const string ProjectFeedbackReportExport = "Project Feedback Report Export";
        private int coloumnsCount = 1;
        private int headerRowsCount = 1;

        private HtmlImage imgResourceNameFilter { get; set; }

        private HtmlImage imgTitleFilter { get; set; }

        public HtmlImage imgStatus { get; set; }

        public HtmlImage imgProjectFilter { get; set; }

        public HtmlImage imgProjectStatus { get; set; }

        public HtmlImage imgReveiwStartDateFilter { get; set; }

        public HtmlImage imgReviewEndDateFilter { get; set; }

        public HtmlImage imgProjectManagers { get; set; }

        private SheetStyles HeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.NONE;
                cellStyle.FontHeight = 350;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 500;

                CellStyles dataCellStyle = new CellStyles();
                dataCellStyle.IsBold = true;
                dataCellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.NONE;
                dataCellStyle.FontHeight = 200;
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);
                datarowStyle.Height = 350;

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };

                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.MergeRegion.Add(new int[] { 0, 0, 0, coloumnsCount - 1 });
                sheetStyle.IsAutoResize = false;

                return sheetStyle;
            }
        }

        private SheetStyles DataSheetStyle
        {
            get
            {
                CellStyles headerCellStyle = new CellStyles();
                headerCellStyle.IsBold = true;
                headerCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                CellStyles dateCellStyle = new CellStyles();
                dateCellStyle.DataFormat = "mm/dd/yy;@";

                CellStyles[] dataCellStylearray = { dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dateCellStyle,
                                                      dateCellStyle,
                                                      dataCellStyle,
                                                      dataCellStyle,
                                                      dateCellStyle,
                                                      dataCellStyle
                                                  };

                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = headerRowsCount;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = headerRowsCount;

                return sheetStyle;
            }
        }

        private PraticeManagement.Reports.ProjectFeedbackReport HostingPage
        {
            get { return ((PraticeManagement.Reports.ProjectFeedbackReport)Page); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cblResource.OKButtonId = cblStatus.OKButtonId = cblTitle.OKButtonId = cblProject.OKButtonId = cblProjectStatus.OKButtonId = cblStartDateMonths.OKButtonId = cblEndDateMonths.OKButtonId = cblProjectManagers.OKButtonId = btnFilterOK.ClientID;
        }

        //protected void btnCheck_Click(object sender, EventArgs e)
        //{
        //    SqlConnection connection = null;

        //        var connectionString = "Server=tcp:gdl05wmfhe.database.windows.net;Database=STGPracticeMgmt_backup;User ID=pracadmin@gdl05wmfhe;Password=Meatballs#;Trusted_Connection=False;Encrypt=True;";
        //        if (string.IsNullOrEmpty(connectionString))
        //            return;
        //        connection = new SqlConnection(connectionString);
        //        SqlCommand cmd = new SqlCommand("dbo.GetPersonsForIntialMailForProjectFeedback", connection);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        connection.Open();

        //        List<ProjectFeedbackMail> feedbacks = new List<ProjectFeedbackMail>();
        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            ReadProjectFeedback(reader, feedbacks);
        //        }
        //        if (feedbacks.Count > 0)
        //        {
        //            SendFeedbackIntialMails(feedbacks);
        //        }
        //}

        //private  void SendFeedbackIntialMails(List<ProjectFeedbackMail> feedbacks)
        //{
        //    try
        //    {
        //        //WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_StartedFeedbackInitialEmails, nextRun);
        //        var feedbackMailTemplate = GetEmailTemplate("IntialProjectFeedbackNotification");

        //        foreach (var feedback in feedbacks)
        //        {
        //            foreach (var resource in feedback.Resources)
        //            {
        //                var subject = String.Format(feedbackMailTemplate.Subject, resource.DueDate.ToString(Constants.Formatting.EntryDateFormat), resource.Person.Name, feedback.Project.ProjectNumber, feedback.Project.Name);
        //                var emailBody = String.Format(feedbackMailTemplate.Body, resource.Person.Name, feedback.Project.ProjectNumber, resource.DueDate.ToString(Constants.Formatting.EntryDateFormat), resource.Person.Title.TitleName, resource.ReviewStartDate.ToString(Constants.Formatting.EntryDateFormat), resource.ReviewEndDate.ToString(Constants.Formatting.EntryDateFormat), feedback.Project.Name);
        //                var ccAddressList = "";
        //                if (!string.IsNullOrEmpty(feedback.ProjectOwnerAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.ProjectOwnerAlias))
        //                {
        //                    ccAddressList += feedback.ProjectOwnerAlias + ",";
        //                }
        //                if (!string.IsNullOrEmpty(feedback.SeniorManagerAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.SeniorManagerAlias) && !ccAddressList.Contains(feedback.SeniorManagerAlias))
        //                {
        //                    ccAddressList += feedback.SeniorManagerAlias + ",";
        //                }
        //                if (!string.IsNullOrEmpty(feedback.ClientDirectorAlias) && !feedback.ProjectManagersAliasList.Contains(feedback.ClientDirectorAlias) && !ccAddressList.Contains(feedback.ClientDirectorAlias))
        //                {
        //                    ccAddressList += feedback.ClientDirectorAlias + ",";
        //                }
        //                //Email(subject, emailBody, true, feedback.ProjectManagersAliasList.Substring(0, feedback.ProjectManagersAliasList.Length - 1), ccAddressList.Substring(0, ccAddressList.Length - 1), null);
        //            }
        //        }

        //        //WorkerRole.SaveSchedularLog(currentWithTimeZone, SuccessStatus, M_FinishedFeedbackInitialEmails, nextRun);
        //    }
        //    catch (Exception ex)
        //    {
        //        //WorkerRole.SaveSchedularLog(currentWithTimeZone, FailedStatus, string.Format(FeedbackIntialMailFailedFormat, ex.Message), nextRun);
        //    }
        //}

        //private  EmailTemplate GetEmailTemplate(string emailTemplateName)
        //{
        //    SqlConnection connection = null;
        //    EmailTemplate emailTemplate = new EmailTemplate();
        //    try
        //    {
        //        var connectionString = "Server=tcp:gdl05wmfhe.database.windows.net;Database=STGPracticeMgmt_backup;User ID=pracadmin@gdl05wmfhe;Password=Meatballs#;Trusted_Connection=False;Encrypt=True;";
        //        connection = new SqlConnection(connectionString);
        //        SqlCommand cmnd = new SqlCommand("dbo.EmailTemplateGetByName", connection);
        //        cmnd.CommandType = CommandType.StoredProcedure;
        //        connection.Open();
        //        SqlParameter emailTemplateNameParam = new SqlParameter("@EmailTemplateName", emailTemplateName);
        //        cmnd.Parameters.Add(emailTemplateNameParam);
        //        using (var reader = cmnd.ExecuteReader())
        //        {
        //            if (reader.HasRows)
        //            {
        //                int emailTemplateBodyIndex = reader.GetOrdinal("EmailTemplateBody");
        //                int emailTemplateSubjectIndex = reader.GetOrdinal("EmailTemplateSubject");
        //                int emailTemplateToIndex = reader.GetOrdinal("EmailTemplateTo");
        //                while (reader.Read())
        //                {
        //                    emailTemplate = new EmailTemplate
        //                    {
        //                        EmailTemplateTo = reader.IsDBNull(emailTemplateToIndex) ? string.Empty : reader.GetString(emailTemplateToIndex),
        //                        Subject = reader.GetString(emailTemplateSubjectIndex),
        //                        Body = reader.GetString(emailTemplateBodyIndex)
        //                    };
        //                }
        //            }
        //        }
        //        connection.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        connection.Close();
        //        throw ex;
        //    }
        //    return emailTemplate;
        //}

        //private static void ReadProjectFeedback(SqlDataReader reader, List<ProjectFeedbackMail> feedbacks)
        //{
        //    if (reader.HasRows)
        //    {
        //        int firstNameIndex = reader.GetOrdinal("FirstName");
        //        int lastNameIndex = reader.GetOrdinal("LastName");
        //        int personIdIndex = reader.GetOrdinal("PersonId");
        //        int titleIdIndex = reader.GetOrdinal("TitleId");
        //        int titleIndex = reader.GetOrdinal("Title");
        //        int reviewStartDateIndex = reader.GetOrdinal("ReviewPeriodStartDate");
        //        int reviewEndDateIndex = reader.GetOrdinal("ReviewPeriodEndDate");
        //        int dueDateIndex = reader.GetOrdinal("DueDate");
        //        int projectIdIndex = reader.GetOrdinal("ProjectId");
        //        int projectNameIndex = reader.GetOrdinal("ProjectName");
        //        int projectNumberIndex = reader.GetOrdinal("ProjectNumber");
        //        int projectManagersAliasesIndex = reader.GetOrdinal("ToAddressList");
        //        int directorAliasIndex = reader.GetOrdinal("DirectorAlias");
        //        int projectOwnerAliasIndex = reader.GetOrdinal("ProjectOwnerAlias");
        //        int seniorManagerAliasIndex = reader.GetOrdinal("SeniorManagerAlias");
               
        //        while (reader.Read())
        //        {
        //            var projectId = reader.GetInt32(projectIdIndex);
        //            var feedback = new ProjectFeedback
        //            {
        //                Person = new Person()
        //                {
        //                    Id = reader.GetInt32(personIdIndex),
        //                    FirstName = reader.GetString(firstNameIndex),
        //                    LastName = reader.GetString(lastNameIndex),
        //                    Title = new Title()
        //                    {
        //                        TitleId = reader.GetInt32(titleIdIndex),
        //                        TitleName = reader.GetString(titleIndex)
        //                    }
        //                },
        //                ReviewStartDate = reader.GetDateTime(reviewStartDateIndex),
        //                ReviewEndDate = reader.GetDateTime(reviewEndDateIndex),
        //                DueDate = reader.GetDateTime(dueDateIndex)
        //            };
        //            if (feedbacks.Any(p => p.Project.Id.Value == projectId))
        //            {
        //                var mail = feedbacks.FirstOrDefault(p => p.Project.Id.Value == projectId);
        //                mail.Resources.Add(feedback);
        //            }
        //            else
        //            {
        //                var feedbackMail = new ProjectFeedbackMail()
        //                {
        //                    Project = new Project()
        //                    {
        //                        Id = reader.GetInt32(projectIdIndex),
        //                        Name = reader.GetString(projectNameIndex),
        //                        ProjectNumber = reader.GetString(projectNumberIndex)
        //                    },
        //                    ProjectManagersAliasList = reader.GetString(projectManagersAliasesIndex),
        //                    ClientDirectorAlias = reader.GetString(directorAliasIndex),
        //                    ProjectOwnerAlias = reader.GetString(projectOwnerAliasIndex),
        //                    SeniorManagerAlias = reader.GetString(seniorManagerAliasIndex),
        //                    Resources = new List<ProjectFeedback>()
        //                    {
        //                        feedback
        //                    }
        //                };
        //                feedbacks.Add(feedbackMail);
        //            }
        //        }
        //    }
        //}

        protected void btnExportToExcel_OnClick(object sender, EventArgs e)
        {
            //“TimePeriod_ByProject_DateRange.xls”.  
            var filename = string.Format("ProjectFeedbackReport_{0}-{1}.xls", HostingPage.StartDate.Value.ToString("MM_dd_yyyy"), HostingPage.EndDate.Value.ToString("MM_dd_yyyy"));
            DataHelper.InsertExportActivityLogMessage(ProjectFeedbackReportExport);
            List<SheetStyles> sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();
            if (HostingPage.StartDate.HasValue && HostingPage.EndDate.HasValue)
            {
                var report = ServiceCallers.Custom.Report(p => p.ProjectFeedbackReport(null, null, HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null, null, null, false, null, null, null, null, null, null, true)).ToList();

                if (report.Count > 0)
                {
                    DataTable header1 = new DataTable();
                    header1.Columns.Add("Project Feedback Report");

                    List<object> row1 = new List<object>();
                    row1.Add(HostingPage.RangeForExcel);
                    header1.Rows.Add(row1.ToArray());

                    headerRowsCount = header1.Rows.Count + 3;

                    var data = PrepareDataTable(report);
                    coloumnsCount = data.Columns.Count;
                    sheetStylesList.Add(HeaderSheetStyle);
                    sheetStylesList.Add(DataSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "ProjectFeedback";
                    dataset.Tables.Add(header1);
                    dataset.Tables.Add(data);
                    dataSetList.Add(dataset);
                }
                else
                {
                    string dateRangeTitle = "There are no entries for feedback for the period selected.";
                    DataTable header = new DataTable();
                    header.Columns.Add(dateRangeTitle);
                    sheetStylesList.Add(HeaderSheetStyle);
                    var dataset = new DataSet();
                    dataset.DataSetName = "ProjectFeedback";
                    dataset.Tables.Add(header);
                    dataSetList.Add(dataset);
                }

                NPOIExcel.Export(filename, dataSetList, sheetStylesList);
            }
        }

        public DataTable PrepareDataTable(List<ProjectFeedback> reportData)
        {
            DataTable data = new DataTable();
            List<object> rownew;
            List<object> row;

            data.Columns.Add("Employee Number");
            data.Columns.Add("Resource Name");
            data.Columns.Add("Project Number");
            data.Columns.Add("Project Name");
            data.Columns.Add("Project Status");
            data.Columns.Add("Account");
            data.Columns.Add("Business Group");
            data.Columns.Add("Business Unit");
            data.Columns.Add("Client Director");
            data.Columns.Add("Senior Manager");
            data.Columns.Add("Project Owner");
            data.Columns.Add("Project Manager(s)");
            data.Columns.Add("Review Period Start Date");
            data.Columns.Add("Review Period End Date");
            data.Columns.Add("Review Status");
            data.Columns.Add("Status Updated By");
            data.Columns.Add("Status Updated Date");
            data.Columns.Add("Cancellation Reason");
            foreach (var feedback in reportData)
            {
                int i = 0;
                string managers = "";
                row = new List<object>();
                row.Add(feedback.Person.EmployeeNumber);
                row.Add(feedback.Person.Name);
                row.Add(feedback.Project.ProjectNumber);
                row.Add(feedback.Project.Name);
                row.Add(feedback.Project.Status.Name);
                row.Add(feedback.Project.Client.Name);
                row.Add(feedback.Project.BusinessGroup.Name);
                row.Add(feedback.Project.Group.Name);
                row.Add(feedback.Project.Director.FirstName == ""?"": feedback.Project.Director.Name);
                row.Add(feedback.Project.SeniorManagerName);
                row.Add(feedback.Project.ProjectOwner.Name);
                for (i = 0; i < feedback.Project.ProjectManagers.Count; i++)
                {
                    managers += feedback.Project.ProjectManagers[i].HtmlEncodedName;
                    if (i != feedback.Project.ProjectManagers.Count - 1)
                    {
                        managers += "; ";
                    }
                }
                row.Add(managers);
                row.Add(feedback.ReviewStartDate);
                row.Add(feedback.ReviewEndDate);
                row.Add(feedback.Status.Name);
                row.Add(feedback.CompletionCertificateBy);
                if (feedback.CompletionCertificateDate == DateTime.MinValue)
                    row.Add("");
                else
                    row.Add(feedback.CompletionCertificateDate);
                row.Add(feedback.CancelationReason);
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void repFeedback_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                imgResourceNameFilter = e.Item.FindControl("imgResourceNameFilter") as HtmlImage;
                imgTitleFilter = e.Item.FindControl("imgTitleFilter") as HtmlImage;
                imgStatus = e.Item.FindControl("imgStatus") as HtmlImage;

                imgProjectFilter = e.Item.FindControl("imgProjectFilter") as HtmlImage;
                imgProjectStatus = e.Item.FindControl("imgProjectStatus") as HtmlImage;
                imgReveiwStartDateFilter = e.Item.FindControl("imgReveiwStartDateFilter") as HtmlImage;
                imgReviewEndDateFilter = e.Item.FindControl("imgReviewEndDateFilter") as HtmlImage;
                imgProjectManagers = e.Item.FindControl("imgProjectManagers") as HtmlImage;

            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int i = 0;
                var dataItem = (ProjectFeedback)e.Item.DataItem;
                var lblProjectManagers = e.Item.FindControl("lblProjectManagers") as Label;
                var hlProjectNumber = e.Item.FindControl("hlProjectNumber") as HyperLink;
                for (i = 0; i < dataItem.Project.ProjectManagers.Count; i++)
                {
                    lblProjectManagers.Text += dataItem.Project.ProjectManagers[i].HtmlEncodedName;
                    if (i != dataItem.Project.ProjectManagers.Count - 1)
                    {
                        lblProjectManagers.Text += "; ";
                    }
                }
            }
        }

        protected void btnFilterOK_OnClick(object sender, EventArgs e)
        {
            PopulateReport(true, true);
        }

        public void PopulateReport(bool isFromFilters, bool applyFilters = false)
        {
            ProjectFeedback[] report;
            if (applyFilters)
                report = ServiceCallers.Custom.Report(p => p.ProjectFeedbackReport(HostingPage.AccountIds, HostingPage.BusinessGroupIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, cblProjectStatus.SelectedItems, cblProject.SelectedItems, HostingPage.DirectorIds, HostingPage.PracticeIds, HostingPage.ExcludeInternalPractices, cblResource.SelectedItems, cblTitle.SelectedItems, cblStartDateMonths.SelectedItems, cblEndDateMonths.SelectedItems, cblProjectManagers.SelectedItems, cblStatus.SelectedItems, false));
            else
                report = ServiceCallers.Custom.Report(p => p.ProjectFeedbackReport(HostingPage.AccountIds, HostingPage.BusinessGroupIds, HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null, HostingPage.DirectorIds, HostingPage.PracticeIds, HostingPage.ExcludeInternalPractices, null, null, null, null, null, null, false));

            DataBindProject(report, isFromFilters);

            SetHeaderValues(report);
        }

        public void SetHeaderValues(ProjectFeedback[] reportData)
        {
            HostingPage.UpdateHeaderSection = true;
            HostingPage.CompletedCount = reportData.Count(p => p.Status.Id == 1);
            HostingPage.NotCompletedCount = reportData.Count(p => p.Status.Id == 2);
            HostingPage.CanceledCount = reportData.Count(p => p.Status.Id == 3);
        }

        public void DataBindProject(ProjectFeedback[] reportData, bool isFromFilters)
        {
            if (reportData.Length > 0)
            {
                PopulateFilterPanels(reportData, isFromFilters);

                divEmptyMessage.Style["display"] = "none";
                btnExportToExcel.Enabled = true;
                repFeedback.Visible = true;
                repFeedback.DataSource = reportData;
                repFeedback.DataBind();

                imgResourceNameFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblResource.FilterPopupClientID,
                cblResource.SelectedIndexes, cblResource.CheckBoxListObject.ClientID, cblResource.WaterMarkTextBoxBehaviorID);
                imgTitleFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblTitle.FilterPopupClientID,
                  cblTitle.SelectedIndexes, cblTitle.CheckBoxListObject.ClientID, cblTitle.WaterMarkTextBoxBehaviorID);
                imgStatus.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblStatus.FilterPopupClientID,
                  cblStatus.SelectedIndexes, cblStatus.CheckBoxListObject.ClientID, cblStatus.WaterMarkTextBoxBehaviorID);

                imgProjectFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProject.FilterPopupClientID,
                  cblProject.SelectedIndexes, cblProject.CheckBoxListObject.ClientID, cblProject.WaterMarkTextBoxBehaviorID);
                imgProjectStatus.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectStatus.FilterPopupClientID,
                 cblProjectStatus.SelectedIndexes, cblProjectStatus.CheckBoxListObject.ClientID, cblProjectStatus.WaterMarkTextBoxBehaviorID);
                imgReveiwStartDateFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblStartDateMonths.FilterPopupClientID,
                  cblStartDateMonths.SelectedIndexes, cblStartDateMonths.CheckBoxListObject.ClientID, cblStartDateMonths.WaterMarkTextBoxBehaviorID);
                imgReviewEndDateFilter.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblEndDateMonths.FilterPopupClientID,
                  cblEndDateMonths.SelectedIndexes, cblEndDateMonths.CheckBoxListObject.ClientID, cblEndDateMonths.WaterMarkTextBoxBehaviorID);
                imgProjectManagers.Attributes["onclick"] = string.Format("Filter_Click(\'{0}\',\'{1}\',\'{2}\',\'{3}\');", cblProjectManagers.FilterPopupClientID,
                  cblProjectManagers.SelectedIndexes, cblProjectManagers.CheckBoxListObject.ClientID, cblProjectManagers.WaterMarkTextBoxBehaviorID);
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repFeedback.Visible = btnExportToExcel.Enabled = false;
            }
        }

        private void PopulateFilterPanels(ProjectFeedback[] reportData, bool isFromFilters)
        {
            var report = ServiceCallers.Custom.Report(p => p.ProjectFeedbackReport(null, null, HostingPage.StartDate.Value, HostingPage.EndDate.Value, null, null, null, null, HostingPage.ExcludeInternalPractices, null, null, null, null, null, null, true));
            int count;
            count = PopulateResourceFilter(report);
            foreach (ListItem item in cblResource.Items)
            {
                if (reportData.Any(p => p.Person.Id.Value.ToString() == item.Value))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            if (cblResource.Items.Count - 1 == count)
            {
                cblResource.Items[0].Selected = true;
            }
            count = PopulateTitleFilter(report);
            foreach (ListItem item in cblTitle.Items)
            {
                if (reportData.Any(p => p.Person.Title.TitleId.ToString() == item.Value))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            if (cblTitle.Items.Count - 1 == count)
            {
                cblTitle.Items[0].Selected = true;
            }
            count = PopulateProjectFilter(report);
            foreach (ListItem item in cblProject.Items)
            {
                if (reportData.Any(p => p.Project.Id.ToString() == item.Value))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            if (cblProject.Items.Count - 1 == count)
            {
                cblProject.Items[0].Selected = true;
            }
            count = PopulateReviewStartDateFilter(report);
            foreach (ListItem item in cblStartDateMonths.Items)
            {
                if (reportData.Any(p => p.ReviewStartDate.Month.ToString() == item.Value))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            if (cblStartDateMonths.Items.Count - 1 == count)
            {
                cblStartDateMonths.Items[0].Selected = true;
            }
            count = PopulateReviewEndDateFilter(report);
            foreach (ListItem item in cblEndDateMonths.Items)
            {
                if (reportData.Any(p => p.ReviewEndDate.Month.ToString() == item.Value))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            if (cblEndDateMonths.Items.Count - 1 == count)
            {
                cblEndDateMonths.Items[0].Selected = true;
            }

            count = PopulateProjectManagersFilter(report);
            foreach (ListItem item in cblProjectManagers.Items)
            {
                if (reportData.Any(p => p.Project.ProjectManagers.Any(r => r.Id.Value.ToString() == item.Value)))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            if (cblProjectManagers.Items.Count - 1 == count)
            {
                cblProjectManagers.Items[0].Selected = true;
            }
            count = PopulateStatusFilter(report);
            foreach (ListItem item in cblStatus.Items)
            {
                if (reportData.Any(p => p.Status.Id.Value.ToString() == item.Value))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            if (cblStatus.Items.Count - 1 == count)
            {
                cblStatus.Items[0].Selected = true;
            }
            count = PopulateProjectStatusFilter(report);
            foreach (ListItem item in cblProjectStatus.Items)
            {
                if (reportData.Any(p => p.Project.Status.Id.ToString() == item.Value))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            if (cblProjectStatus.Items.Count - 1 == count)
            {
                cblProjectStatus.Items[0].Selected = true;
            }
            //SelectFirstItemIfAllSelected();
            if (!isFromFilters)
            {
                SaveInViewState();
            }
        }

        public void SaveInViewState()
        {
            cblResource.SaveSelectedIndexesInViewState();
            cblTitle.SaveSelectedIndexesInViewState();
            cblStatus.SaveSelectedIndexesInViewState();
            cblProjectStatus.SaveSelectedIndexesInViewState();
            cblProject.SaveSelectedIndexesInViewState();
            cblStartDateMonths.SaveSelectedIndexesInViewState();
            cblEndDateMonths.SaveSelectedIndexesInViewState();
            cblProjectManagers.SaveSelectedIndexesInViewState();
        }

        private int PopulateResourceFilter(ProjectFeedback[] reportData)
        {
            var resources = new List<Person>();
            resources = reportData.Select(r => new Person { Id = r.Person.Id, FirstName = r.Person.FirstName, LastName = r.Person.LastName }).Distinct().Select(p => new Person { Id = p.Id, FirstName = p.FirstName, LastName = p.LastName }).ToList().OrderBy(s => s.Name).ToList();

            DataHelper.FillListDefault(cblResource.CheckBoxListObject, "All Resources", resources.Distinct().ToArray(), false, "Id", "Name");
            cblResource.SelectAllItems(true);
            return resources.Count;
        }

        private int PopulateTitleFilter(ProjectFeedback[] reportData)
        {
            var titles = new List<Title>();
            titles = reportData.Select(r => new { TitleId = r.Person.Title.TitleId, TitleName = r.Person.Title.TitleName }).Distinct().Select(p => new Title { TitleId = p.TitleId, TitleName = p.TitleName }).ToList().OrderBy(s => s.TitleName).Distinct().ToList();

            DataHelper.FillListDefault(cblTitle.CheckBoxListObject, "All Titles", titles.ToArray(), false, "TitleId", "TitleName");
            cblTitle.SelectAllItems(true);
            return titles.Count;
        }

        private int PopulateProjectFilter(ProjectFeedback[] reportData)
        {
            var projects = new List<Project>();
            projects = reportData.Select(r => new { Id = r.Project.Id, Name = r.Project.Name }).Distinct().Select(p => new Project { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Name).Distinct().ToList();

            DataHelper.FillListDefault(cblProject.CheckBoxListObject, "All Projects", projects.Distinct().ToArray(), false, "Id", "Name");
            cblProject.SelectAllItems(true);
            return projects.Count;
        }

        private int PopulateReviewStartDateFilter(ProjectFeedback[] reportData)
        {
            var startDateMonths = reportData.Select(r => new { Id = r.ReviewStartDate.Month, Name = r.ReviewStartDate.ToString("MMM-yyyy") }).Distinct().Select(p => new { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Id).ToList();

            DataHelper.FillListDefault(cblStartDateMonths.CheckBoxListObject, "All Review Start Dates", startDateMonths.Distinct().ToArray(), false, "Id", "Name");
            cblStartDateMonths.SelectAllItems(true);
            return startDateMonths.Count;
        }

        private int PopulateReviewEndDateFilter(ProjectFeedback[] reportData)
        {
            var endDateMonths = reportData.Select(r => new { Id = r.ReviewEndDate.Month, Name = r.ReviewEndDate.ToString("MMM-yyyy") }).Distinct().Select(p => new { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Id).ToList();

            DataHelper.FillListDefault(cblEndDateMonths.CheckBoxListObject, "All Review End Dates", endDateMonths.Distinct().ToArray(), false, "Id", "Name");
            cblEndDateMonths.SelectAllItems(true);
            return endDateMonths.Count;
        }

        private int PopulateProjectManagersFilter(ProjectFeedback[] reportData)
        {
            var resources = new List<Person>();
            resources = reportData.SelectMany(b => b.Project.ProjectManagers.Select(r => new Person { Id = r.Id, FirstName = r.FirstName, LastName = r.LastName }).Distinct().ToList().OrderBy(s => s.Name)).ToList();

            DataHelper.FillListDefault(cblProjectManagers.CheckBoxListObject, "All Project Managers", resources.Distinct().ToArray(), false, "Id", "Name");
            cblProjectManagers.SelectAllItems(true);
            return resources.Distinct().ToArray().Length;
        }

        private int PopulateStatusFilter(ProjectFeedback[] reportData)
        {
            var status = new List<ProjectFeedbackStatus>();
            status = reportData.Select(r => new { Id = r.Status.Id, Name = r.Status.Name }).Distinct().Select(p => new ProjectFeedbackStatus { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Name).ToList();

            DataHelper.FillListDefault(cblStatus.CheckBoxListObject, "All Statuses", status.Distinct().ToArray(), false, "Id", "Name");
            cblStatus.SelectAllItems(true);
            return status.Count;
        }

        private int PopulateProjectStatusFilter(ProjectFeedback[] reportData)
        {
            var status = new List<ProjectStatus>();
            status = reportData.Select(r => new ProjectStatus { Id = r.Project.Status.Id, Name = r.Project.Status.Name }).Distinct().Select(p => new ProjectStatus { Id = p.Id, Name = p.Name }).ToList().OrderBy(s => s.Name).ToList();

            DataHelper.FillListDefault(cblProjectStatus.CheckBoxListObject, "All Project Status", status.Distinct().ToArray(), false, "Id", "Name");
            cblProjectStatus.SelectAllItems(true);
            return status.Count;
        }

        protected string GetProjectDetailsLink(int? projectId, bool flag)
        {
            if (projectId.HasValue)
                return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value) + (flag ? "&Feedback=true" : string.Empty),
                                                            Constants.ApplicationPages.ProjectFeedbackReport);
            else
                return string.Empty;
        }
    }
}
