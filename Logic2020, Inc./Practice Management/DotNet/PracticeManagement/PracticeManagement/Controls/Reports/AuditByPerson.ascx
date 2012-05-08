<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuditByPerson.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.AuditByPerson" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" style="width: 90%;padding-top:5px;">
                <asp:Button ID="btnGroupBy" runat="server" Text="Group By Project" UseSubmitBehavior="false"
                    Width="130px" OnClick="btnGroupBy_OnClick" ToolTip="Group By Project" />
            </td>
            <td style="text-align: right; width: 10%; padding-right: 5px;padding-top:5px;">
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
    <asp:Repeater ID="repPersons" runat="server">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <table class="WholeWidthWithHeight">
                <tr style="text-align: left;">
                    <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;
                        font-weight: bold;">
                        <%# Eval("Person.PersonLastFirstName")%>
                        (<%# Eval("Person.Status.Name")%>,<%# Eval("Person.CurrentPay.TimescaleName")%>)
                    </td>
                    <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 10px;">
                        <%# GetDoubleFormat((double)Eval("NetChange"))%>
                    </td>
                </tr>
            </table>
            <asp:Repeater ID="repPersonTimeEntriesHistory" DataSource='<%# Eval("TimeEntryRecords") %>'
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
                                Project-Project Name
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
                            <%# GetDateFormat((DateTime)Eval("MilestoneDate"))%>
                        </td>
                        <td>
                            <%# GetDateFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                        <td title='<%# Eval("ChargeCode.ChargeCodeName")%>'>
                            <%# Eval("ChargeCode.Project.ProjectNumber")%>
                            -
                            <%# Eval("ChargeCode.Project.Name")%>
                        </td>
                        <td>
                            <%# Eval("ChargeCode.TimeType.Name")%>
                        </td>
                        <td>
                            <%# GetDoubleFormat((double)Eval("ActualHours"))%>
                        </td>
                        <td>
                            <%# GetDoubleFormat((double)Eval("OldHours"))%>
                        </td>
                        <td>
                            <%# GetDoubleFormat((double)Eval("NetChange"))%>
                        </td>
                        <td>
                            <img src="../Images/balloon-ellipsis.png" title='<%# Eval("Note")%>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr style="text-align: left; background-color: #ECE9D9;">
                        <td>
                            <%# GetDateFormat((DateTime)Eval("MilestoneDate"))%>
                        </td>
                        <td>
                            <%# GetDateFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                        <td title='<%# Eval("ChargeCode.ChargeCodeName")%>'>
                            <%# Eval("ChargeCode.Project.ProjectNumber")%>
                            -
                            <%# Eval("ChargeCode.Project.Name")%>
                        </td>
                        <td>
                            <%# Eval("ChargeCode.TimeType.Name")%>
                        </td>
                        <td>
                            <%# GetDoubleFormat((double)Eval("ActualHours"))%>
                        </td>
                        <td>
                            <%# GetDoubleFormat((double)Eval("OldHours"))%>
                        </td>
                        <td>
                            <%# GetDoubleFormat((double)Eval("NetChange"))%>
                        </td>
                        <td>
                            <img src="../Images/balloon-ellipsis.png" title='<%# Eval("Note")%>' />
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

