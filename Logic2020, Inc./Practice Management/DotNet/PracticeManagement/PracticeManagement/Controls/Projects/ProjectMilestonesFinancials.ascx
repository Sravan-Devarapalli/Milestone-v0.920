<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectMilestonesFinancials.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectMilestonesFinancials" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="uc" TagName="MessageLabel" Src="~/Controls/MessageLabel.ascx" %>
<asp:Repeater ID="repMilestones" runat="server" OnItemDataBound="repMilestones_ItemDataBound" OnItemCommand="repMilestones_ItemCommand">
    <HeaderTemplate>
        <div class="minheight250Px">
            <table id="tblMilestones" class="gvStrawmen CompPerfTable WholeWidth">
                <thead>
                    <tr class="MilestoneHeaderText CursorPointer">
                        <th class="Width3Percent">
                        </th>
                        <th class="wrapMilestoneName ie-bg NoBorder">
                            Milestone Name<span id="name"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="Width9Percent ie-bg NoBorder">
                            Start Date<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="Width9Percent ie-bg NoBorder">
                            End Date<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg NoBorder Width7Percent"> 
                            Services Revenue<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg NoBorder Width11Percent">
                            Contribution Margin<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg NoBorder Width7Percent">
                            Margin %<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="Width7Percent ie-bg NoBorder">
                            Billable<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                    </tr>
                </thead>
                <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="Width3Percent">
                <asp:ImageButton ID="imgbtnEdit" CommandName="edit" runat="server" ToolTip="Edit Milestone Name"
                    ImageUrl="~/Images/icon-edit.png" />
                <asp:ImageButton ID="imgMilestoneDelete" ToolTip="Delete" runat="server" MilestoneId='<%# Eval("MilestoneId") %>'
                    OnClientClick="if (!confirm('Do you really want to delete the milestone?')) return false;"
                    OnClick="imgMilestoneDelete_Click" ImageUrl="~/Images/icon-delete.png" />
                <asp:CustomValidator ID="custExpenseValidate" ValidationGroup="MilestoneDelete" runat="server"
                    ErrorMessage="This milestone cannot be deleted, because project has expenses during the milestone period."
                    Text="*" ToolTip="This milestone cannot be deleted, because project has expenses during the milestone period."
                    OnServerValidate="custExpenseValidate_OnServerValidate" Display="Dynamic"></asp:CustomValidator>
                <asp:CustomValidator ID="custProjectStatus" ValidationGroup="MilestoneDelete" runat="server"
                    ErrorMessage="Projects with Active status should have atleast one milestone added to it."
                    Text="*" ToolTip="Projects with Active status should have atleast one milestone added to it."
                    OnServerValidate="custProjectStatus_OnServerValidate" Display="Dynamic"></asp:CustomValidator>
                <asp:CustomValidator ID="custCSATValidate" ValidationGroup="MilestoneDelete" runat="server"
                    ErrorMessage="Milestone cannot be deleted as project has CSAT data added to it."
                    Text="*" ToolTip="Milestone cannot be deleted as project has CSAT data added to it."
                    OnServerValidate="custCSATValidate_OnServerValidate" Display="Dynamic"></asp:CustomValidator>
                <asp:CustomValidator ID="custAttribution" ValidationGroup="MilestoneDelete" runat="server"
                    ErrorMessage="Milestone cannot be deleted as project has Attribution data added to it."
                    Text="*" ToolTip="Milestone cannot be deleted as project has Attribution data added to it."
                    OnServerValidate="custAttribution_OnServerValidate" Display="Dynamic"></asp:CustomValidator>
                <asp:CustomValidator ID="custFeedback" ValidationGroup="MilestoneDelete" runat="server"
                    Text="*" ErrorMessage="The milestone cannot be deleted because there are project feedback records has been marked as completed.  The milestone can be deleted if the status of all the feedbacks changed to 'Not Completed' or 'Canceled'. Please navigate to the 'Project Feedback' tab for more information to make the necessary adjustments."
                    ToolTip="The milestone cannot be deleted because there are project feedback records has been marked as completed.  The milestone can be deleted if the status of all the feedbacks changed to 'Not Completed' or 'Canceled'. Please navigate to the 'Project Feedback' tab for more information to make the necessary adjustments."
                    OnServerValidate="custFeedback_OnServerValidate" Display="Dynamic"></asp:CustomValidator>
                <asp:ImageButton ID="imgbtnUpdate" runat="server" ToolTip="Save" OnClick="imgbtnUpdate_OnClick" Visible="false" CommandName="edit"
                    ImageUrl="~/Images/icon-check.png" />
                <asp:ImageButton ID="imgbtnCancel" runat="server" ToolTip="Cancel" OnClick="imgbtnCancel_OnClick" Visible="false"
                    ImageUrl="~/Images/no.png" />
            </td>
            <td class="Width35Percent TextAlignLeftImp">
                <asp:HyperLink ID="hlMilestoneName" runat="server" NavigateUrl='<%# GetMilestoneRedirectUrl(Eval("MilestoneId")) %>'
                    Text='<%# GetWrappedTest(HttpUtility.HtmlEncode((string)Eval("MilestoneName"))) %>'
                    onclick='<%# "return checkDirty(\"" + MILESTONE_TARGET + "\", " + Eval("MilestoneId") + ")" %>' />
                <asp:TextBox ID="tbMilestoneName" runat="server" MilestoneId='<%# Eval("MilestoneId") %>' Visible="false"
                    Text='<%# Bind("MilestoneName") %>'></asp:TextBox> 
                <asp:RequiredFieldValidator ID="rfvMilestoneName" runat="server" ControlToValidate="tbMilestoneName" 
                    EnableClientScript="false" Text="*" ErrorMessage="Milestone Name Required" ToolTip="Milestone Name Required"></asp:RequiredFieldValidator>
            </td>
            <td class="Width9Percent">
                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>' />
            </td>
            <td class="Width9Percent">
                <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime)Eval("ProjectedDeliveryDate")).ToString("MM/dd/yyyy") %>' />
            </td>
            <td>
                <asp:Label ID="lblRevenue" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("Revenue")).ToString() %>'
                    CssClass="Revenue"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblEstimatedMargin" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("GrossMargin")).ToString() %>'
                    NegativeValue='<%# (decimal) Eval("GrossMargin") < 0 %>' CssClass="Margin" />
            </td>
            <td>
                <asp:Label ID="lblTargetMargin" runat="server" Text='<%# string.Format(PraticeManagement.Constants.Formatting.PercentageFormat, Eval("TargetMargin") ?? 0) %>'></asp:Label>
            </td>
            <td class="Width7Percent">
                <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table></div>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" style="display: none;" class="EmptyMessagediv" runat="server">
    No milestones have been created for this project.
