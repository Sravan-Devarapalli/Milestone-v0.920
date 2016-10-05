<%@ Page Title="Expense by Project" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ExpenseReport.aspx.cs" Inherits="PraticeManagement.Reports.ExpenseReport" %>

<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/ExpenseSummaryByProject.ascx" TagPrefix="uc"
    TagName="ByProject" %>
<%@ Register Src="~/Controls/Reports/ExpenseSummaryByType.ascx" TagPrefix="uc" TagName="ByExpenseType" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Expense by Project | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src='<%# Generic.GetClientUrl("../Scripts/ExpandOrCollapse.min.js", this) %>'
        type="text/javascript"></script>
    <link href="../Css/TableSortStyle.chirp.css" rel="stylesheet" type="text/css" />
    <script src="<%# Generic.GetClientUrl("../Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        function setPosition(item, ytop, xleft) {
            item.offset({ top: ytop, left: xleft });
        }

        function SetTooltipText(descriptionText, hlinkObj) {
            var hlinkObjct = $(hlinkObj);
            var displayPanel = $('#<%= pnlMonthToolTipHolder.ClientID %>');
            iptop = hlinkObjct.offset().top;
            ipleft = hlinkObjct.offset().left + hlinkObjct[0].offsetWidth + 5;
            setPosition(displayPanel, iptop - 20, ipleft);
            displayPanel.show();
            setPosition(displayPanel, iptop - 20, ipleft);
            displayPanel.show();
            var lblProjectTooltip = document.getElementById('<%= lblProjectTooltip.ClientID %>');
            lblProjectTooltip.innerHTML = descriptionText.toString();
        }

        function HidePanel() {
            var displayPanel = $('#<%= pnlMonthToolTipHolder.ClientID %>');
            displayPanel.hide();
        }

        $(document).ready(function () {
            SortingDisable();
            $(".filters .chevron").live("click", function () {
                $(this).parent().toggleClass("collapsePanel");
                $(".filters .colPanel").parent().toggleClass("fltPanel");
            });
            ShowRespectiveFilters();

            $(".detailsChevron").live("click", function () {
                if ($(this).hasClass("collapseClass")) {
                    $(this).removeClass("collapseClass").attr("src", "../Images/expand.jpg").attr("title", "Expand Details");

                    var $thisId = $(this).attr("id");
                    var $targetContronId = $(this).attr("TargetControlID");
                    $("#" + $thisId.substring(0, $thisId.lastIndexOf('_') + 1) + $targetContronId).hide();
                }
                else {
                    $(this).addClass("collapseClass").attr("src", "../Images/collapse.jpg").attr("title", "Collapse Details");

                    var $thisId = $(this).attr("id");
                    var $targetContronId = $(this).attr("TargetControlID");
                    $("#" + $thisId.substring(0, $thisId.lastIndexOf('_') + 1) + $targetContronId).show();
                }
            });

            $(".expandOrCollapseAll").live("click", function () {
                if ($(this).hasClass("collapseClass")) {
                    $(this).removeClass("collapseClass").val("Expand All");
                    $(".detailsChevron").removeClass("collapseClass").attr("src", "../Images/expand.jpg").attr("title", "Expand Details");
                    $("div[id$=pnlDetails]").hide();
                    $("div[id$=pnlDateDetails]").hide();
                }
                else {
                    $(this).addClass("collapseClass").val("Collapse All");
                    $(".detailsChevron").addClass("collapseClass").attr("src", "../Images/collapse.jpg").attr("title", "Collapse Details");
                    $("div[id$=pnlDetails]").show();
                    $("div[id$=pnlDateDetails]").show();
                }
            });
        });


        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {
            SortingDisable();
        }

        function SortingDisable() {
            var ddlView = document.getElementById('<%= ddlView.ClientID %>');

            if (ddlView.value == '1' && typeof ($("#tblExpenseSummaryByType").find('tr')[0]) !== "undefined") {

                var numberOfColumnsType = $("#tblExpenseSummaryByType").find('tr')[0].cells.length;

                var HeadersType = {}

                for (var i = 1; i < numberOfColumnsType; i++)
                    HeadersType[i] = { sorter: false }

                $("#tblExpenseSummaryByType").tablesorter({
                    headers: HeadersType
                });
            }
            if (ddlView.value == '0' && typeof ($("#tblExpenseSummaryByProject").find('tr')[0]) !== "undefined") {
                var numberOfColumnsProject = $("#tblExpenseSummaryByProject").find('tr')[0].cells.length;

                var HeadersProject = {}

                for (var i = 5; i < numberOfColumnsProject; i++)
                    HeadersProject[i] = { sorter: false }

                $("#tblExpenseSummaryByProject").tablesorter({
                    headers: HeadersProject
                });
            }
        }

        function EnableResetButton() {
            var button = document.getElementById("<%= btnResetFilter.ClientID%>");
            var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
            if (button != null) {
                button.disabled = false;
                hiddenField.value = "true";
            }
        }

        function ShowRespectiveFilters() {
            var ddlView = document.getElementById('<%= ddlView.ClientID %>');
            if (ddlView.value == '0') {
                document.getElementById('<%= divExpenseFlr.ClientID%>').className = "displayNone";
                document.getElementById('<%= divProjectFlr.ClientID%>').className = "DisplayInline padLeft30";
            }
            else if (ddlView.value == '1') {
                document.getElementById('<%= divExpenseFlr.ClientID%>').className = "DisplayInline padLeft30";
                document.getElementById('<%= divProjectFlr.ClientID%>').className = "displayNone";
            }
            else {
                document.getElementById('<%= divExpenseFlr.ClientID%>').className = "displayNone";
                document.getElementById('<%= divProjectFlr.ClientID%>').className = "displayNone";
            }
        }

        function CheckAndShowCustomDatesPoup(ddlPeriod) {
            imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            if (ddlPeriod.value == '0') {
                imgCalender.attributes["class"].value = "";
                lblCustomDateRange.attributes["class"].value = "fontBold";
                if (imgCalender.fireEvent) {
                    imgCalender.style.display = "";
                    lblCustomDateRange.style.display = "";
                    imgCalender.click();
                }
                if (document.createEvent) {
                    var event = document.createEvent('HTMLEvents');
                    event.initEvent('click', true, true);
                    imgCalender.dispatchEvent(event);
                }
            }
            else {
                imgCalender.attributes["class"].value = "displayNone";
                lblCustomDateRange.attributes["class"].value = "displayNone";
                if (imgCalender.fireEvent) {
                    imgCalender.style.display = "none";
                    lblCustomDateRange.style.display = "none";
                }
            }
        }
        function ReAssignStartDateEndDates() {
            hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
            hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');

            var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
            var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
            if (startDateCalExtender != null) {
                startDateCalExtender.set_selectedDate(hdnStartDate.value);
            }
            if (endDateCalExtender != null) {
                endDateCalExtender.set_selectedDate(hdnEndDate.value);
            }
            btnCustDatesOK = document.getElementById('<%= btnCustDatesOK.ClientID %>');
            btnCustDatesOK.click();
        }

        function ShowOrHideColumns(actual, estimated) {
            if (actual == "True") {
                $(".hideActCol").css("display", "");
            }
            else {
                $(".hideActCol").css("display", "none");
            }
            if (estimated == "True") {
                $(".hideEstCol").css("display", "");
            }
            else {
                $(".hideEstCol").css("display", "none");
            }
            if (actual == "True" && estimated == "True") {
                $(".hideDiffCol").css("display", "");
            }
            else {
                $(".hideDiffCol").css("display", "none");
            }
        }
    </script>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" Visible="true" />
    <asp:UpdatePanel ID="upnlBody" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:UpdatePanel ID="temp" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
                    <div class="filters Margin-Bottom10Px">
                        <div class="filter-section-color">
                            <table class="WholeWidth">
                                <tr>
                                    <td align="left" class="Width50Percent" id="td2">
                                        <div class="chevron">
                                            &nbsp;
                                        </div>
                                        &nbsp; &nbsp;
                                        <asp:Label ID="lblUtilizationFrom" runat="server" Text="Show Report for"></asp:Label>
                                        &nbsp;
                                        <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="false" Onchange="CheckAndShowCustomDatesPoup(this); EnableResetButton();">
                                            <asp:ListItem Text="This Month" Value="30" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="This Year" Value="365"></asp:ListItem>
                                            <asp:ListItem Text="Last Month" Value="-30"></asp:ListItem>
                                            <asp:ListItem Text="Last Year" Value="-365"></asp:ListItem>
                                            <asp:ListItem Text="Q1" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Q2" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Q3" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Q4" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnStartDateCalExtenderBehaviourId" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnEndDateCalExtenderBehaviourId" runat="server" Value="" />
                                        <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                                        <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                        &nbsp;&nbsp;View &nbsp;
                                        <asp:DropDownList ID="ddlView" runat="server" Onchange=" EnableResetButton(); return ShowRespectiveFilters();">
                                            <asp:ListItem Text="By Project" Value="0" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="By Expense type" Value="1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                    </td>
                                    <td align="right">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnUpdateView" runat="server" Text="Update View" CssClass="Width90PxImp"
                                                        OnClick="btnUpdateView_OnClick" EnableViewState="False" />
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" CssClass="Width90PxImp"
                                                        OnClick="btnResetFilter_OnClick" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="fltPanel">
                            <asp:Panel ID="pnlFilters" runat="server" class="colPanel">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                                            CssClass="CustomTabStyle">
                                            <AjaxControlToolkit:TabPanel ID="tpAdvancedFilters" runat="server">
                                                <HeaderTemplate>
                                                    <span class="bg"><a href="#"><span>Filters</span></a> </span>
                                                </HeaderTemplate>
                                                <ContentTemplate>
                                                    <div class="WholeWidth no-wrap">
                                                        <div class="Width10Per DisplayInline">
                                                            <table class="WholeWidth">
                                                                <tr align="center" class="tb-header">
                                                                    <th class="Width150pxImp BorderBottom1px">
                                                                        Include
                                                                    </th>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:CheckBox ID="chbEstimatedExpense" runat="server" Text="Estimated Expense" ToolTip="Include Estimated Expense into report"
                                                                            Checked="false" onclick="EnableResetButton();" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:CheckBox ID="chbActualExpense" runat="server" Text="Actual Expense" ToolTip="Include Actual Expense into report"
                                                                            EnableViewState="False" Checked="false" onclick="EnableResetButton();" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div id="divProjectFlr" class="displayNone" runat="server">
                                                            <table class="WholeWidth">
                                                                <tr align="center" class="tb-header">
                                                                    <th colspan="3" class="BorderBottom1px ">
                                                                        Project Types Included
                                                                    </th>
                                                                    <td class="Width30Px">
                                                                    </td>
                                                                    <th class="Width150pxImp BorderBottom1px">
                                                                        Account
                                                                    </th>
                                                                    <td class="Width30Px">
                                                                    </td>
                                                                    <th class="Width150pxImp BorderBottom1px">
                                                                        Project
                                                                    </th>
                                                                    <td class="Width30Px">
                                                                    </td>
                                                                    <th class="Width150pxImp BorderBottom1px">
                                                                        Division
                                                                    </th>
                                                                    <td class="Width30Px">
                                                                    </td>
                                                                    <th class="Width150pxImp BorderBottom1px">
                                                                        Practice Area
                                                                    </th>
                                                                    <td class="Width30Px">
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="ProjectSummaryCheckboxTd Width50PxImp">
                                                                        <asp:CheckBox ID="chbActive" runat="server" Text="Active" Checked="True" EnableViewState="False"
                                                                            onclick="EnableResetButton();" />
                                                                    </td>
                                                                    <td class="ProjectSummaryCheckboxTd Width50PxImp">
                                                                        <asp:CheckBox ID="chbprojected" runat="server" Text="Projected" Checked="True" EnableViewState="False"
                                                                            onclick="EnableResetButton();" />
                                                                    </td>
                                                                    <td class="ProjectSummaryCheckboxTd Width50PxImp">
                                                                        <asp:CheckBox ID="chbCompleted" runat="server" Text="Completed" Checked="True" EnableViewState="False"
                                                                            onclick="EnableResetButton();" />
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                    <td rowspan="2" class="floatRight">
                                                                        <cc2:ScrollingDropDown ID="cblAccounts" runat="server" AllSelectedReturnType="AllItems"
                                                                            OnSelectedIndexChanged="cblAccount_OnSelectedIndexChanged" onclick="scrollingDropdown_onclick('cblAccounts','Account')"
                                                                            CellPadding="3" AutoPostBack="true" NoItemsType="All" SetDirty="False" DropDownListType="Account"
                                                                            CssClass="UTilTimeLineFilterCblPractices" />
                                                                        <ext:ScrollableDropdownExtender ID="dseAccount" runat="server" TargetControlID="cblAccounts"
                                                                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                                        </ext:ScrollableDropdownExtender>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                    <td rowspan="2" class="floatRight">
                                                                        <cc2:ScrollingDropDown ID="cblProject" runat="server" AllSelectedReturnType="AllItems"
                                                                            onclick="scrollingDropdown_onclick('cblProject','Project')" CellPadding="3" NoItemsType="All"
                                                                            SetDirty="False" DropDownListType="Project" CssClass="ExpenseCblProject" />
                                                                        <ext:ScrollableDropdownExtender ID="sdeProject" runat="server" TargetControlID="cblProject"
                                                                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                                        </ext:ScrollableDropdownExtender>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                    <td rowspan="2" class="floatRight">
                                                                        <cc2:ScrollingDropDown ID="cblDivisions" runat="server" AllSelectedReturnType="AllItems"
                                                                            onclick="scrollingDropdown_onclick('cblDivisions','Division')" CellPadding="3"
                                                                            NoItemsType="All" SetDirty="False" DropDownListType="Division" CssClass="UTilTimeLineFilterCblPractices" />
                                                                        <ext:ScrollableDropdownExtender ID="sdeDivision" runat="server" TargetControlID="cblDivisions"
                                                                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                                        </ext:ScrollableDropdownExtender>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                    <td rowspan="2" class="floatRight">
                                                                        <cc2:ScrollingDropDown ID="cblPractices" runat="server" AllSelectedReturnType="AllItems"
                                                                            onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" CellPadding="3"
                                                                            NoItemsType="All" SetDirty="False" DropDownListType="Practice Area" CssClass="UTilTimeLineFilterCblPractices" />
                                                                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                                                                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                                        </ext:ScrollableDropdownExtender>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="ProjectSummaryCheckboxTd Width50PxImp">
                                                                        <asp:CheckBox ID="chbProposed" runat="server" Text="Proposed" Checked="True" EnableViewState="False"
                                                                            onclick="EnableResetButton();" />
                                                                    </td>
                                                                    <td class="ProjectSummaryCheckboxTd Width50PxImp">
                                                                        <asp:CheckBox ID="chbInactive" runat="server" Text="Inactive" Checked="false" EnableViewState="False"
                                                                            onclick="EnableResetButton();" />
                                                                    </td>
                                                                    <td class="ProjectSummaryCheckboxTd Width50PxImp">
                                                                        <asp:CheckBox ID="chbExperimental" runat="server" Text="Experimental" Checked="false"
                                                                            EnableViewState="False" onclick="EnableResetButton();" />
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div id="divExpenseFlr" runat="server" class="displayNone">
                                                            <table class="WholeWidth" style="height: 60px">
                                                                <tr align="center" class="tb-header">
                                                                    <th class="Width150pxImp BorderBottom1px">
                                                                        Expense Type
                                                                    </th>
                                                                </tr>
                                                                <tr>
                                                                    <td rowspan="2">
                                                                        <cc2:ScrollingDropDown ID="cblExpenseType" runat="server" AllSelectedReturnType="AllItems"
                                                                            onclick="scrollingDropdown_onclick('cblExpenseType','Expense Type')" CellPadding="3"
                                                                            NoItemsType="All" SetDirty="False" DropDownListType="Expense Type" CssClass="UTilTimeLineFilterCblPractices" />
                                                                        <ext:ScrollableDropdownExtender ID="sdeExpenseType" runat="server" TargetControlID="cblExpenseType"
                                                                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                                        </ext:ScrollableDropdownExtender>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </AjaxControlToolkit:TabPanel>
                                        </AjaxControlToolkit:TabContainer>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </asp:Panel>
                        </div>
                        <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                            BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                            DropShadow="false" CancelControlID="btnCustDatesCancel" OnCancelScript="ReAssignStartDateEndDates();" />
                        <asp:Panel ID="pnlCustomDates" runat="server" CssClass="ConfirmBoxClass CustomDatesPopUp"
                            Style="display: none;">
                            <table class="WholeWidth">
                                <tr>
                                    <td align="center">
                                        <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                            FromToDateFieldCssClass="Width70Px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="custBtns">
                                        <asp:Button ID="btnCustDatesOK" runat="server" Text="OK" CausesValidation="true" />
                                        &nbsp; &nbsp;
                                        <asp:Button ID="btnCustDatesCancel" runat="server" Text="Cancel" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="textCenter">
                                        <asp:ValidationSummary ID="valSumDateRange" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
            </div>
            <hr />
            <div id="divWholePage" runat="server" style="display: none">
                <asp:MultiView ID="mvExpenseReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwProjectReport" runat="server">
                        <asp:Panel ID="pnlProjectReport" runat="server">
                            <uc:ByProject ID="tpByProject" runat="server">
                            </uc:ByProject>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwExpenseTypeReport" runat="server">
                        <asp:Panel ID="pnlExpenseTypeReport" runat="server" CssClass="WholeWidth">
                            <uc:ByExpenseType ID="tpByExpenseType" runat="server">
                            </uc:ByExpenseType>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="tpByProject$ucExpenseSummaryDetails$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByExpenseType$ucExpenseSummaryDetails$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByProject$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByExpenseType$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByProject$ucExpenseDetailsByProject$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByExpenseType$ucExpenseDetailsByExpenseType$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByProject$ucExpenseDetailsByProject$btnExportToPDF" />
            <asp:PostBackTrigger ControlID="tpByExpenseType$ucExpenseDetailsByExpenseType$btnExportToPDF" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:Panel ID="pnlMonthToolTipHolder" Style="display: none;" runat="server" CssClass="ToolTip WordWrap ProjectsToolTip">
        <table>
            <tr class="top">
                <td class="lt">
                    <div class="tail">
                    </div>
                </td>
                <td class="tbor">
                </td>
                <td class="rt">
                </td>
            </tr>
            <tr class="middle">
                <td class="lbor">
                </td>
                <td class="content WordWrap">
                    <asp:Label ID="lblProjectTooltip" CssClass="WordWrap" runat="server"></asp:Label>
                </td>
                <td class="rbor">
                </td>
            </tr>
            <tr class="bottom">
                <td class="lb">
                </td>
                <td class="bbor">
                </td>
                <td class="rb">
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

