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

    function changeAlternateitemscolrsForCBL() {
        var cbl = document.getElementById('<%=cblRecurringHolidays.ClientID %>');
        if (cbl != null)
            SetAlternateColors(cbl);
    }

    function SetAlternateColors(chkboxList) {
        var chkboxes = chkboxList.getElementsByTagName('input');
        var index = 0;
        if (chkboxes[0].parentNode.style.display != "none") {
            chkboxes[0].parentNode.style.paddingTop = "6px";
            chkboxes[0].parentNode.style.paddingLeft = "2px";
            chkboxes[0].parentNode.style.paddingRight = "0px";
            chkboxes[0].parentNode.style.paddingBottom = "6px";
            chkboxes[0].parentNode.style.borderBottom = "1px solid black";
            chkboxes[0].parentNode.style.borderRight = "5px solid white";
            chkboxes[0].parentNode.style.borderLeft = "5px solid white";
        }
        for (var i = 0; i < chkboxes.length; i++) {
            if (chkboxes[i].parentNode.style.display != "none") {
                index++;
                if ((index) % 2 == 0) {
                    chkboxes[i].parentNode.style.backgroundColor = "#f9faff";
                }
                else {
                    chkboxes[i].parentNode.style.backgroundColor = "";
                }
                chkboxes[i].parentNode.style.paddingRight = "2px";
            }
        }
    }


    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);


    function endRequestHandle(sender, Args) {

        changeAlternateitemscolrsForCBL();
    }

</script>
<style>
    .setCheckboxesLeft TD, .setCheckboxesLeft div
    {
        text-align: left !important;
    }
    
    .setCheckboxesLeft input
    {
        vertical-align: middle;
        float: right;
    }
    .setCheckboxesLeft Table
    {
        width: 100%;
    }
</style>
<table width="98%">
    <tr>
        <td style="width: 15%;">
            &nbsp;
        </td>
        <td id="tdDescription" align="center" runat="server" style="vertical-align: top;
            width: 70%;">
            <div style="border: 1px solid black; padding: 5px; white-space: normal;">
                <p>
                    Days selected on this calendar will be highlighted as Company Holidays throughout
                    Practice Management.</p>
                <p style="padding-top: 8px;">
                    Common Recurring Holidays can be selected from the drop-down as well. Once selected
                    they will be highlighted as Company Holidays throughout Practice Management for
                    the current year as well as in future years.</p>
            </div>
        </td>
        <td style="width: 15%;">
            &nbsp;
        </td>
    </tr>
</table>
<table width="98%">
    <tr>
        <td style="width: 100%;">
            <asp:UpdatePanel ID="pnlBody" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
                <ContentTemplate>
                    <uc3:LoadingProgress ID="ldProgress" runat="server" />
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
                            <td id="tdRecurringHolidaysDetails" runat="server" rowspan="9" class="setCheckboxesLeft"
                                style="padding-top: 45px; padding-left: 20px;">
                                <uc:ScrollingDropDown ID="cblRecurringHolidays" runat="server" SetDirty="false" Height="100%"
                                    AllSelectedReturnType="AllItems" OnSelectedIndexChanged="cblRecurringHolidays_OnSelectedIndexChanged"
                                    CellPadding="3" AutoPostBack="true" />
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
        </td>
    </tr>
</table>
<br />
<uc2:CalendarLegend ID="CalendarLegend" runat="server" disableChevron="true" />
<br />

