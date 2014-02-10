<%@ Page Language="C#" Title="Targeted Company Field Customization" AutoEventWireup="true"
    MasterPageFile="~/PracticeManagementMain.Master" CodeBehind="TargetedCompanyRecruitingMetrics.aspx.cs"
    Inherits="PraticeManagement.TargetedCompanyRecruitingMetrics" %>

<%@ Register Src="~/Controls/Configuration/RecruitingMetricsHeader.ascx" TagPrefix="uc"
    TagName="RecruitingMetricsHeader" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="body" runat="server">
    <uc:RecruitingMetricsHeader ID="recruitingMetricsHeader" runat="server"></uc:RecruitingMetricsHeader>
    <div style="padding-top: 20px;">
        <asp:UpdatePanel ID="upnlTargetedCompanyRecruitingMetrics" runat="server">
            <ContentTemplate>
                <table class="WholeWidth">
                    <tr>
                        <td class="Width50Percent">
                        </td>
                        <td>
                            <asp:GridView ID="gvTargetedCompanyRecruitingMetrics" runat="server" AutoGenerateColumns="false"
                                OnRowDataBound="gvTargetedCompanyRecruitingMetrics_RowDataBound" CssClass="CompPerfTable gvTitles Width80PercentImp"
                                EmptyDataText="nothing" DataKeyNames="RecruitingMetricsId">
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
                                            <asp:ImageButton ID="imgEditRecruitMetrics" ToolTip="Edit Targeted Company Recruiting Metrics"
                                                OnClick="imgEditTargetedCompanyMetrics_OnClick" runat="server" ImageUrl="~/Images/icon-edit.png" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:ImageButton ID="imgRecruitMetricsUpdate" ToolTip="Save" runat="server" ImageUrl="~/Images/icon-check.png"
                                                TargetedCompanyId='<%# Eval("RecruitingMetricsId") %>' OnClick="imgUpdateTargetedCompanyMetrics_OnClick" />
                                            <asp:ImageButton ID="imgCancel" ToolTip="Cancel" runat="server" ImageUrl="~/Images/no.png"
                                                OnClick="imgCancel_OnClick" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Targeted Company Name
                                            </div>
                                        </HeaderTemplate>
                                        <HeaderStyle CssClass="Width20Percent" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblTargetedCompanyName" CssClass="WS-Normal padLeft5" runat="server"
                                                Text='<%# Eval("HtmlEncodedName") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="tbEditTargetedCompanyName" runat="server" Text='<%# Eval("Name") %>'
                                                CssClass="Width95Percent" OldName='<%# Eval("Name") %>' />
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtTitleName" runat="server"
                                                TargetControlID="tbEditTargetedCompanyName" WatermarkCssClass="watermarkedtext Width95Percent"
                                                WatermarkText="Please enter Targeted Company name">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:RequiredFieldValidator ID="reqTargetedCompanyName" runat="server" ControlToValidate="tbEditTargetedCompanyName"
                                                ErrorMessage="Targeted Company Name is required." ToolTip="Targeted Company Name is required."
                                                SetFocusOnError="true" Text="*" ValidationGroup="EditTargetedCompany"></asp:RequiredFieldValidator>
                                            <asp:CustomValidator ID="cvUniqueTargetedCompany" runat="server" ControlToValidate="tbEditTargetedCompanyName"
                                                ErrorMessage="Targeted Company with this name already exists. Please enter different Targeted Company name."
                                                ToolTip="Targeted Company with this name already exists. Please enter different Targeted Company name."
                                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="EditTargetedCompany" />
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
                                                Text="*" ValidationGroup="EditTargetedCompany"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ControlToValidate="tbEditSortOrder" ID="valRegSortOrder"
                                                runat="server" ErrorMessage="Sort Order value is not valid.Enter only positive integer."
                                                ToolTip="Sort Order value is not valid.Enter only positive integer." Display="Dynamic"
                                                Text="*" ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="EditTargetedCompany" />
                                            <asp:CustomValidator ID="cvUniqueSortOrder" runat="server" ControlToValidate="tbEditSortOrder"
                                                ErrorMessage="Sort Order with this value already exists. Please enter different Sort Order."
                                                ToolTip="Sort Order with this value already exists. Please enter different Sort Order."
                                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="EditTargetedCompany" />
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
                                                TargetedCompanyId='<%# Eval("RecruitingMetricsId") %>' OnClick="imgDelete_OnClick"
                                                ToolTip="Delete Targeted Company Recruiting Metrics" Visible='<%# IsDeleteButtonVisible((bool)Eval("InUse")) %>' />
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgDelete" runat="server" TargetControlID="imgDelete"
                                                ConfirmText="Are you sure. Do you want to delete the Targeted Company?">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Panel ID="pnlInsertTargetedCompany" runat="server">
                                <table class="CompPerfTable gvTitles Width80PercentImp" cellspacing="0">
                                    <tr class="alterrow">
                                        <td class="Width5Percent">
                                            <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                                                ToolTip="Add Targeted Company Recruiting Metrics." Visible="true" />
                                            <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
                                                ToolTip="Save" Visible="false" OnClick="btnInsert_Click" />
                                            <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
                                                ToolTip="Cancel" Visible="false" />
                                        </td>
                                        <td class="Width20Percent">
                                            <asp:TextBox ID="tbInsertTargetedCompany" runat="server" Enabled="false" CssClass="Width95Percent" />
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtInsertTitleName" runat="server"
                                                TargetControlID="tbInsertTargetedCompany" WatermarkCssClass="watermarkedtext Width95Percent"
                                                WatermarkText="Please enter Targeted Company name">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:RequiredFieldValidator ID="reqTargetedCompany" runat="server" ControlToValidate="tbInsertTargetedCompany"
                                                ErrorMessage="Targeted Company Name is required." ToolTip="Targeted Company Name is required."
                                                SetFocusOnError="true" Text="*" ValidationGroup="AddTargetedCompany"></asp:RequiredFieldValidator>
                                            <asp:CustomValidator ID="cvInsertUniqueTargetedCompany" runat="server" ControlToValidate="tbInsertTargetedCompany"
                                                ErrorMessage="Targeted Company with this name already exists. Please enter different Targeted Company name."
                                                ToolTip="Targeted Company with this name already exists. Please enter different Targeted Company name."
                                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="AddTargetedCompany" />
                                        </td>
                                        <td class="Width13Percent">
                                            <asp:TextBox ID="tbInsertSortOrder" runat="server" Enabled="false" CssClass="Width85Percent" />
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkInsertSortOrder"
                                                runat="server" TargetControlID="tbInsertSortOrder" WatermarkCssClass="watermarkedtext Width85Percent"
                                                WatermarkText="Please enter Sort Order">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:RequiredFieldValidator ID="regSortOrder" runat="server" ControlToValidate="tbInsertSortOrder"
                                                ErrorMessage="Sort Order is required." ToolTip="Sort Order is required." SetFocusOnError="true"
                                                Text="*" ValidationGroup="AddTargetedCompany"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ControlToValidate="tbInsertSortOrder" ID="valRegSortOrder"
                                                runat="server" ErrorMessage="Sort Order value is not valid.Enter only positive integer."
                                                ToolTip="Sort Order value is not valid.Enter only positive integer." Display="Dynamic"
                                                Text="*" ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="AddTargetedCompany" />
                                            <asp:CustomValidator ID="cvInsertUniqueSortOrder" runat="server" ControlToValidate="tbInsertSortOrder"
                                                ErrorMessage="Sort Order with this value already exists. Please enter different Sort Order."
                                                ToolTip="Sort Order with this value already exists. Please enter different Sort Order."
                                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="AddTargetedCompany" />
                                        </td>
                                        <td class="Width2Percent">
                                            &nbsp;&nbsp;&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
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
                                <asp:ValidationSummary ID="valSummaryAddTargetedCompany" runat="server" ValidationGroup="AddTargetedCompany"
                                    DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                    ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while adding a Targeted Company." />
                                <asp:ValidationSummary ID="valSummaryEditTargetedCompany" runat="server" ValidationGroup="EditTargetedCompany"
                                    DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                    ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a Targeted Company." />
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
    </div>
</asp:Content>

