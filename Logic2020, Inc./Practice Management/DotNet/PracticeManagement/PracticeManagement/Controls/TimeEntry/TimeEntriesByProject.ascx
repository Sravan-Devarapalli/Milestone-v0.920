<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntriesByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.TimeEntriesByProject" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
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

                html += "<b style='font-size:18px;'>" + lblProjectName.innerHTML + "</b>" + "<br /><br />" + divPersonListSummary[i].innerHTML;
            }
        }

        hdnSaveReportExcelText.value = html + "<b  style='text-align: right;font-size:25px;'> " + lblGrandTotal.innerHTML + "</b>";
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

        hdnSaveReportPDFText.value = html + "<b  style='text-align: right;font-size:25px;'> " + lblGrandTotal.innerHTML + "</b>";
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
<style type="text/css">
    .NotVisible
    {
        display: none;
        visibility: hidden;
    }
    
    .CheckBoxFloatLeft INPUT
    {
        float: left;
        width: 20px;
        margin-right: 6px;
    }
    .CheckBoxFloatLeft LABEL
    {
        line-height: 20px;
        vertical-align: middle;
    }
</style>
<uc3:LoadingProgress ID="LoadingProgress1" runat="server" />
<asp:UpdatePanel ID="updReport" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hdnResetFilter" Value="false" runat="server" />
        <asp:Panel ID="pnlFilters" runat="server" CssClass="buttons-block">
            <table class="opportunity-description">
                <tr>
                    <td valign="top">
                        <table class="opportunity-description">
                            <tr>
                                <td>
                                    Client
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlClients" runat="server" Width="460" OnSelectedIndexChanged="ddlClients_OnSelectedIndexChanged"
                                        AutoPostBack="true">
                                    </asp:DropDownList>
                                </td>
                                <tr>
                                    <td>
                                        Project
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlProjects" runat="server" Enabled="false" AutoPostBack="true"
                                            Width="460" OnSelectedIndexChanged="ddlProjects_OnSelectedIndexChanged">
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
                                            Width="460px" OnSelectedIndexChanged="ddlMilestones_OnSelectedIndexChanged">
                                            <asp:ListItem Text="-- Select a Milestone --" Value="-1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table>
                                            <tr>
                                                <td style="padding-top: 30px;">
                                                    Show only persons with time entered
                                                </td>
                                                <td style="text-align: right; padding-top: 30px;" id="report-date-interval">
                                                    <uc:DateInterval ID="diRange" runat="server" FromToDateFieldWidth="70" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                        </table>
                    </td>
                    <td valign="top">
                        <table class="opportunity-description">
                            <tr>
                                <td valign="top" style="padding-left: 5px;">
                                    <asp:Label ID="lblPersons" runat="server" Text="Persons"></asp:Label>
                                </td>
                                <td style="padding-left: 10px;">
                                    <cc2:ScrollingDropDown ID="cblPersons" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                        BackColor="White" CellPadding="3" NoItemsType="All" SetDirty="False" Width="315"
                                        Height="99" BorderWidth="0">
                                    </cc2:ScrollingDropDown>
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
                    <h3 style="font-weight: bold;">
                        <asp:Label ID="lblPersonName" runat="server" Text='<%# Eval("Key.PersonLastFirstName") %>' /></h3>
                    <br class="NotVisible" />
                    <asp:GridView ID="gvPersonTimeEntries" runat="server" DataSource='<%# Eval("Value") %>'
                        AutoGenerateColumns="false" CssClass="CompPerfTable WholeWidth" GridLines="Both"
                        OnRowDataBound="gvPersonTimeEntries_OnRowDataBound" BackColor="White" ShowFooter="true"
                        OnDataBound="gvPersonTimeEntries_OnDataBound" EmptyDataText='<%# GetEmptyDataText() %>'>
                        <AlternatingRowStyle BackColor="#F9FAFF" />
                        <FooterStyle Font-Bold="true" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <div style="text-align: center; font-weight: bold; vertical-align: middle;" class="ie-bg">
                                        Date</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# ((TimeEntryRecord)Container.DataItem).MilestoneDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                                </ItemTemplate>
                                <HeaderStyle VerticalAlign="Middle" HorizontalAlign="Center" />
                                <ItemStyle Wrap="False" HorizontalAlign="Left" VerticalAlign="Middle" Width="100" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <div style="text-align: center; font-weight: bold; vertical-align: middle;" class="ie-bg">
                                        Note</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("Note")%>
                                </ItemTemplate>
                                <ItemStyle VerticalAlign="Middle" />
                                <HeaderStyle VerticalAlign="Middle" HorizontalAlign="Center" />
                                <FooterStyle BorderStyle="None" VerticalAlign="Middle" HorizontalAlign="Right" />
                                <FooterTemplate>
                                    Total:</FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <div style="text-align: center; vertical-align: middle; font-weight: bold;" class="ie-bg">
                                        Hours</div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div style="text-align: center;">
                                        <%#((TimeEntryRecord)Container.DataItem).ActualHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%></div>
                                </ItemTemplate>
                                <HeaderStyle VerticalAlign="Middle" HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center" BorderStyle="None" VerticalAlign="Middle" />
                                <ItemStyle Wrap="False" Width="50" HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </ItemTemplate>
        </asp:DataList>
        <h3 style="text-align: right">
            <asp:Label ID="lblGrandTotal" runat="server" /></h3>
        <asp:HiddenField ID="hdnGuid" runat="server" />
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="btnExportToExcel" />
        <asp:PostBackTrigger ControlID="btnExportToPDF" />
    </Triggers>
</asp:UpdatePanel>

