using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PraticeManagement.Utils.Excel;
using System.Net.Mail;

namespace PraticeManagement.Utils
{
    /// <summary>
    /// Export Excel using Excel Work book.
    /// </summary>
    public class NPOIExcel
    {
        //<CustomColor>~color~value~</CustomColor>
        public static string CustomColorKey = "<CustomColor>~{0}~{1}~</CustomColor>";
        public static string CustomColorWithBoldKey = "<CustomColorWithBold>~{0}~{1}~</CustomColorWithBold>";
        public static string SuperscriptKey = "<SuperScript>~{0}~{1}~{2}~{3}~</SuperScript>";
        public static string SuperscriptStartTag = "<SuperScript>";
        public static string CustomColorStartTag = "<CustomColor>";
        public static string CustomColorWithBoldStartTag = "<CustomColorWithBold>";
        public static string BoldFontKeyStartTag = "<BoldFont>";

        public static void Export(string fileName, List<DataSet> dsInput, List<SheetStyles> sheetStylesList)
        {
            HSSFWorkbook hssfworkbook = GetWorkbook(dsInput, sheetStylesList);
            Export(fileName, hssfworkbook);
        }

        public static byte[] GetAttachment(List<DataSet> dsInput, List<SheetStyles> sheetStylesList)
        {
            HSSFWorkbook hssfworkbook = GetWorkbook(dsInput, sheetStylesList);
            return PrepareAttachment(hssfworkbook);
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

        public static byte[] PrepareAttachment(HSSFWorkbook hssfworkbook)
        {
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Logic 2020";
            hssfworkbook.DocumentSummaryInformation = dsi;
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            byte[] attachmentByte = file.ToArray();
            return attachmentByte;
        }

        private static int GetLowerNumber(int i, int j)
        {
            return i < j ? i : j;
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
                    if (dataset.Tables.Count <= 0) continue;
                    int i = 0;
                    int tableStartRow = i;
                    ISheet sheet = hssfworkbook.CreateSheet(dataset.DataSetName);
                    for (int table = 0; table < dataset.Tables.Count; table++)
                    {
                        var datatable = dataset.Tables[table];
                        for (; i < tableStartRow + datatable.Rows.Count + 1; i++)
                        {
                            int j = 0;
                            foreach (DataColumn dc in datatable.Columns)
                            {
                                if (i == tableStartRow)
                                {
                                    IRow row = j == 0 ? sheet.CreateRow(i) : sheet.GetRow(i);
                                    ICell cell = row.CreateCell(j);
                                    cell.CellStyle = coloumnHeader;
                                    var value = dc.ColumnName;
                                    bool boolvalue = false;
                                    double doubleValue;
                                    DateTime dateTimeValue;

                                    var sheetNumber = GetLowerNumber(k, sheetStylesList.Count - 1);
                                    var rowNumber = GetLowerNumber(i - tableStartRow, sheetStylesList[sheetNumber].rowStyles.Length - 1);
                                    var cellNumber = GetLowerNumber(j, sheetStylesList[sheetNumber].rowStyles[rowNumber].cellStyles.Length - 1);
                                    var style = sheetStylesList[sheetNumber].rowStyles[rowNumber].cellStyles[cellNumber];
                                    if (style.DataFormat != string.Empty)
                                    {
                                        if (Boolean.TryParse(value, out boolvalue))
                                            cell.SetCellValue(boolvalue);
                                        else if (double.TryParse(value, out doubleValue))
                                            cell.SetCellValue(doubleValue);
                                        else if (DateTime.TryParse(value, out dateTimeValue))
                                            cell.SetCellValue(dateTimeValue);
                                        else
                                            cell.SetCellValue(value);
                                    }
                                    else
                                    {
                                        cell.SetCellValue(value);
                                    }
                                    
                                }
                                else
                                {
                                    IRow row = j == 0 ? sheet.CreateRow(i) : sheet.GetRow(i);
                                    ICell cell = row.CreateCell(j);
                                    var value = datatable.Rows[i - tableStartRow - 1][dc.ColumnName].ToString();

                                    var sheetNumber = GetLowerNumber(k, sheetStylesList.Count - 1);
                                    var rowNumber = GetLowerNumber(i - tableStartRow, sheetStylesList[sheetNumber].rowStyles.Length - 1);
                                    var cellNumber = GetLowerNumber(j, sheetStylesList[sheetNumber].rowStyles[rowNumber].cellStyles.Length - 1);
                                    var style = sheetStylesList[sheetNumber].rowStyles[rowNumber].cellStyles[cellNumber];
                                    bool boolvalue = false;
                                    double doubleValue;
                                    DateTime dateTimeValue;
                                    if (style.DataFormat != string.Empty)
                                    {
                                        if (Boolean.TryParse(value, out boolvalue))
                                            cell.SetCellValue(boolvalue);
                                        else if (double.TryParse(value, out doubleValue))
                                            cell.SetCellValue(doubleValue);
                                        else if (DateTime.TryParse(value, out dateTimeValue))
                                            cell.SetCellValue(dateTimeValue);
                                        else
                                            cell.SetCellValue(value);
                                    }
                                    else
                                    {
                                        if (double.TryParse(value, out doubleValue))
                                            cell.SetCellValue(doubleValue);
                                        else
                                        cell.SetCellValue(value);
                                    }
                                    if (style.CellFormula != string.Empty)
                                    {
                                        cell.SetCellFormula(style.CellFormula);
                                    }
                                }
                                j++;
                            }
                        }
                        sheet.CreateRow(i);
                        i++;
                        tableStartRow = i;
                        sheetStylesList[k].parentWorkbook = sheet.Workbook;
                        sheetStylesList[k].ApplySheetStyles(sheet);
                        if (k < sheetStylesList.Count - 1)
                        {
                            k++;
                        }
                    }
                }
            }
            if (hssfworkbook.NumberOfSheets == 0)
            {
                hssfworkbook.CreateSheet();
            }
            return hssfworkbook;
        }
    }
}

