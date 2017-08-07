using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.GL;
using PX.Objects.PM;

namespace ProjectCostReallocation.DAC
{
    [Serializable]
    public class PMCostReassigmentEntity: IBqlTable 
    {
        #region Selected
        public abstract class selected : IBqlField
        { }
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion
        #region LineID
        public abstract class lineID : IBqlField
        {
        }
        protected int? _LineID;
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual int? LineID
        {
            get
            {
                return _LineID;
            }
            set
            {
                _LineID = value;
            }
        }
        #endregion
        #region ReassignmentDate

        public abstract class reassignmentDate : IBqlField
        {
        }
        protected DateTime? _ReassignmentDate;
        [PXDBDate]
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
        #region SourceLineID
        public abstract class sourceLineID : IBqlField
        {
        }
        protected int? _SourceLineID;
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Source Line Nbr.", Visible = false)]
        public virtual int? SourceLineID
        {
            get
            {
                return _SourceLineID;
            }
            set
            {
                _SourceLineID = value;
            }
        }
        #endregion
        #region DestinaitonLineID
        public abstract class destinationLineID : IBqlField
        {
        }
        protected int? _DestinaitonLineID;
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Destinaiton Line Nbr.", Visible = false)]
        public virtual int? DestinaitonLineID
        {
            get
            {
                return _DestinaitonLineID;
            }
            set
            {
                _DestinaitonLineID = value;
            }
        }
        #endregion
        #region PMReassignmentID
        public abstract class pMReassignmentID : IBqlField
        {
        }
        protected string _PMReassignmentID;
        [PXDBString(10, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXDefault]
        [PXUIField(DisplayName = "Reassignment ID")]        
        [PXSelector(typeof(Search<UsrPMCostReassignment.pMReassignmentID>),
                    typeof(UsrPMCostReassignment.pMReassignmentID),
                    typeof(UsrPMCostReassignment.description))]
        [PXFieldDescription]
        public virtual string PMReassignmentID
        {
            get
            {
                return _PMReassignmentID;
            }
            set
            {
                _PMReassignmentID = value;
            }
        }
        #endregion
        #region RevID
        public abstract class revID : IBqlField
        {
        }
        protected int? _RevID;
        [PXDBInt(IsKey = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "RevID")]
        public virtual int? RevID
        {
            get
            {
                return _RevID;
            }
            set
            {
                _RevID = value;
            }
        }
        #endregion

        #region SourceProjectID
        public abstract class sourceProjectID : IBqlField
        {
        }
        protected int? _SourceProjectID;
        [PXDBInt]
        [PXDefault(typeof(UsrPMCostReassignmentSource.projectID))]
        [PXUIField(DisplayName = "Source Project ID", Required = true)]
        [PXSelector(typeof(Search<PMProject.contractID>),
                    typeof(PMProject.contractCD),
                    typeof(PMProject.description),
                    SubstituteKey = typeof(PMProject.contractCD))]
        public virtual int? SourceProjectID
        {
            get
            {
                return _SourceProjectID;
            }
            set
            {
                _SourceProjectID = value;
            }
        }
        #endregion
        #region SourceProjectDescription

        public abstract class sourceProjectDescription : IBqlField
        {
        }
        protected string _SourceProjectDescription;
        [PXDBLocalizableString(60, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Source Project Description", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]
        public  string SourceProjectDescription
        {
            get
            {
                return _SourceProjectDescription;
            }
            set
            {
                _SourceProjectDescription = value;
            }
        }
        #endregion
        #region SourceTaskID
        public abstract class sourceTaskID : IBqlField
        {
        }
        protected int? _SourceTaskID;
        

        [PXDBInt]
        [PXUIField(DisplayName = "Source Task ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<PMTask.taskID>),
                    typeof(PMTask.taskCD),
                    typeof(PMTask.description),
                    SubstituteKey = typeof(PMTask.taskCD))]
        public virtual int? SourceTaskID
        {
            get
            {
                return _SourceTaskID;
            }
            set
            {
                _SourceTaskID = value;
            }
        }
        #endregion
        #region SourceTaskDescription
        public abstract class sourceTaskDescription : IBqlField
        {
        }
        protected string _SourceTaskDescription;        
        [PXDBString(250, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Source Task Description", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]
        public virtual string SourceTaskDescription
        {
            get
            {
                return _SourceTaskDescription;
            }
            set
            {
                _SourceTaskDescription = value;
            }
        }
        #endregion
        #region SourceTaskStartDate

        public abstract class sourceTaskStartDate : IBqlField
        {
        }
        protected DateTime? _SourceTaskStartDate;
        /// <summary>
        ///  Gets or sets the actual date, when the task is started
        /// </summary>
        [PXDBDate()]
        [PXUIField(DisplayName = "Source Task Start Date")]
        public virtual DateTime? SourceTaskStartDate
        {
            get
            {
                return _SourceTaskStartDate;
            }
            set
            {
                _SourceTaskStartDate = value;
            }
        }
        #endregion
        #region SourceTaskEndDate

        public abstract class sourceTaskEndDate : IBqlField
        {
        }
        protected DateTime? _SourceTaskEndDate;
        /// <summary>
        /// Gets or sets the actual date, when the task is finished.
        /// </summary>
        [PXDBDate()]
        [PXVerifyEndDate(typeof(sourceTaskStartDate), AutoChangeWarning = true)]
        [PXUIField(DisplayName = "Source Task End Date")]
        public virtual DateTime? SourceTaskEndDate
        {
            get
            {
                return _SourceTaskEndDate;
            }
            set
            {
                _SourceTaskEndDate = value;
            }
        }
        #endregion

        #region DestinationProjectID
        public abstract class destinationProjectID : IBqlField
        {
        }
        protected int? _DestinationProjectID;
        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = "Destination Project ID", Required = true)]
        [PXSelector(typeof(Search<PMProject.contractID>),
                    typeof(PMProject.contractCD),
                    typeof(PMProject.description),
                    SubstituteKey = typeof(PMProject.contractCD))]
        public virtual int? DestinationProjectID
        {
            get
            {
                return _DestinationProjectID;
            }
            set
            {
                _DestinationProjectID = value;
            }
        }
        #endregion
        #region DestinationProjectDescription

        public abstract class destinationProjectDescription : IBqlField
        {
        }
        protected string _DestinationProjectDescription;
        [PXDBLocalizableString(60, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Destination Project Description", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]
        public string DestinationProjectDescription
        {
            get
            {
                return _DestinationProjectDescription;
            }
            set
            {
                _DestinationProjectDescription = value;
            }
        }
        #endregion
        #region DestinationTaskID
        public abstract class destinationTaskID : IBqlField
        {
        }
        protected int? _DestinationTaskID;


        [PXDBInt]
        [PXUIField(DisplayName = "Destination Task ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<PMTask.taskID>),
                    typeof(PMTask.taskCD),
                    typeof(PMTask.description),
                    SubstituteKey = typeof(PMTask.taskCD))]
        public virtual int? DestinationTaskID
        {
            get
            {
                return _DestinationTaskID;
            }
            set
            {
                _DestinationTaskID = value;
            }
        }
        #endregion
        #region DestinationTaskDescription
        public abstract class destinationTaskDescription : IBqlField
        {
        }
        protected string _DestinationTaskDescription;
        [PXDBString(250, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Destination Task Description", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]
        public virtual string DestinationTaskDescription
        {
            get
            {
                return _DestinationTaskDescription;
            }
            set
            {
                _DestinationTaskDescription = value;
            }
        }
        #endregion
        #region DestinationTaskStartDate

        public abstract class destinationTaskStartDate : IBqlField
        {
        }
        protected DateTime? _DestinationTaskStartDate;
        /// <summary>
        ///  Gets or sets the actual date, when the task is started
        /// </summary>
        [PXDBDate()]
        [PXUIField(DisplayName = "Destination Task Start Date")]
        public virtual DateTime? DestinationTaskStartDate
        {
            get
            {
                return _DestinationTaskStartDate;
            }
            set
            {
                _DestinationTaskStartDate = value;
            }
        }
        #endregion
        #region DestinationTaskEndDate

        public abstract class destinationTaskEndDate : IBqlField
        {
        }
        protected DateTime? _DestinationTaskEndDate;
        /// <summary>
        /// Gets or sets the actual date, when the task is finished.
        /// </summary>
        [PXDBDate()]
        [PXVerifyEndDate(typeof(destinationTaskStartDate), AutoChangeWarning = true)]
        [PXUIField(DisplayName = "Destination Task End Date")]
        public virtual DateTime? DestinationTaskEndDate
        {
            get
            {
                return _DestinationTaskEndDate;
            }
            set
            {
                _DestinationTaskEndDate = value;
            }
        }
        #endregion

        #region Processing service fields

        [PXBool]
        [PXDefault(false)]
        public virtual bool ProcessingError { get; set; }

        [PXBool]
        [PXDefault(false)]
        public virtual bool ProcessingWarning { get; set; }


        [PXBool]
        [PXDefault(false)]
        public virtual bool ProcessingSuccess { get; set; }

        [PXBool]
        [PXDefault(true)]
        public virtual bool NeedToProcessed { get; set; }

        [PXString]
        [PXDefault("Row is not processed")]
        public virtual string ProcessingMessage { get; set; }

        #endregion
    }
}
