<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="RevenueGoals.aspx.cs" Inherits="PraticeManagement.Config.RevenueGoals" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
 <title>Practice Management - Revenue Goals</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
Revenue Goals
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
<script type="text/javascript">
    function selfChanged(txtBudget) {
        //debugger;
        var hdnId = txtBudget.parentNode.children[1].id;
        var hdn = document.getElementById(hdnId);
        hdn.value = "true";
    }
</script>
<asp:UpdatePanel ID="pnlBudgetEntries" runat="server">
    <ContentTemplate>
        <table cellpadding="0" cellspacing="0" align="center" style="width: 100%;">
            <tr>
                <td style="text-align: right;">
                    <asp:ImageButton ID="imgbtnPrevious" runat="server" ImageUrl="~/Images/previous.gif"
                        OnClick="imgbtnNavigateYear_Click" OnClientClick="if (!confirmSaveDirty(true)) return false;" />
                </td>
                <td style="width: 60px; text-align: center">
                    <asp:Label ID="lblYear" runat="server" Style="vertical-align: top"></asp:Label>
                </td>
                <td style="text-align: left;">
                    <asp:ImageButton ID="imgbtnNext" runat="server" ImageUrl="~/Images/next.gif" OnClick="imgbtnNavigateYear_Click" OnClientClick="if (!confirmSaveDirty(true)) return false;" />
                </td>
            </tr>
        </table>
        <div style="width: 100%; padding: 0px 2px 6px 0px; text-align: right;">
            <asp:Button runat="server" ID="btnSaveGoals" Text="Save Goals" OnClick="SaveGoals_Clicked" />
        </div>
        <div style="overflow-x: scroll;">
            <asp:GridView ID="grdDirectorBudgetEntries" runat="server" AutoGenerateColumns="false"
                GridLines="None" AllowSorting="true" OnSorting="BudgetEntries_Sorting" OnSorted="BudgetEntries_Sorted"
                CssClass="CompPerfTable" EmptyDataText="No Client Directors found for the year selected.">
                <Columns>
                    <asp:TemplateField ItemStyle-Width="200px" HeaderStyle-Width="200px" HeaderStyle-Wrap="false"
                        HeaderStyle-CssClass="ie-bg" ItemStyle-Height="30px">
                        <HeaderTemplate>
                            <div class="HeaderSortStyle ie-bg" style="white-space: nowrap">
                                <asp:LinkButton ID="lbClientDirector" runat="server" Text="Client Director" CommandName="Sort"  OnClientClick="if (!confirmSaveDirty(true)) return false;"
                                    CommandArgument="LastNameFirstName"></asp:LinkButton>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div style="padding-right: 2px; width: 200px;">
                                <%# Eval("LastName") +", " +Eval("FirstName") %></div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField SortExpression="ProjectedFinancialsByMonth">
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Jan</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtJan" runat="server" Width="60px" MonthIndex="1" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),1) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),1) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),1) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtJan" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextJan" runat="server" ControlToValidate="txtJan" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Feb</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtFeb" runat="server" Width="60px" MonthIndex="2" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),2) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),2) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),2) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtFeb" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextFeb" runat="server" ControlToValidate="txtFeb" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Mar</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtMar" runat="server" Width="60px" MonthIndex="3" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),3) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),3) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),3) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtMar" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextMar" runat="server" ControlToValidate="txtMar" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Apr</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtApr" runat="server" Width="60px" MonthIndex="4" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),4) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),4) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),4) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtApr" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextApr" runat="server" ControlToValidate="txtApr" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                May</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtMay" runat="server" Width="60px" MonthIndex="5" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),5) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),5) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),5) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtMay" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextMay" runat="server" ControlToValidate="txtMay" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                            Jun
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtJun" runat="server" Width="60px" MonthIndex="6" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),6) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),6) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),6) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtJun" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextJun" runat="server" ControlToValidate="txtJun" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Jul</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtJul" runat="server" Width="60px" MonthIndex="7" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),7) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),7) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),7) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtJul" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextJul" runat="server" ControlToValidate="txtJul" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Aug</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtAug" runat="server" Width="60px" MonthIndex="8" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),8) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),8) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),8) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtAug" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextAug" runat="server" ControlToValidate="txtAug" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Sep</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtSep" runat="server" Width="60px" MonthIndex="9" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),9) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),9) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),9) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtSep" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextSep" runat="server" ControlToValidate="txtSep" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Oct</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtOct" runat="server" Width="60px" MonthIndex="10" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),10) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),10) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),10) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtOct" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextOct" runat="server" ControlToValidate="txtOct" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Nov</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtNov" runat="server" Width="60px" MonthIndex="11" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),11) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),11) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),11) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtNov" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextNov" runat="server" ControlToValidate="txtNov" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Dec</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtDec" runat="server" Width="60px" MonthIndex="12" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),12) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),12) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),12) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtDec" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextDec" runat="server" ControlToValidate="txtDec" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <asp:ValidationSummary ID="valSummaryDirector" runat="server" ValidationGroup="DirectorAmountValidation" />
        <hr size="2" color="#888888" align="center" />
        <div style="overflow-x: scroll;">
            <asp:GridView ID="grdPraticeAreaBugetEntries" runat="server" AutoGenerateColumns="false"
                GridLines="None" AllowSorting="true" OnSorting="BudgetEntries_Sorting" OnSorted="BudgetEntries_Sorted"
                CssClass="CompPerfTable" EmptyDataText="No Business Practice Areas found for the year selected.">
                <Columns>
                    <asp:TemplateField ItemStyle-Width="200px" HeaderStyle-Wrap="false" HeaderStyle-Width="200px"
                        HeaderStyle-CssClass="ie-bg" ItemStyle-Height="30px">
                        <HeaderTemplate>
                            <div class="HeaderSortStyle ie-bg" style="white-space: nowrap">
                                <asp:LinkButton ID="lbPracticeArea" runat="server" Text="Practice Area" CommandName="Sort"
                                    CommandArgument="Name" OnClientClick="if (!confirmSaveDirty(true)) return false;"></asp:LinkButton>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div style="padding-right: 2px; width: 200px;">
                                <%# Eval("Name")%></div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Jan</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtJan" runat="server" Width="60px" MonthIndex="1" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),1) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),1) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),1) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtJan" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextJan" runat="server" ControlToValidate="txtJan" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Feb</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtFeb" runat="server" Width="60px" MonthIndex="2" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),2) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),2) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),2) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtFeb" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextFeb" runat="server" ControlToValidate="txtFeb" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Mar</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtMar" runat="server" Width="60px" MonthIndex="3" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),3) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),3) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),3) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtMar" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextMar" runat="server" ControlToValidate="txtMar" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Apr</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtApr" runat="server" Width="60px" MonthIndex="4" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),4) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),4) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),4) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtApr" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextApr" runat="server" ControlToValidate="txtApr" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                May</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtMay" runat="server" Width="60px" MonthIndex="5" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),5) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),5) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),5) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtMay" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextMay" runat="server" ControlToValidate="txtMay" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                            Jun
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtJun" runat="server" Width="60px" MonthIndex="6" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),6) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),6) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),6) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtJun" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextJun" runat="server" ControlToValidate="txtJun" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Jul</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtJul" runat="server" Width="60px" MonthIndex="7" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),7) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),7) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),7) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtJul" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextJul" runat="server" ControlToValidate="txtJul" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Aug</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtAug" runat="server" Width="60px" MonthIndex="8" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),8) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),8) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),8) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtAug" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextAug" runat="server" ControlToValidate="txtAug" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Sep</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtSep" runat="server" Width="60px" MonthIndex="9" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),9) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),9) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),9) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtSep" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextSep" runat="server" ControlToValidate="txtSep" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Oct</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtOct" runat="server" Width="60px" MonthIndex="10" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),10) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),10) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),10) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtOct" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextOct" runat="server" ControlToValidate="txtOct" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Nov</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtNov" runat="server" Width="60px" MonthIndex="11" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),11) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),11) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),11) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtNov" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextNov" runat="server" ControlToValidate="txtNov" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Dec</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtDec" runat="server" Width="60px" MonthIndex="12" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PracticeId='<%# Eval("PracticeId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),12) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),12) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),12) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtDec" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextDec" runat="server" ControlToValidate="txtDec" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <asp:ValidationSummary ID="valSummaryPracticeArea" runat="server" ValidationGroup="PracticeAreaAmountValidation" />
        <hr size="2" color="#888888" align="center" />
        <div style="overflow-x: scroll;">
            <asp:GridView ID="grdBDMBudgetEntries" runat="server" AutoGenerateColumns="false"
                GridLines="None" AllowSorting="true" OnSorting="BudgetEntries_Sorting" OnSorted="BudgetEntries_Sorted"
                CssClass="CompPerfTable" EmptyDataText="No Business Development Managers found for the year selected.">
                <Columns>
                    <asp:TemplateField ItemStyle-Width="200px" HeaderStyle-Wrap="false" HeaderStyle-Width="200px"
                        HeaderStyle-CssClass="ie-bg" HeaderText="Business Development Manager" SortExpression="LastNameFirstName"
                        ItemStyle-Height="30px">
                        <HeaderTemplate>
                            <div class="HeaderSortStyle ie-bg" style="white-space: nowrap;">
                                <asp:LinkButton ID="lbBDM" runat="server" Text="Business Development Manager" CommandName="Sort"
                                    CommandArgument="LastNameFirstName" OnClientClick="if (!confirmSaveDirty(true)) return false;"></asp:LinkButton>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div style="padding-right: 2px; width: 200px;">
                                <%# Eval("LastName") +", " +Eval("FirstName") %></div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Jan</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtJan" runat="server" Width="60px" MonthIndex="1" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),1) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),1) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),1) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtJan" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextJan" runat="server" ControlToValidate="txtJan" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Feb</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtFeb" runat="server" Width="60px" MonthIndex="2" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),2) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),2) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),2) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtFeb" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextFeb" runat="server" ControlToValidate="txtFeb" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Mar</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtMar" runat="server" Width="60px" MonthIndex="3" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),3) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),3) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),3) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtMar" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextMar" runat="server" ControlToValidate="txtMar" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Apr</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtApr" runat="server" Width="60px" MonthIndex="4" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),4) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),4) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),4) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtApr" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextApr" runat="server" ControlToValidate="txtApr" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                May</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtMay" runat="server" Width="60px" MonthIndex="5" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),5) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),5) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),5) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtMay" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextMay" runat="server" ControlToValidate="txtMay" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                            Jun
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtJun" runat="server" Width="60px" MonthIndex="6" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),6) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),6) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),6) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtJun" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextJun" runat="server" ControlToValidate="txtJun" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Jul</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtJul" runat="server" Width="60px" MonthIndex="7" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),7) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),7) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),7) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtJul" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextJul" runat="server" ControlToValidate="txtJul" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Aug</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtAug" runat="server" Width="60px" MonthIndex="8" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),8) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),8) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),8) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtAug" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextAug" runat="server" ControlToValidate="txtAug" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Sep</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtSep" runat="server" Width="60px" MonthIndex="9" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),9) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),9) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),9) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtSep" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextSep" runat="server" ControlToValidate="txtSep" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Oct</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtOct" runat="server" Width="60px" MonthIndex="10" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),10) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),10) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),10) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtOct" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextOct" runat="server" ControlToValidate="txtOct" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Nov</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtNov" runat="server" Width="60px" MonthIndex="11" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),11) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),11) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),11) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtNov" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextNov" runat="server" ControlToValidate="txtNov" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="width: 78px;">
                                Dec</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            $<asp:TextBox ID="txtDec" runat="server" Width="60px" MonthIndex="12" AutoPostBack="false"
                                onchange="selfChanged(this); setDirty();" PersonId='<%# Eval("PersonId") %>'
                                Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),12) %>' Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),12) %>'
                                BackColor='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),12) %>'></asp:TextBox>
                            <asp:HiddenField ID="hdnTxtDec" runat="server" Value="false" />
                            <asp:CustomValidator ID="custTextDec" runat="server" ControlToValidate="txtDec" OnServerValidate="Amount_OnServerValidate"
                                EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <asp:ValidationSummary ID="valSummaryBDM" runat="server" ValidationGroup="BDMAmountValidation" />
        <%--<asp:Label ID="lblMessage" runat="server" ForeColor="Green" Text="Saved all entries successfuly."
            Visible="false"></asp:Label>--%>
        <div style="width: 100%; padding: 6px 2px 2px 0px; text-align: right;">
            <asp:Button runat="server" ID="btnBottomSaveGoals" Text="Save Goals" OnClick="SaveGoals_Clicked" />
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSaveGoals" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
<uc:LoadingProgress ID="lpBudgetEntries" runat="server" />
</asp:Content>


