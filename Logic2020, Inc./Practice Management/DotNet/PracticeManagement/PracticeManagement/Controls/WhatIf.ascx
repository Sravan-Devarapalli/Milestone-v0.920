<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WhatIf.ascx.cs" Inherits="PraticeManagement.Controls.WhatIf" %>
<%@ Register Src="GrossMarginComputing.ascx" TagName="GrossMarginComputing" TagPrefix="uc1" %>
<%@ Register TagPrefix="uc" TagName="MessageLabel" Src="~/Controls/MessageLabel.ascx" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<script type="text/javascript" language="javascript">
    function ChangeHourlyBillRate(IsIncrement) {
        var txtBillRate = $get("<%= txtBillRateSlider_BoundControl.ClientID %>");

        if (txtBillRate != null) {
            BillRate = parseFloat(txtBillRate.value);
            if (IsIncrement && BillRate < 350) {

                txtBillRate.value = BillRate + 1;
            }
            else if (!IsIncrement && BillRate > 20) {
                txtBillRate.value = BillRate - 1;
            }
            if (txtBillRate.fireEvent) {
                txtBillRate.fireEvent('onchange');
            }
            if (document.createEvent) {
                var event = document.createEvent('HTMLEvents');
                event.initEvent('change', true, true);
                txtBillRate.dispatchEvent(event);
            }
        }
    }
    function ChangeHoursPerWeek(IsIncrement) {
        var txtHPW = $get("<%= txtHorsPerWeekSlider_BoundControl.ClientID %>");

        if (txtHPW != null) {
            BillRate = parseInt(txtHPW.value);
            if (IsIncrement && BillRate < 60) {

                if (BillRate < 55)
                    txtHPW.value = BillRate + 5;
                else
                    txtHPW.value = 60;
            }
            else if (!IsIncrement) {
                if (BillRate > 15)
                    txtHPW.value = BillRate - 5;
                else
                    txtHPW.value = 10;
            }
            if (txtHPW.fireEvent) {
                txtHPW.fireEvent('onchange');
            }
            if (document.createEvent) {
                var event = document.createEvent('HTMLEvents');
                event.initEvent('change', true, true);
                txtHPW.dispatchEvent(event);
            }
        }
    }