</div>
<asp:CustomValidator runat="server" ID="cvAttributionPopup" OnServerValidate="cvAttributionPopup_ServerValidate"
    ValidationGroup="AttributionPopup"></asp:CustomValidator>
<asp:HiddenField ID="hdnAttribution" Value="false" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeAttribution" runat="server" TargetControlID="hdnAttribution"
    BehaviorID="mpeAttributionBehaviourId" BackgroundCssClass="modalBackground" PopupControlID="pnlAttribution"
    DropShadow="false" />
<asp:Panel ID="pnlAttribution" runat="server" CssClass="popUp yScrollAuto" Style="display: none;">
    <table class="WholeWidth">
        <tr class="PopUpHeader">
            <th colspan="2">
                Attention!
            </th>
        </tr>
        <tr id="trAttributionRecord" runat="server">
            <td>
                <p>
                    &nbsp;&nbsp;&nbsp; This action cannot be done as the following attribution records
                    have the person start date and end date out of the project's start date and project's
                    end date in commissions tab.</p>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Repeater runat="server" ID="repDeliveryPersons">
                    <HeaderTemplate>
                        &nbsp;&nbsp;Delivery Attribution:
                    </HeaderTemplate>
                    <ItemTemplate>
                        <br />
                        &nbsp;&nbsp;&nbsp;&nbsp; <b>
                            <%# Eval("TargetName") %></b>&nbsp;has&nbsp;startdate&nbsp;<b><%# ((DateTime)Eval("StartDate")).ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%></b>&nbsp;and
                        enddate&nbsp;<b><%# ((DateTime)Eval("EndDate")).ToString(PraticeManagement.Constants.Formatting.EntryDateFormat) %></b>.
                    </ItemTemplate>
                    <FooterTemplate>
                        <br />
                        <br />
                    </FooterTemplate>
                </asp:Repeater>
                <asp:HiddenField runat="server" ID="hdnIsUpdate" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Repeater runat="server" ID="repSalesPersons">
                    <HeaderTemplate>
                        &nbsp;&nbsp;Sales Attribution:</HeaderTemplate>
                    <ItemTemplate>
                        <br />
                        &nbsp;&nbsp;&nbsp;&nbsp; <b>
                            <%# Eval("TargetName") %></b>&nbsp;has&nbsp;startdate&nbsp;<b><%# ((DateTime)Eval("StartDate")).ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%></b>&nbsp;and
                        enddate&nbsp;<b><%# ((DateTime)Eval("EndDate")).ToString(PraticeManagement.Constants.Formatting.EntryDateFormat) %></b>.
                    </ItemTemplate>
                    <FooterTemplate>
                        <br />
                        <br />
                    </FooterTemplate>
                </asp:Repeater>
            </td>
        </tr>
        <tr id="trCommissionsStartDateExtend" runat="server">
            <td>
                <p>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label runat="server" ID="lblCommissionsStartDateExtendMessage"></asp:Label>
                </p>
                <br />
            </td>
        </tr>
        <tr id="trCommissionsEndDateExtend" runat="server">
            <td>
                <p>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label runat="server" ID="lblCommissionsEndDateExtendMessage"></asp:Label>
                </p>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <p>
                    &nbsp;&nbsp;&nbsp; Please Click "OK" to accept the change.
                </p>
                <br />
            </td>
        </tr>
        <tr>
            <td class="textCenter">
                <asp:Button ID="btnOkAttribution" runat="server" ToolTip="OK" Text="OK" CssClass="Width100PxImp"
                    OnClick="btnOkAttribution_Click" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCancelAttribution" runat="server" ToolTip="Cancel" Text="Cancel"
                    OnClick="btnCancelAttribution_Click" CssClass="Width100PxImp" />
            </td>
        </tr>
    </table>
