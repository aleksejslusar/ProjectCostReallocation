<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM504532.aspx.cs" Inherits="Page_PM504532" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="ProjectCostReallocation.ProjectCostReassignmentProcess" PrimaryView="Filter" SuspendUnloading="False">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="ViewProjectSource" Visible="False" DependOnGrid="itemsGrid"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewTaskSource" Visible="False" DependOnGrid="itemsGrid"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewProjectDestination" Visible="False" DependOnGrid="itemsGrid"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewTaskDestination" Visible="False" DependOnGrid="itemsGrid"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewReassignment" Visible="False" DependOnGrid="itemsGrid"></px:PXDSCallbackCommand>
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>

<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="130px" AllowAutoHide="False" TabIndex="100">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" StartColumn="True" LabelsWidth="SM"/>
            <px:PXDateTimeEdit ID="edReassignmentDate" runat="server" AlreadyLocalized="False" DataField="ReassignmentDate" CommitChanges="True" DefaultLocale=""></px:PXDateTimeEdit>
            <px:PXSelector ID="edReassignmentFinPeriodID" runat="server" DataField="ReassignmentFinPeriodID" CommitChanges="True" Size="S"></px:PXSelector>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM"></px:PXLayoutRule>
            <px:PXSelector ID="edFromProjectID" runat="server" DataField="FromProjectID" CommitChanges="True"></px:PXSelector>
            <px:PXSelector ID="edFromTaskID" runat="server" DataField="FromTaskID" CommitChanges="True" AutoRefresh="True"></px:PXSelector>
            <px:PXDropDown ID="edFromProjectStatus" runat="server" DataField="FromProjectStatus" CommitChanges="True"></px:PXDropDown>
            <px:PXDropDown ID="edFromTaskStatus" runat="server" DataField="FromTaskStatus" CommitChanges="True"></px:PXDropDown>
		    <px:PXLayoutRule runat="server" LabelsWidth="SM" StartColumn="True"></px:PXLayoutRule>
		    <px:PXSelector ID="edToProjectID" runat="server" CommitChanges="True" DataField="ToProjectID"></px:PXSelector>
            <px:PXSelector ID="edToTaskID" runat="server" CommitChanges="True" DataField="ToTaskID" AutoRefresh="True"></px:PXSelector>
            <px:PXDropDown ID="edToProjectStatus" runat="server" CommitChanges="True" DataField="ToProjectStatus"></px:PXDropDown>
            <px:PXDropDown ID="edToTaskStatus" runat="server" CommitChanges="True" DataField="ToTaskStatus"></px:PXDropDown>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
        <px:PXGrid ID="itemsGrid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="False" TabIndex="4600" TemporaryFilterCaption="Filter Applied">
		<Levels>
			<px:PXGridLevel DataMember="Items">
			    <Columns>

			        <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="13px" AllowCheckAll="True"></px:PXGridColumn>
			        <px:PXGridColumn DataField="PMReassignmentID" TextAlign="Left" Type="HyperLink" LinkCommand="ViewReassignment"></px:PXGridColumn>
			        <px:PXGridColumn DataField="SourceProjectID" Width="100px" TextAlign="Left" Type="HyperLink" LinkCommand="ViewProjectSource"></px:PXGridColumn>
			        <px:PXGridColumn DataField="SourceProjectDescription" Width="200px"></px:PXGridColumn>
			        <px:PXGridColumn DataField="SourceTaskID" Width="100px" TextAlign="Left" Type="HyperLink" LinkCommand="ViewTaskSource"></px:PXGridColumn>
			        <px:PXGridColumn DataField="SourceTaskDescription" Width="200px"></px:PXGridColumn>
			        <px:PXGridColumn DataField="SourceTaskStartDate" Width="90px"></px:PXGridColumn>
			        <px:PXGridColumn DataField="SourceTaskEndDate" Width="90px"></px:PXGridColumn>
			        <px:PXGridColumn DataField="DestinationProjectID" Width="100px" TextAlign="Left" Type="HyperLink" LinkCommand="ViewProjectDestination"></px:PXGridColumn>
			        <px:PXGridColumn DataField="DestinationProjectDescription" Width="200px"></px:PXGridColumn>
			        <px:PXGridColumn DataField="DestinationTaskID" Width="100px" TextAlign="Left" Type="HyperLink" LinkCommand="ViewTaskDestination"></px:PXGridColumn>
			        <px:PXGridColumn DataField="DestinationTaskDescription" Width="200px"></px:PXGridColumn>
			        <px:PXGridColumn DataField="DestinationTaskStartDate" Width="90px"></px:PXGridColumn>
			        <px:PXGridColumn DataField="DestinationTaskEndDate" Width="90px"></px:PXGridColumn>
			        
			    </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>
