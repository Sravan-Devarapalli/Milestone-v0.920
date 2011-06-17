<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpportunityListControl.ascx.cs"
    Inherits="PraticeManagement.Controls.Opportunities.OpportunityListControl" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="System.Data" %>
<%@ Register Src="~/Controls/ProjectNameCellRounded.ascx" TagName="ProjectNameCellRounded"
    TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc2" %>
<%@ Register Src="~/Controls/Opportunities/ProposedResources.ascx" TagName="ProposedResources"
    TagPrefix="uc" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script src="Scripts/jquery.blockUI.js" type="text/javascript"></script>
<script type="text/javascript" language="javascript">
    var currenthdnProposedPersonsIndexesId = "";
    var refreshMessageIdsFromLastRefresh = new Array();
    function ShowPotentialResourcesModal(image) {
        var refreshLableParentNode = image.parentNode.parentNode.children[0];
        refreshLableParentNode.children[refreshLableParentNode.children.length - 1].style.display = "";
        var savebutton = document.getElementById('btnSaveProposedResources');
        var hdnCurrentOpportunityId = document.getElementById('<%=hdnCurrentOpportunityId.ClientID %>');
        var attachedResourcesIndexes = image.parentNode.children[1].value.split(",");
        currenthdnProposedPersonsIndexesId = image.parentNode.children[1].id;
        var refreshLableParentNode = image.parentNode.parentNode.children[0];
        Array.add(refreshMessageIdsFromLastRefresh, refreshLableParentNode.children[refreshLableParentNode.children.length - 1].id);

        var chkboxes = $('#<%=cblPotentialResources.ClientID %> tr td:first-child :input');
        var strikechkboxes = $('#<%=cblPotentialResources.ClientID %> tr td:nth-child(2) :input');

        $find("wmBhOutSideResources").set_Text(image.parentNode.children[2].value);
        $find("wmbhSearchBox").set_Text('');
        for (var i = 0; i < chkboxes.length; i++) {

            chkboxes[i].checked = chkboxes[i].disabled = strikechkboxes[i].checked = strikechkboxes[i].disabled = false;
            chkboxes[i].parentNode.parentNode.parentNode.style.display = "";
            for (var j = 0; j < attachedResourcesIndexes.length; j++) {
                var indexString = attachedResourcesIndexes[j];
                var index = indexString.substring(0, indexString.indexOf(":", 0));
                var checkBoxType = indexString.substring(indexString.indexOf(":", 0) + 1, indexString.length);
                if (i == index && index != '') {
                    if (checkBoxType == 1) {
                        chkboxes[i].checked = true;
                        strikechkboxes[i].disabled = true;
                    }
                    else {
                        strikechkboxes[i].checked = true;
                        chkboxes[i].disabled = true;
                    }
                    break;
                }
            }
        }
        hdnCurrentOpportunityId.value = image.attributes["OpportunityId"].value;
        return false;
    }

    function GetProposedPersonIdsListWithPersonType() {
        var cblPotentialResources = document.getElementById("<%= cblPotentialResources.ClientID%>");
        var potentialCheckboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');
        var hdnProposedPersonIdsList = document.getElementById("<%= hdnProposedResourceIdsWithTypes.ClientID%>");
        var PersonIdList = '';
        if (cblPotentialResources != null) {
            for (var i = 0; i < potentialCheckboxes.length; ++i) {
                if (potentialCheckboxes[i].checked) {
                    PersonIdList += potentialCheckboxes[i].parentNode.attributes['personid'].value + ':' + potentialCheckboxes[i].parentNode.attributes['persontype'].value + ',';
                }
            }
        }
        hdnProposedPersonIdsList.value = PersonIdList;
    }

    function saveProposedResources() {
        var buttonSave = document.getElementById('<%=btnSaveProposedResourcesHidden.ClientID %>');
        var hdnProposedResourceIndexes = document.getElementById('<%=hdnProposedResourceIndexes.ClientID %>');
        var hdnProposedOutSideResources = document.getElementById('<%=hdnProposedOutSideResources.ClientID %>');

        var chkboxes = $('#<%=cblPotentialResources.ClientID %> tr td:first-child :input');
        hdnProposedOutSideResources.value = $find("wmBhOutSideResources").get_Text(); ;
        for (var i = 0; i < chkboxes.length; i++) {
            if (chkboxes[i].checked) {
                if (hdnProposedResourceIndexes.value == '') {
                    hdnProposedResourceIndexes.value = i;
                }
                else {
                    hdnProposedResourceIndexes.value = hdnProposedResourceIndexes.value + "," + i;
                }
            }
        }
        var hdnProposedPersonsIndexes = document.getElementById(currenthdnProposedPersonsIndexesId);
        hdnProposedPersonsIndexes.value = hdnProposedResourceIndexes.value;
        hdnProposedPersonsIndexes.nextSibling.nextSibling.value = hdnProposedOutSideResources.value;
        GetProposedPersonIdsListWithPersonType();
        buttonSave.click();
    }
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
    function endRequestHandle(sender, Args) {
        for (var i = 0; i < refreshMessageIdsFromLastRefresh.length; i++) {
            var label = document.getElementById(refreshMessageIdsFromLastRefresh[i]);
            label.style.display = "";
        }
    }
    function filterPotentialResources(searchtextBox) {
        var chkboxList = document.getElementById('<%=cblPotentialResources.ClientID %>');
        var chkboxes = $('#<%=cblPotentialResources.ClientID %> tr td:first-child :input');
        var searchText = searchtextBox.value.toLowerCase();

        for (var i = 0; i < chkboxes.length; i++) {
            var checkboxText = chkboxes[i].parentNode.children[1].innerHTML.toLowerCase();
            if (checkboxText.length >= searchText.length && checkboxText.substr(0, searchText.length) == searchText) {

                chkboxes[i].parentNode.parentNode.parentNode.style.display = "";
            }
            else {

                chkboxes[i].parentNode.parentNode.parentNode.style.display = "none";
            }
        }
        changeAlternateitemsForProposedResources();
    }
    function ClearProposedResources() {
        var chkboxList = document.getElementById('<%=cblPotentialResources.ClientID %>');
        var chkboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');
        for (var i = 0; i < chkboxes.length; i++) {
            chkboxes[i].checked = false;
            chkboxes[i].disabled = false;
        }
    }
    function clearOutSideResources() {
        $find("wmBhOutSideResources").set_Text('');
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
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div id="opportunity-list">
            <table cellpadding="0" cellspacing="0" align="center" style="width: 100%; padding-bottom: 10px;
                margin-bottom: 10px;">
                <tr>
                    <td align="center">
                        <asp:Label ID="lblOpportunitiesCount" runat="server" Font-Bold="true" Font-Size="Medium"
                            Text="{0} Opportunities"></asp:Label>
                    </td>
                </tr>
            </table>
            <asp:ListView ID="lvOpportunities" runat="server" DataKeyNames="Id" EnableViewState="true"
                OnSorting="lvOpportunities_Sorting" OnItemDataBound="lvOpportunities_OnItemDataBound">
                <LayoutTemplate>
                    <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">
                        <tr runat="server" id="lvHeader" class="CompPerfHeader">
                            <td width="1%">
                                <div class="ie-bg no-wrap">
                                </div>
                            </td>
                            <td width="4%" align="center">
                                <div class="ie-bg no-wrap" style="text-align: center;">
                                    <asp:LinkButton ID="btnPrioritySort" runat="server" Text="Priority" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="Priority" Style="padding-left: 10px !important;" />
                                    <asp:Image ID="imgPriorityHint" runat="server" ImageUrl="~/Images/hint.png" />
                                    <asp:Panel ID="pnlPriority" Style="display: none;" CssClass="MiniReport" runat="server">
                                        <table>
                                            <tr>
                                                <th align="right">
                                                    <asp:Button ID="btnClosePriority" OnClientClick="return false;" runat="server" CssClass="mini-report-close"
                                                        Text="x" />
                                                </th>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:ListView ID="lvOpportunityPriorities" runat="server">
                                                        <LayoutTemplate>
                                                            <div style="max-height: 150px; overflow-y: auto; overflow-x: hidden;">
                                                                <table id="itemPlaceHolderContainer" runat="server" style="background-color: White;"
                                                                    class="WholeWidth">
                                                                    <tr runat="server" id="itemPlaceHolder">
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </LayoutTemplate>
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td style="width: 100%; padding-left: 2px;">
                                                                    <table class="WholeWidth">
                                                                        <tr>
                                                                            <td align="center" valign="middle" style="text-align: center; color: Black; font-size: 12px;
                                                                                padding: 0px;">
                                                                                <asp:Label ID="lblPriority" Width="15px" runat="server" Text='<%# Eval("Priority") %>'></asp:Label>
                                                                            </td>
                                                                            <td align="center" valign="middle" style="text-align: center; color: Black; font-size: 12px;
                                                                                padding: 0px; padding-left: 2px; padding-right: 2px;">
                                                                                -
                                                                            </td>
                                                                            <td style="padding: 0px; color: Black; font-size: 12px;">
                                                                                <asp:Label ID="lblDescription" runat="server" Width="180px" Style="white-space: normal;"
                                                                                    Text='<%# Eval("Description") %>'></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                        <EmptyDataTemplate>
                                                            <tr>
                                                                <td valign="middle" style="padding-left: 2px;">
                                                                    <asp:Label ID="lblNoPriorities" runat="server" Text="No Priorities."></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </EmptyDataTemplate>
                                                    </asp:ListView>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <AjaxControlToolkit:AnimationExtender ID="animHide" TargetControlID="btnClosePriority"
                                        runat="server">
                                    </AjaxControlToolkit:AnimationExtender>
                                    <AjaxControlToolkit:AnimationExtender ID="animShow" TargetControlID="imgPriorityHint"
                                        runat="server">
                                    </AjaxControlToolkit:AnimationExtender>
                                </div>
                            </td>
                            <td width="4%" align="center">
                                <div class="ie-bg no-wrap" style="text-align: center;">
                                    <asp:LinkButton ID="btnStartDateSort" runat="server" Text="Start" CommandName="Sort"
                                        CssClass="arrow" Style="padding-left: 10px !important;" CommandArgument="StartDate" />
                                </div>
                            </td>
                            <td width="13%">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnClientNameSort" runat="server" Text="Client - Group" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="ClientName" />
                                </div>
                            </td>
                            <td width="9%">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnBuyerNameSort" runat="server" Text="Buyer Name" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="BuyerName" />
                                </div>
                            </td>
                            <td width="24%">
                                <div class="ie-bg no-wrap" style="white-space: nowrap">
                                    <asp:LinkButton ID="btnOpportunityNameSort" runat="server" Text="Opportunity Name"
                                        CommandName="Sort" CssClass="arrow" CommandArgument="OpportunityName" />
                                </div>
                            </td>
                            <td width="7%">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnSalespersonSort" runat="server" Text="Sales Team" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="Salesperson" />
                                </div>
                            </td>
                            <td align="center" width="10%">
                                <div class="ie-bg no-wrap" style="text-align: center;">
                                    <asp:LinkButton ID="btnEstimatedRevenue" runat="server" Text="Est. Revenue" CommandName="Sort"
                                        CssClass="arrow" Style="padding-left: 10px !important;" CommandArgument="EstimatedRevenue" />
                                </div>
                            </td>
                            <td align="center" width="28%">
                                <div class="ie-bg no-wrap" style="color: Black;">
                                    Proposed Resources
                                </div>
                            </td>
                        </tr>
                        <tr runat="server" id="itemPlaceholder" class="CompPerfHeader" />
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trOpportunity" runat="server">
                        <td>
                            <div class="cell-pad">
                                <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                                    ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' ButtonCssClass='<%#PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClassByStatus((string)Eval("Status.Name"))%>' />
                            </div>
                        </td>
                        <td align="center">
                            <div class="cell-pad">
                                <asp:Label ID="lblPriority" runat="server" Text='<%# ((Opportunity) Container.DataItem).Priority.Priority %>' /></div>
                        </td>
                        <td align="left">
                            <div class="cell-pad">
                                <%# Eval("ProjectedStartDate") == null ? string.Empty : string.Format("{0:MMM} '{0:yy}", ((DateTime)Eval("ProjectedStartDate")))%>
                            </div>
                        </td>
                        <td>
                            <div class="cell-pad">
                                <asp:Label ID="lblClientName" runat="server" Text='<%# ((Opportunity) Container.DataItem).ClientAndGroup %>' /></div>
                        </td>
                        <td>
                            <div class="cell-pad">
                                <asp:Label ID="lblBuyerName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("BuyerName"))%>' /></div>
                        </td>
                        <td>
                            <div class="cell-pad">
                                <asp:HyperLink ID="hlName" runat="server" NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
                            <%# HttpUtility.HtmlEncode((string)Eval("Name")) %>
                                </asp:HyperLink>
                            </div>
                        </td>
                        <td width="9%">
                            <div class="cell-pad">
                                <asp:Label ID="lblSalesTeam" runat="server" Text='<%# GetSalesTeam((((Opportunity)Container.DataItem).Salesperson),(((Opportunity)Container.DataItem).Owner))%>' /></div>
                        </td>
                        <td align="right" style="padding-right: 10px;">
                            <div class="cell-pad">
                                <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' />
                            </div>
                        </td>
                        <td align="left">
                            <div class="cell-pad">
                                <table width="100%" style="padding: 2px;">
                                    <tr>
                                        <td style="width: 96%;">
                                            <asp:DataList ID="dtlProposedPersons" runat="server" Style="white-space: normal;">
                                                <ItemTemplate>
                                                    <%# GetFormattedPersonName((string)Eval("PersonLastFirstName"),(int)Eval("OpportunityPersonTypeId"))%>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <div style="white-space: normal;">
                                                <asp:Literal ID="ltrlOutSideResources" runat="server"></asp:Literal>
                                            </div>
                                            <asp:Label ID="lblRefreshMessage" runat="server" Text="Please &lt;a href='javascript:location.reload(true)'&gt;refresh&lt;/a&gt; to see new changes."
                                                Style="display: none; font-style: italic;"></asp:Label>
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgPeople_icon" runat="server" ImageUrl="~/Images/People_icon.png"
                                                onclick="ShowPotentialResourcesModal(this);" Style="cursor: pointer;" opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnProposedPersonsIndexes" runat="server" />
                                            <asp:HiddenField ID="hdnOutSideResources" runat="server" />
                                            <AjaxControlToolkit:ModalPopupExtender ID="mpeAttachToProject" runat="server" TargetControlID="imgPeople_icon" EnableViewState="false" 
                                                BackgroundCssClass="modalBackground" PopupControlID="pnlPotentialResources" CancelControlID="btnCancel"
                                                DropShadow="false" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr id="trOpportunity" runat="server" class="rowEven">
                        <td>
                            <div class="cell-pad">
                                <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                                    ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' ButtonCssClass='<%#PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClassByStatus((string)Eval("Status.Name"))%>' />
                            </div>
                        </td>
                        <td align="center">
                            <div class="cell-pad">
                                <asp:Label ID="lblPriority" runat="server" Text='<%#  ((Opportunity) Container.DataItem).Priority.Priority %>' /></div>
                        </td>
                        <td align="left">
                            <div class="cell-pad">
                                <%# Eval("ProjectedStartDate") == null ? string.Empty : string.Format("{0:MMM} '{0:yy}", ((DateTime)Eval("ProjectedStartDate")))%>
                            </div>
                        </td>
                        <td>
                            <div class="cell-pad">
                                <asp:Label ID="lblClientName" runat="server" Text='<%# ((Opportunity) Container.DataItem).ClientAndGroup %>' /></div>
                        </td>
                        <td>
                            <div class="cell-pad">
                                <asp:Label ID="lblBuyerName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("BuyerName"))%>' /></div>
                        </td>
                        <td>
                            <div class="cell-pad">
                                <asp:HyperLink ID="hlName" runat="server" NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
                            <%# HttpUtility.HtmlEncode((string)Eval("Name")) %>
                                </asp:HyperLink>
                            </div>
                        </td>
                        <td>
                            <div class="cell-pad">
                                <asp:Label ID="lblSalesTeam" runat="server" Text='<%# GetSalesTeam((((Opportunity)Container.DataItem).Salesperson),(((Opportunity)Container.DataItem).Owner))%>' /></div>
                        </td>
                        <td align="right" style="padding-right: 10px;">
                            <div class="cell-pad">
                                <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' />
                            </div>
                        </td>
                        <td align="left">
                            <div class="cell-pad">
                                <table width="100%" style="padding: 2px;">
                                    <tr>
                                        <td style="width: 96%;">
                                            <asp:DataList ID="dtlProposedPersons" runat="server" Style="white-space: normal;">
                                                <ItemTemplate>
                                                    <%# GetFormattedPersonName((string)Eval("PersonLastFirstName"),(int)Eval("OpportunityPersonTypeId"))%>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <div style="white-space: normal;">
                                                <asp:Literal ID="ltrlOutSideResources" runat="server"></asp:Literal>
                                            </div>
                                            <asp:Label ID="lblRefreshMessage" runat="server" Text="Please &lt;a href='javascript:location.reload(true)'&gt;refresh&lt;/a&gt; to see new changes."
                                                Style="display: none; font-style: italic;"></asp:Label>
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgPeople_icon" runat="server" ImageUrl="~/Images/People_icon.png"
                                                onclick="ShowPotentialResourcesModal(this);" Style="cursor: pointer;" opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnProposedPersonsIndexes" runat="server" />
                                            <asp:HiddenField ID="hdnOutSideResources" runat="server" />
                                            <AjaxControlToolkit:ModalPopupExtender ID="mpeAttachToProject" runat="server" TargetControlID="imgPeople_icon"
                                                BackgroundCssClass="modalBackground" PopupControlID="pnlPotentialResources" CancelControlID="btnCancel" OkControlID="btnSaveProposedResources"
                                                DropShadow="false" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <EmptyDataTemplate>
                    <tr runat="server" id="EmptyDataRow">
                        <td>
                            No opportunities found.
                        </td>
                    </tr>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <asp:HiddenField ID="hdnPreviouslyClickedRowIndex" runat="server" />
        <asp:Panel ID="pnlPotentialResources" runat="server" BorderColor="Black" BackColor="#d4dff8"
            Width="372px" Style="display: none" BorderWidth="1px">
            <table width="100%">
                <tr>
                    <td style="padding-left: 5px; padding-top: 5px; padding-bottom: 5px; padding-right: 2px;">
                        <center>
                            <b>Potential Resources</b>
                        </center>
                        <asp:TextBox ID="txtSearchBox" runat="server" Width="353px" Height="16px" Style="padding-bottom: 4px;
                            margin-bottom: 4px;" MaxLength="4000" onkeyup="filterPotentialResources(this);"></asp:TextBox>
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmSearch" runat="server" TargetControlID="txtSearchBox"
                            WatermarkText="Begin typing here to filter the list of resources below." EnableViewState="false" 
                            WatermarkCssClass="watermarkedtext" BehaviorID="wmbhSearchBox" />
                        <table>
                            <tr>
                                <td style="width: 300px;">
                                </td>
                                <td style="padding-right: 2px;">
                                    <asp:Image ID="imgCheck" runat="server" ImageUrl="~/Images/right_icon.png" />
                                </td>
                                <td style="padding-left: 2px;">
                                    <asp:Image ID="imgCross" runat="server" ImageUrl="~/Images/cross_icon.png" />
                                </td>
                            </tr>
                        </table>
                        <div class="cbfloatRight" style="height: 250px; width: 350px; overflow-y: scroll;
                            border: 1px solid black; background: white; padding-left: 3px; text-align: left !important;">
                            <uc:MultipleSelectionCheckBoxList ID="cblPotentialResources" runat="server" Width="100%"
                                BackColor="White" AutoPostBack="false" DataTextField="Name" DataValueField="id"
                                CellPadding="3">
                            </uc:MultipleSelectionCheckBoxList>
                        </div>
                        <div style="text-align: right;width: 356px; padding: 8px 0px 8px 0px">
                            <input type="button" value="Clear All" onclick="javascript:ClearProposedResources();" />
                        </div>
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 93% !important;">
                                    <asp:TextBox ID="txtOutSideResources" runat="server" Width="100%" Height="16px" Style="padding-bottom: 4px;
                                        margin-bottom: 4px;" MaxLength="4000"></asp:TextBox>
                                    <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmOutSideResources" runat="server"
                                        TargetControlID="txtOutSideResources" WatermarkText="Enter Other Names(s) (optional) separated by semi-colons."
                                        EnableViewState="false" WatermarkCssClass="watermarkedtext" BehaviorID="wmBhOutSideResources" />
                                </td>
                                <td align="right">
                                    <img id="imgtrash" src="Images/trash-icon-Large.png" onclick="clearOutSideResources();"
                                        style="cursor: pointer; padding-bottom: 5px;" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table width="356px;">
                            <tr>
                                <td align="right">
                                    <input type="button" id="btnSaveProposedResources" value="Save" onclick="javascript:saveProposedResources();" />
                                    &nbsp;
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:HiddenField ID="hdnCurrentOpportunityId" runat="server" Value="" />
        <asp:HiddenField ID="hdnProposedResourceIndexes" runat="server" Value="" />
        <asp:HiddenField ID="hdnProposedResourceIdsWithTypes" runat="server" Value="" />
        <asp:HiddenField ID="hdnProposedOutSideResources" runat="server" Value="" />
        <asp:Button ID="btnSaveProposedResourcesHidden" runat="server" OnClick="btnSaveProposedResources_OnClick"
            Style="display: none;" />
    </ContentTemplate>
</asp:UpdatePanel>
<asp:ValidationSummary ID="valsum" ValidationGroup="Notes" runat="server" />

