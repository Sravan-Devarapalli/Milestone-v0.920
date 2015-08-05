<%@ Page Title="Blocked Resources" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="BadgeBlockedReport.aspx.cs" Inherits="PraticeManagement.Reports.Badge.BadgeBlockedReport" %>

<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../../Scripts/jquery.tablesorter.yui.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblBlocked").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            $("#tblBlocked").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr class="height30P">
                    <td style="padding-bottom: 10px;" class="vBottom fontBold Width3Percent no-wrap">
                        &nbsp;Select report parameters:&nbsp;
                    </td>
                    <td colspan="5">
                    </td>
                    <td class="width60P">
                    </td>
                </tr>
                <tr style="white-space: nowrap">
                    <td class="ReportFilterLabels">
                        Projected Range:&nbsp;
                    </td>
                    <td class="textLeft">
                        <uc:DatePicker ID="dtpStart" runat="server" ValidationGroup="Badge" />
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqBadgeStart" runat="server" ControlToValidate="dtpStart"
                            ValidationGroup="BadgeReport" ErrorMessage="Start date is required." ToolTip="Start date is required."
                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvLastBadgeStart" runat="server" ControlToValidate="dtpStart"
                            ValidationGroup="BadgeReport" Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter projected range start date in the correct format: MM/DD/YYYY."
                            Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter projected range start date in the correct format: MM/DD/YYYY."
                            Text="*" Type="Date" EnableClientScript="false">*</asp:CompareValidator>
                    </td>
                    <td>
                        &nbsp;to&nbsp;
                    </td>
                    <td>
                        <uc:DatePicker ID="dtpEnd" runat="server" ValidationGroup="Badge" />
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqbadgeEnd" runat="server" ControlToValidate="dtpEnd"
                            ValidationGroup="BadgeReport" ErrorMessage="End date is required." ToolTip="End date is required."
                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvbadgeEnd" runat="server" ControlToValidate="dtpEnd" Display="Dynamic"
                            ValidationGroup="BadgeReport" EnableTheming="True" ErrorMessage="Please enter projected range end date in the correct format: MM/DD/YYYY."
                            Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter projected range end date in the correct format: MM/DD/YYYY."
                            Type="Date" EnableClientScript="false">*</asp:CompareValidator>
                        <asp:CompareValidator ID="cvBadgeRange" runat="server" ControlToValidate="dtpEnd"
                            ControlToCompare="dtpStart" Operator="GreaterThanEqual" Type="Date" ValidationGroup="BadgeReport"
                            ErrorMessage="End date should be greater than or equal to Start date." Display="Dynamic"
                            Text="*" ToolTip="End date should be greater than or equal to Start date." SetFocusOnError="true"
                            EnableClientScript="false"></asp:CompareValidator>
                        <asp:CustomValidator ID="custNotMorethan2Years" runat="server" ErrorMessage="The report can't be run for the period more than 2 years."
                            ToolTip="The report can't be run for the period more than 2 years." Display="Dynamic"
                            ValidationGroup="BadgeReport" Text="*" EnableClientScript="false" OnServerValidate="custNotMorethan2Years_ServerValidate"
                            SetFocusOnError="true"></asp:CustomValidator>
                        <asp:CustomValidator ID="custNotBeforeJuly" runat="server" ErrorMessage="The report can't be run for dates before 7/1/2014."
                            ToolTip="The report can't be run for dates before 7/1/2014." Display="Dynamic"
                            ValidationGroup="BadgeReport" Text="*" EnableClientScript="false" OnServerValidate="custNotBeforeJuly_ServerValidate"
                            SetFocusOnError="true"></asp:CustomValidator>
                    </td>
                    <td colspan="2">
                        &nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnUpdateView" runat="server" Text="View Report" OnClick="btnUpdateView_Click" />
                    </td>
                </tr>
                <tr style="white-space: nowrap">
                    <td class="ReportFilterLabels">
                        Pay Type:&nbsp;
                    </td>
                    <td colspan="4" style="padding-top: 5px;">
                        <pmc:ScrollingDropDown ID="cblPayTypes" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            onclick="scrollingDropdown_onclick('cblPayTypes','Pay Type')" NoItemsType="All"
                            DropDownListType="Pay Type" CellPadding="3" CssClass="AllEmpClockCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdePayTypes" runat="server" TargetControlID="cblPayTypes"
                            UseAdvanceFeature="true" Width="245px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td class="textLeft Width90Percent">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr style="white-space: nowrap">
                    <td class="ReportFilterLabels">
                        Person Status:&nbsp;
                    </td>
                    <td colspan="4" style="padding-top: 5px;">
                        <pmc:ScrollingDropDown ID="cblPersonStatus" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            onclick="scrollingDropdown_onclick('cblPersonStatus','Person Status','es')" NoItemsType="All"
                            PluralForm="es" DropDownListType="Person Status" CellPadding="3" CssClass="AllEmpClockCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdePersonStatus" runat="server" TargetControlID="cblPersonStatus"
                            UseAdvanceFeature="true" Width="245px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td class="textLeft Width90Percent">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td colspan="5">
                        &nbsp;
                        <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="BadgeReport" ShowMessageBox="false"
                            ShowSummary="true" EnableClientScript="false" />
                    </td>
                    <td class="textLeft Width90Percent">
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <hr />
            <div id="divWholePage" runat="server" style="display: none">
                <table id="tblRange" runat="server" class="WholeWidth">
                    <tr>
                        <td style="font-weight: bold; font-size: 16px;">
                            Projected Range:
                            <asp:Label ID="lblRange" runat="server"></asp:Label>
                        </td>
                        <td style="text-align: right; padding-right: 30px;">
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Export" OnClick="btnExportToExcel_OnClick"
                                Enabled="true" UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                    </tr>
                    <tr>
                        <td class="PaddingTop10Px">
                            <asp:Label ID="lblTitle" runat="server" Text="Blocked Resources:" Style="font-weight: bold;
                                font-size: 20px;"></asp:Label>
                        </td>
                        <td>
                            &nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
                <asp:Repeater ID="repblocked" runat="server" OnItemDataBound="repblocked_ItemDataBound">
                    <HeaderTemplate>
                        <div class="minheight250Px" style="padding-top: 20px;">
                            <table id="tblBlocked" class="tablesorter PersonSummaryReport zebra">
                                <thead>
                                    <tr>
                                        <th class="TextAlignLeftImp Padding5Imp Width300Px">
                                            List of Resources Blocked from MS Badge
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Resource Level
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Block Start
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Block End
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate">
                            <td class="padLeft5 textLeft">
                                <%# Eval("Person.HtmlEncodedName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alterrow">
                            <td class="padLeft5 textLeft">
                                <%# Eval("Person.HtmlEncodedName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </tbody></table></div>
                    </FooterTemplate>
                </asp:Repeater>
                <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
                    There are no blocked resources for the selected dates.
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

