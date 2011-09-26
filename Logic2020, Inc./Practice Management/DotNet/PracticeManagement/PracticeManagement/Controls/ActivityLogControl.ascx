<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityLogControl.ascx.cs"
    Inherits="PraticeManagement.Controls.ActivityLogControl" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<style>
    .WordWrap
    {
        word-wrap: break word !important; /* Internet Explorer 5.5+ */
        word-break: break-all !important;
        white-space: -moz-pre-wrap !important; /*Mozilla */
        white-space: normal;
    }
    .displayNone
    {
        display: none;
    }
    .CompPerfTable td
    {
        padding-left: 5px;
    }
</style>
<script type="text/javascript" language="javascript">
    function EnableResetButtonForDateIntervalChange(sender, args) {
        var btnreset = document.getElementById('<%= btnResetFilter.ClientID %>');
        var hdnResetFilter = document.getElementById('<%= hdnResetFilter.ClientID %>');
        if (btnreset != null && btnreset != "undefined") {
            hdnResetFilter.value = 'true';
            btnreset.disabled = '';
        }
    }

    function EnableResetButton() {
        var btnreset = document.getElementById('<%= btnResetFilter.ClientID %>');
        var hdnResetFilter = document.getElementById('<%= hdnResetFilter.ClientID %>');
        if (btnreset != null && btnreset != "undefined") {
            btnreset.disabled = '';
            hdnResetFilter.value = 'true';
        }
    }

    function CheckIfDatesValid() {

        txtStartDate = document.getElementById('<%= diRange.ClientID %>_tbFrom');
        txtEndDate = document.getElementById('<%= diRange.ClientID %>_tbTo');
        var startDate = new Date(txtStartDate.value);
        var endDate = new Date(txtEndDate.value);
        if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= endDate) {
            var btnCustDatesClose = document.getElementById('<%= btnCustDatesClose.ClientID %>');
            hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            var startDate = new Date(txtStartDate.value);
            var endDate = new Date(txtEndDate.value);
            var startDateStr = startDate.format("MM/dd/yyyy");
            var endDateStr = endDate.format("MM/dd/yyyy");
            hdnStartDate.value = startDateStr;
            hdnEndDate.value = endDateStr;
            lblCustomDateRange.innerHTML = '(' + startDateStr + '&nbsp;-&nbsp;' + endDateStr + ')';
            btnCustDatesClose.click();

        }
        return false;
    }

    function CheckAndShowCustomDatesPoup(ddlPeriod) {
        imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
        lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
        if (ddlPeriod.value == '0') {
            imgCalender.attributes["class"].value = "";
            lblCustomDateRange.attributes["class"].value = "";
            if (imgCalender.fireEvent) {
                imgCalender.style.display = "";
                lblCustomDateRange.style.display = "";
                imgCalender.click();
            }
            if (document.createEvent) {
                var event = document.createEvent('HTMLEvents');
                event.initEvent('click', true, true);
                imgCalender.dispatchEvent(event);
            }
        }
        else {
            imgCalender.attributes["class"].value = "displayNone";
            lblCustomDateRange.attributes["class"].value = "displayNone";
            if (imgCalender.fireEvent) {
                imgCalender.style.display = "none";
                lblCustomDateRange.style.display = "none";
            }
        }
    }
    function ReAssignStartDateEndDates() {
        hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
        hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
        txtStartDate = document.getElementById('<%= diRange.ClientID %>_tbFrom');
        txtEndDate = document.getElementById('<%= diRange.ClientID %>_tbTo');
        hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
        hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');

        var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
        var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
        if (startDateCalExtender != null) {
            startDateCalExtender.set_selectedDate(hdnStartDate.value);
        }
        if (endDateCalExtender != null) {
            endDateCalExtender.set_selectedDate(hdnEndDate.value);
        }
        CheckIfDatesValid();
    }

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

    function endRequestHandle(sender, Args) {
        imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
        lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
        ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
        if (imgCalender != null && lblCustomDateRange != null && ddlPeriod != null) {
            if (imgCalender.fireEvent && ddlPeriod.value != '0') {
                imgCalender.style.display = "none";
                lblCustomDateRange.style.display = "none";
            }
        }
    }

    function disableProjectsDropDown() {
        
        var eventSource = document.getElementById('<%= ddlEventSource.ClientID %>');
        projectList = document.getElementById('<%= ddlProjects.ClientID %>');
        if (eventSource != null && projectList != null && eventSource.value != 'undefind') {
            if ((eventSource.value >= 3 && eventSource.value <= 5) || (eventSource.value >= 24 && eventSource.value <= 30) || (eventSource.value >= 40 && eventSource.value <= 43)) {
                projectList.disabled = 'true';
                projectList[0].selected = 'true';
            }
            else {
                projectList.disabled = '';
            }
        }
    }

