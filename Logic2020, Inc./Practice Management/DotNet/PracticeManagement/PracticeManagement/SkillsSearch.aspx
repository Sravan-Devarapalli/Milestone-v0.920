﻿<%@ Page Title="Skills Search | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsSearch.aspx.cs" Inherits="PraticeManagement.SkillsSearch" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .SkillsBody
        {
            background-color: #d4dff8;
            padding: 10px;
            padding-top: 25px !important;
            border-color: -moz-use-text-color #999999 #999999;
            border: 1px solid #999999;
        }
        
        .SkillsDataBody
        {
            background-color: White;
            height: 430px;
            padding-left: 10%;
            padding-top: 1%;
            text-align: center;
        }
        .searchResult
        {
            height: 150px;
            overflow: auto;
            width: 100%;
        }
    </style>
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
    <table style="width: 100%; text-align: center;">
        <tr>
            <td align="center" colspan="2" style="padding-bottom: 20px;">
                <h1>
                    <asp:Label ID="lblSearchTitle" runat="server"></asp:Label>
                </h1>
            </td>
        </tr>
    </table>
    <div class="SkillsBody">
        <div style="text-align: center;" class="SkillsDataBody">
            <asp:Panel ID="pnlSearch" runat="server">
                <table style="width: 90%">
                    <tr>
                        <td align="center" style="padding-bottom: 20px;" colspan="2">
                            Please either enter skills separated by commas or choose an employee to see the
                            relevant skills profiles.
                        </td>
                    </tr>
                    <tr>
                        <td style="border-right: 1px solid black; width: 55%; padding-bottom: 20px;" valign="top"
                            align="left">
                            <br />
                            <b>Skills search. Enter skills separated by commas.</b>
                            <table style="width: 90%; margin-top: 20px;">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtSearch" runat="server" Style="width: 96.5%" onkeypress="ChangeDefaultFocusToSearchButton(event);"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqSearch" runat="server" ValidationGroup="Search"
                                            ControlToValidate="txtSearch" Text="*" ToolTip="Please Enter skills separated by commas."
                                            ErrorMessage="Please Enter skills separated by commas."></asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 20px;">
                                        <asp:ValidationSummary ID="valSumSearch" runat="server" ValidationGroup="Search" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
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
                        <td style="padding-left: 15px; padding-bottom: 20px;" valign="top" align="left">
                            <br />
                            <b>Employees</b>
                            <table style="margin-top: 20px;">
                                <tr>
                                    <td style="padding-bottom: 20px;">
                                        <asp:DropDownList ID="ddlEmployees" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
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
                <div style="text-align: center; overflow: auto !important;">
                    <table style="width: 90%">
                        <tr>
                            <td align="center">
                                <h1>
                                    <asp:Label ID="lblSearchResultsTitle" runat="server"></asp:Label>
                                </h1>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 20px 0px 15px 20px;" colspan="2" align="left">
                                <asp:Label ID="lblSearchcriteria" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" style="padding-left: 20px;">
                                <div id="dlPersonDiv" class="searchResult" runat="server">
                                    <asp:DataList ID="dlPerson" runat="server" ItemStyle-Height="20px" AlternatingItemStyle-BackColor="#f9faff"
                                        Width="100%">
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

