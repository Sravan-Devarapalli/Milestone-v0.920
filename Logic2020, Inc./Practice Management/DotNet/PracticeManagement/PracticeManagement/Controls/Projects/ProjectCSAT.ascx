<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectCSAT.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectCSAT" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Panel ID="pnlTabCSAT" runat="server" CssClass="tab-pane">
    <div class="PaddingBottom35Px">
        <asp:ShadowedTextButton ID="stbCSAT" runat="server" CausesValidation="false" OnClick="stbCSAT_Click"
            OnClientClick="if(!ConfirmSaveOrExit()) return false;" CssClass="add-btn" Text="Add CSAT" />
    </div>
    <asp:HiddenField ID="hdnSelectedCSATId" runat="server" Value="-1" />
    <asp:GridView ID="gvCSAT" runat="server" EmptyDataText="There are no CSAT scores captured for this project."
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
                    <asp:ImageButton ID="imgEditCSAT" ToolTip="Edit CSAT" runat="server" OnClick="imgEditCSAT_OnClick"
                        OnClientClick="if(!ConfirmSaveOrExit()) return false;" ImageUrl="~/Images/icon-edit.png" />
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
                <ItemStyle CssClass="textCenter" />
                <ItemTemplate>
<%--                    <asp:LinkButton ID="btnReviewStartDate" runat="server" Text='<%# ((DateTime)Eval("ReviewStartDate")).ToString("MM/dd/yyyy") %>'
                        OnClientClick="if(!ConfirmSaveOrExit()) return false;" CommandArgument='<%# Eval("Id") %>'
                        OnCommand="btnReviewStartDate_Command"></asp:LinkButton>--%>

                           <asp:HyperLink ID="hlReviewStartDate" runat="server" NavigateUrl='<%# GetProjectCSATDetailRedirectUrl(((int)Eval("Id"))) %>'
                    Text='<%# ((DateTime)Eval("ReviewStartDate")).ToString("MM/dd/yyyy") %>'
                    onclick='<%# "return checkDirty(\"" + ProjectCSAT_TARGET + "\", " + Eval("Id") + ")" %>' />

                    <asp:Label ID="lblReviewStartDate" runat="server" Text='<%# ((DateTime)Eval("ReviewStartDate")).ToString("MM/dd/yyyy") %>'></asp:Label>
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
                <ItemStyle CssClass="textCenter" />
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
                        <asp:CompareValidator ID="compEndDateGreater" runat="server" ControlToValidate="dpReviewEndDate"
                            ControlToCompare="dpReviewStartDate" ErrorMessage="The Review Period End Date must be greater than or equal to Review Period Start Date."
                            ToolTip="The Review Period End Date must be greater than or equal to Review Period Start Date."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            Operator="GreaterThanEqual" Type="Date" ValidationGroup="CSATUpdate"></asp:CompareValidator>
                        <asp:CustomValidator ID="custCSATEndDateInGridView" runat="server" ControlToValidate="dpReviewEndDate"
                            ErrorMessage="The Review End Date can not be greater than the date on which project status was set to 'Completed'."
                            ToolTip="The Review End Date can not be greater than the date on which project status was set to 'Completed'."
                            ValidationGroup="CSATUpdate" Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Dynamic" OnServerValidate="custCSATEndDateInGridView_ServerValidate"></asp:CustomValidator>
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
                <ItemStyle CssClass="textCenter" />
                <EditItemTemplate>
                    <span class="fl-left Width85Percent">
                        <uc2:DatePicker ID="dpCompletionDate" ValidationGroup="CSATUpdate" runat="server"
                            TextBoxWidth="90%" AutoPostBack="false" />
                    </span><span class="Width15Percent vMiddle">
                        <asp:RequiredFieldValidator ID="reqCompletionDate" runat="server" ControlToValidate="dpCompletionDate"
                            ValidationGroup="CSATUpdate" ErrorMessage="The Completion Date is required."
                            ToolTip="The Completion Date is required." Text="*" EnableClientScript="false"
                            SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="compCompletionDate" runat="server" ControlToValidate="dpCompletionDate"
                            ValidationGroup="CSATUpdate" ErrorMessage="The Completion Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            ToolTip="The Completion Date has an incorrect format. It must be 'MM/dd/yyyy'."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                        <asp:CustomValidator ID="custCSATCompleteDateInGridView" runat="server" ValidationGroup="CSATUpdate"
                            ErrorMessage="The CSAT Completion Date must be less than or equal to current date."
                            ToolTip="The CSAT Completion Date must be less than or equal to current date."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            OnServerValidate="custCSATCompleteDateInGridView_ServerValidate"></asp:CustomValidator>
                    </span>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle CssClass="Width39Percent CursorPointer" />
                <ItemStyle CssClass="textCenter" />
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
                    <asp:DropDownList ID="ddlReviewer" runat="server" OnSelectedIndexChanged="ddlReviewer_SelectedIndexChanged" AutoPostBack="true" Width="250px">
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
                <ItemStyle CssClass="textCenter" />
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
<asp:ValidationSummary ID="valSumCSATUpdate" runat="server" ValidationGroup="CSATUpdate" />

