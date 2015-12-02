<%@ Page Title="Persons By Project" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="PersonsByProject.aspx.cs" Inherits="PraticeManagement.Reports.Badge.PersonsByProject" %>

<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
    <script src='<%# Generic.GetClientUrl("~/Scripts/ExpandOrCollapse.min.js", this) %>'
        type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.4.1.yui.js" type="text/javascript"></script>
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="hdncpeExtendersIds" runat="server" Value="" />
            <asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
            <asp:HiddenField ID="hdncpeMilestoneExtIds" runat="server" Value="" />
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
                <tr class="no-wrap">
                    <td class="ReportFilterLabels">
                        Account:&nbsp;
                    </td>
                    <td colspan="2" class="PaddingTop5">
                        <pmc:ScrollingDropDown ID="cblClient" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            onclick="scrollingDropdown_onclick('cblClient','Account')" NoItemsType="All" PluralForm="s"
                            DropDownListType="Account" CellPadding="3" CssClass="NewHireReportCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdeCblClient" runat="server" TargetControlID="cblClient"
                            UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="250px">
                        </ext:ScrollableDropdownExtender>

                       
                    </td>
                    <td class="ReportFilterLabels">
                        &nbsp&nbsp&nbsp&nbsp&nbsp Project Status:&nbsp;
                    </td>
                    <td>
                        <pmc:ScrollingDropDown ID="cblProjectTypes" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            onclick="scrollingDropdown_onclick('cblProjectTypes','Project Status','es')"
                            PluralForm="es" NoItemsType="All" DropDownListType="Project Status" CellPadding="3"
                            CssClass="NewHireReportCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdeProjectTypes" runat="server" TargetControlID="cblProjectTypes"
                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td class="textLeft Width90Percent">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr style="white-space: nowrap">
                    <td class="ReportFilterLabels">
                        Pay Type:&nbsp;
                    </td>
                    <td colspan="2" style="padding-top: 5px;">
                         <pmc:ScrollingDropDown ID="cblPayTypes" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            onclick="scrollingDropdown_onclick('cblPayTypes','Pay Type')" NoItemsType="All" PluralForm="s"
                            DropDownListType="Pay Type" CellPadding="3" CssClass="NewHireReportCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdePayTypes" runat="server" TargetControlID="cblPayTypes"
                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td class="ReportFilterLabels">
                        &nbsp&nbsp&nbsp&nbsp&nbsp Practice Area:&nbsp;
                    </td>
                    <td>
                        <pmc:ScrollingDropDown ID="cblPractices" runat="server" AllSelectedReturnType="AllItems"
                            onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" CellPadding="3" PluralForm="s"
                            NoItemsType="All" SetDirty="False" DropDownListType="Practice Area" CssClass="NewHireReportCblTimeScales" />
                        <%--UTilTimeLineFilterCblPractices--%>
                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
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
                    <td colspan="2" style="padding-top: 5px;">
                        <pmc:ScrollingDropDown ID="cblPersonStatus" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            Height="80px" onclick="scrollingDropdown_onclick('cblPersonStatus','Person Status','es')"
                            NoItemsType="All" PluralForm="es" DropDownListType="Person Status" CellPadding="3"
                            CssClass="NewHireReportCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdePersonStatus" runat="server" TargetControlID="cblPersonStatus"
                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkExcludeInternalPractices" runat="server" Text="Exclude Internal Practice Areas"
                            Checked="true" onclick="EnableResetButton();" />
                    </td>
                    <td colspan="2" class="floatright">
                        <asp:Button ID="btnUpdateView" runat="server" Text="View Report" OnClick="btnUpdateView_Click" />
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
                            <asp:Button ID="btnExpandOrCollapseAll" runat="server" Text="Collapse All" CssClass="Width100Px"
                                UseSubmitBehavior="false" ToolTip="Collapse All" />
                            <asp:Button ID="btnHiddenExpandAll" runat="server" Text="Expand All" CssClass="Width100Px hidden"
                                 UseSubmitBehavior="false" ToolTip="Collapse All" />
                        </td>
                        <td class="Width10Percent padRight5">
                            <table class="WholeWidth">
                                <tr>
                                    <td>
                                        Export:
                                    </td>
                                    <td>
                                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                                            UseSubmitBehavior="false" ToolTip="Export To Excel" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <div style="padding-top: 10px;">
                    <asp:Repeater ID="repProjects" runat="server" OnItemDataBound="repProjects_ItemDataBound">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <table class="WholeWidthWithHeight">
                                <tr class="textLeft bgColorD4D0C9">
                                    <td>
                                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeMilestones" runat="Server" CollapsedText="Expand Milestone Details"
                                            ExpandedText="Collapse Milestone Details" EnableViewState="true" BehaviorID="cpeMilestones"
                                            Collapsed="true" TargetControlID="pnlMilestoneDetails" ImageControlID="imgProject"
                                            TextLabelID="lblProject" CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg"
                                            CollapseControlID="imgProject" ExpandControlID="imgProject" />
                                        <asp:Image ID="imgProject" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Milestone Details" />
                                        <asp:Label ID="lblProject" CssClass="displayNone" runat="server"></asp:Label>
                                        <%# Eval("ProjectNumber")%>
                                        -
                                        <%# Eval("HtmlEncodedName")%>
                                        (
                                        <%# GetDateFormat((DateTime)Eval("StartDate"))%>
                                        -
                                        <%# GetDateFormat((DateTime)Eval("EndDate"))%>)
                                    </td>
                                </tr>
                            </table>
                            <asp:Panel ID="pnlMilestoneDetails" runat="server" Style="padding-bottom: 5px; padding-left: 15px;">
                                <asp:Repeater ID="repMilestones" runat="server" OnItemDataBound="repMilestones_ItemDataBound">
                                    <ItemTemplate>
                                        <table class="WholeWidthWithHeight">
                                            <tr>
                                                <td>
                                                    <%# Eval("HtmlEncodedDescription")%>
                                                    (
                                                    <%# GetDateFormat((DateTime)Eval("StartDate"))%>
                                                    -
                                                    <%# GetDateFormat((DateTime)Eval("ProjectedDeliveryDate"))%>)
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Panel ID="pnlResources" runat="server" Style="padding-bottom: 5px; padding-left: 25px;">
                                            <asp:Repeater ID="repResources" runat="server" OnItemDataBound="repResources_ItemDataBound">
                                                <HeaderTemplate>
                                                    <table class="Width80Percent PersonSummaryReport zebra">
                                                        <thead>
                                                            <tr>
                                                                <th class="Width12Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Resource Name
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Resource Level
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Person Status
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    Milestone Resource Start Date
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    Milestone Resource End Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Badge Start Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Badge End Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Organic Break Start
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Organic Break End
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    MSFT Block Start Date
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    MSFT Block End Date
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr class="ReportItemTemplate">
                                                        <td class="textLeft DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("HtmlEncodedName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Title.HtmlEncodedTitleName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Status.Name")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceStartDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceEndDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockEnd" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <AlternatingItemTemplate>
                                                    <tr class="alterrow">
                                                        <td class="textLeft DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("HtmlEncodedName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Title.HtmlEncodedTitleName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Status.Name")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceStartDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceEndDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockEnd" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </AlternatingItemTemplate>
                                                <FooterTemplate>
                                                    </tbody></table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </asp:Panel>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <table class="WholeWidthWithHeight">
                                            <tr class="bgColor_F5FAFF">
                                                <td>
                                                    <%# Eval("HtmlEncodedDescription")%>
                                                    (
                                                    <%# GetDateFormat((DateTime)Eval("StartDate"))%>
                                                    -
                                                    <%# GetDateFormat((DateTime)Eval("ProjectedDeliveryDate"))%>)
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Panel ID="pnlResources" runat="server" Style="padding-bottom: 5px; padding-left: 25px;">
                                            <asp:Repeater ID="repResources" runat="server" OnItemDataBound="repResources_ItemDataBound">
                                                <HeaderTemplate>
                                                    <table class="Width80Percent PersonSummaryReport zebra">
                                                        <thead>
                                                            <tr>
                                                               <th class="Width12Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Resource Name
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Resource Level
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Person Status
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    Milestone Resource Start Date
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    Milestone Resource End Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Badge Start Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Badge End Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Organic Break Start
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Organic Break End
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    MSFT Block Start Date
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    MSFT Block End Date
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr class="ReportItemTemplate">
                                                        <td class="textLeft DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("HtmlEncodedName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Title.HtmlEncodedTitleName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Status.Name")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceStartDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceEndDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockEnd" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <AlternatingItemTemplate>
                                                    <tr class="alterrow">
                                                        <td class="textLeft DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("HtmlEncodedName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Title.HtmlEncodedTitleName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Status.Name")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceStartDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceEndDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockEnd" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </AlternatingItemTemplate>
                                                <FooterTemplate>
                                                    </tbody></table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </asp:Panel>
                                    </AlternatingItemTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <table class="WholeWidthWithHeight">
                                <tr class="textLeft bgcolor_ECE9D9">
                                    <td>
                                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeMilestones" runat="Server" CollapsedText="Expand Milestone Details"
                                            ExpandedText="Collapse Milestone Details" EnableViewState="true" BehaviorID="cpeMilestones"
                                            Collapsed="true" TargetControlID="pnlMilestoneDetails" ImageControlID="imgProject"
                                            TextLabelID="lblProject" CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg"
                                            CollapseControlID="imgProject" ExpandControlID="imgProject" />
                                        <asp:Image ID="imgProject" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Milestone Details" />
                                        <asp:Label ID="lblProject" CssClass="displayNone" runat="server"></asp:Label>
                                        <%# Eval("ProjectNumber")%>
                                        -
                                        <%# Eval("HtmlEncodedName")%>
                                        (
                                        <%# GetDateFormat((DateTime)Eval("StartDate"))%>
                                        -
                                        <%# GetDateFormat((DateTime)Eval("EndDate"))%>)
                                    </td>
                                </tr>
                            </table>
                            <asp:Panel ID="pnlMilestoneDetails" runat="server" Style="padding-bottom: 5px; padding-left: 15px;">
                                <asp:Repeater ID="repMilestones" runat="server" OnItemDataBound="repMilestones_ItemDataBound">
                                    <ItemTemplate>
                                        <table class="WholeWidthWithHeight">
                                            <tr>
                                                <td>
                                                    <%# Eval("HtmlEncodedDescription")%>
                                                    (
                                                    <%# GetDateFormat((DateTime)Eval("StartDate"))%>
                                                    -
                                                    <%# GetDateFormat((DateTime)Eval("ProjectedDeliveryDate"))%>)
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Panel ID="pnlResources" runat="server" Style="padding-bottom: 5px; padding-left: 25px;">
                                            <asp:Repeater ID="repResources" runat="server" OnItemDataBound="repResources_ItemDataBound">
                                                <HeaderTemplate>
                                                    <table class="Width80Percent PersonSummaryReport zebra">
                                                        <thead>
                                                            <tr>
                                                                <th class="Width12Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Resource Name
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Resource Level
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Person Status
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    Milestone Resource Start Date
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    Milestone Resource End Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Badge Start Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Badge End Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Organic Break Start
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Organic Break End
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    MSFT Block Start Date
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    MSFT Block End Date
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr class="ReportItemTemplate">
                                                        <td class="textLeft DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("HtmlEncodedName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Title.HtmlEncodedTitleName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Status.Name")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceStartDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceEndDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockEnd" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <AlternatingItemTemplate>
                                                    <tr class="alterrow">
                                                        <td class="textLeft DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("HtmlEncodedName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Title.HtmlEncodedTitleName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Status.Name")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceStartDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceEndDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockEnd" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </AlternatingItemTemplate>
                                                <FooterTemplate>
                                                    </tbody></table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </asp:Panel>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <table class="WholeWidthWithHeight">
                                            <tr class="bgColor_F5FAFF">
                                                <td>
                                                    <%# Eval("HtmlEncodedDescription")%>
                                                    (
                                                    <%# GetDateFormat((DateTime)Eval("StartDate"))%>
                                                    -
                                                    <%# GetDateFormat((DateTime)Eval("ProjectedDeliveryDate"))%>)
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Panel ID="pnlResources" runat="server" Style="padding-bottom: 5px; padding-left: 25px;">
                                            <asp:Repeater ID="repResources" runat="server" OnItemDataBound="repResources_ItemDataBound">
                                                <HeaderTemplate>
                                                    <table class="Width80Percent PersonSummaryReport zebra">
                                                        <thead>
                                                            <tr>
                                                                <th class="Width12Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Resource Name
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Resource Level
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Person Status
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    Milestone Resource Start Date
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    Milestone Resource End Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Badge Start Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Badge End Date
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Organic Break Start
                                                                </th>
                                                                <th class="Width8Percent DayTotalHoursBorderLeft Padding5Imp">
                                                                    Organic Break End
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    MSFT Block Start Date
                                                                </th>
                                                                <th class="Width10Per DayTotalHoursBorderLeft Padding5Imp">
                                                                    MSFT Block End Date
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr class="ReportItemTemplate">
                                                        <td class="textLeft DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("HtmlEncodedName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Title.HtmlEncodedTitleName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Status.Name")%>
                                                        </td>
                                                         <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceStartDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceEndDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockEnd" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <AlternatingItemTemplate>
                                                    <tr class="alterrow">
                                                        <td class="textLeft DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("HtmlEncodedName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Title.HtmlEncodedTitleName")%>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <%# Eval("Status.Name")%>
                                                        </td>
                                                         <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceStartDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblResourceEndDate" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBadgeEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblOrganicEnd" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockStart" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="DayTotalHoursBorderLeft Padding5Imp">
                                                            <asp:Label ID="lblBlockEnd" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </AlternatingItemTemplate>
                                                <FooterTemplate>
                                                    </tbody></table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </asp:Panel>
                                    </AlternatingItemTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div id="divEmptyMessage" class="EmptyMessagediv hidden" runat="server">
                    There are no resources for the selected filters.
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
            <asp:PostBackTrigger ControlID="btnExportToPDF" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

