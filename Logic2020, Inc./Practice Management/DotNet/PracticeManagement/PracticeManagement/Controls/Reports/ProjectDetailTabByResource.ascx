<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectDetailTabByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectDetailTabByResource" %>
<%@ Import Namespace="DataTransferObjects.Reports" %>
<asp:HiddenField ID="hdncpeExtendersIds" runat="server" Value="" />
<asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
<table id="tblExportSection" runat="server" class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" class="Width90Percent">
            <asp:Button ID="btnExpandOrCollapseAll" runat="server" Text="Collapse All" UseSubmitBehavior="false"
                CssClass="Width100Px" ToolTip="Collapse All" />
            &nbsp;&nbsp;
            <asp:Button ID="btnGroupBy" runat="server" Text="Group By Date" UseSubmitBehavior="false"
                CssClass="Width130px" OnClick="btnGroupBy_OnClick" ToolTip="Group By Date" />
            <asp:HiddenField ID="hdnGroupBy" runat="server" />
        </td>
        <td class="Width10Percent padRight5">
            <table class="WholeWidth">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To Excel" />
                    </td>
                    <td>
                        <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Repeater ID="repPersons" runat="server" OnItemDataBound="repPersons_ItemDataBound">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="WholeWidthWithHeight">
            <tr class="textLeft">
                <td colspan="4" class="ProjectAccountName Width95Percent no-wrap">               
                    <asp:Label ID="lbProject" Style="display: none;" runat="server"></asp:Label>
                    <%# Eval("Person.HtmlEncodedName")%>
                    <b class="fontStyleNormal">
                        <%# GetPersonRole((string)Eval("Person.ProjectRoleName"))%></b>
                    <asp:Image ID="imgOffshore" runat="server" ImageUrl="~/Images/Offshore_Icon.png"
                        ToolTip="Resource is an offshore employee" Visible='<%# (bool)Eval("Person.IsOffshore")%>' />
                </td>
                <td class="PersonDetailTotal">
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlProjectDetails" runat="server" CssClass="bg-white">
            <asp:Repeater ID="repDate" runat="server" OnItemDataBound="repDate_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="WholeWidthWithHeight">
                        <tr class="textLeft bgColorD4D0C9">
                            <td class="Width80Percent padLeft20Imp">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDate" runat="Server" CollapsedText="Expand Date Details"
                                    ExpandedText="Collapse Date Details" EnableViewState="true" BehaviorID="cpeDate"
                                    Collapsed="true" TargetControlID="pnlDateDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                                    ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                                    TextLabelID="lbDate" />
                                <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                                <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                                <%# GetDateFormat((DateTime)Eval("Date"))%>
                            </td>
                            <td class="Width10Percent textRight vMiddle">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="textRight fontBold">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                        <td class="Width20Px">
                                            <asp:Image ID="imgNonBillable" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                                ToolTip="Non-Billable hours are included." Visible='<%# GetNonBillableImageVisibility((double)Eval("NonBillableHours"))%>' />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width80Px">
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlDateDetails" runat="server">
                        <table class="WholeWidthWithHeight WorkTypeTable">
                            <asp:Repeater ID="repWorktype" runat="server">
                                <ItemTemplate>
                                    <tr class="FirstTr">
                                        <td class="FirstTrTd1">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td class="FirstTrTd2">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td class="FirstTrTd3">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" class="FirstTrTd4">
                                        </td>
                                    </tr>
                                    <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td class="wrapword SecondTrTd1">
                                            <table>
                                                <tr>
                                                    <td class="SecondTrTd2">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td class="vTopImp">
                                                        <%# Eval("HtmlEncodedHTMLNote")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr class="FirstTr AlternativeFirstTr">
                                        <td class="FirstTrTd1">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td class="FirstTrTd2">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td class="FirstTrTd3">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" class="FirstTrTd4">
                                        </td>
                                    </tr>
                                    <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td class="wrapword SecondTrTd1">
                                            <table>
                                                <tr>
                                                    <td class="SecondTrTd2">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td class="vTopImp">
                                                        <%# Eval("HtmlEncodedHTMLNote")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </table>
                    </asp:Panel>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="WholeWidthWithHeight">
                        <tr class="textLeft bgcolor_ECE9D9">
                            <td class="Width80Percent padLeft20Imp">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDate" runat="Server" CollapsedText="Expand Date Details"
                                    ExpandedText="Collapse Date Details" EnableViewState="true" BehaviorID="cpeDate"
                                    Collapsed="true" TargetControlID="pnlDateDetails" ImageControlID="imgDate" CollapsedImage="~/Images/expand.jpg"
                                    ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDate" ExpandControlID="imgDate"
                                    TextLabelID="lbDate" />
                                <asp:Image ID="imgDate" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Date Details" />
                                <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                                <%# GetDateFormat((DateTime)Eval("Date"))%>
                            </td>
                            <td class="Width10Percent textRight vMiddle">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="textRight fontBold">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                        <td class="Width20Px">
                                            <asp:Image ID="imgNonBillable" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                                ToolTip="Non-Billable hours are included." Visible='<%# GetNonBillableImageVisibility((double)Eval("NonBillableHours"))%>' />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width80Px">
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlDateDetails" runat="server">
                        <table class="WholeWidthWithHeight WorkTypeTable">
                            <asp:Repeater ID="repWorktype" runat="server">
                                <ItemTemplate>
                                    <tr class="FirstTr">
                                        <td class="FirstTrTd1">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td class="FirstTrTd2">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td class="FirstTrTd3">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" class="FirstTrTd4">
                                        </td>
                                    </tr>
                                    <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td class="wrapword SecondTrTd1">
                                            <table>
                                                <tr>
                                                    <td class="SecondTrTd2">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td class="vTopImp">
                                                        <%# Eval("HtmlEncodedHTMLNote")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr class="FirstTr AlternativeFirstTr">
                                        <td class="FirstTrTd1">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td class="FirstTrTd2">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td class="FirstTrTd3">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" class="FirstTrTd4">
                                        </td>
                                    </tr>
                                    <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td class="wrapword SecondTrTd1">
                                            <table>
                                                <tr>
                                                    <td class="SecondTrTd2">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td class="vTopImp">
                                                        <%# Eval("HtmlEncodedHTMLNote")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </table>
                    </asp:Panel>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
