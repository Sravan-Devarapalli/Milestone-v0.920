<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecruiterInfo.ascx.cs"
    Inherits="PraticeManagement.Controls.RecruiterInfo" %>
<table>
    <tr>
        <td width="100px" style="padding-left: 15px">
            Recruiter
        </td>
        <td>
            <asp:DropDownList ID="ddlRecruiter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlRecruiter_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:HiddenField ID="hidRecruiter" runat="server" />
        </td>
    </tr>
</table>
<table>
    <tr id="trCommissionDetails" runat="server">
        <td scope="row" nowrap="nowrap" style="padding-top: 10px; padding-left: 15px">
            Recruiter commission
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            <asp:TextBox ID="txtRecruiterCommission1" runat="server" Width="60px" onchange="setDirty();"></asp:TextBox>
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            <asp:CustomValidator ID="custRecruiterCommission1" runat="server" ErrorMessage="The Recruiter commission is required."
                ToolTip="The Recruiter commission is required." Text="*" Display="Dynamic" SetFocusOnError="true"
                OnServerValidate="custRecruiterCommission1_OnServerValidate" EnableClientScript="false"
                ValidationGroup="Person"></asp:CustomValidator>
            <asp:CompareValidator ID="compRecruiterCommission1" runat="server" ControlToValidate="txtRecruiterCommission1"
                ErrorMessage="A number with 2 decimal digits is allowed for the Recruiter commission."
                ToolTip="A number with 2 decimal digits is allowed for the Recruiter commission."
                Text="*" SetFocusOnError="true" EnableClientScript="false" Operator="DataTypeCheck"
                Type="Currency" ValidationGroup="Person"></asp:CompareValidator>
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            After
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            <asp:TextBox ID="txtAfter1" runat="server" Width="60px" onchange="setDirty();"></asp:TextBox>
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            <asp:CustomValidator ID="custAfret1" runat="server" ErrorMessage="The After Billed days is required."
                ToolTip="The After Billed days is required." Text="*" Display="Dynamic" SetFocusOnError="true"
                EnableClientScript="false" OnServerValidate="custAfret1_OnServerValidate" ValidationGroup="Person"></asp:CustomValidator>
            <asp:CompareValidator ID="compAfret1" runat="server" ControlToValidate="txtAfter1"
                ErrorMessage="The After Billed days must be an integer number." ToolTip="The After Billed days must be an integer number."
                Text="*" SetFocusOnError="true" EnableClientScript="false" Operator="DataTypeCheck"
                Type="Integer" ValidationGroup="Person"></asp:CompareValidator>
            <asp:HiddenField ID="hidOldAfret1" runat="server" />
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            Billed&nbsp;days,&nbsp;
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            <asp:TextBox ID="txtRecruiterCommission2" runat="server" Width="60px" onchange="setDirty();"></asp:TextBox>
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            <asp:CustomValidator ID="custRecruiterCommission2" runat="server" ErrorMessage="The Recruiter commission is required."
                ToolTip="The Recruiter commission is required." Text="*" Display="Dynamic" SetFocusOnError="true"
                EnableClientScript="false" OnServerValidate="custRecruiterCommission2_OnServerValidate"
                ValidationGroup="Person"></asp:CustomValidator>
            <asp:CompareValidator ID="compRecruiterCommission2" runat="server" ControlToValidate="txtRecruiterCommission2"
                ErrorMessage="A number with 2 decimal digits is allowed for the Recruiter commission."
                ToolTip="A number with 2 decimal digits is allowed for the Recruiter commission."
                Text="*" SetFocusOnError="true" EnableClientScript="false" Operator="DataTypeCheck"
                Type="Currency" Display="Dynamic" ValidationGroup="Person"></asp:CompareValidator>
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            After
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            <asp:TextBox ID="txtAfter2" runat="server" Width="60px" onchange="setDirty();"></asp:TextBox>
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            <asp:CustomValidator ID="custAfter2" runat="server" ErrorMessage="The After Billed days is required."
                ToolTip="The After Billed days is required." Text="*" Display="Dynamic" SetFocusOnError="true"
                EnableClientScript="false" ValidationGroup="Person" OnServerValidate="custAfter2_OnServerValidate"></asp:CustomValidator>
            <asp:CompareValidator ID="compAfter2" runat="server" ControlToValidate="txtAfter2"
                ErrorMessage="The After Billed days must be an integer number." ToolTip="The After Billed days must be an integer number."
                Text="*" SetFocusOnError="true" EnableClientScript="false" Operator="DataTypeCheck"
                Type="Integer" Display="Dynamic" ValidationGroup="Person"></asp:CompareValidator>
            <asp:CompareValidator ID="compAfter" runat="server" ControlToValidate="txtAfter1"
                ControlToCompare="txtAfter2" ErrorMessage="The recruiting commission for the same period already exists."
                ToolTip="The recruiting commission for the same period already exists." Text="*"
                SetFocusOnError="true" EnableClientScript="false" Display="Dynamic" Operator="NotEqual"
                Type="Integer" ValidationGroup="Person"></asp:CompareValidator>
            <asp:HiddenField ID="hidOldAfret2" runat="server" />
        </td>
        <td style="padding-top: 10px; padding-left: 5px">
            Billed&nbsp;days
        </td>
    </tr>
</table>

