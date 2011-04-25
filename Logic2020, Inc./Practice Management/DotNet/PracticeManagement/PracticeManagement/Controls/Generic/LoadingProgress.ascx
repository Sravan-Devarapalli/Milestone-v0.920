<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoadingProgress.ascx.cs"
    Inherits="PraticeManagement.Controls.Generic.LoadingProgress" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic" TagPrefix="pcg" %>
<pcg:StyledUpdateProgress ID="upTimeEntries" runat="server">
    <ProgressTemplate>
        <div class="please-wait-holder ToolTip" style="display: block;">
            <table>
                <tr class="top">
                    <td class="lt">
                    </td>
                    <td class="tbor">
                    </td>
                    <td class="rt">
                    </td>
                </tr>
                <tr class="middle">
                    <td class="lbor">
                    </td>
                    <td class="content">
                        <div id="divWait">
                            <span style="color: Black; font-weight: bold;">
                                <nobr><% = DisplayText %></nobr>
                            </span>
                            <br />
                            <br />
                            <asp:Image ID="img" runat="server" ImageUrl="~/Images/loading.gif" />
                        </div>
                    </td>
                    <td class="rbor">
                    </td>
                </tr>
                <tr class="bottom">
                    <td class="lb">
                    </td>
                    <td class="bbor">
                    </td>
                    <td class="rb">
                    </td>
                </tr>
            </table>
        </div>
    </ProgressTemplate>
</pcg:StyledUpdateProgress>

