<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MarginTest.aspx.cs" Inherits="PraticeManagement.MarginTest"
    MasterPageFile="~/PracticeManagementMain.Master" Title="Margin Test | Practice Management" %>

<%@ Register Src="Controls/GrossMarginComputing.ascx" TagName="GrossMarginComputing"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/PersonnelCompensation.ascx" TagName="PersonnelCompensation"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/WhatIf.ascx" TagName="WhatIf" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="loadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Margin Test | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    One-Off Person Margin
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:loadingProgress ID="lpMarginTest" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <br />
            <table class="WholeWidth">
                <tr>
                    <td class="PaddingTop8 vTop">
                        <table>
                            <tr>
                                <td colspan="2">
                                    <table>
                                        <tr>
                                            <td class="padLeft16 PaddingBottom6">
                                                <asp:RadioButton ID="rbSelectPerson" OnCheckedChanged="rbMarginTest_CheckedChanged"
                                                    Checked="true" runat="server" Text="Select a Person" GroupName="marginTest" AutoPostBack="true" />
                                                <asp:Label ID="lblOr" runat="server" Text="OR" CssClass="MarginTestLabel"></asp:Label>
                                            </td>
                                            <td class="padLeft16 PaddingBottom6">
                                                <asp:RadioButton ID="rbSelectStrawman" OnCheckedChanged="rbMarginTest_CheckedChanged"
                                                    Checked="false" runat="server" Text="Select a Strawman" GroupName="marginTest"
                                                    AutoPostBack="true" />
                                                <asp:Label ID="lblOr1" runat="server" Text="OR" CssClass="MarginTestLabel"></asp:Label>
                                            </td>
                                            <td class="PaddingBottom6">
                                                <asp:RadioButton ID="rbDefineValues" OnCheckedChanged="rbMarginTest_CheckedChanged"
                                                    runat="server" Text="Define Values" GroupName="marginTest" AutoPostBack="true" />
                                            </td>
                                            <td class="PaddingBottom6 padLeft20 textRight">
                                                <asp:Button ID="btnReset" runat="server" OnClick="Reset_Clicked" Text="Reset Form"
                                                    CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="padLeft16">
                                    <asp:DropDownList ID="ddlPersonName" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPersonName_SelectedIndexChanged"
                                        CssClass="Width355Px">
                                    </asp:DropDownList>
                                    <asp:DropDownList ID="ddlStrawmanName" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPersonName_SelectedIndexChanged"
                                        CssClass="Width355Px" Visible="false">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="PaddingTop5 padLeft15">
                                    <uc1:PersonnelCompensation ID="personnelCompensation" runat="server" AutoPostBack="true"
                                        IsMarginTestPage="true" OnCompensationChanged="compensation_Changed" OnCompensationMethodChanged="compensation_Changed"
                                        OnPeriodChanged="compensation_Changed" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="padLeft6">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                        <table>
                            <tr>
                                <td class="padLeft23">
                                    <uc1:WhatIf ID="whatIf" runat="server" DisplayTargetMargin="true" IsMarginTestPage="true" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:ValidationSummary ID="vsumPersonMargin" runat="server" EnableClientScript="false" />
                                    <asp:ValidationSummary ID="vsumComputeRate" runat="server" EnableClientScript="false"
                                        ValidationGroup="ComputeRate" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="WhatIfGrossMarginComputing">
                        <uc1:GrossMarginComputing ID="grossMarginComputing" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

