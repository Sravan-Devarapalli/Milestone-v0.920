<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityLogControl.ascx.cs"
    Inherits="PraticeManagement.Controls.ActivityLogControl" %>
<%@ Register src="~/Controls/Generic/LoadingProgress.ascx" tagname="LoadingProgress" tagprefix="uc" %>
<uc:LoadingProgress ID="lpActivityLog" runat="server" />
<asp:UpdatePanel ID="updActivityLog" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="5" class="WholeWidth">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblDisplay" runat="server" Text="Display"></asp:Label>
                                <asp:DropDownList ID="ddlEventSource" runat="server" Width="150" EnableViewState="true">
                                </asp:DropDownList>
                            </td>
                            <td>
                                &nbsp;over period&nbsp;
                                <asp:DropDownList ID="ddlPeriod" runat="server" Width="100">
                                    <asp:ListItem Text="Day" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Week" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Month" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Year" Value="4" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Unrestricted" Value="5"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <span id="spnPersons" runat="server">&nbsp;user&nbsp;
                                <asp:DropDownList ID="ddlPersonName" runat="server" Width="150"
                                    DataSourceID="odsPersons" DataTextField="PersonLastFirstName" DataValueField="Id"
                                    OnDataBound="ddlPersonName_OnDataBound"/>
                                </span>
                            </td>
                            <td>
                                <span id="spnProjects" runat="server">&nbsp;for&nbsp;
                                    <asp:DropDownList ID="ddlProjects" runat="server" Width="200"
                                        DataSourceID="odsProjects" DataTextField="Name" DataValueField="Id"
                                        OnDataBound="ddlProjects_OnDataBound"
                                     /></span>
                            </td>
                            <td align="right">
                                &nbsp;<asp:Button ID="btnUpdateView" runat="server" Text="Update View" OnClick="btnUpdateView_Click"
                                    Width="100px" CssClass="pm-button" />
                            </td>
                        </tr>
                    </table>
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
                        <PagerSettings Mode="NumericFirstLast"/>
                        <PagerStyle CssClass="cssPager"/>
                        <RowStyle CssClass="al-row"/>
                        <Columns>
                            <asp:TemplateField>
                                <ItemStyle Width="11%" />
                                <HeaderTemplate>
                                    <div class="ie-bg" style="padding-right:'2px';">
                                        Modified / User</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblCreateDate" runat="server" Text='<%# ((DateTime)Eval("LogDate")).ToShortDateString() + " " + ((DateTime)Eval("LogDate")).ToShortTimeString() %>' /> by 
                                    <asp:Label ID="lblUserName" runat="server" Text='<%# Eval("Person.Id") != null ? Eval("Person.PersonLastFirstName") : Eval("SystemUser") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemStyle Width="11%" />
                                <HeaderTemplate>
                                    <div class="ie-bg" style="padding-right:'2px';">
                                        Activity</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblActivityType" runat="server" Text='<%# Eval("ActivityName") %>'></asp:Label>
                                    <asp:Xml ID="xmlActivityItem" runat="server" DocumentContent='<%# AddDefaultProjectAndMileStoneInfo(Eval("LogData")) %>' 
                                        TransformSource="~/Reports/Xslt/ActivityLogItem.xslt"></asp:Xml>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemStyle Width="78%" />
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
                    <asp:ObjectDataSource ID="odsProjects" runat="server" 
                        SelectMethod="GetProjectListCustom" 
                        TypeName="PraticeManagement.ProjectService.ProjectServiceClient">
                        <SelectParameters>
                            <asp:Parameter DefaultValue="true" Name="projected" Type="Boolean" />
                            <asp:Parameter DefaultValue="true" Name="completed" Type="Boolean" />
                            <asp:Parameter DefaultValue="true" Name="active" Type="Boolean" />
                            <asp:Parameter DefaultValue="true" Name="experimantal" Type="Boolean" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="odsPersons" runat="server" 
                        SelectMethod="GetAllPersons" 
                        TypeName="PraticeManagement.Controls.DataHelper">
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="odsActivities" runat="server" TypeName="PraticeManagement.Utils.ActivityLogHelper"
                        SelectCountMethod="GetActivitiesCount" SelectMethod="GetActivities" StartRowIndexParameterName="startRow"
                        MaximumRowsParameterName="maxRows" EnablePaging="true"
                         OnSelecting="odsActivities_Selecting"> <%-- OnDataBinding="odsActivities_OnDataBinding" --%>
                        <SelectParameters>
                            <asp:Parameter Name="periodFilter" />
                            <asp:Parameter Name="personId" />
                            <asp:Parameter Name="sourceFilter"  />
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
    </Triggers>
</asp:UpdatePanel>


