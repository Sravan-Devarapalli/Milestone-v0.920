<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="RevenueGoals.aspx.cs" Inherits="PraticeManagement.RevenueGoals" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Revenue Goals | Practice Management</title>
</asp:Content>
<asp:Content ID="ctrlhead" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Revenue Goals
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function ChangeResetButton() {
            var button = document.getElementById("<%= btnResetFilter.ClientID%>");
            var hdnFiltersChanged = document.getElementById("<%= hdnFiltersChanged.ClientID %>");
            if (button != null) {
                button.disabled = false;
                hdnFiltersChanged.value = "true";
            }
        }
        function EnableResetButton() {
            var hdnFiltersChangedSinceLastUpdate = document.getElementById("<%= hdnFiltersChangedSinceLastUpdate.ClientID %>");
            hdnFiltersChangedSinceLastUpdate.value = "true";
            ChangeResetButton();
        }
    </script>
    <div class="filters Margin-Bottom10Px">
        <div class="buttons-block">
            <table class="WholeWidth">
                <tr>
                    <td class="Width30Px PaddingTop3">
                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                            ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/expand.jpg"
                            ToolTip="Expand Filters" />
                    </td>
                    <td class="Width145px no-wrap">
                        Show Revenue Goals for
                    </td>
                    <td class="Width235Px">
                        <asp:DropDownList ID="ddlGoalsFor" runat="server" onchange="ChangeResetButton();">
                            <asp:ListItem Text="Entire Company" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Executives in Charge" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Practice Areas" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Business Development Managers" Value="3"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="Width40Px">
                        from
                    </td>
                    <td class="Width120Px">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <uc:MonthPicker ID="mpFromControl" runat="server" OnClientChange="EnableResetButton();"
                                    OnSelectedValueChanged="mpFromControl_OnSelectedValueChanged" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td class="Width30Px">
                        to
                    </td>
                    <td class="Width120Px">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <uc:MonthPicker ID="mpToControl" runat="server" OnClientChange="EnableResetButton();"
                                    OnSelectedValueChanged="mpToControl_OnSelectedValueChanged" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <asp:Button ID="btnUpdateView" runat="server" Text="Update View" CssClass="Width100PxImp"
                                                OnClick="btnUpdate_OnClick" EnableViewState="False" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td>
                                    <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" CausesValidation="false"
                                        EnableViewState="False" CssClass="Width100PxImp" OnClientClick="window.location.href = window.location.href;return false;" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlFilters" runat="server">
            <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                CssClass="CustomTabStyle">
                <AjaxControlToolkit:TabPanel runat="server" ID="tpFilters">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Basic</span></a> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <table class="WholeWidth">
                            <tr align="center" class="BorderBottom1pxTd">
                                <td class="Width330Px vTop border">
                                    Project Type
                                </td>
                                <td class="Width20Px">
                                </td>
                                <td class="Width250Px border">
                                    Practice Area
                                </td>
                                <td class="Width20Px">
                                </td>
                                <td class="Width260Px border">
                                    Business Development Manager
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table class="WholeWidth">
                                        <tr>
                                            <td class="Width100Px">
                                                <asp:CheckBox ID="chbActive" runat="server" AutoPostBack="false" Checked="True" onclick="EnableResetButton();"
                                                    Text="Active" ToolTip="Include active projects into report" />
                                            </td>
                                            <td class="Width100Px">
                                                <asp:CheckBox ID="chbInternal" runat="server" AutoPostBack="false" Checked="True"
                                                    onclick="EnableResetButton();" Text="Internal" ToolTip="Include internal projects into report" />
                                            </td>
                                            <td class="Width100Px">
                                                <asp:CheckBox ID="chbInactive" runat="server" AutoPostBack="false" Checked="false"
                                                    onclick="EnableResetButton();" Text="Inactive" ToolTip="Include Inactive projects into report" />
                                            </td>
                                              <td class="Width100Px">
                                                <asp:CheckBox ID="chbProposed" runat="server" AutoPostBack="false" Checked="true"
                                                    onclick="EnableResetButton();" Text="Proposed" ToolTip="Include proposed projects into report" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chbProjected" runat="server" AutoPostBack="false" Checked="True"
                                                    onclick="EnableResetButton();" Text="Projected" ToolTip="Include Projected projects into report" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chbCompleted" runat="server" AutoPostBack="false" Checked="True"
                                                    onclick="EnableResetButton();" Text="Completed" ToolTip="Include Completed projects into report" />
                                            </td>
                                            <td style="white-space:nowrap">
                                                <asp:CheckBox ID="chbExperimental" runat="server" AutoPostBack="false" Checked="false"
                                                    onclick="EnableResetButton();" Text="Experimental" ToolTip="Include Experimental projects into report" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chbAtRisk" runat="server" AutoPostBack="false" Checked="true"
                                                    onclick="EnableResetButton();" Text="At Risk" ToolTip="Include At Risk projects into report" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                                <td>
                                    <div class="PaddingTop5 padLeft3">
                                        <pmc:ScrollingDropDown ID="cblPractice" runat="server" AllSelectedReturnType="AllItems"
                                            onclick="scrollingDropdown_onclick('cblPractice','Practice Area')" NoItemsType="All"
                                            SetDirty="False" DropDownListType="Practice Area" CssClass="scroll_cblPractice" />
                                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractice"
                                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                        </ext:ScrollableDropdownExtender>
                                    </div>
                                    <asp:CheckBox ID="chbExcludeInternalPractices" runat="server" Text="Exclude Internal Practice Areas"
                                        onclick="EnableResetButton();" />
                                </td>
                                <td class="Width30Px">
                                </td>
                                <td class="vTop">
                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                        <ContentTemplate>
                                            <div class="PaddingTop5 padLeft3">
                                                <pmc:ScrollingDropDown ID="cblBDMs" runat="server" AllSelectedReturnType="AllItems"
                                                    onclick="scrollingDropdown_onclick('cblBDMs','Business Development Manager')"
                                                    NoItemsType="All" SetDirty="False" CssClass="scroll_cblBDMs" DropDownListType="Business Development Manager" />
                                                <ext:ScrollableDropdownExtender ID="sdeBDMs" runat="server" TargetControlID="cblBDMs"
                                                    UseAdvanceFeature="true" Width="270px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                </ext:ScrollableDropdownExtender>
                                                <br />
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
            </AjaxControlToolkit:TabContainer>
        </asp:Panel>
    </div>
    <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
    <asp:HiddenField ID="hdnFiltersChangedSinceLastUpdate" runat="server" Value="false" />
    <asp:UpdatePanel ID="upDirectorRevenueGoals" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="xScrollAuto padRight1">
                <asp:Label ID="lblDirectorEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                <asp:ListView ID="lvDirectorBudget" runat="server" OnDataBinding="lvBudget_OnDataBinding"
                    OnItemDataBound="lvPersonBudget_OnItemDataBound" OnPreRender="lvBudget_OnPreRender"
                    OnSorting="lvBudget_OnSorting" OnSorted="lvBudget_Sorted">
                    <LayoutTemplate>
                        <table class="CompPerfTable WholeWidth">
                            <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                <td align="center" class="Width170Px">
                                    <div class="ie-bg Width170Px">
                                        <asp:LinkButton ID="btnSortDirector" CommandArgument="0" CommandName="Sort" runat="server"
                                            CssClass="arrow">
                                        Executive in Charge
                                        </asp:LinkButton>
                                    </div>
                                </td>
                                <td align="center" class="MonthSummary">
                                    <div class="ie-bg">
                                        Grand Total</div>
                                </td>
                            </tr>
                            <tr runat="server" id="itemPlaceholder" />
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="testTr" runat="server" class="Height35Px">
                            <td>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr id="testTr" runat="server" class="rowEven Height35Px">
                            <td>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <EmptyDataTemplate>
                        <tr>
                            <td>
                                There is nothing to be displayed here.
                            </td>
                        </tr>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upPracticeRevenueGoals" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <hr id="hrDirectorAndPracticeSeperator" runat="server" visible="false" size="2" class="color888888"
                align="center" />
            <div class="xScrollAuto padRight1">
                <asp:Label ID="lblPracticeEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                <asp:ListView ID="lvPracticeBudget" runat="server" OnDataBinding="lvBudget_OnDataBinding"
                    OnItemDataBound="lvPracticeBudget_OnItemDataBound" OnPreRender="lvBudget_OnPreRender"
                    OnSorting="lvBudget_OnSorting" OnSorted="lvBudget_Sorted">
                    <LayoutTemplate>
                        <table class="CompPerfTable WholeWidth">
                            <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                <td align="center" class="Width170Px">
                                    <div class="ie-bg Width170Px">
                                        <asp:LinkButton ID="btnSortPractice" CommandArgument="0" CommandName="Sort" runat="server"
                                            CssClass="arrow">
                                        Practice Area
                                        </asp:LinkButton>
                                    </div>
                                </td>
                                <td align="center" class="MonthSummary">
                                    <div class="ie-bg">
                                        Grand Total</div>
                                </td>
                            </tr>
                            <tr runat="server" id="itemPlaceholder" />
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="testTr" runat="server" class="Height35Px">
                            <td>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr id="testTr" runat="server" class="rowEven Height35Px">
                            <td>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <EmptyDataTemplate>
                        <tr>
                            <td>
                                There is nothing to be displayed here.
                            </td>
                        </tr>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upBDMRevenueGoals" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <hr id="hrPracticeAndACMgrSeperator" runat="server" visible="false" size="2" class="color888888"
                align="center" />
            <div class="xScrollAuto padRight1">
                <asp:Label ID="lblBDMEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                <asp:ListView ID="lvBDMBudget" runat="server" OnDataBinding="lvBudget_OnDataBinding"
                    OnItemDataBound="lvPersonBudget_OnItemDataBound" OnPreRender="lvBudget_OnPreRender"
                    OnSorting="lvBudget_OnSorting" OnSorted="lvBudget_Sorted">
                    <LayoutTemplate>
                        <table class="CompPerfTable WholeWidth">
                            <tr runat="server" id="lvHeader" class="CompPerfHeaderGroupBy">
                                <td align="center" class="Width170Px">
                                    <div class="ie-bg Width170Px">
                                        <asp:LinkButton ID="btnSortBDM" CommandArgument="0" CommandName="Sort" runat="server"
                                            CssClass="arrow WhiteSpaceNormal">
                                        Business Development Manager
                                        </asp:LinkButton>
                                    </div>
                                </td>
                                <td align="center" class="MonthSummary">
                                    <div class="ie-bg">
                                        Grand Total</div>
                                </td>
                            </tr>
                            <tr runat="server" id="itemPlaceholder" />
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="testTr" runat="server" class="Height35Px">
                            <td class="Width170Px">
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr id="testTr" runat="server" class="rowEven Height35Px">
                            <td class="Width170Px">
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <EmptyDataTemplate>
                        <tr>
                            <td>
                                There is nothing to be displayed here.
                            </td>
                        </tr>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="progress" runat="server" />
</asp:Content>

