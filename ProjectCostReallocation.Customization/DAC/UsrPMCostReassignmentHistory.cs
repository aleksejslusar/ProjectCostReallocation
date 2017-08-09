using System;
using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation.DAC
{
    [Serializable]
	public class UsrPMCostReassignmentHistory : IBqlTable
	{
        #region TranID

        public abstract class tranID : IBqlField
        {
        }
        protected long? _TranID;
        [PXDBLong(IsKey = true)]
        [PXDefault]
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
        #region PMReassignmentID
        public abstract class pMReassignmentID : IBqlField
		{
		}
		protected string _PMReassignmentID;
		[PXDBString(10, IsFixed = true, IsKey = true)]
		[PXDefault(typeof(UsrPMCostReassignment.pMReassignmentID))]
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
	}
}
