<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessDevelopmentGroupByBusinessUnit.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByAccount.BusinessDevelopmentGroupByBusinessUnit" %>

<asp:Repeater ID="repBusinessUnits" runat="server" OnItemDataBound="repBusinessUnits_ItemDataBound">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <asp:Panel ID="pnlPersonDetails" runat="server">
            <asp:Repeater ID="repPersons" runat="server" OnItemDataBound="repPersons_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="WholeWidthWithHeight">
                        <tr style="text-align: left;">
                            <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpePerson" runat="Server" CollapsedText="Expand Person Details"
                                    ExpandedText="Collapse Person Details" EnableViewState="false" BehaviorID="cpePerson"
                                    Collapsed="true" TargetControlID="pnlProjectDetails" ImageControlID="imgProject"
                                    CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgProject"
                                    ExpandControlID="imgProject" TextLabelID="lbProject" />
                                <asp:Image ID="imgProject" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Person Details" />
                                <asp:Label ID="lbProject" Style="display: none;" runat="server"></asp:Label>
                                <%# Eval("Person.PersonLastFirstName")%>
                                <b style="font-style: normal;">
                                    <%# GetPersonRole((string)Eval("Person.ProjectRoleName"))%></b>
                                <asp:Image ID="imgOffshore" runat="server" ImageUrl="~/Images/Offshore_Icon.png"
                                    ToolTip="Resource is an offshore employee" Visible='<%# (bool)Eval("Person.IsOffshore")%>' />
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
                                        <td style="width: 10%; text-align: right; vertical-align: middle;">
                                            <table width="100%">
                                                <tr>
                                                    <td style="text-align: right; font-weight: bold;">
                                                        <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="width: 80px;">
                                        </td>
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
                                                                    <%# Eval("HTMLNote")%>
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
                                                                    <%# Eval("HTMLNote")%>
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
                                        <td style="width: 10%; text-align: right; vertical-align: middle;">
                                            <table width="100%">
                                                <tr>
                                                    <td style="text-align: right; font-weight: bold;">
                                                        <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="width: 80px;">
                                        </td>
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
                                                                    <%# Eval("HTMLNote")%>
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
                                                                    <%# Eval("HTMLNote")%>
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
                <AlternatingItemTemplate>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
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

