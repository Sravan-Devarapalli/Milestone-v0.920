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
        <div class="border_black">
            <table class="ConsultingDemandDetails">
                <thead>
                    <tr class="headerRow">
                        <th class="FirstTD">
                            <asp:LinkButton ID="btnTitleSkill" runat="server" CausesValidation="false" CommandArgument="TitleSkill"
                                Style="text-decoration: none; color: Black; width: 100%;" OnCommand="btnTitleSkill_Command">
                        Title/Skill Set
                            </asp:LinkButton>
                        </th>
                        <th class="SecondTD">
                            <asp:LinkButton ID="btnOpportunityNumber" runat="server" CausesValidation="false"
                                CommandArgument="OpportunityNumber" Style="text-decoration: none; color: Black;"
                                OnCommand="btnOpportunityNumber_Command">
                            Opportunity Number</asp:LinkButton>
                        </th>
                        <th class="SecondTD">
                            <asp:LinkButton ID="btnProjectNumber" runat="server" CausesValidation="false" CommandArgument="ProjectNumber"
                                Style="text-decoration: none; color: Black;" OnCommand="btnProjectNumber_Command">
                            Project Number
                            </asp:LinkButton>
                        </th>
                        <th class="ThirdTD">
                            <asp:LinkButton ID="btnAccountName" runat="server" CausesValidation="false" CommandArgument="AccountName"
                                Style="text-decoration: none; color: Black;" OnCommand="btnAccountName_Command">
                            Account Name
                            </asp:LinkButton>
                        </th>
                        <th class="ThirdTD">
                            <asp:LinkButton ID="btnProjectName" runat="server" CausesValidation="false" CommandArgument="ProjectName"
                                Style="text-decoration: none; color: Black;" OnCommand="btnProjectName_Command">
                            Project Name
                            </asp:LinkButton>
                        </th>
                        <th class="ThirdTD">
                            <asp:LinkButton ID="btnResourceStartDate" runat="server" CausesValidation="false"
                                CommandArgument="ResourceStartDate" Style="text-decoration: none; color: Black;"
                                OnCommand="btnResourceStartDate_Command">
                            Resource Start Date
                            </asp:LinkButton>
                        </th>
                        <th class="SecondTD">
                            <asp:LinkButton ID="btnTotal" runat="server" CausesValidation="false" CommandArgument="Total"
                                Style="text-decoration: none; color: Black;" OnCommand="btnTotal_Command">
                                <asp:Label ID="lblTotal" runat="server" Text='Total'></asp:Label></asp:LinkButton>
                            <asp:Panel ID="pnlTotal" Style="display: none;" runat="server" CssClass="pnlTotal">
                                <label class="fontBold">
                                    Total Forecasted Demand:
                                </label>
                                <asp:Label ID="lblTotalForecastedDemand" runat="server"></asp:Label>
                            </asp:Panel>
                        </th>
                    </tr>
                </thead>
            </table>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="ConsultingDemandDetails">
            <tr class="bgColorD4D0C9 textLeft">
                <td colspan="6" class="padLeft20Imp no-wrap">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Title/Skill Details"
                        ExpandedText="Collapse Title/Skill Details" EnableViewState="true" BehaviorID="cpeDetails"
                        Collapsed="true" TargetControlID="pnlTitleSkillDetails" ImageControlID="imgDetails"
                        CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                        ExpandControlID="imgDetails" TextLabelID="lbTitleSkill" />
                    <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Title/Skill Details" />
                    <asp:Label ID="lbTitleSkill" CssClass="displayNone" runat="server"></asp:Label>
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
            <asp:Repeater ID="repTitlesDetails" runat="server" OnItemDataBound="repTitlesDetails_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="ConsultingDemandDetails">
                        <tr class="bgcolorwhite">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="SecondTD">
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectId)) %>'>
                                </asp:HyperLink>
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
                            <td class="SecondTDTotals">
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
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="SecondTD">
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectId)) %>'>
                                </asp:HyperLink>
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
                            <td class="SecondTDTotals">
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
                <td colspan="6" class="padLeft20Imp no-wrap">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Title/Skill Details"
                        ExpandedText="Collapse Title/Skill Details" EnableViewState="true" BehaviorID="cpeDetails"
                        Collapsed="true" TargetControlID="pnlTitleSkillDetails" ImageControlID="imgDetails"
                        CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                        ExpandControlID="imgDetails" TextLabelID="lbTitleSkill" />
                    <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Title/Skill Details" />
                    <asp:Label ID="lbTitleSkill" CssClass="displayNone" runat="server"></asp:Label>
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
            <asp:Repeater ID="repTitlesDetails" runat="server" OnItemDataBound="repTitlesDetails_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="ConsultingDemandDetails">
                        <tr class="bgcolorwhite">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="SecondTD">
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectId)) %>'>
                                </asp:HyperLink>
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
                            <td class="SecondTDTotals">
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
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="SecondTD">
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetails)Container.DataItem).ProjectId)) %>'>
                                </asp:HyperLink>
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
                            <td class="SecondTDTotals">
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
        </div>
    </FooterTemplate>
