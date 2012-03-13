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

        function ReAssignStartDateEndDates() {
            hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
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

            var valSummary = document.getElementById('<%= valSumDateRange.ClientID %>');
            valSummary.style.display = 'none';
        }

        function CollapseOrExpandAll(btnExpandOrCollapseAllClientId, hdnCollapsedClientId, hdncpeExtendersIds) {
            var btn = btnExpandOrCollapseAllClientId;
            var hdnCollapsed = hdnCollapsedClientId;
            var isExpand = false;
            if (btn != null) {
                if (btn.value == "Expand All") {
                    isExpand = true;
                    btn.value = "Collapse All";
                    btn.title = "Collapse All";
                    hdnCollapsed.value = 'false';
                }
                else {
                    btn.value = "Expand All";
                    btn.title = "Expand All";
                    hdnCollapsed.value = 'true';
                }
                var projectPanelskvPair = jQuery.parseJSON(hdncpeExtendersIds.value);

                ExpandOrCollapsePanels(projectPanelskvPair, isExpand);

            }

            return false;

        }

        function ExpandOrcollapseExtender(cpe, isExpand) {
            if (cpe != null) {
                if (isExpand)
                    cpe._doOpen();
                else
                    cpe._doClose();
            }
        }

        function ExpandOrCollapsePanels(ids, isExpand) {
            for (var i = 0; i < ids.length; i++) {

                var cpe = $find(ids[i].Key);
                var datePanels = jQuery.parseJSON(ids[i].Value);
                ExpandOrcollapseExtender(cpe, isExpand);
                for (var j = 0; j < datePanels.length; j++) {
                    var cpeDate = $find(datePanels[j]);
                    ExpandOrcollapseExtender(cpeDate, isExpand);
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
            width: 98%;
        }
        
        .info-field
        {
            width: 152px;
        }
        .wrapword
        {
            white-space: -moz-pre-wrap !important; /* Mozilla, since 1999 */
            white-space: -pre-wrap; /* Opera 4-6 */
            white-space: -o-pre-wrap; /* Opera 7 */
            white-space: pre-wrap; /* css-3 */
            word-wrap: break-word; /* Internet Explorer 5.5+ */
            word-break: break-all;
            white-space: normal;
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
            <table width="100%">
                <tr>
                    <td style="width: 70%">
                    </td>
                    <td style="padding-bottom: 10px;">
                        <table>
                            <tr>
                                <td style="width: 40%; text-align: right;">
                                    <b>Person:&nbsp; </b>
                                </td>
                                <td style="text-align: left;">
                                    <asp:DropDownList ID="ddlPerson" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPerson_SelectedIndexChanged"
                                        Width="150px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%">
                    </td>
                    <td style="padding-bottom: 10px;">
                        <table>
                            <tr>
                                <td style="text-align: right; width: 40%">
                                    <b>Range:&nbsp;</b>
                                </td>
                                <td style="text-align: left;">
                                    <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged"
                                        Width="150px">
                                        <asp:ListItem Selected="True" Text="This Week" Value="7"></asp:ListItem>
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
                </tr>
                <tr>
                    <td colspan="2" style="border-bottom: 3px solid black; width: 100%;">
                    </td>
                </tr>
            </table>
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
            <table style="width: 100%;">
                <tr>
                    <td style="width: 55%; font-size: 15px; font-weight: bold;">
                        <asp:Label ID="lblPersonname" runat="server"></asp:Label>
                    </td>
                    <td style="width: 10%;">
                        <table>
                            <tr>
                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                    Total Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px; text-align: center;">
                                    <asp:Literal ID="ltrlTotalHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 10%;">
                        <table>
                            <tr>
                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                    Total Value
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px; text-align: center;">
                                    <asp:Literal ID="ltrlTotalValue" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 12%;">
                        <table>
                            <tr>
                                <td>
                                    BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-bottom: 5px;">
                                    <asp:Literal ID="ltrlBillableHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    NON-BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Literal ID="ltrlNonBillableHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: bottom; width: 5%">
                        <table width="100%">
                            <tr id="trBillable" runat="server">
                                <td style="width: 20px; text-align: center; background-color: #7FD13B; border: 1px solid Gray;">
                                    <asp:Literal ID="ltrlBillablePercent" runat="server"></asp:Literal>
                                    %
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: bottom; width: 5%">
                        <table width="100%">
                            <tr id="trNonBillable" runat="server">
                                <td style="width: 20px; text-align: center; background-color: Gray; border: 1px solid Gray;">
                                    <asp:Literal ID="ltrlNonBillablePercent" runat="server"></asp:Literal>
                                    %
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 3%;">
                    </td>
                </tr>
            </table>
            <asp:Table ID="tblPersonViewSwitch" runat="server" CssClass="CustomTabStyle">
                <asp:TableRow ID="rowSwitcher" runat="server">
                    <asp:TableCell ID="cellSummary" CssClass="SelectedSwitch" runat="server">
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
                    <asp:Panel ID="pnlPersonSummaryReport" runat="server" CssClass="tab-pane">
                        <uc:PersonSummaryReport ID="ucpersonSummaryReport" runat="server" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwPersonDetailReport" runat="server">
                    <asp:Panel ID="pnlPersonDetailReport" runat="server" CssClass="tab-pane">
                        <uc:PersonDetailReport ID="ucpersonDetailReport" runat="server" />
                    </asp:Panel>
                </asp:View>
            </asp:MultiView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

