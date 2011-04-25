<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntryManagement.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.TimeEntryManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.Sorting.MultisortExtender"
    TagPrefix="cc2" %>
<%@ Register Src="~/Controls/Generic/Filtering/TERFilter.ascx" TagName="TERFilter"
    TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc2" %>
<%@ Register Assembly="PraticeManagement" TagPrefix="btn" Namespace="PraticeManagement.Controls.Generic.Buttons" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<script type="text/javascript">
    function notifyReviewed(operation) {
        var confirmString = "You are attempting to " + operation + " a Time Record which has already been approved. This data may have already been used to generate billing information. Press Ok to continue, or stay in view mode by pressing Cancel.";
        return confirm(confirmString);
    }
</script>
<%--Filters--%>
<asp:UpdatePanel ID="updTerFilters" runat="server">
    <ContentTemplate>
        <uc:TERFilter ID="tfMain" runat="server" OnResetAllFilters="tfMain_OnResetAllFilters" OnUpdate="tfMain_OnUpdate" />
        <p style="margin-top: 10px; margin-bottom: 10px;">
            <asp:Label ID="lblRowsNumber" runat="server" /> rows returned.
        </p>
    </ContentTemplate>
</asp:UpdatePanel>
<uc:LoadingProgress ID="LoadingProgress1" runat="server" />
<%--Report--%>
<asp:ValidationSummary ID="valSum" runat="server" />
<asp:UpdatePanel ID="updTerGrid" runat="server">
    <ContentTemplate>
