using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectCostReallocation.ProjectCostReallocation.Helpers;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.PM;
using Messages = PX.Objects.PM.Messages;

namespace ProjectCostReallocation
{
  
  public class ProjectCostReassignmentEntry : PXGraph<ProjectCostReassignmentEntry, UsrPMCostReassignment>
  {
        #region Selects

        public PXSelectOrderBy<UsrPMCostReassignment, OrderBy<Asc<UsrPMCostReassignment.pMReassignmentID>>> PMCostReassignment;

        public PXSelectJoin<UsrPMCostReassignmentSource, 
                  InnerJoin<PMProject, On<PMProject.contractID, Equal<UsrPMCostReassignmentSource.projectID>>,
                   LeftJoin<PMTask, On<PMTask.taskID, Equal<UsrPMCostReassignmentSource.taskID>, 
                                   And<PMTask.projectID, Equal<UsrPMCostReassignmentSource.projectID>>>>>,
                      Where<UsrPMCostReassignmentSource.pMReassignmentID, Equal<Current<UsrPMCostReassignment.pMReassignmentID>>>,
                    OrderBy<Asc<UsrPMCostReassignmentSource.lineID>>> PMCostReassignmentSource;

        public PXSelectJoin<UsrPMCostReassignmentDestination, 
                  InnerJoin<PMProject, On<PMProject.contractID, Equal<UsrPMCostReassignmentDestination.projectID>>,
                   LeftJoin<PMTask,On<PMTask.taskID, Equal<UsrPMCostReassignmentDestination.taskID>, 
                                  And<PMTask.projectID, Equal<UsrPMCostReassignmentDestination.projectID>>>>>,
                     Where<UsrPMCostReassignmentDestination.pMReassignmentID, Equal<Current<UsrPMCostReassignment.pMReassignmentID>>>,
                   OrderBy<Asc<UsrPMCostReassignmentDestination.lineID>>> PMCostReassignmentDestination;

        [PXCopyPasteHiddenView]
        public PXSetup<PMSetup> PMSetupSelect;

        [PXCopyPasteHiddenView]
        public PXSelect<PMProject> PMProjectSelect;

        [PXCopyPasteHiddenView]
        public PXSelect<PMTask> PMTaskSelect;

        [PXCopyPasteHiddenView]
        public PXSelectReadonly<PMTask, Where<PMTask.status, NotEqual<ProjectTaskStatus.completed>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>> PMNotCompletedTaskSelect;

        [PXCopyPasteHiddenView]
        public PXSelectJoin<PMTran,
                  InnerJoin<UsrPMCostReassignmentHistory, On<PMTran.tranID, Equal<UsrPMCostReassignmentHistory.tranID>>, 
                   LeftJoin<PMRegister, On<PMRegister.refNbr, Equal<PMTran.refNbr>, And<PMRegister.module, Equal<PMTran.tranType>>>>>,                    
                      Where<UsrPMCostReassignmentHistory.pMReassignmentID, Equal<Current<UsrPMCostReassignment.pMReassignmentID>>>> PMCostReassignmentHistory;

        [PXCopyPasteHiddenView]
        public PXSelect<UsrPMCostReassignmentRunHistory, 
                  Where<UsrPMCostReassignmentRunHistory.pMReassignmentID, Equal<Current<UsrPMCostReassignment.pMReassignmentID>>, 
                    And<UsrPMCostReassignmentRunHistory.revID, Equal<Current<UsrPMCostReassignment.revID>>,
                    And<UsrPMCostReassignmentRunHistory.destinationTaskID, Equal<Required<UsrPMCostReassignmentRunHistory.destinationTaskID>>,
                    And<UsrPMCostReassignmentRunHistory.destinationTranID, Equal<Required<UsrPMCostReassignmentRunHistory.destinationTranID>>>>>>> RunHistory;

        [PXCopyPasteHiddenView]
        public PXSelect<PMTran, Where<PMTran.projectID, Equal<Required<PMTran.projectID>>, And<PMTran.taskID, Equal<PMTran.taskID>>>> Tran;

        #endregion

