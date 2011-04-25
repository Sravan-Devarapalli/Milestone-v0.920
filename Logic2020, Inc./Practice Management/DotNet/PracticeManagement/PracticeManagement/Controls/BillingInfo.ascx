<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillingInfo.ascx.cs" Inherits="PraticeManagement.Controls.BillingInfo" %>

<table style="height: 130px" border="0"  cellspacing="2">
	<tr>
		<td style="width: 110px">Billing Contact</td>
		<td style="width: 350px" colspan="4">
			<asp:TextBox ID="txtBillingContact" runat="server" MaxLength="100" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;
			<%--<asp:RequiredFieldValidator ID="reqBillingContact" runat="server" ControlToValidate="txtBillingContact"
				ErrorMessage="The Billing Contact is required."
				ToolTip="The Billing Contact is required."
				EnableClientScript="false" Display="Dynamic" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>--%> 
		</td>
		<td style="width: 90px; text-align:left; padding-left:12px;">Billing Phone</td>
		<td style="width: 140px">
			<asp:TextBox ID="txtBillingPhone" runat="server" MaxLength="25" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;
			<%--<asp:RequiredFieldValidator ID="reqBillingPhone" runat="server" ControlToValidate="txtBillingPhone"
				ErrorMessage="The Billing Phone is required."
				ToolTip="The Billing Phone is required."
				EnableClientScript="false" Display="Dynamic" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
		</td>
	</tr>
	<tr>
		<td style="width: 110px">Billing Email</td>
		<td style="width: 350px" colspan="4">
			<asp:TextBox ID="txtBillingEmail" runat="server" MaxLength="100" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;
			<%--<asp:RequiredFieldValidator ID="reqBillingEmail" runat="server" ControlToValidate="txtBillingEmail"
				ErrorMessage="The Billing Email is required."
				ToolTip="The Billing Email is required."
				EnableClientScript="false" Display="Dynamic" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
		</td>
		<td style="width: 90px; text-align:left; padding-left:12px;">Billing Type</td>
		<td style="width: 140px">
			<asp:TextBox ID="txtBillingType" runat="server" MaxLength="25" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;
			<%--<asp:RequiredFieldValidator ID="reqBillingType" runat="server" ControlToValidate="txtBillingType"
				ErrorMessage="The Billing Type is required."
				ToolTip="The Billing Type is required."
				EnableClientScript="false" Display="Dynamic" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
		</td>
	</tr>
	<tr>
		<td style="width: 110px">Billing Address 1</td>
		<td colspan="7">
			<asp:TextBox ID="txtBillingAddress1" runat="server" MaxLength="100" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;
			<%--<asp:RequiredFieldValidator ID="reqBillingAddress1" runat="server" ControlToValidate="txtBillingAddress1"
				ErrorMessage="The Billing Address 1 is required."
				ToolTip="The Billing Address 1 is required."
				EnableClientScript="false" Display="Dynamic" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
		</td>
	</tr>
	<tr>
		<td style="width: 110px">Billing Address 2</td>
		<td colspan="7">
			<asp:TextBox ID="txtBillingAddress2" runat="server" MaxLength="100" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;</td>
	</tr>
	<tr>
		<td style="width: 110px">Billing City</td>
		<td style="width: 140px">
			<asp:TextBox ID="txtBillingCity" runat="server" MaxLength="50" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;
			<%--<asp:RequiredFieldValidator ID="reqBillingCity" runat="server" ControlToValidate="txtBillingCity"
				ErrorMessage="The Billing City is required."
				ToolTip="The Billing City is required."
				EnableClientScript="false" Display="Dynamic" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
		</td>
		<td style="width: 80px; text-align:left; padding-left:12px;">Billing State</td>
		<td style="width: 90px">
			<asp:TextBox ID="txtBillingState" runat="server" MaxLength="50" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;
			<%--<asp:RequiredFieldValidator ID="reqBillingState" runat="server" ControlToValidate="txtBillingState"
				ErrorMessage="The Billing State is required."
				ToolTip="The Billing State is required."
				EnableClientScript="false" Display="Dynamic" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
		</td>
		<td style="width: 90px; text-align:left; padding-left:12px;">Billing Zip</td>
		<td style="width: 140px">
			<asp:TextBox ID="txtBillingZip" runat="server" MaxLength="10" CssClass="WholeWidth" onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;
			<%--<asp:RequiredFieldValidator ID="reqBillingZip" runat="server" ControlToValidate="txtBillingZip"
				ErrorMessage="The Billing Zip is required."
				ToolTip="The Billing Zip is required."
				EnableClientScript="false" Display="Dynamic" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
		</td>
	</tr>
	<tr>
		<td style="width: 110px">Purchase Order</td>
		<td style="width: 350px" colspan="4">
			<asp:TextBox ID="txtPurchaseOrder" runat="server" MaxLength="25" CssClass=WholeWidth onchange="setDirty();"></asp:TextBox>
		</td>
		<td class="ValidationCell">&nbsp;
			<%--<asp:RequiredFieldValidator ID="reqPurchaseOrder" runat="server" ControlToValidate="txtPurchaseOrder"
				ErrorMessage="The Purchase Order is required."
				ToolTip="The Purchase Order is required."
				EnableClientScript="false" Display="Dynamic" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
		</td>
		<td colspan="3">&nbsp;</td>
	</tr>
</table>

