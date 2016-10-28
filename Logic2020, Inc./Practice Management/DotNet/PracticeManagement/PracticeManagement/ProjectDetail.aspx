<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="True"
    CodeBehind="ProjectDetail.aspx.cs" Inherits="PraticeManagement.ProjectDetail"
    Title="Project Details | Practice Management" EnableEventValidation="false" ValidateRequest="False" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Projects/ProjectActivityLog.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register TagPrefix="extDisable" Namespace="PraticeManagement.Controls.Generic.ElementDisabler"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectMilestonesFinancials.ascx"
    TagName="ProjectMilestonesFinancials" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectTimeTypes.ascx" TagName="ProjectTimeTypes" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/CommissionsAttribution.ascx"
    TagName="ProjectAttribution" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectFeedback.ascx" TagName="ProjectFeedback" %>
<%@ Register Src="Controls/ProjectExpenses/ProjectExpensesControl.ascx" TagName="ProjectExpenses"
    TagPrefix="uc2" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectFinancials.ascx" TagName="ProjectFinancials" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectPersons.ascx" TagName="ProjectPersons" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectCSAT.ascx" TagName="ProjectCSAT" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Project Details | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script src="Scripts/FilterTable.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.uploadify.min.js?Id=21" type="text/javascript"></script>
    <uc:LoadingProgress ID="loadingProgress" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                var currentSort = [[1, 0]];
                $(document).ready(function () {
                    $("#tblMilestones").tablesorter({
                        headers: {
                            0: {
                                sorter: false
                            }
                        },
                        sortList: currentSort,
                        sortForce: [[1, 0]]
                    }).bind("sortEnd", function (sorter) {
                        currentSort = sorter.target.config.sortList;
                        var spanName = $("#tblMilestones #name");
                        if (currentSort != '1,0' && currentSort != '1,1') {
                            spanName[0].setAttribute('class', 'backGroundNone');
                        }
                        else {
                            spanName[0].setAttribute('class', '');
                        }
                    });
                });
                var fileError = 0;
                function pageLoad() {

                    document.onkeypress = enterPressed;
                    $("#<%=fuAttachmentsUpload.ClientID%>").fileUpload({
                        'uploader': 'Scripts/uploaderRemovedFolder1.swf',
                        'cancelImg': 'Images/close_16.png',
                        'buttonText': 'Browse File(s)',
                        'script': 'Controls/Projects/AttachmentUpload.ashx',
                        'fileExt': '*.xls;*.xlsx;*.xlw;*.doc;*.docx;*.pdf;*.ppt;*.pptx;*.mpp;*.vsd;*.msg;*.ZIP;*.RAR;*.sig;*.one*',
                        'fileDesc': 'Excel;Word doc;PDF;PowerPoint;MS Project;Visio;Exchange;ZIP;RAR;OneNote',
                        'multi': true,
                        'auto': false,
                        'allowSpecialChars': false,
                        'ValidChars': '._',
                        'sizeLimit': 4294656, //4194kb - 4294656bytes
                        onComplete: function (event, queueID, fileObj, reponse, data) {
                            var div = document.getElementById('<%= uploadedFiles.ClientID%>');
                            var lblUplodedFilesMsg = document.getElementById('<%= lblUplodedFilesMsg.ClientID%>');
                            if (lblUplodedFilesMsg.getAttribute("class") == "displayNone") {
                                lblUplodedFilesMsg.setAttribute("class", "fontBold");
                                div.appendChild(document.createElement("br"));
                            }
                            div.appendChild(document.createTextNode("- " + fileObj.name));
                            div.appendChild(document.createElement("br"));

                            var queueItem = document.getElementById('<%= fuAttachmentsUpload.ClientID %>' + queueID);
                            queueItem.outerHTML = '';
                        },
                        onAllComplete: function (event, queueID, fileObj, response, data) {
                            var uploadButton = document.getElementById('<%= btnUpload.ClientID %>');
                            uploadButton.disabled = "disabled";
                            var progressBar = document.getElementById('<%= loadingProgress.ClientID %>_upTimeEntries');
                            progressBar.setAttribute('style', 'display:none;');
                            if (fileError == 0) {
                                var btnCancel = document.getElementById('<%= btnCancel.ClientID %>');
                                btnCancel.click();
                            }
                        },
                        onError: function (event, queueID, fileObj, errorObj) {
                            fileError++;
                            var elementId = '<%= fuAttachmentsUpload.ClientID %>' + queueID;
                            var queueItem = document.getElementById(elementId);
                            var imgElement = queueItem.firstChild.firstChild;
                            imgElement.setAttribute("onclick", "javascript:(document.getElementById('" + elementId + "')).outerHTML= ''; fileError--; EnableUploadButton();");
                        },
                        onSelectOnce: function () {
                            EnableUploadButton(true);
                        },
                        onCancelComplete: function () {
                            EnableUploadButton();
                        },
                        onErrorComplete: function () {
                            if (!IsQueueContainValidFiles()) {
                                var progressBar = document.getElementById('<%= loadingProgress.ClientID %>_upTimeEntries');
                                progressBar.setAttribute('style', 'display:none;');
                            }

                            EnableUploadButton();
                        }
                    });
                }

                function ChangeCancelDivInnerHTML() {
                    var cancelDiv = $('.fileUploadQueueItem .cancel');
                    for (i = 0; i < cancelDiv.length; i++) {
                        var anchorTags = cancelDiv[i].firstChild;
                        var queueItemId = cancelDiv[i].parentElement.id;

                        var imgElement = document.createElement('Img');
                        imgElement.setAttribute("src", "Images/close_16.png");
                        imgElement.setAttribute("class", "CursorPointer");
                        cancelDiv[i].innerHTML = "";
                        cancelDiv[i].appendChild(imgElement);

                    }
                }

                function ClearVariables() {
                    fileError = 0;
                }

                function startUpload() {
                    var progressBar = document.getElementById('<%= loadingProgress.ClientID %>_upTimeEntries');
                    progressBar.setAttribute('style', '');
                    ChangeCancelDivInnerHTML();
                    var ddlAttachmentCategory = document.getElementById('<%= ddlAttachmentCategory.ClientID %>');
                    var selectedValue = ddlAttachmentCategory.value;
                    var hdnProjectId = document.getElementById('<%= hdnProjectId.ClientID %>');

                    $("#<%=fuAttachmentsUpload.ClientID%>").fileUploadSettings('scriptData', 'ProjectId=' + hdnProjectId.value + '&categoryId=' + selectedValue + '&LoggedInUser=<%= User.Identity.Name %>');
                    $("#<%=fuAttachmentsUpload.ClientID%>").fileUploadStart();
                }

                function EnableUploadButton(selected) {
                    var categorySelected = IsAttachmentCategorySelected();
                    var fileSelected = (selected == true ? true : false);
                    if (categorySelected && !fileSelected) {
                        fileSelected = IsQueueContainValidFiles();
                    }

                    var uploadButton = document.getElementById('<%= btnUpload.ClientID %>');
                    uploadButton.disabled = categorySelected && fileSelected ? "" : "disabled";
                }

                function IsQueueContainValidFiles() {
                    var fileUploadQueue = $('.fileUploadQueueItem');
                    if (fileUploadQueue.length > 0) {
                        return true;
                    }
                    return false;
                }

                function DownloadUnsavedFile(linkButton) {
                    if (linkButton != null) {
                        var lnkbutton = $('#' + linkButton.id)[0];
                        var attachmentId = lnkbutton.getAttribute('attachmentid');
                        var btn = document.getElementById('<%= btnDownloadButton.ClientID %>');
                        var hdn = document.getElementById('<%= hdnDownloadAttachmentId.ClientID %>');
                        hdn.value = attachmentId;
                        btn.click();
                    }
                }

                function enterPressed(evn) {
                    if (window.event && window.event.keyCode == 13) {
                        if (window.event.srcElement.tagName != "TEXTAREA") {
                            return false;
                        }
                    } else if (evn && evn.keyCode == 13) {
                        if (evn.originalTarget.type != "textarea") {
                            return false;
                        }
                    }
                }

                function checkDirty(target, entityId) {
                    if (showDialod()) {
                        __doPostBack('__Page', target + ':' + entityId);
                        return false;
                    }
                    return true;
                }


                function ConfirmSaveOrExit() {
                    var hdnProjectId = document.getElementById('<%= hdnProjectId.ClientID %>');
                    if (getDirty() || hdnProjectId.value == "") {
                        return confirm("Some data isn't saved. Click Ok to Save the data or Cancel to exit.");
                    }
                    return true;
                }


                function cvProjectAttachment_ClientValidationFunction(obj, args) {
                    args.IsValid = IsValidProjectAttachMent();
                }

                function cvAttachmentCategory_ClientValidationFunction(obj, args) {
                    args.IsValid = IsAttachmentCategorySelected();
                }

                function IsAttachmentCategorySelected() {
                    var ddlAttachmentCategory = document.getElementById('<%= ddlAttachmentCategory.ClientID %>');
                    var categoryValue = ddlAttachmentCategory.value;
                    if (categoryValue != "0") {
                        return true; // Valid
                    }
                    else {
                        return false; // Not valid
                    }
                }

                function CanShowPrompt() {
                    return true;
                }

                function ConfirmToDeleteProject() {
                    var hdnProject = document.getElementById('<%= hdnProjectDelete.ClientID %>');
                    var result = confirm("Do you really want to delete the project? The deletion cannot be undone");
                    hdnProject.value = result ? 1 : 0;
                }

                function SetTooltipsForallDropDowns() {
                    var optionList = document.getElementsByTagName('option');

                    for (var i = 0; i < optionList.length; ++i) {

                        optionList[i].title = optionList[i].text;
                    }

                }

                function SetWrapText(str) {
                    for (var i = 30; i < str.length; i = i + 10) {
                        str = str.slice(0, i) + "<wbr />" + str.slice(i, str.length);
                    }
                    return str;
                }

                function GetWrappedText(childObj) {
                    if (childObj != null) {

                        for (var i = 0; i < childObj.children.length; i++) {
                            if (childObj.children[i] != null) {
                                if (childObj.children[i].children.length == 0) {
                                    if (childObj.children[i].innerHTML != null && childObj.children[i].innerHTML != "undefined" && childObj.children[i].innerHTML.length > 70) {
                                        childObj.children[i].innerHTML = SetWrapText(childObj.children[i].innerHTML);
                                    }
                                }
                            }

                        }
                    }
                }

                function ModifyInnerTextToWrapText() {
                    if (navigator.userAgent.indexOf(" Firefox/") > -1) {
                        var tbl = $("table[id*='gvActivities']");
                        if (tbl != null && tbl.length > 0) {
                            var gvActivitiesclientId = tbl[0].id;
                            var lastTds = $('#' + gvActivitiesclientId + ' tr td:nth-child(3)');

                            for (var i = 0; i < lastTds.length; i++) {
                                GetWrappedText(lastTds[i]);
                            }
                        }
                    }
                }

                function CheckIfDatesValid() {
                    txtStartDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbFrom');
                    txtEndDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbTo');
                    var startDate = new Date(txtStartDate.value);
                    var endDate = new Date(txtEndDate.value);
                    if (txtStartDate.value != '' && txtEndDate.value != ''
                        && startDate <= endDate) {
                        var btnCustDatesClose = document.getElementById('<%= activityLog.ClientID %>_btnCustDatesClose');
                        hdnStartDate = document.getElementById('<%= activityLog.ClientID %>_hdnStartDate');
                        hdnEndDate = document.getElementById('<%= activityLog.ClientID %>_hdnEndDate');
                        lblCustomDateRange = document.getElementById('<%= activityLog.ClientID %>_lblCustomDateRange');
                        var startDate = new Date(txtStartDate.value);
                        var endDate = new Date(txtEndDate.value);
                        var startDateStr = startDate.format("MM/dd/yyyy");
                        var endDateStr = endDate.format("MM/dd/yyyy");
                        hdnStartDate.value = startDateStr;
                        hdnEndDate.value = endDateStr;
                        lblCustomDateRange.innerHTML = '(' + startDateStr + '&nbsp;-&nbsp;' + endDateStr + ')';
                        btnCustDatesClose.click();

                    }
                    return false;
                }

                function CheckAndShowCustomDatesPoup(ddlPeriod) {
                    imgCalender = document.getElementById('<%= activityLog.ClientID %>_imgCalender');
                    lblCustomDateRange = document.getElementById('<%= activityLog.ClientID %>_lblCustomDateRange');
                    if (ddlPeriod.value == '0') {
                        imgCalender.attributes["class"].value = "";
                        lblCustomDateRange.attributes["class"].value = "fontBold";
                        if (imgCalender.fireEvent) {
                            imgCalender.style.display = "";
                            lblCustomDateRange.style.display = "";
                            imgCalender.click();
                        }
                        if (document.createEvent) {
                            var event = document.createEvent('HTMLEvents');
                            event.initEvent('click', true, true);
                            imgCalender.dispatchEvent(event);
                        }
                    }
                    else {
                        imgCalender.attributes["class"].value = "displayNone";
                        lblCustomDateRange.attributes["class"].value = "displayNone";
                        if (imgCalender.fireEvent) {
                            imgCalender.style.display = "none";
                            lblCustomDateRange.style.display = "none";
                        }
                    }
                }

                function ReAssignStartDateEndDates() {
                    hdnStartDate = document.getElementById('<%= activityLog.ClientID %>_hdnStartDate');
                    hdnEndDate = document.getElementById('<%= activityLog.ClientID %>_hdnEndDate');
                    txtStartDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbFrom');
                    txtEndDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbTo');
                    hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= activityLog.ClientID %>_hdnStartDateCalExtenderBehaviourId');
                    hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= activityLog.ClientID %>_hdnEndDateCalExtenderBehaviourId');

                    var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
                    var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
                    if (startDateCalExtender != null) {
                        startDateCalExtender.set_selectedDate(hdnStartDate.value);
                    }
                    if (endDateCalExtender != null) {
                        endDateCalExtender.set_selectedDate(hdnEndDate.value);
                    }
                    CheckIfDatesValid();
                }

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

                function endRequestHandle(sender, Args) {
                    SetTooltipsForallDropDowns();
                    $("#tblProjectAttachments").tablesorter(
                    {
                        sortList: [[0, 0]]
                    });
                    $("#tblMilestones").tablesorter({
                        headers: {
                            0: {
                                sorter: false
                            }
                        },
                        sortList: [[1, 0]],
                        sortForce: [[1, 0]]
                    }).bind("sortEnd", function (sorter) {
                        currentSort = sorter.target.config.sortList;
                        var spanName = $("#tblMilestones #name");
                        if (currentSort != '1,0' && currentSort != '1,1') {
                            spanName[0].setAttribute('class', 'backGroundNone');
                        }
                        else {
                            spanName[0].setAttribute('class', '');
                        }
                    });
                    var activityLog = document.getElementById('<%= activityLog.ClientID%>');
                    if (activityLog != null) {
                        imgCalender = document.getElementById('<%= activityLog.ClientID %>_imgCalender');
                        lblCustomDateRange = document.getElementById('<%= activityLog.ClientID %>_lblCustomDateRange');
                        ddlPeriod = document.getElementById('<%=  activityLog.ClientID %>_ddlPeriod');
                        if (imgCalender.fireEvent && ddlPeriod.value != '0') {
                            imgCalender.style.display = "none";
                            lblCustomDateRange.style.display = "none";
                        }
                    }

                }

                //number = 1 to show, 2 to close
                function showProjectNumberTextBox(number) {
                    var projectNumber = document.getElementById('<%= tblSetProjectNumber.ClientID%>');
                    var HiddenField = document.getElementById('<%= hdnVisibilty.ClientID%>');
                    if (number == 1) {
                        projectNumber.rows[0].cells[1].children[0].setAttribute('style', 'display:inline;');
                        HiddenField.value = 'inline';
                    }
                    else {
                        projectNumber.rows[0].cells[1].children[0].setAttribute('style', 'display:none;');
                        HiddenField.value = 'none';
                    }
                }

                //region project time types script

                function btnClose_OnClientClick() {
                    $find("mpeTimetypeAlertMessage").hide();
                    return false;
                }

                function DeleteWorkType(timetypeid) {

                    if (confirm("Are you sure you want to delete this Work Type?")) {
                        var btnDeleteWorkType = document.getElementById('<%= btnDeleteWorkType.ClientID%>');
                        var hdnWorkTypeId = document.getElementById('<%= hdnWorkTypeId.ClientID%>');
                        hdnWorkTypeId.value = timetypeid;
                        btnDeleteWorkType.click();
                    }

                    return false;
                }

                // End Region projecttimetypes script

                $(document).ready(function () {
                    SetTooltipsForallDropDowns();
                    $('script #tableSorterScript').load(function () {
                        $("#tblProjectAttachments").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
                    });
                });

                function mailTo(url) {

                    var mailtoHiddenLink = document.getElementById('mailtoHiddenLink');
                    mailtoHiddenLink.href = url;
                    mailtoHiddenLink.click();
                    return false;
                }


                function RedirectToProject() {
                    var imgRedirectLink = document.getElementById('<%= imgNavigateToProject.ClientID %>');
                    var url = imgRedirectLink.getAttribute("NavigateUrl");

                    window.open(url);
                }



            </script>
            <asp:Table ID="tblProjectTypeViewSwitch" runat="server" CssClass="CustomTabStyle CustomTabStyleProjectDetail">
                <asp:TableRow ID="ProjectTypeSitch" runat="server">
                    <asp:TableCell ID="CellExternal" runat="server" CssClass="SelectedSwitch">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnExternalPRoject" runat="server" Text="External Project" CausesValidation="false"
                                OnCommand="btnProjectView_Command" CommandArgument="0"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="CellInternal" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnInternalProject" runat="server" Text="Internal Project" CausesValidation="false"
                                OnCommand="btnProjectView_Command" CommandArgument="1"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:MultiView ID="mvProjectType" runat="server" ActiveViewIndex="0">
                <asp:View ID="vwExternal" runat="server">
                    <br />
                    <table class="WholeWidth">
                        <tr>
                            <td class="Width2Percent">
                                <a id="mailtoHiddenLink" class="displayNone"></a>
                            </td>
                            <td>
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="width30P">
                                            <asp:TextBox ID="txtProjectNameFirstTime" CssClass="Width500PxImp" runat="server"
                                                Visible="false"></asp:TextBox>
                                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="txtweProjectNameFirstTime" runat="server"
                                                TargetControlID="txtProjectNameFirstTime" WatermarkText="Enter a Project Name here..."
                                                EnableViewState="false" WatermarkCssClass="watermarkedtext Width500PxImp" />
                                            <asp:Label ID="lblProjectNumber" runat="server" CssClass="LabelProject"></asp:Label>
                                            <asp:Label ID="lblProjectName" runat="server" CssClass="LabelProject"></asp:Label>
                                            <asp:Label ID="lblProjectRange" runat="server" CssClass="LabelProject"></asp:Label>
                                            <asp:Image ID="imgEditProjectName" ToolTip="Edit Project Name" ImageUrl="~/Images/icon-edit.png"
                                                runat="server" />
                                            <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="This field contains the name of the project.&#13;&#10;The name of the project should follow standard naming convention and match the opportunity name in CRM when possible.">
                                                *</a>
                                            <asp:CustomValidator ID="cvProjectName" runat="server" ErrorMessage="The Project Name is required."
                                                ToolTip="The Project Name is required." ValidationGroup="Project" Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Dynamic" OnServerValidate="cvProjectName_ServerValidate"></asp:CustomValidator>
                                            <AjaxControlToolkit:ModalPopupExtender ID="mpeEditProjectName" runat="server" TargetControlID="imgEditProjectName"
                                                CancelControlID="btncloseEditProjectName" BehaviorID="mpeEditProjectName" BackgroundCssClass="modalBackground"
                                                PopupControlID="pnlProjectName" DropShadow="false" />
                                        </td>
                                        <td class="Width35Percent padLeft4Per">
                                            <table id="tblSetProjectNumber" runat="server">
                                                <tr>
                                                    <td>
                                                        <asp:RadioButton ID="rbAutoGenerate" runat="server" GroupName="ProjectNumber" Text="Auto generate project number"
                                                            Checked="true" onchange="showProjectNumberTextBox(2)" />
                                                        <asp:RadioButton ID="rbOwnNumber" runat="server" GroupName="ProjectNumber" Text="Set own project number"
                                                            onchange="showProjectNumberTextBox(1)" />
                                                    </td>
                                                    <td class="padLeft5">
                                                        <asp:TextBox ID="txtProjectNumber" runat="server" Style="display: none"></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:CustomValidator ID="custProjectNumberRequired" runat="server" ErrorMessage="The project number is required."
                                                            ToolTip="The project number is required." ValidationGroup="Project" Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" OnServerValidate="custProjectNumberRequired_ServerValidate"></asp:CustomValidator>
                                                        <asp:CustomValidator ID="custNumberExistsInSystem" runat="server" ErrorMessage="The project number already exists in the system."
                                                            ToolTip="The project number already exists in the system." ValidationGroup="Project"
                                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                            OnServerValidate="custNumberExistsInSystem_ServerValidate"></asp:CustomValidator>
                                                        <asp:CustomValidator ID="custFormat" runat="server" ErrorMessage="Project number should be in the format P + 6 Digits or P + 6 Digits + 1 Alphabet."
                                                            ToolTip="Project number should be in the format P + 6 Digits or P + 6 Digits + 1 Alphabet."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic" OnServerValidate="custFormat_ServerValidate"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width35Percent">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="LabelProject TdProjectStatus">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    Project Status
                                                                </td>
                                                                <td>
                                                                    <div id="divStatus" class="PaddingLeft3Px" runat="server">
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td class="DdlProjectStatus">
                                                        <asp:DropDownList ID="ddlProjectStatus" runat="server" onchange="setDirty();" AutoPostBack="True"
                                                            CssClass="WholeWidth" OnSelectedIndexChanged="DropDown_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="TdValidators">
                                                        <asp:RequiredFieldValidator ID="reqProjectStatus" runat="server" ControlToValidate="ddlProjectStatus"
                                                            ErrorMessage="The Project Status is required." ToolTip="The Project Status is required."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="custProjectStatus" runat="server" ControlToValidate="ddlProjectStatus"
                                                            ErrorMessage="Only system administrators can make projects Active or Completed."
                                                            ToolTip="Only system administrators can make projects Active or Completed." ValidationGroup="Project"
                                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                            OnServerValidate="custProjectStatus_ServerValidate"></asp:CustomValidator>
                                                        <asp:CustomValidator ID="custAtRiskStatus" runat="server" ControlToValidate="ddlProjectStatus"
                                                            ErrorMessage="Only system administrators, Operations can make projects At Risk."
                                                            ToolTip="Only system administrators, Operations can make projects At Risk." ValidationGroup="Project"
                                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                            OnServerValidate="custAtRiskStatus_ServerValidate"></asp:CustomValidator>
                                                        <asp:CustomValidator ID="cvIsInternal" runat="server" EnableClientScript="false"
                                                            ErrorMessage="Can not change project status as some work types are already in use."
                                                            ValidateEmptyText="true" Text="*" ToolTip="Can not change project status as some timetypes are already in use."></asp:CustomValidator>
                                                        <asp:CustomValidator ID="custActiveStatus" runat="server" ControlToValidate="ddlProjectStatus"
                                                            ErrorMessage="Project should have milestone added to set status to Active." ToolTip="Project should have milestone added to set status to Active."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic" OnServerValidate="custActiveStatus_ServerValidate"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width2Percent">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="Width2Percent">
                            </td>
                            <td>
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Account
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:DropDownList ID="ddlClientName" runat="server" OnSelectedIndexChanged="ddlClientName_SelectedIndexChanged"
                                                            CssClass="Width94Per" AutoPostBack="True" onchange="setDirty();">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width10Percent">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="This field contains the name of company we are completing the project for.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqClientName" runat="server" ControlToValidate="ddlClientName"
                                                            ErrorMessage="The Account Name is required." ToolTip="The Account Name is required."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="cvClient" runat="server" ErrorMessage="The project's account designation cannot be changed as time has been entered towards the project."
                                                            ToolTip="The project's account designation cannot be changed as time has been entered towards the project."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic" OnServerValidate="cvClient_ServerValidate"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Revenue Type
                                                    </td>
                                                    <td class="width60P WhiteSpaceNoWrap">
                                                        &nbsp;
                                                        <asp:DropDownList ID="ddlRevenueType" runat="server" onchange="setDirty();" CssClass="Width94Per"
                                                            AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="TdValidators">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="The Revenue Type field is to be used for tracking revenue which has a separate P&L associated with it.&#13;&#10;  When multiple Revenue Types can apply, the Revenue Type should be set to General.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqRevenueType" runat="server" ControlToValidate="ddlRevenueType"
                                                            ErrorMessage="The Revenue Type is required." ToolTip="The Revenue Type is required."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Salesperson
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:DropDownList ID="ddlSalesperson" runat="server" CssClass="Width95Per" onchange="setDirty();">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width2Percent WhiteSpaceNoWrap">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="This field contains the name of the Logic20/20 sales person responsible for the opportunity/project.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqSalesperson" runat="server" ControlToValidate="ddlSalesperson"
                                                            EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Salesperson is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Salesperson is required."></asp:RequiredFieldValidator>
                                                    </td>
                                                    <td class="Width8Percent">
                                                        <asp:ImageButton ID="imgMailToSalesperson" runat="server" OnClick="imgMailToSalesperson_OnClick"
                                                            ToolTip="Mail To" ImageUrl="Images/email_envelope.png" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Business Unit
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:DropDownList ID="ddlProjectGroup" runat="server" CssClass="Width94Per" OnSelectedIndexChanged="ddlProjectGroup_SelectedIndexChanged"
                                                            AutoPostBack="true" onchange="setDirty();">
                                                            <asp:ListItem Text="-- Select Business Unit --" Value="" Selected="True"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width10Percent">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="For most clients this field will be set to Default Group.&#13;&#10; For Microsoft projects this field should be set according the group we are working with within Microsoft.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqBusinessUnit" runat="server" ControlToValidate="ddlProjectGroup"
                                                            EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Business Unit is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Business Unit is required."></asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="cvGroup" runat="server" ErrorMessage="The project's Business Unit designation cannot be changed as time has been entered towards the project."
                                                            ToolTip="The project's Business Unit designation cannot be changed as time has been entered towards the project."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic" OnServerValidate="cvGroup_ServerValidate"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Offering
                                                    </td>
                                                    <td class="width60P WhiteSpaceNoWrap">
                                                        &nbsp;
                                                        <asp:DropDownList ID="ddlOffering" runat="server" onchange="setDirty();" CssClass="Width94Per"
                                                            AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="TdValidators">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="The Offering field is to be used to associate a project with a particular service we have positioned in the market.&#13;&#10; This will aid the business in identifying/confirming which of our services are profitable or not.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqOffering" runat="server" ControlToValidate="ddlOffering"
                                                            ErrorMessage="The Offering is required." ToolTip="The Offering is required."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Executive in Charge
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:DropDownList ID="ddlDirector" runat="server" CssClass="Width95Per" onchange="setDirty();"
                                                            OnSelectedIndexChanged="ddlDirector_SelectedIndexChanged" AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width2Percent">
                                                        &nbsp;
                                                    </td>
                                                    <td class="Width8Percent">
                                                        <asp:ImageButton ID="imgMailToClientDirector" runat="server" OnClick="imgMailToClientDirector_OnClick"
                                                            ToolTip="Mail To" ImageUrl="Images/email_envelope.png" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Business Group
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:Label ID="lblBusinessGroup" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td class="Width10Percent">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Buyer Name
                                                    </td>
                                                    <td class="width60P WhiteSpaceNoWrap">
                                                        &nbsp;
                                                        <asp:TextBox ID="txtBuyerName" runat="server" onchange="setDirty();" CssClass="Width93Per"
                                                            MaxLength="100"></asp:TextBox>
                                                    </td>
                                                    <td class="TdValidators">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="This field contains the name of the main client point of contact.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                            ErrorMessage="The Buyer Name is required." ToolTip="The Buyer Name is required."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="valregBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                            ErrorMessage="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe, hyphen, or a single space."
                                                            ToolTip="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe, hyphen, or a single space."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic" ValidationExpression="^[a-zA-Z'\-\ ]{2,30}$"></asp:RegularExpressionValidator>
                                                        <asp:CustomValidator ID="cvBNAllowSpace" runat="server" ControlToValidate="txtBuyerName"
                                                            ErrorMessage="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe, hyphen, or a single space."
                                                            ToolTip="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe, hyphen, or a single space."
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Dynamic" OnServerValidate="cvBNAllowSpace_ServerValidate"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Project Manager
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:DropDownList ID="ddlProjectOwner" runat="server" onchange="setDirty();" CssClass="Width95Per">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width2Percent WhiteSpaceNoWrap">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="This should be set to the Logic20/20 associate who has been assigned the Project Manager role for the project.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqProjectOwner" runat="server" ControlToValidate="ddlProjectOwner"
                                                            Display="Dynamic" EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Project Manager is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Project Manager is required."></asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="cvProjectOwner" runat="server" EnableClientScript="false"
                                                            ValidationGroup="Project" ErrorMessage="The selected Project Manager has been terminated or made inactive.  Please select another Manager."
                                                            ValidateEmptyText="true" OnServerValidate="cvProjectOwner_OnServerValidate" SetFocusOnError="true"
                                                            Display="Dynamic" Text="*" ToolTip="The selected Project Manager has been terminated or made inactive.  Please select another Manager."></asp:CustomValidator>
                                                    </td>
                                                    <td class="Width8Percent">
                                                        <asp:ImageButton ID="imgMailToProjectOwner" runat="server" OnClick="imgMailToProjectOwner_OnClick"
                                                            ToolTip="Mail To" ImageUrl="Images/email_envelope.png" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Division
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:DropDownList ID="ddlDivision" runat="server" onchange="setDirty();" CssClass="Width94Per"
                                                            AutoPostBack="True" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width10Percent">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="This field should be set based on the Division the project falls within.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqDivision" runat="server" ControlToValidate="ddlDivision"
                                                            EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Division is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Division is required."></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        SOW Budget
                                                    </td>
                                                    <td class="width60P WhiteSpaceNoWrap">
                                                        $
                                                        <asp:TextBox ID="txtSowBudget" CssClass="Width92Per" runat="server" onchange="setDirty();"
                                                            MaxLength="100"></asp:TextBox>
                                                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarkSowBudget" runat="server"
                                                            TargetControlID="txtSowBudget" WatermarkText="Ex: 15000" EnableViewState="false"
                                                            WatermarkCssClass="watermarkedtext Width92Per" />
                                                        <AjaxControlToolkit:FilteredTextBoxExtender ID="fteSowBudget" TargetControlID="txtSowBudget"
                                                            FilterType="Numbers,Custom" FilterMode="ValidChars" runat="server" ValidChars=".," />
                                                    </td>
                                                    <td class="TdValidators">
                                                        <asp:CustomValidator ID="custSowBudget" runat="server" ControlToValidate="txtSowBudget"
                                                            ToolTip="A number with 2 decimal digits is allowed for the Est. Revenue." Text="*"
                                                            ErrorMessage="A number with 2 decimal digits is allowed for the SOW Budget."
                                                            EnableClientScript="false" SetFocusOnError="true" OnServerValidate="custSowBudget_ServerValidate"
                                                            Display="Dynamic" ValidationGroup="Project"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Engagement Manager
                                                    </td>
                                                    <td class="width60P WhiteSpaceNoWrap">
                                                        <asp:DropDownList ID="ddlSeniorManager" runat="server" CssClass="Width945Per" onchange="setDirty();">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width2Percent WhiteSpaceNoWrap">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="This field contains the name of the person who is responsible for maintaining the overall account/client relationship.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="req" runat="server" ControlToValidate="ddlSeniorManager"
                                                            Display="Dynamic" EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Engagement Manager is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Engagement Manager is required."></asp:RequiredFieldValidator>
                                                    </td>
                                                    <td class="Width8Percent">
                                                        <asp:ImageButton ID="imgMailToSeniorManager" runat="server" OnClick="imgMailToSeniorManager_OnClick"
                                                            ToolTip="Mail To" ImageUrl="Images/email_envelope.png" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Practice Area
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:DropDownList ID="ddlPractice" runat="server" onchange="setDirty();" CssClass="Width94Per"
                                                            AutoPostBack="True" OnSelectedIndexChanged="DropDown_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width10Percent">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="This field should be set based on the Practice Area the project falls within.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                                            EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Practice Area is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Practice Area is required."></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        New/Extension
                                                    </td>
                                                    <td class="width60PImp WhiteSpaceNoWrap">
                                                        &nbsp;
                                                        <asp:DropDownList ID="ddlBusinessOptions" CssClass="Width94Per" runat="server" onchange="setDirty();"
                                                            AutoPostBack="True" OnSelectedIndexChanged="ddlBusinessOptions_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="TdValidators">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="This field should be set according to whether this is a new project or an extension of an existing project.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqBusinessTypes" runat="server" ControlToValidate="ddlBusinessOptions"
                                                            Display="Dynamic" EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The New/Extension is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The New/Extension is required."></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="LableProjectManager">
                                                        Project Access
                                                    </td>
                                                    <td class="width60P">
                                                        <cc2:ScrollingDropDown ID="cblProjectManagers" runat="server" SetDirty="true" CssClass="ProjectDetailScrollingDropDown Width16point5PercentImp"
                                                            AllSelectedReturnType="AllItems" onclick="scrollingDropdown_onclick('cblProjectManagers','Project Access','es','Project Accesses',33);"
                                                            DropDownListType="Project Access" DropDownListTypePluralForm="Project Accesses"
                                                            PluralForm="es" />
                                                        <ext:ScrollableDropdownExtender ID="sdeProjectManagers" runat="server" TargetControlID="cblProjectManagers"
                                                            Width="94%" UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png">
                                                        </ext:ScrollableDropdownExtender>
                                                    </td>
                                                    <td class="Width10Percent">
                                                        <asp:CustomValidator ID="cvProjectManager" runat="server" EnableClientScript="false"
                                                            ValidationGroup="Project" ErrorMessage="The Project Access is required." ValidateEmptyText="true"
                                                            OnServerValidate="cvProjectManager_OnServerValidate" SetFocusOnError="true" Text="*"
                                                            ToolTip="The Project Access is required."></asp:CustomValidator>
                                                        <asp:CustomValidator ID="cvProjectManagerStatus" runat="server" EnableClientScript="false"
                                                            ValidationGroup="Project" ErrorMessage="The selected Project Access has been terminated or made inactive.  Please select another Project Access."
                                                            ValidateEmptyText="true" OnServerValidate="cvProjectManagerStatus_OnServerValidate"
                                                            SetFocusOnError="true" Text="*" ToolTip="The selected Project Access has been terminated or made inactive.  Please select another Project Access."></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P vTopImp PaddingTop4">
                                                        Capabilities
                                                    </td>
                                                    <td class="width60P">
                                                        <cc2:ScrollingDropDown ID="cblPracticeCapabilities" runat="server" SetDirty="true"
                                                            CssClass="ProjectDetailScrollingDropDown Width16point5PercentImp" AllSelectedReturnType="AllItems"
                                                            onclick="scrollingDropdown_onclick('cblPracticeCapabilities','Capability','ies','Capabilities',29);"
                                                            DropDownListType="Capability" DropDownListTypePluralForm="Capabilities" PluralForm="ies" />
                                                        <ext:ScrollableDropdownExtender ID="sdePracticeCapabilities" runat="server" TargetControlID="cblPracticeCapabilities"
                                                            BehaviorID="sdePracticeCapabilities" MaxNoOfCharacters="29" Width="94%" UseAdvanceFeature="true"
                                                            EditImageUrl="Images/Dropdown_Arrow.png">
                                                        </ext:ScrollableDropdownExtender>
                                                    </td>
                                                    <td class="Width10Percent">
                                                        <asp:CustomValidator ID="cvCapabilities" runat="server" EnableClientScript="false"
                                                            ValidationGroup="Project" ErrorMessage="The Capability(ies) is required." ValidateEmptyText="true"
                                                            OnServerValidate="cvCapabilities_OnServerValidate" SetFocusOnError="true" Text="*"
                                                            ToolTip="The Capability(ies) is required."></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Pricing List
                                                    </td>
                                                    <td class="width60P WhiteSpaceNoWrap">
                                                        &nbsp;
                                                        <asp:DropDownList ID="ddlPricingList" runat="server" CssClass="Width94Per" onchange="setDirty();">
                                                            <asp:ListItem Text="-- Select Pricing List --" Value="" Selected="True"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="TdValidators">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="For most clients this field will be set to Default.&#13;&#10; For Microsoft projects this field should be set in accordance with the Business Unit.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqPricingList" runat="server" ControlToValidate="ddlPricingList"
                                                            Display="Dynamic" EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Pricing List is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Pricing List is required."></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        CSAT Owner
                                                    </td>
                                                    <td class="width60P WhiteSpaceNoWrap">
                                                        <asp:DropDownList ID="ddlCSATOwner" runat="server" onchange="setDirty();" CssClass="Width945Per"
                                                            OnSelectedIndexChanged="ddlCSATOwner_SelectedIndexChanged" AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width2Percent WhiteSpaceNoWrap">
                                                    </td>
                                                    <td class="Width8Percent">
                                                        <asp:ImageButton ID="imgMailToCSATOwner" runat="server" OnClick="imgMailToCSATOwner_OnClick"
                                                            ToolTip="Mail To" ImageUrl="Images/email_envelope.png" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        Channel
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:DropDownList ID="ddlChannel" runat="server" onchange="setDirty();" CssClass="Width94Per"
                                                            AutoPostBack="True" OnSelectedIndexChanged="ddlChannel_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="Width10Percent">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="The Channel field is to be used for identifying and tracking the source of a project.">
                                                            *</a>
                                                        <asp:RequiredFieldValidator ID="reqChannel" runat="server" ControlToValidate="ddlChannel"
                                                            EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Channel is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Channel is required."></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        PO Number
                                                    </td>
                                                    <td class="width60P WhiteSpaceNoWrap">
                                                        &nbsp
                                                        <asp:TextBox ID="txtPONumber" runat="server" CssClass="Width93Per" onchange="setDirty();">
                                                        </asp:TextBox>
                                                    </td>
                                                    <td class="TdValidators">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P vTopImp PaddingTop4">
                                                        Channel-Sub
                                                    </td>
                                                    <td class="width60P">
                                                        <asp:DropDownList ID="ddlSubChannel" runat="server" CssClass="Width94Per" onchange="setDirty();"
                                                            Visible="false">
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="txtChannelSub" CssClass="Width925Per" runat="server" onchange="setDirty();"
                                                            Visible="false" MaxLength="50"></asp:TextBox>
                                                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="tbwChannelSub" runat="server" TargetControlID="txtChannelSub"
                                                            WatermarkText="Enter Channel-sub" EnableViewState="false" WatermarkCssClass="watermarkedtext Width925Per" />
                                                    </td>
                                                    <td class="Width10Percent">
                                                        <a href="ProjectHelp.aspx" class="ProjectHelp" target="_blank" title="The Channel-sub field is to be used to provide detail regarding the source of a project. &#13;&#10;For example; if the Channel is Salesperson, the Channel-sub would the name of the Salesperson.">
                                                            *</a>
                                                        <asp:RegularExpressionValidator ID="regTxtChannelSub" runat="server" ErrorMessage="Channel-Sub should be limited to 2-50 characters in length."
                                                            ToolTip="Channel-Sub should be limited to 2-50 characters in length." ControlToValidate="txtChannelSub"
                                                            ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Enabled="false" Display="Dynamic" ValidationExpression="^[ A-Za-z0-9_@.,/&+-]{2,50}$"></asp:RegularExpressionValidator>
                                                        <asp:RequiredFieldValidator ID="reqddlChannelSub" runat="server" ControlToValidate="ddlSubChannel"
                                                            Display="Dynamic" EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Channel-Sub is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Channel-Sub is required." Enabled="false"></asp:RequiredFieldValidator>
                                                        <asp:RequiredFieldValidator ID="reqTxtChannelSub" runat="server" ControlToValidate="txtChannelSub"
                                                            Display="Dynamic" EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Channel-Sub is required."
                                                            SetFocusOnError="true" Text="*" ToolTip="The Channel-Sub is required." Enabled="false"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td class="width30P">
                                                        PO Amount
                                                    </td>
                                                    <td class="width60P WhiteSpaceNoWrap">
                                                        $
                                                        <asp:TextBox ID="txtPOAmount" CssClass="Width92Per" runat="server" onchange="setDirty();"
                                                            MaxLength="100"></asp:TextBox>
                                                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="tbwePOAmount" runat="server" TargetControlID="txtPOAmount"
                                                            WatermarkText="Ex: 15000" EnableViewState="false" WatermarkCssClass="watermarkedtext Width92Per" />
                                                        <AjaxControlToolkit:FilteredTextBoxExtender ID="ftbePOAmount" TargetControlID="txtPOAmount"
                                                            FilterType="Numbers,Custom" FilterMode="ValidChars" runat="server" ValidChars=".," />
                                                    </td>
                                                    <td class="TdValidators">
                                                        <asp:CustomValidator ID="custPOAmount" runat="server" ControlToValidate="txtPOAmount"
                                                            ToolTip="A number with 2 decimal digits is allowed for the PO Amount." Text="*"
                                                            ErrorMessage="A number with 2 decimal digits is allowed for the PO Amount." EnableClientScript="false"
                                                            SetFocusOnError="true" OnServerValidate="custPOAmount_ServerValidate" Display="Dynamic"
                                                            ValidationGroup="Project"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width2Percent">
                                        </td>
                                        <td class="TdProjectDetailFeild">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width2Percent">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="Width2Percent">
                            </td>
                            <td>
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="TdProjectNotes">
                                            <table class="WholeWidth Height100Percent">
                                                <tr>
                                                    <td class="InnerTdProjectNotes">
                                                        <u>Project Notes</u>
                                                        <asp:CustomValidator ID="custProjectDesciption" runat="server" ControlToValidate="txtDescription"
                                                            Display="Dynamic" OnServerValidate="custProjectDesciption_ServerValidation" SetFocusOnError="True"
                                                            ErrorMessage="The project description cannot be more than 2000 symbols" ToolTip="The project description cannot be more than 2000 symbols"
                                                            ValidationGroup="Project">*</asp:CustomValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" onchange="setDirty();"
                                                            CssClass="TextBoxProjectNotes"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="TdOpportunityLink">
                                            <table class="WholeWidth Height100Percent">
                                                <tr>
                                                    <td class="TdUTimeEntry">
                                                        <u>Previous Project Linking</u>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtPrevProjectLink" runat="server" CssClass="Width200Px" onchange="setDirty();"
                                                                        Enabled="false" MaxLength="50"></asp:TextBox>
                                                                    <asp:ImageButton ID="imgNavigateToProject" runat="server" AlternateText="Navigate to Project"
                                                                        OnClientClick="RedirectToProject(); return false;" ToolTip="Navigate to Project"
                                                                        Visible="false" ImageUrl="~/Images/arrow_16x16.png" />
                                                                    <asp:RequiredFieldValidator ID="rfvPrevProjectLink" runat="server" Text="*" ErrorMessage="The Previous Project Number is Required."
                                                                        ToolTip="The Previous Project Number is Required" EnableClientScript="false"
                                                                        Enabled="false" ControlToValidate="txtPrevProjectLink" ValidationGroup="Project"></asp:RequiredFieldValidator>
                                                                    <asp:CustomValidator ID="cvPreviousProjectNumber" runat="server" ControlToValidate="txtPrevProjectLink"
                                                                        ValidationGroup="Project" OnServerValidate="cvPreviousProjectNumber_ServerValidate"
                                                                        Enabled="false" EnableClientScript="false" ErrorMessage="Enter a valid Project Number"
                                                                        Display="Dynamic" Text="*" ToolTip="Enter a valid Project Number"></asp:CustomValidator>
                                                                </td>
                                                                <td>
                                                                    <div id="divOutsource" runat="server" class="padLeft60" style="display: none;">
                                                                        Outsourced Service Id
                                                                        <asp:RadioButtonList runat="server" ID="rbOutsourcedId" RepeatDirection="Horizontal"
                                                                            RepeatLayout="Flow">
                                                                            <asp:ListItem Value="1" Text="Yes" Selected="False"></asp:ListItem>
                                                                            <asp:ListItem Value="2" Text="No" Selected="False"></asp:ListItem>
                                                                        </asp:RadioButtonList>
                                                                        <asp:RequiredFieldValidator ID="reqOutsourceId" runat="server" Text="*" ErrorMessage="The Outsource Service Id is Required."
                                                                            ToolTip="The Outsource Service Id is Required." EnableClientScript="false" Enabled="false"
                                                                            ControlToValidate="rbOutsourcedId" ValidationGroup="Project"></asp:RequiredFieldValidator>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="TdUTimeEntry">
                                                        <u>Time Entry</u>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="TdProjectNotesDescription">
                                                        Notes for this project are&nbsp;
                                                        <asp:DropDownList ID="ddlNotes" runat="server" CssClass="Width200Px" onchange="setDirty();">
                                                            <asp:ListItem Selected="True" Text="Required" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="Optional" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="TdProjectNotesDescription">
                                                        Client System Time Entry
                                                        <asp:DropDownList ID="ddlClientTimeEntry" runat="server" CssClass="Width200Px" onchange="setDirty();">
                                                            <asp:ListItem Selected="True" Text="Not Required" Value="0"></asp:ListItem>
                                                            <asp:ListItem Text="Required" Value="1"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width2Percent">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Table ID="tblProjectDetailTabViewSwitch" runat="server" CssClass="CustomTabStyle CustomTabStyleProjectDetail">
                                    <asp:TableRow ID="rowSwitcher" runat="server">
                                        <asp:TableCell ID="cellMilestones" runat="server" CssClass="SelectedSwitch">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnMilestones" runat="server" Text="Milestones" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellFeedback" runat="server" Visible="false">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnFeedback" runat="server" Text="Project Feedback" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellProjectTimeTypes" runat="server">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnProjectTimeTypes" runat="server" Text="Project Time Types"
                                                    CausesValidation="false" OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="CellAttachments" runat="server">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnAttachments" runat="server" Text="Attachments" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="3"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellFinancials" runat="server">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnFinancials" runat="server" Text="Financial Summary" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="4"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellCommissions" runat="server">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnCommissions" runat="server" Text="Commissions" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="5"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellExpenses" runat="server">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnExpenses" runat="server" Text="Expenses" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="6"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellPersons" runat="server">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnPersons" runat="server" Text="Persons" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="7"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="TableCellHistoryg" runat="server" Visible="false">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnHstory" runat="server" Text="History" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="8"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellProjectTools" runat="server" Visible="false">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnProjectTools" runat="server" Text="Clone Project" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="9"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellProjectCSAT" runat="server" Visible="false">
                                            <span class="bg"><span>
                                                <asp:LinkButton ID="btnProjectCSAT" runat="server" Text="CSAT" CausesValidation="false"
                                                    OnCommand="btnView_Command" CommandArgument="10"></asp:LinkButton></span>
                                            </span>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <asp:MultiView ID="mvProjectDetailTab" runat="server" ActiveViewIndex="0">
                                    <asp:View ID="vwMilestones" runat="server">
                                        <asp:Panel ID="pnlRevenueMilestones" runat="server" CssClass="tab-pane">
                                            <div class="PaddingBottom35Px">
                                                <asp:ShadowedTextButton ID="btnAddMilistone" runat="server" CausesValidation="false"
                                                    OnClick="btnAddMilistone_Click" CssClass="add-btn" OnClientClick="if (!confirmSaveDirty()) return false;"
                                                    Text="Add Milestone" />
                                            </div>
                                            <uc:ProjectMilestonesFinancials ID="milestones" runat="server" ValidationGroup="MilestoneTab" />
                                        </asp:Panel>
                                    </asp:View>
                                    <asp:View ID="vwFeedback" runat="server">
                                        <asp:Panel ID="pnlFeedback" runat="server" CssClass="tab-pane">
                                            <uc:ProjectFeedback ID="projectFeedback" runat="server" />
                                        </asp:Panel>
                                    </asp:View>
                                    <asp:View ID="View1" runat="server">
                                        <asp:Panel ID="pnlProjectTimeTypes" runat="server" CssClass="tab-pane">
                                            <div class="PaddingBottom6">
                                                <uc:ProjectTimeTypes ID="ucProjectTimeTypes" runat="server" />
                                                <asp:Button ID="btnDeleteWorkType" OnClick="btnDeleteWorkType_OnClick" runat="server"
                                                    Style='display: none;' Text="" /><asp:HiddenField ID="hdnWorkTypeId" runat="server" />
                                                <asp:CustomValidator ID="cvWorkTypesAssigned" runat="server" EnableClientScript="false"
                                                    ErrorMessage="" ValidateEmptyText="true" Text=""></asp:CustomValidator>
                                            </div>
                                        </asp:Panel>
                                    </asp:View>
                                    <asp:View ID="vmAttachments" runat="server">
                                        <asp:Panel ID="pnlAttachments" runat="server" CssClass="tab-pane">
                                            <div class="PaddingBottom35Px">
                                                <asp:ShadowedTextButton ID="stbAttachSOW" runat="server" CausesValidation="false"
                                                    OnClick="stbAttachSOW_Click" OnClientClick="if(!ConfirmSaveOrExit()) return false;"
                                                    CssClass="add-btn" Text="Add Attachment" />
                                                <asp:HiddenField ID="hdnOpenAttachmentPopUp" runat="server" />
                                                <AjaxControlToolkit:ModalPopupExtender ID="mpeAttachSOW" runat="server" TargetControlID="hdnOpenAttachmentPopUp"
                                                    BackgroundCssClass="modalBackground" PopupControlID="pnlAttachSOW" DropShadow="false" />
                                            </div>
                                            <asp:Repeater ID="repProjectAttachments" runat="server">
                                                <HeaderTemplate>
                                                    <table class="CompPerfTable tablesorter" width="100%" align="center" id="tblProjectAttachments">
                                                        <thead>
                                                            <tr class="CompPerfHeader">
                                                                <th class="Width43Percent">
                                                                    <div class="ie-bg NoBorder">
                                                                        Attachment Name
                                                                    </div>
                                                                </th>
                                                                <th class="Width13Percent">
                                                                    <div class="ie-bg NoBorder">
                                                                        Category
                                                                    </div>
                                                                </th>
                                                                <th class="Width11Percent">
                                                                    <div class="ie-bg NoBorder">
                                                                        Size
                                                                    </div>
                                                                </th>
                                                                <th class="Width13Percent">
                                                                    <div class="ie-bg NoBorder">
                                                                        Uploaded Date
                                                                    </div>
                                                                </th>
                                                                <th class="Width15Percent">
                                                                    <div class="ie-bg NoBorder">
                                                                        Uploader
                                                                    </div>
                                                                </th>
                                                                <th class="Width5Percent">
                                                                    <div class="ie-bg NoBorder">
                                                                        &nbsp;
                                                                    </div>
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="textLeft padLeft20">
                                                            <% if (Project != null && Project.Id.HasValue)
                                                               { %>
                                                            <asp:HyperLink ID="hlnkProjectAttachment1" CssClass="ProjectAttachmentNameWrap" runat="server"
                                                                Text='<%# GetWrappedText( (string)Eval("AttachmentFileName")) %>' NavigateUrl='<%# GetNavigateUrl((string)Eval("AttachmentFileName"), (int)Eval("AttachmentId")) %>'></asp:HyperLink>
                                                            <% }
                                                               else
                                                               { %>
                                                            <asp:LinkButton ID="lnkProjectAttachment1" runat="server" CssClass="ProjectAttachmentNameWrap"
                                                                Visible="<%# IsProjectCreated() %>" attachmentid='<%# Eval("AttachmentId") %>'
                                                                Text='<%# GetWrappedText((string)Eval("AttachmentFileName")) %>' OnClientClick="DownloadUnsavedFile(this); return false;"
                                                                OnClick="lnkProjectAttachment_OnClick" />
                                                            <% } %>
                                                        </td>
                                                        <td class="textCenter">
                                                            <asp:Label ID="lblAttachmentCategory" runat="server" Text='<%# Eval("Category")%>'></asp:Label>
                                                        </td>
                                                        <td class="textCenter" sorttable_customkey='<%# ((int)Eval("AttachmentSize")/1024) %>'>
                                                            <asp:Label ID="lblAttachmentSize" runat="server" Text='<%# string.Format("{0}Kb", (int)Eval("AttachmentSize")/1024)  %>'></asp:Label>
                                                        </td>
                                                        <td class="textCenter">
                                                            <asp:Label ID="lblUploadedDate" runat="server" Text='<%# ((DateTime?)Eval("UploadedDate")).HasValue ? string.Format("{0}", ((DateTime?)Eval("UploadedDate")).Value.ToString("MM/dd/yyyy")) : string.Empty %>'></asp:Label>
                                                        </td>
                                                        <td class="textCenter">
                                                            <asp:Label ID="lblUploader" runat="server" Text='<%# Eval("Uploader") %>'></asp:Label>
                                                        </td>
                                                        <td class="textCenter">
                                                            <asp:ImageButton ID="imgbtnDeleteAttachment1" OnClick="imgbtnDeleteAttachment_Click"
                                                                AttachmentId='<%# Eval("AttachmentId") %>' OnClientClick="if(confirm('Do you really want to delete the project attachment?')){ return true;}return false;"
                                                                Visible="true" runat="server" ImageUrl="~/Images/trash-icon-Large.png" ToolTip="Delete Attachment" />
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </tbody> </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                            <div id="divEmptyMessage" style="display: none;" class="BackGroundColorWhite" runat="server">
                                                No attachments have been uploaded for this project.
                                            </div>
                                        </asp:Panel>
                                    </asp:View>
                                    <asp:View ID="vwFinancials" runat="server">
                                        <uc:ProjectFinancials ID="financials" runat="server" />
                                    </asp:View>
                                    <asp:View ID="vwCommissions" runat="server">
                                        <asp:Panel ID="pnlCommissions" runat="server" CssClass="tab-pane">
                                            <uc:ProjectAttribution ID="projectAttribution" runat="server" ValidationGroup="AttributionGroup" />
                                        </asp:Panel>
                                    </asp:View>
                                    <asp:View ID="vwExpenses" runat="server">
                                        <div class="tab-pane">
                                            <uc2:ProjectExpenses runat="server" ID="projectExpenses" />
                                        </div>
                                    </asp:View>
                                    <asp:View ID="vwPersons" runat="server">
                                        <uc:ProjectPersons ID="persons" runat="server" />
                                    </asp:View>
                                    <asp:View ID="vwHistory" runat="server">
                                        <asp:Panel ID="plnTabHistory" runat="server" CssClass="tab-pane">
                                            <uc:ActivityLogControl runat="server" ID="activityLog" DisplayDropDownValue="Project"
                                                ShowProjectDropDown="false" LabelTextBeforeDropDown="changes over " DateFilterValue="Year"
                                                ShowDisplayDropDown="false" />
                                        </asp:Panel>
                                    </asp:View>
                                    <asp:View ID="vwProjectTools" runat="server">
                                        <asp:Panel ID="pnlTools" runat="server" CssClass="tab-pane">
                                            <table class="alterrow">
                                                <tr>
                                                    <td>
                                                        <ul class="ListStyleNone">
                                                            <li>
                                                                <asp:CheckBox ID="chbCloneMilestones" OnCheckedChanged="chbCloneMilestones_CheckedChanged"
                                                                    runat="server" Checked="true" AutoPostBack="True" Text="Clone milestones and milestone person details" /></li>
                                                            <li>
                                                                <asp:CheckBox ID="chbCloneCommissions" runat="server" Checked="true" Text="Clone commissions" /></li>
                                                            <li>Clone status
                                                                <asp:DropDownList ID="ddlCloneProjectStatus" runat="server" />
                                                                <asp:CustomValidator ID="cvCloneStatus" runat="server" ControlToValidate="ddlCloneProjectStatus"
                                                                    ValidationGroup="CloneStatus" OnServerValidate="cvCloneStatus_ServerValidate"
                                                                    ErrorMessage="Project without milestone cannot be cloned with Active status."
                                                                    Display="Dynamic" ToolTip="Project without milestone cannot be cloned with Active status.">*</asp:CustomValidator>
                                                            </li>
                                                        </ul>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="lnkClone" runat="server" Text="Clone *" ToolTip="Clone *" OnClick="lnkClone_OnClick" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="colorGray">* You will be redirected to the cloned project after you click
                                                            the button.</span>
                                                        <extDisable:ElementDisablerExtender ID="edeCloneButton" runat="server" TargetControlID="lnkClone"
                                                            ControlToDisableID="lnkClone" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:ValidationSummary ID="vsCloneStatus" runat="server" EnableClientScript="false"
                                                            ValidationGroup="CloneStatus" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </asp:View>
                                    <asp:View ID="vwCSAT" runat="server">
                                        <uc:ProjectCSAT ID="ucCSAT" runat="server" />
                                    </asp:View>
                                </asp:MultiView>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="3" class="PaddingTop5">
                                <asp:HiddenField ID="hdnProjectId" runat="server" />
                                <asp:HiddenField ID="hdnProjectDelete" runat="server" />
                                <asp:Button ID="btnDelete" runat="server" Text="Delete Project" OnClick="btnDelete_Click"
                                    ToolTip="Delete Project" OnClientClick="ConfirmToDeleteProject();" Enabled="false"
                                    Visible="false" />&nbsp;
                                <asp:Button ID="btnSave" runat="server" Text="Save" ToolTip="Save" OnClick="btnSave_Click"
                                    CssClass="Width115PxImp" ValidationGroup="Project" />&nbsp;
                                <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" CssClass="Width115PxImp" />
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <asp:View ID="vwInternal" runat="server">
                    <br />
                    <table class="width30P">
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtInternalProjectName" CssClass="Width500PxImp" runat="server"></asp:TextBox>
                                <AjaxControlToolkit:TextBoxWatermarkExtender ID="txtWeInternalProject" runat="server"
                                    TargetControlID="txtInternalProjectName" WatermarkText="Enter a Project Name here..."
                                    EnableViewState="false" WatermarkCssClass="watermarkedtext Width500PxImp" />
                                <asp:RequiredFieldValidator ID="rvIntlProjectName" runat="server" Text="*" ErrorMessage="The Project Name is Required."
                                    ToolTip="The Project Name is Required." ControlToValidate="txtInternalProjectName"
                                    ValidationGroup="InternalProject"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td>
                                Project Number
                            </td>
                            <td class="TdProjectDetailFeild">
                                <asp:DropDownList ID="ddlProjectNumberSeries" runat="server">
                                    <asp:ListItem Text="--Select a series--" Value="0" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="P91" Value="91"></asp:ListItem>
                                    <asp:ListItem Text="P92" Value="92"></asp:ListItem>
                                    <asp:ListItem Text="P93" Value="93"></asp:ListItem>
                                    <asp:ListItem Text="P94" Value="94"></asp:ListItem>
                                    <asp:ListItem Text="P95" Value="95"></asp:ListItem>
                                    <asp:ListItem Text="P96" Value="96"></asp:ListItem>
                                    <asp:ListItem Text="P97" Value="97"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvPrjectNumberSeries" runat="server" ControlToValidate="ddlProjectNumberSeries"
                                    Display="Dynamic" EnableClientScript="false" ErrorMessage="The Project Number Series is Required."
                                    InitialValue="0" SetFocusOnError="true" Text="*" ValidationGroup="InternalProject"
                                    ToolTip="The Project Number Series is Required."></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td class="Width15Per">
                                Project Status
                            </td>
                            <td class="TdProjectDetailFeild">
                                Active
                            </td>
                        </tr>
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td class="Width15Per">
                                Account
                            </td>
                            <td class="TdProjectDetailFeild">
                                Logic20/20
                            </td>
                        </tr>
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td class="Width15Per">
                                Business Unit
                            </td>
                            <td class="TdProjectDetailFeild">
                                Operations
                            </td>
                        </tr>
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td class="Width15Per">
                                Division
                            </td>
                            <td class="width60PImp">
                                <asp:DropDownList ID="ddlIntDivision" runat="server" onchange="setDirty();" CssClass="Width94Per"
                                    AutoPostBack="True" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="reqddlIntDivision" runat="server" ControlToValidate="ddlIntDivision"
                                    Display="Dynamic" EnableClientScript="false" ErrorMessage="The Project Division is Required."
                                    SetFocusOnError="true" Text="*" ValidationGroup="InternalProject" ToolTip="The Project Division is Required."></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td class="Width15Per">
                                Practice Area
                            </td>
                            <td class="TdProjectDetailFeild  width60PImp">
                                <asp:DropDownList ID="ddlIntPractice" runat="server" onchange="setDirty();" CssClass="Width94Per"
                                    AutoPostBack="True" Enabled="false">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="reqddlIntPractice" runat="server" ControlToValidate="ddlIntPractice"
                                    Display="Dynamic" EnableClientScript="false" ErrorMessage="The Project Practice Area is Required."
                                    SetFocusOnError="true" Text="*" ValidationGroup="InternalProject" ToolTip="The Project Practice Area is Required."></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr align="center">
                            <td colspan="3" class="PaddingTop15Imp">
                                <asp:Button ID="btnSaveInternalProject" runat="server" Text="Save" ToolTip="Save"
                                    CssClass="Width115PxImp" ValidationGroup="Project" OnClick="btnSaveInternalProject_Click" />&nbsp;
                                <asp:CancelAndReturnButton ID="btnInternalCancelAndReturnButton" runat="server" CssClass="Width115PxImp" />
                            </td>
                        </tr>
                    </table>
                </asp:View>
            </asp:MultiView>
            <asp:HiddenField ID="hdnVisibilty" runat="server" />
            <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
            <asp:HiddenField ID="hdnLinkPopup" runat="server" Value="" />
            <asp:HiddenField ID="hdnCanShowPopup" runat="server" />
            <asp:HiddenField ID="hdnCompletedStatusPopup" runat="server" />
            <asp:Panel ID="pnlProjectName" runat="server" Style="display: none" CssClass="PanelPerson Width480Px">
                <table class="WholeWidth">
                    <tr>
                        <th class="ThEditProjectNameText vMiddle" colspan="3">
                            Edit Project Name
                        </th>
                    </tr>
                    <tr>
                        <td colspan="3">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="TdProjectNameText">
                            Project Name
                        </td>
                        <td class="paddingBottom5px Width70Percent">
                            <asp:TextBox ID="txtProjectName" runat="server" CssClass="Width95Percent"></asp:TextBox>
                        </td>
                        <td class="paddingBottom5px Width70Percent">
                            <asp:RequiredFieldValidator ID="reqProjectName" runat="server" ControlToValidate="txtProjectName"
                                ErrorMessage="The Project Name is required." ToolTip="The Project Name is required."
                                ValidationGroup="ProjectName" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="TextAlignCenter Width70Percent paddingBottom5pxImp">
                            <asp:Button ID="btnUpdateProjectName" runat="server" Text="Update" ToolTip="Update"
                                OnClick="btnUpdateProjectName_OnClick" OnClientClick="setDirty();" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btncloseEditProjectName" runat="server" ToolTip="Cancel" Text="Cancel">
                            </asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="paddingBottom5pxImp paddingLeft5pxImp">
                            <asp:ValidationSummary ID="VsumProjectName" runat="server" EnableClientScript="false"
                                ValidationGroup="ProjectName" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlAttachSOW" runat="server" Style="display: none" CssClass="PanelPerson Width465Px">
                <table class="WholeWidth Padding5">
                    <tr class="BackGroundColorGray Height27Px">
                        <td align="center" class="font14Px LineHeight25Px WS-Normal" colspan="2">
                            Add Attachment
                            <asp:Button ID="btnAttachmentPopupClose" runat="server" CssClass="mini-report-close floatright"
                                ToolTip="Close" Text="X" OnClientClick="ClearVariables();" OnClick="btnCancel_OnClick">
                            </asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="FileUploadAttachment PaddingBottom10" colspan="2">
                            <asp:FileUpload ID="fuAttachmentsUpload" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FileUploadAttachment" colspan="2">
                            <asp:Label ID="lblAttachmentMessage" ForeColor="Gray" runat="server" CssClass="WordWrap"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="FileUploadAttachment PaddingTop10Px">
                            <asp:DropDownList ID="ddlAttachmentCategory" runat="server" onchange="EnableUploadButton();">
                            </asp:DropDownList>
                            <asp:CustomValidator ID="cvAttachmentCategory" runat="server" ControlToValidate="ddlAttachmentCategory"
                                EnableClientScript="true" SetFocusOnError="true" Display="Dynamic" OnServerValidate="cvAttachmentCategory_OnServerValidate"
                                ValidationGroup="ProjectAttachment" Text="*" ToolTip="Category is required."
                                ErrorMessage="Category is required."></asp:CustomValidator>
                        </td>
                        <td align="right" class="FileUploadAttachment PaddingTop10Px no-wrap">
                            <asp:Button ID="btnUpload" ValidationGroup="ProjectAttachment" runat="server" Text="Upload"
                                ToolTip="Upload" Enabled="false"></asp:Button>
                            <asp:Button ID="btnCancel" runat="server" ToolTip="Cancel" Text="Cancel" OnClientClick="ClearVariables();"
                                OnClick="btnCancel_OnClick"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="FileUploadAttachment paddingBottom10px" colspan="2">
                            <label id="lblUplodedFilesMsg" runat="server" class="displayNone">
                                Following files uploaded successfully:</label>
                            <div id="uploadedFiles" runat="server" class="padLeft2 uploadedFilesDiv">
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                OkControlID="btnOKErrorPanel" CancelControlID="btnOKErrorPanel" DropShadow="false" />
            <asp:Panel ID="pnlErrorPanel" runat="server" Style="display: none;" CssClass="ProjectDetailErrorPanel PanelPerson">
                <table class="Width100Per">
                    <tr>
                        <th align="center" class="TextAlignCenter BackGroundColorGray vBottom">
                            <b class="BtnClose">Attention!</b>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10Px">
                            <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                            <asp:ValidationSummary ID="vsumProject" runat="server" DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists"
                                ShowMessageBox="false" ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a project."
                                ValidationGroup="Project" />
                            <asp:ValidationSummary ID="vsInternalProject" runat="server" DisplayMode="BulletList"
                                CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false" ShowSummary="true"
                                EnableClientScript="false" HeaderText="Following errors occurred while saving an Internal project."
                                ValidationGroup="InternalProject" />
                            <asp:ValidationSummary ID="vsumProjectAttachment" runat="server" EnableClientScript="false"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" ValidationGroup="ProjectAttachment" />
                            <asp:ValidationSummary ID="vsumCSAT" runat="server" DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists"
                                ShowMessageBox="false" ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a CSAT details."
                                ValidationGroup="CSATUpdate" />
                            <asp:ValidationSummary ID="vsumAttribution" runat="server" DisplayMode="BulletList"
                                CssClass="ApplyStyleForDashBoardLists" ValidationGroup="AttributionGroup" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving the attribution details." />
                            <asp:ValidationSummary ID="vsumMilestone" runat="server" DisplayMode="BulletList"
                                CssClass="ApplyStyleForDashBoardLists" ValidationGroup="MilestoneTab" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving the milestone details." />
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10Px TextAlignCenter">
                            <asp:Button ID="btnOKErrorPanel" runat="server" ToolTip="OK" Text="OK" CssClass="Width100PxImp"
                                OnClientClick="$find('mpeErrorPanelBehaviourId').hide();return false;" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeTimeEntriesRelatedToitPopup" runat="server"
                TargetControlID="hdnCanShowPopup" CancelControlID="btnClose" BehaviorID="mpeTEsRelatedToItPopup"
                BackgroundCssClass="modalBackground" PopupControlID="pnlPopup" DropShadow="false"
                OkControlID="btnOk" />
            <asp:Panel ID="pnlPopup" runat="server" CssClass="PanelPerson ConfirmBoxClassError"
                Style="display: none">
                <table class="WholeWidth">
                    <tr>
                        <th align="center" class="TextAlignCenter BackGroundColorGray vBottom" colspan="2">
                            <b class="BtnClose">Attention!</b>
                            <asp:Button ID="btnClose" runat="server" CssClass="mini-report-close floatright"
                                ToolTip="Cancel" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10Px" colspan="2">
                            <table>
                                <tr>
                                    <td>
                                        <p>
                                            You cannot delete this Work type.Because, there are some time entries related to
                                            it.
                                        </p>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TextAlignCenter">
                                        <asp:Button ID="btnOk" runat="server" Text="OK" ToolTip="OK" OnClientClick="return false;" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCompletedStatusPopUp" runat="server"
                TargetControlID="hdnCompletedStatusPopup" BehaviorID="mpeCompletedStatusPopUp"
                BackgroundCssClass="modalBackground" PopupControlID="pnlCompletedStatusPopUp"
                DropShadow="false" />
            <asp:Panel ID="pnlCompletedStatusPopUp" runat="server" CssClass="PanelPerson ConfirmBoxClassError"
                Style="display: none">
                <table class="WholeWidth">
                    <tr>
                        <th align="center" class="TextAlignCenter BackGroundColorGray vBottom" colspan="2">
                            <b class="BtnClose">Attention!</b>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10Px" colspan="2">
                            <table>
                                <tr>
                                    <td>
                                        <p>
                                            <asp:CustomValidator ID="cvCompletedStatus" runat="server" ErrorMessage="" ValidationGroup="cvCompletedStatus"></asp:CustomValidator>
                                            <asp:Label ID="lblCompletedStatus" runat="server"></asp:Label>
                                        </p>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TextAlignCenter">
                                        <asp:Button ID="btnOkCompletedStatusPopup" runat="server" Text="OK" ToolTip="OK"
                                            OnClick="btnOkCompletedStatusPopup_Click" />
                                        <asp:Button ID="btnCancelCompletedStatusPopup" runat="server" Text="Cancel" ToolTip="Cancel"
                                            OnClick="btnCancelCompletedStatusPopup_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Button ID="btnDownloadButton" runat="server" OnClick="lnkProjectAttachment_OnClick"
                Style="display: none;" />
            <asp:HiddenField ID="hdnDownloadAttachmentId" runat="server" Value="" />
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownloadButton" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

