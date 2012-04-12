﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillableAndNonBillableTimeEntryBar.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.BillableAndNonBillableTimeEntryBar" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.SelectCutOff"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="ext2" Namespace="PraticeManagement.Controls.Generic.EnableDisableExtender"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/TimeEntry/BillableAndNonBillableSingleTimeEntry.ascx"
    TagName="BillableAndNonBillableSingleTimeEntry" TagPrefix="te" %>
<%@ Register TagPrefix="ext1" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="PraticeManagement.Controls.TimeEntry" %>
<table class="WholeWidth">
    <tr class="time-entry-bar">
        <td class="DeleteWidth">
        </td>
        <td class="time-entry-bar-time-typesNewst">
            <table class="WholeWidth">
                <tr>
                    <td style="width: 10%;padding-left:4px;" id="tdplusProjectSection" runat="server">
                    </td>
                    <td style="width: 90%;">
                        <asp:DropDownList ID="ddlTimeTypes" runat="server" CssClass="time-entry-bar-time-typesNew-select-Normal"
                            OnDataBound="ddlTimeTypes_DataBound" DataTextField="Name" DataValueField="Id"
                            ValidationGroup='<%# ClientID %>' onchange="setDirty();EnableSaveButton(true);" />
                        <ext:SelectCutOffExtender ID="SelectCutOffExtender1" runat="server" NormalCssClass="time-entry-bar-time-typesNew-select-Normal"
                            ExtendedCssClass="time-entry-bar-time-typesNew-select-Extended" TargetControlID="ddlTimeTypes" />
                    </td>
                </tr>
            </table>
        </td>
        <asp:Repeater ID="tes" runat="server" OnItemDataBound="repEntries_ItemDataBound">
            <ItemTemplate>
                <td class="time-entry-bar-single-teNew <%# GetDayOffCssClass(((System.Xml.Linq.XElement)Container.DataItem)) %>">
                    <table class="WholeWidth">
                        <tr>
                            <td align="center">
                                <te:BillableAndNonBillableSingleTimeEntry runat="server" ID="ste" />
                            </td>
                        </tr>
                    </table>
                </td>
            </ItemTemplate>
        </asp:Repeater>
        <td class="time-entry-total-hoursNew-totalColoum">
            <div style="float: right; padding-right: 10px;">
                <label id="lblTotalHours" runat="server" />
            </div>
            <ext1:TotalCalculatorExtender ID="extTotalHours" runat="server" TargetControlID="lblTotalHours" />
            <ext2:EnableDisableExtender ID="extEnableDisable" runat="server" TargetControlID="ddlTimeTypes" />
        </td>
        <td class="DeleteWidth">
            <asp:ImageButton ID="imgDropTes" runat="server" OnClientClick='return confirm ("This will remove the Work Type as well as any time and notes entered!  Are you sure?")'
                ToolTip="Remove Work Type" ImageUrl="~/Images/close_16.png" OnClick="imgDropTes_Click" />
        </td>
    </tr>
</table>

