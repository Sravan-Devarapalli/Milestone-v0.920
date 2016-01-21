<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommissionsAndRates.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.CommissionsAndRates" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc2" %>
<div class="buttons-block marginBT10Px">
    <asp:HiddenField ID="hdnReset" runat="server" />
    <table>
        <tr>
            <td class="Width90Px">
                Select Dates
            </td>
            <td class="Width115Px">
                <uc2:MonthPicker ID="mpPeriodStart" runat="server" AutoPostBack="false" />
            </td>
            <td class="Width26Px textCenter">
                &nbsp;to&nbsp;
            </td>
            <td class="Width115Px">
                <uc2:MonthPicker ID="mpPeriodEnd" runat="server" AutoPostBack="false" />
            </td>
            <td class="Width20Px">
                <asp:CustomValidator ID="custPeriod" runat="server" ErrorMessage="The Period Start must be less than or equal to the Period End"
                    ToolTip="The Period Start must be less than or equal to the Period End" Text="*"
                    EnableClientScript="false" OnServerValidate="custPeriod_ServerValidate" ValidationGroup="Filter"></asp:CustomValidator>
                <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="false"
                    ErrorMessage="The period length must be not more then 24 months." ToolTip="The period length must be not more then 24 months."
                    Text="*" EnableClientScript="false" OnServerValidate="custPeriodLengthLimit_ServerValidate"
                    ValidationGroup="Filter"></asp:CustomValidator>
            </td>
            <td align="right" class="Width360Px">
                <asp:Button ID="btnReset" runat="server" Text="Reset Filter" CssClass="Width100PxImp"
                    OnClientClick="Delete_Cookie('CompanyPerformanceFilterKey', '/', '');"
                    OnClick="btnReset_Click" EnableViewState="False" />
            </td>
            <td align="right" class="Width110Px">
                <asp:Button ID="btnUpdateView" runat="server" Text="Update View" CssClass="Width100PxImp"
                    OnClick="btnUpdateView_Click" ValidationGroup="Filter" EnableViewState="False" />
            </td>
        </tr>
    </table>
</div>
<div class="overflowAuto">
    <asp:GridView ID="gvCommissionsAndRates" runat="server" AutoGenerateColumns="False"
        EmptyDataText="There is nothing to be displayed here." OnRowDataBound="gvCommissionsAndRates_RowDataBound"
        CssClass="CompPerfTable gvCommissionsAndRates" EnableViewState="False">
        <AlternatingRowStyle CssClass="alterrow" />
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="ie-bg no-wrap">
                        Commissions &amp; Rates</div>
                </HeaderTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server"></asp:Label>
                </ItemTemplate>
                <HeaderStyle CssClass="CompPerfDataTitle" />
                <ItemStyle CssClass="CompPerfDataTitle" />
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-CssClass="CompPerfTotalSummary" ItemStyle-CssClass="CompPerfTotalSummary fontBold no-wrap"
                Visible="false">
                <HeaderTemplate>
                    <div class="ie-bg">
                    </div>
                </HeaderTemplate>
                <ItemStyle CssClass="CompPerfTotalSummary fontBold no-wrap" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>

