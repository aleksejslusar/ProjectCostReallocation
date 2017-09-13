using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    [Serializable]
	public class UsrPMCostReassignmentDestination : IBqlTable, IUsrPMCostReassignmentProjectAndTask
    {
		#region PMReassignmentID
		public abstract class pMReassignmentID : IBqlField
		{
		}
		protected string _PMReassignmentID;
		[PXDBString(PMReassignmentIDValues.Length, IsUnicode = true, IsKey = true)]
        [PXDBDefault(typeof(UsrPMCostReassignment.pMReassignmentID))]
        [PXUIField(DisplayName = "PMReassignmentID", Visible = false)]
        [PXParent(typeof(Select<UsrPMCostReassignment, Where<UsrPMCostReassignment.pMReassignmentID, Equal<Current<pMReassignmentID>>>>))]       
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
		#region ProjectID
		public abstract class projectID : IBqlField
		{
		}
		protected int? _ProjectID;
		[PXDBInt(IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Project ID", Required = true)]
        [PXSelector(typeof(Search<PMProject.contractID, Where<PMProject.baseType, Equal<PMProject.ProjectBaseType>, And<PMProject.nonProject, Equal<False>, And<PMProject.isTemplate, NotEqual<True>>>>>),
                    typeof(PMProject.contractCD),
                    typeof(PMProject.description),
                    SubstituteKey = typeof(PMProject.contractCD))]
        public virtual int? ProjectID
		{
			get
			{
				return _ProjectID;
			}
			set
			{
				_ProjectID = value;
			}
		}
		#endregion
		#region TaskID
		public abstract class taskID : IBqlField
		{
		}
		protected int? _TaskID;
		[PXDBInt]
		[PXUIField(DisplayName = "Task ID", Required = true)]
        [PXDefault]
        [PXSelector(typeof(Search<PMTask.taskID, Where<PMTask.projectID, Equal<Current<projectID>>>>),
                    typeof(PMTask.taskCD),
                    typeof(PMTask.description),
                    SubstituteKey = typeof(PMTask.taskCD))]
        public virtual int? TaskID
		{
			get
			{
				return _TaskID;
			}
			set
			{
				_TaskID = value;
			}
		}
		#endregion
		#region ReassignmentSelection
		public abstract class reassignmentSelection : IBqlField
		{
		}        
		protected string _ReassignmentSelection;
		[PXDBString(1, IsFixed = true)]
        [PXDefault("C", PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Reassignment Selection", Required = true)]
        [ReassignmentSelection]
        public virtual string ReassignmentSelection
		{
			get
			{
				return _ReassignmentSelection;
			}
			set
			{
				_ReassignmentSelection = value;
			}
		}
        #endregion
        #region ReassignmentValue1
        public abstract class reassignmentValue1 : IBqlField { }
        private decimal? _reassignmentValue1;
        [PXDBDecimal]
        [PXUIField(DisplayName = "Square Footage", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.00")]
//        [PXFormula(typeof(Switch<Case<Where<PMTask.taskID, Equal<taskID>, And<PMTask.status, NotEqual<ProjectTaskStatus.completed>>>, reassignmentValue1>, decimal0>), typeof(SumCalc<UsrPMCostReassignment.reassignmentValue1Total>))]
        public virtual decimal? ReassignmentValue1 {
            get { return _reassignmentValue1; } 
            set { _reassignmentValue1 = value; }
        }
        #endregion
        #region ReassignmentValue2
        public abstract class reassignmentValue2 : IBqlField { }

        [PXDBInt]
        [PXUIField(DisplayName = "Number of Units", Enabled = false)]
        [PXDefault(0)]
//        [PXFormula(typeof(Switch<Case<Where<PMTask.taskID, Equal<Current<taskID>>, And<PMTask.status, NotEqual<ProjectTaskStatus.completed>>>, reassignmentValue2>, int0>),  typeof(SumCalc<UsrPMCostReassignment.reassignmentValue2Total>))]
        public virtual int? ReassignmentValue2 { get; set; }
        #endregion

        #region System fields
        #region NoteID
        public abstract class noteID : IBqlField
        {
        }
        protected Guid? _NoteID;
        [PXNote]
        public virtual Guid? NoteID
        {
            get
            {
                return _NoteID;
            }
            set
            {
                _NoteID = value;
            }
        }
        #endregion
        #region CreatedByID
        public abstract class createdByID : IBqlField
        {
        }
        protected Guid? _CreatedByID;
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID
        {
            get
            {
                return _CreatedByID;
            }
            set
            {
                _CreatedByID = value;
            }
        }
        #endregion
        #region tstamp
        public abstract class Tstamp : IBqlField
        {
        }
        protected byte[] _tstamp;
        [PXDBTimestamp]
        public virtual byte[] tstamp
        {
            get
            {
                return _tstamp;
            }
            set
            {
                _tstamp = value;
            }
        }
        #endregion
        #region CreatedByScreenID
        public abstract class createdByScreenID : IBqlField
        {
        }
        protected string _CreatedByScreenID;
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID
        {
            get
            {
                return _CreatedByScreenID;
            }
            set
            {
                _CreatedByScreenID = value;
            }
        }
        #endregion
        #region CreatedDateTime
        public abstract class createdDateTime : IBqlField
        {
        }
        protected DateTime? _CreatedDateTime;
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime
        {
            get
            {
                return _CreatedDateTime;
            }
            set
            {
                _CreatedDateTime = value;
            }
        }
        #endregion
        #region LastModifiedByID
        public abstract class lastModifiedByID : IBqlField
        {
        }
        protected Guid? _LastModifiedByID;
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID
        {
            get
            {
                return _LastModifiedByID;
            }
            set
            {
                _LastModifiedByID = value;
            }
        }
        #endregion
        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : IBqlField
        {
        }
        protected DateTime? _LastModifiedDateTime;
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime
        {
            get
            {
                return _LastModifiedDateTime;
            }
            set
            {
                _LastModifiedDateTime = value;
            }
        }
        #endregion
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : IBqlField
        {
        }
        protected string _LastModifiedByScreenID;
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID
        {
            get
            {
                return _LastModifiedByScreenID;
            }
            set
            {
                _LastModifiedByScreenID = value;
            }
        }
        #endregion
        #endregion
    }

    public class ReassignmentSelectionAttribute : PXStringListAttribute
    {
        public class Values
        {
            public const string UnitCount = "C";
            public const string SquareFootage = "F";
        }

        public ReassignmentSelectionAttribute(): base(
            new[] {Values.UnitCount, Values.SquareFootage}, 
            new[] {"Unit Count", "Square Footage"})
        {
        }
    }
}
