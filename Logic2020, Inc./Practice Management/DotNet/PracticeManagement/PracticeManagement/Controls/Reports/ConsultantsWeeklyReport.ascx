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

    function ShowImage() {
        var hdn = document.getElementById("<%= hdnIsChartRenderedFirst.ClientID%>");
        if (hdn.value == 'true') {
            document.getElementById('loading_img').className = 'tab-visible';
        }
    }
    window.scrollTo = function (x, y) {

        return true;
    }

    function HideImage()
    {
    document.getElementById('loading_img').className = 'tab-invisible'
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
<asp:UpdatePanel ID="updConsReport" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="ConsultantsWeeklyReportAlignCenter">
            <asp:Chart ID="chart" CssClass="ConsultantsWeeklyReportAlignCenter" runat="server"
                Width="920px" Height="850px" OnClick="Chart_Click">
                <Legends>
                    <asp:Legend LegendStyle="Row" Name="Botom Legend" TableStyle="Wide" Docking="Bottom"
                        Alignment="Center">
                        <CellColumns>
                            <asp:LegendCellColumn Name="Weeks" Text="">
                                <Margins Left="15" Right="15"></Margins>
                            </asp:LegendCellColumn>
                        </CellColumns>
                    </asp:Legend>
                    <asp:Legend LegendStyle="Row" Name="Top Legend" TableStyle="Wide" Docking="Top" Alignment="Center">
                        <CellColumns>
                            <asp:LegendCellColumn Name="Weeks" Text="">
                                <Margins Left="15" Right="15"></Margins>
                            </asp:LegendCellColumn>
                        </CellColumns>
                    </asp:Legend>
                </Legends>
                <Series>
                    <asp:Series Name="Weeks" ChartType="RangeBar" />
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
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<div id="loading_img" style="width: 100%; text-align: center;" class="tab-invisible">
    Loading detailed report...<br />
    <img alt="Loading..." src="../Images/ajax-loader.gif" />
</div>
<div class="ConsultantsWeeklyReportupdPersonDetails">
    <asp:UpdatePanel ID="updPersonDetails" runat="server">
        <ContentTemplate>
            <div class="textCenter">
                <table cellpadding="0" cellspacing="0" width="100%" runat="server" id="tblDetails">
                    <tr>
                        <td align="center">
                            <a name="details">Detailed report (click on colored bar to load)</a>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="bgcolorwhite">
                            <asp:Chart ID="chartDetails" Width="886px" Height="600px" runat="server" Visible="false">
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

