<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FilteredCheckBoxList.ascx.cs"
    Inherits="PraticeManagement.Controls.FilteredCheckBoxList" %>
<script src="../Scripts/FilterTable.min.js" type="text/javascript"></script>
<script src="../Scripts/FilteredCheckBoxList.min.js" type="text/javascript"></script>
<div id='divFilterPopUp' runat="server" class="FilteredCheckBoxListdivFilterPopUp">
    <table class="WholeWidth" align="center">
        <tr>
            <td align='center' class='Width160px PaddingBottom6 PaddingTop6'>
                <asp:TextBox ID="txtSearchBox" runat="server" CssClass="Width155px" MaxLength="50"></asp:TextBox>
                <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmSearch" runat="server" TargetControlID="txtSearchBox"
                    WatermarkText="Search" WatermarkCssClass="watermarkedtext Width155px" BehaviorID="wmSearch" />
            </td>
        </tr>
        <tr>
            <td class='WholeWidth' align='center'>
                <pmc:ScrollingDropDown ID="cbl" runat="server" AllSelectedReturnType="Null" CssClass="FilteredCheckBoxListScrollingDropDown"
                    NoItemsType="All" SetDirty="False" />
            </td>
        </tr>
        <tr>
            <td align='center'>
                <table class='Width160px'>
                    <tr>
                        <td align='right' class='PaddingBottom6 PaddingTop6'>
                            <input id="btnOk" runat="server" type='button' value='OK' title='OK' class='Width60Px' />
                            <input id="btnCancel" runat="server" type='button' value='Cancel' title='Cancel'
                                class='Width60Px' />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdnSelectedIndexes" runat="server" />
</div>

