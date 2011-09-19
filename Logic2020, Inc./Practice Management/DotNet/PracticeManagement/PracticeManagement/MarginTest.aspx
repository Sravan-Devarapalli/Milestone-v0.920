<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MarginTest.aspx.cs" Inherits="PraticeManagement.MarginTest"
    MasterPageFile="~/PracticeManagementMain.Master" Title="One-Off Person Margin | Practice Management" %>

<%@ Register Src="Controls/GrossMarginComputing.ascx" TagName="GrossMarginComputing"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/PersonnelCompensation.ascx" TagName="PersonnelCompensation"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/RecruiterInfo.ascx" TagName="RecruiterInfo" TagPrefix="uc1" %>
<%@ Register Src="Controls/WhatIf.ascx" TagName="WhatIf" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="loadingProgress" TagPrefix="uc" %>

<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>One-Off Person Margin | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    One-Off Person Margin
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">        //<!--
        //Overriding the getDirty with a dummy one
        function getDirty() { return false; };
        //-->
    </script>
    <uc:loadingProgress ID="lpMarginTest" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <br />
            <table class="WholeWidth">
                <tr>
                    <td style="padding-top: 8px;" valign="top">
                        <table>
                            <tr>
                                <td colspan="2">
                                    <table>
                                        <tr>
                                            <td style="padding-left: 16px; padding-bottom:6px;">
                                                <asp:RadioButton ID="rbSelectPerson" OnCheckedChanged="rbMarginTest_CheckedChanged" Checked="true"
                                                    runat="server" Text="Select a Person" GroupName="marginTest" AutoPostBack="true" />
                                                <asp:Label ID="lblOr" runat="server" Text="OR" Style="padding-left: 20px; padding-right:20px; text-align: right;
                                                    font-weight: bold;"></asp:Label>
                                            </td>
                                            <td style="padding-bottom:6px;">
                                                <asp:RadioButton ID="rbDefineValues" OnCheckedChanged="rbMarginTest_CheckedChanged"
                                                    runat="server" Text="Define Values" GroupName="marginTest" AutoPostBack="true" />
                                            </td>
                                            <td align="right" style="padding-bottom:6px; padding-left:20px;">
                                                <asp:Button ID="btnReset" runat="server" OnClick="Reset_Clicked"
                                                    Text="Reset Form" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-left: 16px;">
                                    <asp:DropDownList ID="ddlPersonName" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPersonName_SelectedIndexChanged"
                                        Width="355px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-top: 5px; padding-left: 15px;">
                                    <uc1:PersonnelCompensation ID="personnelCompensation" runat="server" AutoPostBack="true"
                                        OnCompensationChanged="compensation_Changed" OnCompensationMethodChanged="compensation_Changed"
                                        OnPeriodChanged="compensation_Changed" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-left: 6px;">
                                    <%--<asp:Panel ID="pnlRecruiterInfo" runat="server">
                                        <uc1:RecruiterInfo ID="recruiterInfo" runat="server" ShowCommissionDetails="False"
                                            OnInfoChanged="compensation_Changed" />
                                    </asp:Panel>--%>
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
                                <td style="padding-left: 23px;">
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
                    <td valign="top" width="340px;" style="padding-left: 10px; padding-top: 10px; background-color: #DBEEF3;">
                        <uc1:GrossMarginComputing ID="grossMarginComputing" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

