<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpportunityFilter.ascx.cs"
    Inherits="PraticeManagement.Controls.Generic.Filtering.OpportunityFilter" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.SearchHighlighting" TagPrefix="ext" %>
<ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="CustomTabStyle">
    <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
        <HeaderTemplate>
            <span class="bg"><a href="#"><span>Filters</span></a> </span>
        </HeaderTemplate>
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        <asp:CheckBox ID="chbActiveOnly" runat="server" Checked="true" AutoPostBack="true"
                            Text="Active Opportunities Only" OnCheckedChanged="chbActiveOnly_CheckedChanged" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        Filter by Client
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlClients" runat="server" AutoPostBack="true" CssClass="WholeWidth"
                            OnSelectedIndexChanged="Filter_SelectedIndexChanged" DataSourceID="odsClients"
                            DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true">
                                <asp:ListItem Value="">All</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Filter by Salesperson
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSalespersons" runat="server" AutoPostBack="true" CssClass="WholeWidth"
                            OnSelectedIndexChanged="Filter_SelectedIndexChanged" DataSourceID="odsSalespersons"
                            DataTextField="PersonLastFirstName" DataValueField="Id" AppendDataBoundItems="true">
                                <asp:ListItem Value="">All</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="tpSearch">
        <HeaderTemplate>
            <span class="bg"><a href="#"><span>Search</span></a> </span>
        </HeaderTemplate>
        <ContentTemplate>
            <asp:Panel ID="pnlSsearch" runat="server" DefaultButton="btnSearch">
                <table class="WholeWidth">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="WholeWidth"></asp:TextBox>
                                <ext:SearchHighlightingExtender ID="searchHighlighting" runat="server"
                                    TargetControlID="txtSearch" SearchInsideBlockId="opportunity-list" /> 
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td style="width: 100px;">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click"
                                Width="100px" CssClass="pm-button" />
                        </td>
                    </tr>
<%--                    <tr>
                        <td>
                            <br />
                            <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearch"
                                ErrorMessage="Please type text to be searched." SetFocusOnError="true" CssClass="searchError"
                                Display="Dynamic" EnableClientScript="false" />
                        </td>
                    </tr>--%>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<asp:ObjectDataSource ID="odsClients" runat="server" SelectMethod="ClientListAll"
    TypeName="PraticeManagement.ClientService.ClientServiceClient"></asp:ObjectDataSource>
<asp:ObjectDataSource ID="odsSalespersons" runat="server" SelectMethod="GetSalespersonList"
    TypeName="PraticeManagement.PersonService.PersonServiceClient">
    <SelectParameters>
        <asp:Parameter DefaultValue="false" Name="includeInactive" Type="Boolean" />
    </SelectParameters>
</asp:ObjectDataSource>

