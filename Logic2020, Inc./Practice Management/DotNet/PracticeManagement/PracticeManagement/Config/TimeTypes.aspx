<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="TimeTypes.aspx.cs" Inherits="PraticeManagement.Config.TimeTypes" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Time Types</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Time Types
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
            <asp:GridView ID="gvTimeTypes" runat="server" DataSourceID="odsTimeTypes" DataKeyNames="Id"
                Width="550px" BackColor="White" AutoGenerateColumns="False" OnRowDataBound="gvTimeTypes_RowDataBound"
                CssClass="CompPerfTable" GridLines="None" EnableModelValidation="True" OnRowUpdating="gvTimeTypes_RowUpdating">
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgEdit" runat="server" CommandName="edit" ImageUrl="~/Images/icon-edit.png"
                                ToolTip="Edit Time Type" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:ImageButton ID="imgUpdate" runat="server" CommandName="update" ImageUrl="~/Images/icon-check.png"
                                ToolTip="Confirm" />
                            <asp:ImageButton ID="imgCancel" runat="server" CommandName="cancel" ImageUrl="~/Images/no.png"
                                ToolTip="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Left" Width="65%" Height="25px" />
                        <HeaderTemplate>
                            <span class="bg">Time Type Name</span>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>' Width="96%" />
                            <asp:RequiredFieldValidator ID="rvUpdatedTimeType" runat="server" ControlToValidate="tbName"
                                Display="Dynamic" ErrorMessage="Time Type Name is required" ToolTip="Time Type Name is required"
                                ValidationGroup="UpdateTimeType">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regValUpdatedTimeType" ControlToValidate="tbName"
                                Display="Dynamic" runat="server" ValidationGroup="UpdateTimeType" ValidationExpression="^[\s\S]{0,50}$"
                                ErrorMessage="Time Type Name should not be more than 50 characters." ToolTip="Time Type Name should not be more than 50 characters.">*</asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvUpdatedTimeTypeName" runat="server" ControlToValidate="tbName"
                                Display="Dynamic" ValidationGroup="UpdateTimeType" ErrorMessage="This time type already exists. Please enter a different time type."
                                ToolTip="This time type already exists. Please enter a different time type.">*</asp:CustomValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" Width="20%" />
                        <HeaderTemplate>
                            <span class="bg">Is Default</span>
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center" CssClass="CompPerfProjectNumber" />
                        <ItemTemplate>
                            <asp:RadioButton ID="rbIsDefault" runat="server" Checked='<%# Eval("IsDefault") %>'
                                Enabled="false" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:RadioButton ID="rbIsDefault" runat="server" Checked='<%# Bind("IsDefault") %>' />
                            <%--<asp:CheckBox ID="chbIsDefault" runat="server" Checked='<%# Bind("IsDefault") %>' />--%>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle HorizontalAlign="Center" Width="5%" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgDelete" runat="server" CommandName="delete" ImageUrl="~/Images/icon-delete.png"
                                ToolTip="Delete Time Type" />
                        </ItemTemplate>
                        <EditItemTemplate></EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <AlternatingRowStyle BackColor="#F9FAFF" />
            </asp:GridView>
            <p>
                <table width="550px" style="background-color: White; padding-top: 6px;">
                    <tr style="background-color: #F9FAFF; height: 30px;">
                        <td align="center" valign="middle" style="width: 10%">
                            <asp:ImageButton ID="ibtnInsertTimeType" runat="server" OnClick="ibtnInsertTimeType_Click"
                                ImageUrl="~/Images/add_16.png" OnClientClick="hideSuccessMessage();" ToolTip="Add Time Type" />
                            <asp:ImageButton ID="ibtnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
                                ToolTip="Confirm" Visible="false" OnClick="ibtnInsert_Click" OnClientClick="return hideSuccessMessage();"
                                ValidationGroup="NewTimeType" />
                            <asp:ImageButton ID="ibtnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="ibtnCancel_OnClick"
                                ToolTip="Cancel" Visible="false" />
                        </td>
                        <td align="left" valign="middle" style="width: 65%">
                            <asp:TextBox ID="tbNewTimeType" Style="width: 96%" Text="New time type" runat="server"
                                Visible="false" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarker" runat="server" TargetControlID="tbNewTimeType"
                                WatermarkText="New time type" EnableViewState="false" WatermarkCssClass="watermarked" />
                            <asp:RequiredFieldValidator ID="rvNewTimeType" runat="server" ControlToValidate="tbNewTimeType"
                                Display="Dynamic" ErrorMessage="Name is required" ToolTip="Time Type Name is required"
                                ValidationGroup="NewTimeType">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regValTimeType" ControlToValidate="tbNewTimeType"
                                Display="Dynamic" runat="server" ValidationGroup="NewTimeType" ValidationExpression="[a-zA-Z\s]{0,50}$"
                                ErrorMessage="Time Type Name should have only alphabets and should not be more than 50 characters.">*</asp:RegularExpressionValidator>
                        </td>
                        <td align="center" style="width: 20%">
                            <asp:RadioButton ID="rbIsDefault" runat="server" Visible="false" />
                        </td>
                        <td style="width: 5%">
                            &nbsp;
                        </td>
                        <tr>
                            <td colspan="4">
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
    <asp:ObjectDataSource ID="odsTimeTypes" runat="server" SelectMethod="GetAllTimeTypes"
        UpdateMethod="UpdateTimeType" DeleteMethod="RemoveTimeType" DataObjectTypeName="DataTransferObjects.TimeEntry.TimeTypeRecord"
        TypeName="PraticeManagement.TimeEntryService.TimeEntryServiceClient" OnDeleted="odsTimeTypes_Deleted"
        OnUpdated="odsTimeTypes_Updated"></asp:ObjectDataSource>
</asp:Content>

