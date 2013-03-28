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
                    </th>
                    <th>
                        <asp:Label ID="lblTitleSkill" runat="server"></asp:Label>
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
            <tr>
                <th class="padLeft20Imp">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetail" runat="Server" CollapsedText="Expand Title Details"
                        ExpandedText="Collapse Title Details" EnableViewState="true" BehaviorID="cpeDetail"
                        Collapsed="true" TargetControlID="pnlTitleDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                        TextLabelID="lbDate" />
                    <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                    <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                    <asp:Label ID="lblHeader" runat="server"></asp:Label>
                </th><th></th><th></th><th></th><th></th><th></th><th></th>
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
                                <asp:Label ID="lblOpportunityNumber" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblProjectNumber" runat="server"></asp:Label>
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
                                <asp:Label ID="lblOpportunityNumber" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblProjectNumber" runat="server"></asp:Label>
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
            <tr>
                <th class="padLeft20Imp">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetail" runat="Server" CollapsedText="Expand Title Details"
                        ExpandedText="Collapse Title Details" EnableViewState="true" BehaviorID="cpeDetail"
                        Collapsed="true" TargetControlID="pnlTitleDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                        TextLabelID="lbDate" />
                    <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                    <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                    <asp:Label ID="lblHeader" runat="server"></asp:Label>
                </th><th></th><th></th><th></th><th></th><th></th><th></th>
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
                                <asp:Label ID="lblOpportunityNumber" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblProjectNumber" runat="server"></asp:Label>
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
                                <asp:Label ID="lblOpportunityNumber" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblProjectNumber" runat="server"></asp:Label>
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

