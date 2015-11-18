<%@ Page Title="18-Month Management Reports" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="MSReportsLandingPage.aspx.cs" Inherits="PraticeManagement.Reports.Badge.MSReportsLandingPage" %>

<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
        <br />
            <table class="WholeWidth">
                <thead class="font16Px">
                    <tr>
                        <th class="Width1Percent">
                            &nbsp;
                        </th>
                        <th class="textLeft Width20Percent">
                            Overview of Resources
                        </th>
                        <th class="textLeft">
                            Description
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlBadgedResourcesByTime" runat="server" Text="Badged Resources by Time" ToolTip="Badged Resources by Time"
                                Target="_blank" NavigateUrl="~/Reports/Badge/BadgeResourceByTime.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View resource availability by time (day, week, month)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlResourcesByPractice" runat="server" Text="Available Resources by Practice" ToolTip="Available Resources by Practice"
                                Target="_blank" NavigateUrl="~/Reports/Badge/ResourceByPracticeReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View available resources by practice area
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlResourcesByBusinessLvl" runat="server" Text="Available Resources by Business Level" ToolTip="Available Resources by Business Level"
                                Target="_blank" NavigateUrl="~/Reports/Badge/ResourceByBusinessConsultants.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View available resources by business level
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlResourcesByTechnicalLvl" runat="server" Text="Available Resources by Technical Level" ToolTip="Available Resources by Technical Level"
                                Target="_blank" NavigateUrl="~/Reports/Badge/ResourcesByTechnicalConsultants.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View available resources by technical level
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlAllEmployees18Clock" runat="server" Text="All Employees' 18-Month Clock Dates" ToolTip="All Employees' 18-Month Clock Dates"
                                Target="_blank" NavigateUrl="~/Reports/Badge/AllEmployees18MoClockReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View all resources and respective clock dates
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlPersonsByProject" runat="server" Text="Persons By Project" ToolTip="Persons By Project"
                                Target="_blank" NavigateUrl="~/Reports/Badge/PersonsByProject.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View all resources by project
                        </td>
                    </tr>
                     <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlManagementMeetingReport" runat="server" Text="18-Month Management Meeting Report" ToolTip="18-Month Management Meeting Report"
                                Target="_blank" NavigateUrl="~/Reports/Badge/MSManagementMeetingReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View all resources by level
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            <br />
            <table class="WholeWidth">
                <thead class="font16Px">
                    <tr>
                        <th class="Width1Percent">
                            &nbsp;
                        </th>
                        <th class="textLeft Width20Percent">
                            Available Resources
                        </th>
                        <th>
                            &nbsp;
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlBadgeNotonProject" runat="server" Text="Resources with Active Clocks, Not on Project" ToolTip="Resources with Active Clocks, Not on Project"
                                Target="_blank" NavigateUrl="~/Reports/Badge/BadgedNotOnProjectReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View resources with active 18-month clocks who are not deployed to a project
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlBadgedOnProject" runat="server" Text="Badged Resources on Project" ToolTip="Badged Resources on Project"
                                Target="_blank" NavigateUrl="~/Reports/Badge/BadgedOnProjectReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View badged resources and the projects for which they are aligned
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10"> 
                            <asp:HyperLink ID="hlClockNotStarted" runat="server" Text="Resources with 18-Month Clock Not Started" ToolTip="Resources with 18-Month Clock Not Started"
                                Target="_blank" NavigateUrl="~/Reports/Badge/ClockNotStartedReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View available resources whose 18-month clock has not yet started
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            <br />
            <table class="WholeWidth">
                <thead class="font16Px">
                    <tr>
                        <th class="Width1Percent">
                            &nbsp;
                        </th>
                        <th class="textLeft Width20Percent">
                            Resources with Exceptions
                        </th>
                        <th>
                            &nbsp;
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlPersonBasedException" runat="server" Text="Resources with Person-Based Exceptions" ToolTip="Resources with Person-Based Exceptions"
                                Target="_blank" NavigateUrl="~/Reports/Badge/BadgedNotOnPersonBasedExceptionReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View all resources with person-based exceptions
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlProjectBasedException" runat="server" Text="Resources with Project-Based Exceptions" ToolTip="Resources with Project-Based Exceptions"
                                Target="_blank" NavigateUrl="~/Reports/Badge/BadgedOnProjectBasedExceptionReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View all resources with project-based exceptions
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            <br />
            <table class="WholeWidth">
                <thead class="font16Px">
                    <tr>
                        <th class="Width1Percent">
                            &nbsp;
                        </th>
                        <th class="textLeft Width20Percent">
                            Unavailable Resources
                        </th>
                        <th>
                            &nbsp;
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlBlocked" runat="server" Text="Blocked Resources" Target="_blank" ToolTip="Blocked Resources"
                                NavigateUrl="~/Reports/Badge/BadgeBlockedReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View all resources on a Logic20/20-mandated block
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlBreak" runat="server" Text="Resources on 6-Month Break" Target="_blank" ToolTip="Resources on 6-Month Break"
                                NavigateUrl="~/Reports/Badge/BadgeBreakReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            View all resources who are on a Microsoft-mandated break (they cannot be badged)
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            <br />
            <table class="WholeWidth">
                <thead class="font16Px">
                    <tr>
                        <th class="Width1Percent">
                            &nbsp;
                        </th>
                        <th class="textLeft Width20Percent">
                            Badge Requests
                        </th>
                        <th>
                            &nbsp;
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td class="PaddingTop10">
                            <asp:HyperLink ID="hlBadgeNotApproved" runat="server" Text="Badge Requests Not Yet Approved" ToolTip="Badge Requests Not Yet Approved"
                                Target="_blank" NavigateUrl="~/Reports/Badge/BadgeRequestNotApprovedReport.aspx">
                            </asp:HyperLink>
                        </td>
                        <td class="PaddingTop10">
                            For Operations use; view all badge requests that have not yet been approved
                        </td>
                    </tr>
                </tbody>
            </table>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

