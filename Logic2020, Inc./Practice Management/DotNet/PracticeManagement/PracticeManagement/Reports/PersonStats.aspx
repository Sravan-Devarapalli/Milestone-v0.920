<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PersonStats.aspx.cs" Inherits="PraticeManagement.Reporting.PersonStatsReport"
    MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Reports/PersonStats.ascx" TagPrefix="uc" TagName="PersonStats" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc2" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="ctrlhead" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src='<%# Generic.GetClientUrl("~/Scripts/Cookie.min.js", this) %>'></script>
</asp:Content>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Person Stats | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    PersonStats
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <div class="filters">
        <div class="buttons-block">
            <table class="BorderNone padLeft10 WholeWidth">
                <tr>
                    <td>
                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                            ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Expand Filters" />
                    </td>
                    <td class="Width90Px">
                        Select Dates
                    </td>
                    <td class="Width40Px textCenter">
                        &nbsp;from&nbsp;
                    </td>
                    <td class="Width115Px">
                        <uc2:MonthPicker ID="mpPeriodStart" runat="server" AutoPostBack="false" />
                    </td>
                    <td class="Width26Px textCenter">
                        &nbsp;to&nbsp;
                    </td>
                    <td class="Width115Px">
                        <uc2:MonthPicker ID="mpPeriodEnd" runat="server" AutoPostBack="false" />
                    </td>
                    <td class="Width20Px">
                        <asp:CustomValidator ID="custPeriod" runat="server" ErrorMessage="The Period Start must be less than or equal to the Period End"
                            ToolTip="The Period Start must be less than or equal to the Period End" Text="*"
                            EnableClientScript="False" ValidationGroup="Filter" OnServerValidate="custPeriod_ServerValidate"></asp:CustomValidator>
                        <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="False"
                            ErrorMessage="The period length must be not more then 24 months." ToolTip="The period length must be not more then 24 months."
                            Text="*" EnableClientScript="False" ValidationGroup="Filter" OnServerValidate="custPeriodLengthLimit_ServerValidate"></asp:CustomValidator>
                    </td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Update View" Width="100px" OnClick="btnUpdateView_Click"
                            ValidationGroup="Filter" EnableViewState="False" CssClass="pm-button" />
                        <asp:Button ID="Button2" runat="server" Text="Reset Filter" Width="100px" CausesValidation="False"
                            OnClientClick="this.disabled=true;Delete_Cookie('CompanyPerformanceFilterKey', '/', '');window.location.href=window.location.href;return false;"
                            EnableViewState="False" CssClass="pm-button" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlFilters" runat="server">
            <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                CssClass="CustomTabStyle">
                <AjaxControlToolkit:TabPanel runat="server" ID="tpMainFilters">
                    <HeaderTemplate>
                        <span class="bg DefaultCursor"><span class="NoHyperlink">Main filters</span></span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="project-filter">
                            <table class="personStats" cellpadding="5">
                                <tr>
                                    <td class="FirstTd">
                                        <asp:CheckBox ID="chbActive" runat="server" Text="Active Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbPeriodOnly" runat="server" Text="Total Only Selected Date Window"
                                            Checked="True" EnableViewState="False" onclick='<%# "excludeDualSelection(\"" + chbPrintVersion.ClientID + "\");return true;"%>' />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FirstTd">
                                        <asp:CheckBox ID="chbProjected" runat="server" Text="Projected Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbPrintVersion" runat="server" Text="Print version" EnableViewState="False"
                                            onclick='<%# "excludeDualSelection(\"" + chbPeriodOnly.ClientID + "\");return true;"%>' />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FirstTd">
                                        <asp:CheckBox ID="chbCompleted" runat="server" Text="Completed Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FirstTd">
                                        <asp:CheckBox ID="chbInternal" runat="server" Text="Internal Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FirstTd">
                                        <asp:CheckBox ID="chbExperimental" runat="server" Text="Experimental Projects" EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FirstTd">
                                        <asp:CheckBox ID="chbInactive" runat="server" Text="Inactive Projects" EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </AjaxControlToolkit:TabPanel>
            </AjaxControlToolkit:TabContainer>
        </asp:Panel>
        <asp:ValidationSummary ID="valsPerformance" runat="server" ValidationGroup="Filter"
            CssClass="searchError WholeWidth" />
    </div>
    <div class="xScrollAuto">
        <uc:PersonStats ID="repPersonStats" runat="server" />
    </div>
</asp:Content>

