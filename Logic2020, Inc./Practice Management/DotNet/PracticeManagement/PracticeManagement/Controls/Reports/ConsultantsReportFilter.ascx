<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsultantsReportFilter.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ConsultantsReportFilter" %>
<%@ Register TagPrefix="uc" TagName="monthpicker" Src="~/Controls/MonthPicker.ascx" %>
<table cellpadding="5">
    <tr>
        <td style="text-align: right; vertical-align: top; padding-top:1%" rowspan="2">
            Month&nbsp;&nbsp;
        </td>
        <td style="text-align: left; vertical-align: top;padding-top:0.5%" rowspan="2">
            <uc:monthpicker ID="mpControl" AutoPostBack="false" runat="server" />
        </td>
        <td>
            <asp:CheckBox ID="chbActivePersons" runat="server" Text="Active Persons" ToolTip="Include active persons into report"
                AutoPostBack="false" Checked="True" />
        </td>
        <td>
            <asp:CheckBox ID="chbActiveProjects" runat="server" AutoPostBack="false" Checked="True"
                Text="Active Projects" ToolTip="Include active projects into report" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:CheckBox ID="chbProjectedPersons" runat="server" Text="Projected Persons" ToolTip="Include active persons into report"
                AutoPostBack="false" Checked="false" />
        </td>
        <td>
            <asp:CheckBox ID="chbProjectedProjects" runat="server" AutoPostBack="false" Checked="True"
                Text="Projected Projects" ToolTip="Include projected projects into report" />
        </td>
    </tr>
    <tr>
        <td style="text-align: right; vertical-align: top;">
            &nbsp;
        </td>
        <td style="text-align: left; vertical-align: top;">
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td>
         <asp:CheckBox ID="chbInternalProjects" runat="server" AutoPostBack="false" Checked="true"
                Text="Internal Projects" ToolTip="Include Internal projects into report" />
            
        </td>
    </tr>
    <tr>
        <td style="text-align: right; vertical-align: top;">
            &nbsp;
        </td>
        <td style="text-align: left; vertical-align: top;">
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td>
           <asp:CheckBox ID="chbExperimentalProjects" runat="server" AutoPostBack="false" Text="Experimental Projects"
                ToolTip="Include experimental projects into report" />
        </td>
    </tr>
</table>

