<%@ Page Title="Skills Entry | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsEntry.aspx.cs" Inherits="PraticeManagement.SkillsEntry" %>

<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
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
            var clearLink = row.find("a[id$='lnkbtnClear']")[0];
            if (row.find("select[id$='ddlLevel']")[0].value != 0 || row.find("select[id$='ddlExperience']")[0].value != 0 || row.find("select[id$='ddlLastUsed']")[0].value != 0) {
                clearLink.disabled = false;
                clearLink.attributes['disable'].value = 'False';
                clearLink.style.color = "#0898E6";
                clearLink.style.cursor = "pointer";
            }
            else {
                clearLink.disabled = true;
                clearLink.attributes['disable'].value = 'True';
                clearLink.style.color = "#8F8F8F";
                clearLink.style.cursor = "text";
            }
        }
        function ddlChangedIndustry(changedObject) {
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
            if (clearLink.attributes['disable'].value == 'False') {
                row.find("select[id$='ddlLevel']")[0].value = 0;
                row.find("select[id$='ddlExperience']")[0].value = 0;
                row.find("select[id$='ddlLastUsed']")[0].value = 0;
                var ddlLevel = clearLink;
                clearLink.disabled = true;
                clearLink.attributes['disable'].value = 'True';
                clearLink.style.color = "#8F8F8F";
                clearLink.style.cursor = "text";
                ddlChanged(clearLink);
            }
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
    <div class="TextAlignCenterImp fontSizeLarge">
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
                        <div class="SkillsBodyEntry">
                            <div class="Padding10">
                                <asp:Label runat="server" ID="lblCategory" Text="Category"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlBusinessCategory" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                    DataTextField="Description" DataValueField="Id" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="SkillsEntryDataBody">
                                <asp:GridView ID="gvBusinessSkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvSkills_RowDataBound"
                                    CssClass="WholeWidth TabPadding">
                                    <AlternatingRowStyle CssClass="alterrow" />
                                    <HeaderStyle CssClass="alterrow" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                            </HeaderTemplate>
                                            <ItemStyle CssClass="Width55Percent" />
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField ID="hdnDescription" runat="server" Value='<%# Eval("Description") %>' />
                                                <asp:HiddenField ID="hdnChanged" runat="server" Value="0" />
                                                <asp:Label ID="lblSkillsDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="textLeft" />
                                            <HeaderTemplate>
                                                Level
                                                <asp:Image ID="imgLevelyHint" runat="server" ImageUrl="~/Images/hint.png" />
                                                <div class="DivLevel">
                                                    <asp:Panel ID="pnlLevel" Style="display: none;" CssClass="MiniReport" runat="server">
                                                        <table class="Width350pxImp">
                                                            <tr>
                                                                <th class="textRightImp PaddingBottomTop0PxImp">
                                                                    <asp:Button ID="btnCloseLevel" OnClientClick="return false;" runat="server" CssClass="mini-report-close"
                                                                        Text="x" />
                                                                </th>
                                                            </tr>
                                                            <tr>
                                                                <td class="TdSkillLevel">
                                                                    <asp:DataList ID="dtlSkillLevels" runat="server" CssClass="PopupHeight WholeWidthImp">
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
                                                </div>
                                                <AjaxControlToolkit:AnimationExtender ID="animHide" TargetControlID="btnCloseLevel"
                                                    runat="server">
                                                </AjaxControlToolkit:AnimationExtender>
                                                <AjaxControlToolkit:AnimationExtender ID="animShow" TargetControlID="imgLevelyHint"
                                                    runat="server">
                                                </AjaxControlToolkit:AnimationExtender>
                                            </HeaderTemplate>
                                            <ItemStyle CssClass="Width15Per" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                                <asp:CustomValidator ID="cvLevel" runat="server" Text="*" ToolTip="Level is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="textLeft" />
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemStyle CssClass="Width12Per" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                                <asp:CustomValidator ID="cvExperience" runat="server" Text="*" ToolTip="Experience is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="textLeft" />
                                            <HeaderTemplate>
                                                Last Used
                                            </HeaderTemplate>
                                            <ItemStyle CssClass="Width10Per" />
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
                                            <ItemStyle CssClass="Width8Per" />
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear" ToolTip="Clear Level, Experience, Last Used in this row."
                                                    OnClientClick="ClearAllFields(this);  return false;" CssClass="fontUnderline"
                                                    disable="">
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
                        <div class="SkillsBodyEntry">
                            <div class="Padding10">
                                <asp:Label runat="server" ID="Label1" Text="Category"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlTechnicalCategory" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                    DataTextField="Description" DataValueField="Id" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="SkillsEntryDataBody">
                                <asp:GridView ID="gvTechnicalSkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvSkills_RowDataBound"
                                    CssClass="WholeWidth TabPadding">
                                    <AlternatingRowStyle CssClass="alterrow" />
                                    <HeaderStyle CssClass="alterrow" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                            </HeaderTemplate>
                                            <ItemStyle CssClass="Width55Percent" />
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField ID="hdnDescription" runat="server" Value='<%# Eval("Description") %>' />
                                                <asp:HiddenField ID="hdnChanged" runat="server" Value="0" />
                                                <asp:Label ID="lblSkillsDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="textLeft" />
                                            <HeaderTemplate>
                                                Level
                                                <asp:Image ID="imgLevelyHint" runat="server" ImageUrl="~/Images/hint.png" />
                                                <div class="DivLevel">
                                                    <asp:Panel ID="pnlLevel" Style="display: none;" CssClass="MiniReport" runat="server">
                                                        <table class="Width350pxImp">
                                                            <tr>
                                                                <th class="textRightImp PaddingBottomTop0PxImp">
                                                                    <asp:Button ID="btnCloseLevel" OnClientClick="return false;" runat="server" CssClass="mini-report-close"
                                                                        Text="x" />
                                                                </th>
                                                            </tr>
                                                            <tr>
                                                                <td class="TdSkillLevel">
                                                                    <asp:DataList ID="dtlSkillLevels" runat="server" CssClass="WholeWidthImp">
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
                                                </div>
                                                <AjaxControlToolkit:AnimationExtender ID="animHide" TargetControlID="btnCloseLevel"
                                                    runat="server">
                                                </AjaxControlToolkit:AnimationExtender>
                                                <AjaxControlToolkit:AnimationExtender ID="animShow" TargetControlID="imgLevelyHint"
                                                    runat="server">
                                                </AjaxControlToolkit:AnimationExtender>
                                            </HeaderTemplate>
                                            <ItemStyle CssClass="Width15Per" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                    DataSourceID="odsSkillLevel" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                                <asp:CustomValidator ID="cvLevel" runat="server" Text="*" ToolTip="Level is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="textLeft" />
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemStyle CssClass="Width12Per" />
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id" onchange="ddlChanged(this);">
                                                </asp:DropDownList>
                                                <asp:CustomValidator ID="cvExperience" runat="server" Text="*" ToolTip="Experience is required."
                                                    SetFocusOnError="true"></asp:CustomValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="textLeft" />
                                            <HeaderTemplate>
                                                Last Used
                                            </HeaderTemplate>
                                            <ItemStyle CssClass="Width10Per" />
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
                                            <ItemStyle CssClass="Width8Per" />
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkbtnClear" Text="clear" ToolTip="Clear Level, Experience, Last Used in this row."
                                                    OnClientClick="ClearAllFields(this); return false;" Enabled="false" Font-Underline="true"
                                                    disable="">
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
                        <div class="SkillsBodyEntry">
                            <div class="Padding10">
                                &nbsp;
                            </div>
                            <div class="SkillsEntryDataBody">
                                <asp:GridView ID="gvIndustrySkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvIndustrySkills_RowDataBound"
                                    CssClass="WholeWidth TabPadding">
                                    <AlternatingRowStyle CssClass="alterrow" />
                                    <HeaderStyle CssClass="alterrow" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemStyle CssClass="Width55Percent" />
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField ID="hdnChanged" runat="server" Value="0" />
                                                <asp:Label ID="lblSkillsDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="textLeft" />
                                            <ItemStyle CssClass="Width45Percent" />
                                            <HeaderTemplate>
                                                Experience
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlExperience" DataSourceID="odsExperience"
                                                    DataTextField="Name" DataValueField="Id" onchange="ddlChangedIndustry(this);">
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
                <div class="Width98Percent AlignRight">
                    <asp:Button ID="btnSave" runat="server" Text="Save" ToolTip="Save Changes" OnClick="btnSave_Click"
                        EnableViewState="false" Enabled="false" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel" EnableViewState="false"
                        OnClick="btnCancel_Click" Enabled="false" />
                </div>
            </div>
            <asp:ValidationSummary ID="valSummaryBusiness" runat="server" ShowMessageBox="false"
                ValidationGroup="BusinessGroup" />
            <asp:ValidationSummary ID="valSummaryTechnical1" runat="server" ShowMessageBox="false"
                ValidationGroup="TechnicalGroup" />
            <asp:HiddenField ID="hdnIsValid" runat="server" Value="true" />
            <asp:HiddenField ID="hdnValidationMessage" runat="server" Value="" />
            <asp:Panel ID="pnlValidations" Style="display: none;" runat="server">
                <div class="border1Px">
                    <div class="DivAlert">
                        Alert!
                    </div>
                    <div class="DivAlertText">
                        <b>Please select a value for ‘Level’, ‘Experience’, and ‘Last Used’ for the below skill(s):
                        </b>
                        <br />
                        <br />
                        <div class="padLeft20">
                            <asp:Label ID="lblValidationMessage" runat="server"></asp:Label></div>
                    </div>
                    <div class="PnlValidations">
                        <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="return false;" />
                        </div>
                </div>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeValidations" runat="server" TargetControlID="hdnIsValid"
                BackgroundCssClass="modalBackground" BehaviorID="mpeValidationsBehaviourId" DropShadow="false"
                PopupControlID="pnlValidations" OkControlID="btnOk">
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

