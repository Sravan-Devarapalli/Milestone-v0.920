<%@ Page Language="C#" MasterPageFile="~/TimeManagement.Master" AutoEventWireup="true" CodeBehind="PersonTimeEntry.aspx.cs" Inherits="PraticeManagement.PersonTimeEntry" Title="Untitled Page" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
	<title>Person Time Entry</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
	Person Time Entry
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
	<table>
		<tr>
			<td>
				<asp:Calendar ID="calSelection" runat="server" Font-Size="X-Small"></asp:Calendar>
			</td>
			<td rowspan="2" valign="top">
				<table width="100%">
					<tr>
						<td>Person Name</td>
						<td>
							<asp:DropDownList ID="ddlPerson" runat="server">
								<asp:ListItem Text="Sean R. Cunningham"></asp:ListItem>
							</asp:DropDownList>
						</td>
						<td align="right">
						</td>
					</tr>
					<tr>
						<td colspan="3">
							<table>
								<tr>
									<td>
										<asp:ImageButton ID="btnPraviousWeek" runat="server" ImageUrl="~/Images/previous.gif" />
									</td>
									<td>
										<table>
											<tr>
												<td><span style="font-size: xx-small">Week of</span></td>
												<td rowspan="2">
													<asp:ImageButton ID="btnCalendar" runat="server" ImageUrl="~/Images/calendar.gif" OnClientClick="return false;" />
												</td>
											</tr>
											<tr>
												<td nowrap="nowrap">
													<asp:Label ID="lblWeek" runat="server" EnableViewState="False" 
														Font-Size="X-Large" Font-Bold="True"></asp:Label>
												</td>
											</tr>
										</table>
									</td>
									<td>
										<asp:ImageButton ID="btnNextWeek" runat="server" ImageUrl="~/Images/next.gif" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td colspan="3" style="padding: 10px 20px 0px 20px">
							<asp:GridView ID="gvWeeklyView" runat="server" AutoGenerateColumns="false" 
								ShowFooter="true" CssClass="timeEntry">
								<Columns>
									<asp:TemplateField HeaderText="Project" HeaderStyle-Width="250px">
										<ItemTemplate>
											<asp:DropDownList ID="ddlProject" runat="server" AutoPostBack="true">
												<asp:ListItem Text="[ select a project ]"></asp:ListItem>
												<asp:ListItem Text="Logic-prac-mgm Practice Management"></asp:ListItem>
											</asp:DropDownList>
										</ItemTemplate>
										<FooterTemplate>
											<asp:LinkButton ID="btnAddRows" runat="server" Text="+ Add Rows"></asp:LinkButton>
										</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Milestone" HeaderStyle-Width="150px">
										<ItemTemplate>
											<asp:DropDownList ID="ddlTask" runat="server">
												<asp:ListItem Text="[ select a milestone ]"></asp:ListItem>
												<asp:ListItem Text="Milestone 1"></asp:ListItem>
											</asp:DropDownList>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Task" HeaderStyle-Width="150px">
										<ItemTemplate>
											<asp:DropDownList ID="ddlTask" runat="server">
												<asp:ListItem Text="[ select a task ]"></asp:ListItem>
												<asp:ListItem Text="Dev - Software Dev"></asp:ListItem>
											</asp:DropDownList>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Sun" Visible="false">
										<ItemTemplate>
											<asp:TextBox ID="txtSunday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Mon" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtMonday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Tue" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtTuesday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Wed" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtWednesday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Thu" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtThursday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Fri" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtFriday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Sat" Visible="false">
										<ItemTemplate>
											<asp:TextBox ID="txtSaturday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Total" ItemStyle-CssClass="timeEntryRight" HeaderStyle-CssClass="timeEntryRight">
										<ItemTemplate>
											<asp:Label ID="lblTotal" runat="server" Text="0.00"></asp:Label>
										</ItemTemplate>
									</asp:TemplateField>
								</Columns>
							</asp:GridView>
						</td>
					</tr>
					<tr>
						<td colspan="3" style="padding: 0px 20px 0px 20px">Time off</td>
					</tr>
					<tr>
						<td colspan="3" style="padding: 0px 20px 10px 20px">
							<asp:GridView ID="gvTimeOff" runat="server" AutoGenerateColumns="false" 
								ShowFooter="true" ShowHeader="false" CssClass="timeEntry">
								<Columns>
									<asp:TemplateField ItemStyle-Width="574px">
										<ItemTemplate>
											<asp:DropDownList ID="ddlLeaveType" runat="server">
												<asp:ListItem Text="[ select a leave type ]"></asp:ListItem>
											</asp:DropDownList>
										</ItemTemplate>
										<FooterTemplate>
											<asp:LinkButton ID="btnAddRow" runat="server" Text="+ Add Row"></asp:LinkButton>											
										</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Sun" Visible="false">
										<ItemTemplate>
											<asp:TextBox ID="txtSunday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Mon" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtMonday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Tue" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtTuesday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Wed" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtWednesday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Thu" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtThursday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Fri" Visible="true">
										<ItemTemplate>
											<asp:TextBox ID="txtFriday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Sat" Visible="false">
										<ItemTemplate>
											<asp:TextBox ID="txtSaturday" runat="server"></asp:TextBox>
										</ItemTemplate>
										<FooterTemplate>&nbsp;</FooterTemplate>
									</asp:TemplateField>
									<asp:TemplateField HeaderText="Total" ItemStyle-CssClass="timeEntryRight" HeaderStyle-CssClass="timeEntryRight">
										<ItemTemplate>
											<asp:Label ID="lblTotal" runat="server" Text="0.00"></asp:Label>
										</ItemTemplate>
									</asp:TemplateField>
								</Columns>
							</asp:GridView>
						</td>
					</tr>
					<tr>
						<td colspan="3" style="padding: 10px 20px 0px 20px">
							<table border="1" class="timeEntryRight">
								<tr>
									<td style="width: 574px">&nbsp;</td>
									<td>0.00</td>
									<td>0.00</td>
									<td>0.00</td>
									<td>0.00</td>
									<td>0.00</td>
									<td>0.00</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td align="center">
				<asp:Button ID="btnToday" runat="server" Text="Today" 
					onclick="btnToday_Click" />
			</td>
		</tr>
	</table>
</asp:Content>

