<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="PracticeAreas.aspx.cs" Inherits="PraticeManagement.Config.PracticeAreas" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="PraticeManagement.Controls.Configuration" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Practice Areas</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Practice Areas
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
    <asp:GridView ID="gvPractices" runat="server" AutoGenerateColumns="False" DataSourceID="odsPractices"
        Width="75%" CssClass="CompPerfTable" DataKeyNames="Id" OnRowUpdating="gvPractices_OnRowUpdating"
        OnRowDataBound="gvPractices_RowDataBound" GridLines="None" BackColor="White">
        <AlternatingRowStyle BackColor="#F9FAFF" />
        <Columns>
            <asp:CommandField ShowEditButton="True" ValidationGroup="EditPractice" ButtonType="Image"
                ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center" EditImageUrl="~/Images/icon-edit.png"
                EditText="Edit Practice Area" UpdateText="Confirm" UpdateImageUrl="~/Images/icon-check.png"
                CancelImageUrl="~/Images/no.png" CancelText="Cancel" />
            <asp:TemplateField HeaderText="Practice Area Name" SortExpression="Name">
                <ItemStyle Width="30%" Wrap="false" />
                <ItemTemplate>
                    <asp:Label ID="lblPractice" Style="white-space: normal !important;" runat="server"
                        Text='<%# Bind("Name") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="tbEditPractice" runat="server" Text='<%# Bind("Name") %>' Width="100%"
                        ValidationGroup="EditPractice" />
                    <asp:RequiredFieldValidator ID="valPracticeName" runat="server" ValidationGroup="EditPractice"
                        Text="*" ErrorMessage="Name is required" ControlToValidate="tbEditPractice" />
                    <asp:RegularExpressionValidator ID="regValPracticeName1" ControlToValidate="tbEditPractice"
                        Text="*" runat="server" ValidationGroup="EditPractice" ValidationExpression="^[\s\S]{0,100}$"
                        ErrorMessage="Practice area name should not be more than 100 characters." />
                    <asp:CustomValidator ID="custValEditPractice" runat="server" ValidationGroup="EditPractice"
                        Text="*" ErrorMessage="This practice area already exists. Please enter a different practice."
                        ToolTip="This practice area already exists. Please enter a different practice." />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Active">
                <ItemStyle HorizontalAlign="Center" Width="7%" />
                <ItemTemplate>
                    <asp:CheckBox ID="chbIsActive" runat="server" Enabled="false" Checked='<%# ((PracticeExtended)Container.DataItem).IsActive %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chbIsActiveEd" runat="server" Checked='<%# Bind("IsActive") %>' />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Used" Visible="false">
                <ItemStyle HorizontalAlign="Center" Width="0%" />
                <ItemTemplate>
                    <%# ((PracticeExtended) Container.DataItem).InUse ? "Yes" : "No" %>
                </ItemTemplate>
                <EditItemTemplate>
                    <%# ((PracticeExtended) Container.DataItem).InUse ? "Yes" : "No"%>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Internal" ItemStyle-HorizontalAlign="Center">
                <ItemStyle HorizontalAlign="Center" Width="7%" />
                <ItemTemplate>
                    <asp:CheckBox ID="chbIsCompanyInternal" runat="server" Enabled="false" Checked='<%# ((PracticeExtended) Container.DataItem).IsCompanyInternal %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:CheckBox ID="chbInternal" runat="server" Checked='<%# Bind("IsCompanyInternal") %>' />
                    <%--Enabled='<%# _userIsAdmin %>'--%>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Width="45%" />
                <HeaderTemplate>
                    Practice Area Owner (Status)
                </HeaderTemplate>
                <ItemTemplate>
                    <%# ((Practice) Container.DataItem).PracticeOwner.PersonLastFirstName %>
                    (<%# ((Practice) Container.DataItem).PracticeOwner.Status.Name %>)
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlActivePersons" runat="server" DataSourceID="odsActivePersons"
                        Width="100%" DataValueField="Id" DataTextField="PersonLastFirstName">
                    </asp:DropDownList>
                    <asp:HiddenField ID="hfPracticeOwner" runat="server" Value='<%#Bind("PracticeManagerId")%>' />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="True" ButtonType="Image" ItemStyle-Width="4%"
                ItemStyle-HorizontalAlign="Center" DeleteImageUrl="~/Images/icon-delete.png" />
        </Columns>
    </asp:GridView>
    <asp:Panel ID="pnlInsertPractice" runat="server" Wrap="False">
        <table width="75%" class="CompPerfTable" cellspacing="0" border="0" style="background-color: White;
            border-collapse: collapse;">
            <tr style="background-color: #F9FAFF;">
                <td align="center" style="width: 7%; padding-top: 10px;">
                    <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                        ToolTip="Add Practice Area" Visible="true" />
                    <asp:ImageButton ID="btnInsert" runat="server" ValidationGroup="InsertPractice" ImageUrl="~/Images/icon-check.png"
                        ToolTip="Confirm" Visible="false" OnClick="btnInsert_Click" OnClientClick="return hideSuccessMessage();" />
                    <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
                        ToolTip="Cancel" Visible="false" />
                </td>
                <td style="width: 30%; white-space: nowrap;">
                    <asp:TextBox ID="tbPracticeName" ValidationGroup="InsertPractice" runat="server"
                        Width="100%" Visible="false" />
                    <asp:RequiredFieldValidator ID="valPracticeName" runat="server" ValidationGroup="InsertPractice"
                        Text="*" ErrorMessage="Name is required" ControlToValidate="tbPracticeName" />
                    <asp:RegularExpressionValidator ID="regValPracticeName" ControlToValidate="tbPracticeName"
                        Text="*" runat="server" ValidationGroup="InsertPractice" ValidationExpression="^[\s\S]{0,100}$"
                        ErrorMessage="Practice area Name should not be more than 100 characters." />
                    <asp:CustomValidator ID="cvPracticeName" runat="server" ControlToValidate="tbPracticeName"
                        Text="*" OnServerValidate="cvPracticeName_OnServerValidate" ValidationGroup="InsertPractice"
                        ErrorMessage="This Practice area already exists. Please add a different Practice area" />
                </td>
                <td align="center" style="width: 7%;">
                    <asp:CheckBox ID="chbPracticeActive" runat="server" Checked="true" Visible="false" />
                </td>
                <td align="center" style="width: 7%;">
                    <asp:CheckBox ID="chbIsInternalPractice" runat="server" Checked="false" Visible="false" />
                </td>
                <td style="width: 45%;">
                    <asp:DropDownList ID="ddlPracticeManagers" runat="server" DataValueField="Id" DataTextField="PersonLastFirstName"
                        Width="100%" Visible="false" DataSourceID="odsActivePersons" />
                </td>
                <td style="width: 4%;">
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:ValidationSummary ID="valSummaryInsert" ValidationGroup="InsertPractice" runat="server" />
    <asp:ValidationSummary ID="valSummaryEdit" ValidationGroup="EditPractice" runat="server" />
    <uc:Label ID="mlInsertStatus" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
    <asp:ObjectDataSource ID="odsPractices" runat="server" SelectMethod="GetAllPractices"
        TypeName="PraticeManagement.Controls.Configuration.PracticesHelper" DataObjectTypeName="PraticeManagement.Controls.Configuration.PracticeExtended"
        DeleteMethod="RemovePracticeEx" UpdateMethod="UpdatePracticeEx"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsActivePersons" runat="server" SelectMethod="PersonListShortByRoleAndStatus"
        TypeName="PraticeManagement.PersonService.PersonServiceClient">
        <SelectParameters>
            <asp:Parameter DefaultValue="1" Name="statusId" Type="Int32" />
            <asp:Parameter DefaultValue="Practice Area Manager" Name="roleName" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

