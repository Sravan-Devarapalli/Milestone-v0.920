<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientDetails.aspx.cs"
    Inherits="PraticeManagement.ClientDetails" Title="Account Details | Practice Management"
    MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/Clients/ClientProjects.ascx" TagName="ClientProjects"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Clients/ClientGroups.ascx" TagName="ClientGroups" TagPrefix="uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Account Details | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Account Details
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

        function applyColor(ddlColor) {
            for (var i = 0; i < ddlColor.length; i++) {
                if (ddlColor[i].selected) {
                    if (ddlColor[i].attributes["colorvalue"] != null && ddlColor[i].attributes["colorvalue"] != "undefined") {
                        ddlColor.style.backgroundColor = ddlColor[i].attributes["colorvalue"].value;
                    }
                    break;
                }
            }
        }

        function SetBackGroundColorForDdls() {
            var list = document.getElementsByTagName('select');

            for (var j = 0; j < list.length; j++) {
                applyColor(list[j]);
            }
        }

        window.onload = SetBackGroundColorForDdls;
    </script>
    <div class="filters">
        <AjaxControlToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
            CssClass="CustomTabStyle">
            <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                <HeaderTemplate>
                    <span class="bg"><a href="#"><span>Filters</span></a> </span>
                </HeaderTemplate>
                <ContentTemplate>
                    <table>
                        <tr>
                            <td>
                                Account Active?
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
                                    ToolTip="Projects for this account are billable by default." />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Is Account Internal?
                            </td>
                            <td>
                                <asp:CheckBox ID="chbIsInternal" runat="server" Checked="false" Enabled="false" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Is Note Required?
                            </td>
                            <td>
                                <asp:CheckBox ID="chbIsNoteRequired" runat="server" Checked="true" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Account Name
                            </td>
                            <td>
                                <asp:TextBox ID="txtClientName" runat="server" onchange="setDirty();" CssClass="Width194px"
                                    ValidationGroup="Client" />
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="reqClientName" runat="server" ControlToValidate="txtClientName"
                                    ErrorMessage="The Account Name is required." ToolTip="The Account Name is required."
                                    Text="*" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="Client" />
                                <asp:CustomValidator ID="custClientName" runat="server" ControlToValidate="txtClientName"
                                    ErrorMessage="There is another Account with the same Name." ToolTip="There is another Account with the same Name."
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
                                <asp:DropDownList ID="ddlDefaultSalesperson" runat="server" onchange="setDirty();"
                                    CssClass="Width200Px" ValidationGroup="Client" />
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
                                <asp:DropDownList ID="ddlDefaultDirector" runat="server" onchange="setDirty();" CssClass="Width200Px"
                                    ValidationGroup="Client" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Default Discount
                            </td>
                            <td>
                                <asp:TextBox ID="txtDefaultDiscount" runat="server" onchange="setDirty();" CssClass="Width150px"
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
                                    DataValueField="Frequency" AppendDataBoundItems="true" CssClass="Width155px">
                                    <asp:ListItem Text=""></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                    <div class="PaddingTop10Px">
                        <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="tpMarginGoals">
                <HeaderTemplate>
                    <span class="bg"><a href="#"><span>Contribution Margin Goals</span></a> </span>
                </HeaderTemplate>
                <ContentTemplate>
                    <asp:UpdatePanel ID="upnlClientThrsholds" runat="server">
                        <ContentTemplate>
                            <table class="Width100Per">
                                <tr>
                                    <td class="Width50Percent vTop">
                                        <table class="Width99Percent">
                                            <tr>
                                                <td colspan="3" class="PaddingBottomTop15Px">
                                                    <asp:CheckBox ID="chbMarginThresholds" AutoPostBack="true" OnCheckedChanged="cbMarginThresholds_OnCheckedChanged"
                                                        runat="server" Checked="false" onclick="setDirty();" />&nbsp;&nbsp; Use Color-coded
                                                    Contribution Margin thresholds
                                                </td>
                                                <td class="PaddingBottomTop15Px TextAlignRight">
                                                    <asp:Button ID="btnAddThreshold" Enabled="false" runat="server" Text="Add Threshold"
                                                        OnClientClick="setDirty();" OnClick="btnAddThreshold_OnClick" />
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:GridView ID="gvClientThrsholds" Enabled="false" runat="server" OnRowDataBound="gvClientThrsholds_RowDataBound"
                                            AutoGenerateColumns="False" EmptyDataText="" DataKeyNames="Id" CssClass="CompPerfTable Width99Percent"
                                            GridLines="None">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemStyle HorizontalAlign="Center" CssClass="WidthHeight25" />
                                                    <HeaderTemplate>
                                                        <div class="ie-bg">
                                                            Start</div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="gvddlStartRange" onchange="setDirty();" runat="server">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemStyle HorizontalAlign="Center" CssClass="WidthHeight25" />
                                                    <HeaderTemplate>
                                                        <div class="ie-bg">
                                                            End</div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="gvddlEndRange" onchange="setDirty();" runat="server">
                                                        </asp:DropDownList>
                                                        <asp:CustomValidator ID="cvgvRange" runat="server" ToolTip="The End must be greater than or equals to Start."
                                                            Text="*" EnableClientScript="false" OnServerValidate="cvgvRange_OnServerValidate"
                                                            SetFocusOnError="true" Display="Static" ValidationGroup="Client" />
                                                        <asp:CustomValidator ID="cvgvOverLapRange" runat="server" ToolTip="The specified Threshold Percentage range overlaps with another Threshold Percentage range."
                                                            OnServerValidate="cvgvOverLapRange_OnServerValidate" Text="*" EnableClientScript="false"
                                                            SetFocusOnError="true" Display="Static" ValidationGroup="Client" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemStyle HorizontalAlign="Center" CssClass="Width42Height25" />
                                                    <HeaderTemplate>
                                                        <div class="ie-bg">
                                                            Color</div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <cc2:CustomDropDown ID="gvddlColor" CssClass="Width85Percent" onclick="applyColor(this);" onchange="applyColor(this);setDirty();"
                                                            runat="server">
                                                        </cc2:CustomDropDown>
                                                        <asp:CustomValidator ID="cvgvddlColor" runat="server" OnServerValidate="cvgvddlColor_ServerValidate"
                                                            ToolTip="Please Select a Color." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Static" ValidationGroup="Client" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemStyle HorizontalAlign="Center" CssClass="Width8Height25" />
                                                    <HeaderTemplate>
                                                        <div class="ie-bg">
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnDeleteRow" runat="server" ImageUrl="~/Images/cross_icon.png"
                                                            ToolTip="Delete" OnClientClick="setDirty();" OnClick="btnDeleteRow_OnClick" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                    <td class="Width3Percent">
                                    </td>
                                    <td class="Width44Percent vTop">
                                        <div class="MarginGoalsDescription">
                                            <p>
                                                Enabling this feature and configuring color-coded ranges will allow persons without
                                                unrestricted access to Project and Milestone contribution margin calculations a visual indication
                                                of how Projects and Milestones are tracking with regard to the contribution margin goals defined
                                                by the company.<br />
                                                <br />
                                            </p>
                                            <p>
                                                Contribution Margin goals must add up to at least 100%.<br />
                                                <br />
                                            </p>
                                        </div>
                                    </td>
                                    <td class="Width3Percent">
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </AjaxControlToolkit:TabContainer>
        <asp:CustomValidator ID="cvClientThresholds" runat="server" OnServerValidate="cvClientThresholds_ServerValidate"
            ErrorMessage="Thresholds must be added up to  100% or more and must be continuous."
            ToolTip="Thresholds must be added up to  100% or more and must be continuous."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvColors" runat="server" OnServerValidate="cvColors_ServerValidate"
            ErrorMessage="Color must not be selected more than once." ToolTip="Color must not be selected more than once."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvgvddlColorClone" runat="server" ErrorMessage="Please Select a Color."
            Text="*" EnableClientScript="false" SetFocusOnError="false" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvgvOverLapRangeClone" runat="server" ErrorMessage="The specified Threshold Percentage range overlaps with another Threshold Percentage range."
            Text="*" EnableClientScript="false" SetFocusOnError="false" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvgvRangeClone" runat="server" ErrorMessage="The End must be greater than or equals to Start."
            Text="*" EnableClientScript="false" SetFocusOnError="false" Display="None" ValidationGroup="Client" />
    </div>
    <div class="buttons-block Margin-Bottom10Px">
        <div>
            <asp:ValidationSummary ID="vsumClient" runat="server" ValidationGroup="Client" />
        </div>
        <asp:HiddenField ID="hdnClientId" runat="server" />
        <cc:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />
        &nbsp;
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
                            <ContentTemplate>
                                <uc:ClientProjects ID="projects" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="tpGroups">
                <HeaderTemplate>
                    <span class="bg"><a href="#"><span>Business Units</span></a> </span>
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="project-filter">
                        <asp:UpdatePanel ID="updGroups" runat="server">
                            <ContentTemplate>
                                <uc:ClientGroups ID="groups" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </AjaxControlToolkit:TabContainer>
    </div>
</asp:Content>

