<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsultingDemandGraphs.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ConsultantDemand.ConsultingDemandGraphs" %>
<%@ Register Src="~/Controls/Reports/ConsultantDemand/ConsultingDemandTReportByTitle.ascx"
    TagName="ConsultngTReport" TagPrefix="uc" %>
<div class="BackGroundWhiteImp">
    <asp:UpdatePanel ID="upnlBody" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="TextAlignLeft Padding15Px">
                <asp:LinkButton runat="server" Text="Pipeline SkillSet Demand By Month" ID="hlnkGraph"
                    EnableViewState="true" Visible="false" OnClick="hlnkGraph_Click"></asp:LinkButton>
            </div>
            <div class="ConsultingDemandchartDiv">
                <asp:Chart ID="chartConsultngDemand" runat="server" OnClick="chartConsultngDemand_Click"
                    EnableViewState="true">
                    <Series>
                        <asp:Series Name="chartSeries" ChartArea="MainArea" ChartType="Column" XValueType="String"
                            XAxisType="Primary" YAxisType="Primary" YValueType="Int32" PostBackValue="#VALX,#VALY,False"
                            ToolTip="#VALY Resources" XValueMember="month" YValueMembers="count">
                        </asp:Series>
                        <asp:Series Name="chartSeries1" ChartArea="MainArea" ChartType="Point" XValueType="String"
                            XAxisType="Primary" YAxisType="Primary" YValueType="Double" XValueMember="month"
                            YValueMembers="count" ToolTip="#VALY Resources">
                        </asp:Series>
                        <asp:Series Name="chartSeries2" ChartArea="MainArea" ChartType="Line" XValueType="String"
                            XAxisType="Primary" YAxisType="Primary" YValueType="Double" XValueMember="month"
                            YValueMembers="count" ToolTip="#VALY Resources">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="MainArea">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
                <asp:Chart ID="chartConsultnDemandPipeline" runat="server" EnableViewState="true"
                    OnClick="chartConsultnDemandPipeline_Click">
                    <Series>
                        <asp:Series Name="seriesPipeline" ChartArea="PipelineArea" ChartType="Bar" XValueType="String"
                            YValueType="Int32" PostBackValue="#VALX,#VALY,False" XValueMember="title" YValueMembers="count"
                            ToolTip="#VALY Resources">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="PipelineArea">
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
<asp:Panel ID="pnlDetailView" runat="server" class="tab-pane Width1200px">
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
                                    <asp:Label ID="lblMonth" runat="server"></asp:Label>
                                </td>
                                <td class="SecondTd">
                                    <asp:Label ID="lblCount" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="Width1Percent">
                </td>
                <td class="Width99Percent paddingBottom5px">
                    <div class="NewHireREportGraphDiv">
                        <uc:ConsultngTReport ID="ctrDetails" runat="server"></uc:ConsultngTReport>
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

