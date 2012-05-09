<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuditByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.AuditByProject" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" style="width: 90%; padding-top: 5px;">
                <asp:Button ID="btnGroupBy" runat="server" Text="Group By Person" UseSubmitBehavior="false"
                    Width="130px" OnClick="btnGroupBy_OnClick" ToolTip="Group By Person" />
            </td>
            <td style="text-align: right; width: 10%; padding-right: 5px; padding-top: 5px;">
                <table width="100%" style="text-align: right;">
                    <tr>
                        <td>
                            Export:
                        </td>
                        <td>
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                                UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                        <td>
                            <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                                Enabled="false" UseSubmitBehavior="false" ToolTip="Export To PDF" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="repProject" runat="server">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <table class="WholeWidthWithHeight">
                <tr style="text-align: left;">
                    <td colspan="4" class="ProjectAccountName" style="width: 90%; white-space: nowrap;
                        font-size: 15px; font-weight: 1500;">
                        <%# Eval("Project.ProjectNumber")%>
                        -
                        <%# Eval("Project.Name")%>
                    </td>
                    <td style="width: 10%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 10px;">
                        <%# GetDoubleFormat((double)Eval("NetChange"))%>
                    </td>
                </tr>
            </table>
            <asp:Repeater ID="repProjectTimeEntriesHistory" DataSource='<%# GetModifiedDatasource(DataBinder.Eval(Container.DataItem, "PersonLevelTimeEntries")) %>'
                runat="server">
                <HeaderTemplate>
                    <table class="WidthWithHeightAndBorders CompPerfTable TableTextCenter" align="center">
                        <tr class="CompPerfHeader">
                            <th>
                                Affected Date
                            </th>
                            <th>
                                Modified Date
                            </th>
                            <th style="width: 20%;">
                                Person Name
                            </th>
                            <th>
                                Work Type
                            </th>
                            <th>
                                Original Hours
                            </th>
                            <th>
                                New Hours
                            </th>
                            <th>
                                Net Change
                            </th>
                            <th>
                            </th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="text-align: left; background-color: #D4D0C9;">
                        <td>
                            <%# GetDateFormat((DateTime)Eval("Value.MilestoneDate"))%>
                        </td>
                        <td>
                            <%# GetDateFormat((DateTime)Eval("Value.ModifiedDate"))%>
                        </td>
                        <td title='<%# Eval("Key.Status.Name")%>,<%# Eval("Key.CurrentPay.TimescaleName")%>'>
                            <%# Eval("Key.PersonLastFirstName")%>
                        </td>
                        <td>
                            <%# Eval("Value.ChargeCode.TimeType.Name")%>
                        </td>
                        <td style="text-align: right; vertical-align: middle;">
                            <table width="100%">
                                <tr>
                                    <td style="text-align: right; font-weight: bold;">
                                        <%# GetDoubleFormat((double)Eval("Value.OldHours"))%>
                                    </td>
                                    <td style="width: 20px">
                                        <asp:Image ID="imgNonBillable" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("Value.ChargeCode.TimeEntrySection"),(bool)Eval("Value.IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td style="text-align: right; font-weight: bold;">
                                        <%# GetDoubleFormat((double)Eval("Value.ActualHours"))%>
                                    </td>
                                    <td style="width: 20px">
                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png" ToolTip="Non-Billable hours."
                                            Visible='<%# GetNonBillableImageVisibility((int)Eval("Value.ChargeCode.TimeEntrySection"),(bool)Eval("Value.IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <%# GetDoubleFormat((double)Eval("Value.NetChange"))%>
                        </td>
                        <td>
                            <img src="../Images/balloon-ellipsis.png" alt="Note" title='<%# Eval("Value.Note")%>'
                                id="imgNote" />
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr style="text-align: left; background-color: #ECE9D9;">
                        <td>
                            <%# GetDateFormat((DateTime)Eval("Value.MilestoneDate"))%>
                        </td>
                        <td>
                            <%# GetDateFormat((DateTime)Eval("Value.ModifiedDate"))%>
                        </td>
                        <td title='<%# Eval("Key.Status.Name")%>,<%# Eval("Key.CurrentPay.TimescaleName")%>'>
                            <%# Eval("Key.PersonLastFirstName")%>
                        </td>
                        <td>
                            <%# Eval("Value.ChargeCode.TimeType.Name")%>
                        </td>
                       <td style="text-align: right; vertical-align: middle;">
                            <table width="100%">
                                <tr>
                                    <td style="text-align: right; font-weight: bold;">
                                        <%# GetDoubleFormat((double)Eval("Value.OldHours"))%>
                                    </td>
                                    <td style="width: 20px">
                                        <asp:Image ID="imgNonBillable" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("Value.ChargeCode.TimeEntrySection"),(bool)Eval("Value.IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td style="text-align: right; font-weight: bold;">
                                        <%# GetDoubleFormat((double)Eval("Value.ActualHours"))%>
                                    </td>
                                    <td style="width: 20px">
                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png" ToolTip="Non-Billable hours."
                                            Visible='<%# GetNonBillableImageVisibility((int)Eval("Value.ChargeCode.TimeEntrySection"),(bool)Eval("Value.IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <%# GetDoubleFormat((double)Eval("Value.NetChange"))%>
                        </td>
                        <td>
                            <img src="../Images/balloon-ellipsis.png" alt="Note" title='<%# Eval("Value.Note")%>'
                                id="imgNote" />
                        </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </ItemTemplate>
        <FooterTemplate>
        </FooterTemplate>
    </asp:Repeater>
</div>
<div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
    runat="server">
    There are no Time Entries that were changed afterwards by any Employee for the selected
    range.
</div>

