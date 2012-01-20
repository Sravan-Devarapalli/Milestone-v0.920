<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectTimeTypes.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectTimeTypes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register TagPrefix="uc" Namespace="PraticeManagement.Controls.Generic" Assembly="PraticeManagement" %>
<table width="100%" class="WholeWidth">
    <tr>
        <td style="padding-top: 5px;">
            <asp:HiddenField ID="hdnTimeTypesAssignedToProject" runat="server" Value="" EnableViewState="true" />
            <asp:HiddenField ID="hdnProjectTimeTypesInUse" runat="server" Value="" EnableViewState="true" />
        </td>
    </tr>
    <tr>
        <td style="padding-top: 5px;">
            <asp:ImageButton ID="imgAddNewTimeType" runat="server" ImageUrl="~/Images/add_16.png"
                ToolTip="Add New WorkType" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeAddTimeType" runat="server" TargetControlID="imgAddNewTimeType"
                BackgroundCssClass="modalBackground" PopupControlID="pnlAddNewTimeType" DropShadow="false"
                CancelControlID="imgCancelTimeType" />
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
                    <td align="center" style="padding-left: 6px; padding-right: 6px; text-align: center;
                        padding-top: 2px; width: 44%">
                        <b>Work Types Assigned to Project</b>
                    </td>
                    <td valign="middle" align="center" style="width: 12%">
                    </td>
                    <td align="center" style="padding-left: 6px; padding-right: 6px; padding-top: 2px;
                        vertical-align: top; text-align: center; width: 44%">
                        <b>Work Types Not Assigned to Project</b>
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr>
                    <td style="padding-left: 6px; padding-right: 6px; padding-top: 0px; width: 44%; margin: auto;">
                        <div id="divTimeTypesAssignedToProject" runat="server" class="cbfloatRight" style="height: 150px !important;
                            margin: auto; width: 100%; overflow-y: auto; border: 1px solid black; background: white;
                            line-height: 19px; vertical-align: middle; padding-left: 1px;">
                            <uc:CustomCheckBoxList ID="cblTimeTypesAssignedToProject" runat="server" Width="100%"
                                BackColor="White" CssClass="WholeWidth" AutoPostBack="false" DataTextField="Name"
                                DataValueField="id" CellSpacing="0" CellPadding="0">
                            </uc:CustomCheckBoxList>
                        </div>
                    </td>
                    <td valign="middle" align="center" style="width: 12%">
                        <input id="btnUnAssignAll" type="button" onclick="UnAssignAllTimeTypes_Click();SetTimeTypesAssignedToProject();"
                            title="Add All" value=">>" /><br />
                        <input id="btnUnAssign" type="button" onclick="UnAssignTimeType_Click();SetTimeTypesAssignedToProject();"
                            title="Add Selected" value=">" />
                        <br />
                        <br />
                        <input id="btnAssign" type="button" onclick="AssignTimeType_Click();SetTimeTypesAssignedToProject();"
                            title="Remove Selected" value="<" />
                        <br />
                        <input id="btnAssignAll" type="button" onclick="AssignAllTimeTypes_Click();SetTimeTypesAssignedToProject();"
                            title="Remove All" value="<<" />
                    </td>
                    <td style="padding-left: 6px; padding-right: 6px; padding-top: 0px; width: 44%; margin: auto;
                        line-height: 19px; vertical-align: middle;">
                        <div id="divTimeTypesNotAssignedToProject" runat="server" class="cbfloatRight" style="height: 150px !important;
                            margin: auto; width: 100%; overflow-y: auto; border: 1px solid black; background: white;
                            padding-left: 1px;">
                            <uc:CustomCheckBoxList ID="cblTimeTypesNotAssignedToProject" runat="server" AutoPostBack="false"
                                BackColor="White" CssClass="WholeWidth" Width="100%" DataTextField="Name" DataValueField="id"
                                CellPadding="0" CellSpacing="0">
                            </uc:CustomCheckBoxList>
                        </div>
                    </td>
                </tr>
            </table>
            <%-- <table class="WholeWidth">
                <tr>
                    <td style="padding: 4px; height: 35px; width: 70%;">
                        <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                        <asp:ValidationSummary ID="vsumNotes" runat="server" ValidationGroup="Notes" />
                    </td>
                    <td align="right" style="padding: 4px; padding-right: 0px; height: 35px; width: 30%;">
                        <asp:Button ID="btnSave" Enabled="false" runat="server" Text="Save Changes" OnClientClick="hideSuccessMessage();"
                            OnClick="btnSave_Click" />
                    </td>
                </tr>
            </table>--%>
        </td>
        <td style="width: 15%;">
        </td>
    </tr>
