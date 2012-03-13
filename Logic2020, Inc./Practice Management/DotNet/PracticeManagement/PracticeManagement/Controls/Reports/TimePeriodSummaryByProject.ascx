<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByProject" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.BillableNonBillableAndTotal"
    Assembly="PraticeManagement" %>
<div class="PaddingBottom6 PaddingTop6 textCenter">
    Show:
    <input type="radio" id="rbBillable" runat="server" name="ByResource" displayvaluetype="billabletotal" />
    Billable
    <input type="radio" id="rbNonBillable" runat="server" name="ByResource" displayvaluetype="nonbillabletotal" />
    Non-Billable
    <input type="radio" id="rbCombined" runat="server" checked="true" name="ByResource"
        displayvaluetype="combinedtotal" />
    Combined Total
</div>
<asp:Repeater ID="repProject" runat="server" OnItemDataBound="repProject_ItemDataBound">
    <HeaderTemplate>
        <table class="CompPerfTable WholeWidth">
            <tr class="CompPerfHeader">
                <th>
                    <div class="ie-bg">
                        Project
                    </div>
                </th>
                <th>
                    <div class="ie-bg">
                        Status
                    </div>
                </th>
                <asp:Repeater ID="repProjectHeaders" runat="server">
                    <ItemTemplate>
                        <th>
                            <div class="ie-bg">
                                <%# Eval("Value")%>
                            </div>
                        </th>
                    </ItemTemplate>
                    <FooterTemplate>
                        <th>
                            <div class="ie-bg">
                                Total
                            </div>
                        </th>
                    </FooterTemplate>
                </asp:Repeater>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="ReportItemTemplate">
            <td>
                <table>
                    <tr>
                        <td style="color: Gray; padding-bottom: 3px; padding-left: 2px;">
                            <%# Eval("Project.Client.Name")%>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold; padding-bottom: 5px; padding-left: 2px;">
                            <%# Eval("Project.ProjectNumber")%>
                            -
                            <%# Eval("Project.Name")%>
                        </td>
                    </tr>
                </table>
            </td>
            <td class="textCenter">
                <%# Eval("Project.Status.Name")%>
            </td>
            <asp:Repeater ID="reProjectHoursPerDay" runat="server" OnItemDataBound="reProjectHoursPerDay_OnItemDataBound">
                <ItemTemplate>
                    <td id="tdDayTotalHours" billabletotal='<%# ((double)Eval("Value.BillabileTotal")).ToString("0.00") %>'
                        nonbillabletotal='<%# ((double)Eval("Value.NonBillableTotal")).ToString("0.00") %>'
                        combinedtotal='<%# ((double)Eval("Value.CombinedTotal")).ToString("0.00") %>'
                        runat="server" class="textCenter">
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td id="tdProjectTotalHours" class="textCenter" billabletotal='<%# ((double)Eval("BillabileTotal")).ToString("0.00") %>'
                nonbillabletotal='<%# ((double)Eval("NonBillableTotal")).ToString("0.00") %>'
                combinedtotal='<%# ((double)Eval("CombinedTotal")).ToString("0.00") %>' runat="server">
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="ReportAlternateItemTemplate">
            <td>
                <table>
                    <tr>
                        <td style="color: Gray; padding-bottom: 3px; padding-left: 2px;">
                            <%# Eval("Project.Client.Name")%>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold; padding-bottom: 5px; padding-left: 2px;">
                            <%# Eval("Project.ProjectNumber")%>
                            -
                            <%# Eval("Project.Name")%>
                        </td>
                    </tr>
                </table>
            </td>
            <td class="textCenter">
                <%# Eval("Project.Status.Name")%>
            </td>
            <asp:Repeater ID="reProjectHoursPerDay" runat="server" OnItemDataBound="reProjectHoursPerDay_OnItemDataBound">
                <ItemTemplate>
                    <td id="tdDayTotalHours" billabletotal='<%# ((double)Eval("Value.BillabileTotal")).ToString("0.00") %>'
                        nonbillabletotal='<%# ((double)Eval("Value.NonBillableTotal")).ToString("0.00") %>'
                        combinedtotal='<%# ((double)Eval("Value.CombinedTotal")).ToString("0.00") %>'
                        runat="server" class="textCenter">
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td id="tdProjectTotalHours" class="textCenter" billabletotal='<%# ((double)Eval("BillabileTotal")).ToString("0.00") %>'
                nonbillabletotal='<%# ((double)Eval("NonBillableTotal")).ToString("0.00") %>'
                combinedtotal='<%# ((double)Eval("CombinedTotal")).ToString("0.00") %>' runat="server">
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
<label id="lblTotalHours" runat="server" />
<ext:BillableNonBillableAndTotalExtender ID="extBillableNonBillableAndTotalExtender"
    runat="server" TargetControlID="lblTotalHours">
</ext:BillableNonBillableAndTotalExtender>

