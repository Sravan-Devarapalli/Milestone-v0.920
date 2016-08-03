<%@ Page Title="Vendor Details | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="VendorDetail.aspx.cs" Inherits="PraticeManagement.Config.VendorDetail" %>

<%@ PreviousPageType VirtualPath="~/DashBoard.aspx" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="cc3" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Projects" %>
<%@ Register Src="../Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLog" %>
<%@ Register Src="../Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Vendor Details | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Vendor Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script src="../Scripts/FilterTable.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.uploadify.yui.js" type="text/javascript"></script>
    <asp:UpdatePanel ID="upnlBody" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <uc:LoadingProgress ID="loadingProgress" runat="server" />
            <script type="text/javascript">
                $(document).ready(function () {
                    $("#tblVendorSummaryByProject").tablesorter({
                        sortList: [[0, 0]],
                        sortForce: [[0, 0]]
                    });
                });

                $(document).ready(function () {
                    $("#tblVendorSummaryByPerson").tablesorter({ sortList: [[0, 0], [0, 0]] });
                }
                );

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

                function endRequestHandle(sender, Args) {
                    $("#tblVendorAttachments").tablesorter(
                    {
                        sortList: [[0, 0]]
                    });
                };

                $(document).ready(function () {
                    $('script #tableSorterScript').load(function () {
                        $("#tblVendorAttachments").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
                    });
                });




                var fileError = 0;
                function pageLoad() {

                    document.onkeypress = enterPressed;
                    $("#<%=fuAttachmentsUpload.ClientID%>").fileUpload({
                        'uploader': '../Scripts/uploaderRemovedFolder1.swf',
                        'cancelImg': '../Images/close_16.png',
                        'buttonText': 'Browse File(s)',
                        'script': '../Controls/Projects/AttachmentUpload.ashx',
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
                        imgElement.setAttribute("src", "../Images/close_16.png");
                        imgElement.setAttribute("class", "CursorPointer");
                        cancelDiv[i].innerHTML = "";
                        cancelDiv[i].appendChild(imgElement);

                    }
                }

                function ClearVariables() {
                    fileError = 0;
                }

                function startUpload() {
                    debugger
                    var progressBar = document.getElementById('<%= loadingProgress.ClientID %>_upTimeEntries');
                    progressBar.setAttribute('style', '');
                    ChangeCancelDivInnerHTML();
                    var hdnVendorId = document.getElementById('<%= hdnVendorId.ClientID %>');
                    $("#<%=fuAttachmentsUpload.ClientID%>").fileUploadSettings('scriptData', 'VendorId=' + hdnVendorId.value + '&LoggedInUser=<%= User.Identity.Name %>');
                    $("#<%=fuAttachmentsUpload.ClientID%>").fileUploadStart();
                }

                function EnableUploadButton(selected) {

                    var fileSelected = (selected == true ? true : false);
                    if (!fileSelected) {
                        fileSelected = IsQueueContainValidFiles();
                    }

                    var uploadButton = document.getElementById('<%= btnUpload.ClientID %>');
                    uploadButton.disabled = fileSelected ? "" : "disabled";
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
                    var hdnVendorId = document.getElementById('<%= hdnVendorId.ClientID %>');
                    if (getDirty() || hdnVendorId.value == "") {
                        return confirm("Some data isn't saved. Click Ok to Save the data or Cancel to exit.");
                    }
                    return true;
                }

                function SetTooltipsForallDropDowns() {
                    var optionList = document.getElementsByTagName('option');

                    for (var i = 0; i < optionList.length; ++i) {
                        optionList[i].title = DecodeString(optionList[i].innerHTML);
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

            </script>
            <table class="VendorDetail">
                <tr>
                    <td>
                        Vendor Name
                    </td>
                    <td>
                        <asp:TextBox ID="txtVendorName" runat="server" CssClass="Width250Px" onchange="setDirty();"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqVendorName" runat="server" ValidationGroup="Vendor"
                            ControlToValidate="txtVendorName" ErrorMessage="The Vendor Name is required."
                            EnableClientScript="False" SetFocusOnError="True" ToolTip="The Vendor Name is required.">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="custVendorName" runat="server" ControlToValidate="txtVendorName"
                            ErrorMessage="There is another Vendor with the same Name." ToolTip="There is another Vendor with the same Name."
                            ValidationGroup="Vendor" Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Dynamic" OnServerValidate="custVendorName_ServerValidate"></asp:CustomValidator>
                        <asp:RegularExpressionValidator ControlToValidate="txtVendorName" ValidationGroup="Vendor"
                            ID="valRegName" runat="server" ErrorMessage="Vendor Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe, hyphen, Numerics or a single space."
                            ToolTip="Vendor Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe, hyphen, Numerics or a single space."
                            EnableClientScript="false" Text="*" ValidationExpression="^[a-zA-Z'\-\d ]{2,35}$" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Contact Name
                    </td>
                    <td>
                        <asp:TextBox ID="txtContactName" runat="server" CssClass="Width250Px" onchange="setDirty();"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqContactName" runat="server" ValidationGroup="Vendor"
                            ControlToValidate="txtContactName" ErrorMessage="The Vendor Contact Name is required."
                            EnableClientScript="False" SetFocusOnError="True" ToolTip="The Vendor Name is required.">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvContactName" runat="server" ControlToValidate="txtContactName"
                            ErrorMessage="There is another Vendor with the same Contact Name." ToolTip="There is another Vendor with the same Contact Name."
                            ValidationGroup="Vendor" Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Dynamic" OnServerValidate="custContactName_ServerValidate"></asp:CustomValidator>
                        <asp:RegularExpressionValidator ControlToValidate="txtContactName" ValidationGroup="Vendor"
                            ID="valRegxContactName" runat="server" ErrorMessage="Vendor Contact Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe, hyphen, Numerics or a single space."
                            ToolTip="Vendor Contact Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe, hyphen, Numerics or a single space."
                            EnableClientScript="false" Text="*" ValidationExpression="^[a-zA-Z'\-\d\ ]{2,35}$" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Email&nbsp;Address
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="Width120Px" onchange="setDirty();"></asp:TextBox>&nbsp;@&nbsp;
                        <asp:DropDownList ID="ddlDomain" runat="server" onchange="setDirty();" CssClass="Width109PxImp">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="rfvEmailAddress" runat="server" ControlToValidate="txtEmailAddress"
                            ErrorMessage="The Email Address is required." ToolTip="The Email Address is required."
                            ValidationGroup="Vendor" Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Dynamic" ValidateEmptyText="true"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="regEmailAddress" runat="server" ControlToValidate="txtEmailAddress"
                            Display="Dynamic" ErrorMessage="The Email Address is not valid." ValidationGroup="Vendor"
                            ToolTip="The Email Address is not valid." Text="*" EnableClientScript="False"
                            ValidationExpression="\w+([-+.']\w+)*"></asp:RegularExpressionValidator>
                        <asp:CustomValidator ID="custEmailAddress" runat="server" ControlToValidate="txtEmailAddress"
                            ErrorMessage="A Vendor with the same email address already exists in the system. Please enter another email address."
                            ToolTip="A Vendor with the same email address already exists in the system. Please enter another email address."
                            ValidationGroup="Vendor" Text="*" EnableClientScript="false" SetFocusOnError="true"
                            Display="Dynamic" OnServerValidate="custEmailAddress_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <tr>
                        <td>
                            Telephone number
                        </td>
                        <td>
                            <asp:TextBox ID="txtTelephoneNumber" runat="server" onchange="setDirty();" CssClass="Width250Px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="rfvTelephoneNumber" runat="server" ErrorMessage="The Telephone number is required."
                                ControlToValidate="txtTelephoneNumber" ToolTip="The Telephone number is required."
                                ValidationGroup="Vendor" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                Display="Dynamic"></asp:RequiredFieldValidator>&nbsp;
                            <asp:RegularExpressionValidator ID="reqTelphoneNumber" runat="server" ControlToValidate="txtTelephoneNumber"
                                Display="Dynamic" ErrorMessage="Phone numbers must be either 10 digits (US) or 12 digits (International) in length.  Please enter only numbers. "
                                ValidationGroup="Vendor" ToolTip="Phone numbers must be either 10 digits (US) or 12 digits (International) in length.  Please enter only numbers."
                                Text="*" EnableClientScript="False" ValidationExpression="^([+]?\d{2,3})?[- .]?(?:[- .()]*\d){10}[^\d\n]*$"></asp:RegularExpressionValidator>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Status
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlStatus" runat="server" onchange="setDirty();">
                                <asp:ListItem Text="Active" Value="1" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="reqStatus" runat="server" ControlToValidate="ddlStatus"
                                ErrorMessage="The Vendor status is required." ToolTip="The Vendor status is required."
                                ValidationGroup="Vendor" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                Display="Dynamic" ValidateEmptyText="true"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Vendor Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlVendorType" runat="server" onchange="setDirty();" CssClass="Width250Px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="reqVendorType" runat="server" ControlToValidate="ddlVendorType"
                                ErrorMessage="The Vendor type is required." ToolTip="The Vendor type is required."
                                ValidationGroup="Vendor" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                Display="Dynamic" ValidateEmptyText="true"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
            </table>
            <br />
            <AjaxControlToolkit:TabContainer ID="tcTabs" runat="server" ActiveTabIndex="0" CssClass="CustomTabStyle"
                Visible="false">
                <ajaxToolkit:TabPanel runat="server" ID="tpProjects">
                    <HeaderTemplate>
                        <span class="bg">
                            <asp:LinkButton ID="lnkProjects" Text="Projects" runat="server" OnClick="lnkProjects_Click"></asp:LinkButton>
                        </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="project-filter">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:Repeater ID="repProjects" runat="server">
                                        <HeaderTemplate>
                                            <div>
                                                <table id="tblVendorSummaryByProject" class="CompPerfTable WholeWidth BackGroundColorWhite">
                                                    <thead>
                                                        <tr>
                                                            <th class="CursorPointer color0898E6 fontUnderline Width35Percent">
                                                                <div class='ie-bg'>
                                                                    Project Name
                                                                </div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline Width20PerImp">
                                                                <div class='ie-bg'>
                                                                    Business Unit</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Start Date</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    End Date</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Division</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Practice Area</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Status</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Billable</div>
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <asp:HyperLink ID="btnProjectName" runat="server" CausesValidation="false" Text='<%# (string)Eval("HtmlEncodedName") %>'
                                                        Target="_blank" NavigateUrl='<%#GetProjectLinkURL((DataTransferObjects.Project) Container.DataItem) %>'
                                                        Enabled='<%# !CheckIfDefaultProject(Eval("Id")) %>'></asp:HyperLink>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Group.HtmlEncodedName")%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Division") != null ? Eval("Division.Name") : string.Empty%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Practice.HtmlEncodedName")%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>
                                                </td>
                                                <td class="textCenter" sorttable_customkey='<%# Eval("IsChargeable") %><%#Eval("HtmlEncodedName")%>'>
                                                    <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <AlternatingItemTemplate>
                                            <tr class="alterrow">
                                                <td>
                                                    <asp:HyperLink ID="HyperLink1" runat="server" CausesValidation="false" Text='<%# (string)Eval("HtmlEncodedName") %>'
                                                        Target="_blank" NavigateUrl='<%#GetProjectLinkURL((DataTransferObjects.Project) Container.DataItem) %>'
                                                        Enabled='<%# !CheckIfDefaultProject(Eval("Id")) %>'></asp:HyperLink>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Group.HtmlEncodedName")%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Division") != null ? Eval("Division.Name") : string.Empty%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Practice.HtmlEncodedName")%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>
                                                </td>
                                                <td class="textCenter" sorttable_customkey='<%# Eval("IsChargeable") %><%#Eval("HtmlEncodedName")%>'>
                                                    <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
                                                </td>
                                            </tr>
                                        </AlternatingItemTemplate>
                                        <FooterTemplate>
                                            </tbody></table></div>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    <div id="divProjectEmptyMessage" style="display: none;" runat="server">
                                        No projects.
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="tpResource">
                    <HeaderTemplate>
                        <span class="bg">
                            <asp:LinkButton ID="lnkResource" Text="Resources" runat="server" OnClick="lnkResource_Click"></asp:LinkButton>
                        </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="project-filter">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <asp:Repeater ID="repPersons" runat="server">
                                        <HeaderTemplate>
                                            <div>
                                                <table id="tblVendorSummaryByPerson" class="CompPerfTable WholeWidth BackGroundColorWhite tablesorter ">
                                                    <thead>
                                                        <tr>
                                                            <th class="CursorPointer color0898E6 fontUnderline Width35Percent">
                                                                <div class='ie-bg'>
                                                                    Person Name
                                                                </div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Status</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Start Date</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    End Date</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Division</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Practice Area</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Pay Type</div>
                                                            </th>
                                                            <th class="CursorPointer color0898E6 fontUnderline">
                                                                <div class='ie-bg'>
                                                                    Title</div>
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <asp:HyperLink ID="btnPersonName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("LastName") + ", " + Eval("FirstName")) %>'
                                                        NavigateUrl='<%# GetPersonDetailsUrlWithReturn((DataTransferObjects.Person) Container.DataItem) %>' />
                                                </td>
                                                <td class="textCenter">
                                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>'></asp:Label>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("HireDate") != null ? ((DateTime)Eval("HireDate")).ToString("MM/dd/yyyy") : string.Empty%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("TerminationDate") != null ? ((DateTime)Eval("TerminationDate")).ToString("MM/dd/yyyy") : string.Empty%>
                                                </td>
                                                <td class="textCenter">
                                                    <%#  Eval("Division") != null ? HttpUtility.HtmlEncode((string)Eval("Division.DivisionName")) : string.Empty %>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("DefaultPractice") != null ? HttpUtility.HtmlEncode((string)Eval("DefaultPractice.Name")) : string.Empty%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("CurrentPay.TimescaleName")%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Title.HtmlEncodedTitleName")%>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <AlternatingItemTemplate>
                                            <tr class="alterrow">
                                                <td>
                                                    <asp:HyperLink ID="btnPersonName" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("LastName") + ", " + Eval("FirstName")) %>'
                                                        NavigateUrl='<%# GetPersonDetailsUrlWithReturn((DataTransferObjects.Person) Container.DataItem) %>' />
                                                </td>
                                                <td class="textCenter">
                                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>'></asp:Label>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("HireDate") != null ? ((DateTime)Eval("HireDate")).ToString("MM/dd/yyyy") : string.Empty%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("TerminationDate") != null ? ((DateTime)Eval("TerminationDate")).ToString("MM/dd/yyyy") : string.Empty%>
                                                </td>
                                                <td class="textCenter">
                                                    <%#  Eval("Division") != null ? HttpUtility.HtmlEncode((string)Eval("Division.DivisionName")) : string.Empty %>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("DefaultPractice") != null ? HttpUtility.HtmlEncode((string)Eval("DefaultPractice.Name")) : string.Empty%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("CurrentPay.TimescaleName")%>
                                                </td>
                                                <td class="textCenter">
                                                    <%# Eval("Title.HtmlEncodedTitleName")%>
                                                </td>
                                            </tr>
                                        </AlternatingItemTemplate>
                                        <FooterTemplate>
                                            </tbody></table></div>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    <div id="divEmptyResource" style="display: none;" runat="server">
                                        No Resources.
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="tpHistory">
                    <HeaderTemplate>
                        <span class="bg">
                            <asp:LinkButton ID="lnkHistory" Text="History" runat="server" OnClick="lnkHistory_Click"></asp:LinkButton>
                        </span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="project-filter">
                            <uc:ActivityLog runat="server" ID="activityLog" DisplayDropDownValue="Vendor" ValidationSummaryEnabled="false"
                                DateFilterValue="Year" ShowDisplayDropDown="false" ShowProjectDropDown="false"
                                ShowPersonDropDown="false" />
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="tpAttachements">
                    <HeaderTemplate>
                        <span class="bg">
                            <asp:LinkButton ID="lnkAttachements" Text="Attachements" runat="server" OnClick="lnkAttachements_Click"> </asp:LinkButton></span>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <div class="project-filter">
                            <asp:Panel ID="pnlAttachments" runat="server" CssClass="tab-pane">
                                <div class="PaddingBottom35Px">
                                    <asp:ShadowedTextButton ID="stbAttachSOW" runat="server" CausesValidation="false"
                                        OnClick="stbAttach_Click" OnClientClick="if(!ConfirmSaveOrExit()) return false;"
                                        CssClass="add-btn" Text="Add Attachment" />
                                    <asp:HiddenField ID="hdnOpenAttachmentPopUp" runat="server" />
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeAttachSOW" runat="server" TargetControlID="hdnOpenAttachmentPopUp"
                                        BackgroundCssClass="modalBackground" PopupControlID="pnlAttach" DropShadow="false" />
                                </div>
                                <asp:Repeater ID="repVendorAttachments" runat="server">
                                    <HeaderTemplate>
                                        <table class="CompPerfTable tablesorter" width="100%" align="center" id="tblVendorAttachments">
                                            <thead>
                                                <tr class="CompPerfHeader">
                                                    <th class="Width43Percent">
                                                        <div class="ie-bg NoBorder">
                                                            Attachment Name
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
                                                <% if (Vendor != null && Vendor.Id.HasValue)
                                                   { %>
                                                <asp:HyperLink ID="hlnkVendorAttachment" CssClass="ProjectAttachmentNameWrap" runat="server"
                                                    Text='<%# GetWrappedText( (string)Eval("AttachmentFileName")) %>' NavigateUrl='<%# GetNavigateUrl((string)Eval("AttachmentFileName"), (int)Eval("AttachmentId")) %>'></asp:HyperLink>
                                                <% }
                                                   else
                                                   { %>
                                                <asp:LinkButton ID="lnkVendorAttachment" runat="server" CssClass="ProjectAttachmentNameWrap"
                                                    attachmentid='<%# Eval("AttachmentId") %>' Text='<%# GetWrappedText((string)Eval("AttachmentFileName")) %>'
                                                    OnClientClick="DownloadUnsavedFile(this); return false;" OnClick="lnkVendorAttachment_OnClick" />
                                                <% } %>
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
                                    No attachments have been uploaded for this Vendor.
                                </div>
                            </asp:Panel>
                        </div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </AjaxControlToolkit:TabContainer>
            <table>
                <tr>
                    <td align="center" class="PaddingTop5">
                        <asp:HiddenField ID="hdnVendorId" runat="server" />
                        <asp:Button ID="btnSave" runat="server" Text="Save" ToolTip="Save" OnClick="btnSave_Click"
                            CssClass="Width115PxImp" ValidationGroup="Project" />&nbsp;
                        <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" CssClass="Width115PxImp"
                            UseSubmitBehavior="false" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                CancelControlID="btnCancelErrorPanel" DropShadow="false" OkControlID="btnOKErrorPanel" />
            <asp:Panel ID="pnlErrorPanel" runat="server" Style="display: none;" CssClass="ProjectDetailErrorPanel PanelPerson">
                <table class="Width100Per">
                    <tr>
                        <th class="bgcolorGray TextAlignCenterImp vBottom">
                            <b class="BtnClose">Attention!</b>
                            <asp:Button ID="btnCancelErrorPanel" runat="server" CssClass="mini-report-close floatright"
                                ToolTip="Cancel" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10Px">
                            <asp:ValidationSummary ID="valsVendor" runat="server" EnableClientScript="false"
                                CssClass="ApplyStyleForDashBoardLists" ValidationGroup="Vendor" />
                            <uc:MessageLabel ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green"
                                WarningColor="Orange" />
                            <uc:MessageLabel ID="mlError" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                                WarningColor="Orange" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10Px TextAlignCenterImp">
                            <asp:Button ID="btnOKErrorPanel" runat="server" Text="OK" Width="100" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlAttach" runat="server" Style="display: none" CssClass="PanelPerson Width465Px">
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
                        <td align="center" class="FileUploadAttachment PaddingTop10Px no-wrap">
                            <asp:Button ID="btnUpload" runat="server" Text="Upload" ToolTip="Upload" Enabled="false">
                            </asp:Button>
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
            <asp:Button ID="btnDownloadButton" runat="server" OnClick="lnkVendorAttachment_OnClick"
                Style="display: none;" />
            <asp:HiddenField ID="hdnDownloadAttachmentId" runat="server" Value="" />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

