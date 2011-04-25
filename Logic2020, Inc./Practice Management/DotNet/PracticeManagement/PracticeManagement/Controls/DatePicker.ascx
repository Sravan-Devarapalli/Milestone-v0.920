<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatePicker.ascx.cs" Inherits="PraticeManagement.Controls.DatePicker" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<table border="0" cellpadding="0" cellspacing="0" style="display: inline; border: none;">
	<tr>
		<td>
			<asp:TextBox ID="txtDate" runat="server" OnTextChanged="txtDate_TextChanged" onchange="setDirty();"></asp:TextBox>
		</td>
        <td style="white-space:nowrap;">&nbsp;</td>
		<td valign="middle">
			<asp:HyperLink ID="lnkCalendar" runat="server" ImageUrl="~/Images/calendar.gif" NavigateUrl="#"></asp:HyperLink>
		</td>
		<td>
			<asp:RangeValidator ID="dateRangeVal" runat="server" ControlToValidate="txtDate"
				Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static" Type="Date"></asp:RangeValidator>
		</td>
	</tr>
</table>
<ajaxToolkit:CalendarExtender ID="txtDate_CalendarExtender" runat="server" TargetControlID="txtDate" PopupButtonID="lnkCalendar"></ajaxToolkit:CalendarExtender>

