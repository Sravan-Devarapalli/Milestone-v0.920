<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UTilTimelineFilter.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.UTilTimeLineFilter" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<script type="text/javascript">
    function ChangeSortByRadioButtons(sender) {
        var sortby = document.getElementById("<%= ddlSortBy.ClientID%>");
        if (sortby.selectedIndex > 1) {
            sender.checked = false;
        }
    }
    function EnableResetButton() {
        var button = document.getElementById("<%= btnResetFilter.ClientID%>");
        var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
        if (button != null) {
            button.disabled = false;
            hiddenField.value = "true";
        }
    }
    function EnableDisableRadioButtons() {

        var sortby = document.getElementById("<%= ddlSortBy.ClientID%>");
        var rbAsc = document.getElementById("<%= rbSortbyAsc.ClientID%>");
        var rbDesc = document.getElementById("<%= rbSortbyDesc.ClientID%>");
        if (sortby.selectedIndex == 0) {
            rbAsc.checked = false;
            rbDesc.checked = true;
        }
        else if (sortby.selectedIndex == 1) {
            rbAsc.checked = true;
            rbDesc.checked = false;
        }
        else if (sortby.selectedIndex == 2) {
            rbAsc.checked = false;
            rbDesc.checked = false;
        }
        else {
            rbAsc.checked = false;
            rbDesc.checked = false;
        }
    }
