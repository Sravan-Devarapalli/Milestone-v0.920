<%@ Page Title="Practice Management - Projects Summary Group by Director" Language="C#"
    MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="GroupByDirector.aspx.cs"
    Inherits="PraticeManagement.GroupByDirector" %>

<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Projects Summary Group by Director</title>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript" src="Scripts/ScrollinDropDown.js"></script>
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
    </script>
    <style type="text/css">
        .ScrollDirector
        {
        }
        .ShowRepCheckBox input
        {
            float: left;
            width: 15px;
            text-align: left;
            margin-right: 0px;
        }
    </style>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
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
                    <td style="width: 100px; white-space: nowrap;" align="left">
                        Show Projects by
                    </td>
                    <td style="width: 130px;" class="ShowRepCheckBox" align="left">
                        <table>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkShowDirector" runat="server" Text="Director" AutoPostBack="false"
                                        ToolTip="Show Group By Director Report" Checked="true" onclick="ChangeResetButton();" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkShowPractice" runat="server" Text="Practice" AutoPostBack="false"
                                        ToolTip="Show Group By Practice Report" Checked="true" onclick="ChangeResetButton();" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkShowAccountManager" runat="server" Text="Account Manager" AutoPostBack="false"
                                        ToolTip="Show Group By Account Manager Report" Checked="true" onclick="ChangeResetButton();" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        from
                    </td>
                    <td style="width: 120px" align="left">
                        <asp:UpdatePanel ID="updFilters" runat="server">
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
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
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
                                        OnClientClick="this.disabled=true;Delete_Cookie('GroupByPersonFilterKey', '/', '');window.location.href = RemoveExtraCharAtEnd(window.location.href);return false;"
                                        CausesValidation="false" EnableViewState="False" CssClass="pm-button" />
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
                                <td style="width: 30px;">
                                </td>
                                <td style="width: 256px !important; border-bottom: 1px solid black;">
                                    Practice
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 110px;">
                                                <asp:CheckBox ID="chbActive" runat="server" AutoPostBack="false" Checked="True" onclick="EnableResetButton();"
                                                    Text="Active" ToolTip="Include active projects into report" />
                                            </td>
                                            <td style="width: 110px;">
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
                                            onclick="scrollingDropdown_onclick('cblPractice','Practice')" BackColor="White"
                                            CellPadding="3" Height="250px" NoItemsType="All" SetDirty="False" DropDownListType="Practice"
                                            Width="260px" BorderWidth="0" />
                                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractice"
                                            UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                        </ext:ScrollableDropdownExtender>
                                    </div>
                                    <asp:CheckBox ID="chbExcludeInternalPractices" runat="server" Text="Exclude Internal Practices"
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
                                </tr>
                                <tr>
                                    <td>
                                        <uc:CascadingMsdd ID="cblClient" runat="server" TargetControlId="cblProjectGroup"
                                            SetDirty="false" Width="170" Height="100" />
                                    </td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" Width="170"
                                            Height="100" />
                                    </td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblSalesperson" runat="server" CssClass="scroll-y" SetDirty="false"
                                            Width="170" Height="100" />
                                    </td>
                                    <td>
                                        <uc:ScrollingDropDown ID="cblProjectOwner" runat="server" CssClass="scroll-y" SetDirty="false"
                                            Width="170" Height="100" />
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
        <div style="overflow-x: auto; overflow-y: auto; max-height: 450px;" class="ScrollDirector">
            <asp:UpdatePanel ID="updGroupByDirector" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label ID="lblDirectorEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                    <asp:ListView ID="lvGroupByDirector" runat="server" OnItemDataBound="lvGroupByPerson_OnItemDataBound"
                        OnDataBinding="lvGroupByPerson_OnDataBinding" OnPreRender="lvGroupByPerson_OnPreRender"
                        OnSorting="PersonListView_OnSorting" OnSorted="lvGroupByPerson_Sorted">
                        <LayoutTemplate>
                            <table class="CompPerfTable WholeWidth">
                                <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                    <td align="center" style="width: 180px!important;">
                                        <div class="ie-bg" style="width: 180px!important;">
                                            <asp:LinkButton ID="btnSortDirector" CommandArgument="0" CommandName="Sort" runat="server"
                                                CssClass="arrow">Director</asp:LinkButton>
                                        </div>
                                    </td>
                                    <td align="center">
                                        <div class="ie-bg" style="width: 220px!important;">
                                            Client / Client-Group / Project</div>
                                    </td>
                                    <td align="center" style="width: 120px!important;">
                                        <div class="ie-bg" style="width: 120px!important;">
                                            Grand Total</div>
                                    </td>
                                </tr>
                                <tr runat="server" id="itemPlaceholder" />
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="testTr" runat="server" class="summary" height="35px">
                                <td>
                                </td>
                                <td>
                                </td>
                                <td align="right" style="padding-right: 8px;">
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
                    <%--</div>--%>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <asp:UpdatePanel ID="updGroupByPractice" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <hr id="hrDirectorAndPracticeSeperator" runat="server" visible="false" width="100%"
                    size="2" color="#888888" align="center" />
                    <asp:Label ID="lblPracticeEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                <div style="overflow-x: auto; overflow-y: auto; max-height: 450px;">
                    <asp:ListView ID="lvGroupByPractice" runat="server" OnItemDataBound="lvGroupByPractice_OnItemDataBound"
                        OnDataBinding="lvGroupByPerson_OnDataBinding" OnPreRender="lvGroupByPractice_OnPreRender"
                        OnSorting="PersonListView_OnSorting" OnSorted="lvGroupByPerson_Sorted">
                        <LayoutTemplate>
                            <table class="CompPerfTable WholeWidth">
                                <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                    <td align="center" style="width: 180px!important;">
                                        <div class="ie-bg" style="width: 180px!important;">
                                            <asp:LinkButton ID="btnSortPracticeManager" CommandArgument="0" CommandName="Sort"
                                                runat="server" CssClass="arrow">Practice (Practice Manager)</asp:LinkButton>
                                        </div>
                                    </td>
                                    <td align="center">
                                        <div class="ie-bg" style="width: 220px!important;">
                                            Client / Client-Group / Project</div>
                                    </td>
                                    <td align="center" style="width: 120px!important;">
                                        <div class="ie-bg" style="width: 120px;">
                                            Grand Total</div>
                                    </td>
                                </tr>
                                <tr runat="server" id="itemPlaceholder" />
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="testTr" runat="server" class="summary" height="35px">
                                <td>
                                </td>
                                <td>
                                </td>
                                <td align="right" style="padding-right: 8px;">
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
                <hr id="hrPracticeAndACMgrSeperator" runat="server" visible="false" width="100%"
                    size="2" color="#888888" align="center" />
                    <asp:Label ID="lblAccountManagerEmptyMessage" Visible="false" runat="server" Text="There is nothing to be displayed here."></asp:Label>
                <div style="overflow-x: auto; overflow-y: auto; max-height: 450px;">
                    <asp:ListView ID="lvGroupByAccountManager" runat="server" OnItemDataBound="lvGroupByPerson_OnItemDataBound"
                        OnDataBinding="lvGroupByPerson_OnDataBinding" OnPreRender="lvGroupByPerson_OnPreRender"
                        OnSorting="PersonListView_OnSorting" OnSorted="lvGroupByPerson_Sorted">
                        <LayoutTemplate>
                            <table class="CompPerfTable WholeWidth">
                                <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                    <td align="center" style="width: 180px!important;">
                                        <div class="ie-bg" style="width: 180px!important;">
                                            <asp:LinkButton ID="btnSortAccountManager" CommandArgument="0" CommandName="Sort"
                                                runat="server" CssClass="arrow">Account Manager</asp:LinkButton>
                                        </div>
                                    </td>
                                    <td align="center">
                                        <div class="ie-bg" style="width: 220px!important;">
                                            Client / Client-Group / Project</div>
                                    </td>
                                    <td align="center" style="width: 120px;">
                                        <div class="ie-bg" style="width: 120px;">
                                            Grand Total</div>
                                    </td>
                                </tr>
                                <tr runat="server" id="itemPlaceholder" />
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="testTr" runat="server" class="summary" height="35px">
                                <td>
                                </td>
                                <td>
                                </td>
                                <td align="right" style="padding-right: 5px;">
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
</asp:Content>

