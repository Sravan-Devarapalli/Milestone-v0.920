<%@ Page Title="Badge Requests Not Yet Approved" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="BadgeRequestNotApprovedReport.aspx.cs" Inherits="PraticeManagement.Reports.Badge.BadgeRequestNotApprovedReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../../Scripts/jquery.tablesorter.yui.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblNotApproved").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            $("#tblNotApproved").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <div id="divWholePage" runat="server">
                <table id="tblRange" runat="server" class="WholeWidth">
                    <tr>
                        <td style="text-align: right; padding-right: 30px;">
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Export" OnClick="btnExportToExcel_OnClick"
                                Enabled="true" UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                    </tr>
                    <tr>
                        <td class="paddingBottom10px">
                            <asp:Label ID="lblTitle" runat="server" Text="Badge Requests Not Yet Approved:" Style="font-weight: bold;
                                font-size: 20px;"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:Repeater ID="repbadgeNotApproved" runat="server">
                    <HeaderTemplate>
                        <div class="minheight250Px">
                            <table id="tblNotApproved" class="tablesorter PersonSummaryReport zebra">
                                <thead>
                                    <tr>
                                        <th class="TextAlignLeftImp Padding5Imp Width300Px">
                                            Current Badge Requests Not Yet Approved
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Resource Level
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Project #
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Milestone
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Project Stage
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Request Date
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Requester
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Badge Start
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Badge End
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            18-mos Clock End Date
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate">
                            <td class="padLeft5 textLeft">
                                <%# Eval("Person.HtmlEncodedName")%>
                            </td>
                             <td class="DayTotalHoursBorderLeft Padding5Imp">
                                 <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Project.ProjectNumber")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:HyperLink ID="hlMilestoneName" runat="server" Text=' <%# Eval("Milestone.Description")%> '
                                    Target="_blank" NavigateUrl='<%# GetMilestoneDetailsLink((int?)(Eval("Milestone.Id")),(int?)(Eval("Project.Id"))) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Project.Status.StatusType")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("PlannedEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Requester")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("ProjectBadgeEndDate"))%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alterrow">
                            <td class="padLeft5 textLeft">
                                <%# Eval("Person.HtmlEncodedName")%>
                            </td>
                             <td class="DayTotalHoursBorderLeft Padding5Imp">
                                 <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Project.ProjectNumber")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:HyperLink ID="hlMilestoneName" runat="server" Text=' <%# Eval("Milestone.Description")%> '
                                    Target="_blank" NavigateUrl='<%# GetMilestoneDetailsLink((int?)(Eval("Milestone.Id")),(int?)(Eval("Project.Id"))) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Project.Status.StatusType")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("PlannedEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Requester")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("ProjectBadgeEndDate"))%>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </tbody></table></div>
                    </FooterTemplate>
                </asp:Repeater>
                <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
                    There are no resources whose badge requestes are not approved.
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

