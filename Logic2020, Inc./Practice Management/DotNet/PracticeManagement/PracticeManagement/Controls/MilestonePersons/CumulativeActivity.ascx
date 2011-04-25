<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CumulativeActivity.ascx.cs" Inherits="PraticeManagement.Controls.MilestonePersons.CumulativeActivity" %>
<asp:Chart ID="chCumulative" runat="server" Height="350px" Width="500px">
    <Legends>
        <asp:Legend Alignment="Center" Docking="Bottom" Name="MainLegend">
        </asp:Legend>
    </Legends>
    <Series>
        <asp:Series Name="Total Projected" ChartArea="CumulativeArea" ChartType="Bar" 
            Legend="MainLegend" LegendToolTip="Total projected hours for period">
        </asp:Series>
        <asp:Series ChartArea="CumulativeArea" ChartType="Bar" 
            LabelToolTip="Actual hours to date" Legend="MainLegend" Name="Actual to Date">
        </asp:Series>
        <asp:Series ChartArea="CumulativeArea" ChartType="Bar" Legend="MainLegend" 
            LegendToolTip="Remaining hours" Name="Projected and Actual">
        </asp:Series>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="CumulativeArea">
            <AxisY IntervalOffsetType="NotSet">
                <MajorGrid LineColor="DarkGray" LineDashStyle="Dash" />
            </AxisY>
            <AxisX>
                <MajorGrid LineWidth="0" />
            </AxisX>
        </asp:ChartArea>
    </ChartAreas>
</asp:Chart>

