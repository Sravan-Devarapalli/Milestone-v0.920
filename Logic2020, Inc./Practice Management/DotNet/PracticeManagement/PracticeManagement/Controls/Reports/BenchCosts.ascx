<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BenchCosts.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.BenchCosts" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
<style type="text/css">
    .floatLeft input
    {
        float: right;
        width: 110px;
        text-align: center;
        margin-right: 10px;
    }
    .wrap
    {
        padding-right: 5px;
    }
</style>
<div class="filters">
    <div class="filter-section-color">
        <table class="WholeWidth">
            <tr>
                <td align="left" style="width: 25px">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                        ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                        ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                    <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                    <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                        ToolTip="Expand Filters and Sort Options" />
                </td>
                <td align="left" style="width: 118px;">
                    <asp:Label ID="lblUtilizationFrom" runat="server" Text="Show Bench Costs"></asp:Label>
                </td>
                <td style="width: 30px; padding-right: 4px" align="left">
                    <asp:Label ID="Label1" runat="server" Text="From "></asp:Label>
                </td>
                <td style="width: 110px;" align="left">
                    <uc:MonthPicker ID="mpStartDate" runat="server" AutoPostBack="false" OnClientChange="EnableResetButton();" />
                </td>
                <td style="width: 15px;" align="left">
                    <asp:Label ID="lblT0" runat="server" Text="to "></asp:Label>
                </td>
                <td style="width: 115px;" align="left">
                    <uc:MonthPicker ID="mpEndDate" runat="server" AutoPostBack="false" OnClientChange="EnableResetButton();" />
                </td>
                <td style="width: 10px !important;" align="left">
                    <asp:CheckBox ID="chbIncludeOverHeads" runat="server" Style="width: 10px !important;"
                        Checked="true" onclick="EnableResetButton();" TextAlign="Left" />
                </td>
                <td align="left">
                    <asp:Label ID="lblOverheads" Style="white-space: nowrap;" Text="Include Overheads in Calculations"
                        runat="server" />
                </td>
                <td style="width: 5px;" align="left">
                    <asp:CustomValidator ID="custPeriod" runat="server" ErrorMessage="The Period Start must be less than or equal to the Period End"
                        ToolTip="The Period Start must be less than or equal to the Period End" Text="*"
                        EnableClientScript="false" OnServerValidate="custPeriod_ServerValidate" ValidationGroup="Filter"
                        Display="Dynamic"></asp:CustomValidator>
                    <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="false"
                        ErrorMessage="The period length must be not more than {0} months." Text="*" EnableClientScript="false"
                        Display="Dynamic" OnServerValidate="custPeriodLengthLimit_ServerValidate" ValidationGroup="Filter"></asp:CustomValidator>
                </td>
                <td align="right">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnUpdateView" runat="server" Text="Update View" Width="100px" OnClick="btnUpdateView_Click"
                                    ValidationGroup="Filter" EnableViewState="False" />
                            </td>
                            <td>
                                <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" Width="100px"
                                    CausesValidation="false" OnClick="btnResetFilter_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="9" align="left" style="padding-top: 5px;">
                    <asp:ValidationSummary ID="ValSumFilter" runat="server" ValidationGroup="Filter" />
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="pnlFilters" runat="server">
        <AjaxControlToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
            CssClass="CustomTabStyle">
            <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                <HeaderTemplate>
                    <span class="bg DefaultCursor"><span class="NoHyperlink">Filters</span></span>
                </HeaderTemplate>
                <ContentTemplate>
                    <%--<uc:ReportFilter ID="rpReportFilter" runat="server" />--%>
                    <style type="text/css">
                        .floatLeft input
                        {
                            float: right;
                            width: 110px;
                            text-align: center;
                            margin-right: 10px;
                        }
                        .wrap
                        {
                            padding-right: 5px;
                        }
                    </style>
                    <table cellpadding="5" cellspacing="2" border="0">
                        <tr align="center">
                            <td style="width: 125px; border-bottom: 1px solid black;" valign="top">
                                Person Status
                            </td>
                            <td style="width: 30px;">
                            </td>
                            <td style="width: 150px; border-bottom: 1px solid black;" valign="top">
                                Pay Type
                            </td>
                            <td style="width: 30px;">
                            </td>
                            <td style="width: 250px; border-bottom: 1px solid black;" colspan="2" valign="top">
                                Project Type
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
                                <asp:CheckBox ID="cbActivePersons" runat="server" Text="Active Persons" Checked="True"
                                    onclick="EnableResetButton();" />
                            </td>
                            <td>
                            </td>
                            <td class="floatRight" style="padding-top: 5px; padding-left: 3px;">
                                <cc2:ScrollingDropDown ID="cblPayType" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblPayType','Pay Type')" BackColor="White"
                                    CellPadding="3" NoItemsType="Nothing" SetDirty="False" Width="200px" DropDownListType="Pay Type"
                                    Height="100px" BorderWidth="0" />
                                <ext:ScrollableDropdownExtender ID="sdePayType" runat="server" TargetControlID="cblPayType"
                                    Width="200px" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                </ext:ScrollableDropdownExtender>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="chbActiveProjects" runat="server" AutoPostBack="false" Checked="True"
                                    onclick="EnableResetButton();" Text="Active" ToolTip="Include active projects into report" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chbCompletedProjects" runat="server" AutoPostBack="false" Checked="True"
                                    onclick="EnableResetButton();" Text="Completed" ToolTip="Include Completed projects into report" />
                            </td>
                            <td>
                            </td>
                            <td class="floatRight" style="padding-top: 5px; padding-left: 3px;">
                                <cc2:ScrollingDropDown ID="cblPractices" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" BackColor="White"
                                    CellPadding="3" NoItemsType="Nothing" SetDirty="False" Width="290px" DropDownListType="Practice Area"
                                    Height="220px" BorderWidth="0" />
                                <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                                    Width="250px" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                </ext:ScrollableDropdownExtender>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cbProjectedPersons" runat="server" Text="Projected Persons" Checked="true"
                                    onclick="EnableResetButton();" />
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="chbProjectedProjects" runat="server" AutoPostBack="false" Checked="True"
                                    Text="Projected" ToolTip="Include projected projects into report" onclick="EnableResetButton();" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chbExperimentalProjects" runat="server" AutoPostBack="false" Text="Experimental"
                                    ToolTip="Include experimental projects into report" onclick="EnableResetButton();" />
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tpSortOptions" runat="server">
                <HeaderTemplate>
                    <span class="bg DefaultCursor"><span class="NoHyperlink">Sort Options</span></span>
                </HeaderTemplate>
                <ContentTemplate>
                    <div style="padding: 10px 0px 10px 0px;">
                        <asp:CheckBox ID="chbSeperateInternalExternal" runat="server" Text="Separate Internal and External Practices into Separate Tables"
                            Checked="true" onclick="EnableResetButton();" />
                        <br />
                        <asp:CheckBox ID="chbIncludeZeroCostEmps" Text="Include Employees with Zero Sum Costs"
                            runat="server" Checked="false" onclick="EnableResetButton();" />
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </AjaxControlToolkit:TabContainer>
    </asp:Panel>
