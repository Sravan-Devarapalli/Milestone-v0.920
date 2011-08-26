<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BenchReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.BenchReport" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<script type="text/javascript">

    function EnableResetButton() {
        var button = document.getElementById("<%= btnResetFilter.ClientID%>");
        var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
        if (button != null) {
            button.disabled = false;
            hiddenField.value = "true";
        }
    }
</script>
<uc:LoadingProgress ID="LoadingProgress1" runat="server" />
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <Triggers>
        <asp:PostBackTrigger ControlID="btnExportToExcel" />
    </Triggers>
    <ContentTemplate>
        <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
        <div class="buttons-block">
            <table class="WholeWidth">
                <tr>
                    <td style="width: 4%;">
                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                            ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Expand Filters" />
                    </td>
                    <td style="width: 3%;">
                        from&nbsp;
                    </td>
                    <td style="width: 8%;">
                        <uc:MonthPicker ID="mpStartDate" OnClientChange="EnableResetButton();" runat="server"
                            AutoPostBack="false" />
                    </td>
                    <td style="width: 3%;">
                        &nbsp;&nbsp;to
                    </td>
                    <td style="width: 8%;">
                        <uc:MonthPicker ID="mpEndDate" runat="server" OnClientChange="EnableResetButton();"
                            AutoPostBack="false" />
                    </td>
                    <td style="width: 38%;">
                        <asp:CustomValidator ID="custPeriod" runat="server" OnServerValidate="custPeriod_ServerValidate"
                            EnableClientScript="false" ErrorMessage="The Period Start must be less than or equal to the Period End."
                            ToolTip="The Period Start must be less than or equal to the Period End." Text="*"
                            ValidationGroup="Filter"></asp:CustomValidator>
                    </td>
                    <td style="width: 36%;" align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnUpdateReport" runat="server" Text="Update" OnClick="btnUpdateReport_Click"
                                        CssClass="pm-button" />
                                </td>
                                <td>
                                    <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" OnClick="btnResetFilter_OnClick"
                                        CssClass="pm-button" />
                                </td>
                                <td>
                                    <asp:Button ID="btnExportToExcel" runat="server" Text="Export" OnClick="btnExportToExcel_Click"
                                        Visible="false" CssClass="pm-button" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="7">
                        <asp:ValidationSummary ID="valSummaryPerformance" runat="server" ValidationGroup="Filter" />
                    </td>
                </tr>
            </table>
            <div class="clear0">
            </div>
        </div>
        <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
            <AjaxControlToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
                CssClass="CustomTabStyle">
                <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                    <HeaderTemplate>
                        <span class="bg DefaultCursor"><span class="NoHyperlink">Filters</span> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <style type="text/css">
                            .wrap
                            {
                                padding-right: 5px;
                            }
                        </style>
                        <table cellpadding="5" cellspacing="2" border="0">
                            <tr align="center">
                                <td style="width: 120px; border-bottom: 1px solid black;" colspan="2" valign="top">
                                    Person Status
                                </td>
                                <td style="width: 30px;">
                                </td>
                                <td style="width: 250px; border-bottom: 1px solid black;">
                                    Practice Area
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;&nbsp;<asp:CheckBox ID="cbActivePersons" runat="server" onclick="EnableResetButton();"
                                        Text="Active" Checked="True" />
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                                <td class="floatRight" style="padding-top: 5px; padding-left: 3px;">
                                    <cc2:ScrollingDropDown ID="cblPractices" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                        onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" BackColor="White"
                                        CellPadding="3" NoItemsType="All" SetDirty="False" Width="350px" DropDownListType="Practice Area"
                                        Height="290px" BorderWidth="0" />
                                    <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                                        Width="250px" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                    </ext:ScrollableDropdownExtender>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;&nbsp;<asp:CheckBox ID="cbProjectedPersons" onclick="EnableResetButton();"
                                        runat="server" Text="Projected" Checked="True" />
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </AjaxControlToolkit:TabContainer>
        </asp:Panel>
        <div style="overflow: auto;">
            <asp:GridView ID="gvBench" runat="server" OnRowDataBound="gvBench_RowDataBound" AutoGenerateColumns="False"
                EmptyDataText="No data found for such request." EnableViewState="true" CssClass="CompPerfTable"
                GridLines="None" BackColor="White" OnDataBound="gvBench_OnDataBound">
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
    </ContentTemplate>
</asp:UpdatePanel>

