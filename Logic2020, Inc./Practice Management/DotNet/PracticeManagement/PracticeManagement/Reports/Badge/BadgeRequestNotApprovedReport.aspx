﻿<%@ Page Title="Badge Requests Not Yet Approved" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="BadgeRequestNotApprovedReport.aspx.cs" Inherits="PraticeManagement.Reports.Badge.BadgeRequestNotApprovedReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
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
                            <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport zebra">
                                <thead>
                                    <tr>
                                        <th class="TextAlignLeftImp Padding5Imp">
                                            Current Badge Requests Not Yet Approved
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Project #
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Request Date
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Badge Start
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Badge End
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
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# Eval("Project.ProjectNumber")%> '
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("PlannedEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeEndDate"))%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alterrow">
                            <td class="padLeft5 textLeft">
                                <%# Eval("Person.HtmlEncodedName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# Eval("Project.ProjectNumber")%> '
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("PlannedEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeEndDate"))%>
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