        #region History and log selects
        public PXSelect<UsrPMCostReassignmentHistory> ReassignmentHistoryView;
        public PXSelect<UsrPMCostReassignmentSourceTran> ReassignmentSourceTranView;
        public PXSelect<UsrPMCostReassignmentPercentage> ReassignmentPercentageView;
        public PXSelect<UsrPMCostReassignmentRunHistory> ReassignmentRunHistoryView;
        public PXSelectJoin<UsrPMCostReassignmentSourceTran,
                  InnerJoin<UsrPMCostReassignmentRunHistory, On<UsrPMCostReassignmentRunHistory.sourceTranID, Equal<UsrPMCostReassignmentSourceTran.tranID>>,
                  InnerJoin<PMTran, On<PMTran.tranID, Equal<UsrPMCostReassignmentRunHistory.destinationTranID>>>>,
                      Where<UsrPMCostReassignmentSourceTran.sourceTranID, Equal<Required<UsrPMCostReassignmentSourceTran.sourceTranID>>,
                        And<UsrPMCostReassignmentRunHistory.pMReassignmentID, Equal<Required<UsrPMCostReassignmentRunHistory.pMReassignmentID>>,
                        And<UsrPMCostReassignmentRunHistory.revID, Equal<Required<UsrPMCostReassignmentRunHistory.revID>>>>>> ProcessedTransactions;           
        #endregion

        #region CacheAttached

