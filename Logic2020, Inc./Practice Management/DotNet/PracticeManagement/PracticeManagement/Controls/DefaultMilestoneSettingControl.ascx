<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultMilestoneSettingControl.ascx.cs" Inherits="PraticeManagement.Controls.DefaultMilestoneSettingControl" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ElementDisabler" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

  <div style="padding-top: 30px;">
        <table>
            <tr>
                <td style="padding-bottom: 20px;">
                    <label>
                        Lower Bound</label>
                    <asp:TextBox ID="txtLowerBound" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rqLowerBound" runat="server" ErrorMessage="Lower Bound cannot be empty."
                        ControlToValidate="txtLowerBound" ValidationGroup="LowerUpperBounds" 
                        ToolTip="Lower Bound cannot be empty.">*</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="regvLowerBound" runat="server" ErrorMessage="Select an integer for lower bound."
                        ToolTip="Select an integer for lower bound." ValidationExpression="^[1-9]+[0-9]*$"
                        ValidationGroup="LowerUpperBounds" ControlToValidate="txtLowerBound">*
                    </asp:RegularExpressionValidator>
                </td>
                <td style="padding-bottom: 20px;">
                    <label>
                        Upper Bound</label>
                    <asp:TextBox ID="txtUpperBound" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rqUpperBound" runat="server" ErrorMessage="Upper Bound cannot be empty."
                        ControlToValidate="txtUpperBound" ValidationGroup="LowerUpperBounds" 
                        ToolTip="Upper Bound cannot be empty.">*</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="regvUpperBound" runat="server" ErrorMessage="Select an integer for upper bound."
                        ToolTip="Select an integer for upper bound." ValidationExpression="^[1-9]+[0-9]*$"
                        ValidationGroup="LowerUpperBounds" ControlToValidate="txtUpperBound">*
                    </asp:RegularExpressionValidator>
                    <asp:Button ID="btnSaveBounds" runat="server" Text="Save Bounds" OnClick="btnSaveBounds_OnClick"
                        ValidationGroup="LowerUpperBounds" />
                </td>
            </tr>
        </table>
        <table border="0" cellpadding="3" cellspacing="3" width="700px">
            <tr>
                <td style="width: 80px;">
                    <label for="<%=ddlClients.ClientID%>">
                        Client</label>
                </td>
                <td style="padding-left: 5px; padding-right: 5px;">
                    <asp:DropDownList ID="ddlClients" Width="200" runat="server">
                    </asp:DropDownList>
                    <asp:CustomValidator ID="cvClients" runat="server" ControlToValidate="ddlClients" ValidationGroup="cvMilestone"
                        EnableClientScript="false" SetFocusOnError="true" ValidateEmptyText="true" ErrorMessage="Please Select a Client"
                        Display="None" OnServerValidate="cvClients_Validate">*</asp:CustomValidator>
                </td>
                <td>
                    &nbsp;
                </td>
                <td style="width: 80px;">
                    <label for="<%=ddlProjects.ClientID%>">
                        Project</label>
                </td>
                <td style="padding-left: 5px; padding-right: 5px;">
                    <asp:DropDownList ID="ddlProjects" Width="300" runat="server">
                    </asp:DropDownList>
                    <asp:CustomValidator ID="cvProjects" runat="server" ControlToValidate="ddlProjects" ValidationGroup="cvMilestone"
                        EnableClientScript="false" SetFocusOnError="true" ValidateEmptyText="true" ErrorMessage="Please Select a Project"
                        Display="None" OnServerValidate="cvProjects_Validate">*</asp:CustomValidator>
                    <ajax:CascadingDropDown ID="cddClientProjects" runat="server" ParentControlID="ddlClients"
                        TargetControlID="ddlProjects" Category="Group" LoadingText="Loading Projects..."
                        EmptyText="No Projects found" ScriptPath="~/Scripts/CascadingDropDownBehavior.js"
                        ServicePath="~/CompanyPerfomanceServ.asmx" ServiceMethod="GetProjects" UseContextKey="true"
                        PromptText="Please Select a Project" />
                </td>
                <td>
                    &nbsp;
                </td>
                <td style="width: 80px;">
                    <label for="<%=ddlMileStones.ClientID%>">
                        Milestone</label>
                </td>
                <td style="padding-left: 5px; padding-right: 5px;">
                    <asp:DropDownList ID="ddlMileStones" Width="250" runat="server">
                    </asp:DropDownList>
                    <asp:CustomValidator ID="cvMileStones" runat="server" ControlToValidate="ddlMileStones" ValidationGroup="cvMilestone"
                        EnableClientScript="false" SetFocusOnError="true" ValidateEmptyText="true" ErrorMessage="Please Select a Milestone"
                        Display="None" OnServerValidate="cvMileStones_Validate">*</asp:CustomValidator>
                    <ajax:CascadingDropDown ID="cddProjectMileStones" runat="server" ParentControlID="ddlProjects"
                        TargetControlID="ddlMileStones" Category="Group" LoadingText="Loading Milestones..."
                        EmptyText="No Milestones found" ScriptPath="~/Scripts/CascadingDropDownBehavior.js"
                        ServicePath="~/CompanyPerfomanceServ.asmx" ServiceMethod="GetMilestones" PromptText="Please Select a Milestone" />
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="9" style="padding-top: 20px;">
                    <asp:CustomValidator ID="cvError" runat="server" ValidationGroup="cvErrorGroup"></asp:CustomValidator>
                    <asp:ValidationSummary ID="vsMilestone" runat="server" ValidationGroup="cvMilestone" />
                    <asp:ValidationSummary ID="vsInvalidUpperLowerBound" runat="server" ValidationGroup="LowerUpperBounds" />
                </td>
            </tr>
            <tr>
                <td colspan="9">
                    <asp:Label ID="lblSuccessMessage" runat="server" ForeColor="Green" Visible="false"
                        EnableViewState="false">
                    </asp:Label>
                    <asp:Label ID="lblInvalidMilestoneMessage" runat="server" ForeColor="Red" Visible="false"
                        EnableViewState="false" Text="Selected Milestone does not fit into the lower and upper bounds.">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    &nbsp;
                </td>
                <td align="center" style="padding-top: 10px;">
                    <asp:Button ID="btnSave" runat="server" Text="Save Milestone" ValidationGroup="cvMilestone" OnClick="btnSave_Click" />
                    <ext:ElementDisablerExtender ID="edeSave" runat="server" TargetControlID="btnSave"
                        ControlToDisableID="btnSave" />
                </td>
                <td colspan="4">
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>

