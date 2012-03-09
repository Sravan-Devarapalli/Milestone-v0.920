<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonDetailReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.PersonDetailReport" %>
<%@ Import Namespace="DataTransferObjects.Reports" %>
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" style="width:90%;">
        </td>
        <td style="text-align:right;width:90%;" >
            <table width="100%" style="text-align: right;">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExcel" runat="server" Text="Excel" />
                    </td>
                    <td>
                        <asp:Button ID="btnPDF" runat="server" Text="PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    </table>
    <asp:Repeater ID="repProjects" runat="server" OnItemDataBound="repProjects_ItemDataBound">
        <ItemTemplate>
            <table class="WholeWidthWithHeight">
            <tr style="text-align: left; background-color: #e2ebff;">
                <td colspan="4" class="ProjectAccountName" style="width:95%">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeProject" runat="Server" CollapsedText="Expand Project Details"
                        ExpandedText="Collapse Project Details" EnableViewState="false" BehaviorID="cpeProject"
                        TargetControlID="pnlProjectDetails" ImageControlID="imgProject" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgProject" ExpandControlID="imgProject" TextLabelID="lbProject" />
                        
                    <asp:Image ID="imgProject" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Project Details" />
                    <asp:Label ID="lbProject" Style="display: none;" runat="server"></asp:Label>
                    <%# Eval("Client.Name") %>
                    >
                    <%# Eval("Project.ProjectNumber")%>
                    -
                    <%# Eval("Project.Name")%>
                </td>
                <td style="width:5%">
                    <%# Eval("TotalHours")%>
                </td>
            </tr>
            </table>
            <asp:Panel ID="pnlProjectDetails" runat="server" CssClass="cp bg-white">
                    <asp:Repeater ID="repDate" runat="server" OnItemDataBound="repDate_ItemDataBound">
                        <ItemTemplate>
                        <table class="WholeWidthWithHeight">
                            <tr style="text-align: left; background-color: Gray;">
                                <td colspan="3" style="width:90%;padding-left:20px;" >
                                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDate" runat="Server" CollapsedText="Expand Date Details"
                                        ExpandedText="Collapse Date Details" EnableViewState="false" BehaviorID="cpeDate"
                                        TargetControlID="pnlDateDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate" TextLabelID="lbDate"/>
                                    <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                                    <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                                    <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%>
                                </td>
                                <td style="width:10%;" colspan="2">
                                    <%# Eval("TotalHours")%>
                                </td>
                            </tr>
                            </table>
                            <asp:Panel ID="pnlDateDetails" runat="server">
                                <table class="WholeWidthWithHeight">
                                    <asp:Repeater ID="repWorktype" runat="server">
                                        <ItemTemplate>
                                            <tr style="text-align: left; background-color: White;">
                                                <td style ="width:70%;padding-left:50px;">
                                                    <%# Eval("TimeType.Name")%>
                                                </td>
                                                <td style ="width:5%">
                                                    B -
                                                    <%# Eval("BillableHours")%>
                                                </td>
                                                <td style ="width:5%">
                                                    NB -
                                                    <%# Eval("NonBillableHours")%>
                                                </td>
                                                <td colspan="2" style ="width:20%">
                                                </td>
                                            </tr>
                                            <tr style="text-align: left; background-color: White;">
                                                <td style="padding-left:50px;" >
                                                    <%# Eval("Note")%>
                                                </td>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <AlternatingItemTemplate>
                                              <tr style="text-align: left; background-color: White;">
                                                <td style ="width:70%">
                                                    <%# Eval("TimeType.Name")%>
                                                </td>
                                                <td style ="width:5%">
                                                    B -
                                                    <%# Eval("BillableHours")%>
                                                </td>
                                                <td style ="width:5%">
                                                    NB -
                                                    <%# Eval("NonBillableHours")%>
                                                </td>
                                                <td colspan="2" style ="width:20%">
                                                </td>
                                            </tr>
                                            <tr style="text-align: left; background-color: White;">
                                                <td>
                                                    <%# Eval("Note")%>
                                                </td>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                        </AlternatingItemTemplate>
                                    </asp:Repeater>
                                </table>
                            </asp:Panel>
                        </ItemTemplate>
                    </asp:Repeater>
            </asp:Panel>
        </ItemTemplate>
    </asp:Repeater>


