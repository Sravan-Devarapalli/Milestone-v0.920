<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ProjectDetail.aspx.cs" Inherits="PraticeManagement.ProjectDetail"
    Title="Practice Management - Project Details" EnableEventValidation="false" ValidateRequest="False" %>

<%@ Register Src="Controls/BillingInfo.ascx" TagName="BillingInfo" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.ElementDisabler"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectMilestonesFinancials.ascx"
    TagName="ProjectMilestonesFinancials" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectFinancials.ascx" TagName="ProjectFinancials" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectPersons.ascx" TagName="ProjectPersons" %>
<%@ Register Src="~/Controls/Generic/Notes.ascx" TagName="Notes" TagPrefix="uc" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Project Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript">
        function checkDirty(target, entityId) {
            if (showDialod()) {
                __doPostBack('__Page', target + ':' + entityId);
                return true;
            }

            return false;
        }

        function cvProjectAttachment_ClientValidationFunction(obj, args) {
            var fuControl = document.getElementById('<%= fuProjectAttachment.ClientID %>');
            var FileUploadPath = fuControl.value;
            var Extension = FileUploadPath.substring(FileUploadPath.lastIndexOf('.') + 1).toLowerCase();
            if (Extension == "pdf") {
                args.IsValid = true; // Valid file type
            }
            else {
                args.IsValid = false; // Not valid file type
            }
        }

        function EnableUploadButton() {
            var cvProjectAttachment = document.getElementById('<%= cvProjectAttachment.ClientID %>');
            var UploadButton = document.getElementById('<%= btnUpload.ClientID %>');
            if (cvProjectAttachment.isvalid) {
                UploadButton.disabled = "";
            }
            else {
                UploadButton.disabled = "disabled";
            }
        }

        function CanShowPrompt() {
            var hlnk = document.getElementById('<%= hlnkProjectAttachment.ClientID %>');
            var lnk = document.getElementById('<%= lnkProjectAttachment.ClientID %>');
            var showPrompt = false;

            if (lnk != null && lnk.value != "") {
                showPrompt = true;
            }

            if (!(hlnk.href == "")) {
                showPrompt = true;
            }

            if (showPrompt) {
                var result = confirm("SOW already exists for this project. Click Ok to replace the file or Cancel to continue without replacing.");
                if (result == true) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return true;
            }
        }

        function ConfirmToDeleteProject() {
            var hdnProject = document.getElementById('<%= hdnProjectDelete.ClientID %>');
            var result = confirm("Do you really want to delete the project?");
            hdnProject.value = result ? 1 : 0;
        }

    </script>
    <style type="text/css">
        /* --------- Tabs for person and project details pages ------ */
        
        .CustomTabStyle tr
        {
            height: 30px;
        }
        
        .CustomTabStyle td
        {
            background-color: White;
            float: left;
            padding: 8px 0px 6px 0px;
            position: relative;
        }
        
        .CustomTabStyle td a
        {
            text-decoration: none;
        }
        
        .CustomTabStyle td span a
        {
            border-bottom: 1px dashed #0898e6;
        }
        
        .CustomTabStyle td span a:hover
        {
            border-bottom: 1px dashed #006699;
        }
        
        .CustomTabStyle td a.collapse
        {
            display: none;
            position: absolute;
        }
        
        .CustomTabStyle .SelectedSwitch a.collapse
        {
            display: block;
            right: 5px;
            top: 10px;
        }
        
        .CustomTabStyle td span.bg
        {
            padding: 8px 20px 7px 10px;
        }
        
        .CustomTabStyle .SelectedSwitch span.bg
        {
            background-color: #e2ebff;
        }
        
        .tab-pane
        {
            background-color: #e2ebff;
            padding: 5px;
        }
        
        /* ------------------------ */
        
        table.ProjectDetail-ProjectInfo-Table td
        {
            padding-left: 4px;
        }
    </style>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td>
                        <asp:Panel ID="pnlProjectInfo" runat="server">
                            <table class="ProjectDetail-ProjectInfo-Table">
                                <tr>
                                    <td nowrap="nowrap">
                                        Project Name
                                    </td>
                                    <td style="width: 200px">
                                        <asp:TextBox ID="txtProjectName" runat="server" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqProjectName" runat="server" ControlToValidate="txtProjectName"
                                            ErrorMessage="The Project Name is required." ToolTip="The Project Name is required."
                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    </td>
                                    <td nowrap="nowrap">
                                        Duration
                                    </td>
                                    <td style="width: 200px">
                                        <asp:Label ID="lblProjectStart" Font-Bold="true" runat="server" Text="Undefined" />&mdash;
                                        <asp:Label ID="lblProjectEnd" Font-Bold="true" runat="server" Text="Undefined" />
                                    </td>
                                    <td colspan="2">
                                        Status&nbsp;
                                    </td>
                                    <td colspan="4">
                                        <asp:DropDownList ID="ddlProjectStatus" runat="server" onchange="setDirty();" AutoPostBack="True"
                                            OnSelectedIndexChanged="DropDown_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqProjectStatus" runat="server" ControlToValidate="ddlProjectStatus"
                                            ErrorMessage="The Status is required." ToolTip="The Status is required." ValidationGroup="Project"
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="custProjectStatus" runat="server" ControlToValidate="ddlProjectStatus"
                                            ErrorMessage="Only administrators can make projects Active or Completed." ToolTip="Only administrators can make projects Active or Completed."
                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                            Display="Dynamic" OnServerValidate="custProjectStatus_ServerValidate"></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Client
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlClientName" runat="server" CssClass="WholeWidth" OnSelectedIndexChanged="ddlClientName_SelectedIndexChanged"
                                            AutoPostBack="True" onchange="setDirty();">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqClientName" runat="server" ControlToValidate="ddlClientName"
                                            ErrorMessage="The Client Name is required." ToolTip="The Client Name is required."
                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        Group
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlProjectGroup" runat="server" CssClass="WholeWidth" OnSelectedIndexChanged="ddlProjectGroup_SelectedIndexChanged"
                                            AutoPostBack="true">
                                        </asp:DropDownList>
                                    </td>
                                    <td colspan="2" style="white-space: nowrap">
                                        Practice Area
                                    </td>
                                    <td colspan="3">
                                        <asp:DropDownList ID="ddlPractice" runat="server" onchange="setDirty();" CssClass="WholeWidth"
                                            AutoPostBack="True" OnSelectedIndexChanged="DropDown_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                            EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Practice Area is required."
                                            SetFocusOnError="true" Text="*" ToolTip="The Practice Area is required."></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">
                                        Salesperson
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSalesperson" runat="server" AutoPostBack="True" CssClass="WholeWidth"
                                            onchange="setDirty();" OnSelectedIndexChanged="ddlSalesperson_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="hidSalesCommissionId" runat="server" />
                                    </td>
                                    <td>
                                    </td>
                                    <td style="white-space: nowrap;">
                                        Client Director
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlDirector" runat="server" CssClass="WholeWidth" onchange="setDirty();">
                                        </asp:DropDownList>
                                    </td>
                                    <td colspan="2">
                                        Owner
                                    </td>
                                    <td colspan="3">
                                        <asp:DropDownList ID="ddlProjectManager" runat="server" CssClass="WholeWidth" onchange="setDirty();"
                                            DataSourceID="odsActivePersons" DataValueField="Id" DataTextField="PersonLastFirstName" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">
                                        Project Number
                                    </td>
                                    <td>
                                        <asp:Label ID="lblProjectNumber" runat="server"></asp:Label>
                                        <asp:HiddenField ID="hidPracticeManagementCommissionId" runat="server" />
                                    </td>
                                    <td colspan="3">
                                        &nbsp;
                                    </td>
                                    <td colspan="4">
                                        <asp:CheckBox ID="chbIsChargeable" runat="server" runat="server" onclick="setDirty();"
                                            Text="Milestones in this project are billable by default" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">
                                        Buyer Name
                                    </td>
                                    <td colspan="4">
                                        <asp:TextBox ID="txtBuyerName" runat="server" CssClass="WholeWidth" onchange="setDirty();"
                                            MaxLength="100"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                            ErrorMessage="The Buyer Name is required." ToolTip="The Buyer Name is required."
                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="valregBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                            ErrorMessage="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                                            ToolTip="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                            Display="Dynamic" ValidationExpression="^[a-zA-Z'\-]{2,30}$"></asp:RegularExpressionValidator>
                                    </td>
                                    <td>
                                        Client discount&nbsp;
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtClientDiscount" runat="server" onchange="setDirty();"></asp:TextBox>
                                        <asp:CompareValidator ID="compClientDiscount" runat="server" ControlToValidate="txtClientDiscount"
                                            ErrorMessage="A number with 2 decimal digits is allowed for the Client Discount."
                                            ToolTip="A number with 2 decimal digits is allowed for the Client Discount."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Project"
                                            Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="middle">
                                        <asp:Button ID="btnAttachSOW" runat="server" Text="Attach SOW" ToolTip="Attach SOW" />
                                    </td>
                                    <td align="left" valign="middle" colspan="5" style="padding-left: 10px; vertical-align: middle;
                                        white-space: nowrap;">
                                        <table>
                                            <tr>
                                                <td style="padding:0px;">
                                                    <asp:HyperLink ID="hlnkProjectAttachment" runat="server"></asp:HyperLink><asp:LinkButton
                                                        ID="lnkProjectAttachment" runat="server" Visible="false" OnClick="lnkProjectAttachment_OnClick" />
                                                </td>
                                                <td style="padding:0px;padding-left:3px;">
                                                    &nbsp;<asp:Label ID="lblAttachmentsize" runat="server"></asp:Label>
                                                </td>
                                                <td style="padding:0px;">
                                                    &nbsp;<asp:Label ID="lblAttachmentUploadedDate" runat="server"></asp:Label>
                                                </td>
                                                <td valign="middle" style="padding:0px;">
                                                    &nbsp;<asp:ImageButton ID="imgbtnDeleteAttachment" OnClick="imgbtnDeleteAttachment_Click"
                                                        OnClientClick="if(confirm('Do you really want to delete the project attachment?')){ return true;}return false;"
                                                        Visible="false" runat="server" ImageUrl="~/Images/trash-icon-Large.png" ToolTip="Delete Attachment" /><AjaxControlToolkit:ModalPopupExtender
                                                            ID="mpeAttachSOW" runat="server" TargetControlID="btnAttachSOW" BackgroundCssClass="modalBackground"
                                                            PopupControlID="pnlAttachSOW" DropShadow="false" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" nowrap="nowrap">
                                        <asp:HyperLink ID="lbOpportunity" Visible="false" runat="server">This project is linked to an opportunity. Click here to open it</asp:HyperLink>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:ShadowedTextButton ID="btnAddMilistone" runat="server" CausesValidation="false"
                            OnClick="btnAddMilistone_Click" CssClass="add-btn" OnClientClick="if (!confirmSaveDirty()) return false;"
                            Text="Add Milestone" />
                        <asp:Table ID="tblProjectDetailTabViewSwitch" runat="server" CssClass="CustomTabStyle">
                            <asp:TableRow ID="rowSwitcher" runat="server">
                                <asp:TableCell ID="cellFinancials" runat="server" CssClass="SelectedSwitch">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnFinancials" runat="server" Text="Financials" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellMilestones" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnMilestones" runat="server" Text="Milestones" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellCommissions" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnCommissions" runat="server" Text="Commissions" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellPersons" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnPersons" runat="server" Text="Persons" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="3"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellBillingInfo" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnBillingInfo" runat="server" Text="Billing Info" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="4"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCellHistoryg" runat="server" Visible="false">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnHstory" runat="server" Text="History" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="5"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellProjectTools" runat="server" Visible="false">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnProjectTools" runat="server" Text="Tools" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="6"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <asp:MultiView ID="mvProjectDetailTab" runat="server" ActiveViewIndex="0">
                            <asp:View ID="vwFinancials" runat="server">
                                <uc:ProjectFinancials ID="financials" runat="server" />
                            </asp:View>
                            <asp:View ID="vwMilestones" runat="server">
                                <asp:Panel ID="pnlRevenueMilestones" runat="server" CssClass="tab-pane">
                                    <uc:ProjectMilestonesFinancials ID="milestones" runat="server" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwCommissions" runat="server">
                                <asp:Panel ID="pnlCommissions" runat="server" CssClass="tab-pane">
                                    <table>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox ID="chbReceivesSalesCommission" runat="server" Text="Receives sales commission of"
                                                    AutoPostBack="True" OnCheckedChanged="chbReceivesSalesCommission_CheckedChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSalesCommission" runat="server" Width="40px" onchange="setDirty();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="reqSalesCommission" runat="server" ControlToValidate="txtSalesCommission"
                                                    ErrorMessage="The Sales Commission is required." ToolTip="The Sales Commission is required."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cstSalesCommission" runat="server" ControlToValidate="txtSalesCommission"
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    ValidationGroup="Project" OnServerValidate="cstSalesCommission_ServerValidate"></asp:CustomValidator>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap" rowspan="2">
                                                <asp:CheckBox ID="chbReceiveManagementCommission" runat="server" Text="receives practice mgmt commission"
                                                    AutoPostBack="True" OnCheckedChanged="chbReceiveManagementCommission_CheckedChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtManagementCommission" runat="server" Width="40px" onchange="setDirty();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="reqManagementCommission" runat="server" ControlToValidate="txtManagementCommission"
                                                    ErrorMessage="The Management Commission is required." ToolTip="The Management Commission is required."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cstManagementCommission" runat="server" ControlToValidate="txtManagementCommission"
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    ValidationGroup="Project" OnServerValidate="cstManagementCommission_ServerValidate"></asp:CustomValidator>
                                            </td>
                                            <td rowspan="2">
                                                based&nbsp;on&nbsp;
                                            </td>
                                            <td rowspan="2">
                                                <asp:RadioButtonList ID="rlstManagementCommission" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="rlstManagementCommission_SelectedIndexChanged">
                                                    <asp:ListItem Text="Own margin" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Sub-ordinate person margin" Value="2" Selected="True"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwPersons" runat="server">
                                <uc:ProjectPersons ID="persons" runat="server" />
                            </asp:View>
                            <asp:View ID="vwBillingInfo" runat="server">
                                <asp:Panel ID="pnlTabBilling" runat="server" CssClass="tab-pane">
                                    <uc1:BillingInfo ID="billingInfo" runat="server" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwHistory" runat="server">
                                <asp:Panel ID="plnTabHistory" runat="server" CssClass="tab-pane">
                                    <uc:Notes ID="nProject" runat="server" Target="Project" GridVisible="false" OnNoteAdded="nOpportunity_OnNoteAdded" />
                                    <uc:ActivityLogControl runat="server" ID="activityLog" DisplayDropDownValue="Project"
                                        DateFilterValue="Year" ShowDisplayDropDown="false" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwProjectTools" runat="server">
                                <asp:Panel ID="pnlTools" runat="server" CssClass="tab-pane">
                                    <table style="background-color: #F9FAFF">
                                        <tr>
                                            <td>
                                                <ul style="list-style: none;">
                                                    <li>
                                                        <asp:CheckBox ID="chbCloneMilestones" runat="server" Checked="true" Text="Clone milestones and milestone person details" /></li>
                                                    <li>
                                                        <asp:CheckBox ID="chbCloneCommissions" runat="server" Checked="true" Text="Clone commissions" /></li>
                                                    <li>
                                                        <asp:CheckBox ID="chbCloneBillingInfo" runat="server" Checked="true" Text="Clone billing info" /></li>
                                                    <li>
                                                        <asp:CheckBox ID="chbCloneNotes" runat="server" Checked="true" Text="Clone notes" /></li>
                                                    <li>Clone status
                                                        <asp:DropDownList ID="ddlCloneProjectStatus" runat="server" DataSourceID="odsProjectStatus"
                                                            DataTextField="Name" DataValueField="Id" />
                                                    </li>
                                                </ul>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="lnkClone" runat="server" Text="Clone *" OnClick="lnkClone_OnClick" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span style="color: Gray">* You will be redirected to the cloned project after you click
                                                    the button.</span>
                                                <ext:ElementDisablerExtender ID="edeCloneButton" runat="server" TargetControlID="lnkClone"
                                                    ControlToDisableID="lnkClone" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:ObjectDataSource ID="odsProjectStatus" runat="server" SelectMethod="GetProjectStatuses"
                                        TypeName="PraticeManagement.ProjectStatusService.ProjectStatusServiceClient">
                                    </asp:ObjectDataSource>
                                </asp:Panel>
                            </asp:View>
                        </asp:MultiView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:ValidationSummary ID="vsumProject" runat="server" EnableClientScript="false"
                            ValidationGroup="Project" />
                        <asp:ValidationSummary ID="vsumProjectAttachment" runat="server" EnableClientScript="false"
                            ValidationGroup="ProjectAttachment" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:HiddenField ID="hdnProjectId" runat="server" />
                        <asp:HiddenField ID="hdnProjectDelete" runat="server" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete Project" OnClick="btnDelete_Click"
                            OnClientClick="ConfirmToDeleteProject();" Enabled="false" Visible="false" />&nbsp;
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" ValidationGroup="Project" />&nbsp;
                        <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlAttachSOW" runat="server" BackColor="White" BorderColor="Black"
                Style="display: none" BorderWidth="2px">
                <table width="100%" style="padding: 5px;">
                    <tr style="background-color: Gray; height: 27px;">
                        <td align="center" style="white-space: nowrap; font-size: 14px; width: 100%">
                            Attach SOW to Existing Project
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="white-space: nowrap; padding-left: 5px; padding-right: 5px;">
                            <asp:FileUpload ID="fuProjectAttachment" onchange="EnableUploadButton();" BackColor="White"
                                runat="server" Width="375px" Size="56" />
                            <asp:CustomValidator ID="cvProjectAttachment" runat="server" ControlToValidate="fuProjectAttachment"
                                EnableClientScript="true" ClientValidationFunction="cvProjectAttachment_ClientValidationFunction"
                                SetFocusOnError="true" Display="Dynamic" OnServerValidate="cvProjectAttachment_OnServerValidate"
                                ValidationGroup="ProjectAttachment" Text="*" ToolTip="File Format must be PDF."
                                ErrorMessage="File Format must be PDF."></asp:CustomValidator>
                            <asp:CustomValidator ID="cvalidatorProjectAttachment" runat="server" ControlToValidate="fuProjectAttachment"
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" OnServerValidate="cvalidatorProjectAttachment_OnServerValidate"
                                ValidationGroup="ProjectAttachment" Text="*"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="white-space: nowrap; padding-left: 5px; padding-right: 5px;">
                            <asp:Label ID="lblAttachmentMessage" ForeColor="Gray" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" style="padding: 5px; white-space: nowrap;">
                            <asp:Button ID="btnUpload" Enabled="false" ValidationGroup="ProjectAttachment" runat="server"
                                Text="Upload" OnClick="btnUpload_Click" />
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnCancel" OnClick="btnCancel_OnClick" runat="server" Text="Cancel" />
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
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSave" />
            <asp:PostBackTrigger ControlID="btnUpload" />
            <asp:PostBackTrigger ControlID="btnCancel" />
            <asp:PostBackTrigger ControlID="lnkProjectAttachment" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="odsActivePersons" runat="server" SelectMethod="PersonListAllShort"
        TypeName="PraticeManagement.PersonService.PersonServiceClient">
        <SelectParameters>
            <asp:Parameter Name="practice" Type="Int32" />
            <asp:Parameter DefaultValue="1" Name="statusId" Type="Int32" />
            <asp:Parameter Name="startDate" Type="DateTime" />
            <asp:Parameter Name="endDate" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

