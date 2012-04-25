<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FilteredCheckBoxList.ascx.cs"
    Inherits="PraticeManagement.Controls.FilteredCheckBoxList" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<script src="../Scripts/FilterTable.js" type="text/javascript"></script>
<script type="text/javascript">

    function btnOk_Click(okButton) {
        okButton.click();
    }

    function btnCancelButton_Click(filterdiv) {
        var divObject = document.getElementById(filterdiv);
        divObject.style.display = 'none';
        return false;
    }

    function uncheckAllCheckBoxes(cblList) {
        for (var i = 0; i < cblList.length; i++) {
            cblList[i].checked = false;
        }

    }

    function Filter_Click(filterdiv, selectedIndexes, cbl, txtSearchBox) {

        var divObject = document.getElementById(filterdiv);
        divObject.style.display = '';
        var txtSearchBoxObject = document.getElementById(txtSearchBox);
        txtSearchBoxObject.value = '';
        filterTableRows(txtSearchBox, cbl, true);
        var indexesArray = selectedIndexes.split('_');
        var cblList = document.getElementById(cbl).getElementsByTagName('input');

        uncheckAllCheckBoxes(cblList);

        for (var i = 0; i < indexesArray.length; i++) {
            if (indexesArray[i] != '')
                cblList[indexesArray[i]].checked = true;
        }
    }
</script>
<div id='divFilterPopUp' runat="server" style='border: 2px solid black; background-color: #d4dff8;
    width: 175px;'>
    <table style='text-align: center; width: 100%;'>
        <tr>
            <td align='center'>
                <table style='width: 160px'>
                    <tr>
                        <td style='padding: 6px 0px 6px 0px;'>
                            <asp:TextBox ID="txtSearchBox" runat="server" style='width: 155px;'
                                MaxLength="50"></asp:TextBox>
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmSearch" runat="server" TargetControlID="txtSearchBox"
                                WatermarkText="Search" WatermarkCssClass="watermarkedtext" BehaviorID="wmbhSearchBox" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style='width: 100%'>
                <cc2:ScrollingDropDown ID="cbl" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="Null"
                    Height="155px" BackColor="White" CellPadding="3" NoItemsType="All" SetDirty="False"
                    Width="160px" BorderWidth="0" />
            </td>
        </tr>
        <tr>
            <td align='center'>
                <table style='width: 160px'>
                    <tr>
                        <td align='right' style='padding: 6px 0px 6px 0px;'>
                            <input id="btnOk" runat="server" type='button' value='OK' title='OK' style='width: 60px;' />
                            <input id="btnCancel" runat="server" type='button' value='Cancel' title='Cancel'
                                style='width: 60px;' />
                        </td>
                    </tr>
                </table>
    </table>
</div>

