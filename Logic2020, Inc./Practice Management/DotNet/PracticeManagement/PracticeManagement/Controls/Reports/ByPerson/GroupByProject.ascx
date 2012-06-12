<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByPerson.GroupByProject" %>
<%@ Register Src="~/Controls/Reports/PersonDetailReport.ascx" TagPrefix="uc" TagName="PersonDetailReport" %>
<table style="width: 100%; padding: 5px;">
    <tr style="background-color: rgb(245, 250, 255);">
        <td style="width: 1%;">
        </td>
        <td style="width: 99%;">
            <table class="WholeWidthWithHeight">
                <tr style="text-align: left;">
                    <td class="ProjectAccountName" style="width: 95%; white-space: nowrap;">
                        <asp:Label ID="lblPerson" runat="server" Font-Size="16px" Font-Bold="true"></asp:Label>
                    </td>
                    <td style="width: 5%; font-weight: bolder; font-size: 16px; text-align: right; padding-right: 20px;">
                        <asp:Label ID="lblTotalHours" runat="server" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td style="width: 1%;">
        </td>
        <td style="width: 99%;">
            <div style="max-height: 500px; overflow-y: auto; width: 100%;">
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 99%;">
                            <uc:PersonDetailReport ID="ucPersonDetailReport" runat="server" />
                        </td>
                        <td style="width: 1%;">
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>

