<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="DashBoard.aspx.cs" Inherits="PraticeManagement.DashBoard" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>DashBoard | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    fDashBoard
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="upnlDashBoard" runat="server">
        <ContentTemplate>
            <script type="text/javascript">

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

                function mpeAnnouncements_OnCancelScript() {
                    var text = document.getElementById('<%= pnlHtmlAnnounceMent.ClientID %>');
                    var editor = document.getElementById('<%= ckeAnnouncementEditor.ClientID %>');

                    if (text != null && editor != null) {
                        editor.text = text.innerHTML;
                    }
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
            <table class="CompPerfTable WholeWidth">
                <tr>
                    <td valign="top" style="width: 65%;">
                        <h1>
                            Practice Management Announcements</h1>
                        <table class="CompPerfTable WholeWidth">
                            <tr>
                                <td style="width: 10%; padding-top: 15px;">
                                </td>
                                <td style="width: 90%; padding-top: 15px;">
                                    <asp:Panel ID="pnlHtmlAnnounceMent" BorderWidth="1px" Width="100%" Height="100px"
                                        BorderStyle="Solid" BorderColor="Black" runat="server" style="padding:5px; overflow-y:auto;">
                                    </asp:Panel>
                                    <asp:Button ID="imgEditAnnouncement" runat="server" Text="Edit" ToolTip="Edit"/>
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeAnnouncement" runat="server" CancelControlID="btnCancelAnnouncement"
                                        PopupControlID="pnlEditAnnounceMent" TargetControlID="imgEditAnnouncement" BackgroundCssClass="modalBackground"
                                        DropShadow="false" OnCancelScript="mpeAnnouncements_OnCancelScript();">
                                    </AjaxControlToolkit:ModalPopupExtender>
                                    <asp:Panel ID="pnlEditAnnounceMent" BorderWidth="1px" Width="500px" BorderStyle="Solid"
                                        BorderColor="Black" runat="server" Style="display: none;">
                                        <CKEditor:CKEditorControl ID="ckeAnnouncementEditor" ResizeEnabled="false" Toolbar="Basic"
                                            runat="server"></CKEditor:CKEditorControl>
                                        <asp:Button ID="btnSaveAnnouncement" runat="server" OnClick="btnSaveAnnouncement_OnClick"
                                            Text="Save AnnounceMent" ToolTip="Save AnnounceMent" />
                                        <asp:Button ID="btnCancelAnnouncement" runat="server" Text="Cancel" ToolTip="Cancel" />
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 5%;">
                        &nbsp;
                    </td>
                    <td style="width: 25%;" valign="top">
                        <asp:Panel ID="pnlDashBoard" runat="server">
                            Go to
                            <asp:DropDownList ID="ddlDashBoardType" AutoPostBack="true" OnSelectedIndexChanged="ddlDashBoardType_OnSelectedIndexChanged"
                                runat="server">
                            </asp:DropDownList>
                            DashBoard
                        </asp:Panel>
                        <asp:Panel ID="pnlSearchSection" runat="server">
                            <h1 style="text-align: center; width: 100%;">
                                Search</h1>
                            <table class="CompPerfTable WholeWidth">
                                <tr>
                                    <td align="right" style="padding-top: 15px;">
                                        <asp:TextBox ID="txtSearchText" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                            ErrorMessage="Please type text to be searched." ToolTip="Please type text to be searched."
                                            Text="*" SetFocusOnError="true" ValidationGroup="Search" Display="Static" />
                                        <asp:DropDownList onchange="ddlSearchType_onchange(this);" ID="ddlSearchType" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="padding-top: 15px;">
                                        <asp:Button ID="btnSearchAll" runat="server" Text="Go" ToolTip="Go" ValidationGroup="Search"
                                            PostBackUrl="~/ProjectSearch.aspx" EnableViewState="False" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                    <td style="width: 5%;">
                    </td>
                </tr>
                <tr>
                    <td valign="top" style="width: 65%;">
                        <h1>
                            My Projects</h1>
                        <table class="CompPerfTable WholeWidth">
                            <tr>
                                <td style="width: 10%; padding-top: 15px;">
                                </td>
                                <td style="width: 90%; padding-top: 15px;">
                                    <asp:Panel ID="Panel1" BorderWidth="1px" Width="100%" Height="100px" BorderStyle="Solid"
                                        BorderColor="Black" runat="server">
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 5%;">
                        &nbsp;
                    </td>
                    <td valign="top" style="width: 25%;">
                        <h1 style="text-align: center; width: 100%;">
                            Quick Links</h1>
                        <asp:Panel ID="pnlQuickLinks" BorderWidth="1px" Width="100%" Style="min-height: 100px;"
                            BorderStyle="Solid" BorderColor="Black" runat="server">
                            <table class="WholeWidth">
                                <tr>
                                    <td>
                                        <asp:Repeater ID="repQuickLinks" OnItemDataBound="repQuickLinks_OnItemDataBound"
                                            runat="server">
                                            <ItemTemplate>
                                                <table width="100%">
                                                    <tr>
                                                        <td style="width: 90%; padding-left: 3px; height: 20px;">
                                                            <asp:HyperLink ID="hlnkPage" runat="server" NavigateUrl='<%# GetVirtualPath((string)Eval("VirtualPath")) %>'
                                                                Text='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>' ToolTip='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>'></asp:HyperLink>
                                                        </td>
                                                        <td align="right" style="width: 10%; padding-right: 5px; height: 15px;">
                                                            <asp:ImageButton ID="imgDeleteQuickLink" QuickLinkId='<%# Eval("Id") %>' runat="server"
                                                                ImageUrl="~/Images/cross_icon.png" OnClick="imgDeleteQuickLink_OnClick" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <table width="100%" style="background-color: #F9FAFF;">
                                                    <tr>
                                                        <td style="width: 90%; padding-left: 3px; height: 20px;">
                                                            <asp:HyperLink ID="hlnkPage" runat="server" NavigateUrl='<%# GetVirtualPath((string)Eval("VirtualPath")) %>'
                                                                Text='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>' ToolTip='<%# HttpUtility.HtmlEncode((string)Eval("LinkName")) %>'></asp:HyperLink>
                                                        </td>
                                                        <td align="right" style="width: 10%; padding-right: 5px; height: 15px;">
                                                            <asp:ImageButton ID="imgDeleteQuickLink" QuickLinkId='<%# Eval("Id") %>' runat="server"
                                                                ImageUrl="~/Images/cross_icon.png" OnClick="imgDeleteQuickLink_OnClick" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </AlternatingItemTemplate>
                                        </asp:Repeater>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="bottom" style="padding-bottom: 5px; padding-top: 5px;" align="center">
                                        <asp:Button ID="btnAddQuicklink" runat="server" Text="Add Quicklink" ToolTip="Add Quicklink" />
                                        <AjaxControlToolkit:ModalPopupExtender ID="mpeQuicklink" runat="server" TargetControlID="btnAddQuicklink"
                                            OnCancelScript="mpeQuicklink_OnCancelScript();" EnableViewState="false" BackgroundCssClass="modalBackground"
                                            PopupControlID="pnlQuicklink" CancelControlID="btnCancel" DropShadow="false" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
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
                    </td>
                    <td style="width: 5%;">
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

