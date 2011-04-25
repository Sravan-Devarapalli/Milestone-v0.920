<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientGroups.ascx.cs"
    Inherits="PraticeManagement.Controls.Clients.ClientGroups" %>
<%@ Import Namespace="DataTransferObjects" %>
<table width="100%">
    <tr>
        <td>
            <asp:GridView ID="gvGroups" runat="server" EmptyDataText="There is nothing to be displayed here"
                Width="40%" AutoGenerateColumns="False" DataKeyNames="Key" OnRowUpdating="gvGroups_RowUpdating"
                OnRowDeleting="gvGroups_RowDeleting" OnRowEditing="gvGroups_RowEditing" OnRowCancelingEdit="gvGroups_RowCancelingEdit"
                OnRowDataBound="gvGroups_RowDataBound" CssClass="CompPerfTable" GridLines="None"
                BackColor="White">
                <AlternatingRowStyle BackColor="#F9FAFF" />
                <Columns>
                    <asp:CommandField ShowEditButton="True" ValidationGroup="EditPractice" ButtonType="Image"
                        ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center" EditImageUrl="~/Images/icon-edit.png"
                        EditText="Edit Practice Area" UpdateText="Confirm" UpdateImageUrl="~/Images/icon-check.png"
                        CancelImageUrl="~/Images/no.png" CancelText="Cancel" />
                    <asp:TemplateField HeaderText="Id" Visible="false">
                        <ItemStyle Width="0%" />
                        <ItemTemplate>
                            <asp:Label ID="lblId" runat="server" Text='<%# Eval("Key") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="60%" Wrap="false" />
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Name</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblGroupName" Style="white-space: normal !important;" runat="server"
                                Text='<%# ((ProjectGroup)Eval("Value")).Name %>'></asp:Label>
                            <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("Key") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtGroupName" runat="server" Text='<%# ((ProjectGroup)Eval("Value")).Name %>'
                                Width="96%" ValidationGroup="UpdateGroup"></asp:TextBox>
                            <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("Key") %>' />
                            <asp:RequiredFieldValidator ID="reqGropuName" runat="server" ControlToValidate="txtGroupName"
                                ErrorMessage="The name of group is required." ToolTip="The name of group is required."
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="UpdateGroup">*</asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custNewGroupName" runat="server" ControlToValidate="txtGroupName"
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="The name of group must to be unique." ToolTip="The name of group must to be unique."
                                ValidationGroup="UpdateGroup" >*</asp:CustomValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false" >
                        <ItemStyle HorizontalAlign="Center" Width="0%" />
                        <HeaderTemplate>
                            <div class="ie-bg">
                                In use</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblGroupInuse" runat="server" Text='<%# ((ProjectGroup)Eval("Value")).InUse ? "Yes" : "No" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active">
                        <ItemStyle HorizontalAlign="Center" Width="15%" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbIsActive" runat="server" Enabled="false" Checked='<%# ((ProjectGroup)Eval("Value")).IsActive %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chbIsActiveEd" runat="server" Checked='<%# ((ProjectGroup)Eval("Value")).IsActive %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowDeleteButton="True" ButtonType="Image" ItemStyle-Width="10%"
                        ItemStyle-HorizontalAlign="Center" DeleteImageUrl="~/Images/icon-delete.png" />
                </Columns>
            </asp:GridView>
            <table width="40% !important;" class="CompPerfTable" cellspacing="0" border="0" style="background-color: White;
                border-collapse: collapse;">
                <tr style="background-color: #F9FAFF;">
                    <td align="center" style="width: 15%; padding-top: 10px;">
                        <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                            ToolTip="Add Group" Visible="true" />
                        <asp:ImageButton ID="btnAddGroup" runat="server" ValidationGroup="NewGroup" ImageUrl="~/Images/icon-check.png"
                            ToolTip="Confirm" Visible="false" OnClick="btnAddGroup_Click" />
                        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_Click"
                            ToolTip="Cancel" Visible="false" />
                    </td>
                    <td style="width: 0%;">
                    </td>
                    <td style="width: 60%; white-space: nowrap;">
                        <asp:Label ID="lblNewGroupName" runat="server" AssociatedControlID="txtNewGroupName" />
                        <asp:TextBox ID="txtNewGroupName" runat="server" ValidationGroup="NewGroup" Width="96%"
                            Visible="false" />
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarker" runat="server" TargetControlID="txtNewGroupName"
                            WatermarkText="New Group" WatermarkCssClass="watermarked" />
                        <asp:RequiredFieldValidator ID="reqNewGroupName" runat="server" ControlToValidate="txtNewGroupName"
                            ErrorMessage="Group name is required." ToolTip="Group name is required." EnableClientScript="false"
                            SetFocusOnError="true" Display="Dynamic" ValidationGroup="NewGroup">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="custNewGroupName" runat="server" ControlToValidate="txtNewGroupName"
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ErrorMessage="There is already a group with the same name." ToolTip="There is already a group with the same name."
                            ValidationGroup="NewGroup" OnServerValidate="custNewGroupName_ServerValidate">*</asp:CustomValidator>
                    </td>
                    <td style="width: 0%;"></td>
                    <td align="center" style="width: 15%;">
                        <asp:CheckBox ID="chbGroupActive" runat="server" Checked="true" Visible="false" />
                    </td>
                    <td style="width: 10%;">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:ValidationSummary ID="valSumGroups" runat="server" ValidationGroup="NewGroup" />
<asp:ValidationSummary ID="valSumUpdation" runat="server" ValidationGroup="UpdateGroup" />
<asp:ObjectDataSource ID="odsClientGroups" runat="server" SelectMethod="GroupListAll"
    TypeName="PraticeManagement.ProjectGroupService.ProjectGroupServiceClient">
    <SelectParameters>
        <asp:QueryStringParameter Name="clientId" QueryStringField="id" Type="Int32" />
        <asp:Parameter Name="projectId" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

