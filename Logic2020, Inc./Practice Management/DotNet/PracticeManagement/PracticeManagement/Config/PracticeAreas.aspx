<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="PracticeAreas.aspx.cs" Inherits="PraticeManagement.Config.PracticeAreas" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Import Namespace="PraticeManagement.Controls.Configuration" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="pmc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register TagPrefix="uc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Areas | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Practice Areas
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery-1.4.1.yui.js" type="text/javascript"></script>
    <script type="text/javascript">

        function doConfirm(msg, yesFn, noFn) {
            var confirmBox = $("#<%=confirmBoxJavascript.ClientID %>");
            confirmBox.find(".message").text(msg);
            confirmBox.find(".yes,.no").unbind().click(function () {
                var confirmBoxTest_backgroundElement = $("#confirmBoxTest_backgroundElement");
                confirmBoxTest_backgroundElement.hide();
                confirmBox.hide();
            });
            confirmBox.find(".yes").click(yesFn);
            confirmBox.find(".no").click(noFn);
            confirmBox.show();
            var confirmBoxTest_backgroundElement = $("#confirmBoxTest_backgroundElement");
            confirmBoxTest_backgroundElement[0].style.width = $(window).width() + 'px';
            confirmBoxTest_backgroundElement[0].style.height = $(window).height() + 'px';
            confirmBoxTest_backgroundElement.show();
        }



        function showcapabilityActivePopup(chbIsActiveEdId, updateBtn, hdCapabilitiesInactivePopUpOperationId, confirmBoxJavascriptId) {
            var chbIsActiveEd = document.getElementById(chbIsActiveEdId);
            var hdCapabilitiesInactivePopUpOperation = document.getElementById("<%=hdCapabilitiesInactivePopUpOperation.ClientID %>");
            var confirmBox = $("#<%=confirmBoxJavascript.ClientID %>");
            confirmBox[0].attributes['PopupShow'].value
            if (!chbIsActiveEd.checked && confirmBox[0].attributes['PopupShow'].value == 'false' && hdCapabilitiesInactivePopUpOperation.value != "approved") {
                doConfirm("You have chosen to deactivate a Practice Area. Any corresponding capabilities will be set to Inactive. Are you sure you want to continue?", function yes() {
                    try {
                        confirmBox[0].style.display = false;
                    } catch (err) {
                        confirmBox[0].style.display = 'none';
                    }
                    confirmBox[0].attributes['PopupShow'].value = 'true';
                    hdCapabilitiesInactivePopUpOperation.value = "approved";
                    updateBtn.click();
                }, function no() {
                    hdCapabilitiesInactivePopUpOperation.value = "cancel";
                    try {
                        confirmBox[0].style.display = false;
                    } catch (err) {
                        confirmBox[0].style.display = 'none';
                    }
                    confirmBox[0].attributes['PopupShow'].value = 'true';
                    updateBtn.click();
                });

                //                if (!confirm('You have chosen to deactivate a Practice Area. Any corresponding capabilities will be set to Inactive. Are you sure you want to continue?')) {
                //                    hdCapabilitiesInactivePopUpOperation.value = "cancel";
                //                }
                return false;
            }
            return true;
        }

        $(document).ready(function () {
            $("#cblDivision").css("display", "none");
            $("#sdeCblDivision").css("display", "none");
        });

        $('.cblScrollingDropDown table input').live('click', function (e) {
            var src = e.srcElement || e.target;
            processRequest(this.id.substr(0, this.id.lastIndexOf('_')), src);
            $(this).parent().click();

        }); 

    </script>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="updTimeEntries" runat="server">
        <ContentTemplate>
            <div id="confirmBoxJavascript" style="display: none;" popupshow="false" class="ProjectDetailErrorPanel PanelPerson confirmBoxJavascript "
                runat="server">
                <table class="Width100Per">
                    <tbody>
                        <tr>
                            <th class="bgcolorGray TextAlignCenterImp vBottom">
                                <b class="BtnClose">Attention!</b>
                            </th>
                        </tr>
                        <tr>
                            <td class="Padding10Px">
                                <div class="message">
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="Padding10Px TextAlignCenterImp">
                                <input style="width: 100px;" id="yes" name="yes" value="Yes" type="button" class="yes">
                                <input style="width: 100px;" id="no" name="no" value="No" type="button" class="no">
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div style="left: 0px; top: 0px; position: fixed; z-index: 10000;" id="confirmBoxTest_backgroundElement"
                class="modalBackground">
            </div>
            <asp:HiddenField ID="hdCapabilitiesInactivePopUpOperation" runat="server" Value="false" />
            <asp:GridView ID="gvPractices" runat="server" AutoGenerateColumns="False" CssClass="CompPerfTable gvPractices"
                OnRowDataBound="gvPractices_RowDataBound">
                <AlternatingRowStyle CssClass="alterrow" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width5Percent" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/Images/icon-edit.png" OnClick="imgEdit_OnClick"
                                ToolTip="Edit Practice Area" />
                            <asp:HiddenField ID="hdPracticeId" runat="server" Value='<%#Bind("Id")%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:HiddenField ID="hdPracticeId" runat="server" Value='<%#Bind("id")%>' />
                            <asp:ImageButton ID="imgUpdate" runat="server" ImageUrl="~/Images/icon-check.png"
                                OnClick="imgUpdate_OnClick" ToolTip="Confirm" />
                            <asp:ImageButton ID="imgCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="imgCancel_OnClick"
                                ToolTip="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Practice Area Name" SortExpression="Name">
                        <HeaderStyle CssClass="Width25Percent padRight10" />
                        <ItemStyle CssClass="Left no-wrap padRight10" />
                        <ItemTemplate>
                            <asp:Label ID="lblPractice" runat="server" CssClass="WS-Normal" Text='<%# Bind("HtmlEncodedName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditPractice" runat="server" Text='<%# Bind("Name") %>' CssClass="Width95Percent"
                                ValidationGroup="EditPractice" />
                            <asp:RequiredFieldValidator ID="valPracticeName" runat="server" ValidationGroup="EditPractice"
                                EnableClientScript="false" Display="Dynamic" ToolTip="Practice Area name is required."
                                Text="*" ErrorMessage="Practice Area name is required." ControlToValidate="tbEditPractice" />
                            <asp:RegularExpressionValidator ID="regValPracticeName1" ControlToValidate="tbEditPractice"
                                Display="Dynamic" ToolTip="Practice Area name should not be more than 100 characters."
                                Text="*" runat="server" ValidationGroup="EditPractice" ValidationExpression="^[\s\S]{0,100}$"
                                ErrorMessage="Practice Area name should not be more than 100 characters." />
                            <asp:CustomValidator ID="custValEditPractice" runat="server" ValidationGroup="EditPractice"
                                Display="Dynamic" Text="*" ErrorMessage="Practice area name already exists. Please enter a different practice area name."
                                ToolTip="Practice area name already exists. Please enter a different practice area name." />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Abbreviation">
                        <HeaderStyle CssClass="Width5Percent" />
                        <ItemStyle CssClass="Left no-wrap padLeft20" />
                        <ItemTemplate>
                            <asp:Label ID="lblAbbreviation" runat="server" CssClass="WS-Normal" Text='<%# Bind("Abbreviation") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="tbEditAbbreviation" runat="server" Text='<%# Bind("Abbreviation") %>'
                                CssClass="Width95Percent" ValidationGroup="EditPractice" />
                            <asp:RegularExpressionValidator ID="regValAbbreviation" ControlToValidate="tbEditAbbreviation"
                                Display="Dynamic" Text="*" runat="server" ValidationGroup="EditPractice" ValidationExpression="^[\s\S]{0,100}$"
                                ToolTip="Abbreviation should not be more than 100 characters." ErrorMessage="Abbreviation should not be more than 100 characters." />
                            <asp:CustomValidator ID="custValEditPracticeAbbreviation" runat="server" ValidationGroup="EditPractice"
                                Display="Dynamic" Text="*" ErrorMessage="Abbreviation with this name already exists for a practice area. Please enter different abbreviation name."
                                ToolTip="Abbreviation with this name already exists for a practice area. Please enter different abbreviation name." />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active">
                        <HeaderStyle CssClass="Width5Percent" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbIsActive" runat="server" Enabled="false" Checked='<%# ((Practice)Container.DataItem).IsActive %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chbIsActiveEd" runat="server" Checked='<%# Bind("IsActive") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Internal">
                        <HeaderStyle CssClass="Width5Percent" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbIsCompanyInternal" runat="server" Enabled="false" Checked='<%# ((Practice) Container.DataItem).IsCompanyInternal %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="chbInternal" runat="server" Checked='<%# Bind("IsCompanyInternal") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="Width15Percent" />
                        <ItemStyle CssClass="Left" />
                        <HeaderTemplate>
                            Practice Area Owner (Status)
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblPracticeManager" runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlActivePersons" runat="server" CssClass="Width95Percent">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="Width20Percent" />
                        <ItemStyle CssClass="Left LeftPadding10px" />
                        <HeaderTemplate>
                           Person Division(s)
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblDivisions" runat="server" ></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <span class="Left">
                                <pmc:ScrollingDropDown ID="cblActiveDivisions" runat="server" SetDirty="false" CssClass="PracticeAreasCblDivision cblActiveDivisionstd cblScrollingDropDown"
                                    OnClick="scrollingDropdown_onclick('cblActiveDivisions','Division')" DropDownListType="Division" />
                                <ext:ScrollableDropdownExtender ID="sdeCblActiveDivisions" runat="server" TargetControlID="cblActiveDivisions"
                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                </ext:ScrollableDropdownExtender>
                            </span>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="Width20Percent" />
                        <ItemStyle CssClass="Left LeftPadding10px" />
                        <HeaderTemplate>
                            Project Division(s)
                        </HeaderTemplate>
                        <ItemTemplate >
                            <asp:Label ID="lblProjectDivisions" runat="server" ></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <span class="Left">
                                <pmc:ScrollingDropDown ID="cblProjectDivisions" runat="server" SetDirty="false" CssClass="PracticeAreasCblDivision cblActiveDivisionstd cblScrollingDropDown"
                                    OnClick="scrollingDropdown_onclick('cblProjectDivisions','Division')" DropDownListType="Division" />
                                <ext:ScrollableDropdownExtender ID="sdeCblProjectDivisions" runat="server" TargetControlID="cblProjectDivisions"
                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                </ext:ScrollableDropdownExtender>
                            </span>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                &nbsp;
                            </div>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="Width5Percent" />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/icon-delete.png"
                                OnClick="imgDelete_OnClick" ToolTip="Delete Practice Area" />
                        </ItemTemplate>
                        <EditItemTemplate>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Panel ID="pnlInsertPractice" runat="server" Wrap="False">
                <table class="CompPerfTable gvPractices" cellspacing="0">
                    <tr id="trInsertPractice" runat="server" class="alterrow">
                        <td class="Width5Percent PaddingTop10">
                            <asp:ImageButton ID="btnPlus" runat="server" ImageUrl="~/Images/add_16.png" OnClick="btnPlus_Click"
                                ToolTip="Add Practice Area" Visible="true" />
                            <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
                                ToolTip="Confirm" Visible="false" OnClick="btnInsert_Click" />
                            <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
                                ToolTip="Cancel" Visible="false" />
                        </td>
                        <td class="Width25Percent Left padRight10">
                            <asp:TextBox ID="tbPracticeName" ValidationGroup="InsertPractice" runat="server"
                                CssClass="Width95Percent" Visible="false" />
                            <asp:RequiredFieldValidator ID="valPracticeName" runat="server" ValidationGroup="InsertPractice"
                                ToolTip="Practice Area name is required." Text="*" ErrorMessage="Practice Area name is required."
                                SetFocusOnError="true" ControlToValidate="tbPracticeName" />
                            <asp:RegularExpressionValidator ID="regValPracticeName" ControlToValidate="tbPracticeName"
                                Display="Dynamic" Text="*" runat="server" ValidationGroup="InsertPractice" ValidationExpression="^[\s\S]{0,100}$"
                                ToolTip="Practice Area name should not be more than 100 characters." ErrorMessage="Practice Area name should not be more than 100 characters." />
                            <asp:CustomValidator ID="cvPracticeName" runat="server" ControlToValidate="tbPracticeName"
                                Display="Dynamic" Text="*" OnServerValidate="cvPracticeName_OnServerValidate"
                                ToolTip="Practice area name already exists. Please enter a different practice area name."
                                ValidationGroup="InsertPractice" ErrorMessage="Practice area name already exists. Please enter a different practice area name." />
                        </td>
                        <td class="Width5Percent Left padLeft20">
                            <asp:TextBox ID="tbAbbreviation" ValidationGroup="InsertPractice" runat="server"
                                CssClass="Width95Percent" Visible="false" />
                            <asp:RegularExpressionValidator ID="regValAbbreviation" ControlToValidate="tbAbbreviation"
                                Display="Dynamic" Text="*" runat="server" ValidationGroup="InsertPractice" ValidationExpression="^[\s\S]{0,100}$"
                                ToolTip="Abbreviation should not be more than 100 characters." ErrorMessage="Abbreviation should not be more than 100 characters." />
                            <asp:CustomValidator ID="custValEditPracticeAbbreviation" runat="server" ValidationGroup="InsertPractice"
                                Display="Dynamic" Text="*" ErrorMessage="Abbreviation with this name already exists for a practice area. Please enter different abbreviation name."
                                ToolTip="Abbreviation with this name already exists for a practice area. Please enter different abbreviation name." />
                        </td>
                        <td class="Width5Percent">
                            <asp:CheckBox ID="chbPracticeActive" runat="server" Checked="true" Visible="false" />
                        </td>
                        <td class="Width5Percent">
                            <asp:CheckBox ID="chbIsInternalPractice" runat="server" Checked="false" Visible="false" />
                        </td>
                        <td class="Width15Percent Left">
                            <asp:DropDownList ID="ddlPracticeManagers" runat="server" CssClass="Width95Percent"
                                Visible="false" />
                            <asp:CustomValidator ID="custPracticeManager" runat="server" ValidationGroup="InsertPractice"
                                Display="Dynamic" Text="*" ErrorMessage="Please add a person as Practice area owner."
                                OnServerValidate="custPracticeManager_ServerValidate" ToolTip="Please add a person as Practice area owner." />
                        </td>
                        <td class="Width20Percent Left">
                            <span class="Left">
                                <pmc:ScrollingDropDown ID="cblDivision" runat="server" SetDirty="false" CssClass="PracticeAreasCblDivision hidden"
                                    onclick="scrollingDropdown_onclick('cblDivision','Division')" DropDownListType="Division" />
                                <ext:ScrollableDropdownExtender ID="sdeCblDivision" runat="server" TargetControlID="cblDivision"
                                    Display="none" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png"
                                    Width="240px">
                                </ext:ScrollableDropdownExtender>
                            </span>
                        </td>
                        <td class="Width20Percent Left">
                            <span class="Left">
                                <pmc:ScrollingDropDown ID="cblProjectDivision" runat="server" SetDirty="false" CssClass="PracticeAreasCblDivision hidden"
                                    onclick="scrollingDropdown_onclick('cblProjectDivision','Division')" DropDownListType="Division" />
                                <ext:ScrollableDropdownExtender ID="sdecblProjectDivision" runat="server" TargetControlID="cblProjectDivision"
                                    Display="none" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png"
                                    Width="240px">
                                </ext:ScrollableDropdownExtender>
                            </span>
                        </td>
                        <td class="Width5Percent">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                CancelControlID="btnCancelErrorPanel" DropShadow="false" />
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
                            <asp:ValidationSummary ID="valSummaryInsert" ValidationGroup="InsertPractice" runat="server"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a practice." />
                            <asp:ValidationSummary ID="valSummaryEdit" ValidationGroup="EditPractice" runat="server"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a practice." />
                            <uc:Label ID="mlInsertStatus" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10Px TextAlignCenterImp">
                            <asp:Button ID="btnOKErrorPanel" runat="server" Text="OK" Width="100" OnClientClick="$find('mpeErrorPanelBehaviourId').hide();return false;" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--  <asp:ObjectDataSource ID="odsPractices" runat="server" SelectMethod="GetAllPractices"
                TypeName="PraticeManagement.Controls.Configuration.PracticesHelper" DataObjectTypeName="PraticeManagement.Controls.Configuration.Practice"
                DeleteMethod="RemovePracticeEx" ></asp:ObjectDataSource>--%>
            <%--<asp:Repeater ID="repTest" runat="server" OnItemDataBound="repTest_DataBound">
                <HeaderTemplate>
                    <table>
                        <tr>
                            <td>
                                Division
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <span class="Left">
                                <pmc:ScrollingDropDown ID="cblDivision" runat="server" SetDirty="false" CssClass="UTilTimeLineFilterCblPractices"
                                    onclick="scrollingDropdown_onclick('cblDivision','Division')" DropDownListType="Division"
                                    Style="display: none;" />
                                <ext:ScrollableDropdownExtender ID="sdeCblDivision" runat="server" TargetControlID="cblDivision"
                                    Display="none" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png"
                                    Width="240px">
                                </ext:ScrollableDropdownExtender>
                            </span>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

