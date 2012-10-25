<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="PracticeAreas.aspx.cs" Inherits="PraticeManagement.Config.PracticeAreas" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="PraticeManagement.Controls.Configuration" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register TagPrefix="uc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Areas | Practice Management</title>
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
    </div>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="updTimeEntries" runat="server">
        <ContentTemplate>
            <asp:GridView ID="gvPractices" runat="server" AutoGenerateColumns="False" DataSourceID="odsPractices"
                CssClass="CompPerfTable gvPractices" DataKeyNames="Id" OnRowUpdating="gvPractices_OnRowUpdating"
                OnRowUpdated="gvPractices_RowUpdated" OnRowDeleted="gvPractices_RowDeleted" OnRowEditing="gvPractices_OnRowEditing"
                OnRowDataBound="gvPractices_RowDataBound">
                <AlternatingRowStyle CssClass="alterrow" />
                <Columns>
                    <asp:CommandField ShowEditButton="True" ValidationGroup="EditPractice" ButtonType="Image"
                        HeaderStyle-CssClass="Width7Percent" EditImageUrl="~/Images/icon-edit.png" EditText="Edit Practice Area"
                        UpdateText="Confirm" UpdateImageUrl="~/Images/icon-check.png" CancelImageUrl="~/Images/no.png"
                        CancelText="Cancel" />
                    <asp:TemplateField HeaderText="Practice Area Name" SortExpression="Name">
                        <HeaderStyle CssClass="Width28Percent padRight10" />
                        <ItemStyle CssClass="Left no-wrap padRight10" />
                        <ItemTemplate>
                            <asp:Label ID="lblPractice" runat="server" CssClass="WS-Normal" Text='<%# Bind("HtmlEncodedName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditPractice" runat="server" Text='<%# Bind("Name") %>' CssClass="Width95Percent"
                                ValidationGroup="EditPractice" />
                            <asp:RequiredFieldValidator ID="valPracticeName" runat="server" ValidationGroup="EditPractice"
                                EnableClientScript="false" Display="Dynamic" ToolTip="Practice Area name is required."
                                Text="*" ErrorMessage="Practice Area name is required." ControlToValidate="tbEditPractice" />
                            <asp:RegularExpressionValidator ID="regValPracticeName1" ControlToValidate="tbEditPractice"
                                Display="Dynamic" ToolTip="Practice Area name should not be more than 100 characters."
                                Text="*" runat="server" ValidationGroup="EditPractice" ValidationExpression="^[\s\S]{0,100}$"
                                ErrorMessage="Practice Area name should not be more than 100 characters." />
                            <asp:CustomValidator ID="custValEditPractice" runat="server" ValidationGroup="EditPractice"
                                Display="Dynamic" Text="*" ErrorMessage="This practice area already exists. Please enter a different practice."
                                ToolTip="This practice area already exists. Please enter a different practice." />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Abbreviation">
                        <HeaderStyle CssClass="Width7Percent" />
                        <ItemStyle CssClass="Left no-wrap padLeft20" />
                        <ItemTemplate>
                            <asp:Label ID="lblAbbreviation" runat="server" CssClass="WS-Normal" Text='<%# Bind("Abbreviation") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditAbbreviation" runat="server" Text='<%# Bind("Abbreviation") %>'
                                CssClass="Width95Percent" ValidationGroup="EditPractice" />
                            <asp:RegularExpressionValidator ID="regValAbbreviation" ControlToValidate="tbEditAbbreviation"
                                Display="Dynamic" Text="*" runat="server" ValidationGroup="EditPractice" ValidationExpression="^[\s\S]{0,100}$"
                                ToolTip="Practice area Abbreviation should not be more than 100 characters."
                                ErrorMessage="Practice area Abbreviation should not be more than 100 characters." />
                            <asp:CustomValidator ID="custValEditPracticeAbbreviation" runat="server" ValidationGroup="EditPractice"
                                Display="Dynamic" Text="*" ErrorMessage="This practice area abbreviation already exists. Please enter a different practice area abbreviation."
                                ToolTip="This practice area abbreviation already exists. Please enter a different practice area abbreviation." />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active">
                        <HeaderStyle CssClass="Width7Percent" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbIsActive" runat="server" Enabled="false" Checked='<%# ((PracticeExtended)Container.DataItem).IsActive %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chbIsActiveEd" runat="server" Checked='<%# Bind("IsActive") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Used" Visible="false">
                        <HeaderStyle CssClass="Width0Percent" />
                        <ItemTemplate>
                            <%# ((PracticeExtended) Container.DataItem).InUse ? "Yes" : "No" %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <%# ((PracticeExtended) Container.DataItem).InUse ? "Yes" : "No"%>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Internal">
                        <HeaderStyle CssClass="Width7Percent" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbIsCompanyInternal" runat="server" Enabled="false" Checked='<%# ((PracticeExtended) Container.DataItem).IsCompanyInternal %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chbInternal" runat="server" Checked='<%# Bind("IsCompanyInternal") %>' />
                            <%--Enabled='<%# _userIsAdmin %>'--%>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="Width40P" />
                        <ItemStyle CssClass="Left" />
                        <HeaderTemplate>
                            Practice Area Owner (Status)
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# ((Practice) Container.DataItem).PracticeOwner.PersonLastFirstName %>
                            (<%# ((Practice) Container.DataItem).PracticeOwner.Status.Name %>)
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlActivePersons" runat="server" DataSourceID="odsActivePersons"
                                CssClass="Width95Percent" DataValueField="Id" DataTextField="PersonLastFirstName">
                            </asp:DropDownList>
                            <asp:HiddenField ID="hfPracticeOwner" runat="server" Value='<%#Bind("PracticeManagerId")%>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowDeleteButton="True" ButtonType="Image" HeaderStyle-CssClass="Width4Percent"
                        DeleteImageUrl="~/Images/icon-delete.png" />
                </Columns>
            </asp:GridView>
            <asp:Panel ID="pnlInsertPractice" runat="server" Wrap="False">
                <table class="CompPerfTable gvPractices" cellspacing="0">
                    <tr id="trInsertPractice" runat="server" class="alterrow">
                        <td class="Width7Percent PaddingTop10">
                            <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                                ToolTip="Add Practice Area" Visible="true" />
                            <asp:ImageButton ID="btnInsert" runat="server" ValidationGroup="InsertPractice" ImageUrl="~/Images/icon-check.png"
                                ToolTip="Confirm" Visible="false" OnClick="btnInsert_Click" OnClientClick="return hideSuccessMessage();" />
                            <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
                                ToolTip="Cancel" Visible="false" />
                        </td>
                        <td class="Width28Percent Left padRight10">
                            <asp:TextBox ID="tbPracticeName" ValidationGroup="InsertPractice" runat="server"
                                CssClass="Width95Percent" Visible="false" />
                            <asp:RequiredFieldValidator ID="valPracticeName" runat="server" ValidationGroup="InsertPractice"
                                ToolTip="Practice Area name is required." Display="Dynamic" Text="*" ErrorMessage="Practice Area name is required."
                                ControlToValidate="tbPracticeName" />
                            <asp:RegularExpressionValidator ID="regValPracticeName" ControlToValidate="tbPracticeName"
                                Display="Dynamic" Text="*" runat="server" ValidationGroup="InsertPractice" ValidationExpression="^[\s\S]{0,100}$"
                                ToolTip="Practice Area name should not be more than 100 characters." ErrorMessage="Practice Area name should not be more than 100 characters." />
                            <asp:CustomValidator ID="cvPracticeName" runat="server" ControlToValidate="tbPracticeName"
                                Display="Dynamic" Text="*" OnServerValidate="cvPracticeName_OnServerValidate"
                                ToolTip="This practice area already exists. Please enter a different practice."
                                ValidationGroup="InsertPractice" ErrorMessage="This practice area already exists. Please enter a different practice." />
                        </td>
                        <td class="Width7Percent Left padLeft20">
                            <asp:TextBox ID="tbAbbreviation" ValidationGroup="InsertPractice" runat="server"
                                CssClass="Width95Percent" Visible="false" />
                            <asp:RegularExpressionValidator ID="regValAbbreviation" ControlToValidate="tbAbbreviation"
                                Display="Dynamic" Text="*" runat="server" ValidationGroup="EditPractice" ValidationExpression="^[\s\S]{0,100}$"
                                ToolTip="Practice area Abbreviation should not be more than 100 characters."
                                ErrorMessage="Practice area Abbreviation should not be more than 100 characters." />
                            <asp:CustomValidator ID="custValEditPracticeAbbreviation" runat="server" ValidationGroup="EditPractice"
                                Display="Dynamic" Text="*" ErrorMessage="This practice area abbreviation already exists. Please enter a different practice area abbreviation."
                                ToolTip="This practice area abbreviation already exists. Please enter a different practice area abbreviation." />
                        </td>
                        <td class="Width7Percent">
                            <asp:CheckBox ID="chbPracticeActive" runat="server" Checked="true" Visible="false" />
                        </td>
                        <td class="Width7Percent">
                            <asp:CheckBox ID="chbIsInternalPractice" runat="server" Checked="false" Visible="false" />
                        </td>
                        <td class="Width40P Left">
                            <asp:DropDownList ID="ddlPracticeManagers" runat="server" DataValueField="Id" DataTextField="PersonLastFirstName"
                                CssClass="Width95Percent" Visible="false" DataSourceID="odsActivePersons" />
                        </td>
                        <td class="Width4Percent">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                CancelControlID="btnCancelErrorPanel" DropShadow="false" />
            <asp:Panel ID="pnlErrorPanel" runat="server" Style="display: none;" CssClass="ProjectDetailErrorPanel PanelPerson">
                <table class="Width100Per">
                    <tr>
                        <th class="bgcolorGray TextAlignCenterImp vBottom">
                            <b class="BtnClose">Attention!</b>
                            <asp:Button ID="btnCancelErrorPanel" runat="server" CssClass="mini-report-close floatright"
                                ToolTip="Cancel" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10Px">
                            <asp:ValidationSummary ID="valSummaryInsert" ValidationGroup="InsertPractice" runat="server"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a practice." />
                            <asp:ValidationSummary ID="valSummaryEdit" ValidationGroup="EditPractice" runat="server"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a practice." />
                            <uc:Label ID="mlInsertStatus" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10Px TextAlignCenterImp">
                            <asp:Button ID="btnOKErrorPanel" runat="server" Text="OK" Width="100" OnClientClick="$find('mpeErrorPanelBehaviourId').hide();return false;" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:ObjectDataSource ID="odsPractices" runat="server" SelectMethod="GetAllPractices"
                TypeName="PraticeManagement.Controls.Configuration.PracticesHelper" DataObjectTypeName="PraticeManagement.Controls.Configuration.PracticeExtended"
                DeleteMethod="RemovePracticeEx" UpdateMethod="UpdatePracticeEx"></asp:ObjectDataSource>
            <asp:ObjectDataSource ID="odsActivePersons" runat="server" SelectMethod="PersonListShortByRoleAndStatus"
                TypeName="PraticeManagement.PersonService.PersonServiceClient">
                <SelectParameters>
                    <asp:Parameter DefaultValue="1,5" Name="statusIds" Type="String" />
                    <asp:Parameter DefaultValue="Practice Area Manager" Name="roleName" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

