<%@ Page Title="Project Feedback" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ProjectFeedbackReport.aspx.cs" Inherits="PraticeManagement.Reports.ProjectFeedbackReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/Reports/ProjectFeedbackSummaryReport.ascx" TagPrefix="uc"
    TagName="FeedbackSummary" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblAccountSummaryByBusinessReport").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });

            $("#tblAccountSummaryByProject").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            $("#tblAccountSummaryByBusinessReport").tablesorter(
                {
                    sortList: [[0, 0]],
                    sortForce: [[0, 0]]
                });

            $("#tblAccountSummaryByProject").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table>
                <tr>
                    <td class="ReportFilterLabels">
                        Show Report For: &nbsp;
                    </td>
                    <td class="textLeft">
                        <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" CssClass="Width220Px"
                            OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                            <asp:ListItem Text="Please Select" Value="Please Select"></asp:ListItem>
                            <asp:ListItem Text="This Month" Value="30"></asp:ListItem>
                            <asp:ListItem Text="Last Month" Value="-30"></asp:ListItem>
                            <asp:ListItem Text="Q1" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Q2" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Q3" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Q4" Value="4"></asp:ListItem>
                            <asp:ListItem Selected="True" Text="Year To Date" Value="-1"></asp:ListItem>
                            <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                        <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                        <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                        <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr class="height30P">
                    <td class="vBottom fontBold Width3Percent no-wrap">
                        &nbsp;Select report parameters:&nbsp;
                    </td>
                    <td>
                    </td>
                    <td class="width60P">
                    </td>
                </tr>
                <tr class="height30P WholeWidth">
                    <td class="ReportFilterLabels">
                        Account:&nbsp;
                    </td>
                    <td class="textLeft Width10Per">
                        <pmc:ScrollingDropDown ID="cblAccount" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            NoItemsType="All" onclick="scrollingDropdown_onclick('cblAccount','Account')"
                            OnSelectedIndexChanged="cblAccount_SelectedIndexChanged" AutoPostBack="true"
                            DropDownListType="Account" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv Width232PxImp" />
                        <ext:ScrollableDropdownExtender ID="sdeAccount" runat="server" TargetControlID="cblAccount"
                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                        <span class="ReportFilterLabels padLeft15">Business Group:</span> &nbsp;
                        <pmc:ScrollingDropDown ID="cblBusinessGroup" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            NoItemsType="All" onclick="scrollingDropdown_onclick('cblBusinessGroup','Business Group')"
                            OnSelectedIndexChanged="cblBusinessGroup_SelectedIndexChanged" AutoPostBack="true"
                            DropDownListType="Business Group" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv Width232PxImp" />
                        <ext:ScrollableDropdownExtender ID="sdeProjectGroup" runat="server" TargetControlID="cblBusinessGroup"
                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Executive in Charge:&nbsp;
                    </td>
                    <td class="textLeft">
                        <pmc:ScrollingDropDown ID="cblDirector" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            NoItemsType="All" DropdownListFirst="Executive" DropdownListSecond="in Charge" OnSelectedIndexChanged="cblDirector_SelectedIndexChanged" AutoPostBack="true" onclick="scrolling_onclick('cblDirector','Executive in Charge','s','Executives in Charge',33,'Executive','in Charge')" DropDownListTypePluralForm="Executives in Charge"
                            DropDownListType="Executive in Charge" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv Width232PxImp" />
                        <ext:ScrollableDropdownExtender ID="sdeDirector" runat="server" TargetControlID="cblDirector"
                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Practices:&nbsp;
                    </td>
                    <td class="textLeft">
                        <pmc:ScrollingDropDown ID="cblPractices" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            NoItemsType="All" onclick="scrollingDropdown_onclick('cblPractices','Practice')"
                            OnSelectedIndexChanged="cblPractice_SelectedIndexChanged" AutoPostBack="true"
                            DropDownListType="Practice" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv Width232PxImp" />
                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                    </td>
                    <td class="textLeft">
                        <asp:CheckBox ID="chbExcludeInternal" runat="server" Text="Exclude Internal Practice Areas"
                            AutoPostBack="true" OnCheckedChanged="chbExcludeInternal_CheckedChanged" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td colspan="2">
                        &nbsp;
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td colspan="2">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="ReportBorderBottomByAccount">
                    <td colspan="3">
                    </td>
                </tr>
            </table>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                DropShadow="false" />
            <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass CustomDatesPopUp" Style="display: none;">
                <table class="WholeWidth">
                    <tr>
                        <td align="center">
                            <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                ValidationGroup="valCustom" FromToDateFieldCssClass="Width70Px" />
                        </td>
                        <td>
                            <asp:CustomValidator ID="cstvalPeriodRange" runat="server" OnServerValidate="cstvalPeriodRange_ServerValidate"
                                ValidationGroup="valCustom" Text="*" EnableClientScript="true" ToolTip="The time period selected cannot be greater than 1 year."
                                ErrorMessage="The time period selected cannot be greater than 1 year." Display="Dynamic"></asp:CustomValidator>
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
                            <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="valCustom" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <div id="divWholePage" runat="server">
                <table class="PaddingTenPx AccountSummaryReportHeader">
                    <tr>
                        <td style="width: 80%">
                        </td>
                        <td style="width: 20%">
                            <table class="ReportHeaderTotals">
                                <tr class="vBottomImp">
                                    <td class="Width24Percent">
                                        <table class="font14Px">
                                            <tr>
                                                <td class="alignRight">
                                                    Completed:
                                                </td>
                                                <td>
                                                    <asp:Literal ID="ltrlCompleted" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="alignRight">
                                                    Not Completed:
                                                </td>
                                                <td>
                                                    <asp:Literal ID="ltrlNotCompleted" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="alignRight">
                                                    Canceled:
                                                </td>
                                                <td>
                                                    <asp:Literal ID="ltrlCanceled" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="ReportHeaderBandNBGraph">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlCompletedCount" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trCompleted" runat="server" title="Completed Feedback Count.">
                                                            <td class="CompledStatusGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="ReportHeaderBandNBGraph">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlNotCompletedCount" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trNotCompleted" runat="server" title="Not Completed Feeedback Count.">
                                                            <td class="NotCompletedStatusGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="ReportHeaderBandNBGraph">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlCanceledCount" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trCancel" runat="server" title="Canceled Feedback Count.">
                                                            <td class="nonBillingGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width2Percent">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblProjectViewSwitch" runat="server" CssClass="CommonCustomTabStyle AccountSummaryReportCustomTabStyle">
                    <asp:TableRow ID="rowSwitcher" runat="server">
                        <asp:TableCell ID="cellReport" CssClass="SelectedSwitch" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="0" ToolTip="Summary"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:MultiView ID="mvProjectFeedbackReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwFeedbackReport" runat="server">
                        <asp:Panel ID="pnlFeedbackReport" runat="server" CssClass="WholeWidth">
                            <uc:FeedbackSummary ID="feedbackSummary" runat="server" />
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="feedbackSummary$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

