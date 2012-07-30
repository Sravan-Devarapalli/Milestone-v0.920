<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="TimeZones.aspx.cs" Inherits="PraticeManagement.Config.TimeZones" %>

<asp:content id="Content1" contentplaceholderid="title" runat="server">
    <title>Time Zone | Practice Management</title>
</asp:content>
<asp:content id="Content2" contentplaceholderid="head" runat="server">
</asp:content>
<asp:content id="Content3" contentplaceholderid="header" runat="server">
    Time Zone
</asp:content>
<asp:content id="Content4" contentplaceholderid="body" runat="server">
    <table class="Width50Percent Padding5">
        <tr>
            <td>
                <asp:dropdownlist id="ddlTimeZones" runat="server" datasourceid="odsTimezones" onchange="setDirty()"
                    datatextfield="GMTName" datavaluefield="GMT" CssClass="WholeWidth">
                </asp:dropdownlist>
            </td>
            <td class="padLeft10">
                <asp:button id="btnSetTimeZone" runat="server" onclick="btnSetTimeZone_Clicked" text="Set TimeZone" />
            </td>
        </tr>
        <tr>
            <td class="PaddingTop10 PaddingBottom5">
                <asp:CheckBox id="cbIsDayLightSavingsTimeEffect" runat="server" Text="Daylight Savings Time is in effect for this location" ></asp:CheckBox>
            </td>
        </tr>
    </table>
    <asp:label id="successMessage" runat="server" forecolor="green"></asp:label>
    <asp:label id="errorMessage" runat="server" forecolor="red"></asp:label>
    <asp:objectdatasource id="odsTimezones" runat="server" typename="PraticeManagement.TimeEntryService.TimeEntryServiceClient"
        selectmethod="TimeZonesAll" onselected="odsTimeZones_Selected" dataobjecttypename="DataTransferObjects.Timezone">
    </asp:objectdatasource>
</asp:content>
<asp:content id="Content5" contentplaceholderid="footer" runat="server">
</asp:content>

