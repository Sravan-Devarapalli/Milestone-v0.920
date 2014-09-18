<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BenchCosts.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.BenchCosts" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
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
            lblCustomDateRange.attributes["class"].value = "fontBold";
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
<div class="filters">
    <div class="filter-section-color">
        <table class="WholeWidth">
            <tr>
                <td class="Width25Px textLeft">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                        ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                        ExpandControlID="btnExpandCollapseFilter" Collapsed="true" TextLabelID="lblFilter" />
                    <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                    <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                        ToolTip="Expand Filters and Sort Options" />
                </td>
                <td class="ShowBenchCosts">
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
                    <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                    &nbsp;
                </td>
                <td class="Width10PxImp textLeft">
                    <asp:CheckBox ID="chbIncludeOverHeads" runat="server" class="Width10PxImp" Checked="true"
                        onclick="EnableResetButton();" TextAlign="Left" />
                </td>
                <td class="Width5Px textLeft">
                    <asp:CustomValidator ID="custPeriod" runat="server" ErrorMessage="The Period Start must be less than or equal to the Period End"
                        ToolTip="The Period Start must be less than or equal to the Period End" Text="*"
                        EnableClientScript="false" OnServerValidate="custPeriod_ServerValidate" ValidationGroup="Filter"
                        Display="Dynamic"></asp:CustomValidator>
                    <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="false"
                        ErrorMessage="The period length must be not more than {0} months." Text="*" EnableClientScript="false"
                        Display="Dynamic" OnServerValidate="custPeriodLengthLimit_ServerValidate" ValidationGroup="Filter"></asp:CustomValidator>
                </td>
                <td class="Width200Px textLeft">
                    <asp:Label ID="lblOverheads" CssClass="lblOverheadsBenchCost" Text="Include Overheads in Calculations"
                        runat="server" />
                </td>
                <td align="right">
                    <table class="Width250Px">
                        <tr>
                            <td>
                                <asp:Button ID="btnUpdateView" runat="server" Text="Update View" CssClass="Width100Px"
                                    OnClick="btnUpdateView_Click" ValidationGroup="Filter" EnableViewState="False" />
                            </td>
                            <td>
                                <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" CssClass="Width100Px"
                                    CausesValidation="false" OnClick="btnResetFilter_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td class="PaddingTop5">
                                <asp:Button ID="bntExportExcel" runat="server" Text="Export" CssClass="Width100Px"
                                    Visible="false" OnClick="btnExportToExcel_OnClick" ValidationGroup="Filter" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="9" class="PaddingTop5 textLeft">
                    <asp:ValidationSummary ID="ValSumFilter" runat="server" ValidationGroup="Filter" />
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="pnlCustomDates" runat="server" CssClass="ConfirmBoxClass CustomDatesPopUp"
        Style="display: none;">
        <table class="WholeWidth">
            <tr>
                <td align="Center">
                    <table>
                        <tr>
                            <td>
                                <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                    FromToDateFieldCssClass="Width70Px" />
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="custBtns">
                    <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="return CheckIfDatesValid();"
                        ValidationGroup="<%# ClientID %>" Text="OK" CausesValidation="true" />
                    <asp:Button ID="btnCustDatesClose" runat="server" CssClass="displayNone" CausesValidation="true"
                        OnClientClick="return false;" />
                    &nbsp; &nbsp;
                    <asp:Button ID="btnCustDatesCancel" runat="server" Text="Cancel" />
                </td>
            </tr>
            <tr>
                <td class="textCenter">
                    <asp:ValidationSummary ID="valSum" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlFilters" runat="server">
        <AjaxControlToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
            CssClass="CustomTabStyle">
            <AjaxControlToolkit:TabPanel runat="server" ID="tpMainFilters">
                <HeaderTemplate>
                    <span class="bg DefaultCursor"><span class="NoHyperlink">Filters</span></span>
                </HeaderTemplate>
                <ContentTemplate>
                    <%--<uc:ReportFilter ID="rpReportFilter" runat="server" />--%>
                    <table cellpadding="5" cellspacing="2" border="0">
                        <tr class="textCenter">
                            <td class="BenchcostProjectTypeTd" colspan="3">
                                Project Type
                            </td>
                            <td class="Width30Px">
                            </td>
                            <td class="BenchcostpracticeAreaTd">
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
                                <asp:CheckBox ID="chbProposed" runat="server" AutoPostBack="false" Checked="True"
                                    onclick="EnableResetButton();" Text="Proposed" ToolTip="Include Proposed projects into report" />
                            </td>
                            <td>
                            </td>
                            <td class="floatRight practicesTd">
                                <pmc:ScrollingDropDown ID="cblPractices" runat="server" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" CssClass="ScrollingDropDown_cblPractices"
                                    NoItemsType="Nothing" SetDirty="False" DropDownListType="Practice Area" />
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
                            <td>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </AjaxControlToolkit:TabPanel>
            <AjaxControlToolkit:TabPanel ID="tpSortOptions" runat="server">
                <HeaderTemplate>
                    <span class="bg DefaultCursor"><span class="NoHyperlink">Sort Options</span></span>
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="seperateInternalExternal">
                        <asp:CheckBox ID="chbSeperateInternalExternal" runat="server" Text="Separate Internal and External Practices into Separate Tables"
                            Checked="true" onclick="EnableResetButton();" />
                        <br />
                        <asp:CheckBox ID="chbIncludeZeroCostEmps" Text="Include Employees with Zero Sum Costs"
                            runat="server" Checked="false" onclick="EnableResetButton();" />
                    </div>
                </ContentTemplate>
            </AjaxControlToolkit:TabPanel>
        </AjaxControlToolkit:TabContainer>
    </asp:Panel>