</script>
<script language="javascript" type="text/javascript" src="../../Scripts/jquery-1.4.1.js"></script>
<div class="filters" style="margin-bottom: 10px;">
    <div class="buttons-block">
        <table class="WholeWidth">
            <tr>
                <td align="left" style="width: 20px">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                        ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                        ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                    <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                    <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                        ToolTip="Expand Filters and Sort Options" />
                </td>
                <td align="left" style="width: 90px;" align="left">
                    <asp:Label ID="lblUtilizationFrom" runat="server" Text="Show Utilization"></asp:Label>
                </td>
                <td style="width: 45px; padding-right: 4px" align="right">
                    <asp:Label ID="Label1" runat="server" Text="from "></asp:Label>
                </td>
                <td style="width: 115px;" align="left">
                    <uc:MonthPicker ID="mpFromControl" OnSelectedValueChanged="mpFromControl_OnSelectedValueChanged"  
                        runat="server" />
                </td>
                <td style="width: 15px;" align="left">
                    <asp:Label ID="lblT0" runat="server" Text="to "></asp:Label>
                </td>
                <td style="width: 115px;" align="left">
                    <uc:MonthPicker ID="mpToControl" runat="server" OnClientChange="EnableResetButton();"
                        OnSelectedValueChanged="mpToControl_OnSelectedValueChanged" />
                </td>
                <td style="width: 15px;" align="left">
                    <asp:Label ID="lblBy" runat="server" Text="by "></asp:Label>
                </td>
                <td style="width: 85px;" align="left">
                    <asp:DropDownList ID="ddlDetalization" runat="server" AutoPostBack="false" onchange="EnableResetButton();">
                        <asp:ListItem Value="1">1 Day</asp:ListItem>
                        <asp:ListItem Selected="True" Value="7">1 Week</asp:ListItem>
                        <asp:ListItem Value="14">2 Weeks</asp:ListItem>
                        <asp:ListItem Value="30">1 Month</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td style="width: 160px;" align="left">
                    <asp:Label ID="lblUtilization" runat="server" Text="  where U% is "></asp:Label>
                    <asp:DropDownList ID="ddlAvgUtil" runat="server" AutoPostBack="false" onchange="EnableResetButton();">
                        <asp:ListItem Value="2147483647">0 - 106+</asp:ListItem>
                        <asp:ListItem Value="106">&lt; 106+</asp:ListItem>
                        <asp:ListItem Value="90">&lt; 90</asp:ListItem>
                        <asp:ListItem Value="50">&lt; 50</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td align="right">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnUpdateView" runat="server" Text="Update View" Width="100px" OnClick="btnUpdateView_OnClick"
                                    EnableViewState="False" />
                            </td>
                            <td>
                                <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" Width="100px"
                                    OnClick="btnResetFilter_OnClick" />
                            </td>
                            <td>
                                <asp:Button ID="btnSaveReport" runat="server" Text="Save Report" Width="100px" OnClick="btnSaveReport_OnClick"
                                    EnableViewState="False" />
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
                    <span class="bg"><a href="#"><span>Filters</span></a> </span>
                </HeaderTemplate>
                <ContentTemplate>
                    <table class="WholeWidth">
                        <tr align="center">
                            <td style="width: 125px; border-bottom: 1px solid black;" valign="top">
                                Person Status
                            </td>
                            <td style="width: 30px;">
                            </td>
                            <td style="width: 150px; border-bottom: 1px solid black;" valign="top">
                                Pay Type
                            </td>
                            <td style="width: 30px;">
                            </td>
                            <td style="width: 320px; border-bottom: 1px solid black;" colspan="2" valign="top">
                                Project Type
                            </td>
                            <td style="width: 30px;">
                            </td>
                            <td style="width: 250px; border-bottom: 1px solid black;">
                                Practice Area
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="chbActivePersons" runat="server" Text="Active" ToolTip="Include active persons into report"
                                    AutoPostBack="false" Checked="True" onclick="EnableResetButton();" />
                            </td>
                            <td>
                            </td>
                            <td rowspan="2" class="floatRight">
                                <cc2:ScrollingDropDown ID="cblTimeScales" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblTimeScales','Pay Type')" BackColor="White"
                                    CellPadding="3" NoItemsType="All" SetDirty="False" Width="200px" DropDownListType="Pay Type"
                                    Height="100px" BorderWidth="0" />
                                <ext:ScrollableDropdownExtender ID="sdeTimeScales" runat="server" TargetControlID="cblTimeScales"
                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="200px">
                                </ext:ScrollableDropdownExtender>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="chbActiveProjects" runat="server" AutoPostBack="false" Checked="True"
                                    onclick="EnableResetButton();" Text="Active" ToolTip="Include active projects into report" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chbInternalProjects" runat="server" AutoPostBack="false" Checked="True"
                                    onclick="EnableResetButton();" Text="Internal" ToolTip="Include internal projects into report" />
                            </td>
                            <td>
                            </td>
                            <td class="floatRight" style="padding-top: 5px; padding-left: 3px;">
                                <cc2:ScrollingDropDown ID="cblPractices" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" BackColor="White"
                                    CellPadding="3" Height="250px" NoItemsType="All" SetDirty="False" DropDownListType="Practice Area"
                                    Width="260px" BorderWidth="0" />
                                <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                                    UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                </ext:ScrollableDropdownExtender>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="chbProjectedPersons" runat="server" Text="Projected" ToolTip="Include projected persons into report"
                                    AutoPostBack="false" Checked="false" onclick="EnableResetButton();" />
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="chbProjectedProjects" runat="server" AutoPostBack="false" Checked="True"
                                    Text="Projected" ToolTip="Include projected projects into report" onclick="EnableResetButton();" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chbExperimentalProjects" runat="server" AutoPostBack="false" Text="Experimental"
                                    ToolTip="Include experimental projects into report" onclick="EnableResetButton();" />
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkExcludeInternalPractices" runat="server" Text="Exclude Internal Practice Areas"
                                    onclick="EnableResetButton();" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </AjaxControlToolkit:TabPanel>
            <AjaxControlToolkit:TabPanel runat="server" ID="tpGranularity">
                <HeaderTemplate>
                    <span class="bg"><a href="#"><span>Sort Options</span></a> </span>
                </HeaderTemplate>
                <ContentTemplate>
                    <table class="opportunity-description" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                Sort by &nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlSortBy" runat="server" Style="width: 300px;" onchange="EnableResetButton(); EnableDisableRadioButtons();">
                                    <asp:ListItem Value="0">Average Utilization by Period</asp:ListItem>
                                    <asp:ListItem Value="1">Alphabetical by User</asp:ListItem>
                                    <asp:ListItem Value="2">Pay Type</asp:ListItem>
                                    <asp:ListItem Value="3">Practice Area</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td style="width: 40px;">
                            </td>
                            <td>
                                <asp:RadioButton ID="rbSortbyAsc" runat="server" Text="Ascending" onclick="EnableResetButton();ChangeSortByRadioButtons(this);"
                                    GroupName="Sortby" AutoPostBack="false" />
                            </td>
                            <td>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:RadioButton ID="rbSortbyDesc" runat="server" Text="Descending" onclick="EnableResetButton();ChangeSortByRadioButtons(this);"
                                    GroupName="Sortby" AutoPostBack="false" Checked="true" />
                            </td>
                        </tr>
                    </table>
                    <br />
                </ContentTemplate>
            </AjaxControlToolkit:TabPanel>
        </AjaxControlToolkit:TabContainer>
    </asp:Panel>
    <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
    <asp:Label ID="lblMessage" runat="server" ></asp:Label>
</div>
 <asp:HiddenField ID="hdnIsSampleReport" runat="server" />

