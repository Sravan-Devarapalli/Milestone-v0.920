<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ActivityLog.aspx.cs" Inherits="PraticeManagement.Config.ActivityLog" %>

<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Activity Log | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Activity Log
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
    function SetTooltipsForallDropDowns() {
        var optionList = document.getElementsByTagName('option');

        for (var i = 0; i < optionList.length; ++i) {

            optionList[i].title = optionList[i].innerHTML;
        }

    }
     
        
        function SetWrapText(str) {
            for (var i = 30; i < str.length; i = i + 10) {
                str = str.slice(0, i) + "<wbr/>" + str.slice(i, str.length);
            }
            return str;
        }

    function GetWrappedText(childObj) {
        if (childObj != null) {

            for (var i = 0; i < childObj.children.length; i++) {
                if (childObj.children[i] != null) {

                    if (childObj.children[i].innerHTML != null && childObj.children[i].innerHTML != "undefined" && childObj.children[i].innerHTML.length > 70) {
                        childObj.children[i].innerHTML = SetWrapText(childObj.children[i].innerHTML);
                    }
                }

            }
        }
    }

    function ModifyInnerTextToWrapText() {
        var tbl = $("table[id*='gvActivities']");
        if(tbl != null && tbl.length>0)
        {
                var gvActivitiesclientId = tbl[0].id;
                var lastTds = $('#' + gvActivitiesclientId + ' tr td:nth-child(3)');

                for (var i = 0; i < lastTds.length; i++) {
                    GetWrappedText(lastTds[i]);
                }
        }
    }
         
         Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
    function endRequestHandle(sender, Args) {
        ModifyInnerTextToWrapText();
    }

    </script>
    <uc:ActivityLogControl runat="server" ID="activityLog" IsActivityLogPage="true" />
</asp:Content>

