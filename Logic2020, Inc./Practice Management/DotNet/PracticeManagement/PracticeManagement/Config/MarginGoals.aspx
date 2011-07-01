<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="MarginGoals.aspx.cs" Inherits="PraticeManagement.Config.MarginGoals" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Margin Goals</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Margin Goals
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function applyColor(ddlColor) {
            for (var i = 0; i < ddlColor.length; i++) {
                if (ddlColor[i].selected) {
                    if (ddlColor[i].attributes["colorvalue"] != null && ddlColor[i].attributes["colorvalue"] != "undefined") {
                        ddlColor.style.backgroundColor = ddlColor[i].attributes["colorvalue"].value;
                    }
                    break;
                }
            }
        }

        function SetBackGroundColorForDdls() {
            var list = document.getElementsByTagName('select');

            for (var j = 0; j < list.length; j++) {
                applyColor(list[j]);
            }
        }

        window.onload = SetBackGroundColorForDdls;
    </script>
    <div style="background-color: #d4dff8; border-top: 1px solid #fff; border-bottom: 1px solid #eef2fc;
        padding-left: 10px;">
        <div style="padding-top: 10px; padding-bottom: 10px;">
            <b>Client Goal Default</b></div>
        <asp:UpdatePanel ID="upnlClientGoalDefault" runat="server">
            <ContentTemplate>
                <table width="100%">
                    <tr>
                        <td valign="top" style="width: 50%; padding-bottom: 10px;">
                            <table width="99%">
                                <tr>
                                    <td colspan="3" style="padding-bottom: 15px; padding-top: 15px;">
                                        <asp:CheckBox ID="chbClientGoalDefaultThreshold" AutoPostBack="true" OnCheckedChanged="chbClientGoalDefaultThreshold_OnCheckedChanged"
                                            runat="server" Checked="false" onclick="setDirty();" />&nbsp;&nbsp; Use Color-coded
                                        Margin thresholds
                                    </td>
                                    <td align="right" style="padding-bottom: 15px; padding-top: 15px;">
                                        <asp:Button ID="btnClientGoalDefaultAddThreshold" Enabled="false" runat="server"
                                            Text="Add Threshold" OnClientClick="setDirty();" OnClick="btnClientGoalDefaultAddThreshold_OnClick" />
                                    </td>
                                </tr>
                            </table>
                            <asp:GridView ID="gvClientGoalDefaultThreshold" Enabled="false" runat="server" Width="99%"
                                OnRowDataBound="gvClientGoalDefaultThreshold_RowDataBound" AutoGenerateColumns="False"
                                EmptyDataText="" DataKeyNames="Id" CssClass="CompPerfTable" GridLines="None">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemStyle Height="25px" HorizontalAlign="Center" Width="25%" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Start</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:DropDownList ID="gvClientddlStartRange" onchange="setDirty();" runat="server">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle Height="25px" HorizontalAlign="Center" Width="25%" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                End</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:DropDownList ID="gvClientddlEndRange" onchange="setDirty();" runat="server">
                                            </asp:DropDownList>
                                            <asp:CustomValidator ID="cvgvClientRange" runat="server" ToolTip="The End must be greater than or equals to Start."
                                                Text="*" EnableClientScript="false" OnServerValidate="cvgvClientRange_OnServerValidate"
                                                SetFocusOnError="true" Display="Static" ValidationGroup="Client" />
                                            <asp:CustomValidator ID="cvgvClientOverLapRange" runat="server" ToolTip="The specified Threshold Percentage range overlaps with another Threshold Percentage range."
                                                OnServerValidate="cvgvClientOverLapRange_OnServerValidate" Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Static" ValidationGroup="Client" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle Height="25px" HorizontalAlign="Center" Width="42%" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Color</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <cc2:CustomDropDown ID="gvClientddlColor" Width="85%" onclick="applyColor(this);"
                                                onchange="applyColor(this);setDirty();" runat="server">
                                            </cc2:CustomDropDown>
                                            <asp:CustomValidator ID="cvgvClientddlColor" runat="server" OnServerValidate="cvgvClientddlColor_ServerValidate"
                                                ToolTip="Please Select a Color." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                Display="Static" ValidationGroup="Client" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle Height="25px" HorizontalAlign="Center" Width="8%" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                            </div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnClientDeleteRow" runat="server" ImageUrl="~/Images/cross_icon.png"
                                                ToolTip="Delete" OnClientClick="setDirty();" OnClick="btnClientDeleteRow_OnClick" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                        <td style="width: 3%; padding-bottom: 10px;">
                        </td>
                        <td valign="top" style="width: 44%; padding-bottom: 10px;">
                            <div style="background-color: White; padding: 10px;">
                                <p>
                                    Enabling this feature and configuring color-coded ranges will allow persons without
                                    unrestricted access to Project and Milestone margin calculations a visual indication
                                    of how Projects and Milestones are tracking with regard to the margin goals defined
                                    by the company.<br />
                                    <br />
                                </p>
                                <p>
                                    Margin goals must add up to at least 100%.<br />
                                    <br />
                                </p>
                                <p>
                                    NOTE: It is also possible to specify individual Client margin goals from each Client's
                                    profile page, either in lieu of these default margin goals, or by overriding them.<br />
                                    <br />
                                </p>
                            </div>
                        </td>
                        <td style="width: 3%; padding-bottom: 10px;">
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:CustomValidator ID="cvClientThresholds" runat="server" OnServerValidate="cvClientThresholds_ServerValidate"
            ErrorMessage="Thresholds must be added up to  100% or more and must be continuous."
            ToolTip="Thresholds must be added up to  100% or more and must be continuous."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvClientColors" runat="server" OnServerValidate="cvClientColors_ServerValidate"
            ErrorMessage="Color must not be selected more than once." ToolTip="Color must not be selected more than once."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvgvddlColorClone" runat="server" ErrorMessage="Please Select a Color."
            Text="*" EnableClientScript="false" SetFocusOnError="false" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvgvOverLapRangeClone" runat="server" ErrorMessage="The specified Threshold Percentage range overlaps with another Threshold Percentage range."
            Text="*" EnableClientScript="false" SetFocusOnError="false" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvgvRangeClone" runat="server" ErrorMessage="The End must be greater than or equals to Start."
            Text="*" EnableClientScript="false" SetFocusOnError="false" Display="None" ValidationGroup="Client" />
    </div>
    <div style="background-color: #d4dff8; border-top: 1px solid #fff; border-bottom: 1px solid #eef2fc;
        padding-left: 10px;">
        <div style="padding-top: 10px; padding-bottom: 10px;">
            <b>Person Goal</b></div>
        <asp:UpdatePanel ID="upnlPersonThrsholds" runat="server">
            <ContentTemplate>
                <table width="100%">
                    <tr>
                        <td valign="top" style="width: 50%; padding-bottom: 10px;">
                            <table width="99%">
                                <tr>
                                    <td colspan="3" style="padding-bottom: 15px; padding-top: 15px;">
                                        <asp:CheckBox ID="chbPersonMarginThresholds" AutoPostBack="true" OnCheckedChanged="chbPersonMarginThresholds_OnCheckedChanged"
                                            runat="server" Checked="false" onclick="setDirty();" />&nbsp;&nbsp; Use Color-coded
                                        Margin thresholds
                                    </td>
                                    <td align="right" style="padding-bottom: 15px; padding-top: 15px;">
                                        <asp:Button ID="btnPersonAddThreshold" Enabled="false" runat="server" Text="Add Threshold"
                                            OnClientClick="setDirty();" OnClick="btnPersonAddThreshold_OnClick" />
                                    </td>
                                </tr>
                            </table>
                            <asp:GridView ID="gvPersonThrsholds" Enabled="false" runat="server" Width="99%" OnRowDataBound="gvPersonThrsholds_RowDataBound"
                                AutoGenerateColumns="False" EmptyDataText="" DataKeyNames="Id" CssClass="CompPerfTable"
                                GridLines="None">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemStyle Height="25px" HorizontalAlign="Center" Width="25%" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Start</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:DropDownList ID="gvPersonddlStartRange" onchange="setDirty();" runat="server">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle Height="25px" HorizontalAlign="Center" Width="25%" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                End</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:DropDownList ID="gvPersonddlEndRange" onchange="setDirty();" runat="server">
                                            </asp:DropDownList>
                                            <asp:CustomValidator ID="cvgvPersonRange" runat="server" ToolTip="The End must be greater than or equals to Start."
                                                Text="*" EnableClientScript="false" OnServerValidate="cvgvPersonRange_OnServerValidate"
                                                SetFocusOnError="true" Display="Static" ValidationGroup="Client" />
                                            <asp:CustomValidator ID="cvgvPersonOverLapRange" runat="server" ToolTip="The specified Threshold Percentage range overlaps with another Threshold Percentage range."
                                                OnServerValidate="cvgvPersonOverLapRange_OnServerValidate" Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Static" ValidationGroup="Client" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle Height="25px" HorizontalAlign="Center" Width="42%" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Color</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <cc2:CustomDropDown ID="gvPersonddlColor" Width="85%" onclick="applyColor(this);"
                                                onchange="applyColor(this);setDirty();" runat="server">
                                            </cc2:CustomDropDown>
                                            <asp:CustomValidator ID="cvgvPersonddlColor" runat="server" OnServerValidate="cvgvPersonddlColor_ServerValidate"
                                                ToolTip="Please Select a Color." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                Display="Static" ValidationGroup="Client" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle Height="25px" HorizontalAlign="Center" Width="8%" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                            </div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnPersonDeleteRow" runat="server" ImageUrl="~/Images/cross_icon.png"
                                                ToolTip="Delete" OnClientClick="setDirty();" OnClick="btnPersonDeleteRow_OnClick" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                        <td style="width: 3%; padding-bottom: 10px;">
                        </td>
                        <td valign="top" style="width: 44%; padding-bottom: 10px;">
                        </td>
                        <td style="width: 3%; padding-bottom: 10px;">
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:CustomValidator ID="cvPersonThresholds" runat="server" OnServerValidate="cvPersonThresholds_ServerValidate"
            ErrorMessage="Thresholds must be added up to  100% or more and must be continuous."
            ToolTip="Thresholds must be added up to  100% or more and must be continuous."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvPersonColors" runat="server" OnServerValidate="cvPersonColors_ServerValidate"
            ErrorMessage="Color must not be selected more than once." ToolTip="Color must not be selected more than once."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
    </div>
    <div class="buttons-block" style="margin-bottom: 10px;" >
        <div>
            <asp:ValidationSummary ID="vsumClient" runat="server" ValidationGroup="Client" />
             <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
        </div>
        &nbsp;
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="pm-button"
            ValidationGroup="Client" />&nbsp;
    </div>
</asp:Content>

