using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectCostReallocation.BL;
using ProjectCostReallocation.DAC;
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
                LeftJoin<PMTask,On<PMTask.taskID, Equal<UsrPMCostReassignmentSource.taskID>, And<PMTask.projectID, Equal<UsrPMCostReassignmentSource.projectID>>>>>,
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
        public PXSelectJoin<PMTran,
                    InnerJoin<UsrPMCostReassignmentHistory, On<PMTran.tranID, Equal<UsrPMCostReassignmentHistory.tranID>>, 
                    LeftJoin<PMRegister, On<PMRegister.refNbr, Equal<PMTran.refNbr>, And<PMRegister.module, Equal<PMTran.tranType>>>>>,                    
                Where<UsrPMCostReassignmentHistory.pMReassignmentID, Equal<Current<UsrPMCostReassignment.pMReassignmentID>>>> PMCostReassignmentHistory;

        [PXCopyPasteHiddenView]
        public PXSelect<UsrPMCostReassignmentRunHistory, Where<UsrPMCostReassignmentRunHistory.pMReassignmentID, Equal<Current<UsrPMCostReassignment.pMReassignmentID>>, 
                                                           And<UsrPMCostReassignmentRunHistory.revID, Equal<Current<UsrPMCostReassignment.revID>>,
                                                           And<UsrPMCostReassignmentRunHistory.destinationTaskID, Equal<Required<UsrPMCostReassignmentRunHistory.destinationTaskID>>,
                                                           And<UsrPMCostReassignmentRunHistory.destinationTranID, Equal<Required<UsrPMCostReassignmentRunHistory.destinationTranID>>>>>>> RunHistory;

        [PXCopyPasteHiddenView]
        public PXSelect<PMTran, Where<PMTran.projectID, Equal<Required<PMTran.projectID>>, And<PMTran.taskID, Equal<PMTran.taskID>>>> Tran;

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
                ViewProjectCommon(currentProjectID);
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
                ViewTaskCommon(currentTaskID);
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
                ViewProjectCommon(currentProjectID);
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
                ViewTaskCommon(currentTaskID);
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
                ViewProjectCommon(currentProjectID);
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
                ViewTaskCommon(currentTaskID);
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

        #region Event handlers

        #region UsrPMCostReassignment

        protected virtual void UsrPMCostReassignment_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            var row = (UsrPMCostReassignment) e.Row;
            if (!CheckIsAutonumberingConfigured(sender, row))
            {
                e.Cancel = true;
            }
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

                //no need to calculate when doing import. It will just slow down the import.
                if (!IsImport)
                {
                    row.ReassignmentValue1Total = 0;
                    row.ReassignmentValue2Total = 0;
                                       
                    foreach (var result in PMCostReassignmentDestination.Select())
                    {
                        if (VerifyDestinationRowProcessed(result)) continue;
                        var dest = result.GetItem<UsrPMCostReassignmentDestination>();
                        row.ReassignmentValue1Total += dest.ReassignmentValue1;
                        row.ReassignmentValue2Total += dest.ReassignmentValue2;
                    }
                }
            }
        }

      

      protected virtual void UsrPMCostReassignment_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            var row = (UsrPMCostReassignment) e.Row;
            if (!CheckIsAutonumberingConfigured(sender, row))
            {
                e.Cancel = true;
            }
        }

        protected virtual void UsrPMCostReassignment_Active_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var row = e.Row as UsrPMCostReassignment;
                if (row != null && (row.ReassignmentValue1Total.GetValueOrDefault() == 0 && row.ReassignmentValue2Total == 0))
                {
                    e.NewValue = false;
                    e.Cancel = true;
                    throw new PXSetPropertyException(PMCostReassignmentMessages.SOURCE_AND_DESTINATION_TOTALS_MUST_BE_BALANCED_BEFORE_ACTIVATING_THE_REASSIGNMENT);
                }                
            }
        }

        protected virtual void UsrPMCostReassignment_Active_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignment;
            if (row?.CreatedByID != null && row.Active.GetValueOrDefault() == false && (bool)e.NewValue && PMCostReassignmentHistory.Select().Any() )
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

        protected virtual void UsrPMCostReassignmentSource_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
            {
                SetParentID<UsrPMCostReassignmentSource>(e);
            }
        }

        protected virtual void UsrPMCostReassignmentSource_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (UsrPMCostReassignmentSource)e.Row;
            row.TaskID = null;
            row.AccountGroupFrom = null;
            row.AccountGroupTo = null;
        }

        protected virtual void UsrPMCostReassignmentSource_TaskID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            var newTaskId = e.NewValue as int?;
            var usedTaskIDs = PMCostReassignmentDestination.Select().Select(i => ((UsrPMCostReassignmentDestination)i).TaskID);
            if (usedTaskIDs.Any(i => i.GetValueOrDefault() == newTaskId.GetValueOrDefault()))
            {
                e.Cancel = true;
                e.NewValue = ((UsrPMCostReassignmentSource)e.Row).TaskID;
                throw new PXSetPropertyException(PMCostReassignmentMessages.SAME_PROJECT_TASK_CANNOT_BE_USED_AS_A_SOURCE_AND_AS_A_DESTINATION_TASK_IN_A_SINGLE_REASSIGNMENT, PXErrorLevel.Error);
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

        protected virtual void UsrPMCostReassignmentDestination_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (UsrPMCostReassignmentDestination)e.Row;
            row.TaskID = null;
            row.ReassignmentSelection = "C";
        }

        protected virtual void UsrPMCostReassignmentDestination_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
            {
                SetParentID<UsrPMCostReassignmentDestination>(e);
            }
        }

        protected virtual void UsrPMCostReassignmentDestination_SquareFootage_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentDestination;
            if (row != null)
            {                
                VerifyingPMTaskUseInOtherReassignments(row);
            }
        }

        protected virtual void UsrPMCostReassignmentDestination_NumberOfUnits_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentDestination;
            if (row != null)
            {                
                VerifyingPMTaskUseInOtherReassignments(row);
            }
        }

        protected virtual void UsrPMCostReassignmentDestination_SquareFootage_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            VerifyingSquareFootageMoreThanZero(e);
        }

        protected virtual void UsrPMCostReassignmentDestination_NumberOfUnits_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            VerifyingNumberOfUnitsMoreThanZero(e);
        }

        protected virtual void UsrPMCostReassignmentDestination_TaskID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            var newTaskId = e.NewValue as int?;
            var usedTaskIDs = PMCostReassignmentSource.Select().Select(i => ((UsrPMCostReassignmentSource)i).TaskID);
            if (usedTaskIDs.Any(i => i.GetValueOrDefault() == newTaskId.GetValueOrDefault()))
            {
                e.Cancel = true;
                e.NewValue = ((UsrPMCostReassignmentDestination)e.Row).TaskID;
                throw new PXSetPropertyException(PMCostReassignmentMessages.SAME_PROJECT_TASK_CANNOT_BE_USED_AS_A_SOURCE_AND_AS_A_DESTINATION_TASK_IN_A_SINGLE_REASSIGNMENT, PXErrorLevel.Error);
            }
        }

        #endregion

        #endregion

        #region Private

        private bool VerifyDestinationRowProcessed(PXResult<UsrPMCostReassignmentDestination> result)
        {
            //Check task is completed
            var task = result.GetItem<PMTask>();
            return task != null && task.Status == ProjectTaskStatus.Completed;
        }

        private bool CheckIsAutonumberingConfigured(PXCache sender, UsrPMCostReassignment row)
        {
            //Check PMsetup value for autonumbering
            var value = GetPMSetupUsrReassignmentNumberingID();
            if (string.IsNullOrEmpty(value))
            {
                row.PMReassignmentID = null;
                sender.RaiseExceptionHandling<UsrPMCostReassignment.pMReassignmentID>(row, null, new PXSetPropertyException(PMCostReassignmentMessages.REASSIGNMENT_NUMBERING_SEQUENCE_VALUE_NOT_CONFIGURED, PXErrorLevel.Error));
                return false;
            }
            return true;
        }

        private string GetPMSetupUsrReassignmentNumberingID()
        {
            PMSetup setup = PXSelect<PMSetup>.Select(this);
            var value = setup.GetExtension<PMSetupExt>()?.UsrReassignmentNumberingID;
            return value;
        }

        private static void ViewProjectCommon(int? currentProjectID)
        {
            var graph = CreateInstance<ProjectEntry>();
            graph.Project.Current = graph.Project.Search<PMProject.contractID>(currentProjectID);
            if (graph.Project.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, Messages.ViewProject) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
        }

        private static void ViewTaskCommon(int? currentTaskID)
        {
            var graph = CreateInstance<ProjectTaskEntry>();
            graph.Task.Current = graph.Task.Search<PMTask.taskID>(currentTaskID);
            throw new PXRedirectRequiredException(graph, true, Messages.ViewTask) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
        }

        private void SetParentID<T>(PXRowPersistingEventArgs e) where T : IUsrPMCostReassignmentProjectAndTask
        {
            var row = (T)e.Row;
            if (row != null && PMCostReassignment.Current != null)
            {
                if (!row.PMReassignmentID.Equals(PMCostReassignment.Current.PMReassignmentID))
                {
                    row.PMReassignmentID = PMCostReassignment.Current.PMReassignmentID;
                }
            }            
        }        

        private void VerifyingPMTaskUseInOtherReassignments(IUsrPMCostReassignmentProjectAndTask row)
        {
            var sourceReassignments = new PXSelect<UsrPMCostReassignmentSource, Where<UsrPMCostReassignmentSource.taskID, Equal<Required<UsrPMCostReassignmentSource.taskID>>>>(this);
            var destReassignments = new PXSelect<UsrPMCostReassignmentDestination, Where<UsrPMCostReassignmentDestination.taskID, Equal<Required<UsrPMCostReassignmentDestination.taskID>>>>(this);
            if (row != null)
            {
                PMTask taskRow = PMTaskSelect.Search<PMTask.taskID>(row.TaskID);
                var sources = sourceReassignments.Select(row.TaskID).Select(i => ((UsrPMCostReassignmentSource)i).PMReassignmentID).Where(i => i != row.PMReassignmentID).ToList();
                var dests = destReassignments.Select(row.TaskID).Select(i => ((UsrPMCostReassignmentDestination)i).PMReassignmentID).Where(i => i != row.PMReassignmentID).ToList();
                if (sources.Any() || dests.Any())
                {
                    var listIDs = new List<string>();
                    listIDs.AddRange(sources);
                    listIDs.AddRange(dests);
                    throw new PXSetPropertyException(string.Format(PMCostReassignmentMessages.PROJECT_TASK_ALREADY_USED, taskRow.TaskCD, string.Join(",", listIDs)), PXErrorLevel.Warning);

                }
            }
        }

        private static void VerifyingSquareFootageMoreThanZero(PXFieldVerifyingEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentDestination;
            var square = e.NewValue as decimal?;
            if (square.GetValueOrDefault() < 0)
            {
                e.Cancel = true;
                e.NewValue = row?.ReassignmentValue1;
                throw new PXSetPropertyException(PMCostReassignmentMessages.SQUARE_FOOTAGE_VALUE_CANNOT_BE_NEGATIVE, PXErrorLevel.Warning);
            }
        }

        private static void VerifyingNumberOfUnitsMoreThanZero(PXFieldVerifyingEventArgs e)
        {
            var row = e.Row as UsrPMCostReassignmentDestination;
            var numberOfUnits = e.NewValue as int?;
            if (numberOfUnits.GetValueOrDefault() < 0)
            {
                e.Cancel = true;
                e.NewValue = row?.ReassignmentValue2;
                throw new PXSetPropertyException(PMCostReassignmentMessages.UNIT_COUNT_VALUE_CANNOT_BE_NEGATIVE, PXErrorLevel.Warning);
            }
        }

        #endregion

    }
}