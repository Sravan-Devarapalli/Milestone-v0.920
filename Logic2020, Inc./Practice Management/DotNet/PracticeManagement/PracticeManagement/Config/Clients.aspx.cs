﻿using System;
using System.ServiceModel;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ClientService;
using PraticeManagement.Controls;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Drawing;

namespace PraticeManagement.Config
{
    public partial class Clients : PracticeManagementPageBase
    {
        private const string ViewingRecords = "Viewing {0} - {1} of {2} Clients";

        private string previousAlphabetLnkButtonId
        {
            get
            {
                string value;

                value = ViewState["PreviousAlphabet"] != null ? (string)ViewState["PreviousAlphabet"] : null;

                return value;
            }
            set
            {
                ViewState["PreviousAlphabet"] = value;
            }
        }

        private const string CLIENTS_LIST_KEY = "ClientsListKey";

        private Client[] ClientsList
        {
            get
            {
                if (ViewState[CLIENTS_LIST_KEY] != null)
                {
                    return ViewState[CLIENTS_LIST_KEY] as Client[];
                }
                else
                {
                    using (var serviceClient = new ClientServiceClient())
                    {
                        try
                        {
                            var result = IsShowActive
                                    ? serviceClient.ClientListAll()
                                    : serviceClient.ClientListAllWithInactive();
                            ViewState[CLIENTS_LIST_KEY] = result;
                            return result;
                        }
                        catch (FaultException<ExceptionDetail>)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }
            }
        }

        private Client[] FilteredClientsList{get;set;}

        private bool IsShowActive
        {
            get { return Convert.ToBoolean(hdnActive.Value); }
            set { hdnActive.Value = value.ToString(); }
        }

        protected override void Display()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int pagesize = Convert.ToInt32(ddlView.SelectedValue);
            gvClients.PageSize = pagesize == -1 ? ClientsList.Count() : pagesize;

            if (pagesize == -1)
            {
                gvClients.AllowPaging = false;
            }
            else
            {
                gvClients.AllowPaging = true;
            }

            AddAlphabetButtons();
            if (!IsPostBack)
            {
                IsShowActive = true;
                ViewState.Remove(CLIENTS_LIST_KEY);
                DataBindClients(ClientsList);
                previousAlphabetLnkButtonId = lnkbtnAll.ID;
            }

            gvClients.EmptyDataText = "No results found.";
        }

        protected void DataBindClients(Client[] clientsList)
        {
            gvClients.DataSource = clientsList;

            if (!IsPostBack && clientsList.Length > 0)
            {
                gvClients.SelectedIndex = 0;
            }
            gvClients.DataBind();
        }

