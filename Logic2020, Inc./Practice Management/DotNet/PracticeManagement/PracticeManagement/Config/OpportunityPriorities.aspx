<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="OpportunityPriorities.aspx.cs" Inherits="PraticeManagement.Config.OpurtunityPriorities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Opportunity Sales Stages | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Opportunity Sales Stages
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">

        function canShowConfirm(popupMsg) {
            var hdnfield = document.getElementById('<%= hdnField.ClientID %>');
            if (hdnfield.value == "true") {
                if (confirm(popupMsg)) {
                    hdnfield.value = "false";
                    return true;
                }
                else {
                    return false;
                }
            }
            return true;
        }

        function SethdnFieldValue(oldValue, ddl) {
            for (var i = 0; i < ddl.length; i++) {
                if (ddl[i].selected) {
                    var hdnfield = document.getElementById('<%= hdnField.ClientID %>');
                    if (ddl[i].value != oldValue) {
                        hdnfield.value = "true";
                        break;
                    }
                    else {
                        hdnfield.value = "false";
                    }
                }
            }
        }
    </script>
    <asp:UpdatePanel ID="upnlOpportunityPriority" runat="server">
        <ContentTemplate>
            <asp:GridView ID="gvOpportunityPriorities" runat="server" AutoGenerateColumns="False"
                CssClass="CompPerfTable gvOpportunityPriorities" DataKeyNames="Id" OnRowDataBound="gvOpportunityPriorities_RowDataBound">
                <AlternatingRowStyle CssClass="alterrow" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width6Percent" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgEditPriority" ToolTip="Edit Opportunity Priority" runat="server"
                                OnClick="imgEditPriority_OnClick" ImageUrl="~/Images/icon-edit.png" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:ImageButton ID="imgUpdatePriority" PriorityId='<%# Eval("Id") %>' ToolTip="Save"
                                runat="server" ImageUrl="~/Images/icon-check.png" OnClick="imgUpdatePriority_OnClick" />
                            <asp:ImageButton ID="imgCancel" ToolTip="Cancel" runat="server" ImageUrl="~/Images/no.png"
                                OnClick="imgCancel_OnClick" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Sales Stage
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width20Percent" />
                        <ItemTemplate>
                            <asp:Label ID="lblDisplayName" CssClass="WS-Normal padLeft5" runat="server" Text='<%# Eval("HtmlEncodedDisplayName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditDisplayName" MaxLength="100" runat="server" Text='<%# Eval("DisplayName") %>'
                                CssClass="Width95Percent" OldName='<%# Eval("DisplayName") %>' />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtDisplayName" runat="server"
                                TargetControlID="tbEditDisplayName" WatermarkCssClass="watermarkedtext Width95Percent"
                                WatermarkText="Please enter Sales Stage name">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RequiredFieldValidator ID="reqDisplayName" runat="server" ControlToValidate="tbEditDisplayName"
                                ErrorMessage="Sales Stage is required." ToolTip="Sales Stage is required." EnableClientScript="false"
                                SetFocusOnError="true" Text="*" ValidationGroup="EditSalesStage"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ControlToValidate="tbEditDisplayName" ValidationGroup="EditSalesStage"
                                ID="valRegDisplayName" runat="server" ErrorMessage="Display Name should be limited to 15 characters in length."
                                ToolTip="Display Name should be limited to 15 characters in length." EnableClientScript="false"
                                Display="Dynamic" Text="*" ValidationExpression="^[a-zA-Z0-9 ]{0,15}$" SetFocusOnError="true" />
                            <asp:CustomValidator ID="cvUniquesDisplayName" runat="server" ControlToValidate="tbEditDisplayName"
                                ErrorMessage="This Sales Stage already exists.Please enter different Sales Stage."
                                ToolTip="This Sales Stage already exists.Please enter different Sales Stage."
                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="EditSalesStage" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Opportunity Order
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width20Percent" />
                        <ItemTemplate>
                            <asp:Label ID="lblOpportunityPriority" CssClass="WS-Normal padLeft5" runat="server"
                                Text='<%# Eval("Priority") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlOpportunityPriority" onchange="SethdnFieldValue(this.SelectedValue,this);"
                                CssClass="Width35Percent" runat="server">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Sales Stage Description (Optional)
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width50Percent" />
                        <ItemStyle CssClass="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblPriorityDescription" CssClass="WS-Normal" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Description")) %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditPriorityDescription" MaxLength="100" runat="server" Text='<%# Eval("Description") %>'
                                CssClass="Width95Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server"
                                TargetControlID="tbEditPriorityDescription" WatermarkCssClass="watermarkedtext Width95Percent"
                                WatermarkText="Enter text here to define the opportunity priority">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width4Percent" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgDeletePriority" PriorityId='<%# Eval("Id") %>' InUse='<%# Eval("InUse") %>'
                                ToolTip="Delete Opportunity Priority" runat="server" OnClick="imgDeletePriority_OnClick"
                                ImageUrl="~/Images/icon-delete.png" />
                        </ItemTemplate>
                        <EditItemTemplate>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:HiddenField ID="hdnField" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeOpportunityPriorities" runat="server"
                TargetControlID="hdnField" CancelControlID="buttonCancel" BackgroundCssClass="modalBackground"
                PopupControlID="pnlOpportunityPriorities" DropShadow="false" />
            <asp:Panel ID="pnlInsertPriority" runat="server" Wrap="False">
                <table class="CompPerfTable gvOpportunityPriorities" cellspacing="0">
                    <tr class="alterrow">
                        <td class="Width6Percent PaddingTop10 vMiddle">
                            <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                                ToolTip="Add Opportunity Priority" Visible="true" />
                            <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
                                ToolTip="Save" Visible="false" OnClick="btnInsert_Click" />
                            <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
                                ToolTip="Cancel" Visible="false" />
                        </td>
                        <td class="Width20Percent">
                            <asp:TextBox ID="tdInsertDisplayName" MaxLength="100" runat="server" Enabled="false"
                                class="Width95Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtDisplayName" runat="server"
                                TargetControlID="tdInsertDisplayName" WatermarkCssClass="watermarkedtext Width95Percent"
                                WatermarkText="Please enter Sales Stage name">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RequiredFieldValidator ID="reqDisplayName" runat="server" ControlToValidate="tdInsertDisplayName"
                                ErrorMessage="Sales Stage is required." ToolTip="Sales Stage is required." EnableClientScript="false"
                                SetFocusOnError="true" Text="*" ValidationGroup="AddSalesStage"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ControlToValidate="tdInsertDisplayName" ValidationGroup="AddSalesStage"
                                ID="valRegDisplayName" runat="server" ErrorMessage="Display Name should be limited to 15 characters in length."
                                ToolTip="Display Name should be limited to 15 characters in length." EnableClientScript="false"
                                Text="*" ValidationExpression="^[a-zA-Z0-9 ]{0,15}$" Display="Dynamic" />
                            <asp:CustomValidator ID="cvInsertUniqueDisplayName" runat="server" ControlToValidate="tdInsertDisplayName"
                                ErrorMessage="This Sales Stage already exists.Please enter different Sales Stage."
                                ToolTip="This Sales Stage already exists.Please enter different Sales Stage."
                                EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="AddSalesStage" />
                        </td>
                        <td class="Width20Percent">
                            <asp:DropDownList ID="ddlInsertOpportunityPriority" CssClass="Width35Percent" Enabled="false"
                                runat="server">
                            </asp:DropDownList>
                        </td>
                        <td class="Width50Percent Left">
                            <asp:TextBox ID="tbInsertPriorityDescription" MaxLength="100" runat="server" Enabled="false"
                                class="Width95Percent" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server"
                                TargetControlID="tbInsertPriorityDescription" WatermarkCssClass="watermarkedtext Width95Percent"
                                WatermarkText="Enter text here to define the opportunity sales stage">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                        </td>
                        <td class="Width4Percent">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:ValidationSummary ID="valSummarySalesStageAdd" runat="server" ValidationGroup="AddSalesStage"
                EnableClientScript="false" SetFocusOnError="false" />
            <asp:ValidationSummary ID="valSummaryEditSalesStage" runat="server" ValidationGroup="EditSalesStage"
                EnableClientScript="false" SetFocusOnError="false" />
            <asp:Panel ID="pnlOpportunityPriorities" runat="server" CssClass="popUp OpportunityPrioritiesPopUp"
                Style="display: none">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th>
                            Assign Sales Stage to Opportunities
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10">
                            <span>This Sales Stage is linked to existing opportunities already and can not be deleted<br />
                                without first assigning an new Sales Stage to these opportunities. Please select
                                a<br />
                                replacement from the drop-down below then click OK.<br />
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="OpportunityPrioritiesPopUpTd">
                            <asp:DropDownList ID="ddlOpportunityPriorities" runat="server" AutoPostBack="false"
                                CssClass="Width100Px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="OpportunityPrioritiesPopUpTd no-wrap">
                            <asp:Button ID="btnOK" runat="server" Text="OK" OnClick="btnOK_Click" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="buttonCancel" runat="server" Text="Cancel" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

