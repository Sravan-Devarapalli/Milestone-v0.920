using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.HSSF.Record.CF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

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

        private void SetCellValue(ICell cell, IRichTextString value)
        {
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

        public void ApplyStyles(ICell cell, List<ICellStyle> allCellStyles, Dictionary<string, short> allDataFormats)
        {
            if (parentWorkbook == null) return;
            if (cell.CellType == CellType.STRING && cell.StringCellValue.StartsWith(NPOIExcel.CustomColorStartTag))
            {
                string[] values = cell.StringCellValue.Split(new[] { '~' });
                string cellValue = values[2];
                string cellcolor = values[1];
                SetCellValue(cell, cellValue);
                SetFontColorIndex(cellcolor);
            }
            if (cell.CellType == CellType.STRING && cell.StringCellValue.StartsWith(NPOIExcel.SuperscriptStartTag))
            {
                string[] values = cell.StringCellValue.Split(new[] { '~' });
                string cellValue = values[2];
                string cellcolor = values[1];
                string cellSuperscript = values[3];
                string cellLegend = values[4]; //cellLegend = 1 for Legend in bench cost report, 0 for others
                SetFontColorIndex(cellcolor);
                var fontSuperscript = parentWorkbook.CreateFont();
                fontSuperscript.TypeOffset = FontFormatting.SS_SUPER;
                fontSuperscript.Color = FontColorIndex;
                IRichTextString richtext;
                if (cellLegend == "0")
                {
                    richtext = new HSSFRichTextString(cellValue + cellSuperscript);
                    richtext.ApplyFont(richtext.Length - cellSuperscript.Length, richtext.Length, fontSuperscript);
                }
                else
                {
                    richtext = new HSSFRichTextString(cellSuperscript + cellValue);
                    richtext.ApplyFont(0, 1, fontSuperscript);
                }
                SetCellValue(cell, richtext);
            }
            if (cell.CellType == CellType.STRING && cell.StringCellValue.StartsWith(NPOIExcel.CustomColorWithBoldStartTag))
            {
                string[] values = cell.StringCellValue.Split(new[] { '~' });
                string cellValue = values[2];
                string cellcolor = values[1];
                SetFontColorIndex(cellcolor);
                var fontInBold = parentWorkbook.CreateFont();
                fontInBold.Boldweight = (short)FontBoldWeight.BOLD;
                fontInBold.Color = FontColorIndex;
                IRichTextString richtext = new HSSFRichTextString(cellValue);
                richtext.ApplyFont(fontInBold);
                SetCellValue(cell, richtext);
            }
            ICellStyle coloumnstyle = FindCellStyle(allCellStyles, allDataFormats);
            if (coloumnstyle == null)
            {
                coloumnstyle = parentWorkbook.CreateCellStyle();

                //coloumnstyle.Indention = 10;
                coloumnstyle.BorderBottom = coloumnstyle.BorderLeft = coloumnstyle.BorderRight = coloumnstyle.BorderTop = BorderStyle;

                coloumnstyle.Alignment = HorizontalAlignment;
                coloumnstyle.VerticalAlignment = VerticalAlignment;
                coloumnstyle.ShrinkToFit = ShrinkToFit;
                coloumnstyle.WrapText = WrapText;

                IFont font = parentWorkbook.FindFont(IsBold ? (short)FontBoldWeight.BOLD : (short)FontBoldWeight.NORMAL, FontColorIndex, FontHeight, "Arial", false, false, FontFormatting.SS_NONE, (byte)FontUnderlineType.NONE);
                if (font == null)
                {
                    font = parentWorkbook.CreateFont();
                    font.Boldweight = IsBold ? (short)FontBoldWeight.BOLD : (short)FontBoldWeight.NORMAL;
                    font.Color = FontColorIndex;
                    font.FontHeight = FontHeight;
                }
                coloumnstyle.SetFont(font);
                short dataFormatShort = 0;
                if (!string.IsNullOrEmpty(DataFormat))
                {
                    var formatId = HSSFDataFormat.GetBuiltinFormat(DataFormat);
                    if (formatId == -1)
                    {
                        var newDataFormat = parentWorkbook.CreateDataFormat();
                        dataFormatShort = newDataFormat.GetFormat(DataFormat);
                    }
                    else
                        dataFormatShort = formatId;

                    if (allDataFormats.All(k => k.Key != DataFormat))
                    {
                        allDataFormats.Add(DataFormat, dataFormatShort);
                    }
                }

                coloumnstyle.DataFormat = dataFormatShort;
                allCellStyles.Add(coloumnstyle);
            }
            coloumnstyle.FillBackgroundColor = BackGroundColorIndex;

            cell.CellStyle = coloumnstyle;
        }

        public ICellStyle FindCellStyle(List<ICellStyle> allCellStyles, Dictionary<string, short> allDataFormats)
        {
            if ((string.IsNullOrEmpty(DataFormat) || allDataFormats.Any(k => k.Key == DataFormat)) &&
                allCellStyles.Any(c => c.BorderBottom == BorderStyle &&
                                    c.Alignment == HorizontalAlignment &&
                                    c.VerticalAlignment == VerticalAlignment &&
                                    c.ShrinkToFit == ShrinkToFit &&
                                    c.WrapText == WrapText &&
                                    c.GetFont(parentWorkbook).Boldweight == (short)(IsBold ? FontBoldWeight.BOLD : FontBoldWeight.NORMAL) &&
                                    c.GetFont(parentWorkbook).Color == FontColorIndex &&
                                    c.GetFont(parentWorkbook).FontHeight == FontHeight &&
                                     (string.IsNullOrEmpty(DataFormat) || c.DataFormat == allDataFormats.First(k => k.Key == DataFormat).Value)
                                    ))
            {
                return allCellStyles.First(c => c.BorderBottom == BorderStyle &&
                                       c.Alignment == HorizontalAlignment &&
                                       c.VerticalAlignment == VerticalAlignment &&
                                       c.ShrinkToFit == ShrinkToFit &&
                                       c.WrapText == WrapText &&
                                       c.GetFont(parentWorkbook).Boldweight == (short)(IsBold ? FontBoldWeight.BOLD : FontBoldWeight.NORMAL) &&
                                        c.GetFont(parentWorkbook).Color == FontColorIndex &&
                                        c.GetFont(parentWorkbook).FontHeight == FontHeight &&
                                        (string.IsNullOrEmpty(DataFormat) || c.DataFormat == allDataFormats.First(k => k.Key == DataFormat).Value)
                                       );
            }
            return null;
        }
    }
}
