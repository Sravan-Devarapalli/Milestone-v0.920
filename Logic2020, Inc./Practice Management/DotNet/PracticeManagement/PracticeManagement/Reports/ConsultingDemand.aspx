<%@ Page Title="Consulting Demand | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ConsultingDemand.aspx.cs" Inherits="PraticeManagement.Reporting.ConsultingDemand" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/Reports/UTilTimelineFilter.ascx" TagName="UtilTimelineFilter"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Consulting Demand | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Consulting Demand
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <style type="text/css">
        .FadingTooltip
        {
            border-right: darkgray 1px outset;
            border-top: darkgray 1px outset;
            font-size: 10px;
            border-left: darkgray 1px outset;
            width: auto;
            color: black;
            border-bottom: darkgray 1px outset;
            height: auto;
            background-color: lemonchiffon;
            borderbottomwidths: "3,3,3,3";
            font-size: 11px;
        }
        .displayNone
        {
            display: none;
        }        
        
        .SetScrollRightButtonStyle
        { 
            padding-left:10px; padding-right:8px; padding-top:4px; padding-bottom:4px; width:8px;
        }
        .SetScrollLeftButtonStyle
        {
            padding-left:2px; padding-right:12px; padding-top:4px; padding-bottom:4px; width:8px;
        }
        
    </style>
    <script type="text/javascript" language="javascript">
        var FADINGTOOLTIP;
        var wnd_height, wnd_width;
        var tooltip_height, tooltip_width;
        var tooltip_shown = false;
        var transparency = 100;
        var timer_id = 1;
        var tooltiptext;

        // override events
        window.onload = WindowLoading;
        window.onresize = UpdateWindowSize;
        document.onmousemove = AdjustToolTipPosition;

        function DisplayTooltip(tooltip_text) {
            FADINGTOOLTIP.innerHTML = tooltip_text;
            tooltip_shown = (tooltip_text != "") ? true : false;
            if (tooltip_text != "") {
                // Get tooltip window height
                tooltip_height = (FADINGTOOLTIP.style.pixelHeight) ? FADINGTOOLTIP.style.pixelHeight : FADINGTOOLTIP.offsetHeight;
                transparency = 0;
                ToolTipFading();
            }
            else {
                clearTimeout(timer_id);
                FADINGTOOLTIP.style.visibility = "hidden";
            }
        }

        function AdjustToolTipPosition(e) {
            if (tooltip_shown) {
                e = e || window.event;

                FADINGTOOLTIP.style.visibility = "visible";
                setPosition($(FADINGTOOLTIP), getPosition(e).y, getPosition(e).x + 20)
            }
        }

        function setPosition(item, ytop, xleft) {
            item.offset({ top: ytop, left: xleft });
        }

        function getPosition(e) {
            var cursor = { x: 0, y: 0 };
            if (e.pageX || e.pageY) {
                cursor.x = e.pageX;
                cursor.y = e.pageY;
            }
            else {
                var de = document.documentElement;
                var b = document.body;

                cursor.x = e.clientX +
            (de.scrollLeft || b.scrollLeft) - (de.clientLeft || 0);
                cursor.y = e.clientY +
            (de.scrollTop || b.scrollTop) - (de.clientTop || 0);
            }
            return cursor;
        }



        function WindowLoading() {
            FADINGTOOLTIP = document.getElementById('FADINGTOOLTIP');

            // Get tooltip  window width				
            tooltip_width = (FADINGTOOLTIP.style.pixelWidth) ? FADINGTOOLTIP.style.pixelWidth : FADINGTOOLTIP.offsetWidth;

            // Get tooltip window height
            tooltip_height = (FADINGTOOLTIP.style.pixelHeight) ? FADINGTOOLTIP.style.pixelHeight : FADINGTOOLTIP.offsetHeight;

            UpdateWindowSize();
        }

        function ToolTipFading() {
            if (transparency <= 100) {
                FADINGTOOLTIP.style.filter = "alpha(opacity=" + transparency + ")";
                transparency += 5;
                timer_id = setTimeout('ToolTipFading()', 35);
            }
        }

        function UpdateWindowSize() {
            wnd_height = document.body.clientHeight;
            wnd_width = document.body.clientWidth;
        }

        function EnableResetButton() {
            var button = document.getElementById("<%= btnResetFilter.ClientID%>");
            var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
            if (button != null) {
                button.disabled = false;
                hiddenField.value = "1";
            }
        }

        function CheckAndShowCustomDatesPoup(ddlPeriod) {
            imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            if (ddlPeriod.value == '0') {
                imgCalender.attributes["class"].value = "";
                lblCustomDateRange.attributes["class"].value = "";
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

        function ChangeStartEndDates() {
            ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
            if (ddlPeriod.value == '0') {

                hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
                hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
                hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
                hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
                txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
                txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
                hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
                hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');
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

                    startDate = new Date(startMonth.toString() + '/01/' + startYear.toString());
                    if (endMonth == 12) {
                        endYear = endYear + 1;
                        endMonth = 1;
                        endDate = new Date('01/01/' + endYear.toString());
                    }
                    else {
                        endMonth = endMonth + 1;
                        endDate = new Date(endMonth.toString() + '/01/' + endYear.toString());
                    }
                    endDate = new Date((endDate - (1000 * 60 * 60 * 24)));
                    //                    if ((endYear - startYear) * 12 + endMonth - startMonth > 4) {
                    //                        endMonth = (startMonth + 2) % 12;
                    //                        if (startMonth > endMonth) {
                    //                            endYear = startYear + 1;
                    //                        }
                    //                        else {
                    //                            endYear = startYear;
                    //                        }
                    //                        endDate = new Date((endMonth + 1).toString() + '/01/' + endYear.toString());
                    //                        endDate = new Date((endDate - (1000 * 60 * 60 * 24)));
                    //                    }
                    var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
                    var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
                    if (startDateCalExtender != null) {
                        startDateCalExtender.set_selectedDate(new Date(startDate.format("MM/dd/yyyy")));
                    }
                    if (endDateCalExtender != null) {
                        endDateCalExtender.set_selectedDate(new Date(endDate.format("MM/dd/yyyy")));
                    }

                    if (PeriodValidate()) {
                        hdnStartDate.value = startDate.format("MM/dd/yyyy");
                        hdnEndDate.value = endDate.format("MM/dd/yyyy");

                        lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
                        lblCustomDateRange.innerHTML = '(' + hdnStartDate.value + '&nbsp;-&nbsp;' + hdnEndDate.value + ')';
                    }

                }
            }
        }
        function CheckIfDatesValid() {

            if (PeriodValidate()) {

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

            return false;
        }

        function PeriodValidate() {
            var cstvalPeriodRange = document.getElementById('<%= cstvalPeriodRange.ClientID %>');
            var valSummary = document.getElementById('<%= valSum.ClientID %>');
            var lblPeriodRange = document.getElementById('<%= lblPeriodRange.ClientID %>');

            cstvalPeriodRange.style.display = valSummary.style.display = lblPeriodRange.style.display = 'none';

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
                if ((startYear == endYear && ((endMonth - startMonth + 1) <= 12))
            || (((((endYear - startYear) * 12 + endMonth) - startMonth + 1)) <= 12)
            || ((endDate - startDate) / (1000 * 60 * 60 * 24)) < 90
            ) {
                    return true;
                }
                else {
                    cstvalPeriodRange.style.display = 'block';
                    lblPeriodRange.style.display = '';
                }
            }
            else {
                valSummary.style.display = 'block';
            }

            return false;
        }

        function ValidatePeriod(sender, args) {
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
                    args.IsValid = ((endMonth - startMonth + 1) <= 12);
                }
                else {
                    args.IsValid = (((((endYear - startYear) * 12 + endMonth) - startMonth + 1)) <= 12);
                }
                if ((startYear == endYear && ((endMonth - startMonth + 1) <= 12))
            || (((((endYear - startYear) * 12 + endMonth) - startMonth + 1)) <= 12)
            || ((endDate - startDate) / (1000 * 60 * 60 * 24)) < 90
            ) {
                    args.IsValid = true;
                }
            }
            else {
                args.IsValid = true;
            }
        }

        function ClearValidations() {
            var valSummary = document.getElementById('<%= valSum.ClientID %>');
            var cstvalPeriodRange = document.getElementById('<%= cstvalPeriodRange.ClientID %>');
            var lblPeriodRange = document.getElementById('<%= lblPeriodRange.ClientID %>');

            cstvalPeriodRange.style.display = lblPeriodRange.style.display = 'none';
            valSummary.style.display = 'none';
        }


        function ResetFilters(btnReset) {

            var ddlPeriod = document.getElementById('<%= ddlPeriod.ClientID %>');
            var hdnFiltersChanged = document.getElementById('<%= hdnFiltersChanged.ClientID %>');
            var lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            var imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            var hdnDefaultStartDate = document.getElementById('<%= hdnDefaultStartDate.ClientID %>');
            var hdnDefaultEndDate = document.getElementById('<%= hdnDefaultEndDate.ClientID %>');
            var hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            var hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');

            if (ddlPeriod != null) {
                ddlPeriod.value = '4';
            }
            if (hdnFiltersChanged != null) {
                hdnFiltersChanged.value = '0';
            }
            if (hdnDefaultStartDate != null && hdnDefaultEndDate != null) {
                var startDate = new Date(hdnDefaultStartDate.value);
                var endDate = new Date(hdnDefaultEndDate.value);
                var hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
                var hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');
                var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
                var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);

                if (startDateCalExtender != null) {
                    startDateCalExtender.set_selectedDate(new Date(startDate.format("MM/dd/yyyy")));
                }
                if (endDateCalExtender != null) {
                    endDateCalExtender.set_selectedDate(new Date(endDate.format("MM/dd/yyyy")));
                }

                hdnStartDate.value = startDate.format("MM/dd/yyyy");
                hdnEndDate.value = endDate.format("MM/dd/yyyy");

                lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
                lblCustomDateRange.innerHTML = '(' + hdnStartDate.value + '&nbsp;-&nbsp;' + hdnEndDate.value + ')';
            }

            imgCalender.attributes["class"].value = "displayNone";
            lblCustomDateRange.attributes["class"].value = "displayNone";
            btnReset.disabled = 'disabled';

            return false;
        }

    </script>
    <div class="FadingTooltip" id="FADINGTOOLTIP" style="z-index: 999; padding: 5px;
        visibility: hidden; position: absolute">
    </div>
    <uc:LoadingProgress ID="lpConsultingDemand" runat="server" />
    <asp:UpdatePanel ID="updConsReport" runat="server">
        <ContentTemplate>
            <div class="buttons-block">
                <table class="WholeWidth">
                    <tr>
                        <td align="left" style="width: 98%;">
                            Show Consultants Demand for &nbsp;
                            <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="false" onchange="EnableResetButton(); CheckAndShowCustomDatesPoup(this);">
                                <asp:ListItem Selected="True" Text="Next 4 Months" Value="4"></asp:ListItem>
                                <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;<asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server"
                                Text=""></asp:Label>
                            &nbsp;<asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                                CancelControlID="btnCustDatesCancel" OkControlID="btnCustDatesClose" BackgroundCssClass="modalBackground"
                                PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates" DropShadow="false"
                                OnCancelScript="ReAssignStartDateEndDates(); ClearValidations();" OnOkScript="return false;" />
                        </td>
                        <td>
                            <td>
                                <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                <asp:HiddenField ID="hdnStartDateTxtBoxId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnEndDateTxtBoxId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnStartDateCalExtenderBehaviourId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnEndDateCalExtenderBehaviourId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnDefaultStartDate" runat="server" Value="" />
                                <asp:HiddenField ID="hdnDefaultEndDate" runat="server" Value="" />
                                <asp:Button ID="btnUpdateView" runat="server" Text="Update View" Width="90px" OnClick="btnUpdateView_OnClick"
                                    EnableViewState="False" />
                            </td>
                            <td>
                                <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" Width="90px" OnClientClick="if(!ResetFilters(this)){return false;}" />
                            </td>
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
                    CssClass="ConfirmBoxClass" Style="padding-top: 20px; display: none;" BorderWidth="2px">
                    <table class="WholeWidth">
                        <tr>
                            <td align="center">
                                <table>
                                    <tr>
                                        <td>
                                            <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                                FromToDateFieldWidth="70" />
                                        </td>
                                        <td style="min-width: 5px;">
                                            <asp:CustomValidator ID="cstvalPeriodRange" runat="server" ClientValidationFunction="ValidatePeriod" ValidationGroup='<%# ClientID %>'
                                                SetFocusOnError="true" Text="*" EnableClientScript="true" ToolTip="Period should not be more than four months"
                                                ErrorMessage="Period should not be more than 4 months." Display="Dynamic"></asp:CustomValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" style="padding: 10px 0px 10px 0px;">
                                <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="if(!CheckIfDatesValid()){return false;}" ValidationGroup='<%# ClientID %>'
                                    Text="OK" Style="float: none !Important;" CausesValidation="true" />
                                <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true"
                                    OnClientClick="return false;" />
                                &nbsp; &nbsp;
                                <asp:Button ID="btnCustDatesCancel" runat="server" Text="Cancel" Style="float: none !Important;" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup='<%# ClientID %>' />
                                <asp:Label ID="lblPeriodRange" runat="server" Text="Period should not be more than 12 months." style="display:none;" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
            <div id="chartDiv" runat="server" style="overflow-x: auto; overflow-y: hidden; text-align:center;">
                <asp:Chart ID="chrtConsultingDemand" runat="server" Width="920px">
                    <Legends>
                        <asp:Legend LegendStyle="Row" Name="Botom Legend" TableStyle="Wide" Docking="Bottom"
                            Alignment="Center">
                            <CellColumns>
                                <asp:LegendCellColumn Name="Weeks" Text="">
                                    <Margins Left="15" Right="15" Top="15" Bottom="15"></Margins>
                                </asp:LegendCellColumn>
                            </CellColumns>
                        </asp:Legend>
                        <asp:Legend LegendStyle="Row" Name="Top Legend" TableStyle="Wide" Docking="Top" Alignment="Center">
                            <CellColumns>
                                <asp:LegendCellColumn Name="Weeks" Text="">
                                    <Margins Left="15" Right="15" Top="1" Bottom="1"></Margins>
                                </asp:LegendCellColumn>
                            </CellColumns>
                        </asp:Legend>
                    </Legends>
                    <Series>
                        <asp:Series Name="chartSeries" ChartType="RangeBar" />
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="MainArea">
                            <AxisY IsLabelAutoFit="False" LineDashStyle="NotSet">
                                <MajorGrid LineColor="DimGray" />
                                <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                                <LabelStyle Format="dd"/>
                            </AxisY>
                            <AxisY2 IsLabelAutoFit="False" Enabled="True">
                                <MajorGrid LineColor="DimGray" />
                                <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                                <LabelStyle Format="dd" />
                            </AxisY2>
                            <AxisX IsLabelAutoFit="true">
                                <MajorGrid Interval="Auto" LineDashStyle="Dot" />
                                <MajorTickMark Enabled="False" />
                                <%--<LabelStyle TruncatedLabels="false" IsStaggered="true" />--%>
                            </AxisX>
                            <Area3DStyle Inclination="5" IsClustered="True" IsRightAngleAxes="False" LightStyle="Realistic"
                                WallWidth="30" Perspective="1" />
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

