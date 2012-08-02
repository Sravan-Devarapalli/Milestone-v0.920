﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupByPerson.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByAccount.GroupByPerson" %>
<asp:Repeater ID="repPersonsList" runat="server" OnItemDataBound="repPersonsList_ItemDataBound">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="WholeWidthWithHeight">
            <tr class="textLeft">
                <td colspan="4" class="ProjectAccountName Width95Percent no-wrap">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpePerson" runat="Server" CollapsedText="Expand Person Details"
                        ExpandedText="Collapse Person Details" EnableViewState="false" Collapsed="true"
                        TargetControlID="pnlPersonDetails" ImageControlID="imgProject" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgProject" ExpandControlID="imgProject"
                        TextLabelID="lbProject" />
                    <asp:Image ID="imgProject" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Person Details" />
                    <asp:Label ID="lbProject" Style="display: none;" runat="server"></asp:Label>
                    <%# Eval("Person.HtmlEncodedName")%>
                </td>
                <td class="PersonDetailTotal">
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlPersonDetails" runat="server">
            <asp:Repeater ID="repBusinessUnit" runat="server" DataSource='<%# Eval("BusinessUnitLevelGroupedHoursList") %>'
                OnItemDataBound="repBusinessUnit_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="WholeWidthWithHeight">
                        <tr class="textLeft bgcolorwhite">
                            <td colspan="4" class="ProjectAccountName Width95Percent no-wrap padLeft20Imp">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeBusinessUnit" runat="server"
                                    CollapsedText="Expand Business Unit Details" ExpandedText="Collapse Business Unit Details"
                                    EnableViewState="false" Collapsed="true" TargetControlID="pnlBusinessUnitDetails"
                                    ImageControlID="imgBusinessUnit" CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg"
                                    CollapseControlID="imgBusinessUnit" ExpandControlID="imgBusinessUnit" TextLabelID="lbBusinessUnit" />
                                <asp:Image ID="imgBusinessUnit" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Person Details" />
                                <asp:Label ID="lbBusinessUnit" Style="display: none;" runat="server"></asp:Label>
                                <%# Eval("BusinessUnit.HtmlEncodedName")%>
                                <b class="fontStyleNormal">
                                    <%# GetBusinessUnitStatus((bool)Eval("BusinessUnit.IsActive"))%></b>
                            </td>
                            <td class="PersonDetailTotal padRight60Imp">
                                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlBusinessUnitDetails" runat="server" CssClass="bg-white">
                        <asp:Repeater ID="repDate" runat="server" DataSource='<%# Eval("DayTotalHours") %>'
                            OnItemDataBound="repDate_ItemDataBound">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table class="WholeWidthWithHeight">
                                    <tr class="textleft bgColorD4D0C9">
                                        <td colspan="4" class="ProjectAccountName Width95Percent no-wrap padLeft40Imp">
                                            <%# GetDateFormat((DateTime)Eval("Date"))%>
                                        </td>
                                        <td class="PersonDetailTotal padRight110Imp">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDateDetails" runat="server">
                                    <table class="WholeWidthWithHeight WorkTypeTable">
                                        <asp:Repeater ID="repWorktype" DataSource='<%# Eval("DayTotalHoursList") %>' runat="server">
                                            <ItemTemplate>
                                                <tr class="FirstTr">
                                                    <td class="FirstTrTd1_GroupByBusinessUnit">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td class="FirstTrTd3_GroupByBusinessUnit">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" class="Width10Percent">
                                                    </td>
                                                </tr>
                                                <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                                    <td class="wrapword SecondTrTd1">
                                                        <table>
                                                            <tr>
                                                                <td class="SecondTrTd2">
                                                                    <b>NOTE:&nbsp;</b>
                                                                </td>
                                                                <td class="vTopImp">
                                                                    <%# Eval("HtmlEncodedHTMLNote")%>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td colspan="3">
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <tr class="FirstTr AlternativeFirstTr">
                                                    <td class="FirstTrTd1_GroupByBusinessUnit">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td class="FirstTrTd3_GroupByBusinessUnit">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" class="Width10Percent">
                                                    </td>
                                                </tr>
                                                <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                                    <td class="wrapword SecondTrTd1">
                                                        <table>
                                                            <tr>
                                                                <td class="SecondTrTd2">
                                                                    <b>NOTE:&nbsp;</b>
                                                                </td>
                                                                <td class="vTopImp">
                                                                    <%# Eval("HtmlEncodedHTMLNote")%>
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
                                    <tr class="textleft bgcolor_ECE9D9">
                                        <td colspan="4" class="ProjectAccountName Width95Percent no-wrap padLeft40Imp">
                                            <%# GetDateFormat((DateTime)Eval("Date"))%>
                                        </td>
                                        <td class="PersonDetailTotal padRight110Imp">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDateDetails" runat="server">
                                    <table class="WholeWidthWithHeight WorkTypeTable">
                                        <asp:Repeater ID="repWorktype" DataSource='<%# Eval("DayTotalHoursList") %>' runat="server">
                                            <ItemTemplate>
                                                <tr class="FirstTr">
                                                    <td class="FirstTrTd1_GroupByBusinessUnit">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td class="FirstTrTd3_GroupByBusinessUnit">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" class="Width10Percent">
                                                    </td>
                                                </tr>
                                                <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                                    <td class="wrapword SecondTrTd1">
                                                        <table>
                                                            <tr>
                                                                <td class="SecondTrTd2">
                                                                    <b>NOTE:&nbsp;</b>
                                                                </td>
                                                                <td class="vTopImp">
                                                                    <%# Eval("HtmlEncodedHTMLNote")%>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td colspan="3">
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <tr class="FirstTr AlternativeFirstTr">
                                                    <td class="FirstTrTd1_GroupByBusinessUnit">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td class="FirstTrTd3_GroupByBusinessUnit">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" class="Width10Percent">
                                                    </td>
                                                </tr>
                                                <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                                    <td class="wrapword SecondTrTd1">
                                                        <table>
                                                            <tr>
                                                                <td class="SecondTrTd2">
                                                                    <b>NOTE:&nbsp;</b>
                                                                </td>
                                                                <td class="vTopImp">
                                                                    <%# Eval("HtmlEncodedHTMLNote")%>
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
                        <tr class="textLeft bgColor_F5FAFF">
                            <td colspan="4" class="ProjectAccountName Width95Percent no-wrap padLeft20Imp">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeBusinessUnit" runat="server"
                                    CollapsedText="Expand Business Unit Details" ExpandedText="Collapse Business Unit Details"
                                    EnableViewState="false" Collapsed="true" TargetControlID="pnlBusinessUnitDetails"
                                    ImageControlID="imgBusinessUnit" CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg"
                                    CollapseControlID="imgBusinessUnit" ExpandControlID="imgBusinessUnit" TextLabelID="lbBusinessUnit" />
                                <asp:Image ID="imgBusinessUnit" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Person Details" />
                                <asp:Label ID="lbBusinessUnit" Style="display: none;" runat="server"></asp:Label>
                                <%# Eval("BusinessUnit.Name")%>
                                <b class="fontStyleNormal">
                                    <%# GetBusinessUnitStatus((bool)Eval("BusinessUnit.IsActive"))%></b>
                            </td>
                            <td class="PersonDetailTotal padRight60Imp">
                                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlBusinessUnitDetails" runat="server" CssClass="bg-white">
                        <asp:Repeater ID="repDate" runat="server" DataSource='<%# Eval("DayTotalHours") %>'
                            OnItemDataBound="repDate_ItemDataBound">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table class="WholeWidthWithHeight">
                                    <tr class="textleft bgColorD4D0C9">
                                        <td colspan="4" class="ProjectAccountName Width95Percent no-wrap padLeft40Imp">
                                            <%# GetDateFormat((DateTime)Eval("Date"))%>
                                        </td>
                                        <td class="PersonDetailTotal padRight110Imp">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDateDetails" runat="server">
                                    <table class="WholeWidthWithHeight WorkTypeTable">
                                        <asp:Repeater ID="repWorktype" DataSource='<%# Eval("DayTotalHoursList") %>' runat="server">
                                            <ItemTemplate>
                                                <tr class="FirstTr">
                                                    <td class="FirstTrTd1_GroupByBusinessUnit">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td class="FirstTrTd3_GroupByBusinessUnit">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" class="Width10Percent">
                                                    </td>
                                                </tr>
                                                <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                                    <td class="wrapword SecondTrTd1">
                                                        <table>
                                                            <tr>
                                                                <td class="SecondTrTd2">
                                                                    <b>NOTE:&nbsp;</b>
                                                                </td>
                                                                <td class="vTopImp">
                                                                    <%# Eval("HtmlEncodedHTMLNote")%>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td colspan="3">
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <tr class="FirstTr AlternativeFirstTr">
                                                    <td class="FirstTrTd1_GroupByBusinessUnit">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td class="FirstTrTd3_GroupByBusinessUnit">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" class="Width10Percent">
                                                    </td>
                                                </tr>
                                                <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                                    <td class="wrapword SecondTrTd1">
                                                        <table>
                                                            <tr>
                                                                <td class="SecondTrTd2">
                                                                    <b>NOTE:&nbsp;</b>
                                                                </td>
                                                                <td class="vTopImp">
                                                                    <%# Eval("HtmlEncodedHTMLNote")%>
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
                                    <tr class="textleft bgcolor_ECE9D9">
                                        <td colspan="4" class="ProjectAccountName Width95Percent no-wrap padLeft40Imp">
                                            <%# GetDateFormat((DateTime)Eval("Date"))%>
                                        </td>
                                        <td class="PersonDetailTotal padRight110Imp">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDateDetails" runat="server">
                                    <table class="WholeWidthWithHeight WorkTypeTable">
                                        <asp:Repeater ID="repWorktype" DataSource='<%# Eval("DayTotalHoursList") %>' runat="server">
                                            <ItemTemplate>
                                                <tr class="FirstTr">
                                                    <td class="FirstTrTd1_GroupByBusinessUnit">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td class="FirstTrTd3_GroupByBusinessUnit">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" class="Width10Percent">
                                                    </td>
                                                </tr>
                                                <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                                    <td class="wrapword SecondTrTd1">
                                                        <table>
                                                            <tr>
                                                                <td class="SecondTrTd2">
                                                                    <b>NOTE:&nbsp;</b>
                                                                </td>
                                                                <td class="vTopImp">
                                                                    <%# Eval("HtmlEncodedHTMLNote")%>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td colspan="3">
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <tr class="FirstTr AlternativeFirstTr">
                                                    <td class="FirstTrTd1_GroupByBusinessUnit">
                                                        <%# Eval("TimeType.Name")%>
                                                    </td>
                                                    <td class="FirstTrTd3_GroupByBusinessUnit">
                                                        <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                                    </td>
                                                    <td colspan="2" class="Width10Percent">
                                                    </td>
                                                </tr>
                                                <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                                    <td class="wrapword SecondTrTd1">
                                                        <table>
                                                            <tr>
                                                                <td class="SecondTrTd2">
                                                                    <b>NOTE:&nbsp;</b>
                                                                </td>
                                                                <td class="vTopImp">
                                                                    <%# Eval("HtmlEncodedHTMLNote")%>
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
<div id="divEmptyMessage" style="display: none;" class="EmptyMessagediv" runat="server">
    There are no Time Entries towards this range selected.
</div>

