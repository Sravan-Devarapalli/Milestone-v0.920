<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByResource" %>
<div>
    Show:
</div>
<asp:Repeater ID="repResource" runat="server" OnItemDataBound="repResource_ItemDataBound">
    <HeaderTemplate>
        <table>
            <tr>
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
                                <%# Eval("Date")%>
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
        <tr>
            <td>
                <%# Eval("person.PersonLastFirstName") %>
            </td>
            <td>
                <%# Eval("person.Seniority.Name") %>
            </td>
            <asp:Repeater ID="repResourceHoursPerDay" runat="server" OnItemDataBound="repResourceHoursPerDay_ItemDataBound">
                <ItemTemplate>
                    <td>
                        <%# Eval("Value") %>
                    </td>
                </ItemTemplate>
                <FooterTemplate>
                    <td>
                    </td>
                </FooterTemplate>
            </asp:Repeater>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>

