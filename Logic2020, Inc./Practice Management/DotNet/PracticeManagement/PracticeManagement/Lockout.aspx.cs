using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using System.Xml.Linq;
using System.Text;
using DataTransferObjects;

namespace PraticeManagement
{
    public partial class Lockout : System.Web.UI.Page
    {
        private const string LockoutsXmlOpen = "<Lockouts>";
        private const string LockoutsXmlClose = "</Lockouts>";
        private const string LockoutPageXmlOpen = "<LockoutPage LockoutPageId=\"{0}\">";
        private const string LockoutPageXmlClose = "</LockoutPage>";
        private const string LockoutXmlOpen = "<Lockout LockoutId=\"{0}\" FunctionalityName=\"{1}\" Lockout=\"{2}\" LockoutDate=\"{3}\">";
        private const string LockoutXmlClose = "</Lockout>";
        private const string LockoutXname = "Lockout";
        private const string LockoutPageXname = "LockoutPage";
        private const string LockoutPageIdXName = "LockoutPageId";
        private const string DateRequiredErrorMessage = "Lock down date is required as you selected a functionality '{0}' to lock down";
        private const string LockdownRequiredErrorMessage = "Select functionality '{0}' to lock down as you have set lock down date";
        public int count = 0;

        public string LockoutPageName
        {
            get;
            set;
        }

        public string LockoutXML
        {
            get;
            set;
        }

        public string TimeEntryXML
        {
            get;
            set;
        }

        public string CalendarXML
        {
            get;
            set;
        }

        public string PersonDetailXML
        {
            get;
            set;
        }

