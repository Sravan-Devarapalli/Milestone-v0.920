<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Persons.aspx.cs" Inherits="PraticeManagement.Config.Persons" %>

<%@ PreviousPageType VirtualPath="~/DashBoard.aspx" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/PersonsFilter.ascx" TagName="PersonsFilter" TagPrefix="uc1" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Persons | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Person List
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
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
    <uc:LoadingProgress ID="lpPersons" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <div class="buttons-block upnlBodyPadding">
                <table class="WholeWidth">
                    <tr class="vMiddle">
                        <td colspan="3">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="Width3Per">
                                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="../Images/expand.jpg"
                                            ExpandedImage="../Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                            ToolTip="Expand Filters" />
                                    </td>
                                    <td class="PersonsSearch WhiteSpaceNoWrap vMiddle">
                                        <table class="WholeWidth">
                                            <tr>
                                                <td class="Width97Per SearchTextBox">
                                                    <asp:TextBox runat="server" ID="txtSearch" CssClass="WholeWidthImp" OnTextChanged="txtSearch_TextChanged" MaxLength="40"></asp:TextBox>
                                                    <ajaxToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server" TargetControlID="txtSearch"
                                                        WatermarkCssClass="watermarkedtext WholeWidthImp" WatermarkText="To search for a person, click here to begin typing and hit enter...">
                                                    </ajaxToolkit:TextBoxWatermarkExtender>
                                                </td>
                                                <td class="Width3Per">
                                                    <asp:RequiredFieldValidator ID="reqSearchText" runat="server" Text="*" ErrorMessage="Please type text to be searched."
                                                        ToolTip="Please type text to be searched." ControlToValidate="txtSearch" EnableClientScript="true"
                                                        ValidationGroup="ValSearch" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width8Per">
                                        <asp:Button ID="btnSearchAll" ValidationGroup="ValSearch" runat="server" Text="Search All"
                                            OnClick="btnSearchAll_OnClick" />
                                    </td>
                                    <td class="Width8Per">
                                        <asp:Button ID="btnClearResults" Enabled="false" runat="server" Text="Clear Results"
                                            OnClick="ResetFilter_Clicked" />
                                    </td>
                                    <td class="TdSpace">
                                    </td>
                                    <td class="Width26Per" align="right">
                                        <table >
                                            <tr>
                                                <td>
                                                    <asp:ShadowedHyperlink runat="server" Text="Add Person" ID="lnkAddPerson" CssClass="add-btn"
                                                        NavigateUrl="~/PersonDetail.aspx?returnTo=Config/Persons.aspx?ApplyFilterFromCookie=true" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr class="Height35Px vBottom">
                        <td colspan="2" class="ValSumSearch">
                            <asp:ValidationSummary ID="valsumSearch" runat="server" ValidationGroup="ValSearch" />
                        </td>
                        <td class="Width10Per TextAlignRight">
                            <asp:DropDownList ID="ddlView" runat="server" OnSelectedIndexChanged="DdlView_SelectedIndexChanged"
                                AutoPostBack="true">
                                <asp:ListItem Text="View 25" Value="25"></asp:ListItem>
                                <asp:ListItem Text="View 50" Value="50"></asp:ListItem>
                                <asp:ListItem Text="View 100" Value="100"></asp:ListItem>
                                <asp:ListItem Text="View All" Value="-1" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
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
                                    <td class="Width70Per">
                                        <uc1:PersonsFilter ID="personsFilter" runat="server" OnFilterChanged="personsFilter_FilterChanged" />
                                    </td>
                                    <td class="Width22Per">
                                        <table class="WholeWidth">
                                            <tr class="TextAlignCenter">
                                                <td class="RecruiterTd">
                                                    <span>Recruiter</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="PaddingTop10Px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td rowspan="2" class="floatRight SddRecruiter">
                                                    <cc2:ScrollingDropDown ID="cblRecruiters" runat="server"  AllSelectedReturnType="AllItems"
                                                        onclick="scrollingDropdown_onclick('cblRecruiters','Recruiter')" 
                                                         NoItemsType="All" SetDirty="False" DropDownListType="Recruiter" CssClass="PersonPage_cblRecruiters"
                                                         />
                                                    <ext:ScrollableDropdownExtender ID="sdeRecruiters" runat="server" TargetControlID="cblRecruiters"
                                                        UseAdvanceFeature="True" Width="200px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                    </ext:ScrollableDropdownExtender>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="FilterBtnsTd Width9Per">
                                        <table class="WholeWidth">
                                            <tr>
                                                <td class="FilterBtnsTd">
                                                    <asp:Button ID="btnUpdateView" CssClass="Width100Per" runat="server" Text="Update" OnClick="UpdateView_Clicked" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FilterBtnsTd">
                                                    <asp:Button ID="btnResetFilter" CssClass="Width100Per" runat="server" Text="Reset" OnClick="ResetFilter_Clicked" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hdnActive" runat="server" Value="true" />
                            <asp:HiddenField ID="hdnProjected" runat="server" Value="false" />
                            <asp:HiddenField ID="hdnTerminatedPending" runat="server" Value="false" />
                            <asp:HiddenField ID="hdnTerminated" runat="server" Value="false" />
                            <asp:HiddenField ID="hdnLooked" runat="server" />
                            <asp:HiddenField ID="hdnAlphabet" runat="server" />
                            <asp:HiddenField ID="hdnCleartoDefaultView" runat="server" Value="false" />
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>
            </asp:Panel>
            <br />
            <div class="PaddingBottom20Px">
                <asp:GridView ID="gvPersons" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here"
                    DataKeyNames="Id" OnRowCommand="gvPersons_RowCommand" OnDataBinding="gvPersons_DataBound"
                    OnRowDataBound="gvPersons_RowDataBound" AllowPaging="False" DataSourceID="odsPersons"
                    AllowSorting="true" CssClass="CompPerfTable WholeWidth" OnPageIndexChanged="gvPersons_PageIndexChanged"
                    GridLines="None" OnSorting="gvPersons_Sorting" OnPreRender="gvPersons_PreRender">
                    <AlternatingRowStyle CssClass="alterrow"/>
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="lnkPersonName" CommandName="Sort" CommandArgument="LastName"
                                        runat="server">Person Name</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <ItemStyle CssClass="Width21Percent"/>
                            <ItemTemplate>
                                <asp:HyperLink ID="btnPersonName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("LastName") + ", " + Eval("FirstName")) %>'
                                    NavigateUrl='<%# GetPersonDetailsUrlWithReturn((DataTransferObjects.Person) Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="LinkButton1" CommandName="Sort" CommandArgument="HireDate" runat="server">Start Date</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <HeaderStyle CssClass="TextAlignCenterImp no-wrap"/>
                            <ItemStyle CssClass="Width7Percent TextAlignCenterImp" />
                            <ItemTemplate>
                                <asp:Label ID="lblStartDate" runat="server" Text='<%# GetHireDate((DataTransferObjects.Person) Container.DataItem) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="LinkEndDate" CommandName="Sort" CommandArgument="TerminationDate"
                                        runat="server">End Date</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <HeaderStyle CssClass="TextAlignCenterImp no-wrap" />
                            <ItemStyle CssClass="Width7Percent TextAlignCenterImp"/>
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
                            <ItemStyle CssClass="Width16Percent WhiteSpaceNormal" />
                            <ItemTemplate>
                                <asp:Label ID="lblPracticeName" runat="server" Text='<%# Eval("DefaultPractice") != null ? HttpUtility.HtmlEncode((string)Eval("DefaultPractice.Name")) : string.Empty %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="LinkButton4" CommandName="Sort" CommandArgument="TimescaleName"
                                        runat="server">Pay Type</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <ItemStyle CssClass="Width7Percent TextAlignCenterImp"  />
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
                            <ItemStyle CssClass="Width8Percent TextAlignCenterImp"  />
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
                            <HeaderStyle CssClass="TextAlignCenterImp no-wrap" />
                            <ItemStyle CssClass="Width8Percent TextAlignCenterImp" />
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
                            <ItemStyle CssClass="Width12Percent" />
                            <ItemTemplate>
                                <asp:Label ID="lblSeniority" runat="server" Text='<%# Eval("Seniority.Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    <asp:LinkButton ID="LinkButton8" CommandName="Sort" CommandArgument="ManagerLastName"
                                        runat="server">Career Counselor</asp:LinkButton>
                                </div>
                            </HeaderTemplate>
                            <ItemStyle CssClass="Width14Percent" />
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
                    <asp:Parameter Name="terminatedPending" />
                    <asp:Parameter Name="alphabet" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <div class="buttons-block upnlBodyPadding">
                <table class="WholeWidth">
                    <tr class="Height25Px">
                        <td class="PaddingLeft10Px Width17Per WhiteSpaceNoWrap textLeft">
                            <asp:LinkButton ID="lnkbtnPrevious" runat="server" Text="<- PREVIOUS"
                                OnClick="Previous_Clicked" CssClass="LnkBtnPrevious"></asp:LinkButton>
                        </td>
                        <td class="Width66Per TextAlignCenterImp vMiddle">
                            <table class="WholeWidth">
                                <tr id="trAlphabeticalPaging" runat="server">
                                    <td class="TextAlignCenter paddingBottom10px PaddingLeft10Px PaddingTop10Px">
                                        <asp:LinkButton ID="lnkbtnAll" Top="lnkbtnAll" Bottom="lnkbtnAll1" runat="server"
                                            Text="All" 
                                             OnClick="Alphabet_Clicked" CssClass="LnkBtnAll"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="Width17Per WhiteSpaceNoWrap PaddingRight10Px textRightImp">
                            <asp:LinkButton ID="lnkbtnNext" runat="server" Text="NEXT ->" CssClass="LnkBtnPrevious"
                                OnClick="Next_Clicked"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr class="Height40Px">
                        <td colspan="3">
                            <table class="WholeWidth">
                                <tr>
                                    <td class="Width15Per">
                                    </td>
                                    <td class="Width70Per TextAlignCenter">
                                        <asp:Label ID="lblRecords" runat="server" CssClass="fontBold ColorBlack"></asp:Label>
                                    </td>
                                    <td class="Width15Per PaddingRight10Px WhiteSpaceNoWrap TextAlignRight">
                                        <asp:Button ID="btnExportToExcel" CssClass="Width100Per" runat="server" OnClick="btnExportToExcel_Click"
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

