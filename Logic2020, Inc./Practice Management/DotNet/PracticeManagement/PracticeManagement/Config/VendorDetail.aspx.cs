using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Projects;
using DataTransferObjects;
using PraticeManagement.VendorService;
using Resources;
using PraticeManagement.Utils;
using PraticeManagement.Configuration;
using System.Text.RegularExpressions;


namespace PraticeManagement.Config
{
    public partial class VendorDetail : PracticeManagementPageBase
    {
        #region Constants

        private const string VendorKey = "Vendor";
        private const string DuplicateName = "There is another vendor with the same Name.";
        private const string DuplicateContactName = "There is another vendor with the same Contact Name.";
        private const string DuplicateEmail = "There is another vendor with the Email.";
        private const string AttachmentHandlerUrl = "../Controls/Projects/AttachmentHandler.ashx?VendorId={0}&FileName={1}&AttachmentId={2}";
        private const string AttachMessage = "File should be in PDF, Word format, Excel, PowerPoint, MS Project, Visio, Exchange, OneNote, ZIP or RAR and should be no larger than {0} KB.";

        #endregion

        #region Properties

        private bool ShowPopup;
        private string ExMessage;

        public int? VendorId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    hdnVendorId.Value = SelectedId.Value.ToString();
                    return SelectedId;
                }
                if (!string.IsNullOrEmpty(hdnVendorId.Value))
                {
                    int vendorId;
                    if (Int32.TryParse(hdnVendorId.Value, out vendorId))
                    {
                        return vendorId;
                    }
                }
                return null;
            }
            set
            {
                hdnVendorId.Value = value.ToString();
            }
        }

        private string Email
        {
            get
            {
                string email = string.Empty;
                email = !string.IsNullOrEmpty(txtEmailAddress.Text) ? txtEmailAddress.Text + '@' + ddlDomain.SelectedValue : string.Empty;
                return email;
            }
        }

        public Vendor Vendor
        {
            get
            {
                return ViewState[VendorKey] as Vendor;
            }
            set
            {
                ViewState[VendorKey] = value;
            }
        }



        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
            if (VendorId.HasValue)
            {
                tcTabs.Visible = true;
                PopulateProjects();
                PopulateResources();

            }
            btnUpload.Attributes["onclick"] = "startUpload(); return false;";

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (ShowPopup)
            {
                mpeErrorPanel.Show();
            }
            if (IsPostBack)
            {
                activityLog.Update();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ValidateSaveAndPopulate(true);
        }

        private void ValidateSaveAndPopulate(bool showSuccessPopup)
        {
            if (ValidateAndSave())
            {
                ClearDirty();
                if (showSuccessPopup)
                {
                    ShowPopup = true;
                    mlConfirmation.ShowInfoMessage(string.Format(Messages.SavedDetailsConfirmation, "Vendor"));
                }
                if (Page.IsValid)
                {
                    Vendor = GetVendor(VendorId);
                    PopulateControls(Vendor);
                }
            }
            else
            {
                mlConfirmation.ClearMessage();
                ShowPopup = true;
            }
        }

        protected override bool ValidateAndSave()
        {
            Page.Validate(valsVendor.ValidationGroup);
            if (Page.IsValid)
            {
                ServerValidateVendor();
            }

            bool result = false;
            if (Page.IsValid)
            {
                Vendor vendor = new Vendor();
                PopulateData(vendor);
                var isNew = !VendorId.HasValue;
                using (var serviceClient = new VendorServiceClient())
                {
                    int? vendorId = serviceClient.SaveVendorDetail(vendor, User.Identity.Name);
                    if (vendorId.HasValue)
                    {
                        VendorId = vendorId;
                        result = true;
                    }
                    hdnVendorId.Value = vendorId.ToString();
                    if (isNew && VendorId.HasValue)
                    {
                        Response.Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.VendorDetail,
                                 VendorId.Value) + "&returnTo=" + Constants.ApplicationPages.Vendors);
                    }
                }
            }
            return result;
        }

        private void ServerValidateVendor()
        {
            var vendor = new Vendor();
            PopulateData(vendor);
            using (var serviceClient = new VendorServiceClient())
            {
                try
                {
                    serviceClient.VendorValidations(vendor);
                }
                catch (Exception ex)
                {
                    serviceClient.Abort();
                    ExMessage = ex.Message;
                    Page.Validate();

                    if (!Page.IsValid)
                    {
                        ShowPopup = true;
                    }
                }
            }
        }

        protected override void Display()
        {
            DataHelper.FillVendorTypeList(ddlVendorType, false);
            DataHelper.FillDomainsList(ddlDomain);
            if (VendorId.HasValue)
            {
                Vendor = GetVendor(VendorId);
                PopulateControls(Vendor);
                //PopulateAttachments();
            }
        }

        protected void custVendorName_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                e.IsValid = !(ExMessage == DuplicateName);
            }
        }

        protected void custContactName_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                e.IsValid = !(ExMessage == DuplicateContactName);
            }
        }
        protected void custEmailAddress_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                e.IsValid = !(ExMessage == DuplicateEmail);
            }
        }




        private void PopulateData(Vendor vendor)
        {
            vendor.Id = VendorId;
            vendor.Name = txtVendorName.Text;
            vendor.ContactName = txtContactName.Text;
            vendor.Status = ddlStatus.SelectedValue == "1" ? true : false;
            vendor.Email = Email;
            vendor.TelephoneNumber = FormatTelePhoneNumber();
            vendor.VendorType = new VendorType { Id = int.Parse(ddlVendorType.SelectedValue) };
        }

        private string FormatTelePhoneNumber()
        {
            string resultString = null;

            Regex regexObj = new Regex(@"[^\d]");
            resultString = regexObj.Replace(txtTelephoneNumber.Text, "");

            //International Number format XX-XXX-XXXXXXX
            if (resultString.Length >= 12)
            {
                resultString = resultString.Insert(resultString.Length - 7, "-");
                resultString = resultString.Insert(resultString.Length - 11, "-");
            }
            //US Number format (XXX) XXX-XXXX
            if (resultString.Length == 10)
            {
                resultString = resultString.Insert(resultString.Length - 4, "-");
                resultString = resultString.Insert(resultString.Length - 8, ") ");
                resultString = resultString.Insert(0, "(");
            }
            return resultString;

        }

        private void PopulateControls(Vendor vendor)
        {
            if (vendor != null)
            {
                txtVendorName.Text = vendor.Name;
                txtContactName.Text = vendor.ContactName;
                txtTelephoneNumber.Text = vendor.TelephoneNumber;
                txtEmailAddress.Text = vendor.EmailWithoutDomain;
                ddlDomain.SelectedValue = vendor.Domain;
                ddlStatus.SelectedValue = vendor.Status ? "1" : "0";
                ddlVendorType.SelectedValue = vendor.VendorType.Id.ToString();
                PopulateAttachments();
            }
        }

        private Vendor GetVendor(int? id)
        {
            using (var serviceClient = new VendorServiceClient())
            {
                try
                {
                    var vendor = serviceClient.GetVendorById(id.Value);
                    return vendor;
                }
                catch
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void lnkProjects_Click(object sender, EventArgs e)
        {
            PopulateProjects();
        }

        private void PopulateProjects()
        {
            using (var serviceClient = new VendorServiceClient())
            {
                var projects = serviceClient.ProjectListByVendor(VendorId.Value);
                if (projects.Length == 0)
                {
                    divProjectEmptyMessage.Style["display"] = "";
                    return;
                }
                repProjects.DataSource = projects;
                repProjects.DataBind();
                divProjectEmptyMessage.Style["display"] = "none";
            }
        }

        private void PopulateResources()
        {
            using (var serviceClient = new VendorServiceClient())
            {
                var persons = serviceClient.PersonListByVendor(VendorId.Value);
                if (persons.Length == 0)
                {
                    divEmptyResource.Style["display"] = "";
                    return;
                }
                repPersons.DataSource = persons;
                repPersons.DataBind();
                divEmptyResource.Style["display"] = "none";
            }
        }


        public bool CheckIfDefaultProject(object projectIdObj)
        {
            var defaultProjectId = MileStoneConfigurationManager.GetProjectId();
            var projectId = Int32.Parse(projectIdObj.ToString());
            return defaultProjectId.HasValue && defaultProjectId.Value == projectId;
        }

        protected void lnkResource_Click(object sender, EventArgs e)
        {
            PopulateResources();
        }

        protected void lnkHistory_Click(object sender, EventArgs e)
        {
            activityLog.Update();
        }

        protected void lnkAttachements_Click(object sender, EventArgs e)
        {
            if (VendorId.HasValue)
            {
                Vendor = GetVendor(VendorId.Value);
                PopulateAttachments();
            }
        }

        protected void imgbtnDeleteAttachment_Click(object sender, EventArgs e)
        {
            var deleteBtn = (ImageButton)sender;
            var id = Convert.ToInt32(deleteBtn.Attributes["AttachmentId"]);

            if (VendorId.HasValue)
            {
                using (var serviceClient = new VendorServiceClient())
                {
                    serviceClient.DeleteVendorAttachmentById(id, VendorId.Value, User.Identity.Name);
                }
                Vendor.Attachments.Remove(Vendor.Attachments.Find(va => va.AttachmentId == id));

                PopulateAttachments();
            }
        }

        private void PopulateAttachments()
        {
            if (Vendor != null && Vendor.Attachments != null && Vendor.Attachments.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repVendorAttachments.Visible = true;
                repVendorAttachments.DataSource = Vendor.Attachments;
                repVendorAttachments.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repVendorAttachments.Visible = false;
            }
        }

        protected void stbAttach_Click(object sender, EventArgs e)
        {
            if (IsDirty || !VendorId.HasValue)
            {
                ValidateSaveAndPopulate(false);
            }
            if (!IsDirty && VendorId.HasValue)
            {
                int size = Convert.ToInt32(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Project, Constants.ResourceKeys.AttachmentFileSize));
                lblAttachmentMessage.Text = string.Format(AttachMessage, size / 1000);
                mpeAttachSOW.Show();
            }
        }

        public string GetWrappedText(string name)
        {
            if (name.Length > 30)
            {
                for (int i = 30; i < name.Length; i = i + 30)
                {
                    name = name.Insert(i, "<wbr />");
                }
            }
            return name;
        }

        protected void lnkVendorAttachment_OnClick(object sender, EventArgs e)
        {
        }


        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            if (VendorId.HasValue)
            {
                Vendor = GetVendor(VendorId.Value);
                PopulateAttachments();
            }
        }

        public string GetNavigateUrl(string attachmentFileName, int attachmentId)
        {
            if (Vendor != null && Vendor.Id.HasValue)
            {
                return string.Format(AttachmentHandlerUrl, Vendor.Id.ToString(), HttpUtility.UrlEncode(attachmentFileName), attachmentId);
            }
            return string.Empty;
        }

        public string GetProjectLinkURL(object sender)
        {
            int projectId;
            if (sender is Project)
            {
                var project = sender as Project;
                projectId = project.Id.Value;
            }
            else if (sender == null)
            {
                return string.Empty;
            }
            else
            {
                projectId = (int)sender;
            }
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                    Constants.ApplicationPages.ProjectDetail,
                    projectId), Request.Url.AbsoluteUri + (Request.Url.Query.Length > 0 ? string.Empty : Constants.FilterKeys.QueryStringOfApplyFilterFromCookie));
        }

        protected string GetPersonDetailsUrlWithReturn(object obj)
        {
            int personId;
            if (obj is Person)
            {
                var person = obj as Person;
                personId = person.Id.Value;
            }
            else if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                personId = (int)obj;
            }
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(GetPersonDetailsUrl(personId),
                       Request.Url.AbsoluteUri + (Request.Url.Query.Length > 0 ? string.Empty : Constants.FilterKeys.QueryStringOfApplyFilterFromCookie));
        }

        private static string GetPersonDetailsUrl(object args)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.PersonDetail,
                                 args);
        }


    }
}