</script>
<uc:LoadingProgress ID="lpActivityLog" runat="server" />
<asp:UpdatePanel ID="updActivityLog" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="5" class="WholeWidth">
            <tr>
                <td>
                    <div id="divActivitylog" style="padding: 10px;" runat="server">
                        <table id="tblActivitylog" runat="server" style="white-space: nowrap;">
                            <tr>
                                <td id="tdEventSource" runat="server" style="padding-left:0px;padding-right:0px;">
                                    <asp:Label ID="lblDisplay" runat="server" Text="Show "></asp:Label><asp:DropDownList
                                        ID="ddlEventSource" runat="server" EnableViewState="true">
                                        <asp:ListItem Text="ALL Events" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="ALL Errors" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Person Events" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Added Persons Only" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Changed Persons Only" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Target Person" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="Opportunity Events" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Added Opportunities Only" Value="8"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Changed Opportunities Only" Value="9"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Deleted Opportunities Only" Value="10"></asp:ListItem>
                                        <asp:ListItem Text="Project Events" Value="11"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Added Projects Only" Value="12"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Changed Projects Only" Value="13"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Deleted Projects Only" Value="14"></asp:ListItem>
                                        <asp:ListItem Text="Milestone Events" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Added Milestones Only" Value="16"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Changed Milestones Only" Value="17"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Deleted Milestones Only" Value="18"></asp:ListItem>
                                        <asp:ListItem Text="Projects And Milestones Events" Value="23"></asp:ListItem>
                                        <asp:ListItem Text="Logon Events" Value="24"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Successfull Logins Only" Value="25"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Login Errors Only" Value="26"></asp:ListItem>
                                        <asp:ListItem Text="Security Events" Value="27"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Password Reset Requests Only" Value="28"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Account Lockouts Only" Value="29"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Become Users Only" Value="30"></asp:ListItem>
                                        <asp:ListItem Text="Time Entry Events" Value="31"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Added TimeEntries Only" Value="32"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Changed TimeEntries Only" Value="33"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Deleted TimeEntries Only" Value="34"></asp:ListItem>
                                        <asp:ListItem Text="Project Attachment Events" Value="35"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Added Project Attachments Only" Value="36"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Deleted Project Attachments Only" Value="37"></asp:ListItem>
                                        <asp:ListItem Text="Notes" Value="38"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Project Notes Only" Value="39"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Milestone Notes Only" Value="40"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Opportunity Notes Only" Value="41"></asp:ListItem>
                                        <asp:ListItem Text="&nbsp; Person Notes Only" Value="42"></asp:ListItem>
                                        <asp:ListItem Text="Exports" Value="43"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td id="tdYear" runat="server" style="padding-left:2px;">
                                    <asp:Label ID="Label3" runat="server" Text="&nbsp;&nbsp;over&nbsp;&nbsp;"></asp:Label><asp:DropDownList
                                        ID="ddlPeriod" runat="server" EnableViewState="true">
                                        <asp:ListItem Text="Last Day" Value="-1"></asp:ListItem>
                                        <asp:ListItem Text="Last Week" Selected="True" Value="-7"></asp:ListItem>
                                        <asp:ListItem Text="Last Month" Value="-30"></asp:ListItem>
                                        <asp:ListItem Text="Last 3 months" Value="-90"></asp:ListItem>
                                        <asp:ListItem Text="Last 6 months" Value="-180"></asp:ListItem>
                                        <asp:ListItem Text="Last Year" Value="-360"></asp:ListItem>
                                        <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                                        EnableViewState="true" OkControlID="btnCustDatesClose" CancelControlID="btnCustDatesCancel"
                                        BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                                        DropShadow="false" OnCancelScript="ReAssignStartDateEndDates();" OnOkScript="return false;" />
                                    <asp:HiddenField ID="hdnStartDate" EnableViewState="true" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" EnableViewState="true" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnStartDateCalExtenderBehaviourId" EnableViewState="true" runat="server"
                                        Value="" />
                                    <asp:HiddenField ID="hdnEndDateCalExtenderBehaviourId" EnableViewState="true" runat="server"
                                        Value="" />
                                    <asp:Label ID="lblCustomDateRange" EnableViewState="true" Style="font-weight: bold;"
                                        runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" EnableViewState="true" runat="server" ImageUrl="~/Images/calendar.gif" />
                                </td>
                                <td id="spnPersons" runat="server">
                                    <asp:Label ID="Label1" runat="server" Text="for "></asp:Label><asp:DropDownList ID="ddlPersonName"
                                        runat="server" DataSourceID="odsPersons" DataTextField="PersonLastFirstName"
                                        DataValueField="Id" OnDataBound="ddlPersonName_OnDataBound" />
                                </td>
                                <td id="spnProjects" runat="server">
                                    <asp:Label ID="Label2" runat="server" Text="&nbsp;on "></asp:Label><asp:DropDownList ID="ddlProjects"
                                        runat="server" DataSourceID="odsProjects" DataTextField="Name" DataValueField="Id"
                                        OnDataBound="ddlProjects_OnDataBound" />
                                </td>
                                <td id="tdBtnList" runat="server">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnUpdateView" runat="server" Text="Update" ToolTip="Update" OnClick="btnUpdateView_Click" />
                                            </td>
                                            <td style="padding-left: 4px;">
                                                <asp:Button ID="btnResetFilter" runat="server" Text="Reset " ToolTip="Reset" OnClick="btnResetFilter_Click"
                                                    Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:HiddenField ID="hdnResetFilter" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gvActivities" runat="server" AutoGenerateColumns="False" EmptyDataText="No activity for given parameters."
                        DataSourceID="odsActivities" AllowPaging="True" PageSize="20" CssClass="CompPerfTable WholeWidth"
                        GridLines="None" BackColor="White" OnRowDataBound="gvActivities_OnRowDataBound"
                        OnDataBound="gvActivities_OnDataBound">
                        <AlternatingRowStyle BackColor="#F9FAFF" />
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle CssClass="cssPager" />
                        <RowStyle CssClass="al-row" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemStyle Width="11%" />
                                <HeaderTemplate>
                                    <div class="ie-bg" style="padding-right: '2px';">
                                        Modified / User</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblCreateDate" runat="server" Text='<%# ((DateTime)Eval("LogDate")).ToString("MM/dd/yyyy") + " " + ((DateTime)Eval("LogDate")).ToShortTimeString() %>' />
                                    by
                                    <asp:Label ID="lblUserName" runat="server" Text='<%# GetModifiedByDetails( Eval("Person.Id"), Eval("Person.PersonLastFirstName"), Eval("SystemUser").ToString(), Eval("LogData")) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemStyle Width="12%" CssClass="WordWrap" Wrap="true" />
                                <HeaderTemplate>
                                    <div class="ie-bg" style="padding-right: '2px';">
                                        Activity</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblActivityType" runat="server" Text='<%# NoNeedToShowActivityType(Eval("ActivityName")) %>'></asp:Label>
                                    <asp:Xml ID="xmlActivityItem" runat="server" DocumentContent='<%# AddDefaultProjectAndMileStoneInfo(Eval("LogData")) %>'
                                        TransformSource="~/Reports/Xslt/ActivityLogItem.xslt"></asp:Xml>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemStyle Width="77%" CssClass="WordWrap" Wrap="true" />
                                <HeaderTemplate>
                                    <div class="ie-bg">
                                        Changes</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Xml ID="xmlChanges" runat="server" DocumentContent='<%# AddDefaultProjectAndMileStoneInfo(Eval("LogData")) %>'
                                        TransformSource="~/Reports/Xslt/ActivityLogChanges.xslt"></asp:Xml>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="odsProjects" runat="server" SelectMethod="GetProjectListCustom"
                        TypeName="PraticeManagement.ProjectService.ProjectServiceClient">
                        <SelectParameters>
                            <asp:Parameter DefaultValue="true" Name="projected" Type="Boolean" />
                            <asp:Parameter DefaultValue="true" Name="completed" Type="Boolean" />
                            <asp:Parameter DefaultValue="true" Name="active" Type="Boolean" />
                            <asp:Parameter DefaultValue="true" Name="experimantal" Type="Boolean" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="odsPersons" runat="server" SelectMethod="GetAllPersons"
                        TypeName="PraticeManagement.Controls.DataHelper"></asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="odsActivities" runat="server" TypeName="PraticeManagement.Utils.ActivityLogHelper"
                        SelectCountMethod="GetActivitiesCount" SelectMethod="GetActivities" StartRowIndexParameterName="startRow"
                        MaximumRowsParameterName="maxRows" EnablePaging="true" OnSelecting="odsActivities_Selecting">
                        <SelectParameters>
                            <asp:Parameter Name="startDateFilter" Type="DateTime" />
                            <asp:Parameter Name="endDateFilter" Type="DateTime" />
                            <asp:Parameter Name="personId" />
                            <asp:Parameter Name="sourceFilter" />
                            <asp:Parameter Name="projectId" />
                            <asp:Parameter Name="opportunityId" />
                            <asp:Parameter Name="milestoneId" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
            CssClass="ConfirmBoxClass" Style="padding-top: 20px; display: none;" BorderWidth="2px">
            <table class="WholeWidth">
                <tr>
                    <td align="center">
                        <table>
                            <tr>
                                <td>
                                    <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                        EnableViewState="true" FromToDateFieldWidth="70" />
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" style="padding: 10px 0px 10px 0px;">
                        <asp:Button ID="btnCustDatesOK" runat="server" Text="OK" Style="float: none !Important;"
                            ValidationGroup="<%# ClientID %>" OnClientClick="return CheckIfDatesValid();"
                            CausesValidation="true" />
                        <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true" />
                        &nbsp; &nbsp;
                        <asp:Button ID="btnCustDatesCancel" CausesValidation="false" runat="server" Text="Cancel"
                            Style="float: none !Important;" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:ValidationSummary ID="valSum" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnUpdateView" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="btnResetFilter" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
