<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientGroups.ascx.cs"
    Inherits="PraticeManagement.Controls.Clients.ClientGroups" %>
<%@ Import Namespace="DataTransferObjects" %>
<table class="Width100Per">
    <tr>
        <td>
            <asp:GridView ID="gvGroups" runat="server" EmptyDataText="There is nothing to be displayed here"
                AutoGenerateColumns="False" DataKeyNames="Key" OnRowUpdating="gvGroups_RowUpdating"
                OnRowDeleting="gvGroups_RowDeleting" OnRowEditing="gvGroups_RowEditing" OnRowCancelingEdit="gvGroups_RowCancelingEdit"
                OnRowDataBound="gvGroups_RowDataBound" CssClass="CompPerfTable Width40P BackGroundColorWhite"
                GridLines="None">
                <AlternatingRowStyle CssClass="alterrow" />
                <Columns>
                    <asp:CommandField ShowEditButton="True" ValidationGroup="EditPractice" ButtonType="Image"
                        EditImageUrl="~/Images/icon-edit.png" EditText="Edit Practice Area" UpdateText="Confirm"
                        UpdateImageUrl="~/Images/icon-check.png" CancelImageUrl="~/Images/no.png" CancelText="Cancel"
                        ItemStyle-CssClass="Width15PercentImp TextAlignCenterImp" />
                    <asp:TemplateField>
                        <ItemStyle CssClass="width60P no-wrap" />
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Name</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblGroupName" CssClass="WS-Normal" runat="server" Text='<%# ((ProjectGroup)Eval("Value")).HtmlEncodedName %>'></asp:Label>
                            <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("Key") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtGroupName" runat="server" Text='<%# ((ProjectGroup)Eval("Value")).Name %>'
                                ValidationGroup="UpdateGroup" CssClass="Width96Per"></asp:TextBox>
                            <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("Key") %>' />
                            <asp:RequiredFieldValidator ID="reqGropuName" runat="server" ControlToValidate="txtGroupName"
                                ErrorMessage="The name of Business Unit is required." ToolTip="The name of Business Unit is required."
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="UpdateGroup">*</asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custNewGroupName" runat="server" ControlToValidate="txtGroupName"
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="The name of Business Unit must be unique." ToolTip="The name of Business Unit  must be unique."
                                ValidationGroup="UpdateGroup">*</asp:CustomValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false">
                        <ItemStyle CssClass="Width0Percent TextAlignCenterImp" />
                        <HeaderTemplate>
                            <div class="ie-bg">
                                In use</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblGroupInuse" runat="server" Text='<%# ((ProjectGroup)Eval("Value")).InUse ? "Yes" : "No" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active">
                        <ItemStyle CssClass="Width15Percent TextAlignCenterImp" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbIsActive" runat="server" Enabled="false" Checked='<%# ((ProjectGroup)Eval("Value")).IsActive %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chbIsActiveEd" runat="server" Checked='<%# ((ProjectGroup)Eval("Value")).IsActive %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowDeleteButton="True" ButtonType="Image" DeleteImageUrl="~/Images/icon-delete.png"
                        ItemStyle-CssClass="Width10Per TextAlignCenterImp" />
                </Columns>
            </asp:GridView>
            <table class="CompPerfTable gvCommissionsAndRates Width40P Border0">
                <tr class="alterrow">
                    <td class="Width15Percent PaddingTop10Px TextAlignCenter">
                        <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                            ToolTip="Add Business Unit" />
                        <asp:ImageButton ID="btnAddGroup" runat="server" ValidationGroup="NewGroup" ImageUrl="~/Images/icon-check.png"
                            ToolTip="Confirm" Visible="false" OnClick="btnAddGroup_Click" />
                        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_Click"
                            ToolTip="Cancel" Visible="false" />
                    </td>
                    <td class="width60P no-wrap">
                        <asp:Label ID="lblNewGroupName" runat="server" AssociatedControlID="txtNewGroupName" />
                        <asp:TextBox ID="txtNewGroupName" runat="server" ValidationGroup="NewGroup" CssClass="Width96Per"
                            Visible="false" />
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarker" runat="server" TargetControlID="txtNewGroupName"
                            WatermarkText="New Business Unit" WatermarkCssClass="watermarked Width96Per" />
                        <asp:RequiredFieldValidator ID="reqNewGroupName" runat="server" ControlToValidate="txtNewGroupName"
                            ErrorMessage="Business Unit name is required." ToolTip="Business Unit name is required."
                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="NewGroup">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="custNewGroupName" runat="server" ControlToValidate="txtNewGroupName"
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ErrorMessage="There is already a Business Unit with the same name." ToolTip="There is already a Business Unit with the same name."
                            ValidationGroup="NewGroup" OnServerValidate="custNewGroupName_ServerValidate">*</asp:CustomValidator>
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
<asp:ValidationSummary ID="valSumGroups" runat="server" ValidationGroup="NewGroup" />
<asp:ValidationSummary ID="valSumUpdation" runat="server" ValidationGroup="UpdateGroup" />

