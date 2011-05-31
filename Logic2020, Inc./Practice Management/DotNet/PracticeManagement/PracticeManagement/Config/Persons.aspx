<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Persons.aspx.cs" Inherits="PraticeManagement.Config.Persons" %>

<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/PersonsFilter.ascx" TagName="PersonsFilter" TagPrefix="uc1" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Persons</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Person List
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script language="javascript" type="text/javascript" src="../Scripts/jquery-1.4.1.js"></script>
    <script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
    <script type="text/javascript">
        
        function changeAlternateitemscolrsForCBL() {
            var cbl = document.getElementById('<%=cblRecruiters.ClientID %>');
            SetAlternateColors(cbl);
        }

        function SetAlternateColors(chkboxList) {
            var chkboxes = chkboxList.getElementsByTagName('input');
            var index = 0;
            for (var i = 0; i < chkboxes.length; i++) {
                if (chkboxes[i].parentNode.style.display != "none") {
                    index++;
                    if ((index) % 2 == 0) {
                        chkboxes[i].parentNode.style.backgroundColor = "#f9faff";
                    }
                    else {
                        chkboxes[i].parentNode.style.backgroundColor = "";
                    }
                }
            }
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <div class="buttons-block" style="padding-left: 5px !important; padding-right: 5px !important;">
                <table class="WholeWidth">
                    <tr valign="middle">
                        <td colspan="3">
                            <table class="WholeWidth">
                                <tr>
                                    <td style="width: 4%;">
                                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="../Images/expand.jpg"
                                            ExpandedImage="../Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                            ToolTip="Expand Filters" />
                                    </td>
                                    <td valign="middle" style="width: 39%; white-space: nowrap; padding-left: 5px; padding-right: 0px;">
                                        <table class="WholeWidth">
                                            <tr>
                                                <td style="width: 97%;" >
                                                    <asp:TextBox runat="server" ID="txtSearch" Width="100%" Style="text-align: left;"
                                                        OnTextChanged="txtSearch_TextChanged" MaxLength="40"></asp:TextBox>
                                                    <ajaxToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server" TargetControlID="txtSearch"
                                                        WatermarkCssClass="watermarkedtext" WatermarkText="To search for a person, click here to begin typing and hit enter...">
                                                    </ajaxToolkit:TextBoxWatermarkExtender>
                                                </td>
                                                <td style="width: 3%;">
                                                    <asp:RequiredFieldValidator ID="reqSearchText"  runat="server"
                                                        Text="*" ErrorMessage="Please type text to be searched." ToolTip="Please type text to be searched."
                                                        ControlToValidate="txtSearch" EnableClientScript="true" ValidationGroup="ValSearch" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 1%; padding-left: 0px;">
                                    </td>
                                    <td style="width: 11%;">
                                        <asp:Button ID="btnSearchAll" ValidationGroup="ValSearch" Width="100%" runat="server"
                                            Text="Search All" OnClick="btnSearchAll_OnClick" />
                                    </td>
                                    <td style="width: 1%;">
                                    </td>
                                    <td style="width: 11%;">
                                        <asp:Button ID="btnClearResults" Width="100%" Enabled="false" runat="server" Text="Clear Results"
                                            OnClick="ResetFilter_Clicked" />
                                    </td>
                                    <td style="width: 9%;">
                                    </td>
                                    <td style="width: 12%; text-align: right">
                                        <asp:DropDownList ID="ddlView" runat="server" OnSelectedIndexChanged="DdlView_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Text="View 25" Value="25"></asp:ListItem>
                                            <asp:ListItem Text="View 50" Value="50"></asp:ListItem>
                                            <asp:ListItem Text="View 100" Value="100"></asp:ListItem>
                                            <asp:ListItem Text="View All" Value="-1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td align="right" style="width: 12%; text-align: right">
                                        <asp:ShadowedHyperlink runat="server" Text="Add Person" ID="lnkAddPerson" CssClass="add-btn"
                                            NavigateUrl="~/PersonDetail.aspx?returnTo=Config/Persons.aspx" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="width: 100%; padding-left: 3%; white-space: nowrap;">
                            <asp:ValidationSummary ID="valsumSearch" runat="server" ValidationGroup="ValSearch" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left" style="width: 17%; white-space: nowrap; padding-left: 10px;">
                            <asp:LinkButton ID="lnkbtnPrevious1" runat="server" Text="<- PREVIOUS" Font-Underline="false"
                                OnClick="Previous_Clicked"></asp:LinkButton>
                        </td>
                        <td valign="middle" align="center" style="width: 66%; text-align: center;">
                            <table class="WholeWidth">
                                <tr id="trAlphabeticalPaging1" runat="server">
                                    <td style="padding-left: 10px; padding-top: 10px; padding-bottom: 10px; text-align: center;">
                                        <asp:LinkButton ID="lnkbtnAll1" Top="lnkbtnAll" Bottom="lnkbtnAll1" runat="server"
                                            Text="All" Font-Underline="false" Font-Bold="true" OnClick="Alphabet_Clicked"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="right" style="width: 17%; white-space: nowrap; padding-right: 10px;">
                            <asp:LinkButton ID="lnkbtnNext1" runat="server" Text="NEXT ->" Font-Underline="false"
                                OnClick="Next_Clicked"></asp:LinkButton>
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
                            <table class="WholeWidth">
                                <tr>
                                    <td style="width: 70%;">
                                        <uc1:PersonsFilter ID="personsFilter" runat="server" OnFilterChanged="personsFilter_FilterChanged" />
                                    </td>
                                    <td style="width: 22%;">
                                        <table class="WholeWidth">
                                            <tr style="text-align: center;">
                                                <td style="padding-right: 5px; border-bottom: 1px solid black;">
                                                    <span>Recruiter</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 10px;">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td rowspan="2" class="floatRight" style="padding-bottom: 8px; padding-left: 3px;">
                                                    <cc2:ScrollingDropDown ID="cblRecruiters" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                                        onclick="scrollingDropdown_onclick('cblRecruiters','Recruiter')" BackColor="White"
                                                        CellPadding="3" Height="200px" NoItemsType="All" SetDirty="False" DropDownListType="Recruiter"
                                                        Width="200px" BorderWidth="0" />
                                                    <ext:ScrollableDropdownExtender ID="sdeRecruiters" runat="server" TargetControlID="cblRecruiters"
                                                        UseAdvanceFeature="true" Width="200px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                    </ext:ScrollableDropdownExtender>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td align="right" style="padding: 5px; width: 9%; padding-right: 0px; text-align: right;">
                                        <table class="WholeWidth">
                                            <tr>
                                                <td align="right" style="padding: 5px; padding-right: 0px; text-align: right;">
                                                    <asp:Button ID="btnUpdateView" Width="100%" runat="server" Text="Update" OnClick="UpdateView_Clicked" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="padding: 5px; padding-right: 0px; text-align: right;">
                                                    <asp:Button ID="btnResetFilter" Width="100%" runat="server" Text="Reset" OnClick="ResetFilter_Clicked" />
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
                            <asp:HiddenField ID="hdnLooked" runat="server" />
                            <asp:HiddenField ID="hdnAlphabet" runat="server" />
                            <asp:HiddenField ID="hdnCleartoDefaultView" runat="server" Value="false" />
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
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="lnkPersonName" CommandName="Sort" CommandArgument="LastName"
                                        runat="server">Person Name</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <ItemStyle Width="21%" />
                            <ItemTemplate>
                                <asp:HyperLink ID="btnPersonName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("LastName") + ", " + Eval("FirstName")) %>'
                                    NavigateUrl='<%# GetPersonDetailsUrlWithReturn(Eval("Id")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="LinkButton1" CommandName="Sort" CommandArgument="HireDate" runat="server">Start Date</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                            <ItemStyle Width="7%" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblStartDate" runat="server" Text='<%# FormatDate((DateTime?)Eval("HireDate")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="LinkEndDate" CommandName="Sort" CommandArgument="TerminationDate"
                                        runat="server">End Date</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                            <ItemStyle Width="7%" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblEndDate" runat="server" Text='<%# FormatDate((DateTime?)Eval("TerminationDate")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="lnkPracticeName" CommandName="Sort" CommandArgument="PracticeName"
                                        runat="server">Practice Area</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <ItemStyle Wrap="true" Width="16%" />
                            <ItemTemplate>
                                <asp:Label ID="lblPracticeName" runat="server" Text='<%# Eval("DefaultPractice") != null ? Eval("DefaultPractice.Name") : string.Empty %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="LinkButton4" CommandName="Sort" CommandArgument="TimescaleName"
                                        runat="server">Pay Type</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <ItemStyle Width="7%" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblTimascaleName" runat="server" Text='<%# Eval("CurrentPay.TimescaleName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="lnkStatus" CommandName="Sort" CommandArgument="PersonStatusName"
                                        runat="server">Status</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <ItemStyle Width="8%" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="lnkLastLogin" CommandName="Sort" CommandArgument="LastLoginDate"
                                        runat="server">Last Login</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                            <ItemStyle Width="8%" HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="lblLastLogin" runat="server" Text='<%# FormatDate((DateTime?) Eval("LastLogin")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="lnkSeniority" CommandName="Sort" CommandArgument="SeniorityName"
                                        runat="server">Seniority</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <ItemStyle Width="12%" />
                            <ItemTemplate>
                                <asp:Label ID="lblSeniority" runat="server" Text='<%# Eval("Seniority.Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="LinkButton8" CommandName="Sort" CommandArgument="ManagerLastName"
                                        runat="server">Line Manager</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <ItemStyle Width="14%" />
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
                EnablePaging="true" SortParameterName="sortBy" CacheDuration="5" TypeName="PraticeManagement.Config.Persons"
                OnSelecting="odsPersons_OnSelecting">
                <SelectParameters>
                    <asp:Parameter Name="practiceIdsSelected" Type="String" />
                    <asp:Parameter Name="active" Type="Boolean" />
                    <asp:Parameter Name="pageSize" Type="Int32" />
                    <asp:Parameter Name="pageNo" Type="Int32" />
                    <asp:Parameter Name="looked" Type="String" />
                    <asp:Parameter Name="recruitersSelected" Type="String" />
                    <asp:Parameter Name="payTypeIdsSelected" Type="String" />
                    <asp:Parameter Name="projected" />
                    <asp:Parameter Name="terminated" />
                    <asp:Parameter Name="inactive" />
                    <asp:Parameter Name="alphabet" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <div class="buttons-block" style="padding-left: 5px !important; padding-right: 5px !important;">
                <table class="WholeWidth">
                    <tr style="height: 25px;">
                        <td align="left" style="width: 17%; white-space: nowrap; padding-left: 10px;">
                            <asp:LinkButton ID="lnkbtnPrevious" runat="server" Text="<- PREVIOUS" Font-Underline="false"
                                OnClick="Previous_Clicked"></asp:LinkButton>
                        </td>
                        <td valign="middle" align="center" style="width: 66%; text-align: center;">
                            <table class="WholeWidth">
                                <tr id="trAlphabeticalPaging" runat="server">
                                    <td style="padding-left: 10px; padding-top: 10px; padding-bottom: 10px; text-align: center;">
                                        <asp:LinkButton ID="lnkbtnAll" Top="lnkbtnAll" Bottom="lnkbtnAll1" runat="server"
                                            Text="All" Font-Underline="false" Font-Bold="true" OnClick="Alphabet_Clicked"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="right" style="width: 17%; white-space: nowrap; padding-right: 10px;">
                            <asp:LinkButton ID="lnkbtnNext" runat="server" Text="NEXT ->" Font-Underline="false"
                                OnClick="Next_Clicked"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr style="height: 40px;">
                        <td colspan="3">
                            <table class="WholeWidth">
                                <tr>
                                    <td style="width: 15%;">
                                    </td>
                                    <td align="center" style="width: 70%; text-align: center;">
                                        <asp:Label ID="lblRecords" runat="server" ForeColor="Black" Font-Bold="true"></asp:Label>
                                    </td>
                                    <td align="right" style="width: 15%; white-space: nowrap; padding-right: 10px;">
                                        <asp:Button ID="btnExportToExcel" Width="100px" runat="server" OnClick="btnExportToExcel_Click"
                                            Text="Export" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

