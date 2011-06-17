<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProposedResources.ascx.cs"
    Inherits="PraticeManagement.Controls.Opportunities.ProposedResources" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<script type="text/javascript" src="../../Scripts/jquery-1.4.1.js"></script>
<script type="text/javascript">
    function Add_Click() {
        var cblPotentialResources = document.getElementById("<%= cblPotentialResources.ClientID%>");
        var cblProposedResources = document.getElementById("<%= cblProposedResources.ClientID%>");

        var potentialCheckboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');

        for (var i = 0; i < potentialCheckboxes.length; ++i) {

            if (potentialCheckboxes[i].checked &&
                !potentialCheckboxes[i].disabled) {

                var IsItemExists = 'false';
                if (cblProposedResources != null) {
                    for (var j = 0; j < cblProposedResources.children[0].children.length; ++j) {

                        if (potentialCheckboxes[i].parentNode.attributes['personid'].value == cblProposedResources.children[0].children[j].children[0].children[0].attributes['personid'].value) {

                            if (potentialCheckboxes[i].parentNode.attributes['persontype'].value == "1") {
                                cblProposedResources.children[0].children[j].children[0].children[0].children[1].innerHTML = potentialCheckboxes[i].parentNode.attributes['personname'].value;
                                cblProposedResources.children[0].children[j].children[0].children[0].attributes['persontype'].value = "1";
                            }
                            else {
                                cblProposedResources.children[0].children[j].children[0].children[0].children[1].innerHTML = potentialCheckboxes[i].parentNode.attributes['personname'].value.strike();
                                cblProposedResources.children[0].children[j].children[0].children[0].attributes['persontype'].value = "2";
                            }
                            IsItemExists = 'true;'
                        }
                    }
                }

                if (IsItemExists == 'false') {

                    if (cblProposedResources == null) {
                        var divProposedResources = document.getElementById("<%= divProposedResources.ClientID%>");
                        cblProposedResources = document.createElement('table');
                        cblProposedResources.setAttribute('id', "<%= cblProposedResources.ClientID%>");
                        cblProposedResources.setAttribute('cellpadding', '3');
                        cblProposedResources.setAttribute('border', '0');
                        cblProposedResources.setAttribute('style', 'background-color:White;width:100%;');
                        tableBody = document.createElement('tbody');
                        cblProposedResources.appendChild(tableBody);
                        divProposedResources.appendChild(cblProposedResources);
                    }
                    addCheckBoxItem(cblProposedResources,
                                    cblProposedResources.children[0].children.length,
                                    '0',
                                    potentialCheckboxes[i].parentNode.attributes['personname'].value,
                                    potentialCheckboxes[i].parentNode.attributes['personid'].value,
                                    potentialCheckboxes[i].parentNode.attributes['persontype'].value
                                    );
                    EnableSaveButton();
                    setDirty();

                }
            }
        }

        GetProposedPersonIdsListWithPersonType();
    }

    function addCheckBoxItem(checkBoxListRef, rowPosition, checkBoxValue, displayText, Id, personType) {

        var checkBoxListId = checkBoxListRef.id;
        var rowArray = checkBoxListRef.getElementsByTagName('tr');
        var rowCount = rowArray.length;

        var rowElement = checkBoxListRef.insertRow(rowPosition);
        var columnElement = rowElement.insertCell(0);

        var spanRef = document.createElement('span');
        var checkBoxRef = document.createElement('input');
        var labelRef = document.createElement('label');

        spanRef.setAttribute('personid', Id);
        spanRef.setAttribute('personname', displayText);
        spanRef.setAttribute('persontype', personType);

        checkBoxRef.type = 'checkbox';
        checkBoxRef.value = checkBoxValue;
        checkBoxRef.id = checkBoxListId + '_' + rowPosition;

        if (personType == "1") {
            labelRef.innerHTML = displayText;
        }
        else {
            labelRef.innerHTML = displayText.strike();
        }
        labelRef.setAttribute('for', checkBoxRef.id);

        columnElement.appendChild(spanRef);

        spanRef.appendChild(checkBoxRef);
        spanRef.appendChild(labelRef);
    }


    function Remove_Click() {
        var cblProposedResources = document.getElementById("<%= cblProposedResources.ClientID%>");
        if (cblProposedResources != null) {
            for (var i = cblProposedResources.children[0].children.length - 1; i >= 0; i--) {
                if (cblProposedResources.children[0].children[i].children[0].children[0].children[0] != null) {
                    if (cblProposedResources.children[0].children[i].children[0].children[0].children[0].checked &&
                !cblProposedResources.children[0].children[i].children[0].children[0].children[0].disabled) {
                        cblProposedResources.deleteRow(i);
                        EnableSaveButton();
                        setDirty();
                    }
                }
            }
        }
        GetProposedPersonIdsListWithPersonType();
    }

    function GetProposedPersonIdsListWithPersonType() {
        var cblProposedResources = document.getElementById("<%= cblProposedResources.ClientID%>");
        var hdnProposedPersonIdsList = document.getElementById("<%= hdnProposedPersonIdsList.ClientID%>");
        var PersonIdList = '';
        if (cblProposedResources != null) {
            for (var i = 0; i < cblProposedResources.children[0].children.length; ++i) {
                PersonIdList += cblProposedResources.children[0].children[i].children[0].children[0].attributes['personid'].value + ':' + cblProposedResources.children[0].children[i].children[0].children[0].attributes['persontype'].value + ',';
            }
        }
        hdnProposedPersonIdsList.value = PersonIdList;
    }
    function DisableAddRemoveButtons() {
        document.getElementById("btnAdd").disabled = "disabled";
        document.getElementById("btnRemove").disabled = "disabled";
    }    
