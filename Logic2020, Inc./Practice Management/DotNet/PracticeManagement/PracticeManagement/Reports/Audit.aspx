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
            <table class="WholeWidth bgcolorE2EBFF">
                <tr>
                    <td class="AuditHeaderTd1">
                        <table>
                            <tr>
                                <td>
                                    Display all time entries from&nbsp;
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlPeriod" runat="server" CssClass="Width160px" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Text="Please Select" Value="Please Select"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Previous" Value="-15"></asp:ListItem>
                                        <asp:ListItem Text="Previous Month" Value="-1"></asp:ListItem>
                                        <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="hdnperiodValue" runat="server" Value="Please Select" />
                                    &nbsp; that were changed afterwards.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td class="PaddingTop5 height20P">
                                    <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                    <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="AuditHeaderTd2">
                        <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_OnClick" Text="Run Report"
                            CssClass="Width150px" Enabled="false" ToolTip="Run Report" />
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr>
                    <td colspan="2" class="ReportBorderBottomHeight15Px">
                    </td>
                </tr>
            </table>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                DropShadow="false" />
            <asp:Panel ID="pnlCustomDates" runat="server" CssClass="ConfirmBoxClass CustomDatesPopUp" style="display:none;">
                <table class="WholeWidth">
                    <tr>
                        <td align="center">
                            <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                FromToDateFieldCssClass="Width70Px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="custBtns">
                            <asp:Button ID="btnCustDatesOK" runat="server" OnClick="btnCustDatesOK_Click" Text="OK"
                                CausesValidation="true" />
                            &nbsp; &nbsp;
                            <asp:Button ID="btnCustDatesCancel" CausesValidation="false" runat="server" Text="Cancel"
                                OnClick="btnCustDatesCancel_OnClick" />
                        </td>
                    </tr>
                    <tr>
                        <td class="textCenter">
                            <asp:ValidationSummary ID="valSumDateRange" runat="server" ValidationGroup='<%# ClientID %>' />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div id="divWholePage" runat="server" style="display: none;" class="bgcolorE2EBFF">
                <table class="PaddingTenPx TimePeriodSummaryReportHeader">
                    <tr>
                        <td class="font16Px fontBold">
                            <table>
                                <tr>
                                    <td class="vtop PaddingBottom10Imp">
                                        <asp:Literal ID="ltrCount" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PaddingTop10Imp vBottom">
                                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="AuditReportTotals">
                            <table class="WholeWidth">
                                <tr>
                                    <td>
                                        BILLABLE
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PaddingBottom5Imp">
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
                        <td class="AuditHeaderTd3">
                            <table class="WholeWidth">
                                <tr>
                                    <td class='FontSize15PX PaddingBottom3Imp'>
                                        Net Change
                                    </td>
                                </tr>
                                <tr>
                                    <td class="font25Px">
                                        <asp:Literal ID="ltrlNetChange" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="Width2Percent">
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

