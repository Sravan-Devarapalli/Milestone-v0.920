<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="MilestonePersonList.aspx.cs" Inherits="PraticeManagement.MilestonePersonList" %>

<%@ Register Src="~/Controls/Milestones/MilestonePersonBar.ascx" TagName="MilestonePersonBar"
    TagPrefix="uc" %>
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
    <title>Practice Management - Milestone-Person List</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Milestone-Person List
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
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

        function checkDirty(entityId) {
            if (showDialod()) {
                __doPostBack('__Page', entityId);
                return true;
            }

            return false;
        }

        function EnableOrDisableSaveButton() {
            var btnSave = document.getElementById("<%= btnSave.ClientID%>");
            var hdnEnableSaveButton = document.getElementById("<%= hdnEnableSaveButton.ClientID%>");
            if (btnSave != null && btnSave != "undefined") {

                if (hdnEnableSaveButton.value == "true") {
                    btnSave.disabled = "";
                }
                else {
                    btnSave.disabled = "disabled";
                }
            }
        }


        function SetValueForhdnField() {
            var hdnGetDirty = document.getElementById("<%= hdnGetDirty.ClientID%>");
            if (getDirty()) {
                hdnGetDirty.value = "true";
            }
            else {
                hdnGetDirty.value = "false";
            }
        }

        function SetTooltipsForallDropDowns() {
            var optionList = document.getElementsByTagName('option');

            for (var i = 0; i < optionList.length; ++i) {

                optionList[i].title = optionList[i].innerHTML;
            }

        }

    </script>
    <asp:UpdatePanel ID="upnlBody" ChildrenAsTriggers="true" runat="server">
        <ContentTemplate>
            <uc1:ProjectInfo ID="pdProjectInfo" runat="server" />
            <p style="margin-top: 10px; margin-bottom: 10px;">
                Milestone
                <asp:Label ID="lblMilestoneName" runat="server" Font-Bold="true"></asp:Label>
                (<asp:Label ID="lblMilestoneStartDate" runat="server" Font-Bold="true" />
                &mdash;
                <asp:Label ID="lblMilestoneEndDate" runat="server" Font-Bold="true" />)</p>
            <br />
            <table class="WholeWidth">
                <tr>
                    <td align="right" style="padding: 10px; padding-right: 0px;">
                        <asp:Button ID="btnAddPerson" runat="server" Text="Add Person" OnClick="btnAddPerson_Click"
                            ToolTip="Add Person" />
                        <asp:HiddenField ID="hdnGetDirty" Value="false" runat="server" />
                        <asp:HiddenField ID="hdnEnableSaveButton" Value="false" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:GridView ID="gvMilestonePersonEntries" runat="server" AutoGenerateColumns="False"
                OnRowDataBound="gvMilestonePersonEntries_RowDataBound" CellPadding="0" CssClass="CompPerfTable WholeWidth"
                OnRowUpdating="gvMilestonePersonEntries_RowUpdating" EditRowStyle-Wrap="false"
                RowStyle-Wrap="false" HeaderStyle-Wrap="false" GridLines="None" BackColor="White">
                <AlternatingRowStyle BackColor="#F9FAFF" Height="25px" />
                <RowStyle BackColor="White" Height="25px" />
                <Columns>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <ItemStyle Width="5%" HorizontalAlign="Center" Height="20px" Wrap="false" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgMilestonePersonEntryEdit" ToolTip="Edit" runat="server" OnClientClick="startUpdating();"
                                OnClick="imgMilestonePersonEntryEdit_OnClick" ImageUrl="~/Images/icon-edit.png" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:ImageButton ID="imgMilestonePersonEntryUpdate" ToolTip="Update" runat="server"
                                OnClientClick="SetValueForhdnField();" ImageUrl="~/Images/icon-check.png" CommandName="Update" />
                            <asp:ImageButton ID="imgMilestonePersonEntryCancel" ToolTip="Cancel" runat="server"
                                ImageUrl="~/Images/no.png" OnClick="imgMilestonePersonEntryCancel_OnClick" OnClientClick="SetValueForhdnField();startUpdating();" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                Person</div>
                        </HeaderTemplate>
                        <ItemStyle Width="22%" Height="20px" />
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkPersonName" runat="server" OnClick="lnkPersonName_OnClick"
                                MilestonePersonId='<%# Eval("MilestonePersonId") %>' Text='<%# HttpUtility.HtmlEncode(string.Format("{0}, {1}", Eval("ThisPerson.LastName"), Eval("ThisPerson.FirstName"))) %>'
                                OnClientClick="if (!confirmSaveDirty()) return false;" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlPersonName" onchange="setDirty();" Width="98%" runat="server" />
                            <asp:HiddenField ID="hdnPersonId" runat="server" Value='<%# Eval("ThisPerson.Id") %>' />
                            <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPersonName"
                                ErrorMessage="The Person Name is required." ToolTip="The Person Name is required."
                                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                ValidationGroup="MilestonePerson"></asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="reqMilestonePersonName" runat="server" ControlToValidate="ddlPersonName"
                                ErrorMessage="The Person Name is required." ToolTip="The Person Name is required."
                                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                ValidationGroup="MilestonePersonEntry"></asp:RequiredFieldValidator>
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
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                Role</div>
                        </HeaderTemplate>
                        <ItemStyle Width="10%" Height="20px" Wrap="false" />
                        <ItemTemplate>
                            <asp:Label ID="lblRole" runat="server" Text='<%# Eval("Role.Name") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlRole" onchange="setDirty();" runat="server" Width="98%">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                Start Date</div>
                        </HeaderTemplate>
                        <ItemStyle Width="11%" Height="20px" HorizontalAlign="Center" Wrap="false" />
                        <ItemTemplate>
                            <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <uc2:DatePicker ID="dpPersonStart" runat="server" ValidationGroup="MilestonePersonEntry"
                                OnClientChange="setDirty();" TextBoxWidth="85%" OnSelectionChanged="dpPersonStart_SelectionChanged"
                                AutoPostBack="true" DateValue='<%# Eval("StartDate") %>' />
                            <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpPersonStart"
                                ErrorMessage="The Person Start Date is required." ToolTip="The Person Start Date is required."
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
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                End Date</div>
                        </HeaderTemplate>
                        <ItemStyle Width="11%" Height="20px" Wrap="false" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <uc2:DatePicker ID="dpPersonEnd" runat="server" ValidationGroup="MilestonePersonEntry"
                                OnClientChange="setDirty();" TextBoxWidth="85%" OnSelectionChanged="dpPersonEnd_SelectionChanged"
                                AutoPostBack="true" DateValue='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value : DateTime.MinValue %>' />
                            <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                ErrorMessage="The Person End Date is required." ToolTip="The Person End Date is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="MilestonePersonEntry"></asp:RequiredFieldValidator>
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
                            <asp:CompareValidator ID="compPersonEnd" runat="server" ControlToValidate="dpPersonEnd"
                                ControlToCompare="dpPersonStart" ErrorMessage="The Person End Date must be greater than or equal to the Person Start Date."
                                ToolTip="The Person End Date must be greater than or equal to the Person Start Date."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="MilestonePersonEntry" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                Hours per day</div>
                        </HeaderTemplate>
                        <ItemStyle Width="10%" Height="20px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="lblHoursPerDay" runat="server" Text='<%# Eval("HoursPerDay") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtHoursPerDay" runat="server" Width="70%" Text='<%# Eval("HoursPerDay") %>'
                                onchange="setDirty();"></asp:TextBox>
                            <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
                                ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                Type="Currency" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                            <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txtHoursPerDay"
                                ErrorMessage=" The Hours Per Day must be greater than 0 and less or equals to 24."
                                ToolTip=" The Hours Per Day must be greater than 0 and less or equals to 24."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                MinimumValue="0.01" MaximumValue="24" Type="Double" ValidationGroup="MilestonePersonEntry"></asp:RangeValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                Hourly Rate</div>
                        </HeaderTemplate>
                        <ItemStyle Width="8%" Wrap="false" Height="20px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("HourlyAmount") != null ? Eval("HourlyAmount") : string.Empty %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Label ID="lblAmountInsert" runat="server" Text="$"></asp:Label>
                            <asp:TextBox ID="txtAmount" runat="server" onchange="setDirty();" Width="70%" Text='<%# Eval("HourlyAmount") != null ? Eval("HourlyAmount.Value") : string.Empty %>'></asp:TextBox>
                            <asp:CustomValidator ID="reqHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                ValidateEmptyText="true" ErrorMessage="The Amount is required." ToolTip="The Amount is required."
                                Text="*" SetFocusOnError="true" EnableClientScript="false" Display="Dynamic"
                                ValidationGroup="MilestonePersonEntry" OnServerValidate="reqHourlyRevenue_ServerValidate"></asp:CustomValidator>
                            <asp:CompareValidator ID="compHourlyRevenue" runat="server" ControlToValidate="txtAmount"
                                ErrorMessage="A number with 2 decimal digits is allowed for the Revenue." ToolTip="A number with 2 decimal digits is allowed for the Revenue."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Currency" ValidationGroup="MilestonePersonEntry"></asp:CompareValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                Margin %</div>
                        </HeaderTemplate>
                        <ItemStyle Width="7%" Height="20px" HorizontalAlign="Right" />
                        <ItemTemplate>
                            <asp:Label ID="lblTargetMargin" runat="server" Text='<%# string.Format(Constants.Formatting.PercentageFormat, Eval("ComputedFinancials.TargetMargin") ?? 0) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                Total Hours in Milestone</div>
                        </HeaderTemplate>
                        <ItemStyle Width="16%" Height="20px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="lblHoursInPeriodDay" runat="server" Text='<%# Eval("ProjectedWorkloadWithVacation")  %>'></asp:Label>
                            <asp:Label ID="lblVacationIncludedAsterix" runat="server" Text="*" ForeColor="Red"
                                Visible="false" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtHoursInPeriod" Width="50%" runat="server" Text='<%# Eval("ProjectedWorkloadWithVacation") %>'
                                onchange="setDirty();"></asp:TextBox>
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
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Panel ID="pnlInsertMilestonePerson" runat="server">
                <table width="100%" class="CompPerfTable WholeWidth" cellspacing="0" border="0" style="background-color: White;
                    border-collapse: collapse;">
                    <tr id="thInsertMilestonePerson" runat="server" style="white-space: nowrap;" visible="false">
                        <th align="center" style="height: 20px; width: 5%; white-space: nowrap;" scope="col">
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </th>
                        <th align="center" style="height: 20px; width: 22%;" scope="col">
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
                        <th align="center" style="width: 10%;" scope="col">
                            <div class="ie-bg no-wrap">
                                Hours per day</div>
                        </th>
                        <th id="thHourlyRate" runat="server" align="center" style="width: 8%; white-space: nowrap;"
                            scope="col">
                            <div class="ie-bg no-wrap">
                                Hourly Rate</div>
                        </th>
                        <th align="center" style="width: 7%;" scope="col">
                            <div class="ie-bg no-wrap">
                                Margin %</div>
                        </th>
                        <th align="center" style="width: 16%;" scope="col">
                            <div class="ie-bg no-wrap">
                                Total Hours in Milestone</div>
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
                    <td colspan="4">
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
                        <br />
                        <asp:ValidationSummary ID="vsumMileStonePersons" runat="server" EnableClientScript="false" />
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
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" style="padding-bottom: 3px">
                        <asp:Label ID="lblVacationIncludedText" runat="server" Text="*" ForeColor="Red" Visible="false"
                            EnableViewState="false" />
                        <uc:Label ID="lblResultMessage" runat="server" ErrorColor="Red" InfoColor="Green" />
                    </td>
                </tr>
                <tr>
                    <td align="left" colspan="4">
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" ValidationGroup="MilestonePerson"
                            Enabled="false" />&nbsp;
                        <asp:Button ID="btnCancelAndReturn" Text="Return to Person List" runat="server" OnClick="btnCancelAndReturn_OnClick" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

