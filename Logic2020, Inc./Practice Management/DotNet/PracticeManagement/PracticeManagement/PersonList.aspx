<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="PersonList.aspx.cs" Inherits="PraticeManagement.PersonList" Title="Practice Management - Person List"
    EnableEventValidation="false" %>

<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/PracticeFilter.ascx" TagName="PracticeFilter" TagPrefix="uc1" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Person List</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Person List
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <div class="buttons-block">
                <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                    ImageControlID="btnExpandCollapseFilter" CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg"
                    CollapseControlID="btnExpandCollapseFilter" ExpandControlID="btnExpandCollapseFilter"
                    Collapsed="True" TextLabelID="lblFilter" />
                <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                    ToolTip="Expand Filters" />
                <asp:ShadowedHyperlink runat="server" Text="Add Person" ID="lnkAddPerson" CssClass="add-btn"
                    NavigateUrl="~/PersonDetail.aspx?returnTo=PersonList.aspx" />
            </div>
            <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
                <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                    CssClass="CustomTabStyle">
                    <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>Filters</span></a> </span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table>
                                <uc1:PracticeFilter ID="practiceFilter" runat="server" OnFilterChanged="practiceFilter_FilterChanged" />
                                <tr>
                                    <td style="white-space: nowrap">
                                        Recruited by
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlRecruiter" runat="server" AutoPostBack="true" CssClass="WholeWidth"
                                            OnSelectedIndexChanged="ddlRecruiter_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" ID="tpSearch" >
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>Search</span></a> </span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch" class="WholeWidth">
                                <table class="WholeWidth">
                                    <tr>
                                        <td style="width:93%">
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="WholeWidth" MaxLength="40"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ErrorMessage="Please type text to be searched."
                                                ControlToValidate="txtSearch" EnableClientScript="true" ValidationGroup="ValSearch" />
                                        </td>
                                        <td style="width:1%">
                                            &nbsp;
                                        </td>                                        
                                        <td valign="top">
                                            <asp:Button runat="server" ID="btnSearch" ValidationGroup="ValSearch" OnClick="btnSearch_OnClick" Text="Search"
                                                Width="75px" CssClass="pm-button" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>                    
                </AjaxControlToolkit:TabContainer>
            </asp:Panel>
            <br />
            <asp:GridView ID="gvPersons" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here"
                DataKeyNames="Id" OnRowCommand="gvPersons_RowCommand" OnDataBinding="gvPersons_DataBound"
                AllowPaging="True" PageSize="25" DataSourceID="odsPersons" AllowSorting="true"
                CssClass="CompPerfTable WholeWidth" OnPageIndexChanged="gvPersons_PageIndexChanged"
                GridLines="None" OnSorting="gvPersons_Sort">
                <AlternatingRowStyle BackColor="#F9FAFF" />
                <PagerSettings Mode="NumericFirstLast" />
                <Columns>
                    <asp:TemplateField HeaderText="Person Name" SortExpression="LastName">
                        <ItemTemplate>
                            <asp:HyperLink ID="btnPersonName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("LastName") + ", " + Eval("FirstName")) %>'
                                NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Id")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Start Date" SortExpression="HireDate">
                        <ItemTemplate>
                            <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("HireDate")).ToString("d") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="End Date" SortExpression="TerminationDate">
                        <ItemTemplate>
                            <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("TerminationDate") != null ? ((DateTime)Eval("TerminationDate")).ToString("d") : string.Empty %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Practice Area" SortExpression="PracticeName">
                        <ItemTemplate>
                            <asp:Label ID="lblPracticeName" runat="server" Text='<%# Eval("DefaultPractice") != null ? Eval("DefaultPractice.Name") : string.Empty %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Pay Type" SortExpression="TimescaleName">
                        <ItemTemplate>
                            <asp:Label ID="lblTimascaleName" runat="server" Text='<%# Eval("CurrentPay.TimescaleName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status" SortExpression="PersonStatusName">
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>                  
                     <asp:TemplateField HeaderText="Last Login" SortExpression="LastLoginDate">
                        <ItemTemplate>
                            <asp:Label ID="lblLastLogin" runat="server" Text='<%# FormatDate((DateTime?) Eval("LastLogin")) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Seniority" SortExpression="SeniorityName">
                        <ItemTemplate>
                            <asp:Label ID="lblSeniority" runat="server" Text='<%# Eval("Seniority.Name") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Manager" SortExpression="ManagerLastName" HeaderStyle-CssClass="ie-bg">
                        <ItemTemplate>
                            <asp:HyperLink ID="btnManagerName" runat="server" Text='<%# Eval("Manager.PersonLastFirstName") %>'
                                NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Manager.Id")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:GridView ID="excelGrid" runat="server" Visible="false">
            </asp:GridView>
            <asp:ObjectDataSource ID="odsPersons" runat="server" SelectCountMethod="GetPersonCount"
                SelectMethod="GetPersons" StartRowIndexParameterName="startRow" MaximumRowsParameterName="maxRows"
                EnablePaging="true" SortParameterName="sortBy" CacheDuration="5">
                <SelectParameters>
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$practiceFilter" Name="practiceId"
                        PropertyName="PracticeId" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$practiceFilter" Name="active"
                        PropertyName="ActiveOnly" Type="Boolean" />
                    <asp:ControlParameter ControlID="gvPersons" Name="pageSize" PropertyName="PageSize"
                        Type="Int32" />
                    <asp:SessionParameter SessionField="CurrentPageIndex" Name="pageNo"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="tcFilters$tpSearch$txtSearch" Name="looked" PropertyName="Text"
                        Type="String" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$ddlRecruiter" Name="recruiterId"
                        PropertyName="SelectedValue" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="buttons-block" style="margin-top: 10px;">
        <asp:Button ID="btnExportToExcel" runat="server" OnClick="btnExportToExcel_Click"
            Text="Export" />
        <div class="clear0">
        </div>
    </div>
</asp:Content>

