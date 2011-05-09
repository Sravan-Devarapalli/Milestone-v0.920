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
<script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script src="Scripts/jquery.blockUI.js" type="text/javascript"></script>
<script type="text/javascript" language="javascript">
    var currenthdnProposedPersonsIndexesId = "";

    function ShowPotentialResourcesModal(image) {
        var refreshLableParentNode = image.parentNode.parentNode.children[0];
        refreshLableParentNode.children[refreshLableParentNode.children.length - 1].style.display = "";
        var hdnCurrentOpportunityId = document.getElementById('<%=hdnCurrentOpportunityId.ClientID %>');
        var attachedResourcesIndexes = image.parentNode.children[1].value.split(",");
        currenthdnProposedPersonsIndexesId = image.parentNode.children[1].id;
        if (hdnCurrentOpportunityId.value != image.attributes["OpportunityId"].value) {
            var chkboxList = document.getElementById('<%=cblPotentialResources.ClientID %>');
            var chkboxes = chkboxList.getElementsByTagName('input');
            $find("wmBhOutSideResources").set_Text(image.parentNode.children[2].value);
            for (var i = 0; i < chkboxes.length; i++) {
                chkboxes[i].checked = false;
                for (var j = 0; j < attachedResourcesIndexes.length; j++) {
                    if (i == attachedResourcesIndexes[j] && attachedResourcesIndexes[j] != '') {
                        chkboxes[i].checked = true;
                        break;
                    }
                }
            }
        }
        hdnCurrentOpportunityId.value = image.attributes["OpportunityId"].value;
        $.blockUI({ message: $('#divPotentialResources'), css: { width: '362px', top: '20%'} });
        return false;
    }

    function saveProposedResources() {

        var buttonSave = document.getElementById('<%=btnSaveProposedResourcesHidden.ClientID %>');
        var chkboxList = document.getElementById('<%=cblPotentialResources.ClientID %>');
        var hdnProposedResourceIndexes = document.getElementById('<%=hdnProposedResourceIndexes.ClientID %>');
        var hdnProposedOutSideResources = document.getElementById('<%=hdnProposedOutSideResources.ClientID %>');
        var chkboxes = chkboxList.getElementsByTagName('input');
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
        $.unblockUI();
        buttonSave.click();
    }
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
    function endRequestHandle(sender, Args) {
        if (currenthdnProposedPersonsIndexesId != '') {
            var hdnProposedPersonsIndexes = document.getElementById(currenthdnProposedPersonsIndexesId);
            var refreshLableParentNode = hdnProposedPersonsIndexes.parentNode.parentNode.children[0];
            refreshLableParentNode.children[refreshLableParentNode.children.length - 1].style.display = "";
        }
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
                            <td width="4%">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnNumberSort" runat="server" Text="Opp. #" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="Number" />
                                </div>
                            </td>
                            <td width="4%">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnPrioritySort" runat="server" Text="Priority" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="Priority" />
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
                            <td align="left" width="10%">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnEstimatedRevenue" runat="server" Text="Est. Revenue" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="EstimatedRevenue" />
                                </div>
                            </td>
                            <td align="center" width="28%">
                                <div class="ie-bg no-wrap">
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
                        <td align="left">
                            <div class="cell-pad">
                                <%# Eval("OpportunityNumber") %>
                            </div>
                        </td>
                        <td align="center">
                            <div class="cell-pad">
                                <asp:Label ID="lblPriority" runat="server" Text='<%# Eval("Priority") %>' /></div>
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
                                                    <%# Eval("PersonLastFirstName")%></font>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <div style="white-space: normal;">
                                                <asp:Literal ID="ltrlOutSideResources" runat="server"></asp:Literal>
                                            </div>
                                            <asp:Label ID="lblRefreshMessage" runat="server" Text="Please refresh to see new changes."
                                                Style="display: none; font-style: italic;"></asp:Label>
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <img src="Images/People_icon.png" alt="people" onclick="ShowPotentialResourcesModal(this);"
                                                style="cursor: pointer;" opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnProposedPersonsIndexes" runat="server" />
                                            <asp:HiddenField ID="hdnOutSideResources" runat="server" />
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
                        <td align="left">
                            <div class="cell-pad">
                                <%# Eval("OpportunityNumber") %>
                            </div>
                        </td>
                        <td align="center">
                            <div class="cell-pad">
                                <asp:Label ID="lblPriority" runat="server" Text='<%# Eval("Priority") %>' /></div>
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
                                                    <%# Eval("PersonLastFirstName")%></font>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <div style="white-space: normal;">
                                                <asp:Literal ID="ltrlOutSideResources" runat="server"></asp:Literal>
                                            </div>
                                            <asp:Label ID="lblRefreshMessage" runat="server" Text="Please refresh to see new changes."
                                                Style="display: none; font-style: italic;"></asp:Label>
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <img src="Images/People_icon.png" alt="people" style="cursor: pointer;" onclick="ShowPotentialResourcesModal(this);"
                                                opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnProposedPersonsIndexes" runat="server" />
                                            <asp:HiddenField ID="hdnOutSideResources" runat="server" />
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
        <div id="divPotentialResources" style="cursor: default; padding: 4px; background-color: #d4dff8;
            display: none">
            <center>
                <b>Potential Resources</b>
            </center>
            <div class="cbfloatRight" style="height: 250px; width: 350px; overflow-y: scroll;
                border: 1px solid black; background: white; padding-left: 3px; text-align: left !important;">
                <asp:CheckBoxList ID="cblPotentialResources" runat="server" Height="250px" Width="100%"
                    BackColor="White" AutoPostBack="false" DataTextField="Name" DataValueField="id"
                    CellPadding="3">
                </asp:CheckBoxList>
            </div>
            <br />
            <asp:TextBox ID="txtOutSideResources" runat="server" Width="97%" Height="18px" Style="padding-bottom: 4px;
                margin-bottom: 4px;" MaxLength="4000"></asp:TextBox>
            <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmOutSideResources" runat="server"
                TargetControlID="txtOutSideResources" WatermarkText="Enter Other Name(s) here (optional), seperated by semi-colon."
                EnableViewState="false" WatermarkCssClass="watermarkedtext" BehaviorID="wmBhOutSideResources" />
            <br />
            <table width="350px;">
                <tr>
                    <td align="right">
                        <input type="button" value="Save" onclick="javascript:saveProposedResources();" />
                        &nbsp;
                        <input type="button" value="Cancel" onclick="javascript:$.unblockUI();" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:HiddenField ID="hdnCurrentOpportunityId" runat="server" Value="" />
        <asp:HiddenField ID="hdnProposedResourceIndexes" runat="server" Value="" />
        <asp:HiddenField ID="hdnProposedOutSideResources" runat="server" Value="" />
        <asp:Button ID="btnSaveProposedResourcesHidden" runat="server" OnClick="btnSaveProposedResources_OnClick"
            Style="display: none;" />
    </ContentTemplate>
</asp:UpdatePanel>
<asp:ValidationSummary ID="valsum" ValidationGroup="Notes" runat="server" />

