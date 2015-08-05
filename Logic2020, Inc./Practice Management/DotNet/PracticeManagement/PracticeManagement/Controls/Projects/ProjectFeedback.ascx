<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectFeedback.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectFeedback" %>
<script type="text/javascript">
</script>
<asp:UpdatePanel ID="upnlBody" runat="server">
    <ContentTemplate>
        <table class="WholeWidth textRight"><tr><td class="PaddingBottom5Imp">* Refers to gap in feedback review period</td></tr></table>
        <asp:Repeater ID="repFeedback" runat="server" OnItemDataBound="repFeedback_ItemDataBound">
            <HeaderTemplate>
                <div>
                    <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport WholeWidth zebra">
                        <thead>
                            <tr>
                                <th class="">
                                </th>
                                <th class="">
                                    Resource
                                </th>
                                <th class="">
                                    Title
                                </th>
                                <th class="">
                                    Review Period Start Date
                                </th>
                                <th class="">
                                    Review Period End Date
                                </th>
                                <th class="">
                                    Feedback Due Date
                                </th>
                                <th class="">
                                    Status
                                </th>
                                <th class="">
                                    Completion Certification
                                </th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="ReportItemTemplate">
                    <td>
                        <asp:ImageButton ID="imgCancel" runat="server" ToolTip="Cancel" ImageUrl="~/Images/icon-delete.png"
                            FeedbackId='<%# Eval("Id")%>' OnClick="imgCancel_Click" CausesValidation="false" />
                    </td>
                    <td class="padLeft5 textLeft">
                        <%# Eval("Person.HtmlEncodedName")%>
                    </td>
                    <td>
                        <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                    </td>
                    <td>
                        <%# ((DateTime)Eval("ReviewStartDate")).ToString("MM/dd/yyyy") %>
                    </td>
                    <td>
                        <%# ((DateTime)Eval("ReviewEndDate")).ToString("MM/dd/yyyy")%>
                        <asp:Label ID="lblAsterik" runat="server" Text="*"></asp:Label>
                    </td>
                    <td>
                        <%# ((DateTime)Eval("DueDate")).ToString("MM/dd/yyyy")%>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"
                            CausesValidation="false">
                        </asp:DropDownList>
                        <asp:LinkButton ID="lnkCanceled" runat="server" FeedbackId='<%# Eval("Id")%>' FeedbackReason='<%# Eval("CancelationReason")%>'
                            Text="Canceled" OnClick="lnkCancel_Click"></asp:LinkButton>
                    </td>
                    <td>
                        <asp:Label ID="lblCompletedCertification" runat="server" FeedbackId='<%# Eval("Id")%>'></asp:Label>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody></table></div>
            </FooterTemplate>
        </asp:Repeater>
        <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
            There are no milestone entries which are more than 240 hours.
        </div>
        <asp:HiddenField ID="hdnPopup" runat="server" />
        <AjaxControlToolkit:ModalPopupExtender ID="modalEx" runat="server" TargetControlID="hdnPopup"
            PopupControlID="pnlCacelationReason" DropShadow="true" BackgroundCssClass="modalBackground"
            CancelControlID="btnCancel" />
        <asp:Panel ID="pnlCacelationReason" runat="server" Style="display: none; width: 420px;"
            CssClass="pnlTimeEntryCss">
            <table>
                <tr>
                    <td colspan="2" class="te-modal-header alignCenter">
                        Cancelation Reason
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="colorWhite">
                        Please state the reason for canceling this individual’s project review for this
                        time period:
                        <asp:TextBox ID="tbNotes" runat="server" Columns="50" MaxLength="1000" Rows="5" TextMode="MultiLine"
                            TabIndex="1" CssClass="Width99Percent" />
                        <asp:RequiredFieldValidator ID="reqNotes" runat="server" ControlToValidate="tbNotes"
                            ValidationGroup="Feedback" Text="*" ToolTip="You should enter the reason for the cancelation."></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:ValidationSummary ID="vsumFeedback" runat="server" ValidationGroup="Feedback" />
                    </td>
                </tr>
                <tr class="WholeWidth textCenter">
                    <td class="Width50Percent">
                        <asp:Button ID="btnOk" runat="server" OnClick="btnOk_Click" Text="Ok" />
                    </td>
                    <td class="Width50Percent">
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

