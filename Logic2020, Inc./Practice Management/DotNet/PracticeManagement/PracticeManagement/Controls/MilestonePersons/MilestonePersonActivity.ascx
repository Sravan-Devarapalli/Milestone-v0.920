<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestonePersonActivity.ascx.cs" Inherits="PraticeManagement.Controls.MilestonePersons.MilestonePersonActivity" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagPrefix="uc" TagName="MessageLabel" %>
<asp:Chart ID="activityChart" runat="server" Width="900px" Height="300px">
    <Legends>
        <asp:Legend Alignment="Center" Docking="Top" Name="MainLegend">
        </asp:Legend>
        <asp:Legend Alignment="Center" Docking="Bottom" Name="CumulativeLegend" Enabled="false">
        </asp:Legend>
    </Legends>
    <Series>
        <asp:Series Name="Projected" BorderWidth="2" ChartType="StepLine" 
            Legend="MainLegend" ChartArea="MainArea">
        </asp:Series>
        <asp:Series Name="Actual" BorderWidth="2" ChartType="Spline" 
            Legend="MainLegend" Color="Gold" ChartArea="MainArea">
        </asp:Series>
        <asp:Series Name="Actual (points)" BorderWidth="8" ChartType="Point" 
            Legend="MainLegend" ChartArea="MainArea">
        </asp:Series>
        <asp:Series Name="Expectation" BorderWidth="2" ChartType="Spline" 
            Legend="MainLegend" ChartArea="MainArea">
        </asp:Series>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="MainArea">
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

