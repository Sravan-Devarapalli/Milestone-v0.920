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
            font-size: 15px;
        }
        .widthspace
        {
            width: 15px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
   
        <table style="width: 80%">
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
             <div style="padding-left: 5%; padding-right: 10%; text-align: center; overflow:auto; max-height:500px;">
            <table  style="width: 80%">
            <asp:Repeater ID="repIndustries" runat="server" OnItemDataBound="repIndustries_OnItemDataBound"  OnLoad="repIndustries_OnLoad" >
                <HeaderTemplate>
                    <tr>
                        <th class="fontSkilltype" style="width: 40%;">
                            Industries
                        </th>
                        <th style="width: 15%;">
                            Experience
                        </th>
                        <td style="width: 15%;">
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                    <td colspan="4" style="padding-top:15px;font-size:15px;">
                    <asp:Label ID="lblInduatriesMsg" runat="server"  Visible="false"></asp:Label>
                    </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="lblIndustryDesc" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblExp" runat="server"></asp:Label>
                        </td>
                        <td colspan="2">
                        </td>
                    </tr>
                    
                </ItemTemplate>
                
            </asp:Repeater>
            <tr>
                <td colspan="5" class="space">
                </td>
            </tr>
            <asp:Repeater ID="repTypes" runat="server" OnItemDataBound="repTypes_OnItemDataBound">
                <ItemTemplate>
                    <tr>
                        <th class="fontSkilltype">
                            <asp:Label ID="lblTypes" runat="server"></asp:Label>
                        </th>
                        <th>
                            <asp:Label ID="lblLevel" runat="server"></asp:Label>
                        </th>
                        <th>
                            <asp:Label ID="lblLastUsed" runat="server"></asp:Label>
                        </th>
                        <th>
                            <asp:Label ID="lblYearsUsed" runat="server"></asp:Label>
                        </th>
                    </tr>
                      <tr>
                    <td colspan="4" style="padding-top:15px;font-size:15px;">
                    <asp:Label ID="lblTypesMsg" runat="server"  Visible="false"></asp:Label>
                    </td>
                    </tr>
                    <asp:Repeater ID="repCategories" runat="server" OnItemDataBound="repCategories_OnItemDataBound">
                        <ItemTemplate>
                            <tr>
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
                                        <td>
                                            <asp:Label ID="lblSkill" runat="server"></asp:Label>
                                            <span style="width: inherit"></span>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblSkillLevel" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblLastUsed" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblYearsUsed" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
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
            <table style="width: 80%">
            <tr>
                <td colspan="4" align="right" style="text-align:right">
                    <asp:Button runat="server" ID="btnUpdate" Text="Update" />
                </td>
            </tr>
        </table>
 
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

