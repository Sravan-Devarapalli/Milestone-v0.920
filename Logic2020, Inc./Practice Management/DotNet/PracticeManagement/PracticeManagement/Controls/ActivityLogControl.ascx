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

</script>
<uc:LoadingProgress ID="lpActivityLog" runat="server" />
<asp:UpdatePanel ID="updActivityLog" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="5" class="WholeWidth">
            <tr>
                <td>
                    <div id="divActivitylog" style="padding: 10px;" runat="server">
                        <table id="tblActivitylog" runat="server" style="white-space: nowrap;" >
                            <tr>
                                <td id="tdEventSource" runat="server">
                                    <asp:Label ID="lblDisplay" runat="server" Text="Show"></asp:Label>
                                    <asp:DropDownList ID="ddlEventSource" runat="server" EnableViewState="true">
                                    </asp:DropDownList>
                                </td>
                                <td id="tdYear" runat="server" style="padding-left: 4px;">
                                    <uc:DateInterval ID="diYear" IsFromDateRequired="true" IsToDateRequired="true" runat="server"
                                        FromToDateFieldWidth="65" />
                                </td>
                                <td id="spnPersons" runat="server">
                                    <asp:Label ID="Label1" runat="server"  Text="user "></asp:Label><asp:DropDownList ID="ddlPersonName"
                                        runat="server" DataSourceID="odsPersons" DataTextField="PersonLastFirstName"
                                        DataValueField="Id" OnDataBound="ddlPersonName_OnDataBound" />
                                </td>
                                <td id="spnProjects" runat="server" style="padding-left: 4px;">
                                    <asp:Label ID="Label2" runat="server" Text="for "></asp:Label><asp:DropDownList ID="ddlProjects"
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
                                                <asp:Button ID="btnResetFilter" runat="server" Text="Reset" ToolTip="Reset" OnClick="btnResetFilter_Click"
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
                        GridLines="None" BackColor="White" OnRowDataBound="gvActivities_OnRowDataBound">
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
                                    <asp:Label ID="lblCreateDate" runat="server" Text='<%# ((DateTime)Eval("LogDate")).ToShortDateString() + " " + ((DateTime)Eval("LogDate")).ToShortTimeString() %>' />
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
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnUpdateView" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="btnResetFilter" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

