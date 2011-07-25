<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="PraticeManagement.Controls.Calendar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>
<%@ Register Src="~/Controls/MonthCalendar.ascx" TagName="MonthCalendar" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CalendarLegend.ascx" TagName="CalendarLegend" TagPrefix="uc2" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>

    <script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
<script type="text/javascript">
    var updatingCalendarContainer = null;
</script>
<style>
    .setCheckboxesLeft TD, .setCheckboxesLeft div
    {
        text-align:left !important;
    }
</style>
<br />
<uc2:CalendarLegend ID="CalendarLegend" runat="server" />
<br />
<table>
    <tr>
        <td>
            <uc3:LoadingProgress ID="loadingProgress" runat="server" />
            <asp:UpdatePanel ID="pnlBody" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="divWait" style="display: none; background-color: White; border: solid 1px silver;">
                        <span style="color: Black; font-weight: bold;">
                            <nobr>Please Wait...</nobr>
                        </span>
                        <br />
                        <asp:Image ID="imgLoading" runat="server" AlternateText="Please Wait..." ImageUrl="~/Images/ajax-loader.gif" />
                        <%--<img id="imgLoading" name="imgLoading" alt="Please Wait..." src="~/../Images/ajax-loader.gif" />--%>
                    </div>
                    <table class="CalendarTable">
                        <tr id="trPersonDetails" runat="server">
                            <td align="right">
                                Select a Person:
                            </td>
                            <td colspan="2" nowrap="nowrap">
                                <asp:DropDownList ID="ddlPerson" runat="server">
                                </asp:DropDownList>
                                <asp:UpdatePanel ID="pnlButton" runat="server" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:Button ID="btnRetrieveCalendar" runat="server" Text="Retrieve Calendar" OnClick="btnRetrieveCalendar_Click" />
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
                        <tr id="trRecurringHolidaysDetails" runat="server">
                            <td colspan="3" align="center" class="setCheckboxesLeft">
                                Add Recurring Holidays to Calendar:
                                <uc:ScrollingDropDown ID="cblRecurringHolidays" runat="server" SetDirty="false" Width="240px" Height="240px" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblRecurringHolidays','Recurring Holiday')" OnSelectedIndexChanged="cblRecurringHolidays_OnSelectedIndexChanged"
                                    DropDownListType="Recurring Holiday" CellPadding="3" AutoPostBack="true"/>
                                <ext:ScrollableDropdownExtender ID="sdecblRecurringHolidays" runat="server" TargetControlID="cblRecurringHolidays"
                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                </ext:ScrollableDropdownExtender>
                                <asp:HiddenField ID="hdnCheckBoxChanged" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="left">
                                <asp:Label ID="lblConsultantMessage" runat="server" Visible="false" Text="You can review your vacation days, but cannot change them. Please see your Practice Manager for updates to your vacation schedule."></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <table width="100%">
                                    <tr>
                                        <td valign="middle" style="text-align: right;">
                                            <asp:LinkButton ID="btnPrevYear" runat="server" CausesValidation="false" OnClick="btnPrevYear_Click">
                                                <asp:Image ID="imgPrevYear" runat="server" ImageUrl="~/Images/previous.gif" />
                                            </asp:LinkButton>
                                        </td>
                                        <td valign="middle" style="vertical-align: middle !important;" width="15px">
                                            <asp:Label ID="lblYear" Style="font-size: x-large;" runat="server"></asp:Label>
                                        </td>
                                        <td valign="middle" style="text-align: left;">
                                            <asp:LinkButton ID="btnNextYear" runat="server" CausesValidation="false" OnClick="btnNextYear_Click">
                                                <asp:Image ID="imgNextYear" runat="server" ImageUrl="~/Images/next.gif" />
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr class="HeadRow">
                            <td>
                                January
                            </td>
                            <td>
                                February
                            </td>
                            <td>
                                March
                            </td>
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
                            <td>
                                April
                            </td>
                            <td>
                                May
                            </td>
                            <td>
                                June
                            </td>
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
                            <td>
                                July
                            </td>
                            <td>
                                August
                            </td>
                            <td>
                                September
                            </td>
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
                            <td>
                                October
                            </td>
                            <td>
                                November
                            </td>
                            <td>
                                December
                            </td>
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
                    <asp:AsyncPostBackTrigger ControlID="cblRecurringHolidays" EventName="SelectedIndexChanged" />
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
        </td>
        <td id="tdDescription" align="center" runat="server" style="vertical-align: top; width:220px; white-space:nowrap;
            text-align: center; padding-left: 10px;">
            <div id="divDescription" runat="server" style="border: 1px solid black; padding: 4px;
                text-align: left;">
                Days selected on this calendar will<br />
                be highlighted as Company Holidays<br />
                throughout Practice Management.<br />
                <p style="padding-top: 8px;">
                    Common Recurring Holidays can be
                    <br />
                    selected from the drop-down as well.<br />
                    Once selected they will be
                    <br />
                    highlighted as Company Holidays<br />
                    throughout Practice Management for<br />
                    the current year as well as in future<br />
                    years.</p>
            </div>
        </td>
    </tr>
</table>

