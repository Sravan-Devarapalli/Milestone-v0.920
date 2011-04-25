<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PersonStats.aspx.cs" Inherits="PraticeManagement.Reporting.PersonStatsReport"
    MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register Src="~/Controls/Reports/PersonStats.ascx" TagPrefix="uc" TagName="PersonStats" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="uc1" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Reports - Person Stats</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    PersonStats
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <div class="filters">
        <div class="buttons-block">
            <table style="border: none; padding-left: 10px;" class="WholeWidth">
                <tr>
                    <td>
                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                            ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Expand Filters" />
                    </td>
                    <td style="width: 90px">
                        Select Dates
                    </td>
                    <td style="width: 40px; text-align: center;">
                        &nbsp;from&nbsp;
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
                        <asp:CustomValidator ID="custPeriod" runat="server" ErrorMessage="The Period Start must be less than or equal to the Period End"
                            ToolTip="The Period Start must be less than or equal to the Period End" Text="*"
                            EnableClientScript="False" ValidationGroup="Filter" OnServerValidate="custPeriod_ServerValidate"></asp:CustomValidator>
                        <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="False"
                            ErrorMessage="The period length must be not more then {0} months." ToolTip="The period length must be not more then {0} months."
                            Text="*" EnableClientScript="False" ValidationGroup="Filter"
                            OnServerValidate="custPeriodLengthLimit_ServerValidate"></asp:CustomValidator>
                    </td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Update View" Width="100px" OnClick="btnUpdateView_Click"
                            ValidationGroup="Filter" EnableViewState="False" CssClass="pm-button" />
                        <asp:Button ID="Button2" runat="server" Text="Reset Filter" Width="100px" CausesValidation="False"
                            OnClientClick="this.disabled=true;Delete_Cookie('CompanyPerformanceFilterKey', '/', '');window.location.href=window.location.href;return false;"
                            EnableViewState="False" CssClass="pm-button"/>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlFilters" runat="server">
            <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                CssClass="CustomTabStyle">
                <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                    <HeaderTemplate>
                        <span class="bg DefaultCursor"><span class="NoHyperlink">Main filters</span></span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="project-filter">
                            <table class="WholeWidth" cellpadding="5">
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbActive" runat="server" Text="Active Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbPeriodOnly" runat="server" Text="Total Only Selected Date Window"
                                            Checked="True" EnableViewState="False" onclick='<%# "excludeDualSelection(\"" + chbPrintVersion.ClientID + "\");return true;"%>' />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbProjected" runat="server" Text="Projected Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbPrintVersion" runat="server" Text="Print version" EnableViewState="False"
                                            onclick='<%# "excludeDualSelection(\"" + chbPeriodOnly.ClientID + "\");return true;"%>' />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbCompleted" runat="server" Text="Completed Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbInternal" runat="server" Text="Internal Projects" Checked="True"
                                            EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbExperimental" runat="server" Text="Experimental Projects" EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 10px;">
                                        <asp:CheckBox ID="chbInactive" runat="server" Text="Inactive Projects" EnableViewState="False" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>                
            </AjaxControlToolkit:TabContainer>
        </asp:Panel>
        <asp:ValidationSummary ID="valsPerformance" runat="server" Width="100%" ValidationGroup="Filter"
            CssClass="searchError" />
    </div>
    <div style="overflow-x:auto;">
        <uc:PersonStats ID="repPersonStats" runat="server" />
    </div>
</asp:Content>

