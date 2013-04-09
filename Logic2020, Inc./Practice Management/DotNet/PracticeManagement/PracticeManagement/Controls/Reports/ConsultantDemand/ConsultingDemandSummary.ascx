<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsultingDemandSummary.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ConsultantDemand.ConsultingDemandSummary" %>
<%@ Register Src="~/Controls/Reports/ConsultantDemand/ConsultingDemandDetails.ascx"
    TagPrefix="uc" TagName="ConsultantDetailReport" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<table class="WholeWidthWithHeight ">
    <tr>
        <td colspan="4" class="Width95Percent">
        </td>
        <td class=" Width5Percent padRight5 PaddingBottom3">
            <table class="WholeWidth">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" UseSubmitBehavior="false"
                            ToolTip="Export To Excel" OnClick="btnExportToExcel_OnClick" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Panel ID="pnlFilterTitle" Style="display: none;" runat="server">
    <uc:FilteredCheckBoxList ID="cblTitle" runat="server" />
</asp:Panel>
<asp:Panel ID="pnlFilterSkill" Style="display: none;" runat="server">
    <uc:FilteredCheckBoxList ID="cblSkill" runat="server" />
</asp:Panel>
<asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
<asp:Repeater ID="repResource" runat="server" OnItemDataBound="repResource_ItemDataBound">
    <HeaderTemplate>
        <div class="minheight200Px">
            <table id="tblConsultingDemandSummary" class="tablesorter PersonSummaryReport WholeWidth zebra">
                <thead>
                    <tr class="TimeperiodSummaryReportTr bgcolorwhite">
                        <th class="ResourceColum padLeft5Imp">
                            Title/SkillSet
                            <img alt="Filter" src="../../../Images/Title.png" runat="server" title="Title"
                                id="imgTitleFilter" />
                            <AjaxControlToolkit:PopupControlExtender ID="pceTitleFilter" runat="server" TargetControlID="imgTitleFilter"
                                BehaviorID="pceTitleFilter" PopupControlID="pnlFilterTitle" Position="Bottom">
                            </AjaxControlToolkit:PopupControlExtender>
                            <img alt="Filter" src="../../../Images/Skill_2.png" runat="server" title="Skill"
                                id="imgSkillFilter" />
                            <AjaxControlToolkit:PopupControlExtender ID="pceSkillFilter" runat="server" TargetControlID="imgSkillFilter"
                                BehaviorID="pceSkillFilter" PopupControlID="pnlFilterSkill" Position="Bottom">
                            </AjaxControlToolkit:PopupControlExtender>
                        </th>
                        <asp:Repeater ID="repMonthHeader" runat="server" OnItemDataBound="repMonthHeader_ItemDataBound">
                            <ItemTemplate>
                                <th>
                                    <asp:Label ID="lblMonthName" runat="server" Text='<%# (string)Container.DataItem%>'></asp:Label>
                                    <asp:Panel ID="pnlMonthName" Style="display: none;" runat="server" CssClass="pnlTotal">
                                        <label class="fontBold">
                                            Total Forecasted Demand:
                                        </label>
                                        <asp:Label ID="lblTotalForecastedDemand" runat="server"></asp:Label>
                                    </asp:Panel>
                                </th>
                            </ItemTemplate>
                        </asp:Repeater>
                        <th>
                            <asp:Label ID="lblTotal" runat="server" Text='Total'></asp:Label>
                            <asp:Panel ID="pnlTotal" Style="display: none;" runat="server" CssClass="pnlTotal">
                                <label class="fontBold">
                                    Total Forecasted Demand:
                                </label>
                                <asp:Label ID="lblTotalForecastedDemand" runat="server"></asp:Label>
                            </asp:Panel>
                        </th>
                    </tr>
                </thead>
                <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="bgcolorwhite">
            <td>
                <table class="TdLevelNoBorder ResourceColumTable">
                    <tr>
                        <td>
                            <asp:LinkButton runat="server" ID="lnkConsultant" Text='<%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).TitleSkill %>'
                                ToolTip='<%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).TitleSkill %>'
                                Title='<%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).Title %>'
                                Skill='<%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).Skill %>'
                                OnClick="lnkConsultant_OnClick"></asp:LinkButton>
                        </td>
                        <td>
                            <img id="imgZoomIn" runat="server" src="~/Images/Zoom-In-icon.png" style="visibility: hidden;" />
                        </td>
                    </tr>
                </table>
            </td>
            <asp:Repeater ID="repMonthDemandCounts" runat="server">
                <ItemTemplate>
                    <td>
                        <%# ((int)Container.DataItem).ToString()%>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td>
                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).TotalCount %>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="alterrow">
            <td>
                <table class="TdLevelNoBorder ResourceColumTable">
                    <tr>
                        <td>
                            <asp:LinkButton runat="server" ID="lnkConsultant" Text='<%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).TitleSkill %>'
                                ToolTip='<%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).TitleSkill %>'
                                Title='<%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).Title %>'
                                Skill='<%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).Skill %>'
                                OnClick="lnkConsultant_OnClick"></asp:LinkButton>
                        </td>
                        <td>
                            <img id="imgZoomIn" runat="server" src="~/Images/Zoom-In-icon.png" style="visibility: hidden;" />
                        </td>
                    </tr>
                </table>
            </td>
            <asp:Repeater ID="repMonthDemandCounts" runat="server">
                <ItemTemplate>
                    <td>
                        <%# ((int)Container.DataItem).ToString()%>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
            <td>
                <%# ((DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill)Container.DataItem).TotalCount %>
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </tbody></table></div>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
    There are no strawmens for the selected range.
</div>
<asp:HiddenField ID="hdnTempField" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeConsultantDetailReport" runat="server"
    TargetControlID="hdnTempField" CancelControlID="btnCancelConsultantDetailReport"
    BackgroundCssClass="modalBackground" PopupControlID="pnlConsultantDetailReport"
    DropShadow="false" />
<asp:Panel ID="pnlConsultantDetailReport" CssClass="TimePeriodSummaryPersonDetailReportPopUp"
    Style="display: none;" runat="server">
    <table class="WholeWidth Padding5">
        <tr class="bgGroupByProjectHeader">
            <td class="Width1Percent">
            </td>
            <td class="Width99Percent">
                <table class="WholeWidthWithHeight GroupByProjectHeaderTable">
                    <tr class="textleft">
                        <td class="ProjectAccountName FirstTd">
                            <asp:Label ID="lblConsultant" runat="server"></asp:Label>
                        </td>
                        <td class="SecondTd">
                            <asp:Label ID="lblTotalCount" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="Width1Percent">
            </td>
            <td class="Width99Percent">
                <div class="GroupByProjectDiv">
                    <table class="WholeWidth">
                        <tr>
                            <td class="Width99Percent paddingBottom5px">
                                <uc:ConsultantDetailReport ID="ConsultantDetailReport" runat="server" />
                            </td>
                            <td class="Width1Percent">
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <table class="CloseButtonTable">
        <tr>
            <td>
                <asp:Button ID="btnCancelConsultantDetailReport" Text="Close" ToolTip="Close" runat="server" />
            </td>
        </tr>
    </table>
</asp:Panel>

