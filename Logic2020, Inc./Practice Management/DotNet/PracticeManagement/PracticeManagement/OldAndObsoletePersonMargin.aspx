<%@ Page Language="C#" MasterPageFile="~/PracticeManagement.Master" AutoEventWireup="true" CodeBehind="OldAndObsoletePersonMargin.aspx.cs" Inherits="PraticeManagement.PersonMargin" Title="Practice Management - One-Off Person Margin" %>

<%@ Register src="Controls/PersonnelCompensation.ascx" tagname="PersonnelCompensation" tagprefix="uc1" %>
<%@ Register src="Controls/RecruiterInfo.ascx" tagname="RecruiterInfo" tagprefix="uc1" %>
<%@ Register src="Controls/WhatIf.ascx" tagname="WhatIf" tagprefix="uc1" %>

<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
	<title>Practice Management - One-Off Person Margin</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
	One-Off Person Margin
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
	<script type="text/javascript">//<!--
		//Overriding the getDirty with a dummy one
		function getDirty(){ return false; };
	//-->
	</script>
	<asp:UpdatePanel ID="upnlBody" runat="server">
	<ContentTemplate>
		<br />
		<table>
			<tr>
				<td colspan="3">
					<asp:Panel ID="pnlRecruiterInfo" runat="server">
						<uc1:RecruiterInfo ID="recruiterInfo" runat="server" ShowCommissionDetails="False"
							OnInfoChanged="compensation_Changed" />
					</asp:Panel>
				</td>
			</tr>
			<tr>
				<td colspan="3">&nbsp;</td>
			</tr>			
			<tr>
				<td colspan="3">&nbsp;</td>
			</tr>
			<tr>
			    <td>Person Name:</td>
				<td>
					<asp:DropDownList ID="ddlPersonName" runat="server" AutoPostBack="True" 
						onselectedindexchanged="ddlPersonName_SelectedIndexChanged" Width="380px"></asp:DropDownList>
				</td>
				<td>
					<uc1:PersonnelCompensation ID="personnelCompensation" runat="server" AutoPostBack="true"
						OnCompensationChanged="compensation_Changed" OnCompensationMethodChanged="compensation_Changed"
						OnPeriodChanged="compensation_Changed" />
				</td>
			</tr>
			<tr>
				<td colspan="3">&nbsp;</td>
			</tr>
		</table>
		<table>
			<tr>
				<td>
					<uc1:WhatIf ID="whatIf" runat="server" DisplayTargetMargin="true" />
				</td>
			</tr>
			<tr>
				<td>&nbsp;</td>
			</tr>
			<tr>
				<td>
					<asp:ValidationSummary ID="vsumPersonMargin" runat="server" EnableClientScript="false" />
					<asp:ValidationSummary ID="vsumComputeRate" runat="server" EnableClientScript="false" ValidationGroup="ComputeRate" />
				</td>
			</tr>
		</table>
	</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>

