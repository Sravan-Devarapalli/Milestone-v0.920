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
                <table class="WholeWidth">
                    <tr>
                        <td colspan="3" style="width: 100%; padding-left: 10px;">
                            <table class="WholeWidth">
                                <tr>
                                    <td style="width: 40px;">
                                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="../Images/expand.jpg"
                                            ExpandedImage="../Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                            ToolTip="Expand Filters" />
                                    </td>
                                    <td valign="middle" style="width: 410px; white-space: nowrap; padding-right: 0px;">
                                        <asp:TextBox runat="server" ID="txtSearch" Width="400px" Style="text-align: left;"
                                            OnTextChanged="txtSearch_TextChanged" MaxLength="40"></asp:TextBox>
                                        <ajaxToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server" TargetControlID="txtSearch"
                                            WatermarkCssClass="watermarkedtext" WatermarkText="To search for a client, click here to begin typing and hit enter...">
                                        </ajaxToolkit:TextBoxWatermarkExtender>
                                    </td>
                                    <td style="width: 2px; padding-left: 0px;">
                                        <asp:RequiredFieldValidator ID="reqSearchText" runat="server" Text="*" ErrorMessage="Please type text to be searched."
                                            ToolTip="Please type text to be searched." ControlToValidate="txtSearch" EnableClientScript="true"
                                            ValidationGroup="ValSearch" />
                                    </td>
                                    <td style="width: 105px;">
                                        <asp:Button ID="btnSearchAll" ValidationGroup="ValSearch" Width="100px" runat="server"
                                            Text="Search All" OnClick="btnSearchAll_OnClick" />
                                    </td>
                                    <td style="width: 115px;">
                                    </td>
                                    <td style="width: 115px; text-align: right;padding-right:0px;">
                                        <asp:DropDownList ID="ddlView" runat="server" OnSelectedIndexChanged="ddlView_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Text="View 25" Value="25"></asp:ListItem>
                                            <asp:ListItem Text="View 50" Value="50"></asp:ListItem>
                                            <asp:ListItem Text="View 100" Value="100"></asp:ListItem>
                                            <asp:ListItem Text="View All" Value="-1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td align="right" style="width: 95px; text-align: right;padding-left:0px;">
                                        <asp:ShadowedHyperlink runat="server" Text="Add Client" ID="lnkAddClient" CssClass="add-btn"
                                            NavigateUrl="~/ClientDetails.aspx?returnTo=Config/Clients.aspx" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="width: 100%; padding-left: 63px;white-space: nowrap;" >
                            <asp:ValidationSummary ID="valsumSearch" runat="server" ValidationGroup="ValSearch" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left" style="width: 17%; white-space: nowrap; padding-left: 10px;">
                            <asp:LinkButton ID="lnkbtnPrevious" runat="server" Text="<- PREVIOUS" Font-Underline="false"
                                OnClick="Previous_Clicked"></asp:LinkButton>
                        </td>
                        <td valign="middle" align="center" style="width: 66%; text-align: center;">
                            <table>
                                <tr id="trAlphabeticalPaging" runat="server">
                                    <td style="padding-left: 10px; padding-top: 10px; padding-bottom: 10px; text-align: center;">
                                        <asp:LinkButton ID="lnkbtnAll" Top="lnkbtnAll" Bottom="lnkbtnAll1" runat="server"
                                            Text="All" Font-Underline="false" Font-Bold="true" OnClick="Alphabet_Clicked"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="right" style="width: 17%; white-space: nowrap; padding-right: 10px;">
                            <asp:LinkButton ID="lnkbtnNext" runat="server" Text="NEXT ->" Font-Underline="false"
                                OnClick="Next_Clicked"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
                <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                    CssClass="CustomTabStyle">
                    <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>Filters</span></a> </span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:CheckBox ID="chbShowActive" runat="server" AutoPostBack="true" Text="Show Active Clients Only"
                                Checked="True" OnCheckedChanged="chbShowActive_CheckedChanged" />
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>
            </asp:Panel>
            <table class="WholeWidth">
                <tr>
                    <td valign="top" style="width: 45%; padding-top: 12px; padding-left: 5px; padding-right: 5px;
                        padding-bottom: 5px;">
                        <asp:GridView ID="gvClients" AllowPaging="true" runat="server" AutoGenerateColumns="False"
                            EmptyDataText="There is nothind to be displayed here." DataKeyNames="Id" CssClass="CompPerfTable"
                            GridLines="None">
                            <AlternatingRowStyle BackColor="#F9FAFF" />
                            <PagerSettings Visible="false" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemStyle Width="250" />
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            Client Name</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:HyperLink ID="btnClientName" Style="padding-left: 5px;" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
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
                    <td valign="top" style="width: 36%; padding-top: 15px; padding-left: 5px; padding-right: 5px;
                        padding-bottom: 25px;">
                        <div style="border: 1px solid black; background-color: White; padding: 5px;">
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
            <div class="buttons-block">
                <table class="WholeWidth">
                    <tr>
                        <td align="left" style="width: 17%; white-space: nowrap; padding-left: 10px;">
                            <asp:LinkButton ID="lnkbtnPrevious1" runat="server" Text="<- PREVIOUS" Font-Underline="false"
                                OnClick="Previous_Clicked"></asp:LinkButton>
                        </td>
                        <td valign="middle" align="center" style="width: 66%; text-align: center;">
                            <table>
                                <tr id="trAlphabeticalPaging1" runat="server">
                                    <td style="padding-left: 10px; padding-top: 10px; padding-bottom: 10px; text-align: center;">
                                        <asp:LinkButton ID="lnkbtnAll1" Top="lnkbtnAll" Bottom="lnkbtnAll1" runat="server"
                                            Text="All" Font-Underline="false" Font-Bold="true" OnClick="Alphabet_Clicked"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="right" style="width: 17%; white-space: nowrap; padding-right: 10px;">
                            <asp:LinkButton ID="lnkbtnNext1" runat="server" Text="NEXT ->" Font-Underline="false"
                                OnClick="Next_Clicked"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:HiddenField ID="hdnAlphabet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

