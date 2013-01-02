using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using NPOI.HPSF;
using NPOI.HSSF.Record.CF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace PraticeManagement.Utils
{
    /// <summary>
    /// Export Excel using Excel Work book.
    /// </summary>
    public class NPOIExcel
    {
        //<CustomColor>~color~value~</CustomColor>
        public static string CustomColorKey = "<CustomColor>~{0}~{1}~</CustomColor>";

        public static string CustomColorStartTag = "<CustomColor>";

        public static void Export(string fileName, List<DataSet> dsInput, List<SheetStyles> sheetStylesList)
        {
            HSSFWorkbook hssfworkbook = GetWorkbook(dsInput, sheetStylesList);
            Export(fileName, hssfworkbook);
        }

        public static void Export(string fileName, HSSFWorkbook hssfworkbook)
        {
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            HttpContext.Current.Response.Clear();
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Logic 2020";
            hssfworkbook.DocumentSummaryInformation = dsi;
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }

        private static HSSFWorkbook GetWorkbook(List<DataSet> dsInput, List<SheetStyles> sheetStylesList)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            if (dsInput != null)
            {
                ICellStyle coloumnHeader = hssfworkbook.CreateCellStyle();
                coloumnHeader.GetFont(hssfworkbook).Boldweight = 200;

                foreach (DataSet dataset in dsInput)
                {
                    if (dataset.Tables.Count > 0)
                    {
                        int i = 0;
                        int k = 0;
                        int tableStartRow = i;
                        int tableNo = 1;
                        bool isHeaderTableExists = dataset.Tables.Count > 1;
                        ISheet sheet = hssfworkbook.CreateSheet(dataset.DataSetName);
                        foreach (DataTable datatable in dataset.Tables)
                        {
                            for (; i < tableStartRow + datatable.Rows.Count + 1; i++)
                            {
                                int j = 0;
                                foreach (DataColumn dc in datatable.Columns)
                                {
                                    if (i == tableStartRow)
                                    {
                                        IRow row;
                                        if (j == 0)
                                            row = sheet.CreateRow(i);
                                        else
                                            row = sheet.GetRow(i);
                                        ICell cell = row.CreateCell(j);
                                        cell.CellStyle = coloumnHeader;
                                        cell.SetCellValue(dc.ColumnName);
                                    }
                                    else
                                    {
                                        IRow row;
                                        if (j == 0)
                                            row = sheet.CreateRow(i);
                                        else
                                            row = sheet.GetRow(i);
                                        ICell cell = row.CreateCell(j);
                                        var value = datatable.Rows[i - tableStartRow - 1][dc.ColumnName].ToString();
                                        bool boolvalue = false;
                                        double doubleValue;
                                        DateTime dateTimeValue;
                                        if (Boolean.TryParse(value, out boolvalue))
                                            cell.SetCellValue(boolvalue);
                                        else if (double.TryParse(value, out doubleValue))
                                            cell.SetCellValue(doubleValue);
                                        else if (DateTime.TryParse(value, out dateTimeValue))
                                            cell.SetCellValue(dateTimeValue);
                                        else
                                            cell.SetCellValue(value);
                                    }
                                    j++;
                                }
                            }
                            sheet.CreateRow(i);
                            i++;
                            tableStartRow = i;
                            tableNo++;
                            sheetStylesList[k].parentWorkbook = sheet.Workbook;
                            sheetStylesList[k].ApplySheetStyles(sheet);
                            if (k < sheetStylesList.Count - 1)
                            {
                                k++;
                            }
                        }
                    }
                }
            }
            if (hssfworkbook.NumberOfSheets == 0)
            {
                ISheet sheet = hssfworkbook.CreateSheet();
            }
            return hssfworkbook;
        }
    }

    #region Excel Styles

    public class SheetStyles
    {
        public IWorkbook parentWorkbook = null;
        //index 0 header style index 1 data style
        public RowStyles[] rowStyles;
        public bool IsAutoResize = true;
        public bool IsAutoFilter = false;
        public bool IsFreezePane = false;
        public int FreezePanColSplit = 1;
        public int FreezePanRowSplit = 1;
        public int TopRowNo = 1;
        public List<int[]> MergeRegion = new List<int[]>();

        public SheetStyles(RowStyles[] rowStyles)
        {
            this.rowStyles = rowStyles;
        }

        public void ApplySheetStyles(ISheet sheet)
        {
            //IDataValidationHelper dh =  sheet.GetDataValidationHelper();
            // sheet.SheetConditionalFormatting
            if (rowStyles != null && sheet != null && parentWorkbook != null)
            {
                int j = 0;
                int rowno = 1;
                IEnumerator rowEnumerator = sheet.GetRowEnumerator();
                while (rowEnumerator.MoveNext())
                {
                    if (TopRowNo <= rowno)
                    {
                        IRow row = (IRow)rowEnumerator.Current;
                        rowStyles[j].parentWorkbook = parentWorkbook;
                        rowStyles[j].ApplyRowStyles(row);
                        if (j < rowStyles.Length - 1)
                        {
                            j++;
                        }
                    }
                    rowno++;
                }
                if (MergeRegion.Count > 0)
                {
                    foreach (int[] regionCoordinates in MergeRegion)
                    {
                        CellRangeAddress region = new CellRangeAddress(regionCoordinates[0], regionCoordinates[1], regionCoordinates[2], regionCoordinates[3]);
                        sheet.AddMergedRegion(region);
                    }
                }

                if (IsAutoResize)
                {
                    for (int i = 0; i < sheet.GetRow(TopRowNo).Cells.Count; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }
                }
                if (IsAutoFilter)
                {
                    //need to implement this SetAutoFilter
                }
                if (IsFreezePane)
                {
                    sheet.CreateFreezePane(FreezePanColSplit, FreezePanRowSplit);
                }
            }
        }
    }

    public class RowStyles
    {
        public IWorkbook parentWorkbook;
        public CellStyles[] cellStyles;
        public short Height = 300;

        public RowStyles(CellStyles[] cellStyles)
        {
            this.cellStyles = cellStyles;
        }

        public void ApplyRowStyles(IRow row)
        {
            if (parentWorkbook != null)
            {

                if (cellStyles != null && row != null)
                {
                    row.Height = Height;

                    int i = 0;
                    foreach (ICell cell in row.Cells)
                    {
                        cellStyles[i].parentWorkbook = parentWorkbook;
                        cellStyles[i].ApplyStyles(cell);
                        if (i < cellStyles.Length - 1)
                        {
                            i++;
                        }
                    }
                }
            }
        }
    }

    public class CellStyles
    {
        public IWorkbook parentWorkbook;
        public NPOI.SS.UserModel.BorderStyle BorderStyle = NPOI.SS.UserModel.BorderStyle.THIN;
        public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.LEFT;
        public VerticalAlignment VerticalAlignment = VerticalAlignment.TOP;
        public bool ShrinkToFit = false;
        public string DataFormat = "";
        public bool WrapText = false;
        public short BackGroundColorIndex = HSSFColor.WHITE.index;
        public bool IsBold = false;
        public short FontColorIndex = HSSFColor.BLACK.index;
        public short FontHeight = 200;

        public CellStyles()
        {
        }

        private void SetCellValue(ICell cell, string value)
        {
            bool boolvalue = false;
            double doubleValue;
            DateTime dateTimeValue;
            if (Boolean.TryParse(value, out boolvalue))
                cell.SetCellValue(boolvalue);
            else if (double.TryParse(value, out doubleValue))
                cell.SetCellValue(doubleValue);
            else if (DateTime.TryParse(value, out dateTimeValue))
                cell.SetCellValue(dateTimeValue);
            else
                cell.SetCellValue(value);
        }

        private void SetFontColorIndex(string color)
        {
            switch (color)
            {
                case "red":
                    FontColorIndex = HSSFColor.RED.index;
                    break;
                case "purple":
                    FontColorIndex = HSSFColor.VIOLET.index;
                    break;
                case "green":
                    FontColorIndex = HSSFColor.GREEN.index;
                    break;
                default:
                    FontColorIndex = HSSFColor.BLACK.index;
                    break;

            }
        }
        
        public void ApplyStyles(ICell cell)
        {
            if (parentWorkbook != null)
            {

                ICellStyle coloumnstyle = parentWorkbook.CreateCellStyle();
                //coloumnstyle.Indention = 10;
                coloumnstyle.BorderBottom = coloumnstyle.BorderLeft = coloumnstyle.BorderRight = coloumnstyle.BorderTop = BorderStyle;
                if (cell.CellType == CellType.STRING && cell.StringCellValue.StartsWith(NPOIExcel.CustomColorStartTag))
                {
                    string[] values = cell.StringCellValue.Split(new char[] { '~' });
                    string cellValue = values[2];
                    string cellcolor = values[1];
                    SetCellValue(cell, cellValue);
                    SetFontColorIndex(cellcolor);
                }
                IFont font = parentWorkbook.FindFont(IsBold ? (short)FontBoldWeight.BOLD : (short)FontBoldWeight.NORMAL, FontColorIndex, FontHeight, "Arial", false, false, FontFormatting.SS_NONE, (byte)FontUnderlineType.NONE);
                if (font == null)
                {
                    font = parentWorkbook.CreateFont();
                    font.Boldweight = IsBold ? (short)FontBoldWeight.BOLD : (short)FontBoldWeight.NORMAL;
                    font.Color = FontColorIndex;
                    font.FontHeight = FontHeight;
                }
                coloumnstyle.SetFont(font);
                coloumnstyle.Alignment = HorizontalAlignment;
                coloumnstyle.VerticalAlignment = VerticalAlignment;
                coloumnstyle.ShrinkToFit = ShrinkToFit;
                coloumnstyle.WrapText = WrapText;
                coloumnstyle.FillBackgroundColor = BackGroundColorIndex;

                var formatId = HSSFDataFormat.GetBuiltinFormat(DataFormat);
                if (formatId == -1)
                {
                    var newDataFormat = parentWorkbook.CreateDataFormat();
                    coloumnstyle.DataFormat = newDataFormat.GetFormat(DataFormat);
                }
                else
                    coloumnstyle.DataFormat = formatId;
                cell.CellStyle = coloumnstyle;
            }
        }
    }

    #endregion

}
