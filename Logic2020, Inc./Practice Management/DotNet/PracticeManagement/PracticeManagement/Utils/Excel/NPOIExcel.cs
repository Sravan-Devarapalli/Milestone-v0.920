using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PraticeManagement.Utils.Excel;

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
                
                int k = 0;
                foreach (DataSet dataset in dsInput)
                {
                    if (dataset.Tables.Count > 0)
                    {
                        int i = 0;
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
}
