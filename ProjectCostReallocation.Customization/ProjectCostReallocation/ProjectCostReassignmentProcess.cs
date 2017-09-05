using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectCostReallocation.ProjectCostReallocation.Helpers;
using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
  public class ProjectCostReassignmentProcess : PXGraph<ProjectCostReassignmentProcess>
  {
        #region Selects

        public PXCancel<PMCostReassignmentFilter> Cancel;
       
        public PXFilter<PMCostReassignmentFilter> Filter;

        public PXFilteredProcessing<PMCostReassigmentEntity, PMCostReassignmentFilter> Items; 

        public PXSelect<PMProject> PMProjectSelect;

        public PXSelect<PMTask> PMTaskSelect;

        public virtual IEnumerable items()
        {
            var filter = Filter.Current;
            if (filter == null)
            {
                yield break;
            }
            var found = false;
            foreach (PMCostReassigmentEntity item in Items.Cache.Inserted)
            {
                found = true;
                yield return item;
            }
            if (found)
                yield break;

            var source = new PXSelectJoin<UsrPMCostReassignmentSource,
                            InnerJoin<UsrPMCostReassignment, On<UsrPMCostReassignmentSource.pMReassignmentID, Equal<UsrPMCostReassignment.pMReassignmentID>>,
                            InnerJoin<PMProject, On<PMProject.contractID, Equal<UsrPMCostReassignmentSource.projectID>>,
                            InnerJoin<PMTask, On<PMTask.projectID, Equal<PMProject.contractID>, And<PMTask.taskID, Equal<UsrPMCostReassignmentSource.taskID>>>>>>,
                            Where<UsrPMCostReassignment.active, Equal<True>>>(this);

            var destination = new PXSelectJoin<UsrPMCostReassignmentDestination,
                                InnerJoin<UsrPMCostReassignment, On<UsrPMCostReassignmentDestination.pMReassignmentID, Equal<UsrPMCostReassignment.pMReassignmentID>>,
                                InnerJoin<PMProject, On<PMProject.contractID, Equal<UsrPMCostReassignmentDestination.projectID>>,
                                InnerJoin<PMTask, On<PMTask.projectID, Equal<PMProject.contractID>, And<PMTask.taskID, Equal<UsrPMCostReassignmentDestination.taskID>, And<PMTask.status, NotEqual<ProjectTaskStatus.completed>>>>>>>,
                                Where<UsrPMCostReassignment.active, Equal<True>>>(this);

            //Set where condition by filter - source
            if (filter.FromProjectID != null)
            {
                source.WhereAnd<Where<PMProject.contractID, Equal<Current<PMCostReassignmentFilter.fromProjectID>>>>();
            }

            if (filter.FromTaskID != null)
            {
                source.WhereAnd<Where<PMTask.taskID, Equal<Current<PMCostReassignmentFilter.fromTaskID>>>>();
            }

            if (filter.FromProjectStatus != null)
            {
                source.WhereAnd<Where<PMProject.status, Equal<Current<PMCostReassignmentFilter.fromProjectStatus>>>>();
            }

            if (filter.FromTaskStatus != null)
            {
                source.WhereAnd<Where<PMTask.status, Equal<Current<PMCostReassignmentFilter.fromTaskStatus>>>>();
            }


            //Set where condition by filter - destination
            if (filter.ToProjectID != null)
            {
                destination.WhereAnd<Where<PMProject.contractID, Equal<Current<PMCostReassignmentFilter.toProjectID>>>>();
            }

            if (filter.ToTaskID != null)
            {
                destination.WhereAnd<Where<PMTask.taskID, Equal<Current<PMCostReassignmentFilter.toTaskID>>>>();
            }

            if (filter.ToProjectStatus != null)
            {
                destination.WhereAnd<Where<PMProject.status, Equal<Current<PMCostReassignmentFilter.toProjectStatus>>>>();
            }

            if (filter.ToTaskStatus != null)
            {
                destination.WhereAnd<Where<PMTask.status, Equal<Current<PMCostReassignmentFilter.toTaskStatus>>>>();
            }

            var key = 0;

            foreach (var sourceItem in source.Select())
            {
                var reassignment = PXResult.Unwrap<UsrPMCostReassignment>(sourceItem);
                var reassignmentSource = PXResult.Unwrap<UsrPMCostReassignmentSource>(sourceItem);
                var sourceProject = PXResult.Unwrap<PMProject>(sourceItem);
                var sourceTask = PXResult.Unwrap<PMTask>(sourceItem);

                foreach (var destinationItem in destination.Select().Where(d => d.GetItem<UsrPMCostReassignmentDestination>().PMReassignmentID == sourceItem.GetItem<UsrPMCostReassignmentSource>().PMReassignmentID))
                {
                    var reassignmentDestinaiton = PXResult.Unwrap<UsrPMCostReassignmentDestination>(destinationItem);
                    var destinationProject = PXResult.Unwrap<PMProject>(destinationItem);
                    var destinationTask = PXResult.Unwrap<PMTask>(destinationItem);

                    var resultRow = new PMCostReassigmentEntity
                    {
                        LineID = key++,
                        ReassignmentDate = filter.ReassignmentDate,
                        ReassignmentFinPeriodID = filter.ReassignmentFinPeriodID,
                        SourceLineID = reassignmentSource.LineID,
                        PMReassignmentID = reassignment.PMReassignmentID,
                        RevID = reassignment.RevID,
                        SourceProjectID = sourceProject.ContractID,
                        SourceProjectDescription = sourceProject.Description,
                        SourceTaskID = sourceTask.TaskID,
                        SourceTaskDescription = sourceTask.Description,
                        SourceTaskStartDate = sourceTask.StartDate,
                        SourceTaskEndDate = sourceTask.EndDate,
                        DestinaitonLineID = reassignmentDestinaiton.LineID,
                        DestinationProjectID = destinationProject.ContractID,
                        DestinationProjectDescription = destinationProject.Description,
                        DestinationTaskID = destinationTask.TaskID,
                        DestinationTaskDescription = destinationTask.Description,
                        DestinationTaskStartDate = destinationTask.StartDate,
                        DestinationTaskEndDate = destinationTask.EndDate,
                        NeedToProcessed = true
                    };

                    if (Items.Locate(resultRow) == null)
                    {
                        yield return Items.Insert(resultRow);
                    }
                }
            }

            Items.Cache.IsDirty = false;
        }

        #endregion

        #region Cache attached

        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Project Name")]
        protected virtual void PMProject_Description_CacheAttached(PXCache sender)
        {
        }

        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Project Status")]
        protected virtual void PMProject_Status_CacheAttached(PXCache sender)
        {
        }

        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Task Name")]
        protected virtual void PMTask_Description_CacheAttached(PXCache sender)
        {
        }

        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Task Status")]
        protected virtual void PMTask_Status_CacheAttached(PXCache sender)
        {
        }

        #endregion

        #region .ctor
        public ProjectCostReassignmentProcess()
        {
            // Specifying the field to mark data records for processing
            Items.SetSelected<PMCostReassigmentEntity.selected>();
            Items.SetProcessDelegate(ProcessReassignments);
        } 
        #endregion

        #region Event handlers

        protected virtual void PMCostReassignmentFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            if (!sender.ObjectsEqual<PMCostReassignmentFilter.fromProjectID, PMCostReassignmentFilter.fromTaskID,
                PMCostReassignmentFilter.fromProjectStatus, PMCostReassignmentFilter.fromTaskStatus,
                PMCostReassignmentFilter.toProjectID, PMCostReassignmentFilter.toTaskID,
                PMCostReassignmentFilter.toProjectStatus, PMCostReassignmentFilter.toTaskStatus>(e.Row, e.OldRow))
            {
                Items.Cache.Clear();
            }
        }

        protected virtual void PMCostReassignmentFilter_FromProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (PMCostReassignmentFilter) e.Row;
            if (row != null)
            {
                row.FromProjectStatus = null;
                row.FromTaskID = null;
                row.FromTaskStatus = null;
                Filter.Cache.ClearItemAttributes();
            }
        }

        protected virtual void PMCostReassignmentFilter_ToProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (PMCostReassignmentFilter) e.Row;
            if (row != null)
            {
                row.ToProjectStatus = null;
                row.ToTaskID = null;
                row.ToTaskStatus = null;
                Filter.Cache.ClearItemAttributes();
            }
        }

        protected virtual void PMCostReassignmentFilter_FromTaskID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (PMCostReassignmentFilter) e.Row;
            if (row != null)
            {
                row.FromTaskStatus = null;
            }
        }

        protected virtual void PMCostReassignmentFilter_ToTaskID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (PMCostReassignmentFilter)e.Row;
            if (row != null)
            {
                row.ToTaskStatus = null;
            }
        }
        #endregion

        #region Overrides
        public override bool IsDirty => false;
        #endregion
        
        #region Process
        private static void ProcessReassignments(List<PMCostReassigmentEntity> reassigmentList)
        {
            
            var catchedExceptions = new List<Exception>();

            foreach (var reassigmentPair in reassigmentList)
            {
                var reassignmentGraph = CreateInstance<ProjectCostReassignmentEntry>();
                try
                {
                    reassignmentGraph.ProcessReassigment(reassigmentPair);
                }
                catch (Exception e)
                {
                    catchedExceptions.Add(e);   
                }
                
            }

            //Set rows stat
            foreach (var entity in reassigmentList)
            {
                if (entity.ProcessingError.GetValueOrDefault())
                {
                    PXProcessing<PMCostReassigmentEntity>.SetError(reassigmentList.IndexOf(entity), entity.ProcessingMessage);
                }

                if (entity.ProcessingWarning.GetValueOrDefault() || entity.NeedToProcessed.GetValueOrDefault())
                {
                    PXProcessing<PMCostReassigmentEntity>.SetWarning(reassigmentList.IndexOf(entity), entity.ProcessingMessage);
                }

                if (entity.ProcessingSuccess.GetValueOrDefault())
                {
                    PXProcessing<PMCostReassigmentEntity>.SetInfo(reassigmentList.IndexOf(entity), entity.ProcessingMessage);
                }
            }

            if (catchedExceptions.Any() || reassigmentList.Any(e => e.ProcessingError.GetValueOrDefault()))
            {
                throw new PXException(PMCostReassignmentMessages.REASSIGNMENT_NOT_SUCCESS);
            }

        }
        #endregion

        #region Actions

        public PXAction<PMCostReassignmentFilter> viewProjectSource;
        [PXUIField(DisplayName = Messages.ViewProject, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewProjectSource(PXAdapter adapter)
        {
            if (Items.Current != null)
            {
                var currentProjectID = Items.Current.SourceProjectID;
                PMCostReassignmentViewer.ViewProjectCommon(currentProjectID);
            }

            return adapter.Get();
        }

        public PXAction<PMCostReassignmentFilter> viewTaskSource;
        [PXUIField(DisplayName = Messages.ViewTask, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewTaskSource(PXAdapter adapter)
        {
            if (Items.Current != null)
            {
                var currentTaskID = Items.Current.SourceTaskID;
                PMCostReassignmentViewer.ViewTaskCommon(currentTaskID);
            }

            return adapter.Get();
        }

        public PXAction<PMCostReassignmentFilter> viewProjectDestination;
        [PXUIField(DisplayName = Messages.ViewProject, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewProjectDestination(PXAdapter adapter)
        {
            if (Items.Current != null)
            {
                var currentProjectID = Items.Current.DestinationProjectID;
                PMCostReassignmentViewer.ViewProjectCommon(currentProjectID);
            }

            return adapter.Get();
        }

        public PXAction<PMCostReassignmentFilter> viewTaskDestination;
        [PXUIField(DisplayName = Messages.ViewTask, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewTaskDestination(PXAdapter adapter)
        {
            if (Items.Current != null)
            {
                var currentTaskID = Items.Current.DestinationTaskID;
                PMCostReassignmentViewer.ViewTaskCommon(currentTaskID);
            }

            return adapter.Get();
        }


        public PXAction<PMCostReassignmentFilter> viewReassignment;
        [PXUIField(DisplayName = "View Reassignment", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable ViewReassignment(PXAdapter adapter)
        {
            if (Items.Current != null)
            {
                var currentPMReassignmentID = Items.Current.PMReassignmentID;
                var graph = CreateInstance<ProjectCostReassignmentEntry>();
                graph.PMCostReassignment.Current = graph.PMCostReassignment.Search<UsrPMCostReassignment.pMReassignmentID>(currentPMReassignmentID);
                throw new PXRedirectRequiredException(graph, true, Messages.ViewTask) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }

            return adapter.Get();
        }
        #endregion     
    }
}