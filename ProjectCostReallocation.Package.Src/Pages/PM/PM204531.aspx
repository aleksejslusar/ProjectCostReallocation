<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM204531.aspx.cs" Inherits="Page_PM204531" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="ProjectCostReallocation.ProjectCostReassignmentEntry" PrimaryView="PMCostReassignment" SuspendUnloading="False">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="ViewProjectSource" Visible="False" DependOnGrid="PXGridReassignmentSource"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewTaskSource" Visible="False" DependOnGrid="PXGridReassignmentSource"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewProjectDestination" Visible="False" DependOnGrid="PXGridReassignmentDestination"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewTaskDestination" Visible="False" DependOnGrid="PXGridReassignmentDestination"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewProjectHistory" Visible="False" DependOnGrid="PXGridReassignmentHistory"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewTaskHistory" Visible="False" DependOnGrid="PXGridReassignmentHistory"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewRegisterHistory" Visible="False" DependOnGrid="PXGridReassignmentHistory"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="ViewBatchHistory" Visible="False" DependOnGrid="PXGridReassignmentHistory"></px:PXDSCallbackCommand>
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="PMCostReassignment" Width="100%" Height="100px" TabIndex="3700">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"></px:PXLayoutRule>
            <px:PXSelector ID="edPMReassignmentID" runat="server" DataField="PMReassignmentID"></px:PXSelector>
            <px:PXTextEdit ID="edDescription" runat="server" AlreadyLocalized="False" DataField="Description"></px:PXTextEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" ColumnWidth="XM" LabelsWidth="S"></px:PXLayoutRule>
            <px:PXCheckBox ID="edActive" runat="server" AlreadyLocalized="False" DataField="Active" Text="Active" CommitChanges="True"></px:PXCheckBox>
            <px:PXNumberEdit ID="edRevID" runat="server" AlreadyLocalized="False" DataField="RevID" DefaultLocale="">
            </px:PXNumberEdit>
            <px:PXLayoutRule runat="server" LabelsWidth="SM" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXLayoutRule runat="server" GroupCaption="Reassignment Totals" StartGroup="True">
            </px:PXLayoutRule>
            <px:PXNumberEdit ID="edReassignmentValue1Total" runat="server" AlreadyLocalized="False" DataField="ReassignmentValue1Total" DefaultLocale="">
            </px:PXNumberEdit>
            <px:PXNumberEdit ID="edReassignmentValue2Total" runat="server" AlreadyLocalized="False" DataField="ReassignmentValue2Total" DefaultLocale="">
            </px:PXNumberEdit>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXTab ID="tab" runat="server" Style="z-index: 100;" Width="100%" TabIndex="1">
        <Items>
            <px:PXTabItem Text="Source Project and Task">                               
                <Template>
                    <px:PXGrid ID="PXGridReassignmentSource" runat="server" DataSourceID="ds" TabIndex="2100" SkinID="DetailsInTab" Style="z-index: 100" Width="100%" Height="100%" Caption="Source Project and Task" CaptionVisible="False" TemporaryFilterCaption="Filter Applied">
                        <Levels>
                            <px:PXGridLevel DataMember="PMCostReassignmentSource" DataKeyNames="PMReassignmentID,LineID,ProjectID">                                
                                <Columns>
                                    <px:PXGridColumn DataField="ProjectID" CommitChanges="True" TextAlign="Left" Type="HyperLink" LinkCommand="ViewProjectSource" Width="150px"></px:PXGridColumn>                                    
                                    <px:PXGridColumn DataField="PMProject__Description" Width="250px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMProject__Status" Width="100px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="TaskID" CommitChanges="True" TextAlign="Left" Type="HyperLink" LinkCommand="ViewTaskSource" Width="200px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTask__Description" Width="250px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTask__Status" Width="100px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AccountGroupFrom" TextAlign="Left" Width="150px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AccountGroupTo" TextAlign="Left" Width="150px"></px:PXGridColumn>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Destination Project and Task">
                <Template>
                    <px:PXGrid ID="PXGridReassignmentDestination" runat="server" DataSourceID="ds" SkinID="DetailsInTab" TabIndex="7600" Style="z-index: 100" Width="100%" Height="100%" Caption="Destination Project and Task" CaptionVisible="False" TemporaryFilterCaption="Filter Applied">
                        <Levels>
                            <px:PXGridLevel DataMember="PMCostReassignmentDestination" DataKeyNames="PMReassignmentID,LineID,ProjectID">
                                <Columns>
                                    <px:PXGridColumn CommitChanges="True"  TextAlign="Left" Type="HyperLink" DataField="ProjectID" LinkCommand="ViewProjectDestination" Width="150px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMProject__Description" Width="250px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMProject__Status" Width="100px"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True"  TextAlign="Left" Type="HyperLink" DataField="TaskID" LinkCommand="ViewTaskDestination" Width="200px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTask__Description" Width="250px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTask__Status" Width="100px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ReassignmentValue1" Width="150px" TextAlign="Right"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ReassignmentValue2" TextAlign="Right" Width="150px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ReassignmentSelection" Width="150px" CommitChanges="True"></px:PXGridColumn>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Reassignment History">
                <Template>
                    <px:PXGrid ID="PXGridReassignmentHistory" runat="server" DataSourceID="ds" TabIndex="31100" SkinID="Inquire" AllowSearch="True" Style="z-index: 100" Width="100%" Height="100%" Caption="Reassignment History" CaptionVisible="False">
                        <Levels>
                            <px:PXGridLevel DataKeyNames="PMTran__BranchID" DataMember="PMCostReassignmentHistory">
                                <Columns>
                                    <px:PXGridColumn DataField="PMTran__BranchID" Width="100px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTran__TranType"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTran__RefNbr" TextAlign="Left" Type="HyperLink" LinkCommand="ViewRegisterHistory" Width="150px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTran__ProjectID" TextAlign="Left" Type="HyperLink" LinkCommand="ViewProjectHistory"  Width="150px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTran__TaskID" TextAlign="Left" Type="HyperLink" LinkCommand="ViewTaskHistory"  Width="150px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTran__Description" Width="200px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTran__Amount" TextAlign="Right" Width="100px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMRegister__Status"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTran__Date" Width="90px"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTran__FinPeriodID"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PMTran__BatchNbr" TextAlign="Left" Type="HyperLink" LinkCommand="ViewBatchHistory" Width="150px"></px:PXGridColumn>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items> 
    </px:PXTab>
    
</asp:Content>

