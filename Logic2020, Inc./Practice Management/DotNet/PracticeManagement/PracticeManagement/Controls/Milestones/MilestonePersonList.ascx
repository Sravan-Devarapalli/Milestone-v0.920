<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestonePersonList.ascx.cs"
    Inherits="PraticeManagement.Controls.Milestones.MilestonePersonList" %>
<%@ Register Src="~/Controls/Milestones/MilestonePersonBar.ascx" TagName="MilestonePersonBar"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="PraticeManagement" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ElementDisabler" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<table class="WholeWidth">
    <tr>
        <td class="Padding10 padRight0 textRight">
            <asp:Button ID="btnAddPerson" runat="server" Text="Add Resource" OnClick="btnAddPerson_Click"
                ToolTip="Add Resource" />
        </td>
    </tr>
</table>
<asp:GridView ID="gvMilestonePersonEntries" runat="server" AutoGenerateColumns="False"
    OnRowDataBound="gvMilestonePersonEntries_RowDataBound" CssClass="CompPerfTable MileStoneDetailPageResourcesTab"
    EditRowStyle-Wrap="false" RowStyle-Wrap="false" HeaderStyle-Wrap="false" GridLines="None"
    BackColor="White">
    <AlternatingRowStyle CssClass="alterrow" />
    <HeaderStyle CssClass="textCenter" />
    <Columns>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    &nbsp;
                </div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width2Percent" />
            <ItemTemplate>
                <asp:ImageButton ID="imgCopy" ToolTip="Copy" runat="server" OnClick="imgCopy_OnClick"
                    ImageUrl="~/Images/copy.png" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    &nbsp;
                </div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width3Percent" />
            <ItemTemplate>
                <asp:ImageButton ID="imgMilestonePersonEntryEdit" ToolTip="Edit" runat="server" OnClick="imgMilestonePersonEntryEdit_OnClick"
                    ImageUrl="~/Images/icon-edit.png" />
                <asp:ImageButton ID="imgMilestonePersonEntryUpdate" ToolTip="Update" runat="server"
                    OnClick="imgMilestonePersonEntryUpdate_OnClick" Visible="false" ImageUrl="~/Images/icon-check.png" />
                <asp:ImageButton ID="imgMilestonePersonEntryCancel" ToolTip="Cancel" runat="server"
                    Visible="false" ImageUrl="~/Images/no.png" OnClick="imgMilestonePersonEntryCancel_OnClick" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    &nbsp;
                </div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width2Percent" />
            <ItemTemplate>
                <asp:ImageButton ID="imgAdditionalAllocationOfResource" runat="server" OnClick="imgAdditionalAllocationOfResource_OnClick"
                    ToolTip="Extend" ImageUrl="~/Images/add_16.png" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg no-wrap Height30PxImp" style="white-space: normal">
                    Person</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width14Percent textLeft WS-Normal" />
            <ItemTemplate>
                <asp:HyperLink ID="lnkPersonName" runat="server" NavigateUrl='<%# GetMpeRedirectUrl((Eval("MilestonePersonId"))) %>'
                    PersonId='<%# Eval("ThisPerson.Id") %>' onclick="return checkDirtyWithRedirect(this.href);"
                    CssClass="Width98Percent" Text='<%# HttpUtility.HtmlEncode(string.Format("{0}, {1}", Eval("ThisPerson.LastName"), Eval("ThisPerson.FirstName"))) %>' />
                <table class="WholeWidth" id="tblPersonName" runat="server" visible="false">
                    <tr>
                        <td class="Width85Percent">
                            <cc2:CustomDropDown ID="ddlPersonName" CssClass="Width98Percent" runat="server" OnSelectedIndexChanged="ddlPersonName_Changed"
                                PersonId='<%# Eval("ThisPerson.Id") %>' AutoPostBack="true" />
                            <asp:HiddenField ID="hdnPersonId" runat="server" Value='<%# Eval("ThisPerson.Id") %>' />
                            <asp:HiddenField ID="hdnPersonIsStrawMan" runat="server" Value='<%# Eval("ThisPerson.IsStrawMan") %>' />
                        </td>
                        <td class="Width15Percent">
                            <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPersonName"
                                ErrorMessage="The Person Name is required." ToolTip="The Person Name is required."
                                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custPeriod" runat="server" ControlToValidate="ddlPersonName"
                                ErrorMessage="" ToolTip="" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                Display="Dynamic" ValidationGroup="<%# GetValidationGroup(Container) %>" OnServerValidate="custPerson_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="cvMaxRows" runat="server" ControlToValidate="ddlPersonName"
                                ErrorMessage="Milestone person with same role cannot have more than 5 entries."
                                ToolTip="Milestone person with same role cannot have more than 5 entries." Text="*"
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="cvMaxRows_ServerValidate"></asp:CustomValidator>
                            <asp:HiddenField ID="hdnIsFromAddBtn" runat="server" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg no-wrap Height30PxImp" style="white-space: normal">
                    Role</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width7Percent" />
            <ItemTemplate>
                <asp:Label ID="lblRole" runat="server" RoleId='<%# Eval("Role.Id") %>' Text='<%# Eval("Role.Name") %>'></asp:Label>
                <asp:DropDownList ID="ddlRole" runat="server" Visible="false" CssClass="Width98Percent">
                </asp:DropDownList>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Start Date</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width7Percent" />
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>'></asp:Label>
                <table class="WholeWidth" id="tblStartDate" runat="server" visible="false">
                    <tr>
                        <td class="Width85Percent">
                            <asp:HiddenField ID="hdnStartDateValue" runat="server" Value='<%# Eval("StartDate") %>' />
                            <uc2:DatePicker ID="dpPersonStart" runat="server" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnClientChange="dtpStartDate_OnClientChange(this);" TextBoxWidth="95%" AutoPostBack="false"
                                DateValue='<%# Eval("StartDate") %>' OldValue='<%# Eval("StartDate") %>' />
                        </td>
                        <td class="Width15Percent">
                            <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="The Person Start Date is required." ToolTip="The Person Start Date is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="The Person Start Date has an incorrect format. It must be 'MM/DD/YYYY'."
                                ToolTip="The Person Start Date has an incorrect format. It must be 'MM/DD/YYYY'."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:CustomValidator ID="custPersonStart" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="The Person Start Date must be greater than or equal to the Milestone Start Date."
                                ToolTip="The Person Start Date must be greater than or equal to the Milestone Start Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" OnServerValidate="custPersonStart_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="custPeriodOvberlapping" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="A project resource cannot have more than one bill rate on a project during the same time period. Please adjust the start and end dates to make sure that the time periods for the bill rates do not overlap."
                                ToolTip="A project resource cannot have more than one bill rate on a project during the same time period. Please adjust the start and end dates to make sure that the time periods for the bill rates do not overlap."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="custPeriodOvberlapping_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="custPeriodVacationOverlapping" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="The specified period overlaps with Vacation days for this person on the milestone."
                                ToolTip="The specified period overlaps with Vacation days for this person on the milestone."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="custPeriodVacationOverlapping_ServerValidate"></asp:CustomValidator>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    End Date</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width7Percent" />
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                <table class="WholeWidth" id="tblEndDate" runat="server" visible="false">
                    <tr>
                        <td class="Width85Percent">
                            <asp:HiddenField ID="hdnEndDateValue" runat="server" Value='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value : DateTime.MinValue %>' />
                            <uc2:DatePicker ID="dpPersonEnd" runat="server" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnClientChange="dtpStartDate_OnClientChange(this);" OldValue='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value : DateTime.MinValue %>'
                                TextBoxWidth="95%" AutoPostBack="false" DateValue='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value : DateTime.MinValue %>' />
                        </td>
                        <td class="Width15Percent">
                            <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                ErrorMessage="The Person End Date is required." ToolTip="The Person End Date is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                ErrorMessage="The Person End Date must be less than or equal to the Milestone End Date."
                                ToolTip="The Person End Date must be less than or equal to the Milestone End Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                OnServerValidate="custPersonEnd_ServerValidate" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CustomValidator>
                            <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpPersonEnd"
                                ErrorMessage="The Person End Date has an incorrect format. It must be 'MM/DD/YYYY'."
                                ToolTip="The Person End Date has an incorrect format. It must be 'MM/DD/YYYY'."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:CompareValidator ID="compPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                ControlToCompare="dpPersonStart" ErrorMessage="The Person End Date must be greater than or equal to the Person Start Date."
                                ToolTip="The Person End Date must be greater than or equal to the Person Start Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" Operator="GreaterThanEqual"
                                Type="Date"></asp:CompareValidator>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Hours per day</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width6Percent" />
            <ItemTemplate>
                <asp:Label ID="lblHoursPerDay" runat="server" Text='<%# Eval("HoursPerDay") %>'></asp:Label>
                <table class="WholeWidth" id="tblHoursPerDay" runat="server" visible="false">
                    <tr>
                        <td class="Width85Percent">
                            <asp:TextBox ID="txtHoursPerDay" runat="server" CssClass="Width90Percent" Text='<%# Eval("HoursPerDay") %>'></asp:TextBox>
                        </td>
                        <td class="Width15Percent">
                            <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
                                ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                Type="Currency" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                ErrorMessage="The Hours Per Day must be greater than 0 and less or equals to 24."
                                ToolTip=" The Hours Per Day must be greater than 0 and less or equals to 24."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                MinimumValue="0.01" MaximumValue="24" Type="Double" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RangeValidator>
                            <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtHoursPerDay" runat="server"
                                TargetControlID="txtHoursPerDay" FilterMode="ValidChars" FilterType="Custom,Numbers"
                                ValidChars=".">
                            </AjaxControlToolkit:FilteredTextBoxExtender>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Hourly Rate</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width6Percent" />
            <ItemTemplate>
                <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("HourlyAmount") != null ? Eval("HourlyAmount") : string.Empty %>'></asp:Label>
                <table class="WholeWidth" id="tblAmount" runat="server" visible="false">
                    <tr>
                        <td class="Width85Percent">
                            <asp:Label ID="lblAmountInsert" runat="server" Text="$"></asp:Label>
                            <asp:TextBox ID="txtAmount" runat="server" CssClass="Width80Percent" Text='<%# Eval("HourlyAmount") != null ? Eval("HourlyAmount.Value") : string.Empty %>'></asp:TextBox>
                        </td>
                        <td class="Width15Percent">
                            <asp:CustomValidator ID="reqHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                ValidateEmptyText="true" ErrorMessage="The Amount is required." ToolTip="The Amount is required."
                                Text="*" SetFocusOnError="true" EnableClientScript="false" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" OnServerValidate="reqHourlyRevenue_ServerValidate"></asp:CustomValidator>
                            <asp:CompareValidator ID="compHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                ErrorMessage="A number with 2 decimal digits is allowed for the Revenue." ToolTip="A number with 2 decimal digits is allowed for the Revenue."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtAmount" runat="server" TargetControlID="txtAmount"
                                FilterMode="ValidChars" FilterType="Custom,Numbers" ValidChars=".">
                            </AjaxControlToolkit:FilteredTextBoxExtender>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Margin %</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width6Percent textRight" />
            <ItemTemplate>
                <asp:Label ID="lblTargetMargin" runat="server" Text='<%# string.Format(Constants.Formatting.PercentageFormat, Eval("ComputedFinancials.TargetMargin") ?? 0) %>'></asp:Label>
                <%-- Empty in edit mode --%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Total Hours</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width6Percent" />
            <ItemTemplate>
                <table class="WholeWidth" id="tblHoursInPeriod1" runat="server">
                    <tr>
                        <td class="width60P textRightImp">
                            <asp:Label ID="lblHoursInPeriodDay" runat="server" Text='<%# Eval("ProjectedWorkloadWithVacation")  %>'></asp:Label>
                        </td>
                        <td class="textLeft">
                            <asp:Label ID="lbVacationHoursToolTip" runat="server" Text="!" ForeColor="Red" CssClass="error-message fontSizeLarge" />
                        </td>
                    </tr>
                </table>
                <table class="WholeWidth" id="tblHoursInPeriod" runat="server" visible="false">
                    <tr>
                        <td class="Width85Percent">
                            <asp:TextBox ID="txtHoursInPeriod" CssClass="Width90Percent" runat="server" Text='<%# Eval("ProjectedWorkloadWithVacation") %>'></asp:TextBox>
                        </td>
                        <td class="Width15Percent">
                            <asp:CompareValidator ID="compHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriod"
                                ErrorMessage="A number with 2 decimal digits is allowed for the Hours In Period."
                                ToolTip="A number with 2 decimal digits is allowed for the Hours In Period."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:RangeValidator ID="rangHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriod"
                                ErrorMessage="The Total Hours must be greater than 0." ToolTip="The Total Hours must be greater than 0."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                MinimumValue="0.01" MaximumValue="15000" Type="Double" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RangeValidator>
                            <asp:CustomValidator ID="cvHoursInPeriod" runat="server" ToolTip="Total hours should be a larger value so that Hoursperday will be greater than Zero after rounding."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="Total hours should be a larger value so that Hoursperday will be greater than Zero after rounding."
                                OnServerValidate="cvHoursInPeriod_ServerValidate" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CustomValidator>
                            <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtHoursInPeriod" runat="server"
                                TargetControlID="txtHoursInPeriod" FilterMode="ValidChars" FilterType="Custom,Numbers"
                                ValidChars=".">
                            </AjaxControlToolkit:FilteredTextBoxExtender>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    MS Badge Required?
                </div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width4Percent" />
            <ItemTemplate>
                <asp:Label ID="lblBadgeRequired" runat="server" Text='<%# (bool)Eval("MSBadgeRequired") ? "Yes" : "No" %>'></asp:Label>
                <asp:CheckBox ID="chbBadgeRequired" runat="server" Visible="false" Checked='<%# Eval("MSBadgeRequired") %>'
                    PreviouslyChecked='<%# Eval("MSBadgeRequired") %>' AutoPostBack="true" OnCheckedChanged="chbBadgeRequired_CheckedChanged" />
                <asp:CustomValidator ID="custBlocked" runat="server" ErrorMessage="Person is blocked from being badged, please choose another resource or uncheck this box or change the dates."
                    ToolTip="Person is blocked from being badged, please choose another resource or uncheck this box or change the dates."
                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                    ValidationGroup="<%# GetValidationGroup(Container) %>" OnServerValidate="custBlocked_ServerValidate"></asp:CustomValidator>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Badge Start</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width7Percent" />
            <ItemTemplate>
                <asp:Label ID="lblBadgeStart" runat="server" Text='<%# Eval("BadgeStartDate") != null ? ((DateTime?)Eval("BadgeStartDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                <table class="WholeWidth" id="tblBadgeStart" runat="server" visible="false">
                    <tr>
                        <td class="Width85Percent">
                            <asp:HiddenField ID="hdnBadgeStartDateValue" runat="server" Value='<%# Eval("BadgeStartDate") %>' />
                            <uc2:DatePicker ID="dpBadgeStart" runat="server" ValidationGroup='<%# GetValidationGroup(Container) %>'
                                Date='<%# Eval("BadgeStartDate") != null ? ((DateTime?)Eval("BadgeStartDate")).Value.Date : DateTime.MinValue.Date %>'
                                DateValue='<%# Eval("BadgeStartDate") != null ? ((DateTime?)Eval("BadgeStartDate")).Value : DateTime.MinValue %>'
                                OnClientChange="dtpBadgeStartDate_OnClientChange(this);" TextBoxWidth="95%" AutoPostBack="false" />
                        </td>
                        <td class="Width15Percent">
                            <asp:RequiredFieldValidator ID="reqBadgeStart" runat="server" ControlToValidate="dpBadgeStart"
                                ErrorMessage="The Badge Start Date is required." ToolTip="The Badge Start Date is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="compBadgeStartType" runat="server" ControlToValidate="dpBadgeStart"
                                ErrorMessage="The Badge Start Date has an incorrect format. It must be 'MM/DD/YYYY'."
                                ToolTip="The Badge Start Date has an incorrect format. It must be 'MM/DD/YYYY'."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:CustomValidator ID="custBadgeStart" runat="server" ControlToValidate="dpBadgeStart"
                                ErrorMessage="The Badge Start Date must be greater than or equal to the Milestone Start Date."
                                ToolTip="The Badge Start Date must be greater than or equal to the Milestone Start Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" OnServerValidate="custBadgeStart_ServerValidate"></asp:CustomValidator>
                            <asp:CompareValidator ID="compBadgeStartWithPersonStart" runat="server" ControlToValidate="dpBadgeStart"
                                ControlToCompare="dpPersonStart" ErrorMessage="The Badge Start Date must be after or equal to the Person Start Date in the project."
                                ToolTip="The Badge Start Date must be after or equal to the Person Start Date in the project."
                                ValidationGroup="<%# GetValidationGroup(Container) %>" Text="*" EnableClientScript="false"
                                SetFocusOnError="true" Display="Dynamic" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                            <asp:CustomValidator ID="custAftertheBreak" runat="server" ControlToValidate="dpBadgeStart"
                                Text="*" Display="Dynamic" EnableTheming="True" ErrorMessage="18 month start date should be greater than previous break end date."
                                ToolTip="18 month start date should be greater than previous break end date."
                                ValidationGroup="<%# GetValidationGroup(Container) %>" SetFocusOnError="True"
                                OnServerValidate="custAftertheBreak_ServerValidate" EnableClientScript="false"></asp:CustomValidator>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Badge End</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width7Percent" />
            <ItemTemplate>
                <asp:Label ID="lblBadgeEnd" runat="server" Text='<%# Eval("BadgeEndDate") != null ? ((DateTime?)Eval("BadgeEndDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                <table class="WholeWidth" id="tblBadgeEnd" runat="server" visible="false">
                    <tr>
                        <td class="Width85Percent">
                            <asp:HiddenField ID="hdnBadgeEndDateValue" runat="server" Value='<%# Eval("BadgeEndDate") %>' />
                            <uc2:DatePicker ID="dpBadgeEnd" runat="server" ValidationGroup='<%# GetValidationGroup(Container) %>'
                                DateValue='<%# Eval("BadgeEndDate") != null ? ((DateTime?)Eval("BadgeEndDate")).Value : DateTime.MinValue %>'
                                Date='<%# Eval("BadgeEndDate") != null ? ((DateTime?)Eval("BadgeEndDate")).Value.Date : DateTime.MinValue.Date %>'
                                OnClientChange="dtpBadgeStartDate_OnClientChange(this);" TextBoxWidth="95%" AutoPostBack="false" />
                        </td>
                        <td class="Width15Percent">
                            <asp:RequiredFieldValidator ID="reqBadgeEnd" runat="server" ControlToValidate="dpBadgeEnd"
                                ErrorMessage="The Badge End Date is required." ToolTip="The Badge End Date is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custBadgeEnd" runat="server" ControlToValidate="dpBadgeEnd"
                                ErrorMessage="The Badge End Date must be less than or equal to the Milestone End Date."
                                ToolTip="The Badge End Date must be less than or equal to the Milestone End Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                OnServerValidate="custBadgeEnd_ServerValidate" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CustomValidator>
                            <asp:CompareValidator ID="compBadgeEndType" runat="server" ControlToValidate="dpBadgeEnd"
                                ErrorMessage="The Badge End Date has an incorrect format. It must be 'MM/DD/YYYY'."
                                ToolTip="The Badge End Date has an incorrect format. It must be 'MM/DD/YYYY'."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:CompareValidator ID="compBadgeEnd" runat="server" ControlToValidate="dpBadgeEnd"
                                ControlToCompare="dpBadgeStart" ErrorMessage="The Badge End Date must be greater than or equal to the badge Start Date."
                                ToolTip="The Badge End Date must be greater than or equal to the badge Start Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" Operator="GreaterThanEqual"
                                Type="Date"></asp:CompareValidator>
                            <asp:CustomValidator ID="custBadgeInBreakPeriod" runat="server" ErrorMessage="" Text="*"
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="custBadgeInBreakPeriod_ServerValidate"></asp:CustomValidator>
                            <asp:CompareValidator ID="compBadgeEndWithPersonEnd" runat="server" ControlToValidate="dpBadgeEnd"
                                ControlToCompare="dpPersonEnd" ErrorMessage="The Badge End Date must be less than or equal to the Person End Date in the project."
                                ToolTip="The Badge End Date must be less than or equal to the Person End Date in the project."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" Operator="LessThanEqual"
                                Type="Date"></asp:CompareValidator>
                            <asp:CustomValidator ID="custBadgeHasMoredays" runat="server" ErrorMessage="Invalid Badge Start/End dates.  Person cannot be assigned to project/phase longer than 18mos without a badge exception."
                                Text="*" ToolTip="Invalid Badge Start/End dates.  Person cannot be assigned to project/phase longer than 18mos without a badge exception."
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="custBadgeHasMoredays_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="custBadgeNotInEmpHistory" runat="server" ErrorMessage="The person's badge dates in a milestone should be within their hire and termination dates."
                                Text="*" ToolTip="The person's badge dates in a milestone should be within their hire and termination dates."
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="custBadgeNotInEmpHistory_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="custBadgeAfterJuly" runat="server" ErrorMessage="Person's badge dates cannot be before 7/1/2014."
                                Text="*" ToolTip="Person's badge dates cannot be before 7/1/2014." EnableClientScript="false"
                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="custBadgeAfterJuly_ServerValidate"></asp:CustomValidator>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Consultant 18mos End</div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width5Percent" />
            <ItemTemplate>
                <asp:Label ID="lblConsultantsEnd" runat="server" DateValue='<%# Eval("ConsultantEndDate") != null ? ((DateTime?)Eval("ConsultantEndDate")).Value : DateTime.MinValue %>'
                    Text='<%# Eval("ConsultantEndDate") != null ? ((DateTime?)Eval("ConsultantEndDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Badge Exception
                </div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width4Percent" />
            <ItemTemplate>
                <asp:Label ID="lblBadgeException" runat="server" Text='<%# (bool)Eval("BadgeException") ? "Yes" : "No" %>'></asp:Label>
                <asp:CheckBox ID="chbBadgeException" runat="server" Visible="false" Checked='<%# Eval("BadgeException") %>'
                    PreviousChecked='<%# Eval("BadgeException") %>' />
                <asp:CustomValidator ID="custExceptionNotMoreThan18moEndDate" runat="server" ErrorMessage=""
                    Text="*" ToolTip="" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                    ValidationGroup="<%# GetValidationGroup(Container) %>" OnServerValidate="custExceptionNotMoreThan18moEndDate_ServerValidate"></asp:CustomValidator>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    Approved by Ops
                </div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width4Percent" />
            <ItemTemplate>
                <asp:Label ID="lblApproved" runat="server" Text='<%# (bool)Eval("IsApproved") ? "Yes" : "No" %>'></asp:Label>
                <asp:HiddenField ID="hdnApproved" runat="server" Value='<%# (bool)Eval("IsApproved") ? "Yes" : "No" %>' />
                <asp:HiddenField ID="hdnApprovedChange" runat="server" />
                <asp:CheckBox ID="chbOpsApproved" runat="server" Visible="false" Checked='<%# Eval("IsApproved") %>'
                    PreviousChecked='<%# Eval("IsApproved") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="BackImageMilestoneDetail" />
            <HeaderTemplate>
                <div class="ie-bg Height30PxImp" style="white-space: normal">
                    &nbsp;
                </div>
            </HeaderTemplate>
            <ItemStyle CssClass="Width3Percent" />
            <ItemTemplate>
                <asp:ImageButton ID="imgMilestonePersonDelete" ToolTip="Delete" runat="server" OnClientClick="return imgMilestonePersonDelete_OnClientClick(this);"
                    MilestonePersonEntryId='<%# Eval("Id") %>' IsOriginalResource="" ImageUrl="~/Images/cross_icon.png" />
                <%-- Empty in edit mode --%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:Panel ID="pnlTimeOffHoursToolTip" Style="display: none;" runat="server" CssClass="pnlTotal no-wrap">
    <label>
        Time-Off Hour(s):
    </label>
    <asp:Label ID="lblTimeOffHours" runat="server" CssClass="fontBold"></asp:Label>
    <br />
    <label>
        Affected Projected Hours:
    </label>
    <asp:Label ID="lblProjectAffectedHours" runat="server" CssClass="fontBold"></asp:Label>
