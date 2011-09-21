<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="OpportunitySearch.aspx.cs" Inherits="PraticeManagement.OpportunitySearch" %>

<%@ PreviousPageType TypeName="PraticeManagement.Controls.PracticeManagementSearchPageBase" %>
<%@ Register Src="Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/Generic/OpportunityList.ascx" TagName="OpportunityList"
    TagPrefix="uc2" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Opportunity Search Results | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Opportunity Search Results
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function ChangeDefaultFocus(e) {

            if (window.event) {
                e = window.event;
            }
            if (e.keyCode == 13) {
                var btn = document.getElementById('<%= btnSearch.ClientID %>');
                btn.click();
            }

        }
    </script>
    <asp:UpdatePanel ID="pnlBody" runat="server">
        <ContentTemplate>
            <div class="project-filter" style="background: #E2EBFF; margin-bottom: 10px; padding: 5px;">
                <table class="WholeWidth">
                    <tbody>
                        <tr>
                            <td style="padding-right: 8px;">
                                <asp:TextBox ID="txtSearchText" runat="server" onkeypress="ChangeDefaultFocus(event);"
                                    CssClass="WholeWidth" MaxLength="255">
                                </asp:TextBox>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                    ErrorMessage="The Search Text is required." ToolTip="The Search Text is required."
                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 55px;">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click"
                                    UseSubmitBehavior="false" Width="55" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:ValidationSummary ID="vsumProjectSearch" runat="server" EnableClientScript="false" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <uc2:OpportunityList ID="ucOpportunityList" OnFilterOptionsChanged="ucOpportunityList_OnFilterOptionsChanged"
                runat="server" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" />
        </Triggers>
    </asp:UpdatePanel>
    <uc1:LoadingProgress ID="loadingProgress" runat="server" />
</asp:Content>

