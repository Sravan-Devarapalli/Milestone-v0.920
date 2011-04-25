<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Projects.aspx.cs" Inherits="PraticeManagement.Projects"
    Title="Practice Management - Projects" MasterPageFile="~/PracticeManagementMain.Master" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons" Assembly="PraticeManagement" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc1" %>

<%@ Register Src="Controls/PracticeFilter.ascx" TagName="PracticeFilter" TagPrefix="uc1" %>
<%@ Register Src="Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register src="Controls/Generic/LoadingProgress.ascx" tagname="LoadingProgress" tagprefix="uc3" %>

<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Projects</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Projected Project Profit & Loss
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    
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

    </script>

    <div class="filters">
        <div class="buttons-block">
                <table style="border: none; padding-left: 10px;" class="WholeWidth">
                    <tr>
                        <td>
                            <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server"
                                TargetControlID="pnlFilters" ImageControlID="btnExpandCollapseFilter"
                                CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />                        
                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                            <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Filters" />                        
                        </td>
                        <td style="width: 90px">
                            Select Dates
                        </td>
                         <td style="width: 40px; text-align: center;">
                            &nbsp;from&nbsp;
                        </td>
                        <td style="width: 115px">
                            <uc2:MonthPicker ID="mpPeriodStart" runat="server" AutoPostBack="false" />
                        </td>
                        <td style="width: 26px; text-align: center;">
                            &nbsp;to&nbsp;
                        </td>
                        <td style="width: 115px">
                            <uc2:MonthPicker ID="mpPeriodEnd" runat="server" AutoPostBack="false" />
                        </td>
                        <td style="width: 20px">
                            <asp:CustomValidator ID="custPeriod" runat="server" ErrorMessage="The Period Start must be less than or equal to the Period End"
                                ToolTip="The Period Start must be less than or equal to the Period End" Text="*"
                                EnableClientScript="False" OnServerValidate="custPeriod_ServerValidate" ValidationGroup="Filter"></asp:CustomValidator>
                            <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="False"
                                ErrorMessage="The period length must be not more then {0} months." ToolTip="The period length must be not more then {0} months."
                                Text="*" EnableClientScript="False" OnServerValidate="custPeriodLengthLimit_ServerValidate"
                                ValidationGroup="Filter"></asp:CustomValidator>
                        </td>
                        <td>
                        <asp:Button ID="btnExportToExcel" runat="server" OnClick="btnExportToExcel_Click"
                                Text="Export" CssClass="pm-button" />
                            <asp:Button ID="Button2" runat="server" Text="Reset Filter" Width="100px" CausesValidation="False"
                                OnClientClick="this.disabled=true;Delete_Cookie('CompanyPerformanceFilterKey', '/', '');window.location.href=RemoveExtraCharAtEnd(window.location.href);return false;"
                                EnableViewState="False" CssClass="pm-button" />
                                &nbsp;
                            <asp:Button ID="Button1" runat="server" Text="Update View" Width="100px" OnClick="btnUpdateView_Click"
                                ValidationGroup="Filter" EnableViewState="False" CssClass="pm-button" />
                        </td>
                        <td></td>
                        <td><asp:ShadowedHyperlink runat="server" Text="Add Project" ID="lnkAddProject" CssClass="add-btn" NavigateUrl="~/ProjectDetail.aspx?returnTo=Projects.aspx"/></td>
                    </tr>
                </table>
        </div>
        <asp:Panel ID="pnlFilters" runat="server">
            <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                CssClass="CustomTabStyle">
                <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Main filters</span></a>
                        </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="project-filter">
                            <table class="WholeWidth" cellpadding="5">
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbActive" runat="server" Text="Active Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbPeriodOnly" runat="server" Text="Total Only Selected Date Window"
                                            Checked="True" EnableViewState="False" 
                                            onclick='<%# "excludeDualSelection(\"" + chbPrintVersion.ClientID + "\");return true;"%>'/>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbProjected" runat="server" Text="Projected Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbPrintVersion" runat="server" Text="Print version" EnableViewState="False" 
                                            onclick='<%# "excludeDualSelection(\"" + chbPeriodOnly.ClientID + "\");return true;"%>'/>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbCompleted" runat="server" Text="Completed Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbInternal" runat="server" Text="Internal Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbExperimental" runat="server" Text="Experimental Projects" EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbInactive" runat="server" Text="Inactive Projects" EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tpAdvancedFilters" runat="server">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Advanced
                            Filters</span></a> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="project-filter">
                            <table class="WholeWidth">
                                <tr class="tb-header">
                                    <td>
                                        Client
                                    </td>
                                    <td>
                                        Project Group
                                    </td>
                                    <td>
                                        Salesperson
                                    </td>
                                    <td>
                                        Project Owner
                                    </td>
                                    <td>
                                        Practice Area
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <uc:CascadingMsdd ID="cblClient" runat="server" TargetControlId="cblProjectGroup" SetDirty="false" Width="170" Height="100"/>
                                    </td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" Width="170" Height="100" />
                                    </td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblSalesperson" runat="server" CssClass="scroll-y" SetDirty="false" Width="170" Height="100"/>
                                    </td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblProjectOwner" runat="server" CssClass="scroll-y" SetDirty="false" Width="170" Height="100"/>
                                    </td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblPractice" runat="server" CssClass="scroll-y" SetDirty="false" Width="170" Height="100"/>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tpSearch" runat="server">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Search</span></a>
                        </span>
                    </HeaderTemplate>
                    <ContentTemplate>
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
                                        <asp:Button ID="btnSearch" runat="server" Text="Search" ValidationGroup="Search"
                                            PostBackUrl="~/ProjectSearch.aspx" Width="100px" EnableViewState="False" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </AjaxControlToolkit:TabContainer>
        </asp:Panel>
        <asp:ValidationSummary ID="valsPerformance" runat="server" Width="100%" ValidationGroup="Filter" CssClass="searchError"/>
        <asp:ValidationSummary ID="valsSearch" runat="server" ValidationGroup="Search"
            EnableClientScript="true" ShowMessageBox="false" CssClass="searchError"/>
    </div>
    <cc1:StyledUpdatePanel ID="StyledUpdatePanel" runat="server" CssClass="container">
        <ContentTemplate>
            <asp:Panel class="this value set OnPageLoad" runat="server" ID="horisontalScrollDiv" ScrollBars="Auto" Height="480px">
                <asp:ListView ID="lvProjects" runat="server" DataKeyNames="Id" OnItemDataBound="lvProjects_ItemDataBound"
                    ondatabinding="lvProjects_DataBinding" onsorted="lvProjects_Sorted" OnDataBound="lvProjects_OnDataBound"
                    onsorting="lvProjects_Sorting" OnPagePropertiesChanging="lvProjects_PagePropertiesChanging">
                    <LayoutTemplate>
                        <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">
                            <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                <td class="CompPerfProjectState">
                                    <div class="ie-bg"></div>
                                </td>
                                <td class="CompPerfProjectNumber">
                                    <div class="ie-bg">
                                        <asp:LinkButton ID="btnSortProject" CommandArgument="1" CommandName="Sort" runat="server" CssClass="arrow">Project #</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfClient">
                                    <div class="ie-bg">
                                        <asp:LinkButton ID="btnSortClient" CommandArgument="2" CommandName="Sort" runat="server" CssClass="arrow">Client</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfProject">
                                    <div class="ie-bg">
                                        <asp:LinkButton ID="btnSortProjectName" CommandArgument="3" CommandName="Sort" runat="server" CssClass="arrow">Project</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfPeriod">
                                    <div class="ie-bg alignCenter">
                                        <asp:LinkButton ID="btnSortStartDate" CommandArgument="4" CommandName="Sort" runat="server" CssClass="arrow">Start Date</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfPeriod">
                                    <div class="ie-bg  alignCenter">
                                        <asp:LinkButton ID="btnSortEndDate" CommandArgument="5" CommandName="Sort" runat="server" CssClass="arrow">End Date</asp:LinkButton>
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
                                    <div class="cell-pad">Financial Summary</div>
                                </td>
                            </tr>
                            <tbody>
                                <tr runat="server" id="itemPlaceholder" />
                                 <tr>
                                    <td colspan="7">
                                        <asp:DataPager ID="dpProjects" runat="server" PagedControlID="lvProjects" PageSize="10">
                                            <Fields>        
                                                <asp:TemplatePagerField OnPagerCommand="Pager_PagerCommand">
                                                    <PagerTemplate>
                                                        <asp:LinkButton runat="server" Text="Previous" PostBackUrl="#" CommandName="<%# PagerPrevCommand %>" Visible="<%# IsNeedToShowPrevButton() %>"></asp:LinkButton>
                                                    </PagerTemplate>
                                                </asp:TemplatePagerField>                                     
                                                <asp:numericpagerfield ButtonCount="10" NextPageText="..." PreviousPageText="..." />                                                
                                                <asp:TemplatePagerField OnPagerCommand="Pager_PagerCommand">
                                                    <PagerTemplate>
                                                        <asp:LinkButton runat="server" Text="Next" PostBackUrl="#" CommandName="<%# PagerNextCommand %>" Visible="<%# IsNeedToShowNextButton() %>"></asp:LinkButton>
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
                        <tr runat="server" id="boundingRow" style="background-color:White">
                            <td class="CompPerfProjectState"></td>
                            <td class="CompPerfProjectNumber">
                                <asp:Label ID="lblProjectNumber" runat="server" />
                            </td>
                            <td class="CompPerfClient">
                                <asp:HyperLink ID="btnClientName" runat="server" />
                            </td>
                            <td class="CompPerfProject"></td>
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
                            <td class="CompPerfProjectState"></td>
                            <td class="CompPerfProjectNumber">
                                <asp:Label ID="lblProjectNumber" runat="server" />
                            </td>
                            <td class="CompPerfClient">
                                <asp:HyperLink ID="btnClientName" runat="server" />
                            </td>
                            <td class="CompPerfProject"></td>
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
                    <uc3:LoadingProgress ID="LoadingProgress1" runat="server" />
            </asp:Panel>
        </ContentTemplate>
    </cc1:StyledUpdatePanel>
    
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="footer">
    <div class="version">
        Version.
        <asp:Label ID="lblCurrentVersion" runat="server"></asp:Label></div>
</asp:Content>

