<%@ Page Title="ConsultingDemand New" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ConsultingDemand_New.aspx.cs" Inherits="PraticeManagement.Reports.ConsultingDemand_New" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/Reports/ConsultantDemand/ConsultingDemandSummary.ascx"
    TagPrefix="uc" TagName="SummaryView" %>
<%@ Register Src="~/Controls/Reports/ConsultantDemand/ConsultingDemandDetails.ascx"
    TagPrefix="uc1" TagName="DetailsView" %>
<%@ Register Src="~/Controls/Reports/ConsultantDemand/ConsultingDemandGraphs.ascx"
    TagPrefix="uc2" TagName="GraphsView" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
    <script src='<%# Generic.GetClientUrl("~/Scripts/ExpandOrCollapse.min.js", this) %>'
        type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function ShowPanel(object, displaypnl, position) {

            var obj = $("#" + object);
            var displayPanel = $("#" + displaypnl);
            iptop = obj.offset().top + obj[0].offsetHeight;
            ipleft = obj.offset().left - position;
            displayPanel.offset({ top: iptop, left: ipleft });
            displayPanel.show();
            displayPanel.offset({ top: iptop, left: ipleft });
        }
        function ValidatePeriod(sender, args) {
            args.IsValid = PeriodValidate();
        }

        function ValidAll() {
            //var validators = Page_Validators;
            return Page_IsValid && PeriodValidate();
        }

        function CheckIfDatesValid() {
            hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
            hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
            var startDate = new Date(txtStartDate.value);
            var endDate = new Date(txtEndDate.value);
            if (txtStartDate.value != '' && txtEndDate.value != ''
        && startDate <= endDate) {
                var startYear = parseInt(startDate.format('yyyy'));
                var endYear = parseInt(endDate.format('yyyy'));
                var startMonth = 0;
                var endMonth = 0;
                if (startDate.format('MM')[0] == '0') {
                    startMonth = parseInt(startDate.format('MM')[1]);
                }
                else {
                    startMonth = parseInt(startDate.format('MM'));
                }
                if (endDate.format('MM')[0] == '0') {
                    endMonth = parseInt(endDate.format('MM')[1]);
                }
                else {
                    endMonth = parseInt(endDate.format('MM'));
                }
                if ((startYear == endYear && ((endMonth - startMonth + 1) <= 4))
        || (((((endYear - startYear) * 4 + endMonth) - startMonth + 1)) <= 4)
        || ((endDate - startDate) / (1000 * 60 * 60 * 24)) < 360
        ) {
                    ClearValidations();
                    var btnCustDatesClose = document.getElementById('<%= btnCustDatesCancel.ClientID %>');
                    hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
                    hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
                    lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
                    var startDate = new Date(txtStartDate.value);
                    var endDate = new Date(txtEndDate.value);
                    var startDateStr = startDate.format("MM/dd/yyyy");
                    var endDateStr = endDate.format("MM/dd/yyyy");
                    hdnStartDate.value = startDateStr;
                    hdnEndDate.value = endDateStr;
                    lblCustomDateRange.innerHTML = '(' + startDateStr + '&nbsp;-&nbsp;' + endDateStr + ')';
                    btnCustDatesClose.click();
                }
            }
        }

        function PeriodValidate() {
            hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
            hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
            ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
            var startDate = new Date(txtStartDate.value);
            var endDate = new Date(txtEndDate.value);
            if (txtStartDate.value != '' && txtEndDate.value != ''
        && startDate <= endDate && ddlPeriod.value == '0') {
                var startYear = parseInt(startDate.format('yyyy'));
                var endYear = parseInt(endDate.format('yyyy'));
                var startMonth = 0;
                var endMonth = 0;
                if (startDate.format('MM')[0] == '0') {
                    startMonth = parseInt(startDate.format('MM')[1]);
                }
                else {
                    startMonth = parseInt(startDate.format('MM'));
                }
                if (endDate.format('MM')[0] == '0') {
                    endMonth = parseInt(endDate.format('MM')[1]);
                }
                else {
                    endMonth = parseInt(endDate.format('MM'));
                }
                if (startYear == endYear) {
                    return ((endMonth - startMonth + 1) <= 4);
                }
                else {
                    return (((((endYear - startYear) * 4 + endMonth) - startMonth + 1)) <= 4);
                }
                if (((endDate - startDate) / (1000 * 60 * 60 * 24)) < 360) {
                    return true; //args.IsValid = true;
                }
            }
            else {
                return true;  //args.IsValid = true;
            }

            return false;
        }

        function HidePanel(hiddenpnl) {

            var displayPanel = $("#" + hiddenpnl);
            displayPanel.hide();
        }

        $(document).ready(function () {
            $("#tblConsultingDemandSummary").tablesorter(
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
            imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
            if (imgCalender.fireEvent && ddlPeriod.value != '0') {
                imgCalender.style.display = "none";
                lblCustomDateRange.style.display = "none";
            }
            SetTooltipsForallDropDowns();
            $("#tblConsultingDemandSummary").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <div class="filter-section-color Height102px">
                <asp:UpdatePanel ID="upnlHeader" runat="server">
                    <ContentTemplate>
                        <table style="width: 100%;">
                            <tr>
                                <td style="font-weight: bolder; width: 10%; text-align: right;">
                                    Select report parameters:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                                <td style="width: 50%;">
                                </td>
                                <td style="text-align: right;">
                                    <asp:Button ID="btnUpdateView" Text="Run Report" runat="server" OnClick="btnUpdateView_OnClick"
                                        Enabled="false" CssClass="textCenter Width110Px" />
                                    <asp:Button ID="btnResetFilter" Text="Reset Filter" runat="server" Enabled="false"
                                        OnClick="btnResetFilter_OnClick" CssClass="textCenter Width110Px" />
                                </td>
                            </tr>
                            <tr id="trGtypes" runat="server" class="PaddingBottom5Imp">
                                <td style="text-align: right;">
                                    Graph Type&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                                <td class="PaddingBottom3Imp">
                                    <asp:DropDownList runat="server" ID="ddlGraphsTypes" OnSelectedIndexChanged="ddlGraphsTypes_SelectedIndexChanged"
                                        CssClass="Width50Percent" AutoPostBack="true">
                                        <asp:ListItem Text="--Please Select--" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Resource View By Title" Value="TransactionTitle" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Resource View By Skill" Value="TransactionSkill"></asp:ListItem>
                                        <asp:ListItem Text="Resource View By Month" Value="PipeLine"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="hdnGraphType" runat="server" />
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr id="trTitles" runat="server">
                                <td style="text-align: right;">
                                    <asp:Label ID="lblTitle" runat="server"></asp:Label>
                                    <asp:HiddenField ID="hdnPipelineTitleOrSkill" runat="server" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                                <td class="PaddingBottom3Imp">
                                    <table class="Width50Percent" id="tdTitles" runat="server">
                                        <tr>
                                            <td>
                                                <cc2:ScrollingDropDown ID="cblTitles" runat="server" SetDirty="true" AllSelectedReturnType="AllItems"
                                                    OnSelectedIndexChanged="cblTitles_SelectedIndexChanged" CssClass="ProjectDetailScrollingDropDown1  Width100Per"
                                                    onclick="scrollingDropdown_onclick('cblTitles','Title','s','Titles',28);" AutoPostBack="true"
                                                    DropDownListType="Tilte" DropDownListTypePluralForm="Titles" PluralForm="s" />
                                                <ext:ScrollableDropdownExtender ID="sdeTitles" runat="server" TargetControlID="cblTitles"
                                                    BehaviorID="sdeTitles" MaxNoOfCharacters="28" Width="99%" UseAdvanceFeature="true"
                                                    EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                </ext:ScrollableDropdownExtender>
                                                <asp:HiddenField ID="hdnTitles" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="Width50Percent" id="tdSkills" runat="server">
                                        <tr>
                                            <td>
                                                <cc2:ScrollingDropDown ID="cblSkills" runat="server" SetDirty="true" AllSelectedReturnType="AllItems"
                                                    CssClass="ProjectDetailScrollingDropDown1 Width100Per" onclick="scrollingDropdown_onclick('cblSkills','Skill','s','Skills',28);"
                                                    DropDownListType="Skill" DropDownListTypePluralForm="Skills" PluralForm="s" />
                                                <ext:ScrollableDropdownExtender ID="sdeSkills" runat="server" TargetControlID="cblSkills"
                                                    BehaviorID="sdeSkills" MaxNoOfCharacters="28" Width="99%" UseAdvanceFeature="true"
                                                    EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                </ext:ScrollableDropdownExtender>
                                                <asp:HiddenField ID="hdnSkills" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr class="PaddingBottom5Imp">
                                <td style="text-align: right;">
                                    Period&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                                <td class="PaddingBottom3Imp">
                                    <asp:DropDownList runat="server" ID="ddlPeriod" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged"
                                        CssClass="Width50Percent" AutoPostBack="true">
                                        <asp:ListItem Text="Please Select" Value="-1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Current Month" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Next 2 Months" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Next 3 Months" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Next 4 Months" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="hdnPeriodValue" runat="server" />
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnStartDateTxtBoxId" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDateTxtBoxId" runat="server" Value="" />
                                    <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                                        BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                                        DropShadow="false" />
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                        <asp:Panel ID="pnlCustomDates" runat="server" CssClass="ConfirmBoxClass CustomDatesPopUp"
                            Style="display: none;">
                            <table class="WholeWidth">
                                <tr>
                                    <td align="center" class="no-wrap">
                                        <table>
                                            <tr>
                                                <td>
                                                    <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                                        FromToDateFieldCssClass="Width70Px" />
                                                </td>
                                                <td>
                                                    <asp:CustomValidator ID="cstvalPeriodRange" runat="server" ClientValidationFunction="ValidatePeriod"
                                                        Text="*" EnableClientScript="true" ValidationGroup="<%# ClientID %>" ToolTip="Period should not be more than 4 months"
                                                        ErrorMessage="Period should not be more than 4 months." Display="Dynamic"></asp:CustomValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="textCenter">
                                        <asp:ValidationSummary ID="valSumDateRange" runat="server" ValidationGroup='<%# ClientID %>' />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="custBtns">
                                        <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="CheckIfDatesValid(); if(ValidAll()) return false;"
                                            Text="OK" CausesValidation="true" />
                                        &nbsp; &nbsp;
                                        <asp:Button ID="btnCustDatesCancel" CausesValidation="false" runat="server" Text="Cancel"
                                            OnClientClick="return false;" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="ddlGraphsTypes" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <table class="WholeWidth">
                <tr>
                    <td colspan="2" class="ReportBorderBottomHeight15Px">
                    </td>
                </tr>
            </table>
            <br />
            <asp:UpdatePanel ID="upnlTabCell" runat="server">
                <ContentTemplate>
                    <asp:Table ID="tblPersonViewSwitch" runat="server" CssClass="CommonCustomTabStyle PersonDetailReportCustomTabStyle">
                        <asp:TableRow ID="rowSwitcher" runat="server">
                            <asp:TableCell ID="cellSummary" CssClass="SelectedSwitch" runat="server">
                                <span class="bg"><span>
                                    <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                                        OnCommand="btnView_Command" CommandArgument="0" ToolTip="Summary"></asp:LinkButton></span>
                                </span>
                            </asp:TableCell>
                            <asp:TableCell ID="cellDetail" runat="server">
                                <span class="bg"><span>
                                    <asp:LinkButton ID="lnkbtnDetails" runat="server" Text="Details" CausesValidation="false"
                                        OnCommand="btnView_Command" CommandArgument="1" ToolTip="Details"></asp:LinkButton></span>
                                </span>
                            </asp:TableCell>
                            <asp:TableCell ID="cellGraphs" runat="server">
                                <span class="bg"><span>
                                    <asp:LinkButton ID="lnkbtnGraphs" runat="server" Text="Graphs" CausesValidation="false"
                                        OnCommand="btnView_Command" CommandArgument="2" ToolTip="Graphs"></asp:LinkButton></span>
                                </span>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <div class="tab-pane">
                        <asp:MultiView ID="mvConsultingDemandReport" runat="server" ActiveViewIndex="0">
                            <asp:View ID="vwSummary" runat="server">
                                <asp:Panel ID="pnlSummary" runat="server" CssClass="WholeWidth">
                                    <uc:SummaryView runat="server" ID="ucSummary" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwDetails" runat="server">
                                <asp:Panel ID="pnlDetail" runat="server" CssClass="WholeWidth">
                                    <uc1:DetailsView runat="server" ID="ucDetails" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwGraphs" runat="server">
                                <asp:Panel ID="pnlGraph" runat="server" CssClass="WholeWidth">
                                    <uc2:GraphsView runat="server" ID="ucGraphs" />
                                </asp:Panel>
                            </asp:View>
                        </asp:MultiView>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnUpdateView" />
                    <asp:PostBackTrigger ControlID="ucSummary$btnExportToExcel" />
                    <asp:PostBackTrigger ControlID="ucGraphs$ctrDetails$btnExportToExcel" />
                </Triggers>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

