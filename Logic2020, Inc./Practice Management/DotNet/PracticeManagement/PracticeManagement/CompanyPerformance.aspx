<%@ Page Language="C#" MasterPageFile="~/PracticeManagement.Master" AutoEventWireup="true"
    CodeBehind="CompanyPerformance.aspx.cs" Inherits="PraticeManagement.CompanyPerformance"
    Title="Practice Management - Performance Grid (roll-off)" %>

<%@ Register Src="Controls/PracticeFilter.ascx" TagName="PracticeFilter" TagPrefix="uc1" %>
<%@ Register Src="Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Performance Grid (roll-off)</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Projected Project Profit & Loss&nbsp; 
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <table>
        <tr>
            <td>
                <table style="margin-bottom: 10px; margin-top: 10px;">
                    <tr>
                        <td style="width: 90px">
                            Select Dates
                        </td>
                        <td style="width: 115px">
                            <uc2:MonthPicker ID="mpPeriodStart" runat="server" AutoPostBack="false" />
                        </td>
                        <td style="width: 26px; text-align: center;">
                            &nbsp;to&nbsp;
                        </td>
                        <td style="width: 115px">
                            <uc2:MonthPicker ID="mpPeriodEnd" runat="server" AutoPostBack="false" />
                        </td>
                        <td style="width: 20px">
                            <asp:CustomValidator ID="custPeriod" runat="server" ErrorMessage="The Period Start must be less then or equals to the Period End"
                                ToolTip="The Period Start must be less then or equals to the Period End" Text="*"
                                EnableClientScript="false" OnServerValidate="custPeriod_ServerValidate" ValidationGroup="Filter"></asp:CustomValidator>
                            <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="false"
                                ErrorMessage="The period length must be not more then {0} months." ToolTip="The period length must be not more then {0} months."
                                Text="*" EnableClientScript="false" OnServerValidate="custPeriodLengthLimit_ServerValidate"
                                ValidationGroup="Filter"></asp:CustomValidator>
                        </td>
                        <td align="right" style="width: 360px">
                            <asp:Button ID="btnReset" runat="server" Text="Reset Filter" Width="100px" CausesValidation="false"
                                OnClientClick="this.disabled=true;Delete_Cookie('CompanyPerformanceFilterKey', '/', '');window.location.href=window.location.href;return false;"
                                EnableViewState="False" />
                        </td>
                        <td align="right" style="width: 110px">
                            <asp:Button ID="btnUpdateView" runat="server" Text="Update View" Width="100px" 
                                OnClick="btnUpdateView_Click"
                                ValidationGroup="Filter" EnableViewState="False" />
                        </td>
                        <td align="right" valign="middle" style="width: 92px; white-space: nowrap;">
                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                            <asp:ImageButton ID="btnExpandCollapseFilter" runat="server" OnClientClick="return false;"
                                CausesValidation="false" ImageUrl="~/Images/collapse.jpg" />
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="pnlAdvancedFilter" runat="server">
                    <ajaxToolkit:TabContainer ID="tcFilters" runat="server">
                        <ajaxToolkit:TabPanel ID="tpProjectType" runat="server">
                            <HeaderTemplate>Project Type</HeaderTemplate>
                            <ContentTemplate>
                                 <table class="WholeWidth" cellpadding="5">
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chbActive" runat="server" Text="Active Projects" Checked="True"
                                                EnableViewState="False" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chbProjected" runat="server" Text="Projected Projects" Checked="true"
                                                EnableViewState="False" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chbCompleted" runat="server" Text="Completed Projects" Checked="true"
                                                EnableViewState="False" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chbInternal" runat="server" Text="Internal Projects" EnableViewState="False" Checked="true" />
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chbExperimental" runat="server" Text="Experimental Projects" EnableViewState="False" />
                                        </td>
                                    </tr>
                                </table>
                           </ContentTemplate>
                        </ajaxToolkit:TabPanel>                    
                        <ajaxToolkit:TabPanel ID="tpProjectFilters" runat="server">
                            <HeaderTemplate>Project Filters</HeaderTemplate>
                            <ContentTemplate>
                                <table class="WholeWidth" cellpadding="5">
                                    <tr>
                                        <td style="background: #DDDDDD;">
                                            <b>Client</b>
                                        </td>
                                        <td style="background: #DDDDDD;">
                                            <b>Project Group</b>
                                        </td>
                                        <td style="background: #EEEEEE;">
                                            <b>Salesperson</b>
                                        </td>
                                        <td style="background: #EEEEEE;">
                                            <b>Project Owner</b>
                                        </td>
                                        <td style="background: #DDDDDD;">
                                            <b>Practice</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="background: #DDDDDD;">
                                            <uc:CascadingMsdd ID="cblClient" runat="server" Width="200" Height="100" BackColor="#ffffff"
                                                CellPadding="3" BorderColor="#aaaaaa" TargetControlId="cblProjectGroup" SetDirty="false" />
                                        </td>
                                        <td style="background: #DDDDDD;">
                                            <uc:ScrollingDropDown ID="cblProjectGroup" runat="server" NoItemsType="All" Width="200"
                                                Height="100" BackColor="#ffffff" CellPadding="3" BorderColor="#aaaaaa" SetDirty="false" />
                                        </td>
                                        <td style="background: #EEEEEE;">
                                            <uc:ScrollingDropDown ID="cblSalesperson" runat="server" Width="200" Height="100"
                                                BackColor="#ffffff" CellPadding="3" BorderColor="#aaaaaa" SetDirty="false" />
                                        </td>
                                        <td style="background: #EEEEEE;">
                                            <uc:ScrollingDropDown ID="cblProjectOwner" runat="server" Width="200" Height="100"
                                                BackColor="#ffffff" CellPadding="3" BorderColor="#aaaaaa" SetDirty="false" />
                                        </td>
                                        <td style="background: #DDDDDD;">
                                            <uc:ScrollingDropDown ID="cblPractice" runat="server" Width="200" Height="100" BackColor="#ffffff"
                                                CellPadding="3" BorderColor="#aaaaaa" SetDirty="false" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>                    
                        <ajaxToolkit:TabPanel ID="tpMoreOptions" runat="server">
                            <HeaderTemplate>Project Search</HeaderTemplate>
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch">
                                    <table class="WholeWidth" cellpadding="5">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtSearchText" runat="server" CssClass="WholeWidth" EnableViewState="False" />
                                                <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                                    ErrorMessage="Please type text to be searched." ToolTip="Please type text to be searched."
                                                    Text="*" SetFocusOnError="true" ValidationGroup="Search" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server" Text="Search" ValidationGroup="Search"
                                                    PostBackUrl="~/ProjectSearch.aspx" Width="100px" EnableViewState="False" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>                    
                        <ajaxToolkit:TabPanel ID="tpProjectSearch" runat="server">
                            <HeaderTemplate>More Options</HeaderTemplate>
                            <ContentTemplate>
                                <table class="WholeWidth" cellpadding="5">
                                    <tr>
                                        <td style="width: 25%">
                                            <asp:CheckBox ID="chbPeriodOnly" runat="server" Text="Total Only Selected Date Window"
                                                Checked="true" EnableViewState="False" />
                                        </td>
                                        <td style="width: 25%">
                                            <asp:CheckBox ID="chbPrintVersion" runat="server" Text="Print version" EnableViewState="False" />
                                        </td>
                                        <td style="width: 25%">
                                        </td>
                                        <td style="width: 25%">
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>                    
                    </ajaxToolkit:TabContainer>
                </asp:Panel>
                <AjaxControlToolkit:CollapsiblePanelExtender ID="pnlAdvancedFilter_CollapsiblePanelExtender"
                    runat="server" TargetControlID="pnlAdvancedFilter" ImageControlID="btnExpandCollapseFilter"
                    CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                    ExpandControlID="btnExpandCollapseFilter" Collapsed="True" CollapsedText="More filters"
                    ExpandedText="Hide filters" TextLabelID="lblFilter">
                </AjaxControlToolkit:CollapsiblePanelExtender>
            </td>
        </tr>
        <tr>
            <td style="padding-top: 10px">
                <asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here."
                    AllowSorting="true" OnSorting="gvProjects_Sorting" OnRowDataBound="gvProjects_RowDataBound"
                    UseAccessibleHeader="false" HeaderStyle-CssClass="CompPerfHeader" CssClass="CompPerfTable"
                    EnableViewState="False">
                    <Columns>
                        <asp:BoundField HeaderText="Project Num" HeaderStyle-CssClass="CompPerfProjectNumber"
                            ItemStyle-CssClass="CompPerfProjectNumber" SortExpression="ProjectNumber">
                            <HeaderStyle CssClass="CompPerfProjectNumber"></HeaderStyle>
                            <ItemStyle CssClass="CompPerfProjectNumber"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Client" HeaderStyle-CssClass="CompPerfClient" ItemStyle-CssClass="CompPerfClient"
                            SortExpression="Client">
                            <HeaderStyle CssClass="CompPerfClient"></HeaderStyle>
                            <ItemStyle CssClass="CompPerfClient"></ItemStyle>
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Project" HeaderStyle-CssClass="CompPerfProject" ItemStyle-CssClass="CompPerfProject"
                            SortExpression="Project">
                            <HeaderStyle CssClass="CompPerfProject"></HeaderStyle>
                            <ItemStyle CssClass="CompPerfProject"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Start Date" HeaderStyle-CssClass="CompPerfPeriod"
                            ItemStyle-CssClass="CompPerfPeriod" SortExpression="StartDate">
                            <HeaderStyle CssClass="CompPerfPeriod"></HeaderStyle>
                            <ItemStyle CssClass="CompPerfPeriod"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="End Date" HeaderStyle-CssClass="CompPerfPeriod" ItemStyle-CssClass="CompPerfPeriod"
                            SortExpression="EndDate">
                            <HeaderStyle CssClass="CompPerfPeriod"></HeaderStyle>
                            <ItemStyle CssClass="CompPerfPeriod"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Font-Bold="True" ItemStyle-Wrap="False" HeaderStyle-CssClass="CompPerfTotalSummary"
                            ItemStyle-CssClass="CompPerfTotalSummary">
                            <HeaderStyle CssClass="CompPerfTotalSummary"></HeaderStyle>
                            <ItemStyle Wrap="False" CssClass="CompPerfTotalSummary" Font-Bold="True"></ItemStyle>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="CompPerfHeader"></HeaderStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:ValidationSummary ID="valsPerformance" runat="server" Width="100%" ValidationGroup="Filter" />
                <asp:ValidationSummary ID="valsSearch" runat="server" Width="100%" ValidationGroup="Search"
                    EnableClientScript="true" ShowMessageBox="true" />
            </td>
        </tr>
        <tr>
            <td id="tdProjectTabViewSwitch" runat="server">
                <div id="divWait" style="display: none;">
                    <span style="color: Black; font-weight: bold;">
                        <nobr>Please Wait...</nobr>
                    </span>
                    <br />
                    <img id="imgLoading" name="imgLoading" alt="Please Wait..." src="./Images/ajax-loader.gif" />
                </div>
                <div id="divContainer">
                    <asp:UpdatePanel ID="upnlBody" runat="server">
                        <ContentTemplate>
                            <asp:Table ID="tblProjectTabViewSwitch" runat="server" CssClass="Switcher">
                                <asp:TableRow ID="rowSwitcher" runat="server">
                                    <asp:TableCell ID="cellReportDescription" runat="server" CssClass="SelectedSwitch">
                                        <asp:LinkButton ID="btnReportDescription" runat="server" Text="Report Description"
                                            CausesValidation="false" OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton>
                                    </asp:TableCell>
                                    <asp:TableCell ID="cellFinancialSummary" runat="server">
                                        <asp:LinkButton ID="btnFinancialSummary" runat="server" Text="Financial Summary"
                                            CausesValidation="false" OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton>
                                    </asp:TableCell>
                                    <asp:TableCell ID="cellCommissionsAndRates" runat="server" Visible="false">
                                        <asp:LinkButton ID="btnCommissionsAndRates" runat="server" Text="Commissions and Rates"
                                            CausesValidation="false" OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton>
                                    </asp:TableCell>
                                    <asp:TableCell ID="cellPersonStats" runat="server">
                                        <asp:LinkButton ID="btnPersonStats" runat="server" Text="Person Stats" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="3"></asp:LinkButton>
                                    </asp:TableCell>
                                    <%--<asp:TableCell ID="cellBenchRollOffDates" runat="server">
                                        <asp:LinkButton ID="btnBenchRollOffDates" runat="server" Text="Bench Roll-Off Dates"
                                            CausesValidation="false" OnCommand="btnView_Command" CommandArgument="4"></asp:LinkButton>
                                    </asp:TableCell>--%>
                                    <asp:TableCell ID="cellBenchCosts" runat="server">
                                        <asp:LinkButton ID="btnBenchCosts" runat="server" Text="Bench Costs" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="4"></asp:LinkButton>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                            <asp:MultiView ID="mvProjectTab" runat="server"> <%--ActiveViewIndex="0"--%>
                                <asp:View ID="vwReportDescription" runat="server">
                                    <h3>
                                        Reports.</h3>
                                    <p>
                                        Click a tab header to see aggregate information about these projects.</p>
                                </asp:View>
                                <asp:View ID="vwFinancialSummary" runat="server">
                                    <asp:GridView ID="gvFinancialSummary" runat="server" AutoGenerateColumns="False"
                                        EmptyDataText="There is nothing to be displayed here." OnRowDataBound="gvFinancialSummary_RowDataBound"
                                        CssClass="CompPerfTable" EnableViewState="False">
                                        <Columns>
                                            <asp:BoundField HeaderText="Financials" HeaderStyle-CssClass="CompPerfDataTitle"
                                                ItemStyle-CssClass="CompPerfDataTitle">
                                                <HeaderStyle CssClass="CompPerfDataTitle" />
                                                <ItemStyle CssClass="CompPerfDataTitle" />
                                            </asp:BoundField>
                                            <asp:TemplateField ItemStyle-Font-Bold="True" ItemStyle-Wrap="False" HeaderStyle-CssClass="CompPerfTotalSummary"
                                                ItemStyle-CssClass="CompPerfTotalSummary" Visible="false">
                                                <HeaderStyle CssClass="CompPerfTotalSummary" />
                                                <ItemStyle CssClass="CompPerfTotalSummary" Font-Bold="True" Wrap="False" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:View>
                                <asp:View ID="vwCommissionsAndRates" runat="server">
                                    <asp:GridView ID="gvCommissionsAndRates" runat="server" AutoGenerateColumns="False"
                                        EmptyDataText="There is nothing to be displayed here." OnRowDataBound="gvCommissionsAndRates_RowDataBound"
                                        CssClass="CompPerfTable" EnableViewState="False">
                                        <Columns>
                                            <asp:BoundField HeaderText="Commissions & Rates" HeaderStyle-CssClass="CompPerfDataTitle"
                                                ItemStyle-CssClass="CompPerfDataTitle">
                                                <HeaderStyle CssClass="CompPerfDataTitle" />
                                                <ItemStyle CssClass="CompPerfDataTitle" />
                                            </asp:BoundField>
                                            <asp:TemplateField ItemStyle-Font-Bold="True" ItemStyle-Wrap="False" HeaderStyle-CssClass="CompPerfTotalSummary"
                                                ItemStyle-CssClass="CompPerfTotalSummary" Visible="false">
                                                <HeaderStyle CssClass="CompPerfTotalSummary" />
                                                <ItemStyle CssClass="CompPerfTotalSummary" Font-Bold="True" Wrap="False" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:View>
                                <asp:View ID="vwPersonStats" runat="server">
                                    <asp:Table ID="tblPersonStats" runat="server" CssClass="CompPerfTable" Border="1"
                                        CellPadding="0" CellSpacing="0" EnableViewState="False">
                                        <asp:TableHeaderRow>
                                            <asp:TableHeaderCell HorizontalAlign="Right" CssClass="CompPerfDataTitle">Person Stats</asp:TableHeaderCell>
                                        </asp:TableHeaderRow>
                                        <asp:TableRow>
                                            <asp:TableCell CssClass="CompPerfDataTitle">Revenue</asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell CssClass="CompPerfDataTitle"># Employees</asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell CssClass="CompPerfDataTitle"># Consultants</asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell CssClass="CompPerfDataTitle">Virtual Consultants</asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell CssClass="CompPerfDataTitle">Change&nbsp;in&nbsp;VC&nbsp;from&nbsp;last&nbsp;month</asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell CssClass="CompPerfDataTitle">$ Rev/Employee</asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell CssClass="CompPerfDataTitle">$ Rev/Consultant</asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell CssClass="CompPerfDataTitle">Admin Cost as % of Revenue</asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </asp:View>
                                <%--<asp:View ID="vwBenchRollOffDates" runat="server">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="gvBenchRollOffDates" runat="server" AutoGenerateColumns="False"
                                                    EmptyDataText="There is nothing to be displayed here." OnRowDataBound="gvBenchRollOffDates_RowDataBound"
                                                    CssClass="CompPerfTable" EnableViewState="true">
                                                    <Columns>
                                                        <asp:BoundField HeaderText="Bench Roll Off Dates" DataField="Name" HeaderStyle-CssClass="CompPerfDataTitle"
                                                            ItemStyle-CssClass="CompPerfDataTitle">
                                                            <HeaderStyle CssClass="CompPerfDataTitle" />
                                                            <ItemStyle CssClass="CompPerfDataTitle" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField ItemStyle-Font-Bold="True" ItemStyle-Wrap="False" HeaderStyle-CssClass="CompPerfTotalSummary"
                                                            ItemStyle-CssClass="CompPerfTotalSummary" Visible="false">
                                                            <HeaderStyle CssClass="CompPerfTotalSummary" />
                                                            <ItemStyle CssClass="CompPerfTotalSummary" Font-Bold="True" Wrap="False" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:View>--%>
                                <asp:View ID="vwBenchCosts" runat="server">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="gvBenchCosts" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here."
                                                    OnRowDataBound="gvBenchRollOffDates_RowDataBound" CssClass="CompPerfTable" EnableViewState="False">
                                                    <Columns>
                                                        <asp:BoundField HeaderText="Bench Cost" DataField="Name" HeaderStyle-CssClass="CompPerfDataTitle"
                                                            ItemStyle-CssClass="CompPerfDataTitle">
                                                            <HeaderStyle CssClass="CompPerfDataTitle" />
                                                            <ItemStyle CssClass="CompPerfDataTitle" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField ItemStyle-Font-Bold="True" ItemStyle-Wrap="False" HeaderStyle-CssClass="CompPerfTotalSummary"
                                                            ItemStyle-CssClass="CompPerfTotalSummary" Visible="false">
                                                            <HeaderStyle CssClass="CompPerfTotalSummary" />
                                                            <ItemStyle CssClass="CompPerfTotalSummary" Font-Bold="True" Wrap="False" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:View>
                            </asp:MultiView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <%--                <asp:LinkButton ID="btnExportBenchListToExcel" runat="server" 
                        onclick="btnExportBenchListToExcel_Click">Export Bench List</asp:LinkButton>
