<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsEntry.aspx.cs" Inherits="PraticeManagement.SkillsEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style>
        .SkillsTabContainer .ajax__tab_xp
        {
        }
    </style>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <div style="text-align: center; font-size: large;">
        Skills Entry for
        <asp:Label runat="server" ID="lblUserName"></asp:Label>
    </div>
    <AjaxControlToolkit:TabContainer runat="server" ID="tcSkillsEntry" ActiveTabIndex="0">
        <AjaxControlToolkit:TabPanel runat="server" ID="tpBussinessSkills" HeaderText="Bussiness Skills">
            <ContentTemplate>
                <asp:UpdatePanel ID="upBussinessSkills" runat="server">
                    <ContentTemplate>
                        <div style="background-color: #d4dff8; padding: 5px;">
                            <div style="padding: 5px;">
                                <asp:Label runat="server" ID="lblCategory" Text="Category"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlCategory" DataSourceID="odsSkillCategories"
                                    DataTextField="Description" DataValueField="Id">
                                </asp:DropDownList>
                            </div>
                            <div style="background-color: White; overflow: auto;">
                                <table class="WholeWidth">
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td style="width: 14%;">
                                            Level
                                        </td>
                                        <td style="width: 12%;">
                                            Experience
                                        </td>
                                        <td style="width: 12%;">
                                            Last Used
                                        </td>
                                        <td style="width: 12%;">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 50%; padding: 5px;">
                                            Skills Text
                                        </td>
                                        <td>
                                            <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                DataSourceID="odsSkillLevel">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                DataTextField="Name" DataValueField="Id">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList runat="server" ID="ddlLastUsed" DataSourceID="odsLastUsed" DataTextField="Name" DataValueField="Id" >
                                                 
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear"></asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </AjaxControlToolkit:TabPanel>
        <AjaxControlToolkit:TabPanel runat="server" ID="tpTechnicalSkills" HeaderText="Technical Skills">
            <ContentTemplate>
                <asp:UpdatePanel ID="upTechnicalSkills" runat="server">
                    <ContentTemplate>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </AjaxControlToolkit:TabPanel>
        <AjaxControlToolkit:TabPanel runat="server" ID="tpIndustrySkills" HeaderText="Industry Skills">
            <ContentTemplate>
                <asp:UpdatePanel ID="upIndustrySkills" runat="server">
                    <ContentTemplate>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </AjaxControlToolkit:TabPanel>
    </AjaxControlToolkit:TabContainer>
    <br />
    <div style="float: right;">
        <asp:Button ID="btnSave" runat="server" Text="Save" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
    </div>
    <asp:ObjectDataSource ID="odsSkillCategories" runat="server" TypeName="PraticeManagement.Utils.SettingsHelper"
        SelectMethod="GetSkillCategoriesByType" OnSelecting="odsSkillCategories_OnSelecting">
        <SelectParameters>
            <asp:Parameter Name="skillTypeId" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsSkillLevel" runat="server" TypeName="PraticeManagement.Utils.SettingsHelper"
        SelectMethod="GetSkillLevels"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsExperience" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetExperiences"></asp:ObjectDataSource>

        <asp:ObjectDataSource ID="odsLastUsed" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetLastUsedYears"></asp:ObjectDataSource>
</asp:Content>

