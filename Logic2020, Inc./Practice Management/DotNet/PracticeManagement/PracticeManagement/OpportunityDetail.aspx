<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="OpportunityDetail.aspx.cs" Inherits="PraticeManagement.OpportunityDetail"
    Title="Practice Management - Opportunity Details" %>

<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register Src="~/Controls/Opportunities/OpportunityTransitionEditor.ascx" TagPrefix="uc"
    TagName="OpportunityTransitionEditor" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/Controls/Configuration/DefaultUser.ascx" TagPrefix="uc" TagName="DefaultUser" %>
<%@ Register Src="~/Controls/Opportunities/PrevNextOpportunity.ascx" TagPrefix="uc"
    TagName="PrevNextOpportunity" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Opportunities.ViewProjectExtender" %>
<%@ Register Src="~/Controls/Generic/Notes.ascx" TagName="Notes" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/RecentNotes.ascx" TagName="RecentNotes" TagPrefix="uc" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Opportunity Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Opportunity Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <style type="text/css">
        .alignRight
        {
            text-align: right;
        }
    </style>
    <script type="text/javascript">
        function checkDirty(arg) {
            __doPostBack('__Page', arg);
            return true;
        }
    </script>
    <uc:PrevNextOpportunity ID="prevNext" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <asp:Label ID="lblReadOnlyWarning" runat="server" ForeColor="Red" Visible="false">Since you are not the designated owner of this opportunity, you will not be able to make any changes.</asp:Label>
            <table class="opportunity-description">
                <tr>
                    <td style="width: 150px">
                        Number
                    </td>
                    <td style="width: 400px">
                        <asp:Label ID="lblOpportunityNumber" runat="server" />
                        &nbsp;(last updated:
                        <asp:Label ID="lblLastUpdate" runat="server" />)
                    </td>
                    <td style="width: 5px">
                    </td>
                    <td style="width: 120px">
                        Status
                    </td>
                    <td class="style1" style="padding-right: 4px;">
                        <asp:DropDownList ID="ddlStatus" runat="server" onchange="setDirty();" CssClass="WholeWidth">
                        </asp:DropDownList>
                    </td>
                    <td style="width: 20px">
                        <asp:RequiredFieldValidator ID="reqStatus" runat="server" ControlToValidate="ddlStatus"
                            ErrorMessage="The Status is required." ToolTip="The Status is required." Text="*"
                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="custWonConvert" runat="server" Text="*" ErrorMessage="Cannot convert an opportunity with the status Won to project."
                            ValidationGroup="WonConvert" OnServerValidate="custWonConvert_OnServerValidate" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Name
                    </td>
                    <td>
                        <asp:TextBox ID="txtOpportunityName" runat="server" onchange="setDirty();" MaxLength="50"
                            Width="400px"></asp:TextBox>
                    </td>
                    <td style="width: 5px">
                        <asp:RequiredFieldValidator ID="reqOpportunityName" runat="server" ControlToValidate="txtOpportunityName"
                            ErrorMessage="The Opportunity Name is required." ToolTip="The Opportunity Name is required."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ValidationGroup="Opportunity" />
                    </td>
                    <td>
                        Priority
                    </td>
                    <td class="style1" style="padding-right: 4px;">
                        <asp:DropDownList ID="ddlPriority" runat="server" CssClass="WholeWidth" onchange="setDirty();">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="A" Value="A"></asp:ListItem>
                            <asp:ListItem Text="B" Value="B"></asp:ListItem>
                            <asp:ListItem Text="C" Value="C"></asp:ListItem>
                            <asp:ListItem Text="D" Value="D"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqPriority" runat="server" ControlToValidate="ddlPriority"
                            Display="Dynamic" EnableClientScript="false" ErrorMessage="The Priority is required."
                            SetFocusOnError="true" Text="*" ToolTip="The Priority is required." ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        Client
                    </td>
                    <td style="width: 350px">
                        <asp:DropDownList ID="ddlClient" runat="server" onchange="setDirty();" CssClass="WholeWidth" />
                        <ajax:CascadingDropDown ID="cddClientGroups" runat="server" ParentControlID="ddlClient"
                            TargetControlID="ddlClientGroup" Category="Group" LoadingText="Loading Groups..."
                            EmptyText="No Groups found" EmptyValue="-1" ScriptPath="Scripts/CascadingDropDownBehavior.js"
                            ServicePath="CompanyPerfomanceServ.asmx" ServiceMethod="GetDdlProjectGroupContents"
                            UseContextKey="true" PromptText=" " PromptValue="-1" />
                    </td>
                    <td style="width: 5px">
                        <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="ddlClient"
                            ErrorMessage="The Client is required." ToolTip="The Client is required." Text="*"
                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity" />
                    </td>
                    <td>
                        Group
                    </td>
                    <td class="style1" style="padding-right: 4px;">
                        <asp:DropDownList ID="ddlClientGroup" runat="server" onchange="setDirty();" DataTextField="Name"
                            DataValueField="Id" CssClass="WholeWidth">
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        Salesperson
                    </td>
                    <td style="width: 350px">
                        <asp:DropDownList ID="ddlSalesperson" runat="server" onchange="setDirty();" CssClass="WholeWidth">
                        </asp:DropDownList>
                    </td>
                    <td style="width: 5px">
                    </td>
                    <td>
                        Owner
                    </td>
                    <td class="style1" style="padding-right: 4px;">
                        <uc:DefaultUser ID="dfOwner" runat="server" InsertFirtItem="true" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        Buyer Last Name
                    </td>
                    <td>
                        <asp:TextBox ID="txtBuyerName" runat="server" onchange="setDirty();" MaxLength="100"
                            Width="400px"></asp:TextBox>
                    </td>
                    <td style="width: 5px">
                        <asp:RequiredFieldValidator ID="reqBuyerName" runat="server" ControlToValidate="txtBuyerName"
                            ErrorMessage="The Buyer Name is required." ToolTip="The Buyer Name is required."
                            Text="*" SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="valregBuyerName" runat="server" ControlToValidate="txtBuyerName"
                            ErrorMessage="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                            ToolTip="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                            ValidationGroup="Opportunity" Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Dynamic" ValidationExpression="^[a-zA-Z'\-]{2,30}$"></asp:RegularExpressionValidator>
                    </td>
                    <td>
                        Practice Area
                    </td>
                    <td class="style1" style="padding-right: 4px;">
                        <asp:DropDownList ID="ddlPractice" runat="server" onchange="setDirty();" CssClass="WholeWidth">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                            ErrorMessage="The Practice Area is required." ToolTip="The Practice Area is required."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td style="width: 5px">
                    </td>
                    <td>
                        <table width="100%">
                            <tr style="width: 100%;">
                                <td style="text-align: left; white-space: nowrap;">
                                    Est. Revenue
                                </td>
                                <td style="text-align: right; white-space: nowrap;">
                                    $
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 350px; white-space: nowrap;">
                        <asp:TextBox ID="txtEstRevenue" CssClass="alignRight" runat="server" onchange="setDirty();"
                            Width="97%"></asp:TextBox>
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarkEstRevenue" runat="server"
                            TargetControlID="txtEstRevenue" WatermarkText="Ex: 15000, minimum 1000" EnableViewState="false"
                            WatermarkCssClass="watermarkedtext" />
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqEstRevenue" runat="server" ControlToValidate="txtEstRevenue"
                            ErrorMessage="The Est. Revenue is required." ToolTip="The Est. Revenue is required."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="custEstimatedRevenue" runat="server" ControlToValidate="txtEstRevenue"
                            ErrorMessage="A number with 2 decimal digits is allowed for the Est. Revenue."
                            ToolTip="A number with 2 decimal digits is allowed for the Est. Revenue." Text="*"
                            EnableClientScript="false" SetFocusOnError="true" OnServerValidate="custEstimatedRevenue_ServerValidate"
                            Display="Dynamic" ValidationGroup="Opportunity"></asp:CustomValidator>
                        <asp:CustomValidator ID="custEstRevenue" runat="server" ControlToValidate="txtEstRevenue"
                            ErrorMessage="Est. Revenue minimum value should be 1000." ToolTip="Est. Revenue minimum value should be 1000."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                            OnServerValidate="custEstRevenue_ServerValidate" ValidationGroup="Opportunity" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 75px;">
                        Attach Project
                    </td>
                    <td colspan="5" style="padding-right: 4px;">
                        <asp:DropDownList ID="ddlProjects" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlProjects_OnSelectedIndexChanged"
                            onchange="setDirty();" DataTextField="DetailedProjectTitle" DataValueField="Id"
                            AutoPostBack="false" Style="width: 84%">
                            <asp:ListItem Selected="True" Value="-1">Select project...</asp:ListItem>
                        </asp:DropDownList>
                        <ajax:CascadingDropDown ID="cddClientProjects" runat="server" ParentControlID="ddlClient"
                            TargetControlID="ddlProjects" Category="Project" LoadingText="Loading Projects..."
                            EmptyText="No projects found for this client" EmptyValue="-1" PromptText="Select project..."
                            PromptValue="-1" ScriptPath="Scripts/CascadingDropDownBehavior.js" ServicePath="CompanyPerfomanceServ.asmx"
                            ServiceMethod="GetProjects" UseContextKey="true" />
                        <asp:HyperLink ID="hlProject" runat="server" CssClass="tab-invisible" Style="float: right;
                            padding-right: 20px; padding-top: 3px;" Text="<%$ Resources:Controls, OpportunityDetail_Navigate_to_project %>" />
                        <ext:ViewProjectExtender ID="extProjectView" runat="server" EmptyValue="-1" ControlToShowProjectLinkID="hlProject"
                            TargetControlID="ddlProjects" />
                    </td>
                </tr>
            </table>
            <br />
            <ajax:TabContainer ID="tcOpportunityDetails" runat="server" CssClass="CustomTabStyle"
                ActiveTabIndex="0">
                <ajax:TabPanel ID="tpDescription" runat="server">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Description</span></a></span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="opportunity-panel">
                            <table class="WholeWidth">
                                <tr>
                                    <td colspan="5" style="text-align: right">
                                        <asp:CustomValidator ID="custOppDesciption" runat="server" ControlToValidate="txtDescription"
                                            Display="Dynamic" ErrorMessage="The opportunity description cannot be more than 2000 symbols"
                                            OnServerValidate="custOppDescription_ServerValidation" SetFocusOnError="True"
                                            ToolTip="The opportunity description cannot be more than 2000 symbols" ValidationGroup="Opportunity">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5" style="padding-right: 5px">
                                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="8" CssClass="WholeWidth"
                                            onchange="setDirty();" MaxLength="2000"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5">
                                        <b>Recent Notes</b>
                                    </td>
                                    <tr>
                                        <td colspan="5">
                                            <uc:Notes ID="nOpportunity" runat="server" Target="Opportunity" />
                                            <%--<uc:RecentNotes id="rnRecentNotest" runat="server" PeriodFilter="1" 
                                    SourceFilter="8" TransformScript="~/Reports/Xslt/RecentNoteItem.xslt" 
                                    OpportunityId="<%#Request.QueryString[PraticeManagement.Constants.QueryStringParameterNames.Id]%>" />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 32%">
                                            <b>Pipeline</b>
                                        </td>
                                        <td style="width: 2%">
                                        </td>
                                        <td style="width: 32%">
                                            <b>Proposed</b>
                                        </td>
                                        <td style="width: 2%">
                                        </td>
                                        <td style="width: 32%">
                                            <b>Send-Out</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <tr>
                                            <td colspan="5">
                                                <span style="color: gray">Select a person from a drop-down list below to add them to
                                                    a transition. Click a person in a list below to remove them from a transition.</span>
                                            </td>
                                        </tr>
                                        <td>
                                            <asp:TextBox ID="txtPipeline" runat="server" TextMode="MultiLine" Rows="7" CssClass="WholeWidth"
                                                onchange="setDirty();" Visible="False" />
                                            <uc:OpportunityTransitionEditor ID="otePipeline" runat="server" TransitionStatus="Pipeline" />
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtProposed" runat="server" TextMode="MultiLine" Rows="7" CssClass="WholeWidth"
                                                onchange="setDirty();" Visible="False" />
                                            <uc:OpportunityTransitionEditor ID="oteProposed" runat="server" TransitionStatus="Proposed" />
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSendOut" runat="server" TextMode="MultiLine" Rows="7" CssClass="WholeWidth"
                                                onchange="setDirty();" Visible="False" />
                                            <uc:OpportunityTransitionEditor ID="oteSendOut" runat="server" TransitionStatus="SendOut" />
                                        </td>
                                    </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </ajax:TabPanel>
                <ajax:TabPanel ID="tpHistory" runat="server" Visible="false">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>History</span></a></span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <uc:ActivityLogControl runat="server" ID="activityLog" DisplayDropDownValue="Opportunity"
                            DateFilterValue="Year" ShowDisplayDropDown="false" ShowProjectDropDown="false" />
                    </ContentTemplate>
                </ajax:TabPanel>
                <ajax:TabPanel ID="tpTools" runat="server" Visible="false">
                    <HeaderTemplate>
                        <span class="bg"><a href="#"><span>Tools</span></a></span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <table>
                            <tr style="height: 30px;">
                                <td>
                                    Projected Start&nbsp;&nbsp;
                                </td>
                                <td>
                                    <uc1:DatePicker ID="dpProjectedStartDate" runat="server" ValidationGroup="Opportunity"
                                        TextBoxWidth="120px" />
                                </td>
                                <td style="width: 30px">
                                    <asp:RequiredFieldValidator ID="reqProjectedStartDate" runat="server" ControlToValidate="dpProjectedStartDate"
                                        ErrorMessage="The Projected Start is required." ToolTip="The Projected Start is required."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        ValidationGroup="WonConvert"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rngProjectStartDate" runat="server" ControlToValidate="dpProjectedStartDate"
                                        ErrorMessage="The Projected Start should be in the range of 01/01/1985 and 12/31/2100."
                                        ToolTip="date should be in gioven range" Display="Dynamic" EnableClientScript="false"
                                        SetFocusOnError="true" Text="*" ValidationGroup="WonConvert" MinimumValue="1/1/1985"
                                        MaximumValue="12/31/2100" Type="Date"></asp:RangeValidator>
                                </td>
                            </tr>
                            <tr style="height: 30px;">
                                <td>
                                    Projected End&nbsp;&nbsp;
                                </td>
                                <td>
                                    <uc1:DatePicker ID="dpProjectedEndDate" runat="server" ValidationGroup="Opportunity"
                                        TextBoxWidth="120px" />
                                </td>
                                <td style="width: 30px">
                                    <asp:CompareValidator ID="compProjectedEndDate" runat="server" ControlToValidate="dpProjectedEndDate"
                                        ControlToCompare="dpProjectedStartDate" ErrorMessage="The Projected End must be greater or equal to the Projected Start."
                                        ToolTip="The Projected End must be greate or equals to the Projected Start."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        Operator="GreaterThanEqual" Type="Date" ValidationGroup="Opportunity"></asp:CompareValidator><asp:RequiredFieldValidator
                                            ID="reqProjectedEndDate" runat="server" ControlToValidate="dpProjectedEndDate"
                                            ErrorMessage="The Projected End is required." ToolTip="The Projected End is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            ValidationGroup="WonConvert"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rngProjectEndDate" runat="server" ControlToValidate="dpProjectedEndDate"
                                        ErrorMessage="The Projected End should be in the range of 01/01/1985 and 12/31/2100."
                                        ToolTip="date should be in given range" Display="Dynamic" EnableClientScript="false"
                                        SetFocusOnError="true" Text="*" Type="Date" ValidationGroup="WonConvert" MinimumValue="1/1/1985"
                                        MaximumValue="12/31/2100"></asp:RangeValidator>
                                </td>
                            </tr>
                            <tr>
                            </tr>
                            <tr style="height: 30px;">
                                <td colspan="3">
                                    <asp:Button ID="btnConvertToProject" runat="server" Text="Won - Convert to Projected Project"
                                        OnClick="btnConvertToProject_Click" OnClientClick="if (!confirmSaveDirty(true)) return false;" />&nbsp;
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajax:TabPanel>
            </ajax:TabContainer>
            <br />
            <div style="padding-bottom: 8px;">
                <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
            </div>
            <asp:HiddenField ID="hdnOpportunityId" runat="server" />
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />&nbsp;
            <cc:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />
            <asp:CustomValidator ID="custOpportunityNotSaved" runat="server" ErrorMessage="The opportunity must be saved at first."
                ToolTip="The opportunity must be saved at first." EnableClientScript="false"
                EnableViewState="false"></asp:CustomValidator>
            <asp:ValidationSummary ID="vsumOpportunity" runat="server" ValidationGroup="Opportunity"
                EnableClientScript="false" HeaderText="Unable to save opportunity due to the following errors:"
                DisplayMode="BulletList" />
            <asp:ValidationSummary ID="vsumOpportunityTransition" runat="server" ValidationGroup="OpportunityTransition"
                DisplayMode="BulletList" EnableClientScript="false" HeaderText="Unable to proceed with opportunity transition due to the following errors:" />
            <asp:ValidationSummary ID="vsumWonConvert" runat="server" ValidationGroup="WonConvert"
                DisplayMode="BulletList" EnableClientScript="false" HeaderText="Unable to convert opportunity due to the following errors:" />
            <asp:Literal ID="ltrWonConvertInvalid" runat="server" EnableViewState="false" Visible="false"
                Mode="PassThrough">
		<script type="text/javascript">
		    alert('{0}');
		</script> </asp:Literal>
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" DisplayText="Saving..." />
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .style1
        {
            width: 450px;
        }
        .transitions
        {
            height: 100px;
        }
    </style>
</asp:Content>

