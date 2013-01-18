<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectSummaryTabByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectSummaryTabByResource" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<script type="text/javascript">
    function OpenUrlInNewWindow(linkButton) {
        var NavigationUrl = linkButton.getAttribute("NavigationUrl");
        window.open(NavigationUrl);
    }
</script>
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
                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Panel ID="pnlFilterProjectRoles" Style="display: none;" runat="server">
    <uc:FilteredCheckBoxList ID="cblProjectRoles" runat="server" />
</asp:Panel>
<asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_OnClick" Style="display: none;" />
<asp:Repeater ID="repResource" runat="server" OnItemDataBound="repResource_ItemDataBound">
    <HeaderTemplate>
        <div class="minheight250Px">
            <table id="tblProjectSummaryByResource" class="tablesorter PersonSummaryReport WholeWidth">
                <thead>
                    <tr>
                        <th class="ProjectColum">
                            Resource
                        </th>
                        <th class="Width140px">
                        </th>
                        <th class="Width130px">
                            Project Role
                            <img alt="Filter" title="Filter" src="../../Images/search_filter.png" runat="server"
                                id="imgProjectRoleFilter" class="FilterImg" />
                            <AjaxControlToolkit:PopupControlExtender ID="pceProjectRole" runat="server" TargetControlID="imgProjectRoleFilter"
                                PopupControlID="pnlFilterProjectRoles" Position="Bottom">
                            </AjaxControlToolkit:PopupControlExtender>
                        </th>
                        <th class="Width125Px">
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
                    </tr>
                </thead>
                <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="ReportItemTemplate">
            <td class="t-left padLeft5">
                <%# Eval("Person.PersonLastFirstName")%>
                <asp:Image ID="imgOffshore" runat="server" ImageUrl="~/Images/Offshore_Icon.png"
                    ToolTip="Resource is an offshore employee" Visible='<%# (bool)Eval("Person.IsOffshore")%>' />
            </td>
            <td>
            </td>
            <td class="t-center padLeft5">
                <%# Eval("Person.ProjectRoleName")%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("ForecastedHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td>
                <asp:LinkButton ID="lnkActualHours" runat="server" ToolTip='<%# GetDoubleFormat((double)Eval("TotalHours"))%>'
                    OnClientClick="OpenUrlInNewWindow(this);return false;" Text='<%# GetDoubleFormat((double)Eval("TotalHours"))%>'
                    target="_blank"></asp:LinkButton>
            </td>
            <td sorttable_customkey='<%# Eval("BillableHoursVariance") %>'>
                <table class="WholeWidth TdLevelNoBorder">
                    <tr>
                        <td class="Width50Percent textRightImp">
                            <%#((double)Eval("BillableHoursVariance") > 0) ? "+" + GetDoubleFormat((double)Eval("BillableHoursVariance")) : GetDoubleFormat((double)Eval("BillableHoursVariance"))%>
                        </td>
                        <td class="Width50Percent t-left">
                            <asp:Label ID="lblExclamationMark" runat="server" Visible='<%# ((double)Eval("BillableHoursVariance") < 0)%>'
                                Text="!" CssClass="error-message fontSizeLarge" ToolTip="Project Underrun"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table></div>
    </FooterTemplate>
</asp:Repeater>
<br />
<div id="divEmptyMessage" style="display: none;" class="EmptyMessagediv" runat="server">
    There are no Time Entries towards this project.
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

