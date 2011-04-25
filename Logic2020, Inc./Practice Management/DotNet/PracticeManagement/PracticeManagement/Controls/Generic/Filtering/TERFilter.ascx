<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TERFilter.ascx.cs" Inherits="PraticeManagement.Controls.Generic.Filtering.TERFilter" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.Filtering"
    TagPrefix="cc1" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/Generic/Filtering/RealInterval.ascx" TagPrefix="uc"
    TagName="RealInterval" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<div class="buttons-block">
    <table class="WholeWidth">
        <tr>
            <td style="vertical-align: middle">
                <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                    ToolTip="Expand Filters" />
                <asp:Label ID="lblFilter" ToolTip="Show Filters" runat="server" />
            </td>
            <td>
                <uc:DateInterval ID="diWeek" runat="server" FromToDateFieldWidth="70" />
            </td>
            <td style="width: 350px;">
                &nbsp;
            </td>
            <td>
                <asp:Button ID="btnApply" runat="server" Text="Update" OnClick="btnApply_OnClick" />
            </td>
            <td>
                <asp:Button ID="btnReset" runat="server" Text="Reset Filters" OnClick="btnReset_OnClick" />
            </td>
        </tr>
    </table>
    <act:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
        ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
        ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
</div>
<asp:Panel ID="pnlFilters" runat="server">
    <act:TabContainer ID="tabFilterAndSotring" runat="server" CssClass="CustomTabStyle"
        ActiveTabIndex="0">
        <act:TabPanel ID="tpnlBscFilters" runat="server">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Basic Filters</span></a></span>
            </HeaderTemplate>
            <ContentTemplate>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <table class="tem-filter-table">
                                <tr class="tem-filter-row">
                                    <td style="width:70px;">
                                        <strong>Persons</strong>
                                    </td>
                                    <td>
                                        <cc2:ScrollingDropDown CssClass="Zindex" ID="cblPersons" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems" onclick="scrollingDropdown_onclick('cblPersons','Person')"
                                            BackColor="White" CellPadding="3" NoItemsType="All" SetDirty="False" Width="350px" DropDownListType="Person"
                                            BorderWidth="0" Height="303px">
                                        </cc2:ScrollingDropDown>
                                        <ext:ScrollableDropdownExtender ID="sdePersons" runat="server" TargetControlID="cblPersons" Width="250px" UseAdvanceFeature="true"
                                            EditImageUrl="~/Images/Dropdown_Arrow.png">
                                        </ext:ScrollableDropdownExtender>
                                    </td>
                                    <td style="width:70px;padding-left:30px;">
                                        <strong>Milestone</strong>
                                    </td>
                                    <td>
                                        <cc1:SingleOption ID="soMilestones" runat="server" DataSourceID="odsMilestones" DataTextField="MilestoneProjectTitle"
                                            DataValueField="Id" NeedFirstItem="True" Selected="" Width="350px">
                                        </cc1:SingleOption>
                                    </td>
                                </tr>
                                <tr class="tem-filter-row">
                                    <td style="width:70px;">
                                        <strong>Projects</strong>
                                    </td>
                                    <td>
                                        <cc2:ScrollingDropDown ID="cblProjects" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                            BackColor="White" CellPadding="3" NoItemsType="All" SetDirty="False" Width="350px" Height="280px"
                                            BorderWidth="0">
                                        </cc2:ScrollingDropDown>
                                        <ext:ScrollableDropdownExtender ID="sdeProjects" runat="server" TargetControlID="cblProjects" Width="250px" UseAdvanceFeature="false"
                                            DisplayText="Please Choose Projects" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                        </ext:ScrollableDropdownExtender>
                                    </td>
                                    <td style="width:70px;padding-left:30px;">
                                        <strong>Correct</strong>
                                    </td>
                                    <td class="tem-filter-space-cell">
                                        <asp:DropDownList ID="ddlIsCorrect" runat="server" Width="350px">
                                            <asp:ListItem Selected="True" Text="All"></asp:ListItem>
                                            <asp:ListItem Text="Correct only" Value="True"></asp:ListItem>
                                            <asp:ListItem Text="Incorrect only" Value="False"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" HeaderText="Filters" ID="tpnlFilters">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Advanced Filters</span></a></span>
            </HeaderTemplate>
            <ContentTemplate>
                <table class="tem-filter-table">
                    <tr class="tem-filter-row">
                        <td class="tem-filter-title-cell">
                            Actual Hours
                        </td>
                        <td class="tem-filter-control-cell">
                            <uc:RealInterval ID="riActual" runat="server" />
                        </td>
                        <td class="tem-filter-space-cell">
                        </td>
                        <td class="tem-filter-title-cell">
                            Billable Hours
                        </td>
                        <td class="tem-filter-control-cell">
                            <asp:DropDownList ID="ddlIsChargable" runat="server" Width="250px">
                                <asp:ListItem Text="Select..." Selected="True" />
                                <asp:ListItem Text="Yes" Value="True" />
                                <asp:ListItem Text="No" Value="False" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="tem-filter-row">
                        <td class="tem-filter-title-cell">
                            Forecasted Hours
                        </td>
                        <td class="tem-filter-control-cell">
                            <uc:RealInterval ID="riForecasted" runat="server" />
                        </td>
                        <td class="tem-filter-space-cell">
                        </td>
                        <td class="tem-filter-title-cell">
                            Review Status
                        </td>
                        <td class="tem-filter-control-cell">
                            <cc1:SingleOption ID="ddlIsReviewed" runat="server" Width="250px" DataSourceID="odsReviewTypes"
                                NeedFirstItem="True" />
                        </td>
                    </tr>
                    <tr class="tem-filter-row">
                        <td class="tem-filter-title-cell">
                            Milestone Entered
                        </td>
                        <td class="tem-filter-control-cell">
                            <uc:DateInterval ID="diEntered" runat="server" FromToDateFieldWidth="70"></uc:DateInterval>
                        </td>
                        <td class="tem-filter-space-cell">
                        </td>
                        <td class="tem-filter-title-cell">
                            Notes
                        </td>
                        <td class="tem-filter-control-cell">
                            <asp:TextBox ID="tbNotes" runat="server" Width="245px" />
                            <asp:RegularExpressionValidator ID="valNotes" runat="server" ControlToValidate="tbNotes"
                                ValidationGroup='<%# ClientID %>' ErrorMessage="Notes filter should be just letters (a-Z), digits (0-9) or space. Less than 25 symbols long."
                                ValidationExpression="^([0-9a-zA-Z\u0020]){0,25}$">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr class="tem-filter-row">
                        <td class="tem-filter-title-cell">
                            Last Modified
                        </td>
                        <td class="tem-filter-control-cell">
                            <uc:DateInterval ID="diLastModified" runat="server" FromToDateFieldWidth="70"></uc:DateInterval>
                        </td>
                        <td class="tem-filter-space-cell">
                            &nbsp;
                        </td>
                        <td class="tem-filter-title-cell">
                            Time Type
                        </td>
                        <td class="tem-filter-control-cell">
                            <cc1:SingleOption ID="soTimeTypes" runat="server" DataSourceID="odsTimeTypes" DataTextField="Name"
                                DataValueField="Id" NeedFirstItem="True" Width="250px" />
                        </td>
                    </tr>
                    <tr class="tem-filter-row">
                        <td class="tem-filter-title-cell" colspan="3">
                        </td>
                        <td class="tem-filter-title-cell">
                            Billable Projects
                        </td>
                        <td class="tem-filter-control-cell">
                            <asp:DropDownList ID="ddlIsProjectChargeable" runat="server" Width="250px">
                                <asp:ListItem Text="Select..." Selected="True" />
                                <asp:ListItem Text="Yes" Value="True" />
                                <asp:ListItem Text="No" Value="False" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </act:TabPanel>
    </act:TabContainer>
</asp:Panel>

