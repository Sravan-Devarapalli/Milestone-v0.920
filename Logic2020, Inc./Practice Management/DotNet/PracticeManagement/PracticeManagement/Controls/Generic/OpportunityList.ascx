<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpportunityList.ascx.cs"
    Inherits="PraticeManagement.Controls.Generic.OpportunityList" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="System.Data" %>
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

    function SetTooltipText(descriptionText, hlinkObj) {
        var hlinkObjct = $('#' + hlinkObj.id);
        var displayPanel = $('#<%= oppNameToolTipHolder.ClientID %>');
        iptop = hlinkObjct.offset().top - hlinkObjct[0].offsetHeight;
        ipleft = hlinkObjct.offset().left + hlinkObjct[0].offsetWidth + 10;
        iptop = iptop;
        ipleft = ipleft;
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();

        var lbloppNameTooltipContent = document.getElementById('<%= lbloppNameTooltipContent.ClientID %>');
        lbloppNameTooltipContent.innerHTML = descriptionText.toString();
    }

    function HidePanel() {

        var displayPanel = $('#<%= oppNameToolTipHolder.ClientID %>');
        displayPanel.hide();
    }

</script>
<asp:Panel ID="oppNameToolTipHolder" Style="display: none;" runat="server" CssClass="ToolTip WordWrap OpportunityToolTip">
    <table>
        <tr class="top">
            <td class="lt">
                <div class="tail">
                </div>
            </td>
            <td class="tbor">
            </td>
            <td class="rt">
            </td>
        </tr>
        <tr class="middle">
            <td class="lbor">
            </td>
            <td class="content WordWrap">
                <pre>
<asp:Label ID="lbloppNameTooltipContent" CssClass="WordWrap" runat="server"></asp:Label>
</pre>
            </td>
            <td class="rbor">
            </td>
        </tr>
        <tr class="bottom">
            <td class="lb">
            </td>
            <td class="bbor">
            </td>
            <td class="rb">
            </td>
        </tr>
    </table>