</asp:Panel>
<asp:HiddenField ID="hdMpePopupDeleteMileStonePersons" Value="false" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeDeleteMileStonePersons" runat="server"
    TargetControlID="hdMpePopupDeleteMileStonePersons" CancelControlID="btnCancel"
    BehaviorID="mpeDeleteMileStonePersons" BackgroundCssClass="modalBackground" PopupControlID="pnlDeleteMileStonePersons"
    DropShadow="false" />
<asp:Panel ID="pnlDeleteMileStonePersons" runat="server" CssClass="popUp" Style="display: none;
    min-width: 300px !important;">
    <table class="WholeWidth">
        <tr class="PopUpHeader">
            <th>
                Attention!
                <asp:Button ID="btnClose" runat="server" CssClass="mini-report-closeNew" ToolTip="Cancel Changes"
                    OnClientClick="return btnClose_OnClientClick();" Text="X"></asp:Button>
            </th>
        </tr>
        <tr>
            <td class="Padding10 padBottom15">
                <table>
                    <tr id="trDeleteOriginalEntry">
                        <td>
                            Clicking "Delete" will result in deleting only this entry.<br />
                            <br />
                            Clicking "Delete All" will result in deleting all the extensions for this entry.
                        </td>
                    </tr>
                    <tr id="trDeleteExtendedEntry">
                        <td class="padLeft30">
                            Are you sure you want to delete this Entry?
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdMilestonePersonEntryId" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center" class="Padding6 padBottom15">
                <table>
                    <tr>
                        <td class="padRight5">
                            <asp:Button ID="btnDeletePersonEntry" Text="Delete" runat="server" OnClick="btnDeletePersonEntry_OnClick" />
                        </td>
                        <td id="tdbtnDeleteAllPersonEntries">
                            <asp:Button ID="btnDeleteAllPersonEntries" Text="Delete All" runat="server" OnClick="btnDeleteAllPersonEntries_OnClick" />
                        </td>
                        <td class="padLeft5">
                            <asp:Button ID="btnCancel" Text="Cancel" runat="server" OnClientClick="return btnClose_OnClientClick();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlInsertMilestonePerson" runat="server">
    <table class="CompPerfTable MileStoneDetailPageResourcesTab">
        <tr id="thInsertMilestonePerson" runat="server" visible="false">
            <th class="Width2Percent">
                <div class="ie-bg">
                    &nbsp;
                </div>
            </th>
            <th class="Width3Percent">
                <div class="ie-bg">
                    &nbsp;
                </div>
            </th>
            <th class="Width2Percent">
                <div class="ie-bg">
                    &nbsp;
                </div>
            </th>
            <th class="Width14Percent">
                <div class="ie-bg no-wrap">
                    Person</div>
            </th>
            <th class="Width7Percent">
                <div class="ie-bg no-wrap">
                    Role</div>
            </th>
            <th class="Width7Percent">
                <div class="ie-bg no-wrap">
                    Start Date</div>
            </th>
            <th class="Width7Percent">
                <div class="ie-bg no-wrap">
                    End Date</div>
            </th>
            <th class="Width6Percent">
                <div class="ie-bg no-wrap">
                    Hours per day</div>
            </th>
            <th id="thHourlyRate" runat="server" class="Width6Percent">
                <div class="ie-bg no-wrap">
                    Hourly Rate</div>
            </th>
            <th class="Width6Percent">
                <div class="ie-bg no-wrap">
                    Margin %</div>
            </th>
            <th class="Width6Percent">
                <div class="ie-bg no-wrap">
                    Total Hours</div>
            </th>
            <th id="thBadgeRequired" runat="server" class="Width4Percent">
                <div class="ie-bg no-wrap">
                    MS Badge Required?</div>
            </th>
            <th id="thBadgeStart" runat="server" class="Width7Percent">
                <div class="ie-bg no-wrap">
                    Badge Start</div>
            </th>
            <th id="thBadgeEnd" runat="server" class="Width7Percent">
                <div class="ie-bg no-wrap">
                    Badge End</div>
            </th>
            <th id="thConsultantEnd" runat="server" class="Width5Percent">
                <div class="ie-bg no-wrap">
                    Consultant 18mos End</div>
            </th>
            <th id="thBadgeException" runat="server" class="Width4Percent">
                <div class="ie-bg no-wrap">
                    Badge Exception</div>
            </th>
            <th id="thApprovedOps" runat="server" class="Width4Percent">
                <div class="ie-bg no-wrap">
                    Approved by Ops</div>
            </th>
            <th class="Width3Percent">
                <div class="ie-bg">
                    &nbsp;
                </div>
            </th>
        </tr>
        <asp:Repeater ID="repPerson" OnItemDataBound="repPerson_ItemDataBound" runat="server">
            <ItemTemplate>
                <uc:MilestonePersonBar runat="server" BarColor="White" ID="mpbar" />
            </ItemTemplate>
            <AlternatingItemTemplate>
                <uc:MilestonePersonBar runat="server" BarColor="#F9FAFF" ID="mpbar" />
            </AlternatingItemTemplate>
        </asp:Repeater>
    </table>
