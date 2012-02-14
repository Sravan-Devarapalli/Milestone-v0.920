<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdministrativeTimeEntryBar.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.AdministrativeTimeEntryBar" %>
<%@ Register Src="~/Controls/TimeEntry/SingleTimeEntry_New.ascx" TagName="SingleTE"
    TagPrefix="te" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="ext2" Namespace="PraticeManagement.Controls.Generic.EnableDisableExtender"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="PraticeManagement.Controls.TimeEntry" %>
<%@ Register TagPrefix="extS" Namespace="PraticeManagement.Controls.Generic.SelectCutOff"
    Assembly="PraticeManagement" %>
<table class="WholeWidth">
    <tr class="time-entry-bar">
        <td class="DeleteWidth">
        </td>
        <td class="time-entry-bar-time-typesNew">
            <asp:DropDownList ID="ddlTimeTypes" runat="server" CssClass="time-entry-bar-time-typesNew-select-Normal"
                OnDataBound="ddlTimeTypes_DataBound" DataTextField="Name" DataValueField="Id"
                ValidationGroup='<%# ClientID %>' onchange="setDirty();" />
            <extS:SelectCutOffExtender ID="SelectCutOffExtender1" runat="server" NormalCssClass="time-entry-bar-time-typesNew-select-Normal"
                ExtendedCssClass="time-entry-bar-time-typesNew-select-Extended" TargetControlID="ddlTimeTypes" />
            <asp:Label ID="lblTimeType" runat="server"></asp:Label>
            <asp:HiddenField ID="hdnworkTypeId" runat="server" />
        </td>
        <asp:Repeater ID="tes" runat="server" OnItemDataBound="repEntries_ItemDataBound">
            <ItemTemplate>
                <td class="time-entry-bar-single-teNew <%# GetDayOffCssCalss(((System.Xml.Linq.XElement)Container.DataItem)) %>">
                    <table class="WholeWidth">
                        <tr>
                            <td align="center">
                                <te:SingleTE runat="server" ID="ste" />
                            </td>
                        </tr>
                    </table>
                </td>
            </ItemTemplate>
        </asp:Repeater>
        <td class="time-entry-total-hoursNew">
            <label id="lblTotalHours" runat="server" />
            <label id="lblEnableDisable" runat="server" />
            <ext:TotalCalculatorExtender ID="extTotalHours" runat="server" TargetControlID="lblTotalHours" />
            <ext2:EnableDisableExtender ID="extEnableDisable" runat="server" TargetControlID="lblEnableDisable" />
        </td>
        <td class="DeleteWidth">
        </td>
    </tr>
</table>