<asp:HiddenField ID="hfSortingSync" runat="server" Value="ObjectLastName:ASC,MilestoneDate:ASC" />
<asp:GridView ID="gvTimeEntries" runat="server" DataSourceID="odsTimeEntries" AutoGenerateColumns="False"
    AllowPaging="True" PageSize="25" DataKeyNames="Id" OnRowUpdating="gvTimeEntries_RowUpdating"
    OnRowEditing="gvTimeEntries_RowEditing" OnRowDataBound="gvTimeEntries_RowDataBound" 
    CssClass="CompPerfTable WholeWidth" GridLines="None" BackColor="White" ShowFooter="true">
    <AlternatingRowStyle BackColor="#F9FAFF" />
    <RowStyle BackColor="White" />
    <RowStyle />
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate>
                <div class="ie-bg no-wrap"></div>
            </HeaderTemplate>
            <EditItemTemplate>
                <asp:ImageButton ID="lnkUpdate" runat="server" CausesValidation="True" 
                    CommandName="Update" Text="Update" ImageUrl="~/Images/icon-check.png"/>
                <asp:ImageButton ID="lnkCancel" runat="server" CausesValidation="False" 
                    CommandName="Cancel" Text="Cancel" ImageUrl="~/Images/no.png" />
            </EditItemTemplate>
            <ItemTemplate>
                <asp:ImageButton ID="lnkEdit" runat="server" CausesValidation="False" 
                    CommandName="Edit" Text="Edit" ImageUrl="~/Images/icon-edit.png" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <HeaderTemplate>
                <!-- Person Name -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblPersonName" runat="server" Text="Select a Person" ToolTip="Target person"></asp:Label>
                </div>
                <cc2:MultisortExtender ID="msPersonName" runat="server" TargetControlID="lblPersonName"
                    SynchronizerID="hfSortingSync" AscendingText="&uarr;" DescendingText="&darr;"
                    NoSortingText="" SortExpression="ObjectLastName" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="hlPerson" runat="server" NavigateUrl='<%# GetPersonUrl(((TimeEntryRecord)Container.DataItem).ParentMilestonePersonEntry.ThisPerson) %>'>
                    <%# ((TimeEntryRecord)Container.DataItem).
                                ParentMilestonePersonEntry.ThisPerson.ToString() %>
                </asp:HyperLink>
                <asp:HiddenField ID="hfPerson" runat="server" Value='<%# ((TimeEntryRecord)Container.DataItem).ParentMilestonePersonEntry.ThisPerson.Id.Value %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Date" ItemStyle-CssClass="tem-grid-date-cell">
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <HeaderTemplate>
                <!-- Date -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblDate" runat="server" Text="Date" ToolTip="Date this time entry was entered for"></asp:Label>
                </div>
                <cc2:MultisortExtender ID="msDate" runat="server" TargetControlID="lblDate" SynchronizerID="hfSortingSync"
                    AscendingText="&uarr;" DescendingText="&darr;" NoSortingText="" SortExpression="MilestoneDate" />
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((TimeEntryRecord)Container.DataItem).
                MilestoneDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
            </ItemTemplate>
            <ItemStyle CssClass="tem-grid-date-cell grid-item-cell-padding"></ItemStyle>
            <EditItemTemplate>
                <cc2:FormatedTextBox ID="ftbMilestoneDateEdit" runat="server" DataFormatString='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>'
                    DateText='<%# Bind("MilestoneDate") %>' Width="50" />
                <ajaxToolkit:CalendarExtender runat="server" Format='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>'
                    PopupPosition="Right" TargetControlID="ftbMilestoneDateEdit" />
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Frcst">
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <HeaderTemplate>
                <!-- Frcst -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblFrcst" runat="server" Text="Frcst." ToolTip="Forecasted hours"></asp:Label>
                </div>
                <cc2:MultisortExtender ID="msFrcst" runat="server" TargetControlID="lblFrcst" SynchronizerID="hfSortingSync"
                    AscendingText="&uarr;" DescendingText="&darr;" NoSortingText="" SortExpression="ForecastedHours" />
            </HeaderTemplate>
            <ItemStyle CssClass="grid-item-cell-padding" />
            <ItemTemplate>
                <%# ((TimeEntryRecord)Container.DataItem).
                        ForecastedHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%>
                hrs.
            </ItemTemplate>
            <EditItemTemplate>
                         <cc2:FormattedLabel ID="lblFrcstHours" runat="server"  DataFormatString='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>'  
                       DoubleText='<%# Bind("ForecastedHours")%>' Width="30" />  hrs.
            </EditItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lblTotalForecasted" runat="server" /> hrs.
            </FooterTemplate>
            <FooterStyle Font-Bold="true" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Actl.">
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <HeaderTemplate>
                <!-- Actl -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblActl" runat="server" Text="Actl." ToolTip="Actual hours"></asp:Label>
                </div>
                <cc2:MultisortExtender ID="msActl" runat="server" TargetControlID="lblActl" SynchronizerID="hfSortingSync"
                    AscendingText="&uarr;" DescendingText="&darr;" NoSortingText="" SortExpression="ActualHours" />
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((TimeEntryRecord)Container.DataItem).
                        ActualHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%>
                hrs.
            </ItemTemplate>
            <EditItemTemplate>
                <div style="white-space: nowrap">
                    <cc2:FormatedTextBox ID="ftbActualEdit" runat="server" DataFormatString='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>'
                        DoubleText='<%# Bind("ActualHours") %>' Width="30" />
                    <asp:RangeValidator ID="valActualHours" runat="server" ControlToValidate="ftbActualEdit"
                        ErrorMessage="Actual hours is real value between 0 and 24" MaximumValue="24"
                        MinimumValue="0" SetFocusOnError="True" ToolTip="Actual hours is real value between 0 and 24"
                        Type="Double">*</asp:RangeValidator>
                </div>
            </EditItemTemplate>
             <FooterTemplate>
                <asp:Label ID="lblTotalActual" runat="server" /> hrs.
            </FooterTemplate>
            <FooterStyle Font-Bold="true" />
       </asp:TemplateField>
        <asp:TemplateField HeaderText="Project - Milestone">
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <HeaderTemplate>
                <!-- Project - Milestone -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblProjectMilestone" runat="server" Text="Project - Milestone" ToolTip="Client - project - milestone this time entry is about"/>
                </div>
                <cc2:MultisortExtender ID="msProjectMilestone" runat="server" TargetControlID="lblProjectMilestone"
                    SynchronizerID="hfSortingSync" AscendingText="&uarr;" DescendingText="&darr;"
                    NoSortingText="" SortExpression="MilestonePersonId" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label runat="server" ID="lblClientTooltip" ToolTip="Client">[C]</asp:Label>
                <asp:HyperLink ID="hlClient" runat="server" NavigateUrl='<%# GetClientUrl(((TimeEntryRecord)Container.DataItem).ParentMilestonePersonEntry.ParentMilestone.Project.Client) %>'>
                <%# ((TimeEntryRecord)Container.DataItem).
                                    ParentMilestonePersonEntry.ParentMilestone.Project.Client.Name%>
                </asp:HyperLink>
                <br />
                <asp:Label runat="server" ID="lblProjectTooltip" ToolTip="Project">[P]</asp:Label>
                <asp:HyperLink ID="hlroject" runat="server" NavigateUrl='<%# GetProjectUrl(((TimeEntryRecord)Container.DataItem).ParentMilestonePersonEntry.ParentMilestone.Project) %>'>
                <%# ((TimeEntryRecord)Container.DataItem).
                                    ParentMilestonePersonEntry.ParentMilestone.Project.Name%>
                </asp:HyperLink>
                <br />
                <asp:Label runat="server" ID="lblMilestoneTooltip" ToolTip="Milestone">[M]</asp:Label>
                <asp:HyperLink ID="hlMilestone" runat="server" NavigateUrl='<%# GetMilestonePersonUrl(((TimeEntryRecord)Container.DataItem).ParentMilestonePersonEntry) %>'>
                                    <%# 
                    ((TimeEntryRecord)Container.DataItem).
                                    ParentMilestonePersonEntry.ParentMilestone.Description%>
                </asp:HyperLink>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:DropDownList ID="ddlProjectMilestonesEdit" runat="server" DataSourceID="odsCurrentMilestones"
                    DataTextField="Value" DataValueField="Key" Width="130" SelectedValue='<%# Eval("ParentMilestonePersonEntry.MilestonePersonId") %>' />
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <ItemStyle HorizontalAlign="Center" />
            <HeaderTemplate>
                <!-- Review -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblReview" runat="server" Text="Rev." ToolTip="Review status: pending/accepted/declined, click to change"></asp:Label>
                </div>
                <cc2:MultisortExtender ID="msReview" runat="server" TargetControlID="lblReview" SynchronizerID="hfSortingSync"
                    AscendingText="&uarr;" DescendingText="&darr;" NoSortingText="" SortExpression="IsReviewed" />
            </HeaderTemplate>
            <ItemTemplate>
                <btn:ReviewStatusImageButton ID="btnReview" runat="server" ImageAlign="Middle" OnClick="IconButtonClicked"
                    State='<%# ((TimeEntryRecord)Container.DataItem).IsReviewed %>'
                    EntityId='<%# ((TimeEntryRecord)Container.DataItem).Id.Value %>'
                    Enabled='<%# GetEditingAllowed(((TimeEntryRecord)Container.DataItem)) %>'/>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:DropDownList ID="ddlIsReviewedEdit" runat="server" DataSourceID="odsReviewTypes"
                    SelectedValue='<%# Bind("IsReviewed") %>' Width="30" />
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <HeaderTemplate>
                <!-- Correct -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblIsChargeable" runat="server" Text="Bill." ToolTip="Is this time entry billable to the client, click to change" />
                </div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
                <btn:IsChargeableImageButton ID="btnIsChargeable" runat="server" ImageAlign="Middle"
                    OnClick="IconButtonClicked" State='<%# ((TimeEntryRecord)Container.DataItem).IsChargeable %>'
                    EntityId='<%# ((TimeEntryRecord)Container.DataItem).Id.Value %>'
                    Enabled='<%# GetEditingAllowed(((TimeEntryRecord)Container.DataItem)) %>'/>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:CheckBox ID="chbIsChargeable" runat="server" Checked='<%# Bind("IsChargeable") %>' />
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <HeaderTemplate>
                <!-- Correct -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblbIsCorrect" runat="server" Text="Cr." ToolTip="Is this time entry correct, click to change" />
                </div>
                <cc2:MultisortExtender ID="msIsCorrect" runat="server" TargetControlID="lblbIsCorrect"
                    SynchronizerID="hfSortingSync" AscendingText="&uarr;" DescendingText="&darr;"
                    NoSortingText="" SortExpression="IsCorrect" />
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
                <btn:IsCorrectImageButton ID="btnIsCorrect" runat="server" ImageAlign="Middle" OnClick="IconButtonClicked"
                    State='<%# ((TimeEntryRecord)Container.DataItem).IsCorrect %>'
                    EntityId='<%# ((TimeEntryRecord)Container.DataItem).Id.Value %>'
                    Enabled='<%# GetEditingAllowed(((TimeEntryRecord)Container.DataItem)) %>'/>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:CheckBox ID="chbIsCorrectEdit" runat="server" Checked='<%# Bind("IsCorrect") %>' />
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Time Type">
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <ItemStyle Width="100" />
            <HeaderTemplate>
                <!-- Time Type -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblTimeType" runat="server" Text="Time Type" ToolTip="Time type"/>
                </div>
                <cc2:MultisortExtender ID="msTimeType" runat="server" TargetControlID="lblTimeType"
                    SynchronizerID="hfSortingSync" AscendingText="&uarr;" DescendingText="&darr;"
                    NoSortingText="" SortExpression="TimeTypeId" />
            </HeaderTemplate>
            <EditItemTemplate>
                <asp:DropDownList ID="ddlTimeTypeEdit" runat="server" DataSourceID="odsTimeTypes" Width="150"
                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# ((TimeEntryRecord)Container.DataItem).TimeType.Id %>'>
                </asp:DropDownList>
            </EditItemTemplate>
            <ItemTemplate>
                <%# ((TimeEntryRecord)Container.DataItem).
                                    TimeType.Name%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Notes">
            <HeaderTemplate>
                <div class="ie-bg no-wrap">Notes</div>
            </HeaderTemplate>
            <ItemStyle CssClass="grid-item-cell-padding" />
            <ItemTemplate>
                <%# ((TimeEntryRecord)Container.DataItem).HtmlNote %>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- Notes -->
                <asp:TextBox ID="tbNotesEdit" runat="server" Columns="20" Rows="4" TextMode="MultiLine"
                    Text='<%# Bind("Note") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Entered" ItemStyle-CssClass="tem-grid-date-cell">
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <HeaderTemplate>
                <!-- Entered -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblEntered" runat="server" Text="Entered" ToolTip="Item entered date"/>
                </div>
                <cc2:MultisortExtender ID="msEntered" runat="server" TargetControlID="lblEntered"
                    SynchronizerID="hfSortingSync" AscendingText="&uarr;" DescendingText="&darr;"
                    NoSortingText="" SortExpression="EntryDate" />
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((TimeEntryRecord)Container.DataItem).
                EntryDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
            </ItemTemplate>
            <ItemStyle CssClass="tem-grid-date-cell grid-item-cell-padding"></ItemStyle>
            <EditItemTemplate>
                <cc2:FormatedTextBox ID="ftbEntryDateEdit" runat="server" DataFormatString='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>'
                    DateText='<%# Bind("EntryDate") %>' Width="50" />
                <ajaxToolkit:CalendarExtender runat="server" Format='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>'
                    PopupPosition="Right" TargetControlID="ftbEntryDateEdit" />
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Modified by">
            <HeaderStyle CssClass="tem-grid-date-cell" />
            <ItemStyle Width="100" />
            <HeaderTemplate>
                <!-- Modified by -->
                <div class="ie-bg no-wrap">
                    <asp:Label ID="lblModifiedBy" runat="server" Text="Modified" ToolTip="Item modification date and who did that"/>
                </div>
                <cc2:MultisortExtender ID="msModifiedBy" runat="server" TargetControlID="lblModifiedBy"
                    SynchronizerID="hfSortingSync" AscendingText="&uarr;" DescendingText="&darr;"
                    NoSortingText="" SortExpression="ModifiedByLastName" />
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((TimeEntryRecord)Container.DataItem).ModifiedBy.ToString() %>
                <span style="white-space: nowrap;">(<%# ((TimeEntryRecord)Container.DataItem).
                ModifiedDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>)</span>
            </ItemTemplate>
            <EditItemTemplate>
                You (Right now)
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <div class="ie-bg no-wrap"></div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:ImageButton ID="btnRemove" runat="server" CommandArgument='<%# Eval("Id") %>'
                    CommandName="remove" ImageUrl="~/Images/close_16.png" ToolTip="Remove this time entry" 
                    OnCommand="btnRemove_Command" Enabled='<%# GetEditingAllowed(((TimeEntryRecord)Container.DataItem)) %>' />
            </ItemTemplate>
            <EditItemTemplate>
                &nbsp;
            </EditItemTemplate>
        </asp:TemplateField>
        <%--<asp:CommandField ShowDeleteButton="true" />--%>
    </Columns>
