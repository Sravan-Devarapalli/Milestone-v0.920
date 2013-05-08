<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CSATSummaryReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.CSAT.CSATSummaryReport" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent">
            </td>
            <td class="textRight Width10Percent padRight5">
                <table class="textRight WholeWidth">
                    <tr>
                        <td class="PaddingBottom5Imp">
                            Export:
                        </td>
                        <td class="PaddingBottom5Imp">
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                                UseSubmitBehavior="false" ToolTip="Export To Excel"/>
                        </td>
                        <td class="PaddingBottom5Imp">
                            <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                                Enabled="false" UseSubmitBehavior="false" ToolTip="Export To PDF" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="repSummary" runat="server" OnItemDataBound="repSummary_ItemDataBound">
        <HeaderTemplate>
            <table id="tblCSATSummary" class="tablesorter">
                <thead>
                    <tr class="trCSATSummaryHeader">
                        <th class="Width15Percent TextAlignLeftImp">
                            Account
                        </th>
                        <th class="Width15Percent">
                            Business Group
                        </th>
                        <th class="Width15Percent">
                            Business Unit
                        </th>
                        <th class="Width7point5Percent">
                            Project Number
                        </th>
                        <th class="Width15Percent"> 
                            Project Name
                        </th>
                        <th class="Width7point5Percent">
                            Project Status
                        </th>
                        <th class="Width15Percent">
                            Practice Area
                        </th>
                        <th class="Width5Percent">
                            Est. Revenue
                        </th>
                        <th class="Width5Percent">
                            CSAT Score
                        </th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplateCSAT">
                <td class="padLeft5 textLeft">
                    <%# Eval("Client.Name")%>
                </td>
                <td>
                    <%# Eval("BusinessGroup.Name")%>
                </td>
                <td>
                    <%# Eval("Group.Name")%>
                </td>
                <td>
                    <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# Eval("ProjectNumber")%> '
                        NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Id")),false) %>'>
                    </asp:HyperLink>
                </td>
                <td>
                    <%# Eval("Name")%>
                </td>
                <td>
                    <%# Eval("Status.Name")%>
                </td>
                <td>
                    <%# Eval("Practice.Name")%>
                </td>
                <td>
                    $<%# Eval("SowBudget")%>
                </td>
                <td>
                    <table class="WholeWidth">
                        <tr>
                            <td class="width60P textRightImp BorderNoneImp">
                                <asp:HyperLink ID="hlCSATScore" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Id")),true) %>'
                                    runat="server"></asp:HyperLink>
                            </td>
                            <td class="textLeft BorderNoneImp">
                                <asp:Label ID="lblSymblvsble" ForeColor="Red" CssClass="error-message fontSizeLarge"
                                    runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody> </table>
        </FooterTemplate>
    </asp:Repeater>
    <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
        There are no CSATs for this range selected.
    </div>
</div>

