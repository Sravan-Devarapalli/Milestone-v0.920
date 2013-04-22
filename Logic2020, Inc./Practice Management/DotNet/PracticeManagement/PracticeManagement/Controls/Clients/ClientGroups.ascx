<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientGroups.ascx.cs"
    Inherits="PraticeManagement.Controls.Clients.ClientGroups" %>
<%@ Import Namespace="DataTransferObjects" %>
<table class="Width100Per">
    <tr>
        <td>
            <asp:GridView ID="gvGroups" runat="server" EmptyDataText="There is nothing to be displayed here"
                AutoGenerateColumns="False" OnRowDataBound="gvGroups_RowDataBound" CssClass="CompPerfTable Width50Percent BackGroundColorWhite"
                GridLines="None">
                <AlternatingRowStyle CssClass="alterrow" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width15PercentImp" />
                        <ItemStyle CssClass="TextAlignCenterImp" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/Images/icon-edit.png" OnClick="imgEdit_OnClick"
                                ToolTip="Edit Business Unit" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:ImageButton ID="imgUpdate" runat="server" ImageUrl="~/Images/icon-check.png"
                                OnClick="imgUpdate_OnClick" ToolTip="Confirm" />
                            <asp:ImageButton ID="imgCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="imgCancel_OnClick"
                                ToolTip="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="Width35Percent" />
                        <ItemStyle CssClass="no-wrap" />
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
                            <asp:TextBox ID="txtGroupName" runat="server" Text='<%# Eval("Name")%>' ValidationGroup="UpdateGroup" MaxLength="50"
                                CssClass="Width96Per"></asp:TextBox>
                            <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("Id") %>' />
                            <asp:RequiredFieldValidator ID="reqGropuName" runat="server" ControlToValidate="txtGroupName"
                                ErrorMessage="The name of Business Unit is required." ToolTip="The name of Business Unit is required."
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="UpdateGroup">*</asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custNewGroupName" runat="server" ControlToValidate="txtGroupName"
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ErrorMessage="The name of Business Unit must be unique." ToolTip="The name of Business Unit  must be unique."
                                ValidationGroup="UpdateGroup">*</asp:CustomValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="width30P" />
                        <ItemStyle CssClass="no-wrap" />
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Business Group</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblBusinessGroup" runat="server"></asp:Label>
                            <asp:HiddenField ID="hdnBusinessGroupId" runat="server" Value='<%# Eval("BusinessGroupId") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlBusinessGroup" CssClass="Width96Per" runat="server" >
                            </asp:DropDownList>
                            <asp:HiddenField ID="hdnBusinessGroupId" runat="server" Value='<%# Eval("BusinessGroupId") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false">
                        <ItemStyle CssClass="Width0Percent" />
                        <HeaderTemplate>
                            <div class="ie-bg">
                                In use</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblGroupInuse" runat="server" Text='<%# (bool)Eval("InUse") ? "Yes" : "No" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderText="Active">
                        <HeaderStyle CssClass="Width15PercentImp" />
                        <ItemStyle CssClass="TextAlignCenterImp" />
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
                        <HeaderStyle CssClass="Width5PercentImp" />
                        <ItemStyle CssClass="TextAlignCenterImp" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/icon-delete.png" Visible='<%# !(bool)Eval("InUse") %>'
                                OnClick="imgDelete_OnClick" ToolTip="Delete Business Unit" />
                        </ItemTemplate>
                        <EditItemTemplate>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <table class="CompPerfTable gvCommissionsAndRates Width50Percent Border0">
                <tr class="alterrow">
                    <td class="Width15PercentImp PaddingTop10Px TextAlignCenter">
                        <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                            ToolTip="Add Business Unit" />
                        <asp:ImageButton ID="btnAddGroup" runat="server" ValidationGroup="NewGroup" ImageUrl="~/Images/icon-check.png"
                            ToolTip="Confirm" Visible="false" OnClick="btnAddGroup_Click" />
                        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_Click"
                            ToolTip="Cancel" Visible="false" />
                    </td>
                    <td class="Width35Percent no-wrap">
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
                    <td class="width30P">
                        <asp:DropDownList ID="ddlAddBusinessGroup" CssClass="Width96Per" runat="server" Visible="false">
                        </asp:DropDownList>
                    </td>
                    <td class="Width15PercentImp TextAlignCenter">
                        <asp:CheckBox ID="chbGroupActive" runat="server" Checked="true" Visible="false" />
                    </td>
                    <td class="Width5PercentImp">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:ValidationSummary ID="valSumGroups" runat="server" ValidationGroup="NewGroup" />
<asp:ValidationSummary ID="valSumUpdation" runat="server" ValidationGroup="UpdateGroup" />

