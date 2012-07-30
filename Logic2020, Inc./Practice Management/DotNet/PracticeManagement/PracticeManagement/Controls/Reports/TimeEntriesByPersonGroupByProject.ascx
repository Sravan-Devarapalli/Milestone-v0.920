<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntriesByPersonGroupByProject.ascx.cs" EnableViewState="false"
    Inherits="PraticeManagement.Controls.Reports.TimeEntriesByPersonGroupByProject" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Import Namespace="DataTransferObjects" %>
<div id="divPersonListSummary" runat="server">
    <div runat="server" id="divPersonName" class="divPersonName">
    </div>
    <div class="PersonGridLeftPadding divTeTable" runat="server" id="divTeTable">
        <div class="TimeEntrySummary">
            Time Entry Summary</div>
        <asp:Repeater ID="repTeTable" runat="server" 
            OnItemDataBound="repTeTable_OnItemDataBound" EnableViewState="false" OnItemCreated="repTeTable_OnItemCreated">
            <HeaderTemplate>
                <table class="time-entry-person-projects WholeWidth">
                    <thead>
                        <tr>
                            <th class="ClientProjectTimeType">
                                Account - Business Unit - P# - Project Name
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
                        <%# DataBinder.Eval(Container.DataItem, "Key")%>
                    </td>
                    <asp:Repeater ID="dlProject" runat="server" DataSource='<%# GetUpdatedDatasource(DataBinder.Eval(Container.DataItem, "Value")) %>'
                        EnableViewState="false" OnItemDataBound="dlProject_OnItemDataBound">
                        <ItemTemplate>
                            <td>
                                <p class="color_3BA153">
                                    <%#  ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")) != null && ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")).BillableHours != 0  ? "&nbsp;B - " + string.Format("{0:F2}", ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")).BillableHours) : string.Empty%>
                                </p>
                                <p class="colorGray">
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
                    <td class="HeaderDiv FontSize15PX">
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
        <asp:Repeater ID="dlProjects" runat="server" 
            EnableViewState="false" OnItemDataBound="dlProjects_OnItemDataBound">
            <ItemTemplate>
                <div class="ClientAndProjectName">
                    <%# DataBinder.Eval(Container.DataItem, "Key")%>
                </div>
                <asp:GridView ID="gvTimeEntries" runat="server" AutoGenerateColumns="False" DataSource='<%# Eval("Value") %>'
                    EnableViewState="false" EnableModelValidation="True" CssClass="CompPerfTable PaddingClass3px TimeEntryByPerson_gvTimeEntries"
                    ShowFooter="true" OnRowDataBound="gvTimeEntries_OnRowDataBound" EmptyDataText="This person has not entered any time for the period selected.">
                    <AlternatingRowStyle CssClass="alterrow" />
                    <Columns>
                        <asp:TemplateField HeaderStyle-CssClass="Width8Percent">
                            <HeaderTemplate>
                                <div class="ie-bg HeaderDiv">
                                    Date</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# ((TimeEntryRecord)Container.DataItem).ChargeCodeDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="Width55Percent">
                            <HeaderTemplate>
                                <div class="ie-bg HeaderDiv">
                                    Note</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("HtmlEncodedNote")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="Width24Percent" FooterStyle-CssClass="AlignRight">
                            <HeaderTemplate>
                                <div class="ie-bg HeaderDiv">
                                    Work Type</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#((TimeEntryRecord)Container.DataItem).ChargeCode.TimeType.Name %>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div class="ie-bg AlignRight">
                                    <asp:Label ID="lblGvGridTotalText" runat="server" Text="Total =" Font-Bold="true"></asp:Label></div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="Width13Percent" ItemStyle-CssClass="AlignCentre"
                            FooterStyle-CssClass="AlignCentre">
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

