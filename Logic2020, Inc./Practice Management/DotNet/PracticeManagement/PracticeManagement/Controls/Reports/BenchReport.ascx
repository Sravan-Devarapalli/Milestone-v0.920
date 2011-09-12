<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BenchReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.BenchReport" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
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
       
        txtStartDate = document.getElementById('<%= diRange.ClientID %>_tbFrom');
        txtEndDate = document.getElementById('<%= diRange.ClientID %>_tbTo');
        var startDate = new Date(txtStartDate.value);
        var endDate = new Date(txtEndDate.value);
        if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= endDate) {

            var btnCustDatesClose = document.getElementById('<%= btnCustDatesClose.ClientID %>');
            hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            var startDate = new Date(txtStartDate.value);
            var endDate = new Date(txtEndDate.value);
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
    .displayNone
    {
        display: none;
    }
</style>
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
                    <td>
                        Show Bench for &nbsp;
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
                    <td style="width: 36%;" align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnUpdateReport" runat="server" Text="Update" OnClick="btnUpdateReport_Click"
                                        CssClass="pm-button" />
                                </td>
                                <td>
                                    <asp:Button ID="btnResetFilter" runat="server" Text="Reset" OnClick="btnResetFilter_OnClick"
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
                    <td colspan="3">
                        <asp:ValidationSummary ID="valSummaryPerformance" runat="server" ValidationGroup="Filter" />
                    </td>
                </tr>
            </table>
            <div class="clear0">
            </div>
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

