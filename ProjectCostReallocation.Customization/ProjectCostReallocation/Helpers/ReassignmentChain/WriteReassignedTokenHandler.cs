using System.Collections.Generic;
using PX.Data;
using PX.Objects.PM;

namespace ProjectCostReallocation
{
    public class WriteReassignedTokenHandler: TransactionEntityBase
    {

        public WriteReassignedTokenHandler(ProjectCostReassignmentEntry reassignmentEntry, RegisterEntry registerEntry, PMCostReassigmentEntity entity) : base(reassignmentEntry, registerEntry, entity)
        {
        }

        protected override void HandlerRequest()
        {
            UpdatePMTranUsrReassigned(ReassignedSourcePMTrans);
        }

        private void UpdatePMTranUsrReassigned(IEnumerable<PMTran> sourceTrans)
        {
            foreach (var sourceTran in sourceTrans)
            {
                var extension = PXCache<PMTran>.GetExtension<PMTranExt>(sourceTran);
                extension.UsrReassigned = true;
                RegisterEntry.Transactions.Update(sourceTran);
                RegisterEntry.Actions.PressSave();
            }
        }
    }
}
