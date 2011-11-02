<%@ Page Title="Strawman Details | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="StrawManDetails.aspx.cs" Inherits="PraticeManagement.StrawManDetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="Controls/PersonnelCompensation.ascx" TagName="PersonnelCompensation"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Strawman Details | Practice Management</title>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:LoadingProgress ID="lpStrawMan" runat="server" />
    <asp:UpdatePanel ID="upStrawMan" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr>
                    <td>
                        First Name
                        <asp:TextBox ID="tbFirstName" runat="server" onchange="setDirty();"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rqfvFirstName" runat="server" Text="*" ErrorMessage="First Name is required."
                            ControlToValidate="tbFirstName" ToolTip="First Name is required." SetFocusOnError="true"
                            ValidationGroup="StrawmanGroup"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvLengthFirstName" runat="server" Text="*" ErrorMessage="Person First Name characters length must be lessthan or equal to 50."
                            ToolTip="Person First Name character length must be lessthan or equal to 50." ValidationGroup="StrawmanGroup"
                            SetFocusOnError="true" OnServerValidate="cvNameLength_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        Last Name
                        <asp:TextBox ID="tbLastName" runat="server" onchange="setDirty();"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rqfvLastName" runat="server" Text="*" ErrorMessage="Last Name is required."
                            ControlToValidate="tbLastName" ToolTip="Last Name is required." SetFocusOnError="true"
                            ValidationGroup="StrawmanGroup"></asp:RequiredFieldValidator>
                        <%--<asp:CustomValidator ID="cvDupliacteName" runat="server" Text="*" ErrorMessage="There is another Person with the same First Name and Last Name."
                            ToolTip="There is another Person with the same First Name and Last Name." ValidationGroup="StrawmanGroup"
                            SetFocusOnError="true" OnServerValidate="cvDupliacteName_ServerValidate"></asp:CustomValidator>--%>
                        <asp:CustomValidator ID="cvLengthLastName" runat="server" Text="*" ErrorMessage="Person Last Name characters length must be lessthan or equal to 50."
                            ToolTip="Person Last Name character length must be lessthan or equal to 50." ValidationGroup="StrawmanGroup"
                            SetFocusOnError="true" OnServerValidate="cvNameLength_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 10px; font-weight: bold;">
                        Current Compensation :
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnlCompensation" runat="server" CssClass="bg-light-frame">
                            <div class="filters" style="margin-top: 5px; margin-bottom: 10px;">
                                <uc1:PersonnelCompensation ID="personnelCompensation" runat="server" IsStrawmanMode="true" />
                            </div>
                            <div style="padding-top: 10px; font-weight: bold; height:50px">
                                Compensation History :<br />
                                 <span style="color:Gray;font-family:Arial;">Click on Start Date column to edit an item<br /></span>
                            </div>
                            <asp:GridView ID="gvCompensationHistory" runat="server" AutoGenerateColumns="False"
                                EmptyDataText="No compensation history for this person." CssClass="CompPerfTable WholeWidth"
                                GridLines="None" BackColor="White">
                                <AlternatingRowStyle BackColor="#F9FAFF" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Start">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnStartDate" runat="server" Text='<%# ((DateTime?)Eval("StartDate")).HasValue ? ((DateTime?)Eval("StartDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'
                                                CommandArgument='<%# Eval("StartDate") %>' OnCommand="btnStartDate_Command" OnClientClick="if (!confirmSaveDirty()) return false;"></asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Start</div>
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="End">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime?)Eval("EndDate")).HasValue ? ((DateTime?)Eval("EndDate")).Value.AddDays(-1).ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label></ItemTemplate>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                End</div>
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Basis">
                                        <ItemTemplate>
                                            <asp:Label ID="lblBasis" runat="server" Text='<%# Eval("TimescaleName") %>'></asp:Label></ItemTemplate>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Basis</div>
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount">
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Amount</div>
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Amount") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("Amount") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Vacation">
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Vacation</div>
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("VacationDays") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("VacationDays") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                &nbsp;
                                            </div>
                                        </HeaderTemplate>
                                        <ItemStyle Width="3%" HorizontalAlign="Center" Height="20px" Wrap="false" />
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imgCompensationDelete" ToolTip="Delete" runat="server" OnClick="imgCompensationDelete_OnClick"
                                                ImageUrl="~/Images/cross_icon.png" />
                                            <ajaxToolkit:ConfirmButtonExtender ConfirmText="Are you sure you want to delete this compensation record?"
                                                runat="server" TargetControlID="imgCompensationDelete">
                                            </ajaxToolkit:ConfirmButtonExtender>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 15px;">
                        <asp:ValidationSummary ID="valSummary" runat="server" ValidationGroup="StrawmanGroup" />
                        <uc:MessageLabel ID="lblSave" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                            WarningColor="Orange" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center;">
                        <asp:Button ID="btnSave" runat="server" Text="Save" ToolTip="Save" OnClick="btnSave_Click" ValidationGroup="StrawmanGroup"/>
                        <asp:CancelAndReturnButton ID="btnCancelAndRetrun" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

