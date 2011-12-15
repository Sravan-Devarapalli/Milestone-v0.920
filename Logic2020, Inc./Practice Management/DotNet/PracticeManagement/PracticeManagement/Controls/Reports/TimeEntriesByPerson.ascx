<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntriesByPerson.ascx.cs"
    EnableViewState="false" Inherits="PraticeManagement.Controls.Reports.TimeEntriesByPerson" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Import Namespace="DataTransferObjects" %>
<asp:Repeater ID="repPersons" runat="server" OnItemDataBound="dlPersons_OnItemDataBound"
    EnableViewState="false" OnItemCreated="dlPersons_OnItemCreated ">
    <ItemTemplate>
        <div id="divPersonListSummary" runat="server">
            <div runat="server" id="divPersonName" class="divPersonName">
                <%# Eval("PersonName") %>
            </div>
            <div class="PersonGridLeftPadding divTeTable" runat="server" id="divTeTable">
                <div class="TimeEntrySummary">
                    Time Entry Summary</div>
                <asp:Repeater ID="repTeTable" runat="server" DataSource='<%# GetModifiedDatasource(DataBinder.Eval(Container.DataItem, "GroupedTimeEtnries")) %>'
                    OnItemDataBound="repTeTable_OnItemDataBound" EnableViewState="false" OnItemCreated="repTeTable_OnItemCreated">
                    <HeaderTemplate>
                        <table class="time-entry-person-projects WholeWidth">
                            <thead>
                                <tr>
                                    <th valign="middle" colspan="2" class="ClientProjectTimeType">
                                        Client-Project-Time Type
                                    </th>
                                    <asp:Repeater ID="dlProject" runat="server" OnItemCreated="dlProject_OnItemCreated"
                                        EnableViewState="false" OnInit="dlProject_OnInit">
                                        <ItemTemplate>
                                            <th valign="middle" class="<%# PraticeManagement.Utils.Calendar.GetCssClassByCalendarItem((CalendarItem) Container.DataItem) %>">
                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd<br/>MMM d}")%>
                                            </th>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <th valign="middle">
                                        Totals
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="<%# Container.ItemIndex % 2 == 0 ? "alterrow" : string.Empty %>">
                            <td valign="middle" colspan="2" class="ClientProjectTimeType">
                                <%# DataBinder.Eval(Container.DataItem, "Key")%>
                            </td>
                            <asp:Repeater ID="dlProject" runat="server" DataSource='<%# GetUpdatedDatasource(DataBinder.Eval(Container.DataItem, "Value")) %>'
                                EnableViewState="false" OnItemDataBound="dlProject_OnItemDataBound">
                                <ItemTemplate>
                                    <td valign="middle">
                                        <%#  ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")) != null ? string.Format("{0:F2}",((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")).ActualHours) : string.Empty%>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                            <td valign="middle" style="font-weight: bold;">
                                <%# ProjectTotals.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td colspan="2" class="ClientProjectTimeType HeaderDiv">
                                Totals
                            </td>
                            <asp:Repeater ID="dlTotals" runat="server" OnItemDataBound="dlTotals_OnItemDataBound"
                                EnableViewState="false" OnInit="dlTotals_OnInit">
                                <ItemTemplate>
                                    <td class="HeaderDiv">
                                        <%# ((double?)DataBinder.Eval(Container.DataItem, "Value"))!=null ?string.Format("{0:F2}",((double?)DataBinder.Eval(Container.DataItem, "Value")).Value) : string.Empty %>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                            <td class="HeaderDiv" style="font-size: 15px;">
                                <%# GrandTotal.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %>
                            </td>
                        </tr>
                        </tbody></table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="PersonGridLeftPadding divProjects" runat="server" id="divProjects">
                <div class="TimeEntryDetail">
                    Time Entry Detail</div>
                <asp:Repeater ID="dlProjects" runat="server" DataSource='<%# DataBinder.Eval(Container.DataItem, "GroupedTimeEtnries")  %>'
                    EnableViewState="false" OnItemDataBound="dlProjects_OnItemDataBound">
                    <ItemTemplate>
                        <div class="ClientAndProjectName">
                            <%# Eval("Key.Client.Name") + " - " + Eval("Key.Name")%>
                        </div>
                        <asp:GridView ID="gvTimeEntries" runat="server" AutoGenerateColumns="False" DataSource='<%# Eval("Value") %>'
                            EnableViewState="false" EnableModelValidation="True" CssClass="CompPerfTable WholeWidth"
                            GridLines="Both" ShowFooter="true" OnRowDataBound="gvTimeEntries_OnRowDataBound"
                            BackColor="White" EmptyDataText="This person has not entered any time for the period selected.">
                            <AlternatingRowStyle BackColor="#F9FAFF" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg HeaderDiv">
                                            Date</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%# ((TimeEntryRecord)Container.DataItem).MilestoneDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="8%" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg HeaderDiv">
                                            Note</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblNote" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="60%" />
                                </asp:TemplateField>
                                <asp:TemplateField FooterStyle-CssClass="AlignRight">
                                    <HeaderTemplate>
                                        <div class="ie-bg HeaderDiv">
                                            Time Type</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%#((TimeEntryRecord)Container.DataItem).TimeType.Name %>
                                    </ItemTemplate>
                                    <ItemStyle Width="24%" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <FooterTemplate>
                                        <div class="ie-bg" style="text-align: right">
                                            <asp:Label ID="lblGvGridTotalText" runat="server" Text="Total =" Font-Bold="true"></asp:Label></div>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="AlignCentre" FooterStyle-CssClass="AlignCentre">
                                    <HeaderTemplate>
                                        <div style="text-align: center; font-weight: bold" class="ie-bg">
                                            Hours</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div style="text-align: center;">
                                            <%#((TimeEntryRecord)Container.DataItem).ActualHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%>
                                        </div>
                                    </ItemTemplate>
                                    <ItemStyle Width="8%" VerticalAlign="Middle" HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <FooterTemplate>
                                        <div class="ie-bg" style="text-align: center">
                                            <asp:Label ID="lblGvGridTotal" runat="server" Font-Bold="true"></asp:Label></div>
                                    </FooterTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div id="divPersonNotEntered" runat="server" class="PersonGridLeftPadding">
                &nbsp;
                <asp:Label ID="lblnoDataMesssage" runat="server" Text="This person has not entered any time for the period selected."
                    Visible="false"></asp:Label>
            </div>
        </div>
        <div id="divhr" runat="server" class="divHrClass">
            &nbsp;
            <hr size="2" align="center" />
        </div>
    </ItemTemplate>
</asp:Repeater>

