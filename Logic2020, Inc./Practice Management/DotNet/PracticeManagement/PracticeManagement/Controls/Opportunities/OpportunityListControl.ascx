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
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script src="Scripts/jquery.blockUI.js" type="text/javascript"></script>
<script type="text/javascript" language="javascript">
    var currenthdnProposedPersonsIndexesId = "";
    var currenthdnTeamStructurePersonsId = "";
    var refreshOpportunityIdsFromLastRefresh = new Array();

    function ShowTeamStructureModal(image) {
        var oppId = image.attributes['opportunityid'].value;

        var hdnCurrentOpportunityId = document.getElementById('<%=hdnCurrentOpportunityId.ClientID %>');
        var attachedTeam = image.parentNode.children[1].value.split(",");
        currenthdnTeamStructurePersonsId = image.parentNode.children[1].id;
        var refreshLableParentNode = $(image.parentNode.parentNode);
        var lblRefreshMessage = refreshLableParentNode.find("span[id$='lblRefreshMessage']")[0];
        lblRefreshMessage.style.display = 'block';
        Array.add(refreshOpportunityIdsFromLastRefresh, oppId);
        var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
        for (var i = 0; i < trTeamStructure.length; i++) {
            var hdnPersonId = trTeamStructure[i].children[0].getElementsByTagName('input')[0];
            var ddlQuantity = trTeamStructure[i].children[1].getElementsByTagName('SELECT')[0];
            var chkEnabled = trTeamStructure[i].children[2].getElementsByTagName('input')[0];
            ddlQuantity.selectedIndex = 0;
            chkEnabled.checked = false;
            for (var j = 0; j < attachedTeam.length; j++) {
                var personString = attachedTeam[j];
                var personId = personString.substring(0, personString.indexOf(":", 0));
                if (personId == hdnPersonId.value) {
                    var personType = personString.substring(personString.indexOf(":", 0) + 1, personString.indexOf("|", 0));
                    var Quantity = personString.substring(personString.indexOf("|", 0) + 1, personString.length);
                    ddlQuantity.value = Quantity;
                    if (personType != "1") {
                        chkEnabled.checked = true;
                    }
                    break;
                }
            }

        }
        hdnCurrentOpportunityId.value = image.attributes["OpportunityId"].value;
        $find("behaviorIdTeamStructure").show();
        return false;
    }

    function ShowPotentialResourcesModal(image) {
        var oppId = image.attributes['opportunityid'].value;

        var hdnCurrentOpportunityId = document.getElementById('<%=hdnCurrentOpportunityId.ClientID %>');
        var attachedResourcesIndexes = image.parentNode.children[1].value.split(",");
        currenthdnProposedPersonsIndexesId = image.parentNode.children[1].id;

        var refreshLableParentNode = $(image.parentNode.parentNode);
        var lblRefreshMessage = refreshLableParentNode.find("span[id$='lblRefreshMessage']")[0];
        lblRefreshMessage.style.display = 'block';
        Array.add(refreshOpportunityIdsFromLastRefresh, oppId);
        var trPotentialResources = document.getElementById('<%=cblPotentialResources.ClientID %>').getElementsByTagName('tr');

        $find("wmBhOutSideResources").set_Text(image.parentNode.children[2].value);
        $find("wmbhSearchBox").set_Text('');

        for (var i = 0; i < trPotentialResources.length; i++) {
            var checkBox = trPotentialResources[i].children[0].getElementsByTagName('input')[0];
            var strikeCheckBox = trPotentialResources[i].children[1].getElementsByTagName('input')[0];
            checkBox.checked = checkBox.disabled = strikeCheckBox.checked = strikeCheckBox.disabled = false;
            trPotentialResources[i].style.display = "";
            for (var j = 0; j < attachedResourcesIndexes.length; j++) {
                var indexString = attachedResourcesIndexes[j];
                var index = indexString.substring(0, indexString.indexOf(":", 0));
                var checkBoxType = indexString.substring(indexString.indexOf(":", 0) + 1, indexString.length);
                if (i == index && index != '') {
                    if (checkBoxType == 1) {
                        checkBox.checked = true;
                        strikeCheckBox.disabled = true;
                    }
                    else {
                        strikeCheckBox.checked = true;
                        checkBox.disabled = true;
                    }
                    break;
                }
            }
        }
        hdnCurrentOpportunityId.value = image.attributes["OpportunityId"].value;
        $find("behaviorIdPotentialResources").show();
        return false;
    }

    function GetProposedPersonIdsListWithPersonType() {
        var cblPotentialResources = document.getElementById("<%= cblPotentialResources.ClientID%>");
        var potentialCheckboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');
        var hdnProposedPersonIdsList = document.getElementById("<%= hdnProposedResourceIdsWithTypes.ClientID%>");
        var PersonIdList = '';
        var personIndexesList = '';
        if (cblPotentialResources != null) {
            for (var i = 0; i < potentialCheckboxes.length; ++i) {
                if (potentialCheckboxes[i].checked) {
                    PersonIdList += potentialCheckboxes[i].parentNode.attributes['personid'].value + ':' + potentialCheckboxes[i].parentNode.attributes['persontype'].value + ',';
                    personIndexesList += potentialCheckboxes[i].parentNode.attributes['itemIndex'].value + ':' + potentialCheckboxes[i].parentNode.attributes['persontype'].value + ',';
                }
            }
        }
        hdnProposedPersonIdsList.value = PersonIdList;
        var hdnProposedPersonsIndexes = document.getElementById(currenthdnProposedPersonsIndexesId);
        hdnProposedPersonsIndexes.value = personIndexesList;
    }
    function UpdateTeamStructureForHiddenfields(){
     
    var hdnTeamStructure = document.getElementById("<%= hdnTeamStructure.ClientID%>");
    var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
    var PersonIdList = '';
    var personType;
    for (var i = 0; i < trTeamStructure.length; i++) {
            
            var hdnPersonId = trTeamStructure[i].children[0].getElementsByTagName('input')[0];
            var ddlQuantity = trTeamStructure[i].children[1].getElementsByTagName('SELECT')[0];
            var chkEnabled = trTeamStructure[i].children[2].getElementsByTagName('input')[0];
            if(ddlQuantity.selectedIndex >0 || chkEnabled.checked){
            personType ='';
            if(!chkEnabled.checked)
            {
            personType = '1';
            }
            else
            {
              personType = '2';
            }
            PersonIdList=PersonIdList+hdnPersonId.value+':'+personType+'|'+ddlQuantity.value+',';
            }
    }
    var currenthdnTeamStructurePersons = document.getElementById(currenthdnTeamStructurePersonsId);
    hdnTeamStructure.value =  currenthdnTeamStructurePersons.value = PersonIdList;
    }
    function saveProposedResources() {
        var buttonSave = document.getElementById('<%=btnSaveProposedResourcesHidden.ClientID %>');
        var hdnProposedOutSideResources = document.getElementById('<%=hdnProposedOutSideResources.ClientID %>');
        var trPotentialResources = document.getElementById('<%=cblPotentialResources.ClientID %>').getElementsByTagName('tr');
        hdnProposedOutSideResources.value = $find("wmBhOutSideResources").get_Text();
        var hdnProposedPersonsIndexes = document.getElementById(currenthdnProposedPersonsIndexesId);
        hdnProposedPersonsIndexes.nextSibling.nextSibling.value = hdnProposedOutSideResources.value;
        GetProposedPersonIdsListWithPersonType();
        buttonSave.click();
    }
    function saveTeamStructure() {
        var buttonSave = document.getElementById('<%=btnSaveTeamStructureHidden.ClientID %>');
        var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
        UpdateTeamStructureForHiddenfields();
        buttonSave.click();
    }
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
    function endRequestHandle(sender, Args) {
        var lblRefreshMessageList = $("span[id$='lblRefreshMessage']");
        for (var j = 0; j < lblRefreshMessageList.length; j++) {
            for (var i = 0; i < refreshOpportunityIdsFromLastRefresh.length; i++) {
                if (lblRefreshMessageList[j].attributes['opportunityid'].value == refreshOpportunityIdsFromLastRefresh[i]) {
                    lblRefreshMessageList[j].style.display = 'block';
                }
            }
        }

    }
    function filterPotentialResources(searchtextBox) {
        var trPotentialResources = document.getElementById('<%=cblPotentialResources.ClientID %>').getElementsByTagName('tr');
        var searchText = searchtextBox.value.toLowerCase();
        for (var i = 0; i < trPotentialResources.length; i++) {
            var checkBox = trPotentialResources[i].children[0].getElementsByTagName('input')[0];
            var checkboxText = checkBox.parentNode.children[1].innerHTML.toLowerCase();

            if (checkboxText.length >= searchText.length && checkboxText.substr(0, searchText.length) == searchText) {

                trPotentialResources[i].style.display = "";
            }
            else {

                trPotentialResources[i].style.display = "none";
            }
        }
        changeAlternateitemsForProposedResources('<%=cblPotentialResources.ClientID %>');
    }

    function filterTeamStructure(searchtextBox) {
        var trTeamMembers = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
        var searchText = searchtextBox.value.toLowerCase();
        for (var i = 0; i < trTeamMembers.length; i++) {
            var label = trTeamMembers[i].children[0].getElementsByTagName('span')[0];
            var labelText = label.innerHTML.toLowerCase();

            if (labelText.length >= searchText.length && labelText.substr(0, searchText.length) == searchText) {

                trTeamMembers[i].style.display = "";
            }
            else {

                trTeamMembers[i].style.display = "none";
            }
        }
        changeAlternateitemsForProposedResources('tblTeamStructure');
    }

    function ClearProposedResources() {

        var chkboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');
        for (var i = 0; i < chkboxes.length; i++) {
            chkboxes[i].checked = false;
            chkboxes[i].disabled = false;
        }
    }

    function ClearTeamStructure() {

        var ddlQtys = $('#tblTeamStructure tr td :input[type = "select-one"]');
        for (var i = 0; i < ddlQtys.length; i++) {
            ddlQtys[i].selectedIndex = 0;
        }
        var checkboxes = $('#tblTeamStructure tr td :input[type = "checkbox"]');
        for (var i = 0; i < checkboxes.length; i++) {
            checkboxes[i].checked = false;
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

    function ddlPriorityList_onchange(ddlPriority) {
        var urlVal = "OpportunityPriorityHandler.ashx?OpportunityID=" + ddlPriority.attributes["OpportunityID"].value + "&PriorityID=" + ddlPriority.value;
        $.post(urlVal, function (dat) {
        });
    }

    function SetTooltipText(descriptionText, hlinkObj) {
        var hlinkObjct = $('#' + hlinkObj.id);
        var displayPanel = $('#<%= oppNameToolTipHolder.ClientID %>');
        iptop = hlinkObjct.offset().top - hlinkObjct[0].offsetHeight;
        ipleft = hlinkObjct.offset().left + hlinkObjct[0].offsetWidth + 10;
        iptop = iptop;
        ipleft = ipleft;
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();

        var lbloppNameTooltipContent = document.getElementById('<%= lbloppNameTooltipContent.ClientID %>');
        lbloppNameTooltipContent.innerHTML = descriptionText.toString();

        if (navigator.userAgent.indexOf(' Chrome/') > -1) {
            if (descriptionText.toString().length > 50) {
                lbloppNameTooltipContent.style.width = "330px";
            }
            else {
                lbloppNameTooltipContent.style.width = "100%";
            }
        }
    }

    function pageLoad() {
        var lbloppNameTooltipContent = document.getElementById('<%= lbloppNameTooltipContent.ClientID %>');

        if (navigator.userAgent.indexOf(' Chrome/') > -1) {
            lbloppNameTooltipContent.style.width = "330px";
        }
    }

    function HidePanel() {

        var displayPanel = $('#<%= oppNameToolTipHolder.ClientID %>');
        displayPanel.hide();
    }


</script>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Panel ID="oppNameToolTipHolder" Style="display: none; position: absolute; z-index: 2000;"
            runat="server" CssClass="ToolTip">
            <table cellpadding="0" cellspacing="0">
                <tr class="top">
                    <td class="lt">
                        <div class="tail">
                        </div>
                    </td>
                    <td class="tbor">
                    </td>
                    <td class="rt">
                    </td>
                </tr>
                <tr class="middle">
                    <td class="lbor">
                    </td>
                    <td class="content">
                        <pre>
<asp:Label ID="lbloppNameTooltipContent" CssClass="WordWrap" Width="100%" runat="server"></asp:Label>
</pre>
                    </td>
                    <td class="rbor">
                    </td>
                </tr>
                <tr class="bottom">
                    <td class="lb">
                    </td>
                    <td class="bbor">
                    </td>
                    <td class="rb">
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table cellpadding="0" cellspacing="0" align="center" style="width: 100%; padding-bottom: 10px;
            margin-bottom: 10px;">
            <tr>
                <td align="center">
                    <asp:Label ID="lblOpportunitiesCount" runat="server" Font-Bold="true" Font-Size="Medium"
                        Text="{0} Opportunities"></asp:Label>
                </td>
            </tr>
        </table>
        <div>
            <div class="buttons-block">
                <table class="WholeWidth">
                    <tr>
                        <td>
                            <ajaxToolkit:CollapsiblePanelExtender ID="cpeSummary" runat="Server" TargetControlID="pnlSummary"
                                ImageControlID="btnExpandCollapseSummary" CollapsedImage="~/Images/expand.jpg"
                                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseSummary"
                                ExpandControlID="btnExpandCollapseSummary" Collapsed="True" TextLabelID="lblSummary" />
                            <asp:Image ID="btnExpandCollapseSummary" runat="server" ImageUrl="~/Images/collapse.jpg"
                                ToolTip="Expand Summary Details" />&nbsp;
                            <asp:Label ID="lblSummary" runat="server" Text="Summary"></asp:Label>
                        </td>
                        <td>
                            <asp:ShadowedHyperlink runat="server" Text="Add Opportunity" ID="lnkAddOpportunity"
                                CssClass="add-btn" NavigateUrl="~/OpportunityDetail.aspx" />
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Panel CssClass="summary" Style="white-space: nowrap; overflow-x: auto;" ID="pnlSummary"
                runat="server">
            </asp:Panel>
        </div>
        <div id="opportunity-list">
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
                                    ButtonProjectNameToolTip='<%# PraticeManagement.Utils.OpportunitiesHelper.GetToolTip((Opportunity) Container.DataItem) %>'
                                    ButtonCssClass='<%# PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClass((Opportunity) Container.DataItem)%>'
                                    ButtonProjectNameHref="<%# GetProjectDetailUrl((Opportunity) Container.DataItem) %>"
                                    Target="_blank" />
                            </div>
                        </td>
                        <td align="center">
                            <div class="cell-pad">
                                <asp:DropDownList ID="ddlPriorityList" onchange="ddlPriorityList_onchange(this);"
                                    runat="server">
                                </asp:DropDownList>
                            </div>
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
                                <asp:HyperLink ID="hlName" Description='<%# GetWrappedText((string)((Opportunity) Container.DataItem).Description) %>'
                                    onmouseout="HidePanel();" onmouseover="SetTooltipText(this.attributes['Description'].value,this);"
                                    runat="server" NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
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
                                                    <%# GetFormattedPersonName(((DataTransferObjects.OpportunityPerson)Container.DataItem).Person.PersonLastFirstName, 
                                                    ((DataTransferObjects.OpportunityPerson)Container.DataItem).PersonType )%>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <div style="white-space: normal;">
                                                <asp:Literal ID="ltrlOutSideResources" runat="server"></asp:Literal>
                                            </div>
                                            <asp:DataList ID="dtlTeamStructure" runat="server" Style="white-space: normal;">
                                                <ItemTemplate>
                                                    <%# GetFormattedPersonName(((DataTransferObjects.OpportunityPerson)Container.DataItem).Person.PersonLastFirstName, 
                                                    ((DataTransferObjects.OpportunityPerson)Container.DataItem).PersonType )%>
                                                    (<%#((DataTransferObjects.OpportunityPerson)Container.DataItem).Quantity.ToString() %>)
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <asp:Label ID="lblRefreshMessage" opportunityid='<%# Eval("Id") %>' runat="server"
                                                Text="Please &lt;a href='javascript:location.reload(true)'&gt;refresh&lt;/a&gt; to see new changes."
                                                Style="display: none; font-style: italic;"></asp:Label>
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgPeople_icon" runat="server" ImageUrl="~/Images/People_icon.png" ToolTip="Show Proposed Resources"
                                                onclick="ShowPotentialResourcesModal(this);" Style="cursor: pointer;" opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnProposedPersonsIndexes" runat="server" />
                                            <asp:HiddenField ID="hdnOutSideResources" runat="server" />
                                        </td>
                                        <td style="padding-left:4px;width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgTeamStructure" runat="server" ImageUrl="~/Images/Strawman.png" ToolTip="Show straw men"
                                                onclick="ShowTeamStructureModal(this);" Style="cursor: pointer;" opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnTeamStructure" runat="server" />
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
                                    ButtonProjectNameToolTip='<%# PraticeManagement.Utils.OpportunitiesHelper.GetToolTip((Opportunity) Container.DataItem) %>'
                                    ButtonCssClass='<%# PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClass((Opportunity) Container.DataItem)%>'
                                    ButtonProjectNameHref="<%# GetProjectDetailUrl((Opportunity) Container.DataItem) %>"
                                    Target="_blank" />
                            </div>
                        </td>
                        <td align="center">
                            <div class="cell-pad">
                                <asp:DropDownList ID="ddlPriorityList" onchange="ddlPriorityList_onchange(this);"
                                    runat="server">
                                </asp:DropDownList>
                            </div>
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
                                <asp:HyperLink ID="hlName" Description='<%# GetWrappedText((string)((Opportunity) Container.DataItem).Description) %>'
                                    onmouseout="HidePanel();" onmouseover="SetTooltipText(this.attributes['Description'].value,this);"
                                    runat="server" NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
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
                                                    <%# GetFormattedPersonName(((DataTransferObjects.OpportunityPerson)Container.DataItem).Person.PersonLastFirstName, 
                                                    ((DataTransferObjects.OpportunityPerson)Container.DataItem).PersonType )%>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <div style="white-space: normal;">
                                                <asp:Literal ID="ltrlOutSideResources" runat="server"></asp:Literal>
                                            </div>
                                            <asp:DataList ID="dtlTeamStructure" runat="server" Style="white-space: normal;">
                                                <ItemTemplate>
                                                    <%# GetFormattedPersonName(((DataTransferObjects.OpportunityPerson)Container.DataItem).Person.PersonLastFirstName, 
                                                    ((DataTransferObjects.OpportunityPerson)Container.DataItem).PersonType )%>
                                                    (<%#((DataTransferObjects.OpportunityPerson)Container.DataItem).Quantity.ToString() %>)
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <asp:Label ID="lblRefreshMessage" opportunityid='<%# Eval("Id") %>' runat="server"
                                                Text="Please &lt;a href='javascript:location.reload(true)'&gt;refresh&lt;/a&gt; to see new changes."
                                                Style="display: none; font-style: italic;"></asp:Label>
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgPeople_icon" runat="server" ImageUrl="~/Images/People_icon.png" ToolTip="Show Proposed Resources"
                                                onclick="ShowPotentialResourcesModal(this);" Style="cursor: pointer;" opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnProposedPersonsIndexes" runat="server" />
                                            <asp:HiddenField ID="hdnOutSideResources" runat="server" />
                                        </td>
                                        <td style="padding-left:4px;width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgTeamStructure" runat="server" ImageUrl="~/Images/Strawman.png" ToolTip="Show straw men"
                                                onclick="ShowTeamStructureModal(this);" Style="cursor: pointer;" opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnTeamStructure" runat="server" />
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
        <asp:HiddenField ID="hdnmpePotentialResources" runat="server" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpePotentialResources" runat="server"
            BehaviorID="behaviorIdPotentialResources" TargetControlID="hdnmpePotentialResources"
            EnableViewState="false" BackgroundCssClass="modalBackground" PopupControlID="pnlPotentialResources"
            CancelControlID="btnCancel" DropShadow="false" />
        <asp:Panel ID="pnlPotentialResources" runat="server" BorderColor="Black" BackColor="#d4dff8"
            Style="display: none;" Width="372px" BorderWidth="1px">
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
                                <td style="width: 304px;">
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
                        <div style="text-align: right; width: 356px; padding: 8px 0px 8px 0px">
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
        <asp:HiddenField ID="hdnProposedResourceIdsWithTypes" runat="server" Value="" />
        <asp:HiddenField ID="hdnProposedOutSideResources" runat="server" Value="" />
        <asp:Button ID="btnSaveProposedResourcesHidden" runat="server" OnClick="btnSaveProposedResources_OnClick"
            Style="display: none;" />
        <asp:HiddenField ID="hdnmpeTeamStructure" runat="server" Value="" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpeTeamStructure" runat="server" BehaviorID="behaviorIdTeamStructure"
            TargetControlID="hdnmpeTeamStructure" EnableViewState="false" BackgroundCssClass="modalBackground"
            PopupControlID="pnlTeamStructure" CancelControlID="btnTeamCancel" DropShadow="false" />
        <asp:Panel ID="pnlTeamStructure" runat="server" BorderColor="Black" BackColor="#d4dff8"
            Width="372px" BorderWidth="1px" Style="display: none;">
            <table width="100%">
                <tr>
                    <td style="padding-left: 5px; padding-top: 5px; padding-bottom: 5px; padding-right: 2px;">
                        <center>
                            <b>Team structure</b>
                        </center>
                        <asp:TextBox ID="txtTeamSearchBox" runat="server" Width="353px" Height="16px" Style="padding-bottom: 4px;
                            margin-bottom: 4px;" MaxLength="4000" onkeyup="filterTeamStructure(this);"></asp:TextBox>
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmTeamSearchTeam" runat="server"
                            TargetControlID="txtTeamSearchBox" WatermarkText="Begin typing here to filter the list of resources below."
                            EnableViewState="false" WatermarkCssClass="watermarkedtext" BehaviorID="wmbhSearchBox" />
                        <table>
                            <tr>
                                <td style="width: 275px;">
                                </td>
                                <td style="text-align: left; width: 45px;">
                                    QTY
                                </td>
                                <td style="padding-left: 2px;">
                                    <asp:Image ID="imgTeamCross" runat="server" ImageUrl="~/Images/cross_icon.png" />
                                </td>
                            </tr>
                        </table>
                        <div class="cbfloatRight" style="height: 250px; width: 350px; overflow-y: scroll;
                            border: 1px solid black; background: white; padding-left: 3px; text-align: left !important;">
                            <table width="100%" id="tblTeamStructure">
                                <asp:Repeater ID="rpTeamStructure" runat="server" OnItemDataBound="rpTeamStructure_OnItemDataBound">
                                    <ItemTemplate>
                                        <tr>
                                            <td style="padding-top: 4px; width: 275px;">
                                                <asp:Label ID="lblStrawman" runat="server" Text='<%# Eval("Name") %>'>
                                                </asp:Label>
                                                <asp:HiddenField ID="hdnPersonId" runat="server" Value='<%# Eval("Id") %>' />
                                            </td>
                                            <td style="padding-top: 4px; padding-right: 5px; width: 40px;">
                                                <asp:DropDownList ID="ddlQuantity" runat="server" DataTextField="Name" DataValueField="Id">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chkEnabled" runat="server" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <tr style="background-color: #f9faff;">
                                            <td style="padding-top: 4px; width: 275px;">
                                                <asp:Label ID="lblStrawman" runat="server" Text='<%# Eval("Name") %>'>
                                                </asp:Label>
                                                <asp:HiddenField ID="hdnPersonId" runat="server" Value='<%# Eval("Id") %>' />
                                            </td>
                                            <td style="padding-top: 4px; padding-right: 5px; width: 40px;">
                                                <asp:DropDownList ID="ddlQuantity" runat="server" DataTextField="Name" DataValueField="Id">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chkEnabled" runat="server" />
                                            </td>
                                        </tr>
                                    </AlternatingItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                        <div style="text-align: right; width: 356px; padding: 8px 0px 8px 0px">
                            <input type="button" value="Clear All" onclick="javascript:ClearTeamStructure();" />
                        </div>
                        <br />
                        <table width="356px;">
                            <tr>
                                <td align="right">
                                    <input type="button" id="btnSaveTeamStructure" value="Save" onclick="javascript:saveTeamStructure();" />
                                    &nbsp;
                                    <asp:Button ID="btnTeamCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:HiddenField ID="hdnTeamStructure" runat="server" Value="" />
        <asp:Button ID="btnSaveTeamStructureHidden" runat="server" OnClick="btnSaveTeamStructureHidden_OnClick"
            Style="display: none;" />
    </ContentTemplate>
</asp:UpdatePanel>
<asp:ValidationSummary ID="valsum" ValidationGroup="Notes" runat="server" />

