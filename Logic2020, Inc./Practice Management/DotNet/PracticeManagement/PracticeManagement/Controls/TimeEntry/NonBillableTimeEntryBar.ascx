<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NonBillableTimeEntryBar.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.NonBillableTimeEntryBar" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.SelectCutOff"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/TimeEntry/SingleTimeEntry_New.ascx" TagName="SingleTE"
    TagPrefix="te" %>
<%@ Register TagPrefix="ext1" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="ext2" Namespace="PraticeManagement.Controls.Generic.TimeEntryHoursTextBoxEnableDisable"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="PraticeManagement.Controls.TimeEntry" %>
<table class="WholeWidth">
    <tr class="time-entry-bar">
        <td class="time-entry-bar-time-typesNew">
            <asp:DropDownList ID="ddlTimeTypes" runat="server" CssClass="time-entry-bar-time-typesNew-select-Normal"
                OnDataBound="ddlTimeTypes_DataBound" DataTextField="Name" DataValueField="Id"
                ValidationGroup='<%# ClientID %>' onchange="setDirty();" />
            <ext:SelectCutOffExtender ID="SelectCutOffExtender1" runat="server" NormalCssClass="time-entry-bar-time-typesNew-select-Normal"
                ExtendedCssClass="time-entry-bar-time-typesNew-select-Extended" TargetControlID="ddlTimeTypes" />
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
            <ext1:TotalCalculatorExtender ID="extTotalHours" runat="server" TargetControlID="lblTotalHours" />
            <ext2:TimeEntryHoursTextBoxEnableDisable ID="extEnableDisable" runat="server" TargetControlID="ddlTimeTypes" />
        </td>
        <td style="width: 3%; text-align: center;">
            <asp:ImageButton ID="imgDropTes" runat="server" OnClientClick='return confirm ("This will delete the time and notes entered for the entire row!  Are you sure?")'
                ImageUrl="~/Images/close_16.png" OnClick="imgDropTes_Click" />
        </td>
    </tr>
</table>

