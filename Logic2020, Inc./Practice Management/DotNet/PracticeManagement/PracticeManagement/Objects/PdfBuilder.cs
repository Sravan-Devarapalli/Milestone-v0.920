using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using PraticeManagement.Configuration;

namespace PraticeManagement.Objects
{
    #region HtmlToPdfBuilder Class

    /// <summary>
    /// Simplifies generating HTML into a PDF file
    /// </summary>
    public class HtmlToPdfBuilder
    {
        #region Constants

        private const string STYLE_DEFAULT_TYPE = "style";
        private const string DOCUMENT_HTML_START = "<html><head></head><body>";
        private const string DOCUMENT_HTML_END = "</body></html>";
        private const string REGEX_GROUP_SELECTOR = "selector";
        private const string REGEX_GROUP_STYLE = "style";

        //amazing regular expression magic
        private const string REGEX_GET_STYLES = @"(?<selector>[^\{\s]+\w+(\s\[^\{\s]+)?)\s?\{(?<style>[^\}]*)\}";

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Creates a new PDF document template. Use PageSizes.{DocumentSize}
        /// </summary>
        public HtmlToPdfBuilder(Rectangle size)
        {
            this.PageSize = size;
            this._Pages = new List<HtmlPdfPage>();
            this._Styles = new StyleSheet();
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// Method to override to have additional control over the document
        /// </summary>
        public event RenderEvent BeforeRender = (writer, document) => { };

        /// <summary>
        /// Method to override to have additional control over the document
        /// </summary>
        public event RenderEvent AfterRender = (writer, document) => { };

        #endregion Delegates

        #region Properties

        /// <summary>
        /// The page size to make this document
        /// </summary>
        public Rectangle PageSize { get; set; }

        /// <summary>
        /// Returns the page at the specified index
        /// </summary>
        public HtmlPdfPage this[int index]
        {
            get
            {
                return this._Pages[index];
            }
        }

        /// <summary>
        /// Returns a list of the pages available
        /// </summary>
        public IEnumerable<HtmlPdfPage> Pages
        {
            get
            {
                return this._Pages.AsEnumerable();
            }
        }

        #endregion Properties

        #region Members

        private readonly List<HtmlPdfPage> _Pages;
        private readonly StyleSheet _Styles;

        #endregion Members

        #region Working With The Document

        /// <summary>
        /// Appends and returns a new page for this document
        /// </summary>
        public HtmlPdfPage AddPage()
        {
            HtmlPdfPage page = new HtmlPdfPage();
            this._Pages.Add(page);
            return page;
        }

        /// <summary>
        /// Removes the page from the document
        /// </summary>
        public void RemovePage(HtmlPdfPage page)
        {
            this._Pages.Remove(page);
        }

        /// <summary>
        /// Appends a style for this sheet
        /// </summary>
        public void AddTagStyle(string selector, string styles)
        {
            this._Styles.LoadTagStyle(selector, HtmlToPdfBuilder.STYLE_DEFAULT_TYPE, styles);
        }

        /// <summary>
        /// Appends a style for this sheet
        /// </summary>
        public void AddStyle(string selector, string styles)
        {
            this._Styles.LoadStyle(selector, HtmlToPdfBuilder.STYLE_DEFAULT_TYPE, styles);
        }

        /// <summary>
        /// Imports a stylesheet into the document
        /// </summary>
        public void ImportStylesheet(string path)
        {
            //load the file
            string content = File.ReadAllText(path);

            //use a little regular expression magic
            foreach (Match match in Regex.Matches(content, HtmlToPdfBuilder.REGEX_GET_STYLES))
            {
                string selector = match.Groups[HtmlToPdfBuilder.REGEX_GROUP_SELECTOR].Value;
                string style = match.Groups[HtmlToPdfBuilder.REGEX_GROUP_STYLE].Value;
                this.AddTagStyle(selector, style);
            }
        }

        #endregion Working With The Document

        #region Document Navigation

        /// <summary>
        /// Moves a page before another
        /// </summary>
        public void InsertBefore(HtmlPdfPage page, HtmlPdfPage before)
        {
            this._Pages.Remove(page);
            this._Pages.Insert(
                Math.Max(this._Pages.IndexOf(before), 0),
                page);
        }

        /// <summary>
        /// Moves a page after another
        /// </summary>
        public void InsertAfter(HtmlPdfPage page, HtmlPdfPage after)
        {
            this._Pages.Remove(page);
            this._Pages.Insert(
                Math.Min(this._Pages.IndexOf(after) + 1, this._Pages.Count),
                page);
        }

        #endregion Document Navigation

        #region Rendering The PDF Document Using  ITextSharp

        public PdfPTable GetPdftable(String Pdftablevalues = "", TableStyles tableStyles = null, String rowSpliter = "~", String coloumSpliter = "!")
        {
            PdfPTable _Pdftable = null;
            if (Pdftablevalues != "")
            {
                String[] rowSpliterArray = { rowSpliter };
                String[] coloumSpliterArray = { coloumSpliter };

                String[] _PdftableRows = Pdftablevalues.Split(rowSpliterArray, StringSplitOptions.None);

                String[] _HeaderRow = _PdftableRows[0].Split(coloumSpliterArray, StringSplitOptions.None);
                int noOfColoums = _HeaderRow.Length;

                _Pdftable = new PdfPTable(noOfColoums);
                int i = 0;
                foreach (var _LableRow in _PdftableRows)
                {
                    i++;
                    String[] _Lables = _LableRow.Split(coloumSpliterArray, StringSplitOptions.None);
                    foreach (var _Lable in _Lables)
                    {
                        PdfPCell ContentLable = new PdfPCell(new Phrase(_Lable));
                        _Pdftable.AddCell(ContentLable);
                    }
                    if (i != _PdftableRows.Length)
                    {
                        _Pdftable.CompleteRow();
                    }
                }
                if (tableStyles != null)
                {
                    tableStyles.ApplyTableStyles(_Pdftable);
                }
            }

            return _Pdftable;
        }

        public PdfPTable GetPdftablePersonsByProject(String Pdftablevalues = "", TableStyles tableStyles = null, String rowSpliter = "~", String coloumSpliter = "!")
        {
            PdfPTable _Pdftable = null;
            if (Pdftablevalues != "")
            {
                String[] rowSpliterArray = { rowSpliter };
                String[] coloumSpliterArray = { coloumSpliter };

                String[] _PdftableRows = Pdftablevalues.Split(rowSpliterArray, StringSplitOptions.None);

                String[] _HeaderRow = _PdftableRows[0].Split(coloumSpliterArray, StringSplitOptions.None);
                int noOfColoums = _HeaderRow.Length;

                _Pdftable = new PdfPTable(noOfColoums);
                int i = 0;
                foreach (var _LableRow in _PdftableRows)
                {
                    i++;
                    String[] _Lables = _LableRow.Split(coloumSpliterArray, StringSplitOptions.None);
                    int count = 0;
                    string milestoneLabel = "";
                    for (int j = 0; j < _Lables.Length; j++)
                    {
                        if (count < 8 && j + 1 < _Lables.Length && _Lables[j + 1] == "!!!")
                        {
                            count++;
                            if (count == 1)
                            {
                                milestoneLabel = _Lables[j];
                            }
                            if (count == 8)
                            {
                                PdfPCell ContentLable = new PdfPCell(new Phrase(milestoneLabel));
                                ContentLable.MinimumHeight = 20;
                                //ContentLable.FixedHeight = 25;
                                ContentLable.Colspan = 8;
                                _Pdftable.AddCell(ContentLable);
                            }
                        }
                        else
                        {
                            PdfPCell ContentLable = new PdfPCell(new Phrase(_Lables[j] == "!!!" ? string.Empty : _Lables[j]));
                            ContentLable.MinimumHeight = 20;
                            _Pdftable.AddCell(ContentLable);
                        }
                    }
                    if (i != _PdftableRows.Length)
                    {
                        _Pdftable.CompleteRow();
                    }
                }
                if (tableStyles != null)
                {
                    tableStyles.ApplyTableStyles(_Pdftable);
                }
            }

            return _Pdftable;
        }

        /// <summary>
        /// Renders the PDF to an array of bytes
        /// </summary>
        public byte[] RenderPdf()
        {
            //Document is inbuilt class, available in iTextSharp
            MemoryStream file = new MemoryStream();
            Document document = new Document(this.PageSize);
            document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            PdfWriter writer = PdfWriter.GetInstance(document, file);

            //allow modifications of the document
            BeforeRender(writer, document);

            document.Open();

            //render each page that has been added
            foreach (HtmlPdfPage page in this._Pages)
            {
                document.NewPage();

                //generate this page of text
                MemoryStream output = new MemoryStream();
                StreamWriter html = new StreamWriter(output, Encoding.UTF8);

                //get the page output
                html.Write(string.Concat(HtmlToPdfBuilder.DOCUMENT_HTML_START, page._Html.ToString(), HtmlToPdfBuilder.DOCUMENT_HTML_END));
                html.Close();
                html.Dispose();

                //read the created stream
                MemoryStream generate = new MemoryStream(output.ToArray());
                StreamReader reader = new StreamReader(generate);
                foreach (var item in (IEnumerable)HTMLWorker.ParseToList(reader, this._Styles))
                {
                    document.Add((IElement)item);
                }

                //cleanup these streams
                html.Dispose();
                reader.Dispose();
                output.Dispose();
                generate.Dispose();
            }

            //after rendering
            AfterRender(writer, document);

            //return the rendered PDF
            document.Close();
            return file.ToArray();
        }

        #endregion Rendering The PDF Document Using  ITextSharp
    }

