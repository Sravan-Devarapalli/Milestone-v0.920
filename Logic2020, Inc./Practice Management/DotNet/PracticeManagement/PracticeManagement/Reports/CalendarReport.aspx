<%@ Page Title="Time Off" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="CalendarReport.aspx.cs" Inherits="PraticeManagement.Reports.CalendarReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/CalendarReport.ascx" TagPrefix="uc" TagName="CalendarReport" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr class="height30P">
                    <td class="vBottom fontBold Width3Percent no-wrap">
                        &nbsp;Select report parameters:&nbsp;
                    </td>
                    <td>
                    </td>
                    <td class="width60P">
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Range:&nbsp;
                    </td>
                    <td class="textLeft">
                        <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                            ValidationGroup="valRange" IsBillingReport="true" FromToDateFieldCssClass="Width70Px" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Project Status:&nbsp;
                    </td>
                    <td class="textLeft Width90Percent">
                        <pmc:ScrollingDropDown ID="cblProjectStatus" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            NoItemsType="All" onclick="scrollingDropdown_onclick('cblProjectStatus','Project Status','es')"
                            AutoPostBack="true" PluralForm="es" DropDownListType="Project Status" CellPadding="3"
                            CssClass="AccountSummaryBusinessUnitsDiv" />
                        <ext:ScrollableDropdownExtender ID="sdeProjectStatus" runat="server" TargetControlID="cblProjectStatus"
                            UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                    </td>
                    <td class="textLeft">
                        <asp:CheckBox ID="chbIncludeCompanyHolidays" runat="server" />
                        <asp:Label Text="Include Company Holidays" ID="lblCompanyHolidaysCheckbox" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Button ID="btnUpdateView" Text="View Report" runat="server" OnClick="btnUpdateView_Click" />
                    </td>
                </tr>
                <tr class="height30P">
                    <td colspan="2">
                        <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="valRange" ShowMessageBox="false"
                            ShowSummary="true" EnableClientScript="false" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="ReportBorderBottomByAccount">
                    <td colspan="3">
                    </td>
                </tr>
            </table>
            <br />
            <div id="divWholePage" runat="server">
                <table>
                    <tr>
                        <td style="font-weight: bold; font-size: 16px;">
                            Range: (<asp:Label ID="lblRange" runat="server"></asp:Label>)
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblProjectViewSwitch" runat="server" CssClass="CommonCustomTabStyle AccountSummaryReportCustomTabStyle">
                    <asp:TableRow ID="rowSwitcher" runat="server">
                        <asp:TableCell ID="cellSummary" CssClass="SelectedSwitch" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="0" ToolTip="Summary"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:MultiView ID="mvCalendarReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwSummary" runat="server">
                        <asp:Panel ID="pnlSummary" runat="server" CssClass="WholeWidth">
                            <uc:CalendarReport ID="tpCalSummary" runat="server"></uc:CalendarReport>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="tpCalSummary$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

