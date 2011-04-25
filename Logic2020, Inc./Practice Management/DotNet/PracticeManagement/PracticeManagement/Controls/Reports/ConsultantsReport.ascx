<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsultantsReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ConsultantsReport" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<%@ Register Src="~/Controls/Reports/ConsultantsReportFilter.ascx" TagPrefix="uc"
    TagName="ReportFilter" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:UpdatePanel ID="updConsultantsReport" runat="server">
    <ContentTemplate>
        <asp:ObjectDataSource ID="odsReport" runat="server" SelectMethod="GetConsultantsTableReport"
            TypeName="PraticeManagement.Controls.Reports.ReportsHelper">
            <SelectParameters>
                <asp:ControlParameter ControlID="tcFilters$tpFilters$ReportFilter" Name="start" PropertyName="MonthBegin"
                    Type="DateTime" />
                <asp:ControlParameter ControlID="tcFilters$tpFilters$ReportFilter" Name="end" PropertyName="MonthEnd"
                    Type="DateTime" />
                <asp:ControlParameter ControlID="tcFilters$tpFilters$ReportFilter" Name="activePersons"
                    PropertyName="ActivePersons" Type="Boolean" />
                <asp:ControlParameter ControlID="tcFilters$tpFilters$ReportFilter" Name="projectedPersons"
                    PropertyName="ProjectedPersons" Type="Boolean" />
                <asp:ControlParameter ControlID="tcFilters$tpFilters$ReportFilter" ConvertEmptyStringToNull="False"
                    Name="activeProjects" PropertyName="ActiveProjects" Type="Boolean" />
                <asp:ControlParameter ControlID="tcFilters$tpFilters$ReportFilter" Name="projectedProjects"
                    PropertyName="ProjectedProjects" Type="Boolean" />
                <asp:ControlParameter ControlID="tcFilters$tpFilters$ReportFilter" Name="internalProjects"
                    PropertyName="InternalProjects" Type="Boolean" />
                <asp:ControlParameter ControlID="tcFilters$tpFilters$ReportFilter" Name="experimentalProjects"
                    PropertyName="ExperimentalProjects" Type="Boolean" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <div class="filters" style="margin-bottom: 10px;">
            <ajaxToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0" CssClass="CustomTabStyle">
                <ajaxToolkit:TabPanel runat="server" ID="tpFilters">
                    <HeaderTemplate>
                        <span class="bg DefaultCursor"><span class="NoHyperlink">Filters</span></span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <uc:ReportFilter ID="ReportFilter" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
            <div class="buttons-block" style="margin-bottom: 10px">
                <asp:Button ID="btnUpdateView" runat="server" Text="Update View" Width="100px" OnClick="btnUpdateView_OnClick"
                    EnableViewState="False" />
                <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" Width="100px"
                    OnClick="btnResetFilter_OnClick" />
                <div class="clear0">
                </div>
            </div>
        </div>
        <%--<AjaxControlToolkit:TabContainer ID="tabConsReport" runat="server">
                    <AjaxControlToolkit:TabPanel runat="server" ID="tabConsReportTable" HeaderText="Table">
                        <ContentTemplate>--%>
        <asp:GridView ID="gvConsultantsReport" runat="server" AllowSorting="True" DataSourceID="odsReport"
            ShowFooter="True" OnRowDataBound="gvConsultantsReport_RowDataBound" EmptyDataText="No persons match filter parameters (month, status)"
            AutoGenerateColumns="False" OnDataBound="gvConsultantsReport_DataBound" CssClass="CompPerfTable WholeWidth"
            GridLines="None">
            <AlternatingRowStyle BackColor="#F9FAFF" />
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortConsultant" CommandArgument="Consultant #" CommandName="Sort"
                                runat="server">Consultant #</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="lblConsultantNumber" runat="server" Text='<%# Eval("Consultant #") %>'
                            CommandArgument='<%# Eval("PersonId") %>' OnCommand="btnConsultantName_Command"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortName" CommandArgument="Name" CommandName="Sort" runat="server">Name</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="btnConsultantName" runat="server" Text='<%# Eval("Name") %>'
                            CommandArgument='<%# Eval("PersonId") %>' OnCommand="btnConsultantName_Command"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Seniority">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortSeniority" CommandArgument="SeniorityName" CommandName="Sort"
                                runat="server">Seniority</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblSeniority" runat="server" Text='<%# Bind("SeniorityName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortPay" CommandArgument="Pay Type" CommandName="Sort" runat="server">Pay Type</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblPayType" runat="server" Text='<%# Eval("Pay Type") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortWorking" CommandArgument="Working days" CommandName="Sort"
                                runat="server">Working Days</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblWorkingdays" runat="server" Text='<%# Eval("Working days") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortHoliday" CommandArgument="Holiday Hours" CommandName="Sort"
                                runat="server">Holiday Hours</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblHolidayHours" runat="server" Text='<%# Eval("Holiday Hours") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortAvailable" CommandArgument="Available Hours" CommandName="Sort"
                                runat="server">Available Hours</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblAvailableHours" runat="server" Text='<%# Eval("Available Hours") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortProjected" CommandArgument="Projected Hours" CommandName="Sort"
                                runat="server">Projected Hours</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblrojectedHours" runat="server" Text='<%# Eval("Projected Hours") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortBench" CommandArgument="Bench Hours" CommandName="Sort"
                                runat="server">Bench Hours</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblBenchHoursr" runat="server" Text='<%# Eval("Bench Hours") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortUtilization" CommandArgument="Utilization %" CommandName="Sort"
                                runat="server">Utilization %</asp:LinkButton></div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblUtilization" runat="server" Text='<%# Eval("Utilization %") %>'></asp:Label>%
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="LightGray" Font-Bold="True" Height="25px" />
            <RowStyle Height="25px" />
        </asp:GridView>
        <%--</ContentTemplate>
                    </AjaxControlToolkit:TabPanel>
                    <AjaxControlToolkit:TabPanel runat="server" ID="tabConsReportChart" HeaderText="Chart">
                        <ContentTemplate>
                            <asp:Chart ID="UtilizationChart" runat="server" Width="1200px" Height="600px" Visible="false">
                                <Series>
                                    <asp:Series Legend="Utilization Legend" Name="Available Hours" XValueMember="Name" 
                                        YValueMembers="Available Hours" ChartArea="UtilizationArea">
                                    </asp:Series>
                                    <asp:Series Legend="Utilization Legend" Name="Holiday Hours" XValueMember="Name" 
                                        YValueMembers="Holiday Hours" ChartArea="UtilizationArea">
                                    </asp:Series>
                                    <asp:Series Legend="Utilization Legend" Name="Projected Hours" XValueMember="Name" 
                                        YValueMembers="Projected Hours" ChartArea="UtilizationArea">
                                    </asp:Series>
                                    <asp:Series Legend="Utilization Legend" Name="Bench Hours" XValueMember="Name" 
                                        YValueMembers="Bench Hours" ChartArea="UtilizationArea">
                                    </asp:Series>
                                </Series>
                                <Legends>
                                    <asp:Legend Name="Utilization Legend" 
                                            Alignment="Center" Docking="Bottom">
                                    </asp:Legend>
                                </Legends>
                                <ChartAreas>
                                    <asp:ChartArea Name="UtilizationArea" AlignmentOrientation="Horizontal">
                                        <AxisX 
                                            IsLabelAutoFit="False" 
                                            Interval="1"
                                            LabelAutoFitStyle="LabelsAngleStep90">
                                            <MajorGrid Enabled="False" />
                                            <LabelStyle Angle="30" />
                                        </AxisX>
                                        <AxisY Interval="40">
                                            <MajorGrid LineColor="Gray" LineDashStyle="Dash" Interval="40" />
                                            <ScaleBreakStyle Enabled="True" />
                                        </AxisY>
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </ContentTemplate>
                    </AjaxControlToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>--%>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="tcFilters$tpFilters$ReportFilter" />
    </Triggers>
