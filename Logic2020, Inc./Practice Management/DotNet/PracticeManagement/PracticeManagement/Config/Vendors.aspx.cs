using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.VendorService;
using DataTransferObjects;
using DataTransferObjects.Filters;
using PraticeManagement.Utils;
using PraticeManagement.Utils.Excel;
using System.Data;
using System.IO;

namespace PraticeManagement.Config
{
    public partial class Vendors : PracticeManagementPageBase
    {
       
        private const int coloumnsCount = 6;

        public string selectedVendorTypes
        {
            get
            {
                if (cblVendorTypes.Items[0].Selected)
                {
                    return null;
                }
                return cblVendorTypes.SelectedItems;
            }
            set
            {
                cblVendorTypes.SelectedItems = value;
            }
        }

        public bool ShowActive
        {
            get
            {
                return chbShowActive.Checked;
            }
            set
            {
                chbShowActive.Checked = value;
            }
        }

        public bool ShowInActive
        {
            get
            {
                return chbShowInActive.Checked;
            }
            set
            {
                chbShowInActive.Checked = value;
            }
        }

        public Vendor[] VendorsList
        {
            get
            {

                using (var serviceClient = new VendorServiceClient())
                {
                    try
                    {
                        var result = serviceClient.GetListOfVendorsWithFilters(ShowActive, ShowInActive, selectedVendorTypes, txtSearch.Text);
                        
                        return result;
                    }
                    catch
                    {
                        serviceClient.Abort();
                        throw;
                    }

                }
            }

        }



        private SheetStyles HeaderSheetStyle
        {
            get
            {
                CellStyles cellStyle = new CellStyles();
                cellStyle.IsBold = true;
                cellStyle.BorderStyle = NPOI.SS.UserModel.BorderStyle.None;
                cellStyle.FontHeight = 350;
                CellStyles[] cellStylearray = { cellStyle };
                RowStyles headerrowStyle = new RowStyles(cellStylearray);
                headerrowStyle.Height = 500;

                CellStyles dataCellStyle = new CellStyles();
                dataCellStyle.WrapText = true;
                dataCellStyle.IsBold = true;
                CellStyles[] dataCellStylearray = { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray);

                RowStyles[] rowStylearray = { headerrowStyle };

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
                CellStyles headerWrapCellStyle = new CellStyles();
                headerWrapCellStyle.IsBold = true;
                headerWrapCellStyle.HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                headerWrapCellStyle.WrapText = true;

                List<CellStyles> headerCellStyleList = new List<CellStyles>();
                headerCellStyleList.Add(headerWrapCellStyle);
                RowStyles headerrowStyle = new RowStyles(headerCellStyleList.ToArray());

                CellStyles dataCellStyle = new CellStyles();

                var dataCellStylearray = new List<CellStyles>() { dataCellStyle };
                RowStyles datarowStyle = new RowStyles(dataCellStylearray.ToArray());
                RowStyles[] rowStylearray = { headerrowStyle, datarowStyle };
                SheetStyles sheetStyle = new SheetStyles(rowStylearray);
                sheetStyle.TopRowNo = 3;
                sheetStyle.IsFreezePane = true;
                sheetStyle.FreezePanColSplit = 0;
                sheetStyle.FreezePanRowSplit = 3;

                return sheetStyle;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillVendorTypeList(cblVendorTypes, true);
                GetFilterValuesForSession();
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    btnClearResults.Enabled = true;
                }
               
                BindData();
            }

        }

        private void BindData()
        {
            if (VendorsList.Length == 0)
            {
                divVendorEmptyMessage.Style["display"] = "";
                vendorGrid.Style["display"] = "none";
                return;
            }
            else
            {
                repVendors.DataSource = VendorsList;
                repVendors.DataBind();
                vendorGrid.Style["display"] = "";
                divVendorEmptyMessage.Style["display"] = "none";
            }
        }

