<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsultantsWeeklyReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ConsultantsWeeklyReport" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<%@ Register Src="~/Controls/Reports/UTilTimelineFilter.ascx" TagName="UtilTimelineFilter"
    TagPrefix="uc" %>
<script type="text/javascript">
    $(document).ready(
    function () {
        $("#" + "<%= updPersonDetails.ClientID%>").css("width", "100%");
    }
    );

    function saveReport() {
        var printContent;
        var hdnSaveReportText;
        printContent = document.getElementById('<%= divConsultingUtil.ClientID %>');
        hdnSaveReportText = document.getElementById('<%= hdnSaveReportText.ClientID %>');
        hdnSaveReportText.value = printContent.innerHTML;
    }

    function ClosePopUp() {
        var popup = $find('mpeExportBehaviourId');
        popup.hide();
    }

    function Applytargetatt() {
        var area = document.getElementsByTagName("area");
        var hdn = document.getElementById("<%= hdnpopup.ClientID%>");
        if (area.length != 0) {
            for (var i = 0; i < area.length; i++) {
                var type = area[i].getAttribute("shape");
                if (type == "poly" || hdn.value == "true") {
                    area[i].setAttribute("target", "_blank");
                }
                else {
                    area[i].removeAttribute("target");
                }
            }
        }
    }

    function OnCancelClick() {
        var hdn = document.getElementById("<%= hdnpopup.ClientID%>");
        hdn.value = "false";
        Applytargetatt();
    }

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

    function endRequestHandle(sender, Args) {
        Applytargetatt();
    }

    window.scrollTo = function (x, y) {

        return true;
    }

</script>
<asp:UpdatePanel ID="updFilters" runat="server">
    <ContentTemplate>
        <uc:UtilTimelineFilter ID="utf" OnEvntHandler_OnUpDateView_Click="btnUpdateView_OnClick"
            BtnSaveReportVisible="false" OnEvntHandler_OnResetFilter_Click="btnResetFilter_OnClick"
            runat="server" />
        <asp:HiddenField ID="hdnIsChartRenderedFirst" runat="server" Value="false" />
        <asp:HiddenField ID="hdnIsSampleReport" runat="server" />
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
                                    <asp:Button ID="btnExport" runat="server" Text="PDF" OnClick="btnExport_Click" UseSubmitBehavior="false"
                                        ToolTip="Export To PDF" OnClientClick="saveReport();" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div class="ConsultantsWeeklyReportAlignCenter" id="divConsultingUtil" runat="server">
            <asp:Chart ID="chart" CssClass="ConsultantsWeeklyReportAlignCenter" runat="server"
                Width="1100px" Height="850px" OnClick="Chart_Click">
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
                No Resources for the selected filters.
            </div>
            <br />
                <asp:Chart ID="investmentChart" CssClass="ConsultantsWeeklyReportAlignCenter" runat="server"
                    Width="1100px" Height="850px" OnClick="Chart_Click">
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
                <br />
            <div id="emptyInvestment" runat="server" visible="false">
                No InvestmentResources
            </div>
        </div>
        <asp:HiddenField ID="hdnExportOptions" Value="false" runat="server" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpeExportOptions" runat="server" TargetControlID="hdnExportOptions"
            BehaviorID="mpeExportBehaviourId" BackgroundCssClass="modalBackground" PopupControlID="pnlExportOptions"
            CancelControlID="btnCancelExport" DropShadow="false" />
        <asp:Panel ID="pnlExportOptions" runat="server" CssClass="popUp yScrollAuto minwidth350px"
            Style="display: none;">
            <table class="WholeWidth">
                <tr class="PopUpHeader">
                    <th>
                        Please select one of the options below to export
                    </th>
                </tr>
                <tr>
                    <td class="ConsultPopup">
                        <asp:RadioButton ID="rbUtilization" runat="server" Checked="true" GroupName="ExportOptions" />
                        &nbsp;&nbsp; Utilization only
                    </td>
                </tr>
                <tr>
                    <td class="ConsultPopup">
                        <asp:RadioButton ID="rbProjectUtilization" runat="server" GroupName="ExportOptions" />
                        &nbsp;&nbsp; Project and Utilization
                    </td>
                </tr>
                <tr>
                    <td class="ConsultPopup">
                        <asp:RadioButton ID="rbHoursUtilization" runat="server" GroupName="ExportOptions" />
                        &nbsp;&nbsp; Hours
                    </td>
                </tr>
                <tr>
                    <td class="textCenter NewHireGraphEmptyDiv">
                        <asp:Button ID="btnOkExport" runat="server" ToolTip="OK" Text="OK" CssClass="Width100PxImp"
                            OnClientClick="ClosePopUp()" OnClick="btnOkExport_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCancelExport" runat="server" ToolTip="Cancel" Text="Cancel" CssClass="Width100PxImp" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="btnExport" />
        <asp:PostBackTrigger ControlID="btnOkExport" />
    </Triggers>
