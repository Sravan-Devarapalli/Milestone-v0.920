<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="DefaultLineManager.aspx.cs" Inherits="PraticeManagement.Config.DefaultLineManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Default Career Counselor | Practice Management</title>
</asp:Content>
<%@ Register Src="~/Controls/Configuration/DefaultUser.ascx" TagPrefix="uc" TagName="DefaultManager" %>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Default Career Counselor
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <table width="100%">
        <tr>
            <td style="width: 50%">
                <uc:DefaultManager ID="defaultManager" runat="server" AllowChange="true" PersonsRole="Practice Area Manager" />
            </td>
            <td style="width: 50%; padding-left: 40px; vertical-align: top;">
                <span style="background-color:White; border: 1px solid black; padding: 8px; display:block; width:430px;">Once selected,
                    the Default Career Counselor is the person that all users in PM<br />
                    will default to in the event that they are either not assigned a Career Counselor<br />
                    or if their assigned Career Counselor is made Inactive or Terminated. </span>
            </td>
        </tr>
    </table>
</asp:Content>

