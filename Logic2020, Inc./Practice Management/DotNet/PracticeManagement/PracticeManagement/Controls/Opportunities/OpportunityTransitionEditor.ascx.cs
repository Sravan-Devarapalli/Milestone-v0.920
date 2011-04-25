using System;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls.Opportunities
{
    public partial class OpportunityTransitionEditor : PracticeManagementUserControl
    {
        private const string ViewStateTransitionEnabled = "ViewStateTransitionEnabled";
        private const string ViewStateOpportunityId = "OpportunityId";
        private const string ViewStateTransitionStatus = "TransitionStatus";

        protected void Page_Load(object sender, EventArgs e)
        {
            odsTransitions.SelectParameters["statusType"].DefaultValue = TransitionStatus.ToString();
            odsTransitions.SelectParameters["opportunityId"].DefaultValue = OpportunityId.ToString();
        }

        public OpportunityTransitionStatusType? TransitionStatus
        {
            get { return GetViewStateValue<OpportunityTransitionStatusType?>(ViewStateTransitionStatus, null); }
            set { SetViewStateValue(ViewStateTransitionStatus, value); }
        }

        public int OpportunityId
        {
            get { return GetViewStateValue(ViewStateOpportunityId, int.MinValue); }
            set { SetViewStateValue(ViewStateOpportunityId, value); }
        }

        public bool Enabled
        {
            get { return GetViewStateValue(ViewStateTransitionEnabled, true); }
            set
            {
                SetViewStateValue(ViewStateTransitionEnabled, value);
                EnableControl(value);
            }
        }

        protected void EnableControl(bool enable)
        {
            ddlPersons.Enabled = 
                lbTransistions.Enabled = enable;
        }

        protected void ddlPersons_OnSelectedIndexChanged(object sender, EventArgs e)
        {
             Boolean dupeCheck=false;
            if (ddlPersons.SelectedValue != string.Empty)
            {
                var transition = InitTransition();
                // dupeCheck
               foreach (var item in lbTransistions.Items)
               {
                   if (item.ToString() == ddlPersons.SelectedItem.Text)
                   {
                       dupeCheck= true;
                       break;
                   }
               }
               if (!dupeCheck)
               {
                   //  Insert into the DB
                   var transitionId =
                       ServiceCallers.Custom.Opportunity(
                           client => client.OpportunityTransitionInsert(transition));
                   //  Insert into the drop-down
                   lbTransistions.Items.Insert(0, new ListItem
                                                      {
                                                          Text = ddlPersons.SelectedItem.Text,
                                                          Value = transitionId.ToString()
                                                      });
               }
               ddlPersons.SelectedIndex = 0;
            }
        }

        private OpportunityTransition InitTransition()
        {
            return new OpportunityTransition
                       {
                           Opportunity = new Opportunity{Id = OpportunityId},
                           OpportunityTransitionStatus =
                               OpportunityTransitionStatus.FromType(TransitionStatus.Value),
                           Person = new Person
                                        {
                                            Id = DataHelper.CurrentPerson.Id
                                        },
                           TargetPerson = new Person
                                              {
                                                  Id = Convert.ToInt32(ddlPersons.SelectedValue)
                                              }
                       };
        }

        protected void lbTransistions_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            // Remove from the DB
            var transitionId = Convert.ToInt32(lbTransistions.SelectedValue);
            ServiceCallers.Custom.Opportunity(client => client.OpportunityTransitionDelete(transitionId));

            // Remove from drop-down
            lbTransistions.Items.RemoveAt(lbTransistions.SelectedIndex);
        }
    }
}
