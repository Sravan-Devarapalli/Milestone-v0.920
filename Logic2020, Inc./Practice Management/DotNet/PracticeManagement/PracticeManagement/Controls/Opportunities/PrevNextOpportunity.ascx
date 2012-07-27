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
        <td class="Width100Per">
            <table>
                <tr>
                    <td>
                        <asp:Panel ID="leftArrow" CssClass="scroll-left" runat="server" Visible='<%# Container.ItemIndex == 0 %>'>
                            <asp:HyperLink ID="link" NavigateUrl='<%# GetOpportinityDetailsLink((int) Eval("Id")) %>'
                                onclick="return checkDirtyWithRedirect(this.href);" runat="server">
                                <asp:Label ID="captionLeft" runat="server" Text='<%# Eval("HtmlEncodedName") %>' />
                            </asp:HyperLink></asp:Panel>
                    </td>
                    <td class="padRight5 padLeft5">
                        <uc:ProjectNameCellRounded ID="status" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25" Target="_blank"
                            ButtonCssClass='<%# PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClass((DataTransferObjects.Opportunity) Container.DataItem) %>'
                            ButtonProjectNameToolTip='<%# PraticeManagement.Utils.OpportunitiesHelper.GetToolTip((DataTransferObjects.Opportunity) Container.DataItem) %>' 
                            ButtonProjectNameHref = "<%# GetProjectDetailUrl((DataTransferObjects.Opportunity) Container.DataItem) %>"
                            />

                    </td>
                    <td>
                        <asp:Label ID="client" runat="server" Text='<%# Eval("Client.HtmlEncodedName") %>' CssClass="no-wrap" />
                    </td>
                    <td>
                        <asp:Panel ID="rigthArrow" CssClass="scroll-right" runat="server" Visible='<%# Container.ItemIndex > 0 %>'>
                            <asp:HyperLink ID="HyperLink1" NavigateUrl='<%# GetOpportinityDetailsLink((int) Eval("Id")) %>'
                                onclick="return checkDirtyWithRedirect(this.href);" runat="server">
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("HtmlEncodedName") %>' CssClass="no-wrap" />
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
<asp:ObjectDataSource ID="odsPrevNext" runat="server" SelectMethod="GetOpportunitiesPrevNext"
    TypeName="PraticeManagement.Controls.DataHelper">
    <SelectParameters>
        <asp:QueryStringParameter Name="opportunityId" QueryStringField="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