</asp:GridView>
        <br />
        <asp:Repeater ID="repTotals" runat="server" DataSourceID="odsTotals">
            <ItemTemplate>
                <table>
                    <tr>
                        <td><b>Total Forecasted Hours</b>:&nbsp;</td>
                        <td><%# ((TimeEntrySums) Container.DataItem).TotalForecastedHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %> hrs.</td>
                    </tr>
                    <tr>
                        <td><b>Total Actual Hours</b>:&nbsp;</td>
                        <td><%# ((TimeEntrySums) Container.DataItem).TotalActualHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %> hrs.</td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:Repeater>
        <asp:ObjectDataSource ID="odsTotals" runat="server"
            SelectMethod="GetTimeEntrySums" OnSelecting="odsTimeEntries_Selecting" 
            TypeName="PraticeManagement.TimeEntryService.TimeEntryServiceClient">
            <SelectParameters>
                <asp:Parameter Name="selectContext" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:ObjectDataSource ID="odsTimeEntries" runat="server" SelectMethod="GetAllTimeEntries" SelectCountMethod="GetTimeEntriesCount" 
    UpdateMethod="ConstructAndUpdateTimeEntry" DeleteMethod="RemoveTimeEntryById"
    TypeName="PraticeManagement.TimeEntryService.TimeEntryServiceClient" OnUpdating="odsTimeEntries_Updating"
    OnSelecting="odsTimeEntries_Selecting" OnSelected="odsTimeEntries_Selected" EnableCaching="false"
    StartRowIndexParameterName="startRow" MaximumRowsParameterName="maxRows" EnablePaging="true" >
    <SelectParameters>
        <asp:Parameter Name="selectContext"  />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Name="milestoneDate" Type="String" />
        <asp:Parameter Name="entryDate" Type="String" />
        <asp:Parameter Name="milestonePersonId" Type="Int32" />
        <asp:Parameter Name="actualHours" Type="Double" />
        <asp:Parameter Name="forecastedHours" Type="Double" />
        <asp:Parameter Name="timeTypeId" Type="String" />
        <asp:Parameter Name="note" Type="String" />
        <asp:Parameter Name="isReviewed" Type="String" />
        <asp:Parameter Name="isChargeable" Type="Boolean" />
        <asp:Parameter Name="isCorrect" Type="Boolean" />
    </UpdateParameters>
    <DeleteParameters>
        <asp:Parameter Name="Id" Type="Int32" />
    </DeleteParameters>
