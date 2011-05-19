<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonsFilter.ascx.cs" Inherits="PraticeManagement.Controls.PersonsFilter" %>


<table>
    <tr style="text-align: center;">
        <td style="padding-right: 5px; border-bottom:1px solid black;">
            <span>Person Status</span>
        </td>
        <td style="width:15px" ></td>
        <td style="padding-right: 5px; border-bottom:1px solid black;">
            <span>Pay Type</span>
        </td>
        <td style="width:15px" ></td>
        <td style="padding-left: 5px; border-bottom:1px solid black;">
            <span>Practice</span>
        </td>
        <td style="width:15px" ></td>
    </tr>
    <tr>
        <td style="padding-right: 5px;">
            <table>
                <tr>
                    <td>
                        <asp:CheckBox ID="chbShowActive" runat="server" AutoPostBack="false" TextAlign="Left"
                            Checked="true" OnCheckedChanged="chbShowActive_CheckedChanged" />
                        <span style="padding-right: 5px">Active</span>
                    </td>
                    <td style="padding-left: 20px;">
                        <asp:CheckBox ID="chbProjected" runat="server" />
                        <span style="padding-right: 5px">Projected</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chbInactive" runat="server" />
                        <span style="padding-right: 5px">Inactive</span>
                    </td>
                    <td style="padding-left: 20px;">
                        <asp:CheckBox ID="chbTerminated" runat="server" />
                        <span style="padding-right: 5px">Terminated</span>
                    </td>
                </tr>
            </table>
        </td>
        <td style="width:15px" ></td>
        <td rowspan="2" style="padding-right: 5px;">
            <asp:DropDownList ID="ddlPayType" Width="150px" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ddlPayType_SelectedIndexChanged">
            </asp:DropDownList>
        </td>
        <td style="width:15px" ></td>
        <td rowspan="2" style="padding-right: 5px;">
            <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged" />
        </td>
        <td style="width:15px" ></td>
    </tr>
</table>

