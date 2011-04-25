<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntries.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.TimeEntries" %>
<%@ Register Src="~/Controls/TimeEntry/TimeEntryBar.ascx" TagName="TimeEntryBar"
    TagPrefix="te" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register TagPrefix="ext1" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<script type="text/javascript">
<!--
    var areValidTes;
    var areValidTeBarDropdowns;
    function setInvalidArgs(val) {
        areValidTes = val;
    }

    function getInvalidArgs(val) {
        return areValidTes;
    }

    function checkSingleTeNote(element, index, array) {
        if (!element.isValidNote()) {
            setInvalidArgs(false);
            element.showErrorMessageColor();
        }
    }

    function ValidateAllNotes(sender, args) {
        setInvalidArgs(true);

        Array.forEach(allSingleTEs, checkSingleTeNote);

        args.IsValid = getInvalidArgs();
    }

    function checkSingleTeHours(element, index, array) {
        if (!element.isValidHours()) {
            setInvalidArgs(false);
            element.showErrorMessageColor();
        }
    }

    function ValidateAllHours(sender, args) {
        setInvalidArgs(true);

        Array.forEach(allSingleTEs, checkSingleTeHours);

        args.IsValid = getInvalidArgs();
    }

    function ValidateTEBarDropdowns(sender, args) {
        areValidTeBarDropdowns = true;
        //        allInvalidDropdowns = new Array();
        Array.forEach(allSingleTEs, validateDropdowns);
        args.IsValid = areValidTeBarDropdowns;
    }

    function validateDropdowns(element, index, array) {
        if (($get(element.get_NoteIdValue()) != null && element._note() != null && element._note().trim().length > 0)
            || ($get(element.get_ActualHoursIdValue()) != null && element._hours() != null && element._hours().trim().length > 0)) {
            var ddlProjectMilestone = element.get_ProjectMilestoneDropdown();
            var ddlTimeType = element.get_TimeTypeDropdown();
            if (ddlProjectMilestone.selectedIndex == 0) {
                ddlProjectMilestone.style.borderColor = "red";
                areValidTeBarDropdowns = false;
            }
            if (ddlTimeType.selectedIndex == 0) {
                ddlTimeType.style.borderColor = "red";
                areValidTeBarDropdowns = false;
            }
        }
    }

    //    function showProjMileChangedMessage(){
    //        $get("proj-mile-changed-message").style.display = 'inherit';
    //        //onchange="javascript:showProjMileChangedMessage();" 
    //    }
// -->
</script>
<asp:Panel ID="pnlGrid" runat="server" Visible="false" CssClass="cp bg-white">
    <table class="CompPerfTable WholeWidth">
        <tr class="CompPerfHeader WholeWidth">
            <td class="time-entry-bar-project-milestones">
                <div class="ie-bg">
                    Project - Milestone</div>
            </td>
            <td class="time-entry-bar-time-types">
                <div class="ie-bg">
                    Time Type</div>
            </td>
            <asp:Repeater ID="repEntriesHeader" runat="server">
                <ItemTemplate>
                    <td class="time-entry-bar-single-te">
                        <div class="ie-bg">
                            <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%></div>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td class="time-entry-bar-total-hours">
                <div class="ie-bg">
                    TOTAL</div>
            </td>
            <td style="width: 3%">
                &nbsp;
            </td>
        </tr>
    </table>
    <asp:Repeater ID="tes" runat="server" OnItemDataBound="repEntries_ItemDataBound">
        <ItemTemplate>
            <te:TimeEntryBar runat="server" ID="bar" />
            <%--OnRowRemoved="bar_OnRowRemoved"--%>
        </ItemTemplate>
    </asp:Repeater>
    <table class="CompPerfTable WholeWidth">
        <tr class="CompPerfTotalSummary">
            <td class="time-entry-bar-project-milestones">
            </td>
            <td class="time-entry-bar-time-types">
            </td>
            <asp:Repeater ID="repTotalHours" runat="server">
                <ItemTemplate>
                    <%--<td class="time-entry-total-hours-by-day">
                        <Label ID="lblTotalHours" runat="server"/>
                        <ext1:TotalCalculatorExtender ID="extTotalHours" runat="server" TargetControlID="lblTotalHours"/>                              
                    </td>--%>
                    <td class="time-entry-total-hours-by-day">
                        &nbsp;
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td class="time-entry-total-hours" style="border-top: 1px solid black;">
                <label id="lblSpreadsheetTotalHours" runat="server" style="font-size: small; font-weight: bold;" />
                <ext1:TotalCalculatorExtender ID="extSpreadsheetTotalHours" runat="server" TargetControlID="lblSpreadsheetTotalHours" />
            </td>
            <td style="width: 4%">
            </td>
        </tr>
    </table>
    <asp:ValidationSummary ID="valSummary" runat="server" ValidationGroup="TE" />
</asp:Panel>
<uc:MessageLabel ID="mlMessage" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
    WarningColor="Orange" EnableViewState="false" />

