<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectDetailTabByResource.ascx.cs" Inherits="PraticeManagement.Controls.Reports.ProjectDetailTabByResource" %>
<%@ Import Namespace="DataTransferObjects.Reports" %>
<asp:HiddenField ID="hdncpeExtendersIds" runat="server" Value="" />
<asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" style="width: 90%;">
            <asp:Button ID="btnExpandOrCollapseAll" runat="server" Text="Collapse All" UseSubmitBehavior="false"
                ToolTip="Collapse All" />
        </td>
        <td style="text-align: right; width: 10%; padding-right: 5px;">
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
                        <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick" Enabled="false"
                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Repeater ID="repPersons" runat="server" OnItemDataBound="repPersons_ItemDataBound">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="WholeWidthWithHeight">
            <tr style="text-align: left;">
                <td colspan="4" class="ProjectAccountName" style="width: 95%;white-space:nowrap;">
                    <%# Eval("Person.PersonLastFirstName")%>
                    <b style="font-style:normal;"><%# GetPersonRole((string)Eval("Person.ProjectRoleName"))%></b>
                </td>
                <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 10px;">
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlProjectDetails" runat="server" CssClass="cp bg-white">
            <asp:Repeater ID="repDate" runat="server" OnItemDataBound="repDate_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="WholeWidthWithHeight">
                        <tr style="text-align: left; background-color: #D4D0C9;">
                            <td style="width: 80%; padding-left: 20px;">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDate" runat="Server" CollapsedText="Expand Date Details"
                                    ExpandedText="Collapse Date Details" EnableViewState="true" BehaviorID="cpeDate"
                                    Collapsed="true" TargetControlID="pnlDateDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                                    ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                                    TextLabelID="lbDate" />
                                <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                                <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                                <%# GetDateFormat((DateTime)Eval("Date"))%>
                            </td>
                            <td style="width: 10%; text-align: right;vertical-align: middle;">
                                <table width="100%">
                                    <tr>
                                        <td style="text-align: right; font-weight: bold;">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                        <td style="width: 20px">
                                            <asp:Image ID="imgNonBillable" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                                ToolTip="Non-Billable hours are included." Visible='<%# GetNonBillableImageVisibility((double)Eval("NonBillableHours"))%>' />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width:80px;"> </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlDateDetails" runat="server">
                        <table class="WholeWidthWithHeight">
                            <asp:Repeater ID="repWorktype" runat="server">
                                <ItemTemplate>
                                    <tr style="text-align: left; background-color: White;">
                                        <td style="width: 72%; padding-left: 50px;">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td style="width: 8%; color: #3BA153;">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td style="width: 8%; color: Gray;">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" style="width: 12%">
                                        </td>
                                    </tr>
                                    <tr style="text-align: left; background-color: White;" id="trNote" runat="server"
                                        visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td style="padding-left: 55px;" class="wrapword">
                                            <table>
                                                <tr>
                                                    <td style="width: 8%; vertical-align: top;">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td style="vertical-align: top;">
                                                        <%# Eval("Note")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr style="text-align: left; background-color: #F0F0F1;">
                                        <td style="width: 72%; padding-left: 50px;">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td style="width: 8%; color: #3BA153;">
                                            B -
                                            <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                                        </td>
                                        <td style="width: 8%; color: Gray;">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" style="width: 12%">
                                        </td>
                                    </tr>
                                    <tr style="text-align: left; background-color: #F0F0F1;" id="trNote" runat="server"
                                        visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td style="padding-left: 55px;" class="wrapword">
                                            <table>
                                                <tr>
                                                    <td style="width: 8%; vertical-align: top;">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td style="vertical-align: top;">
                                                        <%# Eval("Note")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </table>
                    </asp:Panel>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="WholeWidthWithHeight">
                        <tr style="text-align: left; background-color: #ECE9D9;">
                            <td style="width: 80%; padding-left: 20px;">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDate" runat="Server" CollapsedText="Expand Date Details"
                                    ExpandedText="Collapse Date Details" EnableViewState="true" BehaviorID="cpeDate"
                                    Collapsed="true" TargetControlID="pnlDateDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                                    ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                                    TextLabelID="lbDate" />
                                <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                                <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                                <%# GetDateFormat((DateTime)Eval("Date"))%>
                            </td>
                            <td style="width: 10%; text-align: right;vertical-align: middle;">
                                <table width="100%">
                                    <tr>
                                        <td style="text-align: right; font-weight: bold;">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                        <td style="width: 20px">
                                            <asp:Image ID="imgNonBillable" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                                ToolTip="Non-Billable hours are included." Visible='<%# GetNonBillableImageVisibility((double)Eval("NonBillableHours"))%>' />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width:80px;"> </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlDateDetails" runat="server">
                        <table class="WholeWidthWithHeight">
                            <asp:Repeater ID="repWorktype" runat="server">
                                <ItemTemplate>
                                    <tr style="text-align: left; background-color: White;">
                                        <td style="width: 72%; padding-left: 50px;">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td style="width: 8%; color: #3BA153;">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td style="width: 8%; color: Gray;">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" style="width: 12%">
                                        </td>
                                    </tr>
                                    <tr style="text-align: left; background-color: White;" id="trNote" runat="server"
                                        visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td style="padding-left: 55px;" class="wrapword">
                                            <table>
                                                <tr>
                                                    <td style="width: 8%; vertical-align: top;">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td style="vertical-align: top;">
                                                        <%# Eval("Note")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr style="text-align: left; background-color: #F0F0F1;">
                                        <td style="width: 72%; padding-left: 50px;">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td style="width: 8%; color: #3BA153;">
                                            B -
                                            <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                                        </td>
                                        <td style="width: 8%; color: Gray;">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" style="width: 12%">
                                        </td>
                                    </tr>
                                    <tr style="text-align: left; background-color: #F0F0F1;" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>' >
                                        <td style="padding-left: 55px;" class="wrapword">
                                            <table>
                                                <tr>
                                                    <td style="width: 8%; vertical-align: top;">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td style="vertical-align: top;">
                                                        <%# Eval("Note")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </table>
                    </asp:Panel>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
    runat="server">
    There are no Time Entries towards this project.
</div>