</script>
<asp:Panel ID="pnlWhatIf" runat="server">
    <table class="WholeWidth">
        <tr>
            <td class="vTop">
                <table>
                    <tr>
                        <th class="bgColorTransparent no-wrap" colspan="6">
                            Revenue &amp; Margin Report
                        </th>
                    </tr>
                    <tr>
                        <td class="textCenter" colspan="6">
                            Enter proposed hourly rate and hours per week to calculate margin
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table id="tblEffectiveDate" runat="server">
                                <tr>
                                    <td>
                                        Effective&nbsp;Date
                                    </td>
                                    <td class="padLeft15">
                                        <uc2:DatePicker ID="dtpEffectiveDate" runat="server" OnSelectionChanged="dtpEffectiveDate_SelectionChanged"
                                            AutoPostBack="true" />
                                    </td>
                                    <td>
                                        <asp:CompareValidator ID="compEffectiveDate" runat="server" ControlToValidate="dtpEffectiveDate"
                                            EnableClientScript="False" ErrorMessage="The Effective Date must be in the format 'MM/dd/yyyy'"
                                            Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="ComputeRate"
                                            ToolTip="The Effective Date must be in the format 'MM/dd/yyyy'" Type="Date">*</asp:CompareValidator>
                                        <asp:RequiredFieldValidator ID="reqEffectiveDate" runat="server" ControlToValidate="dtpEffectiveDate"
                                            Display="Dynamic" EnableClientScript="False" ErrorMessage="The Effective Date is required."
                                            SetFocusOnError="True" ValidationGroup="ComputeRate" ToolTip="The Effective Date is required.">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="cvWithTerminationDate" runat="server" ControlToValidate="dtpEffectiveDate"
                                            ErrorMessage="Effective Date should be less than or equal to the Termination Date."
                                            ToolTip="Effective Date should be less than or equal to the Termination Date."
                                            ValidationGroup="ComputeRate" Text="*" Display="Dynamic" OnServerValidate="cvWithTerminationDate_ServerValidate"
                                            ValidateEmptyText="false" SetFocusOnError="true" EnableClientScript="false"></asp:CustomValidator>
                                        <asp:CustomValidator ID="cvNotHavingCompensation" runat="server" ControlToValidate="dtpEffectiveDate"
                                            ErrorMessage="Person doesn't have compensation for the selected Effective Date."
                                            ToolTip="Person doesn't have compensation for the selected Effective Date." ValidationGroup="ComputeRate"
                                            Text="*" Display="Dynamic" OnServerValidate="cvNotHavingCompensation_ServerValidate"
                                            ValidateEmptyText="false" SetFocusOnError="true" EnableClientScript="false"></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="vTop PaddingTop3">
                            Hourly Bill Rate (HBR)
                        </td>
                        <td colspan="5" class="padLeft5">
                            <table>
                                <tr>
                                    <td class="padRight2">
                                        <img id="imgDecrBillRate" runat="server" src="~/Images/MinusIcon.png" onclick="ChangeHourlyBillRate(false);"
                                            class="WhatIfControlIcon" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtBillRateSlider" runat="server" CssClass="SliderBehavior" AutoPostBack="true"
                                            Text="120"></asp:TextBox>
                                        <AjaxControlToolkit:SliderExtender ID="sldBillRate" runat="server" BehaviorID="txtBillRateSlider"
                                            TargetControlID="txtBillRateSlider" BoundControlID="txtBillRateSlider_BoundControl"
                                            Orientation="Horizontal" EnableHandleAnimation="true" Minimum="20" Maximum="350"
                                            Length="350" Decimals="2">
                                        </AjaxControlToolkit:SliderExtender>
                                    </td>
                                    <td class="padLeft2">
                                        <img id="imgIncrBillRate" runat="server" src="~/Images/PlusIcon.png" onclick="ChangeHourlyBillRate(true);"
                                            class="WhatIfControlIcon" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <table class="WholeWidth">
                                            <tr>
                                                <td class="Width60Px textleft">
                                                    $<asp:Label ID="lblBillRateMin" runat="server"></asp:Label>
                                                </td>
                                                <td class="textCenter PaddingTop3">
                                                    <asp:TextBox ID="txtBillRateSlider_BoundControl" runat="server" CssClass="Width60Px textCenter"
                                                        AutoPostBack="true" OnTextChanged="txtBillRateSlider_TextChanged" Text="120"></asp:TextBox>
                                                    <asp:CompareValidator ID="compBillRateSlider_BoundControl" runat="server" ControlToValidate="txtBillRateSlider_BoundControl"
                                                        ErrorMessage="A number with 2 decimal digits is allowed for the Bill Rate." ToolTip="A number with 2 decimal digits is allowed for the Bill Rate."
                                                        Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ComputeRate"
                                                        Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                                </td>
                                                <td class="Width60Px textRight">
                                                    $<asp:Label ID="lblBillRateMax" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="vTop PaddingTop3">
                            Hours per Week (HPW)
                        </td>
                        <td colspan="5" class="padLeft5">
                            <table>
                                <tr>
                                    <td class="padRight2">
                                        <img id="imgDecrHPW" runat="server" src="~/Images/MinusIcon.png" onclick="ChangeHoursPerWeek(false);"
                                            class="WhatIfControlIcon" />
                                    </td>
                                    <td colspan="3">
                                        <asp:TextBox ID="txtHorsPerWeekSlider" runat="server" CssClass="SliderBehavior" AutoPostBack="true"
                                            Text="40"></asp:TextBox>
                                        <AjaxControlToolkit:SliderExtender ID="sldHoursPerMonth" runat="server" BehaviorID="txtHorsPerWeekSlider"
                                            TargetControlID="txtHorsPerWeekSlider" BoundControlID="txtHorsPerWeekSlider_BoundControl"
                                            Orientation="Horizontal" Minimum="10" Maximum="60" Length="350" EnableHandleAnimation="True">
                                        </AjaxControlToolkit:SliderExtender>
                                    </td>
                                    <td class="padLeft2">
                                        <img id="imgIncrHPW" runat="server" src="~/Images/PlusIcon.png" onclick="ChangeHoursPerWeek(true);"
                                            class="WhatIfControlIcon" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td class="Width25Px textleft">
                                        <asp:Label ID="lblHoursMin" runat="server"></asp:Label>
                                    </td>
                                    <td class="textCenter PaddingTop3">
                                        <asp:TextBox ID="txtHorsPerWeekSlider_BoundControl" runat="server" CssClass="Width60Px textCenter"
                                            AutoPostBack="true" Text="40" OnTextChanged="txtBillRateSlider_TextChanged"></asp:TextBox>
                                        <asp:CompareValidator ID="compHorsPerWeekSlider_BoundControl" runat="server" ControlToValidate="txtHorsPerWeekSlider_BoundControl"
                                            ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Week."
                                            ToolTip="A number with 2 decimal digits is allowed for the Hours Per Week." Text="*"
                                            EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ComputeRate"
                                            Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                    </td>
                                    <td class="Width25Px textRight">
                                        <asp:Label ID="lblHoursMax" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" class="PaddingBottom3">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="PaddingBottom3">
                            Monthly Revenue
                        </td>
                        <td colspan="4" class="WhatIfControlStyle">
                            <asp:Label ID="lblMonthlyRevenueWithoutRecruiting" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="PaddingBottom3">
                            Monthly COGS
                        </td>
                        <td colspan="4" class="WhatIfControlStyle">
                            <asp:Label ID="lblMonthlyCogsWithoutRecruiting" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="PaddingBottom3">
                            Monthly Contribution Margin
                        </td>
                        <td colspan="4" class="WhatIfControlStyle">
                            <asp:Label ID="lblMonthlyGrossMarginWithoutRecruiting" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trTargetMargin" runat="server" visible="false">
                        <td colspan="2">
                            Target Margin
                        </td>
                        <td colspan="4" class="padRight242" align="right">
                            <table>
                                <tr>
                                    <td id="tdTargetMarginWithoutRecruiting" runat="server">
                                        <asp:Label ID="lblTargetMarginWithoutRecruiting" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <tr id="tr1" runat="server" visible="true">
                        <td colspan="2">
                            Account Discount (optional)
                        </td>
                        <td colspan="4" class="WhatIfControlStyle PaddingBottom0">
                            <asp:TextBox ID="txtClientDiscount" CssClass="textRight Width95Px" runat="server"
                                AutoPostBack="true" OnTextChanged="txtClientDiscount_TextChanged">0</asp:TextBox>%
                            <asp:CompareValidator ID="compClientDiscount" runat="server" ControlToValidate="txtClientDiscount"
                                ErrorMessage="A number with 2 decimal digits is allowed for the Account Discount."
                                ToolTip="A number with 2 decimal digits is allowed for the Account Discount."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ComputeRate"
                                Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                        </td>
                    </tr> 
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td class="vTop">
                            <asp:GridView ID="gvOverheadWhatIf" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here."
                                ShowFooter="True" CssClass="CompPerfTable gvOverheadWhatIf">
                                <AlternatingRowStyle CssClass="alterrow" />
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Rate / Overhead</div>
                                        </HeaderTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("EncodedName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <br />
                                            <hr />
                                            Fully Loaded Hourly Rate (FLHR)
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle CssClass="textRight" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Amount per Hour</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("HourlyValue") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle CssClass="textRight" />
                                        <FooterTemplate>
                                            <br />
                                            <hr />
                                            <asp:Label ID="lblLoadedHourlyRate" runat="server" CssClass="fontBold"></asp:Label>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
            <td id="tdgrossMarginComputing" visible="false" runat="server" class="WhatIfGrossMarginComputing">
                <uc1:GrossMarginComputing ID="grossMarginComputing" runat="server" />
            </td>
        </tr>
    </table>
</asp:Panel>
<uc:MessageLabel ID="mlMessage" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
    WarningColor="Orange" EnableViewState="false" />

