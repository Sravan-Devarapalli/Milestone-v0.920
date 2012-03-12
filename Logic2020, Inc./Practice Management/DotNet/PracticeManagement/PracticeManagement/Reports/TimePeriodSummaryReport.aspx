<%@ Page Title="Time Period Summary Report" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="TimePeriodSummaryReport.aspx.cs" Inherits="PraticeManagement.Reporting.TimePeriodSummaryReport" %>
    
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc" TagName="DateInterval" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/TimePeriodSummaryByResource.ascx" TagPrefix="uc" TagName="ByResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">

        function ReAssignStartDateEndDates() {
            var hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            var hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            var hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
            var hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');

            var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
            var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
            if (startDateCalExtender != null) {
                startDateCalExtender.set_selectedDate(hdnStartDate.value);
            }
            if (endDateCalExtender != null) {
                endDateCalExtender.set_selectedDate(hdnEndDate.value);
            }

            var valSummary = document.getElementById('<%= valSumDateRange.ClientID %>');
            valSummary.style.display = 'none';
        }
    </script>
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
            padding: 8px 0px 15px 0px;
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
        <ContentTemplate>Range: 
            <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                <asp:ListItem Selected="True" Text="This Week" Value="7"></asp:ListItem>
                <asp:ListItem Text="This Month" Value="30"></asp:ListItem>
                <asp:ListItem Text="This Year" Value="365"></asp:ListItem>
                <asp:ListItem Text="Last Week" Value="-7"></asp:ListItem>
                <asp:ListItem Text="Last Month" Value="-30"></asp:ListItem>
                <asp:ListItem Text="Last Year" Value="-365"></asp:ListItem>
                <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
            </asp:DropDownList>
            <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
            <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
            <asp:HiddenField ID="hdnStartDateCalExtenderBehaviourId" runat="server" Value="" />
            <asp:HiddenField ID="hdnEndDateCalExtenderBehaviourId" runat="server" Value="" />
            &nbsp;
            <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
            <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
            &nbsp;
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                CancelControlID="btnCustDatesCancel" BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates"
                BehaviorID="bhCustomDates" DropShadow="false" OnCancelScript="ReAssignStartDateEndDates();" />
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
                            <asp:Button ID="btnCustDatesCancel" runat="server" Text="Cancel" Style="float: none !Important;" />
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
            <hr />
            <asp:Table ID="tblTimePeriodReportViewSwitch" runat="server" CssClass="CustomTabStyle">
                <asp:TableRow ID="rowSwitcher" runat="server">
                    <asp:TableCell ID="cellResource" CssClass="SelectedSwitch" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="lnkbtnResource" runat="server" Text="By Resource" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellProject" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="lnkbtnProject" runat="server" Text="By Project" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellWorkType" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="lnkbtnWorkType" runat="server" Text="By Work Type" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:MultiView ID="mvTimePeriodReport" runat="server" ActiveViewIndex="0">
                <asp:View ID="vwResourceReport" runat="server">
                    <asp:Panel ID="pnlResourceReport" runat="server" CssClass="tab-pane WholeWidth">
                        <uc:ByResource Id="tpByResource" runat="server"></uc:ByResource>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwProjectReport" runat="server">
                    <asp:Panel ID="pnlProjectReport" runat="server" CssClass="tab-pane WholeWidth">
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwWorkTypeReport" runat="server">
                    <asp:Panel ID="pnlWorkTypeReport" runat="server" CssClass="tab-pane WholeWidth">
                    </asp:Panel>
                </asp:View>
            </asp:MultiView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

