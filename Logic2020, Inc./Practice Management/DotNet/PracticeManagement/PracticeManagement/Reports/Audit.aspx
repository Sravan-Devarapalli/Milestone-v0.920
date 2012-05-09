<%@ Page Title="Audit" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Audit.aspx.cs" Inherits="PraticeManagement.Reporting.Audit" %>

<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/AuditByPerson.ascx" TagPrefix="uc" TagName="ByResource" %>
<%@ Register Src="~/Controls/Reports/AuditByProject.ascx" TagPrefix="uc" TagName="Byproject" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth" style="background-color: #e2ebff;">
                <tr>
                    <td style="width: 90%; font-size: 13px; padding: 5px; font-weight: bold;">
                        <table>
                            <tr>
                                <td>
                                    Display all time entries in&nbsp;
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlPeriod" runat="server" Width="160px" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Text="Please Select" Value="Please Select"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Current" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Pervious" Value="-15"></asp:ListItem>
                                        <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="hdnperiodValue" runat="server" Value="Please Select"/>
                                    &nbsp;
                                    that were changed afterwards.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td style="padding-top: 5px;height:20px;">
                                    <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                    <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 10%;text-align:right;padding:5px;vertical-align:top;">
                        <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_OnClick" Text="Run Report" Width="150px" Enabled="false"
                            ToolTip="Run Report" />
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr>
                    <td colspan="2" style="border-bottom: 3px solid black; width: 100%; height: 15px;">
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
            <div id="divWholePage" runat="server" style="display: none; background-color: #e2ebff;">
                <table class="PaddingTenPx" style="width: 100%; background-color: White; padding-bottom: 5px !important;
                    height: 90px;">
                    <tr>
                        <td style="font-size: 16px; font-weight: bold;">
                            <table>
                                <tr>
                                    <td style="vertical-align: top; padding-bottom: 10px;">
                                        <asp:Literal ID="ltrCount" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 10px; vertical-align: bottom;">
                                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 10%; vertical-align: middle; text-align: center;">
                            <table width="100%">
                                <tr>
                                    <td>
                                        BILLABLE
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-bottom: 5px;">
                                        <asp:Literal ID="ltrlBillableNetChange" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        NON-BILLABLE
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Literal ID="ltrlNonBillableNetChange" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 10%; text-align: center;vertical-align: middle;">
                            <table width="100%">
                                <tr>
                                    <td style="font-size: 15px; padding-bottom: 3px;">
                                        Net Change
                                    </td>
                                </tr>
                                <tr>
                                    <td style="font-size: 25px;">
                                        <asp:Literal ID="ltrlNetChange" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 2%;">
                        </td>
                    </tr>
                </table>
                <asp:MultiView ID="mvTimePeriodReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwResourceReport" runat="server">
                        <asp:Panel ID="pnlResourceReport" runat="server" CssClass="WholeWidth">
                            <uc:ByResource ID="tpByResource" runat="server"></uc:ByResource>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwProjectReport" runat="server">
                        <asp:Panel ID="pnlProjectReport" runat="server" CssClass="WholeWidth">
                            <uc:Byproject ID="tpByProject" runat="server"></uc:Byproject>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="tpByResource$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByProject$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

