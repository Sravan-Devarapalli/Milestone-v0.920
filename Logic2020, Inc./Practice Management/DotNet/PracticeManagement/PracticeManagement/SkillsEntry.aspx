<%@ Page Title="Skills Entry | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsEntry.aspx.cs" Inherits="PraticeManagement.SkillsEntry" %>

<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <style>
        .TabPadding td, .TabPadding th
        {
            padding: 4px;
        }
        
        .ajax__tab_body
        {
            border-bottom: #999999 1px solid !important;
            margin-top: 1px;
            padding: 2px !important;
        }
        
        .SkillsBody
        {
            background-color: #d4dff8;
            padding: 10px;
        }
        
        .SkillsDataBody
        {
            background-color: White;
            height: 400px;
            overflow: auto;
        }
    </style>
    <script type="text/javascript" src="~/Scripts/jquery-1.4.1-vsdoc.js"></script>
    <script type="text/javascript" language="javascript">

        $(document).ready(
        function (){
        setTimeout("CheckAndShowPopUp();",200);
        }
        );

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            CheckAndShowPopUp();
        }


        function CheckAndShowPopUp(){
            var hdnIsValid = document.getElementById('<%= hdnIsValid.ClientID %>');
            var hdnValidationMessage = document.getElementById('<%= hdnValidationMessage.ClientID %>');
            if (hdnIsValid != null && hdnValidationMessage != null) {
                if (hdnIsValid.value == "false")
                    alert(hdnValidationMessage.value);
            }
        }

        function ddlChanged(changedObject) {
            var row = $(changedObject.parentNode.parentNode);
            row.find("input[id$='hdnChanged']")[0].value = 1;
            setDirty();

            disableButtons(false);
        }

        function disableButtons(disable) {
            var saveButton = document.getElementById('<%= btnSave.ClientID %>');
            var cancelButton = document.getElementById('<%= btnCancel.ClientID %>');
            saveButton.disabled = disable;
            cancelButton.disabled = disable;
        }

        function ClearAllFields(clearLink) {
            var row = $(clearLink.parentNode.parentNode);
            row.find("select[id$='ddlLevel']")[0].value = 0;
            row.find("select[id$='ddlExperience']")[0].value = 0;
            row.find("select[id$='ddlLastUsed']")[0].value = 0;
            var ddlLevel = clearLink;
        }

    </script>
    <div style="text-align: center; font-size: large;">
        Skills Entry for
        <asp:Label runat="server" ID="lblUserName"></asp:Label>
    </div>
    <uc:LoadingProgress ID="loadingProgress" runat="server" />
    <asp:UpdatePanel ID="upSkills" runat="server">
        <ContentTemplate>
            <AjaxControlToolkit:TabContainer runat="server" ID="tcSkillsEntry" ActiveTabIndex="0"
                AutoPostBack="true" OnActiveTabChanged="tcSkillsEntry_ActiveTabChanged">
                <AjaxControlToolkit:TabPanel runat="server" ID="tpBusinessSkills" HeaderText="Business Skills">
                    <ContentTemplate>
                        <div class="SkillsBody">
                            <div style="padding: 10px;">
                                <asp:Label runat="server" ID="lblCategory" Text="Category"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlBusinessCategory" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                    DataTextField="Description" DataValueField="Id" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="SkillsDataBody">
                                <asp:GridView ID="gvBusinessSkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvSkills_RowDataBound"
                                    CssClass="WholeWidth TabPadding" AlternatingRowStyle-BackColor="#f9faff">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                            </HeaderTemplate>
                                            <ItemStyle Width="55%" />
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField ID="hdnDescription" runat="server" Value='<%# Eval("Description") %>' />
                                                <asp:HiddenField ID="hdnChanged" runat="server" Value="0" />
                                                <asp:Label ID="lblSkillsDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Level
                                            </HeaderTemplate>
                                            <ItemStyle Width="15%" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemStyle Width="12%" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Last Used
                                            </HeaderTemplate>
                                            <ItemStyle Width="10%" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLastUsed" DataSourceID="odsLastUsed" DataTextField="Name"
                                                    DataValueField="Id" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                                <asp:CustomValidator ID="cvSkills" runat="server" OnServerValidate="cvSkills_ServerValidate"
                                                    Text="*" ToolTip="Skill must have Level, Experience and Last Used." SetFocusOnError="true"
                                                    ValidationGroup="BusinessGroup"></asp:CustomValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemStyle Width="8%" />
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear" OnClientClick="ClearAllFields(this); ddlChanged(this); return false;">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel runat="server" ID="tpTechnicalSkills" HeaderText="Technical Skills">
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
                                    CssClass="WholeWidth TabPadding" AlternatingRowStyle-BackColor="#f9faff">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                            </HeaderTemplate>
                                            <ItemStyle Width="55%" />
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField ID="hdnDescription" runat="server" Value='<%# Eval("Description") %>' />
                                                <asp:HiddenField ID="hdnChanged" runat="server" Value="0" />
                                                <asp:Label ID="lblSkillsDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Level
                                            </HeaderTemplate>
                                            <ItemStyle Width="15%" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemStyle Width="12%" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <HeaderTemplate>
                                                Last Used
                                            </HeaderTemplate>
                                            <ItemStyle Width="10%" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLastUsed" DataSourceID="odsLastUsed" DataTextField="Name"
                                                    DataValueField="Id" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                                <asp:CustomValidator ID="cvSkills" runat="server" OnServerValidate="cvSkills_ServerValidate"
                                                    Text="*" ToolTip="Skill must have Level, Experience and Last Used." SetFocusOnError="true"
                                                    ValidationGroup="TechnicalGroup"></asp:CustomValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemStyle Width="8%" />
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear" OnClientClick="ClearAllFields(this); ddlChanged(this); return false;">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel runat="server" ID="tpIndustrySkills" HeaderText="Industry Skills">
                    <ContentTemplate>
                        <div class="SkillsBody">
                            <div style="padding: 10px;">
                                &nbsp;
                            </div>
                            <div class="SkillsDataBody">
                                <asp:GridView ID="gvIndustrySkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvIndustrySkills_RowDataBound"
                                    CssClass="WholeWidth TabPadding" AlternatingRowStyle-BackColor="#f9faff">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemStyle Width="55%" />
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField ID="hdnChanged" runat="server" Value="0" />
                                                <asp:Label ID="lblSkillsDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle Width="45%" />
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id" onchange="ddlChanged(this);">
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
            <br />
            <div class="WholeWidth">
                <div style="width: 98%; text-align: right;">
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" Enabled="false" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                        Enabled="false" />
                </div>
            </div>
            <asp:ValidationSummary ID="valSummaryBusiness" runat="server" ValidationGroup="BusinessGroup" />
            <asp:ValidationSummary ID="valSummaryTechnical" runat="server" ValidationGroup="TechnicalGroup" />
            <asp:HiddenField ID="hdnIsValid" runat="server" Value="true" />
            <asp:HiddenField ID="hdnValidationMessage" runat="server" Value="" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="odsSkillLevel" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetSkillLevels"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsExperience" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetExperiences"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsLastUsed" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetLastUsedYears"></asp:ObjectDataSource>
</asp:Content>

