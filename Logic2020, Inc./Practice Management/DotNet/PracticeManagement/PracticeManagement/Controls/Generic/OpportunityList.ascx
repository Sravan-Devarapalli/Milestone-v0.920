<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpportunityList.ascx.cs"
    Inherits="PraticeManagement.Controls.Generic.OpportunityList" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="System.Data" %>
<%@ Register Src="~/Controls/ProjectNameCellRounded.ascx" TagName="ProjectNameCellRounded"
    TagPrefix="uc" %>
<script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script type="text/javascript">

    function setHintPosition(img, displayPnl) {
        var image = $("#" + img);
        var displayPanel = $("#" + displayPnl);
        iptop = image.offset().top;
        ipleft = image.offset().left;
        iptop = iptop + 10;
        ipleft = ipleft - 10;
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();
    }

    function setPosition(item, ytop, xleft) {
        item.offset({ top: ytop, left: xleft });
    }
</script>
<div id="opportunity-list">
    <asp:ListView ID="lvOpportunities" runat="server" DataKeyNames="Id" OnSorting="lvOpportunities_Sorting">
        <LayoutTemplate>
            <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">
                <tr runat="server" id="lvHeader" class="CompPerfHeader">
                    <td width="1%">
                        <div class="ie-bg no-wrap">
                        </div>
                    </td>
                    <td width="4%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnNumberSort" runat="server" Text="Opp. #" CommandName="Sort"
                                CssClass="arrow" CommandArgument="Number" />
                        </div>
                    </td>
                    <td width="4%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnPrioritySort" runat="server" Text="Priority" CommandName="Sort"
                                CssClass="arrow" CommandArgument="Priority" />
                            <asp:Image ID="imgPriorityHint" runat="server" ImageUrl="~/Images/hint.png" />
                            <asp:Panel ID="pnlPriority" Style="display: none;" CssClass="MiniReport" runat="server">
                                <table>
                                    <tr>
                                        <th align="right">
                                            <asp:Button ID="btnClosePriority" OnClientClick="return false;" runat="server" CssClass="mini-report-close"
                                                Text="x" />
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:ListView ID="lvOpportunityPriorities" runat="server">
                                                <LayoutTemplate>
                                                    <div style="max-height: 150px; overflow-y: auto;overflow-x:hidden;">
                                                        <table id="itemPlaceHolderContainer" runat="server" style="background-color: White;"
                                                            class="WholeWidth">
                                                            <tr runat="server" id="itemPlaceHolder">
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </LayoutTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td style="width: 100%; padding-left: 2px;">
                                                            <table class="WholeWidth">
                                                                <tr>
                                                                    <td align="center" valign="middle" style="text-align: center;  color:Black;font-size:12px;padding: 0px;">
                                                                        <asp:Label ID="lblPriority" Width="15px" Font-Bold="true" runat="server" Text='<%# Eval("Priority") %>'></asp:Label>
                                                                    </td>
                                                                    <td align="center" valign="middle" style="text-align: center; color:Black; padding: 0px;font-size:12px;padding-left: 2px;padding-right: 2px;">
                                                                        -
                                                                    </td>
                                                                    <td style="padding: 0px;">
                                                                        <asp:Label ID="lblDescription" runat="server" Width="180px" Style="white-space: normal; color:Black;font-size:12px;"
                                                                            Text='<%# Eval("Description") %>'></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <EmptyDataTemplate>
                                                    <tr>
                                                        <td valign="middle" style="padding-left: 2px;">
                                                            <asp:Label ID="lblNoPriorities" runat="server" Text="No Priorities."></asp:Label>
                                                        </td>
                                                    </tr>
                                                </EmptyDataTemplate>
                                            </asp:ListView>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <AjaxControlToolkit:AnimationExtender ID="animHide" TargetControlID="btnClosePriority"
                                runat="server">
                            </AjaxControlToolkit:AnimationExtender>
                            <AjaxControlToolkit:AnimationExtender ID="animShow" TargetControlID="imgPriorityHint"
                                runat="server">
                            </AjaxControlToolkit:AnimationExtender>
                        </div>
                    </td>
                    <td width="15%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnClientNameSort" runat="server" Text="Client - Group" CommandName="Sort"
                                CssClass="arrow" CommandArgument="ClientName" />
                        </div>
                    </td>
                    <td width="11%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnBuyerNameSort" runat="server" Text="Buyer Name" CommandName="Sort"
                                CssClass="arrow" CommandArgument="BuyerName" />
                        </div>
                    </td>
                    <td width="25%">
                        <div class="ie-bg no-wrap" style="white-space: nowrap">
                            <asp:LinkButton ID="btnOpportunityNameSort" runat="server" Text="Opportunity Name"
                                CommandName="Sort" CssClass="arrow" CommandArgument="OpportunityName" />
                        </div>
                    </td>
                    <td width="7%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnSalespersonSort" runat="server" Text="Salesperson" CommandName="Sort"
                                CssClass="arrow" CommandArgument="Salesperson" />
                        </div>
                    </td>
                    <td width="5%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnOwnerSort" runat="server" Text="Owner" CommandName="Sort"
                                CssClass="arrow" CommandArgument="Owner" />
                        </div>
                    </td>
                    <td align="center" width="4%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnEstimatedRevenue" runat="server" Text="Est. Revenue" CommandName="Sort"
                                CssClass="arrow" CommandArgument="EstimatedRevenue" />
                        </div>
                    </td>
                    <td width="4%" style="text-align: center;">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnCreateDateSort" runat="server" Text="Days Old" CommandName="Sort"
                                CssClass="arrow" CommandArgument="CreateDate" />
                        </div>
                    </td>
                    <td width="4%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnLastUpdate" runat="server" Text="Last Change" CommandName="Sort"
                                CssClass="arrow" CommandArgument="Updated" />
                        </div>
                    </td>
                </tr>
                <tr runat="server" id="itemPlaceholder" />
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <div class="cell-pad">
                        <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                            ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' ButtonCssClass='<%#PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClassByStatus((string)Eval("Status.Name"))%>' />
                    </div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblNumber" runat="server" Text='<%# Eval("OpportunityNumber") %>' /></div>
                </td>
                <td align="center">
                    <div class="cell-pad">
                        <asp:Label ID="lblPriority" runat="server" Text='<%# ((Opportunity) Container.DataItem).Priority.Priority %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblClientName" runat="server" Text='<%# ((Opportunity) Container.DataItem).ClientAndGroup %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblBuyerName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("BuyerName"))%>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:HyperLink ID="hlName" runat="server" NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
                            <%# HttpUtility.HtmlEncode((string)Eval("Name")) %>
                        </asp:HyperLink>
                    </div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblSalesperson" runat="server" Text='<%# IsNeedToShowPerson(((Opportunity)Container.DataItem).Salesperson) ? Eval("Salesperson.LastName") : string.Empty %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:HyperLink ID="hlOwner" runat="server" NavigateUrl='<%# IsNeedToShowPerson(((Opportunity)Container.DataItem).Owner) ? GetPersonDetailsLink((int) Eval("Owner.Id"), Container.DisplayIndex) : "#" %>'>
                           <%# IsNeedToShowPerson(((Opportunity)Container.DataItem).Owner) ? Eval("Owner.LastName") : string.Empty%>
                        </asp:HyperLink>
                    </div>
                </td>
                <td align="right" style="padding-right: 10px;">
                    <div class="cell-pad">
                        <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' /></div>
                </td>
                <td style="text-align: center;">
                    <div class="cell-pad">
                        <asp:Label ID="lblCreateDate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("CreateDate"), true) %>' /></div>
                </td>
                <td style="text-align: right;">
                    <div class="cell-pad">
                        <asp:Label ID="lblLastUpdate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("LastUpdate"), false) %>' /></div>
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr style="background: #F9FAFF;">
                <td>
                    <div class="cell-pad">
                        <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                            ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' ButtonCssClass='<%#PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClassByStatus((string)Eval("Status.Name"))%>' />
                    </div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblNumber" runat="server" Text='<%# Eval("OpportunityNumber") %>' /></div>
                </td>
                <td align="center">
                    <div class="cell-pad">
                        <asp:Label ID="lblPriority" runat="server" Text='<%# ((Opportunity) Container.DataItem).Priority.Priority %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblClientName" runat="server" Text='<%# ((Opportunity) Container.DataItem).ClientAndGroup %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblBuyerName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("BuyerName"))%>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:HyperLink ID="hlName" runat="server" NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
                            <%# HttpUtility.HtmlEncode((string)Eval("Name")) %>
                        </asp:HyperLink>
                    </div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblSalesperson" runat="server" Text='<%# IsNeedToShowPerson(((Opportunity)Container.DataItem).Salesperson) ? Eval("Salesperson.LastName") : string.Empty %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# IsNeedToShowPerson(((Opportunity)Container.DataItem).Owner) ? GetPersonDetailsLink((int) Eval("Owner.Id"), Container.DisplayIndex) : "#" %>'>
                           <%# IsNeedToShowPerson(((Opportunity)Container.DataItem).Owner) ? Eval("Owner.LastName") : string.Empty%>
                        </asp:HyperLink>
                    </div>
                </td>
                <td align="right" style="padding-right: 10px;">
                    <div class="cell-pad">
                        <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' /></div>
                </td>
                <td style="text-align: center;">
                    <div class="cell-pad">
                        <asp:Label ID="lblCreateDate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("CreateDate"),true) %>'
                            IsCreateDate="true" /></div>
                </td>
                <td style="text-align: right;">
                    <div class="cell-pad">
                        <asp:Label ID="lblLastUpdate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("LastUpdate"),false) %>'
                            IsCreateDate="false" /></div>
                </td>
            </tr>
        </AlternatingItemTemplate>
        <EmptyDataTemplate>
            <tr runat="server" id="EmptyDataRow">
                <td>
                    No opportunities found.
                </td>
            </tr>
        </EmptyDataTemplate>
    </asp:ListView>
</div>

