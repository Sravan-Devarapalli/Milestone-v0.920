﻿<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="TimeTypes.aspx.cs" Inherits="PraticeManagement.Config.TimeTypes" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Work Types | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Work Types
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function hideSuccessMessage() {
            message = document.getElementById("<%=mlInsertStatus.ClientID %>" + "_lblMessage");
            if (message != null) {
                message.style.display = "none";
            }
            return true;
        }
    </script>
    <asp:UpdatePanel ID="upnlTimeTypes" runat="server">
        <ContentTemplate>
            <asp:GridView ID="gvTimeTypes" runat="server" Width="550px" BackColor="White" AutoGenerateColumns="False"
                OnRowDataBound="gvTimeTypes_RowDataBound" CssClass="CompPerfTable" GridLines="None"
                EnableModelValidation="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/Images/icon-edit.png" OnClick="imgEdit_OnClick"
                                ToolTip="Edit Work Type" Visible='<%# (bool)Eval("IsAllowedToEdit") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:HiddenField ID="hdfTimeTypeId" runat="server" Value='<%# Eval("Id") %>' />
                            <asp:ImageButton ID="imgUpdate" runat="server" ImageUrl="~/Images/icon-check.png"
                                OnClick="imgUpdate_OnClick" ToolTip="Confirm" />
                            <asp:ImageButton ID="imgCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="imgCancel_OnClick"
                                ToolTip="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Left" Width="55%" Height="25px" />
                        <HeaderTemplate>
                            <span class="bg">Work Type Name</span>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>' Width="92%" />
                            <asp:RequiredFieldValidator ID="rvUpdatedTimeType" runat="server" ControlToValidate="tbName"
                                Display="Dynamic" ErrorMessage="Work Type Name is required" ToolTip="Work Type Name is required"
                                ValidationGroup="UpdateTimeType">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regValUpdatedTimeType" ControlToValidate="tbName"
                                Display="Dynamic" runat="server" ValidationGroup="UpdateTimeType" ValidationExpression="^[a-zA-Z\s]{0,50}$"
                                ErrorMessage="Work Type Name should have only alphabets and should not be more than 50 characters."
                                ToolTip="Work Type Name should have only alphabets and should not be more than 50 characters.">*</asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvUpdatedTimeTypeName" runat="server" ControlToValidate="tbName"
                                Display="Dynamic" ValidationGroup="UpdateTimeType" ErrorMessage="This work type already exists. Please enter a different work type."
                                ToolTip="This work type already exists. Please enter a different work type.">*</asp:CustomValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                        <HeaderTemplate>
                            <span class="bg">Is Default</span>
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center" CssClass="CompPerfProjectNumber" />
                        <ItemTemplate>
                            <asp:RadioButton ID="rbIsDefault" runat="server" Checked='<%# Eval("IsDefault") %>'
                                Enabled="false" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:RadioButton ID="rbIsDefault" runat="server" Checked='<%# Bind("IsDefault") %>'
                                Enabled="false" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                        <HeaderTemplate>
                            <span class="bg">Is Internal</span>
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center" CssClass="CompPerfProjectNumber" />
                        <ItemTemplate>
                            <asp:RadioButton ID="rbIsInternal" runat="server" Checked='<%# Eval("IsInternal") %>'
                                Enabled="false" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:RadioButton ID="rbIsInternal" runat="server" Checked='<%# Bind("IsInternal") %>'
                                Enabled="false" />
                            <asp:CustomValidator ID="cvIsDefaultOrInternalEdit" runat="server" Display="Dynamic"
                                ToolTip="WorkType should be Isdefalult OrIsInternal" ErrorMessage="WorkType should be Isdefalult OrIsInternal"
                                ValidationGroup="NewTimeType" OnServerValidate="cvIsDefaultOrInternalEdit_Servervalidate"
                                Text="*" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                        <HeaderTemplate>
                            <span class="bg">Is Active</span>
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center" CssClass="CompPerfProjectNumber" />
                        <ItemTemplate>
                            <asp:CheckBox ID="rbIsActive" runat="server" Checked='<%# Eval("IsActive") %>' Enabled="false" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="rbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' Enabled='<%# !(bool)Eval("InFutureUse") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" Width="5%" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/icon-delete.png"
                                OnClick="imgDelete_OnClick" ToolTip="Delete Work Type" Visible='<%# (bool)Eval("IsAllowedToEdit") %>'
                                timetypeId='<%# Eval("Id") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <AlternatingRowStyle BackColor="#F9FAFF" />
            </asp:GridView>
            <p>
                <table width="550px" style="padding-top: 6px; background-color: white; border-collapse: collapse;"
                    border="0" cellspacing="0">
                    <tr style="background-color: #F9FAFF; height: 30px;">
                        <td align="center" valign="middle" style="width: 7%">
                            <asp:ImageButton ID="ibtnInsertTimeType" runat="server" OnClick="ibtnInsertTimeType_Click"
                                ImageUrl="~/Images/add_16.png" OnClientClick="hideSuccessMessage();" ToolTip="Add Work Type" />
                            <asp:ImageButton ID="ibtnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
                                ToolTip="Confirm" Visible="false" OnClick="ibtnInsert_Click" OnClientClick="return hideSuccessMessage();"
                                ValidationGroup="NewTimeType" />
                            <asp:ImageButton ID="ibtnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="ibtnCancel_OnClick"
                                ToolTip="Cancel" Visible="false" />
                        </td>
                        <td align="left" valign="middle" style="width: 52%">
                            <asp:TextBox ID="tbNewTimeType" Style="width: 92%" Text="New work type" runat="server"
                                Visible="false" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarker" runat="server" TargetControlID="tbNewTimeType"
                                WatermarkText="New work type" EnableViewState="false" WatermarkCssClass="watermarked" />
                            <asp:RequiredFieldValidator ID="rvNewTimeType" runat="server" ControlToValidate="tbNewTimeType"
                                Display="Dynamic" ErrorMessage="Work Type Name is required" ToolTip="Work Type Name is required"
                                ValidationGroup="NewTimeType">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regValTimeType" ControlToValidate="tbNewTimeType"
                                Display="Dynamic" runat="server" ValidationGroup="NewTimeType" ValidationExpression="[a-zA-Z\s]{0,50}$"
                                ErrorMessage="Work Type Name should have only alphabets and should not be more than 50 characters.">*</asp:RegularExpressionValidator>
                        </td>
                        <td align="center" style="width: 11%">
                            <asp:RadioButton ID="rbIsDefault" runat="server" Visible="false" GroupName="rbNewTimeType" />
                        </td>
                        <td align="center" style="width: 15%">
                            <asp:RadioButton ID="rbIsInternal" runat="server" Visible="false" GroupName="rbNewTimeType" />
                            <asp:CustomValidator ID="cvIsDefaultOrInternal" runat="server" Display="Dynamic"
                                ToolTip="WorkType should be Isdefalult OrIsInternal" ErrorMessage="WorkType should be Isdefalult OrIsInternal"
                                ValidationGroup="NewTimeType" OnServerValidate="cvIsDefaultOrInternal_Servervalidate"
                                Text="*" />
                        </td>
                        <td align="center" style="width: 12%">
                            <asp:CheckBox ID="rbIsActive" runat="server" Visible="false" />
                        </td>
                        <td style="width: 3%">
                            &nbsp;
                        </td>
                        <tr>
                            <td colspan="6">
                                <asp:ValidationSummary ID="valsumTimeType" runat="server" ValidationGroup="NewTimeType" />
                                <asp:ValidationSummary ID="valsumUpdateTimeType" runat="server" ValidationGroup="UpdateTimeType" />
                            </td>
                        </tr>
                    </tr>
                </table>
            </p>
            <uc:Label ID="mlInsertStatus" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

