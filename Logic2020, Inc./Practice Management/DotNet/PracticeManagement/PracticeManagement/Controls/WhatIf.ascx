<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WhatIf.ascx.cs" Inherits="PraticeManagement.Controls.WhatIf" %>
<%@ Register Src="GrossMarginComputing.ascx" TagName="GrossMarginComputing" TagPrefix="uc1" %>
<%@ Register TagPrefix="uc" TagName="MessageLabel" Src="~/Controls/MessageLabel.ascx" %>
<script type="text/javascript" language="javascript">
    function ChangeHourlyBillRate(IsIncrement) {
        var txtBillRate = $get("<%= txtBillRateSlider_BoundControl.ClientID %>");

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
    function ChangeHoursPerWeek(IsIncrement) {
        var txtHPW = $get("<%= txtHorsPerWeekSlider_BoundControl.ClientID %>");

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
</script>
<asp:Panel ID="pnlWhatIf" runat="server">
    <table class="WholeWidth">
        <tr>
            <td valign="top">
                <table>
                    <tr>
                        <th style="background-color: Transparent" nowrap="nowrap" colspan="6">
                            Revenue &amp; Margin Report
                        </th>
                    </tr>
                    <tr>
                        <td align="center" colspan="6">
                            Enter proposed hourly rate and hours per week to calculate margin
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="padding-top: 3px;">
                            Hourly Bill Rate (HBR)
                        </td>
                        <td colspan="5" style="padding-left: 5px">
                            <table>
                                <tr>
                                    <td style="padding-right: 2px;">
                                        <img id="imgDecrBillRate" runat="server" src="~/Images/MinusIcon.png" onclick="ChangeHourlyBillRate(false);"
                                            style="background-color: #D8EFF3; border: 1px solid #000000" />
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
                                    <td style="padding-left: 2px;">
                                        <img id="imgIncrBillRate" runat="server" src="~/Images/PlusIcon.png" onclick="ChangeHourlyBillRate(true);"
                                            style="background-color: #D8EFF3; border: 1px solid #000000" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="margin: 0px; padding-right: 2px; width: 15px !important;">
                                    </td>
                                    <td>
                                        <table class="WholeWidth">
                                            <tr>
                                                <td align="left" style="width: 60px;">
                                                    $<asp:Label ID="lblBillRateMin" runat="server"></asp:Label>
                                                </td>
                                                <td align="center" style="margin: 0px; padding-top: 3px;">
                                                    <asp:TextBox ID="txtBillRateSlider_BoundControl" runat="server" Width="60px" AutoPostBack="true"
                                                        OnTextChanged="txtBillRateSlider_TextChanged" Text="120" Style="text-align: center"></asp:TextBox>
                                                    <asp:CompareValidator ID="compBillRateSlider_BoundControl" runat="server" ControlToValidate="txtBillRateSlider_BoundControl"
                                                        ErrorMessage="A number with 2 decimal digits is allowed for the Bill Rate." ToolTip="A number with 2 decimal digits is allowed for the Bill Rate."
                                                        Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ComputeRate"
                                                        Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                                </td>
                                                <td align="right" style="width: 60px;">
                                                    $<asp:Label ID="lblBillRateMax" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="margin: 0px; padding-left: 2px; width: 15px!important;">
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
                        <td valign="top" style="padding-top: 3px;">
                            Hours per Week (HPW)
                        </td>
                        <td colspan="5" style="padding-left: 5px;">
                            <table>
                                <tr>
                                    <td style="padding-right: 2px;">
                                        <img id="imgDecrHPW" runat="server" src="~/Images/MinusIcon.png" onclick="ChangeHoursPerWeek(false);"
                                            style="background-color: #D8EFF3; border: 1px solid #000000" />
                                    </td>
                                    <td colspan="3">
                                        <asp:TextBox ID="txtHorsPerWeekSlider" runat="server" CssClass="SliderBehavior" AutoPostBack="true"
                                            Text="40"></asp:TextBox>
                                        <AjaxControlToolkit:SliderExtender ID="sldHoursPerMonth" runat="server" BehaviorID="txtHorsPerWeekSlider"
                                            TargetControlID="txtHorsPerWeekSlider" BoundControlID="txtHorsPerWeekSlider_BoundControl"
                                            Orientation="Horizontal" Minimum="10" Maximum="60" Length="350" EnableHandleAnimation="True">
                                        </AjaxControlToolkit:SliderExtender>
                                    </td>
                                    <td style="padding-left: 2px;">
                                        <img id="imgIncrHPW" runat="server" src="~/Images/PlusIcon.png" onclick="ChangeHoursPerWeek(true);"
                                            style="background-color: #D8EFF3; border: 1px solid #000000" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="margin: 0px; width: 15px!important;">
                                    </td>
                                    <td align="left" style="margin: 0px; width: 25px!important;">
                                        <asp:Label ID="lblHoursMin" runat="server"></asp:Label>
                                    </td>
                                    <td align="center" style="margin: 0px; padding-top: 3px;">
                                        <asp:TextBox ID="txtHorsPerWeekSlider_BoundControl" runat="server" Width="60px" AutoPostBack="true"
                                            Text="40" OnTextChanged="txtBillRateSlider_TextChanged" Style="text-align: center"></asp:TextBox>
                                        <asp:CompareValidator ID="compHorsPerWeekSlider_BoundControl" runat="server" ControlToValidate="txtHorsPerWeekSlider_BoundControl"
                                            ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Week."
                                            ToolTip="A number with 2 decimal digits is allowed for the Hours Per Week." Text="*"
                                            EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ComputeRate"
                                            Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                    </td>
                                    <td align="right" style="margin: 0px; width: 25px!important;">
                                        <asp:Label ID="lblHoursMax" runat="server"></asp:Label>
                                    </td>
                                    <td style="margin: 0px; width: 15px!important;">
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
                        <td colspan="2" style="padding-bottom: 3px;">
                            &nbsp;
                        </td>
                        <td colspan="2" nowrap="nowrap" style="font-size: smaller; font-weight: bold; padding-bottom: 3px;">
                            With recruiting costs
                        </td>
                        <td colspan="2" nowrap="nowrap" style="font-size: smaller; font-weight: bold; padding-bottom: 3px;">
                            Without recruiting costs
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding-bottom: 3px;">
                            Monthly Revenue
                        </td>
                        <td colspan="2" style="padding-right: 75px; padding-bottom: 3px;" align="right">
                            <asp:Label ID="lblMonthlyRevenue" runat="server"></asp:Label>
                        </td>
                        <td colspan="2" style="padding-right: 42px; padding-bottom: 3px;" align="right">
                            <asp:Label ID="lblMonthlyRevenueWithoutRecruiting" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding-bottom: 3px;">
                            Monthly COGS
                        </td>
                        <td colspan="2" style="padding-right: 75px; padding-bottom: 3px;" align="right">
                            <asp:Label ID="lblMonthlyGogs" runat="server"></asp:Label>
                        </td>
                        <td colspan="2" style="padding-right: 42px; padding-bottom: 3px;" align="right">
                            <asp:Label ID="lblMonthlyCogsWithoutRecruiting" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding-bottom: 3px;">
                            Monthly Gross Margin
                        </td>
                        <td colspan="2" style="padding-right: 75px; padding-bottom: 3px;" align="right">
                            <asp:Label ID="lblMonthlyGrossMargin" runat="server"></asp:Label>
                        </td>
                        <td colspan="2" style="padding-right: 42px; padding-bottom: 3px;" align="right">
                            <asp:Label ID="lblMonthlyGrossMarginWithoutRecruiting" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trTargetMargin" runat="server" visible="false">
                        <td colspan="2">
                            Target Margin
                        </td>
                        <td colspan="2" style="padding-right: 75px;" align="right">
                            <table>
                                <tr>
                                    <td id="tdTargetMargin" runat="server">
                                        <asp:TextBox ID="txtTargetMargin" Style="text-align: right;" Width="95px" runat="server"
                                            AutoPostBack="true" OnTextChanged="txtTargetMargin_TextChanged"></asp:TextBox>
                                        <asp:Label ID="lblTargetMargin" runat="server" Visible="false"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <asp:CustomValidator ID="custTargetMargin" runat="server" ControlToValidate="txtTargetMargin"
                                ErrorMessage="The Target Margin must be a number and be less then 100." ToolTip="The Target Margin must be a number and be less then 100."
                                EnableClientScript="false" Display="Dynamic" Text="*" ValidateEmptyText="false"
                                SetFocusOnError="true" OnServerValidate="custTargetMargin_ServerValidate" ValidationGroup="ComputeRate"></asp:CustomValidator>
                        </td>
                        <td colspan="2" style="padding-right: 42px;" align="right">
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
                    <%--<tr id="trDefaultSalesCommission" runat="server" visible="true">
                        <td colspan="2">
                            Sales Commission
                        </td>
                        <td colspan="2" style="padding-right: 75px;" align="right">
                            <asp:TextBox ID="txtDefaultSalesCommission" Style="text-align: right;" Width="95px"
                                runat="server" AutoPostBack="true" OnTextChanged="txtDefaultSalesCommission_TextChanged">0%</asp:TextBox>
                            <asp:Label ID="lblDefaultSalesCommission" runat="server" Visible="false"></asp:Label>
                            <asp:CustomValidator ID="cvDefaultSalesCommission" runat="server" ControlToValidate="txtDefaultSalesCommission"
                                ErrorMessage="The Default Sales Commission must be a number and be in range of (0.0 - 10.0)."
                                ToolTip="The Default Sales Commission must be a number and be in range of (0.0 - 10.0)."
                                EnableClientScript="false" Display="Dynamic" Text="*" ValidateEmptyText="false"
                                SetFocusOnError="true" OnServerValidate="custDefaultSalesCommision_ServerValidate"
                                ValidationGroup="ComputeRate"></asp:CustomValidator>
                            <asp:RequiredFieldValidator ID="reqDefaultSalesCommision" runat="server" ControlToValidate="txtDefaultSalesCommission"
                                ErrorMessage="The Default Sales Commission is required." ToolTip="The Default Sales Commission is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                        </td>
                        <td colspan="2">
                        </td>
                    </tr>--%>
                    <%--<tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>--%>
                    <tr id="tr1" runat="server" visible="true">
                        <td colspan="2">
                            Client Discount (optional)
                        </td>
                        <td colspan="2" style="padding-right: 75px;" align="right">
                            <asp:TextBox ID="txtClientDiscount" Style="text-align: right;" Width="95px" runat="server"
                                AutoPostBack="true" OnTextChanged="txtClientDiscount_TextChanged">0%</asp:TextBox>
                            <asp:CustomValidator ID="cvClientDiscount" runat="server" ControlToValidate="txtClientDiscount"
                                ErrorMessage="The Client Discount must be a number." ToolTip="The Client Discount must be a number."
                                EnableClientScript="false" Display="Dynamic" Text="*" ValidateEmptyText="false"
                                SetFocusOnError="true" OnServerValidate="custClientDiscount_ServerValidate" ValidationGroup="ComputeRate"></asp:CustomValidator>
                        </td>
                        <td colspan="2">
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
                        <td valign="top">
                            <asp:GridView ID="gvOverheadWhatIf" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here."
                                ShowFooter="True" CssClass="CompPerfTable" GridLines="None" BackColor="White">
                                <AlternatingRowStyle BackColor="#F9FAFF" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Overhead type">
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Rate / Overhead</div>
                                        </HeaderTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                        <FooterTemplate>
                                            <br />
                                            <hr />
                                            Fully Loaded Hourly Rate (FLHR)
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount per hour">
                                        <ItemStyle HorizontalAlign="Right" />
                                        <HeaderTemplate>
                                            <div class="ie-bg">
                                                Amount per Hour</div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("HourlyValue") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Right" />
                                        <FooterTemplate>
                                            <br />
                                            <hr />
                                            <asp:Label ID="lblLoadedHourlyRate" runat="server" Font-Bold="true"></asp:Label>
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
            <td id="tdgrossMarginComputing" visible="false" runat="server" valign="top" width="340px;"
                style="padding-left: 10px; padding-top: 10px; background-color: #DBEEF3;">
                <uc1:GrossMarginComputing ID="grossMarginComputing" runat="server" />
            </td>
        </tr>
    </table>
</asp:Panel>
<uc:MessageLabel ID="mlMessage" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
    WarningColor="Orange" EnableViewState="false" />

