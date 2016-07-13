<%@ Page Title="Project Help | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ProjectHelp.aspx.cs" Inherits="PraticeManagement.ProjectHelp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Project Help | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Project Help
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <style type="text/css">
        .HelpTable
        {
            border-collapse: collapse;
            width: 60%;
        }
        
        td, th
        {
            border: 1px solid #000000;
            text-align: left;
            padding: 8px;
        }
        .txtBold
        {
            font-weight: bold;
        }
        .txtRight
        {
            text-align: right;
        }
    </style>
    <table class="HelpTable">
        <tr>
            <td class="txtBold" style="text-align: center">
                Field
            </td>
            <td class="txtBold" style="text-align: center">
                Definition
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Project Name
            </td>
            <td>
                This field contains the name of the project. The name of the project should follow
                standard naming convention and match the opportunity name in CRM when possible.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Account
            </td>
            <td>
                This field contains the name of company we are completing the project for.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Business Unit
            </td>
            <td>
                For most clients this field will be set to Default Group. For Microsoft projects
                this field should be set according the group we are working with within Microsoft.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Buyer Name
            </td>
            <td>
                This field contains the name of the main client point of contact.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Salesperson
            </td>
            <td>
                This field contains the name of the Logic20/20 sales person responsible for the
                opportunity/project.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                New/Extension
            </td>
            <td>
                This field should be set according to whether this is a new project or an extension
                of an existing project.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Project Manager
            </td>
            <td>
                This should be set to the Logic20/20 associate who has been assigned the Project
                Manager role for the project.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Division
            </td>
            <td>
                This field should be set based on the Division the project falls within.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Practice Area
            </td>
            <td>
                This field should be set based on the Practice Area the project falls within.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Pricing List
            </td>
            <td>
                For most clients this field will be set to Default. For Microsoft projects this
                field should be set in accordance with the Business Unit.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Engagement Manager
            </td>
            <td>
                This field contains the name of the person who is responsible for maintaining the
                overall account/client relationship.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Channel
            </td>
            <td>
                The Channel field is to be used for identifying and tracking the source of a project.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Advisory Board
            </td>
            <td>
                This Channel is to be used when a project is sourced through our network of executive
                advisors.&nbsp; A specific person will be named in the Channel-sub field.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Salesperson (default)
            </td>
            <td>
                This Channel is the default value and is to be used when a project is sourced through
                one of our Sales people.&nbsp; A specific person will be named in the Channel-sub
                field.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Employee
            </td>
            <td>
                This Channel is to be used when a project is sourced through a Logic20/20 employee,
                other than Sales people.&nbsp; A specific person will be named in the Channel-sub
                field.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Inbound Marketing
            </td>
            <td>
                This Channel is to be used when a project is sourced through indirect marketing.&nbsp;
                Example: a client finds us online and calls in.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Outbound Marketing
            </td>
            <td>
                This Channel is to be used when a project is sourced through direct marketing.&nbsp;
                Our marketing department runs campaigns targeting specific verticals/markets.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Partner
            </td>
            <td>
                This Channel is to be used when a project is sourced through a business partner
                of Logic20/20.&nbsp; Example: SmartSheet.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Vendor Management System
            </td>
            <td>
                This Channel is to be used when a project is sourced through a vendor list without
                Salesperson involvement.&nbsp; Examples: Contractor Hub (Microsoft), Government
                vendor lists
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Other Referral
            </td>
            <td>
                This Channel is to be used when a project is sourced through an undefined channel.&nbsp;
                If a source falls within one of the Channels defined above this field should not
                be used.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Channel-sub
            </td>
            <td>
                The Channel-sub field is to be used to provide detail regarding the source of a
                project.&nbsp; For example; if the Channel is Salesperson, the Channel-sub would
                the name of the Salesperson.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Advisory Board
            </td>
            <td>
                This field is a name picker, which is populated via the Vendor List.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Salesperson (default)
            </td>
            <td>
                This field is a name picker, which is populated via Persons who have the role of
                Salesperson associated with them.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Employee
            </td>
            <td>
                This field is a name picker, which is populated via Persons who are active and W2-Salary
                or W2-Hourly.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Inbound Marketing
            </td>
            <td>
                This field is free text.&nbsp; Up to 50 characters are allowed.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Outbound Marketing
            </td>
            <td>
                This field is free text.&nbsp; Up to 50 characters are allowed.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Partner
            </td>
            <td>
                This field is a name picker, which is populated via the Vendor List.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Vendor Management System
            </td>
            <td>
                This field is a name picker, which is populated via the Vendor List.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Other Referral
            </td>
            <td>
                This field is free text.&nbsp; Up to 50 characters are allowed.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Revenue Type
            </td>
            <td>
                The Revenue Type field is to be used for tracking revenue which has a separate P&amp;L
                associated with it. When multiple Revenue Types can apply, the Revenue Type should
                be set to General.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                General (default)
            </td>
            <td>
                This Revenue Type is to be used for the majority of projects.&nbsp; This is the
                default value for the field.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Brainbox
            </td>
            <td>
                This Revenue Type is to be used for projects which were acquired via Brainbox.&nbsp;
                Extensions of those projects should also be tagged with this Revenue Type.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Managed Service
            </td>
            <td>
                This Revenue Type is to be used for projects which are staffed via the Logic20/20
                Managed Service staff, rather than the typical Consulting talent pool.
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Service Line Exec
            </td>
            <td>
                This Revenue Type is to be used for projects which are staffed with Executive level
                resources, rather than the typical Consulting talent pool.&nbsp;
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Adicio
            </td>
            <td>
                This Revenue is to be used for projects which are staffed with resources who are
                identified as Adicio employees.
            </td>
        </tr>
        <tr>
            <td class="txtBold">
                Offering
            </td>
            <td>
                The Offering field is to be used to associate a project with a particular service
                we have positioned in the market.&nbsp; This will aid the business in identifying/confirming
                which of our services are profitable or not.
            </td>
        </tr>
        <%-- <tr>
            <td class="txtRight">
                Advanced Analytics
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Digital Transformation
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Digital Marketing
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Managed Service
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Project Leadership
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="txtRight">
                Service Line Exec
            </td>
            <td>
                &nbsp;
            </td>
        </tr>--%>
    </table>
</asp:Content>

