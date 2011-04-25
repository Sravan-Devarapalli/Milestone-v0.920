<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpportunityTransitionEditor.ascx.cs"
    Inherits="PraticeManagement.Controls.Opportunities.OpportunityTransitionEditor" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" %>
<asp:UpdatePanel ID="updOppTranstion" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="0" class="WholeWidth">
            <tr>
                <td>
                    <asp:DropDownList ID="ddlPersons" runat="server" DataSourceID="odsActivePersons"
                        DataValueField="Id" DataTextField="PersonLastFirstName" CssClass="WholeWidth"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlPersons_OnSelectedIndexChanged"
                        AppendDataBoundItems="true">
                        <asp:ListItem Value="">"Select a person to add them here</asp:ListItem>
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsActivePersons" runat="server" SelectMethod="PersonListAllShort"
                        TypeName="PraticeManagement.PersonService.PersonServiceClient"
                        EnableCaching="true" CacheDuration="5">
                        <SelectParameters>
                            <asp:Parameter Name="practice" Type="Int32" />
                            <asp:Parameter Name="statusId" Type="Int32" />
                            <asp:Parameter Name="startDate" Type="DateTime" />
                            <asp:Parameter Name="endDate" Type="DateTime" />
                        </SelectParameters>
                    </asp:ObjectDataSource>                    
                </td>
            </tr>
            <tr>
                <td>
                    <asp:ListBox ID="lbTransistions" runat="server" DataSourceID="odsTransitions" DataValueField="Id"
                        DataTextField="TransitionText" CssClass="WholeWidth" Height="100" AutoPostBack="true"
                        OnSelectedIndexChanged="lbTransistions_OnSelectedIndexChanged" SelectionMode="Multiple"
                        /> <%-- onchange="return confirm('Are you sure you want to remove this person from transition?')"  --%>
                    <asp:ObjectDataSource ID="odsTransitions" runat="server" SelectMethod="GetOpportunityTransitions"
                        TypeName="PraticeManagement.OpportunityService.OpportunityServiceClient">
                        <SelectParameters>
                            <asp:Parameter Name="opportunityId" />
                            <asp:Parameter Name="statusType" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>
        </table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ddlPersons" EventName="SelectedIndexChanged" />
    </Triggers>
</asp:UpdatePanel>

