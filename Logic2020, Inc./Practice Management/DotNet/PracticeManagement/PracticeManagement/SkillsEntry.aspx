<%@ Page Title="Skills Entry | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsEntry.aspx.cs" Inherits="PraticeManagement.SkillsEntry" %>

<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="pcg" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
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

        function enterPressed(evn) {
            if (window.event && window.event.keyCode == 13) {
                return false;
            } else if (evn && evn.keyCode == 13) {
                return false;
            }
        }

        function CheckAndShowPopUp() {
            var hdnIsValid = document.getElementById('<%= hdnIsValid.ClientID %>');
            var hdnValidationMessage = document.getElementById('<%= hdnValidationMessage.ClientID %>');
            var popup2 = $find('mpeValidationsBehaviourId');
            if (hdnIsValid != null && hdnValidationMessage != null && hdnIsValid.value == "false") {
                popup2.show();
            }
            else {

                var hdnErrorPanel = document.getElementById('<%= hdnTargetErrorPanel.ClientID %>');
                var errorPanelPop = $find('mpeErrorPanelBehaviourId');
                if (hdnErrorPanel != null && errorPanelPop != null && hdnErrorPanel.value == "false") {
                    errorPanelPop.show();
                    hdnErrorPanel.value = "";
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
                clearLink.className = "fontUnderline linkEnableStyle";
            }
            else {
                clearLink.disabled = true;
                clearLink.attributes['disable'].value = 'True';
                clearLink.className = "fontUnderline linkDisableStyle";
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
                clearLink.disabled = true;
                clearLink.attributes['disable'].value = 'True';
                clearLink.className = "fontUnderline linkDisableStyle";
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

        function btnCancelPictureLink_OnClientClick() {
            var element = $('.ClearPersonPictureFileUpload');
            element.html(element.html());

            var addButton = document.getElementById('<%= btnUpdatePictureLink.ClientID %>');
            addButton.disabled = "disabled";
        }
        function pageLoad() {
            document.onkeypress = enterPressed;
        }
        function cvPersonPictureType_ClientValidationFunction(obj, args) {
            args.IsValid = IsValidProfilePicture();
        }

        function IsValidProfilePicture() {

            var fuPersonPicture = document.getElementById('<%= fuPersonPicture.ClientID %>');
            var FileUploadPath = fuPersonPicture.value;
            var Extension = FileUploadPath.substring(FileUploadPath.lastIndexOf('.') + 1).toLowerCase();
            if (Extension == "png" || Extension == "gif" || Extension == "jpg" || Extension == "jpeg" || Extension == "bmp") {
                return true; // Valid file type
            }
            else {
                return false; // Not valid file type
            }
        }

        function EnableAddButton() {
            var addButton = document.getElementById('<%= btnUpdatePictureLink.ClientID %>');
            
            if (HaveAttachement() && IsValidProfilePicture()) {
                addButton.disabled = "";
            }
            else {
                addButton.disabled = "disabled";
            }
        }

        function HaveAttachement() {
            var fuControl = document.getElementById('<%= fuPersonPicture.ClientID %>');
            var fileUploadPath = fuControl.value;
            if (fileUploadPath != null && fileUploadPath != undefined) {
                return true;
            }
            else {
                return false;
            }
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
                                    DataTextField="Description" DataValueField="Id" AutoPostBack="True">
                                </asp:DropDownList>
                            </div>
                            <div class="SkillsEntryDataBody">
                                <asp:GridView ID="gvBusinessSkills" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvSkills_RowDataBound"
                                    CssClass="WholeWidth TabPadding" EnableModelValidation="True">
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
                                                    OnClientClick="ClearAllFields(this);  return false;" CssClass="fontUnderline linkEnableStyle"
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
                                                    OnClientClick="ClearAllFields(this); return false;" Enabled="false" disable=""
                                                    CssClass="fontUnderline linkDisableStyle">
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
            <div class="WholeWidth">
                <div class="Width50Percent displayInline">
                    <asp:Button ID="btnPictureLink" runat="server" Text="Add/Update Consultant Picture"
                        CssClass="Width220Px" />
                    <asp:Button ID="btnProfileLink" runat="server" Text="Add/Update Consultant Profiles"
                        CssClass="Width220Px" />
                </div>
                <div class="Width50Percent textRight floatright">
                    <asp:Button ID="btnSave" runat="server" Text="Save" ToolTip="Save Changes" OnClick="btnSave_Click"
                        CssClass="width55Px" EnableViewState="false" Enabled="false" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel" EnableViewState="false"
                        CssClass="width55Px" OnClick="btnCancel_Click" Enabled="false" />
                </div>
            </div>
            <asp:ValidationSummary ID="valSummaryBusiness" runat="server" ShowMessageBox="false"
                ValidationGroup="BusinessGroup" />
            <asp:ValidationSummary ID="valSummaryTechnical" runat="server" ShowMessageBox="false"
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
            <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                CancelControlID="btnCancelErrorPanel" DropShadow="false" OkControlID="btnOKErrorPanel" />
            <asp:Panel ID="pnlErrorPanel" runat="server" Style="display: none;" CssClass="ProjectDetailErrorPanel PanelPerson">
                <table class="Width100Per">
                    <tr>
                        <th class="bgcolorGray TextAlignCenterImp vBottom">
                            <b class="BtnClose">Attention!</b>
                            <asp:Button ID="btnCancelErrorPanel" runat="server" CssClass="mini-report-close floatright"
                                ToolTip="Cancel" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10Px">
                            <asp:Label ID="lblMessage" runat="server" ForeColor="Green" Text="" CssClass="padLeft20"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10Px TextAlignCenterImp">
                            <asp:Button ID="btnOKErrorPanel" runat="server" Text="OK" Width="100" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpePictureLinkPopup" runat="server" TargetControlID="btnPictureLink"
                BehaviorID="mpePictureLinkPopup" BackgroundCssClass="modalBackground" PopupControlID="pnlPictureLinkPopup"
                DropShadow="false" />
            <asp:Panel ID="pnlPictureLinkPopup" runat="server" CssClass="popUp" Style="display: none">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th>
                            Add/Update
                            <asp:Literal ID="ltrlPicturePopupPersonname" runat="server"></asp:Literal>'s Profile
                            Picture
                            <asp:Button ID="btnClose" runat="server" CssClass="mini-report-closeNew" ToolTip="Cancel"
                                OnClientClick="btnCancelPictureLink_OnClientClick();" OnClick="btnCancelPicture_Click" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="PicturePanelTd ClearPersonPictureFileUpload">
                            <asp:FileUpload ID="fuPersonPicture" CssClass="FileUpload" runat="server" Size="68"
                                onchange="EnableAddButton();" />
                            <asp:CustomValidator ID="cvPersonPictureType" runat="server" ControlToValidate="fuPersonPicture"
                                EnableClientScript="true" ClientValidationFunction="cvPersonPictureType_ClientValidationFunction"
                                SetFocusOnError="true" Display="Dynamic" Text="*" ToolTip="Picture File Format must be PNG/GIF/JPG/JPEG/BMP."
                                ErrorMessage="Picture File Format must be PNG/GIF/JPG/JPEG/BMP."></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="Padding6">
                            <table>
                                <tr>
                                    <td class="padRight3">
                                        <asp:Button ID="btnUpdatePictureLink" OnClick="btnUpdatePictureLink_OnClick" runat="server"
                                            CssClass="Width100Px" TabIndex="0" Text="" ToolTip="Add/Update" Enabled="false" />
                                    </td>
                                    <td class="padLeft3">
                                        <asp:Button ID="btnPictureDelete" runat="server" Text="Delete" ToolTip="Delete Profile Picture"
                                            Enabled="false" CssClass="Width100Px" OnClick="btnPictureDelete_OnClick" />
                                    </td>
                                    <td class="padLeft3">
                                        <asp:Button ID="btnCancelPicture" runat="server" Text="Cancel" ToolTip="Cancel" CssClass="Width100Px"
                                            OnClientClick="btnCancelPictureLink_OnClientClick();" OnClick="btnCancelPicture_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding6">
                            <b>Note</b>: Recommended to select 120 (height) x 100 (wide) dimensional picture
                            for the best display.
                            <br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; File type should be JPG,
                            JPEG, Png, Bmp and Gif only.
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeProfilePopUp" runat="server" TargetControlID="btnProfileLink"
                BehaviorID="mpeProfilePopUp" BackgroundCssClass="modalBackground" PopupControlID="pnlProfilePopup"
                DropShadow="false" />
            <asp:Panel ID="pnlProfilePopup" runat="server" CssClass="popUp ProfilesPanel" Style="display: none">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th>
                            Add/Update
                            <asp:Literal ID="ltrlPersonname" runat="server"></asp:Literal>'s Profile Links
                            <asp:Button ID="btnProfilePopupClose" runat="server" CssClass="mini-report-closeNew"
                                ToolTip="Cancel" OnClick="btnCancelProfile_OnClick" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="ProfilesPanelTd">
                            <asp:Repeater ID="repProfiles" runat="server" OnItemDataBound="repProfiles_OnItemDataBound"
                                EnableViewState="true">
                                <HeaderTemplate>
                                    <table class="repProfilesTable" id="repProfilesTable">
                                        <thead>
                                            <tr>
                                                <th>
                                                    Profile Name
                                                </th>
                                                <th>
                                                    Profile Link
                                                </th>
                                                <th>
                                                    Is Default
                                                    <asp:CustomValidator ID="cvIsDefault" runat="server" ValidationGroup="ProfileValidationGroup"
                                                        Text="*" ToolTip="One of the profile should be default profile." OnServerValidate="cvIsDefault_OnServerValidate"></asp:CustomValidator>
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:HiddenField ID="hdProfileId" runat="server" Value='<%# ((int?)Eval("ProfileId")).HasValue ? ((int?)Eval("ProfileId")).Value : 0 %>' />
                                            <asp:TextBox ID="txtProfileName" runat="server" Text='<%# Eval("ProfileName") %>'
                                                CssClass="Width175Px">
                                            </asp:TextBox>
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="tbWProfileName" runat="server" TargetControlID="txtProfileName"
                                                WatermarkCssClass="watermarkedtext Width175Px" WatermarkText="Enter your Profile Name here..">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:CustomValidator ID="cvProfileName" runat="server" ValidationGroup="ProfileValidationGroup"
                                                Text="*" ToolTip="Profile name is required." OnServerValidate="cvProfileName_OnServerValidate"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvProfileNameDuplicate" runat="server" ValidationGroup="ProfileValidationGroup"
                                                Text="*" ToolTip="Profile name should be unique." OnServerValidate="cvProfileNameDuplicate_OnServerValidate"></asp:CustomValidator>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtProfileLink" runat="server" Text='<%# Eval("ProfileUrl") %>'
                                                CssClass="Width175Px">
                                            </asp:TextBox>
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="tbWProfileLink" runat="server" TargetControlID="txtProfileLink"
                                                WatermarkCssClass="watermarkedtext Width175Px" WatermarkText="Enter your Profile Link here..">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:CustomValidator ID="cvProfileLink" runat="server" ValidationGroup="ProfileValidationGroup"
                                                Text="*" ToolTip="Profile Link is required." OnServerValidate="cvProfileLink_OnServerValidate"></asp:CustomValidator>
                                        </td>
                                        <td class="textCenter">
                                            <pcg:RepeatableRadioButton ID="rbprofileIsDefault" Checked='<%# (bool)Eval("IsDefault") %>'
                                                runat="server" GroupName="rbprofileIsDefault" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:HiddenField ID="hdProfileId" runat="server" Value='<%# ((int?)Eval("ProfileId")).HasValue ? ((int?)Eval("ProfileId")).Value : 0 %>' />
                                            <asp:TextBox ID="txtProfileName" runat="server" Text='<%# Eval("ProfileName") %>'
                                                CssClass="Width175Px">
                                            </asp:TextBox>
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="tbWProfileName" runat="server" TargetControlID="txtProfileName"
                                                WatermarkCssClass="watermarkedtext Width175Px" WatermarkText="Enter your Profile Name here..">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:CustomValidator ID="cvProfileName" runat="server" ValidationGroup="ProfileValidationGroup"
                                                Text="*" ToolTip="Profile name is required." OnServerValidate="cvProfileName_OnServerValidate"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvProfileNameDuplicate" runat="server" ValidationGroup="ProfileValidationGroup"
                                                Text="*" ToolTip="Profile name should be unique." OnServerValidate="cvProfileNameDuplicate_OnServerValidate"></asp:CustomValidator>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtProfileLink" runat="server" Text='<%# Eval("ProfileUrl") %>'
                                                CssClass="Width175Px">
                                            </asp:TextBox>
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="tbWProfileLink" runat="server" TargetControlID="txtProfileLink"
                                                WatermarkCssClass="watermarkedtext Width175Px" WatermarkText="Enter your Profile Link here..">
                                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                                            <asp:CustomValidator ID="cvProfileLink" runat="server" ValidationGroup="ProfileValidationGroup"
                                                Text="*" ToolTip="Profile Link is required." OnServerValidate="cvProfileLink_OnServerValidate"></asp:CustomValidator>
                                        </td>
                                        <td class="textCenter">
                                            <pcg:RepeatableRadioButton ID="rbprofileIsDefault" Checked='<%# (bool)Eval("IsDefault") %>'
                                                runat="server" GroupName="rbprofileIsDefault" />
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                                <FooterTemplate>
                                    </tbody> </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr>
                        <td class="padLeft8">
                            <asp:ImageButton ID="ibtnAddProfile" runat="server" OnClick="ibtnAddProfile_Click"
                                ImageUrl="~/Images/add_16.png" ToolTip="Add New Profile" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="TdRedirectToOppDetail">
                            <table>
                                <tr>
                                    <td class="padRight3">
                                        <asp:Button ID="btnProfilePopupUpdate" runat="server" Text="Add/Update" ToolTip="Add/Update"
                                            CssClass="Width100Px" OnClick="btnProfilePopupUpdate_OnClick" />
                                    </td>
                                    <td class="padLeft3">
                                        <asp:Button ID="btnCancelProfile" runat="server" Text="Cancel" ToolTip="Cancel" OnClick="btnCancelProfile_OnClick"
                                            CssClass="Width100Px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpdatePictureLink" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="odsSkillLevel" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetSkillLevels"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsExperience" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetExperiences"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsLastUsed" runat="server" TypeName="PraticeManagement.SkillsEntry"
        SelectMethod="GetLastUsedYears"></asp:ObjectDataSource>
</asp:Content>

