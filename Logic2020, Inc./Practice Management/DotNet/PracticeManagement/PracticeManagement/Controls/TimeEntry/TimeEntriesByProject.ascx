<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntriesByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.TimeEntriesByProject" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<uc3:LoadingProgress ID="LoadingProgress1" runat="server" />
<asp:UpdatePanel ID="updReport" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlFilters" runat="server" CssClass="buttons-block">
            <table class="opportunity-description">
                <tr>
                    <td valign="top">
                        <table class="opportunity-description">
                            <tr>
                                <td>
                                    Client
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlClients" runat="server" Width="460" OnSelectedIndexChanged="ddlClients_OnSelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </td>
                                <tr>
                                    <td>
                                        Project
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlProjects" runat="server" Enabled="false" AutoPostBack="true" Width="460" 
                                            OnSelectedIndexChanged="ddlProjects_OnSelectedIndexChanged">
                                            <asp:ListItem Text="-- Select a Project --" Value="" ></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table>
                                            <tr>
                                                <td style="padding-top: 30px;">
                                                    Show only persons with time entered
                                                </td>
                                                <td style="text-align: right; padding-top: 30px;" id="report-date-interval">
                                                    <uc:DateInterval ID="diRange" runat="server" FromToDateFieldWidth="70" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                        </table>
                    </td>
                    <td valign="top">
                        <table class="opportunity-description">
                            <tr>
                                <td valign="top" style="padding-left: 5px;">
                                    <asp:Label ID="lblPersons" runat="server" Text="Persons"></asp:Label>
                                </td>
                                <td style="padding-left: 10px;">
                                    <cc2:ScrollingDropDown ID="cblPersons" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                        BackColor="White" CellPadding="3" NoItemsType="All" SetDirty="False" Width="315"
                                        Height="99" BorderWidth="0">
                                    </cc2:ScrollingDropDown>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:ValidationSummary ID="valSum" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_OnClick" />
                    </td>
                </tr>
            </table>
            <asp:ObjectDataSource ID="odsProjects" runat="server" SelectMethod="GetAllTimeEntryProjects"
                TypeName="PraticeManagement.TimeEntryService.TimeEntryServiceClient" />
        </asp:Panel>
        <h2>
            <asp:Label ID="lblProjectName" runat="server" Visible="false" /></h2>
        <asp:DataList ID="dlTimeEntries" runat="server" DataSourceID="odsTimeEntries" CssClass="WholeWidth">
            <ItemTemplate>
                <h3>
                    <asp:Label ID="lblPersonName" runat="server" Text='<%# Eval("Key.PersonLastFirstName") %>' /></h3>
                <asp:GridView ID="gvPersonTimeEntries" runat="server" DataSource='<%# Eval("Value") %>'
                    AutoGenerateColumns="false" CssClass="CompPerfTable WholeWidth" GridLines="None"
                    OnRowDataBound="gvPersonTimeEntries_OnRowDataBound" BackColor="White" ShowFooter="true"
                    OnDataBound="gvPersonTimeEntries_OnDataBound">
                    <AlternatingRowStyle BackColor="#F9FAFF" />
                    <FooterStyle Font-Bold="true" HorizontalAlign="Center" />
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Date</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# ((TimeEntryRecord)Container.DataItem).MilestoneDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" Width="100" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Note</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("Note")%>
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Right" />
                            <FooterTemplate>
                                Total:</FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Hours</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#((TimeEntryRecord)Container.DataItem).ActualHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" Width="50" HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
            </ItemTemplate>
        </asp:DataList>
        <h3 style="text-align: right">
            <asp:Label ID="lblGrandTotal" runat="server" /></h3>
        <asp:ObjectDataSource ID="odsTimeEntries" runat="server" SelectMethod="GetTimeEntriesForProject"
            OnSelecting="odsTimeEntries_OnSelecting" TypeName="PraticeManagement.Utils.TimeEntryHelper"
            OnDataBinding="odsTimeEntries_OnDataBinding" OnSelected="odsTimeEntries_OnSelected">
            <SelectParameters>
                <asp:Parameter Name="projectId" Type="Int32" />
                <asp:Parameter Name="startDate" Type="DateTime" />
                <asp:Parameter Name="endDate" Type="DateTime" />
                <asp:Parameter Name="personIdList" Type="Object" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </ContentTemplate>
</asp:UpdatePanel>

