<%@ Page Title="Projects Group by | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="GroupBy.aspx.cs" Inherits="PraticeManagement.GroupByDirector" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<asp:Content ID="ctrlhead" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Projects Group by | Practice Management</title>
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
        function RemoveExtraCharAtEnd(url) {
            if (url.lastIndexOf('#') == url.length - 1) {
                return url.substring(0, url.length - 1);
            }
            else {
                return url;
            }
        }

        function ExpandCollapseProjects(currentClientGrouprow, rows, actionName) {

            var ClientGroup = currentClientGrouprow.attributes["ClientGroup"].value;
            var Client = currentClientGrouprow.attributes["Client"].value;
            var Person = currentClientGrouprow.attributes["Person"].value;
            for (var index = 0; index < rows.length; index = index + 1) {
                if (rows[index].attributes["ClientGroup"] != null
                            && rows[index].attributes["Client"] != null
                            && rows[index].attributes["Person"] != null
                            && rows[index].attributes["ClientGroup"].value == ClientGroup
                            && rows[index].attributes["Client"].value == Client
                            && rows[index].attributes["Person"].value == Person
                            && rows[index] != currentClientGrouprow) {
                    if (actionName == "Collapse") {
                        rows[index].className = "hidden";
                    }
                    else {
                        rows[index].className = "";
                    }
                }

            }

        }

        function ExpandClientGroups(currentClientrow, rows) {

            for (var index = 0; index < rows.length; index = index + 1) {
                if (rows[index].attributes["Client"] != null
               && rows[index].attributes["Person"] != null
               && rows[index].attributes["Client"].value == currentClientrow.attributes["Client"].value
               && rows[index].attributes["Person"].value == currentClientrow.attributes["Person"].value
               && rows[index] != currentClientrow) {
                    if (rows[index].attributes["ClientGroup"] != null && rows[index].attributes["ExpandCollapseStatus"] != null) {
                        rows[index].className = "";
                        if (rows[index].attributes["ExpandCollapseStatus"].value == "Expand") {
                            ExpandCollapseProjects(rows[index], rows, "Expand");
                        }
                    }
                    else if (rows[index].attributes["ClientGroup"] == null) {
                        rows[index].className = "";
                    }
                }
            }
        }
        function ExpandClients(currentPersonRow, rows) {

            for (var index = 0; index < rows.length; index = index + 1) {
                if (rows[index].attributes["ClientGroup"] == null
               && rows[index].attributes["Client"] != null
               && rows[index].attributes["Person"] != null
               && rows[index].attributes["ExpandCollapseStatus"] != null
               && rows[index].attributes["Person"].value == currentPersonRow.attributes["Person"].value
               && rows[index] != currentPersonRow) {
                    rows[index].className = "";
                    if (rows[index].attributes["ExpandCollapseStatus"].value == "Expand") {
                        ExpandClientGroups(rows[index], rows);
                    }
                }
            }

        }
        function ExpandCollapseChilds(image) {
            image.className = "hidden";
            var actionName = image.attributes["Name"].value;
            if (actionName == "Collapse") {
                image.parentNode.children[1].className = "";
            }
            else {
                image.parentNode.children[0].className = "";
            }
            var currentrow = image.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
            if (currentrow.attributes["ExpandCollapseStatus"] != null) {
                currentrow.attributes["ExpandCollapseStatus"].value = actionName;
            }
            var rows = currentrow.parentNode.children;
            if (image.attributes["ClientGroup"] != null) { //client Group row
                ExpandCollapseProjects(currentrow, rows, actionName);
            }
            else if (image.attributes["Client"] != null) { //client row
                if (actionName == "Collapse") {
                    for (var index = 0; index < rows.length; index = index + 1) {

                        if (rows[index].attributes["Client"] != null
                            && rows[index].attributes["Person"] != null
                            && rows[index].attributes["Client"].value == currentrow.attributes["Client"].value
                            && rows[index].attributes["Person"].value == currentrow.attributes["Person"].value
                            && rows[index] != currentrow) {
                            rows[index].className = "hidden";
                        }
                    }

                }
                else {
                    ExpandClientGroups(currentrow, rows);
                }
            }
            else { //Director or Practice Manager Row

                if (actionName == "Collapse") {

                    for (var index = 0; index < rows.length; index = index + 1) {
                        if (rows[index].attributes["Person"] != null
                            && rows[index].attributes["Person"].value == currentrow.attributes["Person"].value
                            && rows[index] != currentrow) {
                            rows[index].className = "hidden";
                        }
                    }
                }
                else {
                    ExpandClients(currentrow, rows);
                }

            }
        }
        function SetDirectorReportHeight() {
            var div = $get("<%=updGroupByDirector.ClientID %>").children[0];
            if (div != null) {
                div.style.maxHeight = div.scrollHeight + 320 + "px";
            }
        }
        function SetAccountmanagerReportHeight() {

            var updGroupByAccountManager = $get("<%=updGroupByAccountManager.ClientID %>");
            var div = updGroupByAccountManager.children[updGroupByAccountManager.children.length - 1];
            if (div != null) {
                div.style.maxHeight = div.scrollHeight + 320 + "px";
            }
        }
        function SetPracticeReportHeight() {
            var updGroupByPractice = $get("<%=updGroupByPractice.ClientID %>");
            var div = updGroupByPractice.children[updGroupByPractice.children.length - 1];
            if (div != null) {
                div.style.maxHeight = div.scrollHeight + 320 + "px";
            }
        }
    </script>
    <uc:loadingprogress id="LoadingProgress1" runat="server" />
    <div class="filters Margin-Bottom10Px">
        <div class="buttons-block">
            <table class="WholeWidth">
                <tr>
                    <td class="textLeft Width30Px PaddingTop3">
                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                            ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/expand.jpg"
                            ToolTip="Expand Filters" />
                    </td>
                    <td class="textLeft Width100Px no-wrap">
                        Show Projects by
                    </td>
                    <td class="ShowRepCheckBox textLeft Width150px">
                        <table>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkShowDirector" runat="server" Text="Executive in Charge" AutoPostBack="false"
                                        ToolTip="Show Group By Executive in Charge Report" Checked="true" onclick="ChangeResetButton();" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkShowPractice" runat="server" Text="Practice Area" AutoPostBack="false"
                                        ToolTip="Show Group By Practice Area Report" Checked="true" onclick="ChangeResetButton();" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkShowAccountManager" runat="server" Text="Business Development Manager"
                                        AutoPostBack="false" ToolTip="Show Group By Business Development Manager Report"
                                        Checked="true" onclick="ChangeResetButton();" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="PaddingRight10Px textRightImp">
                        from
                    </td>
                    <td class="textLeft Width120Px">
                        <asp:UpdatePanel ID="updFilters" runat="server">
                            <ContentTemplate>
                                <uc:monthpicker id="mpFromControl" runat="server" onclientchange="EnableResetButton();"
                                    onselectedvaluechanged="mpFromControl_OnSelectedValueChanged" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td class="textRightImp Width30Px PaddingRight10Px">
                        to
                    </td>
                    <td class="textLeft Width120Px">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <uc:monthpicker id="mpToControl" runat="server" onclientchange="EnableResetButton();"
                                    onselectedvaluechanged="mpToControl_OnSelectedValueChanged" />
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
                                    <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" OnClientClick="this.disabled=true;window.location.href = RemoveExtraCharAtEnd(window.location.href);return false;"
                                        CausesValidation="false" EnableViewState="False" CssClass="Width100PxImp" />
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
                            <tr class="TextAlignCenterImp">
                                <td class="TdProjectType">
                                    Project Type
                                </td>
                                <td class="Width30Px">
                                </td>
                                <td class="TdPracticeArea">
                                    Practice Area
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table class="WholeWidth">
                                        <tr>
                                            <td class="Width110Px">
                                                <asp:CheckBox ID="chbActive" runat="server" AutoPostBack="false" Checked="True" onclick="EnableResetButton();"
                                                    Text="Active" ToolTip="Include active projects into report" />
                                            </td>
                                            <td class="Width110Px">
                                                <asp:CheckBox ID="chbInternal" runat="server" AutoPostBack="false" Checked="True"
                                                    onclick="EnableResetButton();" Text="Internal" ToolTip="Include internal projects into report" />
                                            </td>
                                            <td class="Width110Px">
                                                <asp:CheckBox ID="chbInactive" runat="server" AutoPostBack="false" Checked="false"
                                                    onclick="EnableResetButton();" Text="Inactive" ToolTip="Include Inactive projects into report" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chbProposed" runat="server" AutoPostBack="false" Checked="false"
                                                    onclick="EnableResetButton();" Text="Proposed" ToolTip="Include Proposed projects into report" />
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
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                                <td>
                                    <div class="PaddingTop5 padLeft3">
                                        <cc2:ScrollingDropDown ID="cblPractice" runat="server" AllSelectedReturnType="AllItems"
                                            onclick="scrollingDropdown_onclick('cblPractice','Practice Area')" CellPadding="3"
                                            NoItemsType="All" SetDirty="False" DropDownListType="Practice Area" CssClass="GroupBySddPractice" />
                                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractice"
                                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                        </ext:ScrollableDropdownExtender>
                                    </div>
                                    <asp:CheckBox ID="chbExcludeInternalPractices" runat="server" Text="Exclude Internal Practice Areas"
                                        onclick="EnableResetButton();" />
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="tpGranularity">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Advanced</span></a> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="project-filter">
                            <table class="WholeWidth">
                                <tr class="tb-header">
                                    <td>
                                        Account
                                    </td>
                                    <td>
                                        Business Unit
                                    </td>
                                    <td>
                                        Salesperson
                                    </td>
                                    <td>
                                        Project Access
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <uc:cascadingmsdd id="cblClient" runat="server" targetcontrolid="cblProjectGroup"
                                            setdirty="false" cssclass="AdvancedTabUc" />
                                    </td>
                                    <td>
                                        <uc:scrollingdropdown id="cblProjectGroup" runat="server" setdirty="false" cssclass="AdvancedTabUc" />
                                    </td>
                                    <td>
                                        <uc:scrollingdropdown id="cblSalesperson" runat="server" cssclass="scroll-y AdvancedTabUc"
                                            setdirty="false" />
                                    </td>
                                    <td>
                                        <uc:scrollingdropdown id="cblProjectOwner" runat="server" cssclass="scroll-y AdvancedTabUc"
                                            setdirty="false" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </asp:Panel>
        <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
        <asp:HiddenField ID="hdnFiltersChangedSinceLastUpdate" runat="server" Value="false" />
        <asp:UpdatePanel ID="updGroupByDirector" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="overflowAuto">
                    <asp:Label ID="lblDirectorEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                    <asp:ListView ID="lvGroupByDirector" runat="server" OnItemDataBound="lvGroupByPerson_OnItemDataBound"
                        OnDataBinding="lvGroupByPerson_OnDataBinding" OnPreRender="lvGroupByPerson_OnPreRender"
                        OnSorting="PersonListView_OnSorting" OnSorted="lvGroupByPerson_Sorted">
                        <LayoutTemplate>
                            <table class="CompPerfTable WholeWidth">
                                <tr runat="server" id="lvHeader" class="CompPerfHeaderGroupBy">
                                    <td class="TextAlignCenterImp Width170PxImp">
                                        <div class="ie-bg Width170PxImp">
                                            <asp:LinkButton ID="btnSortDirector" CommandArgument="0" CommandName="Sort" runat="server"
                                                CssClass="arrow">Executive in Charge</asp:LinkButton>
                                        </div>
                                    </td>
                                    <td class="TextAlignCenterImp Width180PxImp">
                                        <div class="ie-bg Width180PxImp">
                                            Account / Business Unit / Project</div>
                                    </td>
                                    <td class="MonthSummary TextAlignCenterImp">
                                        <div class="ie-bg">
                                            Grand Total</div>
                                    </td>
                                </tr>
                                <tr runat="server" id="itemPlaceholder" />
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="testTr" runat="server" class="summary Height35Px">
                                <td>
                                </td>
                                <td>
                                </td>
                                <td class="PaddingRight3Px" align="right">
                                </td>
                            </tr>
                        </ItemTemplate>
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
        <asp:UpdatePanel ID="updGroupByPractice" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <hr id="hrDirectorAndPracticeSeperator" runat="server" visible="false" size="2" class="TextAlignCenterImp Color888888" />
                <div class="overflowAuto">
                    <asp:Label ID="lblPracticeEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                    <asp:ListView ID="lvGroupByPractice" runat="server" OnItemDataBound="lvGroupByPractice_OnItemDataBound"
                        OnDataBinding="lvGroupByPerson_OnDataBinding" OnPreRender="lvGroupByPractice_OnPreRender"
                        OnSorting="PersonListView_OnSorting" OnSorted="lvGroupByPerson_Sorted">
                        <LayoutTemplate>
                            <table class="CompPerfTable WholeWidth">
                                <tr runat="server" id="lvHeader" class="CompPerfHeaderGroupBy">
                                    <td class="TextAlignCenterImp Width170PxImp">
                                        <div class="ie-bg Width170PxImp">
                                            <asp:LinkButton ID="btnSortPracticeManager" CommandArgument="0" CommandName="Sort"
                                                runat="server" CssClass="arrow WhiteSpaceNormal">Practice Area<br />(Practice Area Manager)</asp:LinkButton>
                                        </div>
                                    </td>
                                    <td class="TextAlignCenterImp Width180PxImp">
                                        <div class="ie-bg Width180PxImp WhiteSpaceNormal">
                                            Account / Business Unit / Project</div>
                                    </td>
                                    <td class="MonthSummary TextAlignCenterImp">
                                        <div class="ie-bg">
                                            Grand Total</div>
                                    </td>
                                </tr>
                                <tr runat="server" id="itemPlaceholder" />
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="testTr" runat="server" class="summary Height35Px">
                                <td>
                                </td>
                                <td>
                                </td>
                                <td class="PaddingRight3Px" align="right">
                                </td>
                            </tr>
                        </ItemTemplate>
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
        <asp:UpdatePanel ID="updGroupByAccountManager" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <hr id="hrPracticeAndACMgrSeperator" runat="server" visible="false" size="2" class="TextAlignCenterImp Width100Per Color888888" />
                <div class="overflowAuto">
                    <asp:Label ID="lblAccountManagerEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                    <asp:ListView ID="lvGroupByAccountManager" runat="server" OnItemDataBound="lvGroupByPerson_OnItemDataBound"
                        OnDataBinding="lvGroupByPerson_OnDataBinding" OnPreRender="lvGroupByPerson_OnPreRender"
                        OnSorting="PersonListView_OnSorting" OnSorted="lvGroupByPerson_Sorted">
                        <LayoutTemplate>
                            <table class="CompPerfTable WholeWidth">
                                <tr runat="server" id="lvHeader" class="CompPerfHeaderGroupBy">
                                    <td class="TextAlignCenterImp Width170PxImp">
                                        <div class="ie-bg Width170PxImp">
                                            <asp:LinkButton ID="btnSortAccountManager" CommandArgument="0" CommandName="Sort"
                                                runat="server" CssClass="arrow WhiteSpaceNormal">Business Development Manager</asp:LinkButton>
                                        </div>
                                    </td>
                                    <td class="TextAlignCenterImp Width180PxImp">
                                        <div class="ie-bg  Width180PxImp WhiteSpaceNormal">
                                            Account / Business Unit / Project</div>
                                    </td>
                                    <td class="MonthSummary TextAlignCenterImp">
                                        <div class="ie-bg">
                                            Grand Total</div>
                                    </td>
                                </tr>
                                <tr runat="server" id="itemPlaceholder" />
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="testTr" runat="server" class="summary Height35Px">
                                <td>
                                </td>
                                <td>
                                </td>
                                <td class="PaddingRight3Px" align="right">
                                </td>
                            </tr>
                        </ItemTemplate>
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
    </div>
    <AjaxControlToolkit:UpdatePanelAnimationExtender ID="uaeGroupByDirector" runat="server"
        TargetControlID="updGroupByDirector">
        <Animations>
        <OnUpdating>
            <Sequence>
                <ScriptAction Script="" />
            </Sequence>
        </OnUpdating>
        <OnUpdated>
            <Sequence>
                <ScriptAction Script="SetDirectorReportHeight();" />
            </Sequence>
        </OnUpdated>
        </Animations>
    </AjaxControlToolkit:UpdatePanelAnimationExtender>
    <AjaxControlToolkit:UpdatePanelAnimationExtender ID="uaeGroupByPractice" runat="server"
        TargetControlID="updGroupByPractice">
        <Animations>
        <OnUpdating>
            <Sequence>
                <ScriptAction Script="" />
            </Sequence>
        </OnUpdating>
        <OnUpdated>
            <Sequence>
                <ScriptAction Script="SetPracticeReportHeight();" />
            </Sequence>
        </OnUpdated>
        </Animations>
    </AjaxControlToolkit:UpdatePanelAnimationExtender>
    <AjaxControlToolkit:UpdatePanelAnimationExtender ID="uaeGroupByAccountManager" runat="server"
        TargetControlID="updGroupByAccountManager">
        <Animations>
        <OnUpdating>
            <Sequence>
                <ScriptAction Script="" />
            </Sequence>
        </OnUpdating>
        <OnUpdated>
            <Sequence>
                <ScriptAction Script="SetAccountmanagerReportHeight();" />
            </Sequence>
        </OnUpdated>
        </Animations>
    </AjaxControlToolkit:UpdatePanelAnimationExtender>
</asp:Content>

