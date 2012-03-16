<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByworkType.ascx.cs" Inherits="PraticeManagement.Controls.Reports.ByworkType" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.BillableNonBillableAndTotal"
    Assembly="PraticeManagement" %>
<div class="PaddingBottom6 PaddingTop6 textCenter">
    Show:
    <input type="radio" id="rbBillable" runat="server" name="ByWorkType" displayvaluetype="billabletotal" />
    Billable
    <input type="radio" id="rbNonBillable" runat="server" name="ByWorkType" displayvaluetype="nonbillabletotal" />
    Non-Billable
    <input type="radio" id="rbCombined" runat="server" checked="true" name="ByWorkType"
        displayvaluetype="combinedtotal" />
    Combined Total
</div>
<asp:Repeater ID="repWorkType" runat="server" OnItemDataBound="repWorkType_ItemDataBound">
    <HeaderTemplate>
        <table class="PersonSummaryReport WholeWidth">
            <tr>
                <th>
                    Work Type
                </th>
                <th>
                    Category
                </th>
                <asp:Repeater ID="repWorkTypeHeaders" runat="server">
                    <ItemTemplate>
                        <th>
                            <%# Eval("Value")%>
                        </th>
                    </ItemTemplate>
                </asp:Repeater>
                <th>
                    Total
                </th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="ReportItemTemplate">
            <td class="padLeft5">
                <%# Eval("WorkType.Name")%>
            </td>
            <td class="padLeft5">
             <%# Eval("WorkType.Category")%>
            </td>
            <asp:Repeater ID="repWorkTypeHoursPerDay" OnItemDataBound="repWorkTypeHoursPerDay_OnItemDataBound"
                runat="server">
                <ItemTemplate>
                    <td id="tdDayTotalHours" billabletotal='<%# ((double)Eval("Value.BillabileTotal")).ToString("0.00") %>'
                        nonbillabletotal='<%# ((double)Eval("Value.NonBillableTotal")).ToString("0.00") %>'
                        combinedtotal='<%# ((double)Eval("Value.CombinedTotal")).ToString("0.00") %>'
                        runat="server" class="textCenter">
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td id="tdPersonTotalHours" class="textCenter" billabletotal='<%# ((double)Eval("BillabileTotal")).ToString("0.00") %>'
                nonbillabletotal='<%# ((double)Eval("NonBillableTotal")).ToString("0.00") %>'
                combinedtotal='<%# ((double)Eval("CombinedTotal")).ToString("0.00") %>' runat="server">
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="ReportAlternateItemTemplate">
              <td class="padLeft5">
                <%# Eval("WorkType.Name")%>
            </td>
            <td class="padLeft5">
             <%# Eval("WorkType.Category")%>
            </td>
            <asp:Repeater ID="repWorkTypeHoursPerDay" OnItemDataBound="repWorkTypeHoursPerDay_OnItemDataBound"
                runat="server">
                <ItemTemplate>
                    <td id="tdDayTotalHours" billabletotal='<%# ((double)Eval("Value.BillabileTotal")).ToString("0.00") %>'
                        nonbillabletotal='<%# ((double)Eval("Value.NonBillableTotal")).ToString("0.00") %>'
                        combinedtotal='<%# ((double)Eval("Value.CombinedTotal")).ToString("0.00") %>'
                        runat="server" class="textCenter">
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td id="tdPersonTotalHours" class="textCenter" billabletotal='<%# ((double)Eval("BillabileTotal")).ToString("0.00") %>'
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

