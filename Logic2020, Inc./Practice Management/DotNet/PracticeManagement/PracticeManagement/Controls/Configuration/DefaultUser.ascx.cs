using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Events;
using PraticeManagement.Utils;
using Resources;

namespace PraticeManagement.Controls.Configuration
{
    public partial class DefaultUser : PracticeManagementUserControl
    {
        #region Constants

        private const string Allowchange = "AllowChange";
        private const string FirtItem = "FirtItem";
        private const string ViewStateDefUserEnabled = "DefUserEnabled";
        private const string personsRole = "PersonsRole";

        #endregion Constants

        #region Fields

        private Person _personToSelect;

        #endregion Fields

        #region Properties

        private PraticeManagement.Config.DefaultLineManager HostingPage
        {
            get
            {
                return ((PraticeManagement.Config.DefaultLineManager)Page);
            }
        }

        public DropDownList ManagerDdl
        {
            get
            {
                return ddlActivePersons;
            }
        }

        public bool AllowChange
        {
            get
            {
                var need = (bool?)ViewState[Allowchange];
                return need.HasValue ? need.Value : false;
            }

            set
            {
                ViewState[Allowchange] = value;
            }
        }

        public string PersonsRole
        {
            get
            {
                return (string)ViewState[personsRole];
            }
            set
            {
                ViewState[personsRole] = value;
            }
        }

        public bool InsertFirtItem
        {
            get
            {
                var need = (bool?)ViewState[FirtItem];
                return need.HasValue ? need.Value : false;
            }

            set
            {
                ViewState[FirtItem] = value;
            }
        }

        public Person SelectedManager
        {
            get
            {
                EnsureDatabound();

                var selectedValue = ddlActivePersons.SelectedValue;
                return string.IsNullOrEmpty(selectedValue) ?
                    null :
                    new Person(Convert.ToInt32(selectedValue));
            }
            set
            {
                _personToSelect = value;

                if (_personToSelect != null)
                {
                    ListItem selectedPersonListItem = ddlActivePersons.Items.FindByValue(_personToSelect.Id.Value.ToString());
                    if (selectedPersonListItem == null)
                    {
                        Person selectedPerson = ServiceCallers.Custom.Person(p => p.GetPersonDetailsShort(_personToSelect.Id.Value));

                        selectedPersonListItem = new ListItem(selectedPerson.PersonLastFirstName, _personToSelect.Id.Value.ToString());
                        ddlActivePersons.Items.Add(selectedPersonListItem);
                        ddlActivePersons.SortByText();
                    }

                    ddlActivePersons.SelectedIndex
                        = ddlActivePersons.Items.IndexOf(
                            ddlActivePersons.Items.FindByValue(_personToSelect.Id.ToString()));
                }
            }
        }

        public bool Enabled
        {
            get { return GetViewStateValue(ViewStateDefUserEnabled, true); }
            set
            {
                SetViewStateValue(ViewStateDefUserEnabled, value);
                EnableControl(value);
            }
        }

        public string OnClientChange
        {
            set;
            get;
        }

        public string CssClass
        {
            set
            {
                tbDefaultUser.Attributes["class"] = value;
            }
        }

        #endregion Properties

        #region Methods

        protected void EnableControl(bool enable)
        {
            ddlActivePersons.Enabled = enable;
        }

        public void EnsureDatabound()
        {
            if (ddlActivePersons.Items.Count == 0)
                ddlActivePersons.DataBind();
        }

        public void ExcludePerson(Person person)
        {
            if (person.Id == null) return;

            EnsureDatabound();

            var selected = ddlActivePersons.Items.FindByValue(person.Id.Value.ToString());

            if (selected != null)
                ddlActivePersons.Items.Remove(selected);
        }

        #endregion Methods

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EnsureChildControls();
                btnSetDefault.Visible = AllowChange;
                //Career Manager list should be populated with w2-salary pay type irrespective of roles.
                DataHelper.FillW2ActivePersons(ddlActivePersons, string.Empty, !InsertFirtItem, _personToSelect == null);
            }

            if (!string.IsNullOrEmpty(OnClientChange))
            {
                ddlActivePersons.Attributes.Add("onchange", OnClientChange);
            }
        }

        protected void btnSetDefault_Click(object sender, EventArgs e)
        {
            SavedNewDefaultManager();
        }

        internal bool SavedNewDefaultManager()
        {
            try
            {
                DataHelper.SetNewDefaultManager(SelectedManager.Id.Value);
                mlMessage.ShowInfoMessage(Messages.ManagerSet);

                HostingPage.ClearDirty();
                return true;
            }
            catch (Exception exc)
            {
                mlMessage.ShowErrorMessage(exc.Message);
                return false;
            }
        }

        protected void ddlActivePersons_OnDataBound(object sender, EventArgs e)
        {
            if (_personToSelect != null)
                SelectDropDownValue(_personToSelect.Id.Value.ToString());

            if (InsertFirtItem)
                ddlActivePersons.Items.Insert(0, new ListItem { Value = string.Empty, Text = string.Empty });
        }

        private void SelectDropDownValue(string valueToSelect)
        {
            try
            {
                ddlActivePersons.SelectedValue = valueToSelect;
            }
            catch (Exception)
            {
                Utils.Generic.InvokeErrorEvent(
                    CustomError,
                    this,
                    new ErrorEventArgs("Unable to select the person because it is not in the list."));
            }
        }

        public void SetEmptyItem()
        {
            var item = ddlActivePersons.Items.FindByValue(string.Empty);
            if (item != null && ddlActivePersons.Items.Contains(item))
            {
                ddlActivePersons.SelectedIndex = ddlActivePersons.Items.IndexOf(item);
            }
        }

        #endregion Events
    }
}

