<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="DefaultLineManager.aspx.cs" Inherits="PraticeManagement.Config.DefaultLineManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Default Career Manager | Practice Management</title>
</asp:Content>
<%@ Register Src="~/Controls/Configuration/DefaultUser.ascx" TagPrefix="uc" TagName="DefaultManager" %>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Default Career Manager
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <table class="WholeWidth">
        <tr>
            <td class="Width50Percent">
                <uc:DefaultManager ID="defaultManager" runat="server" AllowChange="true" PersonsRole="Practice Area Manager" />
            </td>
            <td class="Width50Percent padLeft40 vTop">
                <span class="DefaultLineManagerSpan">Once selected,
                    the Default Career Manager is the person that all users in PM<br />
                    will default to in the event that they are either not assigned a Career Manager<br />
                    or if their assigned Career Manager is made Inactive or Terminated. </span>
            </td>
        </tr>
    </table>
</asp:Content>

