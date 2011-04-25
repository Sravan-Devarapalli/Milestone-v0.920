<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectInfo.ascx.cs" Inherits="PraticeManagement.Controls.ProjectInfo" %>
<table>
	<tr>
		<td class="ProjectDetailBlackLarge">
			<asp:Label ID="lblClientName" runat="server" CssClass="ProjectDetailBlue"/>&nbsp;-
			<asp:Hyperlink ID="linkProjectName" runat="server" CssClass="ProjectDetailBlue" onclick="return checkDirtyWithRedirect(this.href);" />
		</td>
		<td>
		&nbsp;&nbsp;
		</td>
		<td>
			[From <asp:Label ID="lblStartDate" runat="server" Font-Bold="true" />
			&nbsp;to&nbsp;
			<asp:Label ID="lblEndDate" runat="server" Font-Bold="true" />]
		</td>
	</tr>
</table>


