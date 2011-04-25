<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientDetails.aspx.cs"
    Inherits="PraticeManagement.ClientDetails" Title="Practice Management - Client Details"
    MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/Clients/ClientProjects.ascx" TagName="ClientProjects"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Clients/ClientGroups.ascx" TagName="ClientGroups" TagPrefix="uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Client Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Client Details
</asp:Content> 
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function setClassForAddProject() {
            var button = document.getElementById("<%= btnAddProject.ClientID%>");
            var chbActive = document.getElementById("<%= chbActive.ClientID%>");
            if (!chbActive.checked) {
                button.disabled = "disabled";
                button.className = "darkadd-btn-project";
            }
            else {
                button.disabled = "";
                button.className = "add-btn-project";
            }
        }
        function checkhdnchbActive() {
            var hdnchbActive = document.getElementById("<%= hdnchbActive.ClientID%>");
            if (hdnchbActive.value == "true") {
                return true;
            }
            else {
                return false;
            }
        }
        function checkDirty(mpId) {
            if (showDialod()) {
                __doPostBack('__Page', mpId);
                return true;
            }
            return false;
        }
    </script>
    <div class="filters">
        <AjaxControlToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
            CssClass="CustomTabStyle">
            <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                <HeaderTemplate>
                    <span class="bg DefaultCursor"><span class="NoHyperlink">Filters</span></span>
                </HeaderTemplate>
                <ContentTemplate>
                    <table>
                        <tr>
                            <td>
                                Client Active?
                            </td>
                            <td>
                                <asp:CheckBox ID="chbActive" runat="server" Checked="true" onclick="setDirty();setClassForAddProject();" />
                                <asp:HiddenField ID="hdnchbActive" runat="server" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Are projects billable?
                            </td>
                            <td>
                                <asp:CheckBox ID="chbIsChar" runat="server" Checked="true" onclick="setDirty();"
                                    ToolTip="Projects for this client are billable by default." />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Client Name
                            </td>
                            <td>
                                <asp:TextBox ID="txtClientName" runat="server" onchange="setDirty();" Width="194px"
                                    ValidationGroup="Client" />
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="reqClientName" runat="server" ControlToValidate="txtClientName"
                                    ErrorMessage="The Client Name is required." ToolTip="The Client Name is required."
                                    Text="*" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="Client" />
                                <asp:CustomValidator ID="custClientName" runat="server" ControlToValidate="txtClientName"
                                    ErrorMessage="There is another Client with the same Name." ToolTip="There is another Client with the same Name."
                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                    OnServerValidate="custClientName_ServerValidate" ValidationGroup="Client" />
                                <asp:CustomValidator ID="custClient" runat="server" ControlToValidate="txtClientName"
                                    ErrorMessage="An error occurs during saving the data. Please contact your administrator."
                                    ToolTip="An error occurs during saving the data. Please contact your administrator."
                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                    OnServerValidate="custClient_ServerValidate" ValidationGroup="Client" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Default Salesperson
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlDefaultSalesperson" runat="server" onchange="setDirty();" Width="200px"
                                     ValidationGroup="Client" />
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="reqDefaultSalesperson" runat="server" ControlToValidate="ddlDefaultSalesperson"
                                    ErrorMessage="The Default Salesperson is required." ToolTip="The Default Salesperson is required."
                                    Text="*" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="Client" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Default Client Director
                            </td>
                            <td colspan="2">
                                <asp:DropDownList ID="ddlDefaultDirector" runat="server" onchange="setDirty();" Width="200px"
                                    ValidationGroup="Client" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Default Discount
                            </td>
                            <td>
                                <asp:TextBox ID="txtDefaultDiscount" runat="server" onchange="setDirty();" Width="150"
                                    ValidationGroup="Client" />%
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="reqDefaultDiscount" runat="server" ControlToValidate="txtDefaultDiscount"
                                    ErrorMessage="The Default Discount is required." ToolTip="The Default Discount is required."
                                    Text="*" SetFocusOnError="True" EnableClientScript="False" Display="Dynamic"
                                    ValidationGroup="Client" />
                                <asp:CompareValidator ID="compDefaultDiscount" runat="server" ControlToValidate="txtDefaultDiscount"
                                    ErrorMessage="A number with 2 decimal digits is allowed for the Default Discount."
                                    ToolTip="A number with 2 decimal digits is allowed the Default Discount." Text="*"
                                    SetFocusOnError="true" EnableClientScript="false" Display="Dynamic" Operator="DataTypeCheck"
                                    Type="Currency" ValidationGroup="Client" />
                                <asp:RangeValidator ID="rvDefaultDiscount" runat="server" ControlToValidate="txtDefaultDiscount"
                                    ErrorMessage="Default discount should be between 0 and 100" MinimumValue="0"
                                    MaximumValue="100" ToolTip="Default discount should be between 0 and 100" EnableClientScript="false"
                                    Display="Dynamic" Text="*" SetFocusOnError="true" Type="Currency" ValidationGroup="Client"></asp:RangeValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Terms
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlDefaultTerms" runat="server" onchange="setDirty();" DataTextField="Name"
                                    DataValueField="Frequency" AppendDataBoundItems="true" Width="155">
                                    <asp:ListItem Text=""></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                    <asp:ValidationSummary ID="vsumClient" runat="server" ValidationGroup="Client" />
                    <div style="padding-top: 10px;">
                        <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </AjaxControlToolkit:TabContainer>
    </div>
    <div class="buttons-block" style="margin-bottom: 10px">
        <asp:HiddenField ID="hdnClientId" runat="server" />
        <cc:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />&nbsp;
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="pm-button"
            ValidationGroup="Client" />
    </div>
    <div class="filters">
        <asp:ImageButton ImageUrl="~/Images/add-project.png" runat="server" ID="btnAddProject"
            ValidationGroup="Client" CssClass="add-btn-project" OnClick="btnAddProject_Click" />
        <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
            CssClass="CustomTabStyle">
            <ajaxToolkit:TabPanel runat="server" ID="tpProjects">
                <HeaderTemplate>
                    <span class="bg"><a href="#"><span>Projects</span></a> </span>
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="project-filter">
                        <asp:UpdatePanel ID="updprojects" runat="server">
                            <contenttemplate>
                        <uc:ClientProjects ID="projects" runat="server" />
                         </contenttemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="tpGroups">
                <HeaderTemplate>
                    <span class="bg"><a href="#"><span>Groups</span></a> </span>
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="project-filter">
                        <asp:UpdatePanel ID="updGroups" runat="server">
                            <contenttemplate>
                                <uc:ClientGroups ID="groups" runat="server" />
                            </contenttemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </AjaxControlToolkit:TabContainer>
    </div>
</asp:Content>

