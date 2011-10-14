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
    <script type="text/javascript">
        function LoadPanel(container) {
            var name = panel.activeTabIndex;
        }

        function ddlLevelChanged(ddlLevel) {
            setDirty();
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <div style="text-align: center; font-size: large;">
        Skills Entry for
        <asp:Label runat="server" ID="lblUserName"></asp:Label>
    </div>
    <asp:UpdatePanel ID="upSkills" runat="server">
        <ContentTemplate>
            <AjaxControlToolkit:TabContainer runat="server" ID="tcSkillsEntry" ActiveTabIndex="0"
                AutoPostBack="true" OnActiveTabChanged="tcSkillsEntry_ActiveTabChanged">
                <AjaxControlToolkit:TabPanel runat="server" onClick="checkDirtyBase();" ID="tpBussinessSkills" HeaderText="Bussiness Skills">
                    <ContentTemplate>
                        <div style="background-color: #d4dff8; padding: 5px;">
                            <div style="padding: 5px;">
                                <asp:Label runat="server" ID="lblCategory" Text="Category"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlBusinessCategory" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                    DataTextField="Description" DataValueField="Id" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div style="background-color: White; overflow: auto;">
                                <asp:GridView ID="gvBusinessSkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvSkills_RowDataBound"
                                    CssClass="WholeWidth">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField ID="hdnChanged" runat="server" Value="0" />
                                                <asp:Label ID="lblSkillsDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                Level
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel" SkillId='<%# Eval("Id") %>' onchange="ddlLevelChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id" SkillId='<%# Eval("Id") %>' onchange="setDirty();">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                Last Used
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLastUsed" DataSourceID="odsLastUsed" DataTextField="Name"
                                                    DataValueField="Id" SkillId='<%# Eval("Id") %>' onchange="setDirty();">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear"  SkillId='<%# Eval("Id") %>' onchange="setDirty();"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel runat="server" ID="tpTechnicalSkills" onClick="checkDirtyBase();" HeaderText="Technical Skills">
                    <ContentTemplate>
                        <div style="background-color: #d4dff8; padding: 5px;">
                            <div style="padding: 5px;">
                                <asp:Label runat="server" ID="Label1" Text="Category"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlTechnicalCategory" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                    DataTextField="Description" DataValueField="Id" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div style="background-color: White; overflow: auto;">
                                <asp:GridView ID="gvTechnicalSkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvSkills_RowDataBound"
                                    CssClass="WholeWidth">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblSkillsDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                Level
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                Last Used
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLastUsed" DataSourceID="odsLastUsed" DataTextField="Name"
                                                    DataValueField="Id">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel runat="server" ID="tpIndustrySkills" onClick="checkDirtyBase();" HeaderText="Industry Skills">
                    <ContentTemplate>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
            </AjaxControlToolkit:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div style="float: right;">
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
    </div>
    <asp:ObjectDataSource ID="odsSkillLevel" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetSkillLevels"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsExperience" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetExperiences"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsLastUsed" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetLastUsedYears"></asp:ObjectDataSource>
</asp:Content>

