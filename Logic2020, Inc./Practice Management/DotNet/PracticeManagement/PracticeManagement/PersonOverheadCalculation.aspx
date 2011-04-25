<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="PersonOverheadCalculation.aspx.cs" Inherits="PraticeManagement.PersonOverheadCalculation"
    Title="Practice Management - Overhead Calculation" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons" Assembly="PraticeManagement" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Overhead Calculation</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Person Overhead Calculations
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <div class="buttons-block">
         <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server"
                        TargetControlID="pnlFilters" ImageControlID="btnExpandCollapseFilter"
                        CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                        ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />                        
        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Filters" />
        <asp:ShadowedTextButton ID="btnAddOverhead" runat="server" Text="Add Overhead" OnClick="btnAddOverhead_Click"
            CssClass="add-btn" />
    </div>
    <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
        <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
            CssClass="CustomTabStyle">
            <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                <HeaderTemplate>
                    <span class="bg  DefaultCursor"><span class="NoHyperlink">Filters</span></span>
                </HeaderTemplate>
                <ContentTemplate>
                    <asp:CheckBox ID="chbActive" runat="server" Checked="true" Text="Show active only"
                        AutoPostBack="true" OnCheckedChanged="chbActive_CheckedChanged" /></ContentTemplate>
            </ajaxToolkit:TabPanel>
        </AjaxControlToolkit:TabContainer>
    </asp:Panel>
    <br />
    <asp:GridView ID="gvOverhead" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
        EmptyDataText="There is nothing to be displayed here." OnRowCommand="gvOverhead_RowCommand"
        CssClass="CompPerfTable WholeWidth" GridLines="None">
        <AlternatingRowStyle BackColor="#F9FAFF" />
        <Columns>
            <asp:TemplateField HeaderText="Active">
                <ItemTemplate>
                    <asp:CheckBox ID="chbActive" runat="server" Checked='<%# !Convert.ToBoolean(Eval("Inactive")) %>'
                        Enabled="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Overhead Name">
                <ItemTemplate>
                    <asp:LinkButton ID="btnEditOverhead" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Description")) %>'
                        CommandName="EditRecord" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Basis">
                <ItemTemplate>
                    <asp:Label ID="lblBasis" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("RateType.Name")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Cost or Multiplier">
                <ItemTemplate>
                    <asp:Label ID="lblCost" runat="server" Text='<%# Convert.ToBoolean(Eval("RateType.IsPercentage")) ? string.Format("{0}%", Eval("Rate.Value")): Eval("Rate") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="COGS or Expense">
                <ItemTemplate>
                    <asp:Label ID="lblCogsOrExpense" runat="server" Text='<%# Convert.ToBoolean(Eval("IsCogs")) ? "COGS" : "Expense" %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Overhead per hour">
                <ItemTemplate>
                    <asp:Label ID="lblCostHourly" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RatePerHour") == null ? string.Empty : Eval("RatePerHour") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="W2-Hourly">
                <ItemTemplate>
                    <asp:CheckBox ID="chbHourly" runat="server" Checked='<%# ((Dictionary<TimescaleType, bool>)Eval("Timescales"))[TimescaleType.Hourly] %>'
                        Enabled="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="W2-Salary">
                <ItemTemplate>
                    <asp:CheckBox ID="chbSalary" runat="server" Checked='<%# ((Dictionary<TimescaleType, bool>)Eval("Timescales"))[TimescaleType.Salary] %>'
                        Enabled="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="1099">
                <ItemTemplate>
                    <asp:CheckBox ID="chb1099" runat="server" Checked='<%# ((Dictionary<TimescaleType, bool>)Eval("Timescales"))[TimescaleType._1099Ctc] %>'
                        Enabled="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Start Date">
                <ItemTemplate>
                    <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("d") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End Date">
                <ItemTemplate>
                    <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime?)Eval("EndDate")).HasValue ? ((DateTime?)Eval("EndDate")).Value.ToString("d") : string.Empty %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>

