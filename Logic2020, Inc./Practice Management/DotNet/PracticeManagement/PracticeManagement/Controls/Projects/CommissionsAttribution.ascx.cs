using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using DataTransferObjects;
using PraticeManagement.ProjectService;

namespace PraticeManagement.Controls.Projects
{
    public partial class CommissionsAttribution : System.Web.UI.UserControl
    {
        #region Constants

        private const string projectAttributionXmlKey = "ProjectAttribution_Key";
        private const string deliveryPersonAttributionXmlKey = "DeliveryPersonAttribution_Key";
        private const string deliveryPracticeAttributionXmlKey = "DeliveryPracticeAttribution_Key";
        private const string salesPersonAttributionXmlKey = "SalesPersonAttribution_Key";
        private const string salesPracticeAttributionXmlKey = "SalesPracticeAttribution_Key";
        private const string deliveryPersonAttribution = "DeliveryPerson";
        private const string deliveryPracticeAttribution = "DeliveryPractice";
        private const string salesPersonAttribution = "SalesPerson";
        private const string salesPracticeAttribution = "SalesPractice";

        #endregion Constants

        #region XMLConstants

        public const string AttributionTypeIdXname = "AttributionTypeId";
        public const string AttributionRecordTypeIdXname = "AttributionRecordTypeId";
        public const string AttributionIdXname = "AttributionId";
        public const string TargetIdXname = "TargetId";
        public const string TargetNameXname = "TargetName";
        public const string TitleIdXname = "TitleId";
        public const string TitleXname = "Title";
        public const string StartDateXname = "StartDate";
        public const string EndDateXname = "EndDate";
        public const string PercentageXname = "Percentage";
        public const string IsEditModeXname = "IsEditMode";
        public const string IsNewEntryXname = "IsNewEntry";
        public const string TempTargetIdXname = "TempTargetId";
        public const string TempTargetNameXname = "TempTargetName";
        public const string TempStartDateXname = "TempStartDate";
        public const string TempEndDateXname = "TempEndDate";
        public const string TempPercentageXname = "TempPercentage";
        public const string IsCheckboxCheckedXname = "IsCheckboxChecked";
        private const string AttributionTypeXname = "AttributionType";
        private const string AttributionXname = "Attribution";
        private const string AttributionRecordTypeXname = "AttributionRecordType";
        private const string AttributionsXmlOpen = "<Attributions>";
        private const string AttributionsXmlClose = "</Attributions>";
        private const string AttributionTypeXmlOpen = "<AttributionType AttributionTypeId=\"{0}\">";
        private const string AttributionTypeXmlClose = "</AttributionType>";
        private const string AttributionRecordTypeXmlOpen = "<AttributionRecordType AttributionRecordTypeId=\"{0}\">";
        private const string AttributionRecordTypeXmlClose = "</AttributionRecordType>";
        private const string AttributionXmlOpen = "<Attribution AttributionId=\"{0}\" TargetId=\"{1}\" TargetName=\"{2}\" StartDate=\"{3}\" EndDate=\"{4}\" Percentage=\"{5}\" TitleId=\"{6}\" Title=\"{7}\" IsEditMode=\"{8}\" IsNewEntry=\"{9}\" IsCheckboxChecked=\"{10}\" TempTargetId=\"{11}\" TempTargetName=\"{12}\" TempStartDate=\"{13}\" TempEndDate=\"{14}\" TempPercentage=\"{15}\">";
        private const string AttributionXmlClose = "</Attribution>";

        #endregion XMLConstants

        #region Properities

        private PraticeManagement.ProjectDetail HostingPage
        {
            get { return ((PraticeManagement.ProjectDetail)Page); }
        }

        public string ValidationGroup
        {
            get;
            set;
        }

        public List<Attribution> ProjectAttribution { get; set; }

        public List<Attribution> DeliveryPersonAttribution { get; set; }

        public List<Attribution> DeliveryPracticeAttribution { get; set; }

        public List<Attribution> SalesPersonAttribution { get; set; }

        public List<Attribution> SalesPracticeAttribution { get; set; }

        public List<string> Persons { get; set; }

        public string DeliveryPersonAttributionXML
        {
            get { return (string)ViewState[deliveryPersonAttributionXmlKey]; }
            set { ViewState[deliveryPersonAttributionXmlKey] = value; }
        }

        public string DeliveryPracticeAttributionXML
        {
            get { return (string)ViewState[deliveryPracticeAttributionXmlKey]; }
            set { ViewState[deliveryPracticeAttributionXmlKey] = value; }
        }

        public string SalesPersonAttributionXML
        {
            get { return (string)ViewState[salesPersonAttributionXmlKey]; }
            set { ViewState[salesPersonAttributionXmlKey] = value; }
        }

        public string SalesPracticeAttributionXML
        {
            get { return (string)ViewState[salesPracticeAttributionXmlKey]; }
            set { ViewState[salesPracticeAttributionXmlKey] = value; }
        }

        public int ProjectId { get; set; }

        public enum AttributionCategory
        {
            DeliveryPersonAttribution = 1,
            SalesPersonAttribution = 2,
            DeliveryPracticeAttribution = 3,
            SalesPracticeAttribution = 4
        }