</div>
<br />
<div style="overflow: auto">
    <div style="padding-bottom: 10px;">
        <asp:Label ID="lblExternalPractices" runat="server" Style="font-weight: bold;" Text="Persons with External Practices"></asp:Label>
    </div>
    <asp:GridView ID="gvBenchCosts" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here."
        OnRowDataBound="gvBenchRollOffDates_RowDataBound" CssClass="CompPerfTable" EnableViewState="true"
        ShowFooter="true" OnDataBound="gvBench_OnDataBound" GridLines="None" BackColor="White">
        <AlternatingRowStyle BackColor="#F9FAFF" />
        <Columns>
            <asp:TemplateField ItemStyle-Width="220px">
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
            <asp:TemplateField ItemStyle-Width="220px">
                <HeaderTemplate>
                    <div class="ie-bg">
                        <asp:LinkButton ID="btnSortPractice" runat="server" OnClick="btnSortPractice_Click">Practice Area</asp:LinkButton>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPracticeName" runat="server" CssClass="wrap" Text='<%# Eval("Practice.Name") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="80px">
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
        <FooterStyle BackColor="LightGray" Font-Bold="True" Height="25px" />
    </asp:GridView>
    <div id="divBenchCostsInternal" runat="server">
        <hr id="hrDirectorAndPracticeSeperator" runat="server" size="2" color="#888888" align="center" />
        <div style="padding-bottom: 10px;">
            <asp:Label ID="lblInternalPractices" runat="server" Style="font-weight: bold;" Text="Persons with Internal Practices"></asp:Label>
        </div>
        <asp:GridView ID="gvBenchCostsInternal" runat="server" AutoGenerateColumns="False"
            EmptyDataText="There is nothing to be displayed here." OnRowDataBound="gvBenchRollOffDates_RowDataBound"
            CssClass="CompPerfTable" EnableViewState="true" ShowFooter="true" OnDataBound="gvBench_OnDataBound"
            GridLines="None" BackColor="White">
            <AlternatingRowStyle BackColor="#F9FAFF" />
            <Columns>
                <asp:TemplateField ItemStyle-Width="220px">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortConsultant" runat="server" OnClick="btnSortInternalConsultant_Click">Consultant Name</asp:LinkButton>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:HyperLink ID="btnPersonName" CssClass="wrap" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
                            NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Client.Id")) %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="220px">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortPractice" runat="server" OnClick="btnSortInternalPractice_Click">Practice Area</asp:LinkButton>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblPracticeName" runat="server" CssClass="wrap" Text='<%# Eval("Practice.Name") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="80px">
                    <HeaderTemplate>
                        <div class="ie-bg CompPerfDataTitle">
                            <asp:LinkButton ID="btnSortStatus" runat="server" OnClick="btnSortInternalStatus_Click">Status</asp:LinkButton>
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
            <FooterStyle BackColor="LightGray" Font-Bold="True" Height="25px" />
        </asp:GridView>
    </div>
    <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
</div>

