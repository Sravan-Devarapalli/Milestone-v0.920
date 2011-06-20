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
    <td style="padding: 0px 0px 4px 4px; width: 100%; background-color: #e2ebff;">
        <table width="100%">
            <tr style=" padding-top: 2px;">
                <td align="left" style="padding-left: 6px;padding-right: 6px; padding-top: 2px; width: 44%">
                    <div class="cbfloatRight" style="height: 100px; width: 100%; overflow-y: scroll;
                        border: 1px solid black; background: white; padding-left: 3px;">
                        <uc:MultipleSelectionCheckBoxList ID="cblPotentialResources" runat="server" Height="100px"
                            Width="100%" BackColor="White" AutoPostBack="false" DataTextField="Name" DataValueField="id"
                            CellSpacing="0" CellPadding="0" OnDataBound="cblPotentialResources_DataBound">
                        </uc:MultipleSelectionCheckBoxList>
                    </div>
                </td>
                <td valign="middle" align="center" style="width: 12%">
                    <input id="btnAdd" type="button" onclick="Add_Click();" value="Add =>" /><br />
                    <br />
                    <input id="btnRemove" type="button" onclick="Remove_Click();" value="<= Remove" />
                </td>
                <td align="left" style="padding-left: 6px;padding-right: 6px; padding-top: 2px; vertical-align: top;
                    width: 44%">
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
<tr>
    <td style="width: 100%; padding: 1px 0px 4px 0px;">
        <asp:HiddenField ID="hdnOpportunityIdValue" runat="server" />
        <asp:HiddenField ID="hdnProposedPersonIdsList" runat="server" />
    </td>
</tr>

