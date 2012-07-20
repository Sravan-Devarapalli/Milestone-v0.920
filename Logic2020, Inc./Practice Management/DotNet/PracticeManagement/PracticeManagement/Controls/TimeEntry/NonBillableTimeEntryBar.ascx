<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NonBillableTimeEntryBar.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.NonBillableTimeEntryBar" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.SelectCutOff"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/TimeEntry/SingleTimeEntry_New.ascx" TagName="SingleTE"
    TagPrefix="te" %>
<%@ Register TagPrefix="ext1" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="ext2" Namespace="PraticeManagement.Controls.Generic.EnableDisableExtender"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="PraticeManagement.Controls.TimeEntry" %>
<table class="WholeWidth">
    <tr class="time-entry-bar">
        <td class="DeleteWidth">
        </td>
        <td class="time-entry-bar-time-typesNew">
            <table class="WholeWidth">
                <tr>
                    <td id="tdPlusSection" runat="server" class="FirstTd">
                    </td>
                    <td class="SecondTd">
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
                                <te:SingleTE runat="server" ID="ste" />
                            </td>
                        </tr>
                    </table>
                </td>
            </ItemTemplate>
        </asp:Repeater>
        <td class="time-entry-total-hoursNew-totalColoum">
            <div class="TotalDiv">
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

