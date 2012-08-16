<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewHireReportGraphView.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.HumanCapital.NewHireGraphView" %>
<%@ Register Src="~/Controls/Reports/HumanCapital/NewHireReportSummaryView.ascx"
    TagPrefix="uc" TagName="SummaryView" %>
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" class="Width95Percent">
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
<div style="background-color: White !important;">
    <asp:UpdatePanel ID="upnlBody" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="ConsultingDemandchartDiv" >
                <asp:Chart ID="chrtNewHireReportByRecruiter" runat="server" OnClick="chrtNewHireReport_Click">
                    <Series>
                        <asp:Series Name="chartSeries" ChartArea="MainArea" ChartType="Column" XValueType="String"
                            Color="Gray" YValueType="Int32" PostBackValue="#VALX,#VALY,False" ToolTip="#VALY New Hires"
                            XAxisType="Primary" YAxisType="Primary" XValueMember="name" YValueMembers="count">
                        </asp:Series>
                        <%-- <asp:Series Name="chartSeries1" ChartArea="MainArea" ChartType="FastPoint" XValueType="String"
                BorderWidth="3" YValueType="Int32" MapAreaAttributes="#LABEL" PostBackValue="#VALX"
                ToolTip="#PERCENT" XAxisType="Primary" YAxisType="Primary" XValueMember="name"
                YValueMembers="count1">
            </asp:Series>
            <asp:Series Name="chartSeries2" ChartArea="MainArea" ChartType="FastLine" XValueType="String"
                BorderWidth="2" YValueType="Int32" MapAreaAttributes="#LABEL" PostBackValue="#VALX"
                ToolTip="#PERCENT" XAxisType="Primary" YAxisType="Primary" XValueMember="name"
                YValueMembers="count1">
            </asp:Series>--%>
                    </Series>
                    <%-- <MapAreas>
            <asp:MapArea PostBackValue="#VALX" />
        </MapAreas>--%>
                    <ChartAreas>
                        <asp:ChartArea Name="MainArea">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
                <br />
                <asp:Chart ID="chrtNewHireReportBySeniority" runat="server" OnClick="chrtNewHireReport_Click">
                    <%-- 
        BorderlineColor="Black" BorderlineDashStyle="Solid"
        BackColor="182, 214, 236" BackGradientStyle="TopBottom" BackSecondaryColor="White" Width="960px" Height="500px"
        <ChartAreas>
            <asp:ChartArea Name="MainArea" BackGradientStyle="TopBottom" BackSecondaryColor="#B6D6EC"
                BorderDashStyle="Solid" BorderWidth="2">

            </asp:ChartArea>
        </ChartAreas>--%>
                    <Series>
                        <asp:Series Name="chartSeries" ChartArea="MainArea" ChartType="Column" XValueType="String"
                            Color="Gray" YValueType="Int32" PostBackValue="#VALX,#VALY,True" ToolTip="#VALY New Hires"
                            XAxisType="Primary" YAxisType="Primary" XValueMember="name" YValueMembers="count">
                        </asp:Series>
                        <%-- <asp:Series Name="chartSeries1" ChartArea="MainArea" ChartType="FastPoint" XValueType="String"
                BorderWidth="3" YValueType="Int32" MapAreaAttributes="#LABEL" PostBackValue="#VALX"
                ToolTip="#PERCENT" XAxisType="Primary" YAxisType="Primary" XValueMember="name"
                YValueMembers="count1">
            </asp:Series>
            <asp:Series Name="chartSeries2" ChartArea="MainArea" ChartType="FastLine" XValueType="String"
                BorderWidth="2" YValueType="Int32" MapAreaAttributes="#LABEL" PostBackValue="#VALX"
                ToolTip="#PERCENT" XAxisType="Primary" YAxisType="Primary" XValueMember="name"
                YValueMembers="count1">
            </asp:Series>--%>
                    </Series>
                    <%-- <MapAreas>
            <asp:MapArea PostBackValue="#VALX" />
        </MapAreas>--%>
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
                    <div style="overflow: scroll; height: 300px;">
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

