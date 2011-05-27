using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.Utils;

namespace PraticeManagement.Config
{
    public partial class TimeZones : PracticeManagementPageBase, IPostBackEventHandler
    {
        private static string _timezone;

        protected void Page_Load(object sender, EventArgs e)
        {
            successMessage.Text = string.Empty;
            errorMessage.Text = string.Empty;
        }

        protected override void Display()
        { }

        protected void btnSetTimeZone_Clicked(object sender, EventArgs e)
        {
            if (SaveTimeZone())
            {
                successMessage.Text = "TimeZone Set successfully.";
            }
        }

        private bool SaveTimeZone()
        {
            Timezone timezone = new Timezone();
            timezone.GMT = ddlTimeZones.SelectedValue;

            try
            {
                using (var serviceClient = new TimeEntryService.TimeEntryServiceClient())
                {
                    serviceClient.SetTimeZone(timezone);
                }

                SettingsHelper.SaveResourceKeyValuePairItem(SettingsType.Application, Constants.ResourceKeys.TimeZoneKey, timezone.GMT);

                ClearDirty();
                return true;
            }
            catch(Exception e)
            {
                errorMessage.Text = e.Message;
                return false;
            }
        }

        protected void odsTimeZones_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            SelectTimeZone(e.ReturnValue as IEnumerable<Timezone>);
        }

        private void SelectTimeZone(IEnumerable<Timezone> timezones)
        {
            if (timezones != null)
            {
                foreach (Timezone timezone in timezones)
                {
                    if (timezone.IsActive)
                    {
                        _timezone = timezone.GMT;
                        ddlTimeZones.SelectedValue = timezone.GMT;
                        break;
                    }
                }
            }
        }


        #region IPostback Event Handler

        public void RaisePostBackEvent(string eventArgument)
        {
            if (SaveTimeZone())
            {
                Redirect(eventArgument);
            }
        }

        #endregion
    }
}
