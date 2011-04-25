<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectNameCell.ascx.cs" Inherits="PraticeManagement.Controls.ProjectNameCell" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>

<asp:HyperLink ID="btnProjectName" runat="server"/>
    <asp:Panel ID="toolTipHolder" runat="server" CssClass="ToolTip">
        <pre style="margin: 5px"><asp:Label ID="lblTooltipContent" runat="server"></asp:Label></pre>
    </asp:Panel>
<AjaxControlToolkit:HoverMenuExtender ID="hm" runat="server" 
        TargetControlID="btnProjectName" 
        PopupControlID="toolTipHolder" 
        PopupPosition="Right" />
