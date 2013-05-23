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
            ipleft = ipleft - 40;
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
                    <td class="Width72Percent">
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Show Report For:
                    </td>
                    <td class="textLeft PaddingLeft3Px">
                        <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" CssClass="Width91Per"
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
                        <asp:HiddenField ID="hdnPeriod" runat="server" Value="7" />
                    </td>
                    <td>
                        <div id="divCustomDates" runat="server" visible="false">
                            <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                            <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                            <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                            <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                        </div>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Practices:
                    </td>
                    <td class="textLeft PaddingLeft3Px">
                        <pmc:ScrollingDropDown ID="cblPractices" runat="server" AllSelectedReturnType="Null"
                            CssClass="ProjectDetailScrollingDropDown1 Width16point8PercentImp vMiddleImp"
                            OnSelectedIndexChanged="Filters_Changed" AutoPostBack="true" onclick="scrollingDropdown_onclick('cblPractices','Practice Area')"
                            CellPadding="3" NoItemsType="All" SetDirty="False" DropDownListType="Practice Area" />
                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                            Width="90.5%" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Account:
                    </td>
                    <td class="textLeft PaddingLeft3Px">
                        <pmc:ScrollingDropDown ID="cblAccount" runat="server" AllSelectedReturnType="Null"
                            CssClass="ProjectDetailScrollingDropDown1 Width16point8PercentImp vMiddleImp"
                            OnSelectedIndexChanged="Filters_Changed" AutoPostBack="true" onclick="scrollingDropdown_onclick('cblAccount','Account')"
                            CellPadding="3" NoItemsType="All" SetDirty="False" DropDownListType="Account" />
                        <ext:ScrollableDropdownExtender ID="sdeAccount" runat="server" TargetControlID="cblAccount"
                            Width="90.5%" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
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
            <asp:Panel ID="pnlNetPromoterScoreVariables" runat="server" CssClass="pnlNewHireHelp height120px Width415PxImp" Style="display: none;">
                <table class="Height100PerImp">
                    <tr class="trNetPromoterScorePanel">
                        <td class="Width40P vTop">
                            <table class="tableNetPromoterScorePanel">
                                <tr>
                                    <td colspan="2" align="center" class="PaddingTop0PxImp">
                                        <b>Variables</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center" class="PaddingTop0PxImp">
                                        &nbsp;&nbsp;
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
                                        # of Detractors
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
                        <td class="Width60P vTop Height100PerImp padLeft15">
                            <table class="WholeWidth Height100PerImp">
                                <tr class="vTop Height10P">
                                    <td align="center">
                                        <b>Calculation</b>
                                    </td>
                                </tr>
                                <tr class="Height50P">
                                    <td class="fontBold">
                                        <table class="tblPanelNetCSATScore">
                                            <tr class="WholeWidth">
                                                <td class="Width20PerImp padRight10">
                                                    <span class="textCenter">% of</span><br />
                                                    <span class="textCenter">Promoters</span>
                                                </td>
                                                <td class="Width5PercentImp">
                                                    <span class="font16PxImp">-</span>
                                                </td>
                                                <td class="Width20PerImp padRight10 padLeft10">
                                                    <span class="textCenter">% of</span><br />
                                                    <span class="textCenter">Detractors</span>
                                                </td>
                                                <td class="Width5PercentImp">
                                                    =&nbsp;
                                                </td>
                                                <td class="padLeft10 padRight5">
                                                    <span class="textCenter no-wrap">Net Promoter</span><br />
                                                    <span class="textCenter">Score</span>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr class="vTop">
                                    <td class="font14PxImp">
                                        <table class="tblPanelNetCSATScore WholeWidth">
                                            <tr>
                                                <td class="Width20PerImp padRight10 textCenter">
                                                    <asp:Label ID="lblPromotersPercentage" runat="server"></asp:Label>
                                                </td>
                                                <td class="Width5PercentImp">
                                                </td>
                                                <td class="Width20PerImp padRight10 padLeft10 textCenter">
                                                    <asp:Label ID="lblDetractersPercentage" runat="server"></asp:Label>
                                                </td>
                                                <td class="Width5PercentImp">
                                                    &nbsp;
                                                </td>
                                                <td class="padLeft10 padRight5 fontBold textCenter">
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
            <asp:Panel ID="pnlCSATCalculation" runat="server" Style="display: none;" CssClass="Width660PxImp Height160PxIMP pnlCSATCalculation">
                <asp:Image ID="imgCSATCalculation" runat="server" Width="100%" ImageUrl="~/Images/CSATCalculationPanel.jpg" />
            </asp:Panel>
            <div id="divReport" class="PaddingTop10Px" runat="server">
                <table id="tblHeader" runat="server">
                    <tr>
                        <td class="Width50Percent vTopImp font16Px fontBold">
                            <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                        </td>
                        <td class="CSATHeader NewHireReportTotals">
                            <table class="tblCSATHeader tableFixed">
                                <tr>
                                    <td style="width:23%;">
                                    </td>
                                    <td>
                                        <asp:Label Text="Score" ID="lblNetPromoterScoreHeader" runat="server" CssClass="font16Px"></asp:Label><br />
                                        <span class="fontSize11px">(All Company)</span>
                                        <asp:Image alt="Net Promoter Score Hint" ImageUrl="~/Images/hint1.png" runat="server"
                                            ID="imgNetPromoterScoreWithoutFilters" />
                                    </td>
                                    <td style="width:27%;">
                                        <asp:Label Text="Score" runat="server" ID="lblNetPromoterScoreWithFiltersHeader"
                                            CssClass="font16Px"></asp:Label><br />
                                        <span class="fontSize11px">(Based on Selected Filters)</span>
                                        <asp:Image alt="Net Promoter Score Hint" ImageUrl="~/Images/hint1.png" runat="server"
                                            ID="imgNetPromoterScoreWithFilters" />
                                    </td>
                                    <td class="font15PxImp vBottomImp">
                                        # of Completed
                                        <br />
                                        CSATs
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblNetPromoterScoreAllCompany" CssClass="FontSize20PXImp" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblNetPromoterScoreBasedOnFilters" CssClass="FontSize20PXImp" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblCompletedCSATs" CssClass="FontSize20PXImp" runat="server"></asp:Label>
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

