using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using DataTransferObjects.Utils;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Persons;
using PraticeManagement.Controls.TimeEntry;
using DataTransferObjects.CompositeObjects;
using System.Linq;
using System.Web.Security;
using DataTransferObjects;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web;
using PraticeManagement.Objects;

using PraticeManagement.CalendarService;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace PraticeManagement.Sandbox
{
    public partial class TimeEntriesByPerson : Page
    {
        #region Fields

        protected DateTime CurrDate;
        protected double GrandTotal;
        protected double ProjectTotals;
        protected int ColspanForTotals;

        #endregion

        #region Constants

        private const string TEByPersonExport = "Time Entry By Person";

        #endregion

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            if (diRange.FromDate.HasValue)
            {
                btnExportToXL.Disabled = false;
                btnExportToPDF.Enabled = true;

                var personIds = cblPersons.SelectedValues;

                personIds = personIds.Count > 0 ? new List<int>() { } : new List<int>();
                var startDate = this.diRange.FromDate.Value;
                var endDate = this.diRange.ToDate.HasValue ? this.diRange.ToDate.Value : DateTime.Today;
                var payTypeIds = this.cblTimeScales.SelectedValues;
                var practiceIds = this.cblPractices.SelectedValues;

                if (hdnFiltersChanged.Value == "false")
                {
                    btnResetFilter.Attributes.Add("disabled", "true");
                }
                else
                {
                    btnResetFilter.Attributes.Remove("disabled");
                }
                AddAttributesToCheckBoxes(this.cblPractices);
                AddAttributesToCheckBoxes(this.cblTimeScales);
                AddAttributesToCheckBoxes(this.cblPersons);


                var val = cblPersons.SelectedItems;

                string[] array = val.Split(',');

                StringBuilder sb = new StringBuilder();
                StringBuilder ltrlsb = new StringBuilder();

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != "-1" && array[i] != "" && array[i] != "undefined")
                    {
                        sb.Append(array[i]);
                        sb.Append("&ControlId=" + (ClientID + i));
                        sb.Append(",");
                        
                        ltrlsb.Append("<div id='" + (ClientID + i) + "' ></div>");
                    }
                }

                ltrlContainer.Text = ltrlsb.ToString();

                hdnPersonIds.Value = sb.ToString().TrimEnd(',');
                hdnPracticeIds.Value = practiceIds != null ? cblPractices.SelectedItems : "null";
                hdnPayScaleIds.Value = payTypeIds != null ? cblTimeScales.SelectedItems : "null";
                hdnStartDate.Value = startDate.ToString();
                hdnEndDate.Value = endDate.ToString();
                hlnkExportToExcel.NavigateUrl = "../Controls/Reports/TimeEntriesGetByPersonHandler.ashx?ExportToExcel=true&PersonID="
                    + cblPersons.SelectedItems + "&StartDate=" + startDate.ToString() + "&EndDate=" + endDate.ToString()
                    + "&PayScaleIds=" + (payTypeIds != null ? cblTimeScales.SelectedItems : "null") + "&PracticeIds=" + (practiceIds != null ? cblPractices.SelectedItems : "null");

                hdnUpdateClicked.Value = "true";
                updReport.Update();
            }
        }

        protected void ExportToPDF(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TEByPersonExport);

            string fileName = "TimeEntriesForPerson.pdf";
            var html = hdnSaveReportText.Value;
            HTMLToPdf(html, fileName);
        }

        public void HTMLToPdf(String HTML, string fileName)
        {
            if (HTML == String.Empty)
            {
                HTML = " &nbsp;  ";
            }

            HtmlToPdfBuilder builder = new HtmlToPdfBuilder(iTextSharp.text.PageSize.A4_LANDSCAPE);

            string[] splitArray = { hdnGuid.Value };

            string[] htmlArray = HTML.Split(splitArray, StringSplitOptions.RemoveEmptyEntries);

            foreach (var html in htmlArray)
            {
                HtmlPdfPage page = builder.AddPage();

                page.AppendHtml("<div>{0}</div>", html);

            }

            byte[] timeEntriesByPersonBytes = builder.RenderPdf();

            HttpContext.Current.Response.ContentType = "Application/pdf";
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", fileName));


            int len = timeEntriesByPersonBytes.Length;
            int bytes;
            byte[] buffer = new byte[1024];

            Stream outStream = HttpContext.Current.Response.OutputStream;
            using (MemoryStream stream = new MemoryStream(timeEntriesByPersonBytes))
            {
                while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    HttpContext.Current.Response.Flush();
                    len -= bytes;
                }
            }


        }

        protected void Page_Load(object sender, EventArgs e)
        {

            hdnUpdateClicked.Value = "false";

            TextBox fromDate = diRange.FindControl("tbFrom") as TextBox;
            TextBox toDate = diRange.FindControl("tbTo") as TextBox;

            if (!IsPostBack)
            {
                hdnGuid.Value = Guid.NewGuid().ToString();
                Utils.Generic.InitStartEndDate(diRange);
                Populatepersons(true);
                DataHelper.FillPracticeList(this.cblPractices, Resources.Controls.AllPracticesText);
                DataHelper.FillTimescaleList(this.cblTimeScales, Resources.Controls.AllTypes);
                SelectAllItems(this.cblPractices);
                SelectAllItems(this.cblTimeScales);
                if (hdnFiltersChanged.Value == "false")
                {
                    btnResetFilter.Attributes.Add("disabled", "true");
                }
                else
                {
                    btnResetFilter.Attributes.Remove("disabled");
                }
                AddAttributesToCheckBoxes(this.cblPractices);
                AddAttributesToCheckBoxes(this.cblTimeScales);
                AjaxControlToolkit.CalendarExtender fromDateExt = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
                AjaxControlToolkit.CalendarExtender toDateExt = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;



                fromDate.AutoPostBack = true;
                toDate.AutoPostBack = true;
                fromDate.CausesValidation = true;
                toDate.CausesValidation = true;
                fromDateExt.OnClientDateSelectionChanged = "EnableResetButton";
                toDateExt.OnClientDateSelectionChanged = "EnableResetButton";
            }
            fromDate.Attributes.Add("onchange", "return CheckIsPostBackRequired(this);");
            toDate.Attributes.Add("onchange", "return CheckIsPostBackRequired(this);");
            fromDate.TextChanged += diRange_SelectionChanged;
            toDate.TextChanged += diRange_SelectionChanged;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            UpdatePanel1.Update();
        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        private string GetStatusIds()
        {
            string statusList = string.Empty;
            if (chbActivePersons.Checked)
                statusList += ((int)PersonStatusType.Active).ToString();
            if (chbTerminationPendingPersons.Checked)
                statusList += (string.IsNullOrEmpty(statusList) ? string.Empty : ",") + ((int)PersonStatusType.TerminationPending).ToString();
            if (chbTerminatedPersons.Checked)
                statusList += (string.IsNullOrEmpty(statusList) ? string.Empty : ",") + ((int)PersonStatusType.Terminated).ToString();
            if (string.IsNullOrEmpty(statusList))
                statusList = "-1";
            return statusList;
        }


        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            Utils.Generic.InitStartEndDate(diRange);
            this.chbActivePersons.Checked = true;
            this.chbTerminationPendingPersons.Checked = this.chbTerminatedPersons.Checked = false;
            SelectAllItems(this.cblPractices);
            SelectAllItems(this.cblTimeScales);
            Populatepersons(false);
            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
            AddAttributesToCheckBoxes(this.cblPractices);
            AddAttributesToCheckBoxes(this.cblTimeScales);
            SelectCurrentPerson(DataHelper.CurrentPerson.Id);

            ddlView.SelectedIndex = 0;
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        protected void PersonStatus_OnCheckedChanged(object sender, EventArgs e)
        {
            var currentlySelectedPersons = this.cblPersons.SelectedValues;

            Populatepersons(false);
            this.cblPersons.DataSource = null;

            foreach (var selectedPersonId in currentlySelectedPersons)
            {

                ListItem item = this.cblPersons.Items.FindByValue(selectedPersonId.ToString());
                if (item != null)
                    item.Selected = true;
            }
            this.cblPersons.Items[0].Selected = false;
            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
        }

        protected void diRange_SelectionChanged(object sender, EventArgs e)
        {
            if (diRange.FromDate <= diRange.ToDate)
            {
                PersonStatus_OnCheckedChanged(sender, e);
            }
        }

        private void Populatepersons(bool enableDisableChevron)
        {
            var currentPerson = DataHelper.CurrentPerson;
            var personRoles = Roles.GetRolesForUser(currentPerson.Alias);
            string statusIdsList = GetStatusIds();
            int? personId = null;
            if (!personRoles.Any(s => s == "Administrator" || s == "HR"))
            {
                personId = currentPerson.Id;
                if (enableDisableChevron)
                {
                    this.cpe.Enabled = false;
                    pnlFilters.Visible = false;
                }
            }
            DataHelper.FillTimeEntryPersonListBetweenStartAndEndDates(this.cblPersons, Resources.Controls.AllPersons, null, statusIdsList, personId, diRange.FromDate, diRange.ToDate);
            AddAttributesToCheckBoxes(this.cblPersons);
        }

        protected void Page_SaveStateComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SelectCurrentPerson(DataHelper.CurrentPerson.Id);
            }
        }

        private void SelectCurrentPerson(int? personId)
        {
            if (personId.HasValue)
            {
                var currentPersonItem = this.cblPersons.Items.FindByValue(personId.ToString());
                if (currentPersonItem != null)
                {
                    currentPersonItem.Selected = true;

                }
            }
        }


        public override void VerifyRenderingInServerForm(Control control)
        {

            /* Verifies that the control is rendered */

        }


    }
}

