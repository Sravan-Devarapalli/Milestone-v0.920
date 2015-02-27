<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonBadgeHistory.ascx.cs"
    Inherits="PraticeManagement.Controls.Persons.PersonBadgeHistory" %>
<table class="WholeWidth">
    <tr>
        <td style="padding-top:10px">
            <span style="font-size: 14px; font-weight: bold; padding-left: 10px;">Block History:</span>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Repeater ID="repBlockHistory" runat="server" OnItemDataBound="repBlockHistory_DataBound">
                <HeaderTemplate>
                    <div style="width: 800px; padding: 15px">
                        <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport zebra WholeWidth">
                            <thead>
                                <tr>
                                    <th style="width:200px;">
                                        Block Start Date
                                    </th>
                                    <th style="width:200px;">
                                        Block End Date
                                    </th>
                                    <th  style="width:200px;">
                                        Updated By
                                    </th>
                                    <th  style="width:200px;">
                                        Updated On
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="ReportItemTemplate">
                        <td class="padLeft5">
                            <asp:Label ID="lblBlockStartDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBlockEndDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <%# Eval("ModifiedBy")%>
                        </td>
                        <td>
                            <%# GetDateTimeFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="alterrow">
                        <td class="padLeft5">
                            <asp:Label ID="lblBlockStartDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBlockEndDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <%# Eval("ModifiedBy")%>
                        </td>
                        <td>
                            <%# GetDateTimeFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </tbody></table></div>
                </FooterTemplate>
            </asp:Repeater>
            <div id="divBlockEmptyMessage" class="MSBadgeEmptyDiv" style="display: none; width: 100%;
                padding: 10px;" runat="server">
                No Block History is available for this person.
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <span style="font-size: 14px; font-weight: bold; padding-left: 10px;">Override History:</span>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Repeater ID="repOverrideHistory" runat="server" OnItemDataBound="repOverrideHistory_DataBound">
                <HeaderTemplate>
                    <div style="width: 800px; padding: 15px">
                        <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport zebra WholeWidth">
                            <thead>
                                <tr>
                                    <th  style="width:200px;">
                                        Override Start Date
                                    </th>
                                    <th  style="width:200px;">
                                        Override End Date
                                    </th>
                                    <th  style="width:200px;">
                                        Updated By
                                    </th>
                                    <th style="width:200px;">
                                        Updated On
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="ReportItemTemplate">
                        <td class="padLeft5 ">
                            <asp:Label ID="lblOverrideStartDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblOverrideEndDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <%# Eval("ModifiedBy")%>
                        </td>
                        <td>
                            <%# GetDateTimeFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="alterrow">
                        <td class="padLeft5 ">
                            <asp:Label ID="lblOverrideStartDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblOverrideEndDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <%# Eval("ModifiedBy")%>
                        </td>
                        <td>
                            <%# GetDateTimeFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </tbody></table></div>
                </FooterTemplate>
            </asp:Repeater>
            <div id="divOverrideEmpty" class="MSBadgeEmptyDiv" style="display: none; width: 100%;
                padding: 10px;" runat="server">
                No Override History is available for this person.
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <span style="font-size: 14px; font-weight: bold; padding-left: 10px;">18 mos dates History:</span>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Repeater ID="repBadgeHistory" runat="server" OnItemDataBound="repBadgeHistory_DataBound">
                <HeaderTemplate>
                    <div style="width:1350px; padding: 15px">
                        <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport zebra WholeWidth">
                            <thead>
                                <tr>
                                    <th style="width:150px;" class="TextAlignLeftImp">
                                        18 Mos Start Date
                                    </th>
                                    <th style="width:150px;" class="">
                                        18 Mos Start Date Source
                                    </th>
                                    <th style="width:150px;" class="">
                                        Project Planned End Date
                                    </th>
                                    <th  style="width:150px;" class="">
                                        Project Planned End Date Source
                                    </th>
                                    <th  style="width:150px;" class="">
                                        18 Mos End Date
                                    </th>
                                    <th  style="width:150px;" class="">
                                        18 Mos End Date Source
                                    </th>
                                    <th style="width:150px;" class="">
                                        6 Mos Break Start Date
                                    </th>
                                    <th style="width:150px;" class="">
                                        6 Mos Break End Date
                                    </th>
                                    <th style="width:150px;" class="">
                                        Updated By
                                    </th>
                                    <th style="width:150px;" class="">
                                        Updated On
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="ReportItemTemplate">
                        <td class="padLeft5 textLeft">
                            <asp:Label ID="lblBadgeStartDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBadgeStartDateSource" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblProjectPlannedEndDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblProjectPlannedEndDateSource" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBadgeEndDate" runat="server"></asp:Label>
                        </td>
                         <td>
                            <asp:Label ID="lblBadgeEndDateSource" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBreakStartDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBreakEndDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <%# Eval("ModifiedBy")%>
                        </td>
                        <td>
                            <%# GetDateTimeFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="alterrow">
                        <td class="padLeft5 textLeft">
                            <asp:Label ID="lblBadgeStartDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBadgeStartDateSource" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblProjectPlannedEndDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblProjectPlannedEndDateSource" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBadgeEndDate" runat="server"></asp:Label>
                        </td>
                         <td>
                            <asp:Label ID="lblBadgeEndDateSource" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBreakStartDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblBreakEndDate" runat="server"></asp:Label>
                        </td>
                        <td>
                            <%# Eval("ModifiedBy")%>
                        </td>
                        <td>
                            <%# GetDateTimeFormat((DateTime)Eval("ModifiedDate"))%>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </tbody></table></div>
                </FooterTemplate>
            </asp:Repeater>
            <div id="divBadgeHistoryEmpty" class="MSBadgeEmptyDiv" style="display: none; width: 100%;
                padding: 10px;" runat="server">
                No 18 Mos dates History is available for this person.
            </div>
        </td>
    </tr>
</table>