        public string ProjectDetailXML
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mlInsertStatus.ClearMessage();
            if (!IsPostBack)
            {
                PopulateGridData();
            }
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
                mpeErrorPanel.Show();
            }
        }

        public void PopulateGridData()
        {
            using (var service = new ConfigurationService.ConfigurationServiceClient())
            {
                var lockouts = service.GetLockoutDetails(null);
                gvLockout.DataSource = lockouts;
                gvLockout.DataBind();
            }
        }

        protected void custLockoutDate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var source = sender as CustomValidator;
            var gridRow = source.NamingContainer as GridViewRow;
            var chbLockout = gridRow.FindControl("chbLockout") as CheckBox;
            var dpLockoutDate = gridRow.FindControl("dpLockoutDate") as DatePicker;
            var hdnFunctionalityName = gridRow.FindControl("hdnFunctionalityName") as HiddenField;
            var hdnPageId = gridRow.FindControl("hdnPageId") as HiddenField;
            var functionalityName = hdnFunctionalityName.Value;
            if (chbLockout.Checked && dpLockoutDate.TextValue == string.Empty)
            {
                e.IsValid = false;
                source.ErrorMessage = source.ToolTip = string.Format(DateRequiredErrorMessage, functionalityName);
                if (hdnPageId.Value == ((int)LockoutPages.Persondetail).ToString())
                {
                    e.IsValid = true;
                    RaiseValidationForFirstItemInPersonDetail(source.ErrorMessage);
                }
            }
        }

        protected void custchbLockout_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var source = sender as CustomValidator;
            var gridRow = source.NamingContainer as GridViewRow;
            var chbLockout = gridRow.FindControl("chbLockout") as CheckBox;
            var hdnPageId = gridRow.FindControl("hdnPageId") as HiddenField;
            var dpLockoutDate = gridRow.FindControl("dpLockoutDate") as DatePicker;
            var hdnFunctionalityName = gridRow.FindControl("hdnFunctionalityName") as HiddenField;
            var functionalityName = hdnFunctionalityName.Value;
            if (!chbLockout.Checked && dpLockoutDate.TextValue != string.Empty)
            {
                e.IsValid = false;
                source.ErrorMessage = source.ToolTip = string.Format(LockdownRequiredErrorMessage, functionalityName);
                if (hdnPageId.Value == ((int)LockoutPages.Persondetail).ToString())
                {
                    e.IsValid = true;
                    RaiseValidationForFirstItemInPersonDetail(source.ErrorMessage);
                }
            }
        }

        public void RaiseValidationForFirstItemInPersonDetail(string validationMessage)
        {
            foreach (GridViewRow row in gvLockout.Rows)
            {
                var hdnPageId = row.FindControl("hdnPageId") as HiddenField;
                var hdnFunctionalityName = row.FindControl("hdnFunctionalityName") as HiddenField;
                if (hdnPageId.Value == ((int)LockoutPages.Persondetail).ToString() && hdnFunctionalityName.Value == "Amount")
                {
                    var custchbLockout = row.FindControl("custchbLockout") as CustomValidator;
                    
                    var custLockoutDateNotFuture = row.FindControl("custLockoutDateNotFuture") as CustomValidator;
                    var custLockoutDate = row.FindControl("custLockoutDate") as CustomValidator;
                    if (validationMessage.Contains("Lock down date is required as you selected a functionality"))
                    {
                        custLockoutDate.IsValid = false;
                        custLockoutDate.ErrorMessage = custLockoutDate.ToolTip = "Lock down date is required as you selected functionality(ies) to lock down";
                    }
                    else if (validationMessage.Contains("Select functionality"))
                    {
                        if (count == 0)
                        {
                            custchbLockout.IsValid = false;
                            custchbLockout.ErrorMessage = custchbLockout.ToolTip = "Select any of the functionalities to lock down as you have set lock down date for Person detail page.";
                        }
                        else
                        {
                            custchbLockout.IsValid = true;
                        }
                    }
                    else if (validationMessage.Contains("Lock down date cannot be"))
                     custLockoutDateNotFuture.IsValid = false;
                }
            }
        }

        public void UpdateDatesForPersonDetailPage()
        {
            string dateValue = "";
            foreach (GridViewRow row in gvLockout.Rows)
            {
                var hdnPageId = row.FindControl("hdnPageId") as HiddenField;
                var hdnFunctionalityName = row.FindControl("hdnFunctionalityName") as HiddenField;
                var chbLockout = row.FindControl("chbLockout") as CheckBox;
                var dpLockoutDate = row.FindControl("dpLockoutDate") as DatePicker;
                if (hdnPageId.Value == ((int)LockoutPages.Persondetail).ToString())
                {
                    if (hdnFunctionalityName.Value == "Amount")
                        dateValue = dpLockoutDate.TextValue;
                    dpLockoutDate.TextValue = dateValue;
                }
                if (chbLockout.Checked && dpLockoutDate.TextValue != string.Empty && hdnPageId.Value == ((int)LockoutPages.Persondetail).ToString())
                    count++;
            }
        }

        protected void gvLockout_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                LockoutPageName = string.Empty;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lblPageName = e.Row.FindControl("lblPageName") as Label;
                var hdnPageId = e.Row.FindControl("hdnPageId") as HiddenField;
                var hdnLockoutId = e.Row.FindControl("hdnLockoutId") as HiddenField;
                var hdnFunctionalityName = e.Row.FindControl("hdnFunctionalityName") as HiddenField;
                var chbLockout = e.Row.FindControl("chbLockout") as CheckBox;
                var dpLockoutDate = e.Row.FindControl("dpLockoutDate") as DatePicker;
                var dataItem = e.Row.DataItem as DataTransferObjects.Lockout;
                var compCompletionDate = e.Row.FindControl("compCompletionDate") as CompareValidator;
                chbLockout.Checked = dataItem.IsLockout;
                hdnFunctionalityName.Value = dataItem.HtmlEncodedName;
                dpLockoutDate.TextValue = dataItem.LockoutDate.HasValue ? dataItem.LockoutDate.Value.ToString("MM/dd/yyyy") : string.Empty;
                var pageName = DataHelper.GetDescription(dataItem.LockoutPage);
                lblPageName.Text = LockoutPageName == pageName ? string.Empty : pageName;
                hdnPageId.Value = ((int)dataItem.LockoutPage).ToString();
                hdnLockoutId.Value = dataItem.Id.Value.ToString();
                if (pageName == "Person detail" && LockoutPageName == pageName)
                {
                    dpLockoutDate.Visible = false;
                    compCompletionDate.Enabled = false;
                }
                LockoutPageName = pageName;
            }
        }

        protected void custLockoutDateNotFuture_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            CustomValidator custLockoutDateNotFuture = (CustomValidator)sender;
            GridViewRow row = custLockoutDateNotFuture.NamingContainer as GridViewRow;
            DatePicker dpLockoutDate = (DatePicker)row.FindControl("dpLockoutDate");
            var hdnPageId = row.FindControl("hdnPageId") as HiddenField;
            e.IsValid = dpLockoutDate.DateValue.Date <= DateTime.Today.Date;
            if (hdnPageId.Value == ((int)LockoutPages.Persondetail).ToString() && !e.IsValid)
            {
                e.IsValid = true;
                RaiseValidationForFirstItemInPersonDetail(custLockoutDateNotFuture.ErrorMessage);
            }
        }

        public void PrepareXMLForAllLockoutDetails()
        {
            PrepareIntialXMLForPages();
            foreach (GridViewRow row in gvLockout.Rows)
            {
                var hdnLockoutId = row.FindControl("hdnLockoutId") as HiddenField;
                var hdnPageId = row.FindControl("hdnPageId") as HiddenField;
                var chbLockout = row.FindControl("chbLockout") as CheckBox;
                var dpLockoutDate = row.FindControl("dpLockoutDate") as DatePicker;
                var lockout = new DataTransferObjects.Lockout()
                {
                    Id = Convert.ToInt32(hdnLockoutId.Value),
                    LockoutPage = (LockoutPages)Convert.ToInt32(hdnPageId.Value),
                    Name = row.Cells[1].Text,
                    IsLockout = chbLockout.Checked,
                    LockoutDate = dpLockoutDate.TextValue == string.Empty ? null : (DateTime?)dpLockoutDate.DateValue
                };
                var xmlForSingle = new StringBuilder();
                PrePareXmlForLockout(xmlForSingle, lockout);
                AddLockoutXML(xmlForSingle.ToString(), Convert.ToInt32(hdnPageId.Value));
            }
        }

        public string FinalXML()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(LockoutsXmlOpen);
            xml.Append(TimeEntryXML);
            xml.Append(CalendarXML);
            xml.Append(PersonDetailXML);
            xml.Append(ProjectDetailXML);
            xml.Append(LockoutsXmlClose);
            return xml.ToString();
        }

        public void AddLockoutXML(string lockoutXML, int pageId)
        {
            XDocument xdoc;
            List<XElement> xlist;
            switch (pageId)
            {
                case 1: xdoc = XDocument.Parse(TimeEntryXML.ToString());
                    xdoc.Descendants(XName.Get(LockoutPageXname)).Last().Add(XElement.Parse(lockoutXML.ToString()));
                    TimeEntryXML = xdoc.ToString();
                    break;
                case 2: xdoc = XDocument.Parse(CalendarXML.ToString());
                    xdoc.Descendants(XName.Get(LockoutPageXname)).Last().Add(XElement.Parse(lockoutXML.ToString()));
                    CalendarXML = xdoc.ToString();
                    break;
                case 3: xdoc = XDocument.Parse(PersonDetailXML.ToString());
                    xdoc.Descendants(XName.Get(LockoutPageXname)).Last().Add(XElement.Parse(lockoutXML.ToString()));
                    PersonDetailXML = xdoc.ToString();
                    break;
                case 4: xdoc = XDocument.Parse(ProjectDetailXML.ToString());
                    xdoc.Descendants(XName.Get(LockoutPageXname)).Last().Add(XElement.Parse(lockoutXML.ToString()));
                    ProjectDetailXML = xdoc.ToString();
                    break;
            }
        }

        public void PrePareXmlForLockout(StringBuilder xml, DataTransferObjects.Lockout lockout)
        {
            xml.Append(string.Format(LockoutXmlOpen,
                                    lockout.Id,
                                    lockout.HtmlEncodedName,
                                    lockout.IsLockout,
                                    !lockout.LockoutDate.HasValue ? null : lockout.LockoutDate.Value.ToShortDateString()));
            xml.Append(LockoutXmlClose);
        }

        public void PrepareIntialXMLForPages()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(string.Format(LockoutPageXmlOpen, 1));
            xml.Append(LockoutPageXmlClose);
            TimeEntryXML = xml.ToString();
            xml = new StringBuilder();
            xml.Append(string.Format(LockoutPageXmlOpen, 2));
            xml.Append(LockoutPageXmlClose);
            CalendarXML = xml.ToString();
            xml = new StringBuilder();
            xml.Append(string.Format(LockoutPageXmlOpen, 3));
            xml.Append(LockoutPageXmlClose);
            PersonDetailXML = xml.ToString();
            xml = new StringBuilder();
            xml.Append(string.Format(LockoutPageXmlOpen, 4));
            xml.Append(LockoutPageXmlClose);
            ProjectDetailXML = xml.ToString();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            UpdateDatesForPersonDetailPage();
            Page.Validate(valSummaryUpdate.ValidationGroup);
            if (!Page.IsValid)
            {
                mpeErrorPanel.Show();
                return;
            }
            PrepareXMLForAllLockoutDetails();
            var lockoutXML = FinalXML();
            using (var service = new ConfigurationService.ConfigurationServiceClient())
            {
                service.SaveLockoutDetails(lockoutXML);
                mlInsertStatus.ShowInfoMessage("Lock down details saved successfully");
            }
        }
    }
}
