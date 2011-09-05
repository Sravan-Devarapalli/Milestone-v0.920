<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpportunityFilter.ascx.cs"
    Inherits="PraticeManagement.Controls.Generic.Filtering.OpportunityFilter" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.SearchHighlighting"
    TagPrefix="ext" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<script type="text/javascript">
    function EnableOrDisableGroup() {
        var cbl = document.getElementById("<%= cblClient.ClientID %>");
        var arrayOfCheckBoxes = cbl.getElementsByTagName("input");
        var cblList = $("div[id^='sdeCblOpportunityGroup']");
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
        custom_ScrollingDropdown_onclick('cblOpportunityGroup', 'Group');
    }

    function GetDefaultcblList(cblClienitID) {
        var div = document.getElementById(cblClienitID);
        var arrayOfCheckBoxes = div.getElementsByTagName('input');
        for (var i = 0; i < arrayOfCheckBoxes.length; i++) {
            arrayOfCheckBoxes[i].checked = true;
        }
    }



    function resetFiltersTab() {
        document.getElementById("<%= chbActive.ClientID %>").checked = true;
        document.getElementById("<%= chbWon.ClientID %>").checked = false;
        document.getElementById("<%= chbExperimental.ClientID %>").checked = false;
        document.getElementById("<%= chbInActive.ClientID %>").checked = false;
        document.getElementById("<%= chbLost.ClientID %>").checked = false;
        GetDefaultcblList("<%= cblClient.ClientID %>");
        GetDefaultcblList("<%= cblOpportunityOwner.ClientID %>");
        GetDefaultcblList("<%= cblSalesperson.ClientID %>");
        GetDefaultcblList("<%= cblOpportunityGroup.ClientID %>");
        scrollingDropdown_onclick('cblClient', 'Client');
        EnableOrDisableGroup();
        scrollingDropdown_onclick('cblSalesperson', 'Salesperson');
        scrollingDropdown_onclick('cblOpportunityOwner', 'Owner');
        custom_ScrollingDropdown_onclick('cblOpportunityGroup', 'Group');

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
                    text = "Please Choose " + type.toString() + "(s)";
                }
            }
            if (text.length > 32) {
                text = text.substr(0, 30) + "..";
            }
            scrollingDropdownList.parentNode.children[1].children[0].firstChild.nodeValue = text;
        }
    }

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
    function endRequestHandle(sender, Args) {
        addListenersToParent('<%= cblOpportunityGroup.ClientID %>', '<%= cblClient.ClientID %>');
    }
