<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ProjectSearch.aspx.cs" Inherits="PraticeManagement.ProjectSearch"
    Title="Project Search Results | Practice Management" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ PreviousPageType TypeName="PraticeManagement.Controls.PracticeManagementSearchPageBase" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Project Search Results | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Search Results
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <style type="text/css">
        .collapsePanel .colPanel
        {
            display: none;
        }
        
        .collapsePanel .chevron
        {
            background-image: url("Images/expand.jpg");
        }
        
        .chevron
        {
            background: url("Images/collapse.jpg") no-repeat;
            cursor: pointer;
            padding-left: 10px;
            display: inline;
        }
        
        .FltPanel .panel
        {
            display: none;
        }
        
        .FilterDiv .Filter
        {
            background-image: url("Images/expand.jpg");
        }
        
        .Filter
        {
            background: url("Images/collapse.jpg") no-repeat;
            cursor: pointer;
            padding-left: 10px;
            display: inline;
        }
    </style>
    <script src="Scripts/jquery-1.4.1.yui.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">

        function setPosition(item, ytop, xleft) {
            item.offset({ top: ytop, left: xleft });
        }

        function SetTooltipText(descriptionText, hlinkObj) {
            var hlinkObjct = $(hlinkObj);
            var displayPanel = $('#<%= pnlProjectToolTipHolder.ClientID %>');
            iptop = hlinkObjct.offset().top;
            ipleft = hlinkObjct.offset().left + hlinkObjct[0].offsetWidth + 5;
            setPosition(displayPanel, iptop - 20, ipleft);
            displayPanel.show();
            setPosition(displayPanel, iptop - 20, ipleft);
            displayPanel.show();

            var lblProjectTooltip = document.getElementById('<%= lblProjectTooltip.ClientID %>');
            lblProjectTooltip.innerHTML = descriptionText.toString();
        }


        function HidePanel() {
            var displayPanel = $('#<%= pnlProjectToolTipHolder.ClientID %>');
            displayPanel.hide();
        }

        function EnableOrDisableGroup() {
            var cbl = document.getElementById("<%= cblClient.ClientID %>");
            var arrayOfCheckBoxes = cbl.getElementsByTagName("input");
            var cblList = $("div[id^='sdeCblProjectGroup']");
            var anySingleItemChecked = "false";
            for (var i = 0; i < arrayOfCheckBoxes.length; i++) {
                if (arrayOfCheckBoxes[i].checked) {
                    anySingleItemChecked = "true";
                }
            }

            if (anySingleItemChecked == "true") {
                cblList[0].disabled = "";
            }
            else {
                cblList[0].disabled = "disabled";
            }
            custom_ScrollingDropdown_onclick('cblProjectGroup', 'Business Unit');
        }

        function custom_ScrollingDropdown_onclick(control, type) {
            var temp = 0;
            var text = "";
            var scrollingDropdownList = document.getElementById(control.toString());
            var arrayOfCheckBoxes = scrollingDropdownList.getElementsByTagName("input");
            if (arrayOfCheckBoxes.length == 1 && arrayOfCheckBoxes[0].disabled) {
                text = "No " + type.toString() + "s to select.";
            }
            else {
                for (var i = 0; i < arrayOfCheckBoxes.length; i++) {

                    if (arrayOfCheckBoxes[i].checked) {
                        if (arrayOfCheckBoxes[i].parentNode.parentNode.style.display != "none") {
                            temp++;
                            text = arrayOfCheckBoxes[i].parentNode.childNodes[1].innerHTML;
                        }
                    }
                    if (temp > 1) {
                        text = "Multiple " + type.toString() + "s selected";

                    }
                    if (arrayOfCheckBoxes[0].checked) {
                        text = arrayOfCheckBoxes[0].parentNode.childNodes[1].innerHTML;
                    }
                    if (temp === 0) {
                        text = "Select " + type.toString() + "(s)";
                    }
                }
                if (text.length > 32) {
                    text = text.substr(0, 30) + "..";
                }
                scrollingDropdownList.parentNode.children[1].children[0].firstChild.nodeValue = text;
            }
        }

        function resetFiltersTab() {
            GetDefaultcblList();
            resetProjectTypes();
            GetDefault(document.getElementById("<%= cblClient.ClientID %>"));
            GetDefault(document.getElementById("<%= cblSalesperson.ClientID %>"));
            GetDefault(document.getElementById("<%= cblPractice.ClientID %>"));
            GetDefault(document.getElementById("<%= cblProjectOwner.ClientID %>"));

            var scrollingDropdownList = document.getElementById('cblProjectGroup');
            var arrayOfTRs = scrollingDropdownList.getElementsByTagName("tr");
            for (var i = 0; i < arrayOfTRs.length; i++) {
                arrayOfTRs[i].removeAttribute('class');
            }

            custom_ScrollingDropdown_onclick('cblProjectGroup', 'Business Unit');

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

        function resetProjectTypes() {
            document.getElementById("<%= chbActive.ClientID %>").checked = true;
            document.getElementById("<%= chbInternal.ClientID %>").checked = true;
            document.getElementById("<%= chbProjected.ClientID %>").checked = true;
            document.getElementById("<%= chbCompleted.ClientID %>").checked = true;
            document.getElementById("<%= chbExperimental.ClientID %>").checked = true;
            document.getElementById("<%= chbProposed.ClientID %>").checked = true;
            document.getElementById("<%= chbAtRisk.ClientID %>").checked = true;
            document.getElementById("<%= chbInactive.ClientID %>").checked = false;
        }

        $(document).ready(function () {
            $(".collapseExpandEnable .chevron").live("click", function () {
                $(this).parent().toggleClass("collapsePanel");
            });


            $(".FilterPanel .Filter").live("click", function () {
                $(this).parent().toggleClass("FilterDiv");
                $(".FilterPanel .panel").parent().toggleClass("FltPanel", 20000);
            });

            if ($(".collapseExpandEnable .chevron").length > 0) {
                $(".expandAll").show().click(function () {
                    var btnValue = $(this).val();
                    if (btnValue == "Expand All") {
                        $(this).val("Collapse All");
                        $(".collapseExpandEnable .chevron").parent().removeClass("collapsePanel");
                    }
                    else {
                        $(this).val("Expand All");
                        $(".collapseExpandEnable .chevron").parent().addClass("collapsePanel");
                    }
                    return false;
                });
            }
            else {
                $(".expandAll").hide();
            }
        });
    </script>
    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch" class="FilterPanel">
        <div class="project-filter DivProjectFilter ">
            <table class="WholeWidth ">
                <tbody>
                    <tr>
                        <td class="Width3Percent FilterDiv">
                            <div class="Filter">
                                &nbsp;
                            </div>
                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        </td>
                        <td class="padRight8 width60PImp">
                            <asp:TextBox ID="txtSearchText" runat="server" CssClass="WholeWidth" MaxLength="255">
                            </asp:TextBox>
                        </td>
                        <td class="Width5Percent">
                        </td>
                        <td>
                            <table class="Height80Px" cellpadding="5">
                                <tr class="tb-header">
                                    <td colspan="3" class="ProjectSummaryProjectTypeTd">
                                        Project Status
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ProjectSummaryCheckboxTd">
                                        <asp:CheckBox ID="chbActive" runat="server" Text="Active" Checked="True" EnableViewState="False" />
                                    </td>
                                    <td class="ProjectSummaryCheckboxTd">
                                        <asp:CheckBox ID="chbInternal" runat="server" Text="Internal" Checked="True" EnableViewState="False" />
                                    </td>
                                    <td class="ProjectSummaryCheckboxTd">
                                        <asp:CheckBox ID="chbProposed" runat="server" Text="Proposed" Checked="True" EnableViewState="False" />
                                    </td>
                                </tr>
                                <tr class="PadLeft10Td">
                                    <td>
                                        <asp:CheckBox ID="chbProjected" runat="server" Text="Projected" Checked="True" EnableViewState="False" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbInactive" runat="server" Text="Inactive" EnableViewState="False" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbAtRisk" runat="server" Text="At Risk" EnableViewState="False"
                                            Checked="true" />
                                    </td>
                                </tr>
                                <tr class="PadLeft10Td">
                                    <td>
                                        <asp:CheckBox ID="chbCompleted" runat="server" Text="Completed" Checked="True" EnableViewState="False" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbExperimental" runat="server" Text="Experimental" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="Width10Percent" style="text-align: center">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click"
                                CssClass="Width100Px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td colspan="3">
                            <asp:ValidationSummary ID="vsumProjectSearch" runat="server" EnableClientScript="false" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="FltPanel">
            <asp:Panel ID="pnlFilters" runat="server" class="panel">
                <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                    CssClass="CustomTabStyle">
                    <AjaxControlToolkit:TabPanel ID="tpAdvancedFilters" runat="server">
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>Filters</span></a> </span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <div id="divProjectFilter" runat="server">
                                <table class="WholeWidth Height80Px">
                                    <tr class="tb-header ProjectSummaryAdvancedFiltersHeader">
                                        <th>
                                            Account / Business Unit
                                        </th>
                                        <td class="Padding5 Width10Px">
                                        </td>
                                        <th>
                                            Sales Team
                                        </th>
                                        <td class="Padding5 Width10Px">
                                        </td>
                                        <th>
                                            Division / Practice Area
                                        </th>
                                        <td class="Padding5 Width10Px">
                                        </td>
                                        <th>
                                            Revenue Type / Offering
                                        </th>
                                        <td class="Padding5 Width10Px">
                                        </td>
                                        <th>
                                            Channel
                                        </th>
                                        <td class="Padding5 Width10Px">
                                        </td>
                                        <td rowspan="3">
                                        </td>
                                        <td rowspan="3">
                                            <table class="textRight WholeWidth">
                                                <tr>
                                                    <td>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="PaddingTop10" style="padding-right: 40px">
                                                        <asp:Button ID="btnReset" runat="server" Text="Reset" CausesValidation="False" OnClientClick="resetFiltersTab();  return false;"
                                                            EnableViewState="False" CssClass="Width100Px" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="Padding5">
                                            <pmc:CascadingMsdd ID="cblClient" runat="server" TargetControlId="cblProjectGroup"
                                                SetDirty="false" CssClass="ProjectSummaryScrollingDropDown" onclick="scrollingDropdown_onclick('cblClient','Account');EnableOrDisableGroup();"
                                                DropDownListType="Account" />
                                            <ext:ScrollableDropdownExtender ID="sdeCblClient" runat="server" TargetControlID="cblClient"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <pmc:ScrollingDropDown ID="cblSalesperson" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                onclick="scrollingDropdown_onclick('cblSalesperson','Salesperson')" DropDownListType="Salesperson" />
                                            <ext:ScrollableDropdownExtender ID="sdeCblSalesperson" runat="server" TargetControlID="cblSalesperson"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <pmc:ScrollingDropDown ID="cblDivision" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                onclick="scrollingDropdown_onclick('cblDivision','Division')" DropDownListType="Division" />
                                            <ext:ScrollableDropdownExtender ID="sdecblDivision" runat="server" TargetControlID="cblDivision"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <pmc:ScrollingDropDown ID="cblRevenueType" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                onclick="scrollingDropdown_onclick('cblRevenueType','RevenueType')" DropDownListType="Revenue Type" />
                                            <ext:ScrollableDropdownExtender ID="sdecblRevenueType" runat="server" TargetControlID="cblRevenueType"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <pmc:ScrollingDropDown ID="cblChannel" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                onclick="scrollingDropdown_onclick('cblChannel','Channel')" DropDownListType="Channel" />
                                            <ext:ScrollableDropdownExtender ID="sdecblChannel" runat="server" TargetControlID="cblChannel"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="Padding5">
                                            <pmc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                onclick="custom_ScrollingDropdown_onclick('cblProjectGroup','Business Unit')"
                                                DropDownListType="Business Unit" />
                                            <ext:ScrollableDropdownExtender ID="sdeCblProjectGroup" runat="server" TargetControlID="cblProjectGroup"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <pmc:ScrollingDropDown ID="cblProjectOwner" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                onclick="scrollingDropdown_onclick('cblProjectOwner','Project Access','es','Project Accesses',33);"
                                                DropDownListType="Project Access" DropDownListTypePluralForm="Project Accesses"
                                                PluralForm="es" />
                                            <ext:ScrollableDropdownExtender ID="sdeCblProjectOwner" runat="server" TargetControlID="cblProjectOwner"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <pmc:ScrollingDropDown ID="cblPractice" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                onclick="scrollingDropdown_onclick('cblPractice','Practice Area')" DropDownListType="Practice Area" />
                                            <ext:ScrollableDropdownExtender ID="sdeCblPractice" runat="server" TargetControlID="cblPractice"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <pmc:ScrollingDropDown ID="cblOffering" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                onclick="scrollingDropdown_onclick('cblOffering','Offering')" DropDownListType="Offering" />
                                            <ext:ScrollableDropdownExtender ID="sdecblOffering" runat="server" TargetControlID="cblOffering"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                    </tr>
                                </table>
                            </div>
                        </ContentTemplate>
                    </AjaxControlToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>
            </asp:Panel>
        </div>
        <asp:Button ID="ExpandAll" runat="server" Text="Expand All" class="expandAll"></asp:Button>
        <br />
        <br />
        <asp:ListView ID="lvProjects" runat="server" DataKeyNames="Id" OnItemDataBound="lvProjects_ItemDataBound">
            <LayoutTemplate>
                <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth AddLeftPadding collapseExpandEnable">
                    <tr runat="server" id="lvHeader" class="CompPerfHeader">
                        <td class="CompPerfProjectState">
                            <div>
                            </div>
                        </td>
                        <td class="CompPerfProjectState AddLeftPadding">
                            <div class="ie-bg">
                                Project #</div>
                        </td>
                        <td class="CompPerfProjectNumber AddLeftPadding">
                            <div class="ie-bg">
                                Account</div>
                        </td>
                        <td class="CompPerfClient AddLeftPadding">
                            <div class="ie-bg">
                                Project Name
                            </div>
                        </td>
                        <td class="CompPerfProject AddLeftPadding">
                            <div class="ie-bg">
                                PO Number
                            </div>
                        </td>
                        <td class="CompPerfPeriod AddLeftPadding">
                            <div class="ie-bg">
                                Project Start Date
                            </div>
                        </td>
                        <td class="CompPerfPeriod AddLeftPadding">
                            <div class="ie-bg">
                                Project End Date
                            </div>
                        </td>
                    </tr>
                    <tbody>
                        <tr runat="server" id="itemPlaceholder" />
                    </tbody>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server" id="boundingRow" class="MinHeight25Px vTop">
                    <td class="PaddingTop4Px">
                    </td>
                    <td class="CompPerfProjectState AddLeftPadding">
                        <asp:LinkButton ID="btnProjectNumber" runat="server" Text='<%# HighlightFound(Eval("ProjectNumber")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProjectNumber AddLeftPadding">
                        <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# HighlightFound(Eval("Client.HtmlEncodedName")) %>'
                            CommandArgument='<%# Eval("Client.Id") %>' OnCommand="btnClientName_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfClient AddLeftPadding">
                        <div class="secondary collapsePanel">
                            <div class="<%# (bool)Eval("HasMilestones") ? "chevron" : "hide" %>">
                                &nbsp;
                            </div>
                            <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("HtmlEncodedName")) %>'
                                CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                            <div class="colPanel">
                                <asp:Panel ID="pnlMilestones" runat="server" CssClass="padLeft30">
                                    <asp:DataList ID="dtlProposedPersons" runat="server">
                                        <ItemTemplate>
                                            <div class="DivMileStoneNames">
                                                <asp:LinkButton ID="btnMilestoneNames" runat="server" CssClass="height10Px" Text='<%# HighlightFound(Eval("HtmlEncodedDescription")) %>'
                                                    CommandArgument='<%# string.Concat(Eval("Id"), "_", Eval("Project.Id")) %>' OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </asp:Panel>
                            </div>
                        </div>
                    </td>
                    <td class="CompPerfProject AddLeftPadding">
                        <asp:LinkButton ID="btnPONumber" runat="server" Text='<%# Eval("PONumber") %>' CommandArgument='<%# Eval("Id") %>'
                            OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr runat="server" id="boundingRow" class="rowEven MinHeight20Px vTop">
                    <td class="PaddingTop4Px">
                    </td>
                    <td class="CompPerfProjectState AddLeftPadding">
                        <asp:LinkButton ID="btnProjectNumber" runat="server" Text='<%# HighlightFound(Eval("ProjectNumber")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProjectNumber AddLeftPadding">
                        <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# HighlightFound(Eval("Client.HtmlEncodedName")) %>'
                            CommandArgument='<%# Eval("Client.Id") %>' OnCommand="btnClientName_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfClient AddLeftPadding">
                        <div class="secondary collapsePanel">
                            <div class="<%# (bool)Eval("HasMilestones") ? "chevron" : "hide" %>">
                                &nbsp;
                            </div>
                            <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("HtmlEncodedName")) %>'
                                CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                            <div class="colPanel">
                                <asp:Panel ID="pnlMilestones" runat="server" CssClass="padLeft30">
                                    <asp:DataList ID="dtlProposedPersons" runat="server">
                                        <ItemTemplate>
                                            <div class="DivMileStoneNames">
                                                <asp:LinkButton ID="btnMilestoneNames" runat="server" CssClass="height10Px" Text='<%# HighlightFound(Eval("HtmlEncodedDescription")) %>'
                                                    CommandArgument='<%# string.Concat(Eval("Id"), "_", Eval("Project.Id")) %>' OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </asp:Panel>
                            </div>
                        </div>
                    </td>
                    <td class="CompPerfProject AddLeftPadding">
                        <asp:LinkButton ID="btnPONumber" runat="server" Text='<%# Eval("PONumber") %>' CommandArgument='<%# Eval("Id") %>'
                            OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                </tr>
            </AlternatingItemTemplate>
            <EmptyDataTemplate>
                <tr runat="server" id="EmptyDataRow">
                    <td>
                        No projects found.
                    </td>
                </tr>
            </EmptyDataTemplate>
        </asp:ListView>
    </asp:Panel>
    <asp:Panel ID="pnlProjectToolTipHolder" runat="server" CssClass="ToolTip WordWrap PanelOppNameToolTipHolder">
        <table>
            <tr class="top">
                <td class="lt">
                    <div class="tail">
                    </div>
                </td>
                <td class="tbor">
                </td>
                <td class="rt">
                </td>
            </tr>
            <tr class="middle">
                <td class="lbor">
                </td>
                <td class="content WordWrap">
                    <asp:Label ID="lblProjectTooltip" CssClass="WordWrap" runat="server"></asp:Label>
                </td>
                <td class="rbor">
                </td>
            </tr>
            <tr class="bottom">
                <td class="lb">
                </td>
                <td class="bbor">
                </td>
                <td class="rb">
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

