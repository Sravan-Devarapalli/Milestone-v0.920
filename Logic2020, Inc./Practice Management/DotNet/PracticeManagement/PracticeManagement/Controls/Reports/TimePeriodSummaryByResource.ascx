<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByResource" %>
<div class="PaddingBottom6 PaddingTop6 textCenter">
    Show:
    <asp:RadioButton ID="rbBillable" runat="server" GroupName="ByResource" Text="Billable" />
    <asp:RadioButton ID="rbNonBillable" runat="server" GroupName="ByResource" Text="NonBillable" />
    <asp:RadioButton ID="rbCombined" runat="server" GroupName="ByResource" Text="Combined Total" Checked="true" />
</div>
<asp:Repeater ID="repResource" runat="server" OnItemDataBound="repResource_ItemDataBound">
    <HeaderTemplate>
        <table class="CompPerfTable WholeWidth">
            <tr class="CompPerfHeader">
                <th>
                    <div class="ie-bg">
                        Resource
                    </div>
                </th>
                <th>
                    <div class="ie-bg">
                        Seniority
                    </div>
                </th>
                <asp:Repeater ID="repResourceHeaders" runat="server">
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
            <td class="padLeft5">
                <%# Eval("person.PersonLastFirstName") %>
            </td>
            <td class="padLeft5">
                <%# Eval("person.Seniority.Name") %>
            </td>
            <asp:Repeater ID="repResourceHoursPerDay" runat="server" OnItemDataBound="repResourceHoursPerDay_ItemDataBound">
            <HeaderTemplate></HeaderTemplate>
                <ItemTemplate>
                    <td class="textCenter">
                        <%# ((double)Eval("Value")).ToString("0.00") %>
                    </td>
                </ItemTemplate>
                <FooterTemplate>
                    <td class="textCenter">
                        <asp:Label ID="lblTotal" runat="server"></asp:Label>
                    </td>
                </FooterTemplate>
            </asp:Repeater>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="ReportAlternateItemTemplate">
            <td class="padLeft5">
                <%# Eval("person.PersonLastFirstName") %>
            </td>
            <td class="padLeft5">
                <%# Eval("person.Seniority.Name") %>
            </td>
            <asp:Repeater ID="repResourceHoursPerDay" runat="server" OnItemDataBound="repResourceHoursPerDay_ItemDataBound">
            <HeaderTemplate></HeaderTemplate>
                <ItemTemplate>
                    <td class="textCenter">
                        <%# ((double)Eval("Value")).ToString("0.00") %>
                    </td>
                </ItemTemplate>
                <FooterTemplate>
                    <td class="textCenter">
                        <asp:Label ID="lblTotal" runat="server"></asp:Label>
                    </td>
                </FooterTemplate>
            </asp:Repeater>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>

