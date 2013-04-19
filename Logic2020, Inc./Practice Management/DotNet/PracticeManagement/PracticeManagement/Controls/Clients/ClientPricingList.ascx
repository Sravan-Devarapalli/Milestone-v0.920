<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientPricingList.ascx.cs"
    Inherits="PraticeManagement.Controls.Clients.ClientPricingList" %>
<%@ Import Namespace="DataTransferObjects" %>
<table class="Width100Per">
    <tr>
        <td>
            <asp:GridView ID="gvPricingList" runat="server" EmptyDataText="There is nothing to be displayed here"
                AutoGenerateColumns="False" OnRowDataBound="gvPricingList_RowDataBound"
                CssClass="CompPerfTable Width40P BackGroundColorWhite" GridLines="None">
                <AlternatingRowStyle CssClass="alterrow" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg Width98PxImp">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="Width15PercentImp TextAlignCenterImp" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/Images/icon-edit.png" OnClick="imgEdit_OnClick"
                                ToolTip="Edit Pricing List" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:ImageButton ID="imgUpdate" runat="server" ImageUrl="~/Images/icon-check.png"
                                OnClick="imgUpdate_OnClick" ToolTip="Confirm" />
                            <asp:ImageButton ID="imgCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="imgCancel_OnClick"
                                ToolTip="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle CssClass="width60P no-wrap" />
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Name</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblPricingList" CssClass="WS-Normal" runat="server" Text='<%# Eval("HtmlEncodedName") %>' ToolTip='<%# Eval("HtmlEncodedName") %>'></asp:Label>
                            <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("PricingListId") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPricingListName" runat="server" Text='<%# Eval("Name") %>'
                                ValidationGroup="UpdatePricingList" CssClass="Width96Per"></asp:TextBox>
                            <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("PricingListId") %>' />
                            <asp:RequiredFieldValidator ID="reqPricingListName" runat="server" ControlToValidate="txtPricingListName"
                                ErrorMessage="The name of  Pricing List is required." ToolTip="The name of Pricing List is required."
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="UpdatePricingList">*</asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custNewPricingList" runat="server" ControlToValidate="txtPricingListName"
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="The name of Pricing List must be unique." ToolTip="The name of Pricing List  must be unique."
                                ValidationGroup="UpdatePricingList">*</asp:CustomValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false">
                        <ItemStyle CssClass="Width0Percent TextAlignCenterImp" />
                        <HeaderTemplate>
                            <div class="ie-bg">
                                In use</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblGroupInuse" runat="server" Text='<%# (bool)Eval("InUse") ? "Yes" : "No" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="Width10Per TextAlignCenterImp" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/icon-delete.png"
                                OnClick="imgDelete_OnClick" ToolTip="Delete Pricing List" />
                        </ItemTemplate>
                        <EditItemTemplate>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <table class="CompPerfTable gvCommissionsAndRates Width40P Border0">
                <tr class="alterrow">
                    <td class="Width15Percent PaddingTop10Px TextAlignCenter">
                        <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                            ToolTip="Add Pricing List" />
                        <asp:ImageButton ID="btnAddPricingList" runat="server" ValidationGroup="NewPricingList"
                            ImageUrl="~/Images/icon-check.png" ToolTip="Confirm" Visible="false" OnClick="btnAddPricingList_Click" />
                        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_Click"
                            ToolTip="Cancel" Visible="false" />
                    </td>
                    <td class="width60P no-wrap">
                        <asp:Label ID="lblNewPricingListName" runat="server" AssociatedControlID="txtNewPricingListName" />
                        <asp:TextBox ID="txtNewPricingListName" runat="server" ValidationGroup="NewPricingList"
                            CssClass="Width96Per" Visible="false" />
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarker" runat="server" TargetControlID="txtNewPricingListName"
                            WatermarkText="New Pricing List" WatermarkCssClass="watermarked Width96Per" />
                        <asp:RequiredFieldValidator ID="reqNewPricingListName" runat="server" ControlToValidate="txtNewPricingListName"
                            ErrorMessage="Pricing List name is required." ToolTip="Pricing List name is required."
                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="NewPricingList">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="custNewPricingListName" runat="server" ControlToValidate="txtNewPricingListName"
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ErrorMessage="There is already a Pricing List with the same name." ToolTip="There is already a Pricing List with the same name."
                            ValidationGroup="NewPricingList" OnServerValidate="custNewPricingListName_ServerValidate">*</asp:CustomValidator>
                        <asp:RegularExpressionValidator ID="regGroupName" runat="server" ControlToValidate="txtNewPricingListName"
                            ErrorMessage="PricingList Name limited to 1-50 characters in length" ToolTip="PricingList Name limited to 1-50 characters in length"
                            ValidationGroup="NewPricingList" Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Dynamic" ValidationExpression="^[\w]{1,50}$"></asp:RegularExpressionValidator>
                    </td>
                    <td class="Width10Percent">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:ValidationSummary ID="valSumGroups" runat="server" ValidationGroup="NewPricingList" />
<asp:ValidationSummary ID="valSumUpdation" runat="server" ValidationGroup="UpdatePricingList" />

