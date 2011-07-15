<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Calendarview.aspx.cs" Inherits="PraticeManagement.Calendarview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Calendar view</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Calendar view
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="updProjectDetails" runat="server">
        <ContentTemplate>
            <div style="text-align: center;">
                <table cellpadding="0" cellspacing="0" width="100%" runat="server" id="tblDetails">
                    <tr>
                        <td align="center" style="background-color: White;">
                            <asp:Chart ID="chartProjectDetails" Width="800px" runat="server" Visible="false">
                                <Series>
                                    <asp:Series Name="Projects" ChartType="RangeBar" XValueMember="Description" YValueMembers="StartDate,EndDate">
                                    </asp:Series>
                                </Series>
                                <Titles>
                                    <asp:Title Name="ProjectsTitle" />
                                </Titles>
                                <ChartAreas>
                                    <asp:ChartArea Name="ProjectsArea">
                                        <AxisY IsLabelAutoFit="true" IsStartedFromZero="true" Enabled="True" LineDashStyle="NotSet">
                                            <MajorGrid LineColor="DimGray" LineDashStyle="Dash" />
                                            <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                                            <LabelStyle Format="MMM, d, yyyy" />
                                        </AxisY>
                                        <AxisY2 IsLabelAutoFit="true" IsStartedFromZero="true" Enabled="True" LineDashStyle="NotSet">
                                            <MajorGrid LineColor="DimGray" LineDashStyle="Dash" />
                                            <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                                            <LabelStyle Format="MMM, d, yyyy" />
                                        </AxisY2>
                                        <AxisX IsLabelAutoFit="false">
                                            <MajorGrid Interval="1" LineDashStyle="Dot" LineColor="Silver" />
                                            <MajorTickMark Enabled="False" />
                                            <LabelStyle Enabled="false" />
                                        </AxisX>
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

