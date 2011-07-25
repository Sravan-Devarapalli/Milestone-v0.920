<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Notes.aspx.cs" Inherits="PraticeManagement.Config.Notes" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Notes</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Notes
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.4.1.js"></script>
    <script type="text/javascript">

        function changeAlternateitemscolrsForCBL() {
            var cbl = document.getElementById('<%=cblExemptNotes.ClientID %>');
            if (cbl != null)
                SetAlternateColors(cbl);

            var cbl2 = document.getElementById('<%=cblMustEnterNotes.ClientID %>');
            if (cbl2 != null)
                SetAlternateColors(cbl2);
        }

        function SetAlternateColors(chkboxList) {
            var chkboxes = chkboxList.getElementsByTagName('input');
            var index = 0;
            for (var i = 0; i < chkboxes.length; i++) {
                if (chkboxes[i].parentNode.style.display != "none") {
                    index++;
                    if ((index) % 2 == 0) {
                        chkboxes[i].parentNode.parentNode.style.backgroundColor = "#f9faff";
                    }
                    else {
                        chkboxes[i].parentNode.parentNode.style.backgroundColor = "";
                    }
                }
            }
        }

        function ExemptToMustEnterAll_Click() {
            var cblExemptNotesCheckboxes = $('#<%=cblExemptNotes.ClientID %> tr td :input');
            for (var i = 0; i < cblExemptNotesCheckboxes.length; ++i) {
                cblExemptNotesCheckboxes[i].checked = true;
            }
            ExemptToMustEnter_Click();
        }

        function MustEnterToExemptAll_Click() {
            var cblMustEnterNotesCheckboxes = $('#<%=cblMustEnterNotes.ClientID %> tr td :input');
            for (var i = 0; i < cblMustEnterNotesCheckboxes.length; ++i) {
                cblMustEnterNotesCheckboxes[i].checked = true;
            }
            MustEnterToExempt_Click();
        }

        function MustEnterToExempt_Click() {
            var cblMustEnterNotes = document.getElementById("<%= cblMustEnterNotes.ClientID%>");
            var cblExemptNotes = document.getElementById("<%= cblExemptNotes.ClientID%>");

            var cblMustEnterNotesCheckboxes = $('#<%=cblMustEnterNotes.ClientID %> tr td :input');

            for (var i = 0; i < cblMustEnterNotesCheckboxes.length; ++i) {

                if (cblMustEnterNotesCheckboxes[i].checked) {

                    if (cblExemptNotes == null) {
                        var divExemptNotes = document.getElementById("<%= divExemptNotes.ClientID%>");
                        cblExemptNotes = document.createElement('table');
                        cblExemptNotes.setAttribute('id', "<%= cblExemptNotes.ClientID%>");
                        cblExemptNotes.setAttribute('cellpadding', '3');
                        cblExemptNotes.setAttribute('border', '0');
                        cblExemptNotes.setAttribute('style', 'background-color:White;width:100%;');
                        tableBody = document.createElement('tbody');
                        cblExemptNotes.appendChild(tableBody);
                        divExemptNotes.appendChild(cblExemptNotes);
                    }
                    addCheckBoxItem(cblExemptNotes,
                                    cblExemptNotes.children[0].children.length,
                                    '0',
                                    cblMustEnterNotesCheckboxes[i].parentNode.attributes['practicename'].value,
                                    cblMustEnterNotesCheckboxes[i].parentNode.attributes['practiceid'].value
                                    );
                    EnableSaveButton();
                    setDirty();
                }
            }

            RemovePracticesFromList(cblMustEnterNotes);
            changeAlternateitemscolrsForCBL();
        }

        function ExemptToMustEnter_Click() {
            var cblMustEnterNotes = document.getElementById("<%= cblMustEnterNotes.ClientID%>");
            var cblExemptNotes = document.getElementById("<%= cblExemptNotes.ClientID%>");

            var cblExemptNotesCheckboxes = $('#<%=cblExemptNotes.ClientID %> tr td :input');

            for (var i = 0; i < cblExemptNotesCheckboxes.length; ++i) {

                if (cblExemptNotesCheckboxes[i].checked) {

                    if (cblMustEnterNotes == null) {
                        var divMustEnterNotes = document.getElementById("<%= divMustEnterNotes.ClientID%>");
                        cblMustEnterNotes = document.createElement('table');
                        cblMustEnterNotes.setAttribute('id', "<%= cblMustEnterNotes.ClientID%>");
                        cblMustEnterNotes.setAttribute('cellpadding', '3');
                        cblMustEnterNotes.setAttribute('border', '0');
                        cblMustEnterNotes.setAttribute('style', 'background-color:White;width:100%;');
                        tableBody = document.createElement('tbody');
                        cblMustEnterNotes.appendChild(tableBody);
                        divMustEnterNotes.appendChild(cblMustEnterNotes);
                    }
                    addCheckBoxItem(cblMustEnterNotes,
                                    cblMustEnterNotes.children[0].children.length,
                                    '0',
                                    cblExemptNotesCheckboxes[i].parentNode.attributes['practicename'].value,
                                    cblExemptNotesCheckboxes[i].parentNode.attributes['practiceid'].value
                                    );
                    EnableSaveButton();
                    setDirty();
                }
            }

            RemovePracticesFromList(cblExemptNotes);
            changeAlternateitemscolrsForCBL();
        }


        function addCheckBoxItem(checkBoxListRef, rowPosition, checkBoxValue, displayText, Id) {

            var checkBoxListId = checkBoxListRef.id;
            var rowArray = checkBoxListRef.getElementsByTagName('tr');
            var rowCount = rowArray.length;

            var rowElement = checkBoxListRef.insertRow(rowPosition);
            var columnElement = rowElement.insertCell(0);

            var spanRef = document.createElement('span');
            var checkBoxRef = document.createElement('input');
            var labelRef = document.createElement('label');

            spanRef.setAttribute('practiceid', Id);
            spanRef.setAttribute('practicename', displayText);

            checkBoxRef.type = 'checkbox';
            checkBoxRef.value = checkBoxValue;
            checkBoxRef.id = checkBoxListId + '_' + rowPosition;
            labelRef.innerHTML = displayText;
            labelRef.setAttribute('for', checkBoxRef.id);
            columnElement.appendChild(spanRef);
            spanRef.appendChild(checkBoxRef);
            spanRef.appendChild(labelRef);
        }


        function RemovePracticesFromList(cbl) {
            if (cbl != null) {
                for (var i = cbl.children[0].children.length - 1; i >= 0; i--) {
                    if (cbl.children[0].children[i].getElementsByTagName('input')[0] != null) {
                        if (cbl.children[0].children[i].getElementsByTagName('input')[0].checked) {
                            cbl.deleteRow(i);
                            EnableSaveButton();
                            setDirty();
                        }
                    }
                }
            }
        }

        function GetpracticeIdsList() {
            var cblExemptNotes = document.getElementById("<%= cblExemptNotes.ClientID%>");
            var hdnExemptNotesPracticeIdsList = document.getElementById("<%= hdnExemptNotes.ClientID%>");

            var ExemptNotesPracticeIdsList = '';
            if (cblExemptNotes != null) {
                for (var i = 0; i < cblExemptNotes.children[0].children.length; ++i) {
                    ExemptNotesPracticeIdsList += cblExemptNotes.children[0].children[i].getElementsByTagName('span')[0].attributes['practiceid'].value + ',';
                }
            }
            hdnExemptNotesPracticeIdsList.value = ExemptNotesPracticeIdsList;


            var cblMustEnterNotes = document.getElementById("<%= cblMustEnterNotes.ClientID%>");
            var hdnMustEnterNotesPracticeIdsList = document.getElementById("<%= hdnMustEnterNotes.ClientID%>");

            var MustEnterNotesPracticeIdsList = '';
            if (cblMustEnterNotes != null) {
                for (var i = 0; i < cblMustEnterNotes.children[0].children.length; ++i) {
                    MustEnterNotesPracticeIdsList += cblMustEnterNotes.children[0].children[i].getElementsByTagName('span')[0].attributes['practiceid'].value + ',';
                }
            }
            hdnMustEnterNotesPracticeIdsList.value = MustEnterNotesPracticeIdsList;

        }

        function EnableSaveButton() {
            var btnSave = document.getElementById("<%= btnSave.ClientID%>");
            btnSave.disabled = '';
        }

        function EnableOrDisableButtons(val) {
            var btnExemptToMustEnterAll = document.getElementById("btnExemptToMustEnterAll");
            var btnExemptToMustEnter = document.getElementById("btnExemptToMustEnter");
            var btnMustEnterToExempt = document.getElementById("btnMustEnterToExempt");
            var btnMustEnterToExemptAll = document.getElementById("btnMustEnterToExemptAll");

            if (val == "true") {
                btnExemptToMustEnterAll.disabled = '';
                btnExemptToMustEnter.disabled = '';
                btnMustEnterToExempt.disabled = '';
                btnMustEnterToExemptAll.disabled = '';
            }
            else {
                btnExemptToMustEnterAll.disabled = 'disabled';
                btnExemptToMustEnter.disabled = 'disabled';
                btnMustEnterToExempt.disabled = 'disabled';
                btnMustEnterToExemptAll.disabled = 'disabled';
            }
        }


        function chbNotesRequired_Change(chb) {
            EnableSaveButton();
            if (chb.checked) {
                EnableOrDisableButtons("true");
                ExemptToMustEnterAll_Click();
            }
            else {
                EnableOrDisableButtons("false");
                MustEnterToExemptAll_Click();
                var cblExemptNotes = document.getElementById("<%= cblExemptNotes.ClientID%>");
                if (cblExemptNotes != null) {
                    for (var i = 0; i < cblExemptNotes.children[0].children.length; ++i) {
                        cblExemptNotes.children[0].children[i].getElementsByTagName('span')[0].disabled = 'disabled';
                        cblExemptNotes.children[0].children[i].getElementsByTagName('input')[0].checked = false;
                        cblExemptNotes.children[0].children[i].getElementsByTagName('input')[0].disabled = 'disabled';
                    }
                }

                GetpracticeIdsList();
            }
        }

    </script>
    <asp:UpdatePanel ID="updNotes" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="left" valign="top" style="width: 15%;">
                        <table width="100%" class="PaddingClass">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chbNotesRequired" onclick="chbNotesRequired_Change(this);setDirty();"
                                        runat="server" Text="Notes are required" Checked="true" />
                                </td>
                                <td>
                                    <asp:HiddenField ID="hdnExemptNotes" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnMustEnterNotes" runat="server" Value="" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td align="left" valign="top" style="width: 70%;">
                    </td>
                    <td style="width: 15%;">
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top" style="width: 15%;">
                    </td>
                    <td align="left" valign="top" style="width: 70%;">
                        <table width="100%">
                            <tr style="padding-top: 2px;">
                                <td align="center" style="padding-left: 6px; padding-right: 6px; text-align: center;
                                    padding-top: 2px; width: 44%">
                                    <b>Exempt from notes</b>
                                </td>
                                <td valign="middle" align="center" style="width: 12%">
                                </td>
                                <td align="center" style="padding-left: 6px; padding-right: 6px; padding-top: 2px;
                                    vertical-align: top; text-align: center; width: 44%">
                                    <b>Must Enter Notes</b>
                                </td>
                            </tr>
                        </table>
                        <table width="100%">
                            <tr style="padding-top: 2px;">
                                <td align="left" style="padding-left: 6px; padding-right: 6px; padding-top: 2px;
                                    width: 44%">
                                    <div id="divExemptNotes" runat="server" class="cbfloatRight" style="height: 150px;
                                        width: 100%; overflow-y: scroll; border: 1px solid black; background: white;
                                        padding-left: 3px;">
                                        <asp:CheckBoxList ID="cblExemptNotes" runat="server" Width="100%" BackColor="White"
                                            AutoPostBack="false" DataTextField="Name" DataValueField="id" CellSpacing="0"
                                            CellPadding="3" OnDataBound="cblExemptNotes_DataBound">
                                        </asp:CheckBoxList>
                                    </div>
                                </td>
                                <td valign="middle" align="center" style="width: 12%">
                                    <input id="btnExemptToMustEnterAll" type="button" onclick="ExemptToMustEnterAll_Click();GetpracticeIdsList();"
                                        title="Add All" value=">>" /><br />
                                    <input id="btnExemptToMustEnter" type="button" onclick="ExemptToMustEnter_Click();GetpracticeIdsList();"
                                        title="Add Selected" value=">" />
                                    <br />
                                    <br />
                                    <input id="btnMustEnterToExempt" type="button" onclick="MustEnterToExempt_Click();GetpracticeIdsList();"
                                        title="Remove Selected" value="<" />
                                    <br />
                                    <input id="btnMustEnterToExemptAll" type="button" onclick="MustEnterToExemptAll_Click();GetpracticeIdsList();"
                                        title="Remove All" value="<<" />
                                </td>
                                <td align="left" style="padding-left: 6px; padding-right: 6px; padding-top: 2px;
                                    vertical-align: top; width: 44%">
                                    <div id="divMustEnterNotes" runat="server" class="cbfloatRight" style="height: 150px;
                                        width: 100%; overflow-y: scroll; border: 1px solid black; background: white;
                                        padding-left: 3px;">
                                        <asp:CheckBoxList ID="cblMustEnterNotes" runat="server" AutoPostBack="false" BackColor="White"
                                            Width="100%" DataTextField="Name" DataValueField="id" CellPadding="3" CellSpacing="0"
                                            OnDataBound="cblMustEnterNotes_DataBound">
                                        </asp:CheckBoxList>
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <table style="width: 100%;">
                            <tr>
                                <td style="padding: 4px; height: 35px; width: 70%;">
                                    <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                                    <asp:ValidationSummary ID="vsumNotes" runat="server" ValidationGroup="Notes" />
                                </td>
                                <td align="right" style="padding: 4px; padding-right: 0px; height: 35px; width: 30%;">
                                    <asp:Button ID="btnSave" Enabled="false" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 15%;">
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

