<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Vendors.aspx.cs" Inherits="PraticeManagement.Config.Vendors" %>

<%@ PreviousPageType VirtualPath="~/DashBoard.aspx" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Vendors | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Vendors List
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            $("#tblVendors").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        });
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {
            $("#tblVendors").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
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
                                                    <asp:TextBox runat="server" ID="txtSearch" CssClass="WholeWidthImp" OnTextChanged="txtSearch_TextChanged"
                                                        MaxLength="40"></asp:TextBox>
                                                    <ajaxToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server" TargetControlID="txtSearch"
                                                        WatermarkCssClass="watermarkedtext WholeWidthImp" WatermarkText="To search for a vendor, click here to begin typing and hit enter...">
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
                                    <td class="Width26Per PaddingRight10Px" align="right">
                                        <asp:ShadowedHyperlink runat="server" Text="Add Vendor" ID="lnkAddVendor" CssClass="add-btn"
                                            NavigateUrl="~/Config/VendorDetail.aspx?returnTo=~/Config/Vendors.aspx" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr class="Height35Px vBottom">
                        <td colspan="2" class="ValSumSearch">
                            <asp:ValidationSummary ID="valsumSearch" runat="server" ValidationGroup="ValSearch" />
                        </td>
                        <td align="right">
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Export" OnClick="btnExportToExcel_Click"
                                Enabled="true" UseSubmitBehavior="false" ToolTip="Export To Excel" Width="100px" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdnCleartoDefaultView" runat="server" Value="false" />
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
                                    <td>
                                        <table class="width30P">
                                            <tr class="TextAlignCenterImp">
                                                <td class="FltrHeader Width40P">
                                                    <span>Vendor Status</span>
                                                </td>
                                                <td class="Width10PerImp">
                                                </td>
                                                <td class="FltrHeader width50Px">
                                                    <span>Vendor Type</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="PaddingTop5 PaddingLeft3PxImp">
                                                    <table>
                                                        <tr>
                                                            <td class="no-wrap">
                                                                <asp:CheckBox ID="chbShowActive" runat="server" Checked="True" CssClass="textLeft" />
                                                                <span class="padRight5Imp">Active</span>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="no-wrap">
                                                                <asp:CheckBox ID="chbShowInActive" runat="server" Checked="false" CssClass="textLeft" />
                                                                <span class="padRight5Imp">Inactive</span>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>
                                                </td>
                                                <td class="floatRight PaddingTop5 PaddingLeft3PxImp">
                                                    <cc2:ScrollingDropDown ID="cblVendorTypes" runat="server" AllSelectedReturnType="AllItems"
                                                        CellPadding="3" CssClass="PersonFilterSddAllTypes Width220PxImp" DropDownListType="Vendor type"
                                                        NoItemsType="All" onclick="scrollingDropdown_onclick('cblVendorTypes','Vendor type')"
                                                        SetDirty="False">
                                                    </cc2:ScrollingDropDown>
                                                    <ext:ScrollableDropdownExtender ID="sdeVendorTypes" runat="server" Enabled="True"
                                                        EditImageUrl="~/Images/Dropdown_Arrow.png" TargetControlID="cblVendorTypes" UseAdvanceFeature="True"
                                                        Width="93%">
                                                    </ext:ScrollableDropdownExtender>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="FilterBtnsTd Width9Per">
                                        <table class="WholeWidth">
                                            <tr>
                                                <td class="FilterBtnsTd">
                                                    <asp:Button ID="btnUpdateView" CssClass="Width100Per" runat="server" Text="Update"
                                                        OnClick="UpdateView_Clicked" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FilterBtnsTd">
                                                    <asp:Button ID="btnResetFilter" CssClass="Width100Per" runat="server" Text="Reset"
                                                        OnClick="ResetFilter_Clicked" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>
            </asp:Panel>
            <br />
            <div class="Width50Percent">
                <div id="vendorGrid" runat="server">
                    <asp:Repeater ID="repVendors" runat="server">
                        <HeaderTemplate>
                            <div>
                                <table id="tblVendors" class="CompPerfTable WholeWidth BackGroundColorWhite">
                                    <thead>
                                        <tr>
                                            <th class="CursorPointer color0898E6 fontUnderline Width250Px">
                                                <div class='ie-bg'>
                                                    Vendor Name
                                                </div>
                                            </th>
                                            <th class="CursorPointer color0898E6 fontUnderline Width250Px">
                                                <div class='ie-bg'>
                                                    Vendor Type</div>
                                            </th>
                                            <th class="CursorPointer color0898E6 fontUnderline Width100Px">
                                                <div class='ie-bg'>
                                                    Status</div>
                                            </th>
                                            <th class="CursorPointer color0898E6 fontUnderline Width250Px">
                                                <div class='ie-bg'>
                                                    Contact Name</div>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="Left">
                                    <asp:HyperLink ID="btnVendorName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
                                        NavigateUrl='<%# GetVendorDetailsUrlWithReturn(Eval("Id")) %>' />
                                </td>
                                <td class="TextAlignCenterImp no-wrap">
                                    <asp:Label ID="lblVendorType" runat="server" Text='<%#Eval("VendorType.Name") %>'></asp:Label>
                                </td>
                                <td class="TextAlignCenterImp no-wrap">
                                    <asp:Label ID="lblVendorStatus" runat="server" Text='<%#(Boolean.Parse(Eval("Status").ToString())) ? "Active" : "Inactive" %>'></asp:Label>
                                </td>
                                <td class="TextAlignCenterImp no-wrap">
                                    <asp:HyperLink ID="btnContactName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("ContactName")) %>'
                                        NavigateUrl='<%# GetVendorDetailsUrlWithReturn(Eval("Id")) %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr class="alterrow">
                                <td class="Left">
                                    <asp:HyperLink ID="btnVendorName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
                                        NavigateUrl='<%# GetVendorDetailsUrlWithReturn(Eval("Id")) %>' />
                                </td>
                                <td class="TextAlignCenterImp no-wrap">
                                    <asp:Label ID="lblVendorType" runat="server" Text='<%#Eval("VendorType.Name") %>'></asp:Label>
                                </td>
                                <td class="TextAlignCenterImp no-wrap">
                                    <asp:Label ID="lblVendorStatus" runat="server" Text='<%#(Boolean.Parse(Eval("Status").ToString())) ? "Active" : "Inactive" %>'></asp:Label>
                                </td>
                                <td class="TextAlignCenterImp no-wrap">
                                    <asp:HyperLink ID="btnContactName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("ContactName")) %>'
                                        NavigateUrl='<%# GetVendorDetailsUrlWithReturn(Eval("Id")) %>' />
                                </td>
                            </tr>
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                            </tbody></table></div>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div id="divVendorEmptyMessage" style="display: none;" runat="server">
                    No Vendors found for the selected filters.
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

