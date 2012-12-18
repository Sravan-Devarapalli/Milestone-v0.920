<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuditByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.AuditByProject" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent PaddingTop5">
                <asp:Button ID="btnGroupBy" runat="server" Text="Group By Person" UseSubmitBehavior="false"
                    CssClass="Width130px" OnClick="btnGroupBy_OnClick" ToolTip="Group By Person" />
            </td>
            <td class="textRight Width10Percent padRight5 PaddingTop5">
                <table class="textRight WholeWidth">
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
                <tr class="textleft">
                    <td colspan="4" class="AuditReportByProjectHeaderTd1">
                        <%# Eval("Project.ProjectNumber")%>
                        -
                        <%# Eval("Project.HtmlEncodedName")%><b class="fontStyleNormal"> (<%# Eval("Project.Client.HtmlEncodedName")%>
                            >
                            <%# Eval("Project.Group.HtmlEncodedName")%>)</b>
                    </td>
                    <td class="AuditReportByProjectHeaderTd2">
                        <%# GetDoubleFormat((double)Eval("NetChange"))%>
                    </td>
                </tr>
            </table>
            <asp:Repeater ID="repProjectTimeEntriesHistory" DataSource='<%# GetModifiedDatasource(DataBinder.Eval(Container.DataItem, "PersonLevelTimeEntries")) %>'
                runat="server">
                <HeaderTemplate>
                    <div class="padLeft5 padRight5">
                        <table class="WidthWithHeightAndBorders CompPerfTable TableTextCenter" align="center">
                            <tr class="CompPerfHeader">
                                <th class="Width13Percent">
                                    Affected Date
                                </th>
                                <th class="Width13Percent">
                                    Modified Date
                                </th>
                                <th class="Width35Percent">
                                    Person Name
                                </th>
                                <th class="Width12Percent">
                                    Work Type
                                </th>
                                <th class="Width8Percent">
                                    Original Hours
                                </th>
                                <th class="Width8Percent">
                                    New Hours
                                </th>
                                <th class="Width8Percent">
                                    Net Change
                                </th>
                                <th class="Width3Percent">
                                </th>
                            </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="textLeft bgColorD4D0C9">
                        <td class="AuditReportByPersonTd1">
                            <%# GetDateFormat((DateTime)Eval("Value.ChargeCodeDate"))%>
                        </td>
                        <td class="AuditReportByPersonTd2">
                            <%# GetDateFormat((DateTime)Eval("Value.ModifiedDate"))%>
                        </td>
                        <td title='<%# Eval("Key.Status.Name")%>, <%# Eval("Key.CurrentPay.TimescaleName")%>'>
                            <%# Eval("Key.PersonLastFirstName")%>
                        </td>
                        <td>
                            <%# Eval("Value.ChargeCode.TimeType.Name")%>
                        </td>
                        <td class="vMiddle">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("Value.OldHours"))%>
                                    </td>
                                    <td class="AuditReportByPersonTd1">
                                        <asp:Image ID="imgNonBillableOldHours" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("Value.ChargeCode.TimeEntrySection"),(bool)Eval("Value.IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("Value.ActualHours"))%>
                                    </td>
                                    <td class="AuditReportByPersonTd1">
                                        <asp:Image ID="imgNonBillableActualHours" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("Value.ChargeCode.TimeEntrySection"),(bool)Eval("Value.IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("Value.NetChange"))%>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <img src="../Images/notes.png" alt="Note" title='<%# HttpUtility.HtmlEncode(Eval("Value.Note").ToString())%>' id="imgNote" />
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="textLeft bgcolor_ECE9D9">
                        <td class="AuditReportByPersonTd1">
                            <%# GetDateFormat((DateTime)Eval("Value.ChargeCodeDate"))%>
                        </td>
                        <td class="AuditReportByPersonTd2">
                            <%# GetDateFormat((DateTime)Eval("Value.ModifiedDate"))%>
                        </td>
                        <td title='<%# Eval("Key.Status.Name")%>, <%# Eval("Key.CurrentPay.TimescaleName")%>'>
                            <%# Eval("Key.PersonLastFirstName")%>
                        </td>
                        <td>
                            <%# Eval("Value.ChargeCode.TimeType.Name")%>
                        </td>
                        <td class="vMiddle">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("Value.OldHours"))%>
                                    </td>
                                    <td class="AuditReportByPersonTd1">
                                        <asp:Image ID="imgNonBillableOldHours" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("Value.ChargeCode.TimeEntrySection"),(bool)Eval("Value.IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("Value.ActualHours"))%>
                                    </td>
                                    <td class="AuditReportByPersonTd1">
                                        <asp:Image ID="imgNonBillableActualHours" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("Value.ChargeCode.TimeEntrySection"),(bool)Eval("Value.IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("Value.NetChange"))%>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <img src="../Images/notes.png" alt="Note" title='<%# HttpUtility.HtmlEncode( Eval("Value.Note").ToString() )%>' id="imgNote" />
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </table> </div>
                </FooterTemplate>
            </asp:Repeater>
        </ItemTemplate>
        <FooterTemplate>
        </FooterTemplate>
    </asp:Repeater>
</div>
<div id="divEmptyMessage" style="display: none;" class="EmptyMessagediv" runat="server">
    There are no Time Entries that were changed afterwards by any Employee for the selected
    range.
</div>

