using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace PraticeManagement.Controls
{
    [ToolboxData("<{0}:CascadingMsdd runat=server></{0}:CascadingMsdd>")]
    public class CascadingMsdd : ScrollingDropDown
    {
        /// <summary>
        /// Tracks checkbox dependence between source and target controls
        /// </summary>
        public class ControlDependence
        {
            private string source;
            private string target;
            private int srcIndex;
            private int trgIndex;

            /// <summary>
            /// Init constructor of ControlDependence.
            /// </summary>
            /// <param name="source">Source id</param>
            /// <param name="srcIndex">Source checkbox index</param>
            /// <param name="target">Target id</param>
            /// <param name="trgIndex">Target checkbox index</param>
            public ControlDependence(string source, int srcIndex, string target, int trgIndex)
            {
                this.source = source;
                this.target = target;
                this.srcIndex = srcIndex;
                this.trgIndex = trgIndex;
            }

            #region Properties
            /// <summary>
            /// Source id
            /// </summary>
            public string Source
            {
                get
                {
                    return this.source;
                }
                set
                {
                    this.source = value;
                }
            }

            /// <summary>
            /// Target id
            /// </summary>
            public string Target
            {
                get
                {
                    return this.target;
                }
                set
                {
                    this.target = value;
                }
            }

            /// <summary>
            /// Source checkbox index
            /// </summary>
            public int SrcIndex
            {
                get
                {
                    return this.srcIndex;
                }
                set
                {
                    this.srcIndex = value;
                }
            }

            /// <summary>
            /// Target checkbox index
            /// </summary>
            public int TrgIndex
            {
                get
                {
                    return this.trgIndex;
                }
                set
                {
                    this.trgIndex = value;
                }
            }
            #endregion
        }

        private IList<ControlDependence> dependencies;

        private const string additionalInitialization = @"
            addListener(window, 'load',  function(){{
                addListenersToParent('{0}', '{1}');
            }});
        ";

        private const string additionalScript = @"
            
            function findValuesBySource(coll, src)
            {
                var res = [];
                var rc = 0;
                for (var i = 0; i < coll.length; i++){
                    if (coll[i].source == src){
                        res[rc] = coll[i].target;
                        rc++;
                    }
                }
                
                return res;
            }
            
            function collContains(coll, itm)
            {
                for (var i = 0; i < coll.length; i++)
                    if ((coll[i].indexOf(itm) >= 0) && (coll[i].length == itm.length))
                        return true;
                
                return false;
            }

            function selectOnTarget(targetControl, evObj){
                var labelCollection = window.document.getElementsByTagName('input');

                var trgs = findValuesBySource(linking, evObj.id);

                for (var i = 0; i < labelCollection.length; i++) {
                    var currCheckbox = labelCollection.item(i);
                    if (currCheckbox.id.indexOf(targetControl + '_') >= 0 && collContains(trgs, currCheckbox.id)) {
                       currCheckbox.checked = evObj.checked;
                       if (evObj.checked){
                           currCheckbox.parentNode.parentNode.className = '';
                       }
                       else{
                           currCheckbox.parentNode.parentNode.className = 'tab-invisible';
                       }
                    }
                }
            }

            function selectAllNoneDependent(targetControl, anCheckbox) {
                var labelCollection = window.document.getElementsByTagName('input');

                for (var i = 0; i < labelCollection.length; i++) {
                    var currCheckbox = labelCollection.item(i);
                    if (currCheckbox.id.indexOf(targetControl + '_') >= 0) {
                        currCheckbox.checked = anCheckbox.checked;
                        if (currCheckbox.id.indexOf(targetControl + '_0') < 0)
                        {
                           if (anCheckbox.checked){
                               currCheckbox.parentNode.parentNode.className = '';
                           }
                           else{
                               currCheckbox.parentNode.parentNode.className = 'tab-invisible';
                           }
                        }
                    }
                }
            }

            function addListenersToParent(targetControl, ControlClientID) {
                var labelCollection = window.document.getElementsByTagName('input');
                var anCheckbox = window.document.getElementById(ControlClientID + '_0');

                for (var i = 0; i < labelCollection.length; i++) {
                    var currCheckbox = labelCollection.item(i);
                    if (currCheckbox.id.indexOf(ControlClientID + '_') >= 0) {
                        if (currCheckbox == anCheckbox)
                            addListener(currCheckbox, 'click', function(e) {
                                selectAllNoneDependent(targetControl, anCheckbox)
                            }, false);
                        else
                            addListener(currCheckbox, 'click', function(e) {
                                var src = e.srcElement || e.target;
                                selectOnTarget(targetControl, src);
                            }, false);
    
                        if (!currCheckbox.checked)
                            selectOnTarget(targetControl, currCheckbox);
                    }
                }
            }
        ";

        private const string VIEWSTATE_DEPENDANCE_KEY = "Dependence";

        private string targetControlId;

        /// <summary>
        /// Default constructor of CascadingMsdd.
        /// </summary>
        public CascadingMsdd()
        {
            dependencies = new List<ControlDependence>();
        }

        public void AddDependence(ControlDependence dp)
        {
            dependencies.Add(dp);
        }

        protected override string InitializationScrpt
        {
            get
            {
                return base.InitializationScrpt +
                    string.Format(additionalInitialization, TrgClientId, ClientID);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            IncludeScript(Dependence);
            IncludeScript(additionalScript);

            base.Page_Load(sender, e);
        }

        public string Dependence
        {
            get
            {
                String s = (String)ViewState[VIEWSTATE_DEPENDANCE_KEY];

                if (s == null) 
                {
                    s = GenerateLinkingVariable();
                    ViewState[VIEWSTATE_DEPENDANCE_KEY] = s;
                }

                return s;
            }
            set
            {
                ViewState[VIEWSTATE_DEPENDANCE_KEY] = value;
            }
        }

        /// <summary>
        /// Generates control dependence variable in JSON format
        /// </summary>
        /// <returns>Variable</returns>
        private string GenerateLinkingVariable()
        {
            StringBuilder linking = new StringBuilder("var linking = [");
            
            foreach (ControlDependence dp in dependencies)
            {
                linking.AppendFormat(
                    "{{source: '{0}_{1}', target: '{2}_{3}'}},",
                    dp.Source, dp.SrcIndex, dp.Target, dp.TrgIndex);
            }

            return linking.ToString().TrimEnd(new char[] { ',' }) + "];";
        }

        public string TargetControlId
        {
            get
            {
                return targetControlId;
            }
            set
            {
                targetControlId = value;
            }
        }

        private string TrgClientId
        {
            get
            {
                Control ctrl = this.Parent.FindControl(TargetControlId);

                return ctrl == null ? null : ctrl.ClientID;
            }
        }
    }
}

