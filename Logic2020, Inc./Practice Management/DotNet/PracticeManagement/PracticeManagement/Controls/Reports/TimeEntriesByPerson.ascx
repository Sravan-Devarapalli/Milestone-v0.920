﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntriesByPerson.ascx.cs"
    EnableViewState="false" Inherits="PraticeManagement.Controls.Reports.TimeEntriesByPerson" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Import Namespace="DataTransferObjects" %>
<asp:Repeater ID="repPersons" runat="server" OnItemDataBound="dlPersons_OnItemDataBound"
    EnableViewState="false" OnItemCreated="dlPersons_OnItemCreated ">
    <ItemTemplate>
        <div id="divPersonListSummary" runat="server">
            <div runat="server" id="divPersonName" class="divPersonName">
                <%# Eval("Person.Name") %>
            </div>
            <div class="PersonGridLeftPadding divTeTable" runat="server" id="divTeTable">
                <div class="TimeEntrySummary">
                    Time Entry Summary</div>
                <asp:Repeater ID="repTeTable" runat="server" DataSource='<%# DataBinder.Eval(Container.DataItem, "GroupedTimeEtnries") %>'
                    OnItemDataBound="repTeTable_OnItemDataBound" EnableViewState="false" OnItemCreated="repTeTable_OnItemCreated">
                    <HeaderTemplate>
                        <table class="time-entry-person-projects WholeWidth">
                            <thead>
                                <tr>
                                    <th class="ClientProjectTimeType">
                                        Account - Business Unit - P# - Project Name  - Work Type
                                    </th>
                                    <asp:Repeater ID="dlProject" runat="server" OnItemCreated="dlProject_OnItemCreated"
                                        EnableViewState="false" OnInit="dlProject_OnInit">
                                        <ItemTemplate>
                                            <th class="<%# PraticeManagement.Utils.Calendar.GetCssClassByCalendarItem((CalendarItem) Container.DataItem) %>">
                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd<br/>MMM d}")%>
                                            </th>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <th>
                                        Totals
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="<%# Container.ItemIndex % 2 == 0 ? "alterrow" : string.Empty %>">
                            <td class="ClientProjectTimeType">
                                <%# DataBinder.Eval(Container.DataItem, "Key.ChargeCodeName")%>
                            </td>
                            <asp:Repeater ID="dlProject" runat="server" DataSource='<%# GetUpdatedDatasource(DataBinder.Eval(Container.DataItem, "Value")) %>'
                                EnableViewState="false" OnItemDataBound="dlProject_OnItemDataBound">
                                <ItemTemplate>
                                    <td>
                                        <p style="color: #3BA153;">
                                            <%#  ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")) != null && ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")).BillableHours != 0  ? "&nbsp;B - " + string.Format("{0:F2}", ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")).BillableHours) : string.Empty%>
                                        </p>
                                        <p style="color: Gray;">
                                            <%#  ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")) != null && ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")).NonBillableHours != 0 ? "NB - " + string.Format("{0:F2}", ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")).NonBillableHours) : string.Empty%>
                                        </p>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                            <td>
                                <%# ProjectTotals.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td class="ClientProjectTimeType HeaderDiv">
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
                            <%# DataBinder.Eval(Container.DataItem, "Key.ChargeCodeName")%>
                        </div>
                        <asp:GridView ID="gvTimeEntries" runat="server" AutoGenerateColumns="False" DataSource='<%# Eval("Value") %>'
                            EnableViewState="false" EnableModelValidation="True" CssClass="CompPerfTable WholeWidth PaddingClass"
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
                                        <%# ((TimeEntryRecord)Container.DataItem).ChargeCodeDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                                    </ItemTemplate>
                                    <ItemStyle Width="8%" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg HeaderDiv">
                                            Note</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%# Eval("Note") %>
                                    </ItemTemplate>
                                    <ItemStyle Width="55%" />
                                </asp:TemplateField>
                                <asp:TemplateField FooterStyle-CssClass="AlignRight">
                                    <HeaderTemplate>
                                        <div class="ie-bg HeaderDiv">
                                            Work Type</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%#((TimeEntryRecord)Container.DataItem).ChargeCode.TimeType.Name %>
                                    </ItemTemplate>
                                    <ItemStyle Width="24%" />
                                    <FooterTemplate>
                                        <div class="ie-bg AlignRight">
                                            <asp:Label ID="lblGvGridTotalText" runat="server" Text="Total =" Font-Bold="true"></asp:Label></div>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="AlignCentre" FooterStyle-CssClass="AlignCentre">
                                    <HeaderTemplate>
                                        <div class="ie-bg HeaderDiv">
                                            Hours</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="TextAlignRight">
                                            <span style="color: #3BA153;">B -
                                                <%#((TimeEntryRecord)Container.DataItem).BillableHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%>
                                            </span>&nbsp;&nbsp;<span style="color: Gray;">NB -
                                                <%#((TimeEntryRecord)Container.DataItem).NonBillableHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%>
                                            </span>
                                        </div>
                                    </ItemTemplate>
                                    <ItemStyle Width="13%" />
                                    <FooterTemplate>
                                        <div class="TextAlignRight">
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
                <asp:Literal ID="lblnoDataMesssage" runat="server" Text="This person has not entered any time for the period selected."
                    Visible="false"></asp:Literal>
            </div>
        </div>
        <div id="divhr" runat="server" class="divHrClass">
            &nbsp;
            <hr size="2" align="center" />
        </div>
    </ItemTemplate>
</asp:Repeater>

