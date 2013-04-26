using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ClientService;
using System.ServiceModel;

namespace PraticeManagement.Controls.Clients
{
    public partial class ClientPricingList : System.Web.UI.UserControl
    {


        private const string CLIENT_PRICING_KEY = "ClientPricingLists";

        #region PricingListProperties

        private PraticeManagement.ClientDetails HostingPage
        {
            get { return ((PraticeManagement.ClientDetails)Page); }
        }

        public List<PricingList> ClientPricingLists
        {
            get
            {
                if (ViewState[CLIENT_PRICING_KEY] == null)
                {
                    List<PricingList> pricinglist;
                    if (ClientId == 0)
                    {
                        pricinglist = new List<PricingList>() { new PricingList() { PricingListId = 1, Name = PricingList.DefaultPricingListName, IsDefault = true } };
                    }
                    else
                    {
                        pricinglist = ServiceCallers.Custom.Client(g => g.GetPricingLists(ClientId).OrderBy(p => p.Name).ToList());

                    }
                    ViewState[CLIENT_PRICING_KEY] = pricinglist.ToList();
                }
                return ((IEnumerable<PricingList>)ViewState[CLIENT_PRICING_KEY]).ToList();
            }
            set
            {
                if (value != null)
                {
                    value = value.OrderBy(g => g.Name).ToList();
                }
                ViewState[CLIENT_PRICING_KEY] = value;
            }
        }

        protected int? ClientId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Request.QueryString[Constants.QueryStringParameterNames.Id]);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region PricingListMethods

        private bool ValidatePricingListName(string pricingListName,int pricingListId)
        {
            return ClientPricingLists.Where(p => p.Name.Replace(" ", "").ToLowerInvariant() == pricingListName.Replace(" ", "").ToLowerInvariant() && (pricingListId != p.PricingListId || pricingListId == -1)).Count() == 0;
        }

