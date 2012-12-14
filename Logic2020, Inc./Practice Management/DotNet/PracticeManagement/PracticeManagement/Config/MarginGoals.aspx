<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="MarginGoals.aspx.cs" Inherits="PraticeManagement.Config.MarginGoals" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Contribution Margin Goals | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Contribution Margin Goals
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
    <div class="MarginGoalsDiv">
        <div class="PaddingTop10 PaddingBottom10">
            <b>Account Goal Default</b></div>
        <asp:UpdatePanel ID="upnlClientGoalDefault" runat="server">
            <ContentTemplate>
                <table class="MargingoalsTable">
                    <tr>
                        <td class="FirstTd">
                            <table class="MarginGoalsHeaderTable">
                                <tr>
                                    <td colspan="3">
                                        <asp:CheckBox ID="chbClientGoalDefaultThreshold" AutoPostBack="true" OnCheckedChanged="chbClientGoalDefaultThreshold_OnCheckedChanged"
                                            runat="server" Checked="false" onclick="setDirty();" />&nbsp;&nbsp; Use Color-coded
                                        Contribution Margin thresholds
                                    </td>
                                    <td align="right">
                                        <asp:Button ID="btnClientGoalDefaultAddThreshold" Enabled="false" runat="server"
                                            Text="Add Threshold" OnClientClick="setDirty();" OnClick="btnClientGoalDefaultAddThreshold_OnClick" />
                                    </td>
                                </tr>
                            </table>
                            <asp:GridView ID="gvClientGoalDefaultThreshold" Enabled="false" runat="server" OnRowDataBound="gvClientGoalDefaultThreshold_RowDataBound"
                                AutoGenerateColumns="False" EmptyDataText="" DataKeyNames="Id" CssClass="CompPerfTable gvClientGoalDefaultThreshold">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderStyle CssClass="Width25Percent" />
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
                                        <HeaderStyle CssClass="Width25Percent" />
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
                                            <asp:CustomValidator ID="cvgvClientOverLapRange" runat="server" ErrorMessage="The specified Account goal threshold Percentage range overlaps with another Account goal threshold Percentage range."
                                                ToolTip="The specified Account goal threshold Percentage range overlaps with another Account goal threshold Percentage range."
                                                OnServerValidate="cvgvClientOverLapRange_OnServerValidate" Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Static" ValidationGroup="Client" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderStyle CssClass="Width42Percent" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Color</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <pmc:CustomDropDown ID="gvClientddlColor" CssClass="Width85Percent" onclick="applyColor(this);"
                                                onchange="applyColor(this);setDirty();" runat="server">
                                            </pmc:CustomDropDown>
                                            <asp:CustomValidator ID="cvgvClientddlColor" runat="server" OnServerValidate="cvgvClientddlColor_ServerValidate"
                                                ToolTip="Please Select a Color." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                Display="Static" ValidationGroup="Client" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderStyle CssClass="Width8Percent" />
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
                        <td class="SecondTd">
                        </td>
                        <td class="ThirdTd">
                            <div>
                                <p>
                                    Enabling this feature and configuring color-coded ranges will allow persons without
                                    unrestricted access to Project and Milestone contribution margin calculations a visual indication
                                    of how Projects and Milestones are tracking with regard to the contribution margin goals defined
                                    by the company.<br />
                                    <br />
                                </p>
                                <p>
                                    Contribution Margin goals must add up to at least 100%.<br />
                                    <br />
                                </p>
                                <p>
                                    NOTE: It is also possible to specify individual Account contribution margin goals from each Account's
                                    profile page, either in lieu of these default contribution margin goals, or by overriding them.<br />
                                    <br />
                                </p>
                            </div>
                        </td>
                        <td class="SecondTd">
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:CustomValidator ID="cvClientThresholds" runat="server" OnServerValidate="cvClientThresholds_ServerValidate"
            ErrorMessage="Account goal thresholds must be added up to  100% or more and must be continuous."
            ToolTip="Account goal thresholds must be added up to  100% or more and must be continuous."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvClientColors" runat="server" OnServerValidate="cvClientColors_ServerValidate"
            ErrorMessage="Color must not be selected more than once." ToolTip="Color must not be selected more than once."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvgvddlColorClone" runat="server" ErrorMessage="Please Select a Color."
            Text="*" EnableClientScript="false" SetFocusOnError="false" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvgvRangeClone" runat="server" ErrorMessage="The End must be greater than or equals to Start."
            Text="*" EnableClientScript="false" SetFocusOnError="false" Display="None" ValidationGroup="Client" />
    </div>
    <div class="MarginGoalsDiv">
        <div class="PaddingTop10 PaddingBottom10">
            <b>Person Goal</b></div>
        <asp:UpdatePanel ID="upnlPersonThrsholds" runat="server">
            <ContentTemplate>
                <table class="MargingoalsTable">
                    <tr>
                        <td class="FirstTd">
                            <table class="MarginGoalsHeaderTable">
                                <tr>
                                    <td colspan="3">
                                        <asp:CheckBox ID="chbPersonMarginThresholds" AutoPostBack="true" OnCheckedChanged="chbPersonMarginThresholds_OnCheckedChanged"
                                            runat="server" Checked="false" onclick="setDirty();" />&nbsp;&nbsp; Use Color-coded
                                        Contribution Margin thresholds
                                    </td>
                                    <td align="right">
                                        <asp:Button ID="btnPersonAddThreshold" Enabled="false" runat="server" Text="Add Threshold"
                                            OnClientClick="setDirty();" OnClick="btnPersonAddThreshold_OnClick" />
                                    </td>
                                </tr>
                            </table>
                            <asp:GridView ID="gvPersonThrsholds" Enabled="false" runat="server" OnRowDataBound="gvPersonThrsholds_RowDataBound"
                                AutoGenerateColumns="False" EmptyDataText="" DataKeyNames="Id" CssClass="CompPerfTable gvClientGoalDefaultThreshold"
                                GridLines="None">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderStyle CssClass="Width25Percent" />
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
                                        <HeaderStyle CssClass="Width25Percent" />
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
                                            <asp:CustomValidator ID="cvgvPersonOverLapRange" runat="server" ErrorMessage="The specified Person goal threshold Percentage range overlaps with another Person goal threshold Percentage range."
                                                ToolTip="The specified Person goal threshold Percentage range overlaps with another Person goal threshold Percentage range."
                                                OnServerValidate="cvgvPersonOverLapRange_OnServerValidate" Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Static" ValidationGroup="Client" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderStyle CssClass="Width42Percent" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Color</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <pmc:CustomDropDown ID="gvPersonddlColor" CssClass="Width85Percent" onclick="applyColor(this);"
                                                onchange="applyColor(this);setDirty();" runat="server">
                                            </pmc:CustomDropDown>
                                            <asp:CustomValidator ID="cvgvPersonddlColor" runat="server" OnServerValidate="cvgvPersonddlColor_ServerValidate"
                                                ToolTip="Please Select a Color." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                Display="Static" ValidationGroup="Client" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderStyle CssClass="Width8Percent" />
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
                        <td class="SecondTd">
                        </td>
                        <td class="ThirdTd">
                            <div>
                                <p>
                                    Enabling this feature and configuring color-coded ranges will allow persons without
                                    unrestricted access to the Margin Test page and its calculations a visual indication
                                    of how acceptable a calculated contribution margin is, based on the selected Bill Rate and Hours
                                    per Week.
                                    <br />
                                    <br />
                                </p>
                                <p>
                                    Contribution Margin goals must add up to at least 100%.<br />
                                    <br />
                                </p>
                            </div>
                        </td>
                        <td class="SecondTd">
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:CustomValidator ID="cvPersonThresholds" runat="server" OnServerValidate="cvPersonThresholds_ServerValidate"
            ErrorMessage="Person goal thresholds must be added up to  100% or more and must be continuous."
            ToolTip="Person goal thresholds must be added up to  100% or more and must be continuous."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
        <asp:CustomValidator ID="cvPersonColors" runat="server" OnServerValidate="cvPersonColors_ServerValidate"
            ErrorMessage="Color must not be selected more than once." ToolTip="Color must not be selected more than once."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="None" ValidationGroup="Client" />
    </div>
    <div class="buttons-block Margin-Bottom10Px">
        <div>
            <asp:ValidationSummary ID="vsumClient" runat="server" ValidationGroup="Client" />
            <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
        </div>
        &nbsp;
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" ValidationGroup="Client" />&nbsp;
    </div>
</asp:Content>

