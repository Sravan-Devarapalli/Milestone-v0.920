<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CumulativeDailyActivity.ascx.cs" Inherits="PraticeManagement.Controls.MilestonePersons.CumulativeDailyActivity" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagPrefix="uc" TagName="MessageLabel" %>
<asp:Chart ID="activityChart" runat="server" Width="900px" Height="300px">
    <Legends>
        <asp:Legend Alignment="Center" Docking="Top" Name="MainLegend">
        </asp:Legend>
    </Legends>
    <Series>
        <asp:Series Name="Expectation Cumulative" BorderWidth="2" ChartType="Area" 
            Legend="MainLegend" Color="228,92,46" ChartArea="CumulativeArea">
        </asp:Series>
        <asp:Series Name="Actual Cumulative" BorderWidth="2" ChartType="Area" 
            Legend="MainLegend" Color="Gold" ChartArea="CumulativeArea">
        </asp:Series>
        <asp:Series Name="Projected Cumulative" BorderWidth="4" ChartType="StepLine" 
            Legend="MainLegend" Color="117,171,243" ChartArea="CumulativeArea">
        </asp:Series>
        <asp:Series Name="Projected Total" BorderWidth="2" ChartType="StepLine" 
            Legend="MainLegend" Color="117,171,243" ChartArea="CumulativeArea">
        </asp:Series>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="CumulativeArea">
            <AxisY Title="Hours">
                <MajorGrid LineColor="Gray" LineDashStyle="DashDot" />                 
            </AxisY>
            <AxisX Title="Time">
                <MajorGrid LineColor="DarkGray" LineDashStyle="Dot" IntervalType="Days"/>                 
                <LabelStyle Angle="60" />
            </AxisX>
        </asp:ChartArea>
    </ChartAreas>
</asp:Chart>
<uc:MessageLabel ID="mlErrors" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
            WarningColor="Orange" EnableViewState="false" />

