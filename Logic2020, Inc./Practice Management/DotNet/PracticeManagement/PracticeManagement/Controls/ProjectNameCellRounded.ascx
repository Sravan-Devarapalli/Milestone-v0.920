<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectNameCellRounded.ascx.cs" Inherits="PraticeManagement.Controls.ProjectNameCellRounded" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>

<asp:HyperLink ID="btnProjectName" runat="server"/>
    <asp:Panel ID="toolTipHolder" runat="server" CssClass="ToolTip">
        <table>
            <tr class="top">
                <td class="lt">
                    <div class="tail"></div>
                </td>
                <td class="tbor"></td>
                <td class="rt"></td>
            </tr>
            <tr class="middle">
                <td class="lbor"></td>
                <td class="content">
                    <pre style="margin: 5px"><asp:Label ID="lblTooltipContent" runat="server"></asp:Label></pre>
                </td>
                <td class="rbor"></td>
            </tr>
            <tr class="bottom">
                <td class="lb"></td>
                <td class="bbor"></td>
                <td class="rb"></td>
            </tr>
        </table>
    </asp:Panel>
<AjaxControlToolkit:HoverMenuExtender ID="hm" runat="server" 
        TargetControlID="btnProjectName" 
        PopupControlID="toolTipHolder" 
        PopupPosition="Right"/>