        protected string GetVendorDetailsUrlWithReturn(object id)
        {
            if (id == null)
            {
                return string.Empty;
            }
            var nvgUrl = string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.VendorDetail,
                                 id.ToString());
            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(nvgUrl,
                       Request.Url.AbsoluteUri + (Request.Url.Query.Length > 0 ? string.Empty : Constants.FilterKeys.QueryStringOfApplyFilterFromCookie));

        }

        protected void btnSearchAll_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                btnClearResults.Enabled = true;
                ShowActive = true;
                ShowInActive = true;
                selectedVendorTypes = null;
                SaveFilterValuesForSession();
                BindData();
            }
        }

        protected void ResetFilter_Clicked(object sender, EventArgs e)
        {
            btnClearResults.Enabled = false;
            txtSearch.Text = string.Empty;
            ResetControls();
            BindData();
        }

        private void ResetControls()
        {
            chbShowActive.Checked = true;
            chbShowInActive.Checked = false;
            cblVendorTypes.SelectAll();
            SaveFilterValuesForSession();
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            string fileName = "VendorList.xls";
            var sheetStylesList = new List<SheetStyles>();
            var dataSetList = new List<DataSet>();

            if (VendorsList.Length > 0)
            {
                List<Vendor> excelList = VendorsList.ToList();
                string Title = "Vendor List";
                DataTable header = new DataTable();
                header.Columns.Add(Title);
                var data = PrepareDataTable(excelList);
                sheetStylesList.Add(HeaderSheetStyle);
                sheetStylesList.Add(DataSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "VendorsList";
                dataset.Tables.Add(header);
                dataset.Tables.Add(data);
                dataSetList.Add(dataset);
            }
            else
            {
                string Title = "There are no resources for the selected filters.";
                DataTable header = new DataTable();
                header.Columns.Add(Title);
                sheetStylesList.Add(HeaderSheetStyle);
                var dataset = new DataSet();
                dataset.DataSetName = "VendorsList";
                dataset.Tables.Add(header);
                dataSetList.Add(dataset);
            }
            NPOIExcel.Export(fileName, dataSetList, sheetStylesList);
        }

        public DataTable PrepareDataTable(List<Vendor> vendorList)
        {
            DateTime now = SettingsHelper.GetCurrentPMTime();
            DataTable data = new DataTable();
            List<object> row;
            data.Columns.Add("Vendor Name");
            data.Columns.Add("Contact Name");
            data.Columns.Add("Email Address");
            data.Columns.Add("Telephone Number");
            data.Columns.Add("Status");
            data.Columns.Add("Vendor Type");
            foreach (var vendor in vendorList)
            {
                row = new List<object>();
                row.Add(vendor.Name);
                row.Add(vendor.ContactName);
                row.Add(vendor.Email);
                row.Add(vendor.TelephoneNumber);
                row.Add(vendor.Status ? "Active" : "Inactive");
                row.Add(vendor.VendorType.Name);
                data.Rows.Add(row.ToArray());
            }
            return data;
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            chbShowActive.Checked = true;
            chbShowInActive.Checked = true;
            selectedVendorTypes = null;
            SaveFilterValuesForSession();
            BindData();
        }

        protected void UpdateView_Clicked(object sender, EventArgs e)
        {
            btnClearResults.Enabled = false;
            txtSearch.Text = string.Empty;
            SaveFilterValuesForSession();
            BindData();
        }

        protected override void Display()
        {

        }

        private void SaveFilterValuesForSession()
        {
            VendorFilters filters = new VendorFilters();
            filters.ShowActive = chbShowActive.Checked;
            filters.ShowInactive = chbShowInActive.Checked;
            filters.VendorTypeIds = cblVendorTypes.SelectedItems;
            filters.SearchText = txtSearch.Text;
            ReportsFilterHelper.SaveFilterValues(ReportName.VendorSummary, filters);
        }

        private void GetFilterValuesForSession()
        {
            var filters = ReportsFilterHelper.GetFilterValues(ReportName.VendorSummary) as VendorFilters;
            if (filters != null)
            {
                ShowActive = filters.ShowActive;
                ShowInActive = filters.ShowInactive;
                selectedVendorTypes = filters.VendorTypeIds;
                txtSearch.Text = filters.SearchText;
            }
            else
            {
                ResetControls();
            }
        }
    }
}
