<%@ Page Title="Skills Entry | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsEntry.aspx.cs" Inherits="PraticeManagement.SkillsEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style>
        .TabPadding td, .TabPadding th
        {
            padding:4px;
        } 
        
        .ajax__tab_body
        {
            border-bottom:#999999 1px solid !important;
            margin-top:1px;
            padding:2px !important;
        }
        
        .SkillsBody
        {
            background-color: #d4dff8; 
            padding: 10px;
        }
        
        .SkillsDataBody
        {
            background-color: White;
            height:400px; 
            overflow: auto;
        }
    </style>
    <script type="text/javascript" language="javascript">

        function ddlLevelChanged(changedObject) {
            //var hdnChanged = changedObject.parentElement.parentElement.cells[0].childNodes[2];
            var row = $(changedObject.parentNode.parentNode);
            row.find("input[id$='hdnChanged']")[0].value = 1;
            setDirty();
        }

        function ClearAllFields(clearLink) {
            var row = $(clearLink.parentNode.parentNode);
            row.find("select[id$='ddlLevel']")[0].value = 0;
            row.find("select[id$='ddlExperience']")[0].value = 0;
            row.find("select[id$='ddlLastUsed']")[0].value = 0;
            var ddlLevel = clearLink;
        }

        function CheckDirtyBehaviour(ddl) {

            //var id = ddlLevel.attributes['abcd'].value;
            var result = true;
            //            if (getDirty()) {
            //                result = confirm("Some data isn't saved. Click Ok to Save the data or Cancel to continue without saving.");
            //                if (result) {
            //                    return true;
            //                }
            //                else {
            //                    var previous = ddl.attributes['PreviousSelected'];
            //                    if (previous != null) {
            //                        var index = previous.value;
            //                        var selectItem = ddl[index];
            //                        if (selectItem != null) {
            //                            selectItem.selected = true;
            //                        }
            //                    }
            //                    return false;
            //                }
            //            }

            return result;
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
                <AjaxControlToolkit:TabPanel runat="server" onclick="confirmSaveDirty();" ID="tpBussinessSkills"
                    HeaderText="Bussiness Skills">
                    <ContentTemplate>
                        <div class="SkillsBody">
                            <div style="padding: 10px;">
                                <asp:Label runat="server" ID="lblCategory" Text="Category"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlBusinessCategory" onchange=" if(!CheckDirtyBehaviour(this)){return false;}" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                    DataTextField="Description" DataValueField="Id" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="SkillsDataBody">
                                <asp:GridView ID="gvBusinessSkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvSkills_RowDataBound"
                                    CssClass="WholeWidth TabPadding">
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
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Level
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel" onchange="ddlLevelChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id" onchange="ddlLevelChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Last Used
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLastUsed" DataSourceID="odsLastUsed" DataTextField="Name"
                                                    DataValueField="Id" onchange="ddlLevelChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear" OnClientClick="ClearAllFields(this); ddlLevelChanged(this); return false;">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel runat="server" ID="tpTechnicalSkills" onClick="checkDirtyBase();"
                    HeaderText="Technical Skills">
                    <ContentTemplate>
                        <div class="SkillsBody">
                            <div style="padding: 10px;">
                                <asp:Label runat="server" ID="Label1" Text="Category"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlTechnicalCategory" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                    DataTextField="Description" DataValueField="Id" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="SkillsDataBody">
                                <asp:GridView ID="gvTechnicalSkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvSkills_RowDataBound"
                                    CssClass="WholeWidth TabPadding">
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
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Level
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel" onchange="ddlLevelChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id" onchange="ddlLevelChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Last Used
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLastUsed" DataSourceID="odsLastUsed" DataTextField="Name"
                                                    DataValueField="Id" onchange="ddlLevelChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear" OnClientClick="ClearAllFields(this); ddlLevelChanged(this); return false;">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel runat="server" ID="tpIndustrySkills" onClick="checkDirtyBase();"
                    HeaderText="Industry Skills">
                    <ContentTemplate>
                        <div class="SkillsBody">
                            <div style="padding:10px;">&nbsp;
                            </div>
                            <div class="SkillsDataBody">
                                <asp:GridView ID="gvIndustrySkills" runat="server" AutoGenerateColumns="false" CssClass="WholeWidth TabPadding">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemStyle Width="40%" />
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField ID="hdnChanged" runat="server" Value="0" />
                                                <asp:Label ID="lblSkillsDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle Width="60%" />
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
            </AjaxControlToolkit:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div class="WholeWidth">
        <div style="width:90%; text-align:right;">
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
        </div>
    </div>
    <asp:ObjectDataSource ID="odsSkillLevel" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetSkillLevels"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsExperience" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetExperiences"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsLastUsed" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetLastUsedYears"></asp:ObjectDataSource>
</asp:Content>

