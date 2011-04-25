using System;
using System.Web.Security;
using System.Web.UI;
using DataTransferObjects;

namespace PraticeManagement.Controls.Persons
{
    public partial class PersonChooser : UserControl
    {
        #region Constants

        private const string SELECTED_PERSON_ID = "SelectedPersonId";

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
            get { return (Person) ViewState[SELECTED_PERSON_ID]; }
            set { ViewState[SELECTED_PERSON_ID] = value; }
        }

        public int SelectedPersonId
        {
            get { return SelectedPerson.Id.Value; }
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

                if (userIsAdministrator)
                {
                    DataHelper.FillPersonList(ddlPersons, null);
                    ddlPersons.Items.RemoveAt(0);

                    string strSelectedPersonId = Request.QueryString["SelectedPersonId"];
                    if (!string.IsNullOrEmpty(strSelectedPersonId) && !Int32.TryParse(strSelectedPersonId, out personId))
                    {
                        personId = currentPerson.Id.Value;
                    }
                    ddlPersons.SelectedValue =
                        personId.ToString();
                }
                else
                {
                    ddlPersons.Visible = false;
                    lblTip.Text =
                        string.Format(
                            Resources.Controls.TE_SelectedPerson,
                            currentPerson);
                }

                SelectedPerson = personId == currentPerson.Id.Value ? currentPerson:DataHelper.GetPerson(personId);
            }
        }

        protected void ddlPersons_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPersonChanged(Convert.ToInt32(ddlPersons.SelectedValue));
        }

        protected virtual void OnPersonChanged(int personId)
        {
            SelectedPerson = DataHelper.GetPersonWithoutFinancials(personId);

            PersonChanged(this,
                          new PersonChangedEventArguments(
                              new Person
                                  {
                                      Id = personId
                                  }));
        }
    }
}
