<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommissionsAttribution.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.CommissionsAttribution" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<asp:UpdatePanel runat="server" ID="upCommissionAttribution">
    <ContentTemplate>
        <asp:Panel ID="pnlCommissions" runat="server" CssClass="tab-pane">
            <table class="WholeWidth">
                <tr style="vertical-align: top; height: 10%">
                    <td class="Width47Percent textCenter PaddingBottom2">
                        <span class="attributionHeader">Delivery Attribution</span>
                    </td>
                    <td class="Width6PercentImp">
                    </td>
                    <td class="Width47Percent textCenter PaddingBottom2">
                        <span class="attributionHeader">Sales Attribution </span>
                    </td>
                </tr>
                <tr style="height: 10%">
                    <td class="attributionFirstTd">
                        <asp:Button ID="btnAddDeliveryAttributionResource" runat="server" Text="Add Resource"
                            CssClass="Width95Px" OnClientClick="setDirty();" Attribution="1" OnClick="btnAddRecord_Click" />
                    </td>
                    <td class="Width6PercentImp">
                    </td>
                    <td class="attributionFirstTd">
                        <asp:Button ID="btnAddSalesAttributionResource" runat="server" Text="Add Resource"
                            CssClass="Width95Px" OnClientClick="setDirty();" Attribution="2" OnClick="btnAddRecord_Click" />
                    </td>
                </tr>
                <tr style="height: 45%">
                    <td class="Width47Percent vTop borderLeftRight_black PaddingLeftRight2px">
                        <asp:GridView ID="gvDeliveryAttributionPerson" runat="server" AutoGenerateColumns="False"
                            Attribution="1" OnRowDataBound="gvDeliveryAttributionPerson_RowDataBound" CssClass="CompPerfTable MileStoneDetailPageResourcesTab"
                            EditRowStyle-Wrap="false" RowStyle-Wrap="false" HeaderStyle-Wrap="false" EmptyDataText='There are no people assigned to receive Delivery credit. Please select "Add Resource" to attribute credit.'
                            AttributionId="0" GridLines="None" BackColor="White">
                            <AlternatingRowStyle CssClass="bgcolorF9FAFFImp" />
                            <HeaderStyle CssClass="textCenter" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg" style="border-left: #c5c5c5 1px solid;">
                                            <asp:HiddenField ID="hdnAttributionType" runat="server" Value="Delivery" />
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width2Percent" />
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="hdnAttributionId" />
                                        <asp:CheckBox ID="chbAttribution" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width6Percent" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgDeliveryPersonAttributeEdit" ToolTip="Edit" runat="server"
                                            OnClientClick="setDirty();" OnClick="imgPersonEdit_Click" ImageUrl="~/Images/icon-edit.png" />
                                        <asp:ImageButton ID="imgDeliveryPersonAttributeUpdate" ToolTip="Update" runat="server"
                                            OnClick="imgPersonUpdate_Click" Visible="false" ImageUrl="~/Images/icon-check.png" />
                                        <asp:ImageButton ID="imgDeliveryPersonAttributeCancel" ToolTip="Cancel" runat="server"
                                            OnClick="imgPersonCancel_Click" Visible="false" ImageUrl="~/Images/no.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Person</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width25Percent textLeft" />
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnPersonId" runat="server" />
                                        <asp:HiddenField ID="hdnEditMode" runat="server" />
                                        <asp:Label ID="lblPersonName" runat="server" CssClass="padLeft8"></asp:Label>
                                        <asp:DropDownList ID="ddlPerson" runat="server" Visible="false" CssClass="Width96Per">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPerson"
                                            ErrorMessage="Delivery Attribution: The person name is required." ToolTip="The person name is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator runat="server" ID="custValidRange" Display="Dynamic" ErrorMessage="Delivery Attribution: The start date and end date of the person is not within the individual's hire date and termination range."
                                            ToolTip="The start date and end date of the person is not within the individual's hire date and termination range."
                                            OnServerValidate="custValidRange_ServerValidate" Text="*"></asp:CustomValidator>
                                        <asp:CustomValidator runat="server" ID="custPaytypeValidation" Display="Dynamic"
                                            ErrorMessage="Delivery Attribution: The person is not a 'W2-Hourly/W2-Salary' during the period."
                                            ToolTip="The person is not a 'W2-Hourly/W2-Salary' during the period." Text="*"
                                            OnServerValidate="custPaytypeValidation_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator runat="server" ID="custDivision" Display="Dynamic" Text="*"
                                            OnServerValidate="custDivision_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Title
                                            <asp:CustomValidator runat="server" ID="custTitleValidation" Display="Dynamic" ErrorMessage="Delivery Attribution: The start date and end date of two persons with same title cannot overlap."
                                                ToolTip="The start date and end date of two persons with same title cannot overlap."
                                                Text="*" OnServerValidate="custTitleValidation_ServerValidate"></asp:CustomValidator>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width25Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="hdnTitleId" />
                                        <asp:Label ID="lblTitleName" runat="server" CssClass="WholeWidth"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Start Date</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width19Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                                        <uc2:DatePicker ID="dpStartDate" runat="server" SetDirty="false" Visible="false"
                                            TextBoxWidth="90%" AutoPostBack="false" />
                                        <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="Delivery Attribution: The person start date is required." ToolTip="The person start date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="Delivery Attribution: The person start date is in incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The person start date is in incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonStart" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="Delivery Attribution: The person start date must be greater than or equal to the project start date."
                                            ToolTip="The person start date must be greater than or equal to the project start date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonStart_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            End Date</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width19Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblEndDate" runat="server"></asp:Label>
                                        <uc2:DatePicker ID="dpEndDate" runat="server" SetDirty="false" Visible="false" TextBoxWidth="90%"
                                            AutoPostBack="false" />
                                        <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="Delivery Attribution: The person end date is required." ToolTip="The person end date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="Delivery Attribution: The person end date is in incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The person end date is in incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="Delivery Attribution: The person end date must be less than or equal to the project end date."
                                            ToolTip="The person end date must be less than or equal to the project end date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonEnd_ServerValidate"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ControlToCompare="dpStartDate" ErrorMessage="Delivery Attribution: The person end date must be greater than or equal to the person start date."
                                            ToolTip="The person end date must be greater than or equal to the person start date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custLockdown" runat="server" Text="*" EnableClientScript="false"
                                            SetFocusOnError="true" Display="Dynamic" OnServerValidate="custLockdown_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgDeliveryAttributionPersonDelete" ToolTip="Delete" runat="server"
                                            OnClientClick="setDirty();" OnClick="imgPersonDelete_Click" ImageUrl="~/Images/icon-delete.png" />
                                        <asp:CustomValidator ID="custPersonDatesOverlapping" runat="server" ErrorMessage="Delivery Attribution: The start date and end date of two persons with same title cannot overlap."
                                            ToolTip="The start date and end date of two persons with same title cannot overlap."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonDatesOverlapping_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                    <td class="Width6PercentImp">
                        <table class="WholeWidth alignCenter">
                            <tr>
                                <td class="paddingBottom5px">
                                    <asp:Button ID="btnCopyAlltoRight" runat="server" Text=">>" OnClick="btnCopyAlltoRight_Click"
                                        OnClientClick="setDirty();" CssClass="Width30Px" Style="padding: 2px 4px 2px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="paddingBottom5px">
                                    <asp:Button ID="btnCopySelectedItemstoRight" runat="server" Text=">" OnClick="btnCopySelectedItemstoRight_Click"
                                        OnClientClick="setDirty();" CssClass="Width30Px" Style="padding: 2px 4px 2px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="paddingBottom5px">
                                    <asp:Button ID="btnCopyAlltoLeft" runat="server" Text="<<" OnClick="btnCopyAlltoLeft_Click"
                                        OnClientClick="setDirty();" CssClass="Width30Px" Style="padding: 2px 4px 2px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="paddingBottom5px">
                                    <asp:Button ID="btnCopySelectedItemstoLeft" runat="server" Text="<" OnClick="btnCopySelectedItemstoLeft_Click"
                                        OnClientClick="setDirty();" CssClass="Width30Px" Style="padding: 2px 4px 2px" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width47Percent vTop borderLeftRight_black PaddingLeftRight2px">
                        <asp:GridView ID="gvSalesAttributionPerson" runat="server" AutoGenerateColumns="False"
                            Attribution="2" CssClass="CompPerfTable MileStoneDetailPageResourcesTab" EditRowStyle-Wrap="false"
                            EmptyDataText='There are no people assigned to receive Sales credit. Please select "Add Resource" to attribute credit.'
                            RowStyle-Wrap="false" AttributionId="0" HeaderStyle-Wrap="false" GridLines="None"
                            BackColor="White" OnRowDataBound="gvSalesAttributionPerson_RowDataBound">
                            <AlternatingRowStyle CssClass="bgcolorF9FAFFImp" />
                            <HeaderStyle CssClass="textCenter" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg" style="border-left: #c5c5c5 1px solid;">
                                            <asp:HiddenField ID="hdnAttributionType" runat="server" Value="Sales" />
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width2Percent" />
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="hdnAttributionId" />
                                        <asp:CheckBox ID="chbAttribution" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width6Percent" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgSalesPersonAttributeEdit" ToolTip="Edit" runat="server" OnClick="imgPersonEdit_Click"
                                            OnClientClick="setDirty();" ImageUrl="~/Images/icon-edit.png" />
                                        <asp:ImageButton ID="imgSalesPersonAttributeUpdate" ToolTip="Update" runat="server"
                                            Visible="false" OnClick="imgPersonUpdate_Click" ImageUrl="~/Images/icon-check.png" />
                                        <asp:ImageButton ID="imgSalesPersonAttributeCancel" ToolTip="Cancel" runat="server"
                                            OnClick="imgPersonCancel_Click" Visible="false" ImageUrl="~/Images/no.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Person</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width24Percent textLeft" />
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnPersonId" runat="server" />
                                        <asp:HiddenField ID="hdnEditMode" runat="server" />
                                        <asp:Label ID="lblPersonName" runat="server" CssClass="padLeft8"></asp:Label>
                                        <asp:DropDownList ID="ddlPerson" runat="server" Visible="false" CssClass="Width96Per">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPerson"
                                            ErrorMessage="Sales Attribution: The person Name is required." ToolTip="The Person Name is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator runat="server" ID="custValidRange" Display="Dynamic" ErrorMessage="Sales Attribution: The start date and end date of the person is not within the individual's hire date and termination range."
                                            ToolTip="The start date and end date of the person is not within the individual's hire date and termination range."
                                            OnServerValidate="custValidRange_ServerValidate" Text="*"></asp:CustomValidator>
                                        <asp:CustomValidator runat="server" ID="custPaytypeValidation" Display="Dynamic"
                                            ErrorMessage="Sales Attribution: The person is not a 'W2-Hourly/W2-Salary' during the period."
                                            ToolTip="The person is not a 'W2-Hourly/W2-Salary' during the period." Text="*"
                                            OnServerValidate="custPaytypeValidation_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator runat="server" ID="custDivision" Display="Dynamic" Text="*"
                                            OnServerValidate="custDivision_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Title
                                            <asp:CustomValidator runat="server" ID="custTitleValidation" Display="Dynamic" ErrorMessage="Sales Attribution: The start date and end date of two persons with same title cannot overlap."
                                                ToolTip="The start date and end date of two persons with same title cannot overlap."
                                                Text="*" OnServerValidate="custTitleValidation_ServerValidate"></asp:CustomValidator>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width24Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="hdnTitleId" />
                                        <asp:Label ID="lblTitleName" runat="server" CssClass="WholeWidth"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Start Date</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width20Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                                        <uc2:DatePicker ID="dpStartDate" runat="server" SetDirty="false" Visible="false"
                                            TextBoxWidth="90%" AutoPostBack="false" />
                                        <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="Sales Attribution: The person start date is required." ToolTip="The person start date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="Sales Attribution: The person start date is in incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The person start date is in incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonStart" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="Sales Attribution: The person start date must be greater than or equal to the project start date."
                                            ToolTip="The person start date must be greater than or equal to the project start date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonStart_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            End Date</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width20Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblEndDate" runat="server"></asp:Label>
                                        <uc2:DatePicker ID="dpEndDate" runat="server" SetDirty="false" Visible="false" TextBoxWidth="90%"
                                            AutoPostBack="false" />
                                        <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="Sales Attribution: The person end date is required." ToolTip="The person end date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="Sales Attribution: The person end date is in incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The person end date is in incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="Sales Attribution: The person end date must be less than or equal to the project end date."
                                            ToolTip="The person end date must be less than or equal to the project end date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonEnd_ServerValidate"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ControlToCompare="dpStartDate" ErrorMessage="Sales Attribution: The person end date must be greater than or equal to the person start date."
                                            ToolTip="The person end date must be greater than or equal to the person start date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custLockdown" runat="server" Text="*" EnableClientScript="false"
                                            SetFocusOnError="true" Display="Dynamic" OnServerValidate="custLockdown_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgSalesAttributionPersonDelete" ToolTip="Delete" runat="server"
                                            OnClientClick="setDirty();" OnClick="imgPersonDelete_Click" ImageUrl="~/Images/icon-delete.png" />
                                        <asp:CustomValidator ID="custPersonDatesOverlapping" runat="server" ErrorMessage="Sales Attribution: The start date and end date of two persons with same title cannot overlap."
                                            ToolTip="The start date and end date of two persons with same title cannot overlap."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonDatesOverlapping_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr style="height: 10%">
                    <td class="Width47Percent AlignRight PaddingTop6 paddingBottom5px borderLeftRight_black padRight5">
                        <asp:Button ID="btnAddDeliveryAttributionPractice" runat="server" Text="Add Practice"
                            CssClass="Width95Px" OnClientClick="setDirty();" Attribution="3" OnClick="btnAddRecord_Click" />
                    </td>
                    <td class="Width6PercentImp">
                    </td>
                    <td class="Width47Percent AlignRight PaddingTop6 paddingBottom5px borderLeftRight_black padRight5">
                        <asp:Button ID="btnAddSalesAttributionPractice" runat="server" Text="Add Practice"
                            CssClass="Width95Px" OnClientClick="setDirty();" Attribution="4" OnClick="btnAddRecord_Click" />
                    </td>
                </tr>
                <tr style="height: 25%">
                    <td class="Width47Percent vTop border_blackExceptTop PaddingLeftRight2px paddingBottom10px">
                        <asp:GridView ID="gvDeliveryAttributionPractice" runat="server" AutoGenerateColumns="False"
                            Attribution="3" CssClass="CompPerfTable MileStoneDetailPageResourcesTab" EditRowStyle-Wrap="false"
                            AttributionId="0" OnRowDataBound="gvDeliveryAttributionPractice_RowDataBound"
                            EmptyDataText='There are no practices assigned to receive Delivery credit. Please select "Add Practice" to attribute credit.'
                            RowStyle-Wrap="false" HeaderStyle-Wrap="false" GridLines="None" BackColor="White">
                            <AlternatingRowStyle CssClass="bgcolorF9FAFFImp" />
                            <HeaderStyle CssClass="textCenter" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg" style="border-left: #c5c5c5 1px solid;">
                                            <asp:HiddenField ID="hdnAttributionType" runat="server" Value="Delivery" />
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width6Percent" />
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="hdnAttributionId" />
                                        <asp:ImageButton ID="imgDeliveryPracticeAttributeEdit" ToolTip="Edit" runat="server"
                                            OnClientClick="setDirty();" OnClick="imgPracticeEdit_Click" ImageUrl="~/Images/icon-edit.png" />
                                        <asp:ImageButton ID="imgDeliveryPracticeAttributeUpdate" ToolTip="Update" runat="server"
                                            OnClick="imgPracticeUpdate_Click" Visible="false" ImageUrl="~/Images/icon-check.png" />
                                        <asp:ImageButton ID="imgDeliveryPracticeAttributeCancel" ToolTip="Cancel" runat="server"
                                            OnClick="imgPracticeCancel_Click" Visible="false" ImageUrl="~/Images/no.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Practice
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width78Percent textLeft" />
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnPracticeId" runat="server" />
                                        <asp:HiddenField ID="hdnEditMode" runat="server" />
                                        <asp:Label ID="lblPractice" runat="server" CssClass="PaddingLeft10Px"></asp:Label>&nbsp;&nbsp;
                                        <asp:DropDownList ID="ddlPractice" runat="server" Visible="false">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                            ErrorMessage="Delivery Attribution: The Practice is required." ToolTip="The Practice is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            %
                                            <asp:CustomValidator ID="custCommissionsPercentage" runat="server" ErrorMessage="Delivery Attribution: Attribution percentages cannot total more than 100% for all selected Practice Areas."
                                                ToolTip="Attribution percentages cannot total more than 100% for all selected Practice Areas."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                OnServerValidate="custCommissionsPercentage_ServerValidate"></asp:CustomValidator></div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width12Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblCommisssionPercentage" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtCommisssionPercentage" runat="server" Visible="false" CssClass="Width85Percent"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="reqCommisssionPercentage" runat="server" ControlToValidate="txtCommisssionPercentage"
                                            ErrorMessage="Delivery Attribution: The commisssion percentage is required."
                                            ToolTip="The commisssion percentage is required." Display="Dynamic" Text="*"
                                            EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator runat="server" ID="compCommissionPercentage" Operator="GreaterThanEqual"
                                            ControlToValidate="txtCommisssionPercentage" ValueToCompare="1" Type="Double"
                                            ErrorMessage="Delivery Attribution: The percentage of commission for a practice area should be greater than or equal to 1."
                                            Text="*" ToolTip="The percentage of commission for a practice area should be greater than or equal to 1."></asp:CompareValidator>
                                        <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtPercentage" runat="server"
                                            TargetControlID="txtCommisssionPercentage" FilterMode="ValidChars" FilterType="Custom,Numbers"
                                            ValidChars=".">
                                        </AjaxControlToolkit:FilteredTextBoxExtender>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgDeliveryAttributionPracticeDelete" ToolTip="Delete" runat="server"
                                            OnClientClick="setDirty();" OnClick="imgPracticeDelete_Click" ImageUrl="~/Images/icon-delete.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                    <td class="Width6PercentImp">
                    </td>
                    <td class="Width47Percent vTop border_blackExceptTop PaddingLeftRight2px paddingBottom10px">
                        <asp:GridView ID="gvSalesAttributionPractice" runat="server" AutoGenerateColumns="False"
                            Attribution="4" CssClass="CompPerfTable MileStoneDetailPageResourcesTab" EditRowStyle-Wrap="false"
                            AttributionId="0" EmptyDataText='There are no practices assigned to receive Sales credit. Please select "Add Practice" to attribute credit.'
                            RowStyle-Wrap="false" HeaderStyle-Wrap="false" GridLines="None" BackColor="White"
                            OnRowDataBound="gvSalesAttributionPractice_RowDataBound">
                            <AlternatingRowStyle CssClass="bgcolorF9FAFFImp" />
                            <HeaderStyle CssClass="textCenter" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg" style="border-left: #c5c5c5 1px solid;">
                                            <asp:HiddenField ID="hdnAttributionType" runat="server" Value="Sales" />
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width6Percent" />
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="hdnAttributionId" />
                                        <asp:ImageButton ID="imgSalesPracticeAttributeEdit" ToolTip="Edit" runat="server"
                                            OnClientClick="setDirty();" OnClick="imgPracticeEdit_Click" ImageUrl="~/Images/icon-edit.png" />
                                        <asp:ImageButton ID="imgSalesPracticeAttributeUpdate" ToolTip="Update" runat="server"
                                            OnClick="imgPracticeUpdate_Click" Visible="false" ImageUrl="~/Images/icon-check.png" />
                                        <asp:ImageButton ID="imgSalesPracticeAttributeCancel" ToolTip="Cancel" runat="server"
                                            OnClick="imgPracticeCancel_Click" Visible="false" ImageUrl="~/Images/no.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Practice</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width78Percent textLeft" />
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnPracticeId" runat="server" />
                                        <asp:HiddenField ID="hdnEditMode" runat="server" />
                                        <asp:Label ID="lblPractice" runat="server" CssClass="PaddingLeft10Px"></asp:Label>&nbsp;&nbsp;
                                        <asp:DropDownList ID="ddlPractice" runat="server" Visible="false">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                            ErrorMessage="Sales Attribution: The Practice is required." ToolTip="The Practice is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            %
                                            <asp:CustomValidator ID="custCommissionsPercentage" runat="server" ErrorMessage="Sales Attribution: Attribution percentages cannot total more than 100% for all selected Practice Areas."
                                                ToolTip="Attribution percentages cannot total more than 100% for all selected Practice Areas."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                OnServerValidate="custCommissionsPercentage_ServerValidate"></asp:CustomValidator></div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width12Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblCommisssionPercentage" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtCommisssionPercentage" runat="server" Visible="false" CssClass="Width85Percent"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="reqCommisssionPercentage" runat="server" ControlToValidate="txtCommisssionPercentage"
                                            ErrorMessage="Sales Attribution: The commisssion percentage is required." ToolTip="The commisssion percentage is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator runat="server" ID="compCommissionPercentage" Operator="GreaterThanEqual"
                                            ControlToValidate="txtCommisssionPercentage" ValueToCompare="1" Type="Double"
                                            ErrorMessage="Sales Attribution: The percentage of commission for a practice area should be greater than or equal to 1."
                                            Text="*" ToolTip="The percentage of commission for a practice area should be greater than or equal to 1."></asp:CompareValidator>
                                        <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtPercentage" runat="server"
                                            TargetControlID="txtCommisssionPercentage" FilterMode="ValidChars" FilterType="Custom,Numbers"
                                            ValidChars=".">
                                        </AjaxControlToolkit:FilteredTextBoxExtender>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent textCenter" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgSalesAttributionPracticeDelete" ToolTip="Delete" runat="server"
                                            OnClientClick="setDirty();" OnClick="imgPracticeDelete_Click" ImageUrl="~/Images/icon-delete.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