</script>
<ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="CustomTabStyle">
    <ajaxToolkit:TabPanel runat="server" ID="tpSearch">
        <HeaderTemplate>
            <span class="bg"><a href="#"><span>Search</span></a> </span>
        </HeaderTemplate>
        <ContentTemplate>
            <asp:Panel ID="pnlSearch" runat="server" Style="height: 80px;" CssClass="project-filter"
                DefaultButton="btnSearchAll">
                <table class="WholeWidth">
                    <tr>
                        <td style="padding-right: 8px; padding-left: 4px;">
                            <asp:TextBox ID="txtSearchText" runat="server" CssClass="WholeWidth" EnableViewState="False" />
                        </td>
                        <td style="width: 10px;">
                            <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                ErrorMessage="Please type text to be searched." ToolTip="Please type text to be searched."
                                Text="*" SetFocusOnError="true" ValidationGroup="Search" CssClass="searchError"
                                Display="Static" />
                        </td>
                        <td>
                            <asp:Button ID="btnSearchAll" runat="server" Text="Search All" ValidationGroup="Search"
                                PostBackUrl="~/OpportunitySearch.aspx" Width="100px" EnableViewState="False" />
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ValidationSummary ID="valsSearch" runat="server" ValidationGroup="Search" EnableClientScript="true"
                                ShowMessageBox="false" CssClass="searchError" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
        <HeaderTemplate>
            <span class="bg"><a href="#"><span>Filters</span></a> </span>
        </HeaderTemplate>
        <ContentTemplate>
            <div class="project-filter">
                <table class="WholeWidth">
                    <tr class="tb-header">
                        <td colspan="2" style="border-bottom: 1px solid black; text-align: center; width: 200px;">
                            Opportunity Status
                        </td>
                        <td style="width: 20px;">
                        </td>
                        <td style="border-bottom: 1px solid black; width: 190px; text-align: center">
                            Client / Group
                        </td>
                        <td style="padding: 5px; width: 10px;">
                        </td>
                        <td style="border-bottom: 1px solid black; width: 190px; text-align: center;">
                            Sales Team
                        </td>
                        <td align="right" rowspan="3" style="text-align: right;">
                            <table style="text-align: right; width: 100%;">
                                <tr>
                                    <td style="padding-bottom: 5px;">
                                        <asp:Button ID="btnUpdateFilters" runat="server" Text="Update" Width="100px" OnClick="btnUpdateView_Click"
                                            ValidationGroup="Filter" EnableViewState="False" CssClass="pm-button" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 5px;">
                                        <asp:Button ID="btnResetFilters" runat="server" Text="Reset" Width="100px" CausesValidation="False"
                                            OnClientClick="resetFiltersTab(); return false;" EnableViewState="False" CssClass="pm-button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td rowspan="3" style="padding-left: 10px; width: 100px;">
                            <table>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbActive" runat="server" Checked="true" Text="Active" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbWon" runat="server" Checked="false" Text="Won" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbExperimental" runat="server" Checked="false" Text="Experimental" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td rowspan="3" style="padding-left: 10px; width: 100px;">
                            <table>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbInActive" runat="server" Checked="false" Text="Inactive" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbLost" runat="server" Checked="false" Text="Lost" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 20px;">
                            &nbsp;
                        </td>
                        <td style="padding: 5px;">
                            <uc:CascadingMsdd ID="cblClient" runat="server" TargetControlId="cblOpportunityGroup"
                                SetDirty="false" Width="240" Height="240px" onclick="scrollingDropdown_onclick('cblClient','Client');EnableOrDisableGroup();"
                                DropDownListType="Client" CellPadding="3" />
                            <ext:ScrollableDropdownExtender ID="sdeCblClient" runat="server" TargetControlID="cblClient"
                                UseAdvanceFeature="true" EditImageUrl="../../../Images/Dropdown_Arrow.png" Width="240px">
                            </ext:ScrollableDropdownExtender>
                        </td>
                        <td>
                        </td>
                        <td>
                            <uc:ScrollingDropDown ID="cblSalesperson" runat="server" SetDirty="false" Width="240"
                                Height="240px" onclick="scrollingDropdown_onclick('cblSalesperson','Salesperson');"
                                DropDownListType="Salesperson" CellPadding="3" />
                            <ext:ScrollableDropdownExtender ID="sdeCblSalesperson" runat="server" TargetControlID="cblSalesperson"
                                UseAdvanceFeature="true" EditImageUrl="../../../Images/Dropdown_Arrow.png" Width="240px">
                            </ext:ScrollableDropdownExtender>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20px;">
                        </td>
                        <td style="padding: 5px;">
                            <uc:ScrollingDropDown ID="cblOpportunityGroup" runat="server" SetDirty="false" Width="240"
                                Height="240px" onclick="custom_ScrollingDropdown_onclick('cblOpportunityGroup','Group')"
                                DropDownListType="Group" CellPadding="3" />
                            <ext:ScrollableDropdownExtender ID="sdeCblOpportunityGroup" runat="server" TargetControlID="cblOpportunityGroup"
                                UseAdvanceFeature="true" EditImageUrl="../../../Images/Dropdown_Arrow.png" Width="240px">
                            </ext:ScrollableDropdownExtender>
                        </td>
                        <td>
                        </td>
                        <td>
                            <uc:ScrollingDropDown ID="cblOpportunityOwner" runat="server" SetDirty="false" Width="240"
                                Height="240px" onclick="scrollingDropdown_onclick('cblOpportunityOwner','Owner');"
                                DropDownListType="Owner" CellPadding="3" />
                            <ext:ScrollableDropdownExtender ID="sdeCblOpportunityOwner" runat="server" TargetControlID="cblOpportunityOwner"
                                UseAdvanceFeature="true" EditImageUrl="../../../Images/Dropdown_Arrow.png" Width="240px">
                            </ext:ScrollableDropdownExtender>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>

