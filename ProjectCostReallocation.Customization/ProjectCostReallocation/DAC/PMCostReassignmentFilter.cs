using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    [Serializable]
    public class PMCostReassignmentFilter : IBqlTable
    {
        #region ReassignmentDate

        public abstract class reassignmentDate : IBqlField
        {
        }
        protected DateTime? _ReassignmentDate;
        [PXDate]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Reassignment Date", Visibility = PXUIVisibility.Visible, Required = false)]
        public virtual DateTime? ReassignmentDate
        {
            get
            {
                return _ReassignmentDate;
            }
            set
            {
                _ReassignmentDate = value;
            }
        }
        #endregion
        #region InvFinPeriodID

        public abstract class reassignmentFinPeriodID : IBqlField
        {
        }
        protected string _ReassignmentFinPeriodID;
        [OpenPeriod(typeof(BillingProcess.BillingFilter.invoiceDate))]
        [PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.Visible, Required = false)]
        public virtual string ReassignmentFinPeriodID
        {
            get
            {
                return _ReassignmentFinPeriodID;
            }
            set
            {
                _ReassignmentFinPeriodID = value;
            }
        }
        #endregion

        #region FromProjectID

        public abstract class fromProjectID : IBqlField
        {
        }
        protected int? _FromProjectID;
        [PXInt]
        [PXUIField(DisplayName = "From Project")]            
        [PXSelector(typeof(Search<PMProject.contractID, Where<PMProject.baseType, Equal<PMProject.ProjectBaseType>, And<PMProject.isTemplate, NotEqual<True>>>>),
            typeof(PMProject.contractCD),
            typeof(PMProject.description),
            SubstituteKey = typeof(PMProject.contractCD), DirtyRead = true)]
        public virtual int? FromProjectID
        {
            get
            {
                return _FromProjectID;
            }
            set
            {
                _FromProjectID = value;
            }
        }
        #endregion
        #region FromProjectStatus

        public abstract class fromProjectStatus : IBqlField
        {
        }

        protected string _FromProjectStatus;            
        [PXString(1, IsFixed = true)]
        [ProjectStatus.List]            
        [PXUIField(DisplayName = "From Project Status")]
        public string FromProjectStatus
        {
            get
            {
                return _FromProjectStatus;
            }
            set
            {
                _FromProjectStatus = value;
            }
        }
        #endregion
        #region FromTaskID

        public abstract class fromTaskID : IBqlField
        {
        }
        protected int? _FromTaskID;
        [PXInt]
        [PXUIField(DisplayName = "From Task", Visibility = PXUIVisibility.SelectorVisible)]            
        [PXSelector(typeof(Search<PMTask.taskID, Where<PMTask.projectID, Equal<Current<fromProjectID>>>>),
            typeof(PMTask.taskCD),
            typeof(PMTask.description),
            SubstituteKey = typeof(PMTask.taskCD), DirtyRead = true)]
        public virtual int? FromTaskID
        {
            get
            {
                return _FromTaskID;
            }
            set
            {
                _FromTaskID = value;
            }
        }
        #endregion
        #region FromTaskStatus

        public abstract class fromTaskStatus : IBqlField
        {
        }
        protected string _FromTaskStatus;           
        [PXString(1, IsFixed = true)]
        [ProjectTaskStatus.List]
        [PXUIField(DisplayName = "From Task Status")]
        public virtual string FromTaskStatus
        {
            get
            {
                return _FromTaskStatus;
            }
            set
            {
                _FromTaskStatus = value;
            }
        }
        #endregion

        #region ToProjectID

        public abstract class toProjectID : IBqlField
        {
        }
        protected int? _ToProjectID;
        [PXInt]
        [PXUIField(DisplayName = "To Project")]
        [PXSelector(typeof(Search<PMProject.contractID, Where<PMProject.baseType, Equal<PMProject.ProjectBaseType>, And<PMProject.isTemplate, NotEqual<True>>>>),
            typeof(PMProject.contractCD),
            typeof(PMProject.description),
            SubstituteKey = typeof(PMProject.contractCD))]
        public virtual int? ToProjectID
        {
            get
            {
                return _ToProjectID;
            }
            set
            {
                _ToProjectID = value;
            }
        }
        #endregion
        #region ToProjectStatus

        public abstract class toProjectStatus : IBqlField
        {
        }

        protected string _ToProjectStatus;
        [PXString(1, IsFixed = true)]
        [ProjectStatus.List]
        [PXUIField(DisplayName = "To Project Status")]
        public string ToProjectStatus
        {
            get
            {
                return _ToProjectStatus;
            }
            set
            {
                _ToProjectStatus = value;
            }
        }
        #endregion
        #region ToTaskID

        public abstract class toTaskID : IBqlField
        {
        }
        protected int? _ToTaskID;
        [PXInt]
        [PXUIField(DisplayName = "To Task", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<PMTask.taskID, Where<PMTask.projectID, Equal<Current<toProjectID>>>>),
            typeof(PMTask.taskCD),
            typeof(PMTask.description),
            SubstituteKey = typeof(PMTask.taskCD))]
        public virtual int? ToTaskID
        {
            get
            {
                return _ToTaskID;
            }
            set
            {
                _ToTaskID = value;
            }
        }
        #endregion
        #region ToTaskStatus

        public abstract class toTaskStatus : IBqlField
        {
        }
        protected string _ToTaskStatus;
        [PXString(1, IsFixed = true)]
        [ProjectTaskStatus.List]
        [PXUIField(DisplayName = "To Task Status")]
        public virtual string ToTaskStatus
        {
            get
            {
                return _ToTaskStatus;
            }
            set
            {
                _ToTaskStatus = value;
            }
        }
        #endregion
    }
}