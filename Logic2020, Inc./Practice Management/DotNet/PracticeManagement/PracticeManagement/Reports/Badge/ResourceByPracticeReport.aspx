<%@ Page Title="Available Resources by Practice" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ResourceByPracticeReport.aspx.cs" Inherits="PraticeManagement.Reports.Badge.ResourceByPracticeReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/BadgeResourcesByPracticeFilter.ascx" TagName="Filter"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function ClosePopUp() {
            var popup = $find('mpeBadgePanelBehaviourId');
            popup.hide();
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
                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                            ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Expand Filters and Sort Options" />
                        &nbsp; Projected Range:&nbsp;
                    </td>
                    <td class="textLeft">
                        <uc:DatePicker ID="dtpStart" runat="server" AutoPostBack="true" OnSelectionChanged="dtpStart_SelectionChanged" ValidationGroup="Badge"/>
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
                        <uc:DatePicker ID="dtpEnd" runat="server" AutoPostBack="true" OnSelectionChanged="dtpStart_SelectionChanged" ValidationGroup="Badge"/>
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
                    <td>
                        &nbsp;by&nbsp;
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlView" runat="server">
                            <asp:ListItem Text="1 Day" Value="1"></asp:ListItem>
                            <asp:ListItem Text="1 Week" Value="7"></asp:ListItem>
                            <asp:ListItem Text="1 Month" Value="30"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btnUpdateView" runat="server" Text="View Report" OnClick="btnUpdateView_Click" />
                    </td>
                </tr>
                <tr class="height1Px">
                    <td colspan="5">
                        &nbsp;
                    </td>
                    <td class="textLeft Width90Percent">
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlFilters" runat="server" CssClass="bgColor_d4dff8" Style="padding: 10px;
                padding-left: 50px;">
                <uc:Filter ID="filter" runat="server" ddlType="1" />
            </asp:Panel>
            <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="BadgeReport" ShowMessageBox="false"
                ShowSummary="true" EnableClientScript="false" />
            <hr />
            <div id="divWholePage" runat="server" style="display: none">
                <table class="WholeWidth">
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
                </table>
                <asp:Chart ID="chartReport" runat="server" EnableViewState="true">
                    <Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="MainArea">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
                <asp:Repeater ID="repReportTable" runat="server" OnItemDataBound="repReportTable_ItemDataBound">
                    <HeaderTemplate>
                        <div class="minheight250Px WholeWidth" style="overflow-x: scroll;">
                            <table class="tablesorter PersonSummaryReport zebra">
                                <thead>
                                    <tr>
                                        <th style="padding-left: 60px; padding-right: 60px">
                                            Practice Area
                                        </th>
                                        <asp:Repeater ID="repDatesHeaders" runat="server">
                                            <ItemTemplate>
                                                <th class="DayTotalHoursBorderLeft" style="padding-left: 8px; padding-right: 8px;">
                                                    <%# Eval("date")%>
                                                </th>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tr>
                                </thead>
                                <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate">
                            <td>
                                <asp:Label ID="lblPractice" runat="server" Text='<%# Eval("Practice.HtmlEncodedName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repCount" runat="server" OnItemDataBound="repCount_ItemDataBound">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderLeft">
                                        <asp:Label ID="lblCount" runat="server"></asp:Label>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alterrow">
                            <td>
                                <asp:Label ID="lblPractice" runat="server" Text='<%# Eval("Practice.HtmlEncodedName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repCount" runat="server" OnItemDataBound="repCount_ItemDataBound">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderLeft">
                                        <asp:Label ID="lblCount" runat="server"></asp:Label>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </tbody> </table> </div>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:HiddenField ID="hdnTargetPanel" Value="false" runat="server" />
                <AjaxControlToolkit:ModalPopupExtender ID="mpeBadgePanel" runat="server" BehaviorID="mpeBadgePanelBehaviourId"
                    TargetControlID="hdnTargetPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlValidationPanel"
                    CancelControlID="btnCancel" DropShadow="false" />
                <asp:Panel ID="pnlValidationPanel" runat="server" CssClass="popUp ValidationPopUp"
                    Style="display: none;">
                    <table class="WholeWidth">
                        <tr class="PopUpHeader">
                            <th colspan="2">
                                Attention!
                            </th>
                        </tr>
                        <tr>
                            <td class="Padding10" colspan="2">
                                <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Padding10 textCenter">
                                <asp:Button ID="btnOKValidationPanel" runat="server" ToolTip="OK" Text="OK" CssClass="Width100Px"
                                    OnClientClick="ClosePopUp()" OnClick="btnOKValidationPanel_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel" CssClass="Width100Px" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
            <div id="divEmptyMessage" class="MSBadgeEmptyDiv font14PxImp" style="display: none;" runat="server">
                There are no available resources for the practices selected.
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpdateView" />
            <asp:PostBackTrigger ControlID="btnOKValidationPanel" />
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

