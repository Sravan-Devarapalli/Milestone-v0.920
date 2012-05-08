<%@ Page Title="Audit" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Audit.aspx.cs" Inherits="PraticeManagement.Reporting.Audit" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <table class="WholeWidth" style="background-color:#e2ebff;">
        <tr>
            <td style="width: 90%;">
                Display all time entries in
                <asp:DropDownList ID="ddlPeriod" runat="server" Width="160px" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                    <asp:ListItem Selected="True" Text="Please Select" Value="Please Select"></asp:ListItem>
                    <asp:ListItem Text="Payroll – P1" Value="15"></asp:ListItem>
                    <asp:ListItem Text="Payroll – P2" Value="-15"></asp:ListItem>
                    <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                </asp:DropDownList>
                <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
                <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                that were changed afterwards.
            </td>
            <td style="width: 10%;">
                <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_OnClick" Text="Run Report" ToolTip="Run Report"/>
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
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

