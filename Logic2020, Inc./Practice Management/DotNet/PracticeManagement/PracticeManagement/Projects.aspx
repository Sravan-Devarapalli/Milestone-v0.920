<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Projects.aspx.cs" Inherits="PraticeManagement.Projects"
    Title="Practice Management - Projects" MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Src="Controls/PracticeFilter.ascx" TagName="PracticeFilter" TagPrefix="uc1" %>
<%@ Register Src="Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Projects</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Projected Project Profit & Loss
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script language="javascript" type="text/javascript" src="Scripts/ScrollinDropDown.js"></script>
    <script type="text/javascript">

        function excludeDualSelection(target) {
            var targetElement = $get(target);

            if (targetElement)
                if (targetElement.checked)
                    targetElement.checked = false;
        }
        function RemoveExtraCharAtEnd(url) {
            if (url.lastIndexOf('#') == url.length - 1) {
                return url.substring(0, url.length - 1);
            }
            else {
                return url;
            }
        }
        function correctMonthMiniReportPosition(reportPanelId, headerId, scrollPanelId) {
            var reportPanel = $get(reportPanelId);
            var header = $get(headerId);
            var scrollPanel = $get(scrollPanelId);

            var reportPanelPos = getPosition(reportPanel);
            var headerPosition = getPosition(header);

            reportPanel.style.left = headerPosition.left - scrollPanel.scrollLeft - 25;
        }

        function getPosition(obj) {
            var topValue = 0, leftValue = 0;
            while (obj) {
                leftValue += obj.offsetLeft;
                topValue += obj.offsetTop;
                obj = obj.offsetParent;
            }

            function point() {
                this.left = 0;
                this.top = 0;
            }

            var result = new point();
            result.left = leftValue;
            result.top = topValue;

            return result;
        }

        function enableClientGroup(clients) {
            var ClientGroup = document.getElementById("<%= cblProjectGroup.ClientID %>");
            var arrayOfCheckBoxes = clients.getElementsByTagName("input");
            if (arrayOfCheckBoxes.length == 1 && arrayOfCheckBoxes[0].disabled) {
                ClientGroup.disabled = disabled;
            }
            else {
                var temp = 0;
                for (var i = 0; i < arrayOfCheckBoxes.length; i++) {
                    if (arrayOfCheckBoxes[i].checked) {
                        temp++;
                    }
                }
                if (temp == 0) {
                    ClientGroup.disabled = true;
                }
                else {
                    ClientGroup.disabled = false;
                }
            }
        }

        function resetFiltersTab() {
            GetDefaultcblList();
            GetDefault(document.getElementById("<%= cblClient.ClientID %>"));
            GetDefault(document.getElementById("<%= cblSalesperson.ClientID %>"));
            GetDefault(document.getElementById("<%= cblPractice.ClientID %>"));
            GetDefault(document.getElementById("<%= cblProjectGroup.ClientID %>"));
            GetDefault(document.getElementById("<%= cblProjectOwner.ClientID %>"));
        }

        function GetDefaultcblList() {
            var div = document.getElementById("<%= divProjectFilter.ClientID %>");
            var arrayOfCheckBoxes = div.getElementsByTagName('input');
            for (var i = 0; i < arrayOfCheckBoxes.length; i++) {
                arrayOfCheckBoxes[i].checked = true;
            }
         }

        function GetDefault(control) {
            control.fireEvent('onclick');
        }

        function resetCalculationsTab() {
            document.getElementById("<%= chbActive.ClientID %>").checked = true;
            document.getElementById("<%= chbInternal.ClientID %>").checked = true;
            document.getElementById("<%= chbProjected.ClientID %>").checked = true;
            document.getElementById("<%= chbCompleted.ClientID %>").checked = true;
            document.getElementById("<%= chbExperimental.ClientID %>").checked = false;
            document.getElementById("<%= chbInactive.ClientID %>").checked = false;

            document.getElementById("<%= ddlCalculateRange.ClientID %>").selectedIndex = 0;
        }

    </script>
    <style>
        .xScrollProjects
        {
            overflow-x:auto;
            }
    </style>
    <asp:UpdatePanel ID="flrPanel" runat="server">
        <ContentTemplate>
    <div class="filters">
        <div class="buttons-block">
            <table style="border: none; padding-left: 10px;" class="WholeWidth">
                <tr>
                    <td style="width:3%">
                        <ajaxtoolkit:collapsiblepanelextender id="cpe" runat="Server" targetcontrolid="pnlFilters"
                            imagecontrolid="btnExpandCollapseFilter" collapsedimage="Images/expand.jpg" expandedimage="Images/collapse.jpg"
                            collapsecontrolid="btnExpandCollapseFilter" expandcontrolid="btnExpandCollapseFilter"
                            collapsed="True" textlabelid="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Search, Filter and Calculation Options" />
                    </td>
                    <td style="width:8%;">
                        &nbsp;Show&nbsp;Projects&nbsp;for&nbsp;
                    </td>
                    <td style="width:10%;">
                        <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                            <asp:ListItem Text="Next 3 months" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Next 6 months" Value="6"></asp:ListItem>
                            <asp:ListItem Text="Next 12 months" Value="12"></asp:ListItem>
                            <asp:ListItem Text="Last 3 months" Value="-3"></asp:ListItem>
                            <asp:ListItem Text="Last 6 months" Value="-6"></asp:ListItem>
                            <asp:ListItem Text="Last 12 months" Value="-12"></asp:ListItem>
                            <asp:ListItem Text="Previous FY" Value="-13"></asp:ListItem>
                            <asp:ListItem Text="Current FY" Value="13"></asp:ListItem>
                            <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td style="width: 38%;">
                        <table id="tblDateSelection" runat="server" visible="false">
                            <tr>
                                <td style="text-align: center;">
                                    &nbsp;from&nbsp;
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="upPeriodStart" runat="server">
                                        <ContentTemplate>
                                            <uc2:MonthPicker ID="mpPeriodStart" runat="server" AutoPostBack="true" OnSelectedValueChanged="mpPeriodStart_SelectedValueChanged" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td style="text-align: center;">
                                    &nbsp;to&nbsp;
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="upPeriodEnd" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <uc2:MonthPicker ID="mpPeriodEnd" runat="server" AutoPostBack="false" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td>
                                    <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="False"
                                        ErrorMessage="The period length must be not more than {0} months." ToolTip="The period length must be not more than {0} months."
                                        Text="*" EnableClientScript="False" OnServerValidate="custPeriodLengthLimit_ServerValidate"
                                        ValidationGroup="Filter"></asp:CustomValidator>
                                </td>
                                <td>
                                    <asp:Button ID="btnUpdateDates" runat="server" Text="Update" style="width:60px;" OnClick="btnUpdateView_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width:10%; text-align:right;">
                        <asp:Button ID="btnExportToExcel" runat="server" OnClick="btnExportToExcel_Click"
                            Text="Export" CssClass="pm-button" />
                        <asp:Button ID="btnExportAllToExcel" runat="server" OnClick="btnExportAllToExcel_Click"
                            Text="Export All" CssClass="pm-button" />
                        <%--<asp:Menu ID="menuExport" runat="server" DynamicTopSeparatorImageUrl="~/Images/arrow-down.png" DynamicPopOutImageUrl="~/Images/arrow-down.png">
                            <Items>
                                <asp:MenuItem Text="Export View" Selected="true"></asp:MenuItem>
                                <asp:MenuItem Text="Export All"></asp:MenuItem>
                            </Items>
                        </asp:Menu>--%>
                    </td>
                    <td style="text-align:right; width:9%;">
                        <asp:DropDownList ID="ddlView" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlView_SelectedIndexChanged">
                            <asp:ListItem Text="View 10" Value="10"></asp:ListItem>
                            <asp:ListItem Text="View 25" Value="25"></asp:ListItem>
                            <asp:ListItem Text="View 50" Value="50"></asp:ListItem>
                            <asp:ListItem Text="View ALL" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td style="width:13%;">
                        <asp:ShadowedHyperlink runat="server" Text="Add Project" ID="lnkAddProject" CssClass="add-btn"
                            NavigateUrl="~/ProjectDetail.aspx?returnTo=Projects.aspx" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlFilters" runat="server">
            <ajaxcontroltoolkit:tabcontainer id="tcFilters" runat="server" activetabindex="0"
                cssclass="CustomTabStyle">
                <ajaxtoolkit:tabpanel id="tpSearch" runat="server">
                    <headertemplate>
                        <span class="bg"><a href="#"><span>Search</span></a>
                        </span>
                    </headertemplate>
                    <contenttemplate>
                        <asp:Panel ID="pnlSearch" runat="server" CssClass="project-filter" DefaultButton="btnSearch">
                            <table class="WholeWidth">
                                <tr>
                                    <td style="padding-right: 8px;">
                                        <asp:TextBox ID="txtSearchText" runat="server" CssClass="WholeWidth" EnableViewState="False" />
                                    </td>
                                    <td><asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                            ErrorMessage="Please type text to be searched." ToolTip="Please type text to be searched."
                                            Text="*" SetFocusOnError="true" ValidationGroup="Search" CssClass="searchError" Display="Dynamic"/></td>
                                    <td>
                                        <asp:Button ID="btnSearch" runat="server" Text="Search View" ValidationGroup="Search"
                                            OnClick="btnSearch_Clicked" Width="100px" EnableViewState="False" />
                                        <asp:Button ID="btnSearchAll" runat="server" Text="Search All" ValidationGroup="Search"
                                            PostBackUrl="~/ProjectSearch.aspx" Width="100px" EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </contenttemplate>
                </ajaxtoolkit:tabpanel>
                <ajaxtoolkit:tabpanel id="tpAdvancedFilters" runat="server">
                    <headertemplate>
                        <span class="bg"><a href="#"><span>Filters</span></a> </span>
                    </headertemplate>
                    <contenttemplate>
                        <div id="divProjectFilter" runat="server" class="project-filter">
                            <table class="WholeWidth">
                                <tr class="tb-header">
                                    <td style="border-bottom:1px solid black; width:190px; text-align:center">
                                        Client / Group
                                    </td>
                                    <td style="padding:5px; width:10px;"></td>
                                    <td style="border-bottom:1px solid black; width:190px; text-align:center;">
                                        SalesTeam
                                    </td>
                                    <td style="padding:5px; width:10px;"></td>
                                    <td style="border-bottom:1px solid black; width:190px; text-align:center;">
                                        Practice Area
                                    </td>
                                    <td rowspan="3" style="text-align:right;">
                                        <table style="text-align:right; width:100%;">
                                            <tr>
                                                <td style="padding-bottom:5px;">
                                                    <asp:Button ID="btnUpdateFilters" runat="server" Text="Update" Width="100px" OnClick="btnUpdateView_Click"
                                                        ValidationGroup="Filter" EnableViewState="False" CssClass="pm-button" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top:5px;">
                                                    <asp:Button ID="btnResetFilters" runat="server" Text="Reset" Width="100px" CausesValidation="False"
                                                        OnClientClick="resetFiltersTab(); return false;"
                                                        EnableViewState="False" CssClass="pm-button" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:5px;">
                                        <uc:CascadingMsdd ID="cblClient" runat="server" TargetControlId="cblProjectGroup" SetDirty="false" Width="240" Height="240px" 
                                            onclick="enableClientGroup(this); scrollingDropdown_onclick('cblClient','Client');" DropDownListType="Client" CellPadding="3" />
                                        <ext:ScrollableDropdownExtender ID="sdeCblClient" runat="server" TargetControlID="cblClient" 
                                            UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px"></ext:ScrollableDropdownExtender>
                                    </td>
                                    <td></td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblSalesperson" runat="server" SetDirty="false" Width="240" Height="240px"
                                            onclick="scrollingDropdown_onclick('cblSalesperson','Salesperson')" DropDownListType="Salesperson" CellPadding="3" />
                                        <ext:ScrollableDropdownExtender ID="sdeCblSalesperson" runat="server" TargetControlID="cblSalesperson"
                                            UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px">
                                        </ext:ScrollableDropdownExtender>
                                    </td>
                                    <td></td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblPractice" runat="server"  SetDirty="false" Width="240" Height="240px" 
                                            onclick="scrollingDropdown_onclick('cblPractice','Practice Area')" DropDownListType="Practice Area" CellPadding="3" />
                                        <ext:ScrollableDropdownExtender ID="sdeCblPractice" runat="server" TargetControlID="cblPractice"
                                            UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px"></ext:ScrollableDropdownExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:5px;">
                                        <uc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" Width="240" Height="240px"
                                            onclick="scrollingDropdown_onclick('cblProjectGroup','Group')" DropDownListType="Group" CellPadding="3" />
                                        <ext:ScrollableDropdownExtender ID="sdeCblProjectGroup" runat="server" TargetControlID="cblProjectGroup"
                                            UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px"></ext:ScrollableDropdownExtender>
                                    </td>
                                    <td></td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblProjectOwner" runat="server" SetDirty="false" Width="240" Height="240px"
                                            onclick="scrollingDropdown_onclick('cblProjectOwner','Owner')" DropDownListType="Owner" CellPadding="3" />
                                        <ext:ScrollableDropdownExtender ID="sdeCblProjectOwner" runat="server" TargetControlID="cblProjectOwner"
                                            UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px"></ext:ScrollableDropdownExtender>
                                    </td>
                                    <td></td>
                                    <td></td>
                                </tr>
                            </table>
                        </div>
                    </contenttemplate>
                </ajaxtoolkit:tabpanel>
                <ajaxtoolkit:tabpanel runat="server" id="tpMainFilters">
                    <headertemplate>
                        <span class="bg"><a href="#"><span>Calculations</span></a>
                        </span>
                    </headertemplate>
                    <contenttemplate>
                        <div class="project-filter">
                            <table class="WholeWidth" cellpadding="5">
                                <tr class="tb-header">
                                    <td colspan="2" style="border-bottom:1px solid black; text-align:center; width:300px">
                                        Project Types Included
                                    </td>
                                    <td style="width:20px;" rowspan="4"></td>
                                    <td style="border-bottom:1px solid black; text-align:center; width:180px">
                                        Calculated Grand Total
                                    </td>
                                    <td rowspan="4" style="text-align:right;">
                                        <table style="text-align:right; width:100%;">
                                            <tr>
                                                <td style="padding-bottom:5px;">
                                                    <asp:Button ID="btnUpdateCalculations" runat="server" Text="Update" Width="100px" OnClick="btnUpdateView_Click"
                                                        ValidationGroup="Filter" EnableViewState="False" CssClass="pm-button" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top:5px;">
                                                    <asp:Button ID="btnResetCalculations" runat="server" Text="Reset" Width="100px" CausesValidation="False"
                                                        OnClientClick="resetCalculationsTab(); return false;"
                                                        EnableViewState="False" CssClass="pm-button" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px; width:160px; padding-top:5px;">
                                        <asp:CheckBox ID="chbActive" runat="server" Text="Active Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td style="padding-left: 10px; width:160px; padding-top:5px;">
                                        <asp:CheckBox ID="chbInternal" runat="server" Text="Internal Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td rowspan="2" style="text-align:center">
                                        <asp:DropDownList ID="ddlCalculateRange" runat="server" AutoPostBack="false" Width="170px">
                                            <asp:ListItem Text="Project Value in Range" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Total Project Value" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Current Fiscal Year" Value="3"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbProjected" runat="server" Text="Projected Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbInactive" runat="server" Text="Inactive Projects" EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbCompleted" runat="server" Text="Completed Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbExperimental" runat="server" Text="Experimental Projects" EnableViewState="False" />
                                    </td>
                                    <td></td>
                                </tr>
                            </table>
                        </div>
                    </contenttemplate>
                </ajaxtoolkit:tabpanel>
            </ajaxcontroltoolkit:tabcontainer>
        </asp:Panel>
        <asp:ValidationSummary ID="valsPerformance" runat="server" Width="100%" ValidationGroup="Filter"
            CssClass="searchError" />
        <asp:ValidationSummary ID="valsSearch" runat="server" ValidationGroup="Search" EnableClientScript="true"
            ShowMessageBox="false" CssClass="searchError" />
    </div>
    </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
            <asp:PostBackTrigger ControlID="btnExportAllToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <cc1:StyledUpdatePanel ID="StyledUpdatePanel" runat="server" CssClass="container"
        UpdateMode="Conditional">
        <ContentTemplate>
            <div style="font-weight: bold; width: 100%; text-align: center; padding-top:5px; padding-bottom:10px; font-size:larger;">Viewing&nbsp;
                <asp:Label ID="lblCurrentPageCount" runat="server" Text='<%# GetCurrentPageCount() %>'></asp:Label>
                &nbsp;of&nbsp;
                <asp:Label ID="lblTotalCount" runat="server" Text='<%# GetTotalCount() %>'></asp:Label>&nbsp;
                Projects
            </div>
            <asp:Panel class="this value set OnPageLoad" runat="server" ID="horisontalScrollDiv" CssClass="xScrollProjects">
                <asp:ListView ID="lvProjects" runat="server" DataKeyNames="Id" OnItemDataBound="lvProjects_ItemDataBound"
                    OnDataBinding="lvProjects_DataBinding" OnSorted="lvProjects_Sorted" OnDataBound="lvProjects_OnDataBound"
                    OnSorting="lvProjects_Sorting" OnPagePropertiesChanging="lvProjects_PagePropertiesChanging">
                    <LayoutTemplate>
                        <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">
                            <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                <td class="CompPerfProjectState">
                                    <div class="ie-bg">
                                    </div>
                                </td>
                                <td class="CompPerfProjectNumber">
                                    <div class="ie-bg">
                                        <asp:LinkButton ID="btnSortProject" CommandArgument="1" CommandName="Sort" runat="server"
                                            CssClass="arrow">Project #</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfClient">
                                    <div class="ie-bg">
                                        <asp:LinkButton ID="btnSortClient" CommandArgument="2" CommandName="Sort" runat="server"
                                            CssClass="arrow">Client</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfProject">
                                    <div class="ie-bg">
                                        <asp:LinkButton ID="btnSortProjectName" CommandArgument="3" CommandName="Sort" runat="server"
                                            CssClass="arrow">Project</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfPeriod">
                                    <div class="ie-bg alignCenter">
                                        <asp:LinkButton ID="btnSortStartDate" CommandArgument="4" CommandName="Sort" runat="server"
                                            CssClass="arrow">Start Date</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfPeriod">
                                    <div class="ie-bg  alignCenter">
                                        <asp:LinkButton ID="btnSortEndDate" CommandArgument="5" CommandName="Sort" runat="server"
                                            CssClass="arrow">End Date</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfTotalSummary">
                                    <div class="ie-bg alignCenter">
                                        Total
                                    </div>
                                </td>
                            </tr>
                            <tr runat="server" id="lvSummary" class="summary">
                                <td colspan="6">
                                    <div class="cell-pad">
                                        Financial Summary</div>
                                </td>
                            </tr>
                            <tbody>
                                <tr runat="server" id="itemPlaceholder" />
                                <tr>
                                    <td colspan="7">
                                        <asp:DataPager ID="dpProjects" runat="server" PagedControlID="lvProjects">
                                            <Fields>
                                                <asp:TemplatePagerField OnPagerCommand="Pager_PagerCommand">
                                                    <PagerTemplate>
                                                        <asp:LinkButton ID="LinkButton1" runat="server" Text="Previous" PostBackUrl="#" CommandName="<%# PagerPrevCommand %>"
                                                            Visible="<%# IsNeedToShowPrevButton() %>"></asp:LinkButton>
                                                    </PagerTemplate>
                                                </asp:TemplatePagerField>
                                                <asp:NumericPagerField ButtonCount="10" NextPageText="..." PreviousPageText="..." />
                                                <asp:TemplatePagerField OnPagerCommand="Pager_PagerCommand">
                                                    <PagerTemplate>
                                                        <asp:LinkButton ID="LinkButton2" runat="server" Text="Next" PostBackUrl="#" CommandName="<%# PagerNextCommand %>"
                                                            Visible="<%# IsNeedToShowNextButton() %>"></asp:LinkButton>
                                                    </PagerTemplate>
                                                </asp:TemplatePagerField>
                                            </Fields>
                                        </asp:DataPager>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr runat="server" id="boundingRow" style="background-color: White">
                            <td class="CompPerfProjectState">
                            </td>
                            <td class="CompPerfProjectNumber">
                                <asp:Label ID="lblProjectNumber" runat="server" />
                            </td>
                            <td class="CompPerfClient">
                                <asp:HyperLink ID="btnClientName" runat="server" />
                            </td>
                            <td class="CompPerfProject">
                            </td>
                            <td class="CompPerfPeriod">
                                <asp:Label ID="lblStartDate" runat="server" />
                            </td>
                            <td class="CompPerfPeriod">
                                <asp:Label ID="lblEndDate" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr runat="server" id="boundingRow" class="rowEven">
                            <td class="CompPerfProjectState">
                            </td>
                            <td class="CompPerfProjectNumber">
                                <asp:Label ID="lblProjectNumber" runat="server" />
                            </td>
                            <td class="CompPerfClient">
                                <asp:HyperLink ID="btnClientName" runat="server" />
                            </td>
                            <td class="CompPerfProject">
                            </td>
                            <td class="CompPerfPeriod">
                                <asp:Label ID="lblStartDate" runat="server" />
                            </td>
                            <td class="CompPerfPeriod">
                                <asp:Label ID="lblEndDate" runat="server" />
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <EmptyDataTemplate>
                        <tr runat="server" id="EmptyDataRow">
                            <td>
                                There is nothing to be displayed here.
                            </td>
                        </tr>
                    </EmptyDataTemplate>
                </asp:ListView>
                <uc3:LoadingProgress ID="LoadingProgress1" runat="server" DisplayText="Refreshing Data..." />
            </asp:Panel>
        </ContentTemplate>
    </cc1:StyledUpdatePanel>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="footer">
    <div class="version">
        Version.
        <asp:Label ID="lblCurrentVersion" runat="server"></asp:Label></div>
</asp:Content>

