<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectSummaryTabByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectSummaryTabByResource" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" class="Width90Percent">
        </td>
        <td class="Width10Percent padRight5">
            <table class="WholeWidth">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To Excel" />
                    </td>
                    <td>
                        <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Panel ID="pnlFilterProjectRoles" Style="display: none;" runat="server">
    <uc:FilteredCheckBoxList ID="cblProjectRoles" runat="server" />
</asp:Panel>
<asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_OnClick" Style="display: none;" />
<asp:Repeater ID="repResource" runat="server" OnItemDataBound="repResource_ItemDataBound">
    <HeaderTemplate>
        <div class="minheight250Px">
            <table id="tblProjectSummaryByResource" class="tablesorter PersonSummaryReport WholeWidth">
                <thead>
                    <tr>
                        <th class="ProjectColum">
                            Resource
                        </th>
                        <th class="Width140px">
                        </th>
                        <th class="Width130px">
                            Project Role
                            <img alt="Filter" title="Filter" src="../../Images/search_filter.png" runat="server"
                                id="imgProjectRoleFilter" class="FilterImg" />
                            <AjaxControlToolkit:PopupControlExtender ID="pceProjectRole" runat="server" TargetControlID="imgProjectRoleFilter"
                                PopupControlID="pnlFilterProjectRoles" Position="Bottom">
                            </AjaxControlToolkit:PopupControlExtender>
                        </th>
                        <th class="Width100Px">
                            Billable
                        </th>
                        <th class="Width100Px">
                            Non-Billable
                        </th>
                        <th class="Width100Px">
                            Total
                        </th>
                        <th class="Width140px">
                        </th>
                        <th class="Width325Px">
                            Project Variance (in Hours)
                        </th>
                    </tr>
                </thead>
                <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="ReportItemTemplate">
            <td class="t-left padLeft5">
                <%# Eval("Person.PersonLastFirstName")%>
                <asp:Image ID="imgOffshore" runat="server" ImageUrl="~/Images/Offshore_Icon.png"
                    ToolTip="Resource is an offshore employee" Visible='<%# (bool)Eval("Person.IsOffshore")%>' />
            </td>
            <td>
            </td>
            <td class="t-center padLeft5">
                <%# Eval("Person.ProjectRoleName")%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
            </td>
            <td>
            </td>
            <td>
                <table class="WholeWidth  TdLevelNoBorder">
                    <tr>
                        <td class="Width75Percent">
                            <table class="WholeWidth">
                                <tr class="border1Px">
                                    <td class="Width50Percent borderRightImp">
                                        <table class="WholeWidth">
                                            <tr>
                                                <td style="<%# Eval("BillableFirstHalfHtmlStyle")%>">
                                                </td>
                                                <td style="<%# Eval("BillableSecondHalfHtmlStyle")%>">
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width50Percent borderLeft">
                                        <table class="WholeWidth">
                                            <tr>
                                                <td style="<%# Eval("ForecastedFirstHalfHtmlStyle")%>">
                                                </td>
                                                <td style="<%# Eval("ForecastedSecondHalfHtmlStyle")%>">
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="Width25Percent">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="TimePeriodByProjectVariance">
                                        <%# Eval("Variance")%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table></div>
    </FooterTemplate>
</asp:Repeater>
<br />
<div id="divEmptyMessage" style="display: none;" class="EmptyMessagediv" runat="server">
    There are no Time Entries towards this project.
</div>

