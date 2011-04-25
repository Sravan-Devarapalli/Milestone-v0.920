<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RealInterval.ascx.cs"
    Inherits="PraticeManagement.Controls.Generic.Filtering.RealInterval" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<table style="white-space: nowrap; font-size: 100%">
    <tr>
        <td>&nbsp;From&nbsp;</td>
        <td><asp:TextBox ID="tbFrom" runat="server" Width="50" Text="0.0" ValidationGroup='<%# ClientID %>'/></td>
        <td>
            <asp:RangeValidator ID="RangeValidator1" runat="server" 
                ControlToValidate="tbFrom" ErrorMessage="'From' is real between 0 and 24" 
                ValidationGroup='<%# ClientID %>' MaximumValue="24.0" MinimumValue="0.0" 
                Type="Double">*</asp:RangeValidator>
        </td>
        <td>&nbsp;To&nbsp;</td>
        <td><asp:TextBox ID="tbTo" runat="server" Width="50" Text="24.0" ValidationGroup='<%# ClientID %>' /></td>
        <td>
            <asp:RangeValidator ID="RangeValidator2" runat="server" 
                ControlToValidate="tbTo" ErrorMessage="'To' is real between 0 and 24" 
                ValidationGroup='<%# ClientID %>' MaximumValue="24.0" MinimumValue="0.0" 
                Type="Double">*</asp:RangeValidator>
                 <asp:CompareValidator ID="compToHour" runat="server" ControlToValidate="tbTo" ControlToCompare="tbFrom"
                ErrorMessage="To hours must be greater or equal to the from hours." ToolTip="To hours must be greater or equal to the from hours."
                Operator="GreaterThanEqual" Type="Double" ValidationGroup='<%# ClientID %>'>*</asp:CompareValidator>

        </td>
    </tr>
</table>


