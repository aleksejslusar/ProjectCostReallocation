using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CS;
using PX.Objects.GL;

namespace ProjectCostReallocation.DAC
{
    [Serializable]
	public class UsrPMCostReassignment : IBqlTable
	{        
        #region PMReassignmentID
        public abstract class pMReassignmentID : IBqlField
		{
		}
		protected string _PMReassignmentID;
		[PXDBString(10, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault]
		[PXUIField(DisplayName = "Reassignment ID")]
        [AutoNumber(typeof(Search<PMSetupExt.usrReassignmentNumberingID>), typeof(AccessInfo.businessDate))]
        [PXSelector(typeof(Search<pMReassignmentID>),
                    typeof(pMReassignmentID),
                    typeof(description))]
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
		#region BranchID
		public abstract class branchID : IBqlField
		{
		}
		protected int? _BranchID;
        [Branch]
        public virtual int? BranchID
		{
			get
			{
				return _BranchID;
			}
			set
			{
				_BranchID = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : IBqlField
		{
		}
		protected string _Description;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual string Description
		{
			get
			{
				return _Description;
			}
			set
			{
				_Description = value;
			}
		}
		#endregion
		#region Active
		public abstract class active : IBqlField
		{
		}
		protected bool? _Active;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? Active
		{
			get
			{
				return _Active;
			}
			set
			{
				_Active = value;
			}
		}
        #endregion
        #region RevID
        public abstract class revID : IBqlField
        {
        }
        protected int? _RevID;
        [PXDBInt]
        [PXDefault(1)]
        [PXUIField(DisplayName = "Revision ID", Enabled = false)]
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
        #region ReassignmentValue1Total
        public abstract class reassignmentValue1Total : IBqlField
        { }
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Square Footage", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual decimal? ReassignmentValue1Total { get; set; }
        #endregion
        #region ReassignmentValue2Total
        public abstract class reassignmentValue2Total : IBqlField
        { }
        [PXInt]
        [PXUIField(DisplayName = "Number of Units", Enabled = false)]
        [PXDefault(0)]
        public virtual int? ReassignmentValue2Total { get; set; }
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
        [PXDBCreatedByID()]
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
        [PXDBCreatedByScreenID()]
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
        [PXDBCreatedDateTime()]
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
        [PXDBLastModifiedByID()]
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
        [PXDBLastModifiedDateTime()]        
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
        [PXDBLastModifiedByScreenID()]
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
