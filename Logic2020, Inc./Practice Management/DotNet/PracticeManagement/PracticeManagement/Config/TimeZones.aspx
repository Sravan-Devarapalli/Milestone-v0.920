﻿<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="TimeZones.aspx.cs" Inherits="PraticeManagement.Config.TimeZones" %>

<asp:content id="Content1" contentplaceholderid="title" runat="server">
    <title>Practice Management - Time Zone</title>
</asp:content>
<asp:content id="Content2" contentplaceholderid="head" runat="server">
</asp:content>
<asp:content id="Content3" contentplaceholderid="header" runat="server">
    Time Zone
</asp:content>
<asp:content id="Content4" contentplaceholderid="body" runat="server">
    <table width="50%" style="padding: 5px;">
        <tr>
            <td>
                <asp:dropdownlist id="ddlTimeZones" runat="server" datasourceid="odsTimezones" onchange="setDirty()"
                    datatextfield="GMTName" datavaluefield="GMT" width="100%">
                </asp:dropdownlist>
            </td>
            <td style="padding-left:10px;">
                <asp:button id="btnSetTimeZone" runat="server" onclick="btnSetTimeZone_Clicked" text="Set TimeZone" />
            </td>
        </tr>
        <tr>
            <td>
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

