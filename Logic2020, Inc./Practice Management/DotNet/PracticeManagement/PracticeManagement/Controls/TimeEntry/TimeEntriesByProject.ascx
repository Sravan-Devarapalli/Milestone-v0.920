<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntriesByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.TimeEntriesByProject" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<script type="text/javascript">
    function SelectAllPersons() {

        var btnReset = document.getElementById('<%=btnReset.ClientID %>');
        btnReset.disabled = 'disabled';
        var hdnResetFilter = document.getElementById('<%=hdnResetFilter.ClientID %>');
        hdnResetFilter.value = "false";

        var cblPersons = document.getElementById('<%=cblPersons.ClientID %>');
        if (cblPersons != null && cblPersons != "undefined") {
            var chkBoxes = cblPersons.getElementsByTagName('input');
            for (var i = 0; i < chkBoxes.length; i++) {
                chkBoxes[i].checked = true;
            }
        }

        return false;
    }


    function saveReportForExcel() {

        var lblProjectName = document.getElementById('<%= lblProjectName.ClientID %>');
        var lblGrandTotal = document.getElementById('<%= lblGrandTotal.ClientID %>');
        var divPersonListSummary = $("div[id$='divPersonListSummary']");
        var hdnSaveReportExcelText = document.getElementById('<%= hdnSaveReportExcelText.ClientID %>');
        var html = "";
        if (divPersonListSummary != null && divPersonListSummary.length > 0) {
            for (var i = 0; i < divPersonListSummary.length; i++) {
                var employeeNumberLabel = (divPersonListSummary[i].getElementsByTagName('span'))[0];
                employeeNumberLabel.style.display = '';
                html += "<b style='font-size:18px;'>" + lblProjectName.innerHTML + "</b>" + "<br /><br />" + divPersonListSummary[i].innerHTML;
                employeeNumberLabel.style.display = 'none';
            }
        }

        if (lblGrandTotal != null && lblGrandTotal != "undefined") {
            hdnSaveReportExcelText.value = html + "<b  style='text-align: right;font-size:25px;'> " + lblGrandTotal.innerHTML + "</b>";
        } else {
            hdnSaveReportExcelText.value = html;
        }
    }

    function saveReportForPDF() {
        var hdnGuid = document.getElementById('<%= hdnGuid.ClientID %>');
        var lblProjectName = document.getElementById('<%= lblProjectName.ClientID %>');
        var lblGrandTotal = document.getElementById('<%= lblGrandTotal.ClientID %>');
        var divPersonListSummary = $("div[id$='divPersonListSummary']");
        var hdnSaveReportPDFText = document.getElementById('<%= hdnSaveReportPDFText.ClientID %>');
        var html = "";

        if (divPersonListSummary != null && divPersonListSummary.length > 0) {
            for (var i = 0; i < divPersonListSummary.length; i++) {

                html += "<b style='font-size:18px;'>" + lblProjectName.innerHTML + "</b>" + "<br /><br />" + divPersonListSummary[i].innerHTML + hdnGuid.value;
            }
        }
        if (lblGrandTotal != null && lblGrandTotal != "undefined") {
            hdnSaveReportPDFText.value = html + "<b  style='text-align: right;font-size:25px;'> " + lblGrandTotal.innerHTML + "</b>";
        } else {
            hdnSaveReportPDFText.value = html;
        }

    }

    function EnableOrDisableResetFilterButton() {
        var btnReset = document.getElementById('<%=btnReset.ClientID %>');
        var cblPersons = document.getElementById('<%=cblPersons.ClientID %>');
        var hdnResetFilter = document.getElementById('<%=hdnResetFilter.ClientID %>');


        var AllSelected = true;
        if (cblPersons != null) {
            var chkBoxes = cblPersons.getElementsByTagName('input');
            for (var i = 0; i < chkBoxes.length; i++) {
                if (chkBoxes[i].checked != true) {
                    AllSelected = false;
                }
            }
        }

        if (AllSelected) {
            btnReset.disabled = 'disabled';
            hdnResetFilter.value = "false";
        }
        else {
            btnReset.disabled = '';
            hdnResetFilter.value = "true";
        }
    }


