<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="CompensationDetail.aspx.cs" Inherits="PraticeManagement.CompensationDetail"
    Title="Practice Management - Compensation Details" %>

<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="Controls/PersonnelCompensation.ascx" TagName="PersonnelCompensation"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/PersonInfo.ascx" TagName="PersonInfo" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Compensation Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Compensation Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <cc1:StyledUpdatePanel ID="pnlBody" runat="server" CssClass="bg-light-frame">
        <ContentTemplate>
            <uc1:PersonInfo ID="personInfo" runat="server" />
            <table>
                <tr>
                    <td>
                        <uc1:PersonnelCompensation ID="personnelCompensation" runat="server" OnCompensationMethodChanged="personnelCompensation_CompensationMethodChanged"
                            OnPeriodChanged="personnelCompensation_PeriodChanged" />
                    </td>
                </tr>
                <tr>
                    <asp:TextBox ID="txtDummy" runat="server" Style="display: none"></asp:TextBox>
                    <asp:CustomValidator ID="custdateRangeBegining" runat="server" EnableClientScript="false"
                        ErrorMessage="The Start Date is incorrect. There are several other compensation records for the specified period. Please edit them first."
                        ToolTip="The Start Date is incorrect. There are several other compensation records for the specified period. Please edit them first."
                        ControlToValidate="txtDummy" ValidateEmptyText="True" Text="*" OnServerValidate="custdateRangeBegining_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custdateRangeEnding" runat="server" EnableClientScript="false"
                        ErrorMessage="The End Date is incorrect. There are several other compensation records for the specified period. Please edit them first."
                        ToolTip="The End Date is incorrect. There are several other compensation records for the specified period. Please edit them first."
                        ControlToValidate="txtDummy" ValidateEmptyText="True" Text="*" OnServerValidate="custdateRangeEnding_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custdateRangePeriod" runat="server" EnableClientScript="false"
                        ErrorMessage="The period is incorrect. There records falls into the period specified in an existing record."
                        ToolTip="The period is incorrect. There records falls into the period specified in an existing record."
                        ControlToValidate="txtDummy" ValidateEmptyText="True" Text="*" OnServerValidate="custdateRangePeriod_ServerValidate"></asp:CustomValidator>
                </tr>
            </table>
        </ContentTemplate>
    </cc1:StyledUpdatePanel>
    <div class="buttons-block">
        <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
        <asp:ValidationSummary ID="vsumCompensation" runat="server" EnableClientScript="false" />
        <cc:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />&nbsp;
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        <div class="clear0">
        </div>
    </div>
</asp:Content>

