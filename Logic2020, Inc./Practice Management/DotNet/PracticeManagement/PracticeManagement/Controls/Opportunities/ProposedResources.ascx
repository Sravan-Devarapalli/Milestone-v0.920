<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProposedResources.ascx.cs"
    Inherits="PraticeManagement.Controls.Opportunities.ProposedResources" %>
<script type="text/javascript">

    function SethdnPotentialResourcesValueToTrue() {
        var hiddenField = document.getElementById("<%= hdnPotentialResources.ClientID%>");
        var checkBoxList = document.getElementById("<%= cblPotentialResources.ClientID%>");
        var count = 0;
        for (var i = 0; i < checkBoxList.children[0].children.length; ++i) {
            if (checkBoxList.children[0].children[i].children[0].children[0] != null) {
                if (checkBoxList.children[0].children[i].children[0].children[0].checked &&
                !checkBoxList.children[0].children[i].children[0].children[0].disabled) {
                    count = 1;
                    break;
                }
            }

            if (checkBoxList.children[0].children[i].children[0].children[0].children[0] != null) {
                if (checkBoxList.children[0].children[i].children[0].children[0].children[0].checked &&
                !checkBoxList.children[0].children[i].children[0].children[0].children[0].disabled) {
                    count = 1;
                    break;
                }
            }
        }

        if (count == 1) {
            hiddenField.value = "true";
        }
        else {
            hiddenField.value = "";
        }
    }

    function SethdnProposedResourcesValueToTrue() {
        var hiddenField = document.getElementById("<%= hdnProposedResources.ClientID%>");
        var checkBoxList = document.getElementById("<%= cblProposedResources.ClientID%>");
        var count = 0;
        for (var i = 0; i < checkBoxList.children[0].children.length; ++i) {
            if (checkBoxList.children[0].children[i].children[0].children[0] != null) {
                if (checkBoxList.children[0].children[i].children[0].children[0].checked &&
                !checkBoxList.children[0].children[i].children[0].children[0].disabled) {
                    count = 1;
                    break;
                }
            }

            if (checkBoxList.children[0].children[i].children[0].children[0].children[0] != null) {
                if (checkBoxList.children[0].children[i].children[0].children[0].children[0].checked &&
                !checkBoxList.children[0].children[i].children[0].children[0].children[0].disabled) {
                    count = 1;
                    break;
                }
            }
        }

        if (count == 1) {
            hiddenField.value = "true";
        }
        else {
            hiddenField.value = "";
        }
    }
    function Add_Click() {
        var hiddenField = document.getElementById("<%= hdnPotentialResources.ClientID%>");
        if (hiddenField.value == "true") {
            hiddenField.value = "";
            return true;
        }
        else {
            return false;
        }
    }

    function Remove_Click() {
        var hiddenField = document.getElementById("<%= hdnProposedResources.ClientID%>");
        if (hiddenField.value == "true") {
            hiddenField.value = "";
            return true;
        }
        else {
            return false;
        }
    }
</script>
<tr>
    <td colspan="6" style="width: 100%; border-bottom: 1px solid black; padding: 1px 0px 4px 0px;">
        <asp:HiddenField ID="hdnOpportunityIdValue" runat="server" />
        <asp:HiddenField ID="hdnPotentialResources" runat="server" />
        <asp:HiddenField ID="hdnProposedResources" runat="server" />
    </td>
</tr>
<tr>
    <td>
    </td>
</tr>
<tr style="background-color: #e2ebff">
    <td colspan="6" style="width: 100%; padding-left: 4px;">
        <div style="text-align: center; padding: 4px 0px 4px 0px;">
            Select from the list of Potential resources below and click the "Add" button to
            note them as Proposed for this Opportunity.<asp:Image ID="hintDate" runat="server" ImageUrl="~/Images/hint.png" ToolTip="Choosing a Start and End Date for the Opportunity, even loosely, will result in a much more accurate idea of which Potential Resources are available for this Opportunity." />
        </div>
    </td>
</tr>
<tr>
    <td colspan="6" style="padding: 0px 0px 4px 4px; width: 100%; background-color: #e2ebff;">
        <table width="100%" style="padding: 4px">
            <tr style="padding: 2px 0px 2px 0px !important;">
                <td align="center">
                    <asp:Label ID="lblPotentialResources" runat="server" Text="Potential Resources" Style="font-weight: bold;
                        padding-bottom: 2px;"></asp:Label>
                </td>
                <td>
                </td>
                <td align="center">
                    <asp:Label ID="lblProposedResources" runat="server" Text="Proposed Resources" Style="font-weight: bold;
                        padding-bottom: 2px;"></asp:Label>
                </td>
            </tr>
            <tr style="padding-left: 20px; padding-top: 2px;">
                <td style="padding-right: 20px; padding-top: 2px; width: 45%">
                    <div class="cbfloatRight" style="height: 100px; width: 100%; overflow-y: scroll;
                        border: 1px solid black; background: white; padding-left: 3px;">
                        <asp:CheckBoxList ID="cblPotentialResources" runat="server" Height="100px" Width="100%"
                            BackColor="White" AutoPostBack="false" DataTextField="Name" DataValueField="id"
                            CellPadding="3" OnDataBound="cblPotentialResources_DataBound">
                        </asp:CheckBoxList>
                    </div>
                </td>
                <td valign="middle" align="center" style="width: 10%">
                    <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add =>" /><br />
                    <br />
                    <asp:Button ID="btnRemove" runat="server" OnClick="btnRemove_Click" Text="<= Remove" />
                </td>
                <td style="padding-left: 20px; padding-top: 2px; vertical-align: top; width: 45%">
                    <div class="cbfloatRight" style="height: 100px; width: 100%; overflow-y: scroll;
                        border: 1px solid black; background: white;">
                        <asp:CheckBoxList ID="cblProposedResources" runat="server" AutoPostBack="false" BackColor="White"
                            Width="100%" DataTextField="Name" DataValueField="id" CellPadding="4" OnDataBound="cblProposedResources_DataBound">
                        </asp:CheckBoxList>
                    </div>
                </td>
            </tr>
        </table>
    </td>
</tr>