</script>
<uc3:LoadingProgress ID="LoadingProgress1" runat="server" />
<asp:UpdatePanel ID="updReport" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hdnResetFilter" Value="false" runat="server" />
        <asp:Panel ID="pnlFilters" runat="server" CssClass="buttons-block">
            <table class="opportunity-description">
                <tr>
                    <td class="vTopImp">
                        <table class="opportunity-description">
                            <tr>
                                <td>
                                    Account
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlClients" runat="server" CssClass="Width460Px" OnSelectedIndexChanged="ddlClients_OnSelectedIndexChanged"
                                        AutoPostBack="true">
                                    </asp:DropDownList>
                                </td>
                                <tr>
                                    <td>
                                        Project
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlProjects" runat="server" Enabled="false" AutoPostBack="true"
                                            CssClass="Width460Px" OnSelectedIndexChanged="ddlProjects_OnSelectedIndexChanged">
                                            <asp:ListItem Text="-- Select a Project --" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbActiveInternalProject" runat="server" Checked="true" CssClass="CheckBoxFloatLeft"
                                            Text="Active and Internal Projects Only" AutoPostBack="true" OnCheckedChanged="ddlClients_OnSelectedIndexChanged">
                                        </asp:CheckBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Milestone
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlMilestones" runat="server" Enabled="false" AutoPostBack="true"
                                            CssClass="Width460Px" OnSelectedIndexChanged="ddlMilestones_OnSelectedIndexChanged">
                                            <asp:ListItem Text="-- Select a Milestone --" Value="-1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table>
                                            <tr>
                                                <td class="PaddingTop30Imp">
                                                    Show only persons with time entered
                                                </td>
                                                <td class="PaddingTop30Imp textRightImp" id="report-date-interval">
                                                    <uc:DateInterval ID="diRange" runat="server" FromToDateFieldCssClass="Width70PxImp" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                        </table>
                    </td>
                    <td class="vTopImp">
                        <table class="opportunity-description">
                            <tr>
                                <td class="vTopImp paddingLeft5pxImp">
                                    <asp:Label ID="lblPersons" runat="server" Text="Persons"></asp:Label>
                                </td>
                                <td class="LeftPadding10px">
                                    <pmc:ScrollingDropDown ID="cblPersons" runat="server" AllSelectedReturnType="AllItems"
                                        CellPadding="3" NoItemsType="All" SetDirty="False" CssClass="CblPersons">
                                    </pmc:ScrollingDropDown>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnUpdate" runat="server" Text="Update View" OnClick="btnUpdate_OnClick" />
                                </td>
                                <td>
                                    <asp:Button ID="btnReset" runat="server" Text="Reset Filter" Enabled="false" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:ValidationSummary ID="valSum" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnExportToExcel" runat="server" Text="Export To Excel" OnClientClick="saveReportForExcel();"
                                        Enabled="false" OnClick="btnExport_OnClick" EnableViewState="False" />
                                    <asp:HiddenField ID="hdnSaveReportExcelText" runat="server" />
                                </td>
                                <td>
                                    <asp:Button ID="btnExportToPDF" runat="server" Text="Export To PDF" OnClientClick="saveReportForPDF();"
                                        Enabled="false" OnClick="ExportToPDF" EnableViewState="False" /><asp:HiddenField
                                            ID="hdnSaveReportPDFText" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:ObjectDataSource ID="odsProjects" runat="server" SelectMethod="GetAllTimeEntryProjects"
                TypeName="PraticeManagement.TimeEntryService.TimeEntryServiceClient" />
        </asp:Panel>
        <h2>
            <asp:Label ID="lblProjectName" runat="server" Visible="false" /></h2>
        <asp:DataList ID="dlTimeEntries" OnItemDataBound="dlTimeEntries_OnItemDataBound"
            runat="server" CssClass="WholeWidth">
            <ItemTemplate>
                <div id="divPersonListSummary" runat="server">
                    <h3 class="fontBold">
                        <asp:Label ID="lblEmployeeNumber" runat="server" Style="display: none;" Text='<%# GetEmployeeNumber(Eval("Key.Id").ToString()) %>'></asp:Label>
                        <asp:Label ID="lblPersonName" runat="server" Text='<%# Eval("Key.HtmlEncodedName") %>' /></h3>
                    <br class="NotVisible" />
                    <asp:GridView ID="gvPersonTimeEntries" runat="server" DataSource='<%# Eval("Value") %>'
                        AutoGenerateColumns="false" CssClass="bgcolorwhite CompPerfTable WholeWidth"
                        GridLines="Both" OnRowDataBound="gvPersonTimeEntries_OnRowDataBound" ShowFooter="true"
                        OnDataBound="gvPersonTimeEntries_OnDataBound" EmptyDataText='<%# GetEmptyDataText() %>'>
                        <AlternatingRowStyle CssClass="alterrow" />
                        <FooterStyle CssClass="fontBold TextAlignCenterImp" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <div class="ie-bg fontBold TextAlignCenterImp ValignMiddleImp">
                                        Date</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# ((TimeEntryRecord)Container.DataItem).ChargeCodeDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                                </ItemTemplate>
                                <HeaderStyle CssClass="TextAlignCenterImp ValignMiddleImp" />
                                <ItemStyle CssClass="ValignMiddleImp t-left Width100PxImp no-wrap" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <div class="ie-bg fontBold TextAlignCenterImp ValignMiddleImp">
                                        Note</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("HtmlEncodedNote")%>
                                </ItemTemplate>
                                <ItemStyle CssClass="ValignMiddleImp" />
                                <HeaderStyle CssClass="TextAlignCenterImp ValignMiddleImp" />
                                <FooterStyle CssClass="ValignMiddleImp textRightImp BorderNone" />
                                <FooterTemplate>
                                    Total:</FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <div class="ie-bg fontBold TextAlignCenterImp ValignMiddleImp">
                                        Hours</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="TextAlignCenterImp">
                                        <%#((TimeEntryRecord)Container.DataItem).ActualHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%></div>
                                </ItemTemplate>
                                <HeaderStyle CssClass="TextAlignCenterImp ValignMiddleImp" />
                                <FooterStyle CssClass="ValignMiddleImp TextAlignCenterImp BorderNone" />
                                <ItemStyle CssClass="TextAlignCenterImp no-wrap Width50PxImp" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </ItemTemplate>
        </asp:DataList>
        <h3 class="textRightImp">
            <asp:Label ID="lblGrandTotal" runat="server" /></h3>
        <asp:HiddenField ID="hdnGuid" runat="server" />
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="btnExportToExcel" />
        <asp:PostBackTrigger ControlID="btnExportToPDF" />
    </Triggers>
</asp:UpdatePanel>

