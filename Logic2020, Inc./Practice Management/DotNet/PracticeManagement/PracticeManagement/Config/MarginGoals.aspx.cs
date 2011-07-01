using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.ClientService;
using System.ServiceModel;
using PraticeManagement.Controls;
using PraticeManagement.ConfigurationService;
using PraticeManagement.Utils;

namespace PraticeManagement.Config
{
    public partial class MarginGoals : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        private const string ClientGoalDefault_THRESHOLDS_LIST_KEY = "CLIENTGOALDEFAULT_THRESHOLDS_LIST_KEY";
        private const string gvClientddlStartRange = "gvClientddlStartRange";
        private const string gvClientddlEndRange = "gvClientddlEndRange";
        private const string gvClientddlColor = "gvClientddlColor";

        private const string gvPersonddlStartRange = "gvPersonddlStartRange";
        private const string gvPersonddlEndRange = "gvPersonddlEndRange";
        private const string gvPersonddlColor = "gvPersonddlColor";
        private const string PersonMarginColorInfo_THRESHOLDS_LIST_KEY = "PERSONMARGINCOLORINFO_THRESHOLDS_LIST_KEY";

        #endregion


        private List<ClientMarginColorInfo> ClientGoalDefaultMarginColorInfoList
        {
            get
            {
                if (ViewState[ClientGoalDefault_THRESHOLDS_LIST_KEY] != null)
                {
                    var output = ViewState[ClientGoalDefault_THRESHOLDS_LIST_KEY] as List<ClientMarginColorInfo>;
                    return output;
                }
                else
                {
                    var result = SettingsHelper.GetMarginColorInfoDefaults(DefaultGoalType.Client);

                    if (result != null)
                    {
                        return result;
                    }
                    var cmci = new List<ClientMarginColorInfo>();
                    cmci.Add(new ClientMarginColorInfo() { ColorInfo = new ColorInformation() });
                    ViewState[ClientGoalDefault_THRESHOLDS_LIST_KEY] = cmci;
                    return cmci;
                }
            }
            set { ViewState[ClientGoalDefault_THRESHOLDS_LIST_KEY] = value; }
        }

        private List<ClientMarginColorInfo> PersonMarginColorInfoList
        {
            get
            {
                if (ViewState[PersonMarginColorInfo_THRESHOLDS_LIST_KEY] != null)
                {
                    var output = ViewState[PersonMarginColorInfo_THRESHOLDS_LIST_KEY] as List<ClientMarginColorInfo>;
                    return output;
                }
                else
                {
                    var result = SettingsHelper.GetMarginColorInfoDefaults(DefaultGoalType.Person);

                    if (result != null)
                    {
                        return result;
                    }

                    var cmci = new List<ClientMarginColorInfo>();
                    cmci.Add(new ClientMarginColorInfo() { ColorInfo = new ColorInformation() });
                    ViewState[PersonMarginColorInfo_THRESHOLDS_LIST_KEY] = cmci;
                    return cmci;
                }
            }
            set { ViewState[PersonMarginColorInfo_THRESHOLDS_LIST_KEY] = value; }
        }

