<%@ Page Title="Skills Search | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsSearch.aspx.cs" Inherits="PraticeManagement.SkillsSearch" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" language="javascript">
        function ClearSearchText() {
            var txtSearch = document.getElementById('<%# txtSearch.ClientID %>');
            txtSearch.value = '';
        }

        function ChangeDefaultFocusToSearchButton(e) {
            if (window.event) {
                e = window.event;
            }
            if (e.keyCode == 13) {
                var btn = document.getElementById('<%= btnSearch.ClientID %>');
                btn.click();

            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <table class="WholeWidthImp TextAlignCenterImp">
        <tr>
            <td colspan="2" class="TextAlignCenterImp PaddingBottom20Px">
                <h1>
                    <asp:Label ID="lblSearchTitle" runat="server"></asp:Label>
                </h1>
            </td>
        </tr>
    </table>
    <div class="SkillsSearchBody">
        <div class="SkillsSearchDataBody TextAlignCenterImp">
            <asp:Panel ID="pnlSearch" runat="server">
                <table class="Width90PercentImp">
                    <tr>
                        <td colspan="2" class="TextAlignCenterImp PaddingBottom20Px">
                            Please either enter skills separated by commas or choose an employee to see the
                            relevant skills profiles.
                        </td>
                    </tr>
                    <tr>
                        <td class="TextSkillSearch">
                            <br />
                            <b>Skills search. Enter skills separated by commas.</b>
                            <table class="Width90PercentImp MarginTop20Px">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtSearch" runat="server" CssClass="Width965Per" onkeypress="ChangeDefaultFocusToSearchButton(event);"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqSearch" runat="server" ValidationGroup="Search"
                                            ControlToValidate="txtSearch" Text="*" ToolTip="Please Enter skills separated by commas."
                                            ErrorMessage="Please Enter skills separated by commas."></asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="height20P">
                                        <asp:ValidationSummary ID="valSumSearch" runat="server" ValidationGroup="Search" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="textRightImp">
                                        <button id="btnClear" type="button" onclick="ClearSearchText();return false;">
                                            Clear</button>
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_OnClick"
                                            UseSubmitBehavior="false" EnableViewState="false" ValidationGroup="Search" CausesValidation="true" />
                                        &nbsp;&nbsp;
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="paddingLeft15Imp PaddingBottom20Px vTopImp t-left">
                            <br />
                            <b>Employees</b>
                            <table class="MarginTop20Px">
                                <tr>
                                    <td class="PaddingBottom20Px">
                                        <asp:DropDownList ID="ddlEmployees" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="textRightImp">
                                        <asp:Button ID="btnEmployeeOK" runat="server" Text="  OK  " OnClick="btnEmployeeOK_OnClick" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlSearchResults" runat="server" Visible="false">
                <div class="OverFlowAutoImp TextAlignCenterImp">
                    <table class="Width90PercentImp">
                        <tr>
                            <td class="TextAlignCenterImp">
                                <h1>
                                    <asp:Label ID="lblSearchResultsTitle" runat="server"></asp:Label>
                                </h1>
                            </td>
                        </tr>
                        <tr>
                            <td class="LblSeachCriteria" colspan="2">
                                <asp:Label ID="lblSearchcriteria" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="t-left padLeft20Imp">
                                <div id="dlPersonDiv" class="searchResult" runat="server">
                                    <asp:DataList ID="dlPerson" runat="server" CssClass="WholeWidthImp">
                                        <ItemStyle CssClass="height20PImp" />
                                        <AlternatingItemStyle CssClass="alterrow height20PImp" />
                                        <ItemTemplate>
                                            <asp:HyperLink NavigateUrl='<%# GetSkillProfileUrl(((DataTransferObjects.Person)Container.DataItem).Id.ToString()) %>'
                                                Text="<%# GetPersonFirstLastName(((DataTransferObjects.Person)Container.DataItem))  %>"
                                                runat="server" Target="_blank"></asp:HyperLink>
                                        </ItemTemplate>
                                        <AlternatingItemTemplate>
                                            <asp:HyperLink NavigateUrl='<%# GetSkillProfileUrl(((DataTransferObjects.Person)Container.DataItem).Id.ToString()) %>'
                                                Text="<%# GetPersonFirstLastName(((DataTransferObjects.Person)Container.DataItem))%>"
                                                runat="server" Target="_blank"></asp:HyperLink>
                                        </AlternatingItemTemplate>
                                    </asp:DataList>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