</asp:Panel>
<br />
<table style="width: 100%; background-color: White;" id="tblValidation" runat="server"
    visible="false">
    <tr>
        <td>
            <asp:ValidationSummary ID="vsumMilestonePersonEntry" runat="server" EnableClientScript="false"
                ValidationGroup="<%# GetValidationGroup(Container) %>" />
            <br />
            <asp:ValidationSummary ID="vsumMileStonePersons" runat="server" EnableClientScript="false" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="PaddingBottom3">
            <asp:Label ID="lblVacationIncludedText" runat="server" Text="*" ForeColor="Red" Visible="false"
                EnableViewState="false" />
            <uc:Label ID="lblResultMessage" runat="server" ErrorColor="Red" InfoColor="Green" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdnTargetValidationPanel" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeBadgePanel" runat="server" BehaviorID="mpeBadgePanelBehaviourId"
    TargetControlID="hdnTargetValidationPanel" BackgroundCssClass="modalBackground"
    PopupControlID="pnlValidationPanel" OkControlID="btnOKValidationPanel" CancelControlID="btnOKValidationPanel"
    DropShadow="false" />
<asp:Panel ID="pnlValidationPanel" runat="server" CssClass="popUp ValidationPopUp"
    Style="display: none;">
    <table class="WholeWidth">
        <tr class="PopUpHeader">
            <th>
                Attention!
            </th>
        </tr>
        <tr>
            <td class="Padding10">
                <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
            </td>
        </tr>
        <tr>
            <td class="Padding10 textCenter">
                <asp:Button ID="btnOKValidationPanel" runat="server" ToolTip="OK" Text="OK" CssClass="Width100Px" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:HiddenField ID="hdnTargetExceptionValidation" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeExceptionValidation" runat="server"
    BehaviorID="mpeExceptionValidationBehaviourId" TargetControlID="hdnTargetExceptionValidation"
    BackgroundCssClass="modalBackground" PopupControlID="pnlExceptionValidationPanel"
    OkControlID="btnOkExceptionValidation" DropShadow="false" />
<asp:Panel ID="pnlExceptionValidationPanel" runat="server" CssClass="popUp ValidationPopUp"
    Style="display: none;">
    <table class="WholeWidth">
        <tr class="PopUpHeader">
            <th>
                Attention!
            </th>
        </tr>
        <tr>
            <td class="Padding10 colorRed">
                Requested badge exception will shorten the person’s 18-month clock. See Operations
                and uncheck exception box.
            </td>
        </tr>
        <tr>
            <td class="Padding10 textCenter" style="padding-top: 0px;">
                <asp:Button ID="btnOkExceptionValidation" runat="server" ToolTip="OK" Text="OK" CssClass="Width100Px" />
            </td>
        </tr>
    </table>
</asp:Panel>

