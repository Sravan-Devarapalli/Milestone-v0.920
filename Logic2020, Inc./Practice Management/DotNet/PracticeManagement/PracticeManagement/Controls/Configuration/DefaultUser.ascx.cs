using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Events;
using Resources;
using System.Linq;
using PraticeManagement.Utils;
namespace PraticeManagement.Controls.Configuration
{
    public partial class DefaultUser : PracticeManagementUserControl
    {
        #region Constants

        private const string Allowchange = "AllowChange";
        private const string FirtItem = "FirtItem";
        private const string ViewStateDefUserEnabled = "DefUserEnabled";
        private const string personsRole = "PersonsRole";

        #endregion

        #region Fields

        private int _defManagerId = -1;
        private Person _personToSelect;

        #endregion

        #region Properties

        private PraticeManagement.Config.DefaultLineManager HostingPage
        {
            get
            {
                return ((PraticeManagement.Config.DefaultLineManager)Page);
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
                        Person selectedPerson = DataHelper.GetPersonWithoutFinancials(_personToSelect.Id.Value);

                        selectedPersonListItem = new ListItem(selectedPerson.PersonLastFirstName, selectedPerson.Id.Value.ToString());
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

        #endregion

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

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EnsureChildControls();
                btnSetDefault.Visible = AllowChange;
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
                DataHelper.SetNewDefaultManager(SelectedManager);
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
            else
            {
                if (_defManagerId >= 0)
                    SelectDropDownValue(_defManagerId.ToString());
            }
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

        protected void odsPersons_OnSelected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (_personToSelect == null)
                SelectDefaultManager(e.ReturnValue as IEnumerable<Person>);
            //else
            //    SelectDropDownValue(_personToSelect.Id.Value.ToString());
        }
        protected void odsPersons_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["roleName"] = PersonsRole;
        }
        private void SelectDefaultManager(IEnumerable<Person> persons)
        {
            if (persons != null)
                foreach (var person in persons)
                    if (person.Manager != null)
                    {
                        _defManagerId = person.Manager.Id.Value;
                        break;
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

        #endregion
    }
}

