using System;
using PX.Data;

namespace ProjectCostReallocation
{
    [Serializable]
	public class UsrPMCostReassignmentPercentage : IBqlTable
	{
        #region PMReassignmentID
        public abstract class pMReassignmentID : IBqlField
        {
        }
        protected string _pMReassignmentID;
        [PXDBString(PMReassignmentIDValues.Length, IsUnicode = true, IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "ReassignmentID", Visible = false)]
        public virtual string PMReassignmentID
        {
            get
            {
                return _pMReassignmentID;
            }
            set
            {
                _pMReassignmentID = value;
            }
        }
        #endregion
        #region TranID
        public abstract class tranID : IBqlField
		{
		}
		protected long? _TranID;
		[PXDBLong(IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "TranID")]
		public virtual long? TranID
		{
			get
			{
				return _TranID;
			}
			set
			{
				_TranID = value;
			}
		}
		#endregion
		#region ReassignmentSelection
		public abstract class reassignmentSelection : IBqlField
		{
		}
		protected string _ReassignmentSelection;
		[PXDBString]
        [PXDefault("C", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Reassignment Selection", Required = true, Visible = false)]
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
		#region SourceValue
		public abstract class sourceValue : IBqlField
		{
		}
		protected decimal? _SourceValue;
		[PXDBDecimal(19)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "SourceValue")]
		public virtual decimal? SourceValue
		{
			get
			{
				return _SourceValue;
			}
			set
			{
				_SourceValue = value;
			}
		}
		#endregion
		#region DestinationValue
		public abstract class destinationValue : IBqlField
		{
		}
		protected decimal? _DestinationValue;
		[PXDBDecimal(19)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "DestinationValue")]
		public virtual decimal? DestinationValue
		{
			get
			{
				return _DestinationValue;
			}
			set
			{
				_DestinationValue = value;
			}
		}
		#endregion
		#region Percentage
		public abstract class percentage : IBqlField
		{
		}
		protected decimal? _Percentage;
		[PXDBDecimal(19)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Percentage")]
		public virtual decimal? Percentage
		{
			get
			{
				return _Percentage;
			}
			set
			{
				_Percentage = value;
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
