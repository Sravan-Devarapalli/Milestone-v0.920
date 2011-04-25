<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrevNextOpportunity.ascx.cs"
    Inherits="PraticeManagement.Controls.Opportunities.PrevNextOpportunity" %>
<%@ Register Src="~/Controls/ProjectNameCellRounded.ascx" TagName="ProjectNameCellRounded"
    TagPrefix="uc" %>
<asp:Repeater ID="repPrevNext" runat="server" DataSourceID="odsPrevNext">
    <HeaderTemplate>
        <div class="main-content" id="divPrevNextMainContent">
            <div class="page-hscroll-wrapper">
                <div class="side-r">
                </div>
                <div class="side-l">
                </div>
                <table class="WholeWidth">
                    <tr>
    </HeaderTemplate>
    <ItemTemplate>
        <td style="width: 100%">
            <table>
                <tr>
                    <td>
                        <asp:Panel ID="leftArrow" CssClass="scroll-left" runat="server" Visible='<%# Container.ItemIndex == 0 %>'>
                            <asp:HyperLink ID="link" NavigateUrl='<%# GetOpportinityDetailsLink((int) Eval("Id")) %>'
                                onclick="return checkDirtyWithRedirect(this.href);" runat="server">
                                <asp:Label ID="captionLeft" runat="server" Text='<%# Eval("Name") %>' />
                            </asp:HyperLink></asp:Panel>
                    </td>
                    <td style="padding-right: 5px; padding-left: 5px">
                        <uc:ProjectNameCellRounded ID="status" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                            ButtonCssClass='<%# PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClassByStatus((string) Eval("Status.Name")) %>'
                            ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' />
                    </td>
                    <td>
                        <asp:Label ID="client" runat="server" Text='<%# Eval("Client.Name") %>' Style="white-space: nowrap" />
                    </td>
                    <td>
                        <asp:Panel ID="rigthArrow" CssClass="scroll-right" runat="server" Visible='<%# Container.ItemIndex > 0 %>'>
                            <asp:HyperLink ID="HyperLink1" NavigateUrl='<%# GetOpportinityDetailsLink((int) Eval("Id")) %>'
                                onclick="return checkDirtyWithRedirect(this.href);" runat="server">
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Name") %>' Style="white-space: nowrap" />
                            </asp:HyperLink></asp:Panel>
                    </td>
                </tr>
            </table>
        </td>
    </ItemTemplate>
    <FooterTemplate>
        </tr> </table> </div> </div>
    </FooterTemplate>
</asp:Repeater>
<%--        <table class="WholeWidth">            
            <tr>
                <td style="width:50%">
                    <table>            
                        <tr> 
                            <td>
                                <div id="divLeft" class="scroll-left" runat="server">
			                        <asp:HyperLink id="hlLeft" NavigateUrl="#" onclick="return checkDirtyWithRedirect(this.href);" runat="server" ><span id="captionLeft" runat="server"></span></asp:HyperLink>
			                    </div>
                            </td>                   
                            <td style="padding-right:5px; padding-left:5px">
                                <uc:ProjectNameCellRounded ID="crStatusLeft" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"/>
                            </td>
                            <td>
                                <label id="lblLeft" runat="server"></label>
                            </td>                
                        </tr>
                    </table>
                </td>
                <td style="width:50%;" align="right">
                    <table>            
                        <tr>                               
                            <td style="padding-right:5px;">
                                <uc:ProjectNameCellRounded ID="crStatusRight" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"/>                       
                            </td>
                            <td style="padding-right:5px;">
                                <label id="lblRight" runat="server"></label>
                            </td>
                            <td>
                                <div id="divRight" class="scroll-right" runat="server">	
                                    <asp:HyperLink id="hlRight" NavigateUrl="#" onclick="return checkDirtyWithRedirect(this.href);" runat="server"><span id="captionRight" runat="server"></span></asp:HyperLink>	
                                </div>
                            </td>
                        </tr>
                    </table> 
                </td>
            </tr>
        </table> 
--%>
<asp:ObjectDataSource ID="odsPrevNext" runat="server" SelectMethod="GetOpportunitiesPrevNext"
    TypeName="PraticeManagement.Controls.DataHelper">
    <SelectParameters>
        <asp:QueryStringParameter Name="opportunityId" QueryStringField="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

