using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ConfigurationService;
using System.ServiceModel;

namespace PraticeManagement.Controls.Configuration
{
    public partial class EmailTemplates : System.Web.UI.UserControl
    {
        private int SelectedTemplateIndex
        {
            get { return ViewState["SelectedEmailTemplate"] == null ? -1 : (int)ViewState["SelectedEmailTemplate"]; }
            set { ViewState["SelectedEmailTemplate"] = value; }
        }

        private List<EmailTemplate> EmailTemplatesCollection
        {
            get { return ViewState["EmailTemplateCollection"] as List<EmailTemplate>; }
            set { ViewState["EmailTemplateCollection"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void lvEmailTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTemplate(SelectedTemplateIndex);
        }

        protected void lblEmptyTemplate_Click(object sender, EventArgs e)
        {
            SelectedTemplateIndex = -1;
            FillTemplate(SelectedTemplateIndex);
        }

        protected void lvEmailTemplates_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
        {
            SelectedTemplateIndex = e.NewSelectedIndex;
        }
        protected void btnRemove_Click(object sender, EventArgs e)
        {
            DeleteTemplate(SelectedTemplateIndex);
        }

        protected void btnAddAsNew_Click(object sender, EventArgs e)
        {
            Page.Validate("TemplateFields");
            if (Page.IsValid)
                AddTemplate(SelectedTemplateIndex);
        }

        protected void btnUpate_Click(object sender, EventArgs e)
        {
            Page.Validate("TemplateFields");
            if (Page.IsValid)
                UpdateTemplate(SelectedTemplateIndex);
        }

        private void FillTemplate(int index)
        {
            var title = txTemplateTitle;//lvEmailTemplates.FindControl("txTemplateTitle") as TextBox;
            var emailTo = txTemplateTo;
            var emailCc = txTemplateCc;
            var subject = txtEmailSubject;//lvEmailTemplates.FindControl("txtEmailSubject") as TextBox;
            var body = txtEmailBody;//lvEmailTemplates.FindControl("txtEmailBody") as TextBox;

            var removeButton = btnRemove;//lvEmailTemplates.FindControl("btnRemove") as Button;
            var updateButton = btnUpdate;// lvEmailTemplates.FindControl("btnUpdate") as Button;
            var addButton = btnAddAsNew;// lvEmailTemplates.FindControl("btnAddAsNew") as Button;

            removeButton.Enabled = updateButton.Enabled = index >= 0;
            addButton.Enabled = true;

            if (index < 0)
            {
                title.Text = string.Empty;
                emailTo.Text = string.Empty;
                emailCc.Text = string.Empty;
                subject.Text = string.Empty;
                body.Text = string.Empty;
            }
            else
            {
                title.Text = EmailTemplatesCollection[index].Name;
                emailTo.Text = EmailTemplatesCollection[index].EmailTemplateTo;
                emailCc.Text = EmailTemplatesCollection[index].EmailTemplateCc;
                subject.Text = EmailTemplatesCollection[index].Subject;
                body.Text = EmailTemplatesCollection[index].Body;
            }
        }

        private void DeleteTemplate(int index)
        {
            if (index < 0)
            {
                return; // empty template selected
            }

            using (var service = new ConfigurationServiceClient())
            {
                try
                {
                    if (EmailTemplatesCollection[index].Id.HasValue &&
                        service.DeleteEmailTemplate(EmailTemplatesCollection[index].Id.Value))
                    {
                        BindEmailTemplates(service);
                    }
                }
                catch (FaultException<ExceptionDetail>)
                {
                    service.Abort();
                    throw;
                }
            }
            SelectedTemplateIndex = -1;
            
            FillTemplate(SelectedTemplateIndex);
        }

        private void AddTemplate(int index)
        {
            var si = new EmailTemplate();

            var title = txTemplateTitle;
            var emailTo = txTemplateTo;
            var emailCc = txTemplateCc;
            var subject = txtEmailSubject;
            var body = txtEmailBody;

            foreach (EmailTemplate sc in EmailTemplatesCollection)
            {
                if (sc.Name == title.Text)
                {
                    TemplateNameCustomValidator.IsValid = false;
                    return;
                }
            }

            si.Name = title.Text;
            si.EmailTemplateTo = string.IsNullOrEmpty(emailTo.Text) ? null : emailTo.Text ;
            si.EmailTemplateCc = string.IsNullOrEmpty(emailCc.Text) ? null : emailCc.Text;
            si.Body = body.Text;
            si.Subject = subject.Text;


            using (var service = new ConfigurationServiceClient())
            {
                try
                {
                    if (service.AddEmailTemplate(si))
                    {
                        BindEmailTemplates(service);
                    }

                }
                catch (FaultException<ExceptionDetail>)
                {
                    service.Abort();
                    throw;
                }
            }
            SelectedTemplateIndex = EmailTemplatesCollection.IndexOf(si);
            BindData();
            FillTemplate(SelectedTemplateIndex);
        }
        
        private void UpdateTemplate(int index)
        {
            if (index < 0)
                return;

            var si = EmailTemplatesCollection[index];

            var title = txTemplateTitle;//lvEmailTemplates.FindControl("txTemplateTitle") as TextBox;
            var emailTo = txTemplateTo;
            var emailCc = txTemplateCc;
            var subject = txtEmailSubject;//lvEmailTemplates.FindControl("txtEmailSubject") as TextBox;
            var body = txtEmailBody;//lvEmailTemplates.FindControl("txtEmailBody") as TextBox;

            foreach (EmailTemplate sc in EmailTemplatesCollection)
            {
                if (sc.Name == title.Text && sc.Id != si.Id)
                {
                    TemplateNameCustomValidator.IsValid = false;
                    return;
                }
            }

            if (si.Name != title.Text || 
                si.EmailTemplateTo != txTemplateTo.Text ||
                si.EmailTemplateCc != txTemplateCc.Text ||
                si.Body != body.Text ||
                si.Subject != subject.Text)
            {
                si.Name = title.Text;
                si.EmailTemplateTo = string.IsNullOrEmpty(emailTo.Text) ? null : emailTo.Text;
                si.EmailTemplateCc = string.IsNullOrEmpty(emailCc.Text) ? null : emailCc.Text;
                si.Body = body.Text;
                si.Subject = subject.Text;
                using (var service = new ConfigurationServiceClient())
                {
                    try
                    {
                        if (service.UpdateEmailTemplate(si))
                        {
                            BindEmailTemplates(service);
                        }
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        service.Abort();
                        throw;
                    }
                }
            }

            BindData();
            FillTemplate(SelectedTemplateIndex);
        }

        private void BindEmailTemplates(ConfigurationServiceClient service)
        {
            EmailTemplatesCollection.Clear();
            var res = service.GetAllEmailTemplates();
            EmailTemplatesCollection.AddRange(res);
            lvEmailTemplates.DataSource = EmailTemplatesCollection;
            lvEmailTemplates.DataBind();
        }
        private void BindData()
        {
            if (EmailTemplatesCollection == null)
            {
                EmailTemplatesCollection = new List<EmailTemplate>();
            }
            using (var service = new ConfigurationServiceClient())
            {
                try
                {
                    BindEmailTemplates(service);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    service.Abort();
                    throw;
                }
            }
        }

        protected void lvEmailTemplates_LayoutCreated(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                FillTemplate(SelectedTemplateIndex);
            }
        }

    }
}
