<%@ Page Title="Person Detail Time Report" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="PersonDetailTimeReport.aspx.cs" Inherits="PraticeManagement.Reporting.PersonDetailTimeReport" %>

<%@ Register Src="~/Controls/Reports/PersonSummaryReport.ascx" TagPrefix="uc" TagName="PersonSummaryReport" %>
<%@ Register Src="~/Controls/Reports/PersonDetailReport.ascx" TagPrefix="uc" TagName="PersonDetailReport" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">

        function CheckAndShowCustomDatesPoup(ddlPeriod) {
            imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            if (ddlPeriod.value == '0') {
                imgCalender.attributes["class"].value = "";
                lblCustomDateRange.attributes["class"].value = "";
                if (imgCalender.fireEvent) {
                    imgCalender.style.display = "";
                    lblCustomDateRange.style.display = "";
                    imgCalender.click();
                }
                if (document.createEvent) {
                    var event = document.createEvent('HTMLEvents');
                    event.initEvent('click', true, true);
                    imgCalender.dispatchEvent(event);
                }
            }
            else {
                imgCalender.attributes["class"].value = "displayNone";
                lblCustomDateRange.attributes["class"].value = "displayNone";
                if (imgCalender.fireEvent) {
                    imgCalender.style.display = "none";
                    lblCustomDateRange.style.display = "none";
                }
            }
        }

        function ReAssignStartDateEndDates() {
            hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
            hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
            hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');

            var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
            var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
            if (startDateCalExtender != null) {
                startDateCalExtender.set_selectedDate(hdnStartDate.value);
            }
            if (endDateCalExtender != null) {
                endDateCalExtender.set_selectedDate(hdnEndDate.value);
            }
            btnCustDatesOK = document.getElementById('<%= btnCustDatesOK.ClientID %>');
            btnCustDatesOK.click();
        }

        function CheckIfDatesValid() {
            hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
            hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
            var startDate = new Date(txtStartDate.value);
            var endDate = new Date(txtEndDate.value);
            if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= endDate) {
                var startYear = parseInt(startDate.format('yyyy'));
                var endYear = parseInt(endDate.format('yyyy'));
                var startMonth = 0;
                var endMonth = 0;
                if (startDate.format('MM')[0] == '0') {
                    startMonth = parseInt(startDate.format('MM')[1]);
                }
                else {
                    startMonth = parseInt(startDate.format('MM'));
                }
                if (endDate.format('MM')[0] == '0') {
                    endMonth = parseInt(endDate.format('MM')[1]);
                }
                else {
                    endMonth = parseInt(endDate.format('MM'));
                }
                if ((startYear == endYear && ((endMonth - startMonth + 1) <= 3))
            || (((((endYear - startYear) * 12 + endMonth) - startMonth + 1)) <= 3)
            || ((endDate - startDate) / (1000 * 60 * 60 * 24)) < 90
            ) {
                    var btnCustDatesClose = document.getElementById('<%= btnCustDatesClose.ClientID %>');
                    hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
                    hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
                    lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
                    var startDate = new Date(txtStartDate.value);
                    var endDate = new Date(txtEndDate.value);
                    var startDateStr = startDate.format("MM/dd/yyyy");
                    var endDateStr = endDate.format("MM/dd/yyyy");
                    hdnStartDate.value = startDateStr;
                    hdnEndDate.value = endDateStr;
                    lblCustomDateRange.innerHTML = '(' + startDateStr + '&nbsp;-&nbsp;' + endDateStr + ')';
                    btnCustDatesClose.click();
                }
            }
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
    <script language="javascript" type="text/javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
            if (imgCalender.fireEvent && ddlPeriod.value != '0') {
                imgCalender.style.display = "none";
                lblCustomDateRange.style.display = "none";
            }
        }

    </script>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <asp:DropDownList ID="ddlPerson" runat="server">
            </asp:DropDownList>
            <asp:DropDownList ID="ddlPeriod" onchange="CheckAndShowCustomDatesPoup(this);" runat="server">
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
            <asp:HiddenField ID="hdnStartDateTxtBoxId" runat="server" Value="" />
            <asp:HiddenField ID="hdnEndDateTxtBoxId" runat="server" Value="" />
            <asp:HiddenField ID="hdnStartDateCalExtenderBehaviourId" runat="server" Value="" />
            <asp:HiddenField ID="hdnEndDateCalExtenderBehaviourId" runat="server" Value="" />
            &nbsp;
            <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
            <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
            &nbsp;
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                CancelControlID="btnCustDatesCancel" OkControlID="btnCustDatesClose" BackgroundCssClass="modalBackground"
                PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates" DropShadow="false"
                OnCancelScript="ReAssignStartDateEndDates();" OnOkScript="return false;" />
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
                            <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="CheckIfDatesValid();"
                                Text="OK" Style="float: none !Important;" CausesValidation="true" />
                            <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true"
                                OnClientClick="return false;" />
                            &nbsp; &nbsp;
                            <asp:Button ID="btnCustDatesCancel" runat="server" Text="Cancel" Style="float: none !Important;" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:ValidationSummary ID="valSum" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Table ID="tblPersonViewSwitch" runat="server" CssClass="CustomTabStyle">
                <asp:TableRow ID="rowSwitcher" runat="server">
                    <asp:TableCell ID="cellSummary"  CssClass="SelectedSwitch" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellDetail" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="lnkbtnDetail" runat="server" Text="Detail" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:MultiView ID="mvPersonDetailReport" runat="server" ActiveViewIndex="0">
                <asp:View ID="vwPersonSummaryReport" runat="server">
                    <asp:Panel ID="pnlPersonSummaryReport" runat="server" CssClass="tab-pane WholeWidth">
                        <uc:PersonSummaryReport ID="personSummaryReport" runat="server" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwPersonDetailReport" runat="server">
                    <asp:Panel ID="pnlPersonDetailReport" runat="server" CssClass="tab-pane WholeWidth">
                        <uc:PersonDetailReport ID="personDetailReport" runat="server" />
                    </asp:Panel>
                </asp:View>
            </asp:MultiView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

