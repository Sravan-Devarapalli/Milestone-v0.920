<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdministrativeTimeEntryBar.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.AdministrativeTimeEntryBar" %>
<%@ Register Src="~/Controls/TimeEntry/AdministrativeSingleTimeEntry.ascx" TagName="SingleTE"
    TagPrefix="te" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="ext2" Namespace="PraticeManagement.Controls.Generic.EnableDisableExtender"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="PraticeManagement.Controls.TimeEntry" %>
<%@ Register TagPrefix="ext3" Namespace="PraticeManagement.Controls.Generic.SelectCutOff"
    Assembly="PraticeManagement" %>
<table class="WholeWidth">
    <tr class="time-entry-bar">
        <td class="DeleteWidth">
        </td>
        <td class="time-entry-bar-time-typesNew" id="tdTimeTypes" runat="server">
            <table class="WholeWidth">
                <tr>
                    <td id="tdPlusSection" runat="server" class="FirstTd">
                    </td>
                    <td class="SecondTd">
                        <pmc:CustomDropDown ID="ddlTimeTypes" runat="server" CssClass="time-entry-bar-time-typesNew-select-Normal"
                            OnDataBound="ddlTimeTypes_DataBound" DataTextField="Name" DataValueField="Id"
                            ValidationGroup='<%# ClientID %>' onchange="setDirty();EnableSaveButton(true);" />
                        <ext3:SelectCutOffExtender ID="SelectCutOffExtender1" runat="server" NormalCssClass="time-entry-bar-time-typesNew-select-Normal"
                            ExtendedCssClass="time-entry-bar-time-typesNew-select-Extended" TargetControlID="ddlTimeTypes" />
                        <asp:Label ID="lblTimeType" runat="server"></asp:Label>
                        <asp:HiddenField ID="hdnworkTypeId" runat="server" />
                    </td>
                </tr>
            </table>
        </td>
        <asp:Repeater ID="tes" runat="server" OnItemDataBound="repEntries_ItemDataBound">
            <HeaderTemplate>
            </HeaderTemplate>
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
            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>
        <td class="time-entry-total-hoursNew-totalColoum">
            <div class="TotalDiv">
                <label id="lblTotalHours" runat="server" />
            </div>
            <label id="lblEnableDisable" runat="server" />
            <ext:TotalCalculatorExtender ID="extTotalHours" runat="server" TargetControlID="lblTotalHours" />
            <ext2:EnableDisableExtender ID="extEnableDisable" runat="server" TargetControlID="ddlTimeTypes" />
        </td>
        <td class="DeleteWidth">
            <asp:ImageButton ID="imgDropTes" runat="server" OnClientClick='return confirm ("This will remove the Work Type as well as any time and notes entered!  Are you sure?")'
                ToolTip="Remove Work Type" ImageUrl="~/Images/close_16.png" OnClick="imgDropTes_Click" />
        </td>
    </tr>
</table>

