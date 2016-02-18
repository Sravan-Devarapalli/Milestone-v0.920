<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Divisions.aspx.cs" Inherits="PraticeManagement.Config.Divisions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Divisions | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Divisions
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <asp:GridView ID="gvDivisions" runat="server" AutoGenerateColumns="false" CssClass="CompPerfTable gvDivisions"
        OnRowDataBound="gvDivisions_RowDataBind" RowStyle-Height="30px" BorderStyle="Solid" GridLines="None">
        <AlternatingRowStyle CssClass="alterrow" />
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="ie-bg">
                        &nbsp;
                    </div>
                </HeaderTemplate>
                <HeaderStyle CssClass="Width10Percent" />
                <ItemTemplate>
                    <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/Images/icon-edit.png" OnClick="imgEdit_OnClick"
                        ToolTip="Edit Division" />
                    <asp:HiddenField ID="hdDivisionId" runat="server" Value='<%#Bind("DivisionId")%>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:HiddenField ID="hdDivisionId" runat="server" Value='<%#Bind("DivisionId")%>' />
                    <asp:ImageButton ID="imgUpdate" runat="server" ImageUrl="~/Images/icon-check.png"
                        OnClick="imgUpdate_OnClick" ToolTip="Confirm" />
                    <asp:ImageButton ID="imgCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="imgCancel_OnClick"
                        ToolTip="Cancel" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Division Name" SortExpression="Name">
                <HeaderStyle CssClass="Width45Percent padRight10" />
                <ItemStyle CssClass="Left no-wrap padRight10" />
                <ItemTemplate>
                    <asp:Label ID="lblDivision" runat="server" CssClass="WS-Normal" Text='<%# Bind("HtmlEncodedName") %>' />
                </ItemTemplate>
                <%--<EditItemTemplate>
                    <asp:Label ID="lblDivision" runat="server" CssClass="WS-Normal" Text='<%# Bind("HtmlEncodedName") %>' />
                </EditItemTemplate>--%>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle CssClass="Width45Percent" />
                <ItemStyle CssClass="Left" />
                <HeaderTemplate>
                    Division Owner
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblDivisionOwner" runat="server"></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlActivePersons" runat="server" CssClass="Width95Percent">
                    </asp:DropDownList>
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>

