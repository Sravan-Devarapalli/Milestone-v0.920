<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByPerson.GroupByProject" %>
<%@ Register Src="~/Controls/Reports/PersonDetailReport.ascx" TagPrefix="uc" TagName="PersonDetailReport" %>
<table class="WholeWidth Padding5">
    <tr class="bgGroupByProjectHeader">
        <td class="Width1Percent">
        </td>
        <td class="Width99Percent">
            <table class="WholeWidthWithHeight GroupByProjectHeaderTable">
                <tr class="textleft">
                    <td class="ProjectAccountName FirstTd">
                        <asp:Label ID="lblPerson" runat="server"></asp:Label>
                    </td>
                    <td class="SecondTd">
                        <asp:Label ID="lblTotalHours" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="Width1Percent">
        </td>
        <td class="Width99Percent">
            <div class="GroupByProjectDiv">
                <table class="WholeWidth">
                    <tr>
                        <td class="Width99Percent">
                            <uc:PersonDetailReport ID="ucPersonDetailReport" runat="server" />
                        </td>
                        <td class="Width1Percent">
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>

