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
<script type="text/javascript" language="javascript">
    var PrevOptyId = 0;
    function SetPrevOptyIdtoZero() {
        PrevOptyId = 0;
    }

    function RaisebtnOpportunity_Click(Control) {
        var button = Control.children[8].children[0].children[0].children[0].children[0].children[1].children[1];
        var currentOptyId = button.value;
        if (currentOptyId != PrevOptyId || PrevOptyId == 0) {
            PrevOptyId = currentOptyId;
            button.click();
        }
    }

    function SetCursorToTextEnd(textControlID) {
        var text = document.getElementById(textControlID);
        if (text != null && text.value.length > 0) {
            if (text.createTextRange) {
                var FieldRange = text.createTextRange();
                FieldRange.moveStart('character', text.value.length);
                FieldRange.collapse();
                FieldRange.select();
            }
        }
    }

    function btnDelete_OnClientClick(obj) {
        if (obj.children[0].children[0].attributes["NoteId"] != null && obj.children[0].children[0].attributes["NoteId"].value != "") {
            return true;
        }
        else {
            obj.children[0].children[0].value = "";
            obj.children[0].children[0].attributes.MyDirty = "false";
            return false;
        }
    }

    function btnSave_OnClientClick(obj) {
        if (obj.children[0].children[0].MyDirty != null && obj.children[0].children[0].MyDirty == "true") {
            return true;
        }
        else {
            return false;
         }
    }

    function pageLoad() {
        $find('behavior')._onSubmit = function () {
        };
    }
