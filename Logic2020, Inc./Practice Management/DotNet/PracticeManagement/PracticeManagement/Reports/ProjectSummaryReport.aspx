<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ProjectSummaryReport.aspx.cs" Inherits="PraticeManagement.Reporting.ProjectSummaryReport" %>

<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/ProjectSummaryByResource.ascx" TagPrefix="uc"
    TagName="ByResource" %>
<%@ Register Src="~/Controls/Reports/ProjectSummaryByMatrix.ascx" TagPrefix="uc"
    TagName="ByMatrix" %>
<%@ Register Src="~/Controls/Reports/ByworkType.ascx" TagPrefix="uc" TagName="ByWorkType" %>
<%@ Register Src="~/Controls/Reports/BillableAndNonBillableGraph.ascx" TagPrefix="uc"
    TagName="BillableAndNonBillableGraph" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style>
        /* --------- Tabs for person and project details pages ------ */
        
        .CustomTabStyle tr
        {
            height: 30px;
        }
        
        .CustomTabStyle td
        {
            background-color: White;
            float: left;
            padding: 8px 0px 5px 0px;
            position: relative;
        }
        
        .CustomTabStyle td a
        {
            text-decoration: none;
        }
        
        .CustomTabStyle td span a
        {
            border-bottom: 1px dashed #0898e6;
        }
        
        .CustomTabStyle td span a:hover
        {
            border-bottom: 1px dashed #006699;
        }
        
        .CustomTabStyle td a.collapse
        {
            display: none;
            position: absolute;
        }
        
        .CustomTabStyle .SelectedSwitch a.collapse
        {
            display: block;
            right: 2px;
            top: 10px;
        }
        
        .CustomTabStyle td span.bg
        {
            padding: 8px 10px 7px 10px;
        }
        
        .CustomTabStyle .SelectedSwitch span.bg
        {
            background-color: #e2ebff;
        }
        
        .tab-pane
        {
            background-color: #e2ebff;
            padding: 5px;
        }
        
        .info-field
        {
            width: 152px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <br />
            <hr />
            <table style="width: 100%;">
                <tr>
                    <td align="center">
                        <uc:BillableAndNonBillableGraph ID="ucBillableAndNonBillable" runat="server"></uc:BillableAndNonBillableGraph>
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr>
                    <td align="center">
                        <asp:Table ID="tblProjectsummaryReportViewSwitch" runat="server" CssClass="CustomTabStyle">
                            <asp:TableRow ID="rowSwitcher" runat="server">
                                <asp:TableCell ID="cellResource" CssClass="SelectedSwitch" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="lnkbtnResource" runat="server" Text="By Resource" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellWorkType" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="lnkbtnWorkType" runat="server" Text="By Work Type" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellProject" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="lnkbtnProject" runat="server" Text="Matrix" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                </tr>
            </table>
            <asp:MultiView ID="mvProjectSummaryReport" runat="server" ActiveViewIndex="0">
                <asp:View ID="vwResourceReport" runat="server">
                    <asp:Panel ID="pnlResourceReport" runat="server" CssClass="tab-pane WholeWidth">
                        <uc:ByResource ID="ucByResource" runat="server"></uc:ByResource>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwProjectReport" runat="server">
                    <asp:Panel ID="pnlProjectReport" runat="server" CssClass="tab-pane WholeWidth">
                        <uc:ByWorkType ID="ucByWorktype" runat="server"></uc:ByWorkType>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwWorkTypeReport" runat="server">
                    <asp:Panel ID="pnlWorkTypeReport" runat="server" CssClass="tab-pane WholeWidth">
                        <uc:ByMatrix ID="ucByMatrix" runat="server"></uc:ByMatrix>
                    </asp:Panel>
                </asp:View>
            </asp:MultiView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

