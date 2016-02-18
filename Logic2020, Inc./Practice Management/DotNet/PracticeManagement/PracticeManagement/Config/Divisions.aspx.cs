using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;

namespace PraticeManagement.Config
{
    public partial class Divisions : PracticeManagementPageBase
    {
        //Constants
        private const string Divisions_KEY = "DivisionsList";
        private const string DivisionsOwner_KEY = "DivisionOwnersList";

        public List<PersonDivision> DivisionsList
        {
            get
            {
                if (ViewState[Divisions_KEY] == null)
                {
                    ViewState[Divisions_KEY] = ServiceCallers.Custom.Person(p => p.GetPersonDivisions()).ToList();
                }

                return (List<PersonDivision>)ViewState[Divisions_KEY];
            }
            set { ViewState[Divisions_KEY] = value; }
        }

        public List<Person> DivisionOwners
        {
            get
            {
                if (ViewState[DivisionsOwner_KEY] == null)
                {
                    string statusIds = ((int)DataTransferObjects.PersonStatusType.Active).ToString();
                    string paytypeIds = ((int)TimescaleType.Salary).ToString();
                    ViewState[DivisionsOwner_KEY] = ServiceCallers.Custom.Person(p => p.GetPersonsByPayTypesAndByStatusIds(statusIds, paytypeIds)).OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToList();
                }

                return (List<Person>)ViewState[DivisionsOwner_KEY];
            }
            set { ViewState[DivisionsOwner_KEY] = value; }
        }

        public ListItem[] DivisionOwnersList
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<ListItem> divisionOwnersList = new List<ListItem>();
            foreach (var item in DivisionOwners)
            {
                divisionOwnersList.Add(new ListItem() { Text = item.PersonLastFirstName, Value = item.Id.Value.ToString() });
            }
            DivisionOwnersList = divisionOwnersList.ToArray();
        }

        protected override void Display()
        {
            gvDivisions.DataSource = DivisionsList;
            gvDivisions.DataBind();
        }

        protected void gvDivisions_RowDataBind(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var item = e.Row.DataItem as PersonDivision;
            if (item != null)
            {
                var lblDivisionOwner = e.Row.FindControl("lblDivisionOwner") as Label;
                if (lblDivisionOwner != null)
                {
                    lblDivisionOwner.Text = item.DivisionOwner == null ? "Unassigned" : item.DivisionOwner.PersonLastFirstName;
                }
            }

            // Edit mode.
            if ((e.Row.RowState & DataControlRowState.Edit) != 0)
            {


                DropDownList ddlActivePersons = e.Row.FindControl("ddlActivePersons") as DropDownList;
                if (ddlActivePersons != null)
                {
                    ddlActivePersons.Items.AddRange(DivisionOwnersList);
                    string id = item.DivisionOwnerId.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    if (ddlActivePersons.Items.FindByValue(id) != null)
                    {
                        ddlActivePersons.SelectedValue = id;
                    }
                    else
                    {
                        // Inactive owner.
                        ddlActivePersons.SelectedIndex = 0;
                    }
                }
            }

        }

        protected void imgEdit_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var gvDivisionItem = imgEdit.NamingContainer as GridViewRow;
            gvDivisions.EditIndex = gvDivisionItem.DataItemIndex;
            gvDivisions.DataSource = DivisionsList;
            gvDivisions.DataBind();
        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            gvDivisions.EditIndex = -1;
            gvDivisions.DataSource = DivisionsList;
            gvDivisions.DataBind();
        }

        protected void imgUpdate_OnClick(object sender, EventArgs e)
        {
            var imgEdit = sender as ImageButton;
            var row = imgEdit.NamingContainer as GridViewRow;
            int divisionId = int.Parse(((HiddenField)row.FindControl("hdDivisionId")).Value);
            var ddlActivePersons = row.FindControl("ddlActivePersons") as DropDownList;
            PersonDivision oldDivision = DivisionsList.Find(d=>d.DivisionId==divisionId);
            PersonDivision newdivision = new PersonDivision();
            newdivision.DivisionId = divisionId;
            newdivision.DivisionName = oldDivision.DivisionName;
            newdivision.DivisionOwner = new Person() { Id = int.Parse(ddlActivePersons.SelectedValue) };
            //update division
            ServiceCallers.Custom.Person(p => p.UpdatePersonDivision(newdivision));
            DivisionsList = null;
            gvDivisions.EditIndex = -1;
            gvDivisions.DataSource = DivisionsList;
            gvDivisions.DataBind();
        }
    }
}
