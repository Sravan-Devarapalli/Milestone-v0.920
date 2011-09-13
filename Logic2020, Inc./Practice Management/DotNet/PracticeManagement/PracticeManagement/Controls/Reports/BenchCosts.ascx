﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BenchCosts.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.BenchCosts" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc" TagName="DateInterval" %>
<script type="text/javascript">
    function EnableResetButton() {
        var button = document.getElementById("<%= btnResetFilter.ClientID%>");
        var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
        if (button != null) {
            button.disabled = false;
            hiddenField.value = "true";
        }
    }

    function CheckIfDatesValid() {
        txtStartDate = document.getElementById('<%= (diRange.FindControl("tbFrom") as TextBox).ClientID %>');
        txtEndDate = document.getElementById('<%= (diRange.FindControl("tbTo") as TextBox).ClientID %>');
        var startDate = new Date(txtStartDate.value);
        var endDate = new Date(txtEndDate.value);
        if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= endDate) {

            var btnCustDatesClose = document.getElementById('<%= btnCustDatesClose.ClientID %>');
            hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            var startDateStr = startDate.format("MM/dd/yyyy");
            var endDateStr = endDate.format("MM/dd/yyyy");
            hdnStartDate.value = startDateStr;
            hdnEndDate.value = endDateStr;
            lblCustomDateRange.innerHTML = '(' + startDateStr + '&nbsp;-&nbsp;' + endDateStr + ')';
            btnCustDatesClose.click();

        }
        return false;
    }


    function CheckAndShowCustomDatesPoup(ddlPeriod) {
        imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
        lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
        if (ddlPeriod.value == '0') {
            imgCalender.attributes["class"].value = "";
            lblCustomDateRange.attributes["class"].value = "";
            if (imgCalender.fireEvent) {
                imgCalender.style.display = "";
                lblCustomDateRange.style.display = "";
                imgCalender.click();
            }
            if (document.createEvent) {
                var event = document.createEvent('HTMLEvents');
                event.initEvent('click', true, true);
                imgCalender.dispatchEvent(event);
            }
        }
        else {
            imgCalender.attributes["class"].value = "displayNone";
            lblCustomDateRange.attributes["class"].value = "displayNone";
            if (imgCalender.fireEvent) {
                imgCalender.style.display = "none";
                lblCustomDateRange.style.display = "none";
            }
        }
    }
    function ReAssignStartDateEndDates() {
        hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
        hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
        hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
        hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');

        var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
        var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
        if (startDateCalExtender != null) {
            startDateCalExtender.set_selectedDate(hdnStartDate.value);
        }
        if (endDateCalExtender != null) {
            endDateCalExtender.set_selectedDate(hdnEndDate.value);
        }
        CheckIfDatesValid();
    }

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

    function endRequestHandle(sender, Args) {
        imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
        lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
        ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
        if (imgCalender.fireEvent && ddlPeriod.value != '0') {
            imgCalender.style.display = "none";
            lblCustomDateRange.style.display = "none";
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
    .displayNone
    {
        display: none;
    }
    .MinWidth
    {
        min-width: 150px;
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
                <td align="left" style="white-space:nowrap; width:118px;">                 
                        Show Bench Costs for &nbsp;
                        <asp:DropDownList ID="ddlPeriod" runat="server" onchange="EnableResetButton(); CheckAndShowCustomDatesPoup(this);">
                            <asp:ListItem Text="Current Month" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Previous Month" Value="-1"></asp:ListItem>
                            <asp:ListItem Text="Current FY" Value="13"></asp:ListItem>
                            <asp:ListItem Text="Previous FY" Value="-13"></asp:ListItem>
                            <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;
                        <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                            CancelControlID="btnCustDatesCancel" OkControlID="btnCustDatesClose" BackgroundCssClass="modalBackground"
                            PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates" DropShadow="false"
                            OnCancelScript="ReAssignStartDateEndDates();" OnOkScript="return false;" />
                        <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                        <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                        <asp:HiddenField ID="hdnStartDateCalExtenderBehaviourId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnEndDateCalExtenderBehaviourId" runat="server" Value="" />
                        &nbsp;
                        <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
                        <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                        &nbsp;
                </td>
                <td style="width: 10px !important;" align="left">
                    <asp:CheckBox ID="chbIncludeOverHeads" runat="server" Style="width: 10px !important;"
                        Checked="true" onclick="EnableResetButton();" TextAlign="Left" />
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
                <td align="left" style="width: 200px;">
                    <asp:Label ID="lblOverheads" Style="white-space: nowrap;" Text="Include Overheads in Calculations"
                        runat="server" />
                </td>
                <td align="right">
                    <table style="width: 250px;">
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
    <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
            CssClass="ConfirmBoxClass" Style="padding-top: 20px; display: none;" BorderWidth="2px">
            <table class="WholeWidth">
                <tr>
                    <td align="center">
                        <table>
                            <tr>
                                <td>
                                    <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                        FromToDateFieldWidth="70" />
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" style="padding: 10px 0px 10px 0px;">
                        <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="return CheckIfDatesValid();"
                            ValidationGroup="<%# ClientID %>" Text="OK" Style="float: none !Important;" CausesValidation="true" />
                        <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true"
                            OnClientClick="return false;" />
                        &nbsp; &nbsp;
                        <asp:Button ID="btnCustDatesCancel" runat="server" Text="Cancel" Style="float: none !Important;" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:ValidationSummary ID="valSum" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
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
<div style="overflow: auto">
    <div style="padding-bottom: 10px;">
        <asp:Label ID="lblExternalPractices" runat="server" Style="font-weight: bold;" Text="Persons with External Practices"></asp:Label>
    </div>
    <asp:GridView ID="gvBenchCosts" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here."
        OnRowDataBound="gvBenchRollOffDates_RowDataBound" CssClass="CompPerfTable" EnableViewState="true"
        ShowFooter="true" OnDataBound="gvBench_OnDataBound" GridLines="None" BackColor="White">
        <AlternatingRowStyle BackColor="#F9FAFF" />
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="ie-bg">
                        <asp:LinkButton ID="btnSortConsultant" runat="server" OnClick="btnSortConsultant_Click">Consultant Name</asp:LinkButton>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HyperLink ID="btnPersonName" Width="220px" CssClass="wrap" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
                        NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Client.Id")) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="220px" ItemStyle-CssClass="MinWidth">
                <HeaderTemplate>
                    <div class="ie-bg">
                        <asp:LinkButton ID="btnSortPractice" runat="server" OnClick="btnSortPractice_Click">Practice Area</asp:LinkButton>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPracticeName" runat="server" CssClass="wrap" Text='<%# Eval("Practice.Name") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <div class="ie-bg CompPerfDataTitle">
                        <asp:LinkButton ID="btnSortStatus" runat="server" OnClick="btnSortStatus_Click">Status</asp:LinkButton>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPracticeName" Width="70px" runat="server" Text='<%# Eval("ProjectNumber") %>'></asp:Label>
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
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortConsultant" runat="server" OnClick="btnSortInternalConsultant_Click">Consultant Name</asp:LinkButton>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:HyperLink ID="btnPersonName" Width="220px" CssClass="wrap" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
                            NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Client.Id")) %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="220px" ItemStyle-CssClass="MinWidth">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortPractice" runat="server" OnClick="btnSortInternalPractice_Click">Practice Area</asp:LinkButton>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblPracticeName" runat="server" CssClass="wrap" Text='<%# Eval("Practice.Name") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <div class="ie-bg CompPerfDataTitle">
                            <asp:LinkButton ID="btnSortStatus" runat="server" OnClick="btnSortInternalStatus_Click">Status</asp:LinkButton>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblPracticeName" Width="70px" runat="server" Text='<%# Eval("ProjectNumber") %>'></asp:Label>
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
<br />
<div class="buttons-block" style="padding-left: 15px">
    <table>
        <tr>
            <td style="padding-bottom: 5px;">
                <b>Legend</b>
            </td>
        </tr>
        <tr>
            <td style="padding-bottom: 5px;">
                <sup style='font-size: 11px;'>1</sup> - Person was hired during this month.
            </td>
        </tr>
        <tr>
            <td style="padding-bottom: 5px;">
                <sup style='font-size: 11px;'>2</sup> - Person was terminated during this month.
            </td>
        </tr>
        <tr>
            <td style="padding-bottom: 5px;">
                <sup style='font-size: 11px;'>3</sup> - Person was changed from salaried to hourly
                compensation during this month.
            </td>
        </tr>
        <tr>
            <td style="padding-bottom: 5px;">
                <sup style='font-size: 11px;'>4</sup> - Person was changed from hourly to salaried
                compensation during this month.
            </td>
        </tr>
    </table>
</div>

