using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation.DAC
{
    [System.SerializableAttribute()]
	public class UsrPMCostReassignmentHistory : IBqlTable
	{
        #region TranID

        public abstract class tranID : IBqlField
        {
        }
        protected long? _TranID;
        [PXDBLong(IsKey = true)]
        [PXDefault(typeof(PMTran.tranID))]
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
        [PXParent(typeof(Select<UsrPMCostReassignment, Where<UsrPMCostReassignment.pMReassignmentID, Equal<Current<UsrPMCostReassignmentSource.pMReassignmentID>>>>))]
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
