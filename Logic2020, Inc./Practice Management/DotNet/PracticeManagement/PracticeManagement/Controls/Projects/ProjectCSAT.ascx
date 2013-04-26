﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectCSAT.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectCSAT" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Panel ID="pnlTabCSAT" runat="server" CssClass="tab-pane">
    <div class="PaddingBottom35Px">
        <asp:ShadowedTextButton ID="stbCSAT" runat="server" CausesValidation="false" OnClick="stbCSAT_Click"
            OnClientClick="if(!ConfirmSaveOrExit()) return false;" CssClass="add-btn" Text="Add CSAT" />
        <asp:HiddenField ID="hdnCSATPopUp" runat="server" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpeCSAT" runat="server" TargetControlID="hdnCSATPopUp"
            BackgroundCssClass="modalBackground" PopupControlID="pnlCSAT" DropShadow="false" />
    </div>
    <asp:Panel ID="pnlCSAT" runat="server" Style="display: none" CssClass="PanelCSAT">
        <table class="WholeWidth Padding5">
            <tr class="BackGroundColorGray Height27Px">
                <td align="center" class="font14Px LineHeight25Px WS-Normal" colspan="2">
                    Add CSAT
                    <asp:Button ID="btnCancelPopUpHeader" runat="server" CssClass="mini-report-close floatright"
                        OnClick="btnCancelPopUp_Click" ToolTip="Close" Text="X"></asp:Button>
                </td>
            </tr>
            <tr>
                <td class="width30P PaddingTop5">
                    Project Review period
                </td>
                <td class="PaddingTop5">
                    <table class="WholeWidth">
                        <tr>
                            <td>
                                Start Date
                            </td>
                            <td>
                                <span class="fl-left Width85Percent">
                                    <uc2:DatePicker ID="dpReviewStartDate" ValidationGroup="CSATPopup" runat="server"
                                        TextBoxWidth="90%" AutoPostBack="false" />
                                </span><span class="Width15Percent vMiddle">
                                    <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpReviewStartDate"
                                        ValidationGroup="CSATPopup" ErrorMessage="The Review Start Date is required."
                                        ToolTip="The Start Date is required." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                        Display="Static"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="compStartDate" runat="server" ControlToValidate="dpReviewStartDate"
                                        ValidationGroup="CSATPopup" ErrorMessage="The Review Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        ToolTip="The Review Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                </span>
                            </td>
                            <td>
                                End Date
                            </td>
                            <td>
                                <span class="fl-left Width85Percent">
                                    <uc2:DatePicker ID="dpReviewEndDate" ValidationGroup="CSATPopup" runat="server" TextBoxWidth="90%"
                                        AutoPostBack="false" />
                                </span><span class="Width15Percent vMiddle">
                                    <asp:RequiredFieldValidator ID="reqEndDate" runat="server" ControlToValidate="dpReviewEndDate"
                                        ValidationGroup="CSATPopup" ErrorMessage="The Review End Date is required." ToolTip="The End Date is required."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpReviewEndDate"
                                        ValidationGroup="CSATPopup" ErrorMessage="The Review End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        ToolTip="The Review End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                </span>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="PaddingTop5">
                    CSAT Completion Date
                </td>
                <td class="PaddingTop5">
                    <span class="fl-left Width85Percent">
                        <uc2:DatePicker ID="dpCompletionDate" ValidationGroup="CSATPopup" runat="server"
                            TextBoxWidth="90%" AutoPostBack="false" />
                    </span><span class="Width15Percent vMiddle">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="dpCompletionDate"
                            ValidationGroup="CSATPopup" ErrorMessage="The Completion Date is required." ToolTip="The Completion Date is required."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="dpCompletionDate"
                            ValidationGroup="CSATPopup" ErrorMessage="The Completion Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            ToolTip="The Completion Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                    </span>
                </td>
            </tr>
            <tr>
                <td class="PaddingTop5">
                    CSAT Reviewer
                </td>
                <td class="PaddingTop5">
                    <asp:DropDownList ID="ddlReviewer" runat="server">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="reqReviewer" runat="server" ControlToValidate="ddlReviewer"
                        ValidationGroup="CSATPopup" ErrorMessage="The Reviewer is required." ToolTip="The Reviewer is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="PaddingTop5">
                    On a scale of 1-10, Would you refer<br />
                    Logic20/20 to a friend or colleague?
                </td>
                <td class="PaddingTop5">
                    <asp:DropDownList ID="ddlScore" runat="server" CssClass="Width100Px">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="reqScore" runat="server" ControlToValidate="ddlScore"
                        ValidationGroup="CSATPopup" ErrorMessage="The Referral Score is required." ToolTip="The Referral Score is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="PaddingTop5">
                    Comments
                </td>
                <td class="PaddingTop5">
                    <textarea id="taComments" runat="server" rows="5"></textarea>
                    <asp:RequiredFieldValidator ID="reqComments" runat="server" ControlToValidate="taComments"
                        ValidationGroup="CSATPopup" ErrorMessage="The Comments is required." ToolTip="The Comments is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="PaddingTop5" colspan="2" align="center">
                    <asp:Button ID="btnSavePopUp" runat="server" Text="Save" OnClick="btnSavePopUp_Click" Width="100px" />
                    <asp:Button ID="btnCancelPopUp" runat="server" Text="Cancel" OnClick="btnCancelPopUp_Click" Width="100px"  />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField ID="hdnCopyCSAT" runat="server" Value="-1" />
    <asp:GridView ID="gvCSAT" runat="server" EmptyDataText="There is nothing to be displayed here"
        AutoGenerateColumns="False" OnRowDataBound="gvCSAT_RowDataBound" CssClass="CompPerfTable gvStrawmen tablesorter">
        <AlternatingRowStyle CssClass="alterrow" />
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="ie-bg">
                        &nbsp;
                    </div>
                </HeaderTemplate>
                <HeaderStyle CssClass="Width8Percent" />
                <ItemTemplate>
                    <asp:HiddenField ID="hdCSATId" runat="server" Value='<%# Eval("Id") %>' />
                    <asp:ImageButton ID="imgCopyCSAT" ToolTip="Copy CSAT" runat="server" OnClick="imgCopyCSAT_OnClick"
                        ImageUrl="~/Images/copy.png" />
                    &nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="imgEditCSAT" ToolTip="Edit CSAT" runat="server" OnClick="imgEditCSAT_OnClick"
                        ImageUrl="~/Images/icon-edit.png" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:HiddenField ID="hdCSATId" runat="server" Value='<%# Eval("Id") %>' />
                    <asp:ImageButton ID="imgUpdateCSAT" ToolTip="Save CSAT" runat="server" ImageUrl="~/Images/icon-check.png"
                        OnClick="imgUpdateCSAT_OnClick" />
                    <asp:ImageButton ID="imgCancel" ToolTip="Cancel" runat="server" ImageUrl="~/Images/no.png"
                        OnClick="imgCancel_OnClick" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="ie-bg">
                        Review Start Date</div>
                </HeaderTemplate>
                <HeaderStyle CssClass="Width15Percent" />
                <ItemTemplate>
                    <asp:LinkButton ID="btnReviewStartDate" runat="server" Text='<%# ((DateTime)Eval("ReviewStartDate")).ToString("MM/dd/yyyy") %>'
                        CommandArgument='<%# Eval("Id") %>' OnCommand="btnReviewStartDate_Command"></asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                    <span class="fl-left Width85Percent">
                        <uc2:DatePicker ID="dpReviewStartDate" ValidationGroup="CSATUpdate" runat="server"
                            TextBoxWidth="90%" AutoPostBack="false" />
                    </span><span class="Width15Percent vMiddle">
                        <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpReviewStartDate"
                            ValidationGroup="CSATUpdate" ErrorMessage="The Review Start Date is required."
                            ToolTip="The Start Date is required." Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Static"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="compStartDate" runat="server" ControlToValidate="dpReviewStartDate"
                            ValidationGroup="CSATUpdate" ErrorMessage="The Review Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            ToolTip="The Review Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                    </span>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="ie-bg">
                        Review End Date</div>
                </HeaderTemplate>
                <HeaderStyle CssClass="Width15Percent" />
                <ItemTemplate>
                    <%# ((DateTime)Eval("ReviewEndDate")).ToString("MM/dd/yyyy") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <span class="fl-left Width85Percent">
                        <uc2:DatePicker ID="dpReviewEndDate" ValidationGroup="CSATUpdate" runat="server"
                            TextBoxWidth="90%" AutoPostBack="false" />
                    </span><span class="Width15Percent vMiddle">
                        <asp:RequiredFieldValidator ID="reqEndDate" runat="server" ControlToValidate="dpReviewEndDate"
                            ValidationGroup="CSATUpdate" ErrorMessage="The Review End Date is required."
                            ToolTip="The End Date is required." Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Static"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpReviewEndDate"
                            ValidationGroup="CSATUpdate" ErrorMessage="The Review End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            ToolTip="The Review End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                    </span>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="ie-bg">
                        Completion Date</div>
                </HeaderTemplate>
                <HeaderStyle CssClass="Width15Percent" />
                <ItemTemplate>
                    <%# ((DateTime)Eval("CompletionDate")).ToString("MM/dd/yyyy")%>
                </ItemTemplate>
                <EditItemTemplate>
                    <span class="fl-left Width85Percent">
                        <uc2:DatePicker ID="dpCompletionDate" ValidationGroup="CSATUpdate" runat="server"
                            TextBoxWidth="90%" AutoPostBack="false" />
                    </span><span class="Width15Percent vMiddle">
                        <asp:RequiredFieldValidator ID="reqEndDate" runat="server" ControlToValidate="dpCompletionDate"
                            ValidationGroup="CSATUpdate" ErrorMessage="The Completion Date is required."
                            ToolTip="The Completion Date is required." Text="*" EnableClientScript="false"
                            SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpCompletionDate"
                            ValidationGroup="CSATUpdate" ErrorMessage="The Completion Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            ToolTip="The Completion Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                    </span>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle CssClass="Width39Percent CursorPointer" />
                <ItemStyle CssClass="Left" />
                <HeaderStyle CssClass="width30P" />
                <HeaderTemplate>
                    <div class="ie-bg">
                        Reviewer
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HiddenField ID="hdCSATReviewerId" runat="server" Value='<%# Eval("ReviewerId")%>' />
                    <asp:Label ID="lblCSATReviewerName" CssClass="Ws-Normal padLeft25" runat="server"
                        Text='<%# Eval("ReviewerName")%>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlReviewer" runat="server">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="reqReviewer" runat="server" ControlToValidate="ddlReviewer"
                        ValidationGroup="CSATUpdate" ErrorMessage="The Reviewer is required." ToolTip="The Reviewer is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle CssClass="Width13Percent CursorPointer" />
                <HeaderTemplate>
                    <div class="ie-bg">
                    Referral Score
                </HeaderTemplate>
                <HeaderStyle CssClass="Width15Percent" />
                <ItemTemplate>
                    <%# Eval("ReferralScore")%>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlScore" runat="server" CssClass="Width100Px">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="reqScore" runat="server" ControlToValidate="ddlScore"
                        ValidationGroup="CSATUpdate" ErrorMessage="The Referral Score is required." ToolTip="The Referral Score is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <div class="ie-bg">
                        &nbsp;</div>
                </HeaderTemplate>
                <HeaderStyle CssClass="Width7Percent" />
                <ItemTemplate>
                    <asp:ImageButton ID="imgDeleteCSAT" OnClientClick="return confirm('Do you really want to delete the CSAT?');"
                        ToolTip="Delete CSAT" runat="server" OnClick="imgDeleteCSAT_OnClick" ImageUrl="~/Images/icon-delete.png" />
                </ItemTemplate>
                <EditItemTemplate>
                    <div class="ie-bg">
                        &nbsp;
                    </div>
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>

