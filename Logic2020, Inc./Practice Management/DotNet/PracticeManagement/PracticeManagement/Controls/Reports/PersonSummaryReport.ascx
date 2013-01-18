<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonSummaryReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.PersonSummaryReport" %>
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" class="Width90Percent">
        </td>
        <td class="Width10Percent padRight5">
            <table class="WholeWidth">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To Excel" />
                    </td>
                    <td>
                        <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                            Enabled="false" UseSubmitBehavior="false" ToolTip="Export To PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Repeater ID="repSummary" runat="server" OnItemDataBound="repSummary_ItemDataBound">
    <HeaderTemplate>
        <table id="tblPersonSummaryReport" class="tablesorter TimePeriodByproject WholeWidth">
            <thead>
                <tr>
                    <th class="textLeft Width460Px">
                        Project Name
                    </th>
                    <th class="Width110Px">
                        Status
                    </th>
                    <th class="Width110Px">
                        Billing
                    </th>
                    <th class="Width110Px">
                        <asp:Label ID="lblProjectedHours" runat="server" Text="Projected Hours"></asp:Label>
                    </th>
                    <th class="Width100Px">
                        <asp:Label ID="lblBillable" runat="server" Text="Billable"></asp:Label>
                    </th>
                    <th class="Width100Px">
                        <asp:Label ID="lblNonBillable" runat="server" Text="Non-Billable"></asp:Label>
                    </th>
                    <th class="Width100Px">
                        <asp:Label ID="lblActualHours" runat="server" Text="Actual Hours"></asp:Label>
                    </th>
                    <th class="Width200Px">
                        <asp:Label ID="lblBillableHoursVariance" runat="server" Text="Billable Hours Variance"></asp:Label>
                    </th>
                    <th class="Width215Px">
                        Percent of Actual Hours this Period
                    </th>
                </tr>
            </thead>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="bgcolorwhite">
            <td sorttable_customkey='<%# Eval("Project.TimeEntrySectionId")%><%# Eval("Project.ProjectNumber")%>'
                class="textLeft">
                <table class="TdLevelNoBorder PeronSummaryReport">
                    <tr>
                        <td class="FirstTd">
                            <%# Eval("Client.HtmlEncodedName")%>
                            >
                            <%# Eval("Project.Group.HtmlEncodedName")%>
                        </td>
                    </tr>
                    <tr>
                        <td class="SecondTd">
                            <%# Eval("Project.ProjectNumber")%>
                            -
                            <%# Eval("Project.HtmlEncodedName")%>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <%# Eval("Project.Status.Name")%>
            </td>
            <td>
                <%# Eval("BillableType")%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("ProjectedHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
            </td>
            <td sorttable_customkey='<%# Eval("BillableHoursVariance") %>'>
                <table class="WholeWidth TdLevelNoBorder">
                    <tr>
                        <td class="Width50Percent textRightImp">
                            <%# ((double)Eval("BillableHoursVariance") > 0) ? "+" + GetDoubleFormat((double)Eval("BillableHoursVariance")) : GetDoubleFormat((double)Eval("BillableHoursVariance"))%>
                        </td>
                        <td class="Width50Percent t-left">
                            <asp:Label ID="lblExclamationMark" runat="server" Visible='<%# ((double)Eval("BillableHoursVariance") < 0)%>'
                                Text="!" CssClass="error-message fontSizeLarge" ToolTip="Project Underrun"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td sorttable_customkey='<%# Eval("ProjectTotalHoursPercent") %>'>
                <%# Eval("ProjectTotalHoursPercent")%>
            %
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="alterrow">
            <td sorttable_customkey='<%# Eval("Project.TimeEntrySectionId")%><%# Eval("Project.ProjectNumber")%>'
                class="textLeft">
                <table class="TdLevelNoBorder PeronSummaryReport">
                    <tr>
                        <td class="FirstTd">
                            <%# Eval("Client.HtmlEncodedName")%>
                            >
                            <%# Eval("Project.Group.HtmlEncodedName")%>
                        </td>
                    </tr>
                    <tr>
                        <td class="SecondTd">
                            <%# Eval("Project.ProjectNumber")%>
                            -
                            <%# Eval("Project.HtmlEncodedName")%>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <%# Eval("Project.Status.Name")%>
            </td>
            <td>
                <%# Eval("BillableType")%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("ProjectedHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
            </td>
            <td sorttable_customkey='<%# Eval("BillableHoursVariance") %>'>
                <table class="WholeWidth TdLevelNoBorder">
                    <tr>
                        <td class="Width50Percent textRightImp">
                            <%# ((double)Eval("BillableHoursVariance") > 0) ? "+" + GetDoubleFormat((double)Eval("BillableHoursVariance")) : GetDoubleFormat((double)Eval("BillableHoursVariance"))%>
                        </td>
                        <td class="Width50Percent t-left">
                            <asp:Label ID="lblExclamationMark" runat="server" Visible='<%# ((double)Eval("BillableHoursVariance") < 0)%>'
                                Text="!" CssClass="error-message fontSizeLarge" ToolTip="Project Underrun"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td sorttable_customkey='<%# Eval("ProjectTotalHoursPercent") %>'>
                <%# Eval("ProjectTotalHoursPercent")%>%
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
    This person has not entered Time Entries for the selected period.
</div>
<asp:Panel ID="pnlTotalProjectedHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Projected Hours:
    </label>
    <asp:Label ID="lblTotalProjectedHours" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalBillableHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Billable:
    </label>
    <asp:Label ID="lblTotalBillableHours" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalNonBillableHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Non-Billable:
    </label>
    <asp:Label ID="lblTotalNonBillableHours" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalActualHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <table>
        <tr>
            <td class="fontBold">
                Total Billable:
            </td>
            <td>
                <asp:Label ID="lblTotalBillablePanlActual" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Total Non-Billable:
            </td>
            <td>
                <asp:Label ID="lblTotalNonBillablePanlActual" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold padRight15">
                Total Actual Hours:
            </td>
            <td>
                <asp:Label ID="lblTotalActualHours" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlBillableHoursVariance" Style="display: none;" runat="server" CssClass="pnlTotal Width170PxImp">
    <label class="fontBold">
        Total Billable Hours Variance:
    </label>
    <br />
    <asp:Label ID="lblTotalBillableHoursVariance" runat="server"></asp:Label>
    <asp:Label ID="lblExclamationMarkPanl" runat="server" Visible="false" Text="!" CssClass="error-message fontSizeLarge t-left"
        ToolTip="Project Underrun"></asp:Label>
</asp:Panel>

