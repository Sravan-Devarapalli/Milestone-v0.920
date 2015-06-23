<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NonbillableDetail.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.NonbillableDetail" %>
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
                <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport zebra" style="width:3050px">
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
                            <th class="Width130Px">
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
                    <asp:Label ID="lblProjectNumber" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblAccount" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblProjectGroup" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblProjectName" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblPractice" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblSalesPerson" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblProjectManagers" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblSeniorManager" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblDirector" runat="server"></asp:Label>
                </td>
                <td>
                    <%# Eval("Person.HtmlEncodedName")%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("ProjectNonBillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BillableHours")+(double)Eval("ProjectNonBillableHours"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("BillableRevenue"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("LostRevenue"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("PotentialRevenue"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("BillableCost"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("NonBillableCost"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("TotalCost"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("BillableMargin"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("ActualMargin"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("FLHR"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("BillRate"))%>
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
         <tr class="alterrow">
                <td class="padLeft5 textLeft">
                    <asp:Label ID="lblProjectNumber" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblAccount" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblProjectGroup" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblProjectName" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblPractice" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblSalesPerson" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblProjectManagers" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblSeniorManager" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblDirector" runat="server"></asp:Label>
                </td>
                <td>
                    <%# Eval("Person.HtmlEncodedName")%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("ProjectNonBillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BillableHours")+(double)Eval("ProjectNonBillableHours"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("BillableRevenue"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("LostRevenue"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("PotentialRevenue"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("BillableCost"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("NonBillableCost"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("TotalCost"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("BillableMargin"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("ActualMargin"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("FLHR"))%>
                </td>
                <td>
                    <%# GetCurrencyDecimalFormat((double)Eval("BillRate"))%>
                </td>
            </tr>
        </AlternatingItemTemplate>
        <FooterTemplate>
            <tr>
                <th colspan="10">
                    &nbsp;&nbsp;
                </th>
                <th>
                    <asp:Label ID="lblTotalBillableHours" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalNonbillableHours" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalofTotalHours" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalBillableRevenue" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalLostRevenue" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalPotentialRevnue" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalBillCost" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalNonbillableCost" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalofTotalCost" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalBillMargin" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalActualMargin" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalFlhr" runat="server"></asp:Label>
                </th>
                <th>
                    <asp:Label ID="lblTotalBillRate" runat="server"></asp:Label>
                </th>
            </tr>
            </tbody></table></div>
        </FooterTemplate>
    </asp:Repeater>
</div>

