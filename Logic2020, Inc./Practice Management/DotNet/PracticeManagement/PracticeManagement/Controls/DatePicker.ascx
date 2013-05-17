<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatePicker.ascx.cs" Inherits="PraticeManagement.Controls.DatePicker" %>

<table cellpadding="0" cellspacing="0" class="DatePickerTable">
	<tr>
		<td>
			<asp:TextBox ID="txtDate" runat="server" OnTextChanged="txtDate_TextChanged" ></asp:TextBox>
            <AjaxControlToolkit:FilteredTextBoxExtender ID="fte" runat="server" FilterMode="ValidChars" FilterType="Custom, Numbers"   TargetControlID="txtDate" ValidChars="/" ></AjaxControlToolkit:FilteredTextBoxExtender>
		</td>
        <td class="no-wrap">&nbsp;</td>
		<td class="vMiddle">
			<asp:HyperLink ID="lnkCalendar" runat="server" ImageUrl="~/Images/calendar.gif" NavigateUrl="#"></asp:HyperLink>
		</td>
		<td>
			<asp:RangeValidator ID="dateRangeVal" runat="server" ControlToValidate="txtDate"
				Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static" Type="Date"></asp:RangeValidator>
		</td>
	</tr>
</table>
<AjaxControlToolkit:CalendarExtender ID="txtDate_CalendarExtender"  runat="server" TargetControlID="txtDate" PopupButtonID="lnkCalendar"></AjaxControlToolkit:CalendarExtender>