</asp:ObjectDataSource>
<asp:ObjectDataSource ID="odsMilestones" runat="server" SelectMethod="GetAllTimeEntryMilestones"
    TypeName="PraticeManagement.TimeEntryService.TimeEntryServiceClient" CacheDuration="5" EnableCaching="true"/>
<asp:ObjectDataSource ID="odsPersons" runat="server" SelectMethod="GetAllTimeEntryPersons"
    TypeName="PraticeManagement.TimeEntryService.TimeEntryServiceClient" CacheDuration="5" EnableCaching="true" >
    <SelectParameters>
        <asp:ControlParameter ControlID="tfMain" ConvertEmptyStringToNull="true" DbType="DateTime" Name="entryDateFrom" PropertyName="MilestoneDateFrom" />
        <asp:ControlParameter ControlID="tfMain" ConvertEmptyStringToNull="true" DbType="DateTime" Name="entryDateTo" PropertyName="MilestoneDateTo" />
    </SelectParameters>
</asp:ObjectDataSource>
<asp:ObjectDataSource ID="odsTimeTypes" runat="server" SelectMethod="GetAllTimeTypes"
    CacheDuration="300" EnableCaching="true" TypeName="PraticeManagement.TimeEntryService.TimeEntryServiceClient" />
<asp:ObjectDataSource ID="odsReviewTypes" runat="server" SelectMethod="GetAllReviewStatuses"
    TypeName="PraticeManagement.Utils.TimeEntryHelper" />
<asp:ObjectDataSource ID="odsCurrentMilestones" runat="server" SelectMethod="GetCurrentMilestonesById"
    TypeName="PraticeManagement.Utils.TimeEntryHelper" CacheDuration="5" EnableCaching="true">
    <SelectParameters>
        <asp:Parameter Name="personId" Type="Int32" />
        <asp:ControlParameter ControlID="tfMain" Name="startDate" PropertyName="MilestoneDateFrom"
            Type="DateTime" />
        <asp:ControlParameter ControlID="tfMain" Name="endDate" PropertyName="MilestoneDateTo"
            Type="DateTime" />
    </SelectParameters>
</asp:ObjectDataSource>

