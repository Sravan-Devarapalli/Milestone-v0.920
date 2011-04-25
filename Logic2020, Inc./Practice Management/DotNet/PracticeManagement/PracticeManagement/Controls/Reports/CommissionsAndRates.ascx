<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommissionsAndRates.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.CommissionsAndRates" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc2" %>
<div class="buttons-block" style="margin-bottom: 10px; margin-top: 10px;">
    <table>
    <tr>
        <td style="width: 90px">
            Select Dates
        </td>
        <td style="width: 115px">
            <uc2:monthpicker id="mpPeriodStart" runat="server" autopostback="false" />
        </td>
        <td style="width: 26px; text-align: center;">
            &nbsp;to&nbsp;
        </td>
        <td style="width: 115px">
            <uc2:monthpicker id="mpPeriodEnd" runat="server" autopostback="false" />
        </td>
        <td style="width: 20px">
            <asp:CustomValidator ID="custPeriod" runat="server" ErrorMessage="The Period Start must be less than or equal to the Period End"
                ToolTip="The Period Start must be less than or equal to the Period End" Text="*"
                EnableClientScript="false" OnServerValidate="custPeriod_ServerValidate" ValidationGroup="Filter"></asp:CustomValidator>
            <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="false"
                ErrorMessage="The period length must be not more then {0} months." ToolTip="The period length must be not more then {0} months."
                Text="*" EnableClientScript="false" OnServerValidate="custPeriodLengthLimit_ServerValidate"
                ValidationGroup="Filter"></asp:CustomValidator>
        </td>
        <td align="right" style="width: 360px">
            <asp:Button ID="btnReset" runat="server" Text="Reset Filter" Width="100px" CausesValidation="false"
                OnClientClick="this.disabled=true;Delete_Cookie('CompanyPerformanceFilterKey', '/', '');window.location.href=window.location.href;return false;"
                EnableViewState="False" />
        </td>
        <td align="right" style="width: 110px">
            <asp:Button ID="btnUpdateView" runat="server" Text="Update View" Width="100px" OnClick="btnUpdateView_Click"
                ValidationGroup="Filter" EnableViewState="False" />
        </td>
    </tr>
</table>
</div>
<div style="overflow:auto">
    <asp:GridView ID="gvCommissionsAndRates" runat="server" AutoGenerateColumns="False"
        EmptyDataText="There is nothing to be displayed here." OnRowDataBound="gvCommissionsAndRates_RowDataBound"
        CssClass="CompPerfTable" EnableViewState="False" GridLines="None" BackColor="White">
        <AlternatingRowStyle BackColor="#F9FAFF" />
        <Columns>
            <asp:TemplateField HeaderText="Commissions &amp; Rates">
                <HeaderTemplate>
                    <div class="ie-bg no-wrap">Commissions &amp; Rates</div>
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
            <asp:TemplateField ItemStyle-Font-Bold="True" ItemStyle-Wrap="False" HeaderStyle-CssClass="CompPerfTotalSummary"
                ItemStyle-CssClass="CompPerfTotalSummary" Visible="false">
                <HeaderTemplate>
                    <div class="ie-bg"></div>
                </HeaderTemplate>
                <ItemStyle CssClass="CompPerfTotalSummary" Font-Bold="True" Wrap="False" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>