</asp:Panel>
<div id="opportunity-list">
    <asp:ListView ID="lvOpportunities" runat="server" DataKeyNames="Id" OnSorting="lvOpportunities_Sorting">
        <LayoutTemplate>
            <asp:Panel ID="pnlPriority" CssClass="MiniReport displayNone SummaryMiniReport"
                runat="server">
                <table>
                    <tr>
                        <th class="textRight">
                            <asp:Button ID="btnClosePriority" OnClientClick="return false;" runat="server" CssClass="mini-report-close"
                                Text="x" />
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <asp:ListView ID="lvOpportunityPriorities" runat="server">
                                <LayoutTemplate>
                                    <div class="lvOpportunityPriorities">
                                        <table id="itemPlaceHolderContainer" runat="server" class="WholeWidth bgColorWhite">
                                            <tr runat="server" id="itemPlaceHolder">
                                            </tr>
                                        </table>
                                    </div>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr class="BorderBottomNone">
                                        <td class="LabelPriority">
                                            <asp:Label ID="lblPriority" CssClass="Width100Px DisplayInline" runat="server" Text='<%# Eval("HtmlEncodedDisplayName") %>'></asp:Label>
                                        </td>
                                        <td class="LabelPriority">
                                            -
                                        </td>
                                        <td class="LabelPriority">
                                            <asp:Label ID="lblDescription" runat="server" CssClass="OpportunityPriorityDescription"
                                                Text='<%# HttpUtility.HtmlEncode((string)Eval("Description")) %>'></asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <tr>
                                        <td class="padLeft2 vMiddle">
                                            <asp:Label ID="lblNoPriorities" runat="server" Text="No Priorities."></asp:Label>
                                        </td>
                                    </tr>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">
                <tr runat="server" id="lvHeader" class="CompPerfHeader">
                    <td class="Width1Percent">
                        <div class="ie-bg no-wrap">
                        </div>
                    </td>
                    <td class="Width4Percent">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnNumberSort" runat="server" Text="Opp. #" CommandName="Sort"
                                CssClass="arrow" CommandArgument="Number" />
                        </div>
                    </td>
                    <td class="Width4Percent">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnPrioritySort" runat="server" Text="Sales Stage" CommandName="Sort"
                                CssClass="arrow" CommandArgument="Priority" />
                            <asp:Image ID="imgPriorityHint" runat="server" ImageUrl="~/Images/hint.png" />
                            <AjaxControlToolkit:AnimationExtender ID="animHide" TargetControlID="btnClosePriority"
                                runat="server">
                            </AjaxControlToolkit:AnimationExtender>
                            <AjaxControlToolkit:AnimationExtender ID="animShow" TargetControlID="imgPriorityHint"
                                runat="server">
                            </AjaxControlToolkit:AnimationExtender>
                        </div>
                    </td>
                    <td class="Width15Percent">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnClientNameSort" runat="server" Text="Business Unit" CommandName="Sort"
                                CssClass="arrow" CommandArgument="ClientName" />
                        </div>
                    </td>
                    <td class="Width11Percent">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnBuyerNameSort" runat="server" Text="Buyer Name" CommandName="Sort"
                                CssClass="arrow" CommandArgument="BuyerName" />
                        </div>
                    </td>
                    <td class="Width25Percent">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnOpportunityNameSort" runat="server" Text="Opportunity Name"
                                CommandName="Sort" CssClass="arrow" CommandArgument="OpportunityName" />
                        </div>
                    </td>
                    <td class="Width7Percent">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnSalespersonSort" runat="server" Text="Salesperson" CommandName="Sort"
                                CssClass="arrow" CommandArgument="Salesperson" />
                        </div>
                    </td>
                    <td class="Width5Percent">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnOwnerSort" runat="server" Text="Owner" CommandName="Sort"
                                CssClass="arrow" CommandArgument="Owner" />
                        </div>
                    </td>
                    <td class="Width4Percent" align="center">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnEstimatedRevenue" runat="server" Text="Est. Revenue" CommandName="Sort"
                                CssClass="arrow" CommandArgument="EstimatedRevenue" />
                        </div>
                    </td>
                    <td class="Width4Percent textCenter">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnCreateDateSort" runat="server" Text="Days Old" CommandName="Sort"
                                CssClass="arrow" CommandArgument="CreateDate" />
                        </div>
                    </td>
                    <td class="Width4Percent">
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
                        <asp:HyperLink ID="hlStatus" runat="server" CssClass='<%# PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClass((Opportunity) Container.DataItem)%>'
                            Description='<%# PraticeManagement.Utils.OpportunitiesHelper.GetToolTip((Opportunity) Container.DataItem)%>'
                            onmouseout="HidePanel();" onmouseover="SetTooltipText(this.attributes['Description'].value,this);">                           
                        </asp:HyperLink>
                    </div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblNumber" runat="server" Text='<%# Eval("OpportunityNumber") %>' /></div>
                </td>
                <td align="center">
                    <div class="cell-pad">
                        <asp:Label ID="lblPriority" runat="server" Text='<%# ((Opportunity) Container.DataItem).Priority.HtmlEncodedDisplayName %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblClientName" runat="server" Text='<%# ((Opportunity) Container.DataItem).HtmlEncodedClientAndGroup %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblBuyerName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("BuyerName"))%>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:HyperLink ID="hlName" runat="server" Description='<%# GetWrappedText(((Opportunity) Container.DataItem).Description) %>'
                            onmouseout="HidePanel();" onmouseover="SetTooltipText(this.attributes['Description'].value,this);"
                            NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
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
                <td align="right" class="padRight10">
                    <div class="cell-pad">
                        <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' /></div>
                </td>
                <td class="textCenter">
                    <div class="cell-pad">
                        <asp:Label ID="lblCreateDate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("CreateDate"), true) %>' /></div>
                </td>
                <td class="textRight">
                    <div class="cell-pad">
                        <asp:Label ID="lblLastUpdate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("LastUpdate"), false) %>' /></div>
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr class="alterrow">
                <td>
                    <div class="cell-pad">
                        <asp:HyperLink ID="hlStatus" runat="server" CssClass='<%# PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClass((Opportunity) Container.DataItem)%>'
                            Description='<%# PraticeManagement.Utils.OpportunitiesHelper.GetToolTip((Opportunity) Container.DataItem)%>'
                            onmouseout="HidePanel();" onmouseover="SetTooltipText(this.attributes['Description'].value,this);">                           
                        </asp:HyperLink>
                    </div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblNumber" runat="server" Text='<%# Eval("OpportunityNumber") %>' /></div>
                </td>
                <td align="center">
                    <div class="cell-pad">
                        <asp:Label ID="lblPriority" runat="server" Text='<%# ((Opportunity) Container.DataItem).Priority.HtmlEncodedDisplayName %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblClientName" runat="server" Text='<%# ((Opportunity) Container.DataItem).HtmlEncodedClientAndGroup %>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblBuyerName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("BuyerName"))%>' /></div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:HyperLink ID="hlName" runat="server" Description='<%# GetWrappedText(((Opportunity) Container.DataItem).Description) %>'
                            onmouseout="HidePanel();" onmouseover="SetTooltipText(this.attributes['Description'].value,this);"
                            NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
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
                <td align="right" class="padRight10">
                    <div class="cell-pad">
                        <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' /></div>
                </td>
                <td class="textCenter">
                    <div class="cell-pad">
                        <asp:Label ID="lblCreateDate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("CreateDate"), true) %>' /></div>
                </td>
                <td class="textRight">
                    <div class="cell-pad">
                        <asp:Label ID="lblLastUpdate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("LastUpdate"), false) %>' /></div>
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

