<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="PersonDetail.aspx.cs" Inherits="PraticeManagement.PersonDetail" Title="Practice Management - Person Details" %>

<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.Filtering"
    TagPrefix="cc1" %>
<%@ Register Src="~/Controls/Generic/Notes.ascx" TagName="Notes" TagPrefix="uc" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="Controls/PersonnelCompensation.ascx" TagName="PersonnelCompensation"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/RecruiterInfo.ascx" TagName="RecruiterInfo" TagPrefix="uc1" %>
<%@ Register Src="Controls/GrossMarginComputing.ascx" TagName="GrossMarginComputing"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/WhatIf.ascx" TagName="WhatIf" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/RestrictionPanel.ascx" TagPrefix="uc" TagName="RestrictionPanel" %>
<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLog" %>
<%@ Register Src="~/Controls/Persons/PersonProjects.ascx" TagPrefix="uc" TagName="PersonProjects" %>
<%@ Register Src="~/Controls/Configuration/DefaultUser.ascx" TagPrefix="uc" TagName="DefaultManager" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/OpportunityList.ascx" TagName="OpportunityList"
    TagPrefix="uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ElementDisabler" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register TagPrefix="uc" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Person Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Person Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript" language="javascript">
        /*
        This script is needed to initialize select all/none behavior for checkbox lists
        This is done because tab content is loaded asynchronously and window.load event is not fired
        when the user goes to the Permissions tab. So this method is called each time postback
        goes to the server in any way.
        */

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(pageLoadedHandler);
        function pageLoadedHandler(sender, args) {
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddClients');
            addListenersToParent('ctl00_body_rpPermissions_msddGroups', 'ctl00_body_rpPermissions_msddClients');
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddGroups');
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddSalespersons');
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddPracticeManagers');
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddPractices');
        }
    </script>
    <%--
        The following script is needed to implement dirty checks on Projects tab        
        Use Page.ClientScript.GetPostBackClientHyperlink(...) method to generate
        personProjects control postback url
    --%>
    <script type="text/javascript">
        function checkDirty(target, entityId) {
            if (showDialod()) {
                __doPostBack('ctl00$body$personProjects', target + ':' + entityId);
                return true;
            }

            return false;
        }
    </script>
    <style>
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
        
        .info-field
        {
            width: 152px;
        }
        
        /* ------------------------ */
    </style>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Literal ID="ltrScript" runat="server" Mode="PassThrough"> <script type="text/javascript" defer="defer">//<!--
				function validateStatus()
				{{
					if ({1})
					{{
						var ddl = document.getElementById("{0}");
						var statusId = ddl.selectedIndex >= 0 ? ddl.options[ddl.selectedIndex].value : "";
						var result = statusId != "{2}" || confirm("This person has a status of Terminated, but still has an open-ended compensation record. Click OK to close their compensation as of their termination date, or click Cancel to set their status back to Active.");
						if (!result)
						{{
							for (var i = 0; i < ddl.options.length; i++)
							{{
								if (ddl.options[i].value == "{3}")
								{{
									ddl.selectedIndex = i;
									break;
								}}
							}}
						}}
						return result;
					}}
					return true;
				}}
				
				clientValidateFunc = validateStatus;
			//-->
			</script></asp:Literal>
            <asp:HiddenField ID="hidDirty" runat="server" />
            <table class="PersonForm">
                <tr>
                    <td>
                        <asp:Panel ID="pnlPersonalInfo" runat="server">
                            <table>
                                <tr>
                                    <td nowrap="nowrap">
                                        First Name
                                    </td>
                                    <td class="info-field">
                                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="info-field" onchange="setDirty();"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqFirstName" runat="server" ValidationGroup="Person"
                                            ControlToValidate="txtFirstName" ErrorMessage="The First Name is required." EnableClientScript="False"
                                            SetFocusOnError="True" ToolTip="The First Name is required.">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="custPersonName" runat="server" ControlToValidate="txtFirstName"
                                            ErrorMessage="There is another Person with the same First Name and Last Name."
                                            ToolTip="There is another Person with the same First Name and Last Name." ValidationGroup="Person"
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonName_ServerValidate"></asp:CustomValidator>
                                        <asp:RegularExpressionValidator ControlToValidate="txtFirstName" ValidationGroup="Person"
                                            ID="valRegFirstName" runat="server" ErrorMessage="First Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe or hyphen."
                                            ToolTip="First Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe or hyphen."
                                            EnableClientScript="false" Text="*" ValidationExpression="^[a-zA-Z'\-]{2,35}$" />
                                    </td>
                                    <td>
                                        Status
                                    </td>
                                    <td class="info-field">
                                        <asp:DropDownList ID="ddlPersonStatus" runat="server" CssClass="info-field" onchange="setDirty();">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqPersonStatus" runat="server" ControlToValidate="ddlPersonStatus"
                                            ErrorMessage="The Status is required." ToolTip="The Status is required." ValidationGroup="Person"
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="custPersonStatus" runat="server" ControlToValidate="ddlPersonStatus"
                                            ErrorMessage="Only administrator can set a status to Active or Terminated." ToolTip="Only administrator can set a status to Active or Terminated."
                                            ValidationGroup="Person" Text="*" ValidateEmptyText="false" EnableClientScript="false"
                                            SetFocusOnError="true" Display="Dynamic" OnServerValidate="custPersonStatus_ServerValidate"></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">
                                        Last Name
                                    </td>
                                    <td class="info-field">
                                        <asp:TextBox ID="txtLastName" runat="server" CssClass="info-field" onchange="setDirty();"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqLastName" runat="server" ValidationGroup="Person"
                                            ControlToValidate="txtLastName" ErrorMessage="The Last Name is required." EnableClientScript="False"
                                            SetFocusOnError="True" ToolTip="The Last Name is required.">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ControlToValidate="txtLastName" ID="valRegLastName"
                                            runat="server" ValidationGroup="Person" ErrorMessage="Last Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe or hyphen."
                                            ToolTip="Last Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe or hyphen."
                                            EnableClientScript="false" Text="*" ValidationExpression="^[a-zA-Z'\-]{2,35}$" />
                                    </td>
                                    <td>
                                        Line Manager
                                    </td>
                                    <td class="info-field" style="padding-right: 2px">
                                        <asp:UpdatePanel ID="pnlLineManager" runat="server">
                                            <ContentTemplate>
                                                <uc:DefaultManager ID="defaultManager" runat="server" InsertFirtItem="false" PersonsRole="Practice Area Manager" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="lbSetPracticeOwner" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="lbSetPracticeOwner" runat="server" PostBackUrl="#" OnClick="lbSetPracticeOwner_Click">Set to Practice Area Owner</asp:LinkButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">
                                        Default Practice Area
                                    </td>
                                    <td class="info-field">
                                        <asp:DropDownList ID="ddlDefaultPractice" runat="server" Width="158px" onchange="setDirty();">
                                            <asp:ListItem Text="Technology Solutions"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        Hire&nbsp;Date
                                    </td>
                                    <td class="info-field">
                                        <uc2:DatePicker ID="dtpHireDate" runat="server" OnSelectionChanged="dtpHireDate_SelectionChanged"
                                            AutoPostBack="true" />
                                    </td>
                                    <td>
                                        <asp:CompareValidator ID="compHireDate" runat="server" ControlToValidate="dtpHireDate"
                                            EnableClientScript="False" ErrorMessage="The Hire Date must be in the format 'MM/dd/yyyy'"
                                            Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="Person" ToolTip="The Hire Date must be in the format 'MM/dd/yyyy'"
                                            Type="Date">*</asp:CompareValidator>
                                        <asp:RequiredFieldValidator ID="reqHireDate" runat="server" ControlToValidate="dtpHireDate"
                                            Display="Dynamic" EnableClientScript="False" ErrorMessage="The Hire Date is required."
                                            SetFocusOnError="True" ValidationGroup="Person" ToolTip="The Hire Date is required.">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="custHireDate" runat="server" ControlToValidate="dtpHireDate"
                                            ErrorMessage="Cannot set a Hire Date outside recruiting commissions period."
                                            ToolTip="Cannot set a Hire Date outside recruiting commissions period." ValidationGroup="Person"
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custHireDate_ServerValidate"></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Email&nbsp;Address
                                    </td>
                                    <td class="info-field">
                                        <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="info-field" onchange="setDirty();"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RegularExpressionValidator ID="regEmailAddress" runat="server" ControlToValidate="txtEmailAddress"
                                            Display="Dynamic" ErrorMessage="The Email Address is not valid." ValidationGroup="Person"
                                            ToolTip="The Email Address is not valid." Text="*" EnableClientScript="False"
                                            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                        <asp:CustomValidator ID="custEmailAddress" runat="server" ControlToValidate="txtEmailAddress"
                                            ErrorMessage="There is another Person with the same Email." ToolTip="There is another Person with the same Email."
                                            ValidationGroup="Person" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                            Display="Dynamic" OnServerValidate="custEmailAddress_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator ID="custUserName" runat="server" ControlToValidate="txtEmailAddress"
                                            ErrorMessage="Unknow error occures. Please contact your administrator." ToolTip="Unknow error occures. Please contact your administrator."
                                            ValidateEmptyText="true" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                            Display="Dynamic" ValidationGroup="Person" OnServerValidate="custUserName_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator ID="custReqEmailAddress" runat="server" ControlToValidate="txtEmailAddress"
                                            ErrorMessage="The Email Address is required for active person." ToolTip="The Email Address is required for active person."
                                            ValidationGroup="Person" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                            Display="Dynamic" ValidateEmptyText="true" OnServerValidate="custReqEmailAddress_ServerValidate"></asp:CustomValidator>
                                    </td>
                                    <td nowrap="nowrap">
                                        Termination Date
                                    </td>
                                    <td class="info-field">
                                        <uc2:DatePicker ID="dtpTerminationDate" runat="server" OnSelectionChanged="dtpTerminationDate_SelectionChanged"
                                            AutoPostBack="true" />
                                    </td>
                                    <td>
                                        <asp:CompareValidator ID="compTerminationDate" runat="server" ControlToValidate="dtpTerminationDate"
                                            EnableClientScript="False" Enabled="False" EnableTheming="True" ErrorMessage="The Termination Date must be in the format 'MM/dd/yyyy'"
                                            Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="Person" ToolTip="The Termination Date must be in the format 'MM/dd/yyyy'"
                                            Type="Date">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="custTerminationDate" runat="server" ErrorMessage="To terminate the person the Termination Date should be specified."
                                            ToolTip="The Termination Date is not correct" ValidationGroup="Person" Text="*"
                                            EnableClientScript="false" OnServerValidate="custTerminationDate_ServerValidate"></asp:CustomValidator>
                                        <asp:CompareValidator ID="cmpTerminateDate" runat="server" ControlToValidate="dtpTerminationDate"
                                            ControlToCompare="dtpHireDate" Operator="GreaterThan" Type="Date" ErrorMessage="Termination date should be greater than Hire date."
                                            Text="*" ValidationGroup="Person" ToolTip="Termination date should be greater than Hire date."
                                            SetFocusOnError="true"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Telephone number
                                    </td>
                                    <td class="info-field">
                                        <asp:TextBox ID="txtTelephoneNumber" runat="server" onchange="setDirty();" CssClass="info-field"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RegularExpressionValidator ID="reqTelphoneNumber" runat="server" ControlToValidate="txtTelephoneNumber"
                                            Display="Dynamic" ErrorMessage="The Telephone number is not valid." ValidationGroup="Person"
                                            ToolTip="The Telephone number is not valid." Text="*" EnableClientScript="False"
                                            ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$"></asp:RegularExpressionValidator>
                                      
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
                                <tr>
                                    <td>
                                        <asp:Label ID="lblEmployeeNumber" runat="server" Text="PersonID"></asp:Label>
                                    </td>
                                    <td class="info-field">
                                        <asp:TextBox ID="txtEmployeeNumber" runat="server" MaxLength="12" onchange="setDirty();"
                                            onfocus="if (!this.readOnly &amp;&amp; !confirm('This value should not normally be changed once set. Please be cautious about changing this value. Press OK to continue or Cancel to return without changing it.')) this.blur();"
                                            CssClass="info-field"></asp:TextBox>
                                        <asp:HiddenField ID="hdnPersonId" runat="server" />
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqEmployeeNumber" runat="server" ValidationGroup="Person"
                                            ControlToValidate="txtEmployeeNumber" EnableClientScript="False" ErrorMessage="The Employee Number is required."
                                            SetFocusOnError="True" ToolTip="The Employee Number is required.">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="custEmployeeNumber" runat="server" ControlToValidate="txtEmployeeNumber"
                                            Display="Dynamic" EnableClientScript="false" ValidationGroup="Person" ErrorMessage="There is another Person with the same Employee Number."
                                            OnServerValidate="custEmployeeNumber_ServerValidate" SetFocusOnError="true" Text="*"
                                            ToolTip="There is another Person with the same Employee Number."></asp:CustomValidator>
                                    </td>
                                    <td>
                                        Practice Areas Owned
                                    </td>
                                    <td class="info-field">
                                        <asp:Repeater ID="repPracticesOwned" runat="server">
                                            <HeaderTemplate>
                                                <ul class="practices">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li class="practices">
                                                    <%# Eval("Name") %></li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </ul>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
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
                                    <td>
                                        <asp:CheckBox ID="chbLockedOut" runat="server" Checked="false" onclick="setDirty();"
                                            Text="Locked-Out" />
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
                        <asp:CustomValidator ID="custPersonData" runat="server" OnServerValidate="custPersonData_ServerValidate"></asp:CustomValidator>
                        &nbsp;
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr>
                    <td class="PersonMultiView">
                        <asp:Table ID="tblPersonViewSwitch" runat="server" CssClass="CustomTabStyle">
                            <asp:TableRow ID="rowSwitcher" runat="server">
                                <asp:TableCell ID="cellCompensation" runat="server">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnCompensation" runat="server" Text="Compensation" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellRecruiting" runat="server">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnViewRecruiting" runat="server" Text="Recruiter" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellCommissions" runat="server">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnCommissions" runat="server" Text="Commissions" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellRates" runat="server" Visible="false">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnRates" runat="server" Text="Overhead and Margin" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="3"></asp:LinkButton></span>
                                    </span></span> </span></asp:TableCell>
                                <asp:TableCell ID="cellWhatIf" runat="server">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnWhatIf" runat="server" Text="What-If?" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="4" OnClientClick="if (!confirmSaveDirty()) return false;"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellProjects" runat="server">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnProjects" runat="server" Text="Projects" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="5"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="TableCell1" runat="server">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnOppportunities" runat="server" Text="Opportunities" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="9"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellPermissions" runat="server">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnPermissions" runat="server" Text="Project/Opportunity Permissions"
                                            Style="width: 120px; display: inline-block; vertical-align: top; text-align: center;"
                                            CausesValidation="false" OnCommand="btnView_Command" CommandArgument="6"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellSecurity" runat="server" CssClass="SelectedSwitch">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnSecurity" runat="server" Text="Application Security" CausesValidation="false"
                                            Style="width: 80px; display: inline-block; vertical-align: top; text-align: center;"
                                            OnCommand="btnView_Command" CommandArgument="7"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellActivityLog" runat="server" Visible="false">
                                    <span class="bg" style="padding-bottom: 25px;"><span>
                                        <asp:LinkButton ID="btnActivityLog" runat="server" Text="History" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="8"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <asp:MultiView ID="mvPerson" runat="server" ActiveViewIndex="7">
                            <asp:View ID="vwCompensation" runat="server">
                                <asp:Panel ID="pnlCompensation" runat="server" CssClass="tab-pane">
                                    <div class="filters" style="margin-top: 5px; margin-bottom: 10px;">
                                        <div class="buttons-block">
                                            <span style="color: gray;">Click on Start Date column in the grid to edit compensation
                                                history item.</span>
                                            <asp:ShadowedTextButton ID="btnAddCompensation" runat="server" Text="Add Compensation"
                                                OnClick="btnAddCompensation_Click" CssClass="add-btn" OnClientClick="if (!confirmSaveDirty()) return false;" />
                                            &nbsp;<asp:CustomValidator ID="custCompensationCoversMilestone" runat="server" ValidationGroup="Person"
                                                ErrorMessage="This person has a status of Active, but does not have an active compensation record. &nbsp;Go back to their record so you can create a compensation record for them, or set their status as Projected or Terminated."
                                                OnServerValidate="custCompensationCoversMilestone_ServerValidate" Text="*"> </asp:CustomValidator>
                                            <%--<ext:ElementDisablerExtender ID="edeAddCompensationButton" runat="server" TargetControlID="btnAddCompensation" ControlToDisableID="btnSave" />--%>
                                            <div class="clear0">
                                            </div>
                                        </div>
                                    </div>
                                    <asp:GridView ID="gvCompensationHistory" runat="server" AutoGenerateColumns="False"
                                        EmptyDataText="No compensation history for this person." CssClass="CompPerfTable WholeWidth"
                                        GridLines="None" BackColor="White">
                                        <AlternatingRowStyle BackColor="#F9FAFF" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Start">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToShortDateString() %>'
                                                        CommandArgument='<%# Eval("StartDate") %>' OnCommand="btnStartDate_Command" OnClientClick="if (!confirmSaveDirty()) return false;"></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Start</div>
                                                </HeaderTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="End">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime?)Eval("EndDate")).HasValue ? ((DateTime?)Eval("EndDate")).Value.AddDays(-1).ToShortDateString() : string.Empty %>'></asp:Label></ItemTemplate>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        End</div>
                                                </HeaderTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:templatefield headertext="Practice Area">
                                                <itemtemplate>
                                                    <asp:label id="lblpractice" runat="server" text='<%# Eval("PracticeName")%>'></asp:label></itemtemplate>
                                                <headertemplate>
                                                    <div class="ie-bg">
                                                        Practice Area</div>
                                                </headertemplate>
                                                <itemstyle horizontalalign="center" />
                                            </asp:templatefield>
                                            <asp:templatefield headertext="Seniority">
                                                <itemtemplate>
                                                    <asp:label id="lblseniority" runat="server" text='<%# Eval("SeniorityName")%>'></asp:label></itemtemplate>
                                                <headertemplate>
                                                    <div class="ie-bg">
                                                        Seniority</div>
                                                </headertemplate>
                                                <itemstyle horizontalalign="center" />
                                            </asp:templatefield>
                                            <asp:TemplateField HeaderText="Basis">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblBasis" runat="server" Text='<%# Eval("TimescaleName") %>'></asp:Label></ItemTemplate>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Basis</div>
                                                </HeaderTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Amount">
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Amount</div>
                                                </HeaderTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Amount") %>'></asp:TextBox>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Amount") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Vacation">
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Vacation</div>
                                                </HeaderTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("VacationDays") %>'></asp:TextBox>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("VacationDays") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwRecruiter" runat="server">
                                <asp:Panel ID="pnlRecruiter" runat="server" CssClass="tab-pane WholeWidth">
                                    <uc1:RecruiterInfo ID="recruiterInfo" runat="server" OnInfoChanged="recruiterInfo_InfoChanged" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwCommission" runat="server">
                                <asp:Panel ID="pnlCommission" runat="server" CssClass="tab-pane WholeWidth">
                                    <table>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox ID="chbSalesCommissions" runat="server" Text="Person receives sales commission"
                                                    AutoPostBack="True" OnCheckedChanged="chbSalesCommissions_CheckedChanged" />
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSalesCommissionsGross" runat="server" Width="60px" Enabled="False"
                                                    onchange="setDirty();"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="reqSalesCommissionsGross" runat="server" ControlToValidate="txtSalesCommissionsGross"
                                                    ErrorMessage="The Gross Margin is required." ToolTip="The Gross Margin is required."
                                                    ValidationGroup="Person" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="compSalesCommissionsGross" runat="server" ControlToValidate="txtSalesCommissionsGross"
                                                    ErrorMessage="A number with 2 decimal digits is allowed for the Gross Margin."
                                                    ToolTip="A number with 2 decimal digits is allowed for the Gross Margin." ValidationGroup="Person"
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                            </td>
                                            <td colspan="3">
                                                &nbsp;%&nbsp;of&nbsp;gross&nbsp;margin
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="9" align="left">
                                                <h3>
                                                    Recruiting Commissions</h3>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="8">
                                                <asp:GridView ID="gvRecruitingCommissions" runat="server" EmptyDataText="No commissions."
                                                    AutoGenerateColumns="False" CssClass="CompPerfTable WholeWidth" GridLines="None"
                                                    BackColor="White">
                                                    <AlternatingRowStyle BackColor="#F9FAFF" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Start  Date">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnRecruitingCommissionStartDate" runat="server" CausesValidation="false"
                                                                    OnClientClick="if (!confirmSaveDirty()) return false;" Text='<%# ((DateTime)Eval("StartDate")).ToShortDateString() %>'
                                                                    CommandArgument='<%# Eval("CommissionHeaderId") %>' OnCommand="btnRecruitingCommissionStartDate_Command"
                                                                    Visible='<%# UserIsAdministrator %>'></asp:LinkButton><asp:Label ID="lblRecruitingCommissionStartDate"
                                                                        runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToShortDateString() %>'
                                                                        Visible='<%# !UserIsAdministrator %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderTemplate>
                                                                <div class="ie-bg">
                                                                    Start Date</div>
                                                            </HeaderTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <div class="ie-bg">
                                                                    End Date</div>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value.AddDays(-1).ToShortDateString() : string.Empty %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Commissions">
                                                            <HeaderTemplate>
                                                                <div class="ie-bg">
                                                                    Commissions</div>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("TextLine") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <EditItemTemplate>
                                                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TextLine") %>'></asp:TextBox>
                                                            </EditItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                            <td>
                                                Click on Start Date column to edit an item<br />
                                                <asp:ShadowedTextButton ID="btnAddDefaultRecruitingCommission" runat="server" Text="Add Recruiting Commission"
                                                    OnClientClick="if (!confirmSaveDirty()) return false;" OnClick="btnAddDefaultRecruitingCommission_Click"
                                                    CssClass="add-btn" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox ID="chbManagementCommissions" runat="server" Text="Person receives practice mgmt commission"
                                                    AutoPostBack="True" OnCheckedChanged="chbManagementCommissions_CheckedChanged" />
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtManagementCommission" runat="server" Width="60px" Enabled="False"
                                                    onchange="setDirty();"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="reqManagementCommission" runat="server" ControlToValidate="txtManagementCommission"
                                                    ErrorMessage="The Margin is required." ToolTip="The Margin is required." ValidationGroup="Person"
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="compManagementCommission" runat="server" ControlToValidate="txtManagementCommission"
                                                    ErrorMessage="A number with 2 decimal digits is allowed for the Margin." ToolTip="A number with 2 decimal digits is allowed for the Margin."
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Person"
                                                    Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                            </td>
                                            <td>
                                                &nbsp;of&nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td colspan="2">
                                                <asp:RadioButtonList ID="rlstManagementCommission" runat="server" RepeatDirection="Vertical"
                                                    RepeatLayout="Flow" Enabled="False">
                                                    <asp:ListItem Text="Own margin" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Sub-ordinate person margin" Value="2" Selected="True"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwRates" runat="server">
                                <asp:Panel ID="pnlRates" runat="server" CssClass="tab-pane WholeWidth">
                                    <table>
                                        <tr>
                                            <td nowrap="nowrap">
                                                Raw Hourly Rate
                                            </td>
                                            <td>
                                                <asp:Label ID="lblRawHourlyRate" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td nowrap="nowrap">
                                                Loaded Hourly Rate
                                            </td>
                                            <td>
                                                <asp:Label ID="lblLoadedHourlyRate" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5">
                                                This overhead is generated from data
                                                <asp:LinkButton ID="btnPersonMargin" runat="server" Text="on this page" OnClick="btnPersonMargin_Click"></asp:LinkButton>.<br />
                                                <br />
                                                The following table shows overhead calculations to this person.
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5">
                                                <asp:GridView ID="gvOverhead" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here."
                                                    ShowFooter="true">
                                                    <Columns>
                                                        <asp:BoundField HeaderText="Overhead type" DataField="Name" FooterText="Person Overhead" />
                                                        <asp:BoundField HeaderText="Amount per hour" DataField="HourlyValue">
                                                            <ItemStyle HorizontalAlign="Right" />
                                                            <FooterStyle HorizontalAlign="Right" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwWhatIf" runat="server">
                                <asp:Panel ID="pnlWhatIf" runat="server" CssClass="tab-pane WholeWidth">
                                    <div style="background-color: none">
                                        <uc1:WhatIf ID="whatIf" runat="server" DisplayTargetMargin="True" TargetMarginReadOnly="True" DisplayDefinedTermsAndCalcs="true" />
                                    </div>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwProjects" runat="server">
                                <asp:Panel ID="pnlProjects" runat="server" CssClass="tab-pane WholeWidth">
                                    <uc:PersonProjects ID="personProjects" runat="server" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwPermissions" runat="server" OnPreRender="vwPermissions_PreRender">
                                <asp:Panel ID="pnlPermissions" runat="server" CssClass="tab-pane">
                                    <asp:HiddenField ID="hfReloadPerms" runat="server" Value="False" />
                                    <uc:RestrictionPanel ID="rpPermissions" runat="server" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwSecurity" runat="server" OnPreRender="vwPermissions_PreRender">
                                <asp:Panel ID="Panel6" runat="server" CssClass="tab-pane WholeWidth">
                                    <table cellpadding="3">
                                        <tr>
                                            <td class="style2">
                                                <strong>
                                                    <asp:Localize ID="locRolesLabel" runat="server" Text="Roles"></asp:Localize></strong>
                                                <asp:CustomValidator ID="custRoles" runat="server" Display="Dynamic" EnableClientScript="false"
                                                    ValidationGroup="Person" ErrorMessage="Any person who is projected should not have any roles checked."
                                                    OnServerValidate="custRoles_ServerValidate" SetFocusOnError="true" Text="*" ToolTip="Any person who is projected should not have any roles checked."
                                                    ValidateEmptyText="true"></asp:CustomValidator>
                                                <asp:CustomValidator ID="valRecruterRole" runat="server" Display="Dynamic" EnableClientScript="false"
                                                    ValidationGroup="Person" ErrorMessage="Person with Recruiter role should have recruiting commission."
                                                    OnServerValidate="valRecruterRole_OnServerValidate" SetFocusOnError="true" Text="*"
                                                    ToolTip="Person with Recruiter role should have recruiting commission." ValidateEmptyText="true" />
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                <b>Seniority</b>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style2">
                                                <asp:CheckBoxList ID="chblRoles" runat="server" AutoPostBack="true" OnSelectedIndexChanged="chblRoles_SelectedIndexChanged"
                                                    Width="170px">
                                                </asp:CheckBoxList>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td valign="top">
                                                <asp:DropDownList ID="ddlSeniority" runat="server" onchange="setDirty();" />
                                            </td>
                                            <td>
                                                <asp:CustomValidator ID="custSeniority" runat="server" ValidationGroup="Person" ControlToValidate="ddlPersonStatus"
                                                    Display="Dynamic" EnableClientScript="true" ErrorMessage="The Seniority is required since the person's status is Active."
                                                    OnServerValidate="custSeniority_ServerValidate" Text="*" ToolTip="The Seniority is required since the person's status is Active."></asp:CustomValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style2">
                                                <asp:Button ID="btnResetPassword" runat="server" OnClick="btnResetPassword_Click"
                                                    OnClientClick="return confirm('Do you really want to reset user\'s password?');"
                                                    Text="Reset Password" />
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
                                        <tr>
                                            <td colspan="4">
                                                <asp:Label ID="lblPaswordResetted" runat="server" Text="The password was reset, and the person has been notified by email."
                                                    Visible="false"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <div style="width: 350px; overflow-x: scroll; overflow-y: auto; height: 350px; margin-top: 10px;
                                                    margin-bottom: 10px">
                                                    <asp:GridView ID="gvPermissionDiffenrece" runat="server" AutoGenerateColumns="false"
                                                        CssClass="CompPerfTable WholeWidth" GridLines="None" BackColor="White">
                                                        <AlternatingRowStyle BackColor="#F9FAFF" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <div class="ie-bg no-wrap">
                                                                        Page</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <%# Eval("Title") %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <div class="ie-bg no-wrap">
                                                                        Current</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <%# ((bool)Eval("Old")) ? "Yes" : "No" %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <div class="ie-bg no-wrap">
                                                                        New</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <%# ((bool)Eval("New")) ? "Yes" : "No" %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <div class="ie-bg no-wrap">
                                                                        Diff.</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <%# ((bool)Eval("IsDifferent")) ? "Yes" : "No"%>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwActivityLog" runat="server">
                                <asp:Panel ID="pnlLog" runat="server" CssClass="tab-pane WholeWidth">
                                    <uc:Notes ID="nPerson" runat="server" Target="Person" GridVisible="false" OnNoteAdded="nPerson_OnNoteAdded" />
                                    <uc:ActivityLog runat="server" ID="activityLog" DisplayDropDownValue="TargetPerson"
                                        DateFilterValue="Year" ShowDisplayDropDown="false" ShowProjectDropDown="false"
                                        ShowPersonDropDown="false" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwOpportunities" runat="server">
                                <asp:Panel ID="pnOpportunities" runat="server" CssClass="tab-pane WholeWidth">
                                    <uc:OpportunityList ID="personOpportunities" runat="server" AllowAutoRedirectToDetails="false"
                                        FilterMode="ByTargetPerson" />
                                </asp:Panel>
                            </asp:View>
                        </asp:MultiView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:ValidationSummary ID="valsPerson" runat="server" EnableClientScript="false"
                            ValidationGroup="Person" />
                        <asp:ValidationSummary ID="valsManager" runat="server" EnableClientScript="false"
                            ValidationGroup="ActiveManagers" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:MessageLabel ID="mlError" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                            WarningColor="Orange" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Button ID="btnSave" runat="server" ValidationGroup="Person" Text="Save" OnClick="btnSave_Click"
                            OnClientClick="if (!validateStatus()) return false;" />&nbsp;
                        <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />
                        <script type="text/javascript">
                            function disableSaveButton() {
                                document.getElementById('<%= btnSave.ClientID %>').disabled = true;
                            }
                        </script>
                        <ajaxToolkit:AnimationExtender ID="aeBtnSave" runat="server" TargetControlID="btnSave">
                            <Animations>
					            <OnClick>
					                <ScriptAction Script="disableSaveButton();" />
					            </OnClick>
                            </Animations>
                        </ajaxToolkit:AnimationExtender>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
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
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .style2
        {
            width: 60px;
        }
    </style>
</asp:Content>