</script>
<tr>
    <td colspan="6" style="width: 100%; border-bottom: 1px solid black; padding: 1px 0px 4px 0px;">
        <asp:HiddenField ID="hdnOpportunityIdValue" runat="server" />
        <asp:HiddenField ID="hdnProposedPersonIdsList" runat="server" />
    </td>
</tr>
<tr>
    <td>
    </td>
</tr>
<tr style="background-color: #e2ebff">
    <td colspan="6" style="width: 100%; padding-left: 4px;">
        <div style="text-align: center; padding: 4px 0px 4px 0px;">
            Select from the list of Potential Resources below and click "Add" to note them as
            Proposed for this Opportunity.<asp:Image ID="hintDate" runat="server" ImageUrl="~/Images/hint.png"
                ToolTip="Choosing a Start and End Date for the Opportunity, even loosely, will result in a much more accurate idea of which Potential Resources are available for this Opportunity." />
        </div>
    </td>
</tr>
<tr>
    <td colspan="6" style="padding: 0px 0px 4px 4px; width: 100%; background-color: #e2ebff;">
        <table width="100%" >
            <tr style="padding-left: 20px; padding-top: 2px;">
                <td style="padding-right: 20px; padding-top: 2px; width: 45%">
                    <table width="100%">
                        <tr>
                            <td align="center" style="width: 87%;">
                                <asp:Label ID="lblPotentialResources" runat="server" Text="Potential Resources" Style="font-weight: bold;
                                    padding-bottom: 2px;"></asp:Label>
                            </td>
                            <td align="center" style="width: 4%;">
                                <asp:Image ID="imgCheck" runat="server" ImageUrl="~/Images/right_icon.png" />
                            </td>
                            <td align="center" style="width: 5%;">
                                <asp:Image ID="imgCross" runat="server" ImageUrl="~/Images/cross_icon.png" />
                            </td>
                            <td style="width: 5%;">
                            </td>
                        </tr>
                    </table>
                </td >
                <td valign="middle" align="center" style="width: 10%">
                </td>
                <td align="center"  style="padding-left: 20px; padding-top: 2px; vertical-align: top; width: 45%">
                    <asp:Label ID="lblProposedResources" runat="server" Text="Proposed Resources" Style="font-weight: bold;
                        padding-bottom: 2px;"></asp:Label>
                </td>
            </tr>
            <tr style="padding-left: 20px; padding-top: 2px;">
                <td align="left" style="padding-right: 20px; padding-top: 2px; width: 45%">
                    <div class="cbfloatRight" style="height: 100px; width: 100%; overflow-y: scroll;
                        border: 1px solid black; background: white; padding-left: 3px;">
                        <uc:MultipleSelectionCheckBoxList ID="cblPotentialResources" runat="server" Height="100px"
                            Width="100%" BackColor="White" AutoPostBack="false" DataTextField="Name" DataValueField="id" CellSpacing="0" 
                            CellPadding="0" OnDataBound="cblPotentialResources_DataBound">
                        </uc:MultipleSelectionCheckBoxList>
                    </div>
                </td>
                <td valign="middle" align="center" style="width: 10%">
                    <input id="btnAdd" type="button" onclick="Add_Click();" value="Add =>" /><br />
                    <br />
                    <input id="btnRemove" type="button" onclick="Remove_Click();" value="<= Remove" />
                </td>
                <td style="padding-left: 20px; padding-top: 2px; vertical-align: top; width: 45%">
                    <div id="divProposedResources" runat="server" class="cbfloatRight" style="height: 100px;
                        width: 100%; overflow-y: scroll; border: 1px solid black; background: white;
                        padding-left: 3px;">
                        <asp:CheckBoxList ID="cblProposedResources" runat="server" AutoPostBack="false" BackColor="White"
                            Width="100%" DataTextField="Name" DataValueField="id" CellPadding="3" OnDataBound="cblProposedResources_DataBound">
                        </asp:CheckBoxList>
                    </div>
                </td>
            </tr>
        </table>
    </td>
</tr>