    #endregion HtmlToPdfBuilder Class

    #region PdfpTable Styles

    [Serializable]
    public class TableStyles
    {
        public float[] widths;
        public int tableWidth = 100;

        //index 0 header style index 1 data style
        public TrStyles[] trStyles;

        public string AlternateBackgroundColor = "white";
        public string AlternateBackgroundColorText = "alternative";
        public int[] BackgroundColorRGB = { 0, 0, 0 };
        public bool IsColoumBorders = true;

        public TableStyles()
        {
            const float t = 1.0f;
            this.widths = new float[1];
            this.widths[0] = t;
        }

        public TableStyles(float[] widths, TrStyles[] trStyles, int tableWidth)
        {
            this.trStyles = trStyles;
            this.widths = widths;
            this.tableWidth = tableWidth;
        }

        public TableStyles(float[] widths, TrStyles[] trStyles, int tableWidth, string alternateBackgroundColor)
        {
            this.trStyles = trStyles;
            this.widths = widths;
            this.tableWidth = tableWidth;
            this.AlternateBackgroundColor = alternateBackgroundColor;
        }

        public TableStyles(float[] widths, TrStyles[] trStyles, int tableWidth, string alternateBackgroundColor, int[] backgroundColorRGB)
        {
            this.trStyles = trStyles;
            this.widths = widths;
            this.tableWidth = tableWidth;
            this.AlternateBackgroundColor = alternateBackgroundColor;
            this.BackgroundColorRGB = backgroundColorRGB;
        }