</script>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div id="opportunity-list">
            <asp:ListView ID="lvOpportunities" runat="server" DataKeyNames="Id" EnableViewState="true"
                OnSorting="lvOpportunities_Sorting">
                <LayoutTemplate>
                    <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">
                        <tr runat="server" id="lvHeader" class="CompPerfHeader">
                            <td width="1%">
                                <div class="ie-bg no-wrap">
                                </div>
                            </td>
                            <td width="4%" onclick="SetPrevOptyIdtoZero();">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnNumberSort" runat="server" Text="Opp. #" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="Number" />
                                </div>
                            </td>
                            <td width="4%" onclick="SetPrevOptyIdtoZero();">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnPrioritySort" runat="server" Text="Priority" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="Priority" />
                                </div>
                            </td>
                            <td width="13%" onclick="SetPrevOptyIdtoZero();">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnClientNameSort" runat="server" Text="Client - Group" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="ClientName" />
                                </div>
                            </td>
                            <td width="9%" onclick="SetPrevOptyIdtoZero();">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnBuyerNameSort" runat="server" Text="Buyer Name" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="BuyerName" />
                                </div>
                            </td>
                            <td width="24%" onclick="SetPrevOptyIdtoZero();">
                                <div class="ie-bg no-wrap" style="white-space: nowrap">
                                    <asp:LinkButton ID="btnOpportunityNameSort" runat="server" Text="Opportunity Name"
                                        CommandName="Sort" CssClass="arrow" CommandArgument="OpportunityName" />
                                </div>
                            </td>
                            <td width="7%" onclick="SetPrevOptyIdtoZero();">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnSalespersonSort" runat="server" Text="Sales Team" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="Salesperson" />
                                </div>
                            </td>
                            <td align="left" width="10%" onclick="SetPrevOptyIdtoZero();">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnEstimatedRevenue" runat="server" Text="Est. Revenue" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="EstimatedRevenue" />
                                </div>
                            </td>
                            <td align="center" width="28%">
                                <div class="ie-bg no-wrap">
                                    Note
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
                                <asp:LinkButton ID="lnkNumber" runat="server" Text='<%# Eval("OpportunityNumber") %>'
                                    OnClick="btnOppotunity_Click" /></div>
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
                                            <asp:TextBox ID="txtNote" Wrap="true" Width="96%" OpportunityId='<%# (int) Eval("Id") %>'
                                                onchange="this.MyDirty='true';" NoteId='<%# GetNoteId((int) Eval("Id")) %>'
                                                Text='<%# GetNoteText((int) Eval("Id")) %>' TextMode="MultiLine" Height="45px"
                                                Rows="3" Style="overflow-y: hidden; font-size: 12px; resize: none; font-family: Arial, Helvetica;"
                                                runat="server"></asp:TextBox>
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarker" runat="server" TargetControlID="txtNote"
                                                BehaviorID="behavior" WatermarkText="To add a note, click here and begin typing.Click save button to save your note."
                                                EnableViewState="false" WatermarkCssClass="waterMarkItalic" />
                                            <asp:CustomValidator ID="cvLen" runat="server" Text="*" ErrorMessage="Maximum length of the Note is 2000 characters."
                                                ClientValidationFunction="javascript:len=args.Value.length;args.IsValid=(len>0 && len<=2000);"
                                                OnServerValidate="cvLen_OnServerValidate" EnableClientScript="true" ControlToValidate="txtNote"
                                                ValidationGroup="Notes" Display="Dynamic" />
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/Images/icon-check.png" ToolTip="Save Note"
                                                OnClientClick="return btnSave_OnClientClick(this.parentNode.parentNode);" OnClick="btnSave_OnClick" ValidationGroup="Notes" />
                                            <asp:Button ID="btnOpportunity" Style="display: none;" Text='<%# (int) Eval("Id") %>'
                                                Width="0%" runat="server" OnClick="btnOppotunity_Click" />
                                            <asp:ImageButton ID="imgbtnDelete" runat="server" ImageUrl="~/Images/trash-icon.gif" OnClientClick="return btnDelete_OnClientClick(this.parentNode.parentNode);"
                                              ToolTip="Delete Note" OnClick="btnDelete_OnClick" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr id="trOpportunity" runat="server">
                        <td>
                            <div class="cell-pad">
                                <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                                    ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' ButtonCssClass='<%#PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClassByStatus((string)Eval("Status.Name"))%>' />
                            </div>
                        </td>
                        <td align="left">
                            <div class="cell-pad">
                                <asp:LinkButton ID="lnkNumber" runat="server" Text='<%# Eval("OpportunityNumber") %>'
                                    OnClick="btnOppotunity_Click" /></div>
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
                                            <asp:TextBox ID="txtNote" Wrap="true" Width="96%" OpportunityId='<%# (int) Eval("Id") %>'
                                                onchange="this.MyDirty='true';" NoteId='<%# GetNoteId((int) Eval("Id")) %>'
                                                Text='<%# GetNoteText((int) Eval("Id")) %>' TextMode="MultiLine" Height="45px"
                                                Rows="3" Style="overflow-y: hidden; resize: none; font-size: 12px; font-family: Arial, Helvetica;"
                                                runat="server"></asp:TextBox>
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarker" runat="server" TargetControlID="txtNote"
                                                BehaviorID="behavior" WatermarkText="To add a note, click here and begin typing.Click save button to save your note."
                                                EnableViewState="false" WatermarkCssClass="waterMarkItalic" />
                                            <asp:CustomValidator ID="cvLen" runat="server" Text="*" ErrorMessage="Maximum length of the Note is 2000 characters."
                                                ClientValidationFunction="javascript:len=args.Value.length;args.IsValid=(len>0 && len<=2000);"
                                                OnServerValidate="cvLen_OnServerValidate" EnableClientScript="true" ControlToValidate="txtNote"
                                                ValidationGroup="Notes" Display="Dynamic" />
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/Images/icon-check.png" ToolTip="Save Note"
                                              OnClientClick="return btnSave_OnClientClick(this.parentNode.parentNode);"   OnClick="btnSave_OnClick" ValidationGroup="Notes" />
                                            <asp:Button ID="btnOpportunity" Style="display: none;" Text='<%# (int) Eval("Id") %>'
                                                Width="0%" runat="server" OnClick="btnOppotunity_Click" />
                                            <asp:ImageButton ID="imgbtnDelete" runat="server" ImageUrl="~/Images/trash-icon.gif"
                                                 OnClientClick="return btnDelete_OnClientClick(this.parentNode.parentNode);"
                                                ToolTip="Delete Note" OnClick="btnDelete_OnClick" />
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
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel ID="updProposedResources" UpdateMode="Conditional" runat="server">
    <ContentTemplate>
        <table class="PaddingClass" width="100%">
            <uc:ProposedResources ID="ucProposedResources" runat="server" hintDateVisible="false" />
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:ValidationSummary ID="valsum" ValidationGroup="Notes" runat="server" />

