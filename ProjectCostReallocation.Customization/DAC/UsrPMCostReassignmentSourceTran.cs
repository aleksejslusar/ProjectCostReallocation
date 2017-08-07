using System;
using PX.Data;

namespace ProjectCostReallocation.DAC
{
    [Serializable()]
	public class UsrPMCostReassignmentSourceTran : IBqlTable
	{
        #region PMReassignmentID
        public abstract class pMReassignmentID : IBqlField
		{
		}
		protected string _pMReassignmentID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
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
		#region SourceTranID
		public abstract class sourceTranID : IBqlField
		{
		}
		protected long? _SourceTranID;
		[PXDBLong(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "SourceTranID")]
		public virtual long? SourceTranID
		{
			get
			{
				return _SourceTranID;
			}
			set
			{
				_SourceTranID = value;
			}
		}
		#endregion
		#region TranID
		public abstract class tranID : IBqlField
		{
		}
		protected long? _TranID;
		[PXDBLong(IsKey = true)]
		[PXDefault()]
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
