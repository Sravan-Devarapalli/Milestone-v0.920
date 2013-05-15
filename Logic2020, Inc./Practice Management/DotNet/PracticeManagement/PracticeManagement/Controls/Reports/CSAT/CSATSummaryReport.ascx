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
                                UseSubmitBehavior="false" ToolTip="Export To Excel"/>
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
                        <th class="Width7Percent">
                            Project Number
                        </th>
                        <th class="Width15Percent"> 
                            Project Name
                        </th>
                        <th class="Width6point5Percent">
                            Project Status
                        </th>
                        <th class="Width15Percent">
                            Practice Area
                        </th>
                        <th class="Width7point5Percent">
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
                        NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Id")),false) %>'>
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

