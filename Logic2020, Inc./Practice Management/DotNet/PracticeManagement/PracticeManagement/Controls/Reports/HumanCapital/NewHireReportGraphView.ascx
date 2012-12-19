<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewHireReportGraphView.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.HumanCapital.NewHireGraphView" %>
<%@ Register Src="~/Controls/Reports/HumanCapital/NewHireReportSummaryView.ascx"
    TagPrefix="uc" TagName="SummaryView" %>
<div class="BackGroundWhiteImp">
    <div class="NewHireReportChartDiv" id="NewHireReportChartDiv" runat="server">
        <div style="text-align: left; padding: 15px;">
            <asp:LinkButton runat="server" Text="See New Hires by Recruiter" ID="hlnkGraph" EnableViewState="true"
                OnClick="hlnkGraph_Click"></asp:LinkButton>
        </div>
        <asp:Chart ID="chrtNewHireReportByRecruiter" runat="server" OnClick="chrtNewHireReport_Click"
            EnableViewState="true">
            <Series>
                <asp:Series Name="chartSeries" ChartArea="MainArea" ChartType="Column" XValueType="String"
                    YValueType="Int32" PostBackValue="#VALX:#VALY:False" ToolTip="#VALY New Hires"
                    XAxisType="Primary" YAxisType="Primary" XValueMember="name" YValueMembers="count">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="MainArea">
                </asp:ChartArea>
            </ChartAreas>
        </asp:Chart>
        <asp:Chart ID="chrtNewHireReportByTitle" runat="server" OnClick="chrtNewHireReport_Click"
            EnableViewState="true">
            <Series>
                <asp:Series Name="chartSeries" ChartArea="MainArea" ChartType="Column" XValueType="String"
                    YValueType="Int32" PostBackValue="#VALX:#VALY:True" ToolTip="#VALY New Hires"
                    XAxisType="Primary" YAxisType="Primary" XValueMember="name" YValueMembers="count">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="MainArea">
                    <AxisX>
                        <LabelStyle Format="###" />
                    </AxisX>
                </asp:ChartArea>
            </ChartAreas>
        </asp:Chart>
    </div>
</div>
<div id="divEmptyMessage" class="EmptyMessagediv NewHireGraphEmptyDiv" style="display: none;"
    runat="server">
    No people were hired in the time range selected and/or that match the selected filters.
</div>
<asp:HiddenField ID="hndDetailView" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeDetailView" runat="server" TargetControlID="hndDetailView"
    BackgroundCssClass="modalBackground" PopupControlID="pnlDetailView" BehaviorID="mpeDetailView"
    CancelControlID="btnClose" DropShadow="false" />
<asp:Panel ID="pnlDetailView" runat="server" class="tab-pane" Style="width: 1200px;">
    <table class="WholeWidth Padding5">
        <tbody>
            <tr class="bgGroupByProjectHeader">
                <td class="Width1Percent">
                </td>
                <td class="Width99Percent">
                    <table class="WholeWidthWithHeight NewHireGraphPopUpTable">
                        <tbody>
                            <tr class="textleft">
                                <td class="ProjectAccountName FirstTd">
                                    <asp:Label ID="lbName" runat="server"></asp:Label>
                                </td>
                                <td class="SecondTd">
                                    <asp:Label ID="lbTotalHires" runat="server"></asp:Label>
                                    New Hires
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="Width1Percent">
                </td>
                <td class="Width99Percent">
                    <div class="NewHireREportGraphDiv">
                        <uc:SummaryView ID="tpSummary" runat="server"></uc:SummaryView>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
    <table class="CloseButtonTable">
        <tr>
            <td colspan="4" class="Width95Percent">
            </td>
            <td class=" Width5Percent padRight5">
                <asp:Button ID="btnClose" runat="server" Text="Close" UseSubmitBehavior="false" ToolTip="Close"
                    OnClientClick="return false;" />
            </td>
        </tr>
    </table>
</asp:Panel>