        public PdfPTable ApplyTableStyles(PdfPTable table)
        {
            table.HorizontalAlignment = Element.ALIGN_MIDDLE;
            table.SkipLastFooter = true;
            table.WidthPercentage = tableWidth;

            if (trStyles != null)
            {
                int i = 0;
                int rowno = 0;
                if (table.NumberOfColumns == widths.Length)
                {
                    table.SetWidths(widths);
                }
                foreach (PdfPRow row in table.Rows)
                {
                    if (rowno > 0)
                    {
                        if (trStyles[i].tdStyles.Any(b => b.bold))
                        {
                            trStyles[i].BackgroundColorRGB = trStyles[i].BackgroundColorRGB;
                        }
                        else
                        {
                            if (rowno % 2 == 0)
                            {
                                trStyles[i].BackgroundColor = AlternateBackgroundColorText;
                            }
                            else
                            {
                                trStyles[i].BackgroundColor = "white";
                            }
                        }
                    }
                    trStyles[i].IsFirstRow = rowno == 0;
                    trStyles[i].IsLastRow = rowno == table.Rows.Count;
                    trStyles[i].IsColoumBorders = IsColoumBorders;
                    trStyles[i].ApplyRowStyles(row);
                    if (i < trStyles.Length - 1)
                    {
                        i++;
                    }
                    rowno++;
                }
            }
            return table;
        }

        public PdfPTable ApplyFooterStyle(PdfPTable table, TrStyles trStylesLocal)
        {
            PdfPRow lastRow = table.GetRow(table.Rows.Count - 1);
            trStylesLocal.ApplyRowStyles(lastRow);
            return table;
        }
    }