        protected override void Display()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateControls();
            }
            mlConfirmation.ClearMessage();
        }

        private void PopulateControls()
        {
            chbClientGoalDefaultThreshold.Checked = Convert.ToBoolean(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllClientsKey));
            chbPersonMarginThresholds.Checked = Convert.ToBoolean(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllPersonsKey));
            DataBindClientThresholds(ClientGoalDefaultMarginColorInfoList);
            DataBindPersonThresholds(PersonMarginColorInfoList);
            EnableorDisableClientThrsholdControls(chbClientGoalDefaultThreshold.Checked);
            EnableorDisablePersonThrsholdControls(chbPersonMarginThresholds.Checked);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            EnableOrDisablebtnClientGoalDefaultAddThreshold();
            EnableOrDisablebtnPersonAddThreshold();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            GetLatestMarginInfoValues();
            Page.Validate(vsumClient.ValidationGroup);
            if (Page.IsValid)
            {
                SaveData();
                SettingsHelper.RemoveMarginColorInfoDefaults();
                ClearDirty();
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Margin Goals"));
                ViewState.Remove(ClientGoalDefault_THRESHOLDS_LIST_KEY);
                ViewState.Remove(PersonMarginColorInfo_THRESHOLDS_LIST_KEY);

                var mainDictionary = HttpContext.Current.Cache["ApplicationSettings"] as Dictionary<SettingsType, Dictionary<string, string>>;

                if (mainDictionary != null && mainDictionary[SettingsType.Application] != null)
                {
                    mainDictionary.Remove(SettingsType.Application);
                }

                PopulateControls();
            }
        }

        private void SaveData()
        {
            using (var serviceClient = new ConfigurationServiceClient())
            {
                try
                {
                    string clientValue = chbClientGoalDefaultThreshold.Checked.ToString();
                    var x = new Triple<SettingsType, string, string>(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllClientsKey, clientValue);

                    string personValue = chbPersonMarginThresholds.Checked.ToString();
                    var y = new Triple<SettingsType, string, string>(SettingsType.Application, Constants.ResourceKeys.IsDefaultMarginInfoEnabledForAllPersonsKey, personValue);

                    var list = new List<Triple<DefaultGoalType, Triple<SettingsType, string, string>, ClientMarginColorInfo[]>>();

                    list.Add(new Triple<DefaultGoalType, Triple<SettingsType, string, string>, ClientMarginColorInfo[]>(DefaultGoalType.Client, x, ClientGoalDefaultMarginColorInfoList.AsQueryable().ToArray()));
                    list.Add(new Triple<DefaultGoalType, Triple<SettingsType, string, string>, ClientMarginColorInfo[]>(DefaultGoalType.Person, y, PersonMarginColorInfoList.AsQueryable().ToArray()));

                    serviceClient.SaveMarginInfoDetail(list.AsQueryable().ToArray());
                }
                catch (Exception ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private void FillRangeDropdown(DropDownList ddlRange)
        {
            ddlRange.Items.Clear();

            for (int i = 0; i < 151; i++)
            {
                ddlRange.Items.Add(
                                        new ListItem()
                                        {
                                            Text = string.Format("{0}%", i.ToString()),
                                            Value = i.ToString()
                                        }
                                  );
            }

        }

        private void GetLatestMarginInfoValues()
        {
            GetLatestClientGoalDefaultMarginInfoValues();
            GetLatestPersonGoalMarginInfoValues();
        }




        protected void chbClientGoalDefaultThreshold_OnCheckedChanged(object sender, EventArgs e)
        {
            GetLatestClientGoalDefaultMarginInfoValues();
            EnableorDisableClientThrsholdControls(chbClientGoalDefaultThreshold.Checked);
            DataBindClientThresholds(ClientGoalDefaultMarginColorInfoList);
        }

        private void DataBindClientThresholds(List<ClientMarginColorInfo> clientMarginColorInfoList)
        {
            gvClientGoalDefaultThreshold.DataSource = clientMarginColorInfoList;
            gvClientGoalDefaultThreshold.DataBind();
        }

        private void EnableorDisableClientThrsholdControls(bool ischbMarginThresholdsChecked)
        {
            cvClientThresholds.Enabled = btnClientGoalDefaultAddThreshold.Enabled = gvClientGoalDefaultThreshold.Enabled = ischbMarginThresholdsChecked;
        }

        protected void btnClientGoalDefaultAddThreshold_OnClick(object sender, EventArgs e)
        {
            GetLatestClientGoalDefaultMarginInfoValues();
            var clientMarginColorInfo = new ClientMarginColorInfo();
            clientMarginColorInfo.ColorInfo = new ColorInformation();

            int end = ClientGoalDefaultMarginColorInfoList.Max(m => m.EndRange);
            if (end != 150)
            {
                end = end + 1;
            }
            clientMarginColorInfo.StartRange = end;
            clientMarginColorInfo.EndRange = end;


            ClientGoalDefaultMarginColorInfoList.Add(clientMarginColorInfo);
            DataBindClientThresholds(ClientGoalDefaultMarginColorInfoList);

        }

        protected void btnClientDeleteRow_OnClick(object sender, EventArgs e)
        {
            GetLatestClientGoalDefaultMarginInfoValues();
            ImageButton imgDelete = sender as ImageButton;
            GridViewRow gvRow = imgDelete.NamingContainer as GridViewRow;
            ClientGoalDefaultMarginColorInfoList.RemoveAt(gvRow.RowIndex);

            if (gvClientGoalDefaultThreshold.Rows.Count == 1)
            {
                var cmci = new List<ClientMarginColorInfo>();
                cmci.Add(new ClientMarginColorInfo() { ColorInfo = new ColorInformation() });
                ClientGoalDefaultMarginColorInfoList = cmci;
            }

            DataBindClientThresholds(ClientGoalDefaultMarginColorInfoList);
        }

        protected void gvClientGoalDefaultThreshold_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
            var clientMarginIfo = e.Row.DataItem as ClientMarginColorInfo;

            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) == 0)
            {
                DropDownList ddlSR = e.Row.FindControl(gvClientddlStartRange) as DropDownList;
                DropDownList ddlER = e.Row.FindControl(gvClientddlEndRange) as DropDownList;
                DropDownList ddcolor = e.Row.FindControl(gvClientddlColor) as DropDownList;

                FillRangeDropdown(ddlSR);
                FillRangeDropdown(ddlER);
                DataHelper.FillColorsList(ddcolor, string.Empty);

                ddlSR.SelectedValue = clientMarginIfo.StartRange.ToString();
                ddlER.SelectedValue = clientMarginIfo.EndRange.ToString();

                if (clientMarginIfo.ColorInfo.ColorId != 0)
                {
                    ddcolor.SelectedValue = clientMarginIfo.ColorInfo.ColorId.ToString();
                    ddcolor.Style["background-color"] = clientMarginIfo.ColorInfo.ColorValue;
                }
                else
                {
                    ddcolor.SelectedValue = string.Empty;
                    ddcolor.Style["background-color"] = "White";
                }
            }
        }

        protected void cvClientColors_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (chbClientGoalDefaultThreshold.Checked)
            {
                int i = 0;
                foreach (var item in ClientGoalDefaultMarginColorInfoList)
                {
                    if (ClientGoalDefaultMarginColorInfoList.Any(c => c.ColorInfo.ColorId == ClientGoalDefaultMarginColorInfoList[i].ColorInfo.ColorId && c != item && c.ColorInfo.ColorId != 0))
                    {
                        args.IsValid = false;
                        break;
                    }
                    i++;
                }
            }

        }

        protected void cvClientThresholds_ServerValidate(object source, ServerValidateEventArgs args)
        {

            args.IsValid = true;

            if (chbClientGoalDefaultThreshold.Checked)
            {
                if (ClientGoalDefaultMarginColorInfoList != null && ClientGoalDefaultMarginColorInfoList.Count > 0)
                {
                    int start = ClientGoalDefaultMarginColorInfoList.Min(m => m.StartRange);
                    int end = ClientGoalDefaultMarginColorInfoList.Max(m => m.EndRange);
                    if (start != 0 || end < 100)
                    {
                        args.IsValid = false;
                    }
                    else
                    {
                        var temp = ClientGoalDefaultMarginColorInfoList.OrderBy(k => k.StartRange).ToList();
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (i + 1 != temp.Count)
                            {
                                if (temp[i].EndRange + 1 != temp[i + 1].StartRange)
                                {
                                    args.IsValid = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    args.IsValid = false;
                }
            }
        }

        protected void cvgvClientddlColor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator cvcolor = source as CustomValidator;
            GridViewRow row = cvcolor.NamingContainer as GridViewRow;
            DropDownList ddcolor = row.FindControl(gvClientddlColor) as DropDownList;

            args.IsValid = true;
            if (chbClientGoalDefaultThreshold.Checked)
            {
                if (ddcolor.SelectedIndex == 0)
                {
                    args.IsValid = false;
                    cvgvddlColorClone.IsValid = false;
                }
            }
        }

        protected void cvgvClientRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            if (chbClientGoalDefaultThreshold.Checked)
            {
                CustomValidator cvgvRange = source as CustomValidator;
                GridViewRow row = cvgvRange.NamingContainer as GridViewRow;
                DropDownList ddlSR = row.FindControl(gvClientddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvClientddlEndRange) as DropDownList;

                args.IsValid = true;
                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                if (start > end)
                {
                    args.IsValid = false;
                    cvgvRangeClone.IsValid = false;
                }
            }
        }

        protected void cvgvClientOverLapRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (chbClientGoalDefaultThreshold.Checked)
            {
                CustomValidator cvgvOverLapRange = source as CustomValidator;
                GridViewRow row = cvgvOverLapRange.NamingContainer as GridViewRow;

                DropDownList ddlSR = row.FindControl(gvClientddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvClientddlEndRange) as DropDownList;

                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                if (ClientGoalDefaultMarginColorInfoList != null)
                {
                    List<ClientMarginColorInfo> cmciList = new List<ClientMarginColorInfo>();
                    for (int i = 0; i < ClientGoalDefaultMarginColorInfoList.Count; i++)
                    {
                        if (i != row.RowIndex)
                        {
                            cmciList.Add(ClientGoalDefaultMarginColorInfoList[i]);
                        }

                    }

                    if (cmciList.Any(k => k.StartRange >= start && k.StartRange <= end))
                    {
                        args.IsValid = false;
                        cvgvOverLapRangeClone.IsValid = false;
                    }
                }
            }

        }

        private void GetLatestClientGoalDefaultMarginInfoValues()
        {
            while (ClientGoalDefaultMarginColorInfoList.Count > 0)
            {
                ClientGoalDefaultMarginColorInfoList.RemoveAt(0);
            }

            foreach (GridViewRow row in gvClientGoalDefaultThreshold.Rows)
            {
                DropDownList ddlSR = row.FindControl(gvClientddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvClientddlEndRange) as DropDownList;
                DropDownList ddcolor = row.FindControl(gvClientddlColor) as DropDownList;

                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                int colorId = Convert.ToInt32(ddcolor.SelectedValue);
                string colorValue = ddcolor.SelectedItem.Attributes["colorValue"];
                string colorDescription = ddcolor.SelectedItem.Attributes["Description"];
                ClientGoalDefaultMarginColorInfoList.Add(
                    new ClientMarginColorInfo()
                    {
                        ColorInfo = new ColorInformation()
                        {
                            ColorId = colorId,
                            ColorValue = colorValue,
                            ColorDescription = colorDescription

                        },
                        StartRange = start,
                        EndRange = end
                    });

            }
        }

        private void EnableOrDisablebtnClientGoalDefaultAddThreshold()
        {
            if (gvClientGoalDefaultThreshold.Rows.Count == 5)
            {
                btnClientGoalDefaultAddThreshold.Enabled = false;
            }
            else if (chbClientGoalDefaultThreshold.Checked)
            {
                btnClientGoalDefaultAddThreshold.Enabled = true;
            }

        }





        private void EnableOrDisablebtnPersonAddThreshold()
        {
            if (gvPersonThrsholds.Rows.Count == 5)
            {
                btnPersonAddThreshold.Enabled = false;
            }
            else if (chbPersonMarginThresholds.Checked)
            {
                btnPersonAddThreshold.Enabled = true;
            }
        }

        private void GetLatestPersonGoalMarginInfoValues()
        {
            while (PersonMarginColorInfoList.Count > 0)
            {
                PersonMarginColorInfoList.RemoveAt(0);
            }

            foreach (GridViewRow row in gvPersonThrsholds.Rows)
            {
                DropDownList ddlSR = row.FindControl(gvPersonddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvPersonddlEndRange) as DropDownList;
                DropDownList ddcolor = row.FindControl(gvPersonddlColor) as DropDownList;

                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                int colorId = Convert.ToInt32(ddcolor.SelectedValue);
                string colorValue = ddcolor.SelectedItem.Attributes["colorValue"];
                string colorDescription = ddcolor.SelectedItem.Attributes["Description"];
                PersonMarginColorInfoList.Add(
                    new ClientMarginColorInfo()
                    {
                        ColorInfo = new ColorInformation()
                        {
                            ColorId = colorId,
                            ColorValue = colorValue,
                            ColorDescription = colorDescription

                        },
                        StartRange = start,
                        EndRange = end
                    });

            }
        }

        protected void cvPersonColors_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (chbPersonMarginThresholds.Checked)
            {
                int i = 0;
                foreach (var item in PersonMarginColorInfoList)
                {
                    if (PersonMarginColorInfoList.Any(c => c.ColorInfo.ColorId == PersonMarginColorInfoList[i].ColorInfo.ColorId && c != item && c.ColorInfo.ColorId != 0))
                    {
                        args.IsValid = false;
                        break;
                    }
                    i++;
                }
            }

        }

        protected void cvPersonThresholds_ServerValidate(object source, ServerValidateEventArgs args)
        {

            args.IsValid = true;

            if (chbPersonMarginThresholds.Checked)
            {
                if (PersonMarginColorInfoList != null && PersonMarginColorInfoList.Count > 0)
                {
                    int start = PersonMarginColorInfoList.Min(m => m.StartRange);
                    int end = PersonMarginColorInfoList.Max(m => m.EndRange);
                    if (start != 0 || end < 100)
                    {
                        args.IsValid = false;
                    }
                    else
                    {
                        var temp = PersonMarginColorInfoList.OrderBy(k => k.StartRange).ToList();
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (i + 1 != temp.Count)
                            {
                                if (temp[i].EndRange + 1 != temp[i + 1].StartRange)
                                {
                                    args.IsValid = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    args.IsValid = false;
                }
            }
        }

        protected void cvgvPersonddlColor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator cvcolor = source as CustomValidator;
            GridViewRow row = cvcolor.NamingContainer as GridViewRow;
            DropDownList ddcolor = row.FindControl(gvPersonddlColor) as DropDownList;

            args.IsValid = true;
            if (chbPersonMarginThresholds.Checked)
            {
                if (ddcolor.SelectedIndex == 0)
                {
                    args.IsValid = false;
                    cvgvddlColorClone.IsValid = false;
                }
            }
        }

        protected void cvgvPersonRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            if (chbPersonMarginThresholds.Checked)
            {
                CustomValidator cvgvRange = source as CustomValidator;
                GridViewRow row = cvgvRange.NamingContainer as GridViewRow;
                DropDownList ddlSR = row.FindControl(gvPersonddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvPersonddlEndRange) as DropDownList;

                args.IsValid = true;
                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                if (start > end)
                {
                    args.IsValid = false;
                    cvgvRangeClone.IsValid = false;
                }
            }
        }

        protected void cvgvPersonOverLapRange_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (chbPersonMarginThresholds.Checked)
            {
                CustomValidator cvgvOverLapRange = source as CustomValidator;
                GridViewRow row = cvgvOverLapRange.NamingContainer as GridViewRow;

                DropDownList ddlSR = row.FindControl(gvPersonddlStartRange) as DropDownList;
                DropDownList ddlER = row.FindControl(gvPersonddlEndRange) as DropDownList;

                int start = Convert.ToInt32(ddlSR.SelectedValue);
                int end = Convert.ToInt32(ddlER.SelectedValue);
                if (PersonMarginColorInfoList != null)
                {
                    List<ClientMarginColorInfo> cmciList = new List<ClientMarginColorInfo>();
                    for (int i = 0; i < PersonMarginColorInfoList.Count; i++)
                    {
                        if (i != row.RowIndex)
                        {
                            cmciList.Add(PersonMarginColorInfoList[i]);
                        }

                    }

                    if (cmciList.Any(k => k.StartRange >= start && k.StartRange <= end))
                    {
                        args.IsValid = false;
                        cvgvOverLapRangeClone.IsValid = false;
                    }
                }
            }

        }

        protected void gvPersonThrsholds_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }
            var clientMarginIfo = e.Row.DataItem as ClientMarginColorInfo;

            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) == 0)
            {
                DropDownList ddlSR = e.Row.FindControl(gvPersonddlStartRange) as DropDownList;
                DropDownList ddlER = e.Row.FindControl(gvPersonddlEndRange) as DropDownList;
                DropDownList ddcolor = e.Row.FindControl(gvPersonddlColor) as DropDownList;

                FillRangeDropdown(ddlSR);
                FillRangeDropdown(ddlER);
                DataHelper.FillColorsList(ddcolor, string.Empty);

                ddlSR.SelectedValue = clientMarginIfo.StartRange.ToString();
                ddlER.SelectedValue = clientMarginIfo.EndRange.ToString();

                if (clientMarginIfo.ColorInfo.ColorId != 0)
                {
                    ddcolor.SelectedValue = clientMarginIfo.ColorInfo.ColorId.ToString();
                    ddcolor.Style["background-color"] = clientMarginIfo.ColorInfo.ColorValue;
                }
                else
                {
                    ddcolor.SelectedValue = string.Empty;
                    ddcolor.Style["background-color"] = "White";
                }
            }
        }

        protected void btnPersonAddThreshold_OnClick(object sender, EventArgs e)
        {
            GetLatestPersonGoalMarginInfoValues();
            var clientMarginColorInfo = new ClientMarginColorInfo();
            clientMarginColorInfo.ColorInfo = new ColorInformation();

            int end = PersonMarginColorInfoList.Max(m => m.EndRange);
            if (end != 150)
            {
                end = end + 1;
            }
            clientMarginColorInfo.StartRange = end;
            clientMarginColorInfo.EndRange = end;


            PersonMarginColorInfoList.Add(clientMarginColorInfo);
            DataBindPersonThresholds(PersonMarginColorInfoList);

        }

        protected void btnPersonDeleteRow_OnClick(object sender, EventArgs e)
        {
            GetLatestPersonGoalMarginInfoValues();
            ImageButton imgDelete = sender as ImageButton;
            GridViewRow gvRow = imgDelete.NamingContainer as GridViewRow;
            PersonMarginColorInfoList.RemoveAt(gvRow.RowIndex);

            if (gvPersonThrsholds.Rows.Count == 1)
            {
                var cmci = new List<ClientMarginColorInfo>();
                cmci.Add(new ClientMarginColorInfo() { ColorInfo = new ColorInformation() });
                PersonMarginColorInfoList = cmci;
            }

            DataBindPersonThresholds(PersonMarginColorInfoList);
        }

        protected void chbPersonMarginThresholds_OnCheckedChanged(object sender, EventArgs e)
        {
            GetLatestPersonGoalMarginInfoValues();
            EnableorDisablePersonThrsholdControls(chbPersonMarginThresholds.Checked);
            DataBindPersonThresholds(PersonMarginColorInfoList);
        }

        private void DataBindPersonThresholds(List<ClientMarginColorInfo> personMarginColorInfoList)
        {
            gvPersonThrsholds.DataSource = personMarginColorInfoList;
            gvPersonThrsholds.DataBind();
        }

        private void EnableorDisablePersonThrsholdControls(bool ischbPersonMarginThresholdsChecked)
        {
            cvPersonThresholds.Enabled = btnPersonAddThreshold.Enabled = gvPersonThrsholds.Enabled = ischbPersonMarginThresholdsChecked;
        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            GetLatestMarginInfoValues();
            Page.Validate(vsumClient.ValidationGroup);
            if (Page.IsValid)
            {
                SaveData();
                this.ClearDirty();
                Redirect(eventArgument);
            }
        }

        #endregion

    }
}
