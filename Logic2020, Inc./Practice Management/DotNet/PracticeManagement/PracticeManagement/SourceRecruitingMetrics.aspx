<%@ Page Language="C#" Title="Candidate Source Field Customization" AutoEventWireup="true"
    CodeBehind="SourceRecruitingMetrics.aspx.cs" MasterPageFile="~/PracticeManagementMain.Master"
    Inherits="PraticeManagement.SourceRecruitingMetrics" %>

<%@ Register Src="~/Controls/Configuration/RecruitingMetricsHeader.ascx" TagPrefix="uc"
    TagName="RecruitingMetricsHeader" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="body" runat="server">
    <uc:RecruitingMetricsHeader ID="recruitingMetricsHeader" runat="server"></uc:RecruitingMetricsHeader>
    <div class="divMetrics">
        <asp:UpdatePanel ID="upnlSourceRecruitingMetrics" runat="server">
            <ContentTemplate>
                <table class="WholeWidth">
                    <tr>
                        <td>
                            <asp:GridView ID="gvSourceRecruitingMetrics" runat="server" AutoGenerateColumns="false"
                                OnRowDataBound="gvSourceRecruitingMetrics_RowDataBound" CssClass="CompPerfTable gvTitles Width44point5Percent"
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
                                            <asp:ImageButton ID="imgEditRecruitMetrics" ToolTip="Edit Source Recruiting Metrics"
                                                OnClick="imgEditSourceMetrics_OnClick" runat="server" ImageUrl="~/Images/icon-edit.png" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:ImageButton ID="imgRecruitMetricsUpdate" ToolTip="Save" runat="server" ImageUrl="~/Images/icon-check.png"
                                                SourceId='<%# Eval("RecruitingMetricsId") %>' OnClick="imgUpdateSourceMetrics_OnClick" />
                                            <asp:ImageButton ID="imgCancel" ToolTip="Cancel" runat="server" ImageUrl="~/Images/no.png"
                                                OnClick="imgCancel_OnClick" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Source Name
                                            </div>
                                        </HeaderTemplate>
                                        <HeaderStyle CssClass="Width20Percent" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblSourceName" CssClass="WS-Normal padLeft5" runat="server" Text='<%# Eval("HtmlEncodedName") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="tbEditSourceName" runat="server" Text='<%# Eval("Name") %>' CssClass="Width95Percent"
                                                OldName='<%# Eval("Name") %>' />
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtTitleName" runat="server"
                                                TargetControlID="tbEditSourceName" WatermarkCssClass="watermarkedtext Width95Percent"
                                                WatermarkText="Please enter source name">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:RequiredFieldValidator ID="reqSourceName" runat="server" ControlToValidate="tbEditSourceName"
                                                ErrorMessage="Source Name is required." ToolTip="Source Name is required." SetFocusOnError="true"
                                                Text="*" ValidationGroup="EditSource"></asp:RequiredFieldValidator>
                                            <asp:CustomValidator ID="cvUniqueSource" runat="server" ControlToValidate="tbEditSourceName"
                                                ErrorMessage="Source with this name already exists. Please enter different Source name."
                                                ToolTip="Source with this name already exists. Please enter different Source name."
                                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="EditSource" />
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
                                                Text="*" ValidationGroup="EditSource"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ControlToValidate="tbEditSortOrder" ID="valRegSortOrder"
                                                runat="server" ErrorMessage="Sort Order value is not valid.Enter only positive integer."
                                                ToolTip="Sort Order value is not valid.Enter only positive integer." Display="Dynamic"
                                                Text="*" ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="EditSource" />
                                            <asp:CustomValidator ID="cvUniqueSortOrder" runat="server" ControlToValidate="tbEditSortOrder"
                                                ErrorMessage="Sort Order with this value already exists. Please enter different Sort Order."
                                                ToolTip="Sort Order with this value already exists. Please enter different Sort Order."
                                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="EditSource" />
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
                                                SourceId='<%# Eval("RecruitingMetricsId") %>' OnClick="imgDelete_OnClick" ToolTip="Delete Source Recruiting Metrics"
                                                Visible='<%# IsDeleteButtonVisible((bool)Eval("InUse")) %>' />
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgDelete" runat="server" TargetControlID="imgDelete"
                                                ConfirmText="Are you sure. Do you want to delete the Source?">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Panel ID="pnlInsertSource" runat="server">
                                <table class="CompPerfTable gvTitles Width44point5Percent" cellspacing="0">
                                    <tr class="alterrow">
                                        <td class="Width5Percent">
                                            <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                                                ToolTip="Add Source Recruiting Metrics." Visible="true" />
                                            <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
                                                ToolTip="Save" Visible="false" OnClick="btnInsert_Click" />
                                            <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
                                                ToolTip="Cancel" Visible="false" />
                                        </td>
                                        <td class="Width20Percent">
                                            <asp:TextBox ID="tbInsertSource" runat="server" Enabled="false" CssClass="Width95Percent" />
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtInsertTitleName" runat="server"
                                                TargetControlID="tbInsertSource" WatermarkCssClass="watermarkedtext Width95Percent"
                                                WatermarkText="Please enter source name">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:RequiredFieldValidator ID="reqSource" runat="server" ControlToValidate="tbInsertSource"
                                                ErrorMessage="Source Name is required." ToolTip="Source Name is required." SetFocusOnError="true"
                                                Text="*" ValidationGroup="AddSource"></asp:RequiredFieldValidator>
                                            <asp:CustomValidator ID="cvInsertUniqueSource" runat="server" ControlToValidate="tbInsertSource"
                                                ErrorMessage="Source with this name already exists. Please enter different Source name."
                                                ToolTip="Source with this name already exists. Please enter different Source name."
                                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="AddSource" />
                                        </td>
                                        <td class="Width13Percent">
                                            <asp:TextBox ID="tbInsertSortOrder" runat="server" Enabled="false" CssClass="Width85Percent" />
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkInsertSortOrder"
                                                runat="server" TargetControlID="tbInsertSortOrder" WatermarkCssClass="watermarkedtext Width85Percent"
                                                WatermarkText="Please enter Sort Order">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:RequiredFieldValidator ID="regSortOrder" runat="server" ControlToValidate="tbInsertSortOrder"
                                                ErrorMessage="Sort Order is required." ToolTip="Sort Order is required." SetFocusOnError="true"
                                                Text="*" ValidationGroup="AddSource"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ControlToValidate="tbInsertSortOrder" ID="valRegSortOrder"
                                                runat="server" ErrorMessage="Sort Order value is not valid.Enter only positive integer."
                                                ToolTip="Sort Order value is not valid.Enter only positive integer." Display="Dynamic"
                                                Text="*" ValidationExpression="^[0-9]{1,16}$" SetFocusOnError="true" ValidationGroup="AddSource" />
                                            <asp:CustomValidator ID="cvInsertUniqueSortOrder" runat="server" ControlToValidate="tbInsertSortOrder"
                                                ErrorMessage="Sort Order with this value already exists. Please enter different Sort Order."
                                                ToolTip="Sort Order with this value already exists. Please enter different Sort Order."
                                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="AddSource" />
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
                                <asp:ValidationSummary ID="valSummaryAddSource" runat="server" ValidationGroup="AddSource"
                                    DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                    ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while adding a Source." />
                                <asp:ValidationSummary ID="valSummaryEditSource" runat="server" ValidationGroup="EditSource"
                                    DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                    ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a Source." />
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