    [Serializable]
    public class TrStyles
    {
        public TdStyles[] tdStyles;
        public string BackgroundColor;
        public int[] BackgroundColorRGB = { 0, 0, 0 };
        public bool IsColoumBorders = true;
        public bool IsFirstRow = false;
        public bool IsLastRow = false;

        public TrStyles(TdStyles[] tdStyles)
        {
            this.tdStyles = tdStyles;
        }

        public TrStyles(TdStyles[] tdStyles, string backGroundColor)
        {
            this.tdStyles = tdStyles;
            this.BackgroundColor = backGroundColor;
        }

        public TrStyles(TdStyles[] tdStyles, string backGroundColor, int[] backgroundColorRGB)
        {
            this.tdStyles = tdStyles;
            this.BackgroundColor = backGroundColor;
            this.BackgroundColorRGB = backgroundColorRGB;
        }

        public PdfPRow ApplyRowStyles(PdfPRow row)
        {
            if (tdStyles != null && row != null)
            {
                int i = 0;
                int coloumCount = row.GetCells().Count();
                foreach (PdfPCell cell in row.GetCells())
                {
                    if (cell == null)
                    {
                        coloumCount--;
                        continue;
                    }
                    if (!string.IsNullOrEmpty(BackgroundColor))
                    {
                        tdStyles[i].BackgroundColor = BackgroundColor;
                    }
                    tdStyles[i].BackgroundColorRGB = BackgroundColorRGB;

                    if (!IsColoumBorders)
                    {
                        if (IsFirstRow)
                        {
                            tdStyles[i].BorderWidths = new[] { 1f, 0f, 0.5f, 0f }; //top - right- bottom -left
                        }
                        else if (IsLastRow)
                        {
                            tdStyles[i].BorderWidths = new[] { 0.5f, 0f, 1f, 0f }; //top - right- bottom -left
                        }
                        else
                        {
                            tdStyles[i].BorderWidths = new[] { 0.5f, 0f, 0.5f, 0f }; //top - right- bottom -left
                        }
                        if (coloumCount == 1) //last coloum
                        {
                            if (IsFirstRow)
                            {
                                tdStyles[i].BorderWidths = new[] { 1f, 1f, 0.5f, 0f }; //top - right- bottom -left
                            }
                            else if (IsLastRow)
                            {
                                tdStyles[i].BorderWidths = new[] { 0.5f, 1f, 1f, 0f }; //top - right- bottom -left
                            }
                            else
                            {
                                tdStyles[i].BorderWidths = new[] { 0.5f, 1f, 0.5f, 0f }; //top - right- bottom -left
                            }
                        }
                        else if (coloumCount == row.GetCells().Count()) //first coloum
                        {
                            if (IsFirstRow)
                            {
                                tdStyles[i].BorderWidths = new[] { 1f, 0f, 0.5f, 1f }; //top - right- bottom -left
                            }
                            else if (IsLastRow)
                            {
                                tdStyles[i].BorderWidths = new[] { 0.5f, 0f, 1f, 1f }; //top - right- bottom -left
                            }
                            else
                            {
                                tdStyles[i].BorderWidths = new[] { 0.5f, 0f, 0.5f, 1f }; //top - right- bottom -left
                            }
                        }
                    }

                    tdStyles[i].ApplyStyles(cell);
                    if (i < tdStyles.Length - 1)
                    {
                        i++;
                    }
                    coloumCount--;
                }
            }
            return row;
        }
    }

    [Serializable]
    public class TdStyles
    {
        public String HorizontalAlign = "left";
        public bool bold = false;
        public bool underline = false;
        public int BorderWidth = 1;
        public int fontSize = 7;
        public float PaddingTop = 10f;
        public float PaddingBottom = 10f;
        public float PaddingLeft = 5f;
        public float PaddingRight = 5f;
        public String BackgroundColor = "white";
        public String FontColor = "black";
        public int[] BackgroundColorRGB = { 0, 0, 0 };
        public int[] FontColorRGB = { 0, 0, 0 };
        public float[] BorderWidths = { 0f, 0f, 0f, 0f }; //top - right- bottom -left

        public TdStyles(String horizontalAlign, bool bold, bool underline, int fontSize, int borderWidth)
        {
            this.HorizontalAlign = horizontalAlign;
            this.bold = bold;
            this.underline = underline;
            this.fontSize = fontSize;
            this.BorderWidth = borderWidth;
        }

