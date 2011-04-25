<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpportunityList.ascx.cs"
    Inherits="PraticeManagement.Controls.Generic.OpportunityList" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="System.Data" %>
<%@ Register Src="~/Controls/ProjectNameCellRounded.ascx" TagName="ProjectNameCellRounded"
    TagPrefix="uc" %>

<div id="opportunity-list">
    <asp:ListView ID="lvOpportunities" runat="server" DataKeyNames="Id" onsorting="lvOpportunities_Sorting">
        <layouttemplate>
            <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">            
                <tr runat="server" id="lvHeader" class="CompPerfHeader">
                    <td width="1%">
                        <div class="ie-bg no-wrap">                        
                        </div>
                    </td>
                    <td width="4%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnNumberSort" runat="server" Text="Opp. #" CommandName="Sort" CssClass="arrow" CommandArgument="Number"/>
                        </div>
                    </td>
                    <td width="4%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnPrioritySort" runat="server" Text="Priority" CommandName="Sort" CssClass="arrow" CommandArgument="Priority" />
                        </div>
                    </td> 
                    <td width="15%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnClientNameSort" runat="server" Text="Client - Group" CommandName="Sort" CssClass="arrow" CommandArgument="ClientName" />
                        </div>
                    </td>
                    <td width="11%" >
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnBuyerNameSort" runat="server" Text="Buyer Name" CommandName="Sort" CssClass="arrow" CommandArgument="BuyerName"/>
                        </div>
                    </td>
                    <td width="25%">
                        <div class="ie-bg no-wrap" style="white-space:nowrap">
                            <asp:LinkButton ID="btnOpportunityNameSort" runat="server" Text="Opportunity Name" CommandName="Sort" CssClass="arrow" CommandArgument="OpportunityName"/>
                        </div>
                    </td>
                    <td width="7%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnSalespersonSort" runat="server" Text="Salesperson" CommandName="Sort" CssClass="arrow" CommandArgument="Salesperson"/>
                        </div>
                    </td>
                     <td width="5%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnOwnerSort" runat="server" Text="Owner" CommandName="Sort" CssClass="arrow" CommandArgument="Owner"/>
                        </div>
                    </td>
                    <td align="center"  width="4%">
                        <div class="ie-bg no-wrap">
                            <asp:LinkButton ID="btnEstimatedRevenue" runat="server" Text="Est. Revenue" CommandName="Sort" CssClass="arrow" CommandArgument="EstimatedRevenue"/>
                        </div>
                    </td>
                    <td width="4%" style="text-align:center;">
                        <div class="ie-bg no-wrap" >
                            <asp:LinkButton ID="btnCreateDateSort" runat="server" Text="Days Old" CommandName="Sort" CssClass="arrow" CommandArgument="CreateDate"/>
                        </div>
                    </td>
                    <td width="4%" >
                        <div class="ie-bg no-wrap" >
                            <asp:LinkButton ID="btnLastUpdate" runat="server" Text="Last Change" CommandName="Sort" CssClass="arrow" CommandArgument="Updated"/>
                        </div>
                    </td>
                </tr>
                <tr runat="server" id="itemPlaceholder" />
            </table>
        </layouttemplate>
        <itemtemplate>
            <tr>
                <td>
                    <div class="cell-pad">
                        <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25" ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' ButtonCssClass='<%#PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClassByStatus((string)Eval("Status.Name"))%>'/>
                    </div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblNumber" runat="server" Text='<%# Eval("OpportunityNumber") %>' /></div>
                </td>
                <td align="center">
                    <div class="cell-pad">
                        <asp:Label ID="lblPriority" runat="server" Text='<%# Eval("Priority") %>' /></div>
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
                <td align="right" style="padding-right:10px;">
                    <div class="cell-pad">
                        <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' /></div>
                </td>
                <td style="text-align:center;">
                    <div class="cell-pad">
                        <asp:Label ID="lblCreateDate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("CreateDate"), true) %>' /></div>
                </td>
                <td style="text-align:right;">
                    <div class="cell-pad" >
                        <asp:Label ID="lblLastUpdate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("LastUpdate"), false) %>' /></div>
                </td>
            </tr>
        </itemtemplate>
        <alternatingitemtemplate>
            <tr style="background: #F9FAFF;">
                <td>
                    <div class="cell-pad">
                        <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25" ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' ButtonCssClass='<%#PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClassByStatus((string)Eval("Status.Name"))%>'/>
                    </div>
                </td>
                <td>
                    <div class="cell-pad">
                        <asp:Label ID="lblNumber" runat="server" Text='<%# Eval("OpportunityNumber") %>' /></div>
                </td>
                <td align="center">
                    <div class="cell-pad">
                        <asp:Label ID="lblPriority" runat="server" Text='<%# Eval("Priority") %>' /></div>
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
                <td align="right" style="padding-right:10px;">
                    <div class="cell-pad" >
                        <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' /></div>
                </td>
                <td style="text-align:center;">
                    <div class="cell-pad">
                        <asp:Label ID="lblCreateDate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("CreateDate"),true) %>' IsCreateDate="true" /></div>
                </td>
                 <td style="text-align:right;">
                    <div class="cell-pad">
                        <asp:Label ID="lblLastUpdate" runat="server" Text='<%# GetDaysOld((DateTime)Eval("LastUpdate"),false) %>' IsCreateDate="false" /></div>
                </td>
           </tr>
        </alternatingitemtemplate>
        <emptydatatemplate> 
            <tr runat="server" id="EmptyDataRow">
                <td>
                    No opportunities found.
                </td>
            </tr>
        </emptydatatemplate>
    </asp:ListView>
    <%-- <asp:ObjectDataSource ID="odsOpportunities" runat="server" 
                SelectMethod="GetOpportunities" EnableCaching="false" 
                TypeName="PraticeManagement.Controls.Generic.OpportunityList">
   </asp:ObjectDataSource>--%>
</div>

