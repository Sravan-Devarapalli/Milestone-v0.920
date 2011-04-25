<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="MilestonePersonDetail.aspx.cs" Inherits="PraticeManagement.MilestonePersonDetail"
    Title="Practice Management - Milestone-Person Details" %>

<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="Controls/ProjectInfo.ascx" TagName="ProjectInfo" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/MilestonePersons/MilestonePersonActivity.ascx" TagName="MPActivity"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/MilestonePersons/MilestonePersonFinancials.ascx" TagName="MilestonePersonFinancials"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/MilestonePersons/CumulativeActivity.ascx" TagName="MPCumulative"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/MilestonePersons/CumulativeDailyActivity.ascx" TagName="MPCumulativeDaily"
    TagPrefix="uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="PraticeManagement" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ElementDisabler" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Milestone-Person Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Milestone-Person Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function checkDirty(entityId) {
            if (showDialod()) {
                __doPostBack('__Page', entityId);
                return true;
            }

            return false;
        }
    </script>
    <script type="text/javascript">
        /*
        This script keeps track on is one of the rows being updated
        */
        var updating = false;

        function startUpdating() {
            updating = true;
        }

        function stopUpdating() {
            updating = false;
        }

        function isUpdating() {
            return updating;
        }
    </script>
    <asp:UpdatePanel ID="pnlBody" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <uc1:ProjectInfo ID="pdProjectInfo" runat="server" />
            <p style="margin-top: 10px; margin-bottom: 10px;">
                Milestone
                <asp:Label ID="lblMilestoneName" runat="server" Font-Bold="true"></asp:Label>
                (<asp:Label ID="lblMilestoneStartDate" runat="server" Font-Bold="true" />
                &mdash;
                <asp:Label ID="lblMilestoneEndDate" runat="server" Font-Bold="true" />)</p>
            <table class="WholeWidth">
                <tr>
                    <td colspan="3">
                        <div id="divPrevNextMainContent" visible="false" class="main-content" runat="server">
                            <div class="page-hscroll-wrapper">
                                <div class="side-r">
                                </div>
                                <div class="side-l">
                                </div>
                                <div id="divLeft" class="scroll-left" runat="server">
                                    <asp:HyperLink ID="lnkPrev" runat="server">
                                        <span id="captionLeft" runat="server"></span>
                                    </asp:HyperLink>
                                    <label id="lblLeft" runat="server">
                                    </label>
                                </div>
                                <div id="divRight" class="scroll-right" runat="server">
                                    <asp:HyperLink ID="lnkNext" runat="server">
                                        <span id="captionRight" runat="server"></span>
                                    </asp:HyperLink>
                                    <label id="lblRight" runat="server">
                                    </label>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        Select a Person&nbsp;
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlPersonName" runat="server" onchange="setDirty();" Width="580px" />
                        <br />
                        <asp:Label ID="lblTimeEntry" runat="server" ForeColor="Gray" Visible="false" Text="* The person has already entered time for this milestone and cannot be reassigned." />
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPersonName"
                            ErrorMessage="The Person Name is required." ToolTip="The Person Name is required."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="MilestonePerson"></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="reqMilestonePersonName" runat="server" ControlToValidate="ddlPersonName"
                            ErrorMessage="The Person Name is required." ToolTip="The Person Name is required."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="MilestonePersonEntry"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
            <br />
            <AjaxControlToolkit:TabContainer ID="tcReportTabs" runat="server" CssClass="CustomTabStyle WholeWidth">
                <AjaxControlToolkit:TabPanel ID="tpTable" runat="server">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Summary</span></a> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:GridView ID="gvMilestonePersonEntries" runat="server" AutoGenerateColumns="False"
                            OnRowDataBound="gvMilestonePersonEntries_RowDataBound" OnRowCancelingEdit="gvMilestonePersonEntries_RowCancelingEdit"
                            OnRowEditing="gvMilestonePersonEntries_RowEditing" OnRowDeleting="gvMilestonePersonEntries_RowDeleting"
                            OnRowUpdating="gvMilestonePersonEntries_RowUpdating" CellPadding="0" CssClass="CompPerfTable WholeWidth" FooterStyle-Height="25px"
                            GridLines="None" BackColor="White">
                            <AlternatingRowStyle BackColor="#F9FAFF" Height="25px" />
                            <RowStyle BackColor="White" Height="25px" />
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="125px" FooterStyle-Width="125px" FooterStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Start Date</div>
                                    </HeaderTemplate>
                                    <ItemStyle Width="125px" HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToShortDateString() %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <uc2:DatePicker ID="dpPersonStart" runat="server" ValidationGroup="MilestonePersonEntry"
                                            OnSelectionChanged="dpPersonStart_SelectionChanged" AutoPostBack="true" DateValue='<%# Eval("StartDate") %>'
                                            TextBoxWidth="90px" />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <uc2:DatePicker ID="dpPersonStart" runat="server" ValidationGroup="MilestonePersonEntry"
                                            OnSelectionChanged="dpPersonStart_SelectionChanged" AutoPostBack="true" DateValue='<%# Milestone.StartDate %>'
                                            TextBoxWidth="90px" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            End Date</div>
                                    </HeaderTemplate>
                                    <ItemStyle Width="120px" HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value.ToShortDateString() : string.Empty %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <uc2:DatePicker ID="dpPersonEnd" runat="server" ValidationGroup="MilestonePersonEntry"
                                            OnSelectionChanged="dpPersonEnd_SelectionChanged" AutoPostBack="true" DateValue='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value : DateTime.MinValue %>'
                                            TextBoxWidth="90px" />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <uc2:DatePicker ID="dpPersonEnd" runat="server" ValidationGroup="MilestonePersonEntry"
                                            OnSelectionChanged="dpPersonEnd_SelectionChanged" AutoPostBack="true" DateValue='<%# Milestone.ProjectedDeliveryDate %>'
                                            TextBoxWidth="90px" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Role</div>
                                    </HeaderTemplate>
                                    <ItemStyle Width="140px" HorizontalAlign="Center" />
                                    <FooterStyle Width="140px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblRole" runat="server" Text='<%# Eval("Role.Name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ddlRole" runat="server" Width="95" onchange="setDirty();">
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:DropDownList ID="ddlRole" runat="server" Width="95" onchange="setDirty();">
                                        </asp:DropDownList>
                                    </FooterTemplate>
                                </asp:TemplateField>                                
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Hours per day</div>
                                    </HeaderTemplate>
                                    <ItemStyle Width="120px" HorizontalAlign="Center" />
                                    <FooterStyle Width="120px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblHoursPerDay" runat="server" Text='<%# Eval("HoursPerDay") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtHoursPerDay" runat="server" Width="95" Text='<%# Eval("HoursPerDay") %>'
                                            onchange="setDirty();"></asp:TextBox>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="txtHoursPerDay" runat="server" Width="95" onchange="setDirty();"></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Hourly Rate</div>
                                    </HeaderTemplate>
                                    <ItemStyle Width="85px" HorizontalAlign="Center" />
                                    <FooterStyle Width="85px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("HourlyAmount") != null ? Eval("HourlyAmount") : string.Empty %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtAmount" runat="server" onchange="setDirty();" Width="60" Text='<%# Eval("HourlyAmount") != null ? Eval("HourlyAmount.Value") : string.Empty %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="txtAmount" runat="server" Width="60" onchange="setDirty();"></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Margin %</div>
                                    </HeaderTemplate>
                                    <ItemStyle Width="90px" HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblTargetMargin" runat="server" Text='<%# string.Format(Constants.Formatting.PercentageFormat, Eval("ComputedFinancials.TargetMargin") ?? 0) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap" style="padding-left:5px; padding-right:5px">
                                            Total Hours in Milestone</div>
                                    </HeaderTemplate>
                                    <ItemStyle Width="120px" HorizontalAlign="Center" />
                                    <FooterStyle Width="120px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblHoursInPeriodDay" runat="server" Text='<%# Eval("ProjectedWorkloadWithVacation")  %>'></asp:Label>
                                        <asp:Label ID="lblVacationIncludedAsterix" runat="server" Text="*" ForeColor="Red"
                                            Visible="false"  />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtHoursInPeriod" runat="server" Width="95" Text='<%# Eval("ProjectedWorkloadWithVacation") %>'
                                            onchange="setDirty();"></asp:TextBox>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="txtHoursInPeriod" runat="server" Width="95" onchange="setDirty();"></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap" style="padding-left:5px; padding-right:5px">
                                            Billable Hours in Milestone</div>
                                    </HeaderTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Center" />
                                    <FooterStyle Width="120px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblBillableHours" runat="server" Text='<%# Eval("BillableHours") != null ? Eval("BillableHours") : 0 %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="100px" >
                                    <ItemTemplate>
                                        &nbsp;</ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The Person Start Date is required." ToolTip="The Person Start Date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidationGroup="MilestonePersonEntry"></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                            ErrorMessage="The Person End Date is required." ToolTip="The Person End Date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidationGroup="MilestonePersonEntry"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The Person Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The Person Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonStart" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The Person Start Date must be greater than or equal to the Milestone Start Date."
                                            ToolTip="The Person Start Date must be greater than or equal to the Milestone Start Date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidationGroup="MilestonePersonEntry" OnServerValidate="custPersonStart_ServerValidate"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                            ControlToCompare="dpPersonStart" ErrorMessage="The Person End Date must be greater than or equal to the Person Start Date."
                                            ToolTip="The Person End Date must be greater than or equal to the Person Start Date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidationGroup="MilestonePersonEntry" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                            ErrorMessage="The Person End Date must be less than or equal to the Milestone End Date."
                                            ToolTip="The Person End Date must be less than or equal to the Milestone End Date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonEnd_ServerValidate" ValidationGroup="MilestonePersonEntry"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpPersonEnd"
                                            ErrorMessage="The Person End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The Person End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                            ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
                                            ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                            Type="Currency" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                            ErrorMessage=" The Hours Per Day must be greater than 0 and less or equals to 24."
                                            ToolTip=" The Hours Per Day must be greater than 0 and less or equals to 24." Text="*"
                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" MinimumValue="0.01"
                                            MaximumValue="24" Type="Double" ValidationGroup="MilestonePersonEntry"></asp:RangeValidator>
                                        <asp:CustomValidator ID="reqHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                            ErrorMessage="The Amount is required." ToolTip="The Amount is required." Text="*"
                                            SetFocusOnError="true" EnableClientScript="false" Display="Dynamic" ValidationGroup="MilestonePersonEntry"
                                            OnServerValidate="reqHourlyRevenue_ServerValidate"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                            ErrorMessage="A number with 2 decimal digits is allowed for the Revenue." ToolTip="A number with 2 decimal digits is allowed for the Revenue."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Currency" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPeriodOvberlapping" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The specified period overlaps with another for this person on the milestone."
                                            ToolTip="The specified period overlaps with another for this person on the milestone."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidateEmptyText="false" ValidationGroup="MilestonePersonEntry" OnServerValidate="custPeriodOvberlapping_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator ID="custPeriodVacationOverlapping" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The specified period overlaps with Vacation days for this person on the milestone."
                                            ToolTip="The specified period overlaps with Vacation days for this person on the milestone."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidateEmptyText="false" ValidationGroup="MilestonePersonEntry" OnServerValidate="custPeriodVacationOverlapping_ServerValidate"></asp:CustomValidator>
                                        <!-- Hours in period Validation-->
                                        <asp:CompareValidator ID="compHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriod"
                                            ErrorMessage="A number with 2 decimal digits is allowed for the Hours In Period."
                                            ToolTip="A number with 2 decimal digits is allowed for the Hours In Period."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Currency" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:RangeValidator ID="rangHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriod"
                                            ErrorMessage="The Hours In Period must be more then 0 and less or equals to 15,000."
                                            ToolTip="The Hours In Period must be more then 0 and less or equals to 15,000."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            MinimumValue="0" MaximumValue="15000" Type="Double" ValidationGroup="MilestonePersonEntry"></asp:RangeValidator>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The Person Start Date is required." ToolTip="The Person Start Date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidationGroup="MilestonePersonEntry"></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                            ErrorMessage="The Person End Date is required." ToolTip="The Person End Date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidationGroup="MilestonePersonEntry"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The Person Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The Person Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonStart" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The Person Start Date must be greater than or equal to the Milestone Start Date."
                                            ToolTip="The Person Start Date must be greater than or equal to the Milestone Start Date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidationGroup="MilestonePersonEntry" OnServerValidate="custPersonStart_ServerValidate"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                            ControlToCompare="dpPersonStart" ErrorMessage="The Person End Date must be greater than or equal to the Person Start Date."
                                            ToolTip="The Person End Date must be greater than or equal to the Person Start Date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidationGroup="MilestonePersonEntry" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                            ErrorMessage="The Person End Date must be less than or equal to the Milestone End Date."
                                            ToolTip="The Person End Date must be less than or equal to the Milestone End Date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonEnd_ServerValidate" ValidationGroup="MilestonePersonEntry"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpPersonEnd"
                                            ErrorMessage="The Person End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The Person End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                            ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
                                            ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                            Type="Currency" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                            ErrorMessage=" The Hours Per Day must be greater than 0 and less or equals to 24." ToolTip=" The Hours Per Day must be greater than 0 and less or equals to 24."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            MinimumValue="0.01" MaximumValue="24" Type="Double" ValidationGroup="MilestonePersonEntry"></asp:RangeValidator>
                                        <asp:CustomValidator ID="reqHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                            ErrorMessage="The Amount is required." ToolTip="The Amount is required." Text="*"
                                            SetFocusOnError="true" EnableClientScript="false" Display="Dynamic" OnServerValidate="reqHourlyRevenue_ServerValidate"
                                            ValidationGroup="MilestonePersonEntry"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                            ErrorMessage="A number with 2 decimal digits is allowed for the Revenue." ToolTip="A number with 2 decimal digits is allowed for the Revenue."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Currency" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPeriodOvberlapping" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The specified period overlaps with another for this person on the milestone."
                                            ToolTip="The specified period overlaps with another for this person on the milestone."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidateEmptyText="false" ValidationGroup="MilestonePersonEntry" OnServerValidate="custPeriodOvberlapping_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator ID="custPeriodVacationOverlapping" runat="server" ControlToValidate="dpPersonStart"
                                            ErrorMessage="The specified period overlaps with Vacation days for this person on the milestone."
                                            ToolTip="The specified period overlaps with Vacation days for this person on the milestone."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidateEmptyText="false" ValidationGroup="MilestonePersonEntry" OnServerValidate="custPeriodVacationOverlapping_ServerValidate"></asp:CustomValidator>
                                        <!-- Hours in period Validation-->
                                        <asp:CompareValidator ID="compHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriod"
                                            ErrorMessage="A number with 2 decimal digits is allowed for the Hours In Period."
                                            ToolTip="A number with 2 decimal digits is allowed for the Hours In Period."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Currency" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                                        <asp:RangeValidator ID="rangHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriod"
                                            ErrorMessage="The Hours In Period must be more then 0 and less or equals to 15,000."
                                            ToolTip="The Hours In Period must be more then 0 and less or equals to 15,000."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            MinimumValue="0" MaximumValue="15000" Type="Double" ValidationGroup="MilestonePersonEntry"></asp:RangeValidator>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False" ItemStyle-Wrap="false">
                                    <HeaderStyle Width="100px" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                            Text="Edit" OnClientClick="startUpdating()"></asp:LinkButton>
                                        &nbsp;<asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                            Text="Delete"></asp:LinkButton>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:LinkButton ID="btnUpdate" runat="server" CausesValidation="True" CommandName="Update"
                                            Text="Update" OnClientClick="stopUpdating()"></asp:LinkButton>
                                        &nbsp;<asp:LinkButton ID="btnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                            Text="Cancel" OnClientClick="stopUpdating()"></asp:LinkButton>&nbsp;
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:LinkButton ID="btnInsert" runat="server" CausesValidation="true" Text="Add another row"
                                            OnClick="btnInsert_Click" EnableViewState="false" OnClientClick="setDirty();"></asp:LinkButton>&nbsp;
                                    </FooterTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel ID="tpFinancials" runat="server">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Financials</span></a> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <uc:MilestonePersonFinancials ID="financials" runat="server" />
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel ID="tpActivity" runat="server">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Activity per Day</span></a> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <uc:MPActivity ID="mpActivity" runat="server" />
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel ID="tpCumulative" runat="server">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Total Cumulative Activity</span></a> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <uc:MPCumulative ID="mpCumulative" runat="server" />
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel ID="tpCumulativeDaily" runat="server">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Daily Cumulative Activity</span></a> </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <uc:MPCumulativeDaily ID="mpCumulativeDaily" runat="server" />
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
            </AjaxControlToolkit:TabContainer>
            <br />
            <table>
                <%--				<tr>
					<td colspan="4">
						<table>
							<tr>
								<td>Gross Hourly Bill Rate</td>
								<td>
									<asp:Label ID="lblGrossHourlyBillRate" runat="server" Text="Unavailable" CssClass="Revenue"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>Loaded Hourly Pay Rate</td>
								<td>
									<asp:Label ID="lblLoadedHourlyPay" runat="server" Text="Unavailable"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>Projected Hours</td>
								<td>
									<asp:Label ID="lblProjectedHours" runat="server" Text="Unavailable"></asp:Label>
								</td>

							</tr>
							<tr>
								<td>Projected Milestone Revenue Contribution</td>
								<td>
									<asp:Label ID="lblProjectedRevenueContribution" runat="server" Text="Unavalable" CssClass="Revenue"></asp:Label>
								</td>

							</tr>
							<tr>
								<td>Projected Milestone COGS</td>
								<td>
									<asp:Label ID="lblProjectedCogs" runat="server" Text="Unavailable"></asp:Label>
								</td>

							</tr>
							<tr>
								<td>Projected Milestone Margin Contribution</td>
								<td>
									<asp:Label ID="lblProjectedMarginContribution" runat="server" Text="Unavailable" CssClass="Margin"></asp:Label>
								</td>

							</tr>
							<tr>
								<td colspan="4">&nbsp;</td>
							</tr>
						</table>
					</td>
				</tr>
                --%>
                <tr>
                    <td colspan="4">
                        <asp:CustomValidator ID="custPeriod" runat="server" ControlToValidate="ddlPersonName"
                            ErrorMessage="The person you are trying to add is not set as being active during the entire length of their participation in the milestone.  Please adjust the person's hire and compensation records, or change the dates that they are attached to this milestone."
                            ToolTip="The person you are trying to add is not set as being active during the entire length of their participation in the milestone.  Please adjust the person's hire and compensation records, or change the dates that they are attached to this milestone."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ValidationGroup="MilestonePerson" OnServerValidate="custPerson_ServerValidate"></asp:CustomValidator>
                        <asp:CustomValidator ID="custEntries" runat="server" ControlToValidate="ddlPersonName"
                            ErrorMessage="You must specify at least one detail record." ToolTip="You must specify at least one detail record."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ValidationGroup="MilestonePerson" OnServerValidate="custEntries_ServerValidate"></asp:CustomValidator>
                        <asp:CustomValidator ID="custDuplicatedPerson" runat="server" ControlToValidate="ddlPersonName"
                            ErrorMessage="The specified person is already assigned on this milestone." ToolTip="The specified person is already assigned on this milestone."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ValidationGroup="MilestonePerson" OnServerValidate="custDuplicatedPerson_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:ValidationSummary ID="vsumMilestonePerson" runat="server" EnableClientScript="false"
                            ValidationGroup="MilestonePerson" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:ValidationSummary ID="vsumMilestonePersonEntry" runat="server" EnableClientScript="false"
                            ValidationGroup="MilestonePersonEntry" />
                    </td>
                    <td colspan="3">
                        <asp:Panel ID="pnlChangeMilestone" runat="server" Visible="false">
                            Available actions:
                            <asp:Table ID="tblMileMoveActions" runat="server" BorderStyle="None">
                                <asp:TableRow ID="cellMoveMilestone">
                                    <asp:TableCell> * </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:LinkButton ID="btnMoveMilestone" runat="server" OnClick="btnMoveMilestone_Click">Extend the parent milestone's end date</asp:LinkButton>&nbsp;to&nbsp;
                                        <asp:Label ID="lblMoveMilestoneDate" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell> * </asp:TableCell>
                                    <asp:TableCell>Change person's end date for this milestone.</asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow ID="cellTerminationOrCompensation">
                                    <asp:TableCell> * </asp:TableCell>
                                    <asp:TableCell>Check the following: person involved has a termination date before the new end of the milestone or there is no compensation record.</asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                            <%--<ul>
                                <li><asp:LinkButton ID="btnMoveMilestone" runat="server" 
                                        onclick="btnMoveMilestone_Click">Extend the parent milestone's end date</asp:LinkButton>&nbsp;to&nbsp;<asp:Label
                                        ID="lblMoveMilestoneDate" runat="server" Text=""></asp:Label> 
                                </li>
                                <li>Change person end date</li>
                            </ul>--%></asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" style="padding-bottom:3px">
                        <asp:Label ID="lblVacationIncludedText" runat="server" Text="*" ForeColor="Red" Visible="false"
                            EnableViewState="false" />
                            <uc:Label ID="lblResultMessage" runat="server" ErrorColor="Red" InfoColor="Green"/>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="4">
                        <asp:Button ID="btnDelete" runat="server" Text="Delete person from milestone" CausesValidation="False"
                            OnClick="btnDelete_Click" />&nbsp;
                        <AjaxControlToolkit:ConfirmButtonExtender ID="cbtnDelete" runat="server" TargetControlID="btnDelete"
                            ConfirmText="Are you sure you want to delete the person from the milestone?" />
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="if (isUpdating()) {if (!confirmSaveDirty()) return false;}"
                            ValidationGroup="MilestonePerson" />&nbsp;
                        <asp:Button ID="btnCancelAndReturn" Text="Cancel and return" runat="server" OnClick="btnCancelAndReturn_OnClick" />
                        <ext:ElementDisablerExtender ID="edeSave" runat="server" TargetControlID="btnSave"
                            ControlToDisableID="btnSave" />
                        <br />
                        <asp:Label ID="lblDeleteActive" runat="server" ForeColor="Gray" Visible="false" Text="* There is time entry for this milestone-person. Administrator should first remove/reassign that time entries and then remove the person from the milestone." />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

