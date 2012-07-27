<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Notes.aspx.cs" Inherits="PraticeManagement.Config.Notes" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Notes | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Notes
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">

        function changeAlternateitemscolrsForCBL() {
            var cblExemptNotesItemsCount = 0;
            var cblMustEnterNotesItemsCount = 0;
            var cblExemptNotes = document.getElementById('<%=cblExemptNotes.ClientID %>');
            if (cblExemptNotes != null) {
                SetAlternateColors(cblExemptNotes);
                cblExemptNotesItemsCount = cblExemptNotes.children[0].children.length;
            }

            var cblMustEnterNotes = document.getElementById('<%=cblMustEnterNotes.ClientID %>');
            if (cblMustEnterNotes != null) {
                SetAlternateColors(cblMustEnterNotes);
                cblMustEnterNotesItemsCount = cblMustEnterNotes.children[0].children.length;
            }

            var divExemptNotes = document.getElementById("<%= divExemptNotes.ClientID%>");
            var divMustEnterNotes = document.getElementById("<%= divMustEnterNotes.ClientID%>");


            if (cblExemptNotesItemsCount >= cblMustEnterNotesItemsCount) {
                divExemptNotes.style.height = (cblExemptNotesItemsCount * 19).toString() + "px";
                divMustEnterNotes.style.height = (cblExemptNotesItemsCount * 19).toString() + "px";

            }
            else {
                divExemptNotes.style.height = (cblMustEnterNotesItemsCount * 19).toString() + "px";
                divMustEnterNotes.style.height = (cblMustEnterNotesItemsCount * 19).toString() + "px";
            }

            var btnSave = document.getElementById("<%= btnSave.ClientID%>");
            if (!btnSave.disabled && btnSave.disabled != "true" && btnSave.disabled != "disabled") {
                hideSuccessMessage();
                btnSave.focus();
            }
        }

        function hideSuccessMessage() {
            message = document.getElementById("<%=mlConfirmation.ClientID %>" + "_lblMessage");
            if (message != null) {
                message.style.display = "none";
            }
            return true;
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
                        cblExemptNotes.setAttribute('cellpadding', '0');
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
                        cblMustEnterNotes.setAttribute('cellpadding', '0');
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
            <b>Time Entry Notes Policy</b>
            <table class="WholeWidth">
                <tr>
                    <td class="PaddingTop5Imp">
                        <asp:CheckBox ID="chbNotesRequired" onclick="chbNotesRequired_Change(this);setDirty();"
                            runat="server" Text="Notes are required for all of the practices selected below and all newly created practices by default."
                            Checked="true" />
                        <asp:HiddenField ID="hdnExemptNotes" runat="server" Value="" />
                        <asp:HiddenField ID="hdnMustEnterNotes" runat="server" Value="" />
                    </td>
                </tr>
            </table>
            <br />
            <table class="WholeWidth">
                <tr>
                    <td class="textLeft vMiddle Width10PerImp">
                    </td>
                    <td class="textLeft vMiddle Width75Percent">
                        <table class="WholeWidth">
                            <tr class="PaddingTop2Px">
                                <td class="TdNotesOptional">
                                    <b>Notes Optional</b>
                                </td>
                                <td class="vMiddle TextAlignCenterImp Width12Per">
                                </td>
                                <td class="TdNotesOptional">
                                    <b>Notes Required</b>
                                </td>
                            </tr>
                        </table>
                        <table class="WholeWidth">
                            <tr>
                                <td class="TdExemtNotes">
                                    <div id="divExemptNotes" runat="server" class="cbfloatRight DivExemtNotes">
                                        <asp:CheckBoxList ID="cblExemptNotes" runat="server" CssClass="WholeWidthImp bgColorWhite"
                                            AutoPostBack="false" DataTextField="HtmlEncodedName" DataValueField="id" CellSpacing="0"
                                            CellPadding="0" OnDataBound="cblExemptNotes_DataBound">
                                        </asp:CheckBoxList>
                                    </div>
                                </td>
                                <td class="vMiddle TextAlignCenterImp Width12Per">
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
                                <td class="TdExemtNotes">
                                    <div id="divMustEnterNotes" runat="server" class="cbfloatRight DivExemtNotes">
                                        <asp:CheckBoxList ID="cblMustEnterNotes" runat="server" AutoPostBack="false" CssClass="WholeWidthImp bgColorWhite"
                                            DataTextField="HtmlEncodedName" DataValueField="id"
                                            OnDataBound="cblMustEnterNotes_DataBound">
                                        </asp:CheckBoxList>
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <table class="WholeWidth">
                            <tr>
                                <td class="Padding4Px Height35Px Width70Per">
                                    <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                                    <asp:ValidationSummary ID="vsumNotes" runat="server" ValidationGroup="Notes" />
                                </td>
                                <td class="BtnSave">
                                    <asp:Button ID="btnSave" Enabled="false" runat="server" Text="Save Changes" OnClientClick="hideSuccessMessage();"
                                        OnClick="btnSave_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width15PercentImp">
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

