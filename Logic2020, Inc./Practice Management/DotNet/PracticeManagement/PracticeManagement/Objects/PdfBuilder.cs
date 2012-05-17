using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Collections;
using iTextSharp.text.html;
using System.Text.RegularExpressions;
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

        #endregion

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

        #endregion

        #region Delegates

        /// <summary>
        /// Method to override to have additional control over the document
        /// </summary>
        public event RenderEvent BeforeRender = (writer, document) => { };

        /// <summary>
        /// Method to override to have additional control over the document
        /// </summary>
        public event RenderEvent AfterRender = (writer, document) => { };

        #endregion

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

        #endregion

        #region Members

        private List<HtmlPdfPage> _Pages;
        private StyleSheet _Styles;
        private PdfPTable _HeaderTable;

        #endregion

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


        #endregion

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


        #endregion

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
            if (this.BeforeRender is RenderEvent)
            {
                this.BeforeRender(writer, document);
            }

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
            if (this.AfterRender is RenderEvent)
            {
                this.AfterRender(writer, document);
            }

            //return the rendered PDF
            document.Close();
            return file.ToArray();

        }

        #endregion

    }

    #endregion

    #region PdfpTable Styles

    [Serializable]
    public class TableStyles
    {
        public float[] widths;
        public int tableWidth = 100;
        //index 0 header style index 1 data style
        public TrStyles[] trStyles;
        public string AlternateBackgroundColor = "white";
        public int[] BackgroundColorRGB = { 0, 0, 0 };
        public bool IsColoumBorders = true;
        public TableStyles()
        {
            float t = 1.0f;
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

            if (trStyles != null && table != null)
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
                        if (rowno % 2 == 0)
                        {
                            trStyles[i].BackgroundColor = AlternateBackgroundColor;
                            trStyles[i].BackgroundColorRGB = BackgroundColorRGB;
                        }
                        else
                        {
                            trStyles[i].BackgroundColor = "white";
                        }
                    }
                    trStyles[i].IsFirstRow = rowno == 0 ;
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
        public PdfPTable ApplyFooterStyle(PdfPTable table, TrStyles trStyles)
        {
            PdfPRow lastRow = table.GetRow(table.Rows.Count - 1);
            trStyles.ApplyRowStyles(lastRow);
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
                    if (!string.IsNullOrEmpty(BackgroundColor))
                    {
                        tdStyles[i].BackgroundColor = BackgroundColor;
                    }
                    tdStyles[i].BackgroundColorRGB = BackgroundColorRGB;

                    if (!IsColoumBorders)
                    {
                        if (IsFirstRow)
                        {
                            tdStyles[i].BorderWidths = new float[] { 1f, 0f, 0.5f, 0f }; //top - right- bottom -left
                        }
                        else if (IsLastRow)
                        {
                            tdStyles[i].BorderWidths = new float[] { 0.5f, 0f, 1f, 0f }; //top - right- bottom -left
                        }
                        else
                        {
                            tdStyles[i].BorderWidths = new float[] { 0.5f, 0f, 0.5f, 0f }; //top - right- bottom -left
                        }
                        if(coloumCount == 1) //last coloum 
                        {
                            if (IsFirstRow)
                            {
                                tdStyles[i].BorderWidths = new float[] { 1f, 1f, 0.5f, 0f }; //top - right- bottom -left
                            }
                            else if (IsLastRow)
                            {
                                tdStyles[i].BorderWidths = new float[] { 0.5f, 1f, 1f, 0f }; //top - right- bottom -left
                            }
                            else
                            {
                                tdStyles[i].BorderWidths = new float[] { 0.5f, 1f, 0.5f, 0f }; //top - right- bottom -left
                            }
                        }
                        else if (coloumCount == row.GetCells().Count()) //first coloum 
                        {
                            if (IsFirstRow)
                            {
                                tdStyles[i].BorderWidths = new float[] { 1f, 0f, 0.5f, 1f }; //top - right- bottom -left
                            }
                            else if (IsLastRow)
                            {
                                tdStyles[i].BorderWidths = new float[] { 0.5f, 0f, 1f, 1f }; //top - right- bottom -left
                            }
                            else
                            {
                                tdStyles[i].BorderWidths = new float[] { 0.5f, 0f, 0.5f, 1f }; //top - right- bottom -left
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
        public int fontSize = 12;
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

    #endregion

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

        #endregion

        #region Fields

        //parts for generating the page
        internal StringBuilder _Html;

        #endregion

        #region Working With The Html

        /// <summary>
        /// Appends the formatted HTML onto a page
        /// </summary>
        public virtual void AppendHtml(string content, params object[] values)
        {
            this._Html.AppendFormat(content, values);
        }

        #endregion

    }

    #endregion

    #region Rendering Delegate

    /// <summary>
    /// Delegate for rendering events
    /// </summary>
    public delegate void RenderEvent(PdfWriter writer, Document document);

    #endregion
}

