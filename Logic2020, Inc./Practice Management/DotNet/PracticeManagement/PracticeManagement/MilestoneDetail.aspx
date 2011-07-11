<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="MilestoneDetail.aspx.cs" Inherits="PraticeManagement.MilestoneDetail"
    Title="Practice Management - Milestone Details" %>

<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="Controls/ProjectInfo.ascx" TagName="ProjectInfo" TagPrefix="uc1" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/Generic/Notes.ascx" TagName="Notes" TagPrefix="uc" %>
<%@ Register Src="Controls/MilestonePersons/MilestonePersonActivity.ascx" TagName="Activity"
    TagPrefix="mp" %>
<%@ Register Src="Controls/MilestonePersons/CumulativeDailyActivity.ascx" TagName="Cumulative"
    TagPrefix="mp" %>
<%@ Register Src="Controls/MilestonePersons/CumulativeActivity.ascx" TagName="CumulativeTotal"
    TagPrefix="mp" %>
<%@ Register Src="Controls/ProjectExpenses/ProjectExpensesControl.ascx" TagName="ProjectExpenses"
    TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ElementDisabler" %>
<%@ Register TagPrefix="uc" TagName="MessageLabel" Src="~/Controls/MessageLabel.ascx" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Milestone Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Milestone Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function checkDirty(mpId) {
            if (showDialod()) {
                __doPostBack('__Page', mpId);
                return true;
            }

            return false;
        }
    </script>
    <style type="text/css">
        /* --------- Tabs for person and project details pages ------ */
        
        .CustomTabStyle tr
        {
            height: 30px;
        }
        
        .CustomTabStyle td
        {
            background-color: White;
            float: left;
            padding: 8px 0px 6px 0px;
            position: relative;
        }
        
        .CustomTabStyle td a
        {
            text-decoration: none;
        }
        
        .CustomTabStyle td span a
        {
            border-bottom: 1px dashed #0898e6;
        }
        
        .CustomTabStyle td span a:hover
        {
            border-bottom: 1px dashed #006699;
        }
        
        .CustomTabStyle td a.collapse
        {
            display: none;
            position: absolute;
        }
        
        .CustomTabStyle .SelectedSwitch a.collapse
        {
            display: block;
            right: 5px;
            top: 10px;
        }
        
        .CustomTabStyle td span.bg
        {
            padding: 8px 20px 7px 10px;
        }
        
        .CustomTabStyle .SelectedSwitch span.bg
        {
            background-color: #e2ebff;
        }
        
        .tab-pane
        {
            background-color: #e2ebff;
            padding: 5px;
        }
        
        /* ------------------------ */
        
        table.ProjectDetail-ProjectInfo-Table td
        {
            padding-left: 4px;
        }
    </style>
    <table class="WholeWidth">
        <tr>
            <td>
                <uc1:ProjectInfo ID="pdProjectInfo" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="WholeWidth">
                <div id="divPrevNextMainContent" class="main-content" runat="server">
                    <div class="page-hscroll-wrapper">
                        <div class="side-r">
                        </div>
                        <div class="side-l">
                        </div>
                        <div id="divLeft" class="scroll-left" runat="server" visible="false">
                            <asp:HyperLink ID="lnkPrevMilestone" runat="server" ToolTip="Previous milestone">
                                <span id="captionLeft" runat="server"></span>
                            </asp:HyperLink>
                            <label id="lblLeft" runat="server">
                            </label>
                        </div>
                        <div id="divRight" class="scroll-right" runat="server" visible="false">
                            <asp:HyperLink ID="lnkNextMilestone" runat="server" ToolTip="Next milestone">
                                <span id="captionRight" runat="server"></span>
                            </asp:HyperLink>
                            <label id="lblRight" runat="server">
                            </label>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="hidDirty" runat="server" />
            <div style="background-color: #E2EBFF; padding: 5px; margin-bottom: 10px; margin-top: 10px;">
                <table>
                    <tr>
                        <td>
                            Milestone Name
                            <asp:TextBox ID="txtMilestoneName" runat="server" onchange="setDirty();"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqMilestoneName" runat="server" ControlToValidate="txtMilestoneName"
                                ErrorMessage="The Milestone Name is required." ToolTip="The Milestone Name is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Milestone"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td>
                                        From
                                    </td>
                                    <td>
                                        <uc2:DatePicker ID="dtpPeriodFrom" runat="server" ValidationGroup="Milestone" OnSelectionChanged="dtpPeriod_SelectionChanged" />
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqPeriodFrom" runat="server" ControlToValidate="dtpPeriodFrom"
                                            ErrorMessage="The Period From is required." ToolTip="The Period From is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Milestone"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPeriodFrom" runat="server" ControlToValidate="dtpPeriodFrom"
                                            ErrorMessage="The Period From has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The Project Start has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="Milestone"></asp:CompareValidator>
                                        <asp:CustomValidator ID="cstCheckStartDateForExpensesExistance" runat="server" ErrorMessage="From Date cannot be changed because Project has Expenses earlier than selected From Date."
                                            ToolTip="From Date cannot be changed because Project has Expenses earlier than selected From Date."
                                            ValidationGroup="Milestone" Text="*" OnServerValidate="cstCheckStartDateForExpensesExistance_OnServerValidate"
                                            Display="Dynamic">
                                        </asp:CustomValidator>
                                    </td>
                                    <td>
                                        &nbsp;to&nbsp;
                                    </td>
                                    <td>
                                        <uc2:DatePicker ID="dtpPeriodTo" runat="server" ValidationGroup="Milestone" OnSelectionChanged="dtpPeriod_SelectionChanged" />
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqPeriodTo" runat="server" ControlToValidate="dtpPeriodTo"
                                            ErrorMessage="The Period To is required." ToolTip="The Period To is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Milestone"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPeriod" runat="server" ControlToCompare="dtpPeriodTo"
                                            ControlToValidate="dtpPeriodFrom" Display="Dynamic" EnableClientScript="False"
                                            ErrorMessage="The Period To must be greater than or equal to the Period From."
                                            Operator="LessThanEqual" SetFocusOnError="True" Type="Date" ToolTip="The Period To must be greater than or equal to the Period From."
                                            ValidationGroup="Milestone">*</asp:CompareValidator>
                                        <asp:CompareValidator ID="compPeriodTo" runat="server" ControlToValidate="dtpPeriodTo"
                                            ErrorMessage="The Period To has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The Project To has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                            Type="Date" ValidationGroup="Milestone"></asp:CompareValidator>
                                        <asp:CustomValidator ID="cstCheckEndDateForExpensesExistance" runat="server" ErrorMessage="To Date cannot be changed because Project has Expenses beyond selected To Date."
                                            ToolTip="To Date cannot be changed because Project has Expenses beyond selected To Date."
                                            ValidationGroup="Milestone" Text="*" OnServerValidate="cstCheckEndDateForExpensesExistance_OnServerValidate"
                                            Display="Dynamic">
                                        </asp:CustomValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td rowspan="2" valign="middle">
                                        Revenue
                                    </td>
                                    <td colspan="3">
                                        <asp:RadioButton ID="rbtnHourlyRevenue" runat="server" AutoPostBack="true" Checked="true"
                                            GroupName="Revenue" OnCheckedChanged="Revenue_CheckedChanged" onclick="setDirty();"
                                            Text="Hourly - Set hourly rate when you add people to this milestone" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rbtnFixedRevenue" runat="server" AutoPostBack="true" GroupName="Revenue"
                                            OnCheckedChanged="Revenue_CheckedChanged" onclick="setDirty();" Text="Fixed Milestone Payment of" />
                                    </td>
                                    <td>
                                        $
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtFixedRevenue" runat="server" onchange="setDirty();"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="reqFixedRevenue" runat="server" ControlToValidate="txtFixedRevenue"
                                            Display="Dynamic" EnableClientScript="false" ErrorMessage="The Revenue is required."
                                            SetFocusOnError="true" Text="*" ToolTip="The Revenue is required." ValidationGroup="Milestone"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compFixedRevenue" runat="server" ControlToValidate="txtFixedRevenue"
                                            Display="Dynamic" EnableClientScript="false" ErrorMessage="A number with 2 decimal digits is allowed for the Revenue."
                                            Operator="DataTypeCheck" SetFocusOnError="true" Text="*" ToolTip="A number with 2 decimal digits is allowed for the Revenue."
                                            Type="Currency" ValidationGroup="Milestone"></asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbIsChargeable" runat="server" onclick="setDirty();" Text="Time entries in this milestone are billable by default" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbConsultantsCanAdjust" runat="server" onclick="setDirty();" Text="Chargeability of time entries in this milestone can be adjusted by consultants" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:ShadowedTextButton ID="btnAddPerson" runat="server" OnClick="btnAddPerson_Click"
                OnClientClick="if (!confirmSaveDirty()) return false;" Text="Add Person" CssClass="add-btn" />
            <asp:Table ID="tblMilestoneDetailTabViewSwitch" runat="server" CssClass="CustomTabStyle">
                <asp:TableRow ID="rowSwitcher" runat="server">
                    <asp:TableCell ID="cellFinancials" runat="server" CssClass="SelectedSwitch">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnFinancials" runat="server" Text="Financials" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellDetail" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnDetail" runat="server" Text="Detail" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellExpenses" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnExpenses" runat="server" Text="Expenses" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellDailyActivity" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnDailyActivity" runat="server" Text="Daily Activity" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="3"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellCumulativeActivity" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnCumulativeActivity" runat="server" Text="Cumulative Activity"
                                CausesValidation="false" OnCommand="btnView_Command" CommandArgument="4"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellActivitySummary" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnActivitySummary" runat="server" Text="Activity Summary" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="5"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellHistory" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnHistory" runat="server" Text="History" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="6"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellTools" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnTools" runat="server" Text="Tools" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="7"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:MultiView ID="mvMilestoneDetailTab" runat="server" ActiveViewIndex="0">
                <asp:View ID="vwFinancials" runat="server">
                    <asp:Panel ID="pnlFinancials" runat="server" CssClass="tab-pane">
                        <table style="background-color: #F9FAFF">
                            <tr>
                                <td>
                                    Estimated Revenue
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblTotalRevenue" runat="server" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Reimbursed Expenses, $
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblReimbursedExpenses" runat="server" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Client discount (<asp:Label ID="lblClientDiscount" runat="server"></asp:Label>
                                    &nbsp;%)
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblClientDiscountAmount" runat="server" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Revenue net of Discounts
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblTotalRevenueNet" runat="server" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    COGS
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblTotalCogs" runat="server" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Expenses, $
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblExpenses" runat="server" Font-Bold="true" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Gross Margin
                                </td>
                                <td align="right" nowrap="nowrap">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTotalMargin" runat="server" Font-Bold="true" />
                                            </td>
                                            <td id="tdTargetMargin" runat="server">
                                                &nbsp;(<asp:Label ID="lblTargetMargin" runat="server" />)
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Sales commission (<asp:Label ID="lblSalesCommissionPercentage" runat="server"></asp:Label>
                                    &nbsp;%)
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblProjectedSalesCommission" runat="server" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Practice Manager commission
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblPracticeManagerCommission" runat="server" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Net Margin
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblFinalMilestoneMargin" runat="server" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwDetails" runat="server">
                    <asp:Panel ID="pnlDetails" runat="server" CssClass="tab-pane" Style="overflow: auto;
                        margin-bottom: 5px;">
                        <asp:GridView EnableViewState="false" ID="gvPeople" runat="server" AutoGenerateColumns="False"
                            BorderStyle="None" BorderWidth="1" EmptyDataText="There is nothing to be displayed."
                            OnRowDataBound="gvPeople_RowDataBound" ShowFooter="true" CssClass="CompPerfTable WholeWidth"
                            GridLines="None" BackColor="White" OnDataBound="gvPeople_OnDataBound">
                            <AlternatingRowStyle BackColor="#F9FAFF" />
                            <RowStyle BackColor="White" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Person Name</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:HyperLink ID="btnPersonLink" runat="server" NavigateUrl='<%# GetMpeRedirectUrl(Eval("Id")) %>'
                                            Text='<%# HttpUtility.HtmlEncode(string.Format("{0}, {1}", Eval("Person.LastName"), Eval("Person.FirstName"))) %>'
                                            onclick="return checkDirtyWithRedirect(this.href);" />
                                    </ItemTemplate>
                                    <FooterStyle BorderStyle="None" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Role</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblPersonRole" runat="server" CssClass="spacing" Text='<%# Eval("Entries[0].Role.Name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle BorderStyle="None" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap" style="white-space: nowrap;">
                                            Start Date</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartDate" runat="server" CssClass="spacing" Text='<%# ((DateTime)Eval("Entries[0].StartDate")).ToString("d") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle BorderStyle="None" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="End">
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap" style="white-space: nowrap;">
                                            End Date</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblEndDate" runat="server" CssClass="spacing" Text='<%# ((DateTime?)Eval("Entries[0].EndDate")).HasValue ? ((DateTime)Eval("Entries[0].EndDate")).ToString("d") : string.Empty %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle BorderStyle="None" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Eff. Bill Rate</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblEffectiveBillRate" runat="server" CssClass="spacing" Text='<%# Eval("Entries[0].ComputedFinancials.BillRateMinusDiscount")%>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        Totals by months
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total Hours">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            &#931 Hours</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblTotalHours" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.HoursBilled") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Right" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Revenue</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblRevenueContribution" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.Revenue") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle BorderStyle="None" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Right" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Margin</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblMarginContribution" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.GrossMargin") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle BorderStyle="None" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Right" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            &#931 Revenue</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblTotalRevenue" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.Revenue") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle VerticalAlign="Top" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            &#931 Gross Margin</div>
                                        </th>
                                    </HeaderTemplate>
                                    <ItemStyle Font-Bold="true" HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblTotalMargin" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.GrossMargin") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwExpenses" runat="server">
                    <asp:Panel ID="pnlExpenses" runat="server" CssClass="tab-pane">
                        <table class="CompPerfTable" style="width: 50%">
                            <tr>
                                <th>
                                    <div class="ie-bg" style="padding: 0px 0px 0px 2px;">
                                        Expense, $</div>
                                </th>
                                <th>
                                    <div class="ie-bg" style="padding: 0px 0px 0px 2px;">
                                        Reimbursed, %</div>
                                </th>
                                <th>
                                    <div class="ie-bg" style="padding: 0px 0px 0px 2px;">
                                        Reimbursed, $</div>
                                </th>
                            </tr>
                            <tr style="background-color: White;">
                                <td>
                                    <asp:Label ID="lblExpenseAmount" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblReimbursedPrcnt" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblReimbursedAmount" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwDaily" runat="server">
                    <asp:Panel ID="pnlDaily" runat="server" CssClass="tab-pane">
                        <mp:Activity ID="mpaDaily" runat="server" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwCumulativeActivity" runat="server">
                    <asp:Panel ID="plnCumulativeActivity" runat="server" CssClass="tab-pane">
                        <mp:Cumulative ID="mpaCumulative" runat="server" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwActivitySummary" runat="server">
                    <asp:Panel ID="pnlActivitySummary" runat="server" CssClass="tab-pane">
                        <mp:CumulativeTotal ID="mpaTotal" runat="server" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwHistory" runat="server">
                    <asp:Panel ID="pnlHistory" runat="server" CssClass="tab-pane">
                        <uc:Notes ID="nMilestone" runat="server" Target="Milestone" OnNoteAdded="nMilestone_OnNoteAdded"
                            GridVisible="false" />
                        <uc:ActivityLogControl runat="server" ID="activityLog" DisplayDropDownValue="Milestone"
                            DateFilterValue="Year" ShowDisplayDropDown="false" ShowProjectDropDown="false" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwTools" runat="server">
                    <asp:Panel ID="pnlTools" runat="server" CssClass="tab-pane">
                        <table cellpadding="10" style="vertical-align: top">
                            <tr>
                                <td>
                                    <asp:Panel ID="pnlMoveMilestone" runat="server">
                                        <table>
                                            <tr>
                                                <td>
                                                    Move milestone
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtShiftDays" runat="server" TabIndex="2"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="reqShiftDays" runat="server" ControlToValidate="txtShiftDays"
                                                        Display="Dynamic" EnableClientScript="false" ErrorMessage="The Shift for Days is required."
                                                        SetFocusOnError="true" Text="*" ToolTip="The Shift for Days is required." ValidationGroup="ShiftDays"></asp:RequiredFieldValidator>
                                                    <asp:CompareValidator ID="compShiftDays" runat="server" ControlToValidate="txtShiftDays"
                                                        Display="Dynamic" EnableClientScript="false" ErrorMessage="The Shift for Days must be an integer number."
                                                        Operator="DataTypeCheck" SetFocusOnError="true" Text="*" ToolTip="The Shift for Days must be an integer number."
                                                        Type="Integer" ValidationGroup="ShiftDays"></asp:CompareValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    days. (e.g. 3 or -3)
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="text-align: center">
                                                    <asp:CheckBox ID="chbMoveFutureMilestones" runat="server" TabIndex="2" Text="Move Future Milestones" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnMoveMilestone" runat="server" OnClick="btnMoveMilestone_Click"
                                                        OnClientClick="if (!confirm('Do you really want to move the milestone?')) return false;"
                                                        TabIndex="2" Text="Move Milestone" ValidationGroup="ShiftDays" CssClass="pm-button" />
                                                    <ext:ElementDisablerExtender ID="ElementDisablerExtender1" runat="server" TargetControlID="btnMoveMilestone"
                                                        ControlToDisableID="btnClone" />
                                                    <ext:ElementDisablerExtender ID="ElementDisablerExtender2" runat="server" TargetControlID="btnMoveMilestone"
                                                        ControlToDisableID="btnMoveMilestone" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlCloneMilestone" runat="server">
                                        <table>
                                            <tr>
                                                <td>
                                                    Duration
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtCloneDuration" runat="server" TabIndex="3"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="reqCloneDuration" runat="server" ControlToValidate="txtCloneDuration"
                                                        Display="Dynamic" EnableClientScript="false" ErrorMessage="The Duration is required."
                                                        SetFocusOnError="true" Text="*" ToolTip="The Duration is required." ValidationGroup="Clone"></asp:RequiredFieldValidator>
                                                    <asp:CompareValidator ID="compCloneDuration" runat="server" ControlToValidate="txtCloneDuration"
                                                        Display="Dynamic" EnableClientScript="false" ErrorMessage="The Duration must be a positive integer."
                                                        Operator="GreaterThan" SetFocusOnError="true" Text="*" ToolTip="The Duration must be a positive integer."
                                                        Type="Integer" ValidationGroup="Clone" ValueToCompare="0"></asp:CompareValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnClone" runat="server" OnClick="btnClone_Click" TabIndex="3" Text="Clone Milestone"
                                                        ValidationGroup="Clone" CssClass="pm-button" />
                                                    <ext:ElementDisablerExtender ID="edeClone1" runat="server" TargetControlID="btnClone"
                                                        ControlToDisableID="btnClone" />
                                                    <ext:ElementDisablerExtender ID="edeClone2" runat="server" TargetControlID="btnClone"
                                                        ControlToDisableID="btnMoveMilestone" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </asp:View>
            </asp:MultiView>
            <table>
                <tr>
                    <td colspan="2">
                        <uc:MessageLabel ID="lblError" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                            WarningColor="Orange" />
                        <uc:MessageLabel ID="lblResult" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                            WarningColor="Orange" />
                        <asp:ValidationSummary ID="vsumMilestone" runat="server" EnableClientScript="false"
                            ValidationGroup="Milestone" />
                        <asp:ValidationSummary ID="vsumShiftDays" runat="server" EnableClientScript="false"
                            ValidationGroup="ShiftDays" />
                        <asp:ValidationSummary ID="vsumClone" runat="server" EnableClientScript="false" ValidationGroup="Clone" />
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:HiddenField ID="hdnMilestoneId" runat="server" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete Milestone" ToolTip="Delete the milestone"
                            CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="if (!confirm('Do you really want to delete the milestone?')) return false;" />&nbsp;
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CausesValidation="true"
                            ValidationGroup="Milestone" />&nbsp;
                        <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />
                        <script type="text/javascript">
                            function disableSaveButton() {
                                document.getElementById('<%= btnSave.ClientID %>').disabled = true;
                            }
                        </script>
                        <ajaxToolkit:AnimationExtender ID="aeBtnSave" runat="server" TargetControlID="btnSave">
                            <Animations>
					            <OnClick>
					                <ScriptAction Script="disableSaveButton();" />
					            </OnClick>
                            </Animations>
                        </ajaxToolkit:AnimationExtender>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <AjaxControlToolkit:UpdatePanelAnimationExtender ID="upnlBody_UpdatePanelAnimationExtender"
        runat="server" Enabled="True" TargetControlID="upnlBody">
        <Animations>
			<OnUpdating>
				<EnableAction AnimationTarget="btnAddPerson" Enabled="false" />
			</OnUpdating>
        </Animations>
    </AjaxControlToolkit:UpdatePanelAnimationExtender>
</asp:Content>

