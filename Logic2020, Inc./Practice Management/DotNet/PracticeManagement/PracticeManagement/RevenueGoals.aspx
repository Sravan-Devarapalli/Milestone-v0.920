<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="RevenueGoals.aspx.cs" Inherits="PraticeManagement.RevenueGoals" %>

<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Revenue Goals</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Revenue Goals
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/ScrollinDropDown.js" type="text/javascript"></script>
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
    <div class="filters" style="margin-bottom: 10px;">
        <div class="buttons-block">
            <table class="WholeWidth">
                <tr>
                    <td align="left" style="width: 30px; padding-top: 3px;">
                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                            ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/expand.jpg"
                            ToolTip="Expand Filters" />
                    </td>
                    <td style="width: 145px; white-space: nowrap;" align="left">
                        Show Revenue Goals for
                    </td>
                    <td style="width: 235px;">
                        <asp:DropDownList ID="ddlGoalsFor" runat="server" onchange="ChangeResetButton();">
                            <asp:ListItem Text="Entire Company" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Client Directors" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Practice Areas" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Business Development Managers" Value="3"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="left" style="width: 40px">
                        from
                    </td>
                    <td style="width: 120px" align="left">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <uc:MonthPicker ID="mpFromControl" runat="server" OnClientChange="EnableResetButton();"
                                    OnSelectedValueChanged="mpFromControl_OnSelectedValueChanged" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 30px" align="left">
                        to
                    </td>
                    <td style="width: 120px" align="left">
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
                                            <asp:Button ID="btnUpdateView" runat="server" Text="Update View" Width="100px" OnClick="btnUpdate_OnClick"
                                                EnableViewState="False" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td>
                                    <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" Width="100px"
                                        CausesValidation="false" EnableViewState="False" CssClass="pm-button" OnClientClick="window.location.href = window.location.href;return false;" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlFilters" runat="server">
            <ajaxToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0" CssClass="CustomTabStyle">
                <ajaxToolkit:TabPanel runat="server" ID="tpFilters">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Basic</span></a> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <table class="WholeWidth">
                            <tr align="center">
                                <td style="width: 330px !important; border-bottom: 1px solid black;" valign="top">
                                    Project Type
                                </td>
                                <td style="width: 20px;">
                                </td>
                                <td style="width: 250px !important; border-bottom: 1px solid black;">
                                    Practice Area
                                </td>
                                <td style="width: 20px;">
                                </td>
                                <td style="width: 260px !important; border-bottom: 1px solid black;">
                                    Business Development Manager
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 100px;">
                                                <asp:CheckBox ID="chbActive" runat="server" AutoPostBack="false" Checked="True" onclick="EnableResetButton();"
                                                    Text="Active" ToolTip="Include active projects into report" />
                                            </td>
                                            <td style="width: 100px;">
                                                <asp:CheckBox ID="chbInternal" runat="server" AutoPostBack="false" Checked="True"
                                                    onclick="EnableResetButton();" Text="Internal" ToolTip="Include internal projects into report" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chbInactive" runat="server" AutoPostBack="false" Checked="false"
                                                    onclick="EnableResetButton();" Text="Inactive" ToolTip="Include Inactive projects into report" />
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
                                            <td>
                                                <asp:CheckBox ID="chbExperimental" runat="server" AutoPostBack="false" Checked="false"
                                                    onclick="EnableResetButton();" Text="Experimental" ToolTip="Include Experimental projects into report" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                                <td>
                                    <div style="padding-top: 5px; padding-left: 3px;">
                                        <cc2:ScrollingDropDown ID="cblPractice" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                            onclick="scrollingDropdown_onclick('cblPractice','Practice Area')" BackColor="White"
                                            CellPadding="3" Height="250px" NoItemsType="All" SetDirty="False" DropDownListType="Practice Area"
                                            Width="260px" BorderWidth="0" />
                                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractice"
                                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                        </ext:ScrollableDropdownExtender>
                                    </div>
                                    <asp:CheckBox ID="chbExcludeInternalPractices" runat="server" Text="Exclude Internal Practice Areas"
                                        onclick="EnableResetButton();" />
                                </td>
                                <td style="width: 30px;">
                                </td>
                                <td valign="top">
                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                        <ContentTemplate>
                                            <div style="padding-top: 5px; padding-left: 3px;">
                                                <cc2:ScrollingDropDown ID="cblBDMs" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                                    onclick="scrollingDropdown_onclick('cblBDMs','Business Development Manager')"
                                                    BackColor="White" CellPadding="3" Height="250px" NoItemsType="All" SetDirty="False"
                                                    DropDownListType="Business Development Manager" Width="270px" BorderWidth="0" />
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
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </asp:Panel>
    </div>
    <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
    <asp:HiddenField ID="hdnFiltersChangedSinceLastUpdate" runat="server" Value="false" />
    <asp:UpdatePanel ID="upDirectorRevenueGoals" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="overflow-x: auto;padding-right:1px;">
                <asp:Label ID="lblDirectorEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                <asp:ListView ID="lvDirectorBudget" runat="server" OnDataBinding="lvBudget_OnDataBinding"
                    OnItemDataBound="lvPersonBudget_OnItemDataBound" OnPreRender="lvBudget_OnPreRender"
                    OnSorting="lvBudget_OnSorting" OnSorted="lvBudget_Sorted">
                    <LayoutTemplate>
                        <table class="CompPerfTable WholeWidth">
                            <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                <td align="center" style="width: 170px!important;">
                                    <div class="ie-bg" style="width: 170px!important;">
                                        <asp:LinkButton ID="btnSortDirector" CommandArgument="0" CommandName="Sort" runat="server"
                                            CssClass="arrow">
                                        Client Director
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
                        <tr id="testTr" runat="server" height="35px">
                            <td>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr id="testTr" runat="server" height="35px" class="rowEven">
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
            <hr id="hrDirectorAndPracticeSeperator" runat="server" visible="false" size="2" color="#888888"
                align="center" />
            <div style="overflow-x: auto; padding-right:1px;">
                <asp:Label ID="lblPracticeEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                <asp:ListView ID="lvPracticeBudget" runat="server" OnDataBinding="lvBudget_OnDataBinding"
                    OnItemDataBound="lvPracticeBudget_OnItemDataBound" OnPreRender="lvBudget_OnPreRender"
                    OnSorting="lvBudget_OnSorting" OnSorted="lvBudget_Sorted">
                    <LayoutTemplate>
                        <table class="CompPerfTable WholeWidth">
                            <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                <td align="center" style="width: 170px!important;">
                                    <div class="ie-bg" style="width: 170px!important;">
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
                        <tr id="testTr" runat="server" height="35px">
                            <td>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr id="testTr" runat="server" height="35px" class="rowEven">
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
            <hr id="hrPracticeAndACMgrSeperator" runat="server" visible="false" size="2" color="#888888"
                align="center" />
            <div style="overflow-x: auto;padding-right:1px;">
                <asp:Label ID="lblBDMEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                <asp:ListView ID="lvBDMBudget" runat="server" OnDataBinding="lvBudget_OnDataBinding"
                    OnItemDataBound="lvPersonBudget_OnItemDataBound" OnPreRender="lvBudget_OnPreRender"
                    OnSorting="lvBudget_OnSorting" OnSorted="lvBudget_Sorted">
                    <LayoutTemplate>
                        <table class="CompPerfTable WholeWidth">
                            <tr runat="server" id="lvHeader" class="CompPerfHeaderGroupBy">
                                <td align="center" style="width: 170px!important;">
                                    <div class="ie-bg" style="width: 170px!important;">
                                        <asp:LinkButton ID="btnSortBDM" CommandArgument="0" CommandName="Sort" runat="server"
                                            CssClass="arrow" Style="white-space: normal;">
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
                        <tr id="testTr" runat="server" height="35px">
                            <td style="width: 170px!important;">
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr id="testTr" runat="server" height="35px" class="rowEven">
                            <td style="width: 170px!important;">
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

