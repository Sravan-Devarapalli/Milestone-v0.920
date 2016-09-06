<%@ Page Title="PTO Report" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="PTOReport.aspx.cs" Inherits="PraticeManagement.Reports.PTOReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>PTO Report | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    PTO Report
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery-1.4.1.yui.js" type="text/javascript"></script>
    <script type="text/javascript">

        var optionsList = new Array();
        var selectedValue;

        $(document).ready(function () {
            $(".filters .chevron").live("click", function () {
                $(this).parent().toggleClass("collapsePanel");
                $(".filters .colPanel").parent().toggleClass("fltPanel");
            });
        });


        function EnableResetButton() {
            var button = document.getElementById("<%= btnResetFilter.ClientID%>");
            var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
            if (button != null) {
                button.disabled = false;
                hiddenField.value = "true";
            }
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
                debugger;
                var ddlDetail = document.getElementById('<%=  ddlDetalization.ClientID %>');
                selectedValue = ddlDetail.value;

                for (var i = ddlDetail.length - 1; i >= 0; i--) {
                    ddlDetail.removeChild(ddlDetail[i]);
                }
                if ((startYear == endYear && ((endMonth - startMonth + 1) > 3))
            || (((((endYear - startYear) * 12 + endMonth) - startMonth + 1)) > 3)
            || ((endDate - startDate) / (1000 * 60 * 60 * 24)) > 90
            ) {

                    ddlDetail.appendChild(optionsList[optionsList.length - 1]);
                    ddlDetail[0].selected = 'true';

                }
                else {
                    for (var i = 0; i < optionsList.length; i++) {
                        ddlDetail.appendChild(optionsList[i]);
                    }

                    if (ddlDetail.value != selectedValue) {
                        for (var i = ddlDetail.length - 1; i >= 0; i--) {
                            if (ddlDetail[i].value != selectedValue) {
                                ddlDetail.selected = 'false';
                            }
                            else {
                                ddlDetail[i].selected = 'true';
                            }
                        }
                    }
                }

                var btnCustDatesClose = document.getElementById('<%= btnCustDatesClose.ClientID %>');
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
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
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

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
            if (imgCalender.fireEvent && ddlPeriod.value != '0') {
                imgCalender.style.display = "none";
                lblCustomDateRange.style.display = "none";
            }
        }

        function SaveItemsToArray() {
            optionsList = new Array();
            var ddlDetail = document.getElementById('<%=  ddlDetalization.ClientID %>');
            for (var i = 0; i < ddlDetail.length; i++) {
                Array.add(optionsList, ddlDetail[i]);
            }
        }


        function EnableOrDisableItemsOfDetalization() {
            var ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
            var ddlDetail = document.getElementById('<%=  ddlDetalization.ClientID %>');
            if (ddlPeriod.value == '7') {
                for (var i = ddlDetail.length - 1; i >= 0; i--) {
                    ddlDetail.removeChild(ddlDetail[i]);
                }
                for (var i = 0; i < optionsList.length - 1; i++) {
                    ddlDetail.appendChild(optionsList[i]);
                }
                for (var i = ddlDetail.length - 1; i >= 0; i--) {

                    if (ddlDetail[i].value != '1') {
                        ddlDetail.removeChild(ddlDetail[i]);
                    }
                    else {
                        ddlDetail[i].selected = 'true';
                    }
                }
            }
            else {
                selectedValue = ddlDetail.value;

                for (var i = ddlDetail.length - 1; i >= 0; i--) {
                    ddlDetail.removeChild(ddlDetail[i]);
                }
                if (ddlPeriod.value == '1' || ddlPeriod.value == '2') {
                    for (var i = 0; i < optionsList.length - 1; i++) {
                        ddlDetail.appendChild(optionsList[i]);
                    }
                }
                else if (ddlPeriod.value == '12' || ddlPeriod.value == '13') {
                    ddlDetail.appendChild(optionsList[2]);

                }
                else if (ddlPeriod.value == '0') {
                    debugger;
                    CheckIfDatesValid();
                }
                else {
                    for (var i = 0; i < optionsList.length; i++) {
                        ddlDetail.appendChild(optionsList[i]);
                    }
                }

                if (ddlDetail.value != selectedValue) {
                    for (var i = ddlDetail.length - 1; i >= 0; i--) {
                        if (ddlDetail[i].value != selectedValue) {
                            ddlDetail.selected = 'false';

                        }
                        else {
                            ddlDetail[i].selected = 'true';
                        }
                    }
                }
            }
            if (ddlPeriod.value == '0') {
                document.getElementById("tdFirst").className = "Width40P";
            }
            else {
                document.getElementById("tdFirst").className = "width30P";
            }
        }

        

    </script>
    <asp:UpdatePanel ID="updFilters" runat="server">
        <ContentTemplate>
            <div class="filters Margin-Bottom10Px">
                <div class="filter-section-color">
                    <table class="WholeWidth">
                        <tr>
                            <td align="left" class="Width50Percent" id="tdFirst">
                                <div class="chevron">
                                    &nbsp;
                                </div>
                                <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp; &nbsp;
                                <asp:Label ID="lblUtilizationFrom" runat="server" Text="Show PTO Report for"></asp:Label>
                                &nbsp;
                                <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="false" onchange="EnableOrDisableItemsOfDetalization(); EnableResetButton(); CheckAndShowCustomDatesPoup(this);">
                                    <asp:ListItem Text="Next Week" Value="7"></asp:ListItem>
                                    <asp:ListItem Text="Current Month" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Previous Month" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Next 3 Months" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Current Year" Value="12"></asp:ListItem>
                                    <asp:ListItem Text="Previous Year" Value="13"></asp:ListItem>
                                    <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                                    CancelControlID="btnCustDatesCancel" OkControlID="btnCustDatesClose" BackgroundCssClass="modalBackground"
                                    PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates" DropShadow="false"
                                    OnCancelScript="ReAssignStartDateEndDates();" />
                                <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                <asp:HiddenField ID="hdnStartDateTxtBoxId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnEndDateTxtBoxId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnStartDateCalExtenderBehaviourId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnEndDateCalExtenderBehaviourId" runat="server" Value="" />
                                &nbsp;
                                <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                                <asp:Image ID="imgCalender" runat="server" class="" ImageUrl="~/Images/calendar.gif" />
                                &nbsp;
                                <asp:Label ID="lblBy" runat="server" Text="by "></asp:Label>
                                &nbsp;
                                <asp:DropDownList ID="ddlDetalization" runat="server" CssClass="Width75PxImp" AutoPostBack="false"
                                    onchange="EnableResetButton();">
                                    <asp:ListItem Value="1">1 Day</asp:ListItem>
                                    <asp:ListItem Selected="True" Value="7">1 Week</asp:ListItem>
                                    <asp:ListItem Value="30">1 Month</asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;
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
                        <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                            CssClass="CustomTabStyle">
                            <AjaxControlToolkit:TabPanel ID="tpAdvancedFilters" runat="server">
                                <HeaderTemplate>
                                    <span class="bg"><a href="#"><span>Filters</span></a> </span>
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <table class="WholeWidth">
                                        <tr align="center">
                                            <td class="Width100Px BorderBottom1px">
                                                Person Status
                                            </td>
                                            <td class="Width30Px">
                                            </td>
                                            <td class="Width100Px BorderBottom1px">
                                                Pay Type
                                            </td>
                                            <td class="Width30Px">
                                            </td>
                                            <td class="Width200Px BorderBottom1px">
                                                Person Division
                                            </td>
                                            <td class="Width30Px">
                                            </td>
                                            <td class="Width200Px BorderBottom1px ">
                                                Practice Area
                                            </td>
                                            <td class="Width30Px">
                                            </td>
                                            <td class="Width200Px BorderBottom1px">
                                                Title
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chbActivePersons" runat="server" Text="Active" ToolTip="Include active persons into report"
                                                    AutoPostBack="false" Checked="True" onclick="EnableResetButton();" />
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chbW2salary" runat="server" Text="W2-Salary" ToolTip="Include W2-Salary persons into report"
                                                    AutoPostBack="false" Checked="false" onclick="EnableResetButton();" />
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
                                            <td rowspan="2" class="floatRight">
                                                <cc2:ScrollingDropDown ID="cblTitles" runat="server" AllSelectedReturnType="AllItems"
                                                    onclick="scrollingDropdown_onclick('cblTitles','Title')" CellPadding="3" NoItemsType="All"
                                                    SetDirty="False" DropDownListType="Title" CssClass="UTilTimeLineFilterCblPractices" />
                                                <ext:ScrollableDropdownExtender ID="sdeTitles" runat="server" TargetControlID="cblTitles"
                                                    UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chbProjectedPersons" runat="server" Text="Contingent" ToolTip="Include contingent persons into report"
                                                    AutoPostBack="false" Checked="false" onclick="EnableResetButton();" />
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chbW2Hourly" runat="server" Text="W2-Hourly" ToolTip="Include W2-Hourly persons into report"
                                                    AutoPostBack="false" Checked="false" onclick="EnableResetButton();" />
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </AjaxControlToolkit:TabPanel>
                            <AjaxControlToolkit:TabPanel runat="server" ID="tpGranularity">
                                <HeaderTemplate>
                                    <span class="bg"><a href="#"><span>Sort Options</span></a> </span>
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <table class="opportunity-description" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                Sort by &nbsp;&nbsp;
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlSortBy" runat="server" CssClass="Width300Px" onchange="EnableResetButton();">
                                                    <asp:ListItem Value="1">Alphabetical by User</asp:ListItem>
                                                    <asp:ListItem Value="2">By Date of PTO</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td class="Width40Px">
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rbSortbyAsc" runat="server" Text="Ascending" onclick="EnableResetButton();"
                                                    GroupName="Sortby" AutoPostBack="false" Checked="true" />
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:RadioButton ID="rbSortbyDesc" runat="server" Text="Descending" onclick="EnableResetButton();"
                                                    GroupName="Sortby" AutoPostBack="false" />
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                </ContentTemplate>
                            </AjaxControlToolkit:TabPanel>
                        </AjaxControlToolkit:TabContainer>
                    </asp:Panel>
                </div>
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
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="custBtns">
                                <asp:Button ID="btnCustDatesOK" runat="server" Text="OK" CausesValidation="true" />
                                <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true"
                                    OnClientClick="return false;" />
                                &nbsp; &nbsp;
                                <asp:Button ID="btnCustDatesCancel" runat="server" Text="Cancel" />
                            </td>
                        </tr>
                        <tr>
                            <td class="textCenter">
                                <asp:ValidationSummary ID="valSum" runat="server" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
                <asp:HiddenField ID="hdnIsChartRenderedFirst" runat="server" Value="false" />
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc3:LoadingProgress ID="progress" runat="server" />
    <asp:UpdatePanel ID="updConsReport" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <asp:HiddenField ID="hdnSaveReportText" runat="server" />
            <div class="textRight">
                <table class="WholeWidthWithHeight" id="tblExport" runat="server" visible="false">
                    <tr>
                        <td colspan="4" class="Width90Percent">
                        </td>
                        <td class="Width10Percent padRight5">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="Width40P">
                                        Export:
                                    </td>
                                    <td>
                                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                                            Enabled="true" UseSubmitBehavior="false" ToolTip="Export To Excel" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnExport" runat="server" Text="PDF" OnClick="btnPDFExport_Click"
                                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="titleDiv" runat="server" style="text-align: center" visible="false">
                <asp:Label ID="lblTitle" runat="server"></asp:Label>
            </div>
            <br />
            <div class="ConsultantsWeeklyReportAlignCenter" id="divPTOReport" runat="server">
                <asp:Chart ID="chart" CssClass="ConsultantsWeeklyReportAlignCenter" runat="server"
                    Width="1100px">
                    <Series>
                        <asp:Series Name="Weeks" ChartType="RangeBar" IsVisibleInLegend="false" />
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="MainArea">
                            <AxisY IsLabelAutoFit="False" LineDashStyle="NotSet">
                                <MajorGrid LineColor="DimGray" />
                                <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                                <LabelStyle Format="MMM, d" />
                            </AxisY>
                            <AxisY2 IsLabelAutoFit="False" Enabled="True">
                                <MajorGrid LineColor="DimGray" />
                                <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                                <LabelStyle Format="MMM, d" />
                            </AxisY2>
                            <AxisX IsLabelAutoFit="true">
                                <MajorGrid Interval="Auto" LineDashStyle="Dot" />
                                <MajorTickMark Enabled="False" />
                            </AxisX>
                            <AxisX2 Enabled="True">
                                <MajorGrid Interval="Auto" LineDashStyle="Dot" />
                                <MajorTickMark Enabled="False" />
                            </AxisX2>
                            <Area3DStyle Inclination="5" IsClustered="True" IsRightAngleAxes="False" LightStyle="Realistic"
                                Perspective="1" />
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
                <div id="nonInv" runat="server" visible="false">
                    No Resources found for the selected filters.
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExport" />
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:Chart ID="chartPdf" CssClass="ConsultantsWeeklyReportAlignCenter" runat="server"
        Visible="false" Width="920px" Height="850px">
        <Series>
            <asp:Series Name="Weeks" ChartType="RangeBar" IsVisibleInLegend="false" />
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="MainArea">
                <AxisY IsLabelAutoFit="False" LineDashStyle="NotSet">
                    <MajorGrid LineColor="DimGray" />
                    <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                    <LabelStyle Format="MMM, d" />
                </AxisY>
                <AxisY2 IsLabelAutoFit="False" Enabled="True">
                    <MajorGrid LineColor="DimGray" />
                    <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                    <LabelStyle Format="MMM, d" />
                </AxisY2>
                <AxisX IsLabelAutoFit="true">
                    <MajorGrid Interval="Auto" LineDashStyle="Dot" />
                    <MajorTickMark Enabled="False" />
                </AxisX>
                <AxisX2 Enabled="True">
                    <MajorGrid Interval="Auto" LineDashStyle="Dot" />
                    <MajorTickMark Enabled="False" />
                </AxisX2>
                <Area3DStyle Inclination="5" IsClustered="True" IsRightAngleAxes="False" LightStyle="Realistic"
                    Perspective="1" />
            </asp:ChartArea>
        </ChartAreas>
    </asp:Chart>
</asp:Content>

