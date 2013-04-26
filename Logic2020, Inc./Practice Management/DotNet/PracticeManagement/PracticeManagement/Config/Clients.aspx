<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Clients.aspx.cs" Inherits="PraticeManagement.Config.Clients" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Accounts | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Accounts
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
<uc:LoadingProgress ID="lpOpportunityDetails" runat="server" DisplayText="Please Wait..." />
    <asp:UpdatePanel ID="pnlBody" runat="server">
        <ContentTemplate>
            <div class="buttons-block clientsPage">
                <table class="WholeWidth">
                    <tr class="vmiddle">
                        <td colspan="3">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="Width4Percent">
                                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="../Images/expand.jpg"
                                            ExpandedImage="../Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                            ToolTip="Expand Filters" />
                                    </td>
                                    <td class="vMiddle Width39Percent no-wrap padLeft5">
                                        <table class="WholeWidth">
                                            <tr>
                                                <td class="Width97Percent">
                                                    <asp:TextBox runat="server" ID="txtSearch" CssClass="WholeWidthImp textLeft" OnTextChanged="txtSearch_TextChanged"
                                                        MaxLength="40"></asp:TextBox>
                                                    <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server"
                                                        TargetControlID="txtSearch" WatermarkCssClass="watermarkedtext WholeWidthImp textLeft"
                                                        WatermarkText="To search for an Account, click here to begin typing and hit enter...">
                                                    </AjaxControlToolkit:TextBoxWatermarkExtender>
                                                </td>
                                                <td class="Width3Percent">
                                                    <asp:RequiredFieldValidator ID="reqSearchText" runat="server" Text="*" ErrorMessage="Please type text to be searched."
                                                        ToolTip="Please type text to be searched." ControlToValidate="txtSearch" EnableClientScript="true"
                                                        ValidationGroup="ValSearch" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width1Percent">
                                        &nbsp;
                                    </td>
                                    <td class="Width11Percent">
                                        <asp:Button ID="btnSearchAll" ValidationGroup="ValSearch" CssClass="WholeWidthImp" runat="server"
                                            Text="Search All" OnClick="btnSearchAll_OnClick" />
                                    </td>
                                    <td class="Width1Percent">
                                        &nbsp;
                                    </td>
                                    <td class="Width11Percent">
                                        <asp:Button ID="btnClearResults" CssClass="WholeWidthImp" Enabled="false" runat="server" Text="Clear Results"
                                            OnClick="ResetFilter_Clicked" />
                                    </td>
                                    <td class="Width9Percent">
                                    </td>
                                    <td class="Width12Percent textRight">
                                        <asp:DropDownList ID="ddlView" runat="server" OnSelectedIndexChanged="ddlView_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Text="View 25" Value="25"></asp:ListItem>
                                            <asp:ListItem Text="View 50" Value="50"></asp:ListItem>
                                            <asp:ListItem Text="View 100" Value="100"></asp:ListItem>
                                            <asp:ListItem Text="View All" Value="-1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="Width12Percent textRight">
                                        <asp:ShadowedHyperlink runat="server" Text="Add Account" ID="lnkAddClient" CssClass="add-btn"
                                            NavigateUrl="~/ClientDetails.aspx?returnTo=Config/Clients.aspx" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="WholeWidth padLeft3Per no-wrap;">
                            <asp:ValidationSummary ID="valsumSearch" runat="server" ValidationGroup="ValSearch" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Width17Percent no-wrap padLeft10 textLeft">
                            <asp:LinkButton ID="lnkbtnPrevious" runat="server" Text="<- PREVIOUS" 
                                OnClick="Previous_Clicked"></asp:LinkButton>
                        </td>
                        <td  class="Width66Percent vMiddle textcenter">
                            <table class="WholeWidth">
                                <tr id="trAlphabeticalPaging" runat="server">
                                    <td class="ClientAlphabeticAll">
                                        <asp:LinkButton ID="lnkbtnAll" Top="lnkbtnAll" Bottom="lnkbtnAll1" runat="server"
                                            Text="All"  CssClass="fontBold" OnClick="Alphabet_Clicked"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="Width17Percent no-wrap padRight10 textRight">
                            <asp:LinkButton ID="lnkbtnNext" runat="server" Text="NEXT ->" 
                                OnClick="Next_Clicked"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdnActive" runat="server" Value="true" />
                <asp:HiddenField ID="hdnCleartoDefaultView" runat="server" Value="false" />
            </div>
            <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
                <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                    CssClass="CustomTabStyle">
                    <AjaxControlToolkit:TabPanel runat="server" ID="tpMainFilters">
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>Filters</span></a> </span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:CheckBox ID="chbShowActive" runat="server" AutoPostBack="true" Text="Show Active Accounts Only"
                                Checked="True" OnCheckedChanged="chbShowActive_CheckedChanged" />
                        </ContentTemplate>
                    </AjaxControlToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>
            </asp:Panel>
            <table class="WholeWidth">
                <tr>
                    <td class="gvClientsTd">
                        <asp:GridView ID="gvClients" AllowPaging="true" runat="server" AutoGenerateColumns="False"
                            OnPreRender="gvClients_PreRender" EmptyDataText="There is nothind to be displayed here."
                            DataKeyNames="Id" CssClass="CompPerfTable gvClients">
                            <AlternatingRowStyle CssClass="alterrow" />
                            <PagerSettings Visible="false" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderStyle CssClass="Width250Px" />
                                    <ItemStyle CssClass="Left" />
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            Account Name</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:HyperLink ID="btnClientName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
                                            NavigateUrl='<%# GetClientDetailsUrlWithReturn(Eval("Id")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderStyle CssClass="width50Px" />
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
                                    <HeaderStyle CssClass="Width120Px" />
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            Billable by default</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chbIsChargeable" ClientId='<%# Eval("Id") %>' AutoPostBack="true"
                                            runat="server" OnCheckedChanged="chbIsChargeable_CheckedChanged" Checked='<%# Convert.ToBoolean(Eval("IsChargeable")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderStyle CssClass="Width120Px" />
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            Is Note Required</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chbIsNoteRequired" ClientId='<%# Eval("Id") %>' AutoPostBack="true"
                                            runat="server" OnCheckedChanged="chbIsNoteRequired_CheckedChanged" Checked='<%# Convert.ToBoolean(Eval("IsNoteRequired")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                    <td class="Width9Percent Padding5">
                    </td>
                    <td class="ClientsPageInfoTd">
                        <div>
                            <span>To add a new Account to Practice Management, click the "Add Account" button and
                                enter the required data. Upon saving you will be returned to this page.<br />
                                <br />
                                To edit the properties of any existing Account, click the account name and make
                                any necessary changes. Upon saving you will be returned to this page.<br />
                                <br />
                                To make an Account Active or Inactive, check or uncheck the box in the "Active"
                                column. The Change will take effect immediately, but the Account will not be added
                                to or removed from the default view here until the page is refreshed or revisited.<br />
                                <br />
                                To change whether an Account is Billable by default, check or uncheck the box in
                                the "Billable by default" column. The Change will take effect immediately, but previous/existing
                                Projects and Milestones linked to this Account will not be altered. </span>
                        </div>
                    </td>
                    <td class="Width10Percent Padding5">
                    </td>
                </tr>
            </table>
            <div class="buttons-block clientsPage">
                <table class="WholeWidth">
                    <tr>
                         <td class="Width17Percent no-wrap padLeft10 textLeft">
                            <asp:LinkButton ID="lnkbtnPrevious1" runat="server" Text="<- PREVIOUS" 
                                OnClick="Previous_Clicked"></asp:LinkButton>
                        </td>
                        <td  class="Width66Percent vMiddle textcenter">
                            <table class="WholeWidth">
                                <tr id="trAlphabeticalPaging1" runat="server">
                                    <td class="ClientAlphabeticAll">
                                        <asp:LinkButton ID="lnkbtnAll1" Top="lnkbtnAll" Bottom="lnkbtnAll1" runat="server"
                                            Text="All"  CssClass="fontBold" OnClick="Alphabet_Clicked"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="Width17Percent no-wrap padRight10 textRight">
                            <asp:LinkButton ID="lnkbtnNext1" runat="server" Text="NEXT ->" 
                                OnClick="Next_Clicked"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="textCenter">
                            <asp:Label ID="lblPageNumbering" runat="server" CssClass="fontBold TextBlack"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:HiddenField ID="hdnAlphabet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
    

