<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonStats.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.PersonStatsReport" %>
<asp:Table ID="tblPersonStats" runat="server" CssClass="CompPerfTable" Border="0"
    CellPadding="0" CellSpacing="0" EnableViewState="False" GridLines="None">
    <asp:TableHeaderRow>
        <asp:TableHeaderCell>
            <div class="ie-bg no-wrap">Person Stats</div>
        </asp:TableHeaderCell>
    </asp:TableHeaderRow>
    <asp:TableRow>
        <asp:TableCell CssClass="CompPerfDataTitle">Revenue</asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell CssClass="CompPerfDataTitle"># Employees</asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell CssClass="CompPerfDataTitle"># Consultants</asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell CssClass="CompPerfDataTitle">Virtual Consultants</asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell CssClass="CompPerfDataTitle">Change&nbsp;in&nbsp;VC&nbsp;from&nbsp;last&nbsp;month</asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell CssClass="CompPerfDataTitle">$ Rev/Employee</asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell CssClass="CompPerfDataTitle">$ Rev/Consultant</asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell CssClass="CompPerfDataTitle">Admin Cost as % of Revenue</asp:TableCell>
    </asp:TableRow>
</asp:Table>

