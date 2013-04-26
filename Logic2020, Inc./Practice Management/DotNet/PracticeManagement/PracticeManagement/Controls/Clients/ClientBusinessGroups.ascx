<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientBusinessGroups.ascx.cs"
    Inherits="PraticeManagement.Controls.Clients.ClientBusinessGroups" %>
<%@ Import Namespace="DataTransferObjects" %>
<table class="Width100Per">
    <tr>
        <td>
            <asp:GridView ID="gvBusinessGroups" runat="server" EmptyDataText="There is nothing to be displayed here"
                AutoGenerateColumns="False" OnRowDataBound="gvBusinessGroups_RowDataBound" CssClass="CompPerfTable Width40P BackGroundColorWhite"
                GridLines="None">
                <AlternatingRowStyle CssClass="alterrow" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="Width15PercentImp TextAlignCenterImp" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/Images/icon-edit.png" OnClick="imgEdit_OnClick"
                                ToolTip="Edit Business Group" />
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
                            <asp:Label ID="lblGroupName" CssClass="WS-Normal" runat="server" Text='<%# Eval("HtmlEncodedName") %>'
                                ToolTip='<%# Eval("HtmlEncodedName") %>'></asp:Label>
                            <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("Id") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtGroupName" runat="server" Text='<%# Eval("Name")%>' ValidationGroup="UpdateBusinessGroup"
                                MaxLength="50" CssClass="Width96Per"></asp:TextBox>
                            <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("Id") %>' />
                            <asp:RequiredFieldValidator ID="reqGroupName" runat="server" ControlToValidate="txtGroupName"
                                ErrorMessage="The name of Business Group is required." ToolTip="The name of Business Group is required."
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="UpdateBusinessGroup">*</asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custUpdateGroupName" runat="server" ControlToValidate="txtGroupName"
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="The name of Business Group must be unique." ToolTip="The name of Business Group  must be unique."
                                ValidationGroup="UpdateBusinessGroup">*</asp:CustomValidator>
                            <asp:RegularExpressionValidator ID="regGroupName" runat="server" ControlToValidate="txtGroupName"
                                ErrorMessage="Business Group Name limited to 1-50 characters in length" ToolTip="Business Group Name limited to 1-50 characters in length"
                                ValidationGroup="UpdateBusinessGroup" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                Display="Dynamic" ValidationExpression="^[\w\s]{1,50}$"></asp:RegularExpressionValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false">
                        <ItemStyle CssClass="Width0Percent TextAlignCenterImp" />
                        <HeaderTemplate>
                            <div class="ie-bg">
                                In use</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblGroupInuse" runat="server" Text='<%# (bool)Eval("InUse")? "Yes" : "No" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active">
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Active
                                <asp:CustomValidator ID="custBusinessGroupActive" runat="server" Text="*" EnableClientScript="false"
                                    SetFocusOnError="true" Display="Dynamic" ErrorMessage="Atleast one business group must be active."
                                    ToolTip="Atleast one business group must be active." ValidationGroup="UpdateBusinessGroup">*</asp:CustomValidator>
                            </div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="Width15Percent TextAlignCenterImp" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbIsActive" runat="server" Enabled="false" Checked='<%# (bool)Eval("IsActive") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chbIsActiveEd" runat="server" Checked='<%# (bool)Eval("IsActive") %>' />
                        </EditItemTemplate>
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
                                OnClick="imgDelete_OnClick" ToolTip="Delete Business Group" />
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
                            ToolTip="Add Business Group" />
                        <asp:ImageButton ID="btnAddGroup" runat="server" ValidationGroup="NewBusinessGroup"
                            ImageUrl="~/Images/icon-check.png" ToolTip="Confirm" Visible="false" OnClick="btnAddGroup_Click" />
                        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_Click"
                            ToolTip="Cancel" Visible="false" />
                    </td>
                    <td class="width60P no-wrap">
                        <asp:Label ID="lblNewGroupName" runat="server" AssociatedControlID="txtNewGroupName" />
                        <asp:TextBox ID="txtNewGroupName" runat="server" ValidationGroup="NewBusinessGroup"
                            CssClass="Width96Per" Visible="false" />
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarker" runat="server" TargetControlID="txtNewGroupName"
                            WatermarkText="New Business Group" WatermarkCssClass="watermarked Width96Per" />
                        <asp:RequiredFieldValidator ID="reqNewGroupName" runat="server" ControlToValidate="txtNewGroupName"
                            ErrorMessage="Business Group name is required." ToolTip="Business Group name is required."
                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="NewBusinessGroup">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="custNewGroupName" runat="server" ControlToValidate="txtNewGroupName"
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ErrorMessage="There is already a Business Group with the same name." ToolTip="There is already a Business Group with the same name."
                            ValidationGroup="NewBusinessGroup" OnServerValidate="custNewGroupName_ServerValidate">*</asp:CustomValidator>
                        <asp:RegularExpressionValidator ID="regGroupName" runat="server" ControlToValidate="txtNewGroupName"
                            ErrorMessage="Business Group Name limited to 1-50 characters in length" ToolTip="Business Group Name limited to 1-50 characters in length"
                            ValidationGroup="NewBusinessGroup" Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Dynamic" ValidationExpression="^[\w\s]{1,50}$"></asp:RegularExpressionValidator>
                    </td>
                    <td class="Width0Percent">
                    </td>
                    <td class="Width15Percent TextAlignCenter">
                        <asp:CheckBox ID="chbGroupActive" runat="server" Checked="true" Visible="false" />
                    </td>
                    <td class="Width10Percent">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:ValidationSummary ID="valSumGroups" runat="server" ValidationGroup="NewBusinessGroup" />
<asp:ValidationSummary ID="valSumUpdation" runat="server" ValidationGroup="UpdateBusinessGroup" />