</div>
<div class="overflowAuto SupFont">
    <div class="PaddingBottom10">
        <asp:Label ID="lblExternalPractices" runat="server" CssClass="fontBold" Text="Persons with External Practices"></asp:Label>
    </div>
    <asp:GridView ID="gvBenchCosts" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here."
        OnRowDataBound="gvBenchRollOffDates_RowDataBound" CssClass="CompPerfTable gvBenchCosts"
        EnableViewState="true" ShowFooter="true" OnDataBound="gvBench_OnDataBound">
        <AlternatingRowStyle CssClass="alterrow" />
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="ie-bg">
                        <asp:LinkButton ID="btnSortConsultant" runat="server" OnClick="btnSortConsultant_Click">Consultant Name</asp:LinkButton>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HyperLink ID="btnPersonName" runat="server" Text='<%# (string)Eval("HtmlEncodedName") %>'
                        NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Client.Id")) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-CssClass="PracticeArea">
                <HeaderTemplate>
                    <div class="ie-bg">
                        <asp:LinkButton ID="btnSortPractice" runat="server" OnClick="btnSortPractice_Click">Practice Area</asp:LinkButton>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPracticeName" runat="server" CssClass="padRight5" Text='<%# Eval("Practice.HtmlEncodedName") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-CssClass="textCenter Width70Px">
                <HeaderTemplate>
                    <div class="ie-bg">
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
        <FooterStyle CssClass="Footer" />
    </asp:GridView>
    <div id="divBenchCostsInternal" runat="server">
        <hr id="hrDirectorAndPracticeSeperator" runat="server" class="color888888 height1Px" />
        <div class="PaddingBottom10">
            <asp:Label ID="lblInternalPractices" runat="server" CssClass="fontBold" Text="Persons with Internal Practices"></asp:Label>
        </div>
        <asp:GridView ID="gvBenchCostsInternal" runat="server" AutoGenerateColumns="False"
            EmptyDataText="There is nothing to be displayed here." OnRowDataBound="gvBenchRollOffDates_RowDataBound"
            CssClass="CompPerfTable gvBenchCosts" EnableViewState="true" ShowFooter="true"
            OnDataBound="gvBench_OnDataBound">
            <AlternatingRowStyle CssClass="alterrow" />
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortConsultant" runat="server" OnClick="btnSortInternalConsultant_Click">Consultant Name</asp:LinkButton>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:HyperLink ID="btnPersonName" runat="server" Text='<%# Eval("HtmlEncodedName") %>'
                            NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Client.Id")) %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-CssClass="PracticeArea">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            <asp:LinkButton ID="btnSortPractice" runat="server" OnClick="btnSortInternalPractice_Click">Practice Area</asp:LinkButton>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblPracticeName" runat="server" CssClass="padRight5" Text='<%# Eval("Practice.HtmlEncodedName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-CssClass="textCenter Width70Px">
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
            <FooterStyle CssClass="Footer" />
        </asp:GridView>
    </div>
    <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
</div>
<br />
<div class="buttons-block BenchCostFooter SupFont">
    <table>
        <tr>
            <td>
                <b>Legend</b>
            </td>
        </tr>
        <tr>
            <td>
                <sup>1</sup> - Person was hired during this month.
            </td>
        </tr>
        <tr>
            <td>
                <sup>2</sup> - Person was terminated during this month.
            </td>
        </tr>
        <tr>
            <td>
                <sup>3</sup> - Person was changed from salaried to hourly compensation during this
                month.
            </td>
        </tr>
        <tr>
            <td>
                <sup>4</sup> - Person was changed from hourly to salaried compensation during this
                month.
            </td>
        </tr>
    </table>
</div>

