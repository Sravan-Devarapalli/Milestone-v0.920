<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConsultantUtilTimeLineReport.aspx.cs"
    Inherits="PraticeManagement.Reporting.ConsultantUtilTimeLineReport" %>

<%@ Register Src="~/Controls/Reports/ConsultantsWeeklyReport.ascx" TagPrefix="uc"
    TagName="ConsultantsWeeklyReport" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>Consulting Utilization Report | Practice Management</title>
    <style type="text/css">
        .tab-invisible
        {
            display: none;
        }
    </style>
    <script src="<%# Generic.GetClientUrl("~/Scripts/jquery-1.4.1.min.js", this) %>"
        type="text/javascript"></script>
</head>
<body>
    <form id="form2" runat="server">
    <AjaxControlToolkit:ToolkitScriptManager ID="scriptManager" runat="server" LoadScriptsBeforeUI="true"
        CombineScripts="False" EnablePageMethods="True" ScriptMode="Release" EnableScriptLocalization="False">
        <Scripts>
            <asp:ScriptReference Name="MicrosoftAjax.js" Path="~/Scripts/MicrosoftAjax.min.js" />
        </Scripts>
    </AjaxControlToolkit:ToolkitScriptManager>
    <uc:ConsultantsWeeklyReport ID="repWeeklyReport" IsSampleReport="true" runat="server" />
    </form>
</body>
</html>

