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
        .PopupHeight
        {
            max-height: 150px;
            overflow-y: auto;
        }
    </style>
    <script type="text/javascript" src="~/Scripts/jquery-1.4.1-vsdoc.js"></script>
    <script type="text/javascript" language="javascript">

        $(document).ready(
        function () {
            setTimeout("CheckAndShowPopUp();", 200);
        }
        );

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            CheckAndShowPopUp();
        }


        function CheckAndShowPopUp() {
            var hdnIsValid = document.getElementById('<%= hdnIsValid.ClientID %>');
            var hdnValidationMessage = document.getElementById('<%= hdnValidationMessage.ClientID %>');
            var popup2 = $find('mpeValidationsBehaviourId');
            if (hdnIsValid != null && hdnValidationMessage != null) {
                if (hdnIsValid.value == "false") {
                    popup2.show();
                }
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

        function setHintPosition(img, displayPnl) {
            var image = $("#" + img);
            var displayPanel = $("#" + displayPnl);
            iptop = image.offset().top;
            ipleft = image.offset().left;
            iptop = iptop + 10;
            ipleft = ipleft - 10;
            setPosition(displayPanel, iptop, ipleft);
            displayPanel.show();
            setPosition(displayPanel, iptop, ipleft);
            displayPanel.show();
        }

        function setPosition(item, ytop, xleft) {
            item.offset({ top: ytop, left: xleft });
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
                <AjaxControlToolkit:TabPanel runat="server" ID="tpBusinessSkills" HeaderText="Business">
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
                                    CssClass="WholeWidth TabPadding" AlternatingRowStyle-BackColor="#f9faff" HeaderStyle-BackColor="#f9faff">
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
                                                <asp:Image ID="imgLevelyHint" runat="server" ImageUrl="~/Images/hint.png" />
                                                <asp:Panel ID="pnlLevel" Style="display: none;" CssClass="MiniReport" runat="server">
                                                    <table style="width: 350px !important;">
                                                        <tr>
                                                            <th align="right" style="padding-top: 0px; padding-bottom: 0px;">
                                                                <asp:Button ID="btnCloseLevel" OnClientClick="return false;" runat="server" CssClass="mini-report-close"
                                                                    Text="x" />
                                                            </th>
                                                        </tr>
                                                        <tr>
                                                            <td style="background-color: White; font-weight: normal; font-size: small;">
                                                                <asp:DataList ID="dtlSkillLevels" runat="server" Width="100%" CssClass="PopupHeight">
                                                                    <ItemTemplate>
                                                                        <b>
                                                                            <%# ((DataTransferObjects.Skills.SkillLevel)Container.DataItem).Description%>:</b>
                                                                        <%# ((DataTransferObjects.Skills.SkillLevel)Container.DataItem).Definition%>
                                                                    </ItemTemplate>
                                                                </asp:DataList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <AjaxControlToolkit:AnimationExtender ID="animHide" TargetControlID="btnCloseLevel"
                                                    runat="server">
                                                </AjaxControlToolkit:AnimationExtender>
                                                <AjaxControlToolkit:AnimationExtender ID="animShow" TargetControlID="imgLevelyHint"
                                                    runat="server">
                                                </AjaxControlToolkit:AnimationExtender>
                                            </HeaderTemplate>
                                            <ItemStyle Width="15%" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                                <asp:CustomValidator ID="cvLevel" runat="server" Text="*" ToolTip="Level is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
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
                                                <asp:CustomValidator ID="cvExperience" runat="server" Text="*" ToolTip="Experience is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
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
                                                <asp:CustomValidator ID="cvLastUsed" runat="server" Text="*" ToolTip="Last Used is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
                                                <asp:CustomValidator ID="cvSkills" runat="server" OnServerValidate="cvSkills_ServerValidate"
                                                    SetFocusOnError="true" ValidationGroup="BusinessGroup"></asp:CustomValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemStyle Width="8%" />
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear" ToolTip="Clear Level, Experience, Last Used in this row."
                                                    OnClientClick="ClearAllFields(this); ddlChanged(this); return false;">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel runat="server" ID="tpTechnicalSkills" HeaderText="Technical">
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
                                    CssClass="WholeWidth TabPadding" AlternatingRowStyle-BackColor="#f9faff" HeaderStyle-BackColor="#f9faff">
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
                                                <asp:Image ID="imgLevelyHint" runat="server" ImageUrl="~/Images/hint.png" />
                                                <asp:Panel ID="pnlLevel" Style="display: none;" CssClass="MiniReport" runat="server">
                                                    <table style="width: 350px !important;">
                                                        <tr>
                                                            <th align="right" style="padding-top: 0px; padding-bottom: 0px;">
                                                                <asp:Button ID="btnCloseLevel" OnClientClick="return false;" runat="server" CssClass="mini-report-close"
                                                                    Text="x" />
                                                            </th>
                                                        </tr>
                                                        <tr>
                                                            <td style="background-color: White; font-weight: normal; font-size: small;">
                                                                <asp:DataList ID="dtlSkillLevels" runat="server" Width="100%">
                                                                    <ItemTemplate>
                                                                        <b>
                                                                            <%# ((DataTransferObjects.Skills.SkillLevel)Container.DataItem).Description%>:</b>
                                                                        <%# ((DataTransferObjects.Skills.SkillLevel)Container.DataItem).Definition%>
                                                                    </ItemTemplate>
                                                                </asp:DataList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <AjaxControlToolkit:AnimationExtender ID="animHide" TargetControlID="btnCloseLevel"
                                                    runat="server">
                                                </AjaxControlToolkit:AnimationExtender>
                                                <AjaxControlToolkit:AnimationExtender ID="animShow" TargetControlID="imgLevelyHint"
                                                    runat="server">
                                                </AjaxControlToolkit:AnimationExtender>
                                            </HeaderTemplate>
                                            <ItemStyle Width="15%" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                                <asp:CustomValidator ID="cvLevel" runat="server" Text="*" ToolTip="Level is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
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
                                                <asp:CustomValidator ID="cvExperience" runat="server" Text="*" ToolTip="Experience is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
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
                                                <asp:CustomValidator ID="cvLastUsed" runat="server" Text="*" ToolTip="Last Used is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
                                                <asp:CustomValidator ID="cvSkills" runat="server" OnServerValidate="cvSkills_ServerValidate"
                                                    SetFocusOnError="true" ValidationGroup="TechnicalGroup"></asp:CustomValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemStyle Width="8%" />
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear" ToolTip="Clear Level, Experience, Last Used in this row."
                                                    OnClientClick="ClearAllFields(this); ddlChanged(this); return false;">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
                <AjaxControlToolkit:TabPanel runat="server" ID="tpIndustrySkills" HeaderText="Industries">
                    <ContentTemplate>
                        <div class="SkillsBody">
                            <div style="padding: 10px;">
                                &nbsp;
                            </div>
                            <div class="SkillsDataBody">
                                <asp:GridView ID="gvIndustrySkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvIndustrySkills_RowDataBound"
                                    CssClass="WholeWidth TabPadding" AlternatingRowStyle-BackColor="#f9faff" HeaderStyle-BackColor="#f9faff">
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
            <asp:Label ID="lblMessage" runat="server" ForeColor="Green" Text=""></asp:Label>
            <div class="WholeWidth">
                <div style="width: 98%; text-align: right;">
                    <asp:Button ID="btnSave" runat="server" Text="Save" ToolTip="Save Changes" OnClick="btnSave_Click"
                        Enabled="false" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel Changes"
                        OnClick="btnCancel_Click" Enabled="false" />
                </div>
            </div>
            <asp:ValidationSummary ID="valSummaryBusiness" runat="server" ShowMessageBox="false"
                ValidationGroup="BusinessGroup" />
            <asp:ValidationSummary ID="valSummaryTechnical1" runat="server" ShowMessageBox="false"
                ValidationGroup="TechnicalGroup" />
            <asp:HiddenField ID="hdnIsValid" runat="server" Value="true" />
            <asp:HiddenField ID="hdnValidationMessage" runat="server" Value="" />
            <asp:Panel ID="pnlValidations" runat="server">
                <div style="border: 1px solid black;">
                    <div style="text-align: center; font-weight: bold; padding: 3px; background-color: Gray;">
                        Alert!
                    </div>
                    <div style="vertical-align: top; max-height: 500px; overflow-y: auto; padding: 10px;
                        background-color: White;">
                        <b>Please select a value for ‘Level’, ‘Experience’, ‘Last Used’, for below skills:
                        </b>
                        <br />
                        <br />
                        <div style="padding-left: 20px;">
                            <asp:Label ID="lblValidationMessage" runat="server"></asp:Label></div>
                    </div>
                    <div style="text-align: center; background-color: White; padding-bottom: 10px; padding-top: 5px;">
                        <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="return false;" />
                        <asp:Button ID="btnCancelValidations" runat="server" Style="display: none;" OnClientClick="return false;" /></div>
                </div>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeValidations" runat="server" TargetControlID="hdnIsValid"
                BackgroundCssClass="modalBackground" BehaviorID="mpeValidationsBehaviourId" DropShadow="false"
                PopupControlID="pnlValidations" OkControlID="btnOk" CancelControlID="btnCancelValidations">
            </AjaxControlToolkit:ModalPopupExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="odsSkillLevel" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetSkillLevels"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsExperience" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetExperiences"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsLastUsed" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetLastUsedYears"></asp:ObjectDataSource>
</asp:Content>