</asp:UpdatePanel>
<div id="loading_img" style="width: 100%; text-align: center;" class="tab-invisible">
    Loading detailed report...<br />
    <img alt="Loading..." src="../Images/ajax-loader.gif" />
</div>
<AjaxControlToolkit:UpdatePanelAnimationExtender ID="uaeDetails" runat="server" TargetControlID="updPersonDetails">
    <Animations>
        <OnUpdating>
            <Sequence>
                <FadeOut duration=".25" AnimationTarget="container" minimumOpacity="0" />
                <ScriptAction Script="ShowImage();" />
            </Sequence>
        </OnUpdating>
        <OnUpdated>
            <Sequence>
                <ScriptAction Script="HideImage();" />
                <FadeIn duration=".25" AnimationTarget="container" minimumOpacity="0" />
            </Sequence>
        </OnUpdated>
    </Animations>
</AjaxControlToolkit:UpdatePanelAnimationExtender>
<asp:HiddenField ID="hdnTempField" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeConsultantDetailReport" runat="server"
    TargetControlID="hdnTempField" BackgroundCssClass="modalBackground" PopupControlID="pnlConsultantDetailReport"
    DropShadow="false" CancelControlID="btnCancelConsultantDetailReport" OnCancelScript="OnCancelClick();" />
<asp:Panel ID="pnlConsultantDetailReport" CssClass="ConsultantUtilReportPopup" Style="display: none;"
    runat="server">
    <table class="WholeWidth Padding5">
        <tr>
            <td colspan="3" class="BackGroundWhiteImp">
                <asp:Button ID="btnCancelConsultantDetailReport" runat="server" CssClass="mini-report-close floatright"
                    ToolTip="Close" Text="X"></asp:Button>
            </td>
        </tr>
        <tr class="bgGroupByProjectHeader BackGroundWhiteImp">
            <td>
                <div class="ConsultantsWeeklyReportupdPersonDetails">
                    <asp:UpdatePanel ID="updPersonDetails" runat="server">
                        <ContentTemplate>
                            <div class="textCenter">
                                <table id="Table1" cellpadding="0" cellspacing="0" width="100%" runat="server">
                                    <tr>
                                        <td align="center">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" class="bgcolorwhite">
                                            <asp:HiddenField ID="hdnpopup" runat="server" />
                                            <asp:Chart ID="chartDetails" Width="850px" Height="600px" runat="server" Visible="false">
                                                <Series>
                                                    <asp:Series Name="Milestones" ChartType="RangeBar" XValueMember="Description" YValueMembers="StartDate,EndDate">
                                                    </asp:Series>
                                                </Series>
                                                <Titles>
                                                    <asp:Title Name="MilestonesTitle" />
                                                </Titles>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="MilestonesArea">
                                                        <AxisY IsLabelAutoFit="False" LineDashStyle="NotSet">
                                                            <MajorGrid LineColor="DimGray" LineDashStyle="Dash" />
                                                            <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                                                            <LabelStyle Format="MMM, d, yyyy" />
                                                        </AxisY>
                                                        <AxisX IsLabelAutoFit="true" InterlacedColor="#66FF33" IsMarksNextToAxis="False"
                                                            Interval="1">
                                                            <LabelStyle Enabled="false" />
                                                            <MajorGrid Interval="1" LineDashStyle="Dot" LineColor="Silver" />
                                                            <MajorTickMark Enabled="False" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>
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

