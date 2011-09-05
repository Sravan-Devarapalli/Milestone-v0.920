using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace PraticeManagement.Controls
{
    [Serializable]
    public class OpportunityFilterSettings
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showActiveValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showWonValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showExperimentalValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showInactiveValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool showLostValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string clientIdsList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string opportunityGroupIdsList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string salespersonIdsList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string opportunityOwnerIdsList;

        #endregion

        #region Properties

        public bool ShowActive
        {
            get { return showActiveValue; }
            set { showActiveValue = value; }
        }

        public bool ShowWon
        {
            get { return showWonValue; }
            set { showWonValue = value; }
        }

        public bool ShowExperimental
        {
            get { return showExperimentalValue; }
            set { showExperimentalValue = value; }
        }

        public bool ShowInactive
        {
            get { return showInactiveValue; }
            set { showInactiveValue = value; }
        }

        public bool ShowLost
        {
            get { return showLostValue; }
            set { showLostValue = value; }
        }

        public string ClientIdsList
        {
            get { return clientIdsList; }

            set { clientIdsList = value; }
        }

        public string OpportunityGroupIdsList
        {
            get { return opportunityGroupIdsList; }

            set { opportunityGroupIdsList = value; }
        }

        public string SalespersonIdsList
        {
            get { return salespersonIdsList; }

            set { salespersonIdsList = value; }
        }

        public string OpportunityOwnerIdsList
        {
            get { return opportunityOwnerIdsList; }

            set { opportunityOwnerIdsList = value; }
        }

        #endregion

        #region Construction


        public OpportunityFilterSettings()
        {
            ShowWon = false;
            ShowExperimental = false;
            ShowInactive = false;
            ShowLost = false;
            ShowActive = true;
            ClientIdsList = null;
            OpportunityOwnerIdsList = null;
            OpportunityGroupIdsList = null;
            SalespersonIdsList = null;
        }

        #endregion

    }
}
