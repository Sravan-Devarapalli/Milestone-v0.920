<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="RevenueGoals.aspx.cs" Inherits="PraticeManagement.Config.RevenueGoals" %>

<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Revenue Goals | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Revenue Goals
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblDirectorBudget").tablesorter(
            { headers: {
                1: {
                    sorter: false
                },
                2: {
                    sorter: false
                }, 3: {
                    sorter: false
                }, 4: {
                    sorter: false
                }, 5: {
                    sorter: false
                }, 6: {
                    sorter: false
                }, 7: {
                    sorter: false
                }, 8: {
                    sorter: false
                }, 9: {
                    sorter: false
                }, 10: {
                    sorter: false
                },
                11: {
                    sorter: false
                },
                12: {
                    sorter: false
                }
            },
                sortList: [[0, 0]],
                sortForce: [[0, 0]]

            });
            $("#tblPracticeBudget").tablesorter(
            { headers: {
                1: {
                    sorter: false
                },
                2: {
                    sorter: false
                }, 3: {
                    sorter: false
                }, 4: {
                    sorter: false
                }, 5: {
                    sorter: false
                }, 6: {
                    sorter: false
                }, 7: {
                    sorter: false
                }, 8: {
                    sorter: false
                }, 9: {
                    sorter: false
                }, 10: {
                    sorter: false
                },
                11: {
                    sorter: false
                },
                12: {
                    sorter: false
                }
            },
                sortList: [[0, 0]],
                sortForce: [[0, 0]]

            });
            $("#tblBDMBudget").tablesorter(
            { headers: {
                1: {
                    sorter: false
                },
                2: {
                    sorter: false
                }, 3: {
                    sorter: false
                }, 4: {
                    sorter: false
                }, 5: {
                    sorter: false
                }, 6: {
                    sorter: false
                }, 7: {
                    sorter: false
                }, 8: {
                    sorter: false
                }, 9: {
                    sorter: false
                }, 10: {
                    sorter: false
                },
                11: {
                    sorter: false
                },
                12: {
                    sorter: false
                }
            },
                sortList: [[0, 0]],
                sortForce: [[0, 0]]

            });
        });
        function selfChanged(txtBudget) {
            debugger;
            var hdnId = txtBudget.parentNode.children[1].id;
            var hdn = document.getElementById(hdnId);
            hdn.value = "true";
        }
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            $("#tblDirectorBudget").tablesorter(
            { headers: {
                1: {
                    sorter: false
                },
                2: {
                    sorter: false
                }, 3: {
                    sorter: false
                }, 4: {
                    sorter: false
                }, 5: {
                    sorter: false
                }, 6: {
                    sorter: false
                }, 7: {
                    sorter: false
                }, 8: {
                    sorter: false
                }, 9: {
                    sorter: false
                }, 10: {
                    sorter: false
                },
                11: {
                    sorter: false
                },
                12: {
                    sorter: false
                }
            },
                sortList: [[0, 0]],
                sortForce: [[0, 0]]

            });
            $("#tblPracticeBudget").tablesorter(
            { headers: {
                1: {
                    sorter: false
                },
                2: {
                    sorter: false
                }, 3: {
                    sorter: false
                }, 4: {
                    sorter: false
                }, 5: {
                    sorter: false
                }, 6: {
                    sorter: false
                }, 7: {
                    sorter: false
                }, 8: {
                    sorter: false
                }, 9: {
                    sorter: false
                }, 10: {
                    sorter: false
                },
                11: {
                    sorter: false
                },
                12: {
                    sorter: false
                }
            },
                sortList: [[0, 0]],
                sortForce: [[0, 0]]

            });
            $("#tblBDMBudget").tablesorter(
            { headers: {
                1: {
                    sorter: false
                },
                2: {
                    sorter: false
                }, 3: {
                    sorter: false
                }, 4: {
                    sorter: false
                }, 5: {
                    sorter: false
                }, 6: {
                    sorter: false
                }, 7: {
                    sorter: false
                }, 8: {
                    sorter: false
                }, 9: {
                    sorter: false
                }, 10: {
                    sorter: false
                },
                11: {
                    sorter: false
                },
                12: {
                    sorter: false
                }
            },
                sortList: [[0, 0]],
                sortForce: [[0, 0]]

            });
        }

    </script>
    <asp:UpdatePanel ID="pnlBudgetEntries" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" align="center" class="WholeWidth">
                <tr>
                    <td class="textRight">
                        <asp:ImageButton ID="imgbtnPrevious" runat="server" ImageUrl="~/Images/previous.gif"
                            OnClick="imgbtnNavigateYear_Click" OnClientClick="if (!confirmSaveDirty(true)) return false;" />
                    </td>
                    <td class="Width60Px textCenter">
                        <asp:Label ID="lblYear" runat="server" class="vTop"></asp:Label>
                    </td>
                    <td class="textLeft">
                        <asp:ImageButton ID="imgbtnNext" runat="server" ImageUrl="~/Images/next.gif" OnClick="imgbtnNavigateYear_Click"
                            OnClientClick="if (!confirmSaveDirty(true)) return false;" />
                    </td>
                </tr>
            </table>
            <div class="RevenueGoals_saveGoalsDiv">
                <asp:Button runat="server" ID="btnSaveGoals" Text="Save Goals" OnClick="SaveGoals_Clicked" />
            </div>
            <div class="xScrollOnly">
                <asp:Repeater ID="repDirectors" runat="server">
                    <HeaderTemplate>
                        <div class="minheight250Px">
                            <table id="tblDirectorBudget" class="CompPerfTable RevenueGoals_grdDirectorBudgetEntries">
                                <thead>
                                    <tr>
                                        <th class="FirstTd CursorPointer">
                                            <div class='ie-bg'>
                                                Executive in Charge
                                            </div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Jan</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Feb</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Mar</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Apr</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                May</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Jun</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Jul</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Aug</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Sep</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Oct</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Nov</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Dec</div>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("LastName") +", " +Eval("FirstName") %>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtJan" runat="server" MonthIndex="1" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),1) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),1) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),1) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtJan" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextJan" runat="server" ControlToValidate="txtJan" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtFeb" runat="server" MonthIndex="2" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),2) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),2) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),2) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtFeb" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextFeb" runat="server" ControlToValidate="txtFeb" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtMar" runat="server" MonthIndex="3" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),3) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),3) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),3) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtMar" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextMar" runat="server" ControlToValidate="txtMar" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtApr" runat="server" MonthIndex="4" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),4) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),4) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),4) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtApr" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextApr" runat="server" ControlToValidate="txtApr" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtMay" runat="server" MonthIndex="5" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),5) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),5) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),5) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtMay" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextMay" runat="server" ControlToValidate="txtMay" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtJun" runat="server" MonthIndex="6" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),6) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),6) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),6) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtJun" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextJun" runat="server" ControlToValidate="txtJun" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtJul" runat="server" MonthIndex="7" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),7) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),7) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),7) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtJul" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextJul" runat="server" ControlToValidate="txtJul" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtAug" runat="server" MonthIndex="8" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),8) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),8) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),8) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtAug" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextAug" runat="server" ControlToValidate="txtAug" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtSep" runat="server" MonthIndex="9" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),9) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),9) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),9) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtSep" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextSep" runat="server" ControlToValidate="txtSep" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtOct" runat="server" MonthIndex="10" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),10) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),10) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),10) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtOct" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextOct" runat="server" ControlToValidate="txtOct" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtNov" runat="server" MonthIndex="11" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),11) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),11) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),11) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtNov" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextNov" runat="server" ControlToValidate="txtNov" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtDec" runat="server" MonthIndex="12" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),12) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),12) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),12) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtDec" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextDec" runat="server" ControlToValidate="txtDec" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="DirectorAmountValidation"></asp:CustomValidator>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody></table></div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <asp:ValidationSummary ID="valSummaryDirector" runat="server" ValidationGroup="DirectorAmountValidation" />
            <hr size="2" class="color888888" align="center" />
            <div class="xScrollOnly">
                <asp:Repeater ID="repPracticeBudgetEntries" runat="server">
                    <HeaderTemplate>
                        <div class="minheight250Px">
                            <table id="tblPracticeBudget" class="CompPerfTable RevenueGoals_grdDirectorBudgetEntries">
                                <thead>
                                    <tr>
                                        <th class="FirstTd CursorPointer">
                                            <div class='ie-bg'>
                                                Practice Area
                                            </div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Jan</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Feb</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Mar</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Apr</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                May</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Jun</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Jul</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Aug</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Sep</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Oct</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Nov</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Dec</div>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("HtmlEncodedName")%></div>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtJan" runat="server" MonthIndex="1" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),1) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),1) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),1) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtJan" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextJan" runat="server" ControlToValidate="txtJan" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtFeb" runat="server" MonthIndex="2" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),2) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),2) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),2) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtFeb" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextFeb" runat="server" ControlToValidate="txtFeb" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtMar" runat="server" MonthIndex="3" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),3) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),3) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),3) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtMar" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextMar" runat="server" ControlToValidate="txtMar" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtApr" runat="server" MonthIndex="4" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),4) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),4) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),4) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtApr" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextApr" runat="server" ControlToValidate="txtApr" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtMay" runat="server" MonthIndex="5" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),5) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),5) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),5) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtMay" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextMay" runat="server" ControlToValidate="txtMay" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtJun" runat="server" MonthIndex="6" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),6) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),6) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),6) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtJun" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextJun" runat="server" ControlToValidate="txtJun" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtJul" runat="server" MonthIndex="7" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),7) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),7) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),7) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtJul" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextJul" runat="server" ControlToValidate="txtJul" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtAug" runat="server" MonthIndex="8" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),8) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),8) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),8) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtAug" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextAug" runat="server" ControlToValidate="txtAug" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtSep" runat="server" MonthIndex="9" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),9) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),9) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),9) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtSep" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextSep" runat="server" ControlToValidate="txtSep" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtOct" runat="server" MonthIndex="10" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),10) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),10) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),10) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtOct" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextOct" runat="server" ControlToValidate="txtOct" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtNov" runat="server" MonthIndex="11" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),11) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),11) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),11) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtNov" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextNov" runat="server" ControlToValidate="txtNov" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtDec" runat="server" MonthIndex="12" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PracticeId='<%# Eval("PracticeId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),12) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),12) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),12) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtDec" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextDec" runat="server" ControlToValidate="txtDec" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="PracticeAreaAmountValidation"></asp:CustomValidator>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody></table></div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <asp:ValidationSummary ID="valSummaryPracticeArea" runat="server" ValidationGroup="PracticeAreaAmountValidation" />
            <hr size="2" class="color888888" align="center" />
            <div class="xScrollOnly">
                <asp:Repeater ID="repBDMBudgetEntries" runat="server">
                    <HeaderTemplate>
                        <div class="minheight250Px">
                            <table id="tblBDMBudget" class="CompPerfTable RevenueGoals_grdDirectorBudgetEntries">
                                <thead>
                                    <tr>
                                        <th class="FirstTd CursorPointer">
                                            <div class='ie-bg'>
                                                Business Development Manager
                                            </div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Jan</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Feb</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Mar</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Apr</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                May</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Jun</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Jul</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Aug</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Sep</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Oct</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Nov</div>
                                        </th>
                                        <th>
                                            <div class='ie-bg'>
                                                Dec</div>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                               <%# Eval("LastName") +", " +Eval("FirstName") %></div>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtJan" runat="server" MonthIndex="1" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),1) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),1) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),1) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtJan" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextJan" runat="server" ControlToValidate="txtJan" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtFeb" runat="server" MonthIndex="2" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),2) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),2) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),2) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtFeb" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextFeb" runat="server" ControlToValidate="txtFeb" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtMar" runat="server" MonthIndex="3" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),3) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),3) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),3) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtMar" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextMar" runat="server" ControlToValidate="txtMar" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtApr" runat="server" MonthIndex="4" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),4) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),4) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),4) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtApr" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextApr" runat="server" ControlToValidate="txtApr" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtMay" runat="server" MonthIndex="5" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),5) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),5) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),5) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtMay" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextMay" runat="server" ControlToValidate="txtMay" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtJun" runat="server" MonthIndex="6" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),6) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),6) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),6) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtJun" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextJun" runat="server" ControlToValidate="txtJun" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtJul" runat="server" MonthIndex="7" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),7) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),7) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),7) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtJul" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextJul" runat="server" ControlToValidate="txtJul" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtAug" runat="server" MonthIndex="8" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),8) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),8) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),8) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtAug" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextAug" runat="server" ControlToValidate="txtAug" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtSep" runat="server" MonthIndex="9" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),9) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),9) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),9) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtSep" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextSep" runat="server" ControlToValidate="txtSep" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtOct" runat="server" MonthIndex="10" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),10) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),10) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),10) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtOct" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextOct" runat="server" ControlToValidate="txtOct" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtNov" runat="server" MonthIndex="11" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),11) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),11) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),11) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtNov" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextNov" runat="server" ControlToValidate="txtNov" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                            <td>
                                $<asp:TextBox ID="txtDec" runat="server" MonthIndex="12" AutoPostBack="false" onchange="selfChanged(this); setDirty();"
                                    PersonId='<%# Eval("PersonId") %>' Text='<%# GetMonthAmount(Eval("ProjectedFinancialsByMonth"),12) %>'
                                    Enabled='<%# CheckMonthEnabled(Eval("ProjectedFinancialsByMonth"),12) %>' CssClass='<%# GetBackColor(Eval("ProjectedFinancialsByMonth"),12) %>'></asp:TextBox>
                                <asp:HiddenField ID="hdnTxtDec" runat="server" Value="false" />
                                <asp:CustomValidator ID="custTextDec" runat="server" ControlToValidate="txtDec" OnServerValidate="Amount_OnServerValidate"
                                    EnableClientScript="false" SetFocusOnError="true" Display="Static" ValidationGroup="BDMAmountValidation"></asp:CustomValidator>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody></table></div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <asp:ValidationSummary ID="valSummaryBDM" runat="server" ValidationGroup="BDMAmountValidation" />
            <%--<asp:Label ID="lblMessage" runat="server" ForeColor="Green" Text="Saved all entries successfuly."
            Visible="false"></asp:Label>--%>
            <div class="RevenueGoals_saveGoalsDiv1">
                <asp:Button runat="server" ID="btnBottomSaveGoals" Text="Save Goals" OnClick="SaveGoals_Clicked" />
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSaveGoals" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="lpBudgetEntries" runat="server" />
</asp:Content>