</table>
<asp:Panel ID="pnlAddNewTimeType" runat="server" BackColor="White" BorderColor="Black"
    Style="display: none" BorderWidth="2px">
    <table width="100%" style="padding: 5px;">
        <tr style="background-color: Gray; height: 27px;">
            <td align="center" style="white-space: nowrap; font-size: 14px; width: 100%">
                Add New WorkType
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap; padding-left: 10px; padding-right: 20px;">
                <asp:TextBox ID="txtNewTimeType" MaxLength="50" runat="server"></asp:TextBox>
                <ajax:FilteredTextBoxExtender ID="fteNewTimeType" TargetControlID="txtNewTimeType"
                    FilterType="LowercaseLetters ,UppercaseLetters,Custom" FilterMode="ValidChars"
                    runat="server">
                </ajax:FilteredTextBoxExtender>
                <asp:RequiredFieldValidator ID="rvUpdatedTimeType" runat="server" ControlToValidate="txtNewTimeType"
                    Display="Dynamic" ErrorMessage="Work Type Name is required" ToolTip="Work Type Name is required"
                    ValidationGroup="UpdateTimeType">*</asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvUpdatedTimeTypeName" runat="server" ControlToValidate="txtNewTimeType"
                    Display="Dynamic" ToolTip="This work type already exists. Please enter a different work type.">*</asp:CustomValidator>
                <asp:ImageButton ID="imgUpdateTimeType" runat="server" ImageUrl="~/Images/icon-check.png"
                    OnClick="imgUpdateTimeType_OnClick" ToolTip="Confirm" />
                <asp:ImageButton ID="imgCancelTimeType" runat="server" ImageUrl="~/Images/no.png"
                    ToolTip="Cancel" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:HiddenField ID="hdTimetypeAlertMessage" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeTimetypeAlertMessage" runat="server" BehaviorID="mpeTimetypeAlertMessage"
    TargetControlID="hdTimetypeAlertMessage" BackgroundCssClass="modalBackground"
    PopupControlID="pnlTimetypeAlertMessage" DropShadow="false" CancelControlID="btnClose" />
<asp:Panel ID="pnlTimetypeAlertMessage" runat="server" BackColor="White" BorderColor="Black"
    Style="display: none" BorderWidth="2px"  Width="380px">
    <table width="100%" style="padding: 5px; ">
        <tr>
            <th align="center" style="text-align: center; background-color: Gray;" 
                valign="bottom">
                <b style="font-size: 14px; padding-top: 2px;">Attention!</b>
                <asp:Button ID="btnClose" runat="server" CssClass="mini-report-close" ToolTip="Close"
                    Style="float: right;" OnClientClick="return btnClose_OnClientClick();" Text="X"></asp:Button>
            </th>
        </tr>
        <tr>
            <td style="font-weight:bold;">
                &nbsp;
                Below WorkTypes are having time entries <br />
                 for this project so they can't be unassign from the project: <br /><br />
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap; padding-left: 10px; padding-right: 20px;">
                <asp:Label ID="lbAlertMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
                &nbsp;
                &nbsp;

            </td>
        </tr>
    </table>
</asp:Panel>

