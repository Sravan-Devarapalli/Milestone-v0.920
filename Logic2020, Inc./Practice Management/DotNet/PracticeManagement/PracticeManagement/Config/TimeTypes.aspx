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
                BackColor="White" AutoGenerateColumns="False" OnRowDataBound="gvTimeTypes_RowDataBound"
                CssClass="CompPerfTable" GridLines="None" EnableModelValidation="True" OnRowUpdating="gvTimeTypes_RowUpdating">
                <Columns>
                    <asp:CommandField ShowEditButton="True" ButtonType="Image" EditImageUrl="~/Images/icon-edit.png"
                        UpdateImageUrl="~/Images/icon-check.png" CancelImageUrl="~/Images/no.png">
                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                    </asp:CommandField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>Time Type Name</span></a> </span>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbName" runat="server" Text='<%# Bind("Name") %>' Width="250" />
                            <asp:RequiredFieldValidator ID="rvUpdatedTimeType" runat="server" ControlToValidate="tbName"
                                ErrorMessage="Time Type Name is required" ToolTip="Time Type Name is required"
                                ValidationGroup="UpdateTimeType">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regValUpdatedTimeType" ControlToValidate="tbName"
                                runat="server" ValidationGroup="UpdateTimeType" ValidationExpression="^[\s\S]{0,50}$"
                                ErrorMessage="Time Type Name should not be more than 50 characters." ToolTip="Time Type Name should not be more than 50 characters.">*</asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvUpdatedTimeTypeName" runat="server" ControlToValidate="tbName"
                                ValidationGroup="UpdateTimeType" ErrorMessage="This time type already exists. Please enter a different time type."
                                ToolTip="This time type already exists. Please enter a different time type.">*</asp:CustomValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>Is Default</span></a> </span>
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center" CssClass="CompPerfProjectNumber" />
                        <ItemTemplate>
                            <asp:RadioButton ID="rbIsDefault" runat="server" Checked='<%# Eval("IsDefault") %>'
                                Enabled="false" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chbIsDefault" runat="server" Checked='<%# Bind("IsDefault") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowDeleteButton="True" ButtonType="Image" DeleteImageUrl="~/Images/icon-delete.png">
                        <ItemStyle HorizontalAlign="Center" Width="30px" />
                    </asp:CommandField>
                </Columns>
                <AlternatingRowStyle BackColor="#F9FAFF" />
            </asp:GridView>
            <p>
                <table>
                    <tr>
                        <td>
                            <asp:TextBox ID="tbNewTimeType" Text="New time type" runat="server" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarker" runat="server" TargetControlID="tbNewTimeType"
                                WatermarkText="New time type" EnableViewState="false" WatermarkCssClass="watermarked" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="rvNewTimeType" runat="server" ControlToValidate="tbNewTimeType"
                                ErrorMessage="Name is required" ToolTip="Time Type Name is required" ValidationGroup="NewTimeType">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regValTimeType" ControlToValidate="tbNewTimeType"
                                runat="server" ValidationGroup="NewTimeType" ValidationExpression="[a-zA-Z\s]{0,50}$"
                                ErrorMessage="Time Type Name should have only alphabets and should not be more than 50 characters.">*</asp:RegularExpressionValidator>
                        </td>
                        <td>
                            <asp:ShadowedTextButton ID="btnInsertTimeType" runat="server" Text="Add Time Type"
                                OnClick="btnInsertTimeType_Click" OnClientClick="hideSuccessMessage();" CssClass="add-btn"
                                ValidationGroup="NewTimeType" />
                        </td>
                        <tr>
                            <td colspan="3">
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

