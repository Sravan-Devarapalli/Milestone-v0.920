using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace PraticeManagement.Utils
{
    /// <summary>
    /// Export Excel using Excel Work book.
    /// </summary>
    public class NPOIExcel
    {
        public static void Export(string fileName, List<DataSet> dsInput)
        {
            HSSFWorkbook hssfworkbook = GetWorkbook(dsInput);
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

        private static HSSFWorkbook GetWorkbook(List<DataSet> dsInput)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ICellStyle coloumnHeader = hssfworkbook.CreateCellStyle();
            coloumnHeader.GetFont(hssfworkbook).Boldweight = 20;
            foreach (DataSet dataset in dsInput)
            {
                int i = 0;
                int tableStartRow = i;
                ISheet sheet = hssfworkbook.CreateSheet(string.IsNullOrEmpty(dataset.DataSetName) ? dataset.Tables[0].TableName : dataset.DataSetName);
                foreach (DataTable datatable in dataset.Tables)
                {
                    for (; i < tableStartRow + datatable.Rows.Count; i++)
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
                                cell.SetCellValue(datatable.Rows[i - tableStartRow][dc.ColumnName].ToString());
                            }
                            j++;
                        }
                    }

                    sheet.CreateRow(i).CreateCell(0).SetCellValue("");
                    i++;
                    tableStartRow = i;
                }

            }
            return hssfworkbook;
        }
    }
}