</asp:Panel>
<uc:MessageLabel ID="lblError" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
    WarningColor="Orange" />
<asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
    TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
    OkControlID="btnOKErrorPanel" CancelControlID="btnOKErrorPanel" DropShadow="false" />
<asp:Panel ID="pnlErrorPanel" runat="server" Style="display: none;" CssClass="ProjectDetailErrorPanel PanelPerson">
    <table class="Width100Per">
        <tr>
            <th align="center" class="TextAlignCenter BackGroundColorGray vBottom">
                <b class="BtnClose">Attention!</b>
            </th>
        </tr>
        <tr>
            <td class="Padding10Px">
                <asp:ValidationSummary ID="vsumMilestoneDelete" runat="server" DisplayMode="BulletList"
                    CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false" ShowSummary="true"
                    EnableClientScript="false" HeaderText="Following errors occurred while saving a project."
                    ValidationGroup="MilestoneDelete" />
            </td>
        </tr>
        <tr>
            <td class="Padding10Px TextAlignCenter">
                <asp:Button ID="btnOKErrorPanel" runat="server" ToolTip="OK" Text="OK" CssClass="Width100PxImp"
                    OnClientClick="$find('mpeErrorPanelBehaviourId').hide();return false;" />
            </td>
        </tr>
    </table>
</asp:Panel>

