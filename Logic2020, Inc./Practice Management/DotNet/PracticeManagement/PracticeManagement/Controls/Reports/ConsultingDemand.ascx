<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsultingDemand.ascx.cs"
    Inherits="PraticeManagement.Controls.Reporting.ConsultingDemand" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
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
            if ((startYear == endYear && ((endMonth - startMonth + 1) <= 12))
        || (((((endYear - startYear) * 12 + endMonth) - startMonth + 1)) <= 12)
        || ((endDate - startDate) / (1000 * 60 * 60 * 24)) < 360
        ) {
                ClearValidations();
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
    }

    function ValidatePeriod(sender, args) {
        args.IsValid = PeriodValidate();
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
                return ((endMonth - startMonth + 1) <= 12);
            }
            else {
                return (((((endYear - startYear) * 12 + endMonth) - startMonth + 1)) <= 12);
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

    function CheckAndShowCustomDatesPoup(ddlPeriod) {
        imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
        lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
        ClearValidations();

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
        hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
        hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
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
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
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

                var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
                var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
                if (startDateCalExtender != null) {
                    startDateCalExtender.set_selectedDate(new Date(startDate.format("MM/dd/yyyy")));
                }
                if (endDateCalExtender != null) {
                    endDateCalExtender.set_selectedDate(new Date(endDate.format("MM/dd/yyyy")));
                }
            }
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
    }

    function ClearValidations() {
        var valSummary = document.getElementById('<%= valSum.ClientID %>');
        var cstvalPeriodRange = document.getElementById('<%= cstvalPeriodRange.ClientID %>');

        cstvalPeriodRange.style.display = 'none';
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
        var hdnDefaultMonths = document.getElementById('<%= hdnDefaultMonths.ClientID %>');

        if (ddlPeriod != null && hdnDefaultMonths != null) {
            ddlPeriod.value = hdnDefaultMonths.value;
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

            lblCustomDateRange.innerHTML = '(' + hdnStartDate.value + '&nbsp;-&nbsp;' + hdnEndDate.value + ')';
        }

        imgCalender.attributes["class"].value = "displayNone";
        lblCustomDateRange.attributes["class"].value = "displayNone";
        btnReset.disabled = 'disabled';

        return false;
    }

    function ValidAll() {
        //var validators = Page_Validators;
        return Page_IsValid && PeriodValidate();
    }

</script>
<div class="FadingTooltip FadingTooltipDefault" id="FADINGTOOLTIP">
</div>
<uc:LoadingProgress ID="lpConsultingDemand" runat="server" />
<asp:UpdatePanel ID="updConsReport" runat="server">
    <ContentTemplate>
        <div class="buttons-block">
            <table class="WholeWidth">
                <tr>
                    <td align="left" class="Width98Percent">
                        Show Consultants Demand for &nbsp;
                        <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="false" onchange="EnableResetButton(); CheckAndShowCustomDatesPoup(this);">
                            <asp:ListItem Selected="True" Text="Next 3 Months" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;<asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                        &nbsp;<asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                        <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                            CancelControlID="btnCustDatesCancel" OkControlID="btnCustDatesClose" BackgroundCssClass="modalBackground"
                            PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates" DropShadow="false" />
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
                            <asp:HiddenField ID="hdnDefaultMonths" runat="server" Value="3" />
                            <asp:Button ID="btnUpdateView" runat="server" Text="Update View" CssClass="Width90PxImp"
                                OnClick="btnUpdateView_OnClick" EnableViewState="False" />
                        </td>
                        <td>
                            <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" CssClass="Width90PxImp"
                                OnClientClick="if(!ResetFilters(this)){return false;}" />
                        </td>
                    </td>
                </tr>
            </table>
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
                                    <asp:CustomValidator ID="cstvalPeriodRange" runat="server" ClientValidationFunction="ValidatePeriod"
                                        Text="*" EnableClientScript="true" ValidationGroup="<%# ClientID %>" ToolTip="Period should not be more than 12 months"
                                        ErrorMessage="Period should not be more than 12 months." Display="Dynamic"></asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="custBtns">
                        <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="CheckIfDatesValid(); if(ValidAll()) return false;"
                            Text="OK" CausesValidation="true" />
                        <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true"
                            OnClientClick="return false;" />
                        &nbsp; &nbsp;
                        <asp:Button ID="btnCustDatesCancel" OnClientClick="ReAssignStartDateEndDates(); ClearValidations(); return false;"
                            runat="server" Text="Cancel" />
                    </td>
                </tr>
                <tr>
                    <td class="textCenter">
                        <asp:ValidationSummary ID="valSum" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel ID="updReport" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="chartDiv" runat="server" class="ConsultingDemandchartDiv">
            <asp:Chart ID="chrtConsultingDemand" runat="server" CssClass="Width920Px">
                <Legends>
                    <asp:Legend LegendStyle="Row" Name="Botom Legend" TableStyle="Wide" Docking="Bottom"
                        Alignment="Center" TextWrapThreshold="50">
                        <CellColumns>
                            <asp:LegendCellColumn Name="Weeks" Text="">
                                <Margins Left="15" Right="15"></Margins>
                            </asp:LegendCellColumn>
                        </CellColumns>
                    </asp:Legend>
                    <asp:Legend LegendStyle="Row" Name="Top Legend" TableStyle="Wide" Docking="Top" Alignment="Center"
                        TextWrapThreshold="50">
                        <CellColumns>
                            <asp:LegendCellColumn Name="Weeks" Text="">
                                <Margins Left="15" Right="15" Bottom="1"></Margins>
                            </asp:LegendCellColumn>
                        </CellColumns>
                    </asp:Legend>
                </Legends>
                <Series>
                    <asp:Series Name="chartSeries" ChartType="RangeBar">
                    </asp:Series>
                </Series>
                <ChartAreas>
                    <asp:ChartArea Name="MainArea">
                        <AxisY IsLabelAutoFit="False" LineDashStyle="NotSet">
                            <MajorGrid LineColor="DimGray" />
                            <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                            <LabelStyle Angle="0" Format="dd" />
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

