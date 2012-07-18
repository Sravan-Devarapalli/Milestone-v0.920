<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectTimeTypes.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectTimeTypes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register TagPrefix="uc" Namespace="PraticeManagement.Controls.Generic" Assembly="PraticeManagement" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<table class="WholeWidth">
    <tr>
        <td style="padding-top: 5px; width: 100%; text-align: right;">
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
        <td align="left" valign="middle" style="width: 10%;">
        </td>
        <td align="left" valign="middle" style="width: 75%; vertical-align: middle;">
            <table class="WholeWidth">
                <tr style="padding-top: 2px;">
                    <td align="center" style="padding: 2px 6px 0px 6px; vertical-align: top; text-align: center;
                        width: 44%">
                        <b>Work Types Not Assigned to Project</b>
                    </td>
                    <td valign="middle" align="center" style="width: 8%">
                    </td>
                    <td align="center" style="padding: 2px 6px 0px 6px; text-align: center; width: 44%">
                        <b>Work Types Assigned to Project</b>
                        <asp:CustomValidator ID="cvTimetype" runat="server" ValidationGroup="Project" OnServerValidate="cvTimetype_OnServerValidate"
                            Display="Dynamic" ErrorMessage="Atleast one WorkType should be assigned to the project."
                            ToolTip="Atleast one WorkType should be assigned to the project." Text="*"></asp:CustomValidator>
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr style=" height:auto !important;">
                    <td style="padding: 0px 6px 0px 6px; width: 44%; margin: auto; vertical-align: middle;">
                        <asp:TextBox ID="txtTimeTypesNotAssignedToProject" runat="server" Width="100%" Style="border: 1px solid black;
                            padding-left: 1px; padding-right: 1px;"></asp:TextBox>
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmTimeTypesNotAssignedToProject"
                            runat="server" TargetControlID="txtTimeTypesNotAssignedToProject" WatermarkText="Begin typing to sort list below..."
                            WatermarkCssClass="watermarkedtext" />
                    </td>
                    <td valign="middle" align="center" style="width: 8%">
                    </td>
                    <td style="padding: 0px 6px 0px 6px; width: 44%; margin: auto; vertical-align: middle;">
                        <asp:TextBox ID="txtTimeTypesAssignedToProject" runat="server" Width="100%" MaxLength="50"
                            Style="border: 1px solid black; padding-left: 1px; padding-right: 1px;"></asp:TextBox>
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server"
                            TargetControlID="txtTimeTypesAssignedToProject" WatermarkText="Begin typing to sort list below..."
                            WatermarkCssClass="watermarkedtext" />
                    </td>
                </tr>
                <tr>
                    <td style="padding: 0px 6px 0px 6px; width: 44%; margin: auto; line-height: 19px;
                        vertical-align: middle;">
                        <div id="divTimeTypesNotAssignedToProject" class="cbfloatRight" runat="server" style="height: 150px !important;
                            margin: auto; width: 100%; overflow-y: auto; border: 1px solid black; background: white;
                            padding: 1px;">
                            <table id="tblTimeTypesNotAssignedToProject" class="WholeWidth WholeWidthForAllColumns" cellpadding="0" cellspacing="0">
                                <tbody>
                                    <tr isfilteredrow="false" id="tblTimeTypesNotAssignedToProjectDefault" runat="server" style="height:0px !important;">
                                        <td style="padding-top: 2px; padding-bottom: 4px; font-weight: bold; font-style: italic;">
                                            Default
                                        </td>
                                    </tr>
                                    <asp:Repeater ID="repDefaultTimeTypesNotAssignedToProject" runat="server">
                                        <ItemTemplate>
                                            <tr timetypename='<%# Eval("Name") %>' style="height:0px !important;">
                                                <td style="padding-top: 2px; padding-bottom:0px !important;">
                                                    <label id="lblTimeTypesNotAssignedToProject" for="cbTimeTypesNotAssignedToProject"
                                                        title='<%# Eval("Name") %>' runat="server" style="padding-left: 25px;">
                                                        <%# Eval("Name") %>
                                                    </label>
                                                    <input type="image" id="imgDeleteWorkType" alt="Delete Work Type" style="padding-top: 2px;
                                                        visibility: hidden;" runat="server" src="~/Images/close_16.png" title="Delete Work Type"
                                                        timetypeid='<%# Eval("Id") %>' />
                                                    <input type="checkbox" style="height: 16px; " id="cbTimeTypesNotAssignedToProject"
                                                        runat="server" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <tr isfilteredrow="false" id="tblTimeTypesNotAssignedToProjectCustom" runat="server" style="height:0px !important;">
                                        <td style="padding-top: 2px; padding-bottom: 4px; font-weight: bold; font-style: italic;">
                                            Custom
                                        </td>
                                    </tr>
                                    <asp:Repeater ID="repCustomTimeTypesNotAssignedToProject" OnItemDataBound="rep_OnItemDataBound"
                                        runat="server">
                                        <ItemTemplate>
                                            <tr timetypename='<%# Eval("Name") %>' style="height:0px !important;">
                                                <td style="padding-top: 2px; padding-bottom:0px !important;">
                                                    <label id="lblTimeTypesNotAssignedToProject" for="cbTimeTypesNotAssignedToProject"
                                                        title='<%# Eval("Name") %>' runat="server" style="padding-left: 25px;">
                                                        <%# Eval("Name") %>
                                                    </label>
                                                    <input type="image" id="imgDeleteWorkType" runat="server" alt="Delete Work Type"
                                                        style="padding-top: 2px;" src="~/Images/close_16.png" title="Delete Work Type"
                                                        timetypeid='<%# Eval("Id") %>' onclick="return DeleteWorkType(this.getAttribute('timetypeid'));" />
                                                    <input type="checkbox" style="height: 16px; " id="cbTimeTypesNotAssignedToProject"
                                                        runat="server" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </td>
                    <td valign="middle" align="center" style="width: 8%">
                        <asp:Button ID="btnAssignAll" UseSubmitBehavior="false" Text=">>" ToolTip="Add All" OnClientClick="setDirty();"
                            OnClick="btnAssignAll_OnClick" runat="server" />
                        <br />
                        <asp:Button ID="btnAssign" UseSubmitBehavior="false" Text=">" ToolTip="Add Selected" OnClientClick="setDirty();"
                            OnClick="btnAssign_OnClick" runat="server" />
                        <br />
                        <br />
                        <asp:Button ID="btnUnAssign" UseSubmitBehavior="false" Text="<" ToolTip="Remove Selected" OnClientClick="setDirty();"
                            OnClick="btnUnAssign_OnClick" runat="server" />
                        <br />
                        <asp:Button ID="btnUnAssignAll" UseSubmitBehavior="false" Text="<<" ToolTip="Remove All" OnClientClick="setDirty();"
                            OnClick="btnUnAssignAll_OnClick" runat="server" />
                    </td>
                    <td style="padding-left: 6px; padding-right: 6px; padding-top: 0px; width: 44%; margin: auto;">
                        <div id="divTimeTypesAssignedToProject" runat="server" class="cbfloatRight" style="height: 150px !important;
                            margin: auto; width: 100%; overflow-y: auto; border: 1px solid black; background: white;
                            line-height: 19px; vertical-align: middle; padding: 1px;">
                            <table id="tblTimeTypesAssignedToProject" class="WholeWidth WholeWidthForAllColumns" cellpadding="0" cellspacing="0">
                                <tbody>
                                    <tr isfilteredrow="false" id="tblTimeTypesAssignedToProjectDefault" runat="server" style="height:0px !important;">
                                        <td style="padding-top: 2px; padding-bottom: 4px; font-weight: bold; font-style: italic;">
                                            Default
                                        </td>
                                    </tr>
                                    <asp:Repeater ID="repDefaultTimeTypesAssignedToProject" runat="server">
                                        <ItemTemplate>
                                            <tr timetypename='<%# Eval("Name") %>' style="height:0px !important;">
                                                <td style="padding-top: 2px; padding-bottom:0px !important;">
                                                    <label for="cbTimeTypesAssignedToProject" title='<%# Eval("Name") %>' runat="server"
                                                        style="padding-left: 25px;">
                                                        <%# Eval("Name") %>
                                                    </label>
                                                    <input type="image" id="imgDeleteWorkType" alt="Delete Work Type" style="padding-top: 2px;
                                                        visibility: hidden;" runat="server" src="~/Images/close_16.png" title="Delete Work Type"
                                                        timetypeid='<%# Eval("Id") %>' />
                                                    <input id="cbTimeTypesAssignedToProject" style="height: 16px;" type="checkbox" runat="server" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <tr isfilteredrow="false" id="tblTimeTypesAssignedToProjectCustom" runat="server" style="height:0px !important;">
                                        <td style="padding-top: 2px; padding-bottom: 4px; font-weight: bold; font-style: italic;">
                                            Custom
                                        </td>
                                    </tr>
                                    <asp:Repeater ID="repCustomTimeTypesAssignedToProject" OnItemDataBound="rep_OnItemDataBound"
                                        runat="server">
                                        <ItemTemplate>
                                            <tr timetypename='<%# Eval("Name") %>' style="height:0px !important;">
                                                <td style="padding-top: 2px; padding-bottom:0px !important;">
                                                    <label for="cbTimeTypesAssignedToProject" title='<%# Eval("Name") %>' runat="server"
                                                        style="padding-left: 25px;">
                                                        <%# Eval("Name") %>
                                                    </label>
                                                    <input id="imgDeleteWorkType" type="image" runat="server" alt="Delete Work Type"
                                                        style="padding-top: 2px;" src="~/Images/close_16.png" title="Delete Work Type"
                                                        timetypeid='<%# Eval("Id") %>' onclick="return DeleteWorkType(this.getAttribute('timetypeid'));" />
                                                    <input id="cbTimeTypesAssignedToProject" style="height: 16px;" type="checkbox" runat="server" />
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
        <td style="width: 15%;">
        </td>
    </tr>
