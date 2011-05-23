<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Persons.aspx.cs" Inherits="PraticeManagement.Config.Persons" %>

<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/PersonsFilter.ascx" TagName="PersonsFilter" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Persons</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Person List
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <div class="buttons-block">
                <table class="WholeWidth">
                    <tr valign="middle">
                        <td style="width: 40px;">
                            <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                ImageControlID="btnExpandCollapseFilter" CollapsedImage="../Images/expand.jpg"
                                ExpandedImage="../Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                            <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                ToolTip="Expand Filters" />
                        </td>
                        <td align="left" valign="middle" style="float: left;">
                            <asp:TextBox runat="server" ID="txtSearch" Style="width: 400px; text-align: left;"
                                OnTextChanged="txtSearch_TextChanged" MaxLength="40"></asp:TextBox>
                            <ajaxToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server" TargetControlID="txtSearch"
                                WatermarkCssClass="watermarkedtext" WatermarkText="To search for a person, click here to begin typing and hit enter...">
                            </ajaxToolkit:TextBoxWatermarkExtender>
                        </td>
                        <td style="width: 115px;">
                            <asp:Button ID="btnUpdateView" runat="server" Text="Update View" OnClick="UpdateView_Clicked" />
                        </td>
                        <td style="width: 115px;">
                            <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" OnClick="ResetFilter_Clicked" />
                        </td>
                        <td style="width: 115px; text-align:right">
                            <asp:DropDownList ID="ddlView" runat="server" OnSelectedIndexChanged="DdlView_SelectedIndexChanged"
                                AutoPostBack="true">
                                <asp:ListItem Text="View 25" Value="25"></asp:ListItem>
                                <asp:ListItem Text="View 50" Value="50"></asp:ListItem>
                                <asp:ListItem Text="View 100" Value="100"></asp:ListItem>
                                <asp:ListItem Text="View All" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" align="right" style="padding-top:10px;">
                            <asp:ShadowedHyperlink runat="server" Text="Add Person" ID="lnkAddPerson" CssClass="add-btn"
                                NavigateUrl="~/PersonDetail.aspx?returnTo=Config/Persons.aspx" />
                        </td>
                    </tr>
                </table>
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
                                <tr>
                                    <td>
                                        <uc1:PersonsFilter ID="personsFilter" runat="server" OnFilterChanged="personsFilter_FilterChanged" />
                                    </td>
                                    <td rowspan="3" valign="top" align="center" style="padding-left: 5px;">
                                        <table>
                                            <tr>
                                                <td style="border-bottom: 1px solid black;">
                                                    <span>Recruiter</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td rowspan="2" style="padding-top: 9px;">
                                                    <asp:DropDownList ID="ddlRecruiter" Width="200px" runat="server" AutoPostBack="false"
                                                        CssClass="WholeWidth">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hdnActive" runat="server" Value="true" />
                            <asp:HiddenField ID="hdnProjected" runat="server" Value="false" />
                            <asp:HiddenField ID="hdnInactive" runat="server" Value="false" />
                            <asp:HiddenField ID="hdnTerminated" runat="server" Value="false" />
                            <asp:HiddenField ID="hdnPracticeId" runat="server" />
                            <asp:HiddenField ID="hdnPayTypeId" runat="server" />
                            <asp:HiddenField ID="hdnRecruiterId" runat="server" />
                            <asp:HiddenField ID="hdnLooked" runat="server" />
                            <asp:HiddenField ID="hdnAlphabet" runat="server" />
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>
            </asp:Panel>
            <br />
            <div style="padding-bottom: 20px;">
                <asp:GridView ID="gvPersons" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here"
                    DataKeyNames="Id" OnRowCommand="gvPersons_RowCommand" OnDataBinding="gvPersons_DataBound"
                    OnRowDataBound="gvPersons_RowDataBound" AllowPaging="False" PageSize="25" DataSourceID="odsPersons"
                    AllowSorting="true" CssClass="CompPerfTable WholeWidth" OnPageIndexChanged="gvPersons_PageIndexChanged"
                    GridLines="None" OnSorting="gvPersons_Sorting" OnPreRender="gvPersons_PreRender">
                    <AlternatingRowStyle BackColor="#F9FAFF" />
                    <Columns>
                        <asp:TemplateField HeaderText="Person Name" SortExpression="LastName" HeaderStyle-CssClass="ie-bg">
                            <ItemStyle Width="20%" />
                            <ItemTemplate>
                                <asp:HyperLink ID="btnPersonName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("LastName") + ", " + Eval("FirstName")) %>'
                                    NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Id")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Start Date" SortExpression="HireDate">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                            <ItemStyle Width="70px" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblStartDate" runat="server" Text='<%# FormatDate((DateTime?)Eval("HireDate")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="End Date" SortExpression="TerminationDate">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                            <ItemStyle Width="70px" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblEndDate" runat="server" Text='<%# FormatDate((DateTime?)Eval("TerminationDate")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Practice Area" SortExpression="PracticeName">
                            <ItemStyle Wrap="true" Width="13%" />
                            <ItemTemplate>
                                <asp:Label ID="lblPracticeName" runat="server" Text='<%# Eval("DefaultPractice") != null ? Eval("DefaultPractice.Name") : string.Empty %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pay Type" SortExpression="TimescaleName" HeaderStyle-Wrap="false">
                            <ItemStyle Width="70px" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblTimascaleName" runat="server" Text='<%# Eval("CurrentPay.TimescaleName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" SortExpression="PersonStatusName">
                            <ItemStyle Width="72px" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Last Login" SortExpression="LastLoginDate">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblLastLogin" runat="server" Text='<%# FormatDate((DateTime?) Eval("LastLogin")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Seniority" SortExpression="SeniorityName">
                            <ItemStyle Width="130px" />
                            <ItemTemplate>
                                <asp:Label ID="lblSeniority" runat="server" Text='<%# Eval("Seniority.Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Line Manager" SortExpression="ManagerLastName" HeaderStyle-CssClass="ie-bg">
                            <ItemStyle Width="130px" />
                            <ItemTemplate>
                                <asp:HyperLink ID="btnManagerName" runat="server" Text='<%# Eval("Manager.PersonLastFirstName") %>'
                                    NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Manager.Id")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <asp:GridView ID="excelGrid" runat="server" Visible="false">
            </asp:GridView>
            <asp:ObjectDataSource ID="odsPersons" runat="server" SelectCountMethod="GetPersonCount"
                SelectMethod="GetPersons" StartRowIndexParameterName="startRow" MaximumRowsParameterName="maxRows"
                EnablePaging="true" SortParameterName="sortBy" CacheDuration="5" TypeName="PraticeManagement.Config.Persons">
                <SelectParameters>
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$hdnPracticeId" Name="practiceId"
                        PropertyName="Value" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$hdnActive" Name="active"
                        PropertyName="Value" Type="Boolean" />
                    <asp:ControlParameter ControlID="gvPersons" Name="pageSize" PropertyName="PageSize"
                        Type="Int32" />
                    <asp:SessionParameter SessionField="CurrentPageIndex" Name="pageNo" Type="Int32" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$hdnLooked" Name="looked" PropertyName="Value" Type="String" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$hdnRecruiterId" Name="recruiterId"
                        PropertyName="Value" Type="String" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$hdnPayTypeId" Name="payTypeId"
                        PropertyName="Value" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$hdnProjected" Name="projected"
                        PropertyName="Value" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$hdnTerminated" Name="terminated"
                        PropertyName="Value" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$hdnInactive" Name="inactive"
                        PropertyName="Value" />
                    <asp:ControlParameter ControlID="tcFilters$tpMainFilters$hdnAlphabet" Name="alphabet"
                        PropertyName="Value" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <table class="buttons-block WholeWidth" style="background-color: #d4dff8;">
                <tr style="height: 25px;">
                    <td align="left" style="width: 100px; padding-left: 10px;">
                        <asp:LinkButton ID="lnkbtnPrevious" runat="server" Text="<- PREVIOUS"
                            Font-Underline="false" OnClick="Previous_Clicked"></asp:LinkButton>
                    </td>
                    <td align="center">
                        <div id="divAlphabeticalPaging" runat="server">
                            <asp:LinkButton ID="lnkbtnAll" runat="server" Text="All" Font-Underline="false"
                                Font-Bold="true" OnClick="Alphabet_Clicked"></asp:LinkButton>
                        </div>
                    </td>
                    <td align="right" style="width: 100px; padding-right: 10px;">
                        <asp:LinkButton ID="lnkbtnNext" runat="server" Text="NEXT ->" Font-Underline="false"
                            OnClick="Next_Clicked"></asp:LinkButton>
                    </td>
                </tr>
                <tr style="height: 40px;">
                    <td style="width: 150px;">
                    </td>
                    <td align="center">
                        <asp:Label ID="lblRecords" runat="server" ForeColor="Black" Font-Bold="true"></asp:Label>
                    </td>
                    <td align="right" style="width: 150px;">
                        <asp:Button ID="btnExportToExcel" runat="server" OnClick="btnExportToExcel_Click"
                            Text="Export" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

