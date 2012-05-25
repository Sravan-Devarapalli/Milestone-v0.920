﻿<%@ Page Title="By Account" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="AccountSummaryReport.aspx.cs" Inherits="PraticeManagement.Reports.AccountSummaryReport" %>

<%@ Register Src="~/Controls/Reports/TimeEntryReportsHeader.ascx" TagPrefix="uc"
    TagName="TimeEntryReportsHeader" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
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
                    },
                    1: {
                        sorter: false
                    },
                    7: {
                        sorter: false
                    }
                },
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });

            $("#tblProjectSummaryByWorkType").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });

            $("#tblProjectSearchResult").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        });


        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            $("#tblProjectSummaryByResource").tablesorter(
                {
                    headers: {
                        6: {
                            sorter: false
                        },
                        1: {
                            sorter: false
                        },
                        7: {
                            sorter: false
                        }
                    },
                    sortList: [[0, 0]],
                    sortForce: [[0, 0]]
                });

            $("#tblProjectSummaryByWorkType").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });

            $("#tblProjectSearchResult").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        }

    </script>
    <uc:TimeEntryReportsHeader ID="timeEntryReportHeader" runat="server"></uc:TimeEntryReportsHeader>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="height30P vBottom fontBold">
                        2.&nbsp;Select report parameters:
                    </td>
                </tr>
            </table>
            <table width="100%" style="height: 160px;">
                <tr>
                    <td style="width: 5%;">
                    </td>
                    <td style="text-align: center; height: 30px; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="text-align: right; width: 30%; font-weight: bold;">
                                    Account:&nbsp;
                                </td>
                                <td style="text-align: left;">
                                    <asp:DropDownList ID="ddlAccount" runat="server" Width="160px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 65%">
                    </td>
                </tr>
                <tr>
                    <td style="width: 5%;">
                    </td>
                    <td style="text-align: center; height: 30px; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="text-align: right; width: 30%; font-weight: bold;">
                                    Business Unit:&nbsp;
                                </td>
                                <td style="text-align: left;">
                                    <asp:DropDownList ID="ddlBusinessUnit" runat="server" Width="160px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 65%">
                    </td>
                </tr>
                <tr>
                    <td style="width: 5%;">
                    </td>
                    <td style="height: 30px; text-align: center; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="text-align: right; width: 30%; font-weight: bold;">
                                    Range:&nbsp;
                                </td>
                                <td style="text-align: left;">
                                    <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged"
                                        Width="160px">
                                        <asp:ListItem Selected="True" Text="Please Select" Value="Please Select"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Current" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Previous" Value="-15"></asp:ListItem>
                                        <asp:ListItem Text="This Week" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="This Month" Value="30"></asp:ListItem>
                                        <asp:ListItem Text="This Year" Value="365"></asp:ListItem>
                                        <asp:ListItem Text="Last Week" Value="-7"></asp:ListItem>
                                        <asp:ListItem Text="Last Month" Value="-30"></asp:ListItem>
                                        <asp:ListItem Text="Last Year" Value="-365"></asp:ListItem>
                                        <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 65%">
                    </td>
                </tr>
                <tr>
                    <td style="width: 5%;">
                    </td>
                    <td style="height: 30px; text-align: center; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="width: 30%;">
                                </td>
                                <td style="text-align: left; height: 15px;">
                                    <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                    <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 65%">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 30px;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="border-bottom: 3px solid black; width: 100%; height: 10px;">
                    </td>
                </tr>
            </table>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                DropShadow="false" />
            <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass" Style="padding-top: 20px; display: none;" BorderWidth="2px">
                <table class="WholeWidth">
                    <tr>
                        <td align="center">
                            <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                FromToDateFieldWidth="70" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 10px 0px 10px 0px;">
                            <asp:Button ID="btnCustDatesOK" runat="server" OnClick="btnCustDatesOK_Click" Text="OK"
                                Style="float: none !Important;" CausesValidation="true" />
                            &nbsp; &nbsp;
                            <asp:Button ID="btnCustDatesCancel" CausesValidation="false" runat="server" Text="Cancel"
                                Style="float: none !Important;" OnClick="btnCustDatesCancel_OnClick" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:ValidationSummary ID="valSumDateRange" runat="server" ValidationGroup='<%# ClientID %>' />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <div id="divWholePage" runat="server" style="display: none;">
                <asp:MultiView ID="mvAccountReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwBusinessUnitReport" runat="server">
                        <asp:Panel ID="pnlBusinessUnitReport" runat="server" CssClass="WholeWidth">
                            <%--<uc:ByResource ID="tpByResource" runat="server">
                            </uc:ByResource>--%>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwProjectReport" runat="server">
                        <asp:Panel ID="pnlProjectReport" runat="server" CssClass="WholeWidth">
                            <%--<uc:Byproject ID="tpByProject" runat="server">
                            </uc:Byproject>--%>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwBusinessDevelopmentReport" runat="server">
                        <asp:Panel ID="pnlBusinessDevelopmentReport" runat="server" CssClass="WholeWidth">
                            <%--<uc:Byproject ID="tpByProject" runat="server">
                            </uc:Byproject>--%>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

