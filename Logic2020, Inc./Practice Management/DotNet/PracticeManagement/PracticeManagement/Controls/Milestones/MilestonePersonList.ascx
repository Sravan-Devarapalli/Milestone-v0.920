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
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<table class="WholeWidth">
    <tr>
        <td align="right" style="padding: 10px; padding-right: 0px;">
            <asp:Button ID="btnAddPerson" runat="server" Text="Add Resource" OnClick="btnAddPerson_Click"
                ToolTip="Add Resource" />
        </td>
    </tr>
</table>
<asp:GridView ID="gvMilestonePersonEntries" runat="server" AutoGenerateColumns="False"
    OnRowDataBound="gvMilestonePersonEntries_RowDataBound" CellPadding="0" CssClass="CompPerfTable WholeWidth"
    EditRowStyle-Wrap="false" RowStyle-Wrap="false" HeaderStyle-Wrap="false" GridLines="None"
    BackColor="White">
    <AlternatingRowStyle BackColor="#F9FAFF" Height="25px" />
    <RowStyle BackColor="White" Height="25px" />
    <Columns>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg">
                    &nbsp;
                </div>
            </HeaderTemplate>
            <ItemStyle Width="2%" HorizontalAlign="Center" Height="20px" Wrap="false" />
            <ItemTemplate>
                <asp:ImageButton ID="imgCopy" ToolTip="Copy" runat="server" OnClick="imgCopy_OnClick"
                    ImageUrl="~/Images/copy.png" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg">
                    &nbsp;
                </div>
            </HeaderTemplate>
            <ItemStyle Width="4%" HorizontalAlign="Center" Height="20px" Wrap="false" />
            <ItemTemplate>
                <asp:ImageButton ID="imgMilestonePersonEntryEdit" ToolTip="Edit" runat="server" OnClick="imgMilestonePersonEntryEdit_OnClick"
                    ImageUrl="~/Images/icon-edit.png" />
                <asp:ImageButton ID="imgMilestonePersonEntryUpdate" ToolTip="Update" runat="server"
                    OnClick="imgMilestonePersonEntryUpdate_OnClick" Visible="false" ImageUrl="~/Images/icon-check.png" />
                <asp:ImageButton ID="imgMilestonePersonEntryCancel" ToolTip="Cancel" runat="server"
                    Visible="false" ImageUrl="~/Images/no.png" OnClick="imgMilestonePersonEntryCancel_OnClick" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg">
                    &nbsp;
                </div>
            </HeaderTemplate>
            <ItemStyle Width="2%" HorizontalAlign="Center" Height="20px" Wrap="false" />
            <ItemTemplate>
                <asp:ImageButton ID="imgAdditionalAllocationOfResource" runat="server" OnClick="imgAdditionalAllocationOfResource_OnClick"
                    ImageUrl="~/Images/add_16.png" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Person</div>
            </HeaderTemplate>
            <ItemStyle Width="21%" Height="20px" />
            <ItemTemplate>
                <asp:HyperLink ID="lnkPersonName" runat="server" NavigateUrl='<%# GetMpeRedirectUrl((Eval("MilestonePersonId"))) %>'
                    PersonId='<%# Eval("ThisPerson.Id") %>' onclick="return checkDirtyWithRedirect(this.href);"
                    Width="98%" Text='<%# HttpUtility.HtmlEncode(string.Format("{0}, {1}", Eval("ThisPerson.LastName"), Eval("ThisPerson.FirstName"))) %>' />
                <table width="100%" id="tblPersonName" runat="server" visible="false">
                    <tr>
                        <td style="width: 85%;">
                            <asp:DropDownList ID="ddlPersonName" Width="98%" runat="server" />
                            <asp:HiddenField ID="hdnPersonId" runat="server" Value='<%# Eval("ThisPerson.Id") %>' />
                        </td>
                        <td style="width: 15%;">
                            <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPersonName"
                                ErrorMessage="" ToolTip="The Person Name is required." Display="Dynamic" Text="*"
                                EnableClientScript="false" SetFocusOnError="true" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custPeriod" runat="server" ControlToValidate="ddlPersonName"
                                ErrorMessage="" ToolTip="The person you are trying to add is not set as being active during the entire length of their participation in the milestone.  Please adjust the person's hire and compensation records, or change the dates that they are attached to this milestone."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" OnServerValidate="custPerson_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="cvMaxRows" runat="server" ControlToValidate="ddlPersonName"
                                ErrorMessage="" ToolTip="Milestone person with same role cannot have more than 5 entries."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" OnServerValidate="cvMaxRows_ServerValidate"></asp:CustomValidator>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Role</div>
            </HeaderTemplate>
            <ItemStyle Width="10%" Height="20px" Wrap="false" />
            <ItemTemplate>
                <asp:Label ID="lblRole" runat="server" RoleId='<%# Eval("Role.Id") %>' Text='<%# Eval("Role.Name") %>'></asp:Label>
                <asp:DropDownList ID="ddlRole" runat="server" Visible="false" Width="98%">
                </asp:DropDownList>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Start Date</div>
            </HeaderTemplate>
            <ItemStyle Width="11%" Height="20px" HorizontalAlign="Center" Wrap="false" />
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>'></asp:Label>
                <table width="100%" id="tblStartDate" runat="server" visible="false">
                    <tr>
                        <td style="width: 85%;">
                            <uc2:DatePicker ID="dpPersonStart" runat="server" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnClientChange="return true;" TextBoxWidth="95%" AutoPostBack="false" DateValue='<%# Eval("StartDate") %>' />
                        </td>
                        <td style="width: 15%;">
                            <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="" ToolTip="The Person Start Date is required." Text="*" EnableClientScript="false"
                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="" ToolTip="The Person Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:CustomValidator ID="custPersonStart" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="" ToolTip="The Person Start Date must be greater than or equal to the Milestone Start Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" OnServerValidate="custPersonStart_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="custPeriodOvberlapping" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="" ToolTip="The specified period overlaps with another for this person with same role on the milestone."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="custPeriodOvberlapping_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="custPeriodVacationOverlapping" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="" ToolTip="The specified period overlaps with Vacation days for this person on the milestone."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="custPeriodVacationOverlapping_ServerValidate"></asp:CustomValidator>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    End Date</div>
            </HeaderTemplate>
            <ItemStyle Width="11%" Height="20px" Wrap="false" HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                <table width="100%" id="tblEndDate" runat="server" visible="false">
                    <tr>
                        <td style="width: 85%;">
                            <uc2:DatePicker ID="dpPersonEnd" runat="server" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnClientChange="return true;" TextBoxWidth="95%" AutoPostBack="false" DateValue='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value : DateTime.MinValue %>' />
                        </td>
                        <td style="width: 15%;">
                            <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                ErrorMessage="" ToolTip="The Person End Date is required." Text="*" EnableClientScript="false"
                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                ErrorMessage="" ToolTip="The Person End Date must be less than or equal to the Milestone End Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                OnServerValidate="custPersonEnd_ServerValidate" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CustomValidator>
                            <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpPersonEnd"
                                ErrorMessage="" ToolTip="The Person End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:CompareValidator ID="compPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                ControlToCompare="dpPersonStart" ErrorMessage="" ToolTip="The Person End Date must be greater than or equal to the Person Start Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="<%# GetValidationGroup(Container) %>" Operator="GreaterThanEqual"
                                Type="Date"></asp:CompareValidator>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Hours per day</div>
            </HeaderTemplate>
            <ItemStyle Width="11%" Height="20px" HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblHoursPerDay" runat="server" Text='<%# Eval("HoursPerDay") %>'></asp:Label>
                <table width="100%" id="tblHoursPerDay" runat="server" visible="false">
                    <tr>
                        <td style="width: 85%;">
                            <asp:TextBox ID="txtHoursPerDay" runat="server" Width="90%" Text='<%# Eval("HoursPerDay") %>'></asp:TextBox>
                        </td>
                        <td style="width: 15%;">
                            <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                ErrorMessage="" ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                ErrorMessage="" ToolTip=" The Hours Per Day must be greater than 0 and less or equals to 24."
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
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Hourly Rate</div>
            </HeaderTemplate>
            <ItemStyle Width="9%" Wrap="false" Height="20px" HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("HourlyAmount") != null ? Eval("HourlyAmount") : string.Empty %>'></asp:Label>
                <table width="100%" id="tblAmount" runat="server" visible="false">
                    <tr>
                        <td style="width: 85%;">
                            <asp:Label ID="lblAmountInsert" runat="server" Text="$"></asp:Label>
                            <asp:TextBox ID="txtAmount" runat="server" Width="80%" Text='<%# Eval("HourlyAmount") != null ? Eval("HourlyAmount.Value") : string.Empty %>'></asp:TextBox>
                        </td>
                        <td style="width: 15%;">
                            <asp:CustomValidator ID="reqHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                ValidateEmptyText="true" ErrorMessage="" ToolTip="The Amount is required." Text="*"
                                SetFocusOnError="true" EnableClientScript="false" Display="Dynamic" ValidationGroup="<%# GetValidationGroup(Container) %>"
                                OnServerValidate="reqHourlyRevenue_ServerValidate"></asp:CustomValidator>
                            <asp:CompareValidator ID="compHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                ErrorMessage="" ToolTip="A number with 2 decimal digits is allowed for the Revenue."
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
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Margin %</div>
            </HeaderTemplate>
            <ItemStyle Width="7%" Height="20px" HorizontalAlign="Right" />
            <ItemTemplate>
                <asp:Label ID="lblTargetMargin" runat="server" Text='<%# string.Format(Constants.Formatting.PercentageFormat, Eval("ComputedFinancials.TargetMargin") ?? 0) %>'></asp:Label>
                <%-- Empty in edit mode --%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Total Hours</div>
            </HeaderTemplate>
            <ItemStyle Width="9%" Height="20px" HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblHoursInPeriodDay" runat="server" Text='<%# Eval("ProjectedWorkloadWithVacation")  %>'></asp:Label>
                <asp:Label ID="lblVacationIncludedAsterix" runat="server" Text="*" ForeColor="Red"
                    Visible="false" />
                <table width="100%" id="tblHoursInPeriod" runat="server" visible="false">
                    <tr>
                        <td style="width: 85%;">
                            <asp:TextBox ID="txtHoursInPeriod" Width="90%" runat="server" Text='<%# Eval("ProjectedWorkloadWithVacation") %>'></asp:TextBox>
                        </td>
                        <td style="width: 15%;">
                            <asp:CompareValidator ID="compHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriod"
                                ErrorMessage="" ToolTip="A number with 2 decimal digits is allowed for the Hours In Period."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:CompareValidator>
                            <asp:RangeValidator ID="rangHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriod"
                                ErrorMessage="" ToolTip="The Total Hours must be greater than 0." Text="*" EnableClientScript="false"
                                SetFocusOnError="true" Display="Dynamic" MinimumValue="0.01" MaximumValue="15000"
                                Type="Double" ValidationGroup="<%# GetValidationGroup(Container) %>"></asp:RangeValidator>
                            <asp:CustomValidator ID="cvHoursInPeriod" runat="server" ToolTip="Total hours should be a larger value so that Hoursperday will be greater than Zero after rounding."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
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
        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <div class="ie-bg">
                    &nbsp;
                </div>
            </HeaderTemplate>
            <ItemStyle Width="3%" HorizontalAlign="Center" Height="20px" Wrap="false" />
            <ItemTemplate>
                <asp:ImageButton ID="imgMilestonePersonDelete" ToolTip="Delete" runat="server" OnClick="imgMilestonePersonDelete_OnClick"
                    MilestonePersonEntryId='<%# Eval("Id") %>' ImageUrl="~/Images/cross_icon.png" />
                <ajaxToolkit:ConfirmButtonExtender ConfirmText="Are you sure you want to delete this person?"
                    runat="server" TargetControlID="imgMilestonePersonDelete">
                </ajaxToolkit:ConfirmButtonExtender>
                <%-- Empty in edit mode --%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:Panel ID="pnlInsertMilestonePerson" runat="server" Wrap="False">
    <table width="100%" class="CompPerfTable" cellspacing="0" border="0" style="background-color: White;
        border-collapse: collapse;">
        <tr id="thInsertMilestonePerson" runat="server" style="white-space: nowrap;" visible="false">
            <th align="center" style="height: 20px; width: 2%; white-space: nowrap;" scope="col">
                <div class="ie-bg">
                    &nbsp;
                </div>
            </th>
            <th align="center" style="height: 20px; width: 4%; white-space: nowrap;" scope="col">
                <div class="ie-bg">
                    &nbsp;
                </div>
            </th>
            <th align="center" style="height: 20px; width: 2%; white-space: nowrap;" scope="col">
                <div class="ie-bg">
                    &nbsp;
                </div>
            </th>
            <th align="center" style="height: 20px; width: 21%;" scope="col">
                <div class="ie-bg no-wrap">
                    Person</div>
            </th>
            <th align="center" style="height: 20px; width: 10%;" scope="col">
                <div class="ie-bg no-wrap">
                    Role</div>
            </th>
            <th align="center" style="height: 20px; width: 11%;" scope="col">
                <div class="ie-bg no-wrap">
                    Start Date</div>
            </th>
            <th align="center" style="height: 20px; width: 11%;" scope="col">
                <div class="ie-bg no-wrap">
                    End Date</div>
            </th>
            <th align="center" style="width: 11%;" scope="col">
                <div class="ie-bg no-wrap">
                    Hours per day</div>
            </th>
            <th id="thHourlyRate" runat="server" align="center" style="width: 9%; white-space: nowrap;"
                scope="col">
                <div class="ie-bg no-wrap">
                    Hourly Rate</div>
            </th>
            <th align="center" style="width: 7%;" scope="col">
                <div class="ie-bg no-wrap">
                    Margin %</div>
            </th>
            <th align="center" style="width: 9%;" scope="col">
                <div class="ie-bg no-wrap">
                    Total Hours</div>
            </th>
            <th align="center" style="height: 20px; width: 3%; white-space: nowrap;" scope="col">
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
<table>
    <tr>
        <td>
            <asp:ValidationSummary ID="vsumMilestonePersonEntry" runat="server" EnableClientScript="false"
                ValidationGroup="<%# GetValidationGroup(Container) %>" />
            <br />
            <asp:ValidationSummary ID="vsumMileStonePersons" runat="server" EnableClientScript="false" />
        </td>
    </tr>
    <tr>
        <td colspan="2" style="padding-bottom: 3px">
            <asp:Label ID="lblVacationIncludedText" runat="server" Text="*" ForeColor="Red" Visible="false"
                EnableViewState="false" />
            <uc:Label ID="lblResultMessage" runat="server" ErrorColor="Red" InfoColor="Green" />
        </td>
    </tr>
</table>

