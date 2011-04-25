<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonReportFilter.ascx.cs"
    Inherits="PraticeManagement.Controls.Generic.Filtering.PersonReportFilter" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<div>
    <table width="500px">
        <tr>
            <td align="left">
                <cc2:ScrollingDropDown ID="cblPersons" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                    BackColor="White" CellPadding="3" NoItemsType="All" SetDirty="False" Width="350px"
                    BorderWidth="0" />
                <ext:ScrollableDropdownExtender ID="sdePersons" runat="server" TargetControlID="cblPersons"
                    DisplayText="Please Choose Persons" EditImageUrl="~/Images/Dropdown_Arrow.png">
                </ext:ScrollableDropdownExtender>
            </td>
            <td>
                <asp:CheckBox ID="cbInActive" runat="server" Text="Inactive" OnCheckedChanged="status_OnCheckedChanged"
                    AutoPostBack="true" />
            </td>
            <td>
                <asp:CheckBox ID="cbProjected" runat="server" Text="Projected" OnCheckedChanged="status_OnCheckedChanged"
                    AutoPostBack="true" />
            </td>
            <td>
                <asp:CheckBox ID="cbTerminated" runat="server" Text="Terminated" OnCheckedChanged="status_OnCheckedChanged"
                    AutoPostBack="true" />
            </td>
        </tr>
    </table>
    <%--<asp:ObjectDataSource ID="odsPersons" runat="server" TypeName="PraticeManagement.PersonService.PersonServiceClient"
        SelectMethod="PersonistByStatusList" OnSelecting="odsPersons_Selecting">
        <SelectParameters>
            <asp:Parameter Name="PersonStatusIdsList" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>--%>
</div>

