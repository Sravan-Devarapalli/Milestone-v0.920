using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls
{
    /// <summary>
    /// CheckBoxList with vertical scroll pane
    /// </summary>
    [ToolboxData("<{0}:ScrollingDropDown runat=server></{0}:ScrollingDropDown>")]
    public class ScrollingDropDown : CheckBoxList
    {
        #region Nested Types

        /// <summary>
        /// Defines how to return value when all items checkbox is selected
        /// </summary>
        public enum AllSelectedType
        {
            /// <summary>
            /// Returns NULL when all items are selected
            /// </summary>
            Null,

            /// <summary>
            /// Returns list of all items
            /// </summary>
            AllItems,

            /// <summary>
            /// Default value
            /// </summary>
            Default = Null
        }

        /// <summary>
        /// Defines which value to return when there are no itmes in the control
        /// </summary>
        public enum NoItemsBehaviour
        {
            /// <summary>
            /// Return as everything is allowed
            /// </summary>
            All,

            /// <summary>
            /// Return as nothing is allowed
            /// </summary>
            Nothing,

            /// <summary>
            /// Deault value
            /// </summary>
            Default = Nothing
        }

        #endregion

        #region Constants

        /// <summary>
        /// Script key to insure script was included only once
        /// </summary>
        private const string GeneralScriptKey = "ScrollingDropDownScriptKey";

        private const string AllSelectViewstateKey = "AllSelectType";
        private const string SetDirtyViewstateKey = "SetDirtyKey";
        private const string NoItemsViewstateKey = "NoItemsInTheControl";

        #endregion

        #region Scripts

        /// <summary>
        /// Script that handles proper selection model
        /// </summary>
        private const string GeneralScriptSource =
            @"
         <script type=""text/javascript"">
            function addListener(element, type, expression, bubbling)
            {
              bubbling = bubbling || false;

              if (window.addEventListener) { // Standard
                  element.addEventListener(type, expression, bubbling);
                  return true;
              } else if (window.attachEvent) { // IE
                  element.attachEvent('on' + type, expression);
                  return true;
              } else {
                  element['on' + type] = expression;
              };
            }

            function selectAllNone(ControlClientID) {
                var anCheckbox = window.document.getElementById(ControlClientID + '_0');
                var labelCollection = window.document.getElementsByTagName('input');

                for (var i = 0; i < labelCollection.length; i++) {
                    var currCheckbox = labelCollection.item(i);
                    if (currCheckbox.id.indexOf(ControlClientID + '_') >= 0 && currCheckbox != anCheckbox) {
                        currCheckbox.checked = anCheckbox.checked;
                    }
                }
            }
            
            function processRequest(ControlClientID, src) {
                var anCheckbox = window.document.getElementById(ControlClientID + '_0');

                if (src == anCheckbox) {
                    selectAllNone(ControlClientID);
                    return;
                }

                anCheckbox.checked = false;
            }

            function addListenersToAllCheckBoxes(ControlClientID) {
                var labelCollection = window.document.getElementsByTagName('input');

                for (var i = 0; i < labelCollection.length; i++) {
                    var currCheckbox = labelCollection.item(i);
                    if (currCheckbox.id.indexOf(ControlClientID + '_') >= 0) {
                        addListener(currCheckbox, 'click', function(e) {
                            var src = e.srcElement || e.target;
                            processRequest(ControlClientID, src);
                        }, false);
                    " + InitSetDirtyMarker + @"
                    }
                }
            }
        </script>
        ";

        private const string InitSetDirtyScript =
            @"          addListener(currCheckbox, 'change', function(e) {
                            setDirty();
                        }, false);";

        private const string InitSetDirtyMarker = "// init set dirty here";

        /// <summary>
        /// Initialization script. The pattern is substituted with the ClientID value
        /// </summary>
        private const string InitializationScript =
             @"         var prm = Sys.WebForms.PageRequestManager.getInstance();
                        if (!prm.get_isInAsyncPostBack()) {{                                                 
                           prm.add_pageLoaded(function(){{                                                        
                                addListenersToAllCheckBoxes('{0}');
                           }});                      
                        }}
                        addListener(window, 'load',  function(){{                            
                            addListenersToAllCheckBoxes('{0}');
                        }});
        ";

        #endregion

        #region Styles

        /// <summary>
        /// CSS style needed for scrolling div
        /// </summary>
        private const string CssStyle =
            @"
            div.scroll_{5} {{
                height: {0};
                width: {1};
                overflow-y: auto;
                overflow-x: hidden;
                border: 1px solid {2};
                background-color: {3};
                padding: {4};
                display: inline-block;
                z-index:20000;
            }}
        ";

        #endregion

        #region Fields

        private Color _scrollBoxBorderColor;
        private Unit _scrollBoxHeight;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            base.OnInit(e);
        }

        public override Unit Height
        {
            set { _scrollBoxHeight = value; }
        }

        public override Color BorderColor
        {
            set { _scrollBoxBorderColor = value; }
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            //  Include general script and ensure that it was included only once
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GeneralScriptKey))
                Page.ClientScript.RegisterClientScriptBlock(
                    Page.GetType(), GeneralScriptKey, GenerateGeneralScriptSource, false);

            //  Prepare styles for the instance of the objects
            //      by checking if important styles are set
            string styleSheet = string.Format(CssStyle,
                                              _scrollBoxHeight.IsEmpty ? "360px" : _scrollBoxHeight.ToString(),
                                              Width.IsEmpty ? "200px" : Width.ToString(),
                                              _scrollBoxBorderColor.IsEmpty
                                                  ? "#000000"
                                                  : ColorToHex(_scrollBoxBorderColor),
                                              BackColor.IsEmpty ? "#ffffff" : ColorToHex(BackColor),
                                              CellPadding < 0 ? 0 : CellPadding,
                                              ID);
            BorderStyle = BorderStyle.None;

            // Add css style for the current control
            IncludeCss(styleSheet);

            // Add initialization script for the current control
            IncludeScript(InitializationScrpt);
        }

        private string GenerateGeneralScriptSource
        {
            get
            {
                return SetDirty
                           ? GeneralScriptSource.Replace(
                               InitSetDirtyMarker,
                               InitSetDirtyScript)
                           : GeneralScriptSource;
            }
        }

        /// <summary>
        /// Returns initialization script
        /// </summary>
        protected virtual string InitializationScrpt
        {
            get { return string.Format(InitializationScript, ClientID); }
        }

        /// <summary>
        /// Adds script for All/None selection
        /// </summary>
        protected void IncludeScript(string script)
        {
            Page.ClientScript.RegisterStartupScript(
                Page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        /// <summary>
        /// Adds scrolling style to the page header
        /// </summary>
        protected void IncludeCss(string styleSheet)
        {
            //  Add style to the header
            var stylesLink = new HtmlGenericControl("style");
            stylesLink.Attributes["type"] = "text/css";
            stylesLink.InnerText = styleSheet;

            Page.Header.Controls.Add(stylesLink);
        }

        public AllSelectedType AllSelectedReturnType
        {
            get
            {
                var obj = ViewState[AllSelectViewstateKey];
                return obj == null ? AllSelectedType.Default : (AllSelectedType)obj;
            }
            set { ViewState[AllSelectViewstateKey] = value; }
        }

        public bool SetDirty
        {
            get
            {
                var obj = ViewState[SetDirtyViewstateKey];
                return obj == null || (bool)obj;
            }
            set { ViewState[SetDirtyViewstateKey] = value; }
        }

        public NoItemsBehaviour NoItemsType
        {
            get
            {
                var obj = ViewState[NoItemsViewstateKey];
                return obj == null ? NoItemsBehaviour.Default : (NoItemsBehaviour)obj;
            }
            set { ViewState[NoItemsViewstateKey] = value; }
        }

        /// <summary>
        /// Returns comma separated values of selected items. Returns Null if 'All' (the first item) is selected.
        /// </summary>
        public string SelectedItems
        {
            get
            {
                // Check if nothing is displayed and what type should we return
                if (Items.Count == 0 && NoItemsType == NoItemsBehaviour.All)
                    return null;

                // Check if All checkbox is checked
                if (Items.Count > 0 && Items[0].Selected && AllSelectedReturnType == AllSelectedType.Null)
                    return null;

                // If not, build comma separated list of values
                var clientList = new StringBuilder();
                foreach (ListItem item in Items)
                    if (item.Selected)
                        clientList.Append(item.Value).Append(',');

                return clientList.ToString();
            }

            set
            {
                if (value == null)
                    SelectAll();
                else
                {
                    foreach (var itm in StringToLEnumerable(value))
                    {
                        var listItm = Items.FindByValue(itm);
                        if (listItm != null)
                            listItm.Selected = true;
                    }
                }
            }
        }

        public string DropDownListType
        {
            get;
            set;
        }

        public string SelectedString
        {
            get
            {
                int counter = 0;
                string text = null;
                if (Items.Count == 1 && !Items[0].Enabled)
                {
                    return "No " + DropDownListType + "s to select";
                }
                if (Items[0].Selected)
                {
                    text = Items[0].Text;
                    return text;
                }
                foreach (ListItem item in Items)
                {
                    if (item.Selected)
                    {
                        counter++;
                        text = item.Text;
                    }

                    if (counter > 1)
                    {
                        text = "Multiple " + this.DropDownListType + "s selected";
                    }

                    if (counter == 0)
                    {
                        text = "Please Choose " + this.DropDownListType + "(s)";
                    }
                }
                return text;
            }
        }

        public string SelectedValuesString
        {
            get
            {
                return DataTransferObjects.Utils.Generic.EnumerableToCsv(SelectedValues, i => i);
            }
        }

        public List<int> SelectedValues
        {
            get
            {
                // Check if nothing is displayed and what type should we return
                if (Items.Count == 0 && NoItemsType == NoItemsBehaviour.All)
                    return null;

                if (Items.Count > 0 && Items[0].Selected && AllSelectedReturnType == AllSelectedType.Null)
                    return null;

                var res = new List<int>();
                foreach (ListItem itm in Items)
                    if (itm.Selected)
                    {
                        int val;
                        if (int.TryParse(itm.Value, out val))
                            res.Add(val);
                    }

                return res;
            }
        }

        public void SelectItems(List<int> toSelectList)
        {
            if (toSelectList == null)
            {
                SelectAll();
            }
            else
            {
                foreach (ListItem itm in Items)
                {
                    int val;
                    if (int.TryParse(itm.Value, out val))
                    {
                        itm.Selected = toSelectList.Contains(val);
                    }
                }
            }
        }

        /// <summary>
        /// Converts comma separated values in enumerable collection
        /// </summary>
        /// <param name="strList">Comma separated values</param>
        /// <returns>Enumerable collection</returns>
        private static IEnumerable<string> StringToLEnumerable(string strList)
        {
            var selItems = strList.Split(
                new[] { ',' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var sItm in selItems)
                if (sItm.Trim().Length > 0)
                    yield return sItm;
        }

        /// <summary>
        /// Selects all items. Assuming that the first item is always [All]
        /// </summary>
        private void SelectAll()
        {
            if (Items.Count > 1)
                foreach (ListItem item in Items)
                    item.Selected = true;
        }

        /// <summary>
        /// Converts Color object to hex representation
        /// </summary>
        /// <param name="color">Color object</param>
        /// <returns>Hex representation of the color in #RRGGBB format</returns>
        protected static string ColorToHex(Color color)
        {
            return string.Format("#{0}{1}{2}",
                                 color.R.ToString("X"),
                                 color.G.ToString("X"),
                                 color.B.ToString("X"));
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.WriteLine(string.Format("<div id='{0}' class='scroll_{0}' />", ID));
            base.RenderControl(writer);
            writer.WriteLine("</div>");
        }
    }
}

