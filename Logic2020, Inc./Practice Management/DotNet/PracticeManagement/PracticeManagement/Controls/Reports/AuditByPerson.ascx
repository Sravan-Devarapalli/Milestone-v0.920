<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuditByPerson.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.AuditByPerson" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent PaddingTop5">
                <asp:Button ID="btnGroupBy" runat="server" Text="Group By Project" UseSubmitBehavior="false"
                    CssClass="Width130px" OnClick="btnGroupBy_OnClick" ToolTip="Group By Project" />
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
    <asp:Repeater ID="repPersons" runat="server">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <table class="WholeWidthWithHeight">
                <tr class="textleft">
                    <td colspan="4" class="AuditReportByPersonHeaderTd1">
                        <%# Eval("Person.PersonLastFirstName")%>
                        <b class="fontStyleNormal">(<%# Eval("Person.Status.Name")%>,
                            <%# Eval("Person.CurrentPay.TimescaleName")%>)</b>
                    </td>
                    <td class="AuditReportByPersonHeaderTd2">
                        <%# GetDoubleFormat((double)Eval("NetChange"))%>
                    </td>
                </tr>
            </table>
            <asp:Repeater ID="repPersonTimeEntriesHistory" DataSource='<%# Eval("TimeEntryRecords") %>'
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
                                <th class="Width2Percent">
                                </th>
                                <th class="Width31Percent">
                                    Project - Project Name
                                </th>
                                <th class="Width2Percent">
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
                            <%# GetDateFormat((DateTime)Eval("ChargeCodeDate"))%>
                        </td>
                        <td class="AuditReportByPersonTd2">
                            <%# GetDateFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                        <td>
                        </td>
                        <td title='<%# Eval("ChargeCode.ChargeCodeName")%>' class="AuditReportByPersonTd1">
                            <%# Eval("ChargeCode.Project.ProjectNumber")%>
                            -
                            <%# Eval("ChargeCode.Project.HtmlEncodedName")%>
                        </td>
                        <td>
                        </td>
                        <td>
                            <%# Eval("ChargeCode.TimeType.Name")%>
                        </td>
                        <td class="vMiddle">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("OldHours"))%>
                                    </td>
                                    <td class="AuditReportByPersonTd1">
                                        <asp:Image ID="imgNonBillableOldHours" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("ChargeCode.TimeEntrySection"),(bool)Eval("IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("ActualHours"))%>
                                    </td>
                                    <td class="AuditReportByPersonTd1">
                                        <asp:Image ID="imgNonBillableActualHours" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("ChargeCode.TimeEntrySection"),(bool)Eval("IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("NetChange"))%>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <img src="../Images/notes.png" title='<%# Eval("Note")%>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                     <tr class="textLeft bgcolor_ECE9D9">
                        <td class="AuditReportByPersonTd1">
                            <%# GetDateFormat((DateTime)Eval("ChargeCodeDate"))%>
                        </td>
                        <td class="AuditReportByPersonTd2">
                            <%# GetDateFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                        <td>
                        </td>
                        <td title='<%# Eval("ChargeCode.ChargeCodeName")%>' class="AuditReportByPersonTd1">
                            <%# Eval("ChargeCode.Project.ProjectNumber")%>
                            -
                            <%# Eval("ChargeCode.Project.HtmlEncodedName")%>
                        </td>
                        <td>
                        </td>
                        <td>
                            <%# Eval("ChargeCode.TimeType.Name")%>
                        </td>
                        <td class="vMiddle">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("OldHours"))%>
                                    </td>
                                    <td class="AuditReportByPersonTd1">
                                        <asp:Image ID="imgNonBillableOldHours" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("ChargeCode.TimeEntrySection"),(bool)Eval("IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("ActualHours"))%>
                                    </td>
                                    <td class="AuditReportByPersonTd1">
                                        <asp:Image ID="imgNonBillableActualHours" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                            ToolTip="Non-Billable hours." Visible='<%# GetNonBillableImageVisibility((int)Eval("ChargeCode.TimeEntrySection"),(bool)Eval("IsChargeable"))%>' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table class="WholeWidth">
                                <tr>
                                    <td class="AuditReportByPersonTd3">
                                        <%# GetDoubleFormat((double)Eval("NetChange"))%>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <img src="../Images/notes.png" title='<%# Eval("Note")%>' />
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

