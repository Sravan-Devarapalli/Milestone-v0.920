<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Strawmen.aspx.cs" Inherits="PraticeManagement.Config.Strawmen" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="DataTransferObjects" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Strawmen | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Strawmen List
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript" language="javascript">

        function EnableDisableVacationDays(ddlBasic) {
            var vacationdaysId = ddlBasic.getAttribute("vacationdaysId");
            var vacationdays = document.getElementById(vacationdaysId);
            if (!(ddlBasic.value == 'W2-Salary' || ddlBasic.value == 'W2-Hourly')) {
                vacationdays.setAttribute("disabled", "disabled");
            }
            else {
                vacationdays.removeAttribute("disabled");
            }
            return false;
        }
    </script>
    <uc:LoadingProgress ID="loadingProgress" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <div class="tab-pane">
                <table class="WholeWidth">
                    <tr>
                        <td class="textRight Padding10">
                            <asp:ShadowedHyperlink runat="server" Text="Add Strawman" ID="StbAddStrawman" CssClass="add-btn"
                                NavigateUrl="~/StrawManDetails.aspx?returnTo=Config/Strawmen.aspx" />
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="gvStrawmen" runat="server" EmptyDataText="There is nothing to be displayed here"
                    AutoGenerateColumns="False" OnRowDataBound="gvStrawmen_RowDataBound" CssClass="CompPerfTable gvStrawmen">
                    <AlternatingRowStyle CssClass="alterrow" />
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    &nbsp;
                                </div>
                            </HeaderTemplate>
                            <HeaderStyle CssClass="Width7Percent" />
                            <ItemTemplate>
                                <asp:ImageButton ID="imgCompersationStrawman" strawmanId='<%# Eval("Id") %>' ToolTip="Click to open Compersation History"
                                    runat="server" OnClick="imgCompersationStrawman_OnClick" ImageUrl="~/Images/Zoom-In-icon.png" />
                                &nbsp;&nbsp;&nbsp;
                                <asp:ImageButton ID="imgCopyStrawman" strawmanId='<%# Eval("Id") %>' ToolTip="Copy Strawman"
                                    runat="server" OnClick="imgCopyStrawman_OnClick" ImageUrl="~/Images/copy.png" />
                                &nbsp;&nbsp;&nbsp;
                                <asp:ImageButton ID="imgEditStrawman" strawmanId='<%# Eval("Id") %>' ToolTip="Edit Strawman"
                                    runat="server" OnClick="imgEditStrawman_OnClick" ImageUrl="~/Images/icon-edit.png" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgUpdateStrawman" strawmanId='<%# Eval("Id") %>' ToolTip="Save Strawman"
                                    runat="server" ImageUrl="~/Images/icon-check.png" OnClick="imgUpdateStrawman_OnClick" />
                                <asp:ImageButton ID="imgCancel" ToolTip="Cancel" runat="server" ImageUrl="~/Images/no.png"
                                    OnClick="imgCancel_OnClick" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Id" Visible="false">
                            <ItemStyle CssClass="Width0Percent" />
                            <ItemTemplate>
                                <asp:Label ID="lblId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderStyle CssClass="Width39Percent" />
                            <ItemStyle CssClass="Left" />
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Name</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblStrawmen" CssClass="Ws-Normal padLeft25" runat="server" Text='<%# Eval("Name")%>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkEditStrawman" strawmanId='<%# Eval("Id") %>' ToolTip="Click to edit Strawmen Role/Skill"
                                    CssClass="padLeft25" runat="server" OnClick="lnkEditStrawman_OnClick" Text='<%# Eval("Name")%>' />
                                <asp:CustomValidator ID="cvDupliacteName" runat="server" Text="*" ErrorMessage="There is another strawman with the same skill and role."
                                    ToolTip="There is another strawmen with the same skill and role." ValidationGroup="StrawmanGroup"
                                    SetFocusOnError="true" OnServerValidate="cvDupliacteName_ServerValidate"></asp:CustomValidator>
                                <asp:HiddenField ID="hdnLastName" runat="server" Value='<%# Eval("LastName")%>' />
                                <asp:HiddenField ID="hdnFirstName" runat="server" Value='<%# Eval("FirstName")%>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderStyle CssClass="Width13Percent" />
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Basis</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblBasic" runat="server" Text='<%# ((Pay)Eval("CurrentPay")) != null ? ((Pay)Eval("CurrentPay")).TimescaleName : string.Empty %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlBasic" runat="server" CssClass="Width100Px" onchange="return EnableDisableVacationDays(this);">
                                    <asp:ListItem Text="W2-Salary" Value="W2-Salary"></asp:ListItem>
                                    <asp:ListItem Text="W2-Hourly" Value="W2-Hourly"></asp:ListItem>
                                    <asp:ListItem Text="1099/Hourly" Value="1099/Hourly"></asp:ListItem>
                                    <asp:ListItem Text="1099/POR" Value="1099/POR"></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemStyle CssClass="Right" />
                            <HeaderStyle CssClass="Width13Percent" />
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Amount</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAmount" runat="server" Text='<%# ((Pay)Eval("CurrentPay")) != null ? ((Pay)Eval("CurrentPay")).Amount : 0 %>'
                                    CssClass="padRight25"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAmount" runat="server" Text='<%# ((Pay)Eval("CurrentPay")) != null ? ((Pay)Eval("CurrentPay")).Amount : 0%>'
                                    CssClass="Width100Px textRight"></asp:TextBox>
                                <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtAmount" runat="server" TargetControlID="txtAmount"
                                    FilterMode="ValidChars" FilterType="Numbers,Custom" ValidChars=".">
                                </AjaxControlToolkit:FilteredTextBoxExtender>
                                <asp:RequiredFieldValidator ID="rqfvAmount" runat="server" Text="*" ErrorMessage="Amount is required."
                                    ControlToValidate="txtAmount" ToolTip="Amount is required." SetFocusOnError="true"
                                    ValidationGroup="StrawmanGroup"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderStyle CssClass="Width13Percent" />
                            <ItemStyle CssClass="Right" />
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Vacation(In Hours)</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblVacationDays" runat="server" Text='<%# GetVacationDays((Pay)Eval("CurrentPay")) %>'
                                    CssClass="padRight25"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtVacationDays" runat="server" Text='<%# GetVacationDays((Pay)Eval("CurrentPay")) %>'
                                    CssClass="textRight Width100Px"></asp:TextBox>
                                <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtVacationDays" runat="server"
                                    TargetControlID="txtVacationDays" FilterMode="ValidChars" FilterType="Numbers">
                                </AjaxControlToolkit:FilteredTextBoxExtender>
                                <asp:CustomValidator ID="cvVacationDays" runat="server" Text="*" ErrorMessage="VacationDays(In Hours) must be in multiple of 8."
                                    ToolTip="VacationDays(In Hours) must be in multiple of 8." ValidationGroup="StrawmanGroup"
                                    SetFocusOnError="true" OnServerValidate="cvVacationDays_ServerValidate"></asp:CustomValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Active</div>
                            </HeaderTemplate>
                            <HeaderStyle CssClass="Width10Percent" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chbIsActive" runat="server" Enabled="false" Checked='<%# ((PersonStatus)Eval("Status")).ToStatusType() == PersonStatusType.Active ? true:false %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chbIsActiveEd" runat="server" Checked='<%# ((PersonStatus)Eval("Status")).ToStatusType()== PersonStatusType.Active ? true:false  %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    &nbsp;</div>
                            </HeaderTemplate>
                            <HeaderStyle CssClass="Width5Percent" />
                            <ItemTemplate>
                                <asp:ImageButton ID="imgDeleteStrawman" strawmanId='<%# Eval("Id") %>' InUse='<%# Eval("InUse") %>'
                                    OnClientClick="return confirm('Do you really want to delete the strawman?');"
                                    ToolTip="Delete Strawman" runat="server" OnClick="imgDeleteStrawman_OnClick"
                                    ImageUrl="~/Images/icon-delete.png" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <div class="ie-bg">
                                    &nbsp;
                                </div>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <table class="WholeWidth">
                    <tr>
                        <td class="Padding10">
                        </td>
                    </tr>
                </table>
            </div>
            <asp:HiddenField ID="hdnCopyStrawman" runat="server" />
            <asp:HiddenField ID="hdnEditStrawman" runat="server" />
            <asp:HiddenField ID="hdnTargetValidationPanel" runat="server" />
            <asp:HiddenField ID="hdnCompensation" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeEditStrawmanPopup" runat="server" TargetControlID="hdnEditStrawman"
                BackgroundCssClass="modalBackground" PopupControlID="pnlPopup" DropShadow="false" />
            <asp:Panel ID="pnlPopup" runat="server" CssClass="popUp EditStrawmenPopUp" Style="display: none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th colspan="3">
                            Edit Strawmen
                            <asp:Button ID="btnClose" runat="server" CssClass="mini-report-closeNew" ToolTip="Cancel Changes"
                                OnClick="btnCancel_OnClick" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="padRight10 PaddingTop10 textRight">
                            <asp:Label ID="lblastName" runat="server" Text="Role"></asp:Label>
                        </td>
                        <td class="PaddingTop10">
                            <asp:TextBox ID="tbLastName" MaxLength="50" runat="server" CssClass="Width180Px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rqfvLastName" runat="server" Text="*" ErrorMessage="Role is required."
                                ControlToValidate="tbLastName" ToolTip="Role is required." SetFocusOnError="true"
                                ValidationGroup="StrawmanNameGroup"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="padRight10 PaddingTop10 textRight">
                            <asp:Label ID="lbfirstName" runat="server" Text="Skill"></asp:Label>
                        </td>
                        <td class="PaddingTop10">
                            <asp:TextBox ID="tbFirstName" runat="server" MaxLength="50" CssClass="Width180Px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rqfvFirstName" runat="server" Text="*" ErrorMessage="Skill is required."
                                ControlToValidate="tbFirstName" ToolTip="Skill is required." SetFocusOnError="true"
                                ValidationGroup="StrawmanNameGroup"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="PaddingTop20 padRight10 textRight">
                            <asp:Button ID="btnOk" runat="server" Text="OK" ValidationGroup="StrawmanNameGroup"
                                OnClick="btnOK_OnClick" CssClass="Width60Px" />
                            <asp:Button ID="btncancel" runat="server" Text="Cancel" OnClick="btnCancel_OnClick"
                                CssClass="Width60Px" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="padLeft10 PaddingBottom2">
                            <asp:ValidationSummary ID="valSummary" runat="server" ValidationGroup="StrawmanNameGroup" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeValidationPanel" runat="server" BehaviorID="mpeValidationPanelBehaviourId"
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
                            <asp:ValidationSummary ID="vsStrawmenSummary" runat="server" ValidationGroup="StrawmanGroup" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10 textCenter">
                            <asp:Button ID="btnOKValidationPanel" runat="server" ToolTip="OK" Text="OK" CssClass="Width100Px" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCompensation" runat="server" BehaviorID="mpeCompensation"
                TargetControlID="hdnCompensation" BackgroundCssClass="modalBackground" PopupControlID="pnlCompensation"
                OkControlID="btnOkCompersation" CancelControlID="btnOkCompersation" DropShadow="false" />
            <asp:Panel ID="pnlCompensation" runat="server" CssClass="popUp StrawmenCompensationPopUp"
                Style="display: none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th>
                            Compensation History
                        </th>
                    </tr>
                    <tr>
                        <td class="bg-light-frame">
                            <asp:Label ID="lblStrawmanName" runat="server" CssClass="StrawmanName"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="bg-light-frame strawmenPageTd">
                            <asp:GridView ID="gvCompensationHistory" runat="server" AutoGenerateColumns="False"
                                EmptyDataText="No compensation history for this person." CssClass="CompPerfTable gvCompensationHistoryStrawmen">
                                
                                <AlternatingRowStyle CssClass="alterrow" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime?)Eval("StartDate")).HasValue ? ((DateTime?)Eval("StartDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Start</div>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <%# ((DateTime?)Eval("EndDate")).HasValue ? ((DateTime?)Eval("EndDate")).Value.AddDays(-1).ToString("MM/dd/yyyy") : string.Empty %></ItemTemplate>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                End</div>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <%# Eval("TimescaleName") %></ItemTemplate>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Basis</div>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Amount</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <%# Eval("Amount") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Vacation(In Hours)</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <%# ((int?)Eval("VacationDays")).HasValue ? (((int?)Eval("VacationDays")).Value * 8).ToString() : string.Empty %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                &nbsp;
                                            </div>
                                        </HeaderTemplate>
                                        <HeaderStyle CssClass="width5" />
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imgCompensationDelete" ToolTip="Delete" runat="server" OnClick="imgCompensationDelete_OnClick"
                                                strawmanId='<%#(Eval("PersonId")) %>' ImageUrl="~/Images/cross_icon.png" />
                                            <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" ConfirmText="Are you sure you want to delete this compensation record?"
                                                runat="server" TargetControlID="imgCompensationDelete">
                                            </ajaxToolkit:ConfirmButtonExtender>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10 textCenter">
                            <asp:Button ID="btnOkCompersation" runat="server" ToolTip="OK" Text="OK" CssClass="Width100Px" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

