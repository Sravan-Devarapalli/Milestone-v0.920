<%@ Page Title="Badged Resources by Time" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="BadgeResourceByTime.aspx.cs" Inherits="PraticeManagement.Reports.Badge.BadgeResourceByTime" %>

<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
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
                        Projected Range:&nbsp;
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
                            onclick="scrollingDropdown_onclick('cblPersonStatus','Person Status','es')" NoItemsType="All" PluralForm="es"
                            DropDownListType="Person Status" CellPadding="3" CssClass="AllEmpClockCblTimeScales" />
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
                        <asp:Series Name="chartSeries1" ChartArea="MainArea" ChartType="Line" XValueType="String"
                            BorderWidth="2" LegendText="Resources with Active Clocks, Not on Project" IsVisibleInLegend="false"
                            Color="Blue" XAxisType="Primary" YAxisType="Primary" YValueType="Int32" XValueMember="month"
                            YValueMembers="badgedNotOnProjectcount" ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries11" ChartArea="MainArea" ChartType="Line" XValueType="String"
                            BorderWidth="2" LegendText="Resources with Person-Based Exceptions" IsVisibleInLegend="false"
                            Color="Brown" XAxisType="Primary" YAxisType="Primary" YValueType="Int32" XValueMember="month"
                            YValueMembers="badgedNotOnProjectExceptioncount" ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries2" ChartArea="MainArea" ChartType="Line" XValueType="String"
                            Color="Gray" BorderWidth="2" LegendText="18 Month Clock Not Started" IsVisibleInLegend="false"
                            XAxisType="Primary" YAxisType="Primary" YValueType="Int32" XValueMember="month"
                            YValueMembers="clockNotStartedCount" ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries4" ChartArea="MainArea" ChartType="Line" XValueType="String"
                            Color="DarkBlue" BorderWidth="2" LegendText="On 6-Month Break" IsVisibleInLegend="false"
                            XAxisType="Primary" YAxisType="Primary" YValueType="Int32" XValueMember="month"
                            YValueMembers="breakCount" ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries" ChartArea="MainArea" ChartType="Line" XValueType="String"
                            Color="Red" BorderWidth="2" LegendText="Badged on project" IsVisibleInLegend="false"
                            XAxisType="Primary" YAxisType="Primary" YValueType="Int32" ToolTip="#VALY Resources"
                            XValueMember="month" YValueMembers="badgedOnProjectcount">
                        </asp:Series>
                        <asp:Series Name="chartSeries12" ChartArea="MainArea" ChartType="Line" XValueType="String"
                            Color="Pink" BorderWidth="2" LegendText="Badged on project: Project-Based Exceptions"
                            IsVisibleInLegend="false" XAxisType="Primary" YAxisType="Primary" YValueType="Int32"
                            ToolTip="#VALY Resources" XValueMember="month" YValueMembers="badgedProjectExceptioncount">
                        </asp:Series>
                        <asp:Series Name="chartSeries3" ChartArea="MainArea" ChartType="Line" XValueType="String"
                            Color="Orange" BorderWidth="2" LegendText="Blocked" IsVisibleInLegend="false"
                            XAxisType="Primary" YAxisType="Primary" YValueType="Int32" XValueMember="month"
                            YValueMembers="blockedCount" ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries5" ChartArea="MainArea" ChartType="Point" XValueType="String"
                            Color="Red" BorderWidth="2" XAxisType="Primary" IsVisibleInLegend="false" YAxisType="Primary"
                            YValueType="Int32" ToolTip="#VALY Resources" XValueMember="month" YValueMembers="badgedOnProjectcount">
                        </asp:Series>
                        <asp:Series Name="chartSeries6" ChartArea="MainArea" ChartType="Point" XValueType="String"
                            BorderWidth="2" Color="Blue" XAxisType="Primary" IsVisibleInLegend="false" YAxisType="Primary"
                            YValueType="Int32" XValueMember="month" YValueMembers="badgedNotOnProjectcount"
                            ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries7" ChartArea="MainArea" ChartType="Point" XValueType="String"
                            Color="Gray" BorderWidth="2" XAxisType="Primary" IsVisibleInLegend="false" YAxisType="Primary"
                            YValueType="Int32" XValueMember="month" YValueMembers="clockNotStartedCount"
                            ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries8" ChartArea="MainArea" ChartType="Point" XValueType="String"
                            Color="Orange" BorderWidth="2" XAxisType="Primary" IsVisibleInLegend="false"
                            YAxisType="Primary" YValueType="Int32" XValueMember="month" YValueMembers="blockedCount"
                            ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries9" ChartArea="MainArea" ChartType="Point" XValueType="String"
                            Color="DarkBlue" BorderWidth="2" XAxisType="Primary" YAxisType="Primary" YValueType="Int32"
                            XValueMember="month" YValueMembers="breakCount" IsVisibleInLegend="false" ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries13" ChartArea="MainArea" ChartType="Point" XValueType="String"
                            Color="Brown" BorderWidth="2" XAxisType="Primary" YAxisType="Primary" YValueType="Int32"
                            XValueMember="month" YValueMembers="badgedNotOnProjectExceptioncount" IsVisibleInLegend="false"
                            ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries14" ChartArea="MainArea" ChartType="Point" XValueType="String"
                            Color="Pink" BorderWidth="2" XAxisType="Primary" YAxisType="Primary" YValueType="Int32"
                            XValueMember="month" YValueMembers="badgedProjectExceptioncount" IsVisibleInLegend="false"
                            ToolTip="#VALY Resources">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="MainArea">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
                <asp:Repeater ID="repReportTable" runat="server" OnItemDataBound="repReportTable_ItemDataBound">
                    <HeaderTemplate>
                        <div class="minheight250Px" style="overflow-x: auto;">
                            <table class="tablesorter PersonSummaryReport zebra">
                                <thead>
                                    <tr>
                                        <th style="width:1px; padding-left: 200px;">
                                            &nbsp;
                                        </th>
                                        <asp:Repeater ID="repDatesHeaders" runat="server">
                                            <ItemTemplate>
                                                <th class="DayTotalHoursBorderLeft" style="padding-left:4px;padding-right:4px; width: 100px; white-space:nowrap;">
                                                    <%# Eval("date")%>
                                                </th>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <th class="DayTotalHoursBorderLeft" style="padding-left: 8px; padding-right: 8px;">
                                                    TOTALS
                                                </th>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </tr>
                                </thead>
                                <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate">
                            <td>
                                <asp:Label ID="lblCategory" runat="server"></asp:Label>
                            </td>
                            <asp:Repeater ID="repCount" runat="server" OnItemDataBound="repCount_ItemDataBound">
                            <HeaderTemplate></HeaderTemplate>
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderLeft">
                                        <asp:HyperLink ID="hlCount" runat="server" Text='<%# Eval("BadgedOnProjectCount")%>'
                                            Target="_blank"></asp:HyperLink>
                                        <asp:Label ID="lblCount" runat="server" Text='<%# Eval("BadgedOnProjectCount")%>'></asp:Label>
                                    </td>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <td class="DayTotalHoursBorderLeft">
                                        <asp:Label ID="lblTotal" runat="server"></asp:Label>
                                    </td>
                                </FooterTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alterrow">
                            <td>
                                <asp:Label ID="lblCategory" runat="server"></asp:Label>
                            </td>
                            <asp:Repeater ID="repCount" runat="server" OnItemDataBound="repCount_ItemDataBound">
                            <HeaderTemplate></HeaderTemplate>
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderLeft">
                                        <asp:HyperLink ID="hlCount" runat="server" Text='<%# Eval("BadgedOnProjectCount")%>'
                                            Target="_blank"></asp:HyperLink>
                                        <asp:Label ID="lblCount" runat="server" Text='<%# Eval("BadgedOnProjectCount")%>'></asp:Label>
                                    </td>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <td class="DayTotalHoursBorderLeft">
                                       <asp:Label ID="lblTotal" runat="server"></asp:Label>
                                    </td>
                                </FooterTemplate>
                            </asp:Repeater>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                       <%-- <tr class="ReportItemTemplate">
                            <td>
                            TOTALS
                            </td>
                        </tr>--%>
                        </tbody> </table> </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
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
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpdateView" />
            <asp:PostBackTrigger ControlID="btnOKValidationPanel" />
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

