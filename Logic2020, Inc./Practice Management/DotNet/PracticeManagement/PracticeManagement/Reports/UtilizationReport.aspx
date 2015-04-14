<%@ Page Title="Utilization" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="UtilizationReport.aspx.cs" Inherits="PraticeManagement.Reports.UtilizationReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <table class="Width50Percent">
        <tr>
            <td>
                <asp:DropDownList ID="ddlPerson" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPerson_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:Label ID="lblPerson" runat="server" Visible="false" CssClass="fontBold font14Px"></asp:Label>
            </td>
            <td>
                <span class="fontBold">YTD Available Hours</span> 
            </td>
            <td>
                <span class="fontBold">YTD Billable Hours Entered</span> 
            </td>
            <td>
                <span class="fontBold">Utilization</span> 
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                <asp:Label ID="lblTargetHours" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblBillableTime" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblUtilization" runat="server"></asp:Label>
            </td>
        </tr>
<%--        <tr>
            <td style="padding-top:10px;">
                &nbsp;
            </td>
            <td class="vBottom">
                Projected Hours
            </td>
            <td class="vBottom">
                YTD Billable Hours Entered
            </td>
            <td class="vBottom">
               Over/<span class="Bench">(Under)</span> Projection
            </td>
        </tr>
        <tr>
            <td style="padding-top:3px;">
                &nbsp;
            </td>
            <td class="vBottom">
                <asp:Label ID="lblAllocatedBillable" runat="server"></asp:Label>
            </td>
            <td class="vBottom">
                <asp:Label ID="lblBillableTime2" runat="server"></asp:Label>
            </td>
            <td class="vBottom">
                <asp:Label ID="lblAllocatedVsTarget" runat="server"></asp:Label>
            </td>
        </tr>--%>
    </table>
</asp:Content>

