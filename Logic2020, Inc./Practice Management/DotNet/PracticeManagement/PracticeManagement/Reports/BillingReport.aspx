<%@ Page Title="Billing" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="BillingReport.aspx.cs" Inherits="PraticeManagement.Reports.BillingReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/BillingReportSummary.ascx" TagPrefix="uc" TagName="summary" %>
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
<script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblBillingReport").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        });


        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            $("#tblBillingReport").tablesorter(
                {
                    sortList: [[0, 0]],
                    sortForce: [[0, 0]]
                });
        }
    </script>
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
                        Projected Range:&nbsp;
                    </td>
                    <td class="textLeft">
                        <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true" ValidationGroup="valRange" IsBillingReport="true"
                            FromToDateFieldCssClass="Width70Px" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Practices:&nbsp;
                    </td>
                    <td class="textLeft Width90Percent">
                        <pmc:ScrollingDropDown ID="cblPractices" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            NoItemsType="All" onclick="scrollingDropdown_onclick('cblPractices','Practice')"
                            OnSelectedIndexChanged="cblPractices_SelectedIndexChanged" AutoPostBack="true"
                            DropDownListType="Practice" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv" />
                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                            UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P WholeWidth">
                    <td class="ReportFilterLabels">
                        Account:&nbsp;
                    </td>
                    <td class="textLeft Width90Percent">
                    <span>
                        <pmc:ScrollingDropDown ID="cblAccount" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            OnSelectedIndexChanged="cblAccount_SelectedIndexChanged" NoItemsType="All" onclick="scrollingDropdown_onclick('cblAccount','Account')"
                            AutoPostBack="true" DropDownListType="Account" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv" />
                        <ext:ScrollableDropdownExtender ID="sdeAccount" runat="server" TargetControlID="cblAccount"
                            UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender></span>
                        &nbsp;&nbsp;&nbsp;&nbsp; <span class="fontBold">Business Unit:</span> &nbsp;
                        <span>
                        <pmc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            OnSelectedIndexChanged="cblProjectGroup_OnSelectedIndexChanged" NoItemsType="All"
                            onclick="scrollingDropdown_onclick('cblProjectGroup','Business Unit')" AutoPostBack="true"
                            DropDownListType="Business Unit" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv" />
                        <ext:ScrollableDropdownExtender ID="sdeProjectGroup" runat="server" TargetControlID="cblProjectGroup"
                            UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                        </span>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Executive in Charge:&nbsp;
                    </td>
                    <td class="textLeft Width90Percent">
                        <pmc:ScrollingDropDown ID="cblDirector" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            OnSelectedIndexChanged="cblDirector_OnSelectedIndexChanged" NoItemsType="All" DropDownListTypePluralForm="Executives in Charge" DropdownListFirst="Executive" DropdownListSecond="in Charge"
                            onclick="scrolling_onclick('cblDirector','Executive in Charge','s','Executives in Charge',33,'Executive','in Charge')" AutoPostBack="true"
                            CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv" />
                        <ext:ScrollableDropdownExtender ID="sdeDirector" runat="server" TargetControlID="cblDirector"
                            UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Unit of Measure:&nbsp;
                    </td>
                    <td class="textLeft">
                        <asp:DropDownList ID="ddlMeasureUnit" runat="server" CssClass="Width220Px">
                            <asp:ListItem Selected="True" Text="Dollars" Value="Dollars"></asp:ListItem>
                            <asp:ListItem Text="Hours" Value="Hours"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btnUpdateView" Text="View Report" runat="server" OnClick="btnUpdateView_Click" />
                    </td>
                </tr>
                <tr class="height30P">
                    <td colspan="3">
                    &nbsp;
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
                    &nbsp;
                    </td>
                </tr>
            </table>
            
            <br />
            <div id="divWholePage" runat="server" style="display: none">
                <table>
                    <tr>
                        <td style="font-weight:bold; font-size:16px;">
                            Projected Range:
                            (<asp:Label ID="lblRange" runat="server"></asp:Label>)
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
                <asp:MultiView ID="mvAccountReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwBusinessUnitReport" runat="server">
                        <asp:Panel ID="pnlBusinessUnitReport" runat="server" CssClass="WholeWidth">
                            <uc:summary ID="billingSummary" runat="server" />
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="billingSummary$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