        public PdfPCell ApplyStyles(PdfPCell lable)
        {
            lable.VerticalAlignment = Element.ALIGN_MIDDLE;
            lable.BorderWidth = BorderWidth;
            lable.PaddingRight = PaddingRight;
            lable.PaddingLeft = PaddingLeft;
            lable.PaddingBottom = PaddingBottom;
            lable.PaddingTop = PaddingTop;
            lable.BorderWidthTop = BorderWidths[0];
            lable.BorderWidthRight = BorderWidths[1];
            lable.BorderWidthBottom = BorderWidths[2];
            lable.BorderWidthLeft = BorderWidths[3];

            switch (BackgroundColor)
            {
                case "white":
                    lable.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    break;

                case "gray":
                    lable.BackgroundColor = iTextSharp.text.BaseColor.GRAY;
                    break;

                case "light-gray":
                    lable.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    break;
                
                case "alternative":
                    iTextSharp.text.BaseColor customColor1 = new BaseColor(245, 250, 255);
                    lable.BackgroundColor = customColor1;
                    break;

                case "custom":
                    iTextSharp.text.BaseColor customColor = new BaseColor(BackgroundColorRGB[0], BackgroundColorRGB[1], BackgroundColorRGB[2]);
                    lable.BackgroundColor = customColor;
                    break;
            }
            switch (HorizontalAlign)
            {
                case "left":
                    lable.HorizontalAlignment = Element.ALIGN_LEFT;
                    break;

                case "right":
                    lable.HorizontalAlignment = Element.ALIGN_RIGHT;
                    break;

                case "center":
                    lable.HorizontalAlignment = Element.ALIGN_CENTER;
                    break;
            }
            if (lable.Phrase != null)
            {
                lable.Phrase.Font.Size = fontSize;
                switch (FontColor)
                {
                    case "black":
                        lable.Phrase.Font.Color = iTextSharp.text.BaseColor.BLACK;
                        break;

                    case "red":
                        lable.Phrase.Font.Color = iTextSharp.text.BaseColor.RED;
                        break;

                    case "yellow":
                        lable.Phrase.Font.Color = iTextSharp.text.BaseColor.YELLOW;
                        break;

                    case "custom":
                        iTextSharp.text.BaseColor customColor = new BaseColor(FontColorRGB[0], FontColorRGB[1], FontColorRGB[2]);
                        lable.BackgroundColor = customColor;
                        break;
                }
                if (bold)
                {
                    lable.Phrase.Font.SetStyle(Font.BOLD);
                }
                if (underline)
                {
                    lable.Phrase.Font.SetStyle(Font.UNDERLINE);
                }
            }
            return lable;
        }
    }

    #endregion PdfpTable Styles

    #region HtmlPdfPage Class

    /// <summary>
    /// A page to insert into a HtmlToPdfBuilder Class
    /// </summary>
    public class HtmlPdfPage
    {
        #region Constructors

        /// <summary>
        /// The default information for this page
        /// </summary>
        public HtmlPdfPage()
        {
            this._Html = new StringBuilder();
        }

        #endregion Constructors

        #region Fields

        //parts for generating the page
        internal StringBuilder _Html;

        #endregion Fields

        #region Working With The Html

        /// <summary>
        /// Appends the formatted HTML onto a page
        /// </summary>
        public virtual void AppendHtml(string content, params object[] values)
        {
            this._Html.AppendFormat(content, values);
        }

        #endregion Working With The Html
    }

    #endregion HtmlPdfPage Class

    #region Rendering Delegate

    /// <summary>
    /// Delegate for rendering events
    /// </summary>
    public delegate void RenderEvent(PdfWriter writer, Document document);

    #endregion Rendering Delegate

    #region PDFHelper Classes

    public class PDFHelper
    {
        public static PdfPTable GetPdfTableWithGivenString(String text)
        {
            PdfPTable textTable = new PdfPTable(1);
            var _BOLDITALICBaseFont = iTextSharp.text.pdf.BaseFont.CreateFont();
            var _BOLDITALICFont = new Font(_BOLDITALICBaseFont, 16, Font.ITALIC);
            PdfPCell textCell = new PdfPCell(new Phrase(text, _BOLDITALICFont));

            //Styles
            textTable.WidthPercentage = 100;
            textTable.SetWidths(new[] { 1f });
            textCell.BorderWidth = 0;
            textCell.HorizontalAlignment = Element.ALIGN_CENTER;
            textCell.PaddingTop = 20f;
            textCell.VerticalAlignment = Element.ALIGN_TOP;

            textTable.AddCell(textCell);
            textTable.CompleteRow();
            return textTable;
        }

