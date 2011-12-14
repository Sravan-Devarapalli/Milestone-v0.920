<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntriesByPerson.ascx.cs" EnableViewState="false"
    Inherits="PraticeManagement.Controls.Reports.TimeEntriesByPerson" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Import Namespace="DataTransferObjects" %>
   <asp:Repeater ID="repPersons" runat="server" OnItemDataBound="dlPersons_OnItemDataBound"
                    EnableViewState="false" OnItemCreated="dlPersons_OnItemCreated ">
                    <ItemTemplate>
                        <div id="divPersonListSummary" runat="server">
                            <div runat="server" id="divPersonName" style="padding-bottom: 5px;">
                                <font style="font-size: 20px; font-weight: bold;">
                                    <%# Eval("PersonName") %></font>
                            </div>
                            <br class="NotVisible" />
                            <div class="PersonGridLeftPadding" runat="server" id="divTeTable" style="overflow-x: auto;
                                overflow-y: display;">
                                <font style="font-style: italic; font-size: 16px; font-weight: bold;">Time Entry Summary</font>
                                <asp:Repeater ID="repTeTable" runat="server" DataSource='<%# GetModifiedDatasource(DataBinder.Eval(Container.DataItem, "GroupedTimeEtnries")) %>'
                                    OnItemDataBound="repTeTable_OnItemDataBound" EnableViewState="false" OnItemCreated="repTeTable_OnItemCreated">
                                    <HeaderTemplate>
                                        <table class="time-entry-person-projects WholeWidth" border="1" rules="rows" style="display: inline;">
                                            <thead>
                                                <tr style="border-bottom: 1px solid lightgray; border-top: 1px solid lightgray; vertical-align: middle;">
                                                    <th valign="middle" colspan="2" style="text-align: left; border-bottom: 1px solid gray;
                                                        border-top: 1px solid gray; width: 20%;">
                                                        <asp:Label ID="lblPType" runat="server" Font-Bold="true" Text="Client-Project-Time Type"></asp:Label>
                                                    </th>
                                                    <asp:Repeater ID="dlProject" runat="server" OnItemCreated="dlProject_OnItemCreated"
                                                        EnableViewState="false" OnInit="dlProject_OnInit">
                                                        <ItemTemplate>
                                                            <th valign="middle" style="padding-left: 5px; padding-right: 5px; vertical-align: middle;
                                                                text-align: center; border-bottom: 1px solid gray; font-weight: bold; border-top: 1px solid gray;"
                                                                class="<%# PraticeManagement.Utils.Calendar.GetCssClassByCalendarItem((CalendarItem) Container.DataItem) %>">
                                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd<br/>MMM d}")%>
                                                            </th>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <th valign="middle" style="padding-left: 5px; padding-right: 5px; vertical-align: middle;
                                                        text-align: center; border-bottom: 1px solid gray; font-weight: bold; border-top: 1px solid gray;">
                                                        Totals
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr style="border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;"
                                            class="<%# Container.ItemIndex % 2 == 0 ? "alterrow" : string.Empty %>">
                                            <td valign="middle" colspan="2" style="text-align: left; vertical-align: middle;
                                                border-bottom: 1px solid lightgray; border-top: 1px solid lightgray; width: 20%;">
                                                <asp:Label ID="lblPTypeValue" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Key")%>'></asp:Label>
                                            </td>
                                            <asp:Repeater ID="dlProject" runat="server" DataSource='<%# GetUpdatedDatasource(DataBinder.Eval(Container.DataItem, "Value")) %>'
                                                EnableViewState="false" OnItemDataBound="dlProject_OnItemDataBound">
                                                <ItemTemplate>
                                                    <td valign="middle" style="padding-left: 5px; text-align: center; vertical-align: middle;
                                                        padding-right: 5px; border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;">
                                                        <%#  ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")) != null ? string.Format("{0:F2}",((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")).ActualHours) : string.Empty%>
                                                    </td>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <td valign="middle" style="font-weight: bold; padding-left: 5px; text-align: center;
                                                vertical-align: middle; padding-right: 5px; border-bottom: 1px solid lightgray;
                                                border-top: 1px solid lightgray;">
                                                <%# ProjectTotals.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <tr style="font-weight: bold; border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;">
                                            <td colspan="2" style="text-align: left; border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;
                                                width: 20%;">
                                                Totals
                                            </td>
                                            <asp:Repeater ID="dlTotals" runat="server" OnItemDataBound="dlTotals_OnItemDataBound"
                                                EnableViewState="false" OnInit="dlTotals_OnInit">
                                                <ItemTemplate>
                                                    <td style="padding-left: 5px; padding-right: 5px; border-bottom: 1px solid lightgray;
                                                        text-align: center; border-top: 1px solid lightgray;">
                                                        <%# ((double?)DataBinder.Eval(Container.DataItem, "Value"))!=null ?string.Format("{0:F2}",((double?)DataBinder.Eval(Container.DataItem, "Value")).Value) : string.Empty %>
                                                    </td>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <td style="font-size: 15px; padding-left: 5px; padding-right: 5px; border-bottom: 1px solid lightgray;
                                                text-align: center; border-top: 1px solid lightgray;">
                                                <%# GrandTotal.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %>
                                            </td>
                                        </tr>
                                        </tbody></table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="PersonGridLeftPadding" style="overflow-x: auto; padding-top: 10px;" runat="server"
                                id="divProjects">
                                <br class="NotVisible" />
                                <font style="font-style: italic; font-size: 16px; font-weight: bold;">Time Entry Detail</font>
                                <asp:Repeater ID="dlProjects" runat="server" DataSource='<%# DataBinder.Eval(Container.DataItem, "GroupedTimeEtnries")  %>'
                                    EnableViewState="false" OnItemDataBound="dlProjects_OnItemDataBound">
                                    <ItemTemplate>
                                        <div style="padding-top: 3px;">
                                            <font style="font-size: 14px; font-weight: bold;">
                                                <%# Eval("Key.Client.Name") + " - " + Eval("Key.Name")%>
                                            </font>
                                        </div>
                                        <br class="NotVisible" />
                                        <asp:GridView ID="gvTimeEntries" runat="server" AutoGenerateColumns="False" DataSource='<%# Eval("Value") %>'
                                            EnableViewState="false" EnableModelValidation="True" CssClass="CompPerfTable WholeWidth"
                                            GridLines="Both" ShowFooter="true" OnRowDataBound="gvTimeEntries_OnRowDataBound"
                                            BackColor="White" EmptyDataText="This person has not entered any time for the period selected.">
                                            <AlternatingRowStyle BackColor="#F9FAFF" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; font-weight: bold;" class="ie-bg">
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
                                                        <div style="text-align: center; font-weight: bold;" class="ie-bg">
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
                                                        <div style="text-align: center; font-weight: bold;" class="ie-bg">
                                                            Time Type</div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <%#((TimeEntryRecord)Container.DataItem).TimeType.Name %>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="24%" VerticalAlign="Middle" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <FooterTemplate>
                                                        <div class="ie-bg" style="text-align: right;">
                                                            <asp:Label ID="lblGvGridTotalText" runat="server" Text="Total =" Font-Bold="true"></asp:Label></div>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="AlignCentre" FooterStyle-CssClass="AlignCentre">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; font-weight: bold;" class="ie-bg">
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
                                                        <div class="ie-bg" style="text-align: center;">
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
                        <div id="divhr" runat="server" style="padding-top: 10px; padding-bottom: 5px; padding-left: 5px;
                            padding-right: 5px;">
                            &nbsp;
                            <hr width="100%" size="2" color="#888888" align="center" style="margin-left: 5px;
                                margin-right: 5px;" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
