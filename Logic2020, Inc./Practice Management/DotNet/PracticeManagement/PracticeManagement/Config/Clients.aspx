<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Clients.aspx.cs" Inherits="PraticeManagement.Config.Clients" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Clients</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Clients
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="pnlBody" runat="server">
        <ContentTemplate>
            <div class="buttons-block">
                <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                    ImageControlID="btnExpandCollapseFilter" CollapsedImage="../Images/expand.jpg"
                    ExpandedImage="../Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                    ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                    ToolTip="Expand Filters" />
                <asp:ShadowedHyperlink runat="server" Text="Add Client" ID="lnkAddClient" CssClass="add-btn"
                    NavigateUrl="~/ClientDetails.aspx?returnTo=Config/Clients.aspx" />
            </div>
            <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
                <AjaxControlToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
                    CssClass="CustomTabStyle">
                    <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                        <HeaderTemplate>
                            <span class="bg  DefaultCursor"><span class="NoHyperlink">Filters</span></span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:CheckBox ID="chbShowActive" runat="server" AutoPostBack="true" Text="Show Active Clients Only"
                                Checked="True" OnCheckedChanged="chbShowActive_CheckedChanged" />
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>
            </asp:Panel>
            <br />
            <table class="WholeWidth">
                <tr>
                    <td valign="top" style="width: 45%; padding: 5px;">
                        <asp:GridView ID="gvClients" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothind to be displayed here."
                            DataKeyNames="Id" CssClass="CompPerfTable" GridLines="None">
                            <AlternatingRowStyle BackColor="#F9FAFF" />
                            <Columns>
                                <asp:TemplateField>
                                 <ItemStyle  Width="250"  />
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            Client Name</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:HyperLink ID="btnClientName" style="padding-left:5px;" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
                                            NavigateUrl='<%# GetClientDetailsUrlWithReturn(Eval("Id")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" Width="50" />
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            Active</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chbInactive" AutoPostBack="true" ClientId='<%# Eval("Id") %>' OnCheckedChanged="chbInactive_CheckedChanged"
                                            runat="server" Checked='<%# !Convert.ToBoolean(Eval("Inactive")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" Width="120" />
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            Billable by default</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chbIsChargeable" ClientId='<%# Eval("Id") %>' AutoPostBack="true"
                                            runat="server" OnCheckedChanged="chbIsChargeable_CheckedChanged" Checked='<%# Convert.ToBoolean(Eval("IsChargeable")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                     <td style="width: 9%; padding: 5px;">
                    </td>
                    <td valign="top" style="width: 36%; padding-top:50px;;padding-left: 5px;padding-right: 5px;padding-bottom: 25px;">
                        <div style="border: 1px solid black;background-color: #d4dff8; padding: 5px;">
                            <span>To add a new Client to Practice Management, click the "Add Client" button and
                                enter the required data. Upon saving you will be returned to this page.<br />
                                <br />
                                To edit the properties of any existing Client, click the client name and make any
                                necessary changes. Upon saving you will be returned to this page.<br />
                                <br />
                                To make a Client Active or Inactive, check or uncheck the box in the "Active" column.
                                The Change will take effect immediately, but the Client will not be added to or
                                removed from the default view here until the page is refreshed or revisited.<br />
                                <br />
                                To change whether a Client is Billable by default, check or uncheck the box in the
                                "Billable by default" column. The Change will take effect immediately, but previous/existing
                                Projects and Milestones linked to this Client will not be altered. </span>
                        </div>
                    </td>
                    <td style="width: 10%; padding: 5px;">
                    </td>
                </tr>
            </table>
            <table class="WholeWidth" style="background-color: #d4dff8;">
                <tr>
                    <td id="tdAlphabeticalPaging" runat="server" style="padding: 10px; text-align: center;">
                        <asp:LinkButton ID="lnkbtnAll" runat="server" Text="All" Font-Underline="false" Font-Bold="true"
                            OnClick="Alphabet_Clicked"></asp:LinkButton>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnAlphabet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

