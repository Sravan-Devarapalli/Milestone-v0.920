<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="DiscussionReview1.aspx.cs" Inherits="PraticeManagement.DiscussionReview1" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/Controls/Configuration/DefaultUser.ascx" TagPrefix="uc" TagName="DefaultUser" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Opportunities.ViewProjectExtender" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %> 
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Opportunities/ProposedResources.ascx" TagName="ProposedResources"
    TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Opportunities Discussion Review 1</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .transitions
        {
            height: 100px;
        }
        .position
        {
            position: absolute;
        }
        .watermark-Text
        {
            font-style: italic;
            color: Gray;
        }
        .noteWidthHeight
        {
            overflow-y: auto;
            width: 100%;
            height: 100px;
        }
        .black
        {
            color: Black !important;
        }
        
        .selectLink, .selectLink:hover
        {
            text-decoration: none;
            color: Black;
        }
        .FixedHeight
        {
            height: 15px;
            vertical-align: middle;
        }
        .NoteWidth
        {
            padding-right: 3px;
        }
        .InActive
        {
            background-color: InactiveBorder;
        }
        .WordWrap
        {
            word-wrap: break-word !important; /* Internet Explorer 5.5+ */
            word-break: break-all;
            white-space: normal;
        }
        .NoWrap
        {
            white-space: nowrap;
        }
        .lblNoNotes
        {
            background-color: White;
            text-align: left;
            width: 100%;
        }
        .RevenueTextWidth
        {
            width: 98%;
        }
        .LblNoteWidth
        {
            width: 200px;
        }
        
        image:hover:after
        {
            content: attr(title);
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(getEvents);
        function getEvents() {
            var divPriority = $("#<%= divPriorityDescription.ClientID %>");
            var imgPriorityHint = $("#<%= imgPriorityHint.ClientID %>");
            var lblClose = $("#<%= lblClose.ClientID %>");
            divPriority.hide();
            imgPriorityHint.click(function () {
                setHintPosition(imgPriorityHint, divPriority);
            });
            lblClose.click(function () {
                divPriority.hide();
            });
        }

        function setHintPosition(image, displayPanel) {
            displayPanel.show();
            var iptop = image.offset().top;
            var ipleft = image.offset().left;
            iptop = iptop + 10;
            ipleft = ipleft - 10;
            setPosition(displayPanel, iptop, ipleft);
        }

        function setPosition(item, ytop, xleft) {
            item.offset({ top: ytop, left: xleft });
        }

        function checkDirty(arg) {
            __doPostBack('__Page', arg);
            return true;
        }

        function EnableSaveButton() {
            var button = document.getElementById("<%= btnSave.ClientID%>");
            var hiddenField = document.getElementById("<%= hdnValueChanged.ClientID%>");

            if (button != null) {
                button.disabled = false;
                hiddenField.value = "true";
            }
        }

        function FadeOutLabel() {
            $("#" + "<%= lblSaved.ClientID%>").fadeOut(10000);
            document.getElementById("<%= lblSaved.ClientID%>").value = "";
            var divResultDescription = $("#" + "<%= divResultDescription.ClientID%>");
            setTimeout(function () {
                divResultDescription.hide();
            }, 10000);
        }
        function ScrollToImage(selectedIndex) {
            var divOppList = document.getElementById("<%= divOpportunityList.ClientID%>");
            var avgItemHeight = divOppList.scrollHeight / divOppList.children[0].children[0].children.length;
            var position = selectedIndex * avgItemHeight;
            $("#" + "<%= divOpportunityList.ClientID%>").scrollTop(position);
        }

        function ShowHomeTab() {
            var tabBehaviour = $get('<%= tcOpportunityDetails.ClientID%>').control;
            tabBehaviour.set_activeTabIndex(0);
            var lnkHistory = document.getElementById("<%= lnkHistory.ClientID%>");
            lnkHistory.setAttribute('clicked', 'false');
        }

        function lnkHistory_ClientClick(lnkbtnHistory) {
            if (lnkbtnHistory.attributes['clicked'] != null) {
                if (lnkbtnHistory.attributes['clicked'].value == 'true') {
                    return false;
                }
            }
            lnkbtnHistory.setAttribute('clicked', 'true');
            return true;

        }
    </script>
    <table class="CompPerfTable WholeWidth">
        <tr>
            <td colspan="2" style="background: #e2ebff; width: 100%; border-bottom: 1px solid black;">
                <asp:UpdatePanel ID="upTopBarPane" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <table style="width: 100%; background: #e2ebff; text-align: center;">
                            <tr align="center">
                                <td align="center" style="width: 100%; text-align: center;" colspan="4">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr align="center">
                                <td style="width: 30%; white-space: nowrap;">
                                    &nbsp;
                                </td>
                                <td align="center" style="width: 40%; text-align: center;">
                                    <table align="center" style="margin: 0px auto; text-align: center;">
                                        <tr align="center">
                                            <td style="white-space: nowrap; padding-right: 25px;">
                                                <asp:ImageButton ID="imgBtnFirst" Visible="false" runat="server" Style="z-index: 1;"
                                                    ImageUrl="~/Images/arrow-First.png" OnClientClick="ShowHomeTab();if (!confirmSaveDirty(true)) return false;"
                                                    OnClick="imgBtnFirst_OnClick" ToolTip="First Opportunity" />
                                                <asp:Image ID="imgFirst" runat="server" Visible="true" ImageUrl="~/Images/grayedArrow-First.png"
                                                    ToolTip="First Opportunity" />
                                            </td>
                                            <td style="white-space: nowrap; padding-right: 25px;">
                                                <asp:ImageButton ID="imgBtnPrevious" Visible="false" runat="server" Style="z-index: 1;"
                                                    ImageUrl="~/Images/arrow-Previous.png" OnClientClick="ShowHomeTab();if (!confirmSaveDirty(true)) return false;"
                                                    OnClick="imgBtnPrevious_OnClick" ToolTip="Previous Opportunity" />
                                                <asp:Image ID="imgPrevious" runat="server" Visible="true" ImageUrl="~/Images/grayedArrow-Previous.png"
                                                    ToolTip="Previous Opportunity" />
                                            </td>
                                            <td style="white-space: nowrap; padding-right: 25px;">
                                                <asp:Label ID="lblCurrentOpportunity" runat="server" Font-Size="Large" Font-Bold="true"></asp:Label>
                                                &nbsp; <font style="font-size: large;">of</font> &nbsp;
                                                <asp:Label ID="lblTotalOpportunities" Font-Size="Large" Font-Bold="true" runat="server"></asp:Label>
                                            </td>
                                            <td style="white-space: nowrap; padding-right: 25px;">
                                                <asp:ImageButton ID="imgBtnNext" runat="server" Style="z-index: 1;" ImageUrl="~/Images/arrow-Next.png"
                                                    OnClick="imgBtnNext_OnClick" Visible="true" ToolTip="Next Opportunity" OnClientClick="ShowHomeTab();if (!confirmSaveDirty(true)) return false;" />
                                                <asp:Image ID="imgNext" runat="server" Visible="false" ImageUrl="~/Images/grayedArrow-Next.png"
                                                    ToolTip="Next Opportunity" />
                                            </td>
                                            <td style="white-space: nowrap; padding-right: 25px;">
                                                <asp:ImageButton ID="imgBtnLast" runat="server" Style="z-index: 1;" ImageUrl="~/Images/arrow-Last.png"
                                                    OnClientClick="ShowHomeTab();if (!confirmSaveDirty(true)) return false;" OnClick="imgBtnLast_OnClick"
                                                    Visible="true" ToolTip="Last Opportunity" />
                                                <asp:Image ID="imgLast" runat="server" Visible="false" ImageUrl="~/Images/grayedArrow-Last.png"
                                                    ToolTip="Last Opportunity" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 23%; white-space: nowrap;">
                                </td>
                                <td align="center" style="width: 7%; white-space: nowrap; text-align: center; padding-right: 14px;">
                                    <div id="divResultDescription" runat="server" style="display: none; width: 100%;">
                                        <table style="background: yellow; width: 100%">
                                            <tr>
                                                <td style="padding: 1px; text-align: center; width: 100%;">
                                                    <asp:Label ID="lblSaved" ForeColor="Green" runat="server"></asp:Label>
                                                    <asp:Label ID="lblError" ForeColor="Red" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr align="center">
                                <td align="center" style="width: 100%; text-align: center;" colspan="4">
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="imgBtnFirst" />
                        <asp:AsyncPostBackTrigger ControlID="imgBtnPrevious" />
                        <asp:AsyncPostBackTrigger ControlID="imgBtnNext" />
                        <asp:AsyncPostBackTrigger ControlID="imgBtnLast" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td valign="top" id="columnOpportunityList" runat="server" style="border-right: 2px solid black;
                width: 28%;">
                <asp:UpdatePanel ID="upOpportunityList" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <div id="divOpportunityList" runat="server" style="overflow-y: scroll; height: 848px">
                            <asp:ListView ID="lvOpportunities" runat="server" SelectedIndex="0" DataKeyNames="Id"
                                OnSelectedIndexChanging="lvOpportunities_SelectedIndexChanging" OnDataBound="lvOpportunities_OnDataBound">
                                <LayoutTemplate>
                                    <table id="Table1" runat="server" class="CompPerfTable WholeWidth">
                                        <tr runat="server" id="itemPlaceHolder">
                                        </tr>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr style="border-bottom: 1px solid black; padding: 2px 0px 2px 0px;" onclick="ShowHomeTab();if (!confirmSaveDirty(true)) return false;">
                                        <td style="font-weight: bold; padding-right: 2px; padding-left: 2px; width: 5%;">
                                            <asp:LinkButton ID="lnkbtnPriority" CommandName="select" CssClass="selectLink" runat="server">
                                                <div class="WholeWidth FixedHeight" style="vertical-align: middle; padding: 10px 2px 10px 2px;">
                                                    <asp:Label ID="lblPriority" runat="server" Text='<%# Eval("Priority") %>'></asp:Label></div>
                                            </asp:LinkButton>
                                        </td>
                                        <td style="padding: 5px 0px 5px 0px; width: 95%;">
                                            <asp:LinkButton ID="lnkbtnInfo" CommandName="select" CssClass="selectLink" runat="server">
                                                <div class="WholeWidth">
                                                    <table class="WholeWidth">
                                                        <tr style="width: 100%;">
                                                            <td valign="top" style="width: 75%;">
                                                                <asp:LinkButton ID="LinkButton1" CommandName="select" CssClass="selectLink" runat="server">
                                                                    <asp:Label ID="lblClientName" Width="100%" runat="server" Text='<%# ((Opportunity) Container.DataItem).ClientAndGroup %>'></asp:Label>
                                                                </asp:LinkButton>
                                                            </td>
                                                            <td valign="top" align="right" style="width: 25%; text-align: right; padding-right: 6px">
                                                                <asp:LinkButton ID="LinkButton2" CommandName="select" CssClass="selectLink" runat="server">
                                                                    <asp:Label ID="lblEstRevenue" Width="100%" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>'></asp:Label>
                                                                </asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                        <tr style="width: 100%;">
                                                            <td colspan="2" style="width: 100%;">
                                                                <asp:LinkButton ID="LinkButton3" CommandName="select" CssClass="selectLink" runat="server">
                                                                    <asp:Label ID="lblOpportunityName" Width="100%" ToolTip='<%# Eval("Name") %>' runat="server"
                                                                        OpportunityID='<%# ((Opportunity) Container.DataItem).Id %>' Text='<%# GetTruncatedOpportunityName((string)Eval("Name")) %>'
                                                                        CssClass="NoWrap"></asp:Label>
                                                                </asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                        <tr style="width: 100%;">
                                                            <td colspan="2" style="width: 100%;">
                                                                <asp:LinkButton ID="LinkButton4" CommandName="select" CssClass="selectLink" runat="server">
                                                                    <asp:Label ID="lblSalesPersonName" runat="server" Text='<%# Eval("Salesperson.LastName") %>'></asp:Label>&nbsp;&nbsp;-&nbsp;&nbsp;
                                                                    <asp:Label ID="lblBuyerName" runat="server" Text='<%# Eval("BuyerName") %>'></asp:Label>
                                                                </asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <SelectedItemTemplate>
                                    <tr onclick="ShowHomeTab();if (!confirmSaveDirty(true)) return false;" style="background-color: #e2ebff;
                                        border-bottom: 1px solid black; padding: 2px 0px 2px 0px; width: 5%;">
                                        <td style="font-weight: bold; vertical-align: middle; padding-right: 4px; padding-left: 2px">
                                            <asp:Label ID="lblPriority" runat="server" Text='<%# Eval("Priority") %>'></asp:Label>
                                        </td>
                                        <td style="padding: 5px 0px 5px 0px; width: 95%;">
                                            <table class="WholeWidth">
                                                <tr style="width: 100%;">
                                                    <td valign="top" style="width: 75%;">
                                                        <asp:LinkButton ID="LinkButton1" CommandName="select" CssClass="selectLink" runat="server">
                                                            <asp:Label ID="lblClientName" Width="100%" runat="server" Text='<%# ((Opportunity) Container.DataItem).ClientAndGroup %>'></asp:Label>
                                                        </asp:LinkButton>
                                                    </td>
                                                    <td valign="top" align="right" style="width: 25%; text-align: right; padding-right: 6px">
                                                        <asp:LinkButton ID="LinkButton2" CommandName="select" CssClass="selectLink" runat="server">
                                                            <asp:Label ID="lblEstRevenue" Width="100%" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>'></asp:Label>
                                                        </asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr style="width: 100%;">
                                                    <td colspan="2" style="width: 100%;">
                                                        <asp:LinkButton ID="LinkButton3" CommandName="select" CssClass="selectLink" runat="server">
                                                            <asp:Label ID="lblOpportunityName" Width="100%" ToolTip='<%# Eval("Name") %>' runat="server"
                                                                OpportunityID='<%# ((Opportunity) Container.DataItem).Id %>' Text='<%# GetTruncatedOpportunityName((string)Eval("Name")) %>'
                                                                CssClass="NoWrap"></asp:Label>
                                                        </asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr style="width: 100%;">
                                                    <td colspan="2" style="width: 100%;">
                                                        <asp:LinkButton ID="LinkButton4" CommandName="select" CssClass="selectLink" runat="server">
                                                            <asp:Label ID="lblSalesPersonName" runat="server" Text='<%# Eval("Salesperson.LastName") %>'></asp:Label>&nbsp;&nbsp;-&nbsp;&nbsp;
                                                            <asp:Label ID="lblBuyerName" runat="server" Text='<%# Eval("BuyerName") %>'></asp:Label>
                                                        </asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </SelectedItemTemplate>
                                <EmptyDataTemplate>
                                    <tr>
                                        <td valign="middle">
                                            No Opportunities.
                                        </td>
                                    </tr>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancel" />
                        <asp:AsyncPostBackTrigger ControlID="imgBtnFirst" />
                        <asp:AsyncPostBackTrigger ControlID="imgBtnPrevious" />
                        <asp:AsyncPostBackTrigger ControlID="imgBtnNext" />
                        <asp:AsyncPostBackTrigger ControlID="imgBtnLast" />
                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td style="border-left: 2px solid black; padding-left: 5px; width: 72%;" valign="top">
                <div style="overflow-y: scroll; height:848px">
                    <table>
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="upOpportunityDetail" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:Label ID="lblReadOnlyWarning" runat="server" ForeColor="Red" Visible="false">Since you are not the designated owner of this opportunity, you will not be able to make any changes.</asp:Label>
                                        <table style="padding-left: 5px;" class="PaddingClass WholeWidth">
                                            <tr style="height: 30px;">
                                                <td style="width: 12%;">
                                                    Number
                                                </td>
                                                <td style="width: 38%">
                                                    <asp:Label ID="lblOpportunityNumber" runat="server" />
                                                    &nbsp;(last updated:
                                                    <asp:Label ID="lblLastUpdate" runat="server" />)
                                                </td>
                                                <td colspan="2" style="white-space: nowrap; width: 50%;">
                                                    <table cellpadding="4px;">
                                                        <tr>
                                                            <td style="padding-right: 4px;">
                                                                Start Date
                                                            </td>
                                                            <td class="DatePickerPadding" style="padding-left: 4px; padding-right: 4px;">
                                                                <uc1:DatePicker ID="dpStartDate" ValidationGroup="Opportunity" AutoPostBack="false"
                                                                    OnClientChange="EnableSaveButton();setDirty();" TextBoxWidth="62px" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpStartDate"
                                                                    ErrorMessage="Start date is required to add Proposed Resources to project." ToolTip="Start date is required to add Proposed Resources to project."
                                                                    ValidationGroup="HasPersons" Display="Dynamic" Text="*" EnableClientScript="false"></asp:RequiredFieldValidator>
                                                            </td>
                                                            <td style="padding-left: 4px; padding-right: 8px;">
                                                                End Date
                                                            </td>
                                                            <td class="DatePickerPadding">
                                                                <uc1:DatePicker ID="dpEndDate" ValidationGroup="Opportunity" AutoPostBack="false"
                                                                    OnClientChange="EnableSaveButton();setDirty();" TextBoxWidth="62px" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:RequiredFieldValidator ID="reqEndDate" runat="server" ControlToValidate="dpEndDate"
                                                                    ErrorMessage="End date is required to add Proposed Resources to project." ToolTip="End date is required to add Proposed Resources to project."
                                                                    ValidationGroup="HasPersons" Display="Dynamic" Text="*" EnableClientScript="false"></asp:RequiredFieldValidator>
                                                                <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpEndDate"
                                                                    ControlToCompare="dpStartDate" ErrorMessage="The Projected End must be greater or equal to the Projected Start."
                                                                    ToolTip="The Projected End must be greate or equals to the Projected Start."
                                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                                    Operator="GreaterThanEqual" Type="Date" ValidationGroup="Opportunity"></asp:CompareValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold; width: 12%">
                                                    Name
                                                </td>
                                                <td style="width: 38%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 97%">
                                                                <asp:TextBox ID="txtOpportunityName" runat="server" onchange="EnableSaveButton();setDirty();"
                                                                    MaxLength="50" Width="98%"></asp:TextBox>
                                                            </td>
                                                            <td style="width: 3%">
                                                                <asp:RequiredFieldValidator ID="reqOpportunityName" runat="server" ControlToValidate="txtOpportunityName"
                                                                    ToolTip="The Opportunity Name is required." Text="*" EnableClientScript="false"
                                                                    SetFocusOnError="true" Display="Dynamic" Width="100%" ValidationGroup="Opportunity" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="width: 12%; font-weight: bold;">
                                                    Status
                                                </td>
                                                <td style="width: 38% !important;">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 97%">
                                                                <asp:DropDownList ID="ddlStatus" Width="100%" runat="server" onchange="EnableSaveButton();setDirty();"
                                                                    CssClass="WholeWidth">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td style="width: 3%">
                                                                <asp:RequiredFieldValidator ID="reqStatus" runat="server" ControlToValidate="ddlStatus"
                                                                    Width="50%" ToolTip="The Status is required." Text="*" EnableClientScript="false"
                                                                    SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                                <asp:CustomValidator ID="custWonConvert" runat="server" Text="*" Width="50%" ErrorMessage="Cannot convert an opportunity with the status Won to project."
                                                                    ValidationGroup="WonConvert" OnServerValidate="custWonConvert_OnServerValidate" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr style="height: 30px;">
                                                <td style="font-weight: bold; width: 12%;">
                                                    Client
                                                </td>
                                                <td style="width: 38%;">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 97%">
                                                                <asp:DropDownList ID="ddlClient" Width="100%" runat="server" AutoPostBack="true"
                                                                    onchange="EnableSaveButton();setDirty();" OnSelectedIndexChanged="ddlClient_SelectedIndexChanged"
                                                                    CssClass="WholeWidth" />
                                                            </td>
                                                            <td style="width: 3%">
                                                                <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="ddlClient"
                                                                    Width="100%" ToolTip="The Client is required." Text="*" EnableClientScript="false"
                                                                    SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="font-weight: bold; width: 12%">
                                                    Priority
                                                    <asp:Image ID="imgPriorityHint" runat="server" ImageUrl="~/Images/hint.png" />
                                                </td>
                                                <td style="width: 38%;">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 97%">
                                                                <asp:DropDownList ID="ddlPriority" runat="server" Width="100%" CssClass="WholeWidth"
                                                                    onchange="EnableSaveButton();setDirty();">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td style="width: 3%">
                                                                <asp:RequiredFieldValidator ID="reqPriority" runat="server" ControlToValidate="ddlPriority"
                                                                    Width="100%" Display="Dynamic" EnableClientScript="false" SetFocusOnError="true"
                                                                    Text="*" ToolTip="The Priority is required." ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 12%;">
                                                    Group
                                                </td>
                                                <td style="width: 38%;">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 100%">
                                                                <asp:DropDownList ID="ddlClientGroup" Width="97%" runat="server" onchange="EnableSaveButton();setDirty();"
                                                                    DataTextField="Name" DataValueField="Id" CssClass="WholeWidth">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="width: 12%; padding-right: 0px !important;">
                                                    <table width="100%">
                                                        <tr style="width: 100%;">
                                                            <td style="font-weight: bold; padding-left: 1px; white-space: nowrap;">
                                                                Est. Revenue
                                                            </td>
                                                            <td style="text-align: right; white-space: nowrap;">
                                                                $
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="width: 38%; white-space: nowrap;">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 97%">
                                                                <asp:TextBox ID="txtEstRevenue" CssClass="alignRight" runat="server" onchange="EnableSaveButton();setDirty();"
                                                                    Width="98%"></asp:TextBox>
                                                                <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarkEstRevenue" runat="server"
                                                                    TargetControlID="txtEstRevenue" WatermarkText="Ex: 15000, minimum 1000" EnableViewState="false"
                                                                    WatermarkCssClass="watermarkedtext" />
                                                            </td>
                                                            <td style="width: 3%">
                                                                <asp:RequiredFieldValidator ID="reqEstRevenue" runat="server" ControlToValidate="txtEstRevenue"
                                                                    ToolTip="The Est. Revenue is required." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                                    Display="Dynamic" ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                                <asp:CustomValidator ID="custEstimatedRevenue" runat="server" ControlToValidate="txtEstRevenue"
                                                                    ToolTip="A number with 2 decimal digits is allowed for the Est. Revenue." Text="*"
                                                                    EnableClientScript="false" SetFocusOnError="true" OnServerValidate="custEstimatedRevenue_ServerValidate"
                                                                    Display="Dynamic" ValidationGroup="Opportunity"></asp:CustomValidator>
                                                                <asp:CustomValidator ID="custEstRevenue" runat="server" ControlToValidate="txtEstRevenue"
                                                                    ToolTip="Est. Revenue minimum value should be 1000." Text="*" EnableClientScript="false"
                                                                    SetFocusOnError="true" Display="Dynamic" OnServerValidate="custEstRevenue_ServerValidate"
                                                                    ValidationGroup="Opportunity" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr style="height: 30px;">
                                                <td style="font-weight: bold; width: 12%;">
                                                    Salesperson
                                                </td>
                                                <td style="width: 38%;">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 97%">
                                                                <asp:DropDownList ID="ddlSalesperson" Width="100%" runat="server" onchange="EnableSaveButton();setDirty();"
                                                                    CssClass="WholeWidth">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td style="width: 3%">
                                                                <asp:RequiredFieldValidator ID="reqSalesperson" runat="server" ControlToValidate="ddlSalesperson"
                                                                    Width="100%" Display="Dynamic" EnableClientScript="false" SetFocusOnError="true"
                                                                    Text="*" ToolTip="The Salesperson is required." ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="font-weight: bold; width: 12%;">
                                                    Owner
                                                </td>
                                                <td style="width: 38%;">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 97%">
                                                                <uc:DefaultUser ID="dfOwner" runat="server" InsertFirtItem="true" OnClientChange="EnableSaveButton();setDirty();" />
                                                            </td>
                                                            <td style="width: 3%">
                                                                <asp:CustomValidator ID="cvDfOwnerRequired" runat="server" Text="*" ToolTip="Owner is required."
                                                                    OnServerValidate="cvDfOwnerRequired_ServerValidate" Width="100%" ValidationGroup="Opportunity"
                                                                    SetFocusOnError="true" Display="Dynamic"></asp:CustomValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold; width: 12%;">
                                                    Buyer Last Name
                                                </td>
                                                <td style="width: 38%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 97%">
                                                                <asp:TextBox ID="txtBuyerName" runat="server" onchange="EnableSaveButton();setDirty();"
                                                                    MaxLength="100" Width="98%"></asp:TextBox>
                                                            </td>
                                                            <td style="width: 3%">
                                                                <asp:RequiredFieldValidator ID="reqBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                                    ToolTip="The Buyer Name is required." Text="*" SetFocusOnError="true" Display="Dynamic"
                                                                    ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="valregBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                                    ToolTip="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                                                                    ValidationGroup="Opportunity" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                                    Display="Dynamic" ValidationExpression="^[a-zA-Z'\-]{2,30}$"></asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="font-weight: bold; width: 12%;">
                                                    Practice
                                                </td>
                                                <td style="width: 38%;">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 97%">
                                                                <asp:DropDownList ID="ddlPractice" runat="server" onchange="EnableSaveButton();setDirty();"
                                                                    Width="100%" CssClass="WholeWidth">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td style="width: 3%">
                                                                <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                                                    ToolTip="The Practice is required." Width="100%" Text="*" EnableClientScript="false"
                                                                    SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                                        <asp:AsyncPostBackTrigger ControlID="btnCancel" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnFirst" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnPrevious" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnNext" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnLast" />
                                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <br style="height: 1px;" />
                                <ajax:TabContainer ID="tcOpportunityDetails" runat="server" CssClass="CustomTabStyle"
                                    ActiveTabIndex="0">
                                    <ajax:TabPanel ID="tpDescription" runat="server">
                                        <HeaderTemplate>
                                            <span class="bg"><a href="#"><span>Description</span></a></span>
                                        </HeaderTemplate>
                                        <ContentTemplate>
                                            <div style="width: 99%; height: 310px; padding-left: 4px; padding-right: 4px; overflow-y: auto;">
                                                <asp:UpdatePanel ID="upDescription" UpdateMode="Conditional" runat="server">
                                                    <ContentTemplate>
                                                        <table class="WholeWidth">
                                                            <tr>
                                                                <td style="text-align: right">
                                                                    <asp:CustomValidator ID="custOppDesciption" runat="server" ControlToValidate="txtDescription"
                                                                        Display="Dynamic" OnServerValidate="custOppDescription_ServerValidation" SetFocusOnError="True"
                                                                        ToolTip="The opportunity description cannot be more than 2000 symbols" ValidationGroup="Opportunity">*</asp:CustomValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="padding: 0px 5px 0px 4px;">
                                                                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" CssClass="WholeWidth"
                                                                        Height="80px" onchange="EnableSaveButton();setDirty();" MaxLength="2000" Style="overflow-y: auto;
                                                                        resize: none; font-size: 12px; font-family: Arial, Helvetica, sans-serif;"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnCancel" />
                                                        <asp:AsyncPostBackTrigger ControlID="imgBtnFirst" />
                                                        <asp:AsyncPostBackTrigger ControlID="imgBtnPrevious" />
                                                        <asp:AsyncPostBackTrigger ControlID="imgBtnNext" />
                                                        <asp:AsyncPostBackTrigger ControlID="imgBtnLast" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                                <asp:UpdatePanel ID="upNotes" UpdateMode="Conditional" runat="server">
                                                    <ContentTemplate>
                                                        <table class="WholeWidth">
                                                            <tr>
                                                                <td style="padding: 2px 0px 2px 4px;">
                                                                    <b>Recent Notes</b>
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="height: 110px; width: 100%; padding-left: 4px; vertical-align: top;">
                                                                    <asp:ListView ID="lvNotes" runat="server" EnableModelValidation="True">
                                                                        <LayoutTemplate>
                                                                            <table id="Table1" runat="server" style="table-layout: fixed;" class="WholeWidth">
                                                                                <tr>
                                                                                    <td colspan="3">
                                                                                        <table class="WholeWidth">
                                                                                            <tr>
                                                                                                <th style="width: 10%; padding-left: 2px;">
                                                                                                    <div class="ie-bg">
                                                                                                        Created</div>
                                                                                                </th>
                                                                                                <th style="width: 15%; padding-left: 2px;">
                                                                                                    <div class="ie-bg">
                                                                                                        By</div>
                                                                                                </th>
                                                                                                <th style="width: 75%; padding-left: 2px;">
                                                                                                    <div class="ie-bg">
                                                                                                        Note</div>
                                                                                                </th>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan="3">
                                                                                        <div style="background: white; height: 100px; overflow-y: auto;">
                                                                                            <table id="itemPlaceHolderContainer" runat="server" style="background-color: White;"
                                                                                                class="WholeWidth">
                                                                                                <tr runat="server" id="itemPlaceHolder">
                                                                                                </tr>
                                                                                            </table>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </LayoutTemplate>
                                                                        <ItemTemplate>
                                                                            <tr>
                                                                                <td style="width: 10%; padding-left: 6px;">
                                                                                    <asp:Label ID="lblDate" Width="100%" runat="server" Text='<%# Eval("CreateDate", "{0:MM/dd/yyyy}") %>'></asp:Label>
                                                                                </td>
                                                                                <td style="width: 15%; padding-left: 12px;" class="WordWrap">
                                                                                    <asp:Label ID="lblPerson" runat="server" CssClass="WordWrap" Width="100%" Text='<%# Eval("Author.LastName") %>'></asp:Label>
                                                                                </td>
                                                                                <td style="width: 75%; padding-left: 12px;" class="WordWrap">
                                                                                    <asp:Label ID="lblNote" runat="server" Width="100%" Text='<%# GetWrappedText((string) Eval("NoteText")) %>'
                                                                                        CssClass="WordWrap"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                        <AlternatingItemTemplate>
                                                                            <tr style="background-color: #F9FAFF;">
                                                                                <td style="width: 10%; padding-left: 6px;">
                                                                                    <asp:Label ID="lblDate" Width="100%" runat="server" Text='<%# Eval("CreateDate", "{0:MM/dd/yyyy}") %>'></asp:Label>
                                                                                </td>
                                                                                <td style="width: 15%; padding-left: 12px;" class="WordWrap">
                                                                                    <asp:Label ID="lblPerson" Width="100%" CssClass="WordWrap" runat="server" Text='<%# Eval("Author.LastName") %>'></asp:Label>
                                                                                </td>
                                                                                <td style="width: 75%; padding-left: 12px;" class="WordWrap">
                                                                                    <asp:Label ID="lblNote" Width="100%" runat="server" Text='<%# GetWrappedText((string) Eval("NoteText")) %>'
                                                                                        CssClass="WordWrap"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </AlternatingItemTemplate>
                                                                        <EmptyDataTemplate>
                                                                            <table style="table-layout: fixed;" class="WholeWidth">
                                                                                <tr>
                                                                                    <td colspan="3">
                                                                                        <table class="WholeWidth">
                                                                                            <tr>
                                                                                                <th style="width: 10%; padding-left: 2px; white-space: nowrap;">
                                                                                                    <div class="ie-bg">
                                                                                                        Created</div>
                                                                                                </th>
                                                                                                <th style="width: 15%; padding-left: 2px; white-space: nowrap;">
                                                                                                    <div class="ie-bg">
                                                                                                        By</div>
                                                                                                </th>
                                                                                                <th style="width: 75%; padding-left: 2px; white-space: nowrap;">
                                                                                                    <div class="ie-bg">
                                                                                                        Note</div>
                                                                                                </th>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan="3">
                                                                                        <div style="background: white; height: 100px; overflow-y: auto;">
                                                                                            <table id="itemPlaceHolderContainer" runat="server" style="background-color: White;"
                                                                                                class="WholeWidth">
                                                                                                <tr>
                                                                                                    <td colspan="3">
                                                                                                        &nbsp;
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </EmptyDataTemplate>
                                                                    </asp:ListView>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="padding: 2px 0px 0px 4px;">
                                                                    <table width="100%">
                                                                        <tr>
                                                                            <td style="width: 82%; padding: 2px 2px 0px 0px;">
                                                                                <asp:TextBox ID="tbNote" runat="server" CssClass="noteWidthHeight" MaxLength="2000"
                                                                                    Style="overflow-y: auto; resize: none; font-family: Arial, Helvetica, sans-serif;
                                                                                    font-size: 12px;" Font-Size="12px" Rows="3" TextMode="MultiLine" ValidationGroup="Notes"
                                                                                    Height="45px" />
                                                                                <ajax:TextBoxWatermarkExtender ID="twNote" runat="server" TargetControlID="tbNote"
                                                                                    WatermarkText="To add a note, click here and begin typing. When done, click the &quot;Add Note&quot; button to save your entry."
                                                                                    WatermarkCssClass="noteWidthHeight watermark-Text" Enabled="True" />
                                                                            </td>
                                                                            <td style="text-align: center; padding-left: 2px; width: 18%">
                                                                                <asp:Button ID="btnAddNote" OnClick="btnAddNote_Click" runat="server" Text="Add Note" />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:RequiredFieldValidator ID="rvNotes" runat="server" ValidationGroup="Notes" ControlToValidate="tbNote"
                                                                                    ErrorMessage="Note text is empty." Display="Dynamic" />
                                                                                <asp:CustomValidator ID="cvLen" runat="server" ErrorMessage="Maximum length of the Note is 2000 characters."
                                                                                    ClientValidationFunction="javascript:len=args.Value.length;args.IsValid=(len>0 && len<=2000);"
                                                                                    OnServerValidate="cvLen_OnServerValidate" EnableClientScript="true" ControlToValidate="tbNote"
                                                                                    ValidationGroup="Notes" Display="Dynamic" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="imgBtnFirst" />
                                                        <asp:AsyncPostBackTrigger ControlID="imgBtnPrevious" />
                                                        <asp:AsyncPostBackTrigger ControlID="imgBtnNext" />
                                                        <asp:AsyncPostBackTrigger ControlID="imgBtnLast" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </div>
                                        </ContentTemplate>
                                    </ajax:TabPanel>
                                    <ajax:TabPanel ID="tpHistory" runat="server" Visible="true">
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lnkHistory" OnClientClick="return lnkHistory_ClientClick(this);"
                                                runat="server"><span class="bg">
                                               <span>History</span></span></asp:LinkButton>
                                        </HeaderTemplate>
                                        <ContentTemplate>
                                            <asp:UpdatePanel ID="upActivityLog" UpdateMode="Conditional" runat="server">
                                                <ContentTemplate>
                                                    <div style="width: 99%; height: 310px; padding-left: 4px; padding-right: 4px; overflow-y: auto;">
                                                        <asp:PlaceHolder ID="phActivityLog" runat="server"></asp:PlaceHolder>
                                                    </div>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="lnkHistory" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </ContentTemplate>
                                    </ajax:TabPanel>
                                </ajax:TabContainer>
                                <table class="WholeWidth" style="background: #e2ebff;">
                                    <tr>
                                        <td style="padding-left: 8px; padding-right: 8px;">
                                            <asp:UpdatePanel ID="upProposedResources" UpdateMode="Conditional" runat="server">
                                                <ContentTemplate>
                                                    <table class="WholeWidth" style="width: 99%;">
                                                        <uc:ProposedResources ID="ucProposedResources" runat="server" hintDateVisible="true" />
                                                    </table>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                                                    <asp:AsyncPostBackTrigger ControlID="imgBtnFirst" />
                                                    <asp:AsyncPostBackTrigger ControlID="imgBtnPrevious" />
                                                    <asp:AsyncPostBackTrigger ControlID="imgBtnNext" />
                                                    <asp:AsyncPostBackTrigger ControlID="imgBtnLast" />
                                                    <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                                    <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                </table>
                                <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:HiddenField ID="hdnValueChanged" Value="false" runat="server" />
                                        <table class="WholeWidth" style="background: #e2ebff;">
                                            <tr>
                                                <td align="center" style="padding: 10px 0px 0px 4px; width: 100%;">
                                                    <asp:Button ID="btnConvertToProject" runat="server" Text="Convert This Opportunity To Project"
                                                        OnClick="btnConvertToProject_Click" OnClientClick="if (!confirmSaveDirty(true)) return false;" />&nbsp;
                                                    <asp:Image ID="hintConvertProject" runat="server" ImageUrl="~/Images/hint.png" ToolTip="When this button is clicked, Practice Management will attempt to create a new Project with the basic information already contained in this Opportunity. If any Proposed Resources have been selected, they will be attached to the new Project as well. " />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" style="padding: 6px 0px 0px 0px; width: 100%;">
                                                    <asp:Button ID="btnAttachToProject" runat="server" Text="Attach This Opportunity to Existing Project"
                                                        OnClick="btnAttachToProject_Click" ToolTip="Attach This Opportunity to Existing Project" />
                                                    <asp:HiddenField ID="hdnField" runat="server" />
                                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeAttachToProject" runat="server" TargetControlID="hdnField"
                                                        CancelControlID="btnCancel" BackgroundCssClass="modalBackground" PopupControlID="pnlAttachToProject"
                                                        DropShadow="false" />
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdnOpportunityId" runat="server" />
                                        <div style="background-color: #e2ebff; padding: 2px;">
                                            <asp:CustomValidator ID="custOpportunityNotSaved" runat="server" ErrorMessage="The opportunity must be saved at first."
                                                ToolTip="The opportunity must be saved at first." EnableClientScript="false"
                                                EnableViewState="false"></asp:CustomValidator>
                                            <asp:Literal ID="ltrWonConvertInvalid" runat="server" EnableViewState="false" Visible="false"
                                                Mode="PassThrough">
                                        <script type="text/javascript">
                                            alert('{0}');
                                        </script>
                                            </asp:Literal>
                                            <div id="divPriorityDescription" style="width: 100%; max-width: 330px; display: none;"
                                                runat="server">
                                                <table width="100%" style="border: 1px solid #FF9F00; padding-left: 4px; padding-right: 4px;
                                                    background-color: White;">
                                                    <tr style="background-color: #FF9F00 !important;">
                                                        <td style="width: 95%;">
                                                        </td>
                                                        <td style="width: 5%;" valign="top" align="right">
                                                            <asp:Label ID="lblClose" runat="server" Font-Bold="true" Font-Size="Small" ToolTip="Close"
                                                                Style="cursor: pointer;" Text="x"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="padding: 2px;">
                                                            <asp:ListView ID="lvOpportunityPriorities" runat="server">
                                                                <LayoutTemplate>
                                                                    <table id="itemPlaceHolderContainer" runat="server" style="background-color: White;"
                                                                        class="WholeWidth">
                                                                        <tr runat="server" id="itemPlaceHolder">
                                                                        </tr>
                                                                    </table>
                                                                </LayoutTemplate>
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td style="width: 100%; white-space: nowrap; padding-left: 2px;">
                                                                            <asp:Label ID="lblPriority" Width="5%" runat="server" Text='<%# Eval("Priority") %>'></asp:Label>&nbsp;-&nbsp;<asp:Label
                                                                                ID="lblDescription" runat="server" Width="95%" Text='<%# Eval("Description") %>'></asp:Label>
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
                                            </div>
                                        </div>
                                        <table style="width: 100%; background: #e2ebff;">
                                            <tr>
                                                <td style="padding: 4px; height: 35px; width: 64%;">
                                                    <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                                                    <asp:ValidationSummary ID="vsumOpportunity" runat="server" ValidationGroup="Opportunity"
                                                        EnableClientScript="false" HeaderText="Error occurred while saving an opportunity." />
                                                    <asp:ValidationSummary ID="vsumOpportunityTransition" runat="server" ValidationGroup="OpportunityTransition"
                                                        DisplayMode="BulletList" EnableClientScript="false" HeaderText="Unable to proceed with opportunity transition due to the following errors:" />
                                                    <asp:ValidationSummary ID="vsumWonConvert" runat="server" ValidationGroup="WonConvert"
                                                        DisplayMode="BulletList" EnableClientScript="false" HeaderText="Unable to convert opportunity due to the following errors:" />
                                                    <asp:ValidationSummary ID="vsumHasPersons" runat="server" ValidationGroup="HasPersons"
                                                        DisplayMode="BulletList" EnableClientScript="false" HeaderText="Unable to convert opportunity due to the following errors:" />
                                                </td>
                                                <td style="padding: 4px; height: 35px; width: 13%;">
                                                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
                                                </td>
                                                <td style="padding: 4px; height: 35px; width: 13%;">
                                                    <asp:Button ID="btnCancelChanges" runat="server" Text="Cancel Changes" OnClientClick="if(getDirty()){return true;}else{return false;}"
                                                        OnClick="btnCancelChanges_Click" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="padding: 4px; width: 100%;">
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                                        <asp:AsyncPostBackTrigger ControlID="btnCancel" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnFirst" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnPrevious" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnNext" />
                                        <asp:AsyncPostBackTrigger ControlID="imgBtnLast" />
                                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="upAttachToProject" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlAttachToProject" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass" Style="display: none" BorderWidth="2px">
                <table width="100%">
                    <tr style="background-color: Gray; height: 27px;">
                        <td align="center" style="white-space: nowrap; font-size: 14px; width: 100%">
                            Attach This Opportunity to Existing Project
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 6px 6px 2px 2px;">
                            <asp:DropDownList ID="ddlProjects" runat="server" AppendDataBoundItems="true" onchange="setDirty();"
                                AutoPostBack="false" Style="width: 350px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 6px 6px 2px 2px; white-space: nowrap;">
                            <asp:Button ID="btnAttach" runat="server" Text="Attach" OnClick="btnSave_Click" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAttach" />
            <asp:AsyncPostBackTrigger ControlID="btnCancel" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnFirst" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnPrevious" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnNext" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnLast" />
            <asp:AsyncPostBackTrigger ControlID="btnSave" />
            <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
            <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
            <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="lpOpportunityDetails" runat="server" />
</asp:Content>

