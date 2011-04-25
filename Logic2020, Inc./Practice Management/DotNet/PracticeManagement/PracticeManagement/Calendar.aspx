<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="PraticeManagement.Calendar" 
MasterPageFile="~/PracticeManagementMain.Master" Title="Company Calendar" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>
<%@ Register src="Controls/MonthCalendar.ascx" tagname="MonthCalendar" tagprefix="uc1" %>
<%@ Register src="Controls/CalendarLegend.ascx" tagname="CalendarLegend" tagprefix="uc2" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
	<title>Practice Management - Calendar</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
	<asp:UpdatePanel ID="pnlHeader" runat="server">
		<ContentTemplate>
			Calendar&nbsp;-&nbsp;<asp:Label ID="lblCalendarOwnerName" runat="server" Text="Whole Company" EnableViewState="false"></asp:Label>
		</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
	<br>
	<uc2:CalendarLegend ID="CalendarLegend" runat="server"/>
	<br>
	<script type="text/javascript">
		var updatingCalendarContainer = null;
	</script>
	<asp:UpdatePanel ID="pnlBody" runat="server" ChildrenAsTriggers="False" 
		UpdateMode="Conditional">
		<ContentTemplate>
			<div id="divWait" style="display: none; background-color: White; border: solid 1px silver;">
                <span style="color: Black;font-weight: bold;"><nobr>Please Wait...</nobr></span><br />
                <img id="imgLoading" name="imgLoading" alt="Please Wait..." src="./Images/ajax-loader.gif" />
            </div>
			<table class="CalendarTable">
				<tr>
					<td align="right">Select a Person:</td>
					<td colspan="2" nowrap="nowrap">
						<asp:DropDownList ID="ddlPerson" runat="server"></asp:DropDownList>
						<asp:UpdatePanel ID="pnlButton" runat="server" RenderMode="Inline">
							<ContentTemplate>
								<asp:Button ID="btnRetrieveCalendar" runat="server" Text="Retrieve Calendar"
									OnClick="btnRetrieveCalendar_Click"/>
							</ContentTemplate>
						</asp:UpdatePanel>
						<AjaxControlToolkit:UpdatePanelAnimationExtender ID="pnlButton_UpdatePanelAnimationExtender" 
							runat="server" Enabled="True" TargetControlID="pnlButton">
							<Animations>
								<OnUpdating>
									<EnableAction AnimationTarget="btnRetrieveCalendar" Enabled="false" />
								</OnUpdating>
								<OnUpdated>
									<EnableAction AnimationTarget="btnRetrieveCalendar" Enabled="true" />
								</OnUpdated>
							</Animations>
						</AjaxControlToolkit:UpdatePanelAnimationExtender>
					</td>
				</tr>
				<tr>
					<td colspan="3" align="left">
						<asp:Label ID="lblConsultantMessage" runat="server" Visible="false"
							Text="You can review your vacation days, but cannot change them. Please see your Practice Manager for updates to your vacation schedule."></asp:Label>
					</td>
				</tr>
				<tr>
					<td colspan="3" align="center">
						<table>
							<tr>
								<td valign="middle">
									<asp:LinkButton ID="btnPrevYear" runat="server" CausesValidation="false" 
										onclick="btnPrevYear_Click">
										<asp:Image ID="imgPrevYear" runat="server" ImageUrl="~/Images/previous.gif" />
									</asp:LinkButton>
								</td>
								<td valign="middle">
									<h4><asp:Label ID="lblYear" runat="server"></asp:Label></h4>
								</td>
								<td valign="middle">
									<asp:LinkButton ID="btnNextYear" runat="server" CausesValidation="false" 
										onclick="btnNextYear_Click">
										<asp:Image ID="imgNextYear" runat="server" ImageUrl="~/Images/next.gif" />
									</asp:LinkButton>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr class="HeadRow">
					<td>January</td>
					<td>February</td>
					<td>March</td>
				</tr>
				<tr>
					<td>
						<uc1:MonthCalendar ID="mcJanuary" runat="server" Year="2008" Month="1" OnPreRender="calendar_PreRender" />
					</td>
					<td>
						<uc1:MonthCalendar ID="mcFebruary" runat="server" Year="2008" Month="2" OnPreRender="calendar_PreRender" />
					</td>
					<td>
						<uc1:MonthCalendar ID="mcMarch" runat="server" Year="2008" Month="3" OnPreRender="calendar_PreRender" />
					</td>
				</tr>
				<tr class="HeadRow">
					<td>April</td>
					<td>May</td>
					<td>June</td>
				</tr>
				<tr>
					<td>
						<uc1:MonthCalendar ID="mcApril" runat="server" Year="2008" Month="4" OnPreRender="calendar_PreRender" />
					</td>
					<td>
						<uc1:MonthCalendar ID="mcMay" runat="server" Year="2008" Month="5" OnPreRender="calendar_PreRender" />
					</td>
					<td>
						<uc1:MonthCalendar ID="mcJune" runat="server" Year="2008" Month="6" OnPreRender="calendar_PreRender" />
					</td>
				</tr>
				<tr class="HeadRow">
					<td>July</td>
					<td>August</td>
					<td>September</td>
				</tr>
				<tr>
					<td>
						<uc1:MonthCalendar ID="mcJuly" runat="server" Year="2008" Month="7" OnPreRender="calendar_PreRender" />
					</td>
					<td>
						<uc1:MonthCalendar ID="mcAugust" runat="server" Year="2008" Month="8" OnPreRender="calendar_PreRender" />
					</td>
					<td>
						<uc1:MonthCalendar ID="mcSeptember" runat="server" Year="2008" Month="9" OnPreRender="calendar_PreRender" />
					</td>
				</tr>
				<tr class="HeadRow">
					<td>October</td>
					<td>November</td>
					<td>December</td>
				</tr>
				<tr>
					<td>
						<uc1:MonthCalendar ID="mcOctober" runat="server" Year="2008" Month="10" OnPreRender="calendar_PreRender" />
					</td>
					<td>
						<uc1:MonthCalendar ID="mcNovember" runat="server" Year="2008" Month="11" OnPreRender="calendar_PreRender" />
					</td>
					<td>
						<uc1:MonthCalendar ID="mcDecember" runat="server" Year="2008" Month="12" OnPreRender="calendar_PreRender" />
					</td>
				</tr>
			</table>
		</ContentTemplate>
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="btnPrevYear" EventName="Click" />
			<asp:AsyncPostBackTrigger ControlID="btnNextYear" EventName="Click" />
		</Triggers>
	</asp:UpdatePanel>
	<AjaxControlToolkit:UpdatePanelAnimationExtender ID="pnlBody_UpdatePanelAnimationExtender" 
		runat="server" Enabled="True" TargetControlID="pnlBody">
		<Animations>
			<OnUpdating>
				<ScriptAction Script="showInProcessImage($get('divWait'), updatingCalendarContainer);" />
			</OnUpdating>
		</Animations>
	</AjaxControlToolkit:UpdatePanelAnimationExtender>
</asp:Content>

