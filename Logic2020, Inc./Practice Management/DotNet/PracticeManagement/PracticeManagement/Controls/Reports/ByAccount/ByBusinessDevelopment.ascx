<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByBusinessDevelopment.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByAccount.ByBusinessDevelopment" %>
<%@ Register Src="~/Controls/Reports/ByAccount/BusinessDevelopmentGroupByBusinessUnit.ascx"
    TagName="GroupByBusinessUnit" TagPrefix="UC" %>
<%@ Register Src="~/Controls/Reports/ByAccount/BusinessDevelopmentGroupByPerson.ascx"
    TagName="GroupByPerson" TagPrefix="UC" %>
<asp:HiddenField ID="hdncpeExtendersIds" runat="server" Value="" />
<asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" style="width: 90%;">
            <asp:Button ID="btnExpandOrCollapseAll" runat="server" Text="Collapse All" UseSubmitBehavior="false"
                Width="100px" ToolTip="Collapse All" />
            <asp:Button ID="btnGroupBy" runat="server" Text="Group by Person" ToolTip="Group by Person"
                OnClick="btnGroupBy_Click" />
        </td>
        <td style="text-align: right; width: 10%; padding-right: 5px;">
            <table width="100%" style="text-align: right;">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To Excel" />
                    </td>
                    <td>
                        <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<div>
    <asp:MultiView ID="mvBusinessDevelopmentReport" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwBusinessDevelopmentReport" runat="server">
            <asp:Panel ID="pnlBusinessUnitReport" runat="server" CssClass="WholeWidth">
                <UC:GroupByBusinessUnit id="tpByBusinessUnit" runat="server"></UC:GroupByBusinessUnit>
            </asp:Panel>
        </asp:View>
        <asp:View ID="vwPersonReport" runat="server">
            <asp:Panel ID="pnlPersonReport" runat="server" CssClass="WholeWidth">
                <UC:GroupByPerson id="tpByPerson" runat="server"></UC:GroupByPerson>
            </asp:Panel>
        </asp:View>
    </asp:MultiView>
</div>

