<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewHireReportGraphView.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.HumanCapital.NewHireGraphView" %>
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" class="Width95Percent">
        </td>
        <td class=" Width5Percent padRight5">
            <table class="WholeWidth">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To Excel" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<%--<div id="chartDiv" runat="server" class="ConsultingDemandchartDiv">
    <asp:Chart ID="chrtNewHireReport" runat="server" CssClass="Width920Px">
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
                        <Margins Left="15" Right="15" Bottom="1"></Margins>
                    </asp:LegendCellColumn>
                </CellColumns>
            </asp:Legend>
        </Legends>
        <Series>
            <asp:Series Name="chartSeries" ChartType="Bar">
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
                   
                </AxisX>
                <Area3DStyle Inclination="5" IsClustered="True" IsRightAngleAxes="False" LightStyle="Realistic"
                    WallWidth="30" Perspective="1" />
            </asp:ChartArea>
        </ChartAreas>
    </asp:Chart>
</div>
--%>
