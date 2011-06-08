<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="OpportunityPriorities.aspx.cs" Inherits="PraticeManagement.Config.OpurtunityPriorities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Opportunity Priorities</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Opurtunity Priorities
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
                Width="63%" CssClass="CompPerfTable" DataKeyNames="Id" OnRowDataBound="gvOpportunityPriorities_RowDataBound"
                GridLines="None" BackColor="White">
                <AlternatingRowStyle BackColor="#F9FAFF" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <ItemStyle Width="6%" Height="20px" HorizontalAlign="Center" Wrap="false" />
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
                                Opportunity Priority
                            </div>
                        </HeaderTemplate>
                        <ItemStyle Width="26%" Height="20px" HorizontalAlign="Center" Wrap="false" />
                        <ItemTemplate>
                            <asp:Label ID="lblOpportunityPriority" Style="white-space: normal !important; padding-left: 5px;"
                                runat="server" Text='<%# Eval("Priority") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlOpportunityPriority" onchange="SethdnFieldValue(this.SelectedValue,this);"
                                Width="35%" runat="server">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Priority Description (Optional)
                            </div>
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Left" Height="20px" Width="63%" Wrap="false" />
                        <ItemTemplate>
                            <asp:Label ID="lblPriorityDescription" Style="white-space: normal !important;" runat="server"
                                Text='<%# Eval("Description") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditPriorityDescription" MaxLength="100" runat="server" Text='<%# Eval("Description") %>'
                                Width="95%" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server"
                                TargetControlID="tbEditPriorityDescription" WatermarkCssClass="watermarkedtext"
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
                        <ItemStyle HorizontalAlign="Center" Height="20px" Width="5%" Wrap="false" />
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
                <table width="63%"   class="CompPerfTable" cellspacing="0" border="0" style="background-color: White;
                    border-collapse: collapse;">
                    <tr style="background-color: #F9FAFF;">
                        <td align="center" style="width: 6%; padding-top: 10px;">
                            <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                                ToolTip="Add Opportunity Priority" Visible="true" />
                            <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
                                ToolTip="Save" Visible="false" OnClick="btnInsert_Click" />
                            <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
                                ToolTip="Cancel" Visible="false" />
                        </td>
                        <td align="center" style="width: 26%; white-space: nowrap;">
                            <asp:DropDownList ID="ddlInsertOpportunityPriority" Width="35%" Enabled="false" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="left" style="width: 63%;">
                            <asp:TextBox ID="tbInsertPriorityDescription" MaxLength="100" runat="server" Enabled="false"
                                Width="95%" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server"
                                TargetControlID="tbInsertPriorityDescription" WatermarkCssClass="watermarkedtext"
                                WatermarkText="Enter text here to define the opportunity priority">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                        </td>
                        <td style="width: 5%;">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlOpportunityPriorities" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass" Style="display: none" BorderWidth="2px">
                <table width="100%">
                    <tr style="background-color: Gray; height: 27px;">
                        <td align="center" style="white-space: nowrap; font-size: 14px; width: 100%">
                            Assign Priority to Opportunities
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 10px;">
                            <span>This priority is linked to existing opportunities already and can not be deleted<br />
                                without first assigning an new priority to these opportunities. Please select a<br />
                                replacement from the drop-down below then click OK.<br />
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 6px 6px 2px 2px;">
                            <asp:DropDownList ID="ddlOpportunityPriorities" runat="server" AutoPostBack="false"
                                Style="width: 100px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 6px 6px 2px 2px; white-space: nowrap;">
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