</table>
<asp:Panel ID="pnlAddNewTimeType" runat="server" BackColor="White" BorderColor="Black"
    Style="display: none; width: 375px;" BorderWidth="2px">
    <table width="100%" style="padding: 5px;" class="projectTimeTypes">
        <tr style="background-color: Gray; height: 20px;">
            <th valign="middle" align="center" style="text-align: center; color: Black; font-weight: bold;
                background-color: Gray;">
                Add Work Type
            </th>
            <th style="width: 15px;">
                <asp:Button ID="btnCloseWorkType" runat="server" CssClass="mini-report-close" ToolTip="Close"
                    Style="float: right;" OnClick="btnCloseWorkType_OnClick" Text="X"></asp:Button>
            </th>
        </tr>
        <tr style="height: 5px !important;">
            <td colspan="2" style="height: 5px !important;">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap; height: 25px !important; padding-left: 10px !important;">
                <asp:TextBox ID="txtNewTimeType" Width="90%" MaxLength="50" runat="server"></asp:TextBox>
                <ajax:FilteredTextBoxExtender ID="fteNewTimeType" TargetControlID="txtNewTimeType"
                    FilterMode="ValidChars" FilterType="UppercaseLetters,LowercaseLetters,Numbers,Custom"
                    ValidChars=" " runat="server">
                </ajax:FilteredTextBoxExtender>
                <asp:RequiredFieldValidator ID="rvNewTimeType" runat="server" ControlToValidate="txtNewTimeType"
                    ErrorMessage="Work Type Name is required" ValidationGroup="NewTimeType" Display="Dynamic"
                    ToolTip="Work Type Name is required">*</asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvNewTimeTypeName" runat="server" ControlToValidate="txtNewTimeType"
                    ValidationGroup="NewTimeType" OnServerValidate="cvNewTimeTypeName_OnServerValidate"
                    ErrorMessage="This work type already exists. Please enter a different work type."
                    Display="Dynamic" ToolTip="This work type already exists. Please enter a different work type.">*</asp:CustomValidator>
            </td>
            <td style="width: 15px !important;">
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap;padding-left: 10px !important;">
                <asp:ValidationSummary ID="vsumNewTimeType" runat="server" EnableClientScript="false"
                    ValidationGroup="NewTimeType" />
            </td>
            <td style="width: 15px;">
            </td>
        </tr>
        <tr>
            <td align="center" style="padding-left: 10px;">
                <asp:Button ID="btnInsertTimeType" runat="server" OnClick="btnInsertTimeType_OnClick"
                    ToolTip="Confirm" Text="Add" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancleTimeType" runat="server" ToolTip="Cancel" Text="Cancel"
                    OnClick="btnCloseWorkType_OnClick" />
            </td>
            <td style="width: 15px;">
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td style="width: 15px;">
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:HiddenField ID="hdTimetypeAlertMessage" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeTimetypeAlertMessage" runat="server"
    BehaviorID="mpeTimetypeAlertMessage" TargetControlID="hdTimetypeAlertMessage"
    BackgroundCssClass="modalBackground" PopupControlID="pnlTimetypeAlertMessage"
    DropShadow="false" CancelControlID="btnClose" />
<asp:Panel ID="pnlTimetypeAlertMessage" runat="server" BackColor="White" BorderColor="Black"
    Style="display: none" BorderWidth="2px" Width="380px">
    <table width="100%" style="padding: 5px;">
        <tr>
            <th align="center" style="text-align: center; background-color: Gray;" valign="bottom">
                <b style="font-size: 14px; padding-top: 2px;">Attention!</b>
                <asp:Button ID="btnClose" runat="server" CssClass="mini-report-close" ToolTip="Close"
                    Style="float: right;" OnClientClick="return btnClose_OnClientClick();" Text="X">
                </asp:Button>
            </th>
        </tr>
        <tr>
            <td style="font-weight: bold; padding: 8px;">
                Time has already been entered for the following Work Type(s). The Work Type(s) cannot
                be unassigned from this project.
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap; padding-left: 10px; padding-right: 20px;">
                <asp:Label ID="lbAlertMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center" style="padding-bottom: 10px;width:100%;">
                <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="return btnClose_OnClientClick();" />
            </td>
        </tr>
    </table>
</asp:Panel>

