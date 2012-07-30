<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" Title="Dashboard | Practice Management"
    AutoEventWireup="true" CodeBehind="DashBoard.aspx.cs" Inherits="PraticeManagement.DashBoard" %>

<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Dashboard | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Dashboard
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">

        function ChangeDefaultFocus(e) {
            if (window.event) {
                e = window.event;
            }
            if (e.keyCode == 13) {
                var btn = document.getElementById('<%= btnSearchAll.ClientID %>');
                btn.click();
            }

        }

        function mpeQuicklink_OnCancelScript() {
            var hdnSelectedQuckLinks = document.getElementById('<%= hdnSelectedQuckLinks.ClientID %>');
            var txtSearchBox = document.getElementById('<%= txtSearchBox.ClientID %>');
            txtSearchBox.value = "";
            var quckLinksIndexes = hdnSelectedQuckLinks.value.split(",");

            if (document.getElementById('<%=cblQuickLinks.ClientID %>') != null) {
                var trQuickLinks = document.getElementById('<%=cblQuickLinks.ClientID %>').getElementsByTagName('tr');

                for (var i = 0; i < trQuickLinks.length; i++) {
                    var checkBox = trQuickLinks[i].children[0].getElementsByTagName('input')[0];
                    trQuickLinks[i].style.display = "";
                    checkBox.checked = false;
                    for (var j = 0; j < quckLinksIndexes.length; j++) {
                        if (i == quckLinksIndexes[j] && quckLinksIndexes[j] != '') {
                            checkBox.checked = true;
                            break;
                        }
                    }

                }
                changeAlternateitemsForCBL('<%=cblQuickLinks.ClientID %>');
            }
        }

        function HidePanel() {
            var pnlEditAnnounceMent = document.getElementById('<%= pnlEditAnnounceMent.ClientID %>');
            pnlEditAnnounceMent.style.display = "none";
        }

        function pageLoad() {
            var ddlSearchType = document.getElementById('<%= ddlSearchType.ClientID %>');
            ddlSearchType_onchange(ddlSearchType);
            changeAlternateitemsForCBL('<%=cblQuickLinks.ClientID %>');
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            var ddlSearchType = document.getElementById('<%= ddlSearchType.ClientID %>');
            ddlSearchType_onchange(ddlSearchType);
            changeAlternateitemsForCBL('<%=cblQuickLinks.ClientID %>');
        }

        function ddlSearchType_onchange(ddlSearchType) {
            if (ddlSearchType != null && ddlSearchType != "undefined") {
                var btnSearchAll = document.getElementById('<%= btnSearchAll.ClientID %>');

                if (ddlSearchType.value == "Opportunity") {

                    btnSearchAll.setAttribute('onclick', 'javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("ctl00$body$btnSearchAll", "", true, "Search", "OpportunitySearch.aspx", false, false))');
                }
                else if (ddlSearchType.value == "Person") {

                    btnSearchAll.setAttribute('onclick', 'javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("ctl00$body$btnSearchAll", "", true, "Search", "Config/Persons.aspx", false, false))');
                }
                else {
                    btnSearchAll.setAttribute('onclick', 'javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("ctl00$body$btnSearchAll", "", true, "Search", "ProjectSearch.aspx", false, false))');
                }

            }

        }

        function ClearQuickLinks() {
            var chkboxes = $('#<%=cblQuickLinks.ClientID %> tr td :input');
            for (var i = 0; i < chkboxes.length; i++) {
                chkboxes[i].checked = false;
            }
        }

        function filterQuickLinks(searchtextBox) {
            if (document.getElementById('<%=cblQuickLinks.ClientID %>') != null) {
                var trQuickLinks = document.getElementById('<%=cblQuickLinks.ClientID %>').getElementsByTagName('tr');
                var searchText = searchtextBox.value.toLowerCase();
                for (var i = 0; i < trQuickLinks.length; i++) {
                    var checkBox = trQuickLinks[i].children[0].getElementsByTagName('input')[0];
                    var checkboxText = checkBox.parentNode.children[1].innerHTML.toLowerCase();

                    if (checkboxText.length >= searchText.length && checkboxText.substr(0, searchText.length) == searchText) {

                        trQuickLinks[i].style.display = "";
                    }
                    else {

                        trQuickLinks[i].style.display = "none";
                    }
                }
                changeAlternateitemsForCBL('<%=cblQuickLinks.ClientID %>');
            }
        }

        function changeAlternateitemsForCBL(ControlClientID) {
            if (document.getElementById(ControlClientID) != null) {
                var trCBL = document.getElementById(ControlClientID).getElementsByTagName('tr');
                var index = 0;
                for (var i = 0; i < trCBL.length; i++) {
                    if (trCBL[i].style.display != 'none') {
                        index++;
                        if ((index) % 2 == 0) {
                            trCBL[i].style.backgroundColor = '#f9faff';
                        }
                        else {
                            trCBL[i].style.backgroundColor = '';
                        }
                    }
                }
            }
        }


            
    </script>
    <asp:UpdatePanel ID="upnlDashBoard" runat="server">
        <ContentTemplate>
            <table class="CompPerfTable DashboardPageHeaderTable">
                <tr>
                    <td class="FirstTd">
                        <h1>
                            Practice Management Announcements</h1>
                        <table class="WholeWidth">
                            <tr>
                                <td align="right" class="SecondTd">
                                    <asp:Button ID="btnEditAnnouncement" runat="server" OnClick="btnEditAnnouncement_OnClick"
                                        Text="Edit Announcement" ToolTip="Edit Announcement" />
                                </td>
                            </tr>
                        </table>
                        <table class="WholeWidth">
                            <tr>
                                <td class="ThirdTd">
                                </td>
                                <td class="FourthTd">
                                    <asp:Panel ID="pnlHtmlAnnounceMent" CssClass="ApplyStyleForDashBoardLists FirstPanel"
                                        runat="server">
                                        <asp:Label ID="lblAnnounceMent" runat="server"></asp:Label>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlEditAnnounceMent" runat="server" CssClass="SecondPanel" Style="display: none;">
                                        <CKEditor:CKEditorControl ID="ckeAnnouncementEditor" ResizeEnabled="false" Toolbar="Bold|Italic|Underline|Strike|-|NumberedList|BulletedList|Outdent|Indent|Format|Font|FontSize|TextColor|BGColor|Link|Unlink|-|Smiley|Cut|Copy|Paste|Undo|Redo|SpellChecker|"
                                            runat="server"></CKEditor:CKEditorControl>
                                        <table class="FirstPanel">
                                            <tr>
                                                <td align="right" class="FivthTd">
                                                    <asp:Button ID="btnSaveAnnouncement" runat="server" OnClientClick="HidePanel();"
                                                        OnClick="btnSaveAnnouncement_OnClick" Text="Save Announcement" ToolTip="Save Announcement" />
                                                </td>
                                                <td align="left" class="SixthTd">
                                                    <asp:Button ID="btnCancelAnnouncement" runat="server" OnClientClick="HidePanel();"
                                                        OnClick="btnCancelAnnouncement_OnClick" Text="Cancel" ToolTip="Cancel" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="width30P vTop">
                        <table class="WholeWidth">
                            <tr>
                                <td class="WholeWidth">
                                    <asp:Panel ID="pnlDashBoard" class="ThirdPanel" runat="server">
                                        <table class="WholeWidth">
                                            <tr>
                                                <td align="center">
                                                    Go to
                                                    <asp:DropDownList ID="ddlDashBoardType" AutoPostBack="true" OnSelectedIndexChanged="ddlDashBoardType_OnSelectedIndexChanged"
                                                        runat="server">
                                                    </asp:DropDownList>
                                                    DashBoard
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlSearchSection" class="Padding5" runat="server">
                                        <h1 class="WholeWidth textCenter">
                                            Search</h1>
                                        <table class="CompPerfTable WholeWidth">
                                            <tr>
                                                <td align="right" class="PaddingTop5">
                                                    <table class="WholeWidth">
                                                        <tr>
                                                            <td class="Width70Per no-wrap" align="left">
                                                                <asp:TextBox ID="txtSearchText" onkeypress="ChangeDefaultFocus(event);" CssClass="Width97Percent"
                                                                    runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                                                    ErrorMessage="Please type text to be searched." ToolTip="Please type text to be searched."
                                                                    Text="*" SetFocusOnError="true" ValidationGroup="Search" Display="Dynamic" />
                                                            </td>
                                                            <td class="width30P" align="right">
                                                                <asp:DropDownList CssClass="Width100Px" onchange="ddlSearchType_onchange(this);"
                                                                    ID="ddlSearchType" runat="server">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" class="PaddingTop5">
                                                    <asp:Button ID="btnSearchAll" runat="server" Text="Go" ToolTip="Go" ValidationGroup="Search"
                                                        PostBackUrl="~/ProjectSearch.aspx" EnableViewState="False" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        <table class="WholeWidth Border2px">
                            <tr>
                                <td class="WholeWidth">
                                    <h1 class="textCenter WholeWidth">
                                        Quick Links</h1>
                                    <table class="WholeWidth">
                                        <tr>
                                            <td class="Width5Percent PaddingTop5">
                                            </td>
                                            <td class="Width90Percent PaddingTop5">
                                                <asp:Panel ID="pnlQuickLinks" CssClass="WholeWidth bgColorWhite" runat="server">
                                                    <table class="WholeWidth border1Px">
                                                        <tr>
                                                            <td class="SeventhTd">
                                                                <asp:Repeater ID="repQuickLinks" OnItemDataBound="repQuickLinks_OnItemDataBound"
                                                                    runat="server">
                                                                    <ItemTemplate>
                                                                        <table class="repQuickLinks">
                                                                            <tr>
                                                                                <td class="FirstRepTd">
                                                                                    <asp:HyperLink ID="hlnkPage" runat="server" NavigateUrl='<%# GetVirtualPath((string)Eval("VirtualPath")) %>'
                                                                                        Text='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>' ToolTip='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>'></asp:HyperLink>
                                                                                </td>
                                                                                <td align="right" class="SecondRepTd">
                                                                                    <asp:ImageButton ID="imgDeleteQuickLink" QuickLinkId='<%# Eval("Id") %>' runat="server"
                                                                                        ImageUrl="~/Images/cross_icon.png" OnClick="imgDeleteQuickLink_OnClick" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ItemTemplate>
                                                                    <AlternatingItemTemplate>
                                                                        <table class="repQuickLinks alterrow">
                                                                            <tr>
                                                                                <td class="FirstRepTd">
                                                                                    <asp:HyperLink ID="hlnkPage" runat="server" NavigateUrl='<%# GetVirtualPath((string)Eval("VirtualPath")) %>'
                                                                                        Text='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>' ToolTip='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>'></asp:HyperLink>
                                                                                </td>
                                                                                <td align="right" class="SecondRepTd">
                                                                                    <asp:ImageButton ID="imgDeleteQuickLink" QuickLinkId='<%# Eval("Id") %>' runat="server"
                                                                                        ImageUrl="~/Images/cross_icon.png" OnClick="imgDeleteQuickLink_OnClick" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </AlternatingItemTemplate>
                                                                </asp:Repeater>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                                <table class="WholeWidth">
                                                    <tr>
                                                        <td class="PaddingBottom5 vMiddle PaddingTop5" align="center">
                                                            <asp:Button ID="btnAddQuicklink" runat="server" Text="Add Quicklink" ToolTip="Add Quicklink" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td class="Width5Percent PaddingTop5">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeQuicklink" runat="server" TargetControlID="btnAddQuicklink"
                OnCancelScript="mpeQuicklink_OnCancelScript();" BackgroundCssClass="modalBackground"
                PopupControlID="pnlQuicklink" CancelControlID="btnCancel" DropShadow="false" />
            <asp:Panel ID="pnlQuicklink" CssClass="DashBoardQuickLinksPopUp" runat="server" Style="display: none;">
                <table class="WholeWidth">
                    <tr>
                        <td class="FirstTd">
                            <center>
                                <b>Quick Links</b>
                            </center>
                            <asp:TextBox ID="txtSearchBox" runat="server" CssClass="DashBoardQuickLinksPopUpTextBox"
                                MaxLength="50" onkeyup="filterQuickLinks(this);"></asp:TextBox>
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmSearch" runat="server" TargetControlID="txtSearchBox"
                                WatermarkText="Begin typing here to filter the list of Quick Links below." WatermarkCssClass="watermarkedtext DashBoardQuickLinksPopUpTextBox"
                                BehaviorID="wmbhSearchBox" />
                            <div class="cbfloatRight cblQuickLinksDiv">
                                <asp:CheckBoxList ID="cblQuickLinks" runat="server" CssClass="WholeWidth bgColorWhite"
                                    AutoPostBack="false" DataTextField="Key" DataValueField="Value" CellPadding="3">
                                </asp:CheckBoxList>
                            </div>
                            <div class="cblQuickLinksClearButtonDiv">
                                <input type="button" value="Clear All" onclick="javascript:ClearQuickLinks();" />
                            </div>
                            <br />
                            <table class="Width356Px">
                                <tr>
                                    <td align="right">
                                        <asp:Button ID="btnSaveQuickLinks" runat="server" OnClick="btnSaveQuickLinks_OnClick"
                                            Text="Save" ToolTip="Save" />
                                        &nbsp;
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdnSelectedQuckLinks" runat="server" />
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSaveQuickLinks" />
            <asp:AsyncPostBackTrigger ControlID="btnSaveAnnouncement" />
            <asp:AsyncPostBackTrigger ControlID="btnEditAnnouncement" />
            <asp:AsyncPostBackTrigger ControlID="btnCancelAnnouncement" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="lpDashBoard" runat="server" />
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="footer">
    <div class="version">
        Version.
        <asp:Label ID="lblCurrentVersion" runat="server"></asp:Label></div>
</asp:Content>

