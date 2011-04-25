<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BenchReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.BenchReport" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<uc:LoadingProgress ID="LoadingProgress1" runat="server" />
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <triggers>
        <asp:PostBackTrigger ControlID="btnExportToExcel" />
    </triggers>
    <contenttemplate>
        <div class="filters">
            <AjaxControlToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
                CssClass="CustomTabStyle">
                <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                    <HeaderTemplate>
                        <span class="bg DefaultCursor"><span class="NoHyperlink">Filters</span> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <%--<uc:ReportFilter ID="rpReportFilter" runat="server" />--%>
                        <style type="text/css">
                            .wrap
                            {
                                padding-right: 5px;
                            }
                        </style>
                        <table cellpadding="5" cellspacing="2" border="0">
                            <tr>
                                <td style="padding-bottom: 10px;">
                                    Period from&nbsp;
                                </td>
                                <td>
                                    <uc:MonthPicker ID="mpStartDate" runat="server" AutoPostBack="false" />
                                </td><td></td>
                                <td rowspan="2" valign="top" style="padding-top: 2px; padding-left: 5px;" class="floatRight">
                                    <cc2:ScrollingDropDown ID="cblPractices" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems" onclick="scrollingDropdown_onclick('cblPractices','Practice Area')"
                                        BackColor="White" CellPadding="3" NoItemsType="All" SetDirty="False" Width="350px" DropDownListType="Practice Area"  Height="290px"
                                        BorderWidth="0" />
                                    <ext:scrollabledropdownextender id="sdePractices" runat="server" targetcontrolid="cblPractices" Width="250px" UseAdvanceFeature="true"
                                        editimageurl="~/Images/Dropdown_Arrow.png">
                            </ext:scrollabledropdownextender>
                                </td>
                                <td>
                                    &nbsp;&nbsp;<asp:CheckBox ID="cbActivePersons" runat="server" Text="Active Persons"
                                        Checked="True" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    &nbsp;to&nbsp;
                                </td>
                                <td>
                                    <uc:MonthPicker ID="mpEndDate" runat="server" AutoPostBack="false" />
                                </td>
                                <td rowspan="2">
                                    <asp:CustomValidator ID="custPeriod" runat="server"
                                        OnServerValidate="custPeriod_ServerValidate" EnableClientScript="false"
                                        ErrorMessage="The Period Start must be less than or equal to the Period End."
                                        ToolTip="The Period Start must be less than or equal to the Period End."
                                        Text="*" ValidationGroup="Filter"></asp:CustomValidator>
                                </td>
                                <td>
                                    &nbsp;&nbsp;<asp:CheckBox ID="cbProjectedPersons" runat="server" Text="Projected Persons"
                                        Checked="True" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </AjaxControlToolkit:TabContainer>
        </div>
        <div class="buttons-block" style="margin-bottom: 10px">
            <asp:ValidationSummary ID="valSummaryPerformance" runat="server" ValidationGroup="Filter"/>
            <asp:Button ID="btnUpdateReport" runat="server" Text="Update" OnClick="btnUpdateReport_Click"
                CssClass="pm-button" />
            <asp:Button ID="btnExportToExcel" runat="server" Text="Export" OnClick="btnExportToExcel_Click"
                Visible="false" CssClass="pm-button" />
            <div class="clear0">
            </div>
        </div>
        <div style="overflow:auto;">
            <asp:GridView ID="gvBench" runat="server" OnRowDataBound="gvBench_RowDataBound" AutoGenerateColumns="False"
                EmptyDataText="No data found for such request." EnableViewState="true" CssClass="CompPerfTable"
                GridLines="None" BackColor="White">
                <AlternatingRowStyle BackColor="#F9FAFF" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                <asp:LinkButton ID="btnSortConsultant" runat="server" OnClick="btnSortConsultant_Click">Consultant Name</asp:LinkButton>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HyperLink ID="btnPersonName" CssClass="wrap" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
                                NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Client.Id")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                <asp:LinkButton ID="btnSortPractice" runat="server" OnClick="btnSortPractice_Click">Practice Area</asp:LinkButton>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblPracticeName" runat="server" CssClass="wrap" Text='<%# Eval("Practice.Name") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg CompPerfDataTitle">
                                <asp:LinkButton ID="btnSortStatus" runat="server" OnClick="btnSortStatus_Click">Status</asp:LinkButton>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblPracticeName" runat="server" Text='<%# Eval("ProjectNumber") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                    <asp:TemplateField Visible="false" />
                </Columns>
            </asp:GridView>
        </div>
    </contenttemplate>
</asp:UpdatePanel>