        protected void custNewPricingListName_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = ValidatePricingListName(e.Value,-1);
        }

        protected void imgEdit_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var gvGroupsItem = imgEdit.NamingContainer as GridViewRow;
            gvPricingList.EditIndex = gvGroupsItem.DataItemIndex;
            gvPricingList.DataSource = ClientPricingLists;
            gvPricingList.DataBind();
            plusMakeVisible(true);
        }

        protected void imgUpdate_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var row = imgEdit.NamingContainer as GridViewRow;
            var custPricingListActive = gvPricingList.HeaderRow.FindControl("custPricingListActive") as CustomValidator;
            CustomValidator custGroupName = (CustomValidator)row.FindControl("custNewPricingList");
            List<PricingList> tmp = ClientPricingLists;
            int pricingListId = int.Parse(((HiddenField)row.FindControl("hidKey")).Value);
            PricingList oldPricingList = tmp.Any(g => g.PricingListId == pricingListId) ? tmp.First(g => g.PricingListId == pricingListId) : null;
            string oldPricingListName = oldPricingList != null ? oldPricingList.Name : string.Empty;
            CheckBox chbIsActiveEd = (CheckBox)row.FindControl("chbIsActiveEd");
            bool isActive = chbIsActiveEd.Checked;
            string PricingListName = ((TextBox)row.FindControl("txtPricingListName")).Text;
            Page.Validate("UpdatePricingList");

            if (oldPricingListName.ToLowerInvariant() != PricingListName.ToLowerInvariant())
            {
                custGroupName.IsValid = ValidatePricingListName(PricingListName, pricingListId);
            }
            if (!isActive && oldPricingList.IsActive != isActive && !tmp.Any(b => b.IsActive && b.PricingListId != pricingListId))
            {
                custPricingListActive.IsValid = false;
            }

            if (Page.IsValid)
            {

                UpDateProductGroup(pricingListId, PricingListName, isActive);
                if (oldPricingList != null)
                {
                    oldPricingList.Name = PricingListName;
                    oldPricingList.IsActive = isActive;
                }

                ClientPricingLists = tmp;
                gvPricingList.EditIndex = -1;
                gvPricingList.DataSource = ClientPricingLists;
                gvPricingList.DataBind();
            }
        }

        protected void imgDelete_OnClick(object sender, EventArgs e)
        {
            var imgDelete = sender as ImageButton;
            var row = imgDelete.NamingContainer as GridViewRow;
            var custPricingListDelete = row.FindControl("custPricingListDelete") as CustomValidator;
            List<PricingList> tmp = ClientPricingLists;
            int key = int.Parse(((HiddenField)row.FindControl("hidKey")).Value);
            if (tmp.Count <= 1)
            {
                custPricingListDelete.IsValid = false;
            }
            if (custPricingListDelete.IsValid)
              {
                  if (tmp.Any(p => p.PricingListId == key))
                      if (!tmp.First(g => g.PricingListId == key).InUse)
                      {
                          using (var serviceClient = new ClientServiceClient())
                          {
                              try
                              {
                                  serviceClient.PricingListDelete(key, HostingPage.User.Identity.Name);
                              }
                              catch (FaultException<ExceptionDetail>)
                              {
                                  serviceClient.Abort();
                                  throw;
                              }
                          }
                          tmp.Remove(tmp.First(g => g.PricingListId == key));
                      }

                  ClientPricingLists = tmp;
                  gvPricingList.DataSource = ClientPricingLists;
                  gvPricingList.DataBind();
              }
        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            gvPricingList.EditIndex = -1;
            gvPricingList.DataSource = ClientPricingLists;
            gvPricingList.DataBind();
        }

        protected void gvPricingList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var imgDelete = e.Row.FindControl("imgDelete") as ImageButton;
                if (imgDelete != null)
                {
                    var pricinglist = (PricingList)e.Row.DataItem;
                    imgDelete.Visible = !pricinglist.IsDefault && !pricinglist.InUse;
                }
            }
        }

        protected void btnAddPricingList_Click(object sender, EventArgs e)
        {
            Page.Validate("NewPricingList");
            if (Page.IsValid)
            {
                string pricingListName = txtNewPricingListName.Text;
                bool isactive = chbPricingListActive.Checked;
                if (ClientId == 0)
                {
                    List<PricingList> tmp = ClientPricingLists;
                    PricingList pricinglist = new PricingList { PricingListId = (ClientPricingLists.Count() > 0 ? ClientPricingLists.Min(g => g.PricingListId) - 1 : 0), Name = pricingListName, InUse = false };
                    plusMakeVisible(true);
                    tmp.Add(pricinglist);
                    ClientPricingLists = tmp;
                }
                else
                {
                    AddPricingList(pricingListName, isactive);
                    ClientPricingLists = null;
                }
                gvPricingList.Visible = true;
                DisplayPricingList(ClientPricingLists);
            }
        }

        public void DisplayPricingList(List<PricingList> pricinglist, bool fromMainPage = false)
        {
            if (fromMainPage)
            {
                gvPricingList.EditIndex = -1;
                plusMakeVisible(true);
            }
            if (!(ClientId == 0 && pricinglist == null))
            ClientPricingLists = pricinglist;
            gvPricingList.DataSource = ClientPricingLists;
            gvPricingList.DataBind();
        }

        private int AddPricingList(string pricingListName, bool isActive)
        {
            if (ClientId.HasValue)
                using (var serviceGroups = new ClientServiceClient())
                {
                    try
                    {
                        PricingList pricingList = new PricingList();
                        pricingList.ClientId = ClientId.Value;
                        pricingList.Name = pricingListName;
                        pricingList.IsActive = isActive;
                        int result = serviceGroups.PricingListInsert(pricingList, HostingPage.User.Identity.Name);
                        plusMakeVisible(true);
                        return result;
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceGroups.Abort();
                        throw;
                    }
                }

            return -1;
        }

        private void UpDateProductGroup(int pricingListId, string pricingListName,bool isActive)
        {
            if (ClientId.HasValue)
                using (var serviceClient = new ClientServiceClient())
                {
                    try
                    {
                        PricingList pricingList = new PricingList();
                        pricingList.ClientId = ClientId.Value;
                        pricingList.Name = pricingListName;
                        pricingList.PricingListId = pricingListId;
                        pricingList.IsActive = isActive;
                        serviceClient.PricingListUpdate(pricingList, HostingPage.User.Identity.Name);
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            if (gvPricingList.Rows.Count == 0)
            {
                gvPricingList.Visible = false;
            }
            plusMakeVisible(false);
        }

        private void plusMakeVisible(bool isplusVisible)
        {
            if (isplusVisible)
            {
                btnPlus.Visible = true;
                btnAddPricingList.Visible = false;
                btnCancel.Visible = false;
                chbPricingListActive.Visible = txtNewPricingListName.Visible = false;
            }
            else
            {
                btnPlus.Visible = false;
                btnAddPricingList.Visible = true;
                btnCancel.Visible = true;
                txtNewPricingListName.Text = string.Empty;
                chbPricingListActive.Checked = true;
                chbPricingListActive.Visible = txtNewPricingListName.Visible = true;
                if (gvPricingList.EditIndex > -1)
                {
                    gvPricingList.EditIndex = -1;
                    DisplayPricingList(ClientPricingLists);
                }
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            gvPricingList.Visible = true;
            plusMakeVisible(true);
        }

        #endregion
    }
}

