<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectTimeTypes.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectTimeTypes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register TagPrefix="uc" Namespace="PraticeManagement.Controls.Generic" Assembly="PraticeManagement" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<table class="WholeWidth">
    <tr>
        <td class="BtnAddTimeType">
            <asp:ShadowedTextButton ID="btnAddNewTimeType" ToolTip="Add Work Type" runat="server"
                CausesValidation="false" CssClass="add-btn" Text="Add Work Type" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeAddTimeType" runat="server" TargetControlID="btnAddNewTimeType"
                BackgroundCssClass="modalBackground" PopupControlID="pnlAddNewTimeType" DropShadow="false"
                BehaviorID="mpeAddTimeType" />
            <br />
        </td>
    </tr>
</table>
<br />
<table class="WholeWidth">
    <tr>
        <td class="textLeft ValignMiddleImp Width10Percent">
        </td>
        <td class="textLeft ValignMiddleImp Width75Percent">
            <table class="WholeWidth">
                <tr class="PaddingTop2Px">
                    <td class="TdAssignedWorkTypesText vTop paddingBottom5px">
                        <b>Work Types Not Assigned to Project</b>
                    </td>
                    <td class="TextAlignCenterImp vMiddle Width8Percent">
                    </td>
                    <td class="TdAssignedWorkTypesText paddingBottom5px">
                        <b>Work Types Assigned to Project</b>
                        <asp:CustomValidator ID="CustomValidator1" runat="server" ValidationGroup="Project"
                            OnServerValidate="cvTimetype_OnServerValidate" Display="Dynamic" ErrorMessage="Atleast one WorkType should be assigned to the project."
                            ToolTip="Atleast one WorkType should be assigned to the project." Text="*"></asp:CustomValidator>
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr class="HeightAuto">
                    <td class="TdTimeTypesNotAssignedToProject">
                        <asp:TextBox ID="txtTimeTypesNotAssignedToProject" runat="server" CssClass="TbTimeTypesNotAssignedToProject"></asp:TextBox>
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmTimeTypesNotAssignedToProject"
                            runat="server" TargetControlID="txtTimeTypesNotAssignedToProject" WatermarkText="Begin typing to sort list below..."
                            WatermarkCssClass="watermarkedtext TbTimeTypesNotAssignedToProject" />
                    </td>
                    <td class="TextAlignCenterImp vMiddle Width8Percent">
                    </td>
                    <td class="TdTimeTypesNotAssignedToProject">
                        <asp:TextBox ID="txtTimeTypesAssignedToProject" runat="server" MaxLength="50" CssClass="TbTimeTypesNotAssignedToProject"></asp:TextBox>
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server"
                            TargetControlID="txtTimeTypesAssignedToProject" WatermarkText="Begin typing to sort list below..."
                            WatermarkCssClass="watermarkedtext TbTimeTypesNotAssignedToProject" />
                    </td>
                </tr>
                <tr>
                    <td class="TdTimeTypesNotAssignedToProject LineHeight19Px">
                        <div id="divTimeTypesNotAssignedToProject" class="cbfloatRight DivTimeTypes" runat="server">
                            <table id="tblTimeTypesNotAssignedToProject" class="WholeWidth WholeWidthForAllColumns"
                                cellpadding="0" cellspacing="0">
                                <tbody>
                                    <tr isfilteredrow="false" id="tblTimeTypesNotAssignedToProjectDefault" runat="server"
                                        class="Height0pxImp">
                                        <td class="TextDefault">
                                            Default
                                        </td>
                                    </tr>
                                    <asp:Repeater ID="repDefaultTimeTypesNotAssignedToProject" runat="server">
                                        <ItemTemplate>
                                            <tr timetypename='<%# Eval("Name") %>' class="Height0pxImp">
                                                <td class="PaddingTop2PxImp PaddingBottom0pxImp">
                                                    <label id="lblTimeTypesNotAssignedToProject" for="cbTimeTypesNotAssignedToProject"
                                                        title='<%# Eval("Name") %>' runat="server" class="padLeft25">
                                                        <%# Eval("Name") %>
                                                    </label>
                                                    <input type="image" id="imgDeleteWorkType" alt="Delete Work Type" class="PaddingTop2Px VisibilityHidden padRight2"
                                                        runat="server" src="~/Images/close_16.png" title="Delete Work Type" timetypeid='<%# Eval("Id") %>' />
                                                    <input type="checkbox" class="Height16Px" id="cbTimeTypesNotAssignedToProject" runat="server" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <tr isfilteredrow="false" id="tblTimeTypesNotAssignedToProjectCustom" runat="server"
                                        class="Height0pxImp">
                                        <td class="TextDefault">
                                            Custom
                                        </td>
                                    </tr>
                                    <asp:Repeater ID="repCustomTimeTypesNotAssignedToProject" OnItemDataBound="rep_OnItemDataBound"
                                        runat="server">
                                        <ItemTemplate>
                                            <tr timetypename='<%# Eval("Name") %>' class="Height0pxImp">
                                                <td class="PaddingTop2PxImp PaddingBottom0pxImp">
                                                    <label id="lblTimeTypesNotAssignedToProject" for="cbTimeTypesNotAssignedToProject"
                                                        title='<%# Eval("Name") %>' runat="server" class="padLeft25">
                                                        <%# Eval("Name") %>
                                                    </label>
                                                    <input type="image" id="imgDeleteWorkType" runat="server" alt="Delete Work Type"
                                                        class="PaddingTop2Px padRight2" src="~/Images/close_16.png" title="Delete Work Type"
                                                        timetypeid='<%# Eval("Id") %>' onclick="return DeleteWorkType(this.getAttribute('timetypeid'));" />
                                                    <input type="checkbox" class="Height16Px" id="cbTimeTypesNotAssignedToProject" runat="server" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </td>
                    <td class="TextAlignCenterImp vMiddle Width8Percent">
                        <table class="WholeWidth">
                            <tr>
                                <td class="paddingBottom5px WholeWidth">
                                    <asp:Button ID="btnAssignAll" UseSubmitBehavior="false" Text=">>" ToolTip="Add All"
                                        OnClientClick="setDirty();" OnClick="btnAssignAll_OnClick" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="paddingBottom5px">
                                    <asp:Button ID="btnAssign" UseSubmitBehavior="false" Text=">" ToolTip="Add Selected"
                                        OnClientClick="setDirty();" OnClick="btnAssign_OnClick" runat="server" />
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="paddingBottom5px">
                                    <asp:Button ID="btnUnAssign" UseSubmitBehavior="false" Text="<" ToolTip="Remove Selected"
                                        OnClientClick="setDirty();" OnClick="btnUnAssign_OnClick" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="paddingBottom5px">
                                    <asp:Button ID="btnUnAssignAll" UseSubmitBehavior="false" Text="<<" ToolTip="Remove All"
                                        OnClientClick="setDirty();" OnClick="btnUnAssignAll_OnClick" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="TdTimeTypesAssignedToProject">
                        <div id="divTimeTypesAssignedToProject" runat="server" class="cbfloatRight DivTimeTypes LineHeight19Px ValignMiddleImp">
                            <table id="tblTimeTypesAssignedToProject" class="WholeWidth WholeWidthForAllColumns"
                                cellpadding="0" cellspacing="0">
                                <tbody>
                                    <tr isfilteredrow="false" id="tblTimeTypesAssignedToProjectDefault" runat="server"
                                        class="Height0pxImp">
                                        <td class="TextDefault">
                                            Default
                                        </td>
                                    </tr>
                                    <asp:Repeater ID="repDefaultTimeTypesAssignedToProject" runat="server">
                                        <ItemTemplate>
                                            <tr timetypename='<%# Eval("Name") %>' class="Height0pxImp">
                                                <td class="PaddingTop2PxImp PaddingBottom0pxImp">
                                                    <label id="Label1" for="cbTimeTypesAssignedToProject" title='<%# Eval("Name") %>'
                                                        runat="server" class="padLeft25">
                                                        <%# Eval("Name") %>
                                                    </label>
                                                    <input type="image" id="imgDeleteWorkType" alt="Delete Work Type" class="PaddingTop2Px VisibilityHidden padRight2"
                                                        runat="server" src="~/Images/close_16.png" title="Delete Work Type" timetypeid='<%# Eval("Id") %>' />
                                                    <input id="cbTimeTypesAssignedToProject" class="Height16Px" type="checkbox" runat="server" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <tr isfilteredrow="false" id="tblTimeTypesAssignedToProjectCustom" runat="server"
                                        class="Height0pxImp">
                                        <td class="TextDefault">
                                            Custom
                                        </td>
                                    </tr>
                                    <asp:Repeater ID="repCustomTimeTypesAssignedToProject" OnItemDataBound="rep_OnItemDataBound"
                                        runat="server">
                                        <ItemTemplate>
                                            <tr timetypename='<%# Eval("Name") %>' class="Height0pxImp">
                                                <td class="PaddingTop2PxImp PaddingBottom0pxImp">
                                                    <label id="Label2" for="cbTimeTypesAssignedToProject" title='<%# Eval("Name") %>'
                                                        runat="server" class="padLeft25">
                                                        <%# Eval("Name") %>
                                                    </label>
                                                    <input id="imgDeleteWorkType" type="image" runat="server" alt="Delete Work Type"
                                                        class="PaddingTop2Px padRight2" src="~/Images/close_16.png" title="Delete Work Type"
                                                        timetypeid='<%# Eval("Id") %>' onclick="return DeleteWorkType(this.getAttribute('timetypeid'));" />
                                                    <input id="cbTimeTypesAssignedToProject" class="Height16Px" type="checkbox" runat="server" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </td>
        <td class="Width15Percent">
        </td>
    </tr>
