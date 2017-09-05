using System;
using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    [Serializable]
	public class UsrPMCostReassignmentSource : IBqlTable, IUsrPMCostReassignmentProjectAndTask
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
		#region AccountGroupFrom
		public abstract class accountGroupFrom : IBqlField
		{
		}
		protected int? _AccountGroupFrom;
		[PXDBInt]
		[PXUIField(DisplayName = "Account Group From", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<PMAccountGroup.groupID>),
                    typeof(PMAccountGroup.groupCD),
                    typeof(PMAccountGroup.description),
                    SubstituteKey = typeof(PMAccountGroup.groupCD))]        
        public virtual int? AccountGroupFrom
		{
			get
			{
				return _AccountGroupFrom;
			}
			set
			{
				_AccountGroupFrom = value;
			}
		}
		#endregion
		#region AccountGroupTo
		public abstract class accountGroupTo : IBqlField
		{
		}
		protected int? _AccountGroupTo;
		[PXDBInt]
		[PXUIField(DisplayName = "Account Group To", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<PMAccountGroup.groupID>),
                    typeof(PMAccountGroup.groupCD),
                    typeof(PMAccountGroup.description),
                    SubstituteKey = typeof(PMAccountGroup.groupCD))]
        public virtual int? AccountGroupTo
		{
			get
			{
				return _AccountGroupTo;
			}
			set
			{
				_AccountGroupTo = value;
			}
		}
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
}