        #endregion Properities

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HostingPage.ProjectId.HasValue)
                {
                    GetProjectAttributionValues();
                    BindAttributions();
                }
            }
        }

        protected void chbDeliveryAttributes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chbDeliveryAttribute = sender as CheckBox;
            GridViewRow row = chbDeliveryAttribute.NamingContainer as GridViewRow;
            Label lblpersonName = gvDeliveryAttributionPerson.Rows[row.DataItemIndex].FindControl("lblPersonName") as Label;
            Label lblStartDate = gvDeliveryAttributionPerson.Rows[row.DataItemIndex].FindControl("lblStartDate") as Label;
            XDocument xdoc = XDocument.Parse(DeliveryPersonAttributionXML);
            List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
            for (int i = 0; i < xlist.Count; i++)
            {
                if (xlist[i].Attribute(XName.Get(TargetNameXname)).Value == lblpersonName.Text && Convert.ToDateTime(xlist[i].Attribute(XName.Get(StartDateXname)).Value).Date.ToShortDateString() == lblStartDate.Text && xlist[i].Attribute(XName.Get(IsEditModeXname)).Value == false.ToString())
                {
                    xlist[i].Attribute(XName.Get(IsCheckboxCheckedXname)).Value = chbDeliveryAttribute.Checked.ToString();
                }
            }
            DeliveryPersonAttributionXML = xdoc.ToString();
        }

        protected void chbSalesAttributes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chbSalesAttribute = sender as CheckBox;
            GridViewRow row = chbSalesAttribute.NamingContainer as GridViewRow;
            Label lblpersonName = gvSalesAttributionPerson.Rows[row.DataItemIndex].FindControl("lblPersonName") as Label;
            Label lblStartDate = gvSalesAttributionPerson.Rows[row.DataItemIndex].FindControl("lblStartDate") as Label;
            XDocument xdoc = XDocument.Parse(SalesPersonAttributionXML);
            List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
            for (int i = 0; i < xlist.Count; i++)
            {
                if (xlist[i].Attribute(XName.Get(TargetNameXname)).Value == lblpersonName.Text && Convert.ToDateTime(xlist[i].Attribute(XName.Get(StartDateXname)).Value).Date.ToShortDateString() == lblStartDate.Text && xlist[i].Attribute(XName.Get(IsEditModeXname)).Value == false.ToString())
                {
                    xlist[i].Attribute(XName.Get(IsCheckboxCheckedXname)).Value = chbSalesAttribute.Checked.ToString();
                }
            }
            SalesPersonAttributionXML = xdoc.ToString();
        }

        protected void gvDeliveryAttributionPerson_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                Persons = new List<string>();
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = (XElement)e.Row.DataItem;
                EnableDisableValidators(e.Row, deliveryPersonAttribution);
                CheckBox chbDeliveryAttributes = e.Row.FindControl("chbDeliveryAttributes") as CheckBox;
                Label lblPersonName = e.Row.FindControl("lblPersonName") as Label;
                HiddenField hdnPersonId = e.Row.FindControl("hdnPersonId") as HiddenField;
                HiddenField hdnEditMode = e.Row.FindControl("hdnEditMode") as HiddenField;
                Label lblTitleName = e.Row.FindControl("lblTitleName") as Label;
                Label lblStartDate = e.Row.FindControl("lblStartDate") as Label;
                Label lblEndDate = e.Row.FindControl("lblEndDate") as Label;

                ImageButton imgDeliveryPersonAttributeEdit = e.Row.FindControl("imgDeliveryPersonAttributeEdit") as ImageButton;
                ImageButton imgDeliveryPersonAttributeUpdate = e.Row.FindControl("imgDeliveryPersonAttributeUpdate") as ImageButton;
                ImageButton imgDeliveryPersonAttributeCancel = e.Row.FindControl("imgDeliveryPersonAttributeCancel") as ImageButton;
                ImageButton imgDeliveryAttributionAdditionalAllocationOfResource = e.Row.FindControl("imgDeliveryAttributionAdditionalAllocationOfResource") as ImageButton;
                ImageButton imgDeliveryAttributionPersonDelete = e.Row.FindControl("imgDeliveryAttributionPersonDelete") as ImageButton;

                DatePicker dpStartDate = e.Row.FindControl("dpStartDate") as DatePicker;
                DatePicker dpEndDate = e.Row.FindControl("dpEndDate") as DatePicker;
                DropDownList ddlPerson = e.Row.FindControl("ddlPerson") as DropDownList;

                imgDeliveryAttributionPersonDelete.Attributes.Add(AttributionIdXname, item.Attribute(XName.Get(AttributionIdXname)).Value);
                imgDeliveryPersonAttributeCancel.Attributes.Add(IsNewEntryXname, item.Attribute(XName.Get(IsNewEntryXname)).Value);

                chbDeliveryAttributes.Checked = item.Attribute(XName.Get(IsCheckboxCheckedXname)).Value == true.ToString();
                lblPersonName.Text = item.Attribute(XName.Get(TempTargetNameXname)).Value;
                hdnPersonId.Value = item.Attribute(XName.Get(TargetIdXname)).Value;
                lblTitleName.Text = item.Attribute(XName.Get(TitleXname)).Value;
                hdnEditMode.Value = item.Attribute(XName.Get(IsEditModeXname)).Value;
                lblStartDate.Text = item.Attribute(XName.Get(TempStartDateXname)).Value != string.Empty ? Convert.ToDateTime(item.Attribute(XName.Get(TempStartDateXname)).Value).ToShortDateString() : string.Empty;
                lblEndDate.Text = item.Attribute(XName.Get(TempEndDateXname)).Value != string.Empty ? Convert.ToDateTime(item.Attribute(XName.Get(TempEndDateXname)).Value).ToShortDateString() : string.Empty;
                if (item.Attribute(XName.Get(IsEditModeXname)).Value == "True")
                {
                    chbDeliveryAttributes.Visible = lblPersonName.Visible = lblTitleName.Visible = lblStartDate.Visible = lblEndDate.Visible = imgDeliveryAttributionAdditionalAllocationOfResource.Visible = imgDeliveryAttributionPersonDelete.Visible = imgDeliveryPersonAttributeEdit.Visible = false;
                    ddlPerson.Visible = imgDeliveryPersonAttributeUpdate.Visible = imgDeliveryPersonAttributeCancel.Visible = dpStartDate.Visible = dpEndDate.Visible = true;

                    DataHelper.FillPersonListByDivisionId(ddlPerson, "-- Select a Person --", 2);//Fill persons with Consulting division(i.e. division Id = 2)
                    ListItem selectedPerson = null;
                    selectedPerson = ddlPerson.Items.FindByValue(item.Attribute(XName.Get(TargetIdXname)).Value);
                    if (item.Attribute(XName.Get(IsNewEntryXname)).Value != "True" || item.Attribute(XName.Get(TargetIdXname)).Value != "0")
                    {
                        if (selectedPerson == null)
                        {
                            selectedPerson = new ListItem(lblPersonName.Text, item.Attribute(XName.Get(TargetIdXname)).Value);
                            ddlPerson.Items.Add(selectedPerson);
                        }
                        ddlPerson.SelectedValue = selectedPerson.Value;
                    }
                    else
                    {
                        ddlPerson.SelectedValue = string.Empty;
                    }
                    dpStartDate.TextValue = lblStartDate.Text;
                    dpEndDate.TextValue = lblEndDate.Text;
                }
                if (Persons.Any(p => p == lblPersonName.Text))
                {
                    ddlPerson.Visible = lblPersonName.Visible = imgDeliveryAttributionAdditionalAllocationOfResource.Visible = false;
                }
                Persons.Add(lblPersonName.Text);
            }
        }

        protected void gvSalesAttributionPractice_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                EnableDisableValidators(e.Row, salesPracticeAttribution);
                var item = (XElement)e.Row.DataItem;
                Label lblPractice = e.Row.FindControl("lblPractice") as Label;
                Label lblCommisssionPercentage = e.Row.FindControl("lblCommisssionPercentage") as Label;
                HiddenField hdnPracticeId = e.Row.FindControl("hdnPracticeId") as HiddenField;
                HiddenField hdnEditMode = e.Row.FindControl("hdnEditMode") as HiddenField;
                ImageButton imgSalesPracticeAttributeEdit = e.Row.FindControl("imgSalesPracticeAttributeEdit") as ImageButton;
                ImageButton imgSalesPracticeAttributeUpdate = e.Row.FindControl("imgSalesPracticeAttributeUpdate") as ImageButton;
                ImageButton imgSalesPracticeAttributeCancel = e.Row.FindControl("imgSalesPracticeAttributeCancel") as ImageButton;
                ImageButton imgSalesAttributionPracticeDelete = e.Row.FindControl("imgSalesAttributionPracticeDelete") as ImageButton;

                TextBox txtCommisssionPercentage = e.Row.FindControl("txtCommisssionPercentage") as TextBox;
                DropDownList ddlPractice = e.Row.FindControl("ddlPractice") as DropDownList;

                imgSalesAttributionPracticeDelete.Attributes.Add(AttributionIdXname, item.Attribute(XName.Get(AttributionIdXname)).Value);
                imgSalesPracticeAttributeCancel.Attributes.Add(IsNewEntryXname, item.Attribute(XName.Get(IsNewEntryXname)).Value);

                lblPractice.Text = item.Attribute(XName.Get(TempTargetNameXname)).Value;
                hdnPracticeId.Value = item.Attribute(XName.Get(TargetIdXname)).Value;
                hdnEditMode.Value = item.Attribute(XName.Get(IsEditModeXname)).Value;
                lblCommisssionPercentage.Text = item.Attribute(XName.Get(TempPercentageXname)).Value;

                if (item.Attribute(XName.Get(IsEditModeXname)).Value == "True")
                {
                    lblPractice.Visible = lblCommisssionPercentage.Visible = imgSalesPracticeAttributeEdit.Visible = imgSalesAttributionPracticeDelete.Visible = false;
                    imgSalesPracticeAttributeUpdate.Visible = imgSalesPracticeAttributeCancel.Visible = txtCommisssionPercentage.Visible = ddlPractice.Visible = true;
                    DataHelper.FillPracticeListOnlyActive(ddlPractice, "-- Select a Practice Area--");
                    if (item.Attribute(XName.Get(IsNewEntryXname)).Value != "True")
                    {
                        ListItem selectedPractice = null;
                        selectedPractice = ddlPractice.Items.FindByValue(item.Attribute(XName.Get(TargetIdXname)).Value);
                        if (selectedPractice == null)
                        {
                            selectedPractice = new ListItem(lblPractice.Text, item.Attribute(XName.Get(TargetIdXname)).Value);
                            ddlPractice.Items.Add(selectedPractice);
                        }
                        ddlPractice.SelectedValue = selectedPractice.Value;
                    }
                    else
                    {
                        ddlPractice.SelectedValue = string.Empty;
                        List<int> practices = AvailablePractices(SalesPracticeAttributionXML);
                        if (practices.Count > 0)
                        {
                            foreach (var i in practices)
                            {
                                ListItem selectedPractice = null;
                                selectedPractice = ddlPractice.Items.FindByValue(i.ToString());
                                ddlPractice.Items.Remove(selectedPractice);
                            }
                        }
                    }
                    txtCommisssionPercentage.Text = lblCommisssionPercentage.Text;
                }
            }
        }

        protected void gvSalesAttributionPerson_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                Persons = new List<string>();
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                EnableDisableValidators(e.Row, salesPersonAttribution);
                var item = (XElement)e.Row.DataItem;
                CheckBox chbSalesAttributes = e.Row.FindControl("chbSalesAttributes") as CheckBox;
                Label lblPersonName = e.Row.FindControl("lblPersonName") as Label;
                HiddenField hdnPersonId = e.Row.FindControl("hdnPersonId") as HiddenField;
                HiddenField hdnEditMode = e.Row.FindControl("hdnEditMode") as HiddenField;
                Label lblTitleName = e.Row.FindControl("lblTitleName") as Label;
                Label lblStartDate = e.Row.FindControl("lblStartDate") as Label;
                Label lblEndDate = e.Row.FindControl("lblEndDate") as Label;

                ImageButton imgSalesPersonAttributeEdit = e.Row.FindControl("imgSalesPersonAttributeEdit") as ImageButton;
                ImageButton imgSalesPersonAttributeUpdate = e.Row.FindControl("imgSalesPersonAttributeUpdate") as ImageButton;
                ImageButton imgSalesPersonAttributeCancel = e.Row.FindControl("imgSalesPersonAttributeCancel") as ImageButton;
                ImageButton imgSalesAttributionAdditionalAllocationOfResource = e.Row.FindControl("imgSalesAttributionAdditionalAllocationOfResource") as ImageButton;
                ImageButton imgSalesAttributionPersonDelete = e.Row.FindControl("imgSalesAttributionPersonDelete") as ImageButton;

                DatePicker dpStartDate = e.Row.FindControl("dpStartDate") as DatePicker;
                DatePicker dpEndDate = e.Row.FindControl("dpEndDate") as DatePicker;
                DropDownList ddlPerson = e.Row.FindControl("ddlPerson") as DropDownList;

                imgSalesAttributionPersonDelete.Attributes.Add(AttributionIdXname, item.Attribute(XName.Get(AttributionIdXname)).Value);
                imgSalesPersonAttributeCancel.Attributes.Add(IsNewEntryXname, item.Attribute(XName.Get(IsNewEntryXname)).Value);

                hdnEditMode.Value = item.Attribute(XName.Get(IsEditModeXname)).Value;
                chbSalesAttributes.Checked = item.Attribute(XName.Get(IsCheckboxCheckedXname)).Value == true.ToString();
                lblPersonName.Text = item.Attribute(XName.Get(TempTargetNameXname)).Value;
                hdnPersonId.Value = item.Attribute(XName.Get(TargetIdXname)).Value;
                lblTitleName.Text = item.Attribute(XName.Get(TitleXname)).Value;
                lblStartDate.Text = Convert.ToDateTime(item.Attribute(XName.Get(TempStartDateXname)).Value).ToShortDateString();
                lblEndDate.Text = Convert.ToDateTime(item.Attribute(XName.Get(TempEndDateXname)).Value).ToShortDateString();
                if (item.Attribute(XName.Get(IsEditModeXname)).Value == "True")
                {
                    chbSalesAttributes.Visible = lblPersonName.Visible = lblTitleName.Visible = lblStartDate.Visible = lblEndDate.Visible = imgSalesAttributionAdditionalAllocationOfResource.Visible = imgSalesAttributionPersonDelete.Visible = imgSalesPersonAttributeEdit.Visible = false;
                    ddlPerson.Visible = imgSalesPersonAttributeUpdate.Visible = imgSalesPersonAttributeCancel.Visible = dpStartDate.Visible = dpEndDate.Visible = true;
                    DataHelper.FillPersonListByDivisionId(ddlPerson, "-- Select a Person --", 2);//Fill persons with Consulting division(i.e. division Id = 2)
                    ListItem selectedPerson = null;
                    selectedPerson = ddlPerson.Items.FindByValue(item.Attribute(XName.Get(TargetIdXname)).Value);
                    if (item.Attribute(XName.Get(IsNewEntryXname)).Value != "True" || item.Attribute(XName.Get(TargetIdXname)).Value != "0")
                    {
                        if (selectedPerson == null)
                        {
                            selectedPerson = new ListItem(lblPersonName.Text, item.Attribute(XName.Get(TargetIdXname)).Value);
                            ddlPerson.Items.Add(selectedPerson);
                        }
                        ddlPerson.SelectedValue = selectedPerson.Value;
                    }
                    else
                        ddlPerson.SelectedValue = string.Empty;

                    dpStartDate.TextValue = lblStartDate.Text;
                    dpEndDate.TextValue = lblEndDate.Text;
                }
                if (Persons.Any(p => p == lblPersonName.Text))
                {
                    ddlPerson.Visible = lblPersonName.Visible = imgSalesAttributionAdditionalAllocationOfResource.Visible = false;
                }
                Persons.Add(lblPersonName.Text);
            }
        }

        protected void gvDeliveryAttributionPractice_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                EnableDisableValidators(e.Row, deliveryPracticeAttribution);
                var item = (XElement)e.Row.DataItem;
                Label lblPractice = e.Row.FindControl("lblPractice") as Label;
                HiddenField hdnPracticeId = e.Row.FindControl("hdnPracticeId") as HiddenField;
                Label lblCommisssionPercentage = e.Row.FindControl("lblCommisssionPercentage") as Label;
                HiddenField hdnEditMode = e.Row.FindControl("hdnEditMode") as HiddenField;
                ImageButton imgDeliveryPracticeAttributeEdit = e.Row.FindControl("imgDeliveryPracticeAttributeEdit") as ImageButton;
                ImageButton imgDeliveryPracticeAttributeUpdate = e.Row.FindControl("imgDeliveryPracticeAttributeUpdate") as ImageButton;
                ImageButton imgDeliveryPracticeAttributeCancel = e.Row.FindControl("imgDeliveryPracticeAttributeCancel") as ImageButton;
                ImageButton imgDeliveryAttributionPracticeDelete = e.Row.FindControl("imgDeliveryAttributionPracticeDelete") as ImageButton;

                TextBox txtCommisssionPercentage = e.Row.FindControl("txtCommisssionPercentage") as TextBox;
                DropDownList ddlPractice = e.Row.FindControl("ddlPractice") as DropDownList;

                imgDeliveryAttributionPracticeDelete.Attributes.Add(AttributionIdXname, item.Attribute(XName.Get(AttributionIdXname)).Value);
                imgDeliveryPracticeAttributeCancel.Attributes.Add(IsNewEntryXname, item.Attribute(XName.Get(IsNewEntryXname)).Value);

                hdnEditMode.Value = item.Attribute(XName.Get(IsEditModeXname)).Value;
                lblPractice.Text = item.Attribute(XName.Get(TempTargetNameXname)).Value;
                hdnPracticeId.Value = item.Attribute(XName.Get(TargetIdXname)).Value;
                lblCommisssionPercentage.Text = item.Attribute(XName.Get(TempPercentageXname)).Value;

                if (item.Attribute(XName.Get(IsEditModeXname)).Value == "True")
                {
                    lblPractice.Visible = lblCommisssionPercentage.Visible = imgDeliveryPracticeAttributeEdit.Visible = imgDeliveryAttributionPracticeDelete.Visible = false;
                    imgDeliveryPracticeAttributeUpdate.Visible = imgDeliveryPracticeAttributeCancel.Visible = txtCommisssionPercentage.Visible = ddlPractice.Visible = true;
                    DataHelper.FillPracticeListOnlyActive(ddlPractice, "-- Select a Practice Area--");
                    if (item.Attribute(XName.Get(IsNewEntryXname)).Value != "True")
                    {
                        ListItem selectedPractice = null;
                        selectedPractice = ddlPractice.Items.FindByValue(item.Attribute(XName.Get(TargetIdXname)).Value);
                        if (selectedPractice == null)
                        {
                            selectedPractice = new ListItem(lblPractice.Text, item.Attribute(XName.Get(TargetIdXname)).Value);
                            ddlPractice.Items.Add(selectedPractice);
                        }
                        ddlPractice.SelectedValue = selectedPractice.Value;
                    }
                    else
                    {
                        ddlPractice.SelectedValue = string.Empty;
                        List<int> practices = AvailablePractices(DeliveryPracticeAttributionXML);
                        if (practices.Count > 0)
                        {
                            foreach (var i in practices)
                            {
                                ListItem selectedPractice = null;
                                selectedPractice = ddlPractice.Items.FindByValue(i.ToString());
                                ddlPractice.Items.Remove(selectedPractice);
                            }
                        }
                    }
                    txtCommisssionPercentage.Text = lblCommisssionPercentage.Text;
                }
            }
        }

        protected void imgPersonEdit_Click(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            GridView gv = row.NamingContainer as GridView;
            int attributionNum;
            int.TryParse(gv.Attributes["Attribution"], out attributionNum);
            AttributionCategory attribution = (AttributionCategory)(attributionNum);
            CancelAllEditModeRows(attribution);
            CopyTempValuesAsReal(attribution);
            Label lblpersonName = gv.Rows[row.DataItemIndex].FindControl("lblPersonName") as Label;
            Label lblStartDate = gv.Rows[row.DataItemIndex].FindControl("lblStartDate") as Label;
            Label lblTitleName = gv.Rows[row.DataItemIndex].FindControl("lblTitleName") as Label;
            XDocument xdoc;
            if (attribution == AttributionCategory.DeliveryPersonAttribution)
            {
                xdoc = OnEditClick(DeliveryPersonAttributionXML, true, lblpersonName.Text, lblStartDate.Text, lblTitleName.Text);
                DeliveryPersonAttributionXML = xdoc.ToString();
            }
            else
            {
                xdoc = OnEditClick(SalesPersonAttributionXML, true, lblpersonName.Text, lblStartDate.Text, lblTitleName.Text);
                SalesPersonAttributionXML = xdoc.ToString();
            }

            DatabindGridView(gv, xdoc.Descendants(XName.Get(AttributionXname)).ToList());
        }

        protected void imgPracticeEdit_Click(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            GridView gv = row.NamingContainer as GridView;
            int attributionNum;
            int.TryParse(gv.Attributes["Attribution"], out attributionNum);
            AttributionCategory attribution = (AttributionCategory)(attributionNum);
            CancelAllEditModeRows(attribution);
            CopyTempValuesAsReal(attribution);
            Label lblPractice = gv.Rows[row.DataItemIndex].FindControl("lblPractice") as Label;
            XDocument xdoc;
            if (attribution == AttributionCategory.SalesPracticeAttribution)
            {
                xdoc = OnEditClick(SalesPracticeAttributionXML, true, lblPractice.Text);
                SalesPracticeAttributionXML = xdoc.ToString();
            }
            else
            {
                xdoc = OnEditClick(DeliveryPracticeAttributionXML, true, lblPractice.Text);
                DeliveryPracticeAttributionXML = xdoc.ToString();
            }
            DatabindGridView(gv, xdoc.Descendants(XName.Get(AttributionXname)).ToList());
        }

        protected void imgPersonUpdate_Click(object sender, EventArgs e)
        {
            ImageButton imgUpdate = sender as ImageButton;
            GridViewRow row = imgUpdate.NamingContainer as GridViewRow;
            GridView gv = row.NamingContainer as GridView;
            int attributionNum;
            int.TryParse(gv.Attributes["Attribution"], out attributionNum);
            AttributionCategory attributionType = (AttributionCategory)(attributionNum);
            Validate(row, attributionType);
            if (Page.IsValid)
            {
                Label lblpersonName = gv.Rows[row.DataItemIndex].FindControl("lblPersonName") as Label;
                HiddenField hdnPersonId = gv.Rows[row.DataItemIndex].FindControl("hdnPersonId") as HiddenField;
                var ddlPerson = row.FindControl("ddlPerson") as DropDownList;
                var dpStartDate = row.FindControl("dpStartDate") as DatePicker;
                var dpEndDate = row.FindControl("dpEndDate") as DatePicker;

                int personId;
                DateTime startDate;
                DateTime endDate;
                if (int.TryParse(ddlPerson.SelectedValue, out personId)) ;
                if (DateTime.TryParse(dpStartDate.TextValue, out startDate)) ;
                if (DateTime.TryParse(dpEndDate.TextValue, out endDate)) ;

                Title title = ServiceCallers.Custom.Person(p => p.GetPersonTitleByRange(personId, startDate, endDate));
                Attribution attribution = new Attribution
                {
                    Title = title,
                    TargetId = personId,
                    TargetName = ddlPerson.SelectedItem.Text,
                    StartDate = startDate,
                    EndDate = endDate,
                    CommissionPercentage = 100
                };
                XDocument xdocLatest;
                if (attributionType == AttributionCategory.DeliveryPersonAttribution)
                {
                    XDocument xdoc = OnUpdateClick(attribution, DeliveryPersonAttributionXML, lblpersonName.Text, hdnPersonId.Value, true);
                    DeliveryPersonAttributionXML = xdoc.ToString();
                    CopyTempValuesAsReal(attributionType);
                    xdocLatest = XDocument.Parse(DeliveryPersonAttributionXML);
                }
                else
                {
                    XDocument xdoc = OnUpdateClick(attribution, SalesPersonAttributionXML, lblpersonName.Text, hdnPersonId.Value, true);
                    SalesPersonAttributionXML = xdoc.ToString();
                    CopyTempValuesAsReal(attributionType);
                    xdocLatest = XDocument.Parse(SalesPersonAttributionXML);
                }
                DatabindGridView(gv, xdocLatest.Descendants(XName.Get(AttributionXname)).ToList());
            }
        }

        protected void imgPracticeUpdate_Click(object sender, EventArgs e)
        {
            ImageButton imgUpdate = sender as ImageButton;
            GridViewRow row = imgUpdate.NamingContainer as GridViewRow;
            GridView gv = row.NamingContainer as GridView;
            int attributionNum;
            int.TryParse(gv.Attributes["Attribution"], out attributionNum);
            AttributionCategory attributionType = (AttributionCategory)(attributionNum);
            Validate(row, attributionType);
            if (Page.IsValid)
            {
                Label lblPractice = gv.Rows[row.DataItemIndex].FindControl("lblPractice") as Label;
                HiddenField hdnPracticeId = gv.Rows[row.DataItemIndex].FindControl("hdnPracticeId") as HiddenField;
                var ddlPractice = row.FindControl("ddlPractice") as DropDownList;
                var txtCommisssionPercentage = row.FindControl("txtCommisssionPercentage") as TextBox;

                int practiceId;
                decimal commissionPercentage;
                if (int.TryParse(ddlPractice.SelectedValue, out practiceId)) ;
                if (decimal.TryParse(txtCommisssionPercentage.Text, out commissionPercentage)) ;
                Attribution attribution = new Attribution
                {
                    Title = null,
                    TargetId = practiceId,
                    TargetName = ddlPractice.SelectedItem.Text,
                    StartDate = null,
                    EndDate = null,
                    CommissionPercentage = commissionPercentage
                };
                XDocument xdocLatest;
                if (attributionType == AttributionCategory.DeliveryPracticeAttribution)
                {
                    XDocument xdoc = OnUpdateClick(attribution, DeliveryPracticeAttributionXML, lblPractice.Text, hdnPracticeId.Value, false);
                    DeliveryPracticeAttributionXML = xdoc.ToString();
                    CopyTempValuesAsReal(attributionType);
                    xdocLatest = XDocument.Parse(DeliveryPracticeAttributionXML);
                }
                else
                {
                    XDocument xdoc = OnUpdateClick(attribution, SalesPracticeAttributionXML, lblPractice.Text, hdnPracticeId.Value, false);
                    SalesPracticeAttributionXML = xdoc.ToString();
                    CopyTempValuesAsReal(attributionType);
                    xdocLatest = XDocument.Parse(SalesPracticeAttributionXML);
                }
                DatabindGridView(gv, xdocLatest.Descendants(XName.Get(AttributionXname)).ToList());
            }
        }

        protected void imgPersonCancel_Click(object sender, EventArgs e)
        {
            ImageButton imgCancel = sender as ImageButton;
            GridViewRow row = imgCancel.NamingContainer as GridViewRow;
            GridView gv = row.NamingContainer as GridView;
            int attributionNum;
            int.TryParse(gv.Attributes["Attribution"], out attributionNum);
            AttributionCategory attributionType = (AttributionCategory)(attributionNum);
            Label lblpersonName = gv.Rows[row.DataItemIndex].FindControl("lblPersonName") as Label;
            Label lblStartDate = gv.Rows[row.DataItemIndex].FindControl("lblStartDate") as Label;
            Label lblTitleName = gv.Rows[row.DataItemIndex].FindControl("lblTitleName") as Label;
            CopyTempValuesAsReal(attributionType);
            XDocument xdoc;
            if (attributionType == AttributionCategory.SalesPersonAttribution)
            {
                if (imgCancel.Attributes[IsNewEntryXname] == "True")
                {
                    xdoc = DeleteRow(SalesPersonAttributionXML, false, lblpersonName.Text, lblStartDate.Text, lblTitleName.Text);
                }
                else
                {
                    xdoc = OnEditClick(SalesPersonAttributionXML, false, lblpersonName.Text, lblStartDate.Text, lblTitleName.Text);
                }
                SalesPersonAttributionXML = xdoc.ToString();
            }
            else
            {
                if (imgCancel.Attributes[IsNewEntryXname] == "True")
                {
                    xdoc = DeleteRow(DeliveryPersonAttributionXML, false, lblpersonName.Text, lblStartDate.Text, lblTitleName.Text);
                }
                else
                {
                    xdoc = OnEditClick(DeliveryPersonAttributionXML, false, lblpersonName.Text, lblStartDate.Text, lblTitleName.Text);
                }
                DeliveryPersonAttributionXML = xdoc.ToString();
            }
            DatabindGridView(gv, xdoc.Descendants(XName.Get(AttributionXname)).ToList());
        }

        protected void imgPracticeCancel_Click(object sender, EventArgs e)
        {
            ImageButton imgCancel = sender as ImageButton;
            GridViewRow row = imgCancel.NamingContainer as GridViewRow;
            GridView gv = row.NamingContainer as GridView;
            int attributionNum;
            int.TryParse(gv.Attributes["Attribution"], out attributionNum);
            AttributionCategory attributionType = (AttributionCategory)(attributionNum);
            Label lblPractice = gv.Rows[row.DataItemIndex].FindControl("lblPractice") as Label;
            XDocument xdoc;
            if (attributionType == AttributionCategory.DeliveryPracticeAttribution)
            {
                xdoc = OnEditClick(DeliveryPracticeAttributionXML, false, lblPractice.Text);
                CopyTempValuesAsReal(attributionType);
                if (imgCancel.Attributes[IsNewEntryXname] == "True")
                {
                    xdoc = DeleteRow(DeliveryPracticeAttributionXML, false, lblPractice.Text);
                }
                DeliveryPracticeAttributionXML = xdoc.ToString();
            }
            else
            {
                xdoc = OnEditClick(SalesPracticeAttributionXML, false, lblPractice.Text);
                if (imgCancel.Attributes[IsNewEntryXname] == "True")
                {
                    xdoc = DeleteRow(SalesPracticeAttributionXML, false, lblPractice.Text);
                }
                SalesPracticeAttributionXML = xdoc.ToString();
            }
            DatabindGridView(gv, xdoc.Descendants(XName.Get(AttributionXname)).ToList());
        }

        protected void btnAddRecord_Click(object sender, EventArgs e)
        {
            Button btnAdd = sender as Button;
            int attributionNum;
            int.TryParse(btnAdd.Attributes["Attribution"], out attributionNum);
            AttributionCategory attribution = (AttributionCategory)(attributionNum);
            CancelAllEditModeRows(attribution);
            CopyTempValuesAsReal(attribution);
            if (btnAdd.Attributes["Attribution"] == "1")
            {
                XDocument xdoc = XDocument.Parse(DeliveryPersonAttributionXML);
                XDocument latestXdoc = AddEmptyRow(2, 1, xdoc);
                DeliveryPersonAttributionXML = latestXdoc.ToString();
                DatabindGridView(gvDeliveryAttributionPerson, latestXdoc.Descendants(XName.Get(AttributionXname)).ToList());
                DeliveryPersonAttributionXML = xdoc.ToString();
            }
            else if (btnAdd.Attributes["Attribution"] == "2")
            {
                XDocument xdoc = XDocument.Parse(SalesPersonAttributionXML);
                XDocument latestXdoc = AddEmptyRow(1, 1, xdoc);
                SalesPersonAttributionXML = latestXdoc.ToString();
                DatabindGridView(gvSalesAttributionPerson, latestXdoc.Descendants(XName.Get(AttributionXname)).ToList());
                SalesPersonAttributionXML = xdoc.ToString();
            }
            else if (btnAdd.Attributes["Attribution"] == "3")
            {
                XDocument xdoc = XDocument.Parse(DeliveryPracticeAttributionXML);
                XDocument latestXdoc = AddEmptyRow(2, 2, xdoc);
                DeliveryPracticeAttributionXML = latestXdoc.ToString();
                DatabindGridView(gvDeliveryAttributionPractice, latestXdoc.Descendants(XName.Get(AttributionXname)).ToList());
                DeliveryPracticeAttributionXML = xdoc.ToString();
            }
            else
            {
                XDocument xdoc = XDocument.Parse(SalesPracticeAttributionXML);
                XDocument latestXdoc = AddEmptyRow(1, 2, xdoc);
                SalesPracticeAttributionXML = latestXdoc.ToString();
                DatabindGridView(gvSalesAttributionPractice, latestXdoc.Descendants(XName.Get(AttributionXname)).ToList());
                SalesPracticeAttributionXML = xdoc.ToString();
            }
        }

        protected void imgPersonDelete_Click(object sender, EventArgs e)
        {
            ImageButton imgDelete = sender as ImageButton;
            GridViewRow row = imgDelete.NamingContainer as GridViewRow;
            GridView gv = row.NamingContainer as GridView;
            int attributionNum;
            int.TryParse(gv.Attributes["Attribution"], out attributionNum);
            AttributionCategory attributionType = (AttributionCategory)(attributionNum);
            Label lblpersonName = gv.Rows[row.DataItemIndex].FindControl("lblPersonName") as Label;
            Label lblStartDate = gv.Rows[row.DataItemIndex].FindControl("lblStartDate") as Label;
            Label lblTitleName = gv.Rows[row.DataItemIndex].FindControl("lblTitleName") as Label;
            StoreTempDataWhileDeleting(attributionType);
            XDocument latestXml;
            if (attributionType == AttributionCategory.DeliveryPersonAttribution)
            {
                latestXml = DeleteRow(DeliveryPersonAttributionXML, true, lblpersonName.Text, lblStartDate.Text, lblTitleName.Text);
                DeliveryPersonAttributionXML = latestXml.ToString();
            }
            else
            {
                latestXml = DeleteRow(SalesPersonAttributionXML, true, lblpersonName.Text, lblStartDate.Text, lblTitleName.Text);
                SalesPersonAttributionXML = latestXml.ToString();
            }
            DatabindGridView(gv, latestXml.Descendants(XName.Get(AttributionXname)).ToList());
        }

        protected void imgPracticeDelete_Click(object sender, EventArgs e)
        {
            ImageButton imgDelete = sender as ImageButton;
            GridViewRow row = imgDelete.NamingContainer as GridViewRow;
            GridView gv = row.NamingContainer as GridView;
            int attributionNum;
            int.TryParse(gv.Attributes["Attribution"], out attributionNum);
            AttributionCategory attributionType = (AttributionCategory)(attributionNum);
            Label lblPractice = gv.Rows[row.DataItemIndex].FindControl("lblPractice") as Label;
            StoreTempDataWhileDeleting(attributionType);
            XDocument latestXml;
            if (attributionType == AttributionCategory.SalesPracticeAttribution)
            {
                latestXml = DeleteRow(SalesPracticeAttributionXML, true, lblPractice.Text);
                SalesPracticeAttributionXML = latestXml.ToString();
            }
            else
            {
                latestXml = DeleteRow(DeliveryPracticeAttributionXML, true, lblPractice.Text);
                DeliveryPracticeAttributionXML = latestXml.ToString();
            }
            DatabindGridView(gv, latestXml.Descendants(XName.Get(AttributionXname)).ToList());
        }

        protected void imgAdditionalAllocationOfResource_Click(object sender, EventArgs e)
        {
            ImageButton imgAdd = sender as ImageButton;
            GridViewRow row = imgAdd.NamingContainer as GridViewRow;
            GridView gv = row.NamingContainer as GridView;
            int attributionNum;
            int.TryParse(gv.Attributes["Attribution"], out attributionNum);
            AttributionCategory attributionType = (AttributionCategory)(attributionNum);
            CancelAllEditModeRows(attributionType);
            CopyTempValuesAsReal(attributionType);
            Label lblpersonName = gv.Rows[row.DataItemIndex].FindControl("lblPersonName") as Label;
            Label lblStartDate = gv.Rows[row.DataItemIndex].FindControl("lblStartDate") as Label;

            XDocument xdoc;
            xdoc = attributionType == AttributionCategory.DeliveryPersonAttribution ? XDocument.Parse(DeliveryPersonAttributionXML) : XDocument.Parse(SalesPersonAttributionXML);
            List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();

            Attribution attribution = new Attribution();
            int personId;
            int titleId;
            for (int i = 0; i < xlist.Count; i++)
            {
                if (xlist[i].Attribute(XName.Get(TargetNameXname)).Value == lblpersonName.Text && Convert.ToDateTime(xlist[i].Attribute(XName.Get(StartDateXname)).Value).Date.ToShortDateString() == lblStartDate.Text)
                {
                    if (int.TryParse(xlist[i].Attribute(XName.Get(TargetIdXname)).Value, out personId)) ;
                    if (int.TryParse(xlist[i].Attribute(XName.Get(TitleIdXname)).Value, out titleId)) ;
                    attribution.TargetId = personId;
                    attribution.TargetName = xlist[i].Attribute(XName.Get(TargetNameXname)).Value;
                    attribution.StartDate = Convert.ToDateTime(HostingPage.Project.StartDate);
                    attribution.EndDate = Convert.ToDateTime(HostingPage.Project.EndDate);
                    attribution.Title = new Title()
                    {
                        TitleId = titleId,
                        TitleName = xlist[i].Attribute(XName.Get(TitleXname)).Value
                    };
                    attribution.IsEditMode = true;
                    attribution.IsNewEntry = true;
                    break;
                }
            }

            XDocument xdocLatest = AddEmptyRow(2, 1, xdoc, attribution);
            if (attributionType == AttributionCategory.DeliveryPersonAttribution)
                DeliveryPersonAttributionXML = xdocLatest.ToString();
            else
                SalesPersonAttributionXML = xdocLatest.ToString();
            DatabindGridView(gv, xdocLatest.Descendants(XName.Get(AttributionXname)).ToList());
        }

        protected void btnCopyAlltoRight_Click(object sender, EventArgs e)
        {
            XDocument xdocLeft = XDocument.Parse(DeliveryPersonAttributionXML);
            List<XElement> xlistLeft = xdocLeft.Descendants(XName.Get(AttributionXname)).ToList();

            XDocument xdocRight = XDocument.Parse(SalesPersonAttributionXML);
            List<XElement> xlistRight = xdocRight.Descendants(XName.Get(AttributionXname)).ToList();

            CopyToSalesPersonAttribution(xlistLeft, xlistRight);
        }

        protected void btnCopySelectedItemstoRight_Click(object sender, EventArgs e)
        {
            XDocument xdocRight = XDocument.Parse(SalesPersonAttributionXML);
            List<XElement> xlistRight = xdocRight.Descendants(XName.Get(AttributionXname)).ToList();

            XDocument xdocLeft = XDocument.Parse(DeliveryPersonAttributionXML);
            List<XElement> xlistLeft = xdocLeft.Descendants(XName.Get(AttributionXname)).ToList();
            xlistLeft = xlistLeft.FindAll(x => x.Attribute(XName.Get(IsCheckboxCheckedXname)).Value == true.ToString());

            CopyToSalesPersonAttribution(xlistLeft, xlistRight);
        }

        protected void btnCopyAlltoLeft_Click(object sender, EventArgs e)
        {
            XDocument xdocLeft = XDocument.Parse(DeliveryPersonAttributionXML);
            List<XElement> xlistLeft = xdocLeft.Descendants(XName.Get(AttributionXname)).ToList();

            XDocument xdocRight = XDocument.Parse(SalesPersonAttributionXML);
            List<XElement> xlistRight = xdocRight.Descendants(XName.Get(AttributionXname)).ToList();

            CopyToDeliveryPersonAttribution(xlistLeft, xlistRight);
        }

        protected void btnCopySelectedItemstoLeft_Click(object sender, EventArgs e)
        {
            XDocument xdocLeft = XDocument.Parse(DeliveryPersonAttributionXML);
            List<XElement> xlistLeft = xdocLeft.Descendants(XName.Get(AttributionXname)).ToList();

            XDocument xdocRight = XDocument.Parse(SalesPersonAttributionXML);
            List<XElement> xlistRight = xdocRight.Descendants(XName.Get(AttributionXname)).ToList();
            xlistRight = xlistRight.FindAll(x => x.Attribute(XName.Get(IsCheckboxCheckedXname)).Value == true.ToString());

            CopyToDeliveryPersonAttribution(xlistLeft, xlistRight);
        }

        protected void custPersonStart_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator custPersonStart = source as CustomValidator;
            GridViewRow row = custPersonStart.NamingContainer as GridViewRow;
            DatePicker dpStartDate = row.FindControl("dpStartDate") as DatePicker;
            DateTime startDate;
            if (DateTime.TryParse(dpStartDate.TextValue, out startDate)) ;
            args.IsValid = (startDate >= HostingPage.Project.StartDate.Value);
        }

        protected void custPersonEnd_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator custPersonEnd = source as CustomValidator;
            GridViewRow row = custPersonEnd.NamingContainer as GridViewRow;
            DatePicker dpEndDate = row.FindControl("dpEndDate") as DatePicker;
            DateTime endDate;
            if (DateTime.TryParse(dpEndDate.TextValue, out endDate)) ;
            args.IsValid = (endDate <= HostingPage.Project.EndDate.Value);
        }

        protected void custPersonDatesOverlapping_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            CustomValidator custPersonDatesOverlapping = source as CustomValidator;
            GridViewRow row = custPersonDatesOverlapping.NamingContainer as GridViewRow;
            GridView gridView = row.NamingContainer as GridView;
            HiddenField attributionType = gridView.HeaderRow.FindControl("hdnAttributionType") as HiddenField;
            HiddenField hdnEditMode = row.FindControl("hdnEditMode") as HiddenField;
            DatePicker dpEndDate = row.FindControl("dpEndDate") as DatePicker;
            DatePicker dpStartDate = row.FindControl("dpStartDate") as DatePicker;
            DropDownList ddlPerson = row.FindControl("ddlPerson") as DropDownList;
            DateTime startDate;
            DateTime endDate;
            int personId;
            if (hdnEditMode.Value == true.ToString() && DateTime.TryParse(dpStartDate.TextValue, out startDate) &&
                    DateTime.TryParse(dpEndDate.TextValue, out endDate) &&
                    int.TryParse(ddlPerson.SelectedValue, out personId))
            {
                Title title = ServiceCallers.Custom.Person(p => p.GetPersonTitleByRange(personId, startDate, endDate));
                XDocument xdoc = attributionType.Value == "Delivery" ? XDocument.Parse(DeliveryPersonAttributionXML) : XDocument.Parse(SalesPersonAttributionXML);
                List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
                foreach (var item in xlist)
                {
                    if (item.Attribute(XName.Get(IsEditModeXname)).Value == false.ToString())
                    {
                        if (ddlPerson.SelectedValue == item.Attribute(XName.Get(TargetIdXname)).Value &&
                            item.Attribute(XName.Get(TitleIdXname)).Value ==
                            (title == null ? string.Empty : title.TitleId.ToString()))
                        {
                            DateTime itemStartDate = Convert.ToDateTime(item.Attribute(XName.Get(StartDateXname)).Value);
                            DateTime itemEndDate = Convert.ToDateTime(item.Attribute(XName.Get(EndDateXname)).Value);
                            args.IsValid = !(startDate <= itemEndDate && itemStartDate <= endDate);
                            if (!args.IsValid)
                                break;
                        }
                    }
                }
            }
        }

        protected void custCommissionsPercentage_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator custCommissionsPercentage = source as CustomValidator;
            GridViewRow row = custCommissionsPercentage.NamingContainer as GridViewRow;
            GridView gridView = row.NamingContainer as GridView;
            HiddenField attributionType = gridView.HeaderRow.FindControl("hdnAttributionType") as HiddenField;
            HiddenField hdnEditMode = row.FindControl("hdnEditMode") as HiddenField;
            decimal totalPercentage = 0;
            args.IsValid = true;
            List<XElement> xlist;

            if (attributionType.Value == "Delivery")
            {
                XDocument xdoc = XDocument.Parse(DeliveryPracticeAttributionXML);
                xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
            }
            else
            {
                XDocument xdoc = XDocument.Parse(SalesPracticeAttributionXML);
                xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
            }
            foreach (var item in xlist)
            {
                decimal itemPercentage;
                if (decimal.TryParse(item.Attribute(XName.Get(PercentageXname)).Value, out itemPercentage)) ;
                totalPercentage += itemPercentage;
            }
            args.IsValid = totalPercentage == 100;
        }

        #endregion Events

        #region Methods

        public void CopyToSalesPersonAttribution(List<XElement> xlistLeft, List<XElement> xlistRight)
        {
            foreach (var item in xlistLeft)
            {
                int targetId;
                DateTime startDate;
                DateTime endDate;
                if (int.TryParse(item.Attribute(XName.Get(TargetIdXname)).Value, out targetId)) ;
                if (DateTime.TryParse(item.Attribute(XName.Get(StartDateXname)).Value, out startDate)) ;
                if (DateTime.TryParse(item.Attribute(XName.Get(EndDateXname)).Value, out endDate)) ;
                if (xlistRight.Any(x => x.Attribute(XName.Get(TargetIdXname)).Value == targetId.ToString()))
                {
                    List<XElement> xlist = xlistRight.FindAll(x => x.Attribute(XName.Get(TargetIdXname)).Value == targetId.ToString());
                    if (xlist.Any(x1 => (Convert.ToDateTime(x1.Attribute(XName.Get(StartDateXname)).Value) <= endDate) && (startDate <= Convert.ToDateTime(x1.Attribute(XName.Get(EndDateXname)).Value))))
                    {
                    }
                    else if (item.Attribute(XName.Get(IsEditModeXname)).Value != true.ToString())
                    {
                        Attribution attribution = new Attribution()
                        {
                            TargetId = targetId,
                            TargetName = item.Attribute(XName.Get(TargetNameXname)).Value,
                            StartDate = startDate,
                            EndDate = endDate,
                            Title = new Title()
                            {
                                TitleId = Convert.ToInt32(item.Attribute(XName.Get(TitleIdXname)).Value),
                                TitleName = item.Attribute(XName.Get(TitleXname)).Value
                            },
                            IsEditMode = false,
                            IsNewEntry = false
                        };

                        XDocument xdoc = AddEmptyRow(1, 1, XDocument.Parse(SalesPersonAttributionXML), attribution);
                        SalesPersonAttributionXML = xdoc.ToString();
                    }
                }
                else if (item.Attribute(XName.Get(IsEditModeXname)).Value != true.ToString())
                {
                    Attribution attribution = new Attribution()
                    {
                        TargetId = targetId,
                        TargetName = item.Attribute(XName.Get(TargetNameXname)).Value,
                        StartDate = startDate,
                        EndDate = endDate,
                        IsEditMode = false,
                        IsNewEntry = false
                    };
                    if (item.Attribute(XName.Get(TitleIdXname)).Value != string.Empty)
                    {
                        attribution.Title = new Title()
                            {
                                TitleId = Convert.ToInt32(item.Attribute(XName.Get(TitleIdXname)).Value),
                                TitleName = item.Attribute(XName.Get(TitleXname)).Value
                            };
                    }
                    else
                        attribution.Title = null;
                    XDocument xdoc = AddEmptyRow(1, 1, XDocument.Parse(SalesPersonAttributionXML), attribution);
                    SalesPersonAttributionXML = xdoc.ToString();
                }
            }
            XDocument xdocLatest = XDocument.Parse(SalesPersonAttributionXML);
            DatabindGridView(gvSalesAttributionPerson, xdocLatest.Descendants(XName.Get(AttributionXname)).ToList());
        }

        public void CopyToDeliveryPersonAttribution(List<XElement> xlistLeft, List<XElement> xlistRight)
        {
            foreach (var item in xlistRight)
            {
                int targetId;
                DateTime startDate;
                DateTime endDate;
                if (int.TryParse(item.Attribute(XName.Get(TargetIdXname)).Value, out targetId)) ;
                if (DateTime.TryParse(item.Attribute(XName.Get(StartDateXname)).Value, out startDate)) ;
                if (DateTime.TryParse(item.Attribute(XName.Get(EndDateXname)).Value, out endDate)) ;
                if (xlistLeft.Any(x => x.Attribute(XName.Get(TargetIdXname)).Value == targetId.ToString()))
                {
                    List<XElement> xlist = xlistLeft.FindAll(x => x.Attribute(XName.Get(TargetIdXname)).Value == targetId.ToString());
                    if (xlist.Any(x1 => (Convert.ToDateTime(x1.Attribute(XName.Get(StartDateXname)).Value) <= endDate) && (startDate <= Convert.ToDateTime(x1.Attribute(XName.Get(EndDateXname)).Value))))
                    {
                    }
                    else if (item.Attribute(XName.Get(IsEditModeXname)).Value != true.ToString())
                    {
                        Attribution attribution = new Attribution()
                        {
                            TargetId = targetId,
                            TargetName = item.Attribute(XName.Get(TargetNameXname)).Value,
                            StartDate = startDate,
                            EndDate = endDate,
                            Title = new Title()
                            {
                                TitleId = Convert.ToInt32(item.Attribute(XName.Get(TitleIdXname)).Value),
                                TitleName = item.Attribute(XName.Get(TitleXname)).Value
                            },
                            IsEditMode = false,
                            IsNewEntry = false
                        };

                        XDocument xdoc = AddEmptyRow(1, 1, XDocument.Parse(DeliveryPersonAttributionXML), attribution);
                        DeliveryPersonAttributionXML = xdoc.ToString();
                    }
                }
                else if (item.Attribute(XName.Get(IsEditModeXname)).Value != true.ToString())
                {
                    Attribution attribution = new Attribution()
                    {
                        TargetId = targetId,
                        TargetName = item.Attribute(XName.Get(TargetNameXname)).Value,
                        StartDate = Convert.ToDateTime(item.Attribute(XName.Get(StartDateXname)).Value),
                        EndDate = Convert.ToDateTime(item.Attribute(XName.Get(EndDateXname)).Value),
                        IsEditMode = false,
                        IsNewEntry = false
                    };
                    if (item.Attribute(XName.Get(TitleIdXname)).Value != string.Empty)
                    {
                        attribution.Title = new Title()
                        {
                            TitleId = Convert.ToInt32(item.Attribute(XName.Get(TitleIdXname)).Value),
                            TitleName = item.Attribute(XName.Get(TitleXname)).Value
                        };
                    }
                    else
                        attribution.Title = null;
                    XDocument xdoc = AddEmptyRow(1, 1, XDocument.Parse(DeliveryPersonAttributionXML), attribution);
                    DeliveryPersonAttributionXML = xdoc.ToString();
                }
            }
            XDocument xdocLatest = XDocument.Parse(DeliveryPersonAttributionXML);
            DatabindGridView(gvDeliveryAttributionPerson, xdocLatest.Descendants(XName.Get(AttributionXname)).ToList());
        }

        public XDocument OnEditClick(string attributionXML, bool editMode, string targetName, string startDate = null, string title = null)
        {
            XDocument xdoc = XDocument.Parse(attributionXML);
            List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();

            for (int i = 0; i < xlist.Count; i++)
            {
                if (xlist[i].Attribute(XName.Get(TargetNameXname)).Value == targetName && (startDate == null || Convert.ToDateTime(xlist[i].Attribute(XName.Get(StartDateXname)).Value).Date.ToShortDateString() == startDate) && (title == null || xlist[i].Attribute(XName.Get(TitleXname)).Value == title))
                {
                    xlist[i].Attribute(XName.Get(IsEditModeXname)).Value = editMode ? "True" : "False";
                }
            }

            return xdoc;
        }

        public XDocument OnUpdateClick(Attribution attribution, string attributionXML, string targetName, string targetId, bool isPersonType)
        {
            XDocument xdoc = XDocument.Parse(attributionXML);
            List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
            for (int i = 0; i < xlist.Count; i++)
            {
                if (xlist[i].Attribute(XName.Get(IsEditModeXname)).Value == true.ToString())
                {
                    xlist[i].Attribute(XName.Get(IsEditModeXname)).Value = "False";
                    xlist[i].Attribute(XName.Get(TargetIdXname)).Value = attribution.TargetId.ToString();
                    xlist[i].Attribute(XName.Get(TargetNameXname)).Value = attribution.TargetName.ToString();
                    xlist[i].Attribute(XName.Get(StartDateXname)).Value = attribution.StartDate.HasValue ? attribution.StartDate.Value.ToShortDateString() : string.Empty;
                    xlist[i].Attribute(XName.Get(EndDateXname)).Value = attribution.EndDate.HasValue ? attribution.EndDate.Value.ToShortDateString() : string.Empty;
                    xlist[i].Attribute(XName.Get(PercentageXname)).Value = attribution.CommissionPercentage.ToString();
                    xlist[i].Attribute(XName.Get(TitleIdXname)).Value = attribution.Title != null ? attribution.Title.TitleId.ToString() : string.Empty;
                    xlist[i].Attribute(XName.Get(TitleXname)).Value = attribution.Title != null ? attribution.Title.HtmlEncodedTitleName : string.Empty;
                    xlist[i].Attribute(XName.Get(IsNewEntryXname)).Value = "False";
                }
                if (isPersonType)
                {
                    if ((xlist[i].Attribute(XName.Get(TargetIdXname)).Value == targetId && xlist[i].Attribute(XName.Get(TargetNameXname)).Value == targetName))
                    {
                        xlist[i].Attribute(XName.Get(IsEditModeXname)).Value = "False";
                        xlist[i].Attribute(XName.Get(TargetIdXname)).Value = attribution.TargetId.ToString();
                        xlist[i].Attribute(XName.Get(TargetNameXname)).Value = attribution.TargetName.ToString();
                        xlist[i].Attribute(XName.Get(IsNewEntryXname)).Value = "False";
                        Title title = ServiceCallers.Custom.Person(p => p.GetPersonTitleByRange(attribution.TargetId, Convert.ToDateTime(xlist[i].Attribute(XName.Get(StartDateXname)).Value).Date, Convert.ToDateTime(xlist[i].Attribute(XName.Get(EndDateXname)).Value).Date));
                        xlist[i].Attribute(XName.Get(TitleIdXname)).Value = title != null ? title.TitleId.ToString() : string.Empty;
                        xlist[i].Attribute(XName.Get(TitleXname)).Value = title != null ? title.HtmlEncodedTitleName : string.Empty;
                    }
                }
            }
            attributionXML = xdoc.ToString();

            return xdoc;
        }

        public void GetProjectAttributionValues()
        {
            ProjectId = HostingPage.ProjectId.Value;
            ProjectAttribution = ServiceCallers.Custom.Project(p => p.GetProjectAttributionValues(ProjectId)).ToList();
        }

        private XDocument PrePareXmlForAttributionsFromData(string attributionCategory)
        {
            StringBuilder xml = new StringBuilder();

            List<Attribution> attributionList = new List<Attribution>();

            if (attributionCategory == deliveryPersonAttribution)
            {
                attributionList = DeliveryPersonAttribution;

                xml.Append(string.Format(AttributionTypeXmlOpen, (int)AttributionTypes.Delivery));
                xml.Append(string.Format(AttributionRecordTypeXmlOpen, (int)AttributionRecordTypes.Person));
            }

            if (attributionCategory == deliveryPracticeAttribution)
            {
                attributionList = DeliveryPracticeAttribution;

                xml.Append(string.Format(AttributionTypeXmlOpen, (int)AttributionTypes.Delivery));
                xml.Append(string.Format(AttributionRecordTypeXmlOpen, (int)AttributionRecordTypes.Practice));
            }

            if (attributionCategory == salesPersonAttribution)
            {
                attributionList = SalesPersonAttribution;

                xml.Append(string.Format(AttributionTypeXmlOpen, (int)AttributionTypes.Sales));
                xml.Append(string.Format(AttributionRecordTypeXmlOpen, (int)AttributionRecordTypes.Person));
            }

            if (attributionCategory == salesPracticeAttribution)
            {
                attributionList = SalesPracticeAttribution;

                xml.Append(string.Format(AttributionTypeXmlOpen, (int)AttributionTypes.Sales));
                xml.Append(string.Format(AttributionRecordTypeXmlOpen, (int)AttributionRecordTypes.Practice));
            }

            foreach (var attribution in attributionList)
            {
                xml.Append(string.Format(AttributionXmlOpen, attribution.Id, attribution.TargetId, attribution.TargetName, attribution.StartDate == null ? string.Empty : attribution.StartDate.Value.ToShortDateString(), attribution.EndDate == null ? string.Empty : attribution.EndDate.Value.ToShortDateString(), attribution.CommissionPercentage, attribution.Title == null ? string.Empty : attribution.Title.TitleId.ToString(), attribution.Title == null ? string.Empty : attribution.Title.HtmlEncodedTitleName, attribution.IsEditMode, attribution.IsNewEntry, attribution.IsCheckBoxChecked, attribution.TargetId, attribution.TargetName, attribution.StartDate == null ? string.Empty : attribution.StartDate.Value.ToShortDateString(), attribution.EndDate == null ? string.Empty : attribution.EndDate.Value.ToShortDateString(), attribution.CommissionPercentage));
                xml.Append(AttributionXmlClose);
            }

            xml.Append(AttributionRecordTypeXmlClose);
            xml.Append(AttributionTypeXmlClose);

            var xmlStr = xml.ToString();

            return XDocument.Parse(xmlStr);
        }

        private void DatabindGridView(GridView gridView, List<XElement> xlist)
        {
            List<XElement> xlistLatest;
            xlistLatest = xlist.OrderByDescending(x => (x.Attribute(XName.Get(TargetIdXname)).Value != "0" || x.Attribute(XName.Get(IsNewEntryXname)).Value == false.ToString()))
                .ThenBy(x => x.Attribute(XName.Get(TargetNameXname)).Value)
                .ThenBy(x => x.Attribute(XName.Get(IsNewEntryXname)).Value)
                .ThenBy(x => x.Attribute(XName.Get(StartDateXname)).Value).ToList();
            gridView.DataSource = xlistLatest;
            gridView.DataBind();
        }

        private void BindAttributions()
        {
            //for Delivery Person Attributions
            var deliveryPersonAttributions = ProjectAttribution.FindAll(a => a.AttributionType == (AttributionTypes)2 && a.AttributionRecordType == (AttributionRecordTypes)1).ToList();
            DeliveryPersonAttribution = deliveryPersonAttributions;//.OrderBy(x => x.TargetName).ThenBy(x => x.StartDate).ToList();
            var deliveryPersonAttributionsXml = PrePareXmlForAttributionsFromData(deliveryPersonAttribution);
            DeliveryPersonAttributionXML = deliveryPersonAttributionsXml.ToString();
            DatabindGridView(gvDeliveryAttributionPerson, deliveryPersonAttributionsXml.Descendants(XName.Get(AttributionXname)).ToList());

            //for Delivery Practice Attributions
            var deliveryPracticeAttributions = ProjectAttribution.FindAll(a => a.AttributionType == (AttributionTypes)2 && a.AttributionRecordType == (AttributionRecordTypes)2).ToList();
            DeliveryPracticeAttribution = deliveryPracticeAttributions;//.OrderBy(x => x.TargetName).ThenBy(x => x.StartDate).ToList();
            var deliveryPracticeAttributionsXml = PrePareXmlForAttributionsFromData(deliveryPracticeAttribution);
            DeliveryPracticeAttributionXML = deliveryPracticeAttributionsXml.ToString();
            DatabindGridView(gvDeliveryAttributionPractice, deliveryPracticeAttributionsXml.Descendants(XName.Get(AttributionXname)).ToList());

            //for Sales Person Attributions
            var salesPersonAttributions = ProjectAttribution.FindAll(a => a.AttributionType == (AttributionTypes)1 && a.AttributionRecordType == (AttributionRecordTypes)1).ToList();
            SalesPersonAttribution = salesPersonAttributions;//.OrderBy(x => x.TargetName).ThenBy(x => x.StartDate).ToList();
            var salesPersonAttributionsXml = PrePareXmlForAttributionsFromData(salesPersonAttribution);
            SalesPersonAttributionXML = salesPersonAttributionsXml.ToString();
            DatabindGridView(gvSalesAttributionPerson, salesPersonAttributionsXml.Descendants(XName.Get(AttributionXname)).ToList());

            //for Sales Practice Attributions
            var salesPracticeAttributions = ProjectAttribution.FindAll(a => a.AttributionType == (AttributionTypes)1 && a.AttributionRecordType == (AttributionRecordTypes)2).ToList();
            SalesPracticeAttribution = salesPracticeAttributions;//.OrderBy(x => x.TargetName).ThenBy(x => x.StartDate).ToList();
            var salesPracticeAttributionsXml = PrePareXmlForAttributionsFromData(salesPracticeAttribution);
            SalesPracticeAttributionXML = salesPracticeAttributionsXml.ToString();
            DatabindGridView(gvSalesAttributionPractice, salesPracticeAttributionsXml.Descendants(XName.Get(AttributionXname)).ToList());
        }

        public XDocument AddEmptyRow(int attributionType, int attributionRecordType, XDocument xdoc, Attribution attr = null)
        {
            var attribution = new Attribution()
            {
                Id = null,
                AttributionRecordType = (AttributionRecordTypes)attributionRecordType,
                AttributionType = (AttributionTypes)attributionType,
                CommissionPercentage = 100,
                IsEditMode = true,
                StartDate = HostingPage.Project.StartDate.Value.Date,
                EndDate = HostingPage.Project.EndDate.Value.Date,
                IsNewEntry = true
            };

            if (attr != null)
            {
                attribution.IsEditMode = attr.IsEditMode;
                attribution.IsNewEntry = attr.IsNewEntry;
                attribution.TargetId = attr.TargetId;
                attribution.TargetName = attr.TargetName;
                attribution.StartDate = attr.StartDate.Value.Date;
                attribution.EndDate = attr.EndDate.Value.Date;
                attribution.Title = attr.Title;
                attribution.CommissionPercentage = 100;
            }

            StringBuilder xml = new StringBuilder();

            PrePareXmlForAttributionSelection(xml, attribution);

            xdoc.Descendants(XName.Get(AttributionRecordTypeXname)).Last().Add(XElement.Parse(xml.ToString()));

            return xdoc;
        }

        public void PrePareXmlForAttributionSelection(StringBuilder xml, Attribution attribution)
        {
            xml.Append(string.Format(AttributionXmlOpen, attribution.Id, attribution.TargetId, attribution.TargetName, attribution.StartDate == null ? string.Empty : attribution.StartDate.Value.ToShortDateString(), attribution.EndDate == null ? string.Empty : attribution.EndDate.Value.ToShortDateString(), attribution.CommissionPercentage, attribution.Title == null ? string.Empty : attribution.Title.TitleId.ToString(), attribution.Title == null ? string.Empty : attribution.Title.HtmlEncodedTitleName, attribution.IsEditMode, attribution.IsNewEntry, attribution.IsCheckBoxChecked, attribution.TargetId, attribution.TargetName, attribution.StartDate == null ? string.Empty : attribution.StartDate.Value.ToShortDateString(), attribution.EndDate == null ? string.Empty : attribution.EndDate.Value.ToShortDateString(), attribution.CommissionPercentage));
            xml.Append(AttributionXmlClose);
        }

        public XDocument DeleteRow(string attributionXML, bool isDeleteButton, string targetName, string startDate = null, string title = null)
        {
            XDocument xdoc = XDocument.Parse(attributionXML);
            List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();

            for (int i = 0; i < xlist.Count; i++)
            {
                if (isDeleteButton)
                {
                    if (xlist[i].Attribute(XName.Get(TargetNameXname)).Value == targetName && (startDate == null || Convert.ToDateTime(xlist[i].Attribute(XName.Get(StartDateXname)).Value).Date.ToShortDateString() == startDate) && (title == null || xlist[i].Attribute(XName.Get(TitleXname)).Value == title))
                    {
                        xlist[i].Remove();
                        xlist.Remove(xlist[i]);
                    }
                }
                else
                {
                    if (xlist[i].Attribute(XName.Get(IsEditModeXname)).Value == true.ToString())
                    {
                        xlist[i].Remove();
                        xlist.Remove(xlist[i]);
                    }
                }
            }

            return xdoc;
        }

        public void CancelAllEditModeRows(AttributionCategory attribution)
        {
            if (attribution == AttributionCategory.DeliveryPersonAttribution)
            {
                XDocument xdoc = XDocument.Parse(DeliveryPersonAttributionXML);
                List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();

                for (int i = 0; i < xlist.Count; i++)
                {
                    if (xlist[i].Attribute(XName.Get(IsEditModeXname)).Value == true.ToString())
                    {
                        if (xlist[i].Attribute(XName.Get(IsNewEntryXname)).Value == false.ToString())
                        {
                            xlist[i].Attribute(XName.Get(IsEditModeXname)).Value = false.ToString();
                        }
                        else
                        {
                            xdoc = DeleteRow(DeliveryPersonAttributionXML, false, xlist[i].Attribute(XName.Get(TargetNameXname)).Value, xlist[i].Attribute(XName.Get(StartDateXname)).Value, xlist[i].Attribute(XName.Get(TitleXname)).Value);
                        }
                    }
                }
                DeliveryPersonAttributionXML = xdoc.ToString();
            }
            else if (attribution == AttributionCategory.DeliveryPracticeAttribution)
            {
                XDocument xdoc = XDocument.Parse(DeliveryPracticeAttributionXML);
                List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();

                for (int i = 0; i < xlist.Count; i++)
                {
                    if (xlist[i].Attribute(XName.Get(IsEditModeXname)).Value == true.ToString())
                    {
                        if (xlist[i].Attribute(XName.Get(IsNewEntryXname)).Value == false.ToString())
                        {
                            xlist[i].Attribute(XName.Get(IsEditModeXname)).Value = false.ToString();
                        }
                        else
                        {
                            xdoc = DeleteRow(DeliveryPracticeAttributionXML, false, xlist[i].Attribute(XName.Get(TargetNameXname)).Value, xlist[i].Attribute(XName.Get(StartDateXname)).Value);
                        }
                    }
                }
                DeliveryPracticeAttributionXML = xdoc.ToString();
            }
            else if (attribution == AttributionCategory.SalesPersonAttribution)
            {
                XDocument xdoc = XDocument.Parse(SalesPersonAttributionXML);
                List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();

                for (int i = 0; i < xlist.Count; i++)
                {
                    if (xlist[i].Attribute(XName.Get(IsEditModeXname)).Value == true.ToString())
                    {
                        if (xlist[i].Attribute(XName.Get(IsNewEntryXname)).Value == false.ToString())
                        {
                            xlist[i].Attribute(XName.Get(IsEditModeXname)).Value = false.ToString();
                        }
                        else
                        {
                            xdoc = DeleteRow(SalesPersonAttributionXML, false, xlist[i].Attribute(XName.Get(TargetNameXname)).Value, xlist[i].Attribute(XName.Get(StartDateXname)).Value, xlist[i].Attribute(XName.Get(TitleXname)).Value);
                        }
                    }
                }
                SalesPersonAttributionXML = xdoc.ToString();
            }
            else
            {
                XDocument xdoc = XDocument.Parse(SalesPracticeAttributionXML);
                List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();

                for (int i = 0; i < xlist.Count; i++)
                {
                    if (xlist[i].Attribute(XName.Get(IsEditModeXname)).Value == true.ToString())
                    {
                        if (xlist[i].Attribute(XName.Get(IsNewEntryXname)).Value == false.ToString())
                        {
                            xlist[i].Attribute(XName.Get(IsEditModeXname)).Value = false.ToString();
                        }
                        else
                        {
                            xdoc = DeleteRow(SalesPracticeAttributionXML, false, xlist[i].Attribute(XName.Get(TargetNameXname)).Value, xlist[i].Attribute(XName.Get(StartDateXname)).Value);
                        }
                    }
                }
                SalesPracticeAttributionXML = xdoc.ToString();
            }
        }

        public void StoreTempDataWhileDeleting(AttributionCategory attribution)
        {
            XDocument xdoc;
            List<XElement> xlist;
            GridView gv;
            if (attribution == AttributionCategory.DeliveryPersonAttribution)
            {
                gv = gvDeliveryAttributionPerson;
                for (int j = 0; j < gv.Rows.Count; j++)
                {
                    HiddenField hdnEditMode = gv.Rows[j].FindControl("hdnEditMode") as HiddenField;
                    if (hdnEditMode.Value == true.ToString())
                    {
                        Label lblpersonName = gv.Rows[j].FindControl("lblPersonName") as Label;
                        Label lblStartDate = gv.Rows[j].FindControl("lblStartDate") as Label;
                        DropDownList ddlPerson = gv.Rows[j].FindControl("ddlPerson") as DropDownList;
                        DatePicker dpStartDate = gv.Rows[j].FindControl("dpStartDate") as DatePicker;
                        DatePicker dpEndDate = gv.Rows[j].FindControl("dpEndDate") as DatePicker;
                        xdoc = XDocument.Parse(DeliveryPersonAttributionXML);
                        xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
                        for (int i = 0; i < xlist.Count; i++)
                        {
                            if (xlist[i].Attribute(XName.Get(TargetNameXname)).Value == lblpersonName.Text && (Convert.ToDateTime(xlist[i].Attribute(XName.Get(StartDateXname)).Value).Date.ToShortDateString() == lblStartDate.Text))
                            {
                                xlist[i].Attribute(XName.Get(TempTargetNameXname)).Value = ddlPerson.SelectedItem.Text;
                                xlist[i].Attribute(XName.Get(TempTargetIdXname)).Value = ddlPerson.SelectedValue;
                                xlist[i].Attribute(XName.Get(TempStartDateXname)).Value = dpStartDate.TextValue;
                                xlist[i].Attribute(XName.Get(TempEndDateXname)).Value = dpEndDate.TextValue;
                            }
                        }
                        DeliveryPersonAttributionXML = xdoc.ToString();
                    }
                }
            }
            else if (attribution == AttributionCategory.SalesPersonAttribution)
            {
                gv = gvSalesAttributionPerson;
                for (int j = 0; j < gv.Rows.Count; j++)
                {
                    HiddenField hdnEditMode = gv.Rows[j].FindControl("hdnEditMode") as HiddenField;
                    if (hdnEditMode.Value == true.ToString())
                    {
                        Label lblpersonName = gv.Rows[j].FindControl("lblPersonName") as Label;
                        Label lblStartDate = gv.Rows[j].FindControl("lblStartDate") as Label;
                        DropDownList ddlPerson = gv.Rows[j].FindControl("ddlPerson") as DropDownList;
                        DatePicker dpStartDate = gv.Rows[j].FindControl("dpStartDate") as DatePicker;
                        DatePicker dpEndDate = gv.Rows[j].FindControl("dpEndDate") as DatePicker;
                        xdoc = XDocument.Parse(SalesPersonAttributionXML);
                        xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
                        for (int i = 0; i < xlist.Count; i++)
                        {
                            if (xlist[i].Attribute(XName.Get(TargetNameXname)).Value == lblpersonName.Text && (Convert.ToDateTime(xlist[i].Attribute(XName.Get(StartDateXname)).Value).Date.ToShortDateString() == lblStartDate.Text))
                            {
                                xlist[i].Attribute(XName.Get(TempTargetNameXname)).Value = ddlPerson.SelectedItem.Text;
                                xlist[i].Attribute(XName.Get(TempTargetIdXname)).Value = ddlPerson.SelectedValue;
                                xlist[i].Attribute(XName.Get(TempStartDateXname)).Value = dpStartDate.TextValue;
                                xlist[i].Attribute(XName.Get(TempEndDateXname)).Value = dpEndDate.TextValue;
                            }
                        }
                        SalesPersonAttributionXML = xdoc.ToString();
                    }
                }
            }
            else if (attribution == AttributionCategory.DeliveryPracticeAttribution)
            {
                gv = gvDeliveryAttributionPractice;
                for (int j = 0; j < gv.Rows.Count; j++)
                {
                    HiddenField hdnEditMode = gv.Rows[j].FindControl("hdnEditMode") as HiddenField;
                    if (hdnEditMode.Value == true.ToString())
                    {
                        Label lblPractice = gv.Rows[j].FindControl("lblPractice") as Label;
                        Label lblCommisssionPercentage = gv.Rows[j].FindControl("lblCommisssionPercentage") as Label;
                        DropDownList ddlPractice = gv.Rows[j].FindControl("ddlPractice") as DropDownList;
                        TextBox txtCommisssionPercentage = gv.Rows[j].FindControl("txtCommisssionPercentage") as TextBox;
                        xdoc = XDocument.Parse(DeliveryPracticeAttributionXML);
                        xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
                        for (int i = 0; i < xlist.Count; i++)
                        {
                            if (xlist[i].Attribute(XName.Get(TargetNameXname)).Value == lblPractice.Text)
                            {
                                xlist[i].Attribute(XName.Get(TempTargetNameXname)).Value = ddlPractice.SelectedItem.Text;
                                xlist[i].Attribute(XName.Get(TempTargetIdXname)).Value = ddlPractice.SelectedValue;
                                xlist[i].Attribute(XName.Get(TempPercentageXname)).Value = txtCommisssionPercentage.Text;
                            }
                        }
                        DeliveryPracticeAttributionXML = xdoc.ToString();
                    }
                }
            }
            else
            {
                gv = gvSalesAttributionPractice;
                for (int j = 0; j < gv.Rows.Count; j++)
                {
                    HiddenField hdnEditMode = gv.Rows[j].FindControl("hdnEditMode") as HiddenField;
                    if (hdnEditMode.Value == true.ToString())
                    {
                        Label lblPractice = gv.Rows[j].FindControl("lblPractice") as Label;
                        Label lblCommisssionPercentage = gv.Rows[j].FindControl("lblCommisssionPercentage") as Label;
                        DropDownList ddlPractice = gv.Rows[j].FindControl("ddlPractice") as DropDownList;
                        TextBox txtCommisssionPercentage = gv.Rows[j].FindControl("txtCommisssionPercentage") as TextBox;
                        xdoc = XDocument.Parse(SalesPracticeAttributionXML);
                        xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
                        for (int i = 0; i < xlist.Count; i++)
                        {
                            if (xlist[i].Attribute(XName.Get(TargetNameXname)).Value == lblPractice.Text)
                            {
                                xlist[i].Attribute(XName.Get(TempTargetNameXname)).Value = ddlPractice.SelectedItem.Text;
                                xlist[i].Attribute(XName.Get(TempTargetIdXname)).Value = ddlPractice.SelectedValue;
                                xlist[i].Attribute(XName.Get(TempPercentageXname)).Value = txtCommisssionPercentage.Text;
                            }
                        }
                        SalesPracticeAttributionXML = xdoc.ToString();
                    }
                }
            }
        }

        public void CopyTempValuesAsReal(AttributionCategory attribution)
        {
            XDocument xdoc = (attribution == AttributionCategory.DeliveryPersonAttribution) ? XDocument.Parse(DeliveryPersonAttributionXML) : (attribution == AttributionCategory.DeliveryPracticeAttribution) ? XDocument.Parse(DeliveryPracticeAttributionXML) : (attribution == AttributionCategory.SalesPersonAttribution) ? XDocument.Parse(SalesPersonAttributionXML) : XDocument.Parse(SalesPracticeAttributionXML);
            List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
            foreach (var item in xlist)
            {
                item.Attribute(XName.Get(TempTargetNameXname)).Value = item.Attribute(XName.Get(TargetNameXname)).Value;
                item.Attribute(XName.Get(TempTargetIdXname)).Value = item.Attribute(XName.Get(TargetIdXname)).Value;
                item.Attribute(XName.Get(TempStartDateXname)).Value = item.Attribute(XName.Get(StartDateXname)).Value;
                item.Attribute(XName.Get(TempEndDateXname)).Value = item.Attribute(XName.Get(EndDateXname)).Value;
                item.Attribute(XName.Get(TempPercentageXname)).Value = item.Attribute(XName.Get(PercentageXname)).Value;
            }
            if (attribution == AttributionCategory.DeliveryPersonAttribution)
                DeliveryPersonAttributionXML = xdoc.ToString();
            else if (attribution == AttributionCategory.DeliveryPracticeAttribution)
                DeliveryPracticeAttributionXML = xdoc.ToString();
            else if (attribution == AttributionCategory.SalesPersonAttribution)
                SalesPersonAttributionXML = xdoc.ToString();
            else
                SalesPracticeAttributionXML = xdoc.ToString();
        }

        public void EnableDisableValidators(GridViewRow row, string attributionCategory)
        {
            XElement item = (XElement)row.DataItem;
            if (attributionCategory == deliveryPersonAttribution || attributionCategory == salesPersonAttribution)
            {
                RequiredFieldValidator reqPersonName = row.FindControl("reqPersonName") as RequiredFieldValidator;
                RequiredFieldValidator reqPersonStart = row.FindControl("reqPersonStart") as RequiredFieldValidator;
                CompareValidator compPersonStartType = row.FindControl("compPersonStartType") as CompareValidator;
                CustomValidator custPersonStart = row.FindControl("custPersonStart") as CustomValidator;
                RequiredFieldValidator reqPersonEnd = row.FindControl("reqPersonEnd") as RequiredFieldValidator;
                CompareValidator compPersonEndType = row.FindControl("compPersonEndType") as CompareValidator;
                CompareValidator compPersonEnd = row.FindControl("compPersonEnd") as CompareValidator;
                CustomValidator custPersonEnd = row.FindControl("custPersonEnd") as CustomValidator;
                CustomValidator custPersonDatesOverlapping = row.FindControl("custPersonDatesOverlapping") as CustomValidator;
                reqPersonName.ValidationGroup = reqPersonStart.ValidationGroup = compPersonStartType.ValidationGroup = custPersonStart.ValidationGroup = reqPersonEnd.ValidationGroup = compPersonEndType.ValidationGroup = custPersonEnd.ValidationGroup = compPersonEnd.ValidationGroup = custPersonDatesOverlapping.ValidationGroup = ValidationGroup;
                if (item.Attribute(XName.Get(IsEditModeXname)).Value == true.ToString())
                {
                    reqPersonName.Enabled = reqPersonStart.Enabled = compPersonStartType.Enabled = custPersonStart.Enabled = reqPersonEnd.Enabled = compPersonEndType.Enabled = custPersonEnd.Enabled = compPersonEnd.Enabled = custPersonDatesOverlapping.Enabled = true;
                }
                else
                {
                    reqPersonName.Enabled = reqPersonStart.Enabled = compPersonStartType.Enabled = custPersonStart.Enabled = reqPersonEnd.Enabled = compPersonEndType.Enabled = custPersonEnd.Enabled = compPersonEnd.Enabled = custPersonDatesOverlapping.Enabled = false;
                }
            }
            else
            {
                RequiredFieldValidator reqPractice = row.FindControl("reqPractice") as RequiredFieldValidator;
                RequiredFieldValidator reqCommisssionPercentage = row.FindControl("reqCommisssionPercentage") as RequiredFieldValidator;
                GridView gridView = row.NamingContainer as GridView;
                CustomValidator custCommissionsPercentage = gridView.HeaderRow.FindControl("custCommissionsPercentage") as CustomValidator;
                reqPractice.ValidationGroup = reqCommisssionPercentage.ValidationGroup = custCommissionsPercentage.ValidationGroup = ValidationGroup;
                if (item.Attribute(XName.Get(IsEditModeXname)).Value == true.ToString())
                {
                    reqPractice.Enabled = reqCommisssionPercentage.Enabled = true;
                }
                else
                {
                    reqPractice.Enabled = reqCommisssionPercentage.Enabled = false;
                }
                custCommissionsPercentage.Enabled = true;
            }
        }

        public void Validate(GridViewRow row, AttributionCategory attribution)
        {
            XElement item = (XElement)row.DataItem;
            if (attribution == AttributionCategory.DeliveryPersonAttribution || attribution == AttributionCategory.SalesPersonAttribution)
            {
                RequiredFieldValidator reqPersonName = row.FindControl("reqPersonName") as RequiredFieldValidator;
                RequiredFieldValidator reqPersonStart = row.FindControl("reqPersonStart") as RequiredFieldValidator;
                CompareValidator compPersonStartType = row.FindControl("compPersonStartType") as CompareValidator;
                CustomValidator custPersonStart = row.FindControl("custPersonStart") as CustomValidator;
                RequiredFieldValidator reqPersonEnd = row.FindControl("reqPersonEnd") as RequiredFieldValidator;
                CompareValidator compPersonEndType = row.FindControl("compPersonEndType") as CompareValidator;
                CustomValidator custPersonEnd = row.FindControl("custPersonEnd") as CustomValidator;
                CompareValidator compPersonEnd = row.FindControl("compPersonEnd") as CompareValidator;
                CustomValidator custPersonDatesOverlapping = row.FindControl("custPersonDatesOverlapping") as CustomValidator;
                reqPersonName.Validate();
                reqPersonStart.Validate();
                compPersonStartType.Validate();
                custPersonStart.Validate();
                reqPersonEnd.Validate();
                compPersonEndType.Validate();
                custPersonEnd.Validate();
                compPersonEnd.Validate();
                if (reqPersonName.IsValid && reqPersonStart.IsValid && compPersonStartType.IsValid && custPersonStart.IsValid && reqPersonEnd.IsValid && compPersonEndType.IsValid && custPersonEnd.IsValid && compPersonEnd.IsValid)
                    custPersonDatesOverlapping.Validate();
            }
            else
            {
                RequiredFieldValidator reqPractice = row.FindControl("reqPractice") as RequiredFieldValidator;
                RequiredFieldValidator reqCommisssionPercentage = row.FindControl("reqCommisssionPercentage") as RequiredFieldValidator;
                reqPractice.Validate();
                reqCommisssionPercentage.Validate();
            }
        }

        public List<int> AvailablePractices(string attributionXml)
        {
            List<int> practices = new List<int>();
            XDocument xdoc = XDocument.Parse(attributionXml);
            List<XElement> xlist = xdoc.Descendants(XName.Get(AttributionXname)).ToList();
            foreach (var item in xlist)
            {
                int practiceId;
                if (item.Attribute(XName.Get(IsNewEntryXname)).Value == false.ToString())
                {
                    if (int.TryParse(item.Attribute(XName.Get(TargetIdXname)).Value, out practiceId))
                    {
                        practices.Add(practiceId);
                    }
                }
            }
            return practices;
        }

        public string FinalXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(AttributionsXmlOpen);
            xml.Append(DeliveryPersonAttributionXML);
            xml.Append(DeliveryPracticeAttributionXML);
            xml.Append(SalesPersonAttributionXML);
            xml.Append(SalesPracticeAttributionXML);
            xml.Append(AttributionsXmlClose);
            return xml.ToString();
        }

        public void FinalSave()
        {
            if (ValidateCommissionsPercentage())
            {
                string attributionXml = FinalXml();
                ProjectId = HostingPage.ProjectId.Value;
                ServiceCallers.Custom.Project(p => p.SetProjectAttributionValues(ProjectId, attributionXml, Context.User.Identity.Name));
            }
        }

        public bool ValidateCommissionsPercentage()
        {
            CustomValidator custCommissionsPercentage = gvDeliveryAttributionPractice.HeaderRow.FindControl("custCommissionsPercentage") as CustomValidator;
            CustomValidator custCommissionsPercentageSales = gvSalesAttributionPractice.HeaderRow.FindControl("custCommissionsPercentage") as CustomValidator;
            custCommissionsPercentage.ValidationGroup = custCommissionsPercentageSales.ValidationGroup = ValidationGroup;
            custCommissionsPercentage.Validate();
            custCommissionsPercentageSales.Validate();
            return Page.IsValid;
        }

        #endregion Methods
    }
}
