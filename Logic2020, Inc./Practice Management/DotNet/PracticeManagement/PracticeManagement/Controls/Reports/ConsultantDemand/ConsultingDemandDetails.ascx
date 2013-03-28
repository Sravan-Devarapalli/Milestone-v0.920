<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsultingDemandDetails.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ConsultantDemand.ConsultingDemandDetails" %>
<asp:HiddenField ID="hdncpeExtendersIds" runat="server" Value="" />
<asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" class="Width90Percent PaddingBottom3">
            <asp:Button ID="btnExpandOrCollapseAll" runat="server" Text="Collapse All" UseSubmitBehavior="false"
                CssClass="Width100Px" ToolTip="Collapse All" />
            &nbsp;&nbsp;
            <asp:Button ID="btnGroupBy" runat="server" Text="Group By Month" UseSubmitBehavior="false"
                CssClass="Width130px" OnClick="btnGroupBy_OnClick" ToolTip="Group By Month" />
            <asp:HiddenField ID="hdnGroupBy" runat="server" />
            <asp:HiddenField ID="hdSkill" runat="server" />
            <asp:HiddenField ID="hdTitle" runat="server" />
            <asp:HiddenField ID="hdIsSummaryPage" runat="server" />
        </td>
    </tr>
</table>
<asp:Repeater ID="repByTitleSkill" runat="server" OnItemDataBound="repByTitleSkill_ItemDataBound">
    <HeaderTemplate>
        <table class="ConsultingDemandDetails">
            <thead>
                <tr class="headerRow">
                    <th class="FirstTD">
                        Title/Skill Set
                    </th>
                    <th class="SecondTD">
                        Opportunity Number
                    </th>
                    <th class="SecondTD">
                        Project Number
                    </th>
                    <th class="ThirdTD">
                        Account Name
                    </th>
                    <th class="ThirdTD">
                        Project Name
                    </th>
                    <th class="ThirdTD">
                        Resource Start Date
                    </th>
                    <th class="SecondTD">
                        Total
                    </th>
                </tr>
            </thead>
        </table>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="ConsultingDemandDetails">
            <tr class="bgColorD4D0C9 textLeft">
                <td colspan="6" class="ProjectAccountName padLeft20Imp no-wrap">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Title/Skill Details"
                        ExpandedText="Collapse Title/Skill Details" EnableViewState="true" BehaviorID="cpeDetails"
                        Collapsed="true" TargetControlID="pnlTitleSkillDetails" ImageControlID="imgDetails"
                        CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                        ExpandControlID="imgDetails" TextLabelID="lbTitleSkill" />
                    <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Title/Skill Details" />
                    <asp:Label ID="lbTitleSkill" Style="display: none;" runat="server"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).Title %>,
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).Skill%>
                </td>
                <td class="ConsultingDemandDetailsTotal">
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).TotalCount %>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlTitleSkillDetails" runat="server">
            <asp:Repeater ID="repDetails" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="ConsultingDemandDetails">
                        <tr class="bgcolorwhite">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityNumber%>
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectNumber%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).AccountName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).Count%>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="ConsultingDemandDetails">
                        <tr class="alterrow">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityNumber%>
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectNumber%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).AccountName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).Count%>
                            </td>
                        </tr>
                    </table>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </asp:Panel>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <table class="ConsultingDemandDetails">
            <tr class="bgcolor_ECE9D9 textLeft">
                <td colspan="6" class="ProjectAccountName padLeft20Imp no-wrap">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Title/Skill Details"
                        ExpandedText="Collapse Title/Skill Details" EnableViewState="true" BehaviorID="cpeDetails"
                        Collapsed="true" TargetControlID="pnlTitleSkillDetails" ImageControlID="imgDetails"
                        CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                        ExpandControlID="imgDetails" TextLabelID="lbTitleSkill" />
                    <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Title/Skill Details" />
                    <asp:Label ID="lbTitleSkill" Style="display: none;" runat="server"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).Title %>,
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).Skill%>
                </td>
                <td class="ConsultingDemandDetailsTotal">
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).TotalCount %>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlTitleSkillDetails" runat="server">
            <asp:Repeater ID="repDetails" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="ConsultingDemandDetails">
                        <tr class="bgcolorwhite">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityNumber%>
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectNumber%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).AccountName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).Count%>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="ConsultingDemandDetails">
                        <tr class="alterrow">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityNumber%>
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectNumber%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).AccountName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).Count%>
                            </td>
                        </tr>
                    </table>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </asp:Panel>
    </AlternatingItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
