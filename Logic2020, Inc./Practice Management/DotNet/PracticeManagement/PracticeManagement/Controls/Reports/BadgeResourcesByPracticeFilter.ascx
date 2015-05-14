<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BadgeResourcesByPracticeFilter.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.BadgeResourcesByPracticeFilter" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<table class="WholeWidth">
    <tr align="center">
        <td class="Width8Per BorderBottom1px vTop">
            <asp:Label ID="lblCategory" runat="server"></asp:Label>
        </td>
        <td class="Width2PercentImp">
        </td>
        <td class="Width35Percent BorderBottom1px vTop" colspan="3">
            Include people with MS 18 mos category as
        </td>
        <td class="Width2PercentImp">
        </td>
        <td class="BorderBottom1px Width8Per">
            Pay Type
        </td>
        <td class="Width2PercentImp">
        </td>
        <td class="BorderBottom1px Width8Per">
            Person Status
        </td>
    </tr>
    <tr>
        <td style="height: 25px;">
            <span id="tdPractices" runat="server">
                <pmc:ScrollingDropDown ID="cblPractices" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                    onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" NoItemsType="All"
                    DropDownListType="Practice" CellPadding="3" CssClass="NewHireReportCblPractices" />
                <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                    UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                </ext:ScrollableDropdownExtender>
            </span><span id="tdTitles" runat="server">
                <pmc:ScrollingDropDown ID="cblTitles" runat="server" AllSelectedReturnType="Null"
                    onclick="scrollingDropdown_onclick('cblTitles','Title','','Titles')" CellPadding="3"
                    Style="width: 240px;" NoItemsType="All" SetDirty="False" DropDownListType="Title"
                    DropDownListTypePluralForm="Titles" CssClass="NewHireReportCblPractices Height160PxIMP" />
                <ext:ScrollableDropdownExtender ID="sdeTitles" runat="server" TargetControlID="cblTitles"
                    BehaviorID="sdeTitles" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png"
                    Width="250px">
                </ext:ScrollableDropdownExtender>
            </span>
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            <asp:CheckBox ID="chbBadgedNotOnProject" runat="server" AutoPostBack="false" Checked="True"
                Text="Badged not on Project" ToolTip="Badged not on Project" />
        </td>
        <td>
            <asp:CheckBox ID="chbBadgedOnProject" runat="server" AutoPostBack="false" Checked="True"
                Text="Badged on Project" ToolTip="Badged on Project" />
        </td>
        <td>
            <asp:CheckBox ID="chbClockNotStarted" runat="server" AutoPostBack="false" Checked="True"
                Text="18-Month Clock Not Started" ToolTip="18-Month Clock Not Started" />
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            <pmc:ScrollingDropDown ID="cblPayTypes" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                onclick="scrollingDropdown_onclick('cblPayTypes','Pay Type')" NoItemsType="All"
                DropDownListType="Pay Type" CellPadding="3" CssClass="AllEmpClockCblTimeScales" />
            <ext:ScrollableDropdownExtender ID="sdePayTypes" runat="server" TargetControlID="cblPayTypes"
                UseAdvanceFeature="true" Width="220px" EditImageUrl="~/Images/Dropdown_Arrow.png">
            </ext:ScrollableDropdownExtender>
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            <pmc:ScrollingDropDown ID="cblPersonStatus" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                onclick="scrollingDropdown_onclick('cblPersonStatus','Person Status','es')" NoItemsType="All"
                PluralForm="es" DropDownListType="Person Status" CellPadding="3" CssClass="AllEmpClockCblTimeScales" />
            <ext:ScrollableDropdownExtender ID="sdePersonStatus" runat="server" TargetControlID="cblPersonStatus"
                UseAdvanceFeature="true" Width="245px" EditImageUrl="~/Images/Dropdown_Arrow.png">
            </ext:ScrollableDropdownExtender>
        </td>
    </tr>
</table>