        [PXCustomizeBaseAttribute(typeof (PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Project Name")]
        protected virtual void PMProject_Description_CacheAttached(PXCache sender)
        {
        }

        [PXCustomizeBaseAttribute(typeof (PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Project Status")]
        protected virtual void PMProject_Status_CacheAttached(PXCache sender)
        {
        }

        [PXCustomizeBaseAttribute(typeof (PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Task Name")]
        protected virtual void PMTask_Description_CacheAttached(PXCache sender)
        {
        }

        [PXCustomizeBaseAttribute(typeof (PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Task Status")]
        protected virtual void PMTask_Status_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Module", Enabled = false)]
        protected virtual void PMTran_TranType_CacheAttached(PXCache sender)
        {
        }
        
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Batch Nbr.")]
        protected virtual void PMTran_BatchNbr_CacheAttached(PXCache sender)
        {
        }

        #endregion

        #region Actions

        public PXAction<UsrPMCostReassignment> viewProjectSource;
        [PXUIField(DisplayName = Messages.ViewProject, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewProjectSource(PXAdapter adapter)
        {
            if (PMCostReassignmentSource.Current != null)
            {
                var currentProjectID = PMCostReassignmentSource.Current.ProjectID;
                PMCostReassignmentViewer.ViewProjectCommon(currentProjectID);
            }

            return adapter.Get();
        }
       
        public PXAction<UsrPMCostReassignment> viewTaskSource;
        [PXUIField(DisplayName = Messages.ViewTask, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewTaskSource(PXAdapter adapter)
        {
            if (PMCostReassignmentSource.Current != null)
            {
                var currentTaskID = PMCostReassignmentSource.Current.TaskID;
                PMCostReassignmentViewer.ViewTaskCommon(currentTaskID);
            }
                            
            return adapter.Get();
        }

        public PXAction<UsrPMCostReassignment> viewProjectDestination;
        [PXUIField(DisplayName = Messages.ViewProject, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewProjectDestination(PXAdapter adapter)
        {
            if (PMCostReassignmentSource.Current != null)
            {
                var currentProjectID = PMCostReassignmentDestination.Current.ProjectID;
                PMCostReassignmentViewer.ViewProjectCommon(currentProjectID);
            }

            return adapter.Get();
        }

        public PXAction<UsrPMCostReassignment> viewTaskDestination;
        [PXUIField(DisplayName = Messages.ViewTask, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewTaskDestination(PXAdapter adapter)
        {
            if (PMCostReassignmentSource.Current != null)
            {
                var currentTaskID = PMCostReassignmentDestination.Current.TaskID;
                PMCostReassignmentViewer.ViewTaskCommon(currentTaskID);
            }

            return adapter.Get();
        }

        public PXAction<UsrPMCostReassignment> viewProjectHistory;
        [PXUIField(DisplayName = Messages.ViewProject, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewProjectHistory(PXAdapter adapter)
        {
            if (PMCostReassignmentSource.Current != null)
            {
                var currentProjectID = PMCostReassignmentHistory.Current.ProjectID;
                PMCostReassignmentViewer.ViewProjectCommon(currentProjectID);
            }

            return adapter.Get();
        }

        public PXAction<UsrPMCostReassignment> viewTaskHistory;
        [PXUIField(DisplayName = Messages.ViewTask, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewTaskHistory(PXAdapter adapter)
        {
            if (PMCostReassignmentSource.Current != null)
            {
                var currentTaskID = PMCostReassignmentHistory.Current.TaskID;
                PMCostReassignmentViewer.ViewTaskCommon(currentTaskID);
            }
                       
            return adapter.Get();
        }

        public PXAction<UsrPMCostReassignment> viewRegisterHistory;
        [PXUIField(DisplayName = "View Register History", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewRegisterHistory(PXAdapter adapter)
        {
            if (PMCostReassignmentSource.Current != null)
            {
                var currentRefID = PMCostReassignmentHistory.Current.RefNbr;  
                var currentRefType = PMCostReassignmentHistory.Current.TranType;
                var graph = CreateInstance<RegisterEntry>();
                graph.Document.Current = graph.Document.Search<PMRegister.refNbr>(currentRefID, currentRefType);
                throw new PXRedirectRequiredException(graph, true, Messages.ViewTask) { Mode = PXBaseRedirectException.WindowMode.NewWindow };                
            }

            return adapter.Get();
        }

        public PXAction<UsrPMCostReassignment> viewBatchHistory;
        [PXUIField(DisplayName = "View Batch History", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewBatchHistory(PXAdapter adapter)
        {
            if (PMCostReassignmentSource.Current != null)
            {
                var currentBatchNbr = PMCostReassignmentHistory.Current.BatchNbr;
                var currentBatchType = PMCostReassignmentHistory.Current.TranType;
                var graph = CreateInstance<JournalEntry>();
                graph.BatchModule.Current = graph.BatchModule.Search<Batch.batchNbr>(currentBatchNbr, currentBatchType);
                throw new PXRedirectRequiredException(graph, true, Messages.ViewTask) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }

            return adapter.Get();
        }

        #endregion

        #region .ctor

        public ProjectCostReassignmentEntry()
        {
            //Check PMsetup value for autonumbering
            var value = GetPMSetupUsrReassignmentNumberingID();
            if (string.IsNullOrEmpty(value))
            {
                throw new PXSetupNotEnteredException<PMSetup>(PMCostReassignmentMessages.REASSIGNMENT_NUMBERING_SEQUENCE_VALUE_NOT_CONFIGURED);
            }
        }
        #endregion

        #region Event handlers

        #region UsrPMCostReassignment

        protected virtual void UsrPMCostReassignment_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignment;
            CalcutaleReassignmentValueTotals(row);
        }

        protected virtual void UsrPMCostReassignment_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignment;
            if (row != null)
            {
                var isActive = row.Active.GetValueOrDefault();

                PXUIFieldAttribute.SetEnabled<UsrPMCostReassignment.active>(sender, row, true);
                PXUIFieldAttribute.SetEnabled<UsrPMCostReassignment.branchID>(sender, row, !isActive);
                PXUIFieldAttribute.SetEnabled<UsrPMCostReassignment.description>(sender, row, !isActive);

                PMCostReassignment.Cache.AllowDelete = !isActive;

                PMCostReassignmentSource.Cache.AllowInsert = !isActive;
                PMCostReassignmentSource.Cache.AllowUpdate = !isActive;
                PMCostReassignmentSource.Cache.AllowDelete = !isActive;

                PMCostReassignmentDestination.Cache.AllowInsert = !isActive;
                PMCostReassignmentDestination.Cache.AllowUpdate = !isActive;
                PMCostReassignmentDestination.Cache.AllowDelete = !isActive;
            }
        }
       
        protected virtual void UsrPMCostReassignment_Active_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var row = e.Row as UsrPMCostReassignment;
                if (row != null && (row.ReassignmentValue1Total.GetValueOrDefault() == 0 && row.ReassignmentValue2Total == 0))
                {                    
                    throw new PXSetPropertyException(PMCostReassignmentMessages.SOURCE_AND_DESTINATION_TOTALS_MUST_BE_BALANCED_BEFORE_ACTIVATING_THE_REASSIGNMENT);
                }                
            }
        }

        protected virtual void UsrPMCostReassignment_Active_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignment;
            var rowStatus = PMCostReassignment.Cache.GetStatus(row);
            var isNew = rowStatus == PXEntryStatus.Notchanged || rowStatus == PXEntryStatus.Inserted;

            if (row == null || isNew) return;

            var oldActiveValue = row.Active.GetValueOrDefault();
            var newActiveValue = (bool) e.NewValue;
            if ((!oldActiveValue && newActiveValue) && PMCostReassignmentHistory.Select().Any())
            {
                var result = PMCostReassignment.Ask("Reassignment Activation", "Do you want to assign new Revision ID for the given Reassignment?", MessageButtons.YesNo, MessageIcon.Question);
                if (result != WebDialogResult.No)
                {
                    row.RevID++;
                }
            }
        }

      #endregion

        #region UsrPMCostReassignmentSource

        protected virtual void UsrPMCostReassignmentSource_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (UsrPMCostReassignmentSource)e.Row;
            row.TaskID = null;
            row.AccountGroupFrom = null;
            row.AccountGroupTo = null;
        }

        protected virtual void UsrPMCostReassignmentSource_TaskID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentSource;
            if (row != null)
            {
                var newTaskId = e.NewValue as int?;
                var usedTaskIDs = PMCostReassignmentDestination.Select().Select(i => ((UsrPMCostReassignmentDestination) i).TaskID);
                if (usedTaskIDs.Any(i => i.GetValueOrDefault() == newTaskId.GetValueOrDefault()))
                {
                    throw new PXSetPropertyException(PMCostReassignmentMessages.SAME_PROJECT_TASK_CANNOT_BE_USED_AS_A_SOURCE_AND_AS_A_DESTINATION_TASK_IN_A_SINGLE_REASSIGNMENT,PXErrorLevel.RowError);
                }

                VerifyingPMTaskUseInOtherReassignments(row.PMReassignmentID, newTaskId);
            }
        }

        #endregion

        #region UsrPMCostReassignmentDestination

        protected virtual void UsrPMCostReassignmentDestination_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentDestination;
            var isActive = PMCostReassignment.Current.Active.GetValueOrDefault();
            PXUIFieldAttribute.SetEnabled<UsrPMCostReassignmentDestination.reassignmentValue1>(sender, row, !isActive);
            PXUIFieldAttribute.SetEnabled<UsrPMCostReassignmentDestination.reassignmentValue2>(sender, row, !isActive);
        }

        protected virtual void UsrPMCostReassignmentDestination_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentDestination;
            if (row != null)
            {
                AddTotals(row);
            }
            
        }

        protected virtual void UsrPMCostReassignmentDestination_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentDestination;
            if (row != null)
            {
                SubstractTotals(row);
            }
        }

        protected virtual void UsrPMCostReassignmentDestination_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentDestination;
            var oldRow = e.OldRow as UsrPMCostReassignmentDestination;
            if (row != null && oldRow != null && (row.ReassignmentValue1 != oldRow.ReassignmentValue1 || row.ReassignmentValue2 != oldRow.ReassignmentValue2))
            {
                SubstractTotals(oldRow);
                AddTotals(row);
            }
        }

        protected virtual void UsrPMCostReassignmentDestination_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (UsrPMCostReassignmentDestination)e.Row;
            row.TaskID = null;
            row.ReassignmentSelection = ReassignmentSelectionAttribute.Values.UnitCount;
            row.ReassignmentValue1 = 0;
            row.ReassignmentValue2 = 0;
        }

        protected virtual void UsrPMCostReassignmentDestination_TaskID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (UsrPMCostReassignmentDestination)e.Row;            
            row.ReassignmentSelection = ReassignmentSelectionAttribute.Values.UnitCount;
            row.ReassignmentValue1 = 0;
            row.ReassignmentValue2 = 0;
        }

        protected virtual void UsrPMCostReassignmentDestination_ReassignmentValue1_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            VerifyingReassignmentValue1MoreThanZero(e);
        }

        protected virtual void UsrPMCostReassignmentDestination_ReassignmentValue2_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            VerifyingReassignmentValue2MoreThanZero(e);
        }

        protected virtual void UsrPMCostReassignmentDestination_TaskID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentDestination;
            if (row != null)
            {
                var newTaskId = e.NewValue as int?;
                var usedTaskIDs = PMCostReassignmentSource.Select().Select(i => ((UsrPMCostReassignmentSource)i).TaskID);
                if (usedTaskIDs.Any(i => i.GetValueOrDefault() == newTaskId.GetValueOrDefault()))
                {
                    throw new PXSetPropertyException(PMCostReassignmentMessages.SAME_PROJECT_TASK_CANNOT_BE_USED_AS_A_SOURCE_AND_AS_A_DESTINATION_TASK_IN_A_SINGLE_REASSIGNMENT, PXErrorLevel.RowError);
                }

                VerifyingPMTaskUseInOtherReassignments(row.PMReassignmentID, newTaskId);
            }
        }

        #endregion

        #endregion

        #region Public
        public void ProcessReassigment(PMCostReassigmentEntity entity)
        {
            PMCostReassignment.Current = PMCostReassignment.Search<UsrPMCostReassignment.pMReassignmentID>(entity.PMReassignmentID);
            CalcutaleReassignmentValueTotals(PMCostReassignment.Current);
            var registerEntry = CreateInstance<RegisterEntry>();
            var reassignmentEntry = this;

            using (var ts = new PXTransactionScope())
            {
                // === Transaction scope ===
                //1. Create PMRegister
                //2. Process each reassignmentEntity row and create transactions
                //3. Checking whether to save or not. Save changes
                //4. Write data information to history and log tables
                //5. Mark source transations as reassigned
                //6. Release PMRegister

                using (var insertPMRegister = new AddPMRegisterHandler(reassignmentEntry, registerEntry, entity))
                {
                    var insertPMTrans = new AddPMTransHandler(reassignmentEntry, registerEntry, entity);
                    var savePMRegister = new SavePMRegisterHandler(reassignmentEntry, registerEntry, entity);
                    var writeLogData = new WriteLogsDataHandler(reassignmentEntry, registerEntry, entity);
                    var writeReassignedToken = new WriteReassignedTokenHandler(reassignmentEntry, registerEntry, entity);
                    var writeReleasedToken = new WriteReleasedTokenHandler(reassignmentEntry, registerEntry, entity);

                    /*1*/
                    insertPMRegister.Successor = insertPMTrans;
                    /*2*/
                    insertPMTrans.Successor = savePMRegister;
                    /*3*/
                    savePMRegister.Successor = writeLogData;
                    /*4*/
                    writeLogData.Successor = writeReassignedToken;
                    /*5*/
                    writeReassignedToken.Successor = writeReleasedToken;
                    /*6*/
                    insertPMRegister.ExecuteTransaction();                    
                }

                ts.Complete();
            }
            
            //Save 
            reassignmentEntry.Actions.PressSave();
            
            //Clear
            registerEntry.Clear();
            reassignmentEntry.Clear();

        }
        #endregion

        #region Private

        private void CalcutaleReassignmentValueTotals(UsrPMCostReassignment row)
        {                       
            if (row == null) return;
            row.ReassignmentValue1Total = 0;
            row.ReassignmentValue2Total = 0;

            using (new PXConnectionScope())
            {
                var destinations = PXSelectJoin<UsrPMCostReassignmentDestination, 
                                        InnerJoin<PMTask, On<PMTask.taskID, Equal<UsrPMCostReassignmentDestination.taskID>>>, 
                                            Where<PMTask.status, NotEqual<ProjectTaskStatus.completed>,
                                            And<UsrPMCostReassignmentDestination.pMReassignmentID, Equal<Required<UsrPMCostReassignmentDestination.pMReassignmentID>>>>>.Select(this, row.PMReassignmentID);

                foreach (UsrPMCostReassignmentDestination dest in destinations)
                {
                    row.ReassignmentValue1Total += dest.ReassignmentValue1;
                    row.ReassignmentValue2Total += dest.ReassignmentValue2;
                }
            }           
        }

        private void AddTotals(UsrPMCostReassignmentDestination row)
        {
            PMTask task = PMNotCompletedTaskSelect.Select(row.TaskID);
            var parentRow = PMCostReassignment.Current;
            if (parentRow != null && task != null)
            {
                parentRow.ReassignmentValue1Total = parentRow.ReassignmentValue1Total + row.ReassignmentValue1.GetValueOrDefault();
                parentRow.ReassignmentValue2Total = parentRow.ReassignmentValue2Total + row.ReassignmentValue2.GetValueOrDefault();
            }
        }
        private void SubstractTotals(UsrPMCostReassignmentDestination row)
        {
            PMTask task = PMNotCompletedTaskSelect.Select(row.TaskID);
            var parentRow = PMCostReassignment.Current;
            if (parentRow != null && task != null)
            {
                parentRow.ReassignmentValue1Total = parentRow.ReassignmentValue1Total - row.ReassignmentValue1.GetValueOrDefault();
                parentRow.ReassignmentValue2Total = parentRow.ReassignmentValue2Total - row.ReassignmentValue2.GetValueOrDefault();
            }
        }


        private string GetPMSetupUsrReassignmentNumberingID()
        {
            PMSetup setup = PMSetupSelect.Select();
            var value = setup.GetExtension<PMSetupExt>()?.UsrReassignmentNumberingID;
            return value;
        }

        private void VerifyingPMTaskUseInOtherReassignments(string pmReassignmentID, int? taskID)
        {
            var sourceReassignments = new PXSelect<UsrPMCostReassignmentSource, 
                                             Where<UsrPMCostReassignmentSource.taskID, Equal<Required<UsrPMCostReassignmentSource.taskID>>, 
                                               And<UsrPMCostReassignmentSource.pMReassignmentID, NotEqual<Required<UsrPMCostReassignmentSource.pMReassignmentID>>>>>(this);

            var destReassignments = new PXSelect<UsrPMCostReassignmentDestination, 
                                           Where<UsrPMCostReassignmentDestination.taskID, Equal<Required<UsrPMCostReassignmentDestination.taskID>>, 
                                             And<UsrPMCostReassignmentDestination.pMReassignmentID, NotEqual<Required<UsrPMCostReassignmentDestination.pMReassignmentID>>>>>(this);

            PMTask taskRow = PMTaskSelect.Search<PMTask.taskID>(taskID);

            var listIDs = new List<string>();

            foreach (UsrPMCostReassignmentSource sourceReassignment in sourceReassignments.Select(taskID, pmReassignmentID))
            {
                listIDs.Add(sourceReassignment.PMReassignmentID);
            }

            foreach (UsrPMCostReassignmentDestination destReassignment in destReassignments.Select(taskID, pmReassignmentID))
            {
                listIDs.Add(destReassignment.PMReassignmentID);
            }

            if (listIDs.Any())
            {
                throw new PXSetPropertyException(string.Format(PMCostReassignmentMessages.PROJECT_TASK_ALREADY_USED, taskRow.TaskCD, string.Join(",", listIDs)), PXErrorLevel.Warning);
            }
        }

        private static void VerifyingReassignmentValue1MoreThanZero(PXFieldVerifyingEventArgs e)
        {
            var reassignmentValue1 = e.NewValue as decimal?;
            if (reassignmentValue1.GetValueOrDefault() < 0)
            {
                throw new PXSetPropertyException(PMCostReassignmentMessages.REASSIGNMENT_VALUE_ONE_CANNOT_BE_NEGATIVE, PXErrorLevel.Warning);
            }
        }

        private static void VerifyingReassignmentValue2MoreThanZero(PXFieldVerifyingEventArgs e)
        {
            var reassignmentValue2 = e.NewValue as int?;
            if (reassignmentValue2.GetValueOrDefault() < 0)
            {
                throw new PXSetPropertyException(PMCostReassignmentMessages.REASSIGNMENT_VALUE_TWO_CANNOT_BE_NEGATIVE, PXErrorLevel.Warning);
            }
        }

        #endregion

    }
}