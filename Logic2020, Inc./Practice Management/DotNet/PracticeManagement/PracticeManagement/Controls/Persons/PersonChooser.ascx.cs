using System;
using System.Web.Security;
using System.Web.UI;
using DataTransferObjects;
using System.Web.UI.WebControls;
using PraticeManagement.PersonService;
using System.Linq;


namespace PraticeManagement.Controls.Persons
{
    public partial class PersonChooser : UserControl
    {
        #region Constants

        private const string SELECTED_PERSON_ID = "SelectedPersonId";
        private const string SelectedPersonFormat = "{0}";

        #endregion

        #region Events

        #region Delegates

        public delegate void PersonChangedHandler(object sender, PersonChangedEventArguments args);

        #endregion

        public event PersonChangedHandler PersonChanged;

        #endregion

        #region Properties

        public Person SelectedPerson
        {
            get { return (Person)ViewState[SELECTED_PERSON_ID]; }
            set
            {
                Person person = value;
                using (var serviceClient = new PersonServiceClient())
                {
                    person.EmploymentHistory = serviceClient.GetPersonEmploymentHistoryById(person.Id.Value).ToList();
                }
                ViewState[SELECTED_PERSON_ID] = person;
            }
        }

        public DropDownList ddlPersonsDropDown
        {
            get
            {
                return (DropDownList)ddlPersons;
            }
        }

        public int SelectedPersonId
        {
            get { return SelectedPerson.Id.Value; }
        }

        public TimeEntry_New HostingPage
        {
            get
            {
                if (Page is TimeEntry_New)
                    return ((TimeEntry_New)Page);
                else
                    return null;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var userIsAdministrator =
                    Roles.IsUserInRole(
                        DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

                //  Get currently logged in person
                var currentPerson = DataHelper.CurrentPerson;

                // Set it's Id value
                var personId = currentPerson.Id.Value;

                if (!userIsAdministrator)
                {
                    ddlPersons.Visible = false;
                    lblTip.Text =
                        string.Format(SelectedPersonFormat,
                            currentPerson);
                }

                SelectedPerson = personId == currentPerson.Id.Value ? currentPerson : DataHelper.GetPerson(personId);
            }
        }

        protected void ddlPersons_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPersonChanged(Convert.ToInt32(ddlPersons.SelectedValue));
        }

        protected virtual void OnPersonChanged(int personId)
        {
            var isSaved = true;
            if (HostingPage != null)
            {
                if (HostingPage.IsDirty)
                {
                    isSaved = HostingPage.SaveData();
                }
            }

            if (isSaved)
            {
                SelectedPerson = DataHelper.GetPersonWithoutFinancials(personId);

                PersonChanged(this,
                              new PersonChangedEventArguments(
                                  new Person
                                      {
                                          Id = personId
                                      }));
            }
            else
            {
                ddlPersons.SelectedValue = SelectedPerson.Id.Value.ToString();
            }
        }
    }
}

