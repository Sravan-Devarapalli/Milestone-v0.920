<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TerminationReportGraphView.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.HumanCapital.TerminationReportGraphView" %>
<%@ Register Src="~/Controls/Reports/HumanCapital/TerminationReportSummaryView.ascx"
    TagPrefix="uc" TagName="SummaryView" %>
<div style="background-color: White !important;">
    <asp:UpdatePanel ID="upnlBody" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="text-align: left; padding: 15px;">
                <asp:LinkButton runat="server" Text="See Year To Date" ID="hlnkGraph" EnableViewState="true"
                    OnClick="hlnkGraph_Click"></asp:LinkButton>
            </div>
            <div class="ConsultingDemandchartDiv">
                <asp:Chart ID="chrtTerminationAndAttritionLast12Months" runat="server" OnClick="chrtTerminationAndAttritionLast12Months_Click"
                    EnableViewState="true">
                    <Series>
                        <asp:Series Name="chartSeries" ChartArea="MainArea" ChartType="Column" XValueType="String"
                            YValueType="Int32" PostBackValue="#VALX,#VALY,False" ToolTip="#VALY Terminations"
                            XAxisType="Primary" YAxisType="Primary" XValueMember="Month" YValueMembers="TerminationsCountInTheRange">
                        </asp:Series>
                        <asp:Series Name="chartSeries1" ChartArea="MainArea" ChartType="Point" XValueType="String"
                            YValueType="Double" ToolTip="#VALY{P} Attrition" XAxisType="Primary" YAxisType="Secondary"
                            XValueMember="Month" YValueMembers="Attrition">
                        </asp:Series>
                        <asp:Series Name="chartSeries2" ChartArea="MainArea" ChartType="Line" XValueType="String"
                            YValueType="Double" ToolTip="#VALY{P} Attrition" XAxisType="Primary" YAxisType="Secondary"
                            XValueMember="Month" YValueMembers="Attrition">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="MainArea">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
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
                                    <asp:Label ID="lbTotalTerminations" runat="server"></asp:Label>
                                    Terminations
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

