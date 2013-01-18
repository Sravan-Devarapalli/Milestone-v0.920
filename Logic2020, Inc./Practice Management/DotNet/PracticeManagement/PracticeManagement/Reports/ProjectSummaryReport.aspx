<%@ Page Title="By Project" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ProjectSummaryReport.aspx.cs" Inherits="PraticeManagement.Reporting.ProjectSummaryReport" %>
    
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/Reports/TimeEntryReportsHeader.ascx" TagPrefix="uc"
    TagName="TimeEntryReportsHeader" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/ProjectSummaryByResource.ascx" TagPrefix="uc"
    TagName="ByResource" %>
<%@ Register Src="~/Controls/Reports/ByworkType.ascx" TagPrefix="uc" TagName="ByWorkType" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src='<%# Generic.GetClientUrl("~/Scripts/ExpandOrCollapse.min.js", this) %>' type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            SetTooltipsForallDropDowns();
            $("#tblProjectSummaryByResource").tablesorter(
            {
                headers: {
                    1: {
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

        function SetTooltipsForallDropDowns() {
            var optionList = document.getElementsByTagName('option');

            for (var i = 0; i < optionList.length; ++i) {

                optionList[i].title = optionList[i].innerHTML;
            }

        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            SetTooltipsForallDropDowns();

            $("#tblProjectSummaryByResource").tablesorter(
                {
                    headers: {
                        1: {
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


        function txtSearch_onkeypress(e) {
            var keynum;
            if (window.event) // IE8 and earlier
            {
                keynum = e.keyCode;
            }
            else if (e.which) // IE9/Firefox/Chrome/Opera/Safari
            {
                keynum = e.which;
            }
            if (keynum == 13) {
                var btnSearch = document.getElementById('<%= btnProjectSearch.ClientID %>');
                btnSearch.click();
                return false;
            }
            return true;
        }

        function txtSearch_onkeyup(e) {

            var txtProjectSearch = document.getElementById('<%= txtProjectSearch.ClientID %>');
            var btnSearch = document.getElementById('<%= btnProjectSearch.ClientID %>');
            if (txtProjectSearch.value != '') {
                btnSearch.removeAttribute('disabled');
            }
            else {
                btnSearch.setAttribute('disabled', 'disabled');
            }
            return true;
        }

        function ShowPanel(object, displaypnl, position) {
            var obj = $("#" + object);
            var displayPanel = $("#" + displaypnl);
            iptop = obj.offset().top + obj[0].offsetHeight;
            ipleft = obj.offset().left - position;
            displayPanel.offset({ top: iptop, left: ipleft });
            displayPanel.show();
            displayPanel.offset({ top: iptop, left: ipleft });
        }

        function HidePanel(hiddenpnl) {

            var displayPanel = $("#" + hiddenpnl);
            displayPanel.hide();
        }   

    </script>
    <uc:TimeEntryReportsHeader ID="timeEntryReportHeader" runat="server"></uc:TimeEntryReportsHeader>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr>
                    <td class="height30P vBottom fontBold">
                        2.&nbsp;Select report parameters:
                    </td>
                </tr>
            </table>
            <table class="WholeWidth Height160Px">
                <tr>
                    <td id="tdFirst" runat="server" class="Width35Percent">
                        &nbsp;
                    </td>
                    <td class="ReportTdSecond" id="tdSecond" runat="server">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd40">
                                    Project Number:&nbsp;
                                </td>
                                <td class="SecondTd150">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtProjectNumber" AutoPostBack="true" OnTextChanged="txtProjectNumber_OnTextChanged"
                                                    runat="server"></asp:TextBox>
                                                <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtProjectNumber" runat="server"
                                                    TargetControlID="txtProjectNumber" BehaviorID="waterMarkTxtProjectNumber" WatermarkCssClass="watermarkedtext"
                                                    WatermarkText="Ex: P1234767">
                                                </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            </td>
                                            <td>
                                                <asp:Image ID="imgProjectSearch" runat="server" ToolTip="Project Search" ImageUrl="~/Images/search_24.png" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td id="tdThird" runat="server" class="Width35Percent">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd40">
                                    Range:&nbsp;
                                </td>
                                <td class="SecondTd157">
                                    <pmc:CustomDropDown ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Text="Entire Project" Value="*">
                                        </asp:ListItem>
                                        <asp:ListItem Text="Payroll – Current" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Previous" Value="-15"></asp:ListItem>
                                        <asp:ListItem Text="This Week" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="This Month" Value="30"></asp:ListItem>
                                        <asp:ListItem Text="This Year" Value="365"></asp:ListItem>
                                        <asp:ListItem Text="Last Week" Value="-7"></asp:ListItem>
                                        <asp:ListItem Text="Last Month" Value="-30"></asp:ListItem>
                                        <asp:ListItem Text="Last Year" Value="-365"></asp:ListItem>
                                        <asp:ListItem Text="Custom Dates" Value="0">
                                        </asp:ListItem>
                                    </pmc:CustomDropDown>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd40">
                                    &nbsp;
                                </td>
                                <td class="SecondTd157">
                                    <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                    <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd40">
                                    View:&nbsp;
                                </td>
                                <td class="SecondTd157">
                                    <asp:DropDownList ID="ddlView" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlView_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Text="Please Select" Value=""></asp:ListItem>
                                        <asp:ListItem Text="By Resource" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="By WorkType" Value="1"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="Width300Px" colspan="3">
                                    <uc:MessageLabel ID="msgError" runat="server" ErrorColor="Red" InfoColor="Green"
                                        WarningColor="Orange" EnableViewState="false" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" class="ReportBorderBottom">
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
            <AjaxControlToolkit:ModalPopupExtender ID="mpeProjectSearch" runat="server" TargetControlID="imgProjectSearch"
                BackgroundCssClass="modalBackground" PopupControlID="pnlProjectSearch" BehaviorID="mpeProjectSearch"
                DropShadow="false" />
            <asp:Panel ID="pnlProjectSearch" runat="server" CssClass="popUp ProjectSearch" style="display:none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th>
                            Project Search
                            <asp:Button ID="btnclose" runat="server" CssClass="mini-report-closeNew" ToolTip="Close"
                                OnClick="btnclose_OnClick" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="Width100Px textRight">
                                        Account:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlClients" runat="server" CssClass="Width250Px" OnSelectedIndexChanged="ddlClients_OnSelectedIndexChanged"
                                            AutoPostBack="true">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="Width100Px textRight">
                                        Project:
                                    </td>
                                    <td>
                                        <pmc:CustomDropDown ID="ddlProjects" runat="server" Enabled="false" AutoPostBack="true"
                                            CssClass="Width250Px" OnSelectedIndexChanged="ddlProjects_OnSelectedIndexChanged">
                                            <asp:ListItem Text="-- Select a Project --" Value="">
                                            </asp:ListItem>
                                        </pmc:CustomDropDown>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <table class="WholeWidth">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtProjectSearch" onkeypress="return txtSearch_onkeypress(event);"
                                            onkeyup="return txtSearch_onkeyup(event);" CssClass="Width330Px" runat="server"></asp:TextBox>
                                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmeProjectSearch" runat="server"
                                            TargetControlID="txtProjectSearch" WatermarkCssClass="watermarkedtext Width330Px"
                                            WatermarkText="To search for a project, click here to begin typing...">
                                        </AjaxControlToolkit:TextBoxWatermarkExtender>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnProjectSearch" UseSubmitBehavior="false" disabled="disabled" runat="server"
                                            Text="Search" ToolTip="Search" OnClick="btnProjectSearch_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <div class="ProjectSearchResultsDiv">
                                <asp:Repeater ID="repProjectNamesList" runat="server">
                                    <HeaderTemplate>
                                        <table id="tblProjectSearchResult" class="tablesorter CompPerfTable ProjectSearchResultsTable">
                                            <thead>
                                                <tr class="CompPerfHeader">
                                                    <th class="Width20Percent">
                                                        <div class="ie-bg">
                                                            Project
                                                        </div>
                                                    </th>
                                                    <th class="Width50Percent">
                                                        <div class="ie-bg">
                                                            Project Name
                                                        </div>
                                                    </th>
                                                    <th>
                                                        <div class="ie-bg">
                                                            Account
                                                        </div>
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="textCenter">
                                                <asp:LinkButton ID="lnkProjectNumber" ProjectNumber='<%# Eval("ProjectNumber")%>'
                                                    OnClick="lnkProjectNumber_OnClick" runat="server"><%# Eval("ProjectNumber")%></asp:LinkButton>
                                            </td>
                                            <td>
                                                <%# Eval("HtmlEncodedName")%>
                                            </td>
                                            <td>
                                                <%# Eval("Client.HtmlEncodedName")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </tbody></table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="padLeft8">
                            <asp:Literal ID="ltrlNoProjectsText" Visible="false" runat="server" Text="No Projects found."></asp:Literal>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div id="divWholePage" runat="server">
                <asp:MultiView ID="mvProjectSummaryReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwResourceReport" runat="server">
                        <asp:Panel ID="pnlResourceReport" runat="server" CssClass="WholeWidth PaddingTop10">
                            <uc:ByResource ID="ucByResource" runat="server"></uc:ByResource>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwProjectReport" runat="server">
                        <asp:Panel ID="pnlProjectReport" runat="server" CssClass="WholeWidth">
                            <uc:ByWorkType ID="ucByWorktype" runat="server"></uc:ByWorkType>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
                <table class="WholeWidth">
                    <tr>
                        <td class="textCenter PaddingTop5">
                            <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" Text="Return To Pervious Report" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ucByResource$ucProjectSummaryReport$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="ucByResource$ucProjectDetailReport$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="ucByWorktype$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="ucByResource$ucProjectSummaryReport$btnExportToPDF" />
            <asp:PostBackTrigger ControlID="ucByResource$ucProjectDetailReport$btnExportToPDF" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