</table>
<asp:Panel ID="pnlAddNewTimeType" runat="server" Style="display: none;" CssClass="Width375Px PanelPerson">
    <table class="Width100Per Padding5 projectTimeTypes">
        <tr class="BackGroundColorGray height20P">
            <th class="TdTextAddWorkType">
                Add Work Type
            </th>
            <th class="Width15Px">
                <asp:Button ID="Button1" runat="server" CssClass="mini-report-close floatright" ToolTip="Close"
                    OnClick="btnCloseWorkType_OnClick" Text="X"></asp:Button>
            </th>
        </tr>
        <tr class="Height5pxImp">
            <td colspan="2" class="Height5pxImp">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="no-wrap Height25Px LeftPadding10px TextAlignCenterImp">
                <asp:TextBox ID="txtNewTimeType" CssClass="Width90PercentImp" MaxLength="50" runat="server"></asp:TextBox>
                <ajax:FilteredTextBoxExtender ID="fteNewTimeType" TargetControlID="txtNewTimeType"
                    FilterMode="ValidChars" FilterType="UppercaseLetters,LowercaseLetters,Numbers,Custom"
                    ValidChars=" " runat="server">
                </ajax:FilteredTextBoxExtender>
                <asp:RequiredFieldValidator ID="rvNewTimeType" runat="server" ControlToValidate="txtNewTimeType"
                    ErrorMessage="Work Type Name is required" ValidationGroup="NewTimeType" Display="Dynamic"
                    ToolTip="Work Type Name is required">*</asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvNewTimeTypeName" runat="server" ControlToValidate="txtNewTimeType"
                    ValidationGroup="NewTimeType" OnServerValidate="cvNewTimeTypeName_OnServerValidate"
                    ErrorMessage="Work Type with this name already exists. Please enter a different Work Type name."
                    Display="Dynamic" ToolTip="Work Type with this name already exists. Please enter a different Work Type name.">*</asp:CustomValidator>
            </td>
            <td class="Width15Px">
            </td>
        </tr>
        <tr>
            <td class="WhiteSpaceNormal LeftPadding10px">
                <asp:ValidationSummary ID="vsumNewTimeType" runat="server" EnableClientScript="false"
                    ValidationGroup="NewTimeType" />
            </td>
            <td class="Width15Px">
            </td>
        </tr>
        <tr>
            <td class="TextAlignCenterImp LeftPadding10px">
                <asp:Button ID="btnInsertTimeType" runat="server" OnClick="btnInsertTimeType_OnClick"
                    ToolTip="Confirm" Text="Add" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancleTimeType" runat="server" ToolTip="Cancel" Text="Cancel"
                    OnClick="btnCloseWorkType_OnClick" />
            </td>
            <td class="Width15Px">
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td class="Width15Px">
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:HiddenField ID="hdTimetypeAlertMessage" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeTimetypeAlertMessage" runat="server"
    BehaviorID="mpeTimetypeAlertMessage" TargetControlID="hdTimetypeAlertMessage"
    BackgroundCssClass="modalBackground" PopupControlID="pnlTimetypeAlertMessage"
    DropShadow="false" CancelControlID="btnClose" />
<asp:Panel ID="pnlTimetypeAlertMessage" runat="server" Style="display: none" CssClass="PanelPerson Width380px">
    <table class="Width100Per Padding5">
        <tr>
            <th class="TextAlignCenterImp BackGroundColorGray vBottom">
                <b class="FontSize14px PaddingTop2Px">Attention!</b>
                <asp:Button ID="btnClose" runat="server" CssClass="mini-report-close floatright"
                    ToolTip="Close" OnClientClick="return btnClose_OnClientClick();" Text="X"></asp:Button>
            </th>
        </tr>
        <tr>
            <td class="fontBold Padding8">
                Time has already been entered for the following Work Type(s). The Work Type(s) cannot
                be unassigned from this project.
            </td>
        </tr>
        <tr>
            <td class="TdLblAlertMessage">
                <asp:Label ID="lbAlertMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="TextAlignCenterImp paddingBottom10px Width100Per">
                <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="return btnClose_OnClientClick();" />
            </td>
        </tr>
    </table>
</asp:Panel>

