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
            <table class="CompPerfTable WholeWidth" style="width: 100%; height: 600px; background-color: #dbeef3;">
                <tr>
                    <td valign="top" style="width: 70%; padding-left: 5px; border: 2px solid White; border-top-width: 0px;
                        border-bottom-width: 0px;">
                        <h1>
                            Practice Management Announcements</h1>
                        <table class="WholeWidth">
                            <tr>
                                <td align="right" style="width: 100%; height: 28px; padding-right: 20px;">
                                    <asp:Button ID="btnEditAnnouncement" runat="server" OnClick="btnEditAnnouncement_OnClick"
                                        Text="Edit Announcement" ToolTip="Edit Announcement" />
                                </td>
                            </tr>
                        </table>
                        <table class="WholeWidth">
                            <tr>
                                <td style="width: 2%; padding-top: 0px;">
                                </td>
                                <td valign="top" style="width: 98%; padding-top: 0px;">
                                    <asp:Panel ID="pnlHtmlAnnounceMent" Width="100%" CssClass="ApplyStyleForDashBoardLists"
                                        runat="server" Style="padding: 5px;">
                                        <asp:Label ID="lblAnnounceMent" runat="server"></asp:Label>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlEditAnnounceMent" Width="95%" runat="server" Style="padding: 5px;
                                        display: none;">
                                        <CKEditor:CKEditorControl ID="ckeAnnouncementEditor" ResizeEnabled="false" Toolbar="Bold|Italic|Underline|Strike|-|NumberedList|BulletedList|Outdent|Indent|Format|Font|FontSize|TextColor|BGColor|Link|Unlink|-|Smiley|Cut|Copy|Paste|Undo|Redo|SpellChecker|"
                                            runat="server"></CKEditor:CKEditorControl>
                                        <table style="width: 100%; padding: 5px;">
                                            <tr>
                                                <td align="right" style="width: 50%; padding-top: 5px; padding-right: 5px;">
                                                    <asp:Button ID="btnSaveAnnouncement" runat="server" OnClientClick="HidePanel();" OnClick="btnSaveAnnouncement_OnClick"
                                                        Text="Save Announcement" ToolTip="Save Announcement" />
                                                </td>
                                                <td align="left" style="width: 50%; padding-left: 5px; padding-top: 5px;">
                                                    <asp:Button ID="btnCancelAnnouncement" runat="server" OnClientClick="HidePanel();" OnClick="btnCancelAnnouncement_OnClick"
                                                        Text="Cancel" ToolTip="Cancel" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 30%;" valign="top">
                        <table style="width: 100%;">
                            <tr>
                                <td style="width: 100%;">
                                    <asp:Panel ID="pnlDashBoard" Style="border-bottom: 2px solid White; padding: 5px;
                                        padding-top: 15px; padding-bottom: 15px;" runat="server">
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
                                    <asp:Panel ID="pnlSearchSection" Style="padding: 5px;" runat="server">
                                        <h1 style="text-align: center; width: 100%;">
                                            Search</h1>
                                        <table class="CompPerfTable WholeWidth">
                                            <tr>
                                                <td align="right" style="padding-top: 5px;">
                                                    <table style="width: 100%;">
                                                        <tr>
                                                            <td style="width: 70%; white-space: nowrap;" align="left">
                                                                <asp:TextBox ID="txtSearchText"  onkeypress="ChangeDefaultFocus(event);" Width="97%" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                                                    ErrorMessage="Please type text to be searched." ToolTip="Please type text to be searched."
                                                                    Text="*" SetFocusOnError="true" ValidationGroup="Search" Display="Dynamic" />
                                                            </td>
                                                            <td style="width: 30%;" align="right">
                                                                <asp:DropDownList Width="100px" onchange="ddlSearchType_onchange(this);" ID="ddlSearchType"
                                                                    runat="server">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="padding-top: 5px;">
                                                    <asp:Button ID="btnSearchAll" runat="server" Text="Go" ToolTip="Go" ValidationGroup="Search" 
                                                        PostBackUrl="~/ProjectSearch.aspx" EnableViewState="False" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        <table style="width: 100%; border-top: 2px solid White;">
                            <tr>
                                <td style="width: 100%;">
                                    <h1 style="text-align: center; width: 100%;">
                                        Quick Links</h1>
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 5%; padding-top: 5px;">
                                            </td>
                                            <td style="width: 90%; padding-top: 5px;">
                                                <asp:Panel ID="pnlQuickLinks" Width="100%" BackColor="White" runat="server">
                                                    <table style="border: 1px solid Black;" class="WholeWidth">
                                                        <tr>
                                                            <td style="width: 100%; padding: 5px; height: 50px;">
                                                                <asp:Repeater ID="repQuickLinks" OnItemDataBound="repQuickLinks_OnItemDataBound"
                                                                    runat="server">
                                                                    <ItemTemplate>
                                                                        <table width="100%">
                                                                            <tr>
                                                                                <td style="width: 90%; padding-left: 0px; height: 20px;">
                                                                                    <asp:HyperLink ID="hlnkPage" runat="server" NavigateUrl='<%# GetVirtualPath((string)Eval("VirtualPath")) %>'
                                                                                        Text='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>' ToolTip='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>'></asp:HyperLink>
                                                                                </td>
                                                                                <td align="right" style="width: 10%; padding-right: 0px; height: 20px;">
                                                                                    <asp:ImageButton ID="imgDeleteQuickLink" QuickLinkId='<%# Eval("Id") %>' runat="server"
                                                                                        ImageUrl="~/Images/cross_icon.png" OnClick="imgDeleteQuickLink_OnClick" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ItemTemplate>
                                                                    <AlternatingItemTemplate>
                                                                        <table width="100%" style="background-color: #F9FAFF;">
                                                                            <tr>
                                                                                <td style="width: 90%; padding-left: 0px; height: 20px;">
                                                                                    <asp:HyperLink ID="hlnkPage" runat="server" NavigateUrl='<%# GetVirtualPath((string)Eval("VirtualPath")) %>'
                                                                                        Text='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>' ToolTip='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>'></asp:HyperLink>
                                                                                </td>
                                                                                <td align="right" style="width: 10%; padding-right: 0px; height: 20px;">
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
                                                        <td valign="middle" style="padding-bottom: 5px; padding-top: 5px;" align="center">
                                                            <asp:Button ID="btnAddQuicklink" runat="server" Text="Add Quicklink" ToolTip="Add Quicklink" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td style="width: 5%; padding-top: 5px;">
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
            <asp:Panel ID="pnlQuicklink" runat="server" BorderColor="Black" BackColor="#d4dff8"
                Style="display: none;" Width="372px" BorderWidth="1px">
                <table width="100%">
                    <tr>
                        <td style="padding-left: 5px; padding-top: 5px; padding-bottom: 5px; padding-right: 2px;">
                            <center>
                                <b>Quick Links</b>
                            </center>
                            <asp:TextBox ID="txtSearchBox" runat="server" Width="353px" Height="16px" Style="padding-bottom: 4px;
                                margin-bottom: 4px;" MaxLength="50" onkeyup="filterQuickLinks(this);"></asp:TextBox>
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmSearch" runat="server" TargetControlID="txtSearchBox"
                                WatermarkText="Begin typing here to filter the list of Quick Links below." WatermarkCssClass="watermarkedtext"
                                BehaviorID="wmbhSearchBox" />
                            <div class="cbfloatRight" style="height: 250px; width: 350px; overflow-y: scroll;
                                border: 1px solid black; background: white; padding-left: 3px; text-align: left !important;">
                                <asp:CheckBoxList ID="cblQuickLinks" runat="server" Width="100%" BackColor="White"
                                    AutoPostBack="false" DataTextField="Key" DataValueField="Value" CellPadding="3">
                                </asp:CheckBoxList>
                            </div>
                            <div style="text-align: right; width: 356px; padding: 8px 0px 8px 0px">
                                <input type="button" value="Clear All" onclick="javascript:ClearQuickLinks();" />
                            </div>
                            <br />
                            <table width="356px;">
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

