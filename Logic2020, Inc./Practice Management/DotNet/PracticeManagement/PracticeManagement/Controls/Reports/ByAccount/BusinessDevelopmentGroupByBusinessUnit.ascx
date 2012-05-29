<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessDevelopmentGroupByBusinessUnit.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByAccount.BusinessDevelopmentGroupByBusinessUnit" %>
<asp:Repeater ID="repBusinessUnits" runat="server" OnItemDataBound="repBusinessUnits_ItemDataBound">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="WholeWidthWithHeight">
            <tr style="text-align: left;">
                <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeBusinessUnit" runat="Server"
                        CollapsedText="Expand Business Unit Details" ExpandedText="Collapse Business Unit Details"
                        EnableViewState="false" Collapsed="true" TargetControlID="pnlBusinessUnitDetails"
                        ImageControlID="imgProject" CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg"
                        CollapseControlID="imgProject" ExpandControlID="imgProject" TextLabelID="lbProject" />
                    <asp:Image ID="imgProject" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Person Details" />
                    <asp:Label ID="lbProject" Style="display: none;" runat="server"></asp:Label>
                    <%# Eval("BusinessUnit.Name")%>
                    <b style="font-style: normal;">
                        <%# GetBusinessUnitStatus((bool)Eval("BusinessUnit.IsActive"))%></b>
                </td>
                <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 10px;">
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlBusinessUnitDetails" runat="server">
            <asp:Repeater ID="repPersons" runat="server" DataSource='<%# Eval("PersonLevelGroupedHoursList") %>'
                OnItemDataBound="repPersons_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="WholeWidthWithHeight">
                        <tr style="text-align: left; background-color: White;">
                            <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;
                                padding-left: 20px;">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpePerson" runat="Server" CollapsedText="Expand Person Details"
                                    ExpandedText="Collapse Person Details" EnableViewState="false" Collapsed="true"
                                    TargetControlID="pnlPersonDetails" ImageControlID="imgPerson" CollapsedImage="~/Images/expand.jpg"
                                    ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgPerson" ExpandControlID="imgPerson"
                                    TextLabelID="lbPerson" />
                                <asp:Image ID="imgPerson" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Person Details" />
                                <asp:Label ID="lbPerson" Style="display: none;" runat="server"></asp:Label>
                                <%# Eval("Person.PersonLastFirstName")%>
                            </td>
                            <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 60px;">
                                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlPersonDetails" runat="server" CssClass="cp bg-white">
                        <asp:Repeater ID="repDate" runat="server" DataSource='<%# Eval("DayTotalHours") %>'
                            OnItemDataBound="repDate_ItemDataBound">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table class="WholeWidthWithHeight">
                                    <tr style="text-align: left; background-color: #D4D0C9;">
                                        <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;
                                            padding-left: 40px;">
                                            <%--<AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDate" runat="Server" CollapsedText="Expand Date Details"
                                                ExpandedText="Collapse Date Details" EnableViewState="true" BehaviorID="cpeDate"
                                                Collapsed="true" TargetControlID="pnlDateDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                                                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                                                TextLabelID="lbDate" />
                                            <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                                            <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>--%>
                                            <%# GetDateFormat((DateTime)Eval("Date"))%>
                                        </td>
                                        <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 110px;">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDateDetails" runat="server">
                                    <table class="WholeWidthWithHeight">
                                        <asp:Repeater ID="repWorktype" DataSource='<%# Eval("DayTotalHoursList") %>' runat="server">
                                            <ItemTemplate>
                                                <tr style="text-align: left; background-color: White;">
                                                    <td style="width: 80%; padding-left: 50px;">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td style="width: 10%; color: Gray;">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" style="width: 10%">
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
                                                    <td colspan="3">
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <tr style="text-align: left; background-color: #F0F0F1;">
                                                    <td style="width: 80%; padding-left: 50px;">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td style="width: 10%; color: Gray;">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" style="width: 10%">
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
                                                    <td colspan="3">
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
                                        <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;
                                            padding-left: 40px;">
                                            <%--<AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDate" runat="Server" CollapsedText="Expand Date Details"
                                                ExpandedText="Collapse Date Details" EnableViewState="true" BehaviorID="cpeDate"
                                                Collapsed="true" TargetControlID="pnlDateDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                                                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                                                TextLabelID="lbDate" />
                                            <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                                            <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>--%>
                                            <%# GetDateFormat((DateTime)Eval("Date"))%>
                                        </td>
                                        <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 110px;">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDateDetails" runat="server">
                                    <table class="WholeWidthWithHeight">
                                        <asp:Repeater ID="repWorktype" DataSource='<%# Eval("DayTotalHoursList") %>' runat="server">
                                            <ItemTemplate>
                                                <tr style="text-align: left; background-color: White;">
                                                    <td style="width: 80%; padding-left: 50px;">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td style="width: 10%; color: Gray;">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" style="width: 10%">
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
                                                    <td colspan="3">
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <tr style="text-align: left; background-color: #F0F0F1;">
                                                    <td style="width: 80%; padding-left: 50px;">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td style="width: 10%; color: Gray;">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" style="width: 10%">
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
                                                    <td colspan="3">
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
                    <table class="WholeWidthWithHeight">
                        <tr style="text-align: left; background-color: #f5faff;">
                            <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;
                                padding-left: 20px;">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpePerson" runat="Server" CollapsedText="Expand Person Details"
                                    ExpandedText="Collapse Person Details" EnableViewState="false" Collapsed="true"
                                    TargetControlID="pnlPersonDetails" ImageControlID="imgPerson" CollapsedImage="~/Images/expand.jpg"
                                    ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgPerson" ExpandControlID="imgPerson"
                                    TextLabelID="lbPerson" />
                                <asp:Image ID="imgPerson" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Person Details" />
                                <asp:Label ID="lbPerson" Style="display: none;" runat="server"></asp:Label>
                                <%# Eval("Person.PersonLastFirstName")%>
                            </td>
                            <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 60px;">
                                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlPersonDetails" runat="server" CssClass="cp bg-white">
                        <asp:Repeater ID="repDate" runat="server" DataSource='<%# Eval("DayTotalHours") %>'
                            OnItemDataBound="repDate_ItemDataBound">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table class="WholeWidthWithHeight">
                                    <tr style="text-align: left; background-color: #D4D0C9;">
                                        <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;
                                            padding-left: 40px;">
                                            <%--<AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDate" runat="Server" CollapsedText="Expand Date Details"
                                                ExpandedText="Collapse Date Details" EnableViewState="true" BehaviorID="cpeDate"
                                                Collapsed="true" TargetControlID="pnlDateDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                                                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                                                TextLabelID="lbDate" />
                                            <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                                            <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>--%>
                                            <%# GetDateFormat((DateTime)Eval("Date"))%>
                                        </td>
                                        <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 110px;">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDateDetails" runat="server">
                                    <table class="WholeWidthWithHeight">
                                        <asp:Repeater ID="repWorktype" DataSource='<%# Eval("DayTotalHoursList") %>' runat="server">
                                            <ItemTemplate>
                                                <tr style="text-align: left; background-color: White;">
                                                    <td style="width: 86%; padding-left: 50px;">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td style="width: 7%; color: Gray;">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" style="width: 7%">
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
                                                    <td colspan="3">
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <tr style="text-align: left; background-color: #F0F0F1;">
                                                    <td style="width: 86%; padding-left: 50px;">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td style="width: 7%; color: Gray;">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" style="width: 7%">
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
                                                    <td colspan="3">
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
                                        <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;
                                            padding-left: 40px;">
                                            <%--<AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDate" runat="Server" CollapsedText="Expand Date Details"
                                                ExpandedText="Collapse Date Details" EnableViewState="true" BehaviorID="cpeDate"
                                                Collapsed="true" TargetControlID="pnlDateDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                                                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                                                TextLabelID="lbDate" />
                                            <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                                            <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>--%>
                                            <%# GetDateFormat((DateTime)Eval("Date"))%>
                                        </td>
                                        <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 110px;">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDateDetails" runat="server">
                                    <table class="WholeWidthWithHeight">
                                        <asp:Repeater ID="repWorktype" DataSource='<%# Eval("DayTotalHoursList") %>' runat="server">
                                            <ItemTemplate>
                                                <tr style="text-align: left; background-color: White;">
                                                    <td style="width: 86%; padding-left: 50px;">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td style="width: 7%; color: Gray;">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" style="width: 7%">
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
                                                    <td colspan="3">
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <tr style="text-align: left; background-color: #F0F0F1;">
                                                    <td style="width: 86%; padding-left: 50px;">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td style="width: 7%; color: Gray;">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" style="width: 7%">
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
                                                    <td colspan="3">
                                                    </td>
                                                </tr>
                                            </AlternatingItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </asp:Panel>
                            </AlternatingItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
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