<asp:Repeater ID="repDate2" runat="server" OnItemDataBound="repDate2_ItemDataBound">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="WholeWidthWithHeight">
            <tr class="textLeft">
                <td colspan="4" class="ProjectAccountName Width95Percent no-wrap">                   
                    <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                    <%# GetDateFormat((DateTime)Eval("Date"))%>
                </td>
                <td class="PersonDetailTotal">
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlDate2Details" runat="server" CssClass="bg-white">
            <asp:Repeater ID="repPerson2" runat="server" OnItemDataBound="repPerson2_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table class="WholeWidthWithHeight">
                        <tr class="textLeft bgColorD4D0C9">
                            <td class="Width80Percent padLeft20Imp">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpePerson" runat="Server" CollapsedText="Expand Person Details"
                                    ExpandedText="Collapse Person Details" EnableViewState="false" BehaviorID="cpePerson"
                                    Collapsed="true" TargetControlID="pnlProject2Details" ImageControlID="imgProject"
                                    CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgProject"
                                    ExpandControlID="imgProject" TextLabelID="lbProject" />
                                <asp:Image ID="imgProject" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Person Details" />
                                <asp:Label ID="lbProject" Style="display: none;" runat="server"></asp:Label>
                                <%# Eval("Person.HtmlEncodedName")%>
                                <b class="fontStyleNormal">
                                    <%# GetPersonRole((string)Eval("Person.ProjectRoleName"))%></b>
                                <asp:Image ID="imgOffshore" runat="server" ImageUrl="~/Images/Offshore_Icon.png"
                                    ToolTip="Resource is an offshore employee" Visible='<%# (bool)Eval("Person.IsOffshore")%>' />
                            </td>
                            <td class="Width10Percent textRight vMiddle">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="textRight fontBold">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                        <td class="Width20Px">
                                            <asp:Image ID="imgNonBillable" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                                ToolTip="Non-Billable hours are included." Visible='<%# GetNonBillableImageVisibility((double)Eval("NonBillableHours"))%>' />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width80Px">
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlProject2Details" runat="server">
                        <table class="WholeWidthWithHeight WorkTypeTable">
                            <asp:Repeater ID="repWorktype" runat="server">
                                <ItemTemplate>
                                    <tr class="FirstTr">
                                        <td class="FirstTrTd1">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td class="FirstTrTd2">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td class="FirstTrTd3">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" class="FirstTrTd4">
                                        </td>
                                    </tr>
                                    <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td class="wrapword SecondTrTd1">
                                            <table>
                                                <tr>
                                                    <td class="SecondTrTd2">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td class="vTopImp">
                                                        <%# Eval("HtmlEncodedHTMLNote")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr class="FirstTr AlternativeFirstTr">
                                        <td class="FirstTrTd1">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td class="FirstTrTd2">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td class="FirstTrTd3">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" class="FirstTrTd4">
                                        </td>
                                    </tr>
                                    <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td class="wrapword SecondTrTd1">
                                            <table>
                                                <tr>
                                                    <td class="SecondTrTd2">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td class="vTopImp">
                                                        <%# Eval("HtmlEncodedHTMLNote")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </table>
                    </asp:Panel>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <table class="WholeWidthWithHeight">
                        <tr class="textLeft bgcolor_ECE9D9">
                            <td class="Width80Percent padLeft20Imp">
                                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpePerson" runat="Server" CollapsedText="Expand Person Details"
                                    ExpandedText="Collapse Person Details" EnableViewState="false" BehaviorID="cpePerson"
                                    Collapsed="true" TargetControlID="pnlProject2Details" ImageControlID="imgProject"
                                    CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgProject"
                                    ExpandControlID="imgProject" TextLabelID="lbProject" />
                                <asp:Image ID="imgProject" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Person Details" />
                                <asp:Label ID="lbProject" Style="display: none;" runat="server"></asp:Label>
                                <%# Eval("Person.HtmlEncodedName")%>
                                <b class="fontStyleNormal">
                                    <%# GetPersonRole((string)Eval("Person.ProjectRoleName"))%></b>
                                <asp:Image ID="imgOffshore" runat="server" ImageUrl="~/Images/Offshore_Icon.png"
                                    ToolTip="Resource is an offshore employee" Visible='<%# (bool)Eval("Person.IsOffshore")%>' />
                            </td>
                            <td class="Width10Percent textRight vMiddle">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="textRight fontBold">
                                            <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                                        </td>
                                        <td class="Width20Px">
                                            <asp:Image ID="imgNonBillable" runat="server" ImageUrl="~/Images/Non-Billable-Icon.png"
                                                ToolTip="Non-Billable hours are included." Visible='<%# GetNonBillableImageVisibility((double)Eval("NonBillableHours"))%>' />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width80Px">
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlProject2Details" runat="server">
                        <table class="WholeWidthWithHeight WorkTypeTable">
                            <asp:Repeater ID="repWorktype" runat="server">
                                <ItemTemplate>
                                    <tr class="FirstTr">
                                        <td class="FirstTrTd1">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td class="FirstTrTd2">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td class="FirstTrTd3">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" class="FirstTrTd4">
                                        </td>
                                    </tr>
                                    <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td class="wrapword SecondTrTd1">
                                            <table>
                                                <tr>
                                                    <td class="SecondTrTd2">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td class="vTopImp">
                                                        <%# Eval("HtmlEncodedHTMLNote")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr class="FirstTr AlternativeFirstTr">
                                        <td class="FirstTrTd1">
                                            <%# Eval("TimeType.Name")%>
                                        </td>
                                        <td class="FirstTrTd2">
                                            B -
                                            <%# GetDoubleFormat( (double)Eval("BillableHours")) %>
                                        </td>
                                        <td class="FirstTrTd3">
                                            NB -
                                            <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                                        </td>
                                        <td colspan="2" class="FirstTrTd4">
                                        </td>
                                    </tr>
                                    <tr class="SecondTr" id="trNote" runat="server" visible='<%# (bool)GetNoteVisibility((String)Eval("Note"))%>'>
                                        <td class="wrapword SecondTrTd1">
                                            <table>
                                                <tr>
                                                    <td class="SecondTrTd2">
                                                        <b>NOTE:&nbsp;</b>
                                                    </td>
                                                    <td class="vTopImp">
                                                        <%# Eval("HtmlEncodedHTMLNote")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </table>
                    </asp:Panel>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
    runat="server">
    There are no Time Entries towards this project.
</div>

