<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommissionsAttribution.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.CommissionsAttribution" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlCommissions" runat="server" CssClass="tab-pane">
            <table class="WholeWidth">
                <tr style="vertical-align: top; height: 10%">
                    <td class="attributionFirstTd">
                        <span class="attributionHeader">Delivery Attribution</span>
                    </td>
                    <td class="Width6PercentImp">
                    </td>
                    <td class="attributionFirstTd">
                        <span class="attributionHeader">Sales Attribution </span>
                    </td>
                </tr>
                <tr style="height: 10%">
                    <td class="Width47Percent AlignRight PaddingBottom2 borderLeftRight_black">
                        <asp:Button ID="btnAddDeliveryAttributionResource" runat="server" Text="Add Resource"
                            Attribution="1" OnClick="btnAddRecord_Click" />
                    </td>
                    <td class="Width6PercentImp">
                    </td>
                    <td class="Width47Percent AlignRight PaddingBottom2 borderLeftRight_black">
                        <asp:Button ID="btnAddSalesAttributionResource" runat="server" Text="Add Resource"
                            Attribution="2" OnClick="btnAddRecord_Click" />
                    </td>
                </tr>
                <tr style="height: 45%">
                    <td class="Width47Percent vTop borderLeftRight_black">
                        <asp:GridView ID="gvDeliveryAttributionPerson" runat="server" AutoGenerateColumns="False"
                            Attribution="1" OnRowDataBound="gvDeliveryAttributionPerson_RowDataBound" CssClass="CompPerfTable MileStoneDetailPageResourcesTab"
                            EditRowStyle-Wrap="false" RowStyle-Wrap="false" HeaderStyle-Wrap="false" EmptyDataText="No persons were assigned to this attribution."
                            GridLines="None" BackColor="White">
                            <AlternatingRowStyle CssClass="alterrow" />
                            <HeaderStyle CssClass="textCenter" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            <asp:HiddenField ID="hdnAttributionType" runat="server" Value="Delivery" />
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width2Percent" />
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chbDeliveryAttributes" runat="server" OnCheckedChanged="chbDeliveryAttributes_CheckedChanged"
                                            AutoPostBack="true" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgDeliveryPersonAttributeEdit" ToolTip="Edit" runat="server"
                                            OnClick="imgPersonEdit_Click" ImageUrl="~/Images/icon-edit.png" />
                                        <asp:ImageButton ID="imgDeliveryPersonAttributeUpdate" ToolTip="Update" runat="server"
                                            OnClick="imgPersonUpdate_Click" Visible="false" ImageUrl="~/Images/icon-check.png" />
                                        <asp:ImageButton ID="imgDeliveryPersonAttributeCancel" ToolTip="Cancel" runat="server"
                                            OnClick="imgPersonCancel_Click" Visible="false" ImageUrl="~/Images/no.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width2Percent" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgDeliveryAttributionAdditionalAllocationOfResource" runat="server"
                                            ToolTip="Extend" OnClick="imgAdditionalAllocationOfResource_Click" ImageUrl="~/Images/add_16.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Person</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width25Percent textLeft WS-Normal" />
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnPersonId" runat="server" />
                                        <asp:HiddenField ID="hdnEditMode" runat="server" />
                                        <asp:Label ID="lblPersonName" runat="server" CssClass="padLeft8"></asp:Label>
                                        <asp:DropDownList ID="ddlPerson" runat="server" Visible="false" CssClass="WholeWidth">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPerson"
                                            ErrorMessage="The person name is required." ToolTip="The person name is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Title</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width25Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblTitleName" runat="server" CssClass="WholeWidth"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            StartDate</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width19Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                                        <uc2:DatePicker ID="dpStartDate" runat="server" SetDirty="false" Visible="false"
                                            TextBoxWidth="90%" AutoPostBack="false" />
                                        <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="The person start date is required." ToolTip="The person start date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="The person start date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The person start date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonStart" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="The person start date must be greater than or equal to the project start date."
                                            ToolTip="The person start date must be greater than or equal to the project start date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonStart_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            EndDate</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width19Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblEndDate" runat="server"></asp:Label>
                                        <uc2:DatePicker ID="dpEndDate" runat="server" SetDirty="false" Visible="false" TextBoxWidth="90%"
                                            AutoPostBack="false" />
                                        <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="The person end date is required." ToolTip="The person end date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="The person end date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The person end date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="The person end date must be less than or equal to the project end date."
                                            ToolTip="The person end date must be less than or equal to the project end date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonEnd_ServerValidate"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ControlToCompare="dpStartDate" ErrorMessage="The person end date must be greater than or equal to the person start date."
                                            ToolTip="The person end date must be greater than or equal to the person start date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgDeliveryAttributionPersonDelete" ToolTip="Delete" runat="server"
                                            OnClick="imgPersonDelete_Click" ImageUrl="~/Images/cross_icon.png" />
                                        <asp:CustomValidator ID="custPersonDatesOverlapping" runat="server" ErrorMessage="For a person the startDate and endDate with a title should not overlap with the another record of startDate and endDate for the same title of the same person."
                                            ToolTip="For a person the startDate and endDate with a title should not overlap with the another record of startDate and endDate for the same title of the same person."
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
                                <td>
                                    <asp:Button ID="btnCopyAlltoRight" runat="server" Text=">>" OnClick="btnCopyAlltoRight_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnCopySelectedItemstoRight" runat="server" Text=">" OnClick="btnCopySelectedItemstoRight_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnCopyAlltoLeft" runat="server" Text="<<" OnClick="btnCopyAlltoLeft_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnCopySelectedItemstoLeft" runat="server" Text="<" OnClick="btnCopySelectedItemstoLeft_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width47Percent vTop borderLeftRight_black">
                        <asp:GridView ID="gvSalesAttributionPerson" runat="server" AutoGenerateColumns="False"
                            Attribution="2" CssClass="CompPerfTable MileStoneDetailPageResourcesTab" EditRowStyle-Wrap="false"
                            EmptyDataText="No persons were assigned to this attribution." RowStyle-Wrap="false"
                            HeaderStyle-Wrap="false" GridLines="None" BackColor="White" OnRowDataBound="gvSalesAttributionPerson_RowDataBound">
                            <AlternatingRowStyle CssClass="alterrow" />
                            <HeaderStyle CssClass="textCenter" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            <asp:HiddenField ID="hdnAttributionType" runat="server" Value="Sales" />
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width2Percent" />
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chbSalesAttributes" runat="server" OnCheckedChanged="chbSalesAttributes_CheckedChanged" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgSalesPersonAttributeEdit" ToolTip="Edit" runat="server" OnClick="imgPersonEdit_Click"
                                            ImageUrl="~/Images/icon-edit.png" />
                                        <asp:ImageButton ID="imgSalesPersonAttributeUpdate" ToolTip="Update" runat="server"
                                            Visible="false" OnClick="imgPersonUpdate_Click" ImageUrl="~/Images/icon-check.png" />
                                        <asp:ImageButton ID="imgSalesPersonAttributeCancel" ToolTip="Cancel" runat="server"
                                            OnClick="imgPersonCancel_Click" Visible="false" ImageUrl="~/Images/no.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width2Percent" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgSalesAttributionAdditionalAllocationOfResource" runat="server"
                                            ToolTip="Extend" OnClick="imgAdditionalAllocationOfResource_Click" ImageUrl="~/Images/add_16.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Person</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width25Percent textLeft WS-Normal" />
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnPersonId" runat="server" />
                                        <asp:HiddenField ID="hdnEditMode" runat="server" />
                                        <asp:Label ID="lblPersonName" runat="server" CssClass="padLeft8"></asp:Label>
                                        <asp:DropDownList ID="ddlPerson" runat="server" Visible="false" CssClass="WholeWidth">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPerson"
                                            ErrorMessage="The person Name is required." ToolTip="The Person Name is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Title</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width25Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblTitleName" runat="server" CssClass="WholeWidth"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            StartDate</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width19Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                                        <uc2:DatePicker ID="dpStartDate" runat="server" SetDirty="false" Visible="false"
                                            TextBoxWidth="90%" AutoPostBack="false" />
                                        <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="The person start date is required." ToolTip="The person start date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="The person start date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The person start date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonStart" runat="server" ControlToValidate="dpStartDate"
                                            ErrorMessage="The person start date must be greater than or equal to the project start date."
                                            ToolTip="The person start date must be greater than or equal to the project start date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonStart_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            EndDate</div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width19Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblEndDate" runat="server"></asp:Label>
                                        <uc2:DatePicker ID="dpEndDate" runat="server" SetDirty="false" Visible="false" TextBoxWidth="90%"
                                            AutoPostBack="false" />
                                        <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="The person end date is required." ToolTip="The person end date is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="The person end date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The person end date has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ErrorMessage="The person end date must be less than or equal to the project end date."
                                            ToolTip="The person end date must be less than or equal to the project end date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonEnd_ServerValidate"></asp:CustomValidator>
                                        <asp:CompareValidator ID="compPersonEnd" runat="server" ControlToValidate="dpEndDate"
                                            ControlToCompare="dpStartDate" ErrorMessage="The person end date must be greater than or equal to the person start date."
                                            ToolTip="The person end date must be greater than or equal to the person start date."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgSalesAttributionPersonDelete" ToolTip="Delete" runat="server"
                                            OnClick="imgPersonDelete_Click" ImageUrl="~/Images/cross_icon.png" />
                                        <asp:CustomValidator ID="custPersonDatesOverlapping" runat="server" ErrorMessage="For a person the startDate and endDate with a title should not overlap with the another record of startDate and endDate for the same title of the same person."
                                            ToolTip="For a person the startDate and endDate with a title should not overlap with the another record of startDate and endDate for the same title of the same person."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonDatesOverlapping_ServerValidate"></asp:CustomValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr style="height: 10%">
                    <td class="Width47Percent AlignRight PaddingTop6 PaddingBottom2 borderLeftRight_black">
                        <asp:Button ID="btnAddDeliveryAttributionPractice" runat="server" Text="Add Practice"
                            Attribution="3" OnClick="btnAddRecord_Click" />
                    </td>
                    <td class="Width6PercentImp">
                    </td>
                    <td class="Width47Percent AlignRight PaddingTop6 PaddingBottom2 borderLeftRight_black">
                        <asp:Button ID="btnAddSalesAttributionPractice" runat="server" Text="Add Practice"
                            Attribution="4" OnClick="btnAddRecord_Click" />
                    </td>
                </tr>
                <tr style="height: 25%">
                    <td class="Width47Percent vTop border_blackExceptTop">
                        <asp:GridView ID="gvDeliveryAttributionPractice" runat="server" AutoGenerateColumns="False" 
                            Attribution="3" CssClass="CompPerfTable MileStoneDetailPageResourcesTab" EditRowStyle-Wrap="false"
                            OnRowDataBound="gvDeliveryAttributionPractice_RowDataBound" EmptyDataText="No practices were assigned to this attribution."
                            RowStyle-Wrap="false" HeaderStyle-Wrap="false" GridLines="None" BackColor="White">
                            <AlternatingRowStyle CssClass="alterrow" />
                            <HeaderStyle CssClass="textCenter" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            <asp:HiddenField ID="hdnAttributionType" runat="server" Value="Delivery" />
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgDeliveryPracticeAttributeEdit" ToolTip="Edit" runat="server"
                                            OnClick="imgPracticeEdit_Click" ImageUrl="~/Images/icon-edit.png" />
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
                                    <ItemStyle CssClass="Width80Percent textLeft WS-Normal" />
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnPracticeId" runat="server" />
                                        <asp:HiddenField ID="hdnEditMode" runat="server" />
                                        <asp:Label ID="lblPractice" runat="server" CssClass="padLeft8"></asp:Label>
                                        <asp:DropDownList ID="ddlPractice" runat="server" Visible="false">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                            ErrorMessage="The Practice is required." ToolTip="The Practice is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            %
                                            <asp:CustomValidator ID="custCommissionsPercentage" runat="server" ErrorMessage="The sum of commissions percentage should be 100."
                                                ToolTip="The sum of commissions percentage should be 100." Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Dynamic" OnServerValidate="custCommissionsPercentage_ServerValidate"></asp:CustomValidator></div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width12Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblCommisssionPercentage" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtCommisssionPercentage" runat="server" Visible="false"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="reqCommisssionPercentage" runat="server" ControlToValidate="txtCommisssionPercentage"
                                            ErrorMessage="The commisssion percentage is required." ToolTip="The commisssion percentage is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
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
                                    <ItemStyle CssClass="Width4Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgDeliveryAttributionPracticeDelete" ToolTip="Delete" runat="server"
                                            OnClick="imgPracticeDelete_Click" ImageUrl="~/Images/cross_icon.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                    <td class="Width6PercentImp">
                    </td>
                    <td class="Width47Percent vTop border_blackExceptTop">
                        <asp:GridView ID="gvSalesAttributionPractice" runat="server" AutoGenerateColumns="False"
                            Attribution="4" CssClass="CompPerfTable MileStoneDetailPageResourcesTab" EditRowStyle-Wrap="false"
                            EmptyDataText="No practices were assigned to this attribution." RowStyle-Wrap="false"
                            HeaderStyle-Wrap="false" GridLines="None" BackColor="White" OnRowDataBound="gvSalesAttributionPractice_RowDataBound">
                            <AlternatingRowStyle CssClass="alterrow" />
                            <HeaderStyle CssClass="textCenter" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            <asp:HiddenField ID="hdnAttributionType" runat="server" Value="Sales" />
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width4Percent" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgSalesPracticeAttributeEdit" ToolTip="Edit" runat="server"
                                            OnClick="imgPracticeEdit_Click" ImageUrl="~/Images/icon-edit.png" />
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
                                    <ItemStyle CssClass="Width80Percent textLeft WS-Normal" />
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnPracticeId" runat="server" />
                                        <asp:HiddenField ID="hdnEditMode" runat="server" />
                                        <asp:Label ID="lblPractice" runat="server" CssClass="padLeft8"></asp:Label>
                                        <asp:DropDownList ID="ddlPractice" runat="server" Visible="false">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                            ErrorMessage="The practice is required." ToolTip="The practice is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            %
                                            <asp:CustomValidator ID="custCommissionsPercentage" runat="server" ErrorMessage="The sum of commissions percentage should be 100."
                                                ToolTip="The sum of commissions percentage should be 100." Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Dynamic" OnServerValidate="custCommissionsPercentage_ServerValidate"></asp:CustomValidator></div>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="Width12Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblCommisssionPercentage" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtCommisssionPercentage" runat="server" Visible="false"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="reqCommisssionPercentage" runat="server" ControlToValidate="txtCommisssionPercentage"
                                            ErrorMessage="The commisssion percentage is required." ToolTip="The commisssion percentage is required."
                                            Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
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
                                    <ItemStyle CssClass="Width4Percent textCenter WS-Normal" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgSalesAttributionPracticeDelete" ToolTip="Delete" runat="server"
                                            OnClick="imgPracticeDelete_Click" ImageUrl="~/Images/cross_icon.png" />
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
