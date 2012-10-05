<%@ Page Title="Skills Profile | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsProfile.aspx.cs" Inherits="PraticeManagement.SkillsProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <table class="TableSkillsProfile WholeWidth">
        <tr>
            <td colspan="4" class="PaddingBottom30pxImp">
                <center>
                    <h1>
                        Skills Profile for
                        <asp:Label ID="lblPersonName" runat="server"></asp:Label></h1>
                </center>
            </td>
        </tr>
    </table>
    <div class="SkillsProfileBody">
        <div class="SkillsProfileDataBody">
            <table class="WholeWidth">
                <tr>
                    <td class="width77PImp">
                        <table class="TableSkillsProfile WholeWidth TextAlignCenterImp">
                            <asp:Repeater ID="repIndustries" runat="server" OnItemDataBound="repIndustries_OnItemDataBound"
                                OnLoad="repIndustries_OnLoad">
                                <HeaderTemplate>
                                    <tr class="CompPerfHeader">
                                        <td class="Width2PercentImp">
                                            <div class="ie-bg no-wrap">
                                            </div>
                                        </td>
                                        <td class="fontSkilltype Width40PImp">
                                            <div class="ie-bg no-wrap paddingLeft5pxImp t-left">
                                                Industries</div>
                                        </td>
                                        <td class="Width15PercentImp textRightImp">
                                            <div class="ie-bg no-wrap TextAlignCenterImp">
                                                Experience</div>
                                        </td>
                                        <th class="Width15PercentImp">
                                        </th>
                                        <th class="Width10PerImp">
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td colspan="4" class="PaddingTop15Imp fontBold">
                                            <asp:Label ID="lblInduatriesMsg" runat="server" Visible="false"></asp:Label>
                                        </td>
                                    </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <div class="ie-bg no-wrap TextAlignCenterImp">
                                            </div>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblIndustryDesc" runat="server"></asp:Label>
                                        </td>
                                        <td class="TextAlignCenterImp">
                                            <asp:Label ID="lblExp" runat="server"></asp:Label>
                                        </td>
                                        <th colspan="2">
                                        </th>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr>
                                        <td class="alterrow">
                                        </td>
                                        <td class="alterrow">
                                            <asp:Label ID="lblIndustryDesc" runat="server"></asp:Label>
                                        </td>
                                        <td class="alterrow TextAlignCenterImp">
                                            <asp:Label ID="lblExp" runat="server"></asp:Label>
                                        </td>
                                        <th colspan="2">
                                        </th>
                                    </tr>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                            <tr>
                                <td colspan="5" class="space">
                                </td>
                            </tr>
                            <asp:Repeater ID="repTypes" runat="server" OnItemDataBound="repTypes_OnItemDataBound">
                                <ItemTemplate>
                                    <tr class="CompPerfHeader">
                                        <td>
                                            <div class="ie-bg no-wrap TextAlignCenterImp">
                                            </div>
                                        </td>
                                        <td class="fontSkilltype">
                                            <div class="ie-bg no-wrap padLeft5Imp t-left">
                                                <asp:Label ID="lblTypes" runat="server"></asp:Label></div>
                                        </td>
                                        <td>
                                            <div class="ie-bg no-wrap TextAlignCenterImp">
                                                <asp:Label ID="lblLevel" runat="server"></asp:Label></div>
                                        </td>
                                        <td>
                                            <div class="ie-bg no-wrap TextAlignCenterImp">
                                                <asp:Label ID="lblLastUsed" runat="server"></asp:Label></div>
                                        </td>
                                        <td>
                                            <div class="ie-bg no-wrap TextAlignCenterImp">
                                                <asp:Label ID="lblYearsUsed" runat="server"></asp:Label></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td colspan="4" class="PaddingTop15Imp fontBold">
                                            <asp:Label ID="lblTypesMsg" runat="server" Visible="false"></asp:Label>
                                        </td>
                                    </tr>
                                    <asp:Repeater ID="repCategories" runat="server" OnItemDataBound="repCategories_OnItemDataBound">
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                </td>
                                                <th>
                                                    <u>
                                                        <asp:Label ID="lblCategories" runat="server"></asp:Label></u>
                                                </th>
                                                <th colspan="3">
                                                </th>
                                            </tr>
                                            <asp:Repeater ID="repSkills" runat="server" OnItemDataBound="repSkills_OnItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblSkill" runat="server"></asp:Label>
                                                            <span class="WidthInherit"></span>
                                                        </td>
                                                        <td class="TextAlignCenterImp">
                                                            <asp:Label ID="lblSkillLevel" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="TextAlignCenterImp">
                                                            <asp:Label ID="lblLastUsed" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="TextAlignCenterImp">
                                                            <asp:Label ID="lblYearsUsed" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <AlternatingItemTemplate>
                                                    <tr class="alterrow">
                                                        <td>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblSkill" runat="server"></asp:Label>
                                                            <span class="WidthInherit"></span>
                                                        </td>
                                                        <td class="TextAlignCenterImp">
                                                            <asp:Label ID="lblSkillLevel" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="TextAlignCenterImp">
                                                            <asp:Label ID="lblLastUsed" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="TextAlignCenterImp">
                                                            <asp:Label ID="lblYearsUsed" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </AlternatingItemTemplate>
                                            </asp:Repeater>
                                            <tr>
                                                <td colspan="5" class="space1">
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <tr>
                                        <td colspan="5" class="space">
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr>
                                <td colspan="5" class="space">
                                </td>
                            </tr>
                            <asp:Repeater ID="repProfiles" runat="server" OnLoad="repProfiles_OnLoad">
                                <HeaderTemplate>
                                    <tr class="CompPerfHeader">
                                        <td>
                                            <div class="ie-bg no-wrap">
                                            </div>
                                        </td>
                                        <td class="fontSkilltype">
                                            <div class="ie-bg no-wrap t-left paddingLeft5pxImp">
                                                Consultant Profile</div>
                                        </td>
                                        <td>
                                            <div class="ie-bg no-wrap TextAlignCenterImp">
                                                Last Updated</div>
                                        </td>
                                        <td colspan="2">
                                            <div class="ie-bg no-wrap TextAlignCenterImp">
                                                Modified By</div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td colspan="4" class="PaddingTop15Imp fontBold">
                                            <asp:Label ID="lblProfileMsg" runat="server" Visible="false"></asp:Label>
                                        </td>
                                    </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                        </td>
                                        <td class="padLeft10 PaddingBottom2">
                                            <table>
                                                <td class="vMiddle">
                                                    <asp:HyperLink ID="hlPersonProfile" runat="server" Text='<%# Eval("ProfileName") %>'
                                                        Target="_blank" NavigateUrl='<%# Eval("ProfileUrl") %>'></asp:HyperLink>&nbsp;&nbsp;
                                                </td>
                                                <td>
                                                    <asp:Image ID="imgIsDefault" runat="server" ImageUrl="~/Images/Profile-16x16.png"
                                                        ToolTip="Default Profile." Visible='<%# (bool)Eval("IsDefault")%>' />
                                                </td>
                                            </table>
                                        </td>
                                        <td class="TextAlignCenterImp">
                                            <%# Eval("ModifiedDate") %>
                                        </td>
                                        <td class="TextAlignCenterImp" colspan="2">
                                            <%# Eval("ModifiedByName")%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr class="alterrow">
                                        <td>
                                        </td>
                                        <td class="padLeft10 PaddingBottom2">
                                            <table>
                                                <td class="vMiddle">
                                                    <asp:HyperLink ID="hlPersonProfile" runat="server" Text='<%# Eval("ProfileName") %>'
                                                        Target="_blank" NavigateUrl='<%# Eval("ProfileUrl") %>'></asp:HyperLink>&nbsp;&nbsp;
                                                </td>
                                                <td>
                                                    <asp:Image ID="imgIsDefault" runat="server" ImageUrl="~/Images/Profile-16x16.png"
                                                        ToolTip="Default Profile." Visible='<%# (bool)Eval("IsDefault")%>' />
                                                </td>
                                            </table>
                                        </td>
                                        <td class="TextAlignCenterImp">
                                            <%# Eval("ModifiedDate") %>
                                        </td>
                                        <td class="TextAlignCenterImp" colspan="2">
                                            <%# Eval("ModifiedByName")%>
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </table>
                    </td>
                    <td class="Width23Percent vTop padRight15" align="right">
                        <div class="PersonPictureDiv">
                            <asp:Image ID="imgPersonPicture" runat="server" Width="100" Height="120" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <table class="TableSkillsProfile WholeWidth">
        <tr>
            <td class="TextAlignCenterImp PaddingTop15Imp">
                <asp:Button runat="server" ID="btnUpdate" Text="Update" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

