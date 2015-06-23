<%@ Page Title="Non Billable Report" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="NonBillableReport.aspx.cs" Inherits="PraticeManagement.Reports.NonBillableReport" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/NonbillableSummary.ascx" TagPrefix="uc" TagName="summary" %>
<%@ Register Src="~/Controls/Reports/NonbillableDetail.ascx" TagPrefix="uc" TagName="detail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        //        function chooseTarget() {
        //            alert('Hi');
        //            var ddlProjects = document.getElementById('<%= ddlProjectsOption.ClientID %>');
        //            var selectedValue = ddlProjects.value;
        //            var cblDirectors = document.getElementById('<%= cblDirectors.ClientID %>');
        //            var sdeDirectors = document.getElementById('sdeDirectorsLabel');
        //            var cblProjectGroup = document.getElementById('<%= cblProjectGroup.ClientID %>');
        //            var sdeProjectGroup = document.getElementById('sdeProjectGroupLabel');
        //            var cblPractices = document.getElementById('<%= cblPractices.ClientID %>');
        //            var sdePractices = document.getElementById('sdePracticesLabel');
        //            var txtProjectNumber = document.getElementById('<%= txtProjectNumber.ClientID %>');
        //            var imgProjectSearch = document.getElementById('<%= imgProjectSearch.ClientID %>');
        //            var btnUpdateView = document.getElementById('<%= btnUpdateView.ClientID %>'); 
        //            cblDirectors.setAttribute('style', 'display:none;');
        //            sdeDirectors.setAttribute('style', 'display:none;');
        //            cblProjectGroup.setAttribute('style', 'display:none;');
        //            sdeProjectGroup.setAttribute('style', 'display:none;');
        //            cblPractices.setAttribute('style', 'display:none;');
        //            sdePractices.setAttribute('style', 'display:none;');
        //            txtProjectNumber.setAttribute('style', 'display:none;');
        //            imgProjectSearch.setAttribute('style', 'display:none;');
        //            btnUpdateView.removeAttribute('disabled');
        //            switch (selectedValue) {
        //                case '1': txtProjectNumber.setAttribute('style', 'display:inline-block;');
        //                    imgProjectSearch.setAttribute('style', 'display:inline-block;');
        //                    break;
        //                case '3': cblDirectors.setAttribute('style', 'display:inline-block;');
        //                    sdeDirectors.setAttribute('style', 'display:inline-block;');
        //                    break;
        //                case '4': cblProjectGroup.setAttribute('style', 'display:inline-block;');
        //                    sdeProjectGroup.setAttribute('style', 'display:inline-block;');
        //                    break;
        //                case '5': cblPractices.setAttribute('style', 'display:inline-block;');
        //                    sdePractices.setAttribute('style', 'display:inline-block;');
        //                    break;
        //            }
        //        }

        function txtSearch_onkeyup(e) {

            var txtProjectSearch = document.getElementById('<%= txtProjectSearch.ClientID %>');
            var btnSearch = document.getElementById('<%= btnProjectSearch.ClientID %>');
            if (txtProjectSearch.value != '') {
                btnSearch.removeAttribute('disabled');
            }
            else {
                btnSearch.setAttribute('disabled', 'disabled');
            }
            return true;
        }

        function txtSearch_onkeypress(e) {
            var keynum;
            if (window.event) // IE8 and earlier
            {
                keynum = e.keyCode;
            }
            else if (e.which) // IE9/Firefox/Chrome/Opera/Safari
            {
                keynum = e.which;
            }
            if (keynum == 13) {
                var btnSearch = document.getElementById('<%= btnProjectSearch.ClientID %>');
                btnSearch.click();
                return false;
            }
            return true;
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr class="height30P">
                    <td class="fontBold Width3Percent no-wrap">
                        &nbsp;Select report parameters:&nbsp;
                    </td>
                    <td>
                    </td>
                    <td class="width60P">
                    </td>
                </tr>
                <tr class="height30P WholeWidth">
                    <td colspan="2" style="padding-left:2%">
                     <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                            ValidationGroup="valRange" IsBillingReport="true" FromToDateFieldCssClass="Width70Px" />
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Display projects by:&nbsp;
                    </td>
                    <td class="textLeft Width15Per">
                        <asp:DropDownList ID="ddlProjectsOption" runat="server" AutoPostBack="true" CssClass="Width220Px"
                            OnSelectedIndexChanged="ddlProjectsOption_SelecteIndexChanged">
                            <asp:ListItem Text="-- Select a value -- " Value="0" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Project Number" Value="1"></asp:ListItem>
                            <asp:ListItem Text="All Projects" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Executive in Charge" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Business Unit" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Practice Area" Value="5"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtProjectNumber" runat="server" Visible="false"></asp:TextBox>
                                    <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtProjectNumber" runat="server"
                                        TargetControlID="txtProjectNumber" BehaviorID="waterMarkTxtProjectNumber" WatermarkCssClass="watermarkedtext"
                                        WatermarkText="Ex: P1234767">
                                    </AjaxControlToolkit:TextBoxWatermarkExtender>
                                </td>
                                <td>
                                    <asp:Image ID="imgProjectSearch" runat="server" ToolTip="Project Search" ImageUrl="~/Images/search_24.png"
                                        Visible="false" />
                                </td>
                            </tr>
                        </table>
                        <pmc:ScrollingDropDown ID="cblDirectors" runat="server" SetDirty="false" AllSelectedReturnType="Null" AutoPostBack="true" DropdownListFirst="Executive" DropdownListSecond="in Charge"
                            Style="display: none;width:240px" NoItemsType="All" onclick="scrolling_onclick('cblDirector','Executive in Charge','s','Executives in Charge',33,'Executive','in Charge')" DropDownListTypePluralForm="Executives in Charge"
                            DropDownListType="Executive in Charge" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv" /> 
                        <ext:ScrollableDropdownExtender ID="sdeDirectors" runat="server" TargetControlID="cblDirectors"
                            Display="none" UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                        <pmc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" AllSelectedReturnType="Null" AutoPostBack="true"
                            Style="display: none;width:240px" NoItemsType="All" onclick="scrollingDropdown_onclick('cblProjectGroup','Business Unit')"
                            DropDownListType="Business Unit" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv" />
                        <ext:ScrollableDropdownExtender ID="sdeProjectGroup" runat="server" TargetControlID="cblProjectGroup"
                            Display="None" UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                        <pmc:ScrollingDropDown ID="cblPractices" runat="server" SetDirty="false" AllSelectedReturnType="Null" AutoPostBack="true"
                            Style="display:none;width:240px" NoItemsType="All" onclick="scrollingDropdown_onclick('cblPractices','Practice')"
                            DropDownListType="Practice" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv" /> 
                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                            Display="None" UseAdvanceFeature="true" Width="240px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td class="textRight">
                        <asp:Button ID="btnUpdateView" Text="View Report" runat="server" OnClick="btnUpdateView_Click"
                            Enabled="false" />
                    </td>
                </tr>
                <tr class="ReportBorderBottomByAccount">
                    <td colspan="3">
                        <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="valRange" ShowMessageBox="false"
                            ShowSummary="true" EnableClientScript="false" />
                    </td>
                </tr>
            </table>
            <br />
            <div id="divWholePage" runat="server" style="display: none">
                <table>
                    <tr>
                        <td style="font-weight: bold; font-size: 16px;">
                            Projected Range: (<asp:Label ID="lblRange" runat="server"></asp:Label>)
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblProjectViewSwitch" runat="server" CssClass="CommonCustomTabStyle AccountSummaryReportCustomTabStyle">
                    <asp:TableRow ID="rowSwitcher" runat="server">
                        <asp:TableCell ID="cellSummary" CssClass="SelectedSwitch" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="0" ToolTip="Summary"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                        <asp:TableCell ID="cellDetail" runat="server" Enabled="false">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnDetail" runat="server" Text="Detail" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="1" ToolTip="Detail"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:MultiView ID="mvAccountReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwBusinessUnitReport" runat="server">
                        <asp:Panel ID="pnlNonbillableReport" runat="server" CssClass="WholeWidth">
                            <uc:summary ID="nonbillableSummary" runat="server" />
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwDetail" runat="server">
                        <asp:Panel ID="pnlNonbillableDetail" runat="server" CssClass="WholeWidth">
                            <uc:detail ID="nonbillableDetail" runat="server" />
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeProjectSearch" runat="server" TargetControlID="imgProjectSearch"
                BackgroundCssClass="modalBackground" PopupControlID="pnlProjectSearch" BehaviorID="mpeProjectSearch"
                DropShadow="false" />
            <asp:Panel ID="pnlProjectSearch" runat="server" CssClass="popUp ProjectSearch" Style="display: none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th>
                            Project Search
                            <asp:Button ID="btnclose" runat="server" CssClass="mini-report-closeNew" ToolTip="Close"
                                OnClick="btnclose_OnClick" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="Width100Px textRight">
                                        Account:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlClients" runat="server" CssClass="Width250Px" OnSelectedIndexChanged="ddlClients_OnSelectedIndexChanged"
                                            AutoPostBack="true">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="Width100Px textRight">
                                        Project:
                                    </td>
                                    <td>
                                        <pmc:CustomDropDown ID="ddlProjects" runat="server" Enabled="false" AutoPostBack="true"
                                            CssClass="Width250Px" OnSelectedIndexChanged="ddlProjects_OnSelectedIndexChanged">
                                            <asp:ListItem Text="-- Select a Project --" Value="">
                                            </asp:ListItem>
                                        </pmc:CustomDropDown>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <table class="WholeWidth">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtProjectSearch" onkeypress="return txtSearch_onkeypress(event);"
                                            onkeyup="return txtSearch_onkeyup(event);" CssClass="Width330Px" runat="server"></asp:TextBox>
                                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmeProjectSearch" runat="server"
                                            TargetControlID="txtProjectSearch" WatermarkCssClass="watermarkedtext Width330Px"
                                            WatermarkText="To search for a project, click here to begin typing...">
                                        </AjaxControlToolkit:TextBoxWatermarkExtender>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnProjectSearch" UseSubmitBehavior="false" disabled="disabled" runat="server"
                                            Text="Search" ToolTip="Search" OnClick="btnProjectSearch_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <div class="ProjectSearchResultsDiv">
                                <asp:Repeater ID="repProjectNamesList" runat="server">
                                    <HeaderTemplate>
                                        <table id="tblProjectSearchResult" class="tablesorter CompPerfTable ProjectSearchResultsTable">
                                            <thead>
                                                <tr class="CompPerfHeader">
                                                    <th class="Width20Percent">
                                                        <div class="ie-bg">
                                                            Project
                                                        </div>
                                                    </th>
                                                    <th class="Width50Percent">
                                                        <div class="ie-bg">
                                                            Project Name
                                                        </div>
                                                    </th>
                                                    <th>
                                                        <div class="ie-bg">
                                                            Account
                                                        </div>
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="textCenter">
                                                <asp:LinkButton ID="lnkProjectNumber" ProjectNumber='<%# Eval("ProjectNumber")%>'
                                                    OnClick="lnkProjectNumber_OnClick" runat="server"><%# Eval("ProjectNumber")%></asp:LinkButton>
                                            </td>
                                            <td>
                                                <%# Eval("HtmlEncodedName")%>
                                            </td>
                                            <td>
                                                <%# Eval("Client.HtmlEncodedName")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </tbody></table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="padLeft8">
                            <asp:Literal ID="ltrlNoProjectsText" Visible="false" runat="server" Text="No Projects found."></asp:Literal>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="nonbillableSummary$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="nonbillableDetail$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

