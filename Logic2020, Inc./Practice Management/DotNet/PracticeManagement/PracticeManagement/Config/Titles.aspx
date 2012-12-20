<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Titles.aspx.cs" Inherits="PraticeManagement.Config.Titles" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Salary Bands And PTO Accrual | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Salary Bands And PTO Accrual
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="upnlTitles" runat="server">
        <ContentTemplate>
            <asp:GridView ID="gvTitles" runat="server" AutoGenerateColumns="false" CssClass="CompPerfTable gvTitles"
                DataKeyNames="TitleId" OnRowDataBound="gvTitles_RowDataBound">
                <AlternatingRowStyle CssClass="alterrow" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width5Percent" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgEditTitle" ToolTip="Edit Salary Band And PTO Accrual" runat="server"
                                OnClick="imgEditTitle_OnClick" ImageUrl="~/Images/icon-edit.png" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:ImageButton ID="imgUpdateTitle" TitleId='<%# Eval("TitleId") %>' ToolTip="Save"
                                runat="server" ImageUrl="~/Images/icon-check.png" OnClick="imgUpdateTitle_OnClick" />
                            <asp:ImageButton ID="imgCancel" ToolTip="Cancel" runat="server" ImageUrl="~/Images/no.png"
                                OnClick="imgCancel_OnClick" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Title
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width20Percent" />
                        <ItemTemplate>
                            <asp:Label ID="lblTitleName" CssClass="WS-Normal padLeft5" runat="server" Text='<%# Eval("HtmlEncodedTitleName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditTitleName" runat="server" Text='<%# Eval("TitleName") %>'
                                CssClass="Width95Percent" OldName='<%# Eval("TitleName") %>' />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtTitleName" runat="server"
                                TargetControlID="tbEditTitleName" WatermarkCssClass="watermarkedtext Width95Percent"
                                WatermarkText="Please enter Title">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RequiredFieldValidator ID="reqTitleName" runat="server" ControlToValidate="tbEditTitleName"
                                ErrorMessage="Title is required." ToolTip="Title is required." SetFocusOnError="true"
                                Text="*" ValidationGroup="EditTitle"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="cvUniqueTitle" runat="server" ControlToValidate="tbEditTitleName"
                                ErrorMessage="Title with this name already exists. Please enter different Title name."
                                ToolTip="Title with this name already exists. Please enter different Title name."
                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="EditTitle" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Title Type
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width13Percent" />
                        <ItemTemplate>
                            <asp:Label ID="lblTitleType" CssClass="WS-Normal padLeft5" runat="server" Text='<%# Eval("TitleType.TitleTypeName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlTitleType" CssClass="Width70Per" runat="server">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Sort Order
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width13Percent" />
                        <ItemTemplate>
                            <asp:Label ID="lblSortOrder" CssClass="WS-Normal padLeft5" runat="server" Text='<%# Eval("SortOrder")%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditSortOrder" runat="server" Text='<%# Eval("SortOrder") %>'
                                CssClass="Width85Percent" OldSortOrder='<%# Eval("SortOrder") %>' />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSortOrder" runat="server"
                                TargetControlID="tbEditSortOrder" WatermarkCssClass="watermarkedtext Width85Percent"
                                WatermarkText="Please enter Sort Order">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RequiredFieldValidator ID="reqSortOrder" runat="server" ControlToValidate="tbEditSortOrder"
                                ErrorMessage="Sort Order is required." ToolTip="Sort Order is required." SetFocusOnError="true"
                                Text="*" ValidationGroup="EditTitle"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ControlToValidate="tbEditSortOrder" ID="valRegSortOrder"
                                runat="server" ErrorMessage="Sort Order value is not valid.Enter only positive integer."
                                ToolTip="Sort Order value is not valid.Enter only positive integer." Display="Dynamic"
                                Text="*" ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="EditTitle" />
                            <asp:CustomValidator ID="cvUniqueSortOrder" runat="server" ControlToValidate="tbEditSortOrder"
                                ErrorMessage="Sort Order with this value already exists. Please enter different Sort Order."
                                ToolTip="Sort Order with this value already exists. Please enter different Sort Order."
                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="EditTitle" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                PTO Accrual
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width13Percent" />
                        <ItemTemplate>
                            <asp:Label ID="lblPTOAccrual" CssClass="WS-Normal padLeft5" runat="server" Text='<%# Eval("PTOAccrual") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditPTOAccrual" runat="server" Text='<%# Eval("PTOAccrual") %>' MaxLength="2"
                                CssClass="Width85Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtPTOAccrual" runat="server"
                                TargetControlID="tbEditPTOAccrual" WatermarkCssClass="watermarkedtext Width85Percent"
                                WatermarkText="Please enter PTO Accrual">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RegularExpressionValidator ControlToValidate="tbEditPTOAccrual" ID="valRegPTOAccrual"
                                runat="server" ErrorMessage="PTO Accrual value is not valid. Enter only positive integer from 0 - 99."
                                ToolTip="PTO Accrual value is not valid.Enter only positive integer." Display="Dynamic"
                                Text="*" ValidationExpression="^[0-9]{1,2}$" SetFocusOnError="true" ValidationGroup="EditTitle" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Salary Minimum
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width17Percent" />
                        <ItemTemplate>
                            <asp:Label ID="lblMinimumSalary" CssClass="WS-Normal padLeft5" runat="server" Text='<%# GetCurrencyFormat((int?)Eval("MinimumSalary"))%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditMinimumSalary" runat="server" Text='<%# Eval("MinimumSalary") %>'
                                CssClass="Width85Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtMinimumSalary" runat="server"
                                TargetControlID="tbEditMinimumSalary" WatermarkCssClass="watermarkedtext Width85Percent"
                                WatermarkText="Please enter Salary Minimum">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RegularExpressionValidator ControlToValidate="tbEditMinimumSalary" ID="valRegMinimumSalary"
                                runat="server" ErrorMessage="Salary Minimum is not valid.Enter only digits."
                                ToolTip="Salary Minimum is not valid.Enter only digits." Display="Dynamic" Text="*"
                                ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="EditTitle" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Salary Maximum
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width17Percent" />
                        <ItemTemplate>
                            <asp:Label ID="lblMaximumSalary" CssClass="WS-Normal padLeft5" runat="server" Text='<%# GetCurrencyFormat((int?)Eval("MaximumSalary"))%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditMaximumSalary" runat="server" Text='<%# Eval("MaximumSalary") %>'
                                CssClass="Width85Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtMaximumSalary" runat="server"
                                TargetControlID="tbEditMaximumSalary" WatermarkCssClass="watermarkedtext Width85Percent"
                                WatermarkText="Please enter Salary Maximum">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RegularExpressionValidator ControlToValidate="tbEditMaximumSalary" ID="valRegMaximumSalary"
                                runat="server" ErrorMessage="Salary Maximum is not valid.Enter only digits."
                                ToolTip="Salary Maximum is not valid.Enter only digits." Display="Dynamic" Text="*"
                                ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="EditTitle" />
                            <asp:CompareValidator ID="cmpEditMaximumSalary" ControlToValidate="tbEditMaximumSalary"
                                ControlToCompare="tbEditMinimumSalary" runat="server" Operator="GreaterThan"
                                ErrorMessage="Salary Maximum should be greater than Minimum Salary." ToolTip="Salary Maximum should be greater than Minimum Salary."
                                Text="*" SetFocusOnError="true" ValidationGroup="EditTitle" Type="Integer"></asp:CompareValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width2Percent" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/icon-delete.png"
                                OnClick="imgDelete_OnClick" ToolTip="Delete Salary Band And PTO Accrual" TitleId='<%# Eval("TitleId") %>'
                                Visible='<%# IsDeleteButtonVisible((bool)Eval("InUse")) %>' />
                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgDelete" runat="server" TargetControlID="imgDelete"
                                ConfirmText="Are you sure. Do you want to delete the Salary Band And PTO Accrual?">
                            </AjaxControlToolkit:ConfirmButtonExtender>
                        </ItemTemplate>
                        <EditItemTemplate>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Panel ID="pnlInsertTitle" runat="server">
                <table class="CompPerfTable gvTitles" cellspacing="0">
                    <tr class="alterrow">
                        <td class="Width5Percent">
                            <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                                ToolTip="Add Salary Band and PTO Accrual." Visible="true" />
                            <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
                                ToolTip="Save" Visible="false" OnClick="btnInsert_Click" />
                            <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
                                ToolTip="Cancel" Visible="false" />
                        </td>
                        <td class="Width20Percent">
                            <asp:TextBox ID="tbInsertTitleName" runat="server" Enabled="false" CssClass="Width95Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtInsertTitleName" runat="server"
                                TargetControlID="tbInsertTitleName" WatermarkCssClass="watermarkedtext Width95Percent"
                                WatermarkText="Please enter Title">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RequiredFieldValidator ID="reqTitleName" runat="server" ControlToValidate="tbInsertTitleName"
                                ErrorMessage="Title is required." ToolTip="Title is required." SetFocusOnError="true"
                                Text="*" ValidationGroup="AddTitle"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="cvInsertUniqueTitle" runat="server" ControlToValidate="tbInsertTitleName"
                                ErrorMessage="Title with this name already exists. Please enter different Title name."
                                ToolTip="Title with this name already exists. Please enter different Title name."
                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="AddTitle" />
                        </td>
                        <td class="Width13Percent">
                            <asp:DropDownList ID="ddlInsertTitleType" CssClass="Width70Per" Enabled="false" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td class="Width13Percent">
                            <asp:TextBox ID="tbInsertSortOrder" runat="server" Enabled="false" CssClass="Width85Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkInsertSortOrder"
                                runat="server" TargetControlID="tbInsertSortOrder" WatermarkCssClass="watermarkedtext Width85Percent"
                                WatermarkText="Please enter Sort Order">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RequiredFieldValidator ID="regSortOrder" runat="server" ControlToValidate="tbInsertSortOrder"
                                ErrorMessage="Sort Order is required." ToolTip="Sort Order is required." SetFocusOnError="true"
                                Text="*" ValidationGroup="AddTitle"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ControlToValidate="tbInsertSortOrder" ID="valRegSortOrder"
                                runat="server" ErrorMessage="Sort Order value is not valid.Enter only positive integer."
                                ToolTip="Sort Order value is not valid.Enter only positive integer." Display="Dynamic"
                                Text="*" ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="AddTitle" />
                            <asp:CustomValidator ID="cvInsertUniqueSortOrder" runat="server" ControlToValidate="tbInsertSortOrder"
                                ErrorMessage="Sort Order with this value already exists. Please enter different Sort Order."
                                ToolTip="Sort Order with this value already exists. Please enter different Sort Order."
                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="AddTitle" />
                        </td>
                        <td class="Width13Percent">
                            <asp:TextBox ID="tbInsertPTOAccrual" runat="server" Enabled="false" CssClass="Width85Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtInsertPTOAccrual" runat="server"
                                TargetControlID="tbInsertPTOAccrual" WatermarkCssClass="watermarkedtext Width85Percent"
                                WatermarkText="Please enter PTO Accrual">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RegularExpressionValidator ControlToValidate="tbInsertPTOAccrual" ID="valRegPTOAccrual"
                                runat="server" ErrorMessage="PTO Accrual value is not valid. Enter only positive integer from 0 - 99."
                                ToolTip="PTO Accrual value is not valid. Enter only positive integer from 0 - 99." Display="Dynamic"
                                Text="*" ValidationExpression="^[0-9]{1,2}$" SetFocusOnError="true" ValidationGroup="AddTitle" />
                        </td>
                        <td class="Width17Percent">
                            <asp:TextBox ID="tbInsertMinimumSalary" runat="server" Enabled="false" CssClass="Width85Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtInsertMinimumSalary"
                                runat="server" TargetControlID="tbInsertMinimumSalary" WatermarkCssClass="watermarkedtext Width85Percent"
                                WatermarkText="Please enter Salary Minimum">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RegularExpressionValidator ControlToValidate="tbInsertMinimumSalary" ID="valRegMinimumSalary"
                                runat="server" ErrorMessage="Salary Minimum is not valid.Enter only digits."
                                ToolTip="Salary Minimum is not valid.Enter only digits." Display="Dynamic" Text="*"
                                ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="AddTitle" />
                        </td>
                        <td class="Width17Percent">
                            <asp:TextBox ID="tbInsertMaximumSalary" runat="server" Enabled="false" CssClass="Width85Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtInsertMaximumSalary"
                                runat="server" TargetControlID="tbInsertMaximumSalary" WatermarkCssClass="watermarkedtext Width85Percent"
                                WatermarkText="Please enter Salary Maximum">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RegularExpressionValidator ControlToValidate="tbInsertMaximumSalary" ID="valRegMaximumSalary"
                                runat="server" ErrorMessage="Salary Maximum is not valid.Enter only digits."
                                ToolTip="Salary Maximum is not valid.Enter only digits." Display="Dynamic" Text="*"
                                ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="AddTitle" />
                            <asp:CompareValidator ID="cmpEditMaximumSalary" ControlToValidate="tbInsertMaximumSalary"
                                ControlToCompare="tbInsertMinimumSalary" runat="server" Operator="GreaterThan"
                                ErrorMessage="Salary Maximum should be greater than Minimum Salary." ToolTip="Salary Maximum should be greater than Minimum Salary."
                                Text="*" SetFocusOnError="true" ValidationGroup="AddTitle" Type="Integer"></asp:CompareValidator>
                        </td>
                        <td class="Width2Percent">
                            &nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                CancelControlID="btnCancelErrorPanel" DropShadow="false" />
            <asp:Panel ID="pnlErrorPanel" runat="server" Style="display: none;" CssClass="ProjectDetailErrorPanel PanelPerson">
                <table class="Width100Per">
                    <tr>
                        <th class="bgcolorGray TextAlignCenterImp vBottom">
                            <b class="BtnClose">Attention!</b>
                            <asp:Button ID="btnCancelErrorPanel" runat="server" CssClass="mini-report-close floatright"
                                ToolTip="Cancel" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10Px">
                            <asp:ValidationSummary ID="valSummaryAddTitle" runat="server" ValidationGroup="AddTitle"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a Salary Brand and PTO Accrual." />
                            <asp:ValidationSummary ID="valSummaryEditTitle" runat="server" ValidationGroup="EditTitle"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a practice capability." />
                            <uc:Label ID="mlInsertStatus" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10Px TextAlignCenterImp">
                            <asp:Button ID="btnOKErrorPanel" runat="server" Text="OK" Width="100" OnClientClick="$find('mpeErrorPanelBehaviourId').hide();return false;" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

