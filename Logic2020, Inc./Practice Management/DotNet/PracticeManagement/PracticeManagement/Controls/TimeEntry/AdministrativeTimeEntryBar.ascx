<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdministrativeTimeEntryBar.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.AdministrativeTimeEntryBar" %>
<%@ Register Src="~/Controls/TimeEntry/SingleTimeEntry_New.ascx" TagName="SingleTE"
    TagPrefix="te" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="PraticeManagement.Controls.TimeEntry" %>
<table class="WholeWidth">
    <tr class="time-entry-bar">
        <td class="time-entry-bar-time-typesNew">
            <asp:Label ID="lblTimeType" runat="server"></asp:Label>
            <asp:HiddenField ID="hdnworkTypeId" runat="server" />
        </td>
        <asp:Repeater ID="tes" runat="server" OnItemDataBound="repEntries_ItemDataBound">
            <ItemTemplate>
                <td class="time-entry-bar-single-teNew <%# GetDayOffCssCalss(((System.Xml.Linq.XElement)Container.DataItem)) %>">
                    <table cellpadding="0" cellspacing="0" class="WholeWidth">
                        <tr>
                            <td>
                                N
                            </td>
                            <td>
                                <te:SingleTE runat="server" ID="ste" />
                            </td>
                        </tr>
                    </table>
                </td>
            </ItemTemplate>
        </asp:Repeater>
        <td class="time-entry-total-hoursNew">
            <label id="lblTotalHours" runat="server" />
            <ext:TotalCalculatorExtender ID="extTotalHours" runat="server" TargetControlID="lblTotalHours" />
        </td>
        <td style="width: 3%; text-align: center;">
        </td>
    </tr>
</table>

