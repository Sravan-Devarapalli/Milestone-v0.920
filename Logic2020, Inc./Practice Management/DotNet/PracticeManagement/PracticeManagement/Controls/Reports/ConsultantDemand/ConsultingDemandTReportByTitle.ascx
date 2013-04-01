<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsultingDemandTReportByTitle.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ConsultantDemand.ConsultingDemandTReportByTitle" %>
<asp:HiddenField ID="hdncpeExtendersIds" runat="server" Value="" />
<asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" class="Width90Percent">
            <asp:Button ID="btnExpandOrCollapseAll" runat="server" Text="Collapse All" UseSubmitBehavior="false"
                CssClass="Width100Px" ToolTip="Collapse All" />
            &nbsp;&nbsp;
        </td>
        <td class=" Width5Percent padRight5">
            <table class="WholeWidth">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To Excel" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Repeater ID="repTitles" runat="server" OnItemDataBound="repTitles_ItemDataBound">
    <HeaderTemplate>
        <div class="consultngDetailPanel">
            <table class="trConsultngDetailPanel">
                <tr>
                    <th>
                        <asp:Label ID="lblTitleSkillFr" runat="server"></asp:Label>
                    </th>
                    <th>
                        <asp:Label ID="lblTitleSkillSc" runat="server"></asp:Label>
                    </th>
                    <th>
                        Opportunity Number
                    </th>
                    <th>
                        Project Number
                    </th>
                    <th>
                        Account Name
                    </th>
                    <th class="prjctnamewidth20">
                        Project Name
                    </th>
                    <th>
                        Resource Start Date
                    </th>
                </tr>
            </table>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="trConsultngDetailPanelItem">
            <tr class="textLeft">
                <th class="padLeft10Imp">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetail" runat="Server" CollapsedText="Expand Title Details"
                        ExpandedText="Collapse Title Details" EnableViewState="true" BehaviorID="cpeDetail"
                        Collapsed="true" TargetControlID="pnlTitleDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                        TextLabelID="lbDate" />
                    <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                    <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                    <asp:Label ID="lblHeader" runat="server"></asp:Label>
                </th>
                <th>
                </th>
                <th>
                </th>
                <th>
                </th>
                <th>
                </th>
                <th>
                </th>
                <th>
                </th>
            </tr>
        </table>
        <asp:Panel ID="pnlTitleDetails" runat="server">
            <asp:Repeater ID="repDetails" runat="server" OnItemDataBound="repDetails_ItemDataBound">
                <ItemTemplate>
                    <table class="trConsultngDetailPanelInnerItem">
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Label ID="lblTitleSkillItem" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Target="_blank">
                                </asp:HyperLink>
                            </td>
                            <td>
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Target="_blank">
                                </asp:HyperLink>
                            </td>
                            <td>
                                <asp:Label ID="lblAccountName" runat="server"></asp:Label>
                            </td>
                            <td class="prjctnamewidth20">
                                <asp:Label ID="lblProjectName" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblRsrcStartDate" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="alternateTrConsultngDetailPanelInnerItem">
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Label ID="lblTitleSkillItem" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Target="_blank">
                                </asp:HyperLink>
                            </td>
                            <td>
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Target="_blank">
                                </asp:HyperLink>
                            </td>
                            <td>
                                <asp:Label ID="lblAccountName" runat="server"></asp:Label>
                            </td>
                            <td class="prjctnamewidth20">
                                <asp:Label ID="lblProjectName" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblRsrcStartDate" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <table class="alternateTrConsultngDetailPanelItem">
            <tr class="textLeft">
                <th class="padLeft10Imp">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetail" runat="Server" CollapsedText="Expand Title Details"
                        ExpandedText="Collapse Title Details" EnableViewState="true" BehaviorID="cpeDetail"
                        Collapsed="true" TargetControlID="pnlTitleDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                        TextLabelID="lbDate" />
                    <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                    <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                    <asp:Label ID="lblHeader" runat="server"></asp:Label>
                </th>
                <th>
                </th>
                <th>
                </th>
                <th>
                </th>
                <th>
                </th>
                <th>
                </th>
                <th>
                </th>
            </tr>
        </table>
        <asp:Panel ID="pnlTitleDetails" runat="server">
            <asp:Repeater ID="repDetails" runat="server" OnItemDataBound="repDetails_ItemDataBound">
                <ItemTemplate>
                    <table class="trConsultngDetailPanelInnerItem">
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Label ID="lblTitleSkillItem" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Target="_blank">
                                </asp:HyperLink>
                            </td>
                            <td>
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Target="_blank">
                                </asp:HyperLink>
                            </td>
                            <td>
                                <asp:Label ID="lblAccountName" runat="server"></asp:Label>
                            </td>
                            <td class="prjctnamewidth20">
                                <asp:Label ID="lblProjectName" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblRsrcStartDate" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="alternateTrConsultngDetailPanelInnerItem">
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Label ID="lblTitleSkillItem" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Target="_blank">
                                </asp:HyperLink>
                            </td>
                            <td>
                                <asp:HyperLink ID="hlProjectNumber" runat="server" Target="_blank">
                                </asp:HyperLink>
                            </td>
                            <td>
                                <asp:Label ID="lblAccountName" runat="server"></asp:Label>
                            </td>
                            <td class="prjctnamewidth20">
                                <asp:Label ID="lblProjectName" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblRsrcStartDate" runat="server"></asp:Label>
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
<asp:Repeater ID="repByMonth" runat="server" OnItemDataBound="repByMonth_ItemDataBound">
    <HeaderTemplate>
        <table class="ConsultingDemandDetailsByMonth">
            <thead>
                <tr class="headerRow">
                    <th class="SecondTD">
                        Month Year
                    </th>
                    <th class="FirstTD">
                        <asp:Label ID="lblTilteOrSkillHeader" runat="server"></asp:Label>
                    </th>
                    <th class="ForthTD">
                        Opportunity Number
                    </th>
                    <th class="ForthTD">
                        Project Number
                    </th>
                    <th class="ForthTD">
                        Account Name
                    </th>
                    <th class="ForthTD">
                        Project Name
                    </th>
                    <th class="FirstTD">
                        Resource Start Date
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
            </tr>
        </table>
        <asp:Panel ID="pnlMonthDetails" runat="server">
            <asp:Repeater ID="repByMonthDetails" runat="server" OnItemDataBound="repByMonthDetails_ItemDataBound">
                <ItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="bgcolorwhite">
                            <td class="SecondTD">
                            </td>
                            <td class="FirstTD">
                                <asp:Label ID="lblTilteOrSkillItem" runat="server"></asp:Label>
                            </td>
                            <td class="ForthTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="ForthTD">
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
                            <td class="FirstTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="bgcolorwhite">
                            <td class="SecondTD">
                            </td>
                            <td class="FirstTD">
                                <asp:Label ID="lblTilteOrSkillItem" runat="server"></asp:Label>
                            </td>
                            <td class="ForthTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="ForthTD">
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
                            <td class="FirstTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
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
            </tr>
        </table>
        <asp:Panel ID="pnlMonthDetails" runat="server">
            <asp:Repeater ID="repByMonthDetails" runat="server" OnItemDataBound="repByMonthDetails_ItemDataBound">
                <ItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="bgcolorwhite">
                            <td class="SecondTD">
                            </td>
                            <td class="FirstTD">
                                <asp:Label ID="lblTilteOrSkillItem" runat="server"></asp:Label>
                            </td>
                            <td class="ForthTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="ForthTD">
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
                            <td class="FirstTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="ConsultingDemandDetailsByMonth">
                        <tr class="bgcolorwhite">
                            <td class="SecondTD">
                            </td>
                            <td class="FirstTD">
                                <asp:Label ID="lblTilteOrSkillItem" runat="server"></asp:Label>
                            </td>
                            <td class="ForthTD">
                                <asp:HyperLink ID="hlOpportunityNumber" runat="server" Text=' <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityNumber%>'
                                    Target="_blank" NavigateUrl='<%# GetOpportunityDetailsLink((int?)(((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).OpportunityId)) %>'>
                                </asp:HyperLink>
                            </td>
                            <td class="ForthTD">
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
                            <td class="FirstTD">
                                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantDemandDetailsByMonth)Container.DataItem).ResourceStartDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
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

