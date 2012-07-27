<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarLegend.ascx.cs"
    Inherits="PraticeManagement.Controls.CalendarLegend" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<div class="buttons-block">
    <div id="dvCollapsiblePanel" runat="server">
        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
            ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
            ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
        <asp:Label ID="lblFilter" runat="server" Text="Legend" />&nbsp;
        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
            ToolTip="Expand Legend" />
    </div>
    <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
        <table>           
            <tr>
                <td class="TdCompanyDayOn">
                    <table class="InnerTableCompanyDay" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="InnerTdCompanyDayOn">
                                <div class="CompanyDayOn">
                                    &nbsp;</div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="DayDescription">
                    - This is a company work day, and the person is taking paid or otherwise approved time off.
                </td>
            </tr>
            <tr>
                <td class="TdCompanyDayOn">
                    <table class="InnerTableCompanyDay" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="InnerTdCompanyDayOn">
                                <div class="DayOff">
                                    &nbsp;</div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="DayDescription">
                    - This is a company holiday, and the person is not scheduled to work it.
                </td>
            </tr>
            <tr>
                <td class="TdCompanyDayOn">
                    <table class="InnerTableCompanyDay" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="InnerTdCompanyDayOn">
                                <div class="CompanyDayOff">
                                    &nbsp;</div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="DayDescription">
                    - This is a company holiday, but the person is scheduled to work anyway.
                </td>
            </tr>           
        </table>
    </asp:Panel>
</div>

