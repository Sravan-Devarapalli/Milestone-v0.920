<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CSATSummaryReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.CSAT.CSATSummaryReport" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width95Per">
            </td>
            <td class="textRight Width5PercentImp padRight5">
                <table class="textRight WholeWidth">
                    <tr>
                        <td class="PaddingBottom5Imp">
                            Export:
                        </td>
                        <td class="PaddingBottom5Imp">
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                                UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="repSummary" runat="server" OnItemDataBound="repSummary_ItemDataBound">
        <HeaderTemplate>
            <table id="tblCSATSummary" class="tablesorter WholeWidth">
                <thead>
                    <tr class="trCSATSummaryHeader">
                        <th class="Width13Percent TextAlignLeftImp">
                            Account
                        </th>
                        <th class="Width13Percent">
                            Business Group
                        </th>
                        <th class="Width13Percent">
                            Business Unit
                        </th>
                        <th class="Width10Per">
                            Project Number
                        </th>
                        <th class="Width15Per">
                            Project Name
                        </th>
                        <th class="Width8Per">
                            Project Status
                        </th>
                        <th class="Width15Percent">
                            Practice Area
                        </th>
                        <th class="Width10Per">
                            Est. Revenue
                        </th>
                        <th class="Width2Percent">
                            CSAT Score
                        </th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplateCSAT">
                <td class="padLeft5 textLeft">
                    <%# Eval("Client.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("BusinessGroup.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Group.HtmlEncodedName")%>
                </td>
                <td>
                    <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# Eval("ProjectNumber")%> '
                        Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Id")),false) %>'>
                    </asp:HyperLink>
                </td>
                <td>
                    <%# Eval("HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Status.Name")%>
                </td>
                <td>
                    <%# Eval("Practice.HtmlEncodedName")%>
                </td>
                <td sorttable_customkey='<%# Eval("SowBudget")%>'>
                    <%# GetFormatedSowBudget((decimal?)Eval("SowBudget"))%>
                </td>
                <td>
                    <table class="WholeWidth">
                        <tr>
                            <td class="width60P textRightImp BorderNoneImp">
                                <asp:HyperLink ID="hlCSATScore" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Id")),true) %>'
                                    Target="_blank" runat="server"></asp:HyperLink>
                            </td>
                            <td class="textLeft BorderNoneImp">
                                <asp:Label ID="lblSymblvsble" ForeColor="Red" CssClass="error-message fontSizeLarge"
                                    ToolTip="This project has multiple CSAT entries." runat="server"></asp:Label>
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

