<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NonbillableSummary.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.NonbillableSummary" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent">
                &nbsp;&nbsp;
            </td>
            <td class="textRight Width10Percent padRight5">
                <table class="textRight WholeWidth">
                    <tr>
                        <td>
                            Export:
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                                UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="repNonbillable" runat="server" OnItemDataBound="repNonbillable_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px WholeWidth" style="overflow-x:scroll;">
                <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport zebra" style="width:3020px">
                    <thead>
                        <tr>
                            <th colspan="10">
                                &nbsp;&nbsp;
                            </th>
                            <th colspan="3" style="background-color:Yellow">
                                Hours
                            </th>
                            <th colspan="3" style="background-color: #9BC2E6">
                                Revenue
                            </th>
                            <th colspan="3" style="background-color: #C6E0B4">
                                Cost
                            </th>
                            <th colspan="2" style="background-color: #F4B084">
                                Margin
                            </th>
                            <th colspan="2">
                                &nbsp;&nbsp;
                            </th>
                        </tr>
                        <tr>
                            <th class="Width125Px TextAlignLeftImp">
                                Project Number
                            </th>
                            <th class="Width125Px">
                                Account
                            </th>
                            <th class="Width130px">
                                Business Unit
                            </th>
                            <th class="Width150px">
                                Project Name
                            </th>
                            <th class="Width130px">
                                Practice Area
                            </th>
                            <th class="Width130px">
                                Sales person
                            </th>
                            <th class="Width150px">
                                Project Access
                            </th>
                            <th class="Width130px">
                                Engagement Manager
                            </th>
                            <th class="Width130px">
                                Executive in Charge
                            </th>
                            <th class="Width100Px">
                                Resource
                            </th>
                            <th class="Width130px">
                                Billable
                            </th>
                            <th class="Width130px">
                                Nonbillable
                            </th>
                            <th class="Width130px">
                                Total hours
                            </th>
                            <th class="Width140px">
                                Billable revenue
                            </th>
                            <th class="Width140px">
                                Lost revenue
                            </th>
                            <th class="Width140px">
                                Potential Revenue
                            </th>
                            <th class="Width130px">
                                Billable
                            </th>
                            <th class="Width130px">
                                Nonbillable
                            </th>
                            <th class="Width130px">
                                Total cost
                            </th>
                            <th class="Width130px">
                                Billable Margin
                            </th>
                            <th class="Width130px">
                                Actual Margin
                            </th>
                            <th class="Width130px">
                                Cost/Hour
                            </th>
                            <th class="Width130px">
                                Hourly(Billable) Rate
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="padLeft5 textLeft">
                   <asp:LinkButton ID="hlProjectNumber" runat="server" Text='<%# Eval("Project.ProjectNumber")%>' OnClick="hlProjectNumber_Click"></asp:LinkButton>
                </td>
                <td>
                    <%# Eval("Project.Client.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Project.Group.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Project.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Project.Practice.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Project.SalesPersonName")%>
                </td>
                <td>
                    <asp:Label ID="lblProjectManagers" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblSeniorManager" runat="server"></asp:Label>
                </td>
                <td>
                    <%# Eval("Project.DirectorName")%>
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    <asp:Label ID="lblBillableHours" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblNonBillableHours" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblTotalHours" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblBillableRevenue" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblLostRevenue" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblPotentialRevenue" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblBillablecost" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblNonbillableCost" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblTotalCost" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblBillableMargin" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblActualMargin" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblFLHR" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblHourlyRate" runat="server"></asp:Label>
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
        <tr class="alterrow">
                <td class="padLeft5 textLeft">
                   <asp:LinkButton ID="hlProjectNumber" runat="server" Text='<%# Eval("Project.ProjectNumber")%>' OnClick="hlProjectNumber_Click"></asp:LinkButton>
                </td>
                <td>
                    <%# Eval("Project.Client.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Project.Group.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Project.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Project.Practice.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Project.SalesPersonName")%>
                </td>
                <td>
                    <asp:Label ID="lblProjectManagers" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblSeniorManager" runat="server"></asp:Label>
                </td>
                <td>
                    <%# Eval("Project.DirectorName")%>
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    <asp:Label ID="lblBillableHours" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblNonBillableHours" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblTotalHours" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblBillableRevenue" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblLostRevenue" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblPotentialRevenue" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblBillablecost" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblNonbillableCost" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblTotalCost" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblBillableMargin" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblActualMargin" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblFLHR" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblHourlyRate" runat="server"></asp:Label>
                </td>
            </tr>
        </AlternatingItemTemplate>
        <FooterTemplate>
            </tbody></table></div>
        </FooterTemplate>
    </asp:Repeater>
    <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
        There are no resources for the project for the report paramenters selected.
    </div>
</div>

