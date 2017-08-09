﻿using System;
﻿using PX.Data;

namespace ProjectCostReallocation.DAC
{
    [Serializable]
	public class UsrPMCostReassignmentRunHistory : IBqlTable
	{
		#region PMReassignmentID
		public abstract class pMReassignmentID : IBqlField
		{
		}
		protected string _PMReassignmentID;
		[PXDBString(10, IsFixed = true, IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "PMReassignmentID")]
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
		[PXDefault]
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
		#region SourceTranID
		public abstract class sourceTranID : IBqlField
		{
		}
		protected long? _SourceTranID;
		[PXDBLong(IsKey = true)]
		[PXDefault]
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
		#region SourceTaskID
		public abstract class sourceTaskID : IBqlField
		{
		}
		protected int? _SourceTaskID;
		[PXDBInt(IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "SourceTaskID")]
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
		#region DestinationTranID
		public abstract class destinationTranID : IBqlField
		{
		}
		protected long? _DestinationTranID;
		[PXDBLong(IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "DestinationTranID")]
		public virtual long? DestinationTranID
		{
			get
			{
				return _DestinationTranID;
			}
			set
			{
				_DestinationTranID = value;
			}
		}
		#endregion
		#region DestinationTaskID
		public abstract class destinationTaskID : IBqlField
		{
		}
		protected int? _DestinationTaskID;
		[PXDBInt(IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "DestinationTaskID")]
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
	}
}
