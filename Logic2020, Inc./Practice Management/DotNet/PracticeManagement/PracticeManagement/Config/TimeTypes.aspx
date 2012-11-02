<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
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
            <asp:GridView ID="gvTimeTypes" runat="server" AutoGenerateColumns="False"
                OnRowDataBound="gvTimeTypes_RowDataBound" CssClass="CompPerfTable Width80Percent bgColorWhite" GridLines="None"
                EnableModelValidation="True">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="TextAlignCenterImp Width6PercentImp"/>
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
                        <ItemStyle CssClass="textLeft Width45PercentImp Height25Px"/>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Work Type Name
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>' CssClass="Width92Per"/>
                            <asp:RequiredFieldValidator ID="rvUpdatedTimeType" runat="server" ControlToValidate="tbName"
                                Display="Dynamic" ErrorMessage="Work Type Name is required" ToolTip="Work Type Name is required"
                                ValidationGroup="UpdateTimeType">*</asp:RequiredFieldValidator>
                            <AjaxControlToolkit:FilteredTextBoxExtender ID="fteTimeType" runat="server" TargetControlID="tbName"
                                FilterMode="ValidChars" FilterType="UppercaseLetters,LowercaseLetters,Numbers,Custom"
                                ValidChars=" /">
                            </AjaxControlToolkit:FilteredTextBoxExtender>
                            <asp:CustomValidator ID="cvUpdatedTimeTypeName" runat="server" ControlToValidate="tbName"
                                Display="Dynamic" ValidationGroup="UpdateTimeType" ErrorMessage="Work Type with this name already exists. Please enter a different Work Type name."
                                ToolTip="Work Type with this name already exists. Please enter a different Work Type name.">*</asp:CustomValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle CssClass="Width10Percent TextAlignCenterImp"/>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Is Default</div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="TextAlignCenterImp Width10Percent"/>
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
                        <ItemStyle CssClass="TextAlignCenterImp Width10Percent"/>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Is Internal</div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="TextAlignCenterImp Width10Percent" />
                        <ItemTemplate>
                            <asp:RadioButton ID="rbIsInternal" runat="server" Checked='<%# Eval("IsInternal") %>'
                                Enabled="false" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:RadioButton ID="rbIsInternal" runat="server" Checked='<%# Bind("IsInternal") %>'
                                Enabled="false" />
                            <asp:CustomValidator ID="cvIsDefaultOrInternalEdit" runat="server" Display="Dynamic"
                                ToolTip="Work Type should be defalult Or Internal Or Administrative." ErrorMessage="Work Type should be defalult Or Internal Or Administrative."
                                ValidationGroup="NewTimeType" OnServerValidate="cvIsDefaultOrInternalEdit_Servervalidate"
                                Text="*" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle CssClass="TextAlignCenterImp Width14PercentImp"/>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Is Administrative</div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="TextAlignCenterImp Width14PercentImp" />
                        <ItemTemplate>
                            <asp:RadioButton ID="rbIsAdministrative" runat="server" Checked='<%# Eval("IsAdministrative") %>'
                                Enabled="false" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:RadioButton ID="rbIsAdministrative" runat="server" Checked='<%# Bind("IsAdministrative") %>'
                                Enabled="false" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle CssClass="TextAlignCenterImp Width10PerImp"/>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Is Active</div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="TextAlignCenterImp" />
                        <ItemTemplate>
                            <asp:CheckBox ID="rbIsActive" runat="server" Checked='<%# Eval("IsActive") %>' Enabled="false" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="rbIsActive" runat="server" Checked='<%# Bind("IsActive") %>' Enabled='<%# !(bool)Eval("InFutureUse") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="TextAlignCenterImp Width5PercentImp"/>
                        <ItemTemplate>
                            <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/icon-delete.png"
                                OnClick="imgDelete_OnClick" ToolTip="Delete Work Type" Visible='<%# (bool)Eval("IsAllowedToEdit") %>'
                                timetypeId='<%# Eval("Id") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <AlternatingRowStyle CssClass="alterrow"/>
            </asp:GridView>
            <table class="CompPerfTable TableCompPer" cellspacing="0">
                <tr class="alterrow Height25Px">
                    <td class="TextAlignCenterImp Width6PercentImp">
                        <asp:ImageButton ID="ibtnInsertTimeType" runat="server" OnClick="ibtnInsertTimeType_Click"
                            ImageUrl="~/Images/add_16.png" OnClientClick="hideSuccessMessage();" ToolTip="Add Work Type" />
                        <asp:ImageButton ID="ibtnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
                            ToolTip="Confirm" Visible="false" OnClick="ibtnInsert_Click" OnClientClick="return hideSuccessMessage();"
                            ValidationGroup="NewTimeType" />
                        <asp:ImageButton ID="ibtnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="ibtnCancel_OnClick"
                            ToolTip="Cancel" Visible="false" />
                    </td>
                    <td class="textLeft Width45PercentImp">
                        <asp:TextBox ID="tbNewTimeType" CssClass="Width92Per" Text="New work type" runat="server"
                            MaxLength="50" Visible="false" />
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarker" runat="server" TargetControlID="tbNewTimeType"
                            WatermarkText="New work type" EnableViewState="false" WatermarkCssClass="watermarked Width92Per" />
                        <asp:RequiredFieldValidator ID="rvNewTimeType" runat="server" ControlToValidate="tbNewTimeType"
                            Display="Dynamic" ErrorMessage="Work Type Name is required" ToolTip="Work Type Name is required"
                            ValidationGroup="NewTimeType">*</asp:RequiredFieldValidator>
                        <AjaxControlToolkit:FilteredTextBoxExtender ID="fteNewTimeType" runat="server" TargetControlID="tbNewTimeType"
                            FilterMode="ValidChars" FilterType="UppercaseLetters,LowercaseLetters,Numbers,Custom"
                            ValidChars=" /">
                        </AjaxControlToolkit:FilteredTextBoxExtender>
                    </td>
                    <td class="TextAlignCenterImp Width10PerImp">
                        <asp:RadioButton ID="rbIsDefault" runat="server" Visible="false" GroupName="rbNewTimeType" />
                    </td>
                    <td  class="TextAlignCenterImp Width10PerImp">
                        <asp:RadioButton ID="rbIsInternal" runat="server" Visible="false" GroupName="rbNewTimeType" />
                        <asp:CustomValidator ID="cvIsDefaultOrInternal" runat="server" Display="Dynamic"
                            ToolTip="Work Type should be Defalult Or Internal Or Administrative." ErrorMessage="Work Type should be Defalult Or Internal Or Administrative."
                            ValidationGroup="NewTimeType" OnServerValidate="cvIsDefaultOrInternal_Servervalidate"
                            Text="*" />
                    </td>
                    <td class="TextAlignCenterImp Width14PercentImp">
                        <asp:RadioButton ID="rbIsAdministrative" runat="server" Visible="false" GroupName="rbNewTimeType" />
                    </td>
                    <td  class="TextAlignCenterImp Width10PerImp">
                        <asp:CheckBox ID="rbIsActive" runat="server" Visible="false" />
                    </td>
                    <td class="Width5PercentImp">
                        &nbsp;
                    </td>
                    <tr>
                        <td colspan="7">
                            <asp:ValidationSummary ID="valsumTimeType" runat="server" ValidationGroup="NewTimeType" />
                            <asp:ValidationSummary ID="valsumUpdateTimeType" runat="server" ValidationGroup="UpdateTimeType" />
                        </td>
                    </tr>
                </tr>
            </table>
            <uc:Label ID="mlInsertStatus" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

