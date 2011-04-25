using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using DataTransferObjects;

namespace PraticeManagement.Controls.Persons
{
    public partial class PersonChooser : System.Web.UI.UserControl
    {
        #region Constants

        private const string SELECTED_PERSON_ID = "SelectedPersonId";

        #endregion

        #region Events

        public delegate void PersonChangedHandler(object sender, PersonChangedEventArguments args);
        public event PersonChangedHandler PersonChanged;

        #endregion

        #region Properties

        public int SelectedPersonId
        {
            get
            {
                try
                {
                    return (int)ViewState[SELECTED_PERSON_ID];
                }
                catch
                {
                    return -1;
                }
            }

            set
            {
                ViewState[SELECTED_PERSON_ID] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bool userIsAdministrator =
                    Roles.IsUserInRole(
                        DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

                //  Get currently logged in person
                Person currentPerson = DataHelper.CurrentPerson;

                // Set it's Id value
                int personId = currentPerson.Id.Value;
                SelectedPersonId = personId;

                if (userIsAdministrator)
                {
                    DataHelper.FillPersonList(ddlPersons, null);
                    ddlPersons.Items.RemoveAt(0);
                    ddlPersons.SelectedValue =
                        personId.ToString();
                }
                else
                {
                    ddlPersons.Visible = false;
                    lblTip.Text =
                        string.Format(
                            Resources.Controls.TE_SelectedPerson,
                            currentPerson.ToString());
                }
            }
        }

        protected void ddlPersons_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPersonChanged();
        }

        protected virtual void OnPersonChanged()
        {
            int pId = Convert.ToInt32(ddlPersons.SelectedValue);

            SelectedPersonId = pId;

            PersonChanged(this, 
                new PersonChangedEventArguments(
                    new Person { 
                        Id = pId
                    }));
        }
    }
}
