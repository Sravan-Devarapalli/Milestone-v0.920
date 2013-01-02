using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using NPOI.HSSF.Record.CF;
using NPOI.HSSF.UserModel;

namespace PraticeManagement.Utils.Excel
{
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
}
