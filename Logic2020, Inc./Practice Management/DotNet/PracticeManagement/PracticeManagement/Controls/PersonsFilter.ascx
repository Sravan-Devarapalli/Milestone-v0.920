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
    <tr class="TextAlignCenterImp">
        <td class="TdPersonFilter">
            <span>Person Status</span>
        </td>
        <td class="Width15PxImp">
        </td>
        <td class="TdPersonFilter">
            <span>Pay Type</span>
        </td>
        <td class="Width15PxImp">
        </td>
        <td class="TdPersonFilter">
            <span>Practice</span>
        </td>
        <td class="Width15PxImp">
        </td>
    </tr>
    <tr>
        <td class="padRight5Imp">
            <table>
                <tr>
                    <td class="no-wrap">
                        <asp:CheckBox ID="chbShowActive" runat="server" AutoPostBack="false" 
                            Checked="true" OnCheckedChanged="chbShowActive_CheckedChanged" CssClass="textLeft" />
                        <span class="padRight5Imp">Active</span>
                    </td>
                    <td class="no-wrap padLeft20Imp">
                        <asp:CheckBox ID="chbProjected" runat="server" />
                        <span class="padRight5Imp">Contingent</span>
                    </td>
                </tr>
                <tr>
                    <td class="no-wrap">
                        <asp:CheckBox ID="chbTerminationPending" runat="server" Checked="true" />
                        <span class="padRight5Imp">Termination Pending</span>
                    </td>
                    <td class="no-wrap padLeft20Imp">
                        <asp:CheckBox ID="chbTerminated" runat="server" />
                        <span class="padRight5Imp">Terminated</span>
                    </td>
                </tr>
            </table>
        </td>
        <td class="Width15PxImp">
        </td>
        <td rowspan="2" class="floatRight PaddingTop5 PaddingLeft3PxImp">
            <cc2:ScrollingDropDown ID="cblTimeScales" runat="server" AllSelectedReturnType="AllItems"
                AutoPostBack="false" onclick="scrollingDropdown_onclick('cblTimeScales','Pay Type')"
                CellPadding="3" NoItemsType="All" SetDirty="False" 
                DropDownListType="Pay Type" CssClass="PersonFilterSddAllTypes" />
            <ext:ScrollableDropdownExtender ID="sdeTimeScales" runat="server" TargetControlID="cblTimeScales"
                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="190px">
            </ext:ScrollableDropdownExtender>
        </td>
        <td class="Width15PxImp">
        </td>
        <td class="floatRight PaddingTop5 PaddingLeft3PxImp">
            <cc2:ScrollingDropDown ID="cblPractices" runat="server" CssClass="PersonPage_cblPractices" AllSelectedReturnType="AllItems"
                onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" 
                 NoItemsType="All" SetDirty="False" DropDownListType="Practice Area"
                 />
            <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
            </ext:ScrollableDropdownExtender>
        </td>
        <td class="Width15PxImp">
        </td>
    </tr>
</table>

