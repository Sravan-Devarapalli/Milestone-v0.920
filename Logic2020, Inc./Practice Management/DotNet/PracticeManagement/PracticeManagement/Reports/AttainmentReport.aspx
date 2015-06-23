<%@ Page Title="Attainment Report" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="AttainmentReport.aspx.cs" Inherits="PraticeManagement.Reports.AttainmentReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
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
    <uc:LoadingProgress ID="PleaseWaitImage" runat="server" />
    <asp:UpdatePanel ID="flrPanel" runat="server">
        <ContentTemplate>
            <div class="filters">
                <div class="buttons-block">
                    <table class="WholeWidth BorderNone LeftPadding10px">
                        <tr>
                            <td class="Width3Percent">
                            </td>
                            <td class="Width8Percent">
                                &nbsp;Show&nbsp;Projects&nbsp;for&nbsp;
                            </td>
                            <td class="Width9Percent">
                                <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" CssClass="WholeWidth"
                                    OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                                    <asp:ListItem Text="Previous FY" Value="-13"></asp:ListItem>
                                    <asp:ListItem Text="Current FY" Selected="True" Value="13"></asp:ListItem>
                                </asp:DropDownList>
                                <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                                    BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                                    DropShadow="false" />
                                <asp:HiddenField ID="hdnPeriod" runat="server" Value="3" />
                            </td>
                            <td class="Width47Percent padLeft5">
                                <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                                <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                            </td>
                            <td>
                                <asp:Button ID="btnExport" runat="server" Text="Export" OnClick="btnExport_Click" />
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
                        CssClass="ConfirmBoxClass CustomDatesPopUp" Style="display: none;">
                        <table class="WholeWidth">
                            <tr>
                                <td align="center">
                                    <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                        FromToDateFieldCssClass="Width60PxImp" />
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
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExport" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