        /// <summary>
        /// Returns the PdfHeader i.e logo
        /// </summary>
        public static PdfPTable GetPdfHeaderLogo()
        {
            PdfPTable headerTable = new PdfPTable(2);

            //logo
            byte[] buffer = BrandingConfigurationManager.LogoData.Data;
            PdfPCell logo = new PdfPCell(iTextSharp.text.Image.GetInstance(buffer), true);
            headerTable.WidthPercentage = 100;

            //logo styles
            logo.BorderWidth = 0;
            logo.HorizontalAlignment = iTextSharp.text.Image.ALIGN_LEFT;
            logo.FixedHeight = 60f;
            logo.VerticalAlignment = Element.ALIGN_MIDDLE;
            logo.PaddingBottom = 30f;
            headerTable.AddCell(logo);
            return headerTable;
        }

        /// <summary>
        /// Returns the PdfHeader i.e logo and Header Text
        /// </summary>
        public static PdfPTable GetPdfHeader(int pageNo, int pageCount)
        {
            PdfPTable headerTable = new PdfPTable(2);
            //logo
            byte[] buffer = BrandingConfigurationManager.LogoData.Data;
            PdfPCell logo = new PdfPCell(iTextSharp.text.Image.GetInstance(buffer), true);

            var baseFont = iTextSharp.text.pdf.BaseFont.CreateFont();
            var font1 = new Font(baseFont, 10, Font.NORMAL);
            var font2 = new Font(baseFont, 10, Font.NORMAL);
            PdfPTable innerTable = new PdfPTable(2);
            PdfPCell headerText1 = new PdfPCell(new Phrase("Report Date: ", font2));
            PdfPCell headerText2 = new PdfPCell(new Phrase(DateTime.Now.ToString("MM/dd/yyyy"), font1));
            PdfPCell headerText3 = new PdfPCell(new Phrase("Page  ", font2));
            PdfPCell headerText4 = new PdfPCell(new Phrase(pageNo + " of " + pageCount, font1));

            //Styles
            innerTable.WidthPercentage = headerTable.WidthPercentage = 100;
            headerTable.SetWidths(new[] { .8f, .2f });
            innerTable.SetWidths(new[] { .6f, .4f });
            logo.VerticalAlignment = Element.ALIGN_MIDDLE;
            logo.FixedHeight = 40f;
            logo.HorizontalAlignment = iTextSharp.text.Image.ALIGN_LEFT;
            headerText1.BorderWidth = headerText2.BorderWidth = headerText3.BorderWidth = headerText4.BorderWidth = logo.BorderWidth = 0;
            headerText1.HorizontalAlignment = headerText3.HorizontalAlignment = Element.ALIGN_RIGHT;
            headerText2.HorizontalAlignment = headerText4.HorizontalAlignment = Element.ALIGN_LEFT;
            headerText1.VerticalAlignment = headerText2.VerticalAlignment = headerText3.VerticalAlignment = headerText4.VerticalAlignment = Element.ALIGN_TOP;

            innerTable.AddCell(headerText1);
            innerTable.AddCell(headerText2);
            innerTable.CompleteRow();
            innerTable.AddCell(headerText3);
            innerTable.AddCell(headerText4);
            innerTable.CompleteRow();
            PdfPCell innerTableCell = new PdfPCell(innerTable);
            innerTableCell.BorderWidth = 0;
            PdfPCell innerTableCell1 = new PdfPCell(PDFHelper.GetPdfTableWithGivenString(""));
            innerTableCell1.BorderWidth = 0;
            innerTableCell1.Colspan = 2;

            headerTable.AddCell(logo);
            headerTable.AddCell(innerTableCell);
            headerTable.CompleteRow();
            headerTable.AddCell(innerTableCell1);
            headerTable.CompleteRow();

            return headerTable;
        }
    }

    public class MyPageEventHandler : PdfPageEventHelper
    {
        public int PageCount { get; set; }

        public int PageNo { get; set; }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            document.Add((IElement)PDFHelper.GetPdfHeader(PageNo, PageCount));
            PageNo++;
        }
    }

    #endregion PDFHelper Classes
}
