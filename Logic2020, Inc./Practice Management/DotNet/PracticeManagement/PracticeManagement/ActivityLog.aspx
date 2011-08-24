<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ActivityLog.aspx.cs" Inherits="PraticeManagement.Config.ActivityLog" %>

<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Activity Log</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Activity Log
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
<script type="text/javascript" language="javascript">
    function SetTooltipsForallDropDowns() {
        var optionList = document.getElementsByTagName('option');

        for (var i = 0; i < optionList.length; ++i) {

            optionList[i].title = optionList[i].innerHTML;
        }

    }

</script>
    <uc:ActivityLogControl runat="server" ID="activityLog"  IsActivityLogPage="true" />
</asp:Content>

