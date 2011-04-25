<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntryBar.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.TimeEntryBar" %>
<%@ Register Src="~/Controls/TimeEntry/SingleTimeEntry.ascx" TagName="SingleTE" TagPrefix="te" %>
<%@ Import Namespace="PraticeManagement.Controls.TimeEntry" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.SelectCutOff"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="ext1" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<table class="WholeWidth">
    <tr class="time-entry-bar">
        <td class="time-entry-bar-project-milestones">
            <asp:DropDownList ID="ddlProjectMilestone" runat="server" AutoPostBack="true" onchange="setDirty();"
                CssClass="time-entry-bar-project-milestones-select-Normal" OnSelectedIndexChanged="ddlProjectMilestone_OnSelectedIndexChanged" />
            <ext:SelectCutOffExtender ID="scoe1" runat="server" NormalCssClass="time-entry-bar-project-milestones-select-Normal"
                ExtendedCssClass="time-entry-bar-project-milestones-select-Extended" TargetControlID="ddlProjectMilestone" />
        </td>
        <td class="time-entry-bar-time-types">
            <asp:DropDownList ID="ddlTimeTypes" runat="server" DataSourceID="odsTimeTypes" CssClass="time-entry-bar-time-types-select-Normal"
                DataTextField="Name" DataValueField="Id" ValidationGroup='<%# ClientID %>' AutoPostBack="true"
                onchange="setDirty();" OnSelectedIndexChanged="ddlTimeTypes_OnSelectedIndexChanged"
                OnDataBound="ddlTimeTypes_DataBound" />
            <ext:SelectCutOffExtender ID="SelectCutOffExtender1" runat="server" NormalCssClass="time-entry-bar-time-types-select-Normal"
                ExtendedCssClass="time-entry-bar-time-types-select-Extended" TargetControlID="ddlTimeTypes" />
        </td>
        <asp:Repeater ID="tes" runat="server" OnItemDataBound="repEntries_ItemDataBound">
            <ItemTemplate>
                <td class="time-entry-bar-single-te <%# GetDayOffCssCalss((TeGridCell)Container.DataItem) %>">
                    <te:SingleTE runat="server" ID="ste" OnReadyToUpdateTE="steItem_OnReadyToUpdateTE" />
                </td>
            </ItemTemplate>
        </asp:Repeater>
        <td class="time-entry-total-hours">
            <label id="lblTotalHours" runat="server" />
            <ext1:TotalCalculatorExtender ID="extTotalHours" runat="server" TargetControlID="lblTotalHours" />
        </td>
        <td style="width: 3%; text-align: center;">
            <asp:ImageButton ID="imgDropTes" runat="server" OnClientClick='return confirm ("Are you sure you want to delete this row?")'
                ImageUrl="~/Images/close_16.png" OnClick="imgDropTes_Click" />
        </td>
    </tr>
</table>
<asp:ObjectDataSource ID="odsTimeTypes" runat="server" DataObjectTypeName="DataTransferObjects.TimeEntry.TimeTypeRecord"
    SelectMethod="GetAllTimeTypes" CacheDuration="300" EnableCaching="true" CacheKeyDependency="TimeTypeCachedData"
    TypeName="PraticeManagement.Controls.TimeEntry.TimeEntryBar"></asp:ObjectDataSource>

