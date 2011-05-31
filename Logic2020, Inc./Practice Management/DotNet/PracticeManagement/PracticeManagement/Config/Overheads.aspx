<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Overheads.aspx.cs" Inherits="PraticeManagement.Config.Overheads" %>

<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Overheads</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Overheads
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="upnlOverHead" runat="server">
        <ContentTemplate>
            <div class="buttons-block">
                <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                    ImageControlID="btnExpandCollapseFilter" CollapsedImage="../Images/expand.jpg"
                    ExpandedImage="../Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                    ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                    ToolTip="Expand Filters" />
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
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbActive" runat="server" Checked='<%# !Convert.ToBoolean(Eval("Inactive")) %>'
                                Enabled="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Overhead Name">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle CssClass="LeftPadding10px" />
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEditOverhead" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Description")) %>'
                                CommandName="EditRecord" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Basis">
                        <ItemStyle CssClass="LeftPadding10px" />
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblBasis" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("RateType.Name")) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cost or Multiplier">
                        <ItemStyle CssClass="LeftPadding10px" />
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblCost" runat="server" Text='<%# Convert.ToBoolean(Eval("RateType.IsPercentage")) ? string.Format("{0}%", Eval("Rate.Value")): Eval("Rate") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="COGS or Expense">
                        <ItemStyle CssClass="LeftPadding10px" />
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblCogsOrExpense" runat="server" Text='<%# Convert.ToBoolean(Eval("IsCogs")) ? "COGS" : "Expense" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="W2-Hourly">
                        <ItemStyle CssClass="LeftPadding10px" />
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbHourly" runat="server" Checked='<%# ((Dictionary<TimescaleType, bool>)Eval("Timescales"))[TimescaleType.Hourly] %>'
                                Enabled="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="W2-Salary">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle CssClass="LeftPadding10px" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbSalary" runat="server" Checked='<%# ((Dictionary<TimescaleType, bool>)Eval("Timescales"))[TimescaleType.Salary] %>'
                                Enabled="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="1099">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chb1099" runat="server" Checked='<%# ((Dictionary<TimescaleType, bool>)Eval("Timescales"))[TimescaleType._1099Ctc] %>'
                                Enabled="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Start Date">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="End Date">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime?)Eval("EndDate")).HasValue ? ((DateTime?)Eval("EndDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <div style="padding-top: 10px;">
                <hr size="2" color="Black" align="center" style="margin-left: 5px; margin-right: 5px;">
            </div>
            <table class="CompPerfTable WholeWidth">
                <tr style="color: White; height: 1px !important; max-height: 1px !important">
                    <td>
                        Active
                    </td>
                    <td>
                        Overhead Name
                    </td>
                    <td>
                        Basis
                    </td>
                    <td>
                        Cost or Multiplier
                    </td>
                    <td>
                        COGS or Expense
                    </td>
                    <td>
                        W2-Hourly
                    </td>
                    <td>
                        W2-Salary
                    </td>
                    <td>
                        1099
                    </td>
                    <td>
                        Start Date
                    </td>
                    <td>
                        End Date
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chbMLFActive" runat="server" AutoPostBack="true" OnCheckedChanged="chbMLFActive_OnCheckedChanged" />
                    </td>
                    <td>
                        <asp:Label ID="Label1" runat="server" EnableViewState="false" Font-Bold="true" Text="Minimum Load Factor (MLF)"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="prmlbl" runat="server" EnableViewState="false" Text="Pay rate multiplier"></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td style="padding-left: 30px;">
                        <asp:Label ID="Label3" runat="server" EnableViewState="false" Text="COGS"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:DropDownList ID="ddlW2Hourly" runat="server" DataTextField="Name" DataValueField="Id"
                            AutoPostBack="true" OnSelectedIndexChanged="MLF_Changed">
                        </asp:DropDownList>
                    </td>
                    <td align="right">
                        <asp:DropDownList ID="ddlW2Salary" runat="server" DataTextField="Name" DataValueField="Id"
                            AutoPostBack="true" OnSelectedIndexChanged="MLF_Changed">
                        </asp:DropDownList>
                    </td>
                    <td align="right">
                        <asp:DropDownList ID="ddl1099" runat="server" DataTextField="Name" DataValueField="Id"
                            AutoPostBack="true" OnSelectedIndexChanged="MLF_Changed">
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                    <td align="right">
                        <asp:LinkButton ID="lnkBtnViewMLFHistory" OnClick="lnkBtnViewMLFHistory_OnClick"
                            runat="server" Text="View History"></asp:LinkButton>
                        <asp:HiddenField ID="hdnField" runat="server" />
                        <AjaxControlToolkit:ModalPopupExtender ID="mpeViewMLFHistory" runat="server" TargetControlID="hdnField"
                            CancelControlID="btnClose" BackgroundCssClass="modalBackground" PopupControlID="pnlViewMLFHistory"
                            DropShadow="false" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlViewMLFHistory" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass" Style="display: none" BorderWidth="2px">
                <table width="100%">
                    <tr style="height: 20px;">
                        <th align="right" style="padding-right: 3px; font-size: 14px; width: 100%">
                            <asp:Button ID="btnClose" ToolTip="Close" runat="server" CssClass="mini-report-close"
                                Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 6px 6px 2px 6px;">
                            <div style="overflow-y: auto; height: 300px;">
                                <asp:GridView ID="gvMLFHistory" runat="server" AutoGenerateColumns="false" EmptyDataText="There is nothing to be displayed here."
                                    CssClass="CompPerfTable WholeWidth" GridLines="None">
                                    <AlternatingRowStyle BackColor="#F9FAFF" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <div class="ie-bg">
                                                    W2-Salary</div>
                                            </HeaderTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblW2_Salary" runat="server" Text='<%# GetFormattedMLFRate(((Decimal)Eval("W2Salary_Rate"))) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <div class="ie-bg">
                                                    W2-Hourly</div>
                                            </HeaderTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblW2Hourly_Rate" runat="server" Text='<%# GetFormattedMLFRate(((Decimal)Eval("W2Hourly_Rate"))) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <div class="ie-bg">
                                                    1099 Hourly</div>
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_1099_Hourly_Rate" runat="server" Text='<%# GetFormattedMLFRate(((Decimal)Eval("_1099_Hourly_Rate"))) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <div class="ie-bg">
                                                    Start Date</div>
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <div class="ie-bg">
                                                    End Date</div>
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime?)Eval("EndDate")).HasValue ? ((DateTime?)Eval("EndDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

