<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ScrollingDropdownTest.aspx.cs" Inherits="PraticeManagement.Sandbox.ScrollingDropdownTest" %>

<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <cc2:ScrollingDropDown ID="cblPersons" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
        BackColor="White" CellPadding="3" NoItemsType="All" SetDirty="False" Width="350px"
        BorderWidth="0" />
    <ext:ScrollableDropdownExtender ID="sdePersons" runat="server" TargetControlID="cblPersons"
        DisplayText="Please Select Person" EditImageUrl="~/Images/Dropdown_Arrow.png">
    </ext:ScrollableDropdownExtender>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