<asp:Repeater ID="repByMonth" runat="server" OnItemDataBound="repByMonth_ItemDataBound">
    <HeaderTemplate>
        <table class="ConsultingDemandDetailsByMonth">
            <thead>
                <tr class="headerRow">
                    <th class="FirstTD">
                        Month Year
                    </th>
                    <th class="SecondTD">
                        Title/Skill Set
                    </th>
                    <th class="ThirdTD">
                        Opportunity Number
                    </th>
                    <th class="ThirdTD">
                        Project Number
                    </th>
                    <th class="ForthTD">
                        Account Name
                    </th>
                    <th class="ForthTD">
                        Project Name
                    </th>
                    <th class="ThirdTD">
                        Resource Start Date
                    </th>
                    <th class="ThirdTD">
                        Total
                    </th>
                </tr>
            </thead>
        </table>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="ConsultingDemandDetailsByMonth">
            <tr class="bgColorD4D0C9 textLeft">
                <td colspan="7" class="ProjectAccountName padLeft20Imp no-wrap">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Month Details"
                        ExpandedText="Collapse Month Details" EnableViewState="true" BehaviorID="cpeDetails"
                        Collapsed="true" TargetControlID="pnlMonthDetails" ImageControlID="imgDetails"
                        CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                        ExpandControlID="imgDetails" TextLabelID="lbMonth" />
                    <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Month Details" />
                    <asp:Label ID="lbMonth" Style="display: none;" runat="server"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth)Container.DataItem).MonthStartDate.ToString("MMMM-yyyy")%>
                </td>
                <td class="ConsultingDemandDetailsTotal">
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth)Container.DataItem).TotalCount%>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlMonthDetails" runat="server">
            <asp:Repeater ID="repDetails" runat="server">
                <ItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="bgcolorwhite">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Title%>,
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Skill%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectNumber%>
                            </td>
                            <td class="ForthTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).AccountName%>
                            </td>
                            <td class="ForthTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Count%>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="alterrow">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Title%>,
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Skill%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectNumber%>
                            </td>
                            <td class="ForthTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).AccountName%>
                            </td>
                            <td class="ForthTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Count%>
                            </td>
                        </tr>
                    </table>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <table class="ConsultingDemandDetailsByMonth">
            <tr class="bgcolor_ECE9D9 textLeft">
                <td colspan="7" class="ProjectAccountName padLeft20Imp no-wrap">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Month Details"
                        ExpandedText="Collapse Month Details" EnableViewState="true" BehaviorID="cpeDetails"
                        Collapsed="true" TargetControlID="pnlMonthDetails" ImageControlID="imgDetails"
                        CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                        ExpandControlID="imgDetails" TextLabelID="lbMonth" />
                    <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Month Details" />
                    <asp:Label ID="lbMonth" Style="display: none;" runat="server"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth)Container.DataItem).MonthStartDate.ToString("MMMM-yyyy")%>
                </td>
                <td class="ConsultingDemandDetailsTotal">
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth)Container.DataItem).TotalCount%>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlMonthDetails" runat="server">
            <asp:Repeater ID="repDetails" runat="server">
                <ItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="bgcolorwhite">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Title%>,
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Skill%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectNumber%>
                            </td>
                            <td class="ForthTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).AccountName%>
                            </td>
                            <td class="ForthTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Count%>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="alterrow">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Title%>,
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Skill%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectNumber%>
                            </td>
                            <td class="ForthTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).AccountName%>
                            </td>
                            <td class="ForthTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectName%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                            <td class="ThirdTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Count%>
                            </td>
                        </tr>
                    </table>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </AlternatingItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>

