<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultUser.ascx.cs"
    Inherits="PraticeManagement.Controls.Configuration.DefaultUser" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.Filtering"
    TagPrefix="cc1" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<asp:UpdatePanel ID="updDefaultManager" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="WholeWidth" id="tbDefaultUser" runat="server">
            <tr>
                <td>
                    <asp:DropDownList ID="ddlActivePersons" runat="server"
                        onchange="setDirty();" CssClass="WholeWidth" OnDataBound="ddlActivePersons_OnDataBound" />
                </td>
            </tr>
            <tr>
                <td class="textRight PersonDefaultUser">
                    <asp:Button ID="btnSetDefault" runat="server" OnClick="btnSetDefault_Click" Visible="false" CssClass="marginLineManager"
                        Text="Save Default Career Manager" />
                </td>
            </tr>
        </table>
        <%--<asp:LinkButton ID="btnSetDefault" runat="server" OnClick="btnSetDefault_Click" Visible="false">Set as def. Career Manager</asp:LinkButton>--%>
        <uc:MessageLabel ID="mlMessage" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
            WarningColor="Orange" EnableViewState="false" />
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ddlActivePersons" EventName="SelectedIndexChanged" />
    </Triggers>
</asp:UpdatePanel>

