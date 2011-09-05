<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="BecomeUser.aspx.cs" Inherits="PraticeManagement.Config.BecomeUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Become User | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Become User
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <div id="dvBecomeUser" runat="server" visible="false" style="display: block">
        <table width="80%">
            <tr>
                <td style="width: 30%;">
                    <table>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlBecomeUserList" runat="server" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Button ID="lbBecomeUserOk" runat="server" OnClick="lbBecomeUserOk_OnClick" Visible="false"
                                    Text="Become" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width: 5%;">
                </td>
                <td style="width: 65%;">
                    <div style="border: 1px solid black; background-color: White; padding: 5px;">
                        <p>
                            To help troubleshoot issues that anothor person may be having in Practice Management
                            you can use the Become User functionality.
                        </p>
                        <p>
                            &nbsp;
                        </p>
                        <p>
                            Choose an active person from the drop-down, then click the "Become" button to open
                            PM in a new window as the selected person.</p>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

