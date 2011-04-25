<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MonthCalendar.ascx.cs" Inherits="PraticeManagement.Controls.MonthCalendar" %>

<asp:UpdatePanel ID="pnlMonth" runat="server">
	<ContentTemplate>
		<asp:DataList ID="lstCalendar" runat="server" RepeatColumns="7" RepeatDirection="Horizontal">
			<HeaderTemplate>
				</td>
				</tr>
				<tr>
					<th>Sun</th>
					<th>Mon</th>
					<th>Tue</th>
					<th>Wed</th>
					<th>Thu</th>
					<th>Fri</th>
					<th>Sat</th>
				</tr>
				<tr>
				<td colspan="7">
			</HeaderTemplate>
			<ItemStyle HorizontalAlign="Center" />
			<ItemTemplate>
				<asp:Panel ID="pnlDay" runat="server"
					CssClass='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year ? ((bool)Eval("DayOff") ? ((bool)Eval("CompanyDayOff") ? "DayOff" : "CompanyDayOn") : ((bool)Eval("CompanyDayOff") ? "CompanyDayOff" : "DayOn")) : "DayGrayed" %>'>
					<asp:LinkButton ID="btnDay" runat="server" Text='<%# Eval("Date.Day") %>'
						OnCommand="btnDay_Command"
						CommandName='<%# (bool)Eval("DayOff") ? false : true %>'
						CommandArgument='<%# Eval("Date") %>'
						Visible='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year && !(bool)Eval("ReadOnly") %>'
						OnClientClick='<%# DayOnClientClick() %>'></asp:LinkButton>
					<asp:Label ID="lblDay" runat="server" Text='<%# Eval("Date.Day") %>'
						Visible='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year && (bool)Eval("ReadOnly") %>'></asp:Label>
					<asp:Label ID="lblDayOut" runat="server" Text='<%# Eval("Date.Day") %>'
						Visible='<%# ((DateTime)Eval("Date")).Month != Month || ((DateTime)Eval("Date")).Year != Year %>'></asp:Label>
				</asp:Panel>
			</ItemTemplate>
		</asp:DataList>
	</ContentTemplate>
</asp:UpdatePanel>
<AjaxControlToolkit:UpdatePanelAnimationExtender ID="pnlMonth_UpdatePanelAnimationExtender" 
	runat="server" Enabled="True" TargetControlID="pnlMonth">
	<Animations>
		<OnUpdated>
			<ScriptAction Script="hideInProcessImage($get('divWait'));" />
		</OnUpdated>
	</Animations>
</AjaxControlToolkit:UpdatePanelAnimationExtender>


