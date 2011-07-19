<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ProjectCalendar.aspx.cs" Inherits="PraticeManagement.ProjectCalendar" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Project Calendar</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Calendar view
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
     <style type="text/css">
        .FadingTooltip
        {
            border-right: darkgray 1px outset;
            border-top: darkgray 1px outset;
            font-size: 10px;
            border-left: darkgray 1px outset;
            width: auto;
            color: black;
            border-bottom: darkgray 1px outset;
            height: auto;
            background-color: lemonchiffon;
            borderbottomwidths: "3,3,3,3";
        }
    </style>
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var FADINGTOOLTIP
        var wnd_height, wnd_width;
        var tooltip_height, tooltip_width;
        var tooltip_shown = false;
        var transparency = 100;
        var timer_id = 1;
        var tooltiptext;

        // override events
        window.onload = WindowLoading;
        window.onresize = UpdateWindowSize;
        document.onmousemove = AdjustToolTipPosition;

        function DisplayTooltip(tooltip_text) {
            FADINGTOOLTIP.innerHTML = tooltip_text;
            tooltip_shown = (tooltip_text != "") ? true : false;
            if (tooltip_text != "") {
                // Get tooltip window height
                tooltip_height = (FADINGTOOLTIP.style.pixelHeight) ? FADINGTOOLTIP.style.pixelHeight : FADINGTOOLTIP.offsetHeight;
                transparency = 0;
                ToolTipFading();
            }
            else {
                clearTimeout(timer_id);
                FADINGTOOLTIP.style.visibility = "hidden";
            }
        }

        function AdjustToolTipPosition(e) {
            if (tooltip_shown) {
                e = e || window.event;

                FADINGTOOLTIP.style.visibility = "visible";
                setPosition($(FADINGTOOLTIP), getPosition(e).y, getPosition(e).x + 20)
            }
        }

        function setPosition(item, ytop, xleft) {
            item.offset({ top: ytop, left: xleft });
        }

        function getPosition(e) {
            var cursor = { x: 0, y: 0 };
            if (e.pageX || e.pageY) {
                cursor.x = e.pageX;
                cursor.y = e.pageY;
            }
            else {
                var de = document.documentElement;
                var b = document.body;

                cursor.x = e.clientX +
            (de.scrollLeft || b.scrollLeft) - (de.clientLeft || 0);
                cursor.y = e.clientY +
            (de.scrollTop || b.scrollTop) - (de.clientTop || 0);
            }
            return cursor;
        }



        function WindowLoading() {
            FADINGTOOLTIP = document.getElementById('FADINGTOOLTIP');

            // Get tooltip  window width				
            tooltip_width = (FADINGTOOLTIP.style.pixelWidth) ? FADINGTOOLTIP.style.pixelWidth : FADINGTOOLTIP.offsetWidth;

            // Get tooltip window height
            tooltip_height = (FADINGTOOLTIP.style.pixelHeight) ? FADINGTOOLTIP.style.pixelHeight : FADINGTOOLTIP.offsetHeight;

            UpdateWindowSize();
        }

        function ToolTipFading() {
            if (transparency <= 100) {
                FADINGTOOLTIP.style.filter = "alpha(opacity=" + transparency + ")";
                transparency += 5;
                timer_id = setTimeout('ToolTipFading()', 35);
            }
        }

        function UpdateWindowSize() {
            wnd_height = document.body.clientHeight;
            wnd_width = document.body.clientWidth;
        }

    </script>
    <div class="FadingTooltip" id="FADINGTOOLTIP" style="z-index: 999; padding: 5px;
        visibility: hidden; position: absolute">
    </div>

    <asp:UpdatePanel ID="updProjectDetails" runat="server">
        <ContentTemplate>
            <div style="text-align: center;">
                <table cellpadding="0" cellspacing="0" align="center" style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">
                            <asp:ImageButton ID="imgbtnPrevious" runat="server" ImageUrl="~/Images/previous.gif"
                                OnClick="imgbtnNavigateRange_Click" ToolTip="Previous"  />
                        </td>
                        <td style="width: 140px; text-align: center">
                            <asp:Label ID="lblRange" runat="server" Style="vertical-align: top;font-weight:bold;"></asp:Label>
                        </td>
                        <td style="text-align: left;">
                            <asp:ImageButton ID="imgbtnNext" runat="server"  ToolTip="Next" ImageUrl="~/Images/next.gif" OnClick="imgbtnNavigateRange_Click" />
                        </td>
                    </tr>
                </table>
                <table cellpadding="0" cellspacing="0" width="100%" runat="server" id="tblDetails">
                    <tr>
                        <td align="center" style="background-color: White;">
                            <asp:Chart ID="chartProjectDetails" Width="800px" runat="server" Visible="false">
                                <Series>
                                    <asp:Series Name="Projects" ChartType="RangeBar" XValueMember="Description" YValueMembers="StartDate,EndDate">
                                    </asp:Series>
                                </Series>
                                <Titles>
                                    <asp:Title Name="ProjectsTitle" />
                                </Titles>
                                <ChartAreas>
                                    <asp:ChartArea Name="ProjectsArea">
                                        <AxisY IsLabelAutoFit="true" IsStartedFromZero="true" Enabled="True" LineDashStyle="NotSet">
                                            <MajorGrid LineColor="DimGray" LineDashStyle="Dash" />
                                            <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                                            <LabelStyle Format="MMM, d, yyyy" />
                                        </AxisY>
                                        <AxisY2 IsLabelAutoFit="true" IsStartedFromZero="true" Enabled="True" LineDashStyle="NotSet">
                                            <MajorGrid LineColor="DimGray" LineDashStyle="Dash" />
                                            <MinorGrid Enabled="True" LineColor="Silver" LineDashStyle="Dot" />
                                            <LabelStyle Format="MMM, d, yyyy" />
                                        </AxisY2>
                                        <AxisX IsLabelAutoFit="false">
                                            <MajorGrid Interval="1" LineDashStyle="Dot" LineColor="Silver" />
                                            <MajorTickMark Enabled="False" />
                                            <LabelStyle Enabled="false" />
                                        </AxisX>
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <table>
        <tr>
            <td style="margin-top: 5px; padding-top: 5px; padding-right: 5px;">
                <table bgcolor="black" border="1" cellpadding="0" cellspacing="0" width="20">
                    <tr>
                        <td style="background-color: white; border: 1px solid black;">
                            <div style="background-color: rgb(142,213,55); width: 20px; height: 20px;">
                                &nbsp;</div>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="margin-top: 5px; padding-top: 5px;">
                - Active Project with attachment.
            </td>
        </tr>
        <tr>
            <td style="margin-top: 5px; padding-top: 5px; padding-right: 5px;">
                <table bgcolor="black" border="1" cellpadding="0" cellspacing="0" width="20">
                    <tr>
                        <td style="background-color: white; border: 1px solid black;">
                            <div style="background-color: rgb(142,213,55); width: 10px; height: 20px;">
                                &nbsp;</div>
                        </td>
                        <td style="background-color: white; border: 1px solid black;">
                            <div style="background-color: rgb(217,211,68); width: 10px; height: 20px;">
                                &nbsp;</div>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="margin-top: 5px; padding-top: 5px;">
                - Project without attachment.
            </td>
        </tr>
        <tr>
            <td style="margin-top: 5px; padding-top: 5px; padding-right: 5px;">
                <table bgcolor="black" border="1" cellpadding="0" cellspacing="0" width="20">
                    <tr>
                        <td style="background-color: white; border: 1px solid black;">
                            <div style="background-color: rgb(217,211,68); width: 20px; height: 20px; '">
                                &nbsp;</div>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="margin-top: 5px; padding-top: 5px;">
                - Projected Project.
            </td>
        </tr>
    </table>
</asp:Content>

