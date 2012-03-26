<%@ Page Title="Project Summary Report" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ProjectSummaryReport.aspx.cs" Inherits="PraticeManagement.Reporting.ProjectSummaryReport" %>

<%@ Register Src="~/Controls/Reports/TimeEntryReportsHeader.ascx" TagPrefix="uc"
    TagName="TimeEntryReportsHeader" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/ProjectSummaryByResource.ascx" TagPrefix="uc"
    TagName="ByResource" %>
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
    <link href="../Css/TableSortStyle.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        function btnClose_OnClientClick(popup) {
            $find(popup).hide();
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.tablesorter.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblProjectSummaryByResource").tablesorter(
            {
                headers: {
                    6: {
                        sorter: false
                    }
                }
            }

            );
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {
            $(document).ready(function () {
                $("#tblProjectSummaryByResource").tablesorter(
                {
                    headers: {
                        6: {
                            sorter: false
                        }
                    }
                }

                );
            });
        }


    </script>
    <uc:TimeEntryReportsHeader ID="timeEntryReportHeader" runat="server"></uc:TimeEntryReportsHeader>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="height30P vBottom fontBold">
                        2.&nbsp;Select report parameters:
                    </td>
                </tr>
            </table>
            <table style="width: 100%;">
                <tr>
                    <td style="width: 33%;">
                        &nbsp;
                    </td>
                    <td style="width: 34%;" align="center" style="text-align: center;">
                        <table class="PaddingTenPx">
                            <tr>
                                <td>
                                    Project Number:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtProjectNumber" AutoPostBack="true" OnTextChanged="txtProjectNumber_OnTextChanged"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Image ID="imgProjectSearch" runat="server" ToolTip="Project Search" ImageUrl="~/Images/search_24.png" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 33%;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="border-bottom: 3px solid black; width: 100%; padding-top: 10px;">
                    </td>
                </tr>
            </table>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeProjectSearch" runat="server" TargetControlID="imgProjectSearch"
                CancelControlID="btnclose" BackgroundCssClass="modalBackground" PopupControlID="pnlProjectSearch"
                BehaviorID="mpeProjectSearch" DropShadow="false" />
            <asp:Panel ID="pnlProjectSearch" runat="server" BackColor="White" BorderColor="Black"
                Style="display: none;" BorderWidth="2px" Width="350px">
                <table width="100%" class="ProjectSearchPopup">
                    <tr>
                        <th align="center" style="text-align: center; background-color: Gray;" valign="bottom"
                            colspan="2">
                            <b style="font-size: 14px; padding-top: 2px;">Project Search</b>
                            <asp:Button ID="btnclose" runat="server" CssClass="mini-report-close" ToolTip="Close"
                                Style="float: right;" OnClientClick="return btnClose_OnClientClick('mpeProjectSearch');"
                                Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            Account:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlClients" runat="server" Width="250" OnSelectedIndexChanged="ddlClients_OnSelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Project:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlProjects" runat="server" Enabled="false" AutoPostBack="true"
                                Width="250" OnSelectedIndexChanged="ddlProjects_OnSelectedIndexChanged">
                                <asp:ListItem Text="-- Select a Project --" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div id="divWholePage" runat="server">
                <%--          <table style="width: 100%;">
                <tr>
                    <td align="center" style="padding:10px;">
                        <uc:BillableAndNonBillableGraph ID="ucBillableAndNonBillable" runat="server"></uc:BillableAndNonBillableGraph>
                    </td>
                </tr>
            </table>--%>
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
                            <%--<uc:ByWorkType ID="ucByWorktype" runat="server"></uc:ByWorkType>--%>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ucByResource$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

