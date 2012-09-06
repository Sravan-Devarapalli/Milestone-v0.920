﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultUser.ascx.cs"
    Inherits="PraticeManagement.Controls.Configuration.DefaultUser" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.Filtering"
    TagPrefix="cc1" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<asp:UpdatePanel ID="updDefaultManager" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="WholeWidth" id="tbDefaultUser" runat="server">
            <tr>
                <td>
                    <asp:DropDownList ID="ddlActivePersons" runat="server" DataSourceID="odsPersons"
                        onchange="setDirty();" CssClass="WholeWidth" DataTextField="PersonLastFirstName" DataValueField="Id"
                        OnDataBound="ddlActivePersons_OnDataBound" />
                </td>
            </tr>
            <tr>
                <td class="textRight PersonDefaultUser">
                    <asp:Button ID="btnSetDefault" runat="server" OnClick="btnSetDefault_Click" Visible="false" CssClass="marginLineManager"
                        Text="Save Default Career Counselor" />
                </td>
            </tr>
        </table>
        <%--<asp:LinkButton ID="btnSetDefault" runat="server" OnClick="btnSetDefault_Click" Visible="false">Set as def. Career Counselor</asp:LinkButton>--%>
        <asp:ObjectDataSource ID="odsPersons" runat="server" SelectMethod="PersonListShortByRoleAndStatus"
            TypeName="PraticeManagement.PersonService.PersonServiceClient" OnSelecting="odsPersons_OnSelecting"
            OnSelected="odsPersons_OnSelected" CacheDuration="5" EnableCaching="true">
            <SelectParameters>
                <asp:Parameter DefaultValue="1" Name="statusId" Type="Int32" />
                <asp:Parameter Name="roleName" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <uc:MessageLabel ID="mlMessage" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
            WarningColor="Orange" EnableViewState="false" />
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ddlActivePersons" EventName="SelectedIndexChanged" />
    </Triggers>
</asp:UpdatePanel>

