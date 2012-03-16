<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectSummaryByMatrix.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectSummaryByMatrix" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.BillableNonBillableAndTotal"
    Assembly="PraticeManagement" %>
<div class="PaddingBottom6 PaddingTop6 textCenter">
    Show:
    <input type="radio" id="rbBillable" runat="server" name="ByMatrix" displayvaluetype="billabletotal" />
    Billable
    <input type="radio" id="rbNonBillable" runat="server" name="ByMatrix" displayvaluetype="nonbillabletotal" />
    Non-Billable
    <input type="radio" id="rbCombined" runat="server" checked="true" name="ByMatrix"
        displayvaluetype="combinedtotal" />
    Combined Total
</div>
<asp:Repeater ID="repMatrix" runat="server" OnItemDataBound="repMatrix_ItemDataBound">
    <HeaderTemplate>
        <table class="PersonSummaryReport WholeWidth">
            <tr>
                <th>
                    Resource
                </th>
                <th>
                    Project Role
                </th>
                <asp:Repeater ID="repMatrixHeaders" runat="server">
                    <ItemTemplate>
                        <th>
                            <%# Eval("Name")%>
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
                <%# Eval("person.PersonLastFirstName") %>
            </td>
            <td class="padLeft5">
            </td>
            <asp:Repeater ID="repMatrixHoursPerWorkType" OnItemDataBound="repMatrixHoursPerWorkType_OnItemDataBound"
                runat="server">
                <ItemTemplate>
                    <td id="tdWorkTypeTotalHours" billabletotal='<%# ((double)Eval("Value.BillabileHours")).ToString("0.00") %>'
                        nonbillabletotal='<%# ((double)Eval("Value.NonBillabileHours")).ToString("0.00") %>'
                        combinedtotal='<%# ((double)Eval("Value.TotalHours")).ToString("0.00") %>'
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
                <%# Eval("person.PersonLastFirstName") %>
            </td>
            <td class="padLeft5">
            </td>
            <asp:Repeater ID="repMatrixHoursPerWorkType" OnItemDataBound="repMatrixHoursPerWorkType_OnItemDataBound"
                runat="server">
                <ItemTemplate>
                    <td id="tdWorkTypeTotalHours" billabletotal='<%# ((double)Eval("Value.BillabileHours")).ToString("0.00") %>'
                        nonbillabletotal='<%# ((double)Eval("Value.NonBillabileHours")).ToString("0.00") %>'
                        combinedtotal='<%# ((double)Eval("Value.TotalHours")).ToString("0.00") %>'
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

