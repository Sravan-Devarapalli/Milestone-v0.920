<%@ Page Title="Skills Search | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsSearch.aspx.cs" Inherits="PraticeManagement.SkillsSearch" %>

<%@ Register Src="Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript" language="javascript">

        function ddlChangedIndustry(changedObject) {
           
            var row = $(changedObject.parentNode.parentNode);
            disableButtons(false);
            var clearLink = row.find("a[id$='lnkbtnClear']")[0];
            if (row.find("select[id$='ddlIndustry']")[0].value != 0) {
                clearLink.disabled = false;
                clearLink.attributes['disable'].value = 'False';
                clearLink.style.color = "#0898E6";
                clearLink.style.cursor = "pointer";
                var str = clearLink.id;
                var cleanLinkUniqueId = str.ReplaceAll("_", "$");
                clearLink.href = "javascript:__doPostBack('" + cleanLinkUniqueId + "','')"
            }
            else {
                clearLink.disabled = true;
                clearLink.attributes['disable'].value = 'True';
                clearLink.style.color = "#8F8F8F";
                clearLink.style.cursor = "text";
            }
        }


        String.prototype.ReplaceAll = function (stringToFind, stringToReplace) {
            var temp = this;
            var index = temp.indexOf(stringToFind);
            while (index != -1) {
                temp = temp.replace(stringToFind, stringToReplace);
                index = temp.indexOf(stringToFind);
            }
            return temp;
        }

        function ddlChanged(changedObject) {
            var row = $(changedObject.parentNode.parentNode);
            disableButtons(false);
            var clearLink = row.find("a[id$='lnkbtnClear']")[0];
            if (row.find("select[id$='ddlCategory']")[0].value != 0 || row.find("select[id$='ddlSkill']")[0].value != 0 || row.find("select[id$='ddlLevel']")[0].value != 0) {
                clearLink.disabled = false;
                clearLink.attributes['disable'].value = 'False';
                clearLink.style.color = "#0898E6";
                clearLink.style.cursor = "pointer";
            }
            else {
                clearLink.disabled = true;
                clearLink.attributes['disable'].value = 'True';
                clearLink.style.color = "#8F8F8F";
                clearLink.style.cursor = "text";
            }
        }

        function disableButtons(disable) {
            var searchButton = document.getElementById('<%= btnSearch.ClientID %>');
            var clearButton = document.getElementById('<%= btnClearAll.ClientID %>');
            searchButton.disabled = disable;
            clearButton.disabled = disable;
        }

        function EnableDisableOkButton(ddl) {
            var btnEmployeeOK = document.getElementById('<%= btnEmployeeOK.ClientID %>');
            if (ddl.value == '' || ddl.value == undefined || ddl.value == null) {
                btnEmployeeOK.disabled = true;
            }
            else {
                btnEmployeeOK.disabled = false;
            }
        }

    </script>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="updSkillSearch" runat="server">
        <ContentTemplate>
            <table class="WholeWidthImp TextAlignCenterImp">
                <tr>
                    <td colspan="2" class="TextAlignCenterImp PaddingBottom20Px">
                        <h1>
                            <asp:Label ID="lblSearchTitle" runat="server"></asp:Label>
                        </h1>
                    </td>
                </tr>
            </table>
            <div class="SkillsSearchBody">
                <div class="SkillsSearchDataBody TextAlignCenterImp">
                    <asp:Panel ID="pnlSearch" runat="server">
                        <AjaxControlToolkit:TabContainer runat="server" ID="tcSkillsEntry" ActiveTabIndex="0"
                            AutoPostBack="true" OnActiveTabChanged="tcSkillsEntry_ActiveTabChanged">
                            <AjaxControlToolkit:TabPanel runat="server" ID="tpBusinessSkills" HeaderText="Business">
                                <ContentTemplate>
                                    <div>
                                        <asp:GridView ID="gvBusinessSkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvSkills_RowDataBound"
                                            ShowFooter="true" CssClass="WholeWidth TabPadding">
                                            <AlternatingRowStyle CssClass="alterrow" />
                                            <HeaderStyle CssClass="alterrow" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Category
                                                    </HeaderTemplate>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdRowNo" runat="server" />
                                                        <asp:DropDownList runat="server" ID="ddlCategory" onchange="ddlChanged(this);" CssClass="Width97Percent"
                                                            isBusiness="true" DataTextField="Description" DataValueField="Id" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                                            AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Skill
                                                    </HeaderTemplate>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:DropDownList runat="server" ID="ddlSkill" onchange="ddlChanged(this);" DataTextField="Description" 
                                                            CssClass="Width97Percent" DataValueField="Id">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Level
                                                    </HeaderTemplate>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                            CssClass="Width97Percent" onchange="ddlChanged(this);">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" ID="lnkbtnClear" Text="Clear" ToolTip="Clear Category, Skill, Level in this row."
                                                            CssClass="fontUnderline" OnClick="lnkbtnClear_Click" disable="">
                                                        </asp:LinkButton>
                                                        <asp:LinkButton runat="server" ID="lnkbtnDelete" Text="Delete" ToolTip="Delete the entrie row."
                                                            OnClick="lnkbtnDelete_Click" CssClass="fontUnderline">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle CssClass="textLeft padLeft10" />
                                                    <FooterStyle CssClass="textLeft padLeft20" />
                                                    <FooterTemplate>
                                                        <asp:LinkButton runat="server" ID="lnkbtnAddSkill" Text="Add Skill" ToolTip="Adds new row."
                                                            CssClass="fontUnderline" OnClick="lnkbtnAddSkill_Click">
                                                        </asp:LinkButton>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                            </AjaxControlToolkit:TabPanel>
                            <AjaxControlToolkit:TabPanel runat="server" ID="tpTechnicalSkills" HeaderText="Technical">
                                <ContentTemplate>
                                    <div>
                                        <asp:GridView ID="gvTechnicalSkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvSkills_RowDataBound"
                                            ShowFooter="true" CssClass="WholeWidth TabPadding">
                                            <AlternatingRowStyle CssClass="alterrow" />
                                            <HeaderStyle CssClass="alterrow" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Category
                                                    </HeaderTemplate>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdRowNo" runat="server" />
                                                        <asp:DropDownList runat="server" ID="ddlCategory" onchange="ddlChanged(this);" CssClass="Width97Percent"
                                                            isBusiness="false" DataTextField="Description" DataValueField="Id" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                                                            AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Skill
                                                    </HeaderTemplate>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:DropDownList runat="server" ID="ddlSkill" onchange="ddlChanged(this);" DataTextField="Description"
                                                            CssClass="Width97Percent" DataValueField="Id">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Level
                                                    </HeaderTemplate>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:DropDownList runat="server" ID="ddlLevel" DataTextField="Description" DataValueField="Id"
                                                            CssClass="Width97Percent" onchange="ddlChanged(this);">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" ID="lnkbtnClear" Text="Clear" ToolTip="Clear Category, Skill, Level in this row."
                                                            CssClass="fontUnderline" OnClick="lnkbtnClear_Click" disable="">
                                                        </asp:LinkButton>
                                                        <asp:LinkButton runat="server" ID="lnkbtnDelete" Text="Delete" ToolTip="Delete the entrie row."
                                                            OnClick="lnkbtnDelete_Click" CssClass="fontUnderline">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle CssClass="textLeft padLeft10" />
                                                    <FooterStyle CssClass="textLeft padLeft20" />
                                                    <FooterTemplate>
                                                        <asp:LinkButton runat="server" ID="lnkbtnAddSkill" Text="Add Skill" ToolTip="Adds new row."
                                                            CssClass="fontUnderline" OnClick="lnkbtnAddSkill_Click">
                                                        </asp:LinkButton>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                            </AjaxControlToolkit:TabPanel>
                            <AjaxControlToolkit:TabPanel runat="server" ID="tpIndustrySkills" HeaderText="Industry">
                                <ContentTemplate>
                                    <div>
                                        <asp:GridView ID="gvIndustrySkills" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvIndustrySkills_RowDataBound"
                                            ShowFooter="true" CssClass="WholeWidth TabPadding">
                                            <AlternatingRowStyle CssClass="alterrow" />
                                            <HeaderStyle CssClass="alterrow" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Industry
                                                    </HeaderTemplate>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdRowNo" runat="server" />
                                                        <asp:DropDownList runat="server" ID="ddlIndustry" DataTextField="Description" CssClass="Width97Percent"
                                                            onchange="ddlChangedIndustry(this);" DataValueField="Id">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderStyle CssClass="Width25Percent" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" ID="lnkbtnClear" Text="Clear" ToolTip="Clear Industry in this row."
                                                            CssClass="fontUnderline" OnClick="lnkbtnClear_Click" disable="">
                                                        </asp:LinkButton>
                                                        <asp:LinkButton runat="server" ID="lnkbtnDelete" Text="Delete" ToolTip="Delete the entrie row."
                                                            OnClick="lnkbtnDelete_Click" CssClass="fontUnderline">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle CssClass="textLeft padLeft10" />
                                                    <FooterStyle CssClass="textLeft padLeft20" />
                                                    <FooterTemplate>
                                                        <asp:LinkButton runat="server" ID="lnkbtnAdd" Text="Add Industry" ToolTip="Adds new row."
                                                            CssClass="fontUnderline" OnClick="lnkbtnAddSkill_Click">
                                                        </asp:LinkButton>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                    </HeaderTemplate>
                                                    <HeaderStyle CssClass="Width50Percent" />
                                                    <ItemTemplate>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                            </AjaxControlToolkit:TabPanel>
                        </AjaxControlToolkit:TabContainer>
                        <table style="width: 90%;">
                            <tr>
                                <td class="SkillSerachTd" align="right">
                                    <asp:Button ID="btnSearch" Text="Search" runat="server" OnClick="btnSearch_OnClick" Enabled="false" />
                                    &nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btnClearAll" Text="Clear" runat="server" OnClick="btnClearAll_Click" Enabled="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SkillSerachTd">
                                    <hr style="border: 1px solid balck;" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SkillSerachTd" align="right">
                                    <b>Employees</b>&nbsp;&nbsp;&nbsp;
                                    <asp:DropDownList ID="ddlEmployees" runat="server" onchange="EnableDisableOkButton(this);">
                                    </asp:DropDownList>
                                    &nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btnEmployeeOK" runat="server" Text="  OK  " OnClick="btnEmployeeOK_OnClick" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlSearchResults" runat="server" Visible="false">
                        <div class="OverFlowAutoImp">
                            <table class="Width90PercentImp">
                                <tr>
                                    <td class="TextAlignCenterImp">
                                        <h1>
                                            <asp:Label ID="lblSearchResultsTitle" runat="server"></asp:Label>
                                        </h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LblSeachCriteria" colspan="2">
                                        <asp:Label ID="lblSearchcriteria" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="t-left padLeft20Imp">
                                        <div id="dlPersonDiv" class="PaddingBottom10Imp" runat="server">
                                            <asp:DataList ID="dlPerson" runat="server" OnItemDataBound="dlPerson_OnItemDataBound"
                                                CssClass="dlPerson">
                                                <AlternatingItemStyle CssClass="alterrow" />
                                                <HeaderTemplate>
                                                    <table class="WholeWidth textCenter">
                                                        <tr class="CompPerfHeader">
                                                            <th class="firstTd">
                                                                <div class="ie-bg">
                                                                    Consultant Name</div>
                                                            </th>
                                                            <th class="secondTd">
                                                                <div class="ie-bg">
                                                                    Consultant Profile</div>
                                                            </th>
                                                        </tr>
                                                    </table>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <table>
                                                        <tr>
                                                            <td class="padLeft5Imp firstTd">
                                                                <asp:HyperLink ID="hlPersonSkillProfile" runat="server" Target="_blank"></asp:HyperLink>
                                                            </td>
                                                            <td class="secondTd">
                                                                <asp:Label ID="lbPersonProfile" Text="Consultant profile not added." runat="server"
                                                                    Visible="false"></asp:Label>
                                                                <asp:HyperLink ID="hlPersonProfile" runat="server" Text="ConsultantProfile" Target="_blank"></asp:HyperLink>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ItemTemplate>
                                                <AlternatingItemTemplate>
                                                    <table>
                                                        <tr>
                                                            <td class="padLeft5Imp firstTd">
                                                                <asp:HyperLink ID="hlPersonSkillProfile" runat="server" Target="_blank"></asp:HyperLink>
                                                            </td>
                                                            <td class="secondTd">
                                                                <asp:Label ID="lbPersonProfile" Text="Consultant profile not added." runat="server"
                                                                    Visible="false"></asp:Label>
                                                                <asp:HyperLink ID="hlPersonProfile" runat="server" Text="ConsultantProfile" Target="_blank"></asp:HyperLink>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </AlternatingItemTemplate>
                                            </asp:DataList>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