--%>
                </div>
                <ajaxToolkit:UpdatePanelAnimationExtender ID="ae" runat="server" TargetControlID="upnlBody">
                    <Animations>
                        <OnUpdating>
                            <Sequence>
                                <ScriptAction Script="showInProcessImage($get('divWait'), $get('divContainer'));" />
                                <FadeOut duration=".5" AnimationTarget="container" minimumOpacity="0" />
                            </Sequence>
                        </OnUpdating>
                        <OnUpdated>
                            <Sequence>
                                <FadeIn duration=".5" AnimationTarget="container" minimumOpacity="0" />
                                <ScriptAction Script="hideInProcessImage($get('divWait'));" />
                            </Sequence>
                        </OnUpdated>
                    </Animations>
                </ajaxToolkit:UpdatePanelAnimationExtender>
            </td>
        </tr>
        <tr>
            <td id="tdAddButton" runat="server" align="right" valign="top">
                <asp:Button runat="server" ID="buttonAddProject" Text="Add Project" OnClick="buttonAddProject_Click" /><br />
                To edit or delete a project please click the Project Name.<br />
                To edit or delete a client please click the Client Name.
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="footer">
    <h6>
        Version. <asp:Label ID="lblCurrentVersion" runat="server"></asp:Label></h6>
</asp:Content>