</asp:Repeater>
<asp:Repeater ID="repByMonth" runat="server" OnItemDataBound="repByMonth_ItemDataBound">
    <HeaderTemplate>
        <div class="border_black">
            <table class="ConsultingDemandDetailsByMonth">
                <thead>
                    <tr class="headerRow">
                        <th class="FirstTD">
                            <asp:LinkButton ID="btnMonthYear" runat="server" CausesValidation="false" CommandArgument="MonthYear"
                                Style="text-decoration: none; color: Black;" OnCommand="btnMonthYear_Command">
                            Month Year
                            </asp:LinkButton>
                        </th>
                        <th class="SecondTD" id="thTitleSkill" runat="server">
                            <asp:LinkButton ID="btnMonthTitleSkill" runat="server" CausesValidation="false" CommandArgument="MonthTitleSkill"
                                Style="text-decoration: none; color: Black;" OnCommand="btnMonthTitleSkill_Command">
                            Title/Skill Set</asp:LinkButton>
                        </th>
                        <th class="ThirdTD">
                            <asp:LinkButton ID="btnMonthOpportunityNumber" runat="server" CausesValidation="false"
                                CommandArgument="MonthOpportunityNumber" Style="text-decoration: none; color: Black;"
                                OnCommand="btnMonthOpportunityNumber_Command">
                            Opportunity Number
                            </asp:LinkButton>
                        </th>
                        <th class="ThirdTD">
                            <asp:LinkButton ID="btnMonthProjectNumber" runat="server" CausesValidation="false"
                                CommandArgument="MonthProjectNumber" Style="text-decoration: none; color: Black;"
                                OnCommand="btnMonthProjectNumber_Command">
                            Project Number</asp:LinkButton>
                        </th>
                        <th class="ForthTD">
                            <asp:LinkButton ID="btnMonthAccountName" runat="server" CausesValidation="false"
                                CommandArgument="MonthAccountName" Style="text-decoration: none; color: Black;"
                                OnCommand="btnMonthAccountName_Command">
                            Account Name</asp:LinkButton>
                        </th>
                        <th class="ForthTD">
                            <asp:LinkButton ID="btnMonthProjectName" runat="server" CausesValidation="false"
                                CommandArgument="MonthProjectName" Style="text-decoration: none; color: Black;"
                                OnCommand="btnMonthProjectName_Command">
                            Project Name</asp:LinkButton>
                        </th>
                        <th class="ThirdTD">
                            <asp:LinkButton ID="btnMonthResourceStartDate" runat="server" CausesValidation="false"
                                CommandArgument="MonthResourceStartDate" Style="text-decoration: none; color: Black;"
                                OnCommand="btnMonthResourceStartDate_Command">
                            Resource Start Date</asp:LinkButton>
                        </th>
                        <th class="ThirdTD">
                            <asp:LinkButton ID="btnMonthTotal" runat="server" CausesValidation="false" CommandArgument="MonthTotal"
                                Style="text-decoration: none; color: Black;" OnCommand="btnMonthTotal_Command">
                                <asp:Label ID="lblTotal" runat="server" Text='Total'></asp:Label></asp:LinkButton>
                            <asp:Panel ID="pnlTotal" Style="display: none;" runat="server" CssClass="pnlTotal">
                                <label class="fontBold">
                                    Total Forecasted Demand:
                                </label>
                                <asp:Label ID="lblTotalForecastedDemand" runat="server"></asp:Label>
                            </asp:Panel>
                        </th>
                    </tr>
                </thead>
            </table>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="ConsultingDemandDetailsByMonth">
            <tr class="bgColorD4D0C9 textLeft">
                <td colspan="7" class="padLeft20Imp no-wrap Width70Per">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Month Details"
                        ExpandedText="Collapse Month Details" EnableViewState="true" BehaviorID="cpeDetails"
                        Collapsed="true" TargetControlID="pnlMonthDetails" ImageControlID="imgDetails"
                        CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                        ExpandControlID="imgDetails" TextLabelID="lbMonth" />
                    <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Month Details" />
                    <asp:Label ID="lbMonth" Style="display: none;" runat="server"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth)Container.DataItem).MonthStartDate.ToString(PraticeManagement.Constants.Formatting.FullMonthYearFormat)%>
                </td>
                <td class="SecondTD" id="tdTitleSkill" runat="server">
                </td>
                <td class="ConsultingDemandDetailsTotal">
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth)Container.DataItem).TotalCount%>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlMonthDetails" runat="server">
            <asp:Repeater ID="repDetails" runat="server" OnItemDataBound="repDetails_ItemDataBound">
                <ItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="bgcolorwhite">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD" id="tdTitleSkill" runat="server">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Title%>,
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Skill%>
                            </td>
                            <td class="ThirdTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="ThirdTD">
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectId)) %>'>
                                </asp:HyperLink>
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
                            <td class="ThirdTDTotals">
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
                            <td class="SecondTD" id="tdTitleSkill" runat="server">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Title%>,
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Skill%>
                            </td>
                            <td class="ThirdTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="ThirdTD">
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectId)) %>'>
                                </asp:HyperLink>
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
                            <td class="ThirdTDTotals">
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
                <td colspan="7" class="padLeft20Imp no-wrap Width70Per">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Month Details"
                        ExpandedText="Collapse Month Details" EnableViewState="true" BehaviorID="cpeDetails"
                        Collapsed="true" TargetControlID="pnlMonthDetails" ImageControlID="imgDetails"
                        CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                        ExpandControlID="imgDetails" TextLabelID="lbMonth" />
                    <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Month Details" />
                    <asp:Label ID="lbMonth" Style="display: none;" runat="server"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth)Container.DataItem).MonthStartDate.ToString(PraticeManagement.Constants.Formatting.FullMonthYearFormat)%>
                </td>
                <td class="SecondTD" id="tdTitleSkill" runat="server">
                </td>
                <td class="ConsultingDemandDetailsTotal">
                    <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth)Container.DataItem).TotalCount%>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlMonthDetails" runat="server">
            <asp:Repeater ID="repDetails" runat="server" OnItemDataBound="repDetails_ItemDataBound">
                <ItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="bgcolorwhite">
                            <td class="FirstTD">
                            </td>
                            <td class="SecondTD" id="tdTitleSkill" runat="server">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Title%>,
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Skill%>
                            </td>
                            <td class="ThirdTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="ThirdTD">
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectId)) %>'>
                                </asp:HyperLink>
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
                            <td class="ThirdTDTotals">
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
                            <td class="SecondTD" id="tdTitleSkill" runat="server">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Title%>,
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Skill%>
                            </td>
                            <td class="ThirdTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="ThirdTD">
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ProjectId)) %>'>
                                </asp:HyperLink>
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
                            <td class="ThirdTDTotals">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).Count%>
                            </td>
                        </tr>
                    </table>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
</asp:Repeater>

