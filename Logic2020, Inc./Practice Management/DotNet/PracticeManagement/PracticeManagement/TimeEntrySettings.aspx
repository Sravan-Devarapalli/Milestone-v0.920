<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeEntrySettings.aspx.cs"
    Inherits="PraticeManagement.TimeEntrySettings" MasterPageFile="~/PracticeManagement.Master"
    Title="Practice Management - Time Entry Settings" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<%@ Register Src="~/Controls/TimeEntry/TimeEntryManagement.ascx" TagPrefix="uc" TagName="TeManage" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Time Entry Settings</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Time Entry Settings
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <act:TabContainer ID="tabSettings" runat="server">
        <act:TabPanel runat="server" HeaderText="Time Types" ID="tpnlTimeType">
            <ContentTemplate>
                <asp:GridView 
                    ID="gvTimeTypes" 
                    runat="server" 
                    DataSourceID="odsTimeTypes" 
                    DataKeyNames="Id"
                    AutoGenerateColumns="False" onrowdatabound="gvTimeTypes_RowDataBound">
                    <Columns>                        
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                        <asp:CommandField ShowEditButton="True" />
                        <asp:CommandField ShowDeleteButton="True" />
                    </Columns>                    
                </asp:GridView>
                <p>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ControlToValidate="tbNewTimeType" ErrorMessage="Name is required" 
                        ToolTip="Name is required">*</asp:RequiredFieldValidator>
                    <asp:TextBox ID="tbNewTimeType" runat="server" Text="New time type"></asp:TextBox>
                    <asp:Button ID="btnInsertTimeType" runat="server" Text="Add new" 
                        onclick="btnInsertTimeType_Click" />
                </p>
            </ContentTemplate>
        </act:TabPanel>        
    </act:TabContainer>
    <asp:ObjectDataSource ID="odsTimeTypes" runat="server" 
        SelectMethod="GetAllTimeTypes"
        UpdateMethod="UpdateTimeType" 
        DeleteMethod="RemoveTimeType"        
        DataObjectTypeName="DataTransferObjects.TimeEntry.TimeTypeRecord"
        TypeName="PraticeManagement.TimeEntryService.TimeEntryServiceClient"></asp:ObjectDataSource>
</asp:Content>

