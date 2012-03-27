<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectTimeTypes.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectTimeTypes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register TagPrefix="uc" Namespace="PraticeManagement.Controls.Generic" Assembly="PraticeManagement" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<table width="100%" class="WholeWidth">
    <tr>
        <td style="padding-top: 5px;">
            <asp:HiddenField ID="hdnTimeTypesAssignedToProject" runat="server" Value="" EnableViewState="true" />
            <asp:HiddenField ID="hdnProjectTimeTypesInUse" runat="server" Value="" EnableViewState="true" />
        </td>
    </tr>
    <tr>
        <td style="padding-top: 5px;">
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
                    <td align="center" style="padding-left: 6px; padding-right: 6px; padding-top: 2px;
                        vertical-align: top; text-align: center; width: 44%">
                        <b>Work Types Not Assigned to Project</b>
                    </td>
                    <td valign="middle" align="center" style="width: 12%">
                    </td>
                    <td align="center" style="padding-left: 6px; padding-right: 6px; text-align: center;
                        padding-top: 2px; width: 44%">
                        <b>Work Types Assigned to Project</b>
                        <asp:CustomValidator ID="cvTimetype" runat="server" ValidationGroup="Project" OnServerValidate="cvTimetype_OnServerValidate"
                            Display="Dynamic" ErrorMessage="Atleast one WorkType should be assigned to the project."
                            ToolTip="Atleast one WorkType should be assigned to the project." Text="*"></asp:CustomValidator>
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr>
                    <td style="padding-left: 6px; padding-right: 6px; padding-top: 0px; width: 44%; margin: auto;
                        line-height: 19px; vertical-align: middle;">
                        <div id="divTimeTypesNotAssignedToProject" runat="server" class="cbfloatRight" style="height: 150px !important;
                            margin: auto; width: 100%; overflow-y: auto; border: 1px solid black; background: white;
                            padding-left: 1px;">
                            <asp:TextBox ID="txtTimeTypesNotAssignedToProject" runat="server" Width="97%" Style="padding: 4px;"
                                MaxLength="50"></asp:TextBox>
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmTimeTypesNotAssignedToProject"
                                runat="server" TargetControlID="txtTimeTypesNotAssignedToProject" WatermarkText="Begin typing to sort list below..."
                                WatermarkCssClass="watermarkedtext" />
                            <uc:CustomCheckBoxList ID="cblTimeTypesNotAssignedToProject" runat="server" AutoPostBack="false"
                                BackColor="White" CssClass="WholeWidth" Width="100%" DataTextField="Name" DataValueField="id"
                                CellPadding="0" CellSpacing="0">
                            </uc:CustomCheckBoxList>
                        </div>
                    </td>
                    <td valign="middle" align="center" style="width: 12%">
                        <input id="btnAssignAll" type="button" onclick="AssignAllTimeTypes_Click();SetTimeTypesAssignedToProject();"
                            title="Remove All" value=">>" />
                        <br />
                        <input id="btnAssign" type="button" onclick="AssignTimeType_Click();SetTimeTypesAssignedToProject();"
                            title="Remove Selected" value=">" />
                        <br />
                        <br />
                        <input id="btnUnAssign" type="button" onclick="UnAssignTimeType_Click();SetTimeTypesAssignedToProject();"
                            title="Add Selected" value="<" />
                        <br />
                        <input id="btnUnAssignAll" type="button" onclick="UnAssignAllTimeTypes_Click();SetTimeTypesAssignedToProject();"
                            title="Add All" value="<<" />
                    </td>
                    <td style="padding-left: 6px; padding-right: 6px; padding-top: 0px; width: 44%; margin: auto;">
                        <div id="divTimeTypesAssignedToProject" runat="server" class="cbfloatRight" style="height: 150px !important;
                            margin: auto; width: 100%; overflow-y: auto; border: 1px solid black; background: white;
                            line-height: 19px; vertical-align: middle; padding-left: 1px;">
                            <asp:TextBox ID="txtTimeTypesAssignedToProject" runat="server" Width="97%" Style="padding: 4px;"
                                MaxLength="50"></asp:TextBox>
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server"
                                TargetControlID="txtTimeTypesAssignedToProject" WatermarkText="Begin typing to sort list below..."
                                WatermarkCssClass="watermarkedtext" />
                            <uc:CustomCheckBoxList ID="cblTimeTypesAssignedToProject" runat="server" Width="100%"
                                BackColor="White" CssClass="WholeWidth" AutoPostBack="false" DataTextField="Name"
                                DataValueField="id" CellSpacing="0" CellPadding="0">
                            </uc:CustomCheckBoxList>
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
    <table width="100%" style="padding: 5px;">
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
        <tr style="height: 5px;">
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap; height: 25px; padding-left: 10px;">
                <asp:TextBox ID="txtNewTimeType" Width="90%" MaxLength="50" runat="server"></asp:TextBox>
                <ajax:FilteredTextBoxExtender ID="fteNewTimeType" TargetControlID="txtNewTimeType"
                    FilterType="LowercaseLetters ,UppercaseLetters,Custom" FilterMode="ValidChars"
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
            <td style="width: 15px;">
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap; height: 20px; padding-left: 10px;">
                <asp:ValidationSummary ID="vsumNewTimeType" runat="server" EnableClientScript="false"
                    ValidationGroup="NewTimeType" />
            </td>
            <td style="width: 15px;">
            </td>
        </tr>
        <tr>
            <td align="center" style="height: 20px; padding-left: 10px;">
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
            <td align="center" style="padding-bottom: 10px;">
                <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="return btnClose_OnClientClick();" />
            </td>
        </tr>
    </table>
</asp:Panel>

