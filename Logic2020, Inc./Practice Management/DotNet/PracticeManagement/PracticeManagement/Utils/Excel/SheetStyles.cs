using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Collections;

namespace PraticeManagement.Utils.Excel
{
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
        public List<ICellStyle> AllCellStyles = new List<ICellStyle>();

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
                        rowStyles[j].ApplyRowStyles(row, AllCellStyles);
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
}
