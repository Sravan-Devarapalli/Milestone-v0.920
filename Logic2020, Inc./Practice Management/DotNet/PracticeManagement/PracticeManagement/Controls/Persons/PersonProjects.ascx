<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonProjects.ascx.cs"
    Inherits="PraticeManagement.Controls.Persons.PersonProjects" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<%@ Register Src="~/Controls/ProjectNameCellRounded.ascx" TagName="ProjectNameCellRounded"
    TagPrefix="uc" %>
<asp:Repeater ID="repProjects" runat="server" OnItemDataBound="repProjects_ItemDataBound">
    <HeaderTemplate>
        <div class="BackGroundWhiteImp">
            <table id="tblPersonProjects" class="CompPerfTable WholeWidth">
                <thead>
                    <tr class="MilestoneHeaderText CursorPointer">
                        <th class="Width3Percent BorderRightC5C5C5">
                        </th>
                        <th class="ie-bg BorderRightC5C5C5">
                            Project#<span id="name"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg BorderRightC5C5C5">
                            Project<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg BorderRightC5C5C5">
                            Milestone<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg BorderRightC5C5C5">
                            Role<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg BorderRightC5C5C5">
                            Start Date<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg BorderRightC5C5C5">
                            End Date<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg BorderRightC5C5C5">
                            Revenue<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                        <th class="ie-bg">
                            Margin<span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        </th>
                    </tr>
                </thead>
                <tbody class="textCenter">
    </HeaderTemplate>
    <ItemTemplate>
        <tr id="trItem" runat="server" class="Height25Px">
            <td class="cell-pad">
                <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                    Target="_blank" ButtonProjectNameToolTip='<%# GetProjectNameCellToolTip((int)Eval("ProjectStatusId"),(int)Eval("HasAttachments"),(string)Eval("ProjectStatus")) %>'
                    ButtonCssClass='<%# GetProjectNameCellCssClass((int)Eval("ProjectStatusId") , (int)Eval("HasAttachments") )%>' />
            </td>
            <td class="CompPerfPeriod cell-pad">
                <%# Eval("ProjectNumber") %>
            </td>
            <td>
                <asp:HyperLink ID="hlProject" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("ProjectName")) %>'
                    NavigateUrl='<%# GetProjectRedirectUrl(Eval("ProjectId")) %>' onclick='<%# "return checkDirty(\"" + PROJECT_TARGET + "\", " + Eval("ProjectId") + ")" %>' />
            </td>
            <td>
                <asp:HyperLink ID="hlMilestone" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("MilestoneName")) %>'
                    NavigateUrl='<%# GetMilestoneRedirectUrl(Eval("MilestoneId"), Eval("ProjectId")) %>'
                    onclick='<%# "return checkDirty(\"" + MILESTONE_TARGET + "\", " + string.Format("\"{0}:{1}\"", Eval("MilestoneId"), Eval("ProjectId")) + ")" %>' />
            </td>
            <td class="CompPerfPeriod cell-pad">
                <%# Eval("RoleName") %>
            </td>
            <td class="CompPerfPeriod cell-pad">
                <asp:Label ID="Label2" runat="server" Text='<%# Bind("StartDate", "{0:MM/dd/yyyy}") %>'></asp:Label>
            </td>
            <td class="CompPerfPeriod cell-pad">
                <asp:Label ID="Label3" runat="server" Text='<%# Bind("EndDate", "{0:MM/dd/yyyy}") %>'></asp:Label>
            </td>
            <td class="CompPerfPeriod cell-pad" sorttable_customkey='<%# (PracticeManagementCurrency)((decimal)Eval("Revenue")) %>'>
                <asp:Label ID="lblRevenue" runat="server" Text='<%# ((PracticeManagementCurrency)((decimal)Eval("Revenue"))).ToString() %>'
                    CssClass="Revenue"></asp:Label>
            </td>
            <td class="CompPerfPeriod cell-pad" sorttable_customkey='<%# (PracticeManagementCurrency)((decimal)Eval("GrossMargin")) %>'>
                <asp:Label ID="lblGrossMargin" runat="server" Text='<%# ((PracticeManagementCurrency)((decimal)Eval("GrossMargin"))).ToString() %>'
                    CssClass="Margin"></asp:Label>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        <thead>
            <tr class="Height25Px textCenter"">
                <td colspan="6">
                    &nbsp;
                </td>
                <td>
                    <asp:Label ID="lblOverallMargin" runat="server" ToolTip="Overall Margin" Font-Bold="true" />
                </td>
                <td>
                    <asp:Label ID="lblTotalProjectsRevenue" runat="server" ToolTip="Total Projects Revenue"
                        Font-Bold="true" />
                </td>
                <td>
                    <asp:Label ID="lblTotalProjectsMargin" runat="server" ToolTip="Total Projects Margin"
                        Font-Bold="true" />
                </td>
            </tr>
        </thead>
        </tbody></table></div>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" style="display: none;" runat="server">
    There are no projects to be displayed here.
</div>