</asp:UpdatePanel>
<table cellpadding="3" style="margin-top: 10px;">
    <tr>
        <td rowspan="4">
            <asp:Button ID="btnExport" runat="server" OnClick="btnExport_Click" Text="Export"
                Style="text-align: center" CssClass="pm-button" />
        </td>
        <td style="white-space: nowrap">
            <b>Working days</b>
        </td>
        <td class="style2">
            W
        </td>
        <td>
            =
        </td>
        <td style="white-space: nowrap">
            Month working dates - Personal holidays
        </td>
        <td>
            &nbsp;&nbsp;&nbsp;
        </td>
        <td style="white-space: nowrap">
            <b>Bench hours</b>
        </td>
        <td class="style4">
            BH
        </td>
        <td>
            =
        </td>
        <td style="white-space: nowrap">
            A - H - P
        </td>
        <td>
            &nbsp;
        </td>
        <td rowspan="4">
            <asp:Table runat="server">
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="cellW2SalaryColor" runat="server">W2-Salary</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow2" runat="server">
                    <asp:TableCell ID="cellW2HourlyColor" runat="server">W2-Hourly</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow3" runat="server">
                    <asp:TableCell ID="cell1009HourlyColor" runat="server">1099/Hourly</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow4" runat="server">
                    <asp:TableCell ID="cell1009PORColor" runat="server">1099/POR</asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </td>
    </tr>
    <tr>
        <td style="white-space: nowrap">
            <b>Holiday hours</b>
        </td>
        <td>
            H
        </td>
        <td>
            =
        </td>
        <td style="white-space: nowrap">
            [Company holidays + Personal holidays] x DWH
        </td>
        <td>
            &nbsp;
        </td>
        <td style="white-space: nowrap">
            <b>Utilization %</b>
        </td>
        <td class="style4">
            UP
        </td>
        <td>
            =
        </td>
        <td style="white-space: nowrap">
            P / A
        </td>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td style="white-space: nowrap">
            <b>Available hours</b>
        </td>
        <td class="style2">
            A
        </td>
        <td>
            =
        </td>
        <td style="white-space: nowrap">
            [Working days] x DWH
        </td>
        <td>
            &nbsp;
        </td>
        <td style="white-space: nowrap">
            <b>Default Working Hours </b>
        </td>
        <td class="style4">
            &nbsp;DWH
        </td>
        <td>
            =
        </td>
        <td>
            8
        </td>
    </tr>
    <tr>
        <td style="white-space: nowrap">
            <b>Projected Hours</b>
        </td>
        <td style="white-space: nowrap">
            P
        </td>
        <td>
            =
        </td>
        <td style="white-space: nowrap">
            Total hours for all milestones
        </td>
        <td>
            &nbsp;
        </td>
        <td class="style3">
            <b></b>
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
    </tr>
</table>

