<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByBusinessDevelopment.ascx.cs" EnableViewState="false"
    Inherits="PraticeManagement.Controls.Reports.ByAccount.ByBusinessDevelopment" %>
<%@ Register Src="~/Controls/Reports/ByAccount/GroupByBusinessUnit.ascx" TagName="GroupByBusinessUnit"
    TagPrefix="UC" %>
<%@ Register Src="~/Controls/Reports/ByAccount/GroupByPerson.ascx" TagName="GroupByPerson"
    TagPrefix="UC" %>
<asp:HiddenField ID="hdncpeExtendersIds" runat="server" Value="" />
<asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent">
                <asp:Button ID="btnExpandOrCollapseAll" runat="server" Text="Collapse All" UseSubmitBehavior="false"
                    CssClass="Width100Px" ToolTip="Collapse All" />
                <asp:Button ID="btnGroupByPerson" runat="server" Text="Group by Person" ToolTip="Group by Person" Visible="True"
                    OnClick="btnGroupBy_Click" />
                    <asp:Button ID="btnGroupByBU" runat="server" Text="Group by Business Unit" ToolTip="Group by Business Unit" Visible="False"
                    OnClick="btnGroupBy_Click" />
            </td>
            <td class="textRight Width10Percent padRight5">
                <table class="WholeWidth textRight">
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
                                Enabled="false" UseSubmitBehavior="false" ToolTip="Export To PDF" />
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
                    <UC:GroupByBusinessUnit ID="tpByBusinessUnit" runat="server"></UC:GroupByBusinessUnit>
                </asp:Panel>
            </asp:View>
            <asp:View ID="vwPersonReport" runat="server">
                <asp:Panel ID="pnlPersonReport" runat="server" CssClass="WholeWidth">
                    <UC:GroupByPerson ID="tpByPerson" runat="server"></UC:GroupByPerson>
                </asp:Panel>
            </asp:View>
        </asp:MultiView>
    </div>
</div>

