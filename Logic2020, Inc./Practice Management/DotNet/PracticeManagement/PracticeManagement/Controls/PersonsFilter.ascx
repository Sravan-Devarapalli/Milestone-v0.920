<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonsFilter.ascx.cs"
    Inherits="PraticeManagement.Controls.PersonsFilter" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<script type="text/javascript">
    function SetAlternateColorsForCBL() {
        var chkboxList = document.getElementById('<%=cblTimeScales.ClientID %>');
        SetAlternateColors(chkboxList);
        var cbList = document.getElementById('<%=cblPractices.ClientID %>');
        SetAlternateColors(cbList);
    }
     
</script>
<table class="WholeWidth">
    <tr style="text-align: center;">
        <td style="padding-right: 5px; border-bottom: 1px solid black;">
            <span>Person Status</span>
        </td>
        <td style="width: 15px">
        </td>
        <td style="padding-right: 5px; border-bottom: 1px solid black;">
            <span>Pay Type</span>
        </td>
        <td style="width: 15px">
        </td>
        <td style="padding-left: 5px; border-bottom: 1px solid black;">
            <span>Practice</span>
        </td>
        <td style="width: 15px">
        </td>
    </tr>
    <tr>
        <td style="padding-right: 5px;">
            <table>
                <tr>
                    <td style="white-space: nowrap;">
                        <asp:CheckBox ID="chbShowActive" runat="server" AutoPostBack="false" TextAlign="Left"
                            Checked="true" OnCheckedChanged="chbShowActive_CheckedChanged" />
                        <span style="padding-right: 5px">Active</span>
                    </td>
                    <td style="padding-left: 20px; white-space: nowrap;">
                        <asp:CheckBox ID="chbProjected" runat="server" />
                        <span style="padding-right: 5px">Projected</span>
                    </td>
                </tr>
                <tr>
                    <td style="white-space: nowrap;">
                        <asp:CheckBox ID="chbInactive" runat="server" />
                        <span style="padding-right: 5px">Inactive</span>
                    </td>
                    <td style="padding-left: 20px; white-space: nowrap;">
                        <asp:CheckBox ID="chbTerminated" runat="server" />
                        <span style="padding-right: 5px">Terminated</span>
                    </td>
                </tr>
            </table>
        </td>
        <td style="width: 15px">
        </td>
        <td rowspan="2" class="floatRight" style="padding-top: 5px; padding-left: 3px;">
            <cc2:ScrollingDropDown ID="cblTimeScales" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                AutoPostBack="false" onclick="scrollingDropdown_onclick('cblTimeScales','Pay Type')"
                BackColor="White" CellPadding="3" NoItemsType="All" SetDirty="False" Width="190px"
                DropDownListType="Pay Type" Height="100px" BorderWidth="0" />
            <ext:ScrollableDropdownExtender ID="sdeTimeScales" runat="server" TargetControlID="cblTimeScales"
                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="190px">
            </ext:ScrollableDropdownExtender>
        </td>
        <td style="width: 15px">
        </td>
        <td class="floatRight" style="padding-top: 5px; padding-left: 3px;">
            <cc2:ScrollingDropDown ID="cblPractices" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" BackColor="White"
                CellPadding="3" Height="250px" NoItemsType="All" SetDirty="False" DropDownListType="Practice Area"
                Width="240px" BorderWidth="0" />
            <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
            </ext:ScrollableDropdownExtender>
        </td>
        <td style="width: 15px">
        </td>
    </tr>
</table>