        protected void chbShowActive_CheckedChanged(object sender, EventArgs e)
        {
            IsShowActive = chbShowActive.Checked;
            gvClients.PageIndex = 0;
            ViewState.Remove(CLIENTS_LIST_KEY);

            LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousAlphabetLnkButtonId);
            Client[] FilteredClientList = previousLinkButton != null && previousLinkButton.Text != "All" ? ClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().StartsWith(previousLinkButton.Text.ToUpperInvariant())).ToArray() : ClientsList;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                FilteredClientList = FilteredClientList.AsQueryable().Where(c => c.Name.ToUpperInvariant().Contains(txtSearch.Text.ToUpperInvariant())).ToArray();
            }

            DataBindClients(FilteredClientList);
        }

        protected void chbInactive_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chbInactive = sender as CheckBox;
            var ClientId = Convert.ToInt32(chbInactive.Attributes["ClientId"]);

            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    if (chbInactive.Checked)
                    {
                        serviceClient.ClientReactivate(new Client() { Id = ClientId });
                    }
                    else
                    {
                        serviceClient.ClientInactivate(new Client() { Id = ClientId });
                    }
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }

            Client client = ClientsList.AsQueryable().Where(c => c.Id == ClientId).ToArray()[0];

            client.Inactive = !client.Inactive;
        }

        protected void chbIsChargeable_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chbIsChargeable = sender as CheckBox;
            var ClientId = Convert.ToInt32(chbIsChargeable.Attributes["ClientId"]);

            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    serviceClient.UpdateIsChargableForClient(ClientId, chbIsChargeable.Checked);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }

            Client client = ClientsList.AsQueryable().Where(c => c.Id == ClientId).ToArray()[0];

            client.IsChargeable = !client.IsChargeable;
        }

        protected void btnClientName_Command(object sender, CommandEventArgs e)
        {
            object args = e.CommandArgument;
            Response.Redirect(GetClientDetailsUrl(args));
        }

        protected string GetClientDetailsUrlWithReturn(object args)
        {
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(GetClientDetailsUrl(args), Request.Url.AbsoluteUri);
        }

        private static string GetClientDetailsUrl(object args)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.ClientDetails,
                                 args);
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchClients();
        }

        private void SearchClients()
        {
            hdnCleartoDefaultView.Value = "true";
            btnClearResults.Enabled = true;
            IsShowActive = false;
            gvClients.PageIndex = 0;
            ViewState.Remove(CLIENTS_LIST_KEY);

            if (previousAlphabetLnkButtonId != null)
            {
                LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousAlphabetLnkButtonId);

                LinkButton prevtopButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLinkButton.Attributes["Top"]);
                LinkButton prevbottomButton = (LinkButton)trAlphabeticalPaging1.FindControl(previousLinkButton.Attributes["Bottom"]);

                prevtopButton.Font.Bold = false;
                prevbottomButton.Font.Bold = false;
            }

            //lnkbtnAll.Font.Bold = true;
            //lnkbtnAll1.Font.Bold = true;
            hdnAlphabet.Value = null;
            previousAlphabetLnkButtonId = lnkbtnAll.ID;

            LinkButton preLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousAlphabetLnkButtonId);
            FilteredClientsList = preLinkButton != null && preLinkButton.Text != "All" ? ClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().StartsWith(preLinkButton.Text.ToUpperInvariant())).ToArray() : ClientsList;
            FilteredClientsList = FilteredClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().Contains(txtSearch.Text.ToUpperInvariant())).ToArray();
            DataBindClients(FilteredClientsList);

            SetEmptyDataText();
        }

        private void SetEmptyDataText()
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                if (gvClients.Rows.Count == 0)
                {
                    string txt = txtSearch.Text;
                    txt = "<b>" + txt + "</b>";
                    gvClients.EmptyDataText = string.Format("No results found for {0}", txt);
                }
            }
        }

        private void AddAlphabetButtons()
        {
            for (int index = 65; index <= 65 + 25; index++)
            {
                char alphabet = Convert.ToChar(index);

                LinkButton Alphabet = new LinkButton();
                Alphabet.ID = "lnkbtn" + alphabet;
                Alphabet.Attributes.Add("Top", "lnkbtn" + alphabet);
                Alphabet.Attributes.Add("Bottom", "lnkbtn1" + alphabet);

                HtmlTableCell tc = new HtmlTableCell();
                tc.ID = "td" + alphabet;
                tc.Style.Add("padding-left", "15px");
                tc.Style.Add("padding-top", "10px");
                tc.Style.Add("padding-bottom", "10px");
                tc.Style.Add("text-align", "center");

                Alphabet.Text = alphabet.ToString();
                Alphabet.Font.Underline = false;
                Alphabet.Click += new EventHandler(Alphabet_Clicked);

                tc.Controls.Add(Alphabet);

                trAlphabeticalPaging.Controls.Add(tc);

                LinkButton Alphabet1 = new LinkButton();
                Alphabet1.ID = "lnkbtn1" + alphabet;
                Alphabet1.Attributes.Add("Top", "lnkbtn" + alphabet);
                Alphabet1.Attributes.Add("Bottom", "lnkbtn1" + alphabet);


                HtmlTableCell tc1 = new HtmlTableCell();
                tc1.ID = "td1" + alphabet;
                tc1.Style.Add("padding-left", "15px");
                tc1.Style.Add("padding-top", "10px");
                tc1.Style.Add("padding-bottom", "10px");
                tc1.Style.Add("text-align", "center");

                Alphabet1.Text = alphabet.ToString();
                Alphabet1.Font.Underline = false;
                Alphabet1.Click += new EventHandler(Alphabet_Clicked);

                tc1.Controls.Add(Alphabet1);

                trAlphabeticalPaging1.Controls.Add(tc1);
            }
        }

        protected void Alphabet_Clicked(object sender, EventArgs e)
        {
            if (hdnCleartoDefaultView.Value == "true")
            {
                IsShowActive = true;
                ViewState.Remove(CLIENTS_LIST_KEY);
            }

            txtSearch.Text = string.Empty;
            gvClients.PageIndex = 0;
            btnClearResults.Enabled = false;
            hdnCleartoDefaultView.Value = "false";

            if (previousAlphabetLnkButtonId != null)
            {
                LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousAlphabetLnkButtonId);

                LinkButton prevtopButton = (LinkButton)trAlphabeticalPaging.FindControl(previousLinkButton.Attributes["Top"]);
                LinkButton prevbottomButton = (LinkButton)trAlphabeticalPaging1.FindControl(previousLinkButton.Attributes["Bottom"]);

                prevtopButton.Font.Bold = false;
                prevbottomButton.Font.Bold = false;
            }

            LinkButton alpha = (LinkButton)sender;

            LinkButton topButton = (LinkButton)trAlphabeticalPaging.FindControl(alpha.Attributes["Top"]);
            LinkButton bottomButton = (LinkButton)trAlphabeticalPaging1.FindControl(alpha.Attributes["Bottom"]);

            topButton.Font.Bold = true;
            bottomButton.Font.Bold = true;
            hdnAlphabet.Value = topButton.Text != "All" ? topButton.Text : null;
            previousAlphabetLnkButtonId = topButton.ID;

            FilteredClientsList = alpha.Text != "All" ? ClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().StartsWith(alpha.Text.ToUpperInvariant())).ToArray() : ClientsList;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                FilteredClientsList = FilteredClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().Contains(txtSearch.Text.ToUpperInvariant())).ToArray();
            }

            DataBindClients(FilteredClientsList);

            SetEmptyDataText();
        }

        protected void Previous_Clicked(object sender, EventArgs e)
        {
            gvClients.PageIndex = gvClients.PageIndex - 1;
            LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousAlphabetLnkButtonId);
            Client[] FilteredClientList = previousLinkButton != null && previousLinkButton.Text != "All" ? ClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().StartsWith(previousLinkButton.Text.ToUpperInvariant())).ToArray() : ClientsList;
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                FilteredClientList = FilteredClientList.AsQueryable().Where(c => c.Name.ToUpperInvariant().Contains(txtSearch.Text.ToUpperInvariant())).ToArray();
            }

            DataBindClients(FilteredClientList);

            SetEmptyDataText();
        }

        protected void ResetFilter_Clicked(object sender, EventArgs e)
        {
            btnClearResults.Enabled = false;
            hdnCleartoDefaultView.Value = "false";
            txtSearch.Text = string.Empty;
            IsShowActive = chbShowActive.Checked = true;
            ViewState.Remove(CLIENTS_LIST_KEY);
            DataBindClients(ClientsList);
            previousAlphabetLnkButtonId = lnkbtnAll.ID;

            lnkbtnAll.Font.Bold = true;
            lnkbtnAll1.Font.Bold = true;

        }

        protected void Next_Clicked(object sender, EventArgs e)
        {
            gvClients.PageIndex = gvClients.PageIndex + 1;
            LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousAlphabetLnkButtonId);
            Client[] FilteredClientList = previousLinkButton != null && previousLinkButton.Text != "All" ? ClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().StartsWith(previousLinkButton.Text.ToUpperInvariant())).ToArray() : ClientsList;
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                FilteredClientList = FilteredClientList.AsQueryable().Where(c => c.Name.ToUpperInvariant().Contains(txtSearch.Text.ToUpperInvariant())).ToArray();
            }

            DataBindClients(FilteredClientList);

            SetEmptyDataText();
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            LinkButton previousLinkButton = (LinkButton)trAlphabeticalPaging.FindControl(previousAlphabetLnkButtonId);

            Client[] FilteredClientList = previousLinkButton.Text != "All" ? ClientsList.AsQueryable().Where(c => c.Name.ToUpperInvariant().StartsWith(previousLinkButton.Text.ToUpperInvariant())).ToArray() : ClientsList;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                FilteredClientList = FilteredClientList.AsQueryable().Where(c => c.Name.ToUpperInvariant().Contains(txtSearch.Text.ToUpperInvariant())).ToArray();
            }

            DataBindClients(FilteredClientList);

            SetEmptyDataText();
        }

        public void EnableOrDisablePrevNextButtons()
        {
            if (gvClients.PageIndex == 0)
            {
                lnkbtnPrevious.Enabled = lnkbtnPrevious1.Enabled = false;
            }
            else
            {
                lnkbtnPrevious.Enabled = lnkbtnPrevious1.Enabled = true;
            }

            if (!lnkbtnPrevious.Enabled)
            {
                Color color = ColorTranslator.FromHtml("#8F8F8F");
                lnkbtnPrevious.ForeColor = lnkbtnPrevious1.ForeColor = color;
            }
            else
            {
                Color color = ColorTranslator.FromHtml("#0898E6");
                lnkbtnPrevious.ForeColor = lnkbtnPrevious1.ForeColor = color;
            }

            if (gvClients.PageCount - 1 == gvClients.PageIndex || gvClients.PageCount == 0)
            {
                lnkbtnNext.Enabled = lnkbtnNext1.Enabled = false;
            }
            else
            {
                lnkbtnNext.Enabled = lnkbtnNext1.Enabled = true;
            }

            if (!lnkbtnNext.Enabled)
            {
                Color color = ColorTranslator.FromHtml("#8F8F8F");
                lnkbtnNext.ForeColor = lnkbtnNext1.ForeColor = color;
            }
            else
            {
                Color color = ColorTranslator.FromHtml("#0898E6");
                lnkbtnNext.ForeColor = lnkbtnNext1.ForeColor = color;
            }
        }

        protected void btnSearchAll_OnClick(object sender, EventArgs e)
        {
            SearchClients();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            EnableOrDisablePrevNextButtons();
        }

        protected void gvClients_PreRender(object sender, EventArgs e)
        {
            //Page View Count.
            int currentRecords = gvClients.Rows.Count;
            int totalRecords = FilteredClientsList == null ? ClientsList.Count() : FilteredClientsList.Count();
            int startIndex = currentRecords == 0 ? 0 : (gvClients.PageIndex == 0 ? 1 : (gvClients.PageIndex * Convert.ToInt32(ddlView.SelectedValue)) + 1);
            lblPageNumbering.Text = String.Format(ViewingRecords, startIndex, currentRecords == 0 ? 0 : (startIndex + currentRecords - 1), totalRecords);
        }
    }
}

