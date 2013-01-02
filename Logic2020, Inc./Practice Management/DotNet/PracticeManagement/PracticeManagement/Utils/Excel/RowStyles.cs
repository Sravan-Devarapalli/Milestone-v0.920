using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.SS.UserModel;

namespace PraticeManagement.Utils.Excel
{
    public class RowStyles
    {
        public IWorkbook parentWorkbook;
        public CellStyles[] cellStyles;
        public short Height = 300;

        public RowStyles(CellStyles[] cellStyles)
        {
            this.cellStyles = cellStyles;
        }

        public void ApplyRowStyles(IRow row, List<ICellStyle> allCellStyles)
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
                        cellStyles[i].ApplyStyles(cell, allCellStyles);
                        if (i < cellStyles.Length - 1)
                        {
                            i++;
                        }
                    }
                }
            }
        }
    }
}
