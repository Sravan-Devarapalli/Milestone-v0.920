<%@ Page Title="Skills Profile | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SkillsProfile.aspx.cs" Inherits="PraticeManagement.SkillsProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        th
        {
            text-align: left;
            line-height: 20px;
            width: auto;
        }
        td
        {
            text-align: left;
            line-height: 20px;
            width: auto;
        }
        .space
        {
            height: 15px;
        }
        .space1
        {
            height: 10px;
        }
        .fontSkilltype
        {
            font-size: 15px !important;
        }
        .widthspace
        {
            width: 15px;
        }
        .SkillsBody
        {
            background-color: #d4dff8;
            padding : 10px;
            padding-top :25px !important;
            border-color: -moz-use-text-color #999999 #999999;
            border: 1px solid #999999;
        }
        
        .SkillsDataBody
        {
            background-color: White;
            height: 360px;
            overflow: auto;
            padding-left: 2%; 
            padding-top: 1%;  
            text-align: center; 
            overflow: auto;
        }
   
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <table style="width: 100%">
        <tr>
            <td colspan="4" style="padding-bottom: 30px;">
                <center>
                    <h1>
                        Skills Profile for
                        <asp:Label ID="lblPersonName" runat="server"></asp:Label></h1>
                </center>
            </td>
        </tr>
    </table>
    <div  class="SkillsBody">
    <div style="" class="SkillsDataBody">
        <table style="width: 77%; text-align:center;" >
            <asp:Repeater ID="repIndustries" runat="server" OnItemDataBound="repIndustries_OnItemDataBound"
                OnLoad="repIndustries_OnLoad" >
                <HeaderTemplate>
                    <tr class="CompPerfHeader">
                    <td style="width: 2%;">
                           <div class="ie-bg no-wrap" ></div>
                    </td>
                        <td class="fontSkilltype" style="width: 40%;">
                           <div class="ie-bg no-wrap" style="text-align:left;padding-left:5px;"> Industries</div>

                        </td>
                        <td style="width: 15%;text-align:right;">
                            <div class="ie-bg no-wrap" style="text-align:center;">Experience</div>
                        </td>
                        <th style="width: 15%;">
                        </th>
                        <th style="width: 10%;">
                        </th>
                    </tr>
                    <tr>
                    <td></td>
                    <td colspan="4" style="padding-top:15px;font-weight:bold;">
                    <asp:Label ID="lblInduatriesMsg" runat="server"  Visible="false"></asp:Label>
                    </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                    <td> <div class="ie-bg no-wrap" style="text-align:center;"></div></td>
                        <td>
                            <asp:Label ID="lblIndustryDesc" runat="server"></asp:Label>
                        </td>
                        <td style="text-align:center;" >
                            <asp:Label ID="lblExp" runat="server"></asp:Label>
                        </td>
                        <th colspan="2">
                        </th>
                    </tr>
                    
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr >
                        <td style="background-color:#f9faff;"> </td>
                        <td style="background-color:#f9faff;">
                            <asp:Label ID="lblIndustryDesc" runat="server"></asp:Label>
                        </td>
                        <td style="text-align:center;background-color:#f9faff;">
                            <asp:Label ID="lblExp" runat="server"></asp:Label>
                        </td>
                        <th colspan="2">
                        </th>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
            <tr>
                <td colspan="5" class="space">
                </td>
            </tr>
            <asp:Repeater ID="repTypes" runat="server" OnItemDataBound="repTypes_OnItemDataBound">
                <ItemTemplate>
                    <tr class="CompPerfHeader">
                    <td> <div class="ie-bg no-wrap" style="text-align:center;"></div></td>
                        <td class="fontSkilltype">
                            <div class="ie-bg no-wrap" style="text-align:left;padding-left:5px;"><asp:Label ID="lblTypes" runat="server"></asp:Label></div>                        </td>
                        <td>
                            <div class="ie-bg no-wrap" style="text-align:center;"><asp:Label ID="lblLevel" runat="server"></asp:Label></div>
                        </td>
                        <td>
                            <div class="ie-bg no-wrap" style="text-align:center;"><asp:Label ID="lblLastUsed" runat="server"></asp:Label></div>
                        </td>
                        <td>
                            <div class="ie-bg no-wrap" style="text-align:center;"><asp:Label ID="lblYearsUsed" runat="server"></asp:Label></div>
                        </td>
                    </tr>
                      <tr>
                      <td></td>
                    <td colspan="4" style="padding-top:15px;font-weight:bold;">
                    <asp:Label ID="lblTypesMsg" runat="server"  Visible="false"></asp:Label>
                    </td>
                    </tr>
                    <asp:Repeater ID="repCategories" runat="server" OnItemDataBound="repCategories_OnItemDataBound">
                        <ItemTemplate>
                            <tr>
                            <td></td>
                                <th>
                                    <u>
                                        <asp:Label ID="lblCategories" runat="server"></asp:Label></u>
                                </th>
                                <th colspan="3">
                                </th>
                            </tr>
                            <asp:Repeater ID="repSkills" runat="server" OnItemDataBound="repSkills_OnItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                    <td></td>
                                        <td>
                                            <asp:Label ID="lblSkill" runat="server"></asp:Label>
                                            <span style="width: inherit"></span>
                                        </td>
                                        <td style="text-align:center">
                                            <asp:Label ID="lblSkillLevel" runat="server"></asp:Label>
                                        </td>
                                        <td style="text-align:center">
                                            <asp:Label ID="lblLastUsed" runat="server"></asp:Label>
                                        </td>
                                        <td style="text-align:center;">
                                            <asp:Label ID="lblYearsUsed" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr style="background-color:#f9faff;">
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblSkill" runat="server"></asp:Label>
                                            <span style="width: inherit"></span>
                                        </td>
                                        <td style="text-align:center">
                                            <asp:Label ID="lblSkillLevel" runat="server"></asp:Label>
                                        </td>
                                        <td style="text-align:center" >
                                            <asp:Label ID="lblLastUsed" runat="server"></asp:Label>
                                        </td>
                                        <td  style="text-align:center;" >
                                            <asp:Label ID="lblYearsUsed" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                            <tr>
                                <td colspan="5" class="space1">
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr>
                        <td colspan="5" class="space">
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
        </div>
    <table style="width: 100%;">
        <tr>
            <td  style=" text-align:center; padding-top:15px;">
                <asp:Button runat="server" ID="btnUpdate" Text="Update" />
            </td>
        </tr>
    </table>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

