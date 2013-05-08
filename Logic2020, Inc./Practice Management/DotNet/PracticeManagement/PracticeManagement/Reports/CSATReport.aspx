<%@ Page Title="CSAT Net Promoter Score | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="CSATReport.aspx.cs" Inherits="PraticeManagement.Reports.CSATReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/CSAT/CSATSummaryReport.ascx" TagPrefix="uc"
    TagName="CSATSummary" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $("#tblCSATSummary").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        });

        function pageLoad() {
            SetTooltipsForallDropDowns();

        }

        function SetTooltipsForallDropDowns() {
            var optionList = document.getElementsByTagName('option');
            for (var i = 0; i < optionList.length; ++i) {
                optionList[i].title = optionList[i].innerHTML;
            }
        }
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            SetTooltipsForallDropDowns();
            $("#tblCSATSummary").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        }
        function ShowCalculationPanel(imgClientId, pnlClientId, promoters, passives, detracters, totalCSATs, promotersPercent, detractersPercent, netCSATScore) {
            var lblPromoters = document.getElementById('<%= lblPromoters.ClientID %>');
            var lblPassives = document.getElementById('<%= lblPassives.ClientID %>');
            var lblDetracters = document.getElementById('<%= lblDetracters.ClientID %>');
            var lblPromotersPercentage = document.getElementById('<%= lblPromotersPercentage.ClientID %>');
            var lblDetractersPercentage = document.getElementById('<%= lblDetractersPercentage.ClientID %>');
            var lblNetPromoterScore = document.getElementById('<%= lblNetPromoterScore.ClientID %>');
            var lblTotalCSATs = document.getElementById('<%= lblTotalCSATs.ClientID %>');

            lblPromoters.innerHTML = promoters;
            lblPassives.innerHTML = passives;
            lblDetracters.innerHTML = detracters;
            lblTotalCSATs.innerHTML = totalCSATs;
            lblNetPromoterScore.innerHTML = netCSATScore;
            lblPromotersPercentage.innerHTML = promotersPercent + '%';
            lblDetractersPercentage.innerHTML = detractersPercent + '%';
            ShowPanel(imgClientId, pnlClientId);
        }

        function ShowPanel(imgClientId, pnlClientId) {
            var totalPageWidth = $(window).width();
            var obj = $('#' + imgClientId);
            var displayPanel = $('#' + pnlClientId);
            iptop = obj.offset().top + obj[0].offsetHeight;
            ipleft = obj.offset().left;
            var axisLength = ((displayPanel.width() + ipleft) - totalPageWidth);
            if (axisLength > 1) {
                ipleft = ipleft - (axisLength + 30);
            }
            displayPanel.offset({ top: iptop, left: ipleft });
            displayPanel.show();
            displayPanel.offset({ top: iptop, left: ipleft });
        }

        function HidePanel(pnlClientId) {
            var displayPanel = $('#' + pnlClientId);
            displayPanel.hide();
        }
    </script>
    <uc:LoadingProgress ID="PleaseWaitImage" runat="server" />
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
                        Show Report For:
                    </td>
                    <td class="textLeft">
                        <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" CssClass="Width50Percent"
                            OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                            <asp:ListItem Text="Please Select" Value="-1"></asp:ListItem>
                            <asp:ListItem Text="This Month" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Last Month" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Q1" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Q2" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Q3" Value="5"></asp:ListItem>
                            <asp:ListItem Text="Q4" Value="6"></asp:ListItem>
                            <asp:ListItem Text="Year To Date" Value="7" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:HiddenField ID="hdnPeriod" runat="server" Value="7"/>
                    </td>
                </tr>
                <tr id="trCustomDates" runat="server" visible="false">
                    <td>
                    </td>
                    <td class="textLeft height30P">
                        <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                        <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                        <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                        <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Practices:
                    </td>
                    <td class="textLeft">
                        <pmc:ScrollingDropDown ID="cblPractices" runat="server" AllSelectedReturnType="Null"
                            CssClass="ProjectDetailScrollingDropDown1 Width15point3PercentImp vMiddleImp"
                            OnSelectedIndexChanged="Filters_Changed" AutoPostBack="true" onclick="scrollingDropdown_onclick('cblPractices','Practice Area')"
                            CellPadding="3" NoItemsType="All" SetDirty="False" DropDownListType="Practice Area" />
                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                            Width="50%" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Account:
                    </td>
                    <td class="textLeft">
                        <pmc:ScrollingDropDown ID="cblAccount" runat="server" AllSelectedReturnType="Null"
                            CssClass="ProjectDetailScrollingDropDown1 Width15point3PercentImp vMiddleImp"
                            OnSelectedIndexChanged="Filters_Changed" AutoPostBack="true" onclick="scrollingDropdown_onclick('cblAccount','Account')"
                            CellPadding="3" NoItemsType="All" SetDirty="False" DropDownListType="Account" />
                        <ext:ScrollableDropdownExtender ID="sdeAccount" runat="server" TargetControlID="cblAccount"
                            Width="50%" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                </tr>
            </table>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                DropShadow="false" />
            <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass CustomDatesPopUp" Style="display: none;">
                <table class="WholeWidth">
                    <tr>
                        <td align="center">
                            <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                FromToDateFieldCssClass="Width70Px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="custBtns">
                            <asp:Button ID="btnCustDatesOK" runat="server" OnClick="btnCustDatesOK_Click" Text="OK"
                                CausesValidation="true" />
                            &nbsp; &nbsp;
                            <asp:Button ID="btnCustDatesCancel" CausesValidation="false" runat="server" Text="Cancel"
                                OnClick="btnCustDatesCancel_OnClick" />
                        </td>
                    </tr>
                    <tr>
                        <td class="textCenter">
                            <asp:ValidationSummary ID="valSumDateRange" runat="server" ValidationGroup='<%# ClientID %>' />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table class="WholeWidth">
                <tr>
                    <td colspan="2" class="ReportBorderBottomHeight15Px PaddingBottom10Px">
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlNetPromoterScoreVariables" runat="server" Style="display: none;"
                CssClass="pnlNewHireHelp height120px Width400PxImp">
                <table>
                    <tr class="trNetPromoterScorePanel">
                        <td class="Width40P">
                            <table class="tableNetPromoterScorePanel">
                                <tr>
                                    <td colspan="2" align="center">
                                        <b>Variables</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of Promoters
                                    </td>
                                    <td class="Width10PerImp">
                                        <asp:Label ID="lblPromoters" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of Passives
                                    </td>
                                    <td class="Width10PerImp">
                                        <asp:Label ID="lblPassives" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of Detracters
                                    </td>
                                    <td class="Width10PerImp">
                                        <asp:Label ID="lblDetracters" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of Completed CSATs
                                    </td>
                                    <td class="Width10PerImp">
                                        <asp:Label ID="lblTotalCSATs" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="Width60P Height100PerImp">
                            <table class="WholeWidth Height100PerImp">
                                <tr class="PaddingTop5Imp">
                                    <td align="center">
                                        <b>Calculation</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fontBold">
                                        <table class="tblPanelNetCSATScore">
                                            <tr class="WholeWidth">
                                                <td>
                                                    % of Promoters
                                                </td>
                                                <td class="Width5PercentImp">
                                                    -
                                                </td>
                                                <td>
                                                    % of Detractors
                                                </td>
                                                <td class="Width5PercentImp">
                                                    =
                                                </td>
                                                <td>
                                                    Net Promoter Score
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fontBold">
                                        <table class="tblPanelNetCSATScore WholeWidth">
                                            <tr>
                                                <td class="font14PxImp">
                                                    <asp:Label ID="lblPromotersPercentage" runat="server"></asp:Label>
                                                </td>
                                                <td class="Width5PercentImp">
                                                </td>
                                                <td class="font14PxImp">
                                                    <asp:Label ID="lblDetractersPercentage" runat="server"></asp:Label>
                                                </td>
                                                <td class="Width5PercentImp">
                                                </td>
                                                <td class="font14PxImp">
                                                    <asp:Label ID="lblNetPromoterScore" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlCSATCalculation" runat="server" Style="display: none;" CssClass="pnlNewHireHelp height120px Width400PxImp">
                <asp:Image ID="imgCSATCalculation" runat="server" Width="100%" ImageUrl="~/Images/CSATCalculation.png" />
            </asp:Panel>
            <div id="divReport" style="padding-top:10px;" runat="server">
                <table id="tblHeader" runat="server">
                    <tr>
                        <td class="Width50Percent vTopImp font16Px fontBold">
                            <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                        </td>
                        <td class="CSATHeader NewHireReportTotals">
                            <table class="tableFixed WholeWidthTable">
                                <tr>
                                    <td class="Width25Percent">
                                    </td>
                                    <td class="Width25Percent">
                                        <table class="ReportHeaderTotalsTable">
                                            <tr>
                                                <td class="FirstTd fontBold">
                                                    <asp:Label Text="Net Promoter Score" ID="lblNetPromoterScoreHeader" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FirstTd fontBold">
                                                    (All Company)
                                                    <asp:Image alt="Net Promoter Score Hint" ImageUrl="~/Images/hint1.png" runat="server"
                                                        ID="imgNetPromoterScoreWithoutFilters" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="SecondTd">
                                                    <asp:Literal ID="ltrlNetPromoterScoreAllCompany" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width25Percent">
                                        <table class="ReportHeaderTotalsTable">
                                            <tr>
                                                <td class="FirstTd fontBold">
                                                    <asp:Label Text="Net Promoter Score" runat="server" ID="lblNetPromoterScoreWithFiltersHeader"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FirstTd fontBold">
                                                    (Based on Selected Filters)
                                                    <asp:Image alt="Net Promoter Score Hint" ImageUrl="~/Images/hint1.png" runat="server"
                                                        ID="imgNetPromoterScoreWithFilters" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="SecondTd">
                                                    <asp:Literal ID="ltrlNetPromoterScoreBasedOnFilters" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width25Percent">
                                        <table class="ReportHeaderTotalsTable">
                                            <tr>
                                                <td class="FirstTd fontBold">
                                                    Number of Completed
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FirstTd fontBold">
                                                    CSATs
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="SecondTd">
                                                    <asp:Literal ID="ltrlCompletedCSATs" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblCSATViewSwitch" runat="server" CssClass="CommonCustomTabStyle AccountSummaryReportCustomTabStyle">
                    <asp:TableRow ID="rowSwitcher" runat="server">
                        <asp:TableCell ID="cellSummary" CssClass="SelectedSwitch" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="0" ToolTip="Summary"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:MultiView ID="mvCSATReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwCSATReport" runat="server">
                        <asp:Panel ID="pnlCSATSummary" runat="server" CssClass="WholeWidth">
                            <uc:CSATSummary ID="ucCSATSummary" runat="server" />
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ucCSATSummary$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

